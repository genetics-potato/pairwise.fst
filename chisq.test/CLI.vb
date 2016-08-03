Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Pairwise.Fst

Module CLI

    <ExportAPI("/snp.genotype.chisq.test",
               Usage:="/snp.genotype.chisq.test /in <snp.genotypes.csv> [/out <out.csv>]")>
    Public Function SNP_genotypeChisqTest(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".snp.genotype.chisq.test.csv")
        Dim genotypes As IEnumerable(Of SNPGenotype) = [in].LoadCsv(Of SNPGenotype)
        Dim result = TestMatrix.chisqTest(genotypes).ToArray
        Call result.MatrixView(genotypes, [in].BaseName).Save(out, Encodings.ASCII)
        Return result.GetJson.SaveTo(out.TrimSuffix & ".json").CLICode
    End Function

    <ExportAPI("/snp.genotype.chisq.test.batch",
               Usage:="/snp.genotype.chisq.test.batch /in <in.DIR> [/out <out.Csv>]")>
    Public Function Batch_chisqTest(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            args.GetValue("/out", [in].TrimSuffix & ".snp.genotype_chisq.test/")
        Dim cmd As String = GetType(CLI).API(NameOf(SNP_genotypeChisqTest))

        For Each snp As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim out As String = EXPORT & snp.BaseName & ".csv"
            Call App.SelfFolk($"{cmd} /in {snp.CliPath} /out {out.CliPath}").Run()
        Next

        Return 0
    End Function
End Module
