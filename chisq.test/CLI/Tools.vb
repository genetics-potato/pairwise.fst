Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Pairwise.Fst

Partial Module CLI

    <ExportAPI("/Pvalue.Matrix", Usage:="/Pvalue.Matrix /in <DIR> [/tag <p-value> /out <pvalue.matrix.csv>]")>
    Public Function MergeMatrixPvalue(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim tag$ = args.GetValue("/tag", "p-value")
        Dim out$ = args.GetValue("/out", [in].TrimDIR & ".pvalue.matrix.csv")
        Dim dirs$() = (ls - l - lsDIR <= [in]).ToArray
        Dim populations As New Dictionary(Of EntityObject)
        Dim load = Function(path$)
                       Return EntityObject _
                           .LoadDataSet(path) _
                           .ToArray
                   End Function
        Dim indexOf As New Index(Of String)(Genotype.Continents)
        Dim getPopTag = Function(ID$)
                            Dim tokens$() = Strings _
                                .Split(ID, "_vs_") _
                                .Select(Function(s) s.Trim("_"c)) _
                                .ToArray

                            If tokens(0) = tokens(1) Then
                                Return Nothing
                            End If

                            If indexOf(tokens(Scan0)) = -1 AndAlso indexOf(tokens(1)) = -1 Then
                                Return tokens.OrderBy(Function(s) s).JoinBy("__vs_")
                            Else
                                Return Nothing
                            End If
                        End Function

        For Each DIR As String In dirs
            ' 里面的sub folders都是snp位点
            Dim subFolders = (ls - l - "*.csv" <= DIR).ToArray
            Dim type$ = DIR.BaseName

            For Each snp$ In subFolders

                Dim pops As EntityObject() = load(path:=snp)
                Dim name$ = snp.BaseName.Split("_"c).Last & $" [{type}]"

                For Each pop As EntityObject In pops
                    Dim value$ = pop(tag)
                    Dim popID$ = getPopTag(pop.ID)

                    If popID Is Nothing Then
                        Continue For
                    End If

                    If Not populations.ContainsKey(popID) Then
                        Call populations.Add(
                            popID,
                            New EntityObject With {
                                .ID = popID,
                                .Properties = New Dictionary(Of String, String)
                            })
                    End If

                    populations(popID).Properties(name) = value
                Next
            Next
        Next

        Return populations.Values _
            .OrderBy(Function(pop) pop.ID) _
            .ToArray _
            .SaveTo(out, Encodings.ASCII) _
            .CLICode
    End Function
End Module