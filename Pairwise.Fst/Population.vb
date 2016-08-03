#Region "Microsoft.VisualBasic::cde920b710188b5f6bf463c99ccfcbf6, ..\Pairwise.Fst\Population.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Population : Inherits ClassObject

    Public Property Population As String
    Public Property Genotype As Dictionary(Of String, Integer)

    ''' <summary>
    ''' AA, Aa, aa
    ''' </summary>
    ''' <param name="alle"></param>
    ''' <returns></returns>
    Default Public ReadOnly Property Count(alle As String) As Integer
        Get
            Return Genotype(alle)
        End Get
    End Property

    ''' <summary>
    ''' N (number of individuals genotyped).
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property N As Integer
        Get
            Return Genotype.Values.Sum
        End Get
    End Property

    ''' <summary>
    ''' ``p1`` means the frequency of allele ``A`` in <see cref="population"/> 1.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property p As Double
        Get
            Dim AA As Integer = If(Genotype.ContainsKey("AA"), Genotype("AA"), 0)
            Dim A As Integer = If(Genotype.ContainsKey("Aa"), Genotype("Aa"), 0)
            Return (2 * AA + A) / (2 * N)
        End Get
    End Property

    Public ReadOnly Property q As Double
        Get
            Return 1 - p
        End Get
    End Property

    ''' <summary>
    ''' (= observed)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Expected As Dictionary(Of String, Double)
        Get
            Dim exp As New Dictionary(Of String, Double)
            Dim p As Double = Me.p
            Dim N As Double = Me.N

            exp("AA") = N * p ^ 2
            exp("Aa") = N * 2 * p * (1 - p)
            exp("aa") = N * p ^ 2

            Return exp
        End Get
    End Property

    Public ReadOnly Property Hobs As Double
        Get
            Return Genotype("Aa") / N
        End Get
    End Property

    ''' <summary>
    ''' Calculate the local expected heterozygosity, or gene diversity, of each subpopulation
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Hexp As Double
        Get
            Return 1 - (p ^ 2 + q ^ 2)
        End Get
    End Property

    ''' <summary>
    ''' Calculate the local inbreeding coefficient of each subpopulation (same as Eqn 35.4, except that we are subscripting for the subpopulations):
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Fs As Double
        Get
            Dim Hexp As Double = Me.Hexp
            Return (Hexp - Hobs) / Hexp
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pop"></param>
    ''' <param name="genotype">AA, Aa, aa</param>
    Sub New(pop As String, genotype As Integer())
        Me.Population = pop
        Me.Genotype = New Dictionary(Of String, Integer)

        Me.Genotype("AA") = genotype(0)
        Me.Genotype("Aa") = genotype(1)
        Me.Genotype("aa") = genotype(2)
    End Sub

    Sub New(pop As FstPop)
        Me.Population = pop.Population
        Me.Genotype = New Dictionary(Of String, Integer)

        Me.Genotype("AA") = pop.a
        Me.Genotype("Aa") = pop.b
        Me.Genotype("aa") = pop.c
    End Sub

    Sub New(snp As SNPGenotype)
        Dim genes As Char() = snp.Frequency.ToArray(Function(x) x.base)
        Dim a As Char = genes(0)
        Dim b As Char = genes.Get(1)

        If b = Nothing OrElse b = NIL Then
            b = "x"c
        End If

        Me.Population = snp.Population
        Me.Genotype = New Dictionary(Of String, Integer)

        If Not String.IsNullOrEmpty(snp.ssID) Then
            Population &= "-" & snp.ssID
        End If

        Me.Genotype("AA") = snp(a, a).Count
        Me.Genotype("Aa") = snp(a, b).Count
        Me.Genotype("aa") = snp(b, b).Count
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

