'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

#Region "Imports"
'$ Imports
Imports System.Net.Sockets
Imports ThermoInterfaces
#End Region

#Region "Schnittstelle"

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ThermoIOControl_Ethernet_WithOutEvents

#Region "Eigenschaften Ethernet"
    '$ Eigenschaften Ethernet

    Private Const _IDLE As Integer = 0
    Private Const _BUSY As Integer = 1

    Private _Socket As System.Net.Sockets.Socket    'Socket
    Private _DataContainer As ThermoDataContainer_Interface 'Datencontainer, IsReceiveready
    Private WithEvents _Protocol As ThermoProtocol_Interface    'Irgendeine Klasse die dieses Interface implementiert, egal welche!; IsReceiveReady
    Private _MyState As Integer 'wo stehe ich?
    Private _ReadBuffer(1024) As Byte   'Puffer für die Leseop
    Private _WriteBuffer(1024) As Byte 'Puffer für die Schreibop
    Private _ReceiveBuffer(1024) As Byte 'Gesamtpuffer, wird bei bedarf erweitert
    Private _ReceivedString As String   'empfangener string
    Private _TimeStamp As DateTime
    Private _Timeout As Integer
    Private _length As Integer = 0
    Private _ArrayIndex As Integer = 0

    Private _Closing As Boolean

    Private _ComMode As Integer
    Private Const _Array As Integer = 1
    Private Const _String As Integer = 2

    Private _ErrorCounter As Integer = 0

    'Flags die die Events ersetzen
    Private _DisconnectedFlag As Boolean

#End Region

#Region "Öffentliche Eigenschaften"

    ''' <summary>
    ''' Empfangspuffer für die Daten von der seriellen Schnittstelle (String)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReceivedString() As String
        Get
            Return _ReceivedString
        End Get
        Set(ByVal value As String)
            _ReceivedString = value
        End Set
    End Property

    ''' <summary>
    ''' Ethernet Schnittstelle
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MySocket() As Socket
        Set(ByVal value As Socket)
            _Socket = value
            _DisconnectedFlag = False
        End Set
    End Property

    ''' <summary>
    ''' Sende und Empfange ich Byte Array´s(1) oder String´s(2)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ComMode() As Integer
        Get
            Return _ComMode
        End Get
        Set(ByVal value As Integer)
            _ComMode = value
        End Set
    End Property

    ''' <summary>
    ''' Timeout
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Timeout() As Integer
        Get
            Return _Timeout
        End Get
        Set(ByVal value As Integer)
            _Timeout = value
        End Set
    End Property

    Public ReadOnly Property DisconnectedFlag() As Boolean
        Get
            Return _DisconnectedFlag
        End Get
    End Property

#End Region

