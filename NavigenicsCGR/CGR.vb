Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math

''' <summary>
''' Combined genetic risk
''' </summary>
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
    ''' 
    ''' </summary>
    ''' <param name="RR"></param>
    ''' <param name="frequency"></param>
    ''' <param name="genotypes$"></param>
    ''' <returns>Risk FoldChange result</returns>
    <Extension>
    Public Function CalculateCGR(RR As IEnumerable(Of DataSet),
                                 frequency As IEnumerable(Of DataSet),
                                 genotypes$()) As Double

        Dim weights = RR.WeightValue(frequency, genotypes)
        Dim totalWeight = weights.Values.ProductALL
        Dim actualRR = RR.Select(Function(marker) marker(marker("genotype"))).ProductALL
        Dim CGR# = actualRR / totalWeight

        Return CGR
    End Function
End Module
