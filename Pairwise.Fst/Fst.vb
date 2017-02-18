#Region "Microsoft.VisualBasic::d7472d1ae5b2d9f3e5a18da317b87e91, ..\Pairwise.Fst\Fst.vb"

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
Imports Microsoft.VisualBasic.Linq

Public Module Fst

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pops"></param>
    ''' <returns></returns>
    ''' <remarks>``<see cref="pA"/> + <see cref="qa"/> = 1`` using this to check the data is valid or not?</remarks>
    <Extension>
    Public Function pA(pops As IEnumerable(Of Population)) As Double
        Dim array As Population() = pops.ToArray
        Dim N As Double() = array.ToArray(Function(x) x.N * 2.0R)
        Dim A As Double = array.SeqIterator.Sum(Function(x) x.value.p * N(x.i))
        Dim p As Double = A / N.Sum
        Return p
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pops"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ``<see cref="pA"/> + <see cref="qa"/> = 1``
    ''' </remarks>
    <Extension>
    Public Function qa(pops As IEnumerable(Of Population)) As Double
        Dim n As Integer = pops.Sum(Function(x) x("Aa") + x("aa") * 2)
        Dim size As Integer = pops.Sum(Function(x) x.N * 2)
        Return n / size
    End Function

    ''' <summary>
    ''' ``HI`` based on observed heterozygosities in individuals in subpopulations
    ''' </summary>
    ''' <param name="pops"></param>
    ''' <returns></returns>
    <Extension>
    Public Function HI(pops As IEnumerable(Of Population)) As Double
        Dim Hobs As Double = pops.Sum(Function(x) x.Hobs * x.N)
        Dim Ntotal As Integer = pops.Sum(Function(x) x.N)
        Return Hobs / Ntotal
    End Function

    ''' <summary>
    ''' ``HS`` based on expected heterozygosities in subpopulations
    ''' </summary>
    ''' <param name="pops"></param>
    ''' <returns></returns>
    <Extension>
    Public Function HS(pops As IEnumerable(Of Population)) As Double
        Dim Hexp As Double = pops.Sum(Function(x) x.Hexp * x.N)
        Dim Ntotal As Integer = pops.Sum(Function(x) x.N)
        Return Hexp / Ntotal
    End Function

    <Extension>
    Public Function HT(pops As IEnumerable(Of Population)) As Double
        Dim p As Double = pops.pA
        Dim q As Double = pops.qa
        Return 1 - (p ^ 2 + q ^ 2)
    End Function
End Module

