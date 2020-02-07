Imports System.Linq

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_NuclideList

    Private ReadOnly _NuclidesByName As New SortedDictionary(Of String, FHT59N3MCA_Nuclide)
    Private ReadOnly _NuclidesByNumber As New Dictionary(Of Integer, FHT59N3MCA_Nuclide)

    Public Sub New()
    End Sub

    Public Property AlarmMaxViolocatedLevel As Integer = 0



    Public ReadOnly Property NuclideCount() As Integer
        Get
            Return _NuclidesByName.Count
        End Get
    End Property

    Public ReadOnly Property Nuclides() As List(Of FHT59N3MCA_Nuclide)
        Get
            Return _NuclidesByName.Values.ToList()
        End Get
    End Property

    Public Sub AddNuclide(ByVal Nuclide As FHT59N3MCA_Nuclide)
        If Not _NuclidesByName.ContainsKey(Nuclide.Library.Name) Then

            _NuclidesByName.Add(Nuclide.Library.Name, Nuclide)

            'fängt ab 1 an!
            _NuclidesByNumber.Add(_NuclidesByName.Count, Nuclide)
            Nuclide.Library.IndexNr = _NuclidesByName.Count

        End If
    End Sub

    Public Function GetNuclide(ByVal Number As Integer) As FHT59N3MCA_Nuclide

        If _NuclidesByNumber.ContainsKey(Number) Then
            Return _NuclidesByNumber(Number)
        Else
            Return Nothing
        End If

    End Function

    Public Function GetNuclide(ByVal Name As String) As FHT59N3MCA_Nuclide

        If _NuclidesByName.ContainsKey(Name) Then
            Return _NuclidesByName(Name)
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' Sets the global maximum alarm level (if higher than old one, the maximum value is the new global max alarm level)
    ''' </summary>
    ''' <param name="currentLevel">The current level.</param>
    Public Sub SetGlobalMaxAlarmLevel(currentLevel As Integer)
        If (currentLevel > AlarmMaxViolocatedLevel) Then
            AlarmMaxViolocatedLevel = currentLevel
        End If
    End Sub


End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_Nuclide

    'nklHalbWD#
    'lambda
    'lfd
    'nkorr
    'nkta
    'nwte
    'nwtm
    'ident
    'nklWiMittl#
    'nklWiFeler#
    'nklNaGrenz#
    'überschreibung eines Alarmnuklids

    Public Library As New NuclideLibraryParams

    Public SpectrumAnalysis As New NuclideSpectrumParams



End Class

Public Class NuclideLibraryParams

    'Aus NLB-Datei
    Public Property Name As String

    'Aus NLB-Datei
    Public Property NuclidNumber As Integer

    'Aus NLB-Datei
    Public Property NuclideHalfLife As Double

    'Aus NLB-Datei
    Public Property DecayConstant As Double

    'Aus interner Nummer für Auflistung
    Public Property IndexNr As Integer

End Class

Public Class NuclideSpectrumParams
    'Aus aktueller Spektrumanalyse
    'Public Property DecayConstantFilterTime As Double

    'Aus aktueller Spektrumanalyse
    Public Property NuclideCorrectionFactor As Double

    'Aus aktueller Spektrumanalyse
    Public Property DetectionLimit_Bqm3 As Double

    'Aus aktueller Spektrumanalyse
    Public Property DetectionError_Percent As Double

    'Aus aktueller Spektrumanalyse
    Public Property Concentration_Bqm3 As Double

    'Aus aktueller Spektrumanalyse
    Public Property KeyLineFound As Boolean

    'Aus aktueller Spektrumanalyse
    Public Property Activity As Double

    'Aus aktueller Spektrumanalyse
    Public Property DetectError As Double

    'Aus aktueller Spektrumanalyse
    Public Property DetectionLimit As Double

    'Aus aktueller Spektrumanalyse
    Public Property KeyLineEnergy As Double

    Public Property SpectrumNuclideNumber As Integer

    'Aus aktueller Spektrumanalyse
    Public Property ChannelNumber As Integer

    'Aus aktueller Spektrumanalyse
    Public Property ExceededAlarmLevel As Integer

    'Aus Namensvergleich mit Alarmliste 
    Public Property Alarm1Limit As Double

    'Aus Namensvergleich mit Alarmliste 
    Public Property Alarm1LimitCurrent As Double

    'Aus Namensvergleich mit Alarmliste 
    Public Property Alarm2Limit As Double

    'Aus Namensvergleich mit Alarmliste 
    Public Property Alarm2LimitCurrent As Double

    ''' <summary>
    ''' Sets the maximum alarm level.
    ''' </summary>
    ''' <param name="currentLevel">The current level.</param>
    Public Sub SetMaxAlarmLevel(currentLevel As Integer)
        If (currentLevel > ExceededAlarmLevel) Then
            ExceededAlarmLevel = currentLevel
        End If
    End Sub
