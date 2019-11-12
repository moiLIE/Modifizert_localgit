'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

#Region "Imports"

Imports System.Threading
Imports System.IO.Ports
Imports System.Net.Sockets
Imports ThermoInterfaces
Imports ThermoProtocols
Imports ThermoIOControls
Imports ThermoLogging

#End Region

''' <summary>
''' WICHTIG:
''' - es können immer nur Geräte gleichen Typs gleichzeitig behandelt werden
''' - kein Mischbetrieb seriell/ethernet möglich
''' - Trigger für die Maschine kommt von außen
''' SERIELL:
''' - Statusmaschine kann eine Liste von Kommandos an ein/mehrere Gerät senden, es wird solange gewartet bis
'''   das Gerät den Empfang bestätigt hat (IsReceiveReady + DataCollector), d.h. für mehrere Geräte z.B. erst Befehl 1 für
'''   Gerät 1 in die Liste, dann Befehl 1 für Gerät 2 ... usw. (Adresse wird im Befehl codiert!)
''' - ich kann auch "asynchron" was dazwischenschieben, indem ich für die Geräte die Liste ändere und 
'''   die Maschine triggere und die Liste danach wieder zurückändere oder mit der Funktion "SendAsyncCommand..."
'''   einen Befehl an ein bestimmtes Gerät sende und wieder triggere
''' - die Empfangenen Antworten werden in der Returnlist gespeichert und in der übergeordneten Instanz decodiert!
''' ETHERNET:
''' - Statusmaschine kann eine Liste von Befehlen für ein/mehrere Geräte senden (erst Befehl 1 für Gerät 1 - n, dann Befehl 2 usw)
''' - ich kann auch "asynchron" was dazwischenschieben, indem ich für die Geräte die Liste ändere und 
'''   die Maschine triggere und die Liste danach wieder zurückändere oder mit der Funktion "SendAsyncCommand..."
'''   einen Befehl an ein bestimmtes Gerät sende und wieder triggere
''' - die Empfangenen Antworten werden in der Returnlist gespeichert und in der übergeordneten Instanz decodiert!
''' </summary>
''' <remarks></remarks>
Public Class ThermoStateMachine

    '#Region "Private Eigenschaften"

    '	Private Enum States
    '		IDLE = 0
    '		BUSY = 1
    '	End Enum

    '	Private Enum YesNo
    '		NO = 0
    '		YES = 1
    '	End Enum

    '	'Allgemeines
    '	Private _MyInterval As Integer 'Wie oft kommt der Thread dran
    '	Private _MyIndex As Integer	'Index (um die maschinen evtl. in einer Liste zu verwalten)
    '	Private _Thread1 As Thread 'der macher
    '	Private WithEvents _Protocol As ThermoProtocol_Interface	'welches protokoll?
    '	Private _DataContainer As ThermoDataContainer_Interface	'allgemeiner datenconatainer für das protokoll
    '	Private _SpecialCommandStr As String 'Asynchroner Befehl
    '	Private _SpecialCommandAr As Byte()	'Asynchroner Befehl
    '	Private _SpecialCommandUncoded As String 'Asynchroner Befehl, unkodiert
    '	Private _Timeout As Integer
    '    Private _Repeats As Integer
    '    Private _ConnectionTimeout As Integer
    '    Private _LiveCounter As Long = 1 'lebt die ThreadFunctionEth noch?
    '    Private _LiveCounterOld As Long = 0 'lebt die ThreadFunctionEth noch?
    '    Private _SendErrorCounter As Long = 0 'wie oft konnte ich hintereinander nicht mehr senden?
    '    Private _AliveCheckTime As Date
    '    Private _AliveCheckIntervall As Integer

    '	'Zustände & Signale
    '	Private _Running As Boolean	'laufe ich?
    '	Private _MyState As States	'Zustand
    '	Private _MySignal As Signals 'Signal
    '	Private _OldSignal As Signals	'Kopie des Signales damit ich nachdem ich alles abgehandelt hab noch weiß was ich gemacht habe
    '	Private _Ethernet As Boolean 'Eth oder seriell

    '	'Seriell
    '	Private _Next As YesNo 'nächster befehl?
    '	Private _CommandCountSer As Integer	'Befehlszähler, damit ich weiß welchen befehl ich als nächstes abarbeiten muss
    '    Private _MySP As New System.IO.Ports.SerialPort  'Handle zum seriellen Port
    '	Private WithEvents _Interface_To_RS232 As ThermoIOControls_Interface	'Ich brauche ein Interface zum seriellen Port
    '	Private WithEvents _DataCollector As ThermoDataCollector_Interface	'ich brauche einen Datensammler
    '	Private _CommandListString As List(Of String)	'Befehlsliste
    '	Private _CommandListArray As List(Of Byte()) 'Befehlsliste
    '	Private _CommandListStringUncoded As List(Of String)	'Befehlsliste, unkodiert
    '	Private _CommandListArrayUncoded As List(Of String)	'Befehlsliste, unkodiert
    '	Private _ReturnListString As New Dictionary(Of String, String) 'Rückgaben
    '	Private _ReturnListArray As New Dictionary(Of Byte(), Byte())	'Rückgaben
    '	Private _ActualCommandString As String 'aktueller Befehl
    '	Private _ActualCommandArray As Byte()	'aktueller Befehl

    '	'Ethernet
    '	Private _SocketList As New Dictionary(Of String, Socket) 'Socketliste
    '	Private _ConnectedList As New Dictionary(Of String, Boolean)	'Verbunden?
    '	Private _DataContainerAssembly As String
    '	Private _DataContainerName As String
    '	Private _ProtocolAssembly As String
    '	Private _ProtocolName As String
    '	Private _DataContainerList As New Dictionary(Of String, ThermoDataContainer_Interface)
    '	Private _InterfaceListEth As New Dictionary(Of String, ThermoIOControl_Ethernet) 'Liste der Interfaces für den Ethernetport
    '	Private _DataCollectorList As New Dictionary(Of String, ThermoDataCollectorMulti)	'Liste der Datencollectoren
    '	Private _DeviceListEth As New Dictionary(Of String, Integer) 'Ipadresse, Port
    '	Private _CommandListStringEth As New Dictionary(Of String, List(Of String))	'Befehlslisten für die Geräte
    '	Private _CommandListArrayEth As New Dictionary(Of String, List(Of Byte())) 'Befehlslisten für die Geräte
    '	Private _CommandListStringEthUncoded As New Dictionary(Of String, List(Of String))	'Befehlslisten für die Geräte, unkodiert
    '	Private _CommandListArrayEthUncoded As New Dictionary(Of String, List(Of String))	'Befehlslisten für die Geräte, unkodiert
    '	Private _ReturnListStringEth As New Dictionary(Of String, List(Of String)) 'Rückgabelisten für die Geräte
    '	Private _ReturnListArrayEth As New Dictionary(Of String, List(Of Byte()))	'Rückgabelisten für die Geräte
    '    Private _ReturnStringEthAsync As New Dictionary(Of String, String)  'Rückgabelisten für die Geräte
    '	Private _ReturnArrayEthAsync As New Dictionary(Of String, Byte())	'Rückgabelisten für die Geräte
    '	Private _ReceivedListStringEth As New Dictionary(Of String, List(Of String)) 'Rückgabelisten für die Geräte
    '	Private _ReceivedListArrayEth As New Dictionary(Of String, List(Of Byte()))	'Rückgabelisten für die Geräte
    '	Private _NextCommand As New Dictionary(Of String, YesNo) 'kann ich den nächsten Befehl abarbeiten für dieses Gerät
    '	Private _AllDone As New Dictionary(Of String, YesNo) 'alle Befehle abgearbeitet
    '	Private _CommandCount As New Dictionary(Of String, Integer)	'welchen Befehl soll ich asführen
    '	Private _CommandInWork As New Dictionary(Of String, Integer) 'ich arbeite gerade an einem Befehl
    '	Private _SpecialIP As String 'IP für Asynchronen Befehl
    '	Private _ReceiveDataWithoutQuestion As Boolean 'soll die Statusmaschine Daten empfangen und weitergeben ohne eine vorherige Frage
    '	' erst mal nur bei Ethernet
    '	Private _ConnectionList As New List(Of String) 'Liste mit den Geräten die neu gesucht werden müssen!
    '	Private _ThreadList As New Dictionary(Of String, System.Threading.Thread)	'Liste mit den ConnectionThreads

    '    'Synchronisierung
    '    Private _LockObject As New Object

    '	'Fehler
    '	Private NOEHTERNETMODE As New Exception("No Ethernet Mode")
    '	Private NOSERIALMODE As New Exception("No Serial Mode")
    '	Private WRONGIP As New Exception("Incorrect IP Address")

    '    'Logging
    '    Private _MyTraceHandler As TraceHandler

    '#End Region

    '#Region "Öffentliche Eigenschaften"

    '    Public Enum Signals
    '        NOSIGNAL = -1
    '        SIGNALSENDSTRING = 0
    '        SIGNALSENDARRAY = 1
    '        SIGNALSENDSTRINGASYNC = 2
    '        SIGNALSENDARRAYASYNC = 3
    '    End Enum

    '    ''' <summary>
    '    ''' Mein serieller Port-> Einstellungen werden in der Klasse gemacht die diese Statusmaschine aufruft
    '    ''' </summary>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public Property MachineSerialPort() As System.IO.Ports.SerialPort
    '        Get
    '            Return _MySP
    '        End Get
    '        Set(ByVal value As System.IO.Ports.SerialPort)
    '            _MySP = value
    '        End Set
    '    End Property

    '    ''' <summary>
    '    ''' Datencontainer des Gerätes!
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public ReadOnly Property DataContainerEth(ByVal IPAddress As String) As ThermoDataContainer_Interface
    '        Get
    '            If _DataContainerList.ContainsKey(IPAddress) Then
    '                Return _DataContainerList(IPAddress)
    '            Else
    '                Return Nothing
    '            End If
    '        End Get
    '    End Property

    '    ''' <summary>
    '    ''' Datencontainer des Gerätes!
    '    ''' </summary>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public ReadOnly Property DataContainerSer() As ThermoDataContainer_Interface
    '        Get
    '            Return _DataContainer
    '        End Get
    '    End Property

    '    ''' <summary>
    '    ''' Datenkollektor rausgeben, wegen den Diagnosewerten
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public ReadOnly Property DataCollectorEth(ByVal IPAddress As String) As ThermoDataCollectorMulti
    '        Get
    '            If _DataCollectorList.ContainsKey(IPAddress) Then
    '                Return _DataCollectorList(IPAddress)
    '            Else
    '                Return Nothing
    '            End If
    '        End Get
    '    End Property

    '    ''' <summary>
    '    ''' Datenkollektor rausgeben, wegen den Diagnosewerten
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public ReadOnly Property DataCollectorSer(ByVal IPAddress As String) As ThermoDataCollector_Interface
    '        Get
    '            Return _DataCollector
    '        End Get
    '    End Property

    '    ''' <summary>
    '    ''' fürs debuggen
    '    ''' </summary>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public Property TraceHandler() As ThermoLogging.TraceHandler
    '        Get
    '            Return _MyTraceHandler
    '        End Get
    '        Set(ByVal value As ThermoLogging.TraceHandler)
    '            _MyTraceHandler = value
    '        End Set
    '    End Property

    '    ''' <summary>
    '    ''' 
    '    ''' </summary>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public ReadOnly Property CanSend() As Boolean  'Kann man was senden
    '        Get
    '            If _MyState = States.IDLE Then
    '                Return True
    '            Else
    '                Return False
    '            End If
    '        End Get
    '    End Property

    '    ''' <summary>
    '    ''' lebt die ThreadFunctionEth noch?
    '    ''' Wichtig: erst abfragen nachdem eine Sendeanfrage gestellt wurde, weil erst dort wird der Counter erhöht
    '    ''' </summary>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public ReadOnly Property Alive() As Boolean
    '        Get
    '            If _LiveCounter > 1000 Then
    '                _LiveCounter = 1
    '                _LiveCounterOld = 0
    '            End If
    '            If _SendErrorCounter > 1000 Then
    '                _SendErrorCounter = 0
    '            End If
    '            If (Now.Subtract(_AliveCheckTime).TotalMilliseconds >= _AliveCheckIntervall) Then 'nur alle 10 sekunden die Verbindung prüfen
    '                _AliveCheckTime = Now
    '                WriteLog(TraceEventType.Information, "ThermoStateMachine, Alive: Livecounter=" & _LiveCounter.ToString & ";" & "LivecounterOld=" & _LiveCounterOld.ToString)
    '                'entweder ist der Thread nicht mehr drangekommen oder ich konnte 30 mal hintereinander nix senden (SetSignal)
    '                Dim Ret As Boolean
    '                If (_LiveCounter = _LiveCounterOld) And (_MyState = States.BUSY) Then
    '                    Ret = False 'ich bin Busy, sollte also den Counter erhöht haben
    '                Else
    '                    _LiveCounterOld = _LiveCounter
    '                    Ret = True
    '                End If
    '                If Ret And (_SendErrorCounter > 30) Then 'ist bis jetzt noch alles ok dann prüfen ob der SendErrorCounter größer als 30 ist
    '                    WriteLog(TraceEventType.Information, "ThermoStateMachine, Alive: SendErrorCounter=" & _SendErrorCounter.ToString)
    '                    Ret = False
    '                End If
    '                If Ret Then 'Datencollectoren testen
    '                    For Each DIP As String In _DataCollectorList.Keys
    '                        If Not _DataCollectorList(DIP).Diagnosis.Alive Then
    '                            WriteLog(TraceEventType.Information, "ThermoStateMachine, Diagnosis.Alive=False: " & DIP)
    '                            Ret = False
    '                            Exit For
    '                        End If
    '                    Next
    '                End If
    '                Return Ret
    '            Else
    '                Return True
    '            End If
    '        End Get
    '    End Property


    '    ''' <summary>
    '    ''' Event das irgendetwas schiefgelaufen ist
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As Exception)

    '    ''' <summary>
    '    ''' Alle Befehle abgearbeitet, alle Geräte; seriell
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandDoneStr(ByVal Index As Integer, ByVal ReturnListString As Dictionary(Of String, String))

    '    ''' <summary>
    '    ''' Alle Befehle abgearbeitet, alle Geräte; seriell
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandDoneAr(ByVal Index As Integer, ByVal ReturnListArray As Dictionary(Of Byte(), Byte()))

    '    ''' <summary>
    '    ''' Alle Befehle abgearbeitet für ein Gerät; Ethernet
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="ReturnListStringEth"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandsDoneStr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListStringEth As List(Of String))

    '    ''' <summary>
    '    ''' Alle Befehle abgearbeitet für ein Gerät; Ethernet
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="ReturnListArrayEth"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandsDoneAr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListArrayEth As List(Of Byte()))

    '    ''' <summary>
    '    ''' Alle Befehle abgearbeitet für ein Gerät; Ethernet
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="ReturnStringEth"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandsDoneStrAsync(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnStringEth As String)

    '    ''' <summary>
    '    ''' Alle Befehle abgearbeitet für ein Gerät; Ethernet
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="ReturnArrayEth"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandsDoneArAsync(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnArrayEth As Byte())

    '    ''' <summary>
    '    ''' Meldung das alle Befehle an alle Geräte abgearbeitet wurden; kommt nur im Ethernet-Mode
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <remarks></remarks>
    '    Public Event AllWorkDone(ByVal Index As Integer)

    '    ''' <summary>
    '    ''' eine Meldung vom Gerät empfangen (Autosend!)
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="IPAddress"></param>
    '    ''' <param name="ReturnListStringEth"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandReceivedStr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListStringEth As List(Of String))

    '    ''' <summary>
    '    ''' eine Meldung vom Gerät empfangen (Autosend!)
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="IPAddress"></param>
    '    ''' <param name="ReturnListArrayEth"></param>
    '    ''' <remarks></remarks>
    '    Public Event CommandReceivedAr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListArrayEth As List(Of Byte()))

    '    ''' <summary>
    '    ''' Event das ein Gerät verbunden wurde (macht nur mit Ethernet sinn)
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="IPAddress"></param>
    '    ''' <remarks></remarks>
    '    Public Event DeviceConnected(ByVal Index As Integer, ByVal IPAddress As String, ByVal Connected As Boolean)

    '    ''' <summary>
    '    ''' Event das eine Verbindung zu einem Gerät unterbrochen wurde (macht nur mit Ethernet sinn)
    '    ''' </summary>
    '    ''' <param name="Index"></param>
    '    ''' <param name="IPAddress"></param>
    '    ''' <remarks></remarks>
    '    Public Event DeviceDisconnected(ByVal Index As Integer, ByVal IPAddress As String)

    '    ''' <summary>
    '    ''' Nachricht
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public Event NewMessage(ByVal Msg As String, ByVal Debug As Boolean)

    '#End Region

    '#Region "Private Methoden"

    '#Region "Allgemein"

    '    '''' <summary>
    '    '''' Fehlerbehandlung
    '    '''' </summary>
    '    '''' <param name="ex"></param>
    '    '''' <remarks></remarks>
    '    'Private Sub ErrorHandler(ByVal ex As Exception)
    '    '    If (Not ex.Message = "ThreadAbortException") And (Not ex.Message = "Der Thread wurde abgebrochen.") And (Not ex.Message = "Thread was being aborted.") Then
    '    '        'RaiseEvent ErrorEvent(ex)
    '    '        Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '    '    End If
    '    'End Sub

    '    ''' <summary>
    '    ''' nachrichten senden
    '    ''' </summary>
    '    ''' <param name="Msg"></param>
    '    ''' <param name="debug"></param>
    '    ''' <remarks></remarks>
    '    Private Sub MsgHandler(ByVal Msg As String, ByVal debug As Boolean)
    '        RaiseEvent NewMessage(Msg, debug)
    '    End Sub

    '#End Region

    '#Region "Seriell"

    '    ''' <summary>
    '    ''' Was soll ich eigentlich machen
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub ThreadFunction()
    '        Do
    '            Try
    '                If Monitor.TryEnter(_LockObject, 1000) Then     'unterbrechungsfrei arbeiten!?
    '                    Try
    '                        If (_MyState = States.BUSY) And (_MySignal <> Signals.NOSIGNAL) And (_Next = YesNo.YES) Then

    '                            Select Case _MySignal

    '                                Case Signals.SIGNALSENDSTRING   'Strings werden gesendet
    '                                    If (_CommandCountSer <= _CommandListString.Count - 1) Then 'liste abarbeiten
    '                                        Dim NewCommand As String = ""
    '                                        NewCommand = _CommandListString.Item(_CommandCountSer) 'nächster Befehl
    '                                        _DataContainer.Command = _CommandListStringUncoded.Item(_CommandCountSer)
    '                                        _ActualCommandString = NewCommand
    '                                        _Next = YesNo.NO    'ich arbeite gerade an einem Befehl, also keinen nächsten senden
    '                                        _CommandCountSer = _CommandCountSer + 1 'Befehlszeiger erhöhen
    '                                        _DataCollector.SendQuestion(NewCommand, 1)
    '                                    Else 'liste leer
    '                                        _Next = YesNo.NO
    '                                        _MyState = States.IDLE
    '                                        _MySignal = Signals.NOSIGNAL
    '                                        RaiseEvent CommandDoneStr(_MyIndex, _ReturnListString)
    '                                    End If

    '                                Case Signals.SIGNALSENDARRAY    'Arrays werden gesendet
    '                                    If (_CommandCountSer <= _CommandListString.Count - 1) Then  'liste abarbeiten
    '                                        Dim NewCommand As Byte()
    '                                        NewCommand = _CommandListArray.Item(_CommandCountSer)   'nächster Befehl
    '                                        _DataContainer.Command = _CommandListArrayUncoded.Item(_CommandCountSer)
    '                                        _ActualCommandArray = NewCommand
    '                                        _Next = YesNo.NO    'ich arbeite gerade an einem Befehl, also keinen nächsten senden
    '                                        _CommandCountSer = _CommandCountSer + 1 'Befehlszeiger erhöhen
    '                                        _DataCollector.SendQuestion(NewCommand, NewCommand.Length, 1)
    '                                    Else 'liste leer
    '                                        _Next = YesNo.NO
    '                                        _MyState = States.IDLE
    '                                        _MySignal = Signals.NOSIGNAL
    '                                        RaiseEvent CommandDoneAr(_MyIndex, _ReturnListArray)
    '                                    End If

    '                                Case Signals.SIGNALSENDSTRINGASYNC  'asynchronen String senden
    '                                    _DataContainer.Command = _SpecialCommandUncoded
    '                                    _ActualCommandString = _SpecialCommandStr
    '                                    _DataCollector.SendQuestion(_SpecialCommandStr, 1)
    '                                    _MySignal = Signals.NOSIGNAL

    '                                Case Signals.SIGNALSENDARRAYASYNC   'asynchrones Bytearray senden
    '                                    _DataContainer.Command = _SpecialCommandUncoded
    '                                    _ActualCommandArray = _SpecialCommandAr
    '                                    _DataCollector.SendQuestion(_SpecialCommandAr, _SpecialCommandAr.Length, 1)
    '                                    _MySignal = Signals.NOSIGNAL

    '                            End Select

    '                        End If
    '                        Thread.Sleep(_MyInterval)   'Abarbeitung anhalten
    '                    Finally
    '                        Monitor.Exit(_LockObject)
    '                    End Try
    '                End If
    '            Catch ex As Exception
    '                _MySignal = Signals.NOSIGNAL
    '                _MyState = States.IDLE
    '                _Next = YesNo.NO
    '                Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '            End Try
    '        Loop Until _Running = False
    '    End Sub

    '    ''' <summary>
    '    ''' Event vom DataColletor
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DataReadyStr(ByVal ErrorState As Integer)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then   'unterbrechungsfrei arbeiten!?
    '                Try
    '                    Dim Buffer As String
    '                    Buffer = _DataContainer.AnswerAsString 'antwort abholen
    '                    _DataContainer.AnswerAsString = ""
    '                    If _OldSignal = Signals.SIGNALSENDSTRINGASYNC Then  'asynchroner Vorgang
    '                        If ErrorState = 0 Then 'alles ok
    '                            _ReturnListString.Add(_ActualCommandString, Buffer)
    '                        ElseIf ErrorState = 1 Then 'fehler
    '                            Buffer = "Error"
    '                            _ReturnListString.Add(_ActualCommandString, Buffer)
    '                        End If
    '                        _MyState = States.IDLE
    '                        _MySignal = Signals.NOSIGNAL
    '                        RaiseEvent CommandDoneStr(_MyIndex, _ReturnListString)
    '                    Else 'kein asynchroner Vorgang
    '                        If ErrorState = 0 Then 'alles ok
    '                            _ReturnListString.Add(_ActualCommandString, Buffer)
    '                            _Next = YesNo.YES   'nächster Befehl kann abgearbeitet werden
    '                        ElseIf ErrorState = 1 Then 'fehler
    '                            Buffer = "Error"
    '                            _ReturnListString.Add(_ActualCommandString, Buffer)
    '                            _Next = YesNo.NO
    '                            _MyState = States.IDLE
    '                            _MySignal = Signals.NOSIGNAL
    '                        End If
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            _Next = YesNo.NO
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Event vom DataColletor
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DataReadyAr(ByVal ErrorState As Integer)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then   'unterbrechungsfrei arbeiten!?
    '                Try
    '                    Dim Buffer() As Byte
    '                    Buffer = CType(_DataContainer.AnswerAsArray, Byte()) 'antwort abholen
    '                    Array.Clear(_DataContainer.AnswerAsArray, 0, _DataContainer.AnswerAsArray.Length)
    '                    If _OldSignal = Signals.SIGNALSENDARRAYASYNC Then   'asynchroner Vorgang
    '                        If ErrorState = 0 Then 'alles ok
    '                            _ReturnListArray.Add(_ActualCommandArray, Buffer)
    '                        ElseIf ErrorState = 1 Then 'fehler
    '                            Array.Clear(Buffer, 0, Buffer.Length)
    '                            _ReturnListArray.Add(_ActualCommandArray, Buffer)
    '                        End If
    '                        _MyState = States.IDLE
    '                        _MySignal = Signals.NOSIGNAL
    '                        RaiseEvent CommandDoneStr(_MyIndex, _ReturnListString)
    '                    Else 'kein asynchroner Vorgang
    '                        If ErrorState = 0 Then 'alles ok
    '                            _ReturnListArray.Add(_ActualCommandArray, Buffer)
    '                            _Next = YesNo.YES   'nächster Befehl kann abgearbeitet werden
    '                        ElseIf ErrorState = 1 Then 'fehler
    '                            Array.Clear(Buffer, 0, Buffer.Length)
    '                            _ReturnListArray.Add(_ActualCommandArray, Buffer)
    '                            _Next = YesNo.NO
    '                            _MyState = States.IDLE
    '                            _MySignal = Signals.NOSIGNAL
    '                        End If
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            _Next = YesNo.NO
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '#End Region

    '#Region "Ethernet"

    '    ''' <summary>
    '    ''' Connection Thread starten -> eine vorhandene Verbindung wurde abgebrochen!
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub TryConnectionToDevice(ByVal IP As String)
    '        Try
    '            Dim _Thread As System.Threading.Thread 'Thread für die Verbindungsaufnahme
    '            'evtl Thread beenden; muss hierher da es eine ThreadAbort Exception und die kann ich nicht brauchen, wenn
    '            'ich den Thread aus sich selbst heraus beenden würde
    '            WriteLog(TraceEventType.Critical, "ThermoStateMachine, TryConnectionToDevice: IP=" & IP)
    '            If _ThreadList.ContainsKey(IP) Then
    '                Try
    '                    _ThreadList(IP).Abort()
    '                Catch exe As Exception
    '                Finally
    '                    _ThreadList(IP) = Nothing
    '                    _ThreadList.Remove(IP)
    '                End Try
    '            End If
    '            If Not _ConnectionList.Contains(IP) Then 'nur neuen Thread starten, wenn er nicht schon läuft
    '                _ConnectionList.Add(IP)
    '                _Thread = New System.Threading.Thread(AddressOf SetupConnectionToDevice)
    '                _ThreadList.Add(IP, _Thread)
    '                While Not _Thread.ThreadState = ThreadState.Running
    '                    If _Running = False Then Exit While
    '                    Try
    '                        WriteLog(TraceEventType.Critical, "ThermoStateMachine, TryConnectionToDevice: IP=" & IP & " Thread started")
    '                        _Thread.Start(IP)
    '                    Catch ex As Exception
    '                    End Try
    '                End While
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Thread der die Verbindung zum Device sucht
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub SetupConnectionToDevice(ByVal IP As Object)
    '        Try
    '            Dim _MySocket As System.Net.Sockets.Socket = Nothing 'Socket
    '            Dim _MyServerEndPoint As System.Net.IPEndPoint 'IP, Port
    '            Dim _IPAdresse As System.Net.IPAddress 'IP
    '            Dim _Ipaddress As String
    '            Dim _TimTimeout As Thread
    '            _Ipaddress = CType(IP, String)
    '            While True
    '                If _Running = False Then Exit While
    '                Dim _MyConnectionTimeout As New SocketConnectionTimeoutManager(_ConnectionTimeout, _MyTraceHandler)
    '                WriteLog(TraceEventType.Critical, "ThermoStateMachine, SetupConnectionToDevice: ConnectionTimeout=" & _ConnectionTimeout.ToString & ", IP: " & _Ipaddress & ", ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
    '                _TimTimeout = New Thread(AddressOf _MyConnectionTimeout.Timeout)
    '                _MyConnectionTimeout.Running = _Running
    '                Try
    '                    _IPAdresse = System.Net.IPAddress.Parse(_Ipaddress)
    '                    _MyServerEndPoint = New System.Net.IPEndPoint(_IPAdresse, _DeviceListEth(_Ipaddress))
    '                    _MySocket = New System.Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)
    '                    _MyConnectionTimeout.Socket = _MySocket
    '                    _MyConnectionTimeout.Connected = False
    '                    _TimTimeout.Start()
    '                    WriteLog(TraceEventType.Critical, "ThermoStateMachine, SetupConnectionToDevice: Try to connect" & ", IP: " & _Ipaddress)
    '                    _MySocket.Connect(_MyServerEndPoint) 'verbinden
    '                    _MyConnectionTimeout.Connected = True
    '                    WriteLog(TraceEventType.Critical, "ThermoStateMachine, SetupConnectionToDevice: Connection established" & ", IP: " & _Ipaddress)
    '                    Exit While 'Verbindung hat funktioniert!
    '                Catch ex As Exception   'verbindung hat nicht funktioniert
    '                    If Not _MySocket Is Nothing Then    'Client verwerfen
    '                        _MySocket.Close()
    '                        _MySocket = Nothing
    '                    End If
    '                    WriteLog(TraceEventType.Critical, "ThermoStateMachine, SetupConnectionToDevice: No connection")
    '                    WriteLog(TraceEventType.Information, "ThermoStateMachine, SetupConnectionToDevice: No connection, ex.Message: " & ex.Message)
    '                    WriteLog(TraceEventType.Information, "ThermoStateMachine, SetupConnectionToDevice: No connection, ex.StackTrace: " & ex.StackTrace)
    '                    RaiseEvent DeviceConnected(_MyIndex, _Ipaddress, False)
    '                End Try
    '                Try
    '                    _TimTimeout.Abort()
    '                Catch ex As Exception
    '                End Try
    '                System.Threading.Thread.Sleep(2000) '100
    '            End While
    '            _ConnectionList.Remove(_Ipaddress)
    '            OpenConnection(_Ipaddress, _MySocket)
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Alles zur Benutzung der Schnittstelle vorbereiten
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub OpenConnection(ByVal IpAddress As String, ByVal MySocket As System.Net.Sockets.Socket)
    '        Try
    '            Dim Protocol As ThermoProtocol_Interface 'welches protokoll?
    '            Dim ProtocolArgs As String() = {}
    '            Dim DataContainer As ThermoDataContainer_Interface  'allgemeiner datencontainer für das protokoll
    '            Dim DataContainerArgs As String() = {}
    '            Dim InterfaceEth As ThermoIOControl_Ethernet
    '            Dim DataCollector As ThermoDataCollectorMulti
    '            WriteLog(TraceEventType.Critical, "ThermoStateMachine, OpenConnection: IP=" & IpAddress)
    '            If Not _SocketList.ContainsKey(IpAddress) Then
    '                _SocketList.Add(IpAddress, MySocket) 'zur Liste
    '            Else
    '                _SocketList(IpAddress) = MySocket 'zur Liste
    '            End If
    '            'Verbindung ok
    '            _ConnectedList(IpAddress) = True
    '            'Datacontainer dynamisch anlegen
    '            DataContainer = CType(Activator.CreateInstanceFrom(_DataContainerAssembly, _DataContainerName, True, 0, Nothing, DataContainerArgs, Nothing, Nothing, Nothing).Unwrap, ThermoDataContainer_Interface)
    '            _DataContainerList.Add(IpAddress, DataContainer)
    '            'Protokoll dynamisch anlegen
    '            Protocol = CType(Activator.CreateInstanceFrom(_ProtocolAssembly, _ProtocolName, True, 0, Nothing, ProtocolArgs, Nothing, Nothing, Nothing).Unwrap, ThermoProtocol_Interface)
    '            Protocol.TraceHandler = _MyTraceHandler
    '            'Interface anlegen
    '            InterfaceEth = New ThermoIOControl_Ethernet(MySocket, _Timeout, Protocol, DataContainer) 'neues Interface
    '            _InterfaceListEth.Add(IpAddress, InterfaceEth)
    '            'Datencollector anlegen
    '            DataCollector = New ThermoDataCollectorMulti(InterfaceEth, _Repeats, IpAddress, _AliveCheckIntervall, -1) 'neuen Datencollector
    '            _DataCollectorList.Add(IpAddress, DataCollector)
    '            'Eventhandler hinzufügen
    '            AddHandler Protocol.ErrorEvent, AddressOf ErrorInSubClasses
    '            AddHandler InterfaceEth.ErrorEvent, AddressOf ErrorInSubClasses
    '            'AddHandler DataCollector.ErrorEvent, AddressOf ErrorInSubClassesDataColl
    '            AddHandler DataCollector.DataCollectorReadyStr, AddressOf DataReadyStrEth
    '            AddHandler DataCollector.DataCollectorReadyAr, AddressOf DataReadyArEth
    '            AddHandler DataCollector.Disconnected, AddressOf DeviceDisconected
    '            AddHandler Protocol.NewMessage, AddressOf MsgHandler
    '            RaiseEvent DeviceConnected(_MyIndex, IpAddress, True)
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Verbindung unterbrochen
    '    ''' </summary>
    '    ''' <param name="IP"></param>
    '    ''' <remarks></remarks>
    '    Private Sub DeviceDisconected(ByVal IP As String)
    '        Try
    '            WriteLog(TraceEventType.Critical, "ThermoStateMachine, DeviceDiconnected: IP=" & IP)
    '            OnDisconnection(IP, True)
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' aufräumen nach Disconnect!
    '    ''' </summary>
    '    ''' <param name="IP"></param>
    '    ''' <param name="FireEvent"></param>
    '    ''' <remarks></remarks>
    '    Private Sub OnDisconnection(ByVal IP As String, ByVal FireEvent As Boolean)
    '        Try
    '            WriteLog(TraceEventType.Critical, "ThermoStateMachine, OnDisconnection: IP=" & IP)
    '            CloseConnection(IP)
    '            TryConnectionToDevice(IP)
    '            If FireEvent Then
    '                RaiseEvent DeviceDisconnected(_MyIndex, IP)
    '            End If
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Verbindung schliessen
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <remarks></remarks>
    '    Private Sub CloseConnection(ByVal IPAddress As String)
    '        Try
    '            WriteLog(TraceEventType.Critical, "ThermoStateMachine, CloseConnection: IP=" & IPAddress)
    '            _ConnectedList(IPAddress) = False
    '            If _DataCollectorList.ContainsKey(IPAddress) Then
    '                _DataCollectorList(IPAddress) = Nothing
    '                _DataCollectorList.Remove(IPAddress)
    '                _InterfaceListEth(IPAddress).Dispose()
    '                _InterfaceListEth(IPAddress) = Nothing
    '                _InterfaceListEth.Remove(IPAddress)
    '                _SocketList(IPAddress).Close()
    '                _SocketList(IPAddress) = Nothing
    '                _SocketList.Remove(IPAddress)
    '                _DataContainerList(IPAddress) = Nothing
    '                _DataContainerList.Remove(IPAddress)
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Was soll ich eigentlich machen
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub ThreadFunctionEth()
    '        Dim count As Integer = 0
    '        Do
    '            Try
    '                If Monitor.TryEnter(_LockObject, 1000) Then   'unterbrechungsfrei arbeiten!?
    '                    Try
    '                        If (_MyState = States.BUSY) And (_MySignal <> Signals.NOSIGNAL) And (_Running) Then

    '                            _LiveCounter = _LiveCounter + 1
    '                            If _LiveCounter > 1000 Then
    '                                _LiveCounter = 1
    '                            End If
    '                            WriteLog(TraceEventType.Information, "ThermoStateMachine, ThreadFunctionEth: Livecounter=" & _LiveCounter.ToString & ";" & "LivecounterOld=" & _LiveCounterOld.ToString)

    '                            Select Case _MySignal

    '                                Case Signals.SIGNALSENDSTRING   'strings werden gesendet
    '                                    For Each IP As String In _DeviceListEth.Keys 'für alle geräte eine Frage senden
    '                                        If _CommandListStringEth.ContainsKey(IP) Then 'Wenn diese Liste gerade gebaut wird, dann wird sie = nothing
    '                                            If _CommandListStringEth(IP).Count > 0 Then
    '                                                Try
    '                                                    If ((_CommandCount(IP) <= _CommandListStringEth(IP).Count - 1)) And (_ConnectedList(IP) = True) Then
    '                                                        If (_NextCommand(IP) = YesNo.YES) Then  'liste abarbeiten
    '                                                            Dim NewCommand As String = ""
    '                                                            NewCommand = _CommandListStringEth(IP).Item(_CommandCount(IP)) 'nächster befehl
    '                                                            _DataContainerList(IP).Command = _CommandListStringEthUncoded(IP).Item(_CommandCount(IP))
    '                                                            _ActualCommandString = NewCommand
    '                                                            _NextCommand(IP) = YesNo.NO 'ich bearbeite gerade einen befehl
    '                                                            _CommandCount(IP) = _CommandCount(IP) + 1   'Befehlszähler erhöhen
    '                                                            _CommandInWork(IP) = 1 'für den letzten Befehl, sonst kommt der else Zweig dran ohne die antwort des letzten Befehls abzuwarten
    '                                                            WriteLog(TraceEventType.Critical, "ThermoStateMachine, SendQuestion: " & NewCommand)
    '                                                            _DataCollectorList(IP).SendQuestion(NewCommand, 1)
    '                                                        End If
    '                                                    Else 'liste leer oder nicht verbunden
    '                                                        If (Not _CommandInWork(IP) = 1) And (_AllDone(IP) = YesNo.NO) Then  'nur reingehen wenn nicht noch ein Befehl aussteht und wenn ich das Signal nicht schonmal gesendet habe
    '                                                            _NextCommand(IP) = YesNo.NO
    '                                                            _AllDone(IP) = YesNo.YES
    '                                                            If _ConnectedList(IP) = False Then _ReturnListStringEth(IP).Add("ERROR")
    '                                                            RaiseEvent CommandsDoneStr(_MyIndex, IP, _ReturnListStringEth(IP)) 'erfolgreichen Vollzug melden
    '                                                        End If
    '                                                    End If
    '                                                Catch exe As Exception
    '                                                    WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP)
    '                                                    WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: Error=" & exe.Message)
    '                                                    WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: Stack=" & exe.StackTrace)
    '                                                    Try
    '                                                        'Zusatz fürs debug
    '                                                        WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP & " " & _CommandListStringEth(IP).Count.ToString)
    '                                                        WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP & " " & _CommandListStringEth(IP).Item(_CommandCount(IP)))
    '                                                        WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP & " " & _ConnectedList(IP).ToString())
    '                                                        WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP & " " & _CommandCount(IP).ToString())
    '                                                        WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP & " " & _DataContainerList(IP).Command)
    '                                                        WriteLog(TraceEventType.Error, "ThermoStateMachine, ThreadFunctionEth: IP=" & IP & " " & _CommandListStringEthUncoded(IP).Item(_CommandCount(IP)))
    '                                                        'Zusatz Ende
    '                                                    Catch ex As Exception
    '                                                    End Try
    '                                                End Try
    '                                            End If
    '                                        End If
    '                                    Next

    '                                Case Signals.SIGNALSENDARRAY    'arrays werden gesendet
    '                                    For Each IP As String In _DeviceListEth.Keys 'für alle geräte eine Frage senden
    '                                        If _CommandListArrayEth.ContainsKey(IP) Then 'Wenn diese Liste gerade gebaut wird, dann wird sie = nothing
    '                                            If _CommandListArrayEth(IP).Count > 0 Then
    '                                                If ((_CommandCount(IP) <= _CommandListArrayEth(IP).Count - 1)) And (_ConnectedList(IP) = True) Then
    '                                                    If (_NextCommand(IP) = YesNo.YES) Then   'liste abarbeiten
    '                                                        Dim NewCommand As Byte()
    '                                                        NewCommand = _CommandListArrayEth(IP).Item(_CommandCount(IP))   'nächster befehl
    '                                                        _DataContainerList(IP).Command = _CommandListArrayEthUncoded(IP).Item(_CommandCount(IP))
    '                                                        _ActualCommandArray = NewCommand
    '                                                        _NextCommand(IP) = YesNo.NO 'ich bearbeite gerade einen befehl 
    '                                                        _CommandCount(IP) = _CommandCount(IP) + 1   'befehlszähler erhöhen
    '                                                        _CommandInWork(IP) = 1 'für den letzten Befehl, sonst kommt der else Zweig dran ohne die antwort des letzten Befehls abzuwarten
    '                                                        _DataCollectorList(IP).SendQuestion(NewCommand, NewCommand.Length, 1)
    '                                                    End If
    '                                                Else 'liste leer
    '                                                    If (Not _CommandInWork(IP) = 1) And (_AllDone(IP) = YesNo.NO) Then  'nur reingehen wenn nicht noch ein Befehl aussteht und wenn ich das Signal nicht schonmal gesendet habe
    '                                                        _NextCommand(IP) = YesNo.NO
    '                                                        _AllDone(IP) = YesNo.YES
    '                                                        Dim Buffer(0) As Byte
    '                                                        If _ConnectedList(IP) = False Then _ReturnListArrayEth(IP).Add(Buffer)
    '                                                        RaiseEvent CommandsDoneAr(_MyIndex, IP, _ReturnListArrayEth(IP)) 'erfolgreichen Vollzug melden
    '                                                    End If
    '                                                End If
    '                                            End If
    '                                        End If
    '                                    Next

    '                                Case Signals.SIGNALSENDSTRINGASYNC  'asynchronen string senden
    '                                    If Not _CommandInWork(_SpecialIP) = 1 Then
    '                                        _CommandInWork(_SpecialIP) = 1
    '                                        _DataContainerList(_SpecialIP).Command = _SpecialCommandUncoded
    '                                        _DataCollectorList(_SpecialIP).SendQuestion(_SpecialCommandStr, 1)
    '                                        _MySignal = Signals.NOSIGNAL
    '                                    End If

    '                                Case Signals.SIGNALSENDARRAYASYNC   'asynchrones array senden
    '                                    If Not _CommandInWork(_SpecialIP) = 1 Then
    '                                        _CommandInWork(_SpecialIP) = 1
    '                                        _DataContainerList(_SpecialIP).Command = _SpecialCommandUncoded
    '                                        _DataCollectorList(_SpecialIP).SendQuestion(_SpecialCommandAr, _SpecialCommandAr.Length, 1)
    '                                        _MySignal = Signals.NOSIGNAL
    '                                    End If

    '                            End Select

    '                            count = 0
    '                            For Each IP As String In _DeviceListEth.Keys
    '                                If _AllDone(IP) = YesNo.YES Then count = count + 1 'wieviele habe ich abgearbeitet
    '                            Next
    '                            If count = _DeviceListEth.Count Then 'wenn alle abgearbietet
    '                                For Each IP As String In _DeviceListEth.Keys
    '                                    _AllDone(IP) = YesNo.NO 'alles wieder rückgängig
    '                                Next
    '                                _MyState = States.IDLE 'nix machen
    '                                _MySignal = Signals.NOSIGNAL    'nix machen
    '                                RaiseEvent AllWorkDone(_MyIndex)
    '                            End If

    '                        End If

    '                        Thread.Sleep(_MyInterval)   'Abarbeitung anhalten
    '                    Finally
    '                        Monitor.Exit(_LockObject)
    '                    End Try
    '                End If
    '            Catch ex As Exception
    '                _MySignal = Signals.NOSIGNAL
    '                _MyState = States.IDLE
    '                Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '            End Try
    '        Loop Until _Running = False
    '    End Sub

    '    ''' <summary>
    '    ''' Event vom DataColletor
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DataReadyStrEth(ByVal ID As String, ByVal ErrorState As Integer)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then   'unterbrechungsfrei arbeiten!?
    '                Try
    '                    Dim Buffer As String
    '                    If _DeviceListEth.ContainsKey(ID) Then
    '                        Buffer = _DataContainerList(ID).AnswerAsString 'antwort abholen
    '                        _DataContainerList(ID).AnswerAsString = ""
    '                        If _OldSignal = Signals.SIGNALSENDSTRINGASYNC Then  'asynchroner Vorgang
    '                            If ErrorState = 0 Then 'alles ok
    '                                _ReturnStringEthAsync(ID) = Buffer
    '                            ElseIf ErrorState = 1 Then 'fehler
    '                                Buffer = "ERROR"
    '                                _ReturnStringEthAsync(ID) = Buffer
    '                            End If
    '                            _CommandInWork(ID) = 0
    '                            _MyState = States.IDLE
    '                            _MySignal = Signals.NOSIGNAL
    '                            RaiseEvent CommandsDoneStrAsync(_MyIndex, ID, _ReturnStringEthAsync(ID))
    '                        Else 'kein asynchroner Vorgang
    '                            If ErrorState = 0 Then 'alles ok
    '                                If _CommandInWork(ID) = 1 Then
    '                                    _ReturnListStringEth(ID).Add(Buffer)
    '                                    _NextCommand(ID) = YesNo.YES    'nächster Befehl kann abgearbeitet werden
    '                                    _CommandInWork(ID) = 0
    '                                Else 'es wurden Daten empfangen ohne eine vorherige Frage
    '                                    If _ReceiveDataWithoutQuestion Then 'darf der gegenüber das?
    '                                        'wenn ja dann Event schicken!
    '                                        _NextCommand(ID) = YesNo.YES        'wichtig da sonst ein Deadlock möglich wäre (warum weiß ich leider nicht; ist ein Timing Problem)
    '                                        _AllDone(ID) = YesNo.NO 'wichtig da sonst ein Deadlock möglich wäre (warum weiß ich leider nicht; ist ein Timing Problem)
    '                                        If Buffer <> "" Then 'manchmal kommt ein Event vom Socket aber es war keine Frage und der Gegenüber hat eigentlich auch nix gesendet
    '                                            _ReceivedListStringEth(ID).Clear()
    '                                            _ReceivedListStringEth(ID).Add(Buffer)
    '                                            RaiseEvent CommandReceivedStr(_MyIndex, ID, _ReceivedListStringEth(ID))
    '                                        End If
    '                                    End If
    '                                End If
    '                            ElseIf ErrorState = 1 Then 'fehler
    '                                Buffer = "ERROR"
    '                                _ReturnListStringEth(ID).Add(Buffer)
    '                                _NextCommand(ID) = YesNo.NO
    '                                _AllDone(ID) = YesNo.YES
    '                                RaiseEvent CommandsDoneStr(_MyIndex, ID, _ReturnListStringEth(ID))
    '                            End If
    '                        End If
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Event vom DataColletor
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DataReadyArEth(ByVal ID As String, ByVal ErrorState As Integer)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then   'unterbrechungsfrei arbeiten!?
    '                Try
    '                    Dim Buffer() As Byte
    '                    If _DeviceListEth.ContainsKey(ID) Then
    '                        Buffer = CType(_DataContainerList(ID).AnswerAsArray, Byte()) 'antwort abholen
    '                        Array.Clear(_DataContainer.AnswerAsArray, 0, _DataContainer.AnswerAsArray.Length)
    '                        If _OldSignal = Signals.SIGNALSENDARRAYASYNC Then   'asynchroner Vorgang
    '                            If ErrorState = 0 Then 'alles ok
    '                                _ReturnArrayEthAsync(ID) = Buffer
    '                            ElseIf ErrorState = 1 Then 'fehler
    '                                Array.Clear(Buffer, 0, Buffer.Length)
    '                                _ReturnArrayEthAsync(ID) = Buffer
    '                            End If
    '                            _CommandInWork(ID) = 0
    '                            _MyState = States.IDLE
    '                            _MySignal = Signals.NOSIGNAL
    '                            RaiseEvent CommandsDoneArAsync(_MyIndex, ID, _ReturnArrayEthAsync(ID))
    '                        Else 'kein asynchroner Vorgang
    '                            If ErrorState = 0 Then 'alles ok
    '                                If _CommandInWork(ID) = 1 Then
    '                                    _ReturnListArrayEth(ID).Add(Buffer)
    '                                    _NextCommand(ID) = YesNo.YES    'nächster Befehl kann abgearbeitet werden
    '                                    _CommandInWork(ID) = 0
    '                                Else 'es wurden Daten empfangen ohne eine vorherige Frage
    '                                    If _ReceiveDataWithoutQuestion Then 'darf der gegenüber das?
    '                                        'wenn ja dann Event schicken!
    '                                        _NextCommand(ID) = YesNo.YES        'wichtig da sonst ein Deadlock möglich wäre (warum weiß ich leider nicht; ist ein Timing Problem)
    '                                        _AllDone(ID) = YesNo.NO 'wichtig da sonst ein Deadlock möglich wäre (warum weiß ich leider nicht; ist ein Timing Problem)
    '                                        If Buffer.Length <> 0 Then
    '                                            _ReceivedListArrayEth(ID).Clear()
    '                                            _ReceivedListArrayEth(ID).Add(Buffer)
    '                                            RaiseEvent CommandReceivedAr(_MyIndex, ID, _ReceivedListArrayEth(ID))
    '                                        End If
    '                                    End If
    '                                End If
    '                            ElseIf ErrorState = 1 Then 'fehler
    '                                Array.Clear(Buffer, 0, Buffer.Length)
    '                                _ReturnListArrayEth(ID).Add(Buffer)
    '                                _NextCommand(ID) = YesNo.NO
    '                                _AllDone(ID) = YesNo.YES
    '                                RaiseEvent CommandsDoneAr(_MyIndex, ID, _ReturnListArrayEth(ID))
    '                            End If
    '                        End If
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '#End Region

    '#Region "Fehlerbehandlung"

    '    ''' <summary>
    '    ''' Fehler abfangen und weiterleiten
    '    ''' </summary>
    '    ''' <param name="ex"></param>
    '    ''' <remarks></remarks>
    '    Private Sub ErrorInSubClasses(ByVal ex As Exception)
    '        _MySignal = Signals.NOSIGNAL
    '        _MyState = States.IDLE
    '        Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        'RaiseEvent ErrorEvent(ex)
    '    End Sub

    '    ''' <summary>
    '    ''' Fehler abfangen und weiterleiten
    '    ''' </summary>
    '    ''' <param name="ex"></param>
    '    ''' <remarks></remarks>
    '    Private Sub ErrorInSubClassesDataColl(ByVal Id As String, ByVal ex As Exception)
    '        _MySignal = Signals.NOSIGNAL
    '        _MyState = States.IDLE
    '        Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        'RaiseEvent ErrorEvent(ex)
    '    End Sub

    '#End Region

    '#Region "Logging"

    '    ''' <summary>
    '    ''' Logfile schreiben
    '    ''' </summary>
    '    ''' <param name="EntryType"></param>
    '    ''' <param name="LogEntry"></param>
    '    ''' <remarks></remarks>
    '    Private Sub WriteLog(ByVal EntryType As TraceEventType, ByVal LogEntry As String)
    '        'If Not _MyTraceHandler Is Nothing Then
    '        '    _MyTraceHandler.WriteLog(EntryType, 0, LogEntry)
    '        'End If
    '        Select Case EntryType

    '            Case TraceEventType.Critical
    '                Trace.TraceWarning(LogEntry)

    '            Case TraceEventType.Information
    '                Trace.TraceInformation(LogEntry)

    '            Case TraceEventType.Warning
    '                Trace.TraceWarning(LogEntry)

    '            Case Else
    '                Trace.TraceInformation(LogEntry)

    '        End Select

    '    End Sub

    '#End Region

    '#End Region

    '#Region "Öffentliche Methoden"

    '#Region "Allgemeine Methoden"

    '    ''' <summary>
    '    ''' Konstruktor
    '    ''' </summary>
    '    ''' <param name="Interval">wie schnell soll die Statusmaschine arbeiten (10ms)</param>
    '    ''' <param name="Index">index des objektes zur verwaltung in einer Liste</param>
    '    ''' <param name="Timeout">timeout für die serielle übertragung</param>
    '    ''' <remarks></remarks>
    '    Public Sub New(ByVal Interval As Integer, ByVal Index As Integer, ByVal ProtocolAssembly As String, ByVal ProtocolName As String, ByVal DataContainerAssembly As String, ByVal DataContainerName As String, ByVal Timeout As Integer, ByVal Repeats As Integer, ByVal Ethernet As Boolean, ByVal ConnectionTimeout As Integer, ByVal TraceHandler As ThermoLogging.TraceHandler, ByVal AliveCheckIntervall As Integer, Optional ByVal ReceiveDataWithoutQuestion As Boolean = False)
    '        Try
    '            _MyInterval = Interval 'Intervall für die Threads
    '            _Timeout = Timeout
    '            _Repeats = Repeats
    '            _MyIndex = Index
    '            _Running = True
    '            _MyState = States.IDLE
    '            _Ethernet = Ethernet
    '            _ConnectionTimeout = ConnectionTimeout
    '            _ProtocolAssembly = ProtocolAssembly
    '            _ProtocolName = ProtocolName
    '            _DataContainerAssembly = DataContainerAssembly
    '            _DataContainerName = DataContainerName
    '            _ReceiveDataWithoutQuestion = ReceiveDataWithoutQuestion
    '            _MyTraceHandler = TraceHandler
    '            _AliveCheckTime = Now
    '            _AliveCheckIntervall = AliveCheckIntervall
    '            If Not _Ethernet Then
    '                'Protokoll dynamisch anlegen
    '                Dim ProtocolArgs As String() = {}
    '                _Protocol = CType(Activator.CreateInstanceFrom(ProtocolAssembly, ProtocolName, True, 0, Nothing, ProtocolArgs, Nothing, Nothing, Nothing).Unwrap, ThermoProtocol_Interface)
    '                _Protocol.TraceHandler = _MyTraceHandler
    '                Dim DataContainerArgs As String() = {}
    '                _DataContainer = CType(Activator.CreateInstanceFrom(_DataContainerAssembly, _DataContainerName, True, 0, Nothing, DataContainerArgs, Nothing, Nothing, Nothing).Unwrap, ThermoDataContainer_Interface)
    '                _Interface_To_RS232 = New ThermoIOControl_RS232(_MySP, _Timeout, _Protocol, _DataContainer)
    '                _DataCollector = New ThermoDataCollector(_Interface_To_RS232, _Repeats, _AliveCheckIntervall, -1)
    '                AddHandler _Protocol.ErrorEvent, AddressOf ErrorInSubClasses
    '                AddHandler _Interface_To_RS232.ErrorEvent, AddressOf ErrorInSubClasses
    '                AddHandler _DataCollector.ErrorEvent, AddressOf ErrorInSubClasses
    '                AddHandler _DataCollector.DataCollectorReadyStr, AddressOf DataReadyStr
    '                AddHandler _DataCollector.DataCollectorReadyAr, AddressOf DataReadyAr
    '                AddHandler _Protocol.NewMessage, AddressOf MsgHandler
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' alles zerstören
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public Sub Dispose()
    '        If Not _MyTraceHandler Is Nothing Then
    '            _MyTraceHandler.Dispose()
    '        End If
    '    End Sub

    '    ''' <summary>
    '    ''' Maschine starten
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public Sub StartMachine()
    '        Try
    '            _Running = True
    '            If Not _Ethernet Then
    '                _Thread1 = New Thread(AddressOf ThreadFunction)
    '                _Thread1.Start()
    '            Else
    '                _Thread1 = New Thread(AddressOf ThreadFunctionEth)
    '                _Thread1.Start()
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Maschine anhalten
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public Sub StopMachine()
    '        Try
    '            _Running = False
    '            Try
    '                _Thread1.Abort()
    '            Catch ex As Exception
    '            End Try
    '            For Each IP As String In _ThreadList.Keys
    '                Try
    '                    _ThreadList(IP).Abort()
    '                Catch ex As Exception
    '                End Try
    '            Next
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Befehlsliste wegschicken
    '    ''' </summary>
    '    ''' <param name="Signal"></param>
    '    ''' <remarks></remarks>
    '    Public Function SetSignal(ByVal Signal As Signals) As Boolean
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _MyState = States.IDLE Then
    '                        _MyState = States.BUSY
    '                        If Not _Ethernet Then
    '                            _Next = YesNo.YES
    '                            _CommandCountSer = 0
    '                            _ReturnListString.Clear()
    '                            _ReturnListArray.Clear()
    '                        Else
    '                            For Each IP As String In _DeviceListEth.Keys
    '                                _NextCommand(IP) = YesNo.YES
    '                                _AllDone(IP) = YesNo.NO
    '                                _CommandCount(IP) = 0
    '                                _CommandInWork(IP) = 0
    '                            Next
    '                            For Each IP As String In _ReturnListStringEth.Keys
    '                                _ReturnStringEthAsync(IP) = ""
    '                                _ReturnListStringEth(IP).Clear()
    '                                _ReceivedListStringEth(IP).Clear()
    '                            Next
    '                            For Each IP As String In _ReturnListArrayEth.Keys
    '                                _ReturnArrayEthAsync(IP) = Nothing
    '                                _ReturnListArrayEth(IP).Clear()
    '                                _ReceivedListArrayEth(IP).Clear()
    '                            Next
    '                        End If
    '                        _MySignal = Signal
    '                        _OldSignal = Signal
    '                        _SendErrorCounter = 0
    '                        Return True
    '                    Else
    '                        _SendErrorCounter = _SendErrorCounter + 1
    '                        If _SendErrorCounter > 1000 Then
    '                            _SendErrorCounter = 0
    '                        End If
    '                        Return False
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Function

    '#End Region

    '#Region "Seriell"

    '    ''' <summary>
    '    ''' Neue Befehlsliste übergeben
    '    ''' </summary>
    '    ''' <param name="CommandList"></param>
    '    ''' <remarks></remarks>
    '    Public Sub PutNewList(ByVal CommandList As List(Of String), ByVal CommandListUncoded As List(Of String))
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If Not _Ethernet Then
    '                        _CommandListString.Clear()
    '                        _CommandListStringUncoded.Clear()
    '                        _CommandListString = CommandList
    '                        _CommandListStringUncoded = CommandListUncoded
    '                        _ReturnListString.Clear()
    '                    Else
    '                        'RaiseEvent ErrorEvent(NOSERIALMODE)
    '                        Trace.TraceError(NOSERIALMODE.Message)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Neue Befehlsliste übergeben
    '    ''' </summary>
    '    ''' <param name="CommandList"></param>
    '    ''' <remarks></remarks>
    '    Public Sub PutNewList(ByVal CommandList As List(Of Byte()), ByVal CommandListUncoded As List(Of String))
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If Not _Ethernet Then
    '                        _CommandListArray.Clear()
    '                        _CommandListArrayUncoded.Clear()
    '                        _CommandListArray = CommandList
    '                        _CommandListArrayUncoded = CommandListUncoded
    '                        _ReturnListArray.Clear()
    '                    Else
    '                        'RaiseEvent ErrorEvent(NOSERIALMODE)
    '                        Trace.TraceError(NOSERIALMODE.ToString)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' genau einen Befehl an genau ein Device senden
    '    ''' </summary>
    '    ''' <param name="Command"></param>
    '    ''' <remarks></remarks>
    '    Public Sub SendAsyncCommandStrSer(ByVal Command As String, ByVal CommandUncoded As String)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If Not _Ethernet Then
    '                        _SpecialCommandStr = Command
    '                        _SpecialCommandUncoded = CommandUncoded
    '                    Else
    '                        'RaiseEvent ErrorEvent(NOSERIALMODE)
    '                        Trace.TraceError(NOSERIALMODE.ToString)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' genau einen Befehl an genau ein Device senden
    '    ''' </summary>
    '    ''' <param name="Command"></param>
    '    ''' <remarks></remarks>
    '    Public Sub SendAsyncCommandArSer(ByVal Command As Byte(), ByVal CommandUncoded As String)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If Not _Ethernet Then
    '                        _SpecialCommandAr = Command
    '                        _SpecialCommandUncoded = CommandUncoded
    '                    Else
    '                        'RaiseEvent ErrorEvent(NOSERIALMODE)
    '                        Trace.TraceError(NOSERIALMODE.ToString)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '#End Region

    '#Region "Ethernet"

    '    ''' <summary>
    '    ''' Neues Gerät hinzufügen
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <param name="Port"></param>
    '    ''' <remarks></remarks>
    '    Public Sub AddDeviceEth(ByVal IPAddress As String, ByVal Port As Integer)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _Ethernet Then
    '                        If Not _DeviceListEth.ContainsKey(IPAddress) Then
    '                            _DeviceListEth.Add(IPAddress, Port)
    '                            _NextCommand.Add(IPAddress, YesNo.YES)  'muss erst mal ja sein für den ersten befehl
    '                            _AllDone.Add(IPAddress, YesNo.NO)
    '                            _CommandCount.Add(IPAddress, 0)
    '                            _CommandInWork.Add(IPAddress, 0)
    '                            _ConnectedList.Add(IPAddress, False)
    '                            TryConnectionToDevice(IPAddress)
    '                        Else
    '                            Dim exe As New Exception("Device with this ID: " & IPAddress & " is already existing")
    '                            'RaiseEvent ErrorEvent(exe)
    '                            Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '                        End If
    '                    Else
    '                        Trace.TraceError(NOEHTERNETMODE.ToString)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Gerät löschen
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <remarks></remarks>
    '    Public Sub DeleteDeviceEth(ByVal IPAddress As String)
    '        Try
    '            If _Ethernet Then
    '                If _DeviceListEth.ContainsKey(IPAddress) Then
    '                    CloseConnection(IPAddress)
    '                    _DeviceListEth.Remove(IPAddress)
    '                    _NextCommand.Remove(IPAddress)
    '                    _AllDone.Remove(IPAddress)
    '                    _CommandCount.Remove(IPAddress)
    '                    _CommandInWork.Remove(IPAddress)
    '                    _ConnectedList.Remove(IPAddress)
    '                Else
    '                    Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                    'RaiseEvent ErrorEvent(exe)
    '                    Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '                End If
    '            Else
    '                Trace.TraceError(NOEHTERNETMODE.ToString)
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' IP Addresse eines Gerätes wechseln.
    '    ''' </summary>
    '    ''' <param name="OldIP"></param>
    '    ''' <param name="NewIP"></param>
    '    ''' <param name="Port"></param>
    '    ''' <remarks></remarks>
    '    Public Sub ChangeDeviceIP(ByVal OldIP As String, ByVal NewIP As String, ByVal Port As Integer)
    '        Dim RunningOld As Boolean = False
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _Running Then 'anhalten wenn ich gerade laufe
    '                        RunningOld = _Running
    '                        StopMachine()
    '                    End If
    '                    DeleteDeviceEth(OldIP)
    '                    AddDeviceEth(NewIP, Port)
    '                    If RunningOld Then 'sollte ich vorher gelaufen sein, dann wieder neu starten
    '                        StartMachine()
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Neue Befehlsliste übergeben
    '    ''' </summary>
    '    ''' <param name="CommandList"></param>
    '    ''' <remarks></remarks>
    '    Public Function PutNewListEth(ByVal IPAddress As String, ByVal CommandList As List(Of String), ByVal CommandListUncoded As List(Of String)) As Boolean
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _Ethernet Then
    '                        If _MyState = States.IDLE Then
    '                            If _DeviceListEth.ContainsKey(IPAddress) Then
    '                                If _CommandListStringEth.ContainsKey(IPAddress) Then
    '                                    _CommandListStringEth.Remove(IPAddress)
    '                                    _CommandListStringEthUncoded.Remove(IPAddress)
    '                                    _ReturnListStringEth.Remove(IPAddress)
    '                                    _ReceivedListStringEth.Remove(IPAddress)
    '                                End If
    '                                Dim Commanddummylist As New List(Of String)
    '                                Dim Commanddummylist1 As New List(Of String)
    '                                For i As Integer = 0 To CommandList.Count - 1
    '                                    Commanddummylist.Add(CommandList(i))
    '                                    Commanddummylist1.Add(CommandListUncoded(i))
    '                                Next
    '                                _CommandListStringEth.Add(IPAddress, Commanddummylist)
    '                                _CommandListStringEthUncoded.Add(IPAddress, Commanddummylist1)
    '                                Dim dummylist As New List(Of String)
    '                                Dim dummylist1 As New List(Of String)
    '                                _ReturnListStringEth.Add(IPAddress, dummylist)
    '                                _ReceivedListStringEth.Add(IPAddress, dummylist1)
    '                                _ReturnListStringEth(IPAddress).Clear()
    '                                _ReceivedListStringEth(IPAddress).Clear()
    '                                Return True
    '                            Else
    '                                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                                'RaiseEvent ErrorEvent(exe)
    '                                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '                                Return False
    '                            End If
    '                        Else
    '                            Return False
    '                        End If
    '                    Else
    '                        Trace.TraceError(NOEHTERNETMODE.ToString)
    '                        Return False
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Function

    '    ''' <summary>
    '    ''' Neue Befehlsliste übergeben
    '    ''' </summary>
    '    ''' <param name="CommandList"></param>
    '    ''' <remarks></remarks>
    '    Public Function PutNewListEth(ByVal IPAddress As String, ByVal CommandList As List(Of Byte()), ByVal CommandListUncoded As List(Of String)) As Boolean
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _Ethernet Then
    '                        If _MyState = States.IDLE Then
    '                            If _DeviceListEth.ContainsKey(IPAddress) Then
    '                                If _CommandListArrayEth.ContainsKey(IPAddress) Then
    '                                    _CommandListArrayEth.Remove(IPAddress)
    '                                    _CommandListArrayEthUncoded.Remove(IPAddress)
    '                                    _ReturnListArrayEth.Remove(IPAddress)
    '                                    _ReceivedListArrayEth.Remove(IPAddress)
    '                                End If
    '                                _CommandListArrayEth.Add(IPAddress, CommandList)
    '                                _CommandListArrayEthUncoded.Add(IPAddress, CommandListUncoded)
    '                                Dim dummylist As New List(Of Byte())
    '                                Dim dummylist1 As New List(Of Byte())
    '                                _ReturnListArrayEth.Add(IPAddress, dummylist)
    '                                _ReceivedListArrayEth.Add(IPAddress, dummylist1)
    '                                _ReturnListArrayEth(IPAddress).Clear()
    '                                _ReceivedListArrayEth(IPAddress).Clear()
    '                                Return True
    '                            Else
    '                                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                                'RaiseEvent ErrorEvent(exe)
    '                                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '                                Return False
    '                            End If
    '                        Else
    '                            Return False
    '                        End If
    '                    Else
    '                        Trace.TraceError(NOEHTERNETMODE.ToString)
    '                        Return False
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Function

    '    ''' <summary>
    '    ''' genau einen Befehl an genau ein Device senden
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <param name="Command"></param>
    '    ''' <remarks></remarks>
    '    Public Sub SendAsyncCommandStrEth(ByVal IPAddress As String, ByVal Command As String, ByVal CommandUncoded As String)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _Ethernet Then
    '                        If _DeviceListEth.ContainsKey(IPAddress) Then
    '                            _SpecialIP = IPAddress
    '                            _SpecialCommandStr = Command
    '                            _SpecialCommandUncoded = CommandUncoded
    '                            If _ReturnStringEthAsync.ContainsKey(IPAddress) Then
    '                                _ReturnStringEthAsync.Remove(IPAddress)
    '                            End If
    '                            _ReturnStringEthAsync.Add(IPAddress, "")
    '                        Else
    '                            Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                            Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '                            'RaiseEvent ErrorEvent(exe)
    '                        End If
    '                    Else
    '                        Trace.TraceError(NOEHTERNETMODE.ToString)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' genau einen Befehl an genau ein Device senden
    '    ''' </summary>
    '    ''' <param name="IPAddress"></param>
    '    ''' <param name="Command"></param>
    '    ''' <remarks></remarks>
    '    Public Sub SendAsyncCommandArEth(ByVal IPAddress As String, ByVal Command As Byte(), ByVal CommandUncoded As String)
    '        Try
    '            If Monitor.TryEnter(_LockObject, 1000) Then
    '                Try
    '                    If _Ethernet Then
    '                        If _DeviceListEth.ContainsKey(IPAddress) Then
    '                            _SpecialIP = IPAddress
    '                            _SpecialCommandAr = Command
    '                            _SpecialCommandUncoded = CommandUncoded
    '                            If _ReturnArrayEthAsync.ContainsKey(IPAddress) Then
    '                                _ReturnArrayEthAsync.Remove(IPAddress)
    '                            End If
    '                            _ReturnArrayEthAsync.Add(IPAddress, Nothing)
    '                        Else
    '                            Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                            'RaiseEvent ErrorEvent(exe)
    '                            Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '                        End If
    '                    Else
    '                        Trace.TraceError(NOEHTERNETMODE.ToString)
    '                    End If
    '                Finally
    '                    Monitor.Exit(_LockObject)
    '                End Try
    '            End If
    '        Catch ex As Exception
    '            _MySignal = Signals.NOSIGNAL
    '            _MyState = States.IDLE
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '#End Region

    '#End Region

    '#Region "Timeout"

    '    ''' <summary>
    '    ''' Timeout für die Socket Connection verwalten
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Class SocketConnectionTimeoutManager

    '#Region "Private Felder"

    '        Private _MySocket As System.Net.Sockets.Socket
    '        Private _Timeout As Integer
    '        Private _Connected As Boolean
    '        Private _Running As Boolean

    '        'Logging
    '        Private _MyTraceHandler As TraceHandler

    '#End Region

    '#Region "Öffentliche Eigenschaften"

    '        ''' <summary>
    '        ''' Socket Objekt
    '        ''' </summary>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public Property Socket() As System.Net.Sockets.Socket
    '            Get
    '                Return _MySocket
    '            End Get
    '            Set(ByVal value As System.Net.Sockets.Socket)
    '                _MySocket = value
    '            End Set
    '        End Property

    '        ''' <summary>
    '        ''' Verbunden?
    '        ''' </summary>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public Property Connected() As Boolean
    '            Get
    '                Return _Connected
    '            End Get
    '            Set(ByVal value As Boolean)
    '                _Connected = value
    '            End Set
    '        End Property

    '        ''' <summary>
    '        ''' Läuft
    '        ''' </summary>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public Property Running() As Boolean
    '            Get
    '                Return _Running
    '            End Get
    '            Set(ByVal value As Boolean)
    '                _Running = value
    '            End Set
    '        End Property

    '#End Region

    '#Region "Logging"

    '        ''' <summary>
    '        ''' Logfile
    '        ''' </summary>
    '        ''' <param name="EntryType"></param>
    '        ''' <param name="LogEntry"></param>
    '        ''' <remarks></remarks>
    '        Private Sub WriteLog(ByVal EntryType As TraceEventType, ByVal LogEntry As String)
    '            'If Not _MyTraceHandler Is Nothing Then
    '            '    _MyTraceHandler.WriteLog(EntryType, 0, LogEntry)
    '            'End If
    '            Trace.TraceInformation(LogEntry)
    '        End Sub

    '#End Region

    '#Region "Öffentliche Methoden"

    '        Public Sub New(ByVal Timeout As Integer, ByVal MyTraceHandler As TraceHandler)
    '            _Timeout = Timeout
    '            _MyTraceHandler = MyTraceHandler
    '        End Sub

    '        Public Sub Timeout()
    '            Dim StartTime As Date
    '            Dim TS As TimeSpan
    '            StartTime = Now
    '            While _Connected = False
    '                WriteLog(TraceEventType.Critical, "ThermoStateMachine, Timeout, Connected: " & _Connected.ToString & ", Running: " & _Running.ToString)
    '                If _Running = False Then Exit While
    '                TS = Now.Subtract(StartTime)
    '                If TS.TotalMilliseconds > _Timeout Then
    '                    Exit While
    '                End If
    '                Thread.Sleep(100) '20
    '            End While
    '            If Not _Connected Then
    '                WriteLog(TraceEventType.Critical, "ThermoStateMachine, Timeout, Not Connected, Connected: " & _Connected.ToString & ", Running: " & _Running.ToString)
    '                _MySocket.Close()
    '            End If
    '        End Sub

    '#End Region

    '    End Class

    '#End Region

End Class

