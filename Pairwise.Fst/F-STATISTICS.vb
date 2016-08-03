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
End Class
