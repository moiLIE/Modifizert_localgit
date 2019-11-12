#Region "Imports"
Imports System.IO.Ports
Imports System.Net.Sockets
Imports System.Net
Imports System.Threading

Imports ThermoIOControls
Imports ThermoConnections
Imports ThermoDeviceSimaticSPS
Imports ThermoInterfaces

#End Region

Public Class FHT59N3_SPSCommunication

#Region "Konstanten für Transfer-State"
    Public Const TR_IDLE As Integer = 0
    Public Const TR_BUSY As Integer = 1
    Public Const TR_OK As Integer = 2
    Public Const TR_ERROR As Integer = -1
    Public Const TR_NAK As Integer = -4

    Public _DataTransferState As Integer
#End Region


#Region "Private Eigenschaften"

    ''' <summary>
    ''' These string cannot be declared as CONST, because the function SetOldCommands() can reassign them
    ''' </summary>
    ''' <remarks></remarks>
    Private COMMAND1 = "BT"
    Private COMMAND2 = "BS"
    Private COMMAND3 = "BG"
    Private COMMAND4 = "HG"
    Private COMMAND5 = "HS"
    Private COMMAND6 = "PG"
    Private COMMAND7 = "PS"
    Private COMMAND8 = "AG"
    Private COMMAND9 = "AS"
    Private COMMANDA = "YG"
    Private COMMANDB = "YS"
    Private COMMANDC = "EG"
    Private COMMANDD = "ES"
    Private COMMANDE = "SR"
    Private COMMANDF = "MR"
    Private COMMANDG = "GW1"
    Private COMMANDH = "GW0"
    Private COMMANDI = "CG"  'ecooler activated
    Private COMMANDJ = "CS"  'ecooler deactivated
    Private COMMANDK = "ZG"  'SPS 2.0: Set Airflow bezel factor with 1 parameter
    Private COMMANDL = "TG"  'SPS 2.0: Set Airflow norm with 1 parameter
    Private COMMANDM = "SG"  'SPS 2.0: Set Airflow operation qm with 1 parameter
    Private COMMANDN = "CR"  'SPS 2.0: Calculations Read (airflow)
    Private COMMANDV = "VR"

    'Befehle codiert - diese brauchen nur einmal codiert werden da keine weiteren Argumente erforderlich sind
    Private _CodedCommands As New Dictionary(Of String, String)

    'Serial Communication (RS232)
    Private _ComPort As String
    Private _UseStxEtxProtocol As Boolean
    Private _MySP As New ThermoIOControls.SerialPort(False) 'System.IO.Ports.SerialPort

    Private _Interface_To_TCPIP As ThermoIOControl_General
    Private _Interface_To_RS232 As ThermoIOControl_General

    Private WithEvents _DataCollector As ThermoDataCollectorA

    'TCP/IP Communication (TFHN-19)
    Private _networkAddress As String
    Private _remotePort As Integer

    'ATTENTION: Class ThermoIOControl_General uses _Socket
    Private _clientSocket As Socket

    Private m_WatchDogThreadStart As New Threading.ThreadStart(AddressOf SocketWatchDog)
    Private m_connectionWatchDog As New Threading.Thread(m_WatchDogThreadStart)
    Private m_abortSocketWatchdog As Boolean = False

    Private m_SocketConnectionThreadStarter As New Threading.ThreadStart(AddressOf SocketConnectionThread)
    Private m_connectionListener As New Threading.Thread(m_SocketConnectionThreadStarter)

    'SPS
    Private _FHT59N3SPS As ThermoDevice_SimaticSPS_FHT59N3
    Private _FHT59N3SPSDataContainer As ThermoDeviceDataContainer_SimaticSPS_FHT59N3 'Wird versorgt von ThermoDevice

    Private Const TCPIP_RECONNECT_TIME_IN_MS As Integer = 200

    Private _CommunicationEstablished As Boolean = False  'initial: we don't have a connect
    Private _ConnectionEstablishmentOngoing As Boolean
    Private _DataError As Boolean
    Private _Sent As Boolean = False
    Private _ReceiveReturn As Integer

    Private _ActCommandCode As String

    Private _NewDigValuesAvailable As Boolean = False
    Private _NewAnaValuesAvailable As Boolean = False

