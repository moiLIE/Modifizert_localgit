Imports System.ComponentModel
Imports OrderedPropertyGrid
Imports System
Imports System.Net
Imports System.Drawing
Imports System.Text.RegularExpressions

' -------------------------------------------------------------
' Title: system parameters
'
' Description:
' global configuration values (mostly retrieved via XML configuration)
'  - measure parameters
'  - station parameters
'  - system parameters
'
' Note: global data storage is in file FHT59N3_SystemProperties.vb
' -------------------------------------------------------------
<TypeConverter(GetType(PropertySorter))> _
Public Class FHT59N3_SystemParams

    Public Enum CalibrationTypeEnum
        NearAndFarCalibration = 0
        NearOnlyCalibration = 1
    End Enum

    Public Enum AirFlowThroughPutCalculationModeEnum
        ByMonitorProgram = 0  ' this application
        ByTiaSps = 1          ' TIA (total integrated automation) SPS
    End Enum



    Private _StationSerialNumber As String = "00000000"
    Private _DayStartTime As Integer = 6 'Stunde zu der der Tag neu beginnt
    'wie lange soll bestaubt werden in Stunden
    Private _MeasurementTimemin As Integer = 10 'aller wieviel Minuten soll das SPektrum ausgewertet werden
    Private _FilterStepmm As Integer = 120 'Filter step width in measurement mode [mm]
    Private _FilterStepim As Integer = 240 'Filter step width in intensive mode [mm]

    Private _N2FillThreshold As Integer = 3000 'N2 Füllstandsgrenzwert (Wert für den Spannungsabfall über dem heizbarem Wiederstand, ist der gemessene Wert über diesem Wert, so ist zu wenig N2 im Dewar)
    '=0 bei Electrocool
    'Blendenfaktor Luftdurchsatzmessung

    Private _SPSConnectionType As String = "serial (RS-232)"
    Private _SPSIpNetworkAddress As String
    '1 ... 65535
    ' extended August 2013 to be able to switch between plain and secured SPS protocol handshake
    Private _SPSUseSTXEXTProtocol As Boolean = True
    ' extended Feb 2014 to be able to enable SPS checksum error event log
    Private _SPSOutputChecksumError As Boolean = False


    Private _PrinterPositionToMeasureCenterDistanceMM As Integer
    'filter printer activiation

    Private _TCS_CorrFile As String = "Startup."

    Private _CaptureCycleDetectorTemperature As Integer = 10
    Private _EnableEmergencyStopDetect As Boolean = True  'e-Cooler support
    Private _EnableCapturingDetectorTemperature As Boolean = True  'see also _N2FillThreshold which is used for N2 cooling

    'Luftdurchsatz als Betriebs-m³ oder Norm-m³

    'Defaultwerte bewusst so lassen, da beim Einlesen der Werte keine Grenzverletzung auftreten kann (z.B. wäre default MaxAirFlowAlert bei 12 kleiner als BAG-Standardwert von z.B. 16)
    Private _MinAirFlowAlert As Integer = 1 'erlaubter Eingabebereich von 1 bis 20 (m³/h)
    Private _MaxAirFlowAlert As Integer = 25 'erlaubter Eingabebereich von 5 bis 25 (m³/h)

    Private _CrystalTooWarmTempThreshold As Integer = -150 'Abschaltschwelle im Bereich von -145 bis -155 °C 
    Private _CrystalWarmedUpTempThreshold As Integer = 15 'Schwellwert für Aufwärmphase im Bereich von 0°C bis 25 °C

    'Typ der Kalibrierung, Entwerder wie gewohnt, mit Far(Mischstrahler) and Near(CS137) ), oder "nur Near" mit Mischstrahler

    'calculation is done in this SW or at the external SPS

    'Zwei Referenzpunkte zum Kalibrieren der Spannung und der dazu gehörigen Temperaturen des Detektors
    'Spannung in mV bei 0°C
    'Temperatur bei 0 Volt



    <ml_Category(255, "01 Station Parameters"), _
    ml_DisplayName(256, "Customer"), _
    ml_Description(257, "The name of the customer who owns this machine."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(1)> _
    Public Property Customer As String = "DWD"

    <ml_Category(255, "01 Station Parameters"), _
    ml_DisplayName(258, "Station name"), _
    ml_Description(259, "The name of station hosting this machine."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(2)> _
    Public Property StationName As String = "Offenbach"

    <ml_Category(255, "01 Station Parameters"), _
    ml_DisplayName(589, "Serial number"), _
    ml_Description(590, "The serial number of the of station hosting this machine."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(3)> _
    Public Property StationSerialNumber() As String
        Get
            Return _StationSerialNumber
        End Get
        Set(ByVal value As String)
            Dim stripped As String = Regex.Replace(value, "[^0-9]", "")
            Dim maxLength As Integer = 12
            stripped = If(stripped.Length <= maxLength, stripped, stripped.Substring(0, maxLength))
            _StationSerialNumber = stripped
        End Set
    End Property

    <ml_Category(255, "01 Station Parameters"), _
    ml_DisplayName(260, "Station ID"), _
    ml_Description(261, "The ID of the station hosting this machine."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(4)> _
    Public Property StationID As String = "12"



    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(263, "Daily start time [h]"), _
    ml_Description(264, "Hour of day at which the daily spectrum begins."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100)> _
    Public Property DayStartTime() As Integer
        Get
            Return _DayStartTime
        End Get
        Set(ByVal value As Integer)
            If (value < 0 Or value > 24) Then
                _DayStartTime = 0
                GUI_ShowMessageBox(MSG_DayStartTimeOutOfRange, "Ok", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _DayStartTime = value
            End If
        End Set
    End Property

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(265, "Filter Time [h]"), _
    ml_Description(266, "Time between two Filtersteps, in hours."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(101)> _
    Public Property FilterTimeh As Integer = 4

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(33, "Measurement time [min]"), _
    ml_Description(267, "Time between two measurement points."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(102)> _
    Public Property MeasurementTimemin() As Integer
        Get
            Return _MeasurementTimemin
        End Get
        Set(ByVal value As Integer)
            If value < _MinimumMeasurementTimeMinutes Then
                _MeasurementTimemin = _MinimumMeasurementTimeMinutes
                GUI_ShowMessageBox(MSG_MeasureTimeToSmall, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _MeasurementTimemin = value
            End If
        End Set
    End Property

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(268, "Filter step width in meas. mode [mm]"), _
    ml_Description(269, "How long is a filterstep in measuring mode."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(103)> _
    Public Property FilterStepmm() As Integer
        Get
            Return _FilterStepmm
        End Get
        Set(ByVal value As Integer)
            If value < 60 Then
                _FilterStepmm = 60
                GUI_ShowMessageBox(MSG_StepWidthToSmall, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _FilterStepmm = value
            End If
        End Set
    End Property

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(541, "Filter step width in intensive mode [mm]"), _
    ml_Description(542, "How long is a filterstep in intensive mode."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(104)> _
    Public Property FilterStepim() As Integer
        Get
            Return _FilterStepim
        End Get
        Set(ByVal value As Integer)
            If value < 60 Then
                _FilterStepim = 60
                GUI_ShowMessageBox(MSG_StepWidthToSmall, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _FilterStepim = value
            End If
        End Set
    End Property

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(270, "Available filter steps"), _
    ml_Description(271, "How many filtersteps are available right now."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(105)> _
    Public Property FilterSteps As Integer

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(272, "N2 fill threshold"), _
    ml_Description(273, "The threshold for alarming that there is not enough N2 in the dewar. (=0 for electrocool)"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(106)> _
    Public Property N2FillThreshold() As Integer
        Get
            Return _N2FillThreshold
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then
                _N2FillThreshold = 0
                GUI_ShowMessageBox(MSG_N2ThresholdOutOfRange, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _N2FillThreshold = value
            End If
        End Set
    End Property

  

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(274, "Bezel factor"), _
    ml_Description(275, "Factor of bezel for air flow measurement."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(107)> _
    Public Property FactorBezel As Double = 0.845

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(276, "Factor 1950keV"), _
    ml_Description(277, "Factor for 1950keV to stabilize the efficiency calibration."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(108)> _
    Public Property Factor1950keV As Double = 0.9

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(406, "K40 minimal area [Imp/h]"), _
    ml_Description(407, "The minimal area for K40 to be found."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(109)> _
    Public Property K40MinArea As Double

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(408, "K40 full width at half maximum [keV]"), _
    ml_Description(409, "The full width at half maximum of the K40 peak."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(110)> _
    Public Property K40FWHM As Double

    'Luftdurchsatz als Betriebs-m³ oder Norm-m³
    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(410, "Working Airflow"), _
    ml_Description(411, "If [true] the Airflow is measured/displayed as working airflow."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(111)> _
    Public Property AirFlowWorking As Boolean = False

    'vorgegebener Luftdurchsatz als Betriebs-m³ oder Norm-m³
    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(596, "Airflow Setpoint"), _
    ml_Description(597, "airflow setpoint for pump from 0..25 m³/h"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(112)> _
    Public Property AirFlowSetPoint As Integer = 10

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(507, "alert for minimal air flow"), _
    ml_Description(508, "allowed range for the minimal air flow is between 1 and 20 (m³/h)."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(113)> _
    Public Property MinAirFlowAlert() As Integer
        Get
            Return _MinAirFlowAlert
        End Get
        Set(ByVal value As Integer)

            Dim lowerLimit = 1
            Dim upperLimit = 20

            If value < lowerLimit Or value > upperLimit Then
                If (WasInitialized) Then
                    GUI_ShowMessageBox(MSG_MinAirFlowAlertOfRange, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                    ' der Minimumwert ist größer als der Maximumwert, kann nicht sein...
                End If

            ElseIf value > _MaxAirFlowAlert Then

                If (WasInitialized) Then
                    GUI_ShowMessageBox(MSG_MinHigherMaxAlertOfRange, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                End If

            Else
                _MinAirFlowAlert = value
            End If
        End Set
    End Property

    <ml_Category(262, "02 Measurement Parameters"), _
    ml_DisplayName(509, "alert for maximal air flow"), _
    ml_Description(510, "allowed range for the maximal air flow is between 5 and 25 (m³/h)."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(114)> _
    Public Property MaxAirFlowAlert() As Integer
        Get
            Return _MaxAirFlowAlert
        End Get
        Set(ByVal value As Integer)

            Dim lowerLimit = 5
            Dim upperLimit = 25

            If value < lowerLimit Or value > upperLimit Then

                If (WasInitialized) Then
                    GUI_ShowMessageBox(MSG_MaxAirFlowAlertOfRange, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                    ' der Maximumwert ist kleiner als der Minimumwert, kann nicht sein...
                End If

            ElseIf value < _MinAirFlowAlert Then
                If (WasInitialized) Then
                    GUI_ShowMessageBox(MSG_MinHigherMaxAlertOfRange, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                End If
            Else
                _MaxAirFlowAlert = value
            End If
        End Set
    End Property

    <TypeConverter(GetType(ComportList)), _
    ml_Category(278, "03 SPS Parameters"), _
    ml_DisplayName(279, "Serial Port"), _
    ml_Description(280, "Serial port to communicate with the SPS."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(1000)> _
    Public Property SPSCom As String = "COM1"



    <TypeConverter(GetType(TypeOfCommunication)), _
    ml_Category(278, "03 SPS Parameters"), _
    ml_DisplayName(544, "Type of communication"), _
    ml_Description(545, "Type of communication to communicate with the SPS (serial, network, ...)."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(1003)> _
    Public Property SPSConnectionType() As String
        Get
            Return _SPSConnectionType
        End Get
        Set(ByVal value As String)
            If Not _Start And Not value.Equals(_SPSConnectionType) Then
                GUI_ShowMessageBox(MSG_ProgRestart, "Ok", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            End If

            _SPSConnectionType = value
        End Set
    End Property

    <ml_Category(278, "03 SPS Parameters"), _
    ml_DisplayName(546, "Network address for TCP/IP"), _
    ml_Description(547, "Network address for TCP/IP of SPS."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(1004)> _
    Public Property SPSIpNetworkAddress() As String
        Get
            Return _SPSIpNetworkAddress
        End Get
        Set(ByVal value As String)
            If Not value Is Nothing Then
                Try
                    Dim address As IPAddress = IPAddress.Parse(value)
                    _SPSIpNetworkAddress = value
                Catch ex As Exception
                    GUI_ShowMessageBox(MSG_NoValidSPSIpNetworkAddress, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                End Try
            Else
                _SPSIpNetworkAddress = value
            End If
        End Set
    End Property

    <ml_Category(278, "03 SPS Parameters"), _
    ml_DisplayName(548, "Remote port for TCP/IP"), _
    ml_Description(549, "Remote port for TCP/IP of SPS (1 ... 65535)."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(1005)> _
    Public Property SPSIpNetworkPort As UInt16


    <ml_Category(281, "04 Alarm Parameters"), _
    ml_DisplayName(282, "Alarm nuclides"), _
    ml_Description(283, "List of nuclides and thresholds for alarming in Bq/m³."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(10000), _
    Editor(GetType(ThermoStringListEditor), GetType(Drawing.Design.UITypeEditor))> _
    Public Property AlarmNuclideConfigStrings As List(Of String) = New List(Of String)

    <TypeConverter(GetType(LanguageList)), _
    ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(382, "Language"), _
    ml_Description(383, "Language of the System"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100000)> _
    Public Property Language As String = "English"

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(387, "Display Points"), _
    ml_Description(388, "Display points in spectral display or lines."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100001)> _
    Public Property DisplayPoints As Boolean

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(512, "Capture cycle for detector temperature [min]"), _
    ml_Description(513, "Capture cycle for detector temperature in minutes."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100002)> _
    Public Property CaptureCycleDetectorTemperature() As Integer
        Get
            Return _CaptureCycleDetectorTemperature
        End Get
        Set(ByVal value As Integer)
            If value < 1 Or value > 60 Then
                _CaptureCycleDetectorTemperature = 10
                GUI_ShowMessageBox(MSG_CaptureCycleDetectorTemperatureOfRange, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _CaptureCycleDetectorTemperature = value
            End If
        End Set
    End Property

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(514, "Enable Emergency Stop Detect (controller)"), _
    ml_Description(515, "Enable Emergency Stop Detect for E-Cooler."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100003)> _
    Public Property EnableEmergencyStopDetect() As Boolean
        Get
            Return _EnableEmergencyStopDetect
        End Get
        Set(ByVal value As Boolean)
            If Not _EnableCapturingDetectorTemperature And value Then
                _EnableEmergencyStopDetect = False
                GUI_ShowMessageBox(MSG_EmergencyStopDetectOnlyWithTemperaruteDetection, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _EnableEmergencyStopDetect = value
            End If
        End Set
    End Property

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(517, "Enable capturing of detector temperature"), _
    ml_Description(518, "Enable capturing of detector temperature (instead of measurement of N2 used for cooling), show detector temperature diagram"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100004)> _
    Public Property EnableCapturingDetectorTemperature() As Boolean
        Get
            Return _EnableCapturingDetectorTemperature
        End Get
        Set(ByVal value As Boolean)
            _EnableCapturingDetectorTemperature = value
        End Set
    End Property

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(524, "Temperature Threshold 'Crystal too warm'"), _
    ml_Description(525, "Temperature Threshold 'Crystal too warm' in °C"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100005)> _
    Public Property CrystalTooWarmTempThreshold() As Integer
        Get
            Return _CrystalTooWarmTempThreshold
        End Get
        Set(ByVal value As Integer)
            If value < -155 Or value > -145 Then
                _CrystalTooWarmTempThreshold = -150
                GUI_ShowMessageBox(MSG_WrongCrystalTooWarmTempThreshold, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _CrystalTooWarmTempThreshold = value
            End If
        End Set
    End Property

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(527, "Temperature Threshold 'Crystal warmed up'"), _
    ml_Description(528, "Temperature Threshold 'Crystal warmed up' in °C"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(100006)> _
    Public Property CrystalWarmedUpTempThreshold() As Integer
        Get
            Return _CrystalWarmedUpTempThreshold
        End Get
        Set(ByVal value As Integer)
            If value < 0 Or value > 25 Then
                _CrystalWarmedUpTempThreshold = 15
                GUI_ShowMessageBox(MSG_WrongCrystalWarmedUpTempThreshold, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                _CrystalWarmedUpTempThreshold = value
            End If
        End Set
    End Property

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(539, "Turn off high voltage on exit"), _
    ml_Description(540, "Turn off high voltage (MCA) on exit of monitor application"), _
    [ReadOnly](False), _
    Browsable(False), _
    PropertyOrderAttribute(100007)> _
    Public Property KeepActiveHighVoltageOnExitGuiFlag As Boolean = False

    <ml_Category(381, "05 Miscellaneous Parameters"), _
    ml_DisplayName(562, "Turn off eCooler on exit"), _
    ml_Description(563, "Turn off eCooler (SPS) on exit of monitor application"), _
    [ReadOnly](False), _
    Browsable(False), _
    PropertyOrderAttribute(100008)> _
    Public Property KeepActiveEcoolerOnExitGuiFlag As Boolean = False


    <ml_Category(381, "05 Miscellaneous Parameters"),
    ml_DisplayName(661, "TCS Configuration file"),
    ml_Description(662, "Full path of the csv file with the TCS correction."),
    [ReadOnly](False),
    Browsable(True),
    PropertyOrderAttribute(100009)>
    Public Property TCS_CorrFile() As String
        Get
            Return _TCS_CorrFile
        End Get
        Set(value As String)
            If _TCS_CorrFile <> "Startup." Then
                GUI_ShowMessageBox("Please restart for the changes to take effect.", "Ok", "", "", MYCOL_WARNING, Color.Black)
            End If
            _TCS_CorrFile = value
        End Set
    End Property


    <TypeConverter(GetType(ComportList)), _
    ml_Category(580, "06 PaperrollPrinter Parameters"), _
    ml_DisplayName(279, "Serial Port"), _
    ml_Description(581, "Serial port to communicate with the printer."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(110001)> _
    Public Property PrinterSerialPort As String = "COM2"

    <ml_Category(580, "06 PaperrollPrinter Parameters"), _
    ml_DisplayName(582, "Printer Distance"), _
    ml_Description(583, "Printer distance between printer and the center of the detection ring."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(110002)> _
    Public Property PrinterPositionToMeasureCenterDistanceMM() As Integer
        Get
            Return _PrinterPositionToMeasureCenterDistanceMM
        End Get
        Set(ByVal value As Integer)
            _PrinterPositionToMeasureCenterDistanceMM = value
        End Set
    End Property

    <ml_Category(580, "06 PaperrollPrinter Parameters"), _
    ml_DisplayName(584, "Dustation Diameter"), _
    ml_Description(585, "Dustation Diameter in mm."), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(110003)> _
    Public Property DustationDiameterMM As Integer

    <ml_Category(580, "06 PaperrollPrinter Parameters"), _
    ml_DisplayName(586, "Enable Paperroll Printer"), _
    ml_Description(587, "Enable Paperroll Printer (timestamp on paperroll)"), _
    [ReadOnly](False), _
    Browsable(True), _
    PropertyOrderAttribute(110004)> _
    Public Property EnablePaperrollPrinter As Boolean = False


    ''' <summary>
    ''' This is in internal system parameter that shall not be editable by user in the user interface 
    ''' (as too low level)
    ''' </summary>
    ''' <value></value>
    ''' <returns>True if we shall use STX-ETX-BC(ACK-NAK) protocol with two/three-letter commands, 
    ''' False if we shall use un-secured protocol with old one-letter commands</returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public Property SPSUseSTXEXTProtocol() As String
        Get
            Return _SPSUseSTXEXTProtocol
        End Get
        Set(ByVal value As String)
            _SPSUseSTXEXTProtocol = value
        End Set
    End Property

    ''' <summary>
    ''' This is in internal system parameter that shall not be editable by user in the user interface 
    ''' (as too low level)
    ''' </summary>
    ''' <value></value>
    ''' <returns>True if we shall log the SPS checksum error (SPS delivers NAK), 
    ''' False if we shall not output it in the event log window in the user interface</returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public Property SPSOutputChecksumError() As String
        Get
            Return _SPSOutputChecksumError
        End Get
        Set(ByVal value As String)
            _SPSOutputChecksumError = value
        End Set
    End Property



    <Browsable(False)> _
    Private Const _PrinterPositionToMeasureCenterDistanceMmStandard As Integer = 180

    <Browsable(False)> _
    Public ReadOnly Property PrinterPositionOffsetMM As Integer
        Get
            Return _PrinterPositionToMeasureCenterDistanceMM - _PrinterPositionToMeasureCenterDistanceMmStandard
        End Get
    End Property


    <Browsable(False)> _
    Public ReadOnly Property EcoolerEnabled() As Boolean
        Get
            If _N2FillThreshold = 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property


    <Browsable(False)> _
    Public Property AlarmModeSettings As AlarmModeSettings = AlarmModeSettings.Normal

    <Browsable(False)> _
    Public Property IntensiveModeEnabled As Boolean = True

    <Browsable(False)> _
    Public Property CalibrationType As CalibrationTypeEnum

    <Browsable(False)> _
    Public Property AirFlowThroughPutCalculationMode As AirFlowThroughPutCalculationModeEnum

    <Browsable(False)> _
    Public Property ZeroDegreesVoltage As Double = 499.33

    <Browsable(False)> _
    Public Property ZeroVoltsTemperature As Double = -246.98

    <Browsable(False)> _
    Public Property IpAddressOfUpsSnmp As String

    <Browsable(False)> _
    Public Property PortOfUpsSnmp As Int16

    <Browsable(False)> _
    Public Property SnmpCommunity As String

    <Browsable(False)> _
    Public Property IsUpsAvailable As Boolean

    <Browsable(False)> _
    Public Property GenerateAnsiN4242 As Boolean

    <Browsable(False)> _
    Public Property AnsiN4242Settings As AnsiN4242Settings = AnsiN4242Settings.Normal

    <Browsable(False)> _
    Public Property GenerateMonthlySystemMessagesFiles As Boolean

    <Browsable(False)> _
    Public Property CertificateDirectory As String

    <Browsable(False)> _
    Public Property MonitorConfigDirectory As String

    <Browsable(False)> _
    Public Property NuclideLibsDirectory As String

    <Browsable(False)> _
    Public Property SpectraDirectory As String

    <Browsable(False)> _
    Public Property AnalyzationFilesDirectory As String

    <Browsable(False)> _
    Public Property AnalyzationN4242FilesDirectory As String

    <Browsable(False)> _
    Public Property AlarmfilesDirectory As String

    <Browsable(False)> _
    Public Property ReportFilesDirectory As String

    <Browsable(False)> _
    Public Property SensorRecordingDirectory As String

    <Browsable(False)> _
    Public Property LogfilesDirectory As String

    'Nur temporär zum Untersuchen eines Fehlers (Piepen beim Setzen von Gain/Stabi)
    <Browsable(False)> _
    Public Property LynxCommandsDelayTime As Integer

    'Nur temporär zum Untersuchen eines Fehlers (Piepen beim Setzen von Gain/Stabi)
    <Browsable(False)> _
    Public Property LynxCommandsShowMessageBox As Boolean

    ''' <summary>
    ''' notwendig um festzustellen ob GUI-Prüfungen erlaubt sind
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Property WasInitialized As Boolean = False

    ''' <summary>
    ''' Kopiere mich selbst an eine andere Instanz
    ''' WICHITIG: Über das SetValue geht das ganze nur für Variablen die nicht ReadOnly sind!
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CopyMe() As FHT59N3_SystemParams
        Try
            Return CType(MemberwiseClone(), FHT59N3_SystemParams)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return Nothing
        End Try
    End Function

End Class

<FlagsAttribute()> _
Public Enum AnsiN4242Settings
    'Dieser Eintrag muss so bleiben, da wegen Automatismus der erste genommen wird wenn kein Konfigwert gesetzt wurde!
    Normal = 0
    'zeigt auch Nuklide aus der Nuklidliste an deren Konzentration=0 war
    ShowEmptyAnalyzedNuclids = 1
    'generiert blankes XML statt xml.gz-Dateien
    GeneratePlainXml = 2
End Enum

<FlagsAttribute()> _
Public Enum AlarmModeSettings
    'Dieser Eintrag muss so bleiben, da wegen Automatismus der erste genommen wird wenn kein Konfigwert gesetzt wurde!
    Normal = 0
    'verhindern ein Ändern der Filterstandzeit
    DontChangeFilterTime = 1
End Enum
