#Region "Imports"
Imports System.Threading
Imports System.ComponentModel

#End Region



''' <summary>
''' Der Macher schlechthin! ;-}
''' WICHTIG: Möglichst keine Events von hier feuern, diese führen zu einem umschalten des Threadkontextes was nicht unbedingt dienlich ist.
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3_SPSStatemachine

#Region "Private Felder"

    'Status
    Public Const S_IDLE As Integer = 1
    Public Const S_MEAS As Integer = 2
    Private _MyState As Integer

    'Tasks -> Aufgaben die innerhalb eines Zustandes erledigt werden müssen
    Public Const _SynchronJob As Integer = 111
    Public Const _WaitForAnswerFromSPS As Integer = 112
    Public Const _WaitForAsyncAnswerFromSPS As Integer = 113
    Public _TASK As Integer

    'TransferStatus
    Public Enum TransferStates
        TS_OK = 1
        TS_ERROR = 2
        TS_NAK = 4
    End Enum
    Private _MyTransferState As TransferStates

    'Messflag
    Private _TimeForSynchronJob As Boolean 'zum Synchronisieren der Geräte

    'Stack
    Private _CommandStack As New List(Of SpsCommand)
    Private _WaitTillStackIsEmpty As Boolean

    'Befehle

    Public Enum Commands
        CM_Undefined = 0
        CM_SetFilterstep = 1
        CM_DetectorheadOff = 2
        CM_DetectorHeadOn = 3
        CM_HeatingOn = 4
        CM_HeatingOff = 5
        CM_PumpOn = 6
        CM_PumpOff = 7
        CM_AlarmOn = 8
        CM_AlarmOff = 9
        CM_BypassOn = 10
        CM_BypassOff = 11
        CM_ErrorOn = 12
        CM_ErrorOff = 13
        CM_GetDigStates = 14
        CM_GetAnaStates = 15
        CM_MaintenanceOn = 16
        CM_MainteneanceOff = 17
        CM_EcoolerOn = 18
        CM_EcoolerOff = 19
        CM_SetFactorBezel = 20
        CM_SetAirFlowSetpoint = 21
        CM_GetCalculations = 22
        CM_InitEnd = 99
    End Enum

    'Initialisierung
    Private _InitOKCounter As Integer = 0
    Private _InitCommandCounter As Integer = 0

    'Sonstiges
    Private _MySPSCommunication As FHT59N3_SPSCommunication
    Private _InitializationCommandsSuccesfullyPutted As Boolean = True

    Private Const _ERRCNTS = 10
    Private _ErrorCounter As Integer = 0


#End Region

#Region "Öffentliche Felder"

    Public Property MyState() As Integer
        Get
            Return _MyState
        End Get
        Set(ByVal value As Integer)
            _MyState = value
        End Set
    End Property

    Public ReadOnly Property MyTransferState() As TransferStates
        Get
            Return _MyTransferState
        End Get
    End Property

    Public ReadOnly Property MySPSCommunication() As FHT59N3_SPSCommunication
        Get
            Return _MySPSCommunication
        End Get
    End Property

    Public Property TimeForSynchronJob() As Boolean
        Get
            Return _TimeForSynchronJob
        End Get
        Set(ByVal value As Boolean)
            If _MyState = S_MEAS Then
                _TimeForSynchronJob = value
            End If
        End Set
    End Property

#End Region

#Region "Private Methoden"

    ''' <summary>
    ''' Gets asyncron command from list and start sending to FHT681
    ''' </summary>
    ''' <returns>1 if ok, -1 if command cannto be sent, -2 if initialisation command sequence failed </returns>
    ''' <remarks></remarks>
    Private Function SendAsynchronCommand() As Integer
        Try
            SendAsynchronCommand = 1    'default return value: command successfully sent

            Dim actualSpsCommand As SpsCommand = GetCommandFromStack()
            'we know we put command in queue...
            'If (actualSpsCommand Is Nothing) Then
            '    Exit Function
            'End If

            Dim _ActualAsynchronCommand As Commands = actualSpsCommand.CommandId
            Dim _ActualAsynchronParam1 As Object = actualSpsCommand.Param1
            Dim _ActualAsynchronParam2 As Object = actualSpsCommand.Param2
            Dim _ActualAsynchronParam3 As Object = actualSpsCommand.Param3
            Dim _ActualAsynchronParam4 As Object = actualSpsCommand.Param4

            If _ActualAsynchronCommand <> Commands.CM_InitEnd Then   'if a real command should be sent
                If _MySPSCommunication.CommunicationEstablished = False Then   'Com Port not opened.
                    SendAsynchronCommand = -1    'command could not be sent
                    Trace.TraceError("command " & _ActualAsynchronCommand.ToString() & " could not be sent, no connection!")
                    Exit Function
                End If
            End If

            Select Case _ActualAsynchronCommand

                Case Commands.CM_SetFilterstep
                    Thread.Sleep(1000)
                    _MySPSCommunication.SetFilterstep(CType(_ActualAsynchronParam1, Integer))

                Case Commands.CM_DetectorheadOff
                    _MySPSCommunication.DetectorheadOff()

                Case Commands.CM_DetectorHeadOn
                    _MySPSCommunication.DetectorheadOn()

                Case Commands.CM_HeatingOn
                    _MySPSCommunication.HeatingOn()

                Case Commands.CM_HeatingOff
                    _MySPSCommunication.HeatingOff()

                Case Commands.CM_PumpOn
                    _MySPSCommunication.PumpOn()

                Case Commands.CM_PumpOff
                    _MySPSCommunication.PumpOff()

                Case Commands.CM_AlarmOn
                    _MySPSCommunication.AlarmRelaisOn()

                Case Commands.CM_AlarmOff
                    _MySPSCommunication.AlarmRelaisOff()

                Case Commands.CM_BypassOn
                    _MySPSCommunication.BypassOn()

                Case Commands.CM_BypassOff
                    _MySPSCommunication.BypassOff()

                Case Commands.CM_ErrorOn
                    _MySPSCommunication.ErrorOn()

                Case Commands.CM_ErrorOff
                    _MySPSCommunication.ErrorOff()

                Case Commands.CM_GetDigStates
                    _MySPSCommunication.GetDigStates()

                Case Commands.CM_GetAnaStates
                    _MySPSCommunication.GetAnaStates()

                Case Commands.CM_GetCalculations
                    _MySPSCommunication.GetCalculations()

                Case Commands.CM_MaintenanceOn
                    _MySPSCommunication.MaintenanceOn()

                Case Commands.CM_MainteneanceOff
                    _MySPSCommunication.MaintenanceOff()

                Case Commands.CM_EcoolerOn
                    _MySPSCommunication.EcoolerOn()

                Case Commands.CM_EcoolerOff
                    _MySPSCommunication.EcoolerOff()

                Case Commands.CM_SetFactorBezel
                    _MySPSCommunication.SetFactorBezel(CType(_ActualAsynchronParam1, Double))

                Case Commands.CM_SetAirFlowSetpoint
                    _MySPSCommunication.SetAirFlowSetpoint(CType(_ActualAsynchronParam1, Boolean), CType(_ActualAsynchronParam2, Integer))

                Case Commands.CM_InitEnd 'this is not a real command. It is the last request during the initialisation.
                    'If the program reaches here, it starts the measurement automatically if autostart is configured
                    If _InitOKCounter < _InitCommandCounter Then  'then at least one of the initialisation commands to FHT681 failed
                        SendAsynchronCommand = -2    'Init application failed
                    Else    'Initialisation OK
                        _MyState = S_MEAS 'Start the Measurement
                        SendAsynchronCommand = 2
                    End If

            End Select

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Private Function HandleAsyncReceive() As Integer
        Try
            If _InitOKCounter < _InitCommandCounter Then
                _InitOKCounter += 1
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Private Function HandleReceiveError(ByVal Async As Boolean) As Integer
        Try
            _ErrorCounter = _ErrorCounter + 1
            If _ErrorCounter >= _ERRCNTS Then
                _MyTransferState = TransferStates.TS_ERROR
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Private Function HandleCheckSumError(ByVal Async As Boolean) As Integer
        Try
            _MyTransferState = TransferStates.TS_NAK

            'End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks>for internal use only!</remarks>
    Private Sub New()
        _MyState = S_IDLE
        _MyTransferState = TransferStates.TS_OK
        _ErrorCounter = 0
        _TimeForSynchronJob = False
        _WaitTillStackIsEmpty = False
        _TASK = _SynchronJob
    End Sub

    ''' <summary>
    ''' Constructor for use with serial communication to SPS
    ''' </summary>
    ''' <param name="ComPort"></param>
    ''' <param name="UseStxEtxProtocol"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal ComPort As String, ByVal UseStxEtxProtocol As Boolean)
        Me.New()

        Try
            _MySPSCommunication = New FHT59N3_SPSCommunication(ComPort, UseStxEtxProtocol) ', Me
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Constructor for use with network communication via TCP/IP to SPS
    ''' </summary>
    ''' <param name="NetworkAddress"></param>
    ''' <param name="RemotePort"></param>
    ''' <param name="UseStxEtxProtocol"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal NetworkAddress As String, ByVal RemotePort As UInt16, ByVal UseStxEtxProtocol As Boolean)
        Me.New()

        Try
            _MySPSCommunication = New FHT59N3_SPSCommunication(NetworkAddress, RemotePort, UseStxEtxProtocol) ', Me
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Dispose()
        Try
            _MySPSCommunication.Dispose()
            _MySPSCommunication = Nothing
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub InitSPS()
        Try
            Dim ret As Boolean
            _InitCommandCounter = 2  'Anzahl der notwendigen asynchronen Befehle für Initialisierung (ohne Endekennung INIT681END)
            _InitOKCounter = 0  'Anzahl der gültig empfangenen Befehle für Initialisierung rücksetzen

            'sehr wichtig das die Digitalwert/Analogwertabfragen zuerst gesendet werden weil später auf den anderen Befehlen teilweise
            'eine Delta-Prüfung stattfindet!
            ret = PushCommand(Commands.CM_GetDigStates, Nothing, Nothing, Nothing, Nothing)
            If ret = False Then
                _InitializationCommandsSuccesfullyPutted = False
                Exit Sub
            End If

            ret = PushCommand(Commands.CM_GetAnaStates, Nothing, Nothing, Nothing, Nothing)
            If ret = False Then
                _InitializationCommandsSuccesfullyPutted = False
                Exit Sub
            End If

            'ret = PutCommand(Commands.CM_GetCalculations, Nothing, Nothing, Nothing, Nothing)
            'If ret = False Then
            '    _InitializationCommandsSuccesfullyPutted = False
            '    Exit Sub
            'End If

            ret = PushCommand(Commands.CM_InitEnd, Nothing, Nothing, Nothing, Nothing)
            If ret = False Then
                _InitializationCommandsSuccesfullyPutted = False
                Exit Sub
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub DoMyJobs()
        Dim ret As Integer
        Try

            If (Not _InitializationCommandsSuccesfullyPutted) And (_MyState = S_IDLE) Then
                If StackIsEmpty() Then
                    InitSPS()
                End If
            End If


            Select Case _TASK

                Case _SynchronJob
                    If (_MySPSCommunication._DataTransferState <> FHT59N3_SPSCommunication.TR_BUSY) Then
                        If _TimeForSynchronJob Then
                            _TimeForSynchronJob = False
                        Else    'if now is free time for asynchronous transfers to be started
                            If Not StackIsEmpty() Then  'asynchronous transfer requested?
                                ret = SendAsynchronCommand()

                                If ret = 1 Then
                                    _TASK = _WaitForAsyncAnswerFromSPS
                                ElseIf ret = -1 Then 'Fehler beim Senden (bedeutet Netzwerkverbindung steht nicht)
                                    HandleReceiveError(True)
                                    'TASK bleibt _SendQuestionToFHT681
                                ElseIf ret = -2 Then 'error, initialisation command sequence could not be sent
                                    InitSPS() 'wieder versuchen
                                    'TASK bleibt _SendQuestionToFHT681
                                ElseIf ret = 2 Then 'Initialisation bei Programmstart ist erfolgreich abgeschlossen
                                    'TASK bleibt _SendQuestionToFHT681
                                Else
                                    'TASK bleibt _SendQuestionToFHT681
                                End If
                            End If
                        End If
                    End If

                Case _WaitForAnswerFromSPS   'Wait for cyclic response to counter request command

                    _MySPSCommunication.CheckIfDataReceived()

                    If _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_OK Or _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_ERROR Then
                        'Datenübertragung OK oder Übertragungsfehler
                        If _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_OK Then
                            _MyTransferState = TransferStates.TS_OK
                            _ErrorCounter = 0
                            If Not StackIsEmpty() Then  'asynchronous transfer requested? gleich nochmal senden
                                ret = SendAsynchronCommand()
                                If ret = 1 Then 'Senden OK
                                    _TASK = _WaitForAsyncAnswerFromSPS
                                ElseIf ret = -1 Then 'Fehler beim Senden (bedeutet Netzwerkverbindung steht nicht)
                                    HandleReceiveError(True)
                                    _TASK = _SynchronJob
                                ElseIf ret = -2 Then 'error, initialisation command sequence could not be sent
                                    InitSPS() 'wieder versuchen
                                    _TASK = _SynchronJob
                                ElseIf ret = 2 Then 'Initialisation bei Programmstart ist erfolgreich abgeschlossen
                                    _TASK = _SynchronJob
                                Else
                                    _TASK = _SynchronJob
                                End If
                            Else
                                _TASK = _SynchronJob
                            End If
                        Else    'Transferfehler
                            If _MySPSCommunication.ReceiveReturn < 0 And (Not _MySPSCommunication.ReceiveReturn = -3) Then
                                HandleReceiveError(False)
                            End If
                            _TASK = _SynchronJob
                        End If
                    End If

                Case _WaitForAsyncAnswerFromSPS
                    _MySPSCommunication.CheckIfDataReceived()
                    If _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_OK Or _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_ERROR Then
                        'Datenübertragung OK oder Übertragungsfehler
                        If _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_OK Then  'Datentransfer OK
                            _MyTransferState = TransferStates.TS_OK
                            _ErrorCounter = 0
                            HandleAsyncReceive()
                            If Not _TimeForSynchronJob Then
                                If Not StackIsEmpty() Then  'asynchronous transfer requested?
                                    ret = SendAsynchronCommand()
                                    If ret = 1 Then 'Senden OK
                                    ElseIf ret = -1 Then 'Fehler beim Senden (bedeutet Netzwerkverbindung steht nicht)
                                        HandleReceiveError(True)
                                        _TASK = _SynchronJob
                                    ElseIf ret = -2 Then 'error, initialisation command sequence could not be sent
                                        InitSPS() 'wieder versuchen
                                        _TASK = _SynchronJob
                                    ElseIf ret = 2 Then 'Initialisation bei Programmstart ist erfolgreich abgeschlossen
                                        _TASK = _SynchronJob
                                    Else
                                        _TASK = _SynchronJob
                                    End If
                                Else
                                    _TASK = _SynchronJob 'nix mehr zu tun
                                End If
                            Else
                                _TimeForSynchronJob = False
                            End If

                        Else
                            If _MySPSCommunication.ReceiveReturn < 0 And (Not _MySPSCommunication.ReceiveReturn = -3) Then
                                HandleReceiveError(True)
                            End If
                            _TASK = _SynchronJob 'Hier im Gegensatz zum 1702 wieder umschalten, denn der 2 Sekunden Takt hat absoluten Vorrang!
                        End If
                    ElseIf _MySPSCommunication._DataTransferState = FHT59N3_SPSCommunication.TR_NAK Then
                        HandleCheckSumError(True)
                    End If

            End Select

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Asynchrone Befehle -- Stackverwaltung"

    Public Sub ForceCommandStackFlush()
        _CommandStack.Clear()
    End Sub

    Public Sub WaitTillStackEmpty(ByVal secondsToWait As Integer)

        _WaitTillStackIsEmpty = True
        Dim futureWaitDate As Date = DateTime.Now.AddSeconds(secondsToWait)

        While (Not StackIsEmpty())
            Thread.Sleep(2)
            Dim tooLate As Boolean = DateTime.Compare(DateTime.Now, futureWaitDate) <= 0
            If (tooLate) Then
                Exit While
            End If
        End While

    End Sub


    Private Function StackIsEmpty() As Boolean
        Try
            If _CommandStack.Count > 0 Then
                Return False
            Else
                _WaitTillStackIsEmpty = False
                Return True
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function PutCommand(ByVal CommandToQueue As Commands, ByVal Param1 As Object, ByVal Param2 As Object, _
                               ByVal Param3 As Object, ByVal Param4 As Object) As Boolean
        Try
            If (_CommandStack.Count < 40) And (Not _WaitTillStackIsEmpty) Then

                'Doppelte synchrone Befehle unterbinden, sonst werden unnötig Messwertabfragen aufgestapelt...
                For Each CommandInStack As SpsCommand In _CommandStack
                    If CommandInStack.CommandId = CommandToQueue Then
                        Return False
                    End If
                Next

                Dim spsCommand As SpsCommand = New SpsCommand
                spsCommand.CommandId = CommandToQueue
                spsCommand.Param1 = Param1
                spsCommand.Param2 = Param2
                spsCommand.Param3 = Param3
                spsCommand.Param4 = Param4

                _CommandStack.Add(spsCommand)
                Return True
            Else
                _WaitTillStackIsEmpty = True
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        Return False
    End Function


    Public Function PushCommand(ByVal Command As Commands, ByVal Param1 As Object, ByVal Param2 As Object, _
                               ByVal Param3 As Object, ByVal Param4 As Object) As Boolean
        Try
            If (_CommandStack.Count < 40) And (Not _WaitTillStackIsEmpty) Then

                Dim spsCommand As SpsCommand = New SpsCommand
                spsCommand.CommandId = Command
                spsCommand.Param1 = Param1
                spsCommand.Param2 = Param2
                spsCommand.Param3 = Param3
                spsCommand.Param4 = Param4
                spsCommand.Prioritized = True

                'Sicherstellen das hochpriore Befehle (z.B. asynchrone Benutzereingaben oder SPS-Initialisierungsbefehle) nicht verdrängt werden können
                Dim wasInserted As Boolean = False
                For Idx As Integer = 0 To _CommandStack.Count - 1
                    If Not _CommandStack(Idx).Prioritized Then
                        _CommandStack.Insert(Idx, spsCommand)
                        wasInserted = True
                        Exit For
                    End If
                Next
                If Not wasInserted Then
                    _CommandStack.Add(spsCommand)
                End If

                Return True
            Else
                _WaitTillStackIsEmpty = True
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        Return False
    End Function

    Public Function GetCommandFromStack() As SpsCommand
        Try
            Dim spsCommand As SpsCommand
            spsCommand = _CommandStack.Item(0)
            _CommandStack.RemoveAt(0)
            Return spsCommand
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        Return Nothing

    End Function



#End Region

End Class


Public Class SpsCommand
    Public Property CommandId As FHT59N3_SPSStatemachine.Commands
    Public Property Param1 As Object
    Public Property Param2 As Object
    Public Property Param3 As Object
    Public Property Param4 As Object
    Public Property Prioritized As Boolean
End Class