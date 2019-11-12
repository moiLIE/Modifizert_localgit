#Region "Imports"
Imports System.IO
#End Region

<Serializable()> _
Public Class FHT59N3_LynxParams

#Region "HV"

    Private _Range As Integer = 5000
    Private _Limit As Integer = 2000
    Private _Voltage As Integer = 0
    Private _DetectorPolarity As Integer = 0
    Private _InhibitPolarity As Integer = 0

    Public Property HV_Range() As Integer
        Get
            Return _Range
        End Get
        Set(ByVal value As Integer)
            _Range = value
        End Set
    End Property

    Public Property HV_Limit() As Integer
        Get
            Return _Limit
        End Get
        Set(ByVal value As Integer)
            _Limit = value
        End Set
    End Property

    Public Property HV_Voltage() As Integer
        Get
            Return _Voltage
        End Get
        Set(ByVal value As Integer)
            _Voltage = value
        End Set
    End Property

    Public Property HV_DetectorPolarity() As Integer
        Get
            Return _DetectorPolarity
        End Get
        Set(ByVal value As Integer)
            _DetectorPolarity = value
        End Set
    End Property

    Public Property HV_InhibitPolarity() As Integer
        Get
            Return _InhibitPolarity
        End Get
        Set(ByVal value As Integer)
            _InhibitPolarity = value
        End Set
    End Property

#End Region

#Region "Amplifier"

    Private _InputPolarity As Integer = 0
    Private _CoarseGain As Double = 2.0
    Private _FineGain As Double = 0.8
    Private _PoleZero As Integer = 0
    Private _BLRMode As Integer = 0
    Private _FilterRiseTime As Double = 2
    Private _FilterFlatTop As Double = 3.2

    Public Property AMP_InputPolarity() As Integer
        Get
            Return _InputPolarity
        End Get
        Set(ByVal value As Integer)
            _InputPolarity = value
        End Set
    End Property

    Public Property AMP_CoarseGain() As Double
        Get
            Return _CoarseGain
        End Get
        Set(ByVal value As Double)
            _CoarseGain = value
        End Set
    End Property

    Public Property AMP_FineGain() As Double
        Get
            Return _FineGain
        End Get
        Set(ByVal value As Double)
            _FineGain = value
        End Set
    End Property

    Public Property AMP_PoleZero() As Integer
        Get
            Return _PoleZero
        End Get
        Set(ByVal value As Integer)
            _PoleZero = value
        End Set
    End Property

    Public Property AMP_BLRMode() As Integer
        Get
            Return _BLRMode
        End Get
        Set(ByVal value As Integer)
            _BLRMode = value
        End Set
    End Property

    Public Property AMP_FilterRiseTime() As Double
        Get
            Return _FilterRiseTime
        End Get
        Set(ByVal value As Double)
            _FilterRiseTime = value
        End Set
    End Property

    Public Property AMP_FilterFlatTop() As Double
        Get
            Return _FilterFlatTop
        End Get
        Set(ByVal value As Double)
            _FilterFlatTop = value
        End Set
    End Property

#End Region

#Region "ADC"

    Private _AcquisitionMode As Integer = 0
    Private _LLDMode As Integer = 0
    Private _LLD As Double = 1.5
    Private _ULD As Double = 100
    Private _ConversionGain As Integer = 4096

    Public Property ADC_AcquisitionMode() As Integer
        Get
            Return _AcquisitionMode
        End Get
        Set(ByVal value As Integer)
            _AcquisitionMode = value
        End Set
    End Property

    Public Property ADC_LLDMode() As Integer
        Get
            Return _LLDMode
        End Get
        Set(ByVal value As Integer)
            _LLDMode = value
        End Set
    End Property

    Public Property ADC_LLD() As Double
        Get
            Return _LLD
        End Get
        Set(ByVal value As Double)
            _LLD = value
        End Set
    End Property

    Public Property ADC_ULD() As Double
        Get
            Return _ULD
        End Get
        Set(ByVal value As Double)
            _ULD = value
        End Set
    End Property

    Public Property ADC_ConversionGain() As Integer
        Get
            Return _ConversionGain
        End Get
        Set(ByVal value As Integer)
            _ConversionGain = value
        End Set
    End Property

#End Region

#Region "Stabilizer"

    Private _Centroid As Integer = 2920
    Private _Window As Integer = 6
    Private _Spacing As Integer = 10
    Private _Multiplier As Integer = 1
    Private _WindowRatio As Double = 1
    Private _UseNaI As Integer = 0
    Private _GainRatioAutoMode As Integer = 0
    Private _StabMode As Integer = 1

    Public Property STAB_Centroid() As Integer
        Get
            Return _Centroid
        End Get
        Set(ByVal value As Integer)
            _Centroid = value
        End Set
    End Property

    Public Property STAB_Window() As Integer
        Get
            Return _Window
        End Get
        Set(ByVal value As Integer)
            _Window = value
        End Set
    End Property

    Public Property STAB_Spacing() As Integer
        Get
            Return _Spacing
        End Get
        Set(ByVal value As Integer)
            _Spacing = value
        End Set
    End Property

    Public Property STAB_Multiplier() As Integer
        Get
            Return _Multiplier
        End Get
        Set(ByVal value As Integer)
            _Multiplier = value
        End Set
    End Property

    Public Property STAB_WindowRatio() As Double
        Get
            Return _WindowRatio
        End Get
        Set(ByVal value As Double)
            _WindowRatio = value
        End Set
    End Property

    Public Property STAB_UseNaI() As Integer
        Get
            Return _UseNaI
        End Get
        Set(ByVal value As Integer)
            _UseNaI = value
        End Set
    End Property

    Public Property STAB_GainRatioAutoMode() As Integer
        Get
            Return _GainRatioAutoMode
        End Get
        Set(ByVal value As Integer)
            _GainRatioAutoMode = value
        End Set
    End Property

    Public Property STAB_StabMode() As Integer
        Get
            Return _StabMode
        End Get
        Set(ByVal value As Integer)
            _StabMode = value
        End Set
    End Property

#End Region

