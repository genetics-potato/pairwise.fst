#Region "Microsoft.VisualBasic::3bd870b205141c1078d9b51bcbfa27d2, ..\Pairwise.Fst\Genotype.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module Genotype

    ReadOnly __all As KeyValuePair(Of Char, Char)() =
        Comb(Of Char).CreateCompleteObjectPairs({"A"c, "T"c, "G"c, "C"c}) _
                     .MatrixAsIterator _
                     .ToArray

    ''' <summary>
    ''' Example: ``C: 0.844 (162)``
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FrequencyParser(raw As String) As Frequency
        Dim base As Char = raw.First
        raw = Mid(raw, 3).Trim
        Dim Count As String = Regex.Match(raw, "\(\d+\)").Value.GetStackValue("(", ")")
        Dim f As Double = Val(raw)

        Return New Frequency With {
            .base = base,
            .Count = CInt(Count),
            .Frequency = f
        }
    End Function

    Const Frequency As String = "[ATGC]: \d\.\d+ \(\d+\)"

    Public Function Frequencies(field As String) As Frequency()
        Dim fs As String() = Regex.Matches(field, Frequency, RegexICSng).ToArray
        Return fs.ToArray(AddressOf FrequencyParser)
    End Function

    Const Genotype As String = "[ATGC]\|[ATGC]: \d\.\d+ \(\d+\)"

    Public Function Genotypes(field As String) As Frequency()
        Dim fs As String() = Regex.Matches(field, Genotype, RegexICSng).ToArray
        Dim out As New List(Of Frequency)

        For Each m As String In fs
            Dim g As Char = m.First
            m = Mid(m, 3).Trim
            Dim f As Frequency = FrequencyParser(m)
            f.type = g
            out += f
        Next

        Return out
    End Function
End Module

Public Class SNPGenotype

    Public Property Population As String
    Public Property ssID As String

    <Column("Allele: frequency (count)")>
    Public Property AlleleFrequency As String
        Get
            Return String.Join("", Frequency.ToArray(Function(x) x.ToString))
        End Get
        Set(value As String)
            _Frequency = Genotype.Frequencies(value)
        End Set
    End Property

    <Column("Genotype: frequency (count)")>
    Public Property GenotypeFreqnency As String
        Get
            Return String.Join("", Genotypes.ToArray(Function(x) x.ToString))
        End Get
        Set(value As String)
            _Genotypes = Genotype.Genotypes(value)
        End Set
    End Property

    Default Public Overloads ReadOnly Property [GetGenotype](type As Char, base As Char) As Frequency
        Get
            For Each x In Genotypes
                If x.type = type AndAlso x.base = base Then
                    Return x
                End If
            Next

            Return New Frequency
        End Get
    End Property

    Public ReadOnly Property Frequency As Frequency()
    Public ReadOnly Property Genotypes As Frequency()

    Public Sub GetAllele(ByRef x As Char, ByRef y As Char)
        Dim als As Char() = Frequency.ToArray(Function(o) o.base)

        x = als(Scan0)

        If als.Length = 2 Then
            y = als(1)
        Else
            y = "x"c
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class Frequency

    ''' <summary>
    ''' 分子
    ''' </summary>
    ''' <returns></returns>
    Public Property type As Char
    ''' <summary>
    ''' 分母
    ''' </summary>
    ''' <returns></returns>
    Public Property base As Char
    Public Property Frequency As Double
    Public Property Count As Integer

    Public Overrides Function ToString() As String
        If type = Nothing OrElse type = NIL Then
            Return $"{base}: {Frequency} ({Count})"
        Else
            Return $"{type}|{base}: {Frequency} ({Count})"
        End If
    End Function
End Class
