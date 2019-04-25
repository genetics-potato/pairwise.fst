Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math

''' <summary>
''' Combined genetic risk
''' </summary>
''' <remarks>
''' https://zhuanlan.zhihu.com/p/49762423
''' </remarks>
Public Module CGR

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="RR">Relative Risk of the markers</param>
    ''' <param name="frequency"></param>
    ''' <param name="genotypes"></param>
    ''' <returns></returns>
    <Extension>
    Public Function WeightValue(RR As IEnumerable(Of DataSet),
                                frequency As IEnumerable(Of DataSet),
                                genotypes$()) As Dictionary(Of String, Double)
        Dim weight#
        Dim frequencyTable = frequency.ToDictionary
        Dim markerFreq As DataSet
        Dim markerWeights As New Dictionary(Of String, Double)

        For Each marker As DataSet In RR
            markerFreq = frequencyTable(marker.ID)
            weight = Aggregate genotype As String
                     In genotypes
                     Let relativeWeight = marker(genotype) * markerFreq(genotype)
                     Into Sum(relativeWeight)

            markerWeights(marker.ID) = weight
        Next

        Return markerWeights
    End Function

    ''' <summary>
    ''' Calculate combined genetic risk
    ''' </summary>
    ''' <param name="RR"></param>
    ''' <param name="frequency"></param>
    ''' <param name="genotype"></param>
    ''' <returns>Risk FoldChange result</returns>
    <Extension>
    Public Function CalculateCGR(RR As IEnumerable(Of DataSet),
                                 frequency As IEnumerable(Of DataSet),
                                 genotype As Genotype) As Double

        Dim weights = RR.WeightValue(frequency, genotype.combination)
        Dim totalWeight = weights.Values.ProductALL
        Dim actualRR As Double = RR _
            .Select(Function(marker)
                        Return marker(genotype.Markers(marker.ID))
                    End Function) _
            .ProductALL
        Dim CGR# = actualRR / totalWeight

        Return CGR
    End Function
End Module
