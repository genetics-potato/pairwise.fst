Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Pairwise.Fst
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts

''' <summary>
''' ``chisq.test`` matrix services
''' </summary>
Public Module TestMatrix

    <Extension>
    Public Iterator Function pairWise_chisqTest(data As IEnumerable(Of SNPGenotype), Optional keys$() = Nothing) As IEnumerable(Of (String, File))
        Dim source As SNPGenotype() = data.ToArray

        ' 当区域代码不为空的时候，就会只计算所指定的目标区域的数据
        If Not keys.IsNullOrEmpty Then
            source = source _
                .Where(Function(x) Array.IndexOf(keys, x.RegionKey) > -1) _
                .ToArray
        End If

        For Each x As SNPGenotype In source
            Dim out As New File

            For Each y As SNPGenotype In source
                Dim array As SNPGenotype() = {x, y}
                Dim result = TestMatrix.chisqTest(array).ToArray
                Dim name As String = x.Population.Split(":"c).Last & "__vs_" & y.Population.Split(":"c).Last
                Dim df As File = result.MatrixView(array, name)

                out.Append(df)
                out.AppendLine()
            Next

            Yield (x.Population.NormalizePathString(True), out)
        Next
    End Function

    ''' <summary>
    ''' 函数所返回来的是 AA, Aa, aa的chisq.test的数据结果
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function chisqTest(data As IEnumerable(Of SNPGenotype)) As IEnumerable(Of NamedValue(Of chisqTestResult))
        Dim a As Char = Nothing, b As Char = Nothing
        Dim array As SNPGenotype() = data.ToArray
        Dim n As Integer() = array.Select(Function(x) x.Genotypes.Sum(Function(o) o.Count)).ToArray

        Call array(Scan0).GetAllele(a, b)

        Yield array.__chisqTest(a, a, n)
        Yield array.__chisqTest(a, b, n)
        Yield array.__chisqTest(b, a, n)
        Yield array.__chisqTest(b, b, n)
    End Function

    Public Function Allele_chisqTest(a As SNPGenotype, b As SNPGenotype) As chisqTestResult
        Dim cv As New List(Of Integer)
        Dim n As New Value(Of Integer)

        cv += {n = a.Frequency(Scan0).Count, a.Frequency.Select(Function(x) x.Count).Sum - CInt(n)}
        cv += {n = b.Frequency(Scan0).Count, b.Frequency.Select(Function(x) x.Count).Sum - CInt(n)}

        Dim out As chisqTestResult = stats.chisqTest(base.matrix(c(Of Integer)(cv), nrow:=2))
        Return out
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="popa"></param>
    ''' <param name="popb"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ```vbnet
    ''' |-|population a|population b|
    ''' |-|------------|------------|
    ''' |A|      a     |      b     |  
    ''' |B|      c     |      d     |
    ''' 
    ''' a &lt;- matrix(c(a, c, b, d), ncol=2)
    ''' chisq.test(a)
    ''' 
    '''    Pearson's Chi-squared test with Yates' continuity correction
    '''
    ''' data:  a
    ''' X-squared = 1.1447, df = 1, p-value = 0.2847
    ''' ```
    ''' </remarks>
    <Extension>
    Public Function AlleleFrequencyChisqTest(popa As SNPGenotype, popb As SNPGenotype) As (A As Char, B As Char, ca%, cb%, cc%, cd%, chisqTestResult)
        Dim x As Char = Nothing, y As Char = Nothing
        Dim da = popa.Frequency.ToDictionary(Function(al) al.base)
        Dim db = popb.Frequency.ToDictionary(Function(al) al.base)

        Call popa.GetAllele(x, y)

        Dim a% = da.__getCount(x),
            b% = db.__getCount(x),
            c% = da.__getCount(y),
            d% = db.__getCount(y)
        Dim array$ = base.matrix({a, c, b, d}, nrow:=2)
        Dim out As chisqTestResult = stats.chisqTest(x:=array)

        Return (x, y, a, b, c, d, out)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="popa"></param>
    ''' <param name="popb"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ```vbnet
    ''' |-|population a|population b|
    ''' |-|------------|------------|
    ''' |A|      a     |      b     |  
    ''' |B|      c     |      d     |
    ''' |C|      e     |      f     |
    ''' |D|      g     |      h     |
    ''' 
    ''' a &lt;- matrix(c(a, c, e, g, b, d, f, h), ncol=2)
    ''' chisq.test(a)
    ''' 
    '''    Pearson's Chi-squared test with Yates' continuity correction
    '''
    ''' data:  a
    ''' X-squared = 1.1447, df = 1, p-value = 0.2847
    ''' ```
    ''' 
    ''' 2017-2-20 当任意一个位置的值都为0的时候，chisq.test的值将无法进行计算，由于基因型只有GA或者AG这两种杂合体，所以这里只取某一个不为零的即可完成计算了
    ''' </remarks>
    <Extension>
    Public Function GenotypeFrequencyChisqTest(popa As SNPGenotype, popb As SNPGenotype) As (counts As (a%, b%, c%, d%, e%, f%, g%, h%), chisqTestResult)
        Dim x As Char = Nothing, y As Char = Nothing

        Call popa.GetAllele(x, y)

        Dim a% = popa(x, x).Count,
            b% = popb(x, x).Count,
            c% = popa(x, y).Count,
            d% = popb(x, y).Count,
            e% = popa(y, x).Count,
            f% = popb(y, x).Count,
            g% = popa(y, y).Count,
            h% = popb(y, y).Count
        Dim vec%()

        If c = d AndAlso c = 0 Then
            vec = {a, e, g, b, f, h}
        Else  ' 只有 A/G, G/A这两种情况，没有第三种了
            vec = {a, c, g, b, d, h}
        End If

        ' 使用R之中的chisq.test函数进行检验
        Dim array$ = base.matrix(data:=vec, nrow:=3)
        Dim out As chisqTestResult = stats.chisqTest(x:=array)
        Dim counts = (a, b, c, d, e, f, g, h)

        Return (counts, out)
    End Function

    <Extension>
    Public Function PairwiseGenotypeFrequencyChisqTest(populations As IEnumerable(Of SNPGenotype), Optional keys$() = Nothing) As File
        Dim out As New File
        Dim source As SNPGenotype() = If(
            keys.IsNullOrEmpty,
            populations.ToArray,
            populations _
                .Where(Function(p) Array.IndexOf(keys, p.RegionKey) > -1) _
                .ToArray)

        out += {"Polymorphism", "population A", "population B", "Chi square", "p-value"}

        For Each i As SNPGenotype In source
            For Each j As SNPGenotype In source
                Dim result As (counts As (a%, b%, c%, d%, e%, f%, g%, h%), chisqTestResult) = GenotypeFrequencyChisqTest(i, j)

                With result
                    out += {
                        $"{i.RegionKey}__vs_{j.RegionKey}",
                        i.GenotypeFreqnency,
                        j.GenotypeFreqnency,
                        CStr(.Item2.statistic),
                        CStr(.Item2.pvalue)
                    }
                End With
            Next
        Next

        Return out
    End Function

    <Extension>
    Private Function __getCount(frequency As Dictionary(Of Char, Frequency), type As Char) As Integer
        If frequency.ContainsKey(type) Then
            Return frequency(type).Count
        Else
            Return 0
        End If
    End Function

    <Extension>
    Public Function PairwiseAlleleFrequencyChisqTest(populations As IEnumerable(Of SNPGenotype), Optional keys$() = Nothing) As File
        Dim out As New File
        Dim source As SNPGenotype() = If(
            keys.IsNullOrEmpty,
            populations.ToArray,
            populations _
                .Where(Function(p) Array.IndexOf(keys, p.RegionKey) > -1) _
                .ToArray)

        out += {"Polymorphism", "population A", "population B", "Chi square", "p-value"}

        For Each i As SNPGenotype In source
            For Each j As SNPGenotype In source
                Dim chisqTest As (A As Char, B As Char, ca%, cb%, cc%, cd%, chisqTestResult) =
                    AlleleFrequencyChisqTest(i, j)

                With chisqTest
                    out += {
                        $"{i.RegionKey}__vs_{j.RegionKey}",
                        $"{ .A}/{ .B} = { .ca}/{ .cc}",
                        $"{ .A}/{ .B} = { .cb}/{ .cd}",
                        CStr(.Item7.statistic),
                        CStr(.Item7.pvalue)
                    }
                End With
            Next
        Next

        Return out
    End Function

    <Extension>
    Private Function __chisqTest(array As SNPGenotype(), a As Char, b As Char, n As Integer()) As NamedValue(Of chisqTestResult)
        Dim cv As New List(Of Integer)

        For Each x As SeqValue(Of SNPGenotype) In array.SeqIterator
            cv += x.value(a, b).Count
            cv += n(x) - x.value(a, b).Count
        Next

        ' R script hybrids programming

        ' mat <- c(cv);
        ' mat <- matrix(mat, nrow=2);
        '
        ' # 判断这些数据是否存在差异
        ' chisq.test{stats}
        '
        ' out <- chisq.test(mat);

        Dim out As chisqTestResult = stats.chisqTest(base.matrix(c(cv.ToArray), nrow:=2))

        Return New NamedValue(Of chisqTestResult) With {
            .Name = $"{a}/{b}",
            .Value = out
        }
    End Function

    ''' <summary>
    ''' 将``chisq.test``的计算结果转换为``csv``文件之中的行数据
    ''' </summary>
    ''' <param name="chisqTest"></param>
    ''' <param name="data"></param>
    ''' <param name="name"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MatrixView(chisqTest As IEnumerable(Of NamedValue(Of chisqTestResult)),
                               data As IEnumerable(Of SNPGenotype),
                               name As String) As File
        Dim out As New File

        out += {"Polymorphism", "Genotype", "Chi square", "P-value"} _
            .JoinIterates(data.Select(Function(x) x.Population.Split(":"c).Last.Trim))
        out += {name, "", "", ""} _
            .JoinIterates(data.Select(
            Function(x) x.Genotypes.Sum(
            Function(o) o.Count).ToString))

        For Each x In chisqTest
            Dim row As New RowObject From {""}
            Dim a As Char = x.Name.First, b As Char = x.Name.Last
            row += New String() {x.Name, x.Value.statistic, x.Value.pvalue}

            For Each pop In data
                row += $"{pop(a, b).Count}({pop(a, b).Frequency})"
            Next

            out += row
        Next

        Return out
    End Function
End Module
