'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

#Region "Imports"
Imports ThermoIOControls
#End Region

''' <summary>
''' Handelt eine Datenabfrage ab; Timeout und Repeats werden hier verwaltet
''' </summary>
''' <remarks></remarks>
Public Class ThermoDataCollector_WithOutEvents

#Region "Private Felder"
    '$ Private Felder
    Private WithEvents _Interface As ThermoIOControl_Ethernet_WithOutEvents     'Interface zum Gerät
    Private _Repeats As Integer
    Private _RepeatCount As Integer 'gelten für alle Geräte
    Private _Command() As Byte
    Private _CommandString As String
    Private _CommandLength As Integer
    Private _SendFlag As Integer '0= nicht senden, 1= senden
    Private _SetBusy As Boolean

    Private _MyState As Integer
    Private Const _IDLE As Integer = 0
    Private Const _BUSY As Integer = 1

    Private _TransferMode As Integer
    Private _ModeAr As Integer = 1
    Private _ModeStr As Integer = 2
    Private _AliveCheckIntervall As Integer

    'Diagnose
    Private _Diagnose As ThermoConnectionDiagnosis

#End Region

#Region "Öffentliche Eigenschaften"
    '$ Öffentliche Eigenschaften

    ''' <summary>
    ''' Verbindungsdiagnose
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
    ''' Timeout
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Timeout() As Integer
        Get
            Return _Interface.Timeout
        End Get
        Set(ByVal value As Integer)
            _Interface.Timeout = value
        End Set
    End Property

    Public ReadOnly Property DisconnectedFlag() As Boolean
        Get
            Return _Interface.DisconnectedFlag
        End Get
    End Property
#End Region

#Region "Öffentliche Methoden"
    '$ Öffentliche Methoden

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="MyInterface">Interface zur RS232/Ethernet</param>
    ''' <param name="Repeats">Wieviele Wiederholungen beim Timeout</param>
    ''' <param name="AliveCheckIntervall">Aller wieiviel millisekunden wird geschaut ob die Verbindung noch lebt?</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal MyInterface As ThermoIOControl_Ethernet_WithOutEvents, ByVal Repeats As Integer, ByVal AliveCheckIntervall As Integer)
        _RepeatCount = Repeats
        _Interface = MyInterface
        _MyState = _IDLE
        _AliveCheckIntervall = AliveCheckIntervall
        _Diagnose = New ThermoConnectionDiagnosis(_AliveCheckIntervall)
    End Sub

    ''' <summary>
    ''' Abfrage abschicken
    ''' </summary>
    ''' <param name="Command">Befehl an das Gerät</param>
    ''' <param name="ComLength">Länge des Befehls im Puffer</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Function SendQuestion(ByVal Command As Array, ByVal ComLength As Integer, ByVal SendFlag As Integer, ByVal SetBusy As Boolean) As Boolean
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
                _TransferMode = _ModeAr
                If _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag, SetBusy) Then
                    RetVal = True
                End If
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return RetVal
        End Try
    End Function

    ''' <summary>
    ''' Abfrage abschicken
    ''' </summary>
    ''' <param name="Command">Befehl an das Gerät</param>
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Function SendQuestion(ByVal Command As String, ByVal SendFlag As Integer, ByVal SetBusy As Boolean) As Boolean
        Dim RetVal As Boolean = False
        Try
            If _MyState = _IDLE Then
                If SetBusy Then _MyState = _BUSY
                _CommandString = Command
                _CommandLength = Command.Length
                _Repeats = 0
                _SendFlag = SendFlag
                _SetBusy = SetBusy
                _TransferMode = _ModeStr
                If _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, SetBusy) Then
                    RetVal = True
                End If
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return RetVal
        End Try
    End Function

    ''' <summary>
    ''' Reads the Available data from the socket and checks if they are correct (Protocol)
    ''' </summary>
    ''' <returns>
    ''' 0 = still waiting for correct answer
    ''' 1 = correct answer received
    ''' 2 = timeout
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ReadData() As Integer
        Dim ReturnValue As Integer = 0
        Dim ret As Integer = 0
        Try
            ret = _Interface.IsTimeOut
            If ret = 0 Then
                'kein Timeout, Daten lesen
                If _Interface.ReadData Then
                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                    _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
                    _MyState = _IDLE
                    ReturnValue = 1
                End If
            ElseIf ret = 1 Then
                'Timeout, Befehl nochmal senden
                If _Repeats < _RepeatCount Then
                    _Repeats += 1
                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                    _Diagnose.CounterRepeats = _Diagnose.CounterRepeats + 1
                    Select Case _TransferMode

                        Case _ModeAr
                            _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag, _SetBusy)

                        Case _ModeStr
                            _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag, _SetBusy)

                    End Select
                    ReturnValue = 0
                Else
                    _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                    _Diagnose.CounterTimeouts = _Diagnose.CounterTimeouts + 1
                    _MyState = _IDLE
                    ReturnValue = 2
                End If
            ElseIf ret = 2 Then
                'kein Timeout, Daten wurden schon gelesen
                _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
                _MyState = _IDLE
                ReturnValue = 1
            End If
            Return ReturnValue
        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

#End Region

End Class

