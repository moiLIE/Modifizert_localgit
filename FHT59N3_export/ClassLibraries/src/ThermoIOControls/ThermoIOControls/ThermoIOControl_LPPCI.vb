'###################### Header #######################'
'# Firma:  Thermo Electron (Erlangen) GmbH			 #
'# Author: Thomas Kuschel, Wolfgang Sandner		     #	
'#####################################################'

Option Strict Off

#Region "Imports"

Imports ThermoInterfaces
Imports ThermoUtilities

#End Region

#Region "Schnittstelle"
''' <summary>
''' </summary>
''' <remarks></remarks>
Public Class ThermoIOControl_LPPCI
    Implements ThermoInterfaces.ThermoIOControls_Interface

#Region "MODLLL32_PCI.DLL"

    Declare Sub global8000Map Lib "MODDLL32_PCI.DLL" Alias "_global8000Map@0" ()
    Declare Function Init8000 Lib "MODDLL32_PCI.DLL" Alias "_Init8000@0" () As Integer
    Declare Function CloseDevice Lib "MODDLL32_PCI.DLL" Alias "_CloseDevice@0" () As Integer

    'aus DLLFUNC_PCI.c
    Declare Function Get_AnalogIn_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_AnalogIn_PCI@8" (ByVal ModulNo As Integer, ByVal InputNo As Integer) As Integer
    Declare Function Set_AnalogOut_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_AnalogOut_PCI@12" (ByVal ModulNo As Integer, ByVal OutputNo As Integer, ByVal Value As Integer) As Integer
    Declare Function Check8000DLLVersion_PCI Lib "MODDLL32_PCI.DLL" Alias "_Check8000DLLVersion_PCI@4" (ByVal VersionString As String) As Integer
    Declare Function CpuClock_long_PCI Lib "MODDLL32_PCI.DLL" Alias "_CpuClock_long_PCI@0" () As Integer
    Declare Function Get_cKey_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_cKey_PCI@0" () As Integer
    Declare Function Get_CounterStatus_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_CounterStatus_PCI@4" (ByRef pp As Byte) As Integer
    Declare Function Get_Counters_And_Common_Timer_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_Counters_And_Common_Timer_PCI@12" (ByRef stati As Byte, ByRef CounterVal As Integer, ByRef TimerVal As Integer) As Integer
    Declare Function Get_8000DLLVersion_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_8000DLLVersion_PCI@4" (ByVal VersionString As String) As Integer
    Declare Function Get_EEpromReg_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_EEpromReg_PCI@12" (ByVal ModulNo As Integer, ByVal RegNo As Integer, ByRef Value As Integer) As Integer
    Declare Function Get_HV_Values_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_HV_Values_PCI@16" (ByVal ModulNo As Integer, ByRef HVType As Integer, ByRef HVValue1 As Integer, ByRef HVValue2 As Integer) As Integer
    Declare Function Get_Digital_Inputs_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_Digital_Inputs_PCI@8" (ByRef InputValue As Long, ByRef TimeBaseFlag As Integer) As Integer
    Declare Function Get_LpVersion_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_LpVersion_PCI@4" (ByVal VersionString As String) As Integer
    Declare Function Get_ModulCodes_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_ModulCodes_PCI@4" (ByRef i As Short) As Integer
    Declare Function Get_Single_Counter_And_Timer_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_Single_Counter_And_Timer_PCI@16" (ByVal CounterNo As Integer, ByRef stati As Byte, ByRef Value As Integer, ByRef Timer As Integer) As Integer
    Declare Function Get_All_Counters_And_Timer_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_All_Counters_And_Timer_PCI@12" (ByRef stati As Byte, ByRef CounterVal As Integer, ByRef TimerVal As Integer) As Integer
    Declare Sub InitcKeys_PCI Lib "MODDLL32_PCI.DLL" Alias "_InitcKeys_PCI@0" ()
    Declare Function Set_ControlLogic_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_ControlLogic_PCI@4" (ByRef Command As Byte) As Integer
    Declare Function Get_ControlLogic_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_ControlLogic_PCI@4" (ByRef Info As Byte) As Integer
    Declare Function Set_Beep_Control_Get_PB_Chain_Interr_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_Beep_Control_Get_PB_Chain_Interr_PCI@4" (ByRef BeepInfo As Integer) As Integer
    Declare Function Set_5VSupply_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_5VSupply_PCI@12" (ByVal ModulNo As Integer, ByVal InputNo As Integer, ByVal SwitchValue As Integer) As Integer
    Declare Function Set_EEpromReg_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_EEpromReg_PCI@12" (ByVal ModulNo As Integer, ByVal RegNo As Integer, ByVal Value As Integer) As Integer
    Declare Function Set_Detector_Module_Settings_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_Detector_Module_Settings_PCI@28" (ByVal ModuleNo As Integer, ByVal Mask As Integer, ByRef Thresholds As Byte, ByVal InputSwitchFlag As Integer, ByVal NumEnabledOutputs As Integer, ByRef Gains As Byte, ByVal HVBit As Integer) As Integer
    Declare Function Get_Detector_Module_Settings_PCI Lib "MODDLL32_PCI.DLL" Alias "_Get_Detector_Module_Settings_PCI@8" (ByVal ModuleNo As Short, ByRef pp As Byte) As Integer
    Declare Function Set_Digital_Outputs_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_Digital_Outputs_PCI@8" (ByVal OutputMask As Integer, ByVal OutputValue As Integer) As Integer
    Declare Function Set_Hv_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_Hv_PCI@16" (ByVal ModulNo As Integer, ByVal maske As Integer, ByVal HVoltage1 As Integer, ByVal HVoltage2 As Integer) As Integer
    Declare Function Set_TimeBase_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_TimeBase_PCI@4" (ByVal Divisor As Integer) As Integer
    Declare Function Set_DeadTime_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_DeadTime_PCI@12" (ByVal ModulNo As Integer, ByVal InputNo As Integer, ByVal DeadTime As Long) As Integer
    Declare Function Set_HV770T_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_HV770T_PCI@8" (ByVal HVValue1 As Integer, ByVal HVValue2 As Integer) As Integer
    Declare Function SET_VLSTimeout_PCI Lib "MODDLL32_PCI.DLL" Alias "_SET_VLSTimeout_PCI@8" (ByVal LSNo As Integer, ByVal PresetTime As Long) As Integer
    Declare Function GET_VLS_PCI Lib "MODDLL32_PCI.DLL" Alias "_GET_VLS_PCI@12" (ByVal LSnr As Integer, ByRef plstime As Long, ByRef pCtrcnt As Integer) As Integer
    Declare Function StartCounter_PCI Lib "MODDLL32_PCI.DLL" Alias "_StartCounter_PCI@12" (ByVal Mask As Integer, ByVal PresetTime As Integer, ByVal AutostartFlag As Integer) As Integer
    Declare Function StopCounter_PCI Lib "MODDLL32_PCI.DLL" Alias "_StopCounter_PCI@4" (ByVal Mask As Long) As Integer
    Declare Sub ccc_PCI Lib "MODDLL32_PCI.DLL" Alias "_ccc_PCI@0" ()
    Declare Sub psoidomod_PCI Lib "MODDLL32_PCI.DLL" Alias "_psoidomod_PCI@4" (ByVal val As Integer)
    Declare Function ActivateWatchdog_PCI Lib "MODDLL32_PCI.DLL" Alias "_ActivateWatchdog_PCI@4" (ByVal TimeCode As Integer) As Integer
    Declare Function CloseWatchdog_PCI Lib "MODDLL32_PCI.DLL" Alias "_CloseWatchdog_PCI@4" (ByVal nHandle As Integer) As Integer
    Declare Function OpenWatchdog_PCI Lib "MODDLL32_PCI.DLL" Alias "_OpenWatchdog_PCI@4" (ByVal TimeCode As Integer) As Integer
    Declare Function PauseWatchdog_PCI Lib "MODDLL32_PCI.DLL" Alias "_PauseWatchdog_PCI@8" (ByVal nHandle As Integer, ByVal nPause As Integer) As Integer
    Declare Function TriggerWatchdog_PCI Lib "MODDLL32_PCI.DLL" Alias "_TriggerWatchdog_PCI@4" (ByVal nHandle As Integer) As Integer
    Declare Function DeactivateWatchdog_PCI Lib "MODDLL32_PCI.DLL" Alias "_DeactivateWatchdog_PCI@0" () As Integer
    Declare Function Set_TempCalib_PCI Lib "MODDLL32_PCI.DLL" Alias "_Set_TempCalib_PCI@8" (ByVal Cal_Value1 As Integer, ByVal Cal_Value1 As Integer) As Integer
    Declare Function SwitchLogs_PCI Lib "MODDLL32_PCI.DLL" Alias "_SwitchLogs_PCI@8" (ByVal Cal_Value1 As Integer, ByVal Cal_Value1 As Integer) As Integer
    Declare Function Service_PCI Lib "MODDLL32_PCI.DLL" Alias "_Service_PCI@8" (ByVal reset_val As Integer, ByRef service_val As Integer) As Integer
    Declare Sub delayinfo_PCI Lib "MODDLL32_PCI.DLL" Alias "_delayinfo_PCI@8" (ByVal min_val As Integer, ByVal max_val As Integer)

#End Region

#Region "Eigenschaften LPPCI"

    Private Const _IDLE As Integer = 0
    Private Const _BUSY As Integer = 1

    Private _DataContainer As ThermoDataContainer_Interface 'DatenContainer, ISReceiveReady
    Private WithEvents _Protocol As ThermoProtocol_Interface        'Irgendeine Klasse die dieses Interface implementiert, egal welche!, IsReceiveReady
    Private _MyState As Integer 'wo bin ich?
    Private _ReceiveBuffer(1000) As Integer 'eingangspuffer
    Private _ReceivedString As String   'empfangener String (im Stringmode)
    Private Shared _DataRead As Integer 'gelesene Datenmenge -> evtl. Probleme wegen shared
    Private _init8000_done As Boolean = False
    Private _MyUtilities As New ThermoUtilities.ThermoUtilities


#End Region

#Region "Öffentliche Eigenschaften"

    ''' <summary>
    ''' Empfangspuffer für die Daten von der seriellen Schnittstelle
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReceiveBuffer() As Array
        Get
            Dim Buffer(1000) As Integer
            _ReceiveBuffer.CopyTo(Buffer, 0)
            Return Buffer
        End Get
        Set(ByVal value As Array)
            value.CopyTo(_ReceiveBuffer, 0)
        End Set
    End Property

    Public Property Timeout() As Integer Implements ThermoInterfaces.ThermoIOControls_Interface.Timeout
        Get

        End Get
        Set(ByVal value As Integer)

        End Set
    End Property


    ''' <summary>
    ''' Event das Daten vom seriellen Port empfangen wurden (Byte Array)
    ''' </summary>
    ''' <remarks></remarks>
    Public Event DataReceivedEventAr() Implements ThermoInterfaces.ThermoIOControls_Interface.DataReceivedEventAr

    ''' <summary>
    ''' Event das Daten vom seriellen Port empfangen wurden (String)
    ''' </summary>
    ''' <remarks></remarks>
    Public Event DataReceivedEventStr() Implements ThermoInterfaces.ThermoIOControls_Interface.DataReceivedEventStr

    ''' <summary>
    ''' Timeout der Übertragung
    ''' </summary>
    ''' <remarks></remarks>
    Public Event TimeOutEvent() Implements ThermoInterfaces.ThermoIOControls_Interface.TimeOutEvent


    Public Event Disconnected() Implements ThermoInterfaces.ThermoIOControls_Interface.Disconnected

    ''' <summary>
    ''' Fehler
    ''' </summary>
    ''' <param name="ex">
    ''' Exception Klasse
    ''' </param>
    ''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As Exception) Implements ThermoInterfaces.ThermoIOControls_Interface.ErrorEvent

#End Region

#Region "Private Methoden"

    '''' <summary>
    '''' Fehlerbehandlung
    '''' </summary>
    '''' <param name="ex"></param>
    '''' <remarks></remarks>
    'Private Sub ErrorHandler(ByVal ex As Exception) Handles _Protocol.ErrorEvent
    '    RaiseEvent ErrorEvent(ex)
    'End Sub

    Function CtoVbString(ByVal CString As String) As String
        '------------------------------------------------------------------------------
        '  Beschreibung :Converts 0-terminated strings to Visual Basic Strings
        '
        '  Parameter: CString
        '
        '  Rückgabe : CString
        '
        '      Autor:  H.Schleicher
        '   geändert:
        '------------------------------------------------------------------------------

        If InStr(CString, Chr(0)) > 0 Then
            CtoVbString = Left$(CString, InStr(CString, Chr(0)) - 1)
        Else
            CtoVbString = CString
        End If
    End Function

