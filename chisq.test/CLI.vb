Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Pairwise.Fst

Module CLI

    <ExportAPI("/snp.genotype.chisq.test",
               Usage:="/snp.genotype.chisq.test /in <snp.genotypes.csv> [/out <out.json>]")>
    Public Function SNP_genotypeChisqTest(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".snp.genotype.chisq.test.csv")
        Dim genotypes As IEnumerable(Of SNPGenotype) = [in].LoadCsv(Of SNPGenotype)
        Dim result = TestMatrix.chisqTest(genotypes).ToArray
        Call result.MatrixView(genotypes, [in].BaseName).Save(out, Encodings.ASCII)
        Return result.GetJson.SaveTo(out.TrimSuffix & ".json").CLICode
    End Function
End Module
