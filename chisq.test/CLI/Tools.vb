Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

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

        For Each DIR As String In dirs
            ' 里面的sub folders都是snp位点
            Dim subFolders = (ls - l - "*.csv" <= DIR).ToArray
            Dim type$ = DIR.BaseName

            For Each snp$ In subFolders

                Dim pops As EntityObject() = load(path:=snp)
                Dim name$ = snp.BaseName.Split("_"c).Last & $" [{type}]"

                For Each pop As EntityObject In pops
                    Dim value$ = pop(tag)

                    If Not populations.ContainsKey(pop.ID) Then
                        Call populations.Add(
                            pop.ID,
                            New EntityObject With {
                                .ID = pop.ID,
                                .Properties = New Dictionary(Of String, String)
                            })
                    End If

                    populations(pop.ID).Properties(name) = value
                Next
            Next
        Next

        Return populations.Values _
            .ToArray _
            .SaveTo(out, Encodings.ASCII) _
            .CLICode
    End Function
End Module