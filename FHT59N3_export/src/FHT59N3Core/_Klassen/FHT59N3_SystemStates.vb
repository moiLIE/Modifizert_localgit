Imports System.ComponentModel
Imports System.Reflection
Imports System.IO
Imports System.Globalization
Imports ThermoUtilities

<Serializable()> _
Public Class FHT59N3_SystemStates
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_SystemStates.vb 417 2017-06-20 14:24:01Z marcel.bender $
    ' Title: system states
    '
    ' Description:
    ' global data storage (public properties)
    '  - calculated alarms
    '
    ' Note: the calculated values are stored in FHT59N3_SystemProperties
    ' -------------------------------------------------------------

#Region "Status"

    Public Enum StateLevel
        OK = 0
        WARNING = 1
        FATAL = 2
    End Enum

    Public Enum FHT59N3_EmergencyStopCoolingStates
        UNKNOWN = 0
        COOLING_ACTIVE = 1
        WARMING_PHASE_FORCED = 2
        COOLING_PHASE_PREPARED = 3
    End Enum

    Private _SumState As Integer = 0
    Private _SumStateLevel As StateLevel
    Private _MeasValuesCorrupt As Boolean = False
    Private _AlarmMode As Boolean = False
    Private _IntensiveMode As Boolean = False

    Private _Maintenance As Boolean = False
    Private _K40ToLow_NotFound As Boolean = False

    ''' <summary>
    ''' ChangableObject wird verwendet um zu ermöglichen das sich gemerkt werden kann wann zuletzt dieser Status gesetzt worden ist.
    ''' </summary>
    Public _AirFlowLessThen1Cubic As New ChangableObject(Of Boolean) With {.Value = False, .ChangeTimestamp = DateTime.MinValue}

    Private _HVOff As Boolean = False
    Private _NoFilterstep As Boolean = False
    Private _BypassOpen As Boolean = False
    Private _AnalyzationCancelled As Boolean = False
    Private _AirFlowGreaterThen12Cubic As Boolean = False
    Private _EcoolerOff As Boolean = False
    Private _SpectrumDeadTimeBigger20Percent As Boolean = False
    Private _N2FillingGoingLow As Boolean = False
    Private _FilterHasToBeChanged As Boolean = False
    Private _CheckTempPressure As Boolean = False
    Private _K40ToBig As Boolean = False
    Private _DataTransferError As Boolean = False
    Private _UpsOnBattery As Boolean = False

    'state machine for warming/cooling of detector crystal
    Private _EmergencyStopCoolingState As FHT59N3_EmergencyStopCoolingStates = FHT59N3_EmergencyStopCoolingStates.UNKNOWN


    <NonSerialized()> Private _Utillities As New ThermoUtilities.ThermoUtilities

    Public ReadOnly Property SumState() As Integer
        Get
            GenerateSumState()
            Return _SumState
        End Get
    End Property

    Public ReadOnly Property SumStateLevel() As StateLevel
        Get
            Return _SumStateLevel
        End Get
    End Property


    ''' <summary>
    ''' Set or get the "alarm mode"
    ''' It can be activated within the "operations" menu or by the nuklid monitoring
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AlarmMode() As Boolean
        Get
            Return _AlarmMode
        End Get
        Set(ByVal value As Boolean)
            _AlarmMode = value
            RaiseEvent StateChanged()
        End Set
    End Property

    ''' <summary>
    ''' Set or get the "intensive mode"
    ''' Running under "intensive mode" means that the filter tape has more distance between used segments (preventing side nuklides)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IntensiveMode() As Boolean
        Get
            Return _IntensiveMode
        End Get
        Set(ByVal value As Boolean)
            _IntensiveMode = value
            RaiseEvent StateChanged()
        End Set
    End Property

    Public Property MeasValuesCorrupt() As Boolean
        Get
            Return _MeasValuesCorrupt
        End Get
        Set(ByVal value As Boolean)
            _MeasValuesCorrupt = value
            RaiseEvent StateChanged()
        End Set
    End Property

    ''' <summary>
    ''' Global property to get the state of cooler (including emergency stop = warming phase)
    ''' </summary>
    ''' <value>FHT59N3_EmergencyStopCoolingStates</value>
    ''' <returns>FHT59N3_EmergencyStopCoolingStates</returns>
    Public Property EmergencyStopCoolingState() As FHT59N3_EmergencyStopCoolingStates
        Get
            Return _EmergencyStopCoolingState
        End Get
        Set(ByVal value As FHT59N3_EmergencyStopCoolingStates)
            _EmergencyStopCoolingState = value
            RaiseEvent CoolingStateChanged()
        End Set
    End Property


    '--------------------------------------------------------------------------------------
    'alarm software bits
    '--------------------------------------------------------------------------------------

    <Attribute_IntegerValue(1)> _
    Public Property Maintenance() As Boolean
        Get
            Return _Maintenance
        End Get
        Set(ByVal value As Boolean)
            _Maintenance = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(2)> _
    Public Property K40ToLow_NotFound() As Boolean
        Get
            Return _K40ToLow_NotFound
        End Get
        Set(ByVal value As Boolean)
            _K40ToLow_NotFound = value
            RaiseEvent StateChanged()
        End Set
    End Property

    ' the airflow is less then <Z> m3/h
    <Attribute_IntegerValue(4)> _
    Public Property AirFlowLessThen1Cubic() As Boolean
        Get
            Return _AirFlowLessThen1Cubic.Value
        End Get
        Set(ByVal value As Boolean)
            _AirFlowLessThen1Cubic.Value = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(8)> _
    Public Property HVOff() As Boolean
        Get
            Return _HVOff
        End Get
        Set(ByVal value As Boolean)
            _HVOff = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(16)> _
    Public Property NoFilterstep() As Boolean
        Get
            Return _NoFilterstep
        End Get
        Set(ByVal value As Boolean)
            _NoFilterstep = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(32)> _
    Public Property BypassOpen() As Boolean
        Get
            Return _BypassOpen
        End Get
        Set(ByVal value As Boolean)
            _BypassOpen = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(64)> _
    Public Property AnalyzationCancelled() As Boolean
        Get
            Return _AnalyzationCancelled
        End Get
        Set(ByVal value As Boolean)
            _AnalyzationCancelled = value
            RaiseEvent StateChanged()
        End Set
    End Property

    ' the airflow is greater then <Y> m3/h
    <Attribute_IntegerValue(128)> _
    Public Property AirFlowGreaterThen12Cubic() As Boolean
        Get
            Return _AirFlowGreaterThen12Cubic
        End Get
        Set(ByVal value As Boolean)
            _AirFlowGreaterThen12Cubic = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(256)> _
    Public Property EcoolerOff() As Boolean
        Get
            Return _EcoolerOff
        End Get
        Set(ByVal value As Boolean)
            _EcoolerOff = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(512)> _
    Public Property SpectrumDeadTimeBigger20Percent() As Boolean
        Get
            Return _SpectrumDeadTimeBigger20Percent
        End Get
        Set(ByVal value As Boolean)
            _SpectrumDeadTimeBigger20Percent = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(1024)> _
    Public Property N2FillingGoingLow() As Boolean
        Get
            Return _N2FillingGoingLow
        End Get
        Set(ByVal value As Boolean)
            _N2FillingGoingLow = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(2048)> _
    Public Property FilterHasToBeChanged() As Boolean
        Get
            Return _FilterHasToBeChanged
        End Get
        Set(ByVal value As Boolean)
            _FilterHasToBeChanged = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(4096)> _
    Public Property CheckTempPressure() As Boolean
        Get
            Return _CheckTempPressure
        End Get
        Set(ByVal value As Boolean)
            _CheckTempPressure = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(8192)> _
    Public Property K40ToBig() As Boolean
        Get
            Return _K40ToBig
        End Get
        Set(ByVal value As Boolean)
            _K40ToBig = value
            RaiseEvent StateChanged()
        End Set
    End Property

    ''' <summary>
    ''' data transfer error with external SPS
    ''' </summary>
    ''' <value>true: alarm, false: working</value>
    ''' <returns>true: alarm, false: working</returns>
    ''' <remarks>no connection to SPS, no acknowledge from SPS etc.</remarks>
    <Attribute_IntegerValue(16384)> _
    Public Property DataTransferError() As Boolean
        Get
            Return _DataTransferError
        End Get
        Set(ByVal value As Boolean)
            _DataTransferError = value
            RaiseEvent StateChanged()
        End Set
    End Property

    <Attribute_IntegerValue(32768)> _
    Public Property UpsOnBattery As Boolean
        Get
            Return _UpsOnBattery
        End Get
        Set(ByVal value As Boolean)
            _UpsOnBattery = value
            RaiseEvent StateChanged()
        End Set
    End Property

    Public Event StateChanged()

    Public Event CoolingStateChanged()

#End Region

#Region "Private Methoden"


    ''' <summary>
    ''' Summenstatus als Integer Wert erzeugen (Bitcodiert)
    ''' Zu jedem Statusbit gibt es einen dazugehörigen Integerwert,
    ''' der zum Summenstatus addiert wird wenn das Bit = True ist
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GenerateSumState()
        Try
            Dim MyType As Type   'Typ FHT59N3_States
            Dim PropertyAttributeType As Type 'typ des Attributes des propertys
            Dim Propertys() As PropertyInfo 'Propertyliste von MyType
            Dim Properti As PropertyInfo 'eine property
            Dim AttributeList() As Object 'liste aller attribute eines propertys
            Dim AttributeProperti As PropertyInfo   'property des Attributes einer property von t
            Dim AttributeValue As Object   'wert eine attibutes eines property von t
            Dim PropertyValue As Object   'wert eines property von t
            _SumState = 0
            MyType = Me.GetType 'typ herausfinden
            Propertys = MyType.GetProperties 'alle Properties laden
            For i As Integer = 0 To Propertys.Length - 1   'für alle Properties
                Properti = Propertys(i) 'aktuelle Property
                AttributeList = Properti.GetCustomAttributes(False) 'alle Attribute abholen
                For j As Integer = 0 To AttributeList.Length - 1
                    'Reihenfolge
                    If AttributeList(j).ToString = "FHT59N3Core.Attribute_IntegerValue" Then
                        PropertyAttributeType = AttributeList(j).GetType
                        AttributeProperti = PropertyAttributeType.GetProperty("IntegerValue")
                        'AttributeProperti.SetValue(AttributeList(j), System.Convert.ChangeType(1, AttributeProperti.PropertyType, CultureInfo.CurrentCulture), Nothing)  'Wert setzen
                        AttributeValue = AttributeProperti.GetValue(AttributeList(j), Nothing)
                        'Wert der eigenschaft
                        PropertyValue = Properti.GetValue(Me, Nothing)
                        If CType(PropertyValue, Boolean) = True Then
                            _SumState = _SumState + CType(AttributeValue, Integer)
                        End If
                    End If
                Next j
            Next i
            _MeasValuesCorrupt = False
            If _SumState = 0 Then
                _SumStateLevel = StateLevel.OK
            End If

            ' WARNING states
            If _N2FillingGoingLow Or _FilterHasToBeChanged Or _CheckTempPressure Or _K40ToBig Or _DataTransferError Or _UpsOnBattery Then
                _SumStateLevel = StateLevel.WARNING
            End If

            ' FATAL states
            If _Maintenance Or _K40ToLow_NotFound Or AirFlowLessThen1Cubic Or _AirFlowGreaterThen12Cubic Or _HVOff Or _SpectrumDeadTimeBigger20Percent _
                Or _NoFilterstep Or _BypassOpen Or _AnalyzationCancelled Or _EcoolerOff Then
                _SumStateLevel = StateLevel.FATAL
                _MeasValuesCorrupt = True
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Datensatz binär speichern
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveMeBinary(ByVal Path As String)
        'Nach dem hinzufügen eines Events kommt es beim einlesen des binärkodierten streams zu einer Fehlermeldung
        '<NonSerialized()> für ein Event geht leider nicht
        'Deswegen wird der Status als XML Datei gespeichert
        Try
            Dim serializer As New Xml.Serialization.XmlSerializer(Me.GetType) 'System.Runtime.Serialization.Formatters.Binary.BinaryFormatter 
            Dim Filename As String = "FHT59N3SystemState.bin"
            Dim FileStr As FileStream
            Filename = Path & Filename
            FileStr = New FileStream(Filename, FileMode.Create)
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
    Public Function RestoreMeFromFileBinary(ByVal Path As String) As FHT59N3_SystemStates
        Try
            Dim serializer As New Xml.Serialization.XmlSerializer(Me.GetType) 'System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Dim FileStr As FileStream
            Dim Filename As String = "FHT59N3SystemState.bin"
            Dim Data As New FHT59N3_SystemStates
            Filename = Path & Filename
            If File.Exists(Filename) Then
                FileStr = New FileStream(Filename, FileMode.Open)
                Data = CType(serializer.Deserialize(FileStr), FHT59N3_SystemStates)
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

#End Region

End Class

''' <summary>
''' Attribut, welches den Wert einer Property als Integer repräsentiert
''' </summary>
''' <remarks></remarks>
<Serializable()> _
<AttributeUsage(AttributeTargets.All)> _
 Public Class Attribute_IntegerValue
    Inherits System.Attribute

    Dim _IntegerValue As Integer

    ''' <summary>
    ''' Sortierreihenfolge
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IntegerValue() As Integer
        Get
            Return _IntegerValue
        End Get
        Set(ByVal value As Integer)
            _IntegerValue = value
        End Set
    End Property

    Public Sub New(ByVal Order As Integer)
        _IntegerValue = Order
    End Sub

End Class

