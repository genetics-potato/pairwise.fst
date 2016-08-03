Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
End Module