#End Region

#Region "Öffentliche Eigenschaften"

    Public ReadOnly Property CommunicationEstablished() As Boolean
        Get
            Return _CommunicationEstablished
        End Get
    End Property

 

    Public ReadOnly Property DataContainer() As ThermoDeviceDataContainer_SimaticSPS_FHT59N3
        Get
            Return _FHT59N3SPSDataContainer
        End Get
    End Property

    Public ReadOnly Property DataCollector() As ThermoDataCollectorA
        Get
            Return _DataCollector
        End Get
    End Property

    Public ReadOnly Property FHT59N3SPS() As ThermoDevice_SimaticSPS_FHT59N3
        Get
            Return _FHT59N3SPS
        End Get
    End Property

    Public ReadOnly Property ReceiveReturn() As Integer
        Get
            Return _ReceiveReturn
        End Get
    End Property

    Public Property NewDigValuesAvailable() As Boolean
        Get
            Return _NewDigValuesAvailable
        End Get
        Set(ByVal value As Boolean)
            _NewDigValuesAvailable = value
        End Set
    End Property

    Public Property NewAnaValuesAvailable() As Boolean
        Get
            Return _NewAnaValuesAvailable
        End Get
        Set(ByVal value As Boolean)
            _NewAnaValuesAvailable = value
        End Set
    End Property

#End Region

#Region "Private Methoden"

