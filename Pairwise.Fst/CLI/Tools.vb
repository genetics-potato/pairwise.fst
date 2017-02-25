Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.gplots
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.grDevices
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.utils.read.table

Partial Module CLI

    <ExportAPI("/pop.tree", Usage:="/pop.tree /in <in.csv> /out <out.csv>")>
    Public Function PopulationTree(args As CommandLine) As Integer
        Dim [in] = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".tree.csv")
        Dim csv As File = File.Load([in])
        Dim pops = From row As RowObject
                   In csv.Skip(1)
                   Let dist = Strings.Trim(row(Scan0)).Split(","c).Select(AddressOf Trim).ToArray
                   Let popID = row(1)
                   Select ct = dist(Scan0),
                       country = dist.Get(1),
                       populations = popID
                   Group By ct Into Group

        Dim output As New File

        For Each ct In pops
            Dim g = ct.Group.ToArray
            Dim row As New RowObject From {ct.ct, g.First.country, g.First.populations}

            output += row

            For Each pop In g.Skip(1)
                row = New RowObject({"", pop.country, pop.populations})
                output += row
            Next
        Next

        Return output.Save(out, Encodings.ASCII).CLICode
    End Function
End Module
