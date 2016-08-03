Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Module CLI

    Sub New()

    End Sub

    <ExportAPI("/fst",
               Usage:="/fst /in <genotype.Csv> [/out <out.json>]")>
    Public Function fst(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".fst.json")
        Dim data As IEnumerable(Of FstPop) = [in].LoadCsv(Of FstPop)
        Dim array As Population() = data.ToArray(Function(x) New Population(x))
        Dim result As New F_STATISTICS(array)
        Return result.GetJson.SaveTo(out).CLICode
    End Function
End Module
