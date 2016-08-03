Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class FstPop

    Public Property Population As String
    <Column("AA")> Public Property a As Integer
    <Column("Aa")> Public Property b As Integer
    <Column("aa")> Public Property c As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
