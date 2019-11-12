#Region "Imports"

Imports System.Threading
Imports CanberraDeviceAccessLib.ParamCodes
Imports CanberraDeviceAccessLib.ConnectOptions
Imports CanberraDeviceAccessLib.DeviceStatus
Imports CanberraDeviceAccessLib.AcquisitionModes

#End Region

Public Class FHT59N3_LynxCommunication

#Region "DLL"

    'Call this function to set up the Genie2000 environment
    'Lynx Environment
    Declare Function bG2KEnv Lib "G2K_VB" () As Integer
    Declare Function bG2KSetEnv Lib "G2K_VB" (ByVal lpName As String, ByVal lpBuffer As String) As Integer
    'End Lynx Environment

#End Region

#Region "Private Mitglieder"

    Private _MyDetector As New CanberraDeviceAccessLib.DeviceAccess
    Private _MyDetectorState As New CanberraDeviceAccessLib.DeviceStatus
    Private _Measurement As Boolean = False
    Private _Lynx As Canberra.DSA3K.DataTypes.Communications.IDevice

    Private _MeasurementTimeMin As Integer
    Private _MeasurementTimeSec As Integer
    Private _CountToRealTime As Boolean
    Private _WithAir As Boolean
    Private _IPLynx As String
    Private _SimulateLynxSystem As Boolean

    'Delegate für alle Lynx-Befehle
    Public Property BeforeLynxCommandSub As Action(Of String) = Sub()
                                                                End Sub


#End Region

#Region "Öffentliche Eigenschaften"

    Public ReadOnly Property MyDetector() As CanberraDeviceAccessLib.DeviceAccess
        Get
            Return _MyDetector
        End Get
    End Property

    Public ReadOnly Property MyDetectorStatus() As CanberraDeviceAccessLib.DeviceStatus
        Get
            If _SimulateLynxSystem Then
                Return CanberraDeviceAccessLib.DeviceStatus.aAcquireDone
            Else
                Return _MyDetector.AnalyzerStatus
            End If
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MyDetectorRealMeasTime() As Integer
        Get
            If _SimulateLynxSystem Then
                Return 1000
            Else
                Return _MyDetector.Param(CAM_X_EREAL)
            End If
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MyDetectorLiveMeasTime() As Integer
        Get
            If _SimulateLynxSystem Then
                Return 10
            Else
                Return _MyDetector.Param(CAM_X_ELIVE)
            End If
        End Get
    End Property

#End Region

#Region "Öffentliche Methoden"

#Region "Allgemein"

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="IPLynx"></param>
    ''' <param name="SimulateLynxSystem"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal IPLynx As String, ByVal SimulateLynxSystem As Boolean)
        Try
            _IPLynx = IPLynx
            _SimulateLynxSystem = SimulateLynxSystem
            Me.ConnectDetector()

            _Lynx = Canberra.Lynx.Examples.Utilities.GetDeviceInterface
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Dispose()
        Try
            If Not _MyDetector Is Nothing AndAlso _MyDetector.IsConnected Then
                _MyDetector.AcquireStop()
                _MyDetector.Disconnect()

                _MyDetector = Nothing
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Detector"

    Public Sub ClearDetector()
        Try
            BeforeLynxCommandSub.Invoke("ClearDetector")
            _MyDetector.Clear()
            'Trace.TraceInformation(Now.ToShortDateString & " " & Now.ToLongTimeString & ": ClearDetector: Detector cleared")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetDetectorParams(ByVal Customer As String, ByVal StationName As String, ByVal StationID As String)
        Try
            If _MyDetector.IsConnected Then
                _MyDetector.Param(CAM_T_SDESC1) = Customer        'aus CAM_CLS_SAMP
                _MyDetector.Param(CAM_T_SLOCTN) = StationName
                _MyDetector.Param(CAM_T_SIDENT) = StationID
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub ConnectDetector()
        Try

            If Not _SimulateLynxSystem Then

                BeforeLynxCommandSub.Invoke("ConnectDetector: Connect to EBIN")

                _MyDetector.Connect("EBIN", aReadWrite Or aTakeOver Or aTakeControl)


                _MyDetectorState = _MyDetector.AnalyzerStatus
                If ((_MyDetectorState And aAcquireDone) <> aAcquireDone) Then
                    BeforeLynxCommandSub.Invoke("ConnectDetector: AcquireStop")
                    _MyDetector.AcquireStop()
                End If
                BeforeLynxCommandSub.Invoke("ConnectDetector: Detector.Clear")
                _MyDetector.Clear()
                'Trace.TraceInformation(Now.ToShortDateString & " " & Now.ToLongTimeString & ": ConnectDetector: Detector cleared")

                BeforeLynxCommandSub.Invoke("ConnectDetector: Set CAM_T_STYPE,CAM_F_THRESHOLD....CAM_L_ASTFBADCAL")
                _MyDetector.Param(CAM_T_STYPE) = ""
                _MyDetector.Param(CAM_F_THRESHOLD) = 1.5
                _MyDetector.Param(CAM_F_SENSITVTY) = 2.5
                _MyDetector.Param(CAM_L_PEAKSTART) = 75                'aus CAM_CLS_PROC
                _MyDetector.Param(CAM_L_PEAKEND) = 4095
                _MyDetector.Param(CAM_L_PASTART) = 75
                _MyDetector.Param(CAM_L_PAEND) = 4095
                _MyDetector.Param(CAM_F_SQUANT) = 1
                _MyDetector.Param(CAM_F_TOLERANCE) = 1
                _MyDetector.Param(CAM_F_CONFID) = 0.1
                _MyDetector.Param(CAM_F_MDACONFID) = 5
                _MyDetector.Param(CAM_L_NSIGMA) = 1
                _MyDetector.Param(CAM_L_ASTFBADCAL) = 0
                If _Measurement Then
                    StartMeasurement(_MeasurementTimeMin, _CountToRealTime, _WithAir)
                End If

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub DisconnectFromDetector()
        Try

            If _MyDetector.IsConnected Then

                If _Measurement Then
                    BeforeLynxCommandSub.Invoke("DisconnectFromDetector: AcquireStop")
                    _MyDetector.AcquireStop()
                    _MyDetector.Param(CAM_X_EREAL) = 0

                    ClearDetector()
                End If

                BeforeLynxCommandSub.Invoke("DisconnectFromDetector: Disconnect")
                _MyDetector.Disconnect()
                Trace.TraceInformation("DisconnectFromDetector: _MyDetector.IsConnected.ToString=" & _MyDetector.IsConnected.ToString)
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "HV"

    Public Sub SetHVParams(ByVal Range As Integer, ByVal Limit As Integer, ByVal Voltage As Integer, ByVal DetectorPolarity As Integer, ByVal InhibitPolarity As Integer)
        Try
            'die ganzen try/catch Anweisungen sind leider notwendig
            'sonst bekommt man immer eine Fehlermeldung: Gerät ist mit dem ändern von Parametern beschäftigt
            'trotz dieser meldung wird aber der Befehl ausgeführt -> also einfach wegbügeln!

            BeforeLynxCommandSub.Invoke("SetHVParams")

            If _MyDetector.IsConnected Then
                Dim ActHVMode As Boolean = False
                Try
                    ActHVMode = _MyDetector.HighVoltage.On
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set HighVoltage On=False")
                    _MyDetector.HighVoltage.On = False
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set HighVoltage Range")
                    _MyDetector.HighVoltage.Range = Range
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set HighVoltage Limit")
                    _MyDetector.HighVoltage.Limit = Limit
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set HighVoltage Voltage")
                    _MyDetector.HighVoltage.Voltage = Voltage
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set CAM_L_HVPSFPOL")
                    _MyDetector.Param(CAM_L_HVPSFPOL) = DetectorPolarity 'Outputpolarity: 0 = positive, 1 = negative
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set CAM_L_HVPSFLVINH")
                    _MyDetector.Param(CAM_L_HVPSFLVINH) = InhibitPolarity 'Inhibitpolarity: 0 = positive, 1 = negative
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

                Try
                    BeforeLynxCommandSub.Invoke("SetHVParams: set HighVoltage On=Lastsetting")
                    _MyDetector.HighVoltage.On = ActHVMode
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetHVOn()
        Try

            If _MyDetector.IsConnected Then
                _MyDetector.HighVoltage.On = True 'wird trotz Fehlermeldung das das Gerät beschäftigt ist durchgeführt
            End If


        Catch ex As Exception
        End Try
    End Sub

    Public Sub SetHVOff()
        Try
            If _MyDetector.IsConnected Then
                _MyDetector.HighVoltage.On = False 'wird trotz Fehlermeldung das das Gerät beschäftigt ist durchgeführt
            End If
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Gets the state of "high voltage" from detector via LXNX software
    ''' </summary>
    ''' <returns>true: HV is activated, false: HV is off</returns>
    ''' <remarks>Can raise exception, when LYNX (detector software) is not available:
    ''' Sie haben keine Verbindung zu einem Detektor oder es besteht Fehler bei der Speicherreservierung</remarks>
    Public Function GetHVState() As Boolean
        Try
            Dim isHvOn As Boolean = True

            If _MyDetector.IsConnected Then
                isHvOn = _MyDetector.HighVoltage.On
            End If

            Return isHvOn

        Catch ecom As System.Runtime.InteropServices.COMException
            Return True  ' TFHN-11: assume that HV is on when LYNX is not available
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function GetHVInhibitState() As Boolean
        Try
            Return _MyDetector.HighVoltage.InhibitOn
        Catch ex As Exception
        End Try
    End Function

#End Region

#Region "Amplifier"

    Public Sub SetAutoPoleZero()
        Try
            _MyDetector.Amplifier.AutoPoleZero = True
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetPoleZeroValue() As Integer
        Try
            Return CInt(_MyDetector.Param(CAM_L_AMPPZ))
        Catch ex As Exception
        End Try
    End Function

    Public Function GetPoleZeroBusyState() As Boolean
        Try
            Return _MyDetector.Amplifier.PoleZeroBusy
        Catch ex As Exception
        End Try
    End Function

    Public Sub SetAmplifierParams(ByVal InputPolarity As Integer, ByVal CoarseGain As Double, ByVal FineGain As Double, ByVal PoleZero As Integer, ByVal BLRMode As Integer, ByVal FilterRiseTime As Double, ByVal FilterFlatTop As Double)
        Try
            If _MyDetector.IsConnected Then

                Dim ActStabMode As String = _MyDetector.Param(CAM_T_DSSWIN2MODE)
                BeforeLynxCommandSub.Invoke("SetAmplifierParams: Setting CAM_T_DSSWIN2MODE,CAM_T_PRAMPTYPE")
                _MyDetector.Param(CAM_T_DSSWIN2MODE) = "OFF"
                _MyDetector.Param(CAM_T_PRAMPTYPE) = "RC"

                BeforeLynxCommandSub.Invoke("SetAmplifierParams: Setting NegativePolarity")
                If InputPolarity = 0 Then
                    _MyDetector.Amplifier.NegativePolarity = False
                Else
                    _MyDetector.Amplifier.NegativePolarity = True
                End If

                BeforeLynxCommandSub.Invoke("SetAmplifierParams: Setting CAM_F_AMPHWGAIN1,CAM_F_AMPHWGAIN2, AutoPoleZero, CAM_L_AMPPZ(Pole zero)")
                _MyDetector.Param(CAM_F_AMPHWGAIN1) = CoarseGain
                _MyDetector.Param(CAM_F_AMPHWGAIN2) = FineGain
                _MyDetector.Amplifier.AutoPoleZero = False
                _MyDetector.Param(CAM_L_AMPPZ) = PoleZero

                BeforeLynxCommandSub.Invoke("SetAmplifierParams: Setting CAM_T_AMPBLRTYPE")
                Select Case BLRMode
                    Case 0
                        _MyDetector.Param(CAM_T_AMPBLRTYPE) = "Auto"
                    Case 1
                        _MyDetector.Param(CAM_T_AMPBLRTYPE) = "Hard"
                    Case 2
                        _MyDetector.Param(CAM_T_AMPBLRTYPE) = "Medium"
                    Case 3
                        _MyDetector.Param(CAM_T_AMPBLRTYPE) = "Soft"
                End Select

                BeforeLynxCommandSub.Invoke("SetAmplifierParams: Setting CAM_F_AMPFILTERRT, CAM_F_AMPFILTERFT, CAM_T_DSSWIN2MODE")
                _MyDetector.Param(CAM_F_AMPFILTERRT) = FilterRiseTime
                _MyDetector.Param(CAM_F_AMPFILTERFT) = FilterFlatTop
                _MyDetector.Param(CAM_T_DSSWIN2MODE) = ActStabMode

            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "ADC"

    Public Sub SetADCParams(ByVal AcquisitionMode As Integer, ByVal LLDMode As Integer, ByVal LLD As Double, ByVal ULD As Double, ByVal ConversionGain As Integer)
        Try

            BeforeLynxCommandSub.Invoke("SetADCParams...")

            '_MyDetector.Param(CAM_T_ADCACQMODE) = "PHA"
            'Aquisition Mode lässt sich via Com nicht einstellen
            'Werte werden nicht übernommen
            DisconnectFromDetector()

            If _SimulateLynxSystem Then
                Return
            End If

            _Lynx.Open(Canberra.Lynx.Examples.Utilities.GetLocalAddress, _IPLynx)
            Try 'Lock geht manchmal schief
                BeforeLynxCommandSub.Invoke("SetADCParams: LockLynx (administrator,password)")
                _Lynx.Lock("administrator", "password", 1)
                Select Case AcquisitionMode
                    'weiß noch nicht ob das so passt. Werte wurde per Umstellung im Webinterface herausgefunden
                    'komischerweise wird der Wert im Webinterface nicht richtig angezeigt
                    Case 0
                        '_MyDetector.Param(CAM_T_ACQMOD) = "PHA+"
                        BeforeLynxCommandSub.Invoke("SetADCParams: Input_Mode=PHA+")
                        _Lynx.PutParameter(Canberra.DSA3K.DataTypes.ParameterCodes.Input_Mode, Canberra.DSA3K.DataTypes.ParameterValueTypes.InputModes.Pha, 1)
                    Case 1
                        '_MyDetector.Param(CAM_T_ACQMOD) = "MSS"
                        BeforeLynxCommandSub.Invoke("SetADCParams: Input_Mode=MSS")
                        _Lynx.PutParameter(Canberra.DSA3K.DataTypes.ParameterCodes.Input_Mode, Canberra.DSA3K.DataTypes.ParameterValueTypes.InputModes.Mss, 1)
                    Case 2
                        '_MyDetector.Param(CAM_T_ACQMOD) = "LFC+"
                        BeforeLynxCommandSub.Invoke("SetADCParams: Input_Mode=LFC+")
                        _Lynx.PutParameter(Canberra.DSA3K.DataTypes.ParameterCodes.Input_Mode, Canberra.DSA3K.DataTypes.ParameterValueTypes.InputModes.Dlfc, 1)
                    Case 3
                        '_MyDetector.Param(CAM_T_ACQMOD) = "LIST"
                        BeforeLynxCommandSub.Invoke("SetADCParams: Input_Mode=LIST")
                        _Lynx.PutParameter(Canberra.DSA3K.DataTypes.ParameterCodes.Input_Mode, Canberra.DSA3K.DataTypes.ParameterValueTypes.InputModes.List, 1)
                    Case 4
                        '_MyDetector.Param(CAM_T_ACQMOD) = "TLIS"
                        BeforeLynxCommandSub.Invoke("SetADCParams: Input_Mode=TLIS")
                        _Lynx.PutParameter(Canberra.DSA3K.DataTypes.ParameterCodes.Input_Mode, Canberra.DSA3K.DataTypes.ParameterValueTypes.InputModes.Tlist, 1)
                End Select
                BeforeLynxCommandSub.Invoke("SetADCParams: UnlockLynx")
                _Lynx.Unlock("administrator", "password", 1)
            Catch ex As Exception
                Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            End Try

            BeforeLynxCommandSub.Invoke("SetADCParams: Reconnect to lynx (set CAM_L_LLDAUTOMODE)")
            _Lynx.Close()
            ConnectDetector()
            _MyDetector.ADC.PeakDetectMode = CanberraDeviceAccessLib.PeakDetectModes.aAuto
            _MyDetector.Param(CAM_L_LLDAUTOMODE) = LLDMode
            _MyDetector.ADC.LLD = LLD
            _MyDetector.ADC.ULD = ULD
            _MyDetector.ADC.ConversionGain = ConversionGain
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Stabilizer"

    Public Sub SetStabilizerParameters(ByVal Centroid As Integer, ByVal Window As Integer, ByVal Spacing As Integer, ByVal Multiplier As Integer, ByVal WindowRatio As Double, ByVal UseNaI As Integer, ByVal GainRatioAutoMode As Integer)
        Try
            BeforeLynxCommandSub.Invoke("SetStabilizerParameters...")

            If _MyDetector.IsConnected Then

                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_L_STABWIN2C")
                _MyDetector.Param(CAM_L_STABWIN2C) = Centroid
                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_L_STABWIN2R")
                _MyDetector.Param(CAM_L_STABWIN2R) = Window
                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_L_STABWIN2S")
                _MyDetector.Param(CAM_L_STABWIN2S) = Spacing
                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_L_DSSEVMULT2")
                _MyDetector.Param(CAM_L_DSSEVMULT2) = Multiplier
                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_F_DSSGAINRATIO")
                _MyDetector.Param(CAM_F_DSSGAINRATIO) = WindowRatio
                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_L_DSSFRNGWIN2")
                _MyDetector.Param(CAM_L_DSSFRNGWIN2) = UseNaI
                BeforeLynxCommandSub.Invoke("SetStabilizerParameters: CAM_L_DSSFGRATMODE")
                _MyDetector.Param(CAM_L_DSSFGRATMODE) = Not GainRatioAutoMode 'komischerweise ist das ganze hier invertiert?
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetStabilizerMode(ByVal StabMode As Integer)
        Try
            BeforeLynxCommandSub.Invoke("SetStabilizerMode: Set CAM_T_DSSWIN2MODE")
            If _MyDetector.IsConnected Then
                Select Case StabMode
                    Case 0
                        _MyDetector.Param(CAM_T_DSSWIN2MODE) = "OFF"
                    Case 1
                        _MyDetector.Param(CAM_T_DSSWIN2MODE) = "ON"
                    Case 2
                        _MyDetector.Param(CAM_T_DSSWIN2MODE) = "HOLD"
                End Select
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Measurement"

    Public Sub StartMeasurement(ByVal MeasurementTimeMin As Integer, ByVal CountToRealTime As Boolean, ByVal WithAir As Boolean)
        Try
            _MeasurementTimeMin = MeasurementTimeMin
            _MeasurementTimeSec = CInt(_MeasurementTimeMin * 60)  'Messdauer [s]
            _CountToRealTime = CountToRealTime
            _WithAir = WithAir

            BeforeLynxCommandSub.Invoke("StartMeasurement: Detector.Clear")

            _MyDetector.Clear()
            'Trace.TraceInformation(Now.ToShortDateString & " " & Now.ToLongTimeString & ": StartMeasurement: Detector cleared")
            BeforeLynxCommandSub.Invoke("StartMeasurement: Set CAM_T_SSPRSTR3,CAM_T_ASPSTR and CAM_F_ASP2-CAM_F_ASP4")
            _MyDetector.Param(CAM_T_SSPRSTR3) = Microsoft.VisualBasic.Format(Now, "dd.MM.yy HH:mm") 'Messstart für stdS
            _MyDetector.Param(CAM_T_ASPSTR) = Microsoft.VisualBasic.Format(Now, "dd.MM.yy HH:mm")
            _MyDetector.Param(CAM_F_ASP2) = 0           'Luftdurchsaz
            _MyDetector.Param(CAM_F_ASP3) = 0           'Anzal der Widerstarte
            If WithAir Then
                _MyDetector.Param(CAM_F_ASP4) = 1   'Bestaubungsspektrum
            Else
                _MyDetector.Param(CAM_F_ASP4) = 0   'kein Bestaubungsspektrum
            End If

            If _CountToRealTime Then
                BeforeLynxCommandSub.Invoke("StartMeasurement: SpectroscopyAcquireSetup (for Real time)")
                _MyDetector.SpectroscopyAcquireSetup(aCountToRealTime, _MeasurementTimeSec)
            Else
                BeforeLynxCommandSub.Invoke("StartMeasurement: SpectroscopyAcquireSetup (for Live time)")
                _MyDetector.SpectroscopyAcquireSetup(aCountToLiveTime, _MeasurementTimeSec)
            End If

            BeforeLynxCommandSub.Invoke("StartMeasurement: AcquireStart")
            _MyDetector.AcquireStart()           'Messung starten
            _Measurement = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub ReStartMeasurement(Optional ByVal MeasurementTimeMin As Integer = 0)
        Try
            _MeasurementTimeMin = MeasurementTimeMin
            If _MeasurementTimeMin > 0 Then
                _MeasurementTimeSec = CInt(_MeasurementTimeMin * 60)  'Messdauer [s]
                If _CountToRealTime Then 'real oder livetime von startmeas.. übernehmen
                    _MyDetector.SpectroscopyAcquireSetup(aCountToRealTime, _MeasurementTimeSec)
                Else
                    _MyDetector.SpectroscopyAcquireSetup(aCountToLiveTime, _MeasurementTimeSec)
                End If
            End If
            _MyDetector.AcquireStart()           'Messung starten
            _Measurement = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub StopMeasurement(ByVal Pause As Boolean)
        Try

            If _MyDetector.IsConnected Then

                If Pause Then
                    _MyDetector.AcquirePause()
                Else
                    BeforeLynxCommandSub.Invoke("StopMeasurement: AcquireStop")
                    _MyDetector.AcquireStop()
                    _MyDetector.Param(CAM_X_EREAL) = 0
                    Me.ClearDetector()

                    _Measurement = False
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetParamsForActualSpectrum(ByVal SUNITS As String, ByVal ASP2 As Double, ByVal ASP3 As Integer, ByVal ASP4 As Integer, ByVal SSPRSTR4 As String)
        Try
            _MyDetector.Param(CAM_T_SUNITS) = SUNITS
            _MyDetector.Param(CAM_F_ASP2) = ASP2      'gemitt.Luftdurchsaz
            _MyDetector.Param(CAM_F_ASP3) = ASP3            'Anzal addirter Spektren
            _MyDetector.Param(CAM_F_ASP4) = ASP4            'qasi stdS
            _MyDetector.Param(CAM_T_SSPRSTR4) = SSPRSTR4
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GetParamsFromActualSpectrum(ByRef ASPSTR As String, ByRef SSPRSTR3 As String)
        Try
            ASPSTR = _MyDetector.Param(CAM_T_ASPSTR)
            SSPRSTR3 = _MyDetector.Param(CAM_T_SSPRSTR3)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SaveSpectrum(ByVal FileName As String, ByVal OverWrite As Boolean)
        Try
            _MyDetector.Save(FileName, OverWrite)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetECSlope(ByVal ECSLOPE As Double)
        Try
            _MyDetector.Param(CAM_F_ECSLOPE) = ECSLOPE
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetECOffset(ByVal ECOFFSET As Double)
        Try
            _MyDetector.Param(CAM_F_ECOFFSET) = ECOFFSET
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetECQuad(ByVal ECQUAD As Double)
        Try
            _MyDetector.Param(CAM_F_ECQUAD) = ECQUAD
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Genie2000Environment"

    '-------------------------------------------------------
    'GetEnvironmentVar - Get an environment setting
    '
    'Arguments :
    '   strName     String identifying enviroment setting("CAMFILES")
    '
    'Returns :
    '   The environment setting
    '-------------------------------------------------------
    Public Function GetEnvironmentVar(ByVal strName As String) As String
        Try
            Dim strEnv As String = ""

            strEnv = Environment.GetEnvironmentVariable(strName)
            If (Not strEnv Is Nothing) Then
                GetEnvironmentVar = strEnv
            Else
                Return ""
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return ""
        End Try
    End Function

    '-------------------------------------------------------
    'SetEnvironmentVar - Set an environment setting
    '
    'Arguments :
    '   strName     String identifying enviroment, e.g. setting("CAMFILES")
    '   strVal      The environment setting
    '
    'Returns :
    '   True on success
    '-------------------------------------------------------
    Public Function SetEnvironmentVar(ByVal strName As String, ByVal strVal As String) As Boolean
        Try
            SetEnvironmentVar = bG2KSetEnv(strName, strVal)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    '-------------------------------------------------------
    'SetGenieEnvironment - Setup the genie environment settings
    '
    'Arguments :
    '   None
    '
    'Returns :
    '   True on success
    '-------------------------------------------------------
    Public Function SetGenieEnvironment() As Boolean
        Try
            SetGenieEnvironment = bG2KEnv()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

   

#End Region

#End Region

End Class