#End Region

#Region "Öffentliche Methoden"
    '$ Öffentliche Methoden

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="Protocol">Gerätelogik die IsReceiveReady implementiert</param> 
    ''' <remarks></remarks>
    Sub New(ByVal Protocol As ThermoProtocol_Interface, ByVal DataContainer As ThermoDataContainer_Interface)
        Try
            _Protocol = Protocol
            _DataContainer = DataContainer
            _MyState = _IDLE
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' alles zumachen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose()
    End Sub

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">Bytestream der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden</param> 
    ''' <remarks></remarks>
    Public Sub DoTransferAr(ByVal Command() As Byte, ByVal Length As Integer, ByVal SendFlag As Integer) Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferAr
        Try
            Dim ret As Integer
            Dim param_short_array(27) As Short '12
            Dim param_integer_array(40) As Integer '12
            Dim param_byte_array(27) As Byte '12
            Dim a_help_array(10) As Byte
            Dim i As Integer, j As Integer, k As Integer, index As Integer, m As Integer
            Dim VersionString As String
            Dim bbyte As Byte
nochmal:
            VersionString = "12345678901234567890"  'zur Instanzierung
            bbyte = 0
            If (_MyState = _IDLE) Then
                _MyState = _BUSY
                _DataRead = 0
                Array.Clear(_ReceiveBuffer, 0, _ReceiveBuffer.Length)
                If SendFlag = 1 Then
                    If Command(0) <> &HFF And _init8000_done = False Then
                        bbyte = Command(0)
                        Command(0) = &HFF
                        MsgBox("Achtung Init8000 erzwungen!")
                        '                        _MyState = _IDLE
                        '                        RaiseEvent TimeOutEvent()
                        '                        Exit Sub
                    End If
                    Select Case Command(0)
                        Case &HFE
                            ret = CloseDevice
                            If ret <> 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If

                        Case &HFF
                            ret = Init8000
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            ret = Get_8000DLLVersion_PCI(VersionString)
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceivedString = CtoVbString(VersionString)
                            ret = Get_LpVersion_PCI(VersionString)
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceivedString = _ReceivedString & Chr(9) & CtoVbString(VersionString)
                            ret = CpuClock_long_PCI
                            _ReceiveBuffer(0) = ret
                            index = 1
                            ret = Get_ModulCodes_PCI(param_short_array(0))
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            For i = 0 To 5
                                _ReceiveBuffer(index + i) = param_short_array(i)
                                a_help_array(i) = _ReceiveBuffer(1 + i) And &HFF
                            Next
                            index = i + index
                            _DataRead = 7
                            For j = 0 To 5
                                If a_help_array(j) = 0 Or a_help_array(j) = 1 Or a_help_array(j) = 4 Then
                                    If a_help_array(j) = 0 Or a_help_array(j) = 1 Then
                                        ret = Get_Detector_Module_Settings_PCI(j + 1, param_byte_array(0))
                                    Else
                                        ret = Get_HV_Values_PCI(j + 1, param_integer_array(0), param_integer_array(1), param_integer_array(2))
                                        param_byte_array(0) = j  'damit man unten wie bei get_detector weitermachen kann,
                                        param_byte_array(1) = 4  'hier die ersten beiden returns von detektorkünstlich eintragen
                                        param_byte_array(2) = param_integer_array(0) And &HF
                                        param_byte_array(3) = param_integer_array(1) And &HFF
                                        param_byte_array(4) = (param_integer_array(1) And &HFF00) / 256
                                        param_byte_array(5) = param_integer_array(2) And &HFF
                                        param_byte_array(6) = (param_integer_array(2) And &HFF00) / 256
                                    End If
                                    If ret <> 1 Then
                                        _MyState = _IDLE
                                        RaiseEvent TimeOutEvent()
                                        Exit Sub
                                    End If
                                    For i = 0 To 24
                                        _ReceiveBuffer(index + i) = param_byte_array(i)
                                    Next
                                    index = i + index
                                    _DataRead = _DataRead + 25
                                End If
                            Next
                            i = 0
                            ret = Set_Beep_Control_Get_PB_Chain_Interr_PCI(i)
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(index) = i
                            index = 1 + index
                            _DataRead = _DataRead + 1

                            ret = Get_ControlLogic_PCI(param_byte_array(0))
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(index) = param_byte_array(0)
                            index = 1 + index
                            _DataRead = _DataRead + 1

                            _init8000_done = True

                        Case &HA0       'get 16 counter & 1 timer
                            ret = Get_Counters_And_Common_Timer_PCI(param_byte_array(0), param_integer_array(0), param_integer_array(16))
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = param_byte_array(0)  'changed
                            _ReceiveBuffer(1) = param_byte_array(1)  'temperature
                            _ReceiveBuffer(2) = param_byte_array(2)   'status low byte
                            _ReceiveBuffer(2) = _ReceiveBuffer(2) + param_byte_array(3) * 256  'status high byte
                            For i = 0 To 16
                                _ReceiveBuffer(3 + i) = param_integer_array(i)  'incl. common timer
                            Next i
                            _DataRead = 20

                        Case &HA1                   'get 16 counter & 16 timer
                            ret = Get_All_Counters_And_Timer_PCI(param_byte_array(0), param_integer_array(0), param_integer_array(16))
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = param_byte_array(0)  'changed
                            _ReceiveBuffer(1) = param_byte_array(1)  'temperature
                            _ReceiveBuffer(2) = param_byte_array(2)  'status low byte
                            _ReceiveBuffer(2) = _ReceiveBuffer(2) + param_byte_array(3) * 256  'status high byte
                            For i = 0 To 31
                                _ReceiveBuffer(3 + i) = param_integer_array(i)
                            Next i
                            _DataRead = 35

                        Case &HA2                   ' get 1 counter & 1 timer
                            ret = Get_Single_Counter_And_Timer_PCI(Command(1), param_byte_array(0), param_integer_array(0), param_integer_array(1))
                            If ret <> 1 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = param_byte_array(0)  'changed
                            _ReceiveBuffer(1) = param_byte_array(1)  'temperature
                            _ReceiveBuffer(2) = param_byte_array(2)  'status low byte
                            _ReceiveBuffer(2) = _ReceiveBuffer(2) + param_byte_array(3) * 256 'status high byte
                            _ReceiveBuffer(3) = param_integer_array(0)
                            _ReceiveBuffer(4) = param_integer_array(1)
                            _DataRead = 5

                        Case &HA3                'get counter state
                            ret = Get_CounterStatus_PCI(param_byte_array(0))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = param_byte_array(0)  'changed
                            _ReceiveBuffer(1) = param_byte_array(1)  'temperature
                            _ReceiveBuffer(2) = param_byte_array(2)  'status low byte
                            _ReceiveBuffer(2) = _ReceiveBuffer(2) + param_byte_array(3) * 256  'status high byte
                            _DataRead = 3

                        Case &HA5                'counter start
                            i = _MyUtilities.ToInt32(Command, 1) And &H1FFFF  'maske
                            j = _MyUtilities.ToInt32(Command, 5) And &HFFFFFF 'preset time
                            k = _MyUtilities.ToInt32(Command, 9) And &H1          'restart   
                            ret = StartCounter_PCI(i, j, k)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HA6                'counter stop
                            i = _MyUtilities.ToInt32(Command, 1) And &H1FFFF  'maske
                            ret = StopCounter_PCI(i)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HA7                'SetTimeBase_PCI
                            i = _MyUtilities.ToInt32(Command, 1)
                            ret = Set_TimeBase_PCI(i)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HA8                'Get_Digital_Inputs_PCI
                            ret = Get_Digital_Inputs_PCI(param_integer_array(0), param_integer_array(1))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _ReceiveBuffer(1) = param_integer_array(0)  'inputs
                            _ReceiveBuffer(2) = param_integer_array(1)  'timebase
                            _DataRead = 3

                        Case &HA9                'Set_Digital_Outputs_PCI
                            i = _MyUtilities.ToInt32(Command, 1)
                            j = _MyUtilities.ToInt32(Command, 5)
                            ret = Set_Digital_Outputs_PCI(i, j)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HAD                'SetHv_PCI
                            i = _MyUtilities.ToInt32(Command, 1)     'modulnummer
                            j = _MyUtilities.ToInt32(Command, 5)     'maske
                            k = _MyUtilities.ToInt32(Command, 9)     'Wert1
                            m = _MyUtilities.ToInt32(Command, 13)    'Wert2
                            ret = Set_Hv_PCI(i, j, k, m)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HAE                'Get_HV_Values_PCI
                            i = _MyUtilities.ToInt32(Command, 1)     'modulnummer
                            ret = Get_HV_Values_PCI(i, param_integer_array(0), param_integer_array(1), param_integer_array(2))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _ReceiveBuffer(1) = param_integer_array(0)      'Typ
                            _ReceiveBuffer(2) = param_integer_array(1)      'Wert1
                            _ReceiveBuffer(3) = param_integer_array(2)      'Wert2
                            _DataRead = 4

                        Case &HAF                'Set_Detector_Module_Settings_PCI
                            j = _MyUtilities.ToInt32(Command, 5)         'Maske
                            For i = 0 To 15
                                param_byte_array(i) = Command(9 + i)
                            Next
                            param_byte_array(16) = Command(25)       'InputSwitchFlag
                            param_byte_array(17) = Command(26)       'NumEnabledOutputs
                            For i = 0 To 3  'gain
                                param_byte_array(18 + i) = Command(27 + i)
                            Next
                            param_byte_array(22) = Command(31)      'HV switch
                            i = _MyUtilities.ToInt32(Command, 1)         'modulnummer
                            ret = Set_Detector_Module_Settings_PCI(i, j, param_byte_array(0), param_byte_array(16), param_byte_array(17), param_byte_array(18), param_byte_array(22))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HB1                'Set_5VSupply_PCI
                            i = _MyUtilities.ToInt32(Command, 1)         'modulnummer
                            j = Command(5)         'maske
                            k = Command(6)         'werte
                            ret = Set_5VSupply_PCI(i, j, k)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HB7                'Set_TempCalib_PCI
                            i = Command(1)        'Parameter 1
                            j = Command(2)         'Parameter 2
                            ret = Set_TempCalib_PCI(i, j)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HB9                'Set_AnalogOut_PCI
                            i = _MyUtilities.ToInt32(Command, 1)         'modulnummer
                            j = Command(5)         'outp NR
                            k = _MyUtilities.ToInt32(Command, 6)  'wert 0 or 1
                            ret = Set_AnalogOut_PCI(i, j, k)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HBA                'Get_AnalogIn_PCI
                            i = _MyUtilities.ToInt32(Command, 1)         'modulnummer
                            j = Command(5)         'inp NR
                            ret = Get_AnalogIn_PCI(i, j)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = 1
                            _ReceiveBuffer(1) = ret
                            _DataRead = 2

                        Case &HBB       'Set_EEpromReg_PCI(SHORT,SHORT,SHORT)
                            i = Command(1)       'modulnummer
                            j = Command(2)        'register NR
                            k = _MyUtilities.ToInt32(Command, 3)  'wert 
                            For ihelp As Integer = 0 To 5
                                ret = Set_EEpromReg_PCI(i, j, k)
                                If ret = 1 Then Exit For
                            Next
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HBC       'Get_EEpromReg_PCI(SHORT , SHORT , SHORT *)
                            i = Command(1)       'modulnummer
                            j = Command(2)        'register NR
                            For ihelp As Integer = 0 To 5
                                ret = Get_EEpromReg_PCI(i, j, k)
                                If ret = 1 Then Exit For
                            Next
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            If k > &H8000& Then
                                k = k - 65535 - 1
                            End If
                            _ReceiveBuffer(1) = k
                            _DataRead = 2

                        Case &HBE       'Set_ControlLogic_PCI (ByVal CommandString As String) As Integer
                            For i = 0 To 5
                                param_byte_array(i) = Command(1 + i)
                            Next
                            ret = Set_ControlLogic_PCI(param_byte_array(0))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HC0       'Set_Beep_Control_Get_PB_Chain_Interr_PCI
                            i = Command(1)         'info
                            ret = Set_Beep_Control_Get_PB_Chain_Interr_PCI(i)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _ReceiveBuffer(1) = i
                            _DataRead = 2

                        Case &HC1               'Watchdog-Funktionen
                            Select Case Command(1)               'subcode
                                Case 0
                                    i = Command(2)               'timeout
                                    ret = OpenWatchdog_PCI(i)
                                    If ret < 0 Then
                                        _MyState = _IDLE
                                        RaiseEvent TimeOutEvent()
                                        Exit Sub
                                    End If
                                    If ret >= 1 And ret <= 10 Then
                                        _ReceiveBuffer(0) = 1           'erfolg
                                        _ReceiveBuffer(1) = ret     'handle aus return
                                    Else
                                        _ReceiveBuffer(0) = ret         'fehlercode
                                        _ReceiveBuffer(1) = 0       'dummy
                                    End If
                                    _DataRead = 2
                                Case 1                  '
                                    i = Command(2)               'handle
                                    ret = TriggerWatchdog_PCI(i)
                                    _ReceiveBuffer(0) = ret         'fehlercode
                                Case 2                  '
                                    i = Command(2)               'handle
                                    j = Command(3)               'on/off
                                    ret = PauseWatchdog_PCI(i, j)
                                    _ReceiveBuffer(0) = ret         'fehlercode
                                Case 3                  '
                                    i = Command(2)               'handle
                                    ret = CloseWatchdog_PCI(i)
                                    _ReceiveBuffer(0) = ret         'fehlercode
                                Case 4                  '
                                    i = Command(2)               'timeout
                                    ret = ActivateWatchdog_PCI(i)
                                    _ReceiveBuffer(0) = ret         'fehlercode
                                Case 5                  '
                                    ret = DeactivateWatchdog_PCI()
                                    _ReceiveBuffer(0) = ret         'fehlercode
                            End Select
                            If Command(1) <> 0 Then              'nicht bei open
                                If ret < 0 Then
                                    _MyState = _IDLE
                                    RaiseEvent TimeOutEvent()
                                    Exit Sub
                                End If
                                _DataRead = 1
                            End If

                        Case &HC2               'Set_DeadTime_PCI(SHORT , SHORT , LONG)
                            i = Command(1)               'modulnummer
                            j = Command(2)               'input NR
                            k = _MyUtilities.ToInt32(Command, 3)  'wert 
                            ret = Set_DeadTime_PCI(i, j, k)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HC3               'Set_HV770T_PCI(SHORT, SHORT)
                            i = _MyUtilities.ToInt32(Command, 1)                 'wert
                            j = _MyUtilities.ToInt32(Command, 5)                 'wert
                            ret = Set_HV770T_PCI(i, j)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HC4               'SET_VLSTimeout_PCI(SHORT, LONG)
                            i = _MyUtilities.ToInt32(Command, 1)                 'Nummer
                            j = _MyUtilities.ToInt32(Command, 5)                 'wert
                            ret = SET_VLSTimeout_PCI(i, j)
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HC5               'GET_VLS_PCI(SHORT,LONG FAR *,SHORT FAR *)
                            i = _MyUtilities.ToInt32(Command, 1)                 'Nummer
                            j = _MyUtilities.ToInt32(Command, 2)                 'wert
                            ret = GET_VLS_PCI(i, param_integer_array(0), param_integer_array(1))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _ReceiveBuffer(1) = param_integer_array(0)
                            _ReceiveBuffer(2) = param_integer_array(1)
                            _DataRead = 3

                        Case &HC6               'Get_cKey_PCI(void)
                            ret = Get_cKey_PCI()
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            _DataRead = 1

                        Case &HC7               'Service_PCI(SHORT, LONG *)
                            i = _MyUtilities.ToInt32(Command, 1)                 'Code
                            ret = Service_PCI(i, param_integer_array(0))
                            If ret < 0 Then
                                _MyState = _IDLE
                                RaiseEvent TimeOutEvent()
                                Exit Sub
                            End If
                            _ReceiveBuffer(0) = ret
                            For i = 0 To 28
                                _ReceiveBuffer(i + 1) = param_integer_array(i)

                            Next
                            _DataRead = 30

                    End Select

                    If bbyte <> 0 Then
                        _MyState = _IDLE
                        Command(0) = bbyte
                        GoTo nochmal 'GOTO??? Man man man. (Bei Gelegenheit ändern!)
                    End If
                    Dim Arguments() As Object = {ReceiveBuffer, _ReceivedString, _DataRead}
                    _Protocol.IsReceiveReady(_DataContainer, Arguments) '= 1 Then          'Empfang ok???

                End If
                _MyState = _IDLE
                RaiseEvent DataReceivedEventAr()                'Alles ok für dieses Gerät

            End If

        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub DoTransferStr(ByVal Command As String, ByVal Length As Integer, ByVal SendFlag As Integer) Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferStr

    End Sub

#End Region

End Class

#End Region

