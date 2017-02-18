#Region "Microsoft.VisualBasic::aa72ba09c315bda40ac2fb412612a9c9, ..\Pairwise.Fst\F-STATISTICS.vb"

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

Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class F_STATISTICS

    Public Property FIS As Double
    Public Property FST As Double
    Public Property FIT As Double

    Sub New(pops As IEnumerable(Of Population))
        Dim Hs As Double = pops.HS
        Dim HI As Double = pops.HI
        Dim HT As Double = pops.HT

        FIS = (Hs - HI) / Hs
        FST = (HT - Hs) / HT
        FIT = (HT - HI) / HT
    End Sub

    Sub New()
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function PairwiseFst(pops As IEnumerable(Of Population)) As DataSet()
        Dim out As New List(Of DataSet)
        Dim array As Population() = pops.ToArray
        Dim fst As Double

        For Each line As Population In array
            Dim row As New DataSet With {
                .ID = line.Population.Split(":"c).Last.Trim
            }

            For Each x As Population In array
                fst = New F_STATISTICS({line, x}).FST
                row.Properties.Add(x.Population.Split(":"c).Last.Trim, fst)
            Next

            out += row
        Next

        Return out
    End Function
End Class

