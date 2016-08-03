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
        Dim A As Double = array.SeqIterator.Sum(Function(x) x.obj.p * N(x.i))
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
        Return 1 - (pops.pA + pops.qa)
    End Function
End Module
