Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Module CLI

    Sub New()
        Dim path As String = App.HOME & "/fst.population_example.csv"

        If Not path.FileExists Then
            Dim example As FstPop() = {
                New FstPop With {
                    .Population = "pop1",
                    .a = 125,
                    .b = 250,
                    .c = 125
                },
                New FstPop With {
                    .Population = "pop2",
                    .a = 50,
                    .b = 30,
                    .c = 20
                },
                New FstPop With {
                    .Population = "pop3",
                    .a = 100,
                    .b = 500,
                    .c = 400
                }
            }
            Call example.SaveTo(path)
        End If
    End Sub

    <ExportAPI("/fst",
               Usage:="/fst /in <genotype.Csv> [/out <out.json>]")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(FstPop)})>
    <ParameterInfo("/out", True, AcceptTypes:={GetType(F_STATISTICS)})>
    Public Function fst(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".fst.json")
        Dim data As IEnumerable(Of FstPop) = [in].LoadCsv(Of FstPop)
        Dim array As Population() = data.ToArray(Function(x) New Population(x))
        Dim result As New F_STATISTICS(array)
        Return result.GetJson.SaveTo(out).CLICode
    End Function

    <ExportAPI("/SNP.fst",
               Usage:="/SNP.fst /in <snp.genotype.Csv> [/out <out.json>]")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(SNPGenotype)})>
    <ParameterInfo("/out", True, AcceptTypes:={GetType(F_STATISTICS)})>
    Public Function SNPFst(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".snp_fst.json")
        Dim data As IEnumerable(Of SNPGenotype) = [in].LoadCsv(Of SNPGenotype)
        Dim array As Population() = data.ToArray(Function(x) New Population(x))
        Dim result As New F_STATISTICS(array)
        Return result.GetJson.SaveTo(out).CLICode
    End Function
End Module
