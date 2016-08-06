Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Pairwise.Fst
Imports RDotNET.Extensions.VisualBasic.stats

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
            args.GetValue("/out", [in].TrimDIR & ".snp.genotype_chisq.test/")
        Dim cmd As String = GetType(CLI).API(NameOf(SNP_genotypeChisqTest))

        For Each snp As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim out As String = EXPORT & snp.BaseName & ".csv"
            Call App.SelfFolk($"{cmd} /in {snp.CliPath} /out {out.CliPath}").Run()
        Next

        Return 0
    End Function

    <ExportAPI("/snp.genotype.chisq.test.pairwise",
               Usage:="/snp.genotype.chisq.test.pairwise /in <snp.genotypes.csv> [/out <out.csv.DIR>]")>
    Public Function SNP_genotypeChisqTest_pairwise(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimSuffix & ".snp.genotype.chisq.test/")
        Dim genotypes As IEnumerable(Of SNPGenotype) = [in].LoadCsv(Of SNPGenotype)

        For Each df As DocumentStream.File In TestMatrix.pairWise_chisqTest(genotypes)
            Dim out As String = EXPORT & "/" & df.FilePath & ".Csv"
            Call df.Save(out, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/snp.genotype.chisq.test.pairwise.batch",
           Usage:="/snp.genotype.chisq.test.pairwise.batch /in <in.DIR> [/out <out.Csv.DIR>]")>
    Public Function Batch_chisqTest_pairwise(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            args.GetValue("/out", [in].TrimDIR & ".snp.genotype_chisq.test_pairwise/")
        Dim cmd As String = GetType(CLI).API(NameOf(SNP_genotypeChisqTest_pairwise))

        For Each snp As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim out As String = EXPORT & snp.BaseName
            Call App.SelfFolk($"{cmd} /in {snp.CliPath} /out {out.CliPath}").Run()
        Next

        Return 0
    End Function

    <ExportAPI("/snp.genotype.chisq.test.pairwise.region",
       Usage:="/snp.genotype.chisq.test.pairwise.region /in <in.DIR> /keys <key1,key2,key3,...> [/out <out.Csv>]")>
    Public Function snp_chisqTest_pairwise_regions(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim keys As String() = args("/keys").Split(","c)
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-" & args("/keys").NormalizePathString & ".csv")
        Dim combs = Comb(Of String).CreateCompleteObjectPairs(keys).MatrixToList
        Dim output As New List(Of EntityObject)

        ' output += {"site/pops"}.Join(combs.Select(Function(x) $"{x.Key}__vs_{x.Value}"))

        For Each file As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim genotypes As IEnumerable(Of SNPGenotype) = file.LoadCsv(Of SNPGenotype)
            Dim hash = (From x As SNPGenotype
                        In genotypes
                        Let key As String = x.Population.Split(":"c).Last
                        Where Array.IndexOf(keys, key) > -1
                        Select x,
                            key).ToDictionary(Function(x) x.key,
                                              Function(x) x.x)
            Dim null As Boolean = False
            Dim site As New EntityObject With {
                .Identifier = file.BaseName
            }

            For Each pair As KeyValuePair(Of String, String) In combs
                If Not hash.ContainsKey(pair.Key) OrElse
                    Not hash.ContainsKey(pair.Value) Then
                    Exit For
                End If

                Dim result = {hash(pair.Key), hash(pair.Value)}.chisqTest.ToArray

                site.Properties.Add($"{pair.Key}__vs_{pair.Value}(AA)", result(0).x.pvalue & $" ({result(0).x.statistic})")
                site.Properties.Add($"{pair.Key}__vs_{pair.Value}(Aa)", result(1).x.pvalue & $" ({result(1).x.statistic})")
                site.Properties.Add($"{pair.Key}__vs_{pair.Value}(aa)", result(2).x.pvalue & $" ({result(2).x.statistic})")
            Next

            If Not null Then
                output += site
            End If
        Next

        Dim maps As New Dictionary(Of String, String) From {
            {NameOf(EntityObject.Identifier), "sites/pops"}
        }
        Return output.SaveTo(out, maps:=maps).CLICode
    End Function

    <ExportAPI("/snp.allele.chisq.test.pairwise.region",
   Usage:="/snp.allele.chisq.test.pairwise.region /in <in.DIR> /keys <key1,key2,key3,...> [/out <out.Csv>]")>
    Public Function snp_allele_chisqTest_pairwise_regions(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim keys As String() = args("/keys").Split(","c)
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-" & args("/keys").NormalizePathString & ".alleles.csv")
        Dim combs = Comb(Of String).CreateCompleteObjectPairs(keys).MatrixToList
        Dim output As New List(Of EntityObject)

        ' output += {"site/pops"}.Join(combs.Select(Function(x) $"{x.Key}__vs_{x.Value}"))

        For Each file As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim genotypes As IEnumerable(Of SNPGenotype) = file.LoadCsv(Of SNPGenotype)
            Dim hash = (From x As SNPGenotype
                        In genotypes
                        Let key As String = x.Population.Split(":"c).Last
                        Where Array.IndexOf(keys, key) > -1
                        Select x,
                            key).ToDictionary(Function(x) x.key,
                                              Function(x) x.x)
            Dim null As Boolean = False
            Dim site As New EntityObject With {
                .Identifier = file.BaseName
            }

            For Each pair As KeyValuePair(Of String, String) In combs  ' pops combination
                If Not hash.ContainsKey(pair.Key) OrElse
                    Not hash.ContainsKey(pair.Value) Then
                    Exit For
                End If

                Dim result As chisqTestResult = Allele_chisqTest(hash(pair.Key), hash(pair.Value))

                site.Properties.Add($"{pair.Key}__vs_{pair.Value}", $"{result.pvalue} ({result.statistic})")
            Next

            If Not null Then
                output += site
            End If
        Next

        Dim maps As New Dictionary(Of String, String) From {
            {NameOf(EntityObject.Identifier), "sites/pops"}
        }
        Return output.SaveTo(out, maps:=maps).CLICode
    End Function
End Module