#Region "GUI"

    Private _Polarity As New Dictionary(Of String, Integer)
    Private _ManAuto As New Dictionary(Of String, Integer)
    Private _BLR As New Dictionary(Of String, Integer)
    Private _Acq As New Dictionary(Of String, Integer)
    Private _StabRange As New Dictionary(Of String, Integer)
    Private _Stabilizer As New Dictionary(Of String, Integer)
    Private _HVRange As New Dictionary(Of String, Integer)
    Private _ConversionGainL As New Dictionary(Of String, Integer)
    Private _CoarseGainL As New Dictionary(Of String, Double)
    Private _MultiplierL As New Dictionary(Of String, Integer)
    Private _FilterRiseTimeMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _FilterFlatTopMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _AmplifierPoleZeroMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _AmplifierFineGainMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _ADCLLDMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _ADCULDMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _STABCentroidMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _STABWindowMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _STABSpacingMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit
    Private _STABRatioMinMax As New List(Of Object) 'Min, Max, Divider, Format, Unit

    Public Const MSG_SelectOneItemOfList = "Please select one of the listed items."

    Public ReadOnly Property Polarity() As Dictionary(Of String, Integer)
        Get
            Return _Polarity
        End Get
    End Property

    Public ReadOnly Property ManAuto() As Dictionary(Of String, Integer)
        Get
            Return _ManAuto
        End Get
    End Property

    Public ReadOnly Property BLR() As Dictionary(Of String, Integer)
        Get
            Return _BLR
        End Get
    End Property

    Public ReadOnly Property Acq() As Dictionary(Of String, Integer)
        Get
            Return _Acq
        End Get
    End Property

    Public ReadOnly Property StabRange() As Dictionary(Of String, Integer)
        Get
            Return _StabRange
        End Get
    End Property

    Public ReadOnly Property Stabilizer() As Dictionary(Of String, Integer)
        Get
            Return _Stabilizer
        End Get
    End Property

    Public ReadOnly Property HVRange() As Dictionary(Of String, Integer)
        Get
            Return _HVRange
        End Get
    End Property

    Public ReadOnly Property ConversionGainL() As Dictionary(Of String, Integer)
        Get
            Return _ConversionGainL
        End Get
    End Property

    Public ReadOnly Property CoarseGainL() As Dictionary(Of String, Double)
        Get
            Return _CoarseGainL
        End Get
    End Property

    Public ReadOnly Property MultiplierL() As Dictionary(Of String, Integer)
        Get
            Return _MultiplierL
        End Get
    End Property

    Public ReadOnly Property FilterRiseTimeMinMax() As List(Of Object)
        Get
            Return _FilterRiseTimeMinMax
        End Get
    End Property

    Public ReadOnly Property FilterFlatTopMinMax() As List(Of Object)
        Get
            Return _FilterFlatTopMinMax
        End Get
    End Property

    Public ReadOnly Property AmplifierPoleZeroMinMax() As List(Of Object)
        Get
            Return _AmplifierPoleZeroMinMax
        End Get
    End Property

    Public ReadOnly Property AmplifierFineGainMinMax() As List(Of Object)
        Get
            Return _AmplifierFineGainMinMax
        End Get
    End Property

    Public ReadOnly Property ADCLLDMinMax() As List(Of Object)
        Get
            Return _ADCLLDMinMax
        End Get
    End Property

    Public ReadOnly Property ADCULDMinMax() As List(Of Object)
        Get
            Return _ADCULDMinMax
        End Get
    End Property

    Public ReadOnly Property STABCentroidMinMax() As List(Of Object)
        Get
            Return _STABCentroidMinMax
        End Get
    End Property

    Public ReadOnly Property STABWindowMinMax() As List(Of Object)
        Get
            Return _STABWindowMinMax
        End Get
    End Property

    Public ReadOnly Property STABSpacingMinMax() As List(Of Object)
        Get
            Return _STABSpacingMinMax
        End Get
    End Property

    Public ReadOnly Property STABRatioMinMax() As List(Of Object)
        Get
            Return _STABRatioMinMax
        End Get
    End Property

    Private Sub CreateGUILists()
        Try
            _Polarity.Add("Positive", 0)
            _Polarity.Add("Negative", 1)
            _ManAuto.Add("Manual", 0)
            _ManAuto.Add("Automatic", 1)
            _BLR.Add("Automatic", 0)
            _BLR.Add("Hard", 1)
            _BLR.Add("Medium", 2)
            _BLR.Add("Soft", 3)
            _Acq.Add("PHA", 0)
            _Acq.Add("MSS", 1)
            _Acq.Add("DLFC", 2)
            _Acq.Add("List", 3)
            _Acq.Add("Tlist", 4)
            _StabRange.Add("Ge", 0)
            _StabRange.Add("NaI", 1)
            _Stabilizer.Add("Off", 0)
            _Stabilizer.Add("On", 1)
            _Stabilizer.Add("Hold", 2)
            _HVRange.Add("200", 200)
            _HVRange.Add("1500", 1500)
            _HVRange.Add("5000", 5000)
            _ConversionGainL.Add("256", 256)
            _ConversionGainL.Add("512", 512)
            _ConversionGainL.Add("1024", 1024)
            _ConversionGainL.Add("2048", 2048)
            _ConversionGainL.Add("4096", 4096)
            _ConversionGainL.Add("8192", 8192)
            _ConversionGainL.Add("16384", 16384)
            _ConversionGainL.Add("32768", 32768)
            _CoarseGainL.Add("2.00", 2.0)
            _CoarseGainL.Add("2.36", 2.36)
            _CoarseGainL.Add("2.82", 2.82)
            _CoarseGainL.Add("3.36", 3.36)
            _CoarseGainL.Add("4.00", 4.0)
            _CoarseGainL.Add("4.76", 4.76)
            _CoarseGainL.Add("5.66", 5.66)
            _CoarseGainL.Add("6.72", 6.72)
            _CoarseGainL.Add("8.00", 8.0)
            _CoarseGainL.Add("9.52", 9.52)
            _CoarseGainL.Add("11.32", 11.32)
            _CoarseGainL.Add("13.46", 13.46)
            _CoarseGainL.Add("16.00", 16.0)
            _CoarseGainL.Add("19.02", 19.02)
            _CoarseGainL.Add("22.62", 22.62)
            _CoarseGainL.Add("26.90", 26.9)
            _CoarseGainL.Add("32.00", 32.0)
            _CoarseGainL.Add("38.06", 38.06)
            _CoarseGainL.Add("45.26", 45.26)
            _CoarseGainL.Add("53.82", 53.82)
            _CoarseGainL.Add("64.00", 64.0)
            _CoarseGainL.Add("76.10", 76.1)
            _CoarseGainL.Add("90.50", 90.5)
            _CoarseGainL.Add("107.64", 107.64)
            _CoarseGainL.Add("128.00", 128.0)
            _CoarseGainL.Add("152.22", 152.22)
            _CoarseGainL.Add("181.02", 181.02)
            _CoarseGainL.Add("215.26", 215.26)
            _CoarseGainL.Add("256.00", 256.0)
            _CoarseGainL.Add("304.44", 304.44)
            _CoarseGainL.Add("362.04", 362.04)
            _CoarseGainL.Add("430.54", 430.54)
            _MultiplierL.Add("1", 1)
            _MultiplierL.Add("2", 2)
            _MultiplierL.Add("4", 4)
            _MultiplierL.Add("8", 8)
            _MultiplierL.Add("16", 16)
            _MultiplierL.Add("32", 32)
            _MultiplierL.Add("64", 64)
            _MultiplierL.Add("128", 128)
            _MultiplierL.Add("256", 256)
            _MultiplierL.Add("512", 512)
            _FilterRiseTimeMinMax.Add(2)
            _FilterRiseTimeMinMax.Add(510)
            _FilterRiseTimeMinMax.Add(10)
            _FilterRiseTimeMinMax.Add("0.0")
            _FilterRiseTimeMinMax.Add("µS")
            _FilterFlatTopMinMax.Add(0)
            _FilterFlatTopMinMax.Add(32)
            _FilterFlatTopMinMax.Add(10)
            _FilterFlatTopMinMax.Add("0.0")
            _FilterFlatTopMinMax.Add("µS")
            _AmplifierPoleZeroMinMax.Add(0)
            _AmplifierPoleZeroMinMax.Add(4095)
            _AmplifierPoleZeroMinMax.Add(1)
            _AmplifierPoleZeroMinMax.Add("0")
            _AmplifierPoleZeroMinMax.Add("")
            _AmplifierFineGainMinMax.Add(80000)
            _AmplifierFineGainMinMax.Add(120000)
            _AmplifierFineGainMinMax.Add(100000)
            _AmplifierFineGainMinMax.Add("0.00000")
            _AmplifierFineGainMinMax.Add("")
            _ADCLLDMinMax.Add(0)
            _ADCLLDMinMax.Add(1000)
            _ADCLLDMinMax.Add(10)
            _ADCLLDMinMax.Add("0.0")
            _ADCLLDMinMax.Add("%")
            _ADCULDMinMax.Add(0)
            _ADCULDMinMax.Add(1100)
            _ADCULDMinMax.Add(10)
            _ADCULDMinMax.Add("0.0")
            _ADCULDMinMax.Add("%")
            _STABCentroidMinMax.Add(2)
            _STABCentroidMinMax.Add(32766)
            _STABCentroidMinMax.Add(1)
            _STABCentroidMinMax.Add("0")
            _STABCentroidMinMax.Add("")
            _STABWindowMinMax.Add(1)
            _STABWindowMinMax.Add(128)
            _STABWindowMinMax.Add(1)
            _STABWindowMinMax.Add("0")
            _STABWindowMinMax.Add("")
            _STABSpacingMinMax.Add(2)
            _STABSpacingMinMax.Add(512)
            _STABSpacingMinMax.Add(1)
            _STABSpacingMinMax.Add("0")
            _STABSpacingMinMax.Add("")
            _STABRatioMinMax.Add(10)
            _STABRatioMinMax.Add(100000)
            _STABRatioMinMax.Add(1000)
            _STABRatioMinMax.Add("0.00")
            _STABRatioMinMax.Add("")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub New()
        CreateGUILists()
    End Sub

    ''' <summary>
    ''' Datensatz binär speichern
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveMeBinary(ByVal Path As String)
        Try
            Dim serializer As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Dim Filename As String = "FHT59N3LynxPar.bin"
            Dim FileStr As FileStream
            Filename = Path & Filename
            FileStr = New FileStream(Filename, FileMode.OpenOrCreate)
            serializer.Serialize(FileStr, Me)
            FileStr.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' ganzen Historyfile einlesen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function RestoreMeFromFileBinary(ByVal Path As String) As FHT59N3_LynxParams
        Try
            Dim serializer As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Dim FileStr As FileStream
            Dim Filename As String = "FHT59N3LynxPar.bin"
            Dim Data As New FHT59N3_LynxParams
            Filename = Path & Filename
            If File.Exists(Filename) Then
                FileStr = New FileStream(Filename, FileMode.Open)
                Data = CType(serializer.Deserialize(FileStr), FHT59N3_LynxParams)
                FileStr.Close()
                Return Data
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Kopiere mich selbst an eine andere Instanz
    ''' WICHITIG: Über das SetValue geht das ganze nur für Variablen die nicht ReadOnly sind!
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CopyMe() As FHT59N3_LynxParams
        Try
            'Return Container
            Return CType(Me.MemberwiseClone, FHT59N3_LynxParams)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return Nothing
        End Try
    End Function

#End Region

End Class
