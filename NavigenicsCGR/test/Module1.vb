Imports Microsoft.VisualBasic.Data.csv.IO
Imports NavigenicsCGR

Module Module1

    Sub Main()

        Dim RR = {
            New DataSet("Marker1") From {{"AA", 9.06}, {"AB", 2.64}, {"BB", 1}},
            New DataSet("Marker2") From {{"AA", 9.47}, {"AB", 3.12}, {"BB", 1}},
            New DataSet("Marker3") From {{"AA", 6.78}, {"AB", 2.31}, {"BB", 1}}
        }

        Dim freq = {
            New DataSet("Marker1") From {{"AA", 0.016}, {"AB", 0.4}, {"BB", 0.58}},
            New DataSet("Marker2") From {{"AA", 0.4}, {"AB", 0.38}, {"BB", 0.21}},
            New DataSet("Marker3") From {{"AA", 0.88}, {"AB", 0.12}, {"BB", 0}}
        }

        Dim genotype As New Genotype With {
            .combination = {"AA", "AB", "BB"},
            .Markers = New Dictionary(Of String, String) From {
                {"Marker1", "AB"},
                {"Marker2", "AA"},
                {"Marker3", "AA"}
            }
        }

        Dim cgr = NavigenicsCGR.CalculateCGR(RR, freq, genotype)
        ' 计算个体的实际发病率。假设改疾病的群体发病率为3.1%，那么该个体的实际发病率为``2.94 x 3.1%=9%``。
        Dim risk = 3.1 * cgr

        Call Console.WriteLine($"CGR = {cgr}")
        Call Console.WriteLine($"Risk = {risk}")

        Pause()
    End Sub

End Module
