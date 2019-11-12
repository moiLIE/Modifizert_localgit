#Region "Imports"
Imports ThermoWebServices
Imports ThermoLogging
Imports ThermoUtilities
Imports ThermoInterfaces
Imports System.Text
Imports System.Globalization
#End Region

Public Class FHT59N3_RemoteControlWebServer

#Region "Private Eigenschaften"

    Private WithEvents _MyWebServer As ThermoWebServer   'der eigentliche WebServer
    Private _TCPPort As Integer = 5556 'mein TCPPort
    Private _MaxClients As Integer = 1 'wieviele Klienten darf ich maximal haben
    Private _Users As Integer   'Anzahl an Nutzern in der Nutzerdb
    Private _Authentification As New Dictionary(Of String, String)   'Nutzerauthentification (Name und Passwort als Hash)
    Private _UserRights As New Dictionary(Of String, Integer)    'Nutzerauthentification (Name und Passwort als Hash)
    Private _MyUtillities As New ThermoUtilities.ThermoUtilities 'Zeuch
    Private _Password As String 'Passwort für das Servermanagement
    Private _NewPassword As Boolean 'neues Paswort anlegen?
    Private _LogFilePath As String = ""
    Private _MyServerLog As New ThermoHistory_Text(20, 0, _LogFilePath & "\FHT59N3_RemoteLog_" & Format(Now, "ddMM") & ".log", "", vbCrLf)    'ServerLog

    'Verbindung zum eigentlichen Gerät
    Private _ProtocolArgs As String
    Private _DeviceArgs As String
    Private _DataContainerArgs As String
    Private _ProtocolArguments As String()
    Private _DeviceArguments As String()
    Private _DataContainerArguments As String()

    Private _MyTraceHandler As TraceHandler

#End Region

#Region "Öffentliche Eigenschaften"

    Public Event CommandReceived(ByVal ClientNumber As String, ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface)

#End Region

