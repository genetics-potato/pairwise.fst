Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Pairwise.Fst
Imports RDotNet.Extensions
Imports RDotNet.Extensions.Bioinformatics.LDheatmap
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.grDevices
Imports RDotNet.Extensions.VisualBasic.API.grDevices

Partial Module CLI

    <ExportAPI("/Locis.Copy", Usage:="/Locis.Copy /in <inDIR> /id <id.list> [/out <outDIR>]")>
    Public Function CopyFiles(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim ids As String = args("/id")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & "." & ids.BaseName)
        Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.csv") <= inDIR
        Dim hash As Dictionary(Of String, String) =
            files.ToDictionary(Function(x) x.Split.First.ToLower)
        Dim idlist As New List(Of String)(ids.ReadAllLines)

        For Each id As Value(Of String) In idlist.ValuesEnumerator
            If hash.ContainsKey(id = (+id).ToLower) Then
                Dim file As String = hash(+id)
                Dim name As String = FileIO.FileSystem.GetFileInfo(file).Name
                Dim copyTo As String = out & "/" & name

                Call SafeCopyTo(file, copyTo)
            Else
                Call (+id).Warning
            End If
        Next

        Return 0
    End Function

    <ExportAPI("/LD.Matrix.Raw", Usage:="/LD.Matrix.Raw /in <in.DIR> [/out <out.csv>]")>
    Public Function BuildMatrix(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & ".gdat_raw.Csv")
        Dim ds As New Dictionary(Of EntityObject)
        Dim raw As NamedValue(Of SNPGenotype())() =
            LinqAPI.Exec(Of NamedValue(Of SNPGenotype())) <= From file As String
                                                             In ls - l - r - wildcards("*.csv") <= inDIR
                                                             Let id As String = file.BaseName.Split.First
                                                             Select New NamedValue(Of SNPGenotype()) With {
                                                                 .Name = id,
                                                                 .x = file.LoadCsv(Of SNPGenotype)
                                                             }
        Dim allTags As String() = LinqAPI.Exec(Of String) <= From x As SNPGenotype
                                                             In raw.Select(Function(o) o.x).MatrixAsIterator
                                                             Select x.Population.Split(":"c).Last
                                                             Distinct

        For Each tag As String In allTags
            ds += New EntityObject With {
                .Identifier = tag,
                .Properties = New Dictionary(Of String, String)
            }
        Next

        For Each line In raw
            For Each x As SNPGenotype In line.x
                Dim b As Char = "", c As Char = ""

                Call x.GetAllele(b, c)

                If b = "x"c OrElse c = "x"c Then
                    Continue For
                End If

                Dim tag As String = x.Population.Split(":"c).Last
                ds(tag).Properties.Add(line.Name, $"{b}/{c}")
            Next
        Next

        Return ds.Values.SaveTo(out).CLICode
    End Function

    <ExportAPI("/LDheatmap", Usage:="/LDheatmap /in <raw.csv> [/out <outDIR>]")>
    Public Function LDheatmap_CLI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".LDheatmap/")
        Dim df As DocumentStream.File = DocumentStream.File.Load([in])

        Call out.MkDIR
        Call images.tiff(filename:=(out & "/ldheatmap.tiff").UnixPath, width:=1000, height:=1000)
        Call LDheatmap(Bioinformatics.genetics.dataframe(df))
        Call dev.off()

        Return 0
    End Function
End Module