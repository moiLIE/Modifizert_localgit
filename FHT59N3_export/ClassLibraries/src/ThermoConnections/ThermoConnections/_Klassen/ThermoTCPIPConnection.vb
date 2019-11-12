'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

#Region "Imports"
Imports System.Threading
Imports System.Net.Sockets
#End Region

''' <summary>
''' Diese Klasse soll eine Verbindung zu einer IP Adresse und einem Port herstellen. Gleichzeitig ist sie dafür zuständig
''' diese Verbindung aufrecht zu erhalten und bei Verbindungsverlust zu versuchen diese Verbindung wieder aufzunehmen
''' </summary>
''' <remarks></remarks>
Public Class ThermoTCPIPConnection

#Region "Private Eigenschaften"

    'Ethernet
    Private _MySocket As Socket = Nothing 'Socket
    Private _IPAddress As String
    Private _Port As Integer
    Private _StillTryingToConnect As Boolean
    Private _Connected As Boolean

#End Region

#Region "Öffentliche Eigenschaften"

    Public ReadOnly Property MySocket() As Socket
        Get
            Return _MySocket
        End Get
    End Property

    Public ReadOnly Property Connected() As Boolean
        Get
            Return _MySocket.Connected
        End Get
    End Property

#End Region

#Region "Private Methoden"

#Region "Allgemein"

#End Region

#Region "Ethernet"

    ''' <summary>
    ''' Sucht die Verbindung zum Device via asynchronem Call von _MySocket.BeginConnect()
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetupConnectionToDevice()
        Try
            _StillTryingToConnect = True
            Dim _MyServerEndPoint As System.Net.IPEndPoint 'IP, Port
            Dim _IPAdresse As System.Net.IPAddress 'IP
            _IPAdresse = System.Net.IPAddress.Parse(_IPAddress)
            _MyServerEndPoint = New System.Net.IPEndPoint(_IPAdresse, _Port)
            Try
                _MySocket.Close()
                _MySocket = Nothing
            Catch ex As Exception
            End Try
            _MySocket = New System.Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)
            _MySocket.BeginConnect(_MyServerEndPoint, New AsyncCallback(AddressOf Connect_Callback), _MySocket)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Private Sub Connect_Callback(ByVal ar As System.IAsyncResult)
        Try
            Dim s As Socket = CType(ar.AsyncState, Socket)
            s.EndConnect(ar)
            _MySocket = s
            _Connected = True
            _StillTryingToConnect = False
        Catch ex As Exception
            _Connected = False
            _StillTryingToConnect = False
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal IPAddress As String, ByVal Port As Integer, ByVal ConnectionTimeout As Integer)
        Try
            _IPAddress = IPAddress
            _Port = Port
            _StillTryingToConnect = False
            _Connected = False
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Connect()
        Try
            If Not _StillTryingToConnect Then
                SetupConnectionToDevice()
            End If
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' alles zerstören
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose()
        Try
            If Not _MySocket Is Nothing Then
                _MySocket.Close()
                _MySocket = Nothing
            End If
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

End Class