#Region "Private Methoden"
    '$ Private Methoden

    ''' <summary>
    ''' Synchron schreiben
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <remarks></remarks>
    Private Sub WriteAr(ByVal Data() As Byte)
        Try
            If Not _Closing Then
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                Array.Copy(Data, _WriteBuffer, Data.Length)
                Try
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteAr, Data: " & Data.ToString)
                    _Socket.Send(_WriteBuffer)
                Catch ex As Exception
                    _MyState = _IDLE
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteAr, Disconnected")
                    _DisconnectedFlag = True
                End Try
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Synchron schreiben
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <remarks></remarks>
    Private Sub WriteStr(ByVal Data As String)
        Try
            If Not _Closing Then
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _WriteBuffer = System.Text.Encoding.ASCII.GetBytes(Data)
                Try
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteStr, Data: " & Data)
                    _Socket.Send(_WriteBuffer)
                Catch ex As Exception
                    _MyState = _IDLE
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteStr, Disconnected")
                    _DisconnectedFlag = True
                End Try
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Öffentliche Methoden"
    '$ Öffentliche Methoden

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="Eth">Handle für einen geöffneten! Socket</param>
    ''' <param name="Timeout">Timeout für die Abfragen</param>
    ''' <param name="Protocol">Gerätelogik die IsReceiveReady implementiert</param>
    ''' <remarks></remarks>
    Sub New(ByVal Eth As Socket, ByVal Timeout As Integer, ByVal Protocol As ThermoProtocol_Interface, ByVal DataContainer As ThermoDataContainer_Interface)
        Try
            _Closing = False
            _Socket = Eth
            _Timeout = Timeout
            _Protocol = Protocol
            _DataContainer = DataContainer
            _MyState = _IDLE
            _TimeStamp = Now
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
        Try
            _Closing = True
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">Bytestream der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Function DoTransferAr(ByVal Command() As Byte, ByVal Length As Integer, ByVal SendFlag As Integer, ByVal SetBusy As Boolean) As Boolean ' Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferAr
        Try
            Dim RetVal As Boolean = False
            'Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferAr, _MyState=" & _MyState.ToString)
            If _MyState = _IDLE And Not _Closing Then
                If SetBusy Then _MyState = _BUSY
                _ComMode = 1
                Array.Clear(_ReceiveBuffer, 0, _ReceiveBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _ArrayIndex = 0
                If SendFlag = 1 Then
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferAr, StartWriteAr")
                    WriteAr(Command)
                    _TimeStamp = Now
                End If
                RetVal = True
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">String der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Function DoTransferStr(ByVal Command As String, ByVal Length As Integer, ByVal SendFlag As Integer, ByVal SetBusy As Boolean) As Boolean  'Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferStr
        Try
            Dim RetVal As Boolean = False
            'Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferStr, _MyState=" & _MyState.ToString)
            If _MyState = _IDLE And Not _Closing Then
                If SetBusy Then _MyState = _BUSY
                _ComMode = 2
                Array.Clear(_ReceiveBuffer, 0, _ReceiveBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _ReceivedString = ""
                If SendFlag = 1 Then
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferStr, StartWriteStr")
                    WriteStr(Command)
                    _TimeStamp = Now
                End If
                RetVal = True
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

    ''' <summary>
    ''' Synchron lesen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ReadData() As Boolean
        Dim ReturnValue As Boolean = False

        If Not _Closing Then
            Try

                If _Socket.Available > 0 Then

                    'Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, IP: " & _Socket.RemoteEndPoint.ToString & " ,ComMode: " & _ComMode.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)

                    Select Case _ComMode

                        Case _Array

                            Try
                                _Socket.Receive(_ReadBuffer)
                            Catch ex As System.Net.Sockets.SocketException
                            End Try

                            'If Array.IndexOf(_ReadBuffer, Nothing) > 0 Then
                            '    _ErrorCounter = _ErrorCounter + 1
                            '    If _ErrorCounter >= 50 Then 'nach 50 sinnlosen Aktionen auf der Leitung fliegt er raus -> wenn ein Client einfach stirbt ohne den socket korrekt zu schliessen
                            '        RaiseEvent Disconnected()
                            '    End If
                            'Else
                            '    _ErrorCounter = _ErrorCounter - 1
                            'End If
                            'If _ErrorCounter < 0 Then _ErrorCounter = 0
                            _length = Array.IndexOf(_ReadBuffer, Nothing)
                            If _length = -1 Then _length = _ReadBuffer.Length
                            ReDim Preserve _ReceiveBuffer(_ReceiveBuffer.Length + _length)
                            Array.ConstrainedCopy(_ReadBuffer, 0, _ReceiveBuffer, _ArrayIndex, _length)
                            _ArrayIndex = _ArrayIndex + _length
                            Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex}
                            If _Protocol.IsReceiveReady(_DataContainer, Arguments) = 1 Then 'Empfang ok???
                                _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                _MyState = _IDLE
                                ReturnValue = True
                            End If

                        Case _String

                            Try
                                _Socket.Receive(_ReadBuffer)
                            Catch ex As System.Net.Sockets.SocketException
                            End Try
                            _length = Array.IndexOf(_ReadBuffer, Nothing)
                            If _length = -1 Then _length = _ReadBuffer.Length
                            _ReceivedString = _ReceivedString & System.Text.Encoding.ASCII.GetString(_ReadBuffer, 0, _length)
                            'Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, ReceivedString: °" & _ReceivedString & "°, IP: " & _Socket.RemoteEndPoint.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
                            If _ReceivedString = "" Then 'WICHTIG. Sonst kann es bei Verbindungsverlust durch dauerndes StartRead zu einem Hänger kommen
                                _ErrorCounter = _ErrorCounter + 1
                                'Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, Errorcounter Added 1: " & _ErrorCounter.ToString & " , IP: " & _Socket.RemoteEndPoint.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
                                If _ErrorCounter >= 20 Then 'nach 50 sinnlosen Aktionen auf der Leitung fliegt er raus -> wenn ein Client einfach stirbt ohne den socket korrekt zu schliessen
                                    'Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, Disconnected, IP: " & _Socket.RemoteEndPoint.ToString)
                                    _DisconnectedFlag = True
                                End If
                            Else
                                _ErrorCounter = _ErrorCounter - 1
                                'Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, Errorcounter Subtracted 1: " & _ErrorCounter.ToString & " , IP: " & _Socket.RemoteEndPoint.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
                            End If
                            If _ErrorCounter < 0 Then _ErrorCounter = 0

                            Dim Arguments() As Object = {_ReceivedString}
                            If _Protocol.IsReceiveReady(_DataContainer, Arguments) = 1 Then     'Empfang ok???
                                _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                _MyState = _IDLE
                                _ReceivedString = ""
                                ReturnValue = True
                            End If

                    End Select


                End If

                Return ReturnValue

            Catch ex As Exception
                _MyState = _IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
            End Try
        End If
    End Function

    ''' <summary>
    ''' Timeout überwachen
    ''' </summary>
    ''' <returns>
    ''' 0 = no timeout
    ''' 1 = timeout
    ''' 2 = timeout but new data available
    ''' </returns>
    ''' <remarks></remarks>
    Public Function IsTimeOut() As Integer
        Dim dt As DateTime
        Dim ReturnValue As Integer = 0
        Try

            dt = Now
            Dim ts As TimeSpan = dt.Subtract(_TimeStamp)
            If (ts.TotalMilliseconds > _Timeout) Then
                Try
                    If _Socket.Available = 0 Then
                        _MyState = _IDLE
                        ReturnValue = 1
                    Else
                        'scheinbar sind doch noch Daten da, also noch einmal lesen
                        If Not ReadData() Then 'konnten nicht gelesen werden
                            _MyState = _IDLE
                            ReturnValue = 1
                        Else 'daten doch noch gelesen
                            _MyState = _IDLE
                            ReturnValue = 2
                        End If
                    End If
                Catch ex As System.Net.Sockets.SocketException
                    _MyState = _IDLE
                    _DisconnectedFlag = True
                End Try
            End If

            Return ReturnValue

        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

#End Region

End Class

#End Region
