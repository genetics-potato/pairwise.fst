Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO

Public Module CGR

    <Extension>
    Public Function WeightValue(markersRR As IEnumerable(Of DataSet),
                                frequency As IEnumerable(Of DataSet),
                                genotypes$()) As Dictionary(Of String, Double)
        Dim weight#
        Dim frequencyTable = frequency.ToDictionary
        Dim markerFreq As DataSet
        Dim markerWeights As New Dictionary(Of String, Double)

        For Each marker As DataSet In markersRR
            markerFreq = frequencyTable(marker.ID)
            weight = Aggregate genotype As String
                     In genotypes
                     Let relativeWeight = marker(genotype) * markerFreq(genotype)
                     Into Sum(relativeWeight)

            markerWeights(marker.ID) = weight
        Next

        Return markerWeights
    End Function

End Module