#Region "FHT59N3SPS"

    Private Sub OpenSerialPort()
        Try
            If _MySP.IsOpen Then
                _MySP.Close()
            End If

            _MySP.PortName = _ComPort
            _MySP.Baudrate = 9600
            _MySP.Parity = Parity.Even
            _MySP.DataBits = 7
            _MySP.StopBits = StopBits.One
            _MySP.Handshake = Handshake.None
            _MySP.DtrEnable = False
            _MySP.RtsEnable = False
            _MySP.ReceivedBytesThreshold = 1
            _MySP.Open()

            If _MySP.IsOpen Then
                _CommunicationEstablished = True
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>funciton is left, when _ConnectionEstablishmentOngoing is false</remarks>
    Private Sub SocketConnectionThread()

        _clientSocket = Nothing
        _Interface_To_TCPIP.MySocket = Nothing
        _clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

        While _ConnectionEstablishmentOngoing
            Try
                If _clientSocket Is Nothing Then
                    Exit While
                End If

                _clientSocket.Connect(_networkAddress, _remotePort)
                _Interface_To_TCPIP.MySocket = _clientSocket
                If _clientSocket.Connected Then
                    _CommunicationEstablished = True
                    _ConnectionEstablishmentOngoing = False

                    '_DataTransferState is set to OK in the 1st data receiption
                    StartSocketWatchDog()
                End If

            Catch ex As SocketException
                _CommunicationEstablished = False
                _ConnectionEstablishmentOngoing = True
                _DataTransferState = TR_ERROR

                Thread.Sleep(TCPIP_RECONNECT_TIME_IN_MS)
                Continue While

            Catch general As Exception
                _CommunicationEstablished = False
                _ConnectionEstablishmentOngoing = True
                _DataTransferState = TR_ERROR

                Thread.Sleep(TCPIP_RECONNECT_TIME_IN_MS)
                Continue While

            End Try
        End While
    End Sub

    Private Sub StartSocketWatchDog()
        m_connectionWatchDog = New Thread(m_WatchDogThreadStart)
        m_connectionWatchDog.Name = "SPS (TCP/IP) supervision"
        m_connectionWatchDog.Start()
    End Sub

    ''' <summary>
    ''' Supervice the connection via socket-connected
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SocketWatchDog()
        Dim name = Me.GetType().Name

        While (Not m_abortSocketWatchdog)
            If (_clientSocket Is Nothing) Then
                Exit While
            ElseIf (Not _clientSocket.Connected) Then
                ' TODO: raise the event Disconnected() or DataErrorEvent(neue_Nummer) from class ThermoIOControl_General
                Trace.TraceError("SocketWatchDog() in " & name & " has seen a disconnect")
                Exit While
            End If

            If Not _clientSocket.Poll(10, SelectMode.SelectWrite) Then
                Exit While
            End If

            Threading.Thread.Sleep(100)
        End While

        If m_abortSocketWatchdog Then
            Return
        End If

        Me.OpenTcpIpConnection()
    End Sub



    Private Sub OpenTcpIpConnection()
        Try

            If _CommunicationEstablished = True Then
                Try
                    _clientSocket.Disconnect(False)
                    _clientSocket.Close()
                    _CommunicationEstablished = False
                Catch ex As Exception
                    _CommunicationEstablished = False
                End Try
            End If

            _ConnectionEstablishmentOngoing = True

            m_connectionListener = New Thread(m_SocketConnectionThreadStarter)
            m_connectionListener.Name = "Starter for TCP/IP connection"
            m_connectionListener.Start()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub InitializeCommands()
        Try
            BuildCommandFHT59N3SPS(COMMAND2)
            BuildCommandFHT59N3SPS(COMMAND3)
            BuildCommandFHT59N3SPS(COMMAND4)
            BuildCommandFHT59N3SPS(COMMAND5)
            BuildCommandFHT59N3SPS(COMMAND6)
            BuildCommandFHT59N3SPS(COMMAND7)
            BuildCommandFHT59N3SPS(COMMAND8)
            BuildCommandFHT59N3SPS(COMMAND9)
            BuildCommandFHT59N3SPS(COMMANDA)
            BuildCommandFHT59N3SPS(COMMANDB)
            BuildCommandFHT59N3SPS(COMMANDC)
            BuildCommandFHT59N3SPS(COMMANDD)
            BuildCommandFHT59N3SPS(COMMANDE)
            BuildCommandFHT59N3SPS(COMMANDF)
            BuildCommandFHT59N3SPS(COMMANDG)
            BuildCommandFHT59N3SPS(COMMANDH)
            BuildCommandFHT59N3SPS(COMMANDI)  'ecooler on
            BuildCommandFHT59N3SPS(COMMANDJ)  'ecooler off
            BuildCommandFHT59N3SPS(COMMANDK)
            BuildCommandFHT59N3SPS(COMMANDL)
            BuildCommandFHT59N3SPS(COMMANDM)
            BuildCommandFHT59N3SPS(COMMANDN)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Set old commands in case STXETX constructor parameter is set to false (using old protocol)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetOldCommands()
        COMMAND1 = "1"
        COMMAND2 = "2"
        COMMAND3 = "3"
        COMMAND4 = "4"
        COMMAND5 = "5"
        COMMAND6 = "6"
        COMMAND7 = "7"
        COMMAND8 = "8"
        COMMAND9 = "9"
        COMMANDA = "A"
        COMMANDB = "B"
        COMMANDC = "C"
        COMMANDD = "D"
        COMMANDE = "E"
        COMMANDF = "F"
        COMMANDG = "G"
        COMMANDH = "H"
        COMMANDV = "V"
    End Sub

    ''' <summary>
    ''' Kommando kodieren und in einem Dictionary ablegen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BuildCommandFHT59N3SPS(ByVal CommandCode As String, Optional ByVal Arguments As Object() = Nothing)
        Try
            Dim Args As Object() = {CommandCode}

            _FHT59N3SPS.CodeCommand(_FHT59N3SPSDataContainer, Args)
            If Not _CodedCommands.ContainsKey(CommandCode) Then
                _CodedCommands.Add(CommandCode, _FHT59N3SPSDataContainer.CommandAsString)
            Else
                _CodedCommands(CommandCode) = _FHT59N3SPSDataContainer.CommandAsString
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub SendCommandToFHT59N3SPS(ByVal CommandCode As String, Optional ByVal Arguments As Object() = Nothing)
        Try
            If _CommunicationEstablished Then

                Select Case CommandCode

                    Case COMMAND1, COMMANDK, COMMANDL, COMMANDM
                        BuildCommandFHT59N3SPS(CommandCode, Arguments)

                    Case Else
                        _FHT59N3SPSDataContainer.Command = CommandCode
                        _FHT59N3SPSDataContainer.CommandAsString = _CodedCommands(CommandCode)
                        _FHT59N3SPSDataContainer.CommandLength = _FHT59N3SPSDataContainer.CommandAsString.Length

                End Select

                _ActCommandCode = CommandCode
                _DataTransferState = TR_BUSY


                'FHTNT-53: Fehlersuche 2.Filterschritt ausgefallen
                'wir reduzieren die Sleep-Zeit von 3000ms auf 30ms und setzen stattdessen tmrTrigger von 10ms auf 300ms
                _Sent = _DataCollector.StartTransfer(_FHT59N3SPSDataContainer.CommandAsString, 1, False, 30)


                If Not _Sent Then
                    _DataTransferState = TR_ERROR
                    _ReceiveReturn = -1
                End If
                'Trace.TraceInformation(Now.ToShortDateString & " " & Now.ToLongTimeString & ": FHT59N3_SPSCommunication.SendCommandToFHT59N3SPS: Command=" & _ActCommandCode & ": String=" & _FHT59N3SPSDataContainer.CommandAsString)

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#End Region

