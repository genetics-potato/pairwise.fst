Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Pairwise.Fst
Imports RApi = RDotNET.Extensions.VisualBasic.API

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

        For Each snp As String In ls - l - r - "*.csv" <= [in]
            Dim out As String = EXPORT & snp.BaseName & ".csv"
            Call App.SelfFolk($"{cmd} /in {snp.CLIPath} /out {out.CLIPath}").Run()
        Next

        Return 0
    End Function

    <ExportAPI("/snp.genotype.chisq.test.pairwise",
               Usage:="/snp.genotype.chisq.test.pairwise /in <snp.genotypes.csv> [/keys <-/key1,key2,key3,....> /out <out.csv.DIR>]")>
    Public Function SNP_genotypeChisqTest_pairwise(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimSuffix & ".snp.genotype.chisq.test/")
        Dim genotypes As IEnumerable(Of SNPGenotype) = [in].LoadCsv(Of SNPGenotype)
        Dim keys$() = args.GetValue("/keys", "-").Split(","c)

        If keys.Length = 1 AndAlso keys(Scan0) = "-" Then
            keys = Nothing
        End If

        For Each df As (filePath$, File) In TestMatrix.pairWise_chisqTest(genotypes, keys)
            Dim out As String = EXPORT & "/" & df.filePath & ".Csv"
            Dim dataframe As File = df.Item2

            Call dataframe.Save(out, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/snp.genotype.chisq.test.pairwise.batch",
           Usage:="/snp.genotype.chisq.test.pairwise.batch /in <in.DIR> [/keys <-/key1,key2,key3,....> /out <out.Csv.DIR>]")>
    Public Function Batch_chisqTest_pairwise(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            args.GetValue("/out", [in].TrimDIR & ".snp.genotype_chisq.test_pairwise/")
        Dim keys As String = args.GetValue("/keys", "-")
        Dim cmd As String = GetType(CLI).API(NameOf(SNP_genotypeChisqTest_pairwise))
        Dim CLI As String() = LinqAPI.Exec(Of String) <=
 _
            From snp As String
            In ls - l - r - "*.csv" <= [in]
            Let out As String = EXPORT & snp.BaseName
            Select $"{cmd} /in {snp.CLIPath} /out {out.CLIPath} /keys {keys.CLIToken}"

        Return App.SelfFolks(CLI, 32)
    End Function

    <ExportAPI("/snp.genotype.chisq.test.pairwise.region",
       Usage:="/snp.genotype.chisq.test.pairwise.region /in <in.DIR> /keys <key1,key2,key3,...> [/out <out.Csv>]")>
    Public Function snp_chisqTest_pairwise_regions(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim keys As String() = args("/keys").Split(","c)
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-" & args("/keys").NormalizePathString & ".csv")
        Dim combs = Comb(Of String).CreateCompleteObjectPairs(keys).Unlist
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
                .ID = file.BaseName
            }

            For Each pair As Tuple(Of String, String) In combs
                If Not hash.ContainsKey(pair.Item1) OrElse
                    Not hash.ContainsKey(pair.Item2) Then
                    Exit For
                End If

                Dim result = {hash(pair.Item1), hash(pair.Item2)}.chisqTest.ToArray

                site.Properties.Add($"{pair.Item1}__vs_{pair.Item2}(AA)", result(0).Value.pvalue & $" ({result(0).Value.statistic})")
                site.Properties.Add($"{pair.Item1}__vs_{pair.Item2}(Aa)", result(1).Value.pvalue & $" ({result(1).Value.statistic})")
                site.Properties.Add($"{pair.Item1}__vs_{pair.Item2}(aa)", result(2).Value.pvalue & $" ({result(2).Value.statistic})")
            Next

            If Not null Then
                output += site
            End If
        Next

        Dim maps As New Dictionary(Of String, String) From {
            {NameOf(EntityObject.ID), "sites/pops"}
        }
        Return output.SaveTo(out, maps:=maps).CLICode
    End Function

    <ExportAPI("/snp.allele.chisq.test.pairwise.region",
   Usage:="/snp.allele.chisq.test.pairwise.region /in <in.DIR> /keys <key1,key2,key3,...> [/out <out.Csv>]")>
    Public Function snp_allele_chisqTest_pairwise_regions(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim keys As String() = args("/keys").Split(","c)
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-" & args("/keys").NormalizePathString & ".alleles.csv")
        Dim combs = Comb(Of String).CreateCompleteObjectPairs(keys).Unlist
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
                .ID = file.BaseName
            }

            For Each pair As Tuple(Of String, String) In combs  ' pops combination
                If Not hash.ContainsKey(pair.Item1) OrElse
                    Not hash.ContainsKey(pair.Item2) Then
                    Exit For
                End If

                Dim result As RApi.chisqTestResult = Allele_chisqTest(hash(pair.Item1), hash(pair.Item2))

                site.Properties.Add($"{pair.Item1}__vs_{pair.Item2}", $"{result.pvalue} ({result.statistic})")
            Next

            If Not null Then
                output += site
            End If
        Next

        Dim maps As New Dictionary(Of String, String) From {
            {NameOf(EntityObject.ID), "sites/pops"}
        }
        Return output.SaveTo(out, maps:=maps).CLICode
    End Function
End Module
