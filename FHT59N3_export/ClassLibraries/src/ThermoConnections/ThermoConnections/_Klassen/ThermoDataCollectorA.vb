'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

#Region "Imports"
Imports ThermoInterfaces
Imports ThermoIOControls
Imports System.IO.Ports
Imports System.Threading

#End Region

''' <summary>
''' This class is a summary of the DataCollector and the DataCollector_WithOutEvents, so it can be operated with events or with polling
''' Additionally it can handle all data transfers which need a wake up sign before the real data transfer
''' </summary>
''' <remarks></remarks>
Public Class ThermoDataCollectorA

#Region "Private Felder"
    Private WithEvents _Interface As ThermoIOControl_General     'Interface zum Gerät
    Private _Repeats As Integer
    Private _ReceiveBuffer(1000) As Byte
    Private _TimeOut As Integer 'gelten für alle Geräte
    Private _RepeatCount As Integer 'gelten für alle Geräte
    Private _Command() As Byte
    Private _CommandString As String
    Private _CommandLength As Integer
    Private _SendFlag As Integer '0= nicht senden, 1= senden
    Private _SetBusy As Boolean

    Private _MyState As Integer
    Private Const _IDLE As Integer = 0
    Private Const _BUSY As Integer = 1

    'Diagnose
    Private _Diagnose As ThermoConnectionDiagnosis

    Private _CommunicationWithWakeUpSignNecessary As Boolean = False
    Private _WakeUpSignSuccesfullySended As Boolean = False
    Private _WakeUpSign As String = "@"
    Private _WakeUpSignLength As Integer
    Private _WakeUpAnswer As String = ">"
    Private _WakeUpTimeout As Int16
    Private _FullWakeUpHandling As Boolean = False
    Private _TimeUntilNextWakeUpSign As Integer = -1
    Private _TimeOfLastTransfer As Date = Now


#End Region

#Region "Öffentliche Eigenschaften"

    ''' <summary>
    ''' Wieviele Wiederholungen soll es geben
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Repeats() As Integer
        Get
            Return _RepeatCount
        End Get
        Set(ByVal value As Integer)
            _RepeatCount = value
        End Set
    End Property

    ''' <summary>
    ''' Verbindungsdiagnose (Totale Übertragungen, Repeats, Timeouts)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Diagnosis() As ThermoConnectionDiagnosis
        Get
            Return _Diagnose
        End Get
    End Property

    ''' <summary>
    ''' Timeoutzeit der Verbindung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Timeout() As Integer
        Get
            Return _Interface.Timeout
        End Get
        Set(ByVal value As Integer)
            _TimeOut = value
            _Interface.Timeout = value
        End Set
    End Property

    ''' <summary>
    ''' Nur für die Kommunikation mit Weckzeichen.
    ''' Wenn zwischen der letzten Sendung und der aktuellen mehr als TimeUntilNextWakeUpSign ms vergangen sind
    ''' dann wird das Weckzeichen gesendet, sonst nicht.
    ''' Im Falle einer Wiederholung wird auf jeden Fall das Weckzeichen gesendet!
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TimeUntilNextWakeUpSign() As Integer
        Get
            Return _TimeUntilNextWakeUpSign
        End Get
        Set(ByVal value As Integer)
            _TimeUntilNextWakeUpSign = value
        End Set
    End Property

    ''' <summary>
    ''' Gerät ist nicht mehr verbunden
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DisconnectedFlag() As Boolean
        Get
            Return _Interface.DisconnectedFlag
        End Get
    End Property

    ''' <summary>
    ''' Event das ausgelöst wird wenn die Übertragung beendet wurde 
    ''' </summary>
    ''' <param name="ReturnState">
    ''' >0 = Number of Databytes receid
    ''' -1 = Timeout
    ''' -2 = Checksum error
    ''' -3 = Parity error
    ''' -4 = NACK error
    ''' -5 = Special NACK error (eg. from FHT681)
    ''' -10 = Exception occured
    ''' </param>
    ''' <remarks></remarks>
    Public Event DataCollectorReadyAr(ByVal ReturnState As Integer)

    ''' <summary>
    ''' Event das ausgelöst wird wenn die Übertragung beendet wurde 
    ''' </summary>
    ''' <param name="ReturnState">
    ''' >0 = Number of Databytes receid
    ''' -1 = Timeout
    ''' -2 = Checksum error
    ''' -3 = Parity error
    ''' -4 = NACK error
    ''' -5 = Special NACK error (eg. from FHT681)
    ''' -10 = Exception occured
    ''' </param>
    ''' <remarks></remarks>
    Public Event DataCollectorReadyStr(ByVal ReturnState As Integer)

    ''' <summary>
    ''' eine Verbindung zu einem Gerät wurde unterbrochen
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Disconnected()

#End Region

#Region "Private Methoden"

#Region "Asynchron"

    ''' <summary>
    ''' Daten empfangen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataReceived_ByteArray(ByVal ReceivedDataNum As Integer) Handles _Interface.DataReceivedEventAr
        Try
            _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
            _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
            _MyState = _IDLE
            RaiseEvent DataCollectorReadyAr(ReceivedDataNum)
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Daten empfangen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataReceived_String(ByVal ReceivedDataNum As Integer) Handles _Interface.DataReceivedEventStr
        Try
            _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
            _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
            If _CommunicationWithWakeUpSignNecessary And (Not _WakeUpSignSuccesfullySended) Then
                _WakeUpSignSuccesfullySended = True
                _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy)
            Else
                _MyState = _IDLE
                RaiseEvent DataCollectorReadyStr(ReceivedDataNum)
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Timeout verwalten, wiederholen oder Fehler
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TransferErr(ByVal ErrNum As Integer) Handles _Interface.DataErrorEvent
        Try
            If (ErrNum <> ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION) _
            And (ErrNum <> ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK) Then
                If _Repeats < _RepeatCount Then
                    _Repeats += 1

                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                    _Diagnose.CounterRepeats = _Diagnose.CounterRepeats + 1

                    Select Case ErrNum

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_PARITY
                            _Diagnose.CounterParityError = _Diagnose.CounterParityError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                            _Diagnose.CounterChecksumError = _Diagnose.CounterChecksumError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_NAK
                            _Diagnose.CounterNAKError = _Diagnose.CounterNAKError + 1

                    End Select

                    Select Case _Interface.MyComMode

                        Case ThermoIOControl_General.ComMode.CM_ARRAY

                            _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag, _SetBusy)

                        Case ThermoIOControl_General.ComMode.CM_STRING

                            If _CommunicationWithWakeUpSignNecessary Then
                                _WakeUpSignSuccesfullySended = False 'ganze Sendung nochmal von vorne beginnen inkl. Weckzeichen
                                _Interface.DoTransferStr(_WakeUpSign, _WakeUpSignLength, _SendFlag, _SetBusy)
                            ElseIf _FullWakeUpHandling Then 'ganze Sendung nochmal von vorne beginnen inkl. Weckzeichen
                                StartTransferWakeUpComplete(_WakeUpTimeout, _CommandString, _SendFlag, _SetBusy, True)
                            Else
                                _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy)
                            End If

                    End Select

                Else

                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1

                    Select Case ErrNum

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_PARITY
                            _Diagnose.CounterParityError = _Diagnose.CounterParityError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                            _Diagnose.CounterChecksumError = _Diagnose.CounterChecksumError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_NAK
                            _Diagnose.CounterNAKError = _Diagnose.CounterNAKError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT
                            _Diagnose.CounterTimeouts = _Diagnose.CounterTimeouts + 1

                    End Select

                    _MyState = _IDLE

                    Select Case _Interface.MyComMode

                        Case ThermoIOControl_General.ComMode.CM_ARRAY

                            RaiseEvent DataCollectorReadyAr(ErrNum)

                        Case ThermoIOControl_General.ComMode.CM_STRING

                            RaiseEvent DataCollectorReadyStr(ErrNum)

                    End Select

                End If

            Else 'Exception oder SpecialNAK -> keine Wdh. nötig (Programmierfehler)

                _MyState = _IDLE

                Select Case _Interface.MyComMode

                    Case ThermoIOControl_General.ComMode.CM_ARRAY

                        RaiseEvent DataCollectorReadyAr(ErrNum)

                    Case ThermoIOControl_General.ComMode.CM_STRING

                        RaiseEvent DataCollectorReadyStr(ErrNum)

                End Select

            End If

        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Verbindung unterbrochen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisconnectEvent() Handles _Interface.Disconnected
        RaiseEvent Disconnected()
    End Sub

#End Region

#Region "Wartezeit"

    ''' <summary>
    ''' Wartet x ms
    ''' </summary>
    ''' <param name="TimeToWait">Zeit die gewartet werden soll</param>
    ''' <remarks></remarks>
    Private Sub WarteMs(ByVal TimeToWait As Integer)
        Thread.Sleep(TimeToWait)
    End Sub

#End Region

#End Region

#Region "Öffentliche Methoden"

#Region "Konstruktor"

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="MyInterface">Interface zur RS232/Ethernet</param>
    ''' <param name="Repeats">Wieviele Wiederholungen beim Timeout</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal MyInterface As ThermoIOControls.ThermoIOControl_General, ByVal Repeats As Integer)
        Try
            _RepeatCount = Repeats
            _Interface = MyInterface
            _TimeOut = _Interface.Timeout
            _MyState = _IDLE
            _Diagnose = New ThermoConnectionDiagnosis(0)
            _TimeOfLastTransfer = Now.AddDays(-1)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Transfer"

    ''' <summary>
    ''' Abfrage abschicken
    ''' </summary>
    ''' <param name="Command">Befehl an das Gerät</param>
    ''' <param name="ComLength">Länge des Befehls im Puffer</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param>
    ''' <param name="SetBusy">soll die Kommunikation blockieren bis eine vorherige abgeschlossen wurde</param>
    ''' <param name="WaitTimeMS">Wartezeit in ms vor dem Senden (=-1 -> nicht verwendet)</param>
    ''' <remarks></remarks>
    Public Function StartTransfer(ByVal Command As Array, ByVal ComLength As Integer, ByVal SendFlag As Integer, ByVal SetBusy As Boolean, Optional ByVal WaitTimeMS As Integer = -1) As Boolean
        Dim RetVal As Boolean = False
        Try
            If _MyState = _IDLE Then
                If SetBusy Then _MyState = _BUSY
                ReDim _Command(Command.Length)
                Command.CopyTo(_Command, 0)
                _CommandLength = ComLength
                _Repeats = 0
                _SendFlag = SendFlag
                _SetBusy = SetBusy
                _CommunicationWithWakeUpSignNecessary = False
                _FullWakeUpHandling = False

                If (WaitTimeMS <> -1) And (WaitTimeMS > 0) Then
                    WarteMs(WaitTimeMS)
                End If
                If _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag, _SetBusy) Then
                    _TimeOfLastTransfer = Now
                    RetVal = True
                Else
                    _MyState = _IDLE
                End If
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return RetVal
        End Try
    End Function

    ''' <summary>
    ''' Abfrage abschicken
    ''' </summary>
    ''' <param name="Command">Befehl an das Gerät</param>
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <param name="SetBusy">soll die Kommunikation blockieren bis eine vorherige abgeschlossen wurde</param>
    ''' <param name="WaitTimeMS">Wartezeit in ms vor dem Senden (=-1 -> nicht verwendet)</param>
    ''' <remarks></remarks>
    Public Function StartTransfer(ByVal Command As String, ByVal SendFlag As Integer, ByVal SetBusy As Boolean, Optional ByVal WaitTimeMS As Integer = -1) As Boolean
        Dim RetVal As Boolean = False
        Try
            If _MyState = _IDLE Then
                If SetBusy Then _MyState = _BUSY
                _CommandString = Command
                _CommandLength = Command.Length
                _Repeats = 0
                _SendFlag = SendFlag
                _SetBusy = SetBusy
                _CommunicationWithWakeUpSignNecessary = False
                _FullWakeUpHandling = False

                If (WaitTimeMS <> -1) And (WaitTimeMS > 0) Then
                    WarteMs(WaitTimeMS)
                End If
                If _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy) Then
                    _TimeOfLastTransfer = Now
                    RetVal = True
                Else
                    _MyState = _IDLE
                End If
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return RetVal
        End Try
    End Function

    ''' <summary>
    ''' Abfrage abschicken; Weckzeichen wird mitgeschickt, aber Funktion kommt sofort nach senden des Weckzeichens zurück
    ''' das eigentliche Kommando wird nach erhalt der Antwort auf das Weckzeichen automatisch gesendet.
    ''' </summary>
    ''' <param name="Command">Befehl an das Gerät</param>
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <param name="SetBusy">soll die Kommunikation blockieren bis eine vorherige abgeschlossen wurde</param>
    ''' <param name="WaitTimeMS">Wartezeit in ms vor dem Senden (=-1 -> nicht verwendet)</param>
    ''' <remarks></remarks>
    Public Function StartTransferWakeupCharOnly(ByVal Command As String, ByVal SendFlag As Integer, ByVal SetBusy As Boolean, Optional ByVal WaitTimeMS As Integer = -1) As Boolean
        Dim RetVal As Boolean = False
        Dim SendWakeUpSign As Boolean = True
        Try
            If _MyState = _IDLE Then
                If SetBusy Then _MyState = _BUSY
                _WakeUpSignLength = _WakeUpSign.Length
                _CommandString = Command
                _CommandLength = Command.Length
                _Repeats = 0
                _SendFlag = SendFlag
                _SetBusy = SetBusy
                _CommunicationWithWakeUpSignNecessary = True
                _FullWakeUpHandling = False
                If (WaitTimeMS <> -1) And (WaitTimeMS > 0) Then
                    WarteMs(WaitTimeMS)
                End If
                If _TimeUntilNextWakeUpSign > 0 Then
                    If Now.Subtract(_TimeOfLastTransfer).TotalMilliseconds >= _TimeUntilNextWakeUpSign Then
                        SendWakeUpSign = True
                    Else
                        SendWakeUpSign = False
                    End If
                End If
                If SendWakeUpSign Then
                    _WakeUpSignSuccesfullySended = False
                    If _Interface.DoTransferStr(_WakeUpSign, _WakeUpSignLength, _SendFlag, _SetBusy) Then
                        _TimeOfLastTransfer = Now
                        RetVal = True
                    Else
                        _MyState = _IDLE
                    End If
                Else
                    _WakeUpSignSuccesfullySended = True
                    If _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy) Then
                        _TimeOfLastTransfer = Now
                        RetVal = True
                    Else
                        _MyState = _IDLE
                    End If
                End If
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return RetVal
        End Try
    End Function

    ''' <summary>
    ''' Start data transfer using procedure with wake character (used by FH40G and RadEye)
    ''' This function returns when both, wake character and command string, are sent.
    ''' </summary>
    ''' <param name="TimeOutWakeUp">Time intervall which is used for waiting for the response to the wake character (in ms)</param>
    ''' <param name="Command">Command text to be sent to the device</param>
    ''' <param name="SendFlag">Flag, if the command should really be sent (0/1)</param> 
    ''' <param name="SetBusy">soll die Kommunikation blockieren bis eine vorherige abgeschlossen wurde</param>
    ''' <param name="WaitTimeMS">Delay time before sending (=-1 -> no delay)</param>
    ''' <remarks></remarks>
    Public Function StartTransferWakeUpComplete(ByVal TimeOutWakeUp As Int16, ByVal Command As String, ByVal SendFlag As Integer, ByVal SetBusy As Boolean, ByVal Repeated_Transfer As Boolean, Optional ByVal WaitTimeMS As Integer = -1) As Boolean
        Dim RetVal As Boolean = False
        Dim RecVString As New Object
        Dim ret As Integer
        Dim SendWakeUpSign As Boolean = True
        Try
            If _MyState = _IDLE Then
                If SetBusy Then _MyState = _BUSY
                _WakeUpSignLength = _WakeUpSign.Length
                _WakeUpTimeout = TimeOutWakeUp
                _CommandString = Command
                _CommandLength = Command.Length
                If Not Repeated_Transfer Then _Repeats = 0
                _SendFlag = SendFlag
                _SetBusy = SetBusy
                _CommunicationWithWakeUpSignNecessary = False
                _FullWakeUpHandling = True
                If (WaitTimeMS <> -1) And (WaitTimeMS > 0) Then
                    WarteMs(WaitTimeMS)
                End If
                If _TimeUntilNextWakeUpSign > 0 Then
                    If Now.Subtract(_TimeOfLastTransfer).TotalMilliseconds >= _TimeUntilNextWakeUpSign Then
                        SendWakeUpSign = True
                    Else
                        SendWakeUpSign = False
                    End If
                End If
                If SendWakeUpSign Then
                    _Interface.Timeout = _WakeUpTimeout
                    _Interface.LockDataReceivedEvent = True 'Das Warten auf Antwortzeichen findet in jedem Fall synchron statt.
                    'Würde ein Event an die oberen Schichten gefeuert, kommt es zu einem Fehler, da eigentlich erst ein Event ankommen soll wenn
                    'die Antwort auf das eigentliche Kommando gekommen ist. Wird das Event schon für die Anwort auf das Weckzeichen ausgelöst so wird dieses
                    'für die Antwort auf das Weckzeichen gehalten.
                    If Not _Interface.DoTransferStr(_WakeUpSign, _WakeUpSignLength, _SendFlag, _SetBusy) Then
                        _MyState = _IDLE
                        Return False
                    End If
                    Do
                        ret = _Interface.ReadDataSynchron(RecVString)
                        If ret > 0 Then
                            If RecVString.ToString = _WakeUpAnswer Then    'Weckzeichen empfangen
                                _Interface.Timeout = _TimeOut
                                _Interface.LockDataReceivedEvent = False
                                If _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy) Then
                                    _TimeOfLastTransfer = Now
                                    RetVal = True
                                Else
                                    _MyState = _IDLE
                                End If
                                Exit Do
                            End If
                        ElseIf ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_BUSY Then
                            'wait
                        ElseIf (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT) _
                            Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM) _
                            Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION) _
                            Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_PARITY) _
                            Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_NAK) _
                            Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK) Then
                            _MyState = _IDLE
                            RetVal = False
                            _Interface.LockDataReceivedEvent = False
                            Exit Do
                        End If
                    Loop
                Else
                    If _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy) Then
                        _TimeOfLastTransfer = Now
                        RetVal = True
                    Else
                        _MyState = _IDLE
                    End If
                End If
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return RetVal
        End Try
    End Function
#End Region

#Region "Synchron"

    ''' <summary>
    ''' Reads the Available data from the socket and checks if they are correct (Protocol)
    ''' </summary>
    ''' <returns>
    ''' >0 = number of databytes correctly received
    '''  0 = still waiting for correct answer
    ''' -1 = timeout
    ''' -2 = checksum error
    ''' -3 = parity error
    ''' -4 = NAK error
    ''' -10 = Exception
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ReadData() As Integer
        Dim ReturnValue As Integer = 0
        Dim ret As Integer = 0
        Dim RecData As New Object

        Try
            ret = _Interface.ReadDataSynchron(RecData)
            If ret > 0 Then
                _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
                If _CommunicationWithWakeUpSignNecessary And (Not _WakeUpSignSuccesfullySended) Then
                    _WakeUpSignSuccesfullySended = True
                    _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy)
                Else
                    _MyState = _IDLE
                    ReturnValue = ret
                End If
            ElseIf ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_BUSY Then
                ReturnValue = ret
            ElseIf (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT) _
                Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM) _
                Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_PARITY) _
                Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_NAK) Then
                If _Repeats < _RepeatCount Then

                    _Repeats += 1
                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                    _Diagnose.CounterRepeats = _Diagnose.CounterRepeats + 1

                    Select Case ret

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_PARITY
                            _Diagnose.CounterParityError = _Diagnose.CounterParityError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                            _Diagnose.CounterChecksumError = _Diagnose.CounterChecksumError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_NAK
                            _Diagnose.CounterNAKError = _Diagnose.CounterNAKError + 1

                    End Select

                    Select Case _Interface.MyComMode

                        Case ThermoIOControl_General.ComMode.CM_ARRAY
                            _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag, _SetBusy)

                        Case ThermoIOControl_General.ComMode.CM_STRING
                            If _CommunicationWithWakeUpSignNecessary Then
                                _WakeUpSignSuccesfullySended = False 'ganze Sendung nochmal von vorne beginnen inkl. Weckzeichen
                                _Interface.DoTransferStr(_WakeUpSign, _WakeUpSignLength, _SendFlag, _SetBusy)
                            ElseIf _FullWakeUpHandling Then 'ganze Sendung nochmal von vorne beginnen inkl. Weckzeichen
                                _MyState = _IDLE 'StartTransfer... funzt nur wenn ich im IDLE Zustand bin!, Wird sofort wieder umgesetzt
                                StartTransferWakeUpComplete(_WakeUpTimeout, _CommandString, _SendFlag, _SetBusy, True)
                            Else
                                _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy)
                            End If
                    End Select
                    ReturnValue = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_BUSY
                Else
                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                    Select Case ret

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_PARITY
                            _Diagnose.CounterParityError = _Diagnose.CounterParityError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                            _Diagnose.CounterChecksumError = _Diagnose.CounterChecksumError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_NAK
                            _Diagnose.CounterNAKError = _Diagnose.CounterNAKError + 1

                        Case ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT
                            _Diagnose.CounterTimeouts = _Diagnose.CounterTimeouts + 1

                    End Select
                    _MyState = _IDLE
                    ReturnValue = ret
                End If
            ElseIf (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION) _
                Or (ret = ThermoIOControl_General.ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK) Then 'keine Wdh. bei Fehler und SpecialNAK
                _MyState = _IDLE
                ReturnValue = ret
            End If
            Return ReturnValue
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

#End Region

#End Region

End Class