#Region "Private Methoden"


    Private Function GetArguments(ByVal Arguments As String) As String()
        Try
            Dim Dummystring As String = ""
            Dim Args As String() = {}
            Dim i As Integer = 0
            Do
                Dummystring = _MyUtillities.ExtrahiereZeichenkette(Arguments, ";")
                If Dummystring <> "" Then
                    ReDim Preserve Args(i)
                    Args(i) = Dummystring
                    i = i + 1
                End If
            Loop Until Dummystring = ""
            Return Args
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Server meldet neue Verbindung
    ''' </summary>
    ''' <param name="IP"></param>
    ''' <param name="Port"></param>
    ''' <param name="Number"></param>
    ''' <remarks></remarks>
    Private Sub NewConnection(ByVal IP As String, ByVal Port As String, ByVal Number As String, ByVal ActClients As String) Handles _MyWebServer.ConnectionEstablished
        Try
            WriteLog("New Connection(" & Number.ToString & ") from IP: " & IP & " Port: " & Port, True)
            WriteLog("Actual Connections: " & ActClients, True)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Server meldet Verbindungsverlust
    ''' </summary>
    ''' <param name="ClientNumber"></param>
    ''' <param name="IP"></param>
    ''' <param name="Port"></param>
    ''' <remarks></remarks>
    Private Sub ConnectionLost(ByVal ClientNumber As String, ByVal IP As String, ByVal Port As String, ByVal ActClients As String) Handles _MyWebServer.ConnectionLost
        Try
            WriteLog("Lost Connection(" & ClientNumber & ") from IP: " & IP & " Port: " & Port, True)
            WriteLog("Actual Connections: " & ActClients, True)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Server meldet erfolgreiches Login
    ''' </summary>
    ''' <param name="ClientNumber"></param>
    ''' <param name="UserName"></param>
    ''' <remarks></remarks>
    Private Sub ClientLoggedIn(ByVal ClientNumber As String, ByVal UserName As String) Handles _MyWebServer.ClientLoggedIn
        Try
            WriteLog("Client(" & ClientNumber & ") User: " & UserName, True)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Was wollte der Client wissen?
    ''' </summary>
    ''' <param name="ClientNumber"></param>
    ''' <param name="Request"></param>
    ''' <remarks></remarks>
    Private Sub ClientRequest(ByVal ClientNumber As String, ByVal Request As String) Handles _MyWebServer.ClientRequest
        Try
            WriteLog("Client(" & ClientNumber & ") Command: " & Request, True)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Was habe ich geantwortet
    ''' </summary>
    ''' <param name="ClientNumber"></param>
    ''' <param name="Response"></param>
    ''' <remarks></remarks>
    Private Sub ServerResponse(ByVal ClientNumber As String, ByVal Response As String) Handles _MyWebServer.ServerResponse
        Try
            WriteLog("Server(" & ClientNumber & ") Response: " & Response, True)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' ServerLog schreiben
    ''' </summary>
    ''' <param name="LogEntry"></param>
    ''' <remarks></remarks>
    Private Sub WriteLog(ByVal LogEntry As String, ByVal WriteFlag As Boolean)
        Try
            If WriteFlag Then
                LogEntry = LogEntry.TrimEnd(vbCrLf.ToCharArray) 'falls schon eins dranhängt dann weg damit
                Dim _LogEntry() As String = {Format(Now, "dd.MM.yy HH:mm:ss") & " " & LogEntry}
                _MyServerLog.FileName = _LogFilePath & "\FHT59N3_RemoteLog_" & Format(Now, "ddMM") & ".log"
                _MyServerLog.AddToHistory(_LogEntry)
            End If
        Catch ex As Exception
            Trace.TraceError("Error while writing to RemoteLog file: " & ex.Message)
        End Try
    End Sub

    Private Sub CommandReceivedHandler(ByVal ClientNumber As String, ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface) Handles _MyWebServer.ClientCommandReceived
        RaiseEvent CommandReceived(ClientNumber, DataContainer)
    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub New(ByVal LogFilePath As String)
        Try
            Dim _ApplPath As String
            _ApplPath = _MyUtillities.GetApplicationPath("FHT59N3.exe")
            _LogFilePath = LogFilePath
            If _ProtocolArgs <> "" Then
                _ProtocolArguments = GetArguments(_ProtocolArgs)
            End If
            If _DeviceArgs <> "" Then
                _DeviceArguments = GetArguments(_DeviceArgs)
            End If
            If _DataContainerArgs <> "" Then
                _DataContainerArguments = GetArguments(_DataContainerArgs)
            End If
            _MyWebServer = New ThermoWebServer(_TCPPort, _MaxClients, _Authentification, _UserRights, _ApplPath & "\ThermoProtocols.dll", _ApplPath & "\FHT59N3Core.dll", _ApplPath & "\FHT59N3Core.dll", "ThermoProtocols.ThermoDataControl_Protocol", "FHT59N3Core.FHT59N3_RemoteControlLogic", "FHT59N3Core.FHT59N3_RemoteControlLogic_DataContainer", _ProtocolArguments, _DeviceArguments, _DataContainerArguments, _MyTraceHandler)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Dispose()
        Try
            _MyServerLog.StoreHistory(True)
            _MyWebServer.Dispose()
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

End Class

''' <summary>
''' Kontrollogik für den Datenserver für das Net6020GN
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3_RemoteControlLogic
    Implements ThermoInterfaces.ThermoVirtualDevice_Interface

#Region "Private Eigenschaften"

    'Verschiedenes
    'Private _RemoteContainer As FHT59N3_RemoteControlLogic_DataContainer   'Datencontainer, den mir die Serverapplikation mitgibt, in diesen wird die Antwort geschrieben
    Private WithEvents _Protocol As ThermoProtocols.ThermoDataControl_Protocol  'zur Kodierung
    'welche Rechte haben meine Klienten Typ1 = Messapplikation (darf alles), Typ2 = normaler Klient (nur lesen)

    'Nachrichten an den Nutzer
    Private Const _OK As String = "OK"
    Private Const _ERROR As String = "ERROR (FHT59N3_RemoteControlLogic)"

    Private _Disposed As Boolean


#End Region

#Region "Öffentliche Eigenschaften"

    ''' <summary>
    ''' Datencontainer zur Aufnahme der Daten
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataContainer() As ThermoInterfaces.ThermoDataContainer_Interface Implements ThermoInterfaces.ThermoVirtualDevice_Interface.DataContainer
        Get
            Return Nothing
        End Get
        Set(ByVal value As ThermoInterfaces.ThermoDataContainer_Interface)
            '_RemoteContainer = CType(value, FHT59N3_RemoteControlLogic_DataContainer)
        End Set
    End Property

    ''' <summary>
    ''' Datenformat
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DataFormat() As String Implements ThermoInterfaces.ThermoVirtualDevice_Interface.DataFormat
        Get
            Return "String"
        End Get
    End Property

    ''' <summary>
    ''' Typ der Applikation : Zugriffsbeschränkung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ApplicationType() As Integer Implements ThermoInterfaces.ThermoVirtualDevice_Interface.ApplicationType
        Get
            Return 1
        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public ReadOnly Property Busy() As Boolean Implements ThermoInterfaces.ThermoVirtualDevice_Interface.Busy
        Get
            Return False
        End Get
    End Property

    ''' <summary>
    ''' etwas ist faul
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As System.Exception) Implements ThermoInterfaces.ThermoVirtualDevice_Interface.ErrorEvent

    ''' <summary>
    ''' Operation abgeschlossen
    ''' </summary>
    ''' <remarks></remarks>
    Public Event OperationDone() Implements ThermoInterfaces.ThermoVirtualDevice_Interface.OperationDone

    Public Event CommandReceived(ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface) Implements ThermoInterfaces.ThermoVirtualDevice_Interface.CommandReceived

    ''' <summary>
    ''' Operation abgeschlossen, Nachricht an alle Klienten
    ''' </summary>
    ''' <param name="ApplicationType">0 = alle; 1 = Typ 1(Messapplikationen); 2 = Typ 2 ("normale" Klienten)</param>
    ''' <remarks></remarks>
    Public Event OperationDoneBroadcast(ByVal MessageToClient As String, ByVal ApplicationType As Integer) Implements ThermoInterfaces.ThermoVirtualDevice_Interface.OperationDoneBroadcast

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="Protocol"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Protocol As ThermoProtocols.ThermoDataControl_Protocol, ByVal DataContainer As FHT59N3_RemoteControlLogic_DataContainer, ByVal ParamArray Arguments() As String)
        Try
            _Disposed = False
            _Protocol = Protocol
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Kommandopulk auf den Stack legen
    ''' </summary>
    ''' <param name="DataContainer"></param>
    ''' <remarks></remarks>
    Public Sub PutCommand(ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface, ByVal CommandPile As String) Implements ThermoInterfaces.ThermoVirtualDevice_Interface.PutCommand
        Try
            _Protocol.BuildProtocolFrame(DataContainer, _ERROR)
            RaiseEvent OperationDone()
        Catch ex As Exception
            _Protocol.BuildProtocolFrame(DataContainer, _ERROR)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent OperationDone()
        End Try
    End Sub

    ''' <summary>
    ''' Irgend ein Kommando ausführen
    ''' </summary>
    ''' <param name="DataContainer"></param>
    ''' <remarks></remarks>
    Public Sub DoCommand(ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface) Implements ThermoInterfaces.ThermoVirtualDevice_Interface.DoCommand
        Try
            RaiseEvent CommandReceived(DataContainer)
            RaiseEvent OperationDone()
        Catch ex As Exception
            _Protocol.BuildProtocolFrame(DataContainer, _ERROR)
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent OperationDone()
        End Try
    End Sub

    Public Sub Dispose() Implements ThermoInterfaces.ThermoVirtualDevice_Interface.Dispose
        Try
            _Disposed = True
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SetStack() Implements ThermoInterfaces.ThermoVirtualDevice_Interface.SetStack

    End Sub

