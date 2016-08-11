Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' View All: (``InnateDB_genes.csv``) http://innatedb.com/annotatedGenes.do?type=innatedb 
''' 
''' Aside from annotating innate immunity interactions and pathways, the InnateDB curation team has also established 
''' a project to annotate genes that have a role in the innate immune response. This has been initiated in response 
''' to the fact that Gene Ontology annotation of the innate immune response is quite limited in the numbers of genes 
''' which have been identified and in response to the fact that many users have been eager to have a defined list of 
''' innate immune genes. For innate immune gene annotation, curators employ a new tool in the InnateDB curation system 
''' to associate relevant genes with publications which provide evidence for that gene having a role in innate immunity. 
''' Along with the link to the relevant publication(s), the curators provide a one-line summary of the role similar to 
''' Entrez GeneRIFs. Such genes are also automatically associated with the Gene Ontology term "innate immune response" 
''' in InnateDB, which provides a more comprehensive list of these genes for use in the InnateDB Gene Ontology 
''' over-representation analysis tool. To date, 1642 genes have been annotated to some extent (as this is an on-going process). 
''' It should be noted that it is not the intention of InnateDB to comprehensively annotate all the roles of a given gene, 
''' but rather to provide a brief indication if a gene has a role in innate immunity.
''' </summary>
Public Class InnateDB_genes

    Public Property id As String
    Public Property species As String
    Public Property taxonId As String
    Public Property ensembl As String
    Public Property entrez As String
    Public Property name As String
    Public Property fullname As String
    Public Property synonym As String
    Public Property signature As String
    Public Property chromStart As String
    Public Property chromEnd As String
    Public Property chromStrand As String
    Public Property chromName As String
    Public Property band As String
    Public Property goTerms As String
    Public Property [function] As String
    Public Property goFunctions As String
    Public Property goLocalizations As String
    Public Property cerebralLocalization As String
    Public Property nrIntxsValidated As String
    Public Property nrIntxsPredicted As String
    Public Property transcripts As String
    Public Property humanOrthologs As String
    Public Property mouseOrthologs As String
    Public Property bovineOrthologs As String
    Public Property lastModified As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
