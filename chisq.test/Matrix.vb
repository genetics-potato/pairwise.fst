Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Linq
Imports Pairwise.Fst
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic.API.utils
Imports RDotNET.Extensions.VisualBasic.stats

''' <summary>
''' ``chisq.test`` matrix services
''' </summary>
Public Module TestMatrix

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

    <Extension>
    Private Function __chisqTest(array As SNPGenotype(), a As Char, b As Char, n As Integer()) As NamedValue(Of chisqTestResult)
        Dim cv As New List(Of Integer)

        For Each x As SeqValue(Of SNPGenotype) In array.SeqIterator
            cv += x.obj(a, b).Count
            cv += n(x) - x.obj(a, b).Count
        Next

        Dim out As chisqTestResult = stats.chisqTest(matrix(c(cv.ToArray), nrow:=2))

        Return New NamedValue(Of chisqTestResult) With {
            .Name = $"{a}/{b}",
            .x = out
        }
    End Function

    <Extension>
    Public Function MatrixView(chisqTest As IEnumerable(Of NamedValue(Of chisqTestResult)),
                               data As IEnumerable(Of SNPGenotype),
                               name As String) As DocumentStream.File
        Dim out As New DocumentStream.File

        out += {"Polymorphism", "Genotype", "Chi square", "P-value"} _
            .JoinAsIterator(data.Select(Function(x) x.Population.Split(":"c).Last.Trim))
        out += {name, "", "", ""} _
            .JoinAsIterator(data.Select(
            Function(x) x.Genotypes.Sum(
            Function(o) o.Count).ToString))

        For Each x In chisqTest
            Dim row As New RowObject From {""}
            Dim a As Char = x.Name.First, b As Char = x.Name.Last
            row += New String() {x.Name, x.x.statistic, x.x.pvalue}

            For Each pop In data
                row += $"{pop(a, b).Count}({pop(a, b).Frequency})"
            Next

            out += row
        Next

        Return out
    End Function
End Module