#End Region

End Class

''' <summary>
''' Datencontainer für einen Datenserver für das Net6020GN Programm
''' </summary>
''' <remarks></remarks>
Public Class FHT59N3_RemoteControlLogic_DataContainer
    Implements ThermoInterfaces.ThermoDataContainer_Interface

#Region "Private Felder"

    'Allgemeine Felder
    Private _Command As String
    Private _CommandAsString As String 'mit protocol
    Private _CommandAsArray(1000) As Byte   'mit Protocol
    Private _CommandLength As Integer
    Private _AnswerAsString As String    'ohne protocol
    Private _AnswerAsArray(1000) As Byte     'ohne protocol
    Private _AnswerLength As Integer

#End Region

#Region "Öffentliche Eigenschaften"

    ''' <summary>
    ''' zu versendendes Kommando
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Command() As String Implements ThermoInterfaces.ThermoDataContainer_Interface.Command
        Get
            Return _Command
        End Get
        Set(ByVal value As String)
            _Command = value
        End Set
    End Property

    Public Property CommandAsString() As String Implements ThermoInterfaces.ThermoDataContainer_Interface.CommandAsString
        Get
            Return _CommandAsString
        End Get
        Set(ByVal value As String)
            _CommandAsString = value
        End Set
    End Property

    Public Property CommandAsArray() As Byte() Implements ThermoInterfaces.ThermoDataContainer_Interface.CommandAsArray
        Get
            Return _CommandAsArray
        End Get
        Set(ByVal value As Byte())
            value.CopyTo(_CommandAsArray, 0)
        End Set
    End Property

    Public Property CommandLength() As Integer Implements ThermoInterfaces.ThermoDataContainer_Interface.CommandLength
        Get
            Return _CommandLength
        End Get
        Set(ByVal value As Integer)
            _CommandLength = value
        End Set
    End Property

    Public Property AnswerAsString() As String Implements ThermoInterfaces.ThermoDataContainer_Interface.AnswerAsString
        Get
            Return _AnswerAsString
        End Get
        Set(ByVal value As String)
            _AnswerAsString = value
        End Set
    End Property

    Public Property AnswerAsArray() As Byte() Implements ThermoInterfaces.ThermoDataContainer_Interface.AnswerAsArray
        Get
            Return _AnswerAsArray
        End Get
        Set(ByVal value As Byte())
            value.CopyTo(_AnswerAsArray, 0)
        End Set
    End Property

    Public Property AnswerLength() As Integer Implements ThermoInterfaces.ThermoDataContainer_Interface.AnswerLength
        Get
            Return _AnswerLength
        End Get
        Set(ByVal value As Integer)
            _AnswerLength = value
        End Set
    End Property

#End Region

End Class
