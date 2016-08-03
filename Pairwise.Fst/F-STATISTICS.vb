Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class F_STATISTICS

    Public Property FIS As Double
    Public Property FST As Double
    Public Property FIT As Double

    Sub New(pops As IEnumerable(Of Population))
        Dim Hs As Double = pops.HS
        Dim HI As Double = pops.HI
        Dim HT As Double = pops.HT

        FIS = (Hs - HI) / Hs
        FST = (HT - Hs) / HT
        FIT = (HT - HI) / HT
    End Sub

    Sub New()
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function PairwiseFst(pops As IEnumerable(Of Population)) As DataSet()
        Dim out As New List(Of DataSet)
        Dim array As Population() = pops.ToArray
        Dim fst As Double

        For Each line As Population In array
            Dim row As New DataSet With {
                .Identifier = line.Population.Split(":"c).Last.Trim
            }

            For Each x As Population In array
                fst = New F_STATISTICS({line, x}).FST
                row.Properties.Add(x.Population.Split(":"c).Last.Trim, fst)
            Next

            out += row
        Next

        Return out
    End Function
End Class