#Region "Öffentliche Methoden"

#Region "Allgemeines"

    Private Sub New(ByVal UseStxEtxProtocol As Boolean)
        _CommunicationEstablished = False
        _UseStxEtxProtocol = UseStxEtxProtocol

        ' Allow the possibility to switch back to unsecured protocol...
        If Not UseStxEtxProtocol Then
            SetOldCommands()
        End If

        _FHT59N3SPSDataContainer = New ThermoDeviceDataContainer_SimaticSPS_FHT59N3
    End Sub

    ''' <summary>
    ''' Constructor for use of RS232 (serial port)
    ''' </summary>
    ''' <param name="ComPort"></param>
    ''' <param name="UseStxEtxProtocol"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal ComPort As String, ByVal UseStxEtxProtocol As Boolean) ' ByVal StateMachine As Net6020A2_Statemachine,
        Me.New(UseStxEtxProtocol)
        _ComPort = ComPort

        Try
            If (_UseStxEtxProtocol = True) Then

                Dim _FHT59N3SPSProtSTX As ThermoProtocol_STX = New ThermoProtocol_STX
                _FHT59N3SPS = New ThermoDevice_SimaticSPS_FHT59N3(_FHT59N3SPSProtSTX)
                _Interface_To_RS232 = New ThermoIOControl_General(_MySP, 2000, _FHT59N3SPSProtSTX, _FHT59N3SPSDataContainer, False)

            Else
                Dim _FHT59N3SPSProt As ThermoProtocol_SimaticSPS_FHT59N3 = New ThermoProtocol_SimaticSPS_FHT59N3
                _FHT59N3SPS = New ThermoDevice_SimaticSPS_FHT59N3(_FHT59N3SPSProt)
                _Interface_To_RS232 = New ThermoIOControl_General(_MySP, 2000, _FHT59N3SPSProt, _FHT59N3SPSDataContainer, False)
            End If

            _DataCollector = New ThermoDataCollectorA(_Interface_To_RS232, 3)
            InitializeCommands()
            OpenSerialPort()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Constructor for use of TCP/IP protokoll
    ''' </summary>
    ''' <param name="NetworkAddress"></param>
    ''' <param name="RemotePort"></param>
    ''' <param name="UseStxEtxProtocol"></param>
    ''' <remarks></remarks>
    Sub New(ByVal NetworkAddress As String, ByVal RemotePort As Integer, ByVal UseStxEtxProtocol As Boolean)
        Me.New(UseStxEtxProtocol)

        _networkAddress = NetworkAddress
        _remotePort = RemotePort

        _clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

        Try
            'for TCP/IP we only know this protocol for TCP/IP data exchange to S7-1200 at the moment...

            Dim _thermoProtocol As ThermoProtocol_STX_TCP = New ThermoProtocol_STX_TCP
            _FHT59N3SPS = New ThermoDevice_SimaticSPS_FHT59N3(_thermoProtocol)

            _Interface_To_TCPIP = New ThermoIOControl_General(_clientSocket, 2000, _thermoProtocol, _FHT59N3SPSDataContainer, False)

            _DataCollector = New ThermoDataCollectorA(_Interface_To_TCPIP, 3)

            InitializeCommands()

            OpenTcpIpConnection()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Dispose()
        Try
            If Not _Interface_To_RS232 Is Nothing Then
                _Interface_To_RS232.Dispose()
            End If
            If Not _Interface_To_TCPIP Is Nothing Then
                _Interface_To_TCPIP.Dispose()
                m_abortSocketWatchdog = True
                Try
                    If _clientSocket.Connected Then
                        _clientSocket.Disconnect(False)
                        _clientSocket.Close()
                    End If
                Catch ex As Exception

                End Try
            End If
            Try
                _MySP.Close()
            Catch ex As Exception
            End Try

            _clientSocket = Nothing
            _Interface_To_TCPIP = Nothing
            _MySP = Nothing
            _Interface_To_RS232 = Nothing
            _DataCollector = Nothing
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    ''' <summary>
    ''' Check wether data are available or not
    ''' </summary>
    ''' <remarks>
    ''' modifies:
    '''   _DataError
    '''   _DataTransferState
    '''   _Sent
    ''' </remarks>
    Public Sub CheckIfDataReceived()
        Try
            Dim retValueFromReadData As Integer = 0
            If _CommunicationEstablished Then

                'brauchen nicht mehr zu lauschen, haben einen fehler oder gültige Antwort bekommen...
                If Not _Sent Then
                    Return
                End If

                '' >0 = number of databytes correctly received
                ''  0 = still waiting for correct answer
                '' -1 = timeout
                '' -2 = checksum error
                '' -3 = parity error
                '' -4 = NAK error
                '' -10 = Exception
                retValueFromReadData = _DataCollector.ReadData

                If (Not String.IsNullOrEmpty(_FHT59N3SPSDataContainer.AnswerAsString)) Then
                    Trace.TraceInformation("FHT59N3_SPSCommunication.CheckIfDataReceived: Command=" & _ActCommandCode & ": ReceivedData=" & _FHT59N3SPSDataContainer.AnswerAsString)
                End If

                If retValueFromReadData > 0 Then

                    'Wenn alles OK => Decodieren
                    'The sub fills DataContainer
                    _FHT59N3SPS.DecodeData(_FHT59N3SPSDataContainer, _FHT59N3SPSDataContainer.AnswerAsString)

                    'Neue Analogwerte verfügbar...
                    If _ActCommandCode = COMMANDF Then
                        _NewAnaValuesAvailable = True
                    End If

                    'Neue Digitalwerte verfügbar...
                    If _ActCommandCode = COMMANDE Then
                        _NewDigValuesAvailable = True
                    End If

                    _DataTransferState = TR_OK
                    _Sent = False

                    'wir warten gerade noch auf die Antwort...
                ElseIf retValueFromReadData = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_BUSY Then

                    'spezifisch fuer das STX Proto 
                    If _FHT59N3SPSDataContainer.AnswerAsString = "NAK" Then

                        'wieder auskommentiert da nur testhalber: _DataTransferState = TR_OK

                        _DataTransferState = TR_NAK

                        'bei einem NAK - der nie kommen sollte - geben wir der SPS ein Haufen Zeit sich zu erholen...
                        Thread.Sleep(3000)

                        _Sent = True

                    End If
                    'Still waiting

                    'alles kleiner 0 sind Fehlercodes!
                ElseIf retValueFromReadData < 0 Then
                    _DataTransferState = TR_ERROR
                    _Sent = False

                    'Kommunikationsfehler
                    If (retValueFromReadData = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT) Then
                        Trace.TraceError("Timeout in response of command '" & _ActCommandCode & "'")
                    ElseIf (retValueFromReadData = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION) Then
                        Trace.TraceError("Exception in response of command '" & _ActCommandCode & "'")
                    End If
                End If

                _ReceiveReturn = retValueFromReadData
                _FHT59N3SPSDataContainer.AnswerAsString = ""

            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Befehle"

    Public Function SetFilterstep(ByVal StepWidth As Integer) As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_MotorOnOff Then
                    _FHT59N3SPSDataContainer.Filterstep_mm = StepWidth
                    SendCommandToFHT59N3SPS(COMMAND1)
                Else
                    Trace.TraceError("Couldn't send filterstep command as filter motor still running!")
                End If
                Ret = 1
            Else
                Trace.TraceError("Couldn't send filterstep command as communication not established!")
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function SetFactorBezel(ByVal FactorBezel As Double) As Integer
        Try
            Dim Ret As Integer
            Dim MyArray(1) As Object

            If _CommunicationEstablished Then
                _FHT59N3SPSDataContainer.FactorBezel = FactorBezel
                MyArray(0) = FactorBezel

                SendCommandToFHT59N3SPS(COMMANDK, MyArray)
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function SetAirFlowSetpoint(ByVal AirFlowMode As Boolean, ByVal AirFlowSetPoint As Integer) As Integer
        Try
            Dim Ret As Integer
            Dim MyArray(1) As Object

            If _CommunicationEstablished Then
                If AirFlowMode = False Then
                    'norm mode
                    _FHT59N3SPSDataContainer.ThroughputAirNorm = AirFlowSetPoint
                    MyArray(0) = AirFlowSetPoint
                    SendCommandToFHT59N3SPS(COMMANDL, MyArray)
                    Ret = 1
                Else
                    'operation (working) mode
                    _FHT59N3SPSDataContainer.ThroughputAirOperation = AirFlowSetPoint
                    MyArray(0) = AirFlowSetPoint
                    SendCommandToFHT59N3SPS(COMMANDM, MyArray)
                    Ret = 1
                End If
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function DetectorheadOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If (_FHT59N3SPSDataContainer.DigOut_DetectorHeadOnOff) Or (_FHT59N3SPSDataContainer.DigOut_BypassOnOff) Then 'nur senden wenn offen, sonst nimmt die sps den befehl kommentarlos NICHT an was zu einem transfehrfehler führt
                    SendCommandToFHT59N3SPS(COMMAND2)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function DetectorheadOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_DetectorHeadOnOff Then
                    SendCommandToFHT59N3SPS(COMMAND3)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function


    Public Function HeatingOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_HeatingOnOff Then
                    SendCommandToFHT59N3SPS(COMMAND4)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function HeatingOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_HeatingOnOff Then
                    SendCommandToFHT59N3SPS(COMMAND5)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function PumpOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                'Trace.TraceInformation(": FHT59N3_SPSCommunication.PumpOn:  _FHT59N3SPSDataContainer.DigOut_PumpOnOff=" & _FHT59N3SPSDataContainer.DigOut_PumpOnOff.ToString)
                If Not _FHT59N3SPSDataContainer.DigOut_PumpOnOff Then
                    'Trace.TraceInformation(": FHT59N3_SPSCommunication.PumpOn:  SetPumpOn!")
                    SendCommandToFHT59N3SPS(COMMAND6)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function PumpOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_PumpOnOff Then
                    SendCommandToFHT59N3SPS(COMMAND7)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function AlarmRelaisOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_AlarmRelaisOnOff Then
                    SendCommandToFHT59N3SPS(COMMAND8)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function AlarmRelaisOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_AlarmRelaisOnOff Then
                    SendCommandToFHT59N3SPS(COMMAND9)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function BypassOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_BypassOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDA)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function BypassOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_BypassOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDB)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function ErrorOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_ErrorOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDC)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function ErrorOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_ErrorOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDD)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function EcoolerOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_EcoolerOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDI) 'activate
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function EcoolerOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_EcoolerOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDJ) 'deactivate
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function


    ' more complex data requests

    Public Function GetDigStates() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                SendCommandToFHT59N3SPS(COMMANDE)
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function GetAnaStates() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                SendCommandToFHT59N3SPS(COMMANDF)
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function GetCalculations() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                SendCommandToFHT59N3SPS(COMMANDN)
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function MaintenanceOn() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If Not _FHT59N3SPSDataContainer.DigOut_MaintenanceOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDG)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function MaintenanceOff() As Integer
        Try
            Dim Ret As Integer
            If _CommunicationEstablished Then
                If _FHT59N3SPSDataContainer.DigOut_MaintenanceOnOff Then
                    SendCommandToFHT59N3SPS(COMMANDH)
                End If
                Ret = 1
            Else
                Ret = -1
            End If
            Return Ret
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function



#End Region

#End Region



    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