End Class



''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_AlarmNuclides

    Private ReadOnly _NuclideList As New Dictionary(Of String, FHT59N3MCA_AlarmNuclide)
    Private ReadOnly _IndexList As New Dictionary(Of Integer, FHT59N3MCA_AlarmNuclide)


    

    Public ReadOnly Property Nuclides() As IEnumerable(Of FHT59N3MCA_AlarmNuclide)
        Get
            Return _NuclideList.Values
        End Get
    End Property

    Public ReadOnly Property Nuclide_ByNumber(ByVal Number As Integer) As FHT59N3MCA_AlarmNuclide
        Get
            If _IndexList.ContainsKey(Number) Then
                Return _IndexList(Number)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property Nuclide_ByName(ByVal Name As String) As FHT59N3MCA_AlarmNuclide
        Get
            If _NuclideList.ContainsKey(Name) Then
                Return _NuclideList(Name)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property AlarmNuclideCount() As Integer
        Get
            Return _NuclideList.Count
        End Get
    End Property

    Public Sub Clear()
        _NuclideList.Clear()
        _IndexList.Clear()
    End Sub

    Public Sub AddNuclide(ByVal Nuclide As FHT59N3MCA_AlarmNuclide)
        If Not _NuclideList.ContainsKey(Nuclide.Name) Then

            _NuclideList.Add(Nuclide.Name, Nuclide)
            _IndexList.Add(_NuclideList.Count, Nuclide)

        End If
    End Sub
End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_AlarmNuclide
    ''' <summary>
    ''' Gets or sets the name.
    ''' </summary>
    ''' <value>
    ''' The name.
    ''' </value>
    Public Property Name As String

    ''' <summary>
    ''' Gets or sets the alarm value1 with hysterese.
    ''' </summary>
    ''' <value>
    ''' The alarm value1 with hysterese.
    ''' </value>
    Public Property AlarmValue1WithHysterese As Double

    ''' <summary>
    ''' Gets or sets the alarm value2 with hysterese.
    ''' </summary>
    ''' <value>
    ''' The alarm value2 with hysterese.
    ''' </value>
    Public Property AlarmValue2WithHysterese As Double

    ''' <summary>
    ''' Gets or sets the pre-alarm (Alarm1)
    ''' </summary>
    ''' <value>
    ''' The alarm value1.
    ''' </value>
    Public Property AlarmValue1 As Double

    ''' <summary>
    ''' Gets or sets the main alarm (Alarm2)
    ''' </summary>
    ''' <value>
    ''' The alarm value2.
    ''' </value>
    Public Property AlarmValue2 As Double

End Class


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_CertificateNuclides

    Public Property Name As String

    Public Property Energy As Double

    Public Property EmissionProbability As Double

    Public Property ActivitykBq As Double

    Public Property CertificateDate As Date
End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_CertificateActivities

    Public Property Name As String

    Public Property ActivitykBq As Double

    Public Property ProbEnergies As List(Of Double) = New List(Of Double)

    Public Property CertificateDate As Date

    Public Property OK As Boolean

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_Peaks

    Public Property PeakList As New List(Of FHT59N3MCA_Peak)

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3MCA_Peak
    Public Property PeakEnergy As Double

    Public Property PeakChannel As Integer

    Public Property PeakArea As Double

    Public Property PeakAreaErr As Double

    Public Property PeakBckg As Double

    Public Property PeakBckgErr As Double

    'Public Property PeakRChiSq As Double

    Public Property PeakFwhm As Double

    Public Property IsKeyLine As Boolean

    Public Property NuclideNumber As Integer
    Public Property UseWtm As Boolean

    'Public Property NuclideName As String

    Property GrossCounts As Object

End Class