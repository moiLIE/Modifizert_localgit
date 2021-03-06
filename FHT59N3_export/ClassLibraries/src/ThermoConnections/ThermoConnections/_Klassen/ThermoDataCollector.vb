'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

Option Strict Off

#Region "Imports"
Imports ThermoInterfaces
Imports System.IO.Ports
#End Region

''' <summary>
''' Handelt eine Datenabfrage ab; Timeout und Repeats werden hier verwaltet
''' </summary>
''' <remarks></remarks>
Public Class ThermoDataCollector
    Implements ThermoDataCollector_Interface

#Region "Private Felder"
    '$ Private Felder
    Private WithEvents _Interface As ThermoIOControls_Interface     'Interface zum Ger�t
    Private _Repeats As Integer
    Private _ReceiveBuffer(1000) As Byte
    Private _TimeOut As Integer 'gelten f�r alle Ger�te
    Private _RepeatCount As Integer 'gelten f�r alle Ger�te
    Private _Command() As Byte
    Private _CommandString As String
    Private _CommandLength As Integer
    Private _SendFlag As Integer '0= nicht senden, 1= senden

    Private _MyState As Integer
    Private Const _IDLE As Integer = 0
    Private Const _BUSY As Integer = 1

    Private _TransferMode As Integer
    Private _ModeAr As Integer = 1
    Private _ModeStr As Integer = 2
    Private _AliveCheckIntervall As Integer

    'Diagnose
    Private _Diagnose As ThermoConnectionDiagnosis

    'Fehler
    Private _ErrorsTillStopCommunication As Integer
    Private _ErrorCount As Integer
    Private _StopCommunication As Boolean

#End Region

#Region "�ffentliche Eigenschaften"
    '$ �ffentliche Eigenschaften

    ''' <summary>
    ''' Anzahl der Wiederholungen; wird bei New mit eingestellt
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Repeats() As Integer Implements ThermoDataCollector_Interface.Repeats
        Get
            Return _RepeatCount
        End Get
    End Property

    ''' <summary>
    ''' Anzahl der tats�chlich gemachten Wiederholungen;
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ActRepeats() As Integer Implements ThermoDataCollector_Interface.ActRepeats
        Get
            Return _Repeats
        End Get
    End Property

    ''' <summary>
    ''' Wieviele Fehler d�rfen passieren bevor ich die Kommunikation unterbreche
    ''' -1 = aus
    ''' 0-?
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ErrorsTillStopCommunication() As Integer Implements ThermoInterfaces.ThermoDataCollector_Interface.ErrorsTillStopCommunication
        Get
            Return _ErrorsTillStopCommunication
        End Get
        Set(ByVal value As Integer)
            _ErrorsTillStopCommunication = value
        End Set
    End Property

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

    ''' <summary>
    ''' Event das ausgel�st wird wenn die �bertragung beendet wurde 
    ''' </summary>
    ''' <remarks>
    ''' 0 = ok
    ''' 1 = Timeout
    '''</remarks>
    Public Event DataCollectorReadyAr(ByVal ErrorState As Integer) Implements ThermoDataCollector_Interface.DataCollectorReadyAr

    ''' <summary>
    ''' Event das ausgel�st wird wenn die �bertragung beendet wurde 
    ''' </summary>
    ''' <param name="ErrorState">
    ''' 0 = ok
    ''' 1 = Timeout
    ''' 2 = Keine Daten�bertragung mehr!</param>
    ''' <remarks></remarks>
    Public Event DataCollectorReadyStr(ByVal ErrorState As Integer) Implements ThermoDataCollector_Interface.DataCollectorReadyStr

    ''' <summary>
    ''' Fehler
    ''' </summary>
    ''' <param name="ex">
    ''' Exception Klasse
    ''' </param>
    ''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As Exception) Implements ThermoDataCollector_Interface.ErrorEvent

    ''' <summary>
    ''' eine Verbindung zu einem Ger�t wurde unterbrochen
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Disconnected() Implements ThermoInterfaces.ThermoDataCollector_Interface.Disconnected

#End Region

#Region "Private Methoden"
    '$ Private Methoden

    ''' <summary>
    ''' Fehlerbehandlung
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")>
    Private Sub ErrorHandler(ByVal ex As Exception) Handles _Interface.ErrorEvent
        'RaiseEvent ErrorEvent(ex)
    End Sub

    ''' <summary>
    ''' Daten empfangen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataOKAr() Handles _Interface.DataReceivedEventAr
        Try
            _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
            _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
            _MyState = _IDLE
            If _ErrorsTillStopCommunication > -1 Then
                If _ErrorCount > 0 Then
                    _ErrorCount = _ErrorCount - 1
                End If
            End If
            RaiseEvent DataCollectorReadyAr(0)
        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Daten empfangen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataOKStr() Handles _Interface.DataReceivedEventStr
        Try
            _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
            _Diagnose.CounterGoodQueries = _Diagnose.CounterGoodQueries + 1
            _MyState = _IDLE
            If _ErrorsTillStopCommunication > -1 Then
                If _ErrorCount > 0 Then
                    _ErrorCount = _ErrorCount - 1
                End If
            End If
            RaiseEvent DataCollectorReadyStr(0)
        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
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

    ''' <summary>
    ''' Timeout verwalten, wiederholen oder Fehler
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataNotOK() Handles _Interface.TimeOutEvent
        Try
            If _Repeats < _RepeatCount Then
                _Repeats += 1

                _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                _Diagnose.CounterRepeats = _Diagnose.CounterRepeats + 1

                Select Case _TransferMode

                    Case _ModeAr

                        _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag)

                    Case _ModeStr

                        _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag)

                End Select

            Else

                _Diagnose.CounterTotalQueries = _Diagnose.CounterTotalQueries + 1
                _Diagnose.CounterTimeouts = _Diagnose.CounterTimeouts + 1

                _MyState = _IDLE

                Select Case _TransferMode

                    Case _ModeAr

                        RaiseEvent DataCollectorReadyAr(1)

                    Case _ModeStr

                        RaiseEvent DataCollectorReadyStr(1)

                End Select

                If _ErrorsTillStopCommunication > -1 Then
                    _ErrorCount = _ErrorCount + 1
                    If _ErrorCount >= _ErrorsTillStopCommunication Then
                        _StopCommunication = True
                    End If
                End If

            End If

        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "�ffentliche Methoden"
    '$ �ffentliche Methoden

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="MyInterface">Interface zur RS232/Ethernet</param>
    ''' <param name="Repeats">Wieviele Wiederholungen beim Timeout</param>
    ''' <param name="AliveCheckIntervall">Aller wieiviel millisekunden wird geschaut ob die Verbindung noch lebt?</param>
    ''' <param name="ErrorsTillStopCommunication">Wieviele Fehler d�rfen passieren bevor ich die Kommunikation unterbreche(-1 = aus; 0-?</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal MyInterface As ThermoIOControls_Interface, ByVal Repeats As Integer, ByVal AliveCheckIntervall As Integer, ByVal ErrorsTillStopCommunication As Integer)
        _RepeatCount = Repeats
        _Interface = MyInterface
        _MyState = _IDLE
        _AliveCheckIntervall = AliveCheckIntervall
        _Diagnose = New ThermoConnectionDiagnosis(_AliveCheckIntervall)
        _ErrorsTillStopCommunication = ErrorsTillStopCommunication
        _StopCommunication = False
    End Sub

    ''' <summary>
    ''' Abfrage abschicken
    ''' </summary>
    ''' <param name="Command">Befehl an das Ger�t</param>
    ''' <param name="ComLength">L�nge des Befehls im Puffer</param> 
    ''' <param name="SendFlag">Soll der Befehl tats�chlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Sub SendQuestion(ByVal Command As Array, ByVal ComLength As Integer, ByVal SendFlag As Integer) Implements ThermoDataCollector_Interface.SendQuestion
        Try
            If _MyState = _IDLE Then
                _MyState = _BUSY
                ReDim _Command(Command.Length)
                Command.CopyTo(_Command, 0)
                _CommandLength = ComLength
                _Repeats = 0
                _SendFlag = SendFlag

                _TransferMode = _ModeAr

                If Not _StopCommunication Then
                    _Interface.DoTransferAr(_Command, _CommandLength, _SendFlag)
                Else
                    _MyState = _IDLE
                    RaiseEvent DataCollectorReadyAr(2)
                End If

            End If

        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try

    End Sub

    ''' <summary>
    ''' Abfrage abschicken
    ''' </summary>
    ''' <param name="Command">Befehl an das Ger�t</param>
    ''' <param name="SendFlag">Soll der Befehl tats�chlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Sub SendQuestion(ByVal Command As String, ByVal SendFlag As Integer) Implements ThermoDataCollector_Interface.SendQuestion
        Try
            If _MyState = _IDLE Then
                _MyState = _BUSY
                _CommandString = Command
                _CommandLength = Command.Length
                _Repeats = 0
                _SendFlag = SendFlag

                _TransferMode = _ModeStr
                If Not _StopCommunication Then
                    _Interface.DoTransferStr(_CommandString, _CommandLength, _SendFlag)
                Else
                    _MyState = _IDLE
                    RaiseEvent DataCollectorReadyStr(2)
                End If

            End If
        Catch ex As Exception
            _MyState = _IDLE
            'RaiseEvent ErrorEvent(ex)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try

    End Sub

#End Region

End Class
