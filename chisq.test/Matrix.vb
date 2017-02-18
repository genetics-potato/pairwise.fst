Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Pairwise.Fst
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts
Imports RDotNET.Extensions.VisualBasic.API.utils
Imports RDotNET.Extensions.VisualBasic.API.stats
Imports RDotNET.Extensions.VisualBasic.API
Imports Microsoft.VisualBasic.Language

''' <summary>
''' ``chisq.test`` matrix services
''' </summary>
Public Module TestMatrix

    <Extension>
    Public Iterator Function pairWise_chisqTest(data As IEnumerable(Of SNPGenotype)) As IEnumerable(Of File)
        For Each x As SNPGenotype In data
            Dim out As New File With {
                .FilePath = x.Population.NormalizePathString(True)
            }

            For Each y As SNPGenotype In data
                Dim array As SNPGenotype() = {x, y}
                Dim result = TestMatrix.chisqTest(array).ToArray
                Dim name As String = x.Population.Split(":"c).Last & "__vs_" & y.Population.Split(":"c).Last
                Dim df As File = result.MatrixView(array, name)

                out.Append(df)
                out.AppendLine()
            Next

            Yield out
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
        Dim n As Integer() = array.ToArray(Function(x) x.Genotypes.Sum(Function(o) o.Count))

        Call array(Scan0).GetAllele(a, b)

        Yield array.__chisqTest(a, a, n)
        Yield array.__chisqTest(a, b, n)
        Yield array.__chisqTest(b, b, n)
    End Function

    Public Function Allele_chisqTest(a As SNPGenotype, b As SNPGenotype) As chisqTestResult
        Dim cv As New List(Of Integer)
        Dim n As New Value(Of Integer)

        cv += {n = a.Frequency(Scan0).Count, a.Frequency.Select(Function(x) x.Count).Sum - CInt(n)}
        cv += {n = b.Frequency(Scan0).Count, b.Frequency.Select(Function(x) x.Count).Sum - CInt(n)}

        Dim out As chisqTestResult = stats.chisqTest(matrix(c(Of Integer)(cv), nrow:=2))
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

        Dim out As chisqTestResult = stats.chisqTest(matrix(c(Of Integer)(cv.ToArray), nrow:=2))

        Return New NamedValue(Of chisqTestResult) With {
            .Name = $"{a}/{b}",
            .Value = out
        }
    End Function

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
