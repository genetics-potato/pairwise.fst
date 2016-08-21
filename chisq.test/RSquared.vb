Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash

Partial Module CLI

    <ExportAPI("/Locis.Copy", Usage:="/Locis.Copy /in <inDIR> /id <id.list> [/out <outDIR>]")>
    Public Function CopyFiles(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim ids As String = args("/id")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & "." & ids.BaseName)
        Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.csv") <= inDIR
        Dim hash As Dictionary(Of String, String) =
            files.ToDictionary(Function(x) x.Split.First.ToLower)
        Dim idlist As New List(Of String)(ids.ReadAllLines)

        For Each id As Value(Of String) In idlist.ValuesEnumerator
            If hash.ContainsKey(id = (+id).ToLower) Then
                Dim file As String = hash(+id)
                Dim name As String = FileIO.FileSystem.GetFileInfo(file).Name
                Dim copyTo As String = out & "/" & name

                Call SafeCopyTo(file, copyTo)
            End If
        Next

        Return 0
    End Function
End Module