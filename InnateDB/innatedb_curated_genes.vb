Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' ``innatedb_curated_genes.xls``
''' </summary>
Public Class innatedb_curated_genes

    <Column("InnateDB Gene ID")>
    Public Property InnateDBGeneID As String

    <Column("Gene Symbol")>
    Public Property GeneSymbol As String
    Public Property Species As String

    <Column("PubMED ID")>
    Public Property PubMEDID As String
    Public Property Annotation As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
