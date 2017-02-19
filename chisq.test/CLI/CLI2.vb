Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Pairwise.Fst

Partial Module CLI

    <ExportAPI("/snp.Allele.chisq.test.pairwise",
            Usage:="/snp.Allele.chisq.test.pairwise /in <snp.genotypes.csv> [/keys <-/key1,key2,key3,....> /out <out.csv>]")>
    Public Function SNP_AlleleChisqTest_pairwise(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimSuffix & ".snp.Allele.chisq.test.csv")
        Dim genotypes As IEnumerable(Of SNPGenotype) = [in].LoadCsv(Of SNPGenotype)
        Dim keys$() = args.GetValue("/keys", "-").Split(","c)

        If keys.Length = 1 AndAlso keys(Scan0) = "-" Then
            keys = Nothing
        End If

        Dim out As File = genotypes.PairwiseAlleleFrequencyChisqTest(keys)
        Return out.Save(EXPORT, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/snp.Allele.chisq.test.pairwise.batch",
          Usage:="/snp.Allele.chisq.test.pairwise.batch /in <snp.genotypes.csv> [/keys <-/key1,key2,key3,....> /out <out.csv>]")>
    Public Function SNP_AlleleChisqTest_pairwise_BATCH(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            args.GetValue("/out", [in].TrimDIR & ".snp.Allele_chisq.test_pairwise/")
        Dim keys As String = args.GetValue("/keys", "-")
        Dim cmd As String = GetType(CLI).API(NameOf(SNP_AlleleChisqTest_pairwise))
        Dim CLI As String() = LinqAPI.Exec(Of String) <=
 _
            From snp As String
            In ls - l - r - "*.csv" <= [in]
            Let out As String = EXPORT & snp.BaseName & ".csv"
            Select $"{cmd} /in {snp.CLIPath} /out {out.CLIPath} /keys {keys.CLIToken}"

        Return App.SelfFolks(CLI, 32)
    End Function
End Module