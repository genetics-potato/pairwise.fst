Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class Genotype

    ''' <summary>
    ''' AA/AB/BB, ABC/ABB... etc
    ''' </summary>
    ''' <returns></returns>
    Public Property combination As String()
    ''' <summary>
    ''' The markers genotype
    ''' </summary>
    ''' <returns></returns>
    Public Property Markers As Dictionary(Of String, NamedValue(Of Double))

End Class
