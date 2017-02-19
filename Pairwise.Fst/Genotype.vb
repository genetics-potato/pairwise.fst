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
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Public Module Genotype

    ReadOnly __all As Tuple(Of Char, Char)() =
        Comb(Of Char).CreateCompleteObjectPairs({"A"c, "T"c, "G"c, "C"c}) _
                     .IteratesALL _
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="[Imports]"><see cref="SNPGenotype"/></param>
    ''' <param name="filters"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function RegionViews([Imports] As String, Optional filters As IEnumerable(Of String) = Nothing) As IEnumerable(Of SNPRegionView)
        For Each file As String In ls - l - r - "*.csv" <= [Imports]
            Dim data As SNPGenotype() = file.LoadCsv(Of SNPGenotype)
            Dim hash As Dictionary(Of String, SNPGenotype) =
                data.ToDictionary(
                    Function(x) If(Not String.IsNullOrEmpty(x.ssID),
                    x.Population.Split(":"c).Last & "-" & x.ssID,
                    x.Population.Split(":"c).Last))

            Dim out As New SNPRegionView With {
                .site = file.BaseName,
                .Allele = data.__getAllele,
                .regions = New Dictionary(Of String, String)
            }

            If filters Is Nothing Then
                filters = hash.Keys
            End If

            For Each key As String In filters
                If hash.ContainsKey(key) Then
                    out.regions.Add(key, hash(key).GenotypeFreqnency)
                End If
            Next

            Yield out
        Next
    End Function

    <Extension>
    Private Function __getAllele(bufs As SNPGenotype()) As String
        Dim a As Char = Nothing, b As Char = Nothing
        Dim la As New List(Of Char)
        Dim lb As New List(Of Char)

        For Each x In bufs
            Call x.GetAllele(a, b)
            If a <> ASCII.NUL AndAlso b <> ASCII.NUL Then
                Return $"{a}|{b}"
            Else
                la += a
                lb += b
            End If
        Next

        a = la.Where(Function(x) x <> Nothing AndAlso x <> ASCII.NUL).FirstOrDefault
        b = lb.Where(Function(x) x <> Nothing AndAlso x <> ASCII.NUL).FirstOrDefault

        If a = Nothing OrElse a = ASCII.NUL Then
            a = "X"c
        End If
        If b = Nothing OrElse b = ASCII.NUL Then
            b = "x"c
        End If

        Return $"{a}|{b}"
    End Function
End Module

Public Class SNPRegionView

    Public Property site As String
    Public Property Allele As String
    Public Property regions As Dictionary(Of String, String)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class SNPGenotype

    Public Property Population As String
    Public Property ssID As String

    Public ReadOnly Property RegionKey As String
        Get
            Return Population.Split(":"c).Last.Trim
        End Get
    End Property

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

    ''' <summary>
    ''' 这个属性是一个安全的属性，对于不存在的将不会返回空值。
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="base"></param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' Allele
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Frequency As Frequency()
    Public ReadOnly Property Genotypes As Frequency()

    Public Function GetAllele(c As Char) As Frequency
        For Each x In Frequency
            If x.base = c Then
                Return x
            End If
        Next

        Return New Frequency
    End Function

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
    ''' allele, 分母
    ''' </summary>
    ''' <returns></returns>
    Public Property base As Char
    Public Property Frequency As Double
    Public Property Count As Integer

    Public Overrides Function ToString() As String
        If type = Nothing OrElse type = ASCII.NUL Then
            Return $"{base}: {Frequency} ({Count})"
        Else
            Return $"{type}|{base}: {Frequency} ({Count})"
        End If
    End Function
End Class
