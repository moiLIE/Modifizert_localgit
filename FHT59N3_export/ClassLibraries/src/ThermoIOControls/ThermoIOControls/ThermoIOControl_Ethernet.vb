'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

Option Strict Off

#Region "Imports"
'$ Imports
Imports System.Net.Sockets
Imports ThermoInterfaces
Imports system.Threading
#End Region

#Region "Schnittstelle"

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ThermoIOControl_Ethernet
    Implements ThermoInterfaces.ThermoIOControls_Interface

#Region "Eigenschaften Ethernet"
	'$ Eigenschaften Ethernet

	Private Const _IDLE As Integer = 0
	Private Const _BUSY As Integer = 1

	Private _Socket As System.Net.Sockets.Socket	'Socket
	Private _DataContainer As ThermoDataContainer_Interface	'Datencontainer, IsReceiveready
	Private WithEvents _Protocol As ThermoProtocol_Interface	'Irgendeine Klasse die dieses Interface implementiert, egal welche!; IsReceiveReady
	Private _MyState As Integer	'wo stehe ich?
	Private _ReadBuffer(1024) As Byte	'Puffer für die Leseop
	Private _WriteBuffer(1024) As Byte 'Puffer für die Schreibop
	Private _ReceiveBuffer(1024) As Byte 'Gesamtpuffer, wird bei bedraf erweitert
	Private _ReceivedString As String	'empfangener string
	Private _TimTimeout As Thread	'timeout verwalter
	Private _TimeStamp As DateTime
	Private _Timeout As Integer
	Private _read As Integer = 0
	Private _send As Integer = 0
	Private _length As Integer = 0
	Private _ArrayIndex As Integer = 0

	Private _Closing As Boolean

	Private _ComMode As Integer
	Private Const _Array As Integer = 1
	Private Const _String As Integer = 2

    Private _ErrorCounter As Integer = 0

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
	Public Shadows Property Timeout() As Integer Implements ThermoIOControls_Interface.Timeout
		Get
			Return _Timeout
		End Get
		Set(ByVal value As Integer)
			_Timeout = value
		End Set
	End Property

	''' <summary>
	''' Event das Daten vom seriellen Port empfangen wurden (Byte Array)
	''' </summary>
	''' <remarks></remarks>
	Public Shadows Event DataReceivedEventAr() Implements ThermoInterfaces.ThermoIOControls_Interface.DataReceivedEventAr

	''' <summary>
	''' Event das Daten vom seriellen Port empfangen wurden (String)
	''' </summary>
	''' <remarks></remarks>
	Public Shadows Event DataReceivedEventStr() Implements ThermoInterfaces.ThermoIOControls_Interface.DataReceivedEventStr

	''' <summary>
	''' Timeout der Übertragung
	''' </summary>
	''' <remarks></remarks>
	Public Shadows Event TimeOutEvent() Implements ThermoInterfaces.ThermoIOControls_Interface.TimeOutEvent

	''' <summary>
	''' Fehler
	''' </summary>
	''' <param name="ex">
	''' Exception Klasse
	''' </param>
	''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Shadows Event ErrorEvent(ByVal ex As Exception) Implements ThermoInterfaces.ThermoIOControls_Interface.ErrorEvent

	''' <summary>
	''' Verbindung unterbrochen
	''' </summary>
	''' <remarks></remarks>
	Public Shadows Event Disconnected() Implements ThermoInterfaces.ThermoIOControls_Interface.Disconnected

#End Region

#Region "Private Methoden"
	'$ Private Methoden

	''' <summary>
	''' Fehlerbehandlung
	''' </summary>
	''' <param name="ex"></param>
	''' <remarks></remarks>
    Private Sub ErrorHandler(ByVal ex As Exception) 'Handles _Protocol.ErrorEvent
        If (Not ex.Message = "ThreadAbortException") And (Not ex.Message = "Der Thread wurde abgebrochen.") And (Not ex.Message = "Thread was being aborted.") Then
            'RaiseEvent ErrorEvent(ex)
        End If
    End Sub

    ''' <summary>
    ''' Timeout überwachen
    ''' </summary>
    ''' <remarks></remarks>
	Private Sub TimTimeout_TimerEvent()	'(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles _TimTimeout.Elapsed
		Dim dt As DateTime
		Do
			Try

				If _MyState = _BUSY Then

					dt = Now
					Dim ts As TimeSpan = dt.Subtract(_TimeStamp)
					If ts.TotalMilliseconds > _Timeout Then
                        _MyState = _IDLE
                        Trace.TraceWarning("ThermoIOControl_Ehternet, Timeout, IP: " & _Socket.RemoteEndPoint.ToString)
                        RaiseEvent TimeOutEvent()
					End If

				End If

                Thread.Sleep(200)

			Catch ex As Exception
				_MyState = _IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent TimeOutEvent()
            End Try
		Loop Until _Closing
	End Sub

	''' <summary>
	''' Asynchron schreiben
	''' </summary>
	''' <param name="Data"></param>
	''' <remarks></remarks>
	Private Sub StartWriteAr(ByVal Data() As Byte)
        Try
            If Not _Closing Then
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                Array.Copy(Data, _WriteBuffer, Data.Length)
                Try
                    Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteAr, Data: " & Data.ToString)
                    _Socket.BeginSend(_WriteBuffer, 0, _WriteBuffer.Length, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf WriteCallback), _Socket)
                Catch ex As Exception
                    _MyState = _IDLE
                    Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteAr, Disconnected")
                    RaiseEvent Disconnected()
                End Try
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
	End Sub

	''' <summary>
	''' Asynchron schreiben
	''' </summary>
	''' <param name="Data"></param>
	''' <remarks></remarks>
	Private Sub StartWriteStr(ByVal Data As String)
		Try
			If Not _Closing Then
				ReDim _WriteBuffer(Data.Length)
				Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
				Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
				_WriteBuffer = System.Text.Encoding.ASCII.GetBytes(Data)
                Try
                    Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteStr, Data: " & Data)
                    _Socket.BeginSend(_WriteBuffer, 0, _WriteBuffer.Length, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf WriteCallback), _Socket)
                Catch ex As Exception
                    _MyState = _IDLE
                    Trace.TraceWarning("ThermoIOControl_Ehternet, StartWriteStr, Disconnected")
                    RaiseEvent Disconnected()
                End Try
			End If
		Catch ex As Exception
			_MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
	End Sub

	''' <summary>
	''' schreiben beendet
	''' </summary>
	''' <param name="ar"></param>
	''' <remarks></remarks>
	Private Sub WriteCallback(ByVal ar As System.IAsyncResult)
		Try
			If Not _Closing Then
                Try
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, WriteCallback")
                    _send = _Socket.EndSend(ar)
                Catch ex As Exception
                    _MyState = _IDLE
                    Trace.TraceWarning("ThermoIOControl_Ehternet, WriteCallBack, Disconnected")
                    RaiseEvent Disconnected()
                End Try
			End If
		Catch ex As Exception
			_MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
	End Sub

	''' <summary>
	''' asynchron lesen
	''' </summary>
	''' <remarks></remarks>
	Private Sub StartRead()
		Try
            If Not _Closing Then
                System.Windows.Forms.Application.DoEvents() 'WICHTIG. Sonst kann es bei Verbindungsverlust durch dauerndes StartRead zu einem Hänger kommen
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                Try
                    'Trace.TraceWarning("ThermoIOControl_Ehternet, StartRead, Connected: " & _Socket.Connected.ToString)
                    _Socket.BeginReceive(_ReadBuffer, 0, _ReadBuffer.Length, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ReadCallback), _Socket)
                Catch ex As Exception
                    _MyState = _IDLE
                    Trace.TraceWarning("ThermoIOControl_Ehternet, StartRead, Disconnected")
                    RaiseEvent Disconnected()
                End Try
            End If
		Catch ex As Exception
			_MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
	End Sub

	''' <summary>
	''' lesen beendet
	''' </summary>
	''' <param name="ar"></param>
	''' <remarks></remarks>
	Private Sub ReadCallback(ByVal ar As System.IAsyncResult)
		If Not _Closing Then
            Try

                Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, IP: " & _Socket.RemoteEndPoint.ToString & " ,ComMode: " & _ComMode.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)


                Select Case _ComMode

                    Case _Array

                        Try
                            _read = _Socket.EndReceive(ar)
                        Catch ex As Exception
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
                        Try 'wenn der Socket schon closed ist, kann es trotzdem sein das wir noch hierher kommen!
                            If _Socket.Available > 0 And _length <> 0 Then  'wurde überhaupt was gelesen und gibt es noch mehr???
                                StartRead()
                                Exit Sub
                            End If
                        Catch ex As Exception
                            _MyState = _IDLE
                        End Try

                        Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex}
                        If _Protocol.IsReceiveReady(_DataContainer, Arguments) = 1 Then 'Empfang ok???
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = _IDLE
                            RaiseEvent DataReceivedEventAr()        'Alles ok für dieses Gerät
                        End If

                    Case _String

                        Try
                            _read = _Socket.EndReceive(ar)
                        Catch ex As System.Net.Sockets.SocketException
                        End Try
                        _length = Array.IndexOf(_ReadBuffer, Nothing)
                        If _length = -1 Then _length = _ReadBuffer.Length
                        _ReceivedString = _ReceivedString & System.Text.Encoding.ASCII.GetString(_ReadBuffer, 0, _length)
                        Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, ReceivedString: °" & _ReceivedString & "°, IP: " & _Socket.RemoteEndPoint.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
                        If _ReceivedString = "" Then 'WICHTIG. Sonst kann es bei Verbindungsverlust durch dauerndes StartRead zu einem Hänger kommen
                            _ErrorCounter = _ErrorCounter + 1
                            Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, Errorcounter Added 1: " & _ErrorCounter.ToString & " , IP: " & _Socket.RemoteEndPoint.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
                            If _ErrorCounter >= 20 Then 'nach 50 sinnlosen Aktionen auf der Leitung fliegt er raus -> wenn ein Client einfach stirbt ohne den socket korrekt zu schliessen
                                Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, Disconnected, IP: " & _Socket.RemoteEndPoint.ToString)
                                RaiseEvent Disconnected()
                            End If
                        Else
                            _ErrorCounter = _ErrorCounter - 1
                            Trace.TraceWarning("ThermoIOControl_Ehternet, ReadCallback, Errorcounter Subtracted 1: " & _ErrorCounter.ToString & " , IP: " & _Socket.RemoteEndPoint.ToString & " ,ThreadID: " & Thread.CurrentThread.ManagedThreadId.ToString)
                        End If
                        If _ErrorCounter < 0 Then _ErrorCounter = 0
                        Try 'wenn der Socket schon closed ist, kann es trotzdem sein das wir noch hierher kommen!
                            If _Socket.Available > 0 And _length <> 0 Then  'wurde überhaupt was gelesen und gibt es noch mehr???
                                StartRead()
                                Exit Sub
                            End If
                        Catch ex As Exception
                            _MyState = _IDLE
                        End Try

                        Dim Arguments() As Object = {_ReceivedString}
                        If _Protocol.IsReceiveReady(_DataContainer, Arguments) = 1 Then     'Empfang ok???
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = _IDLE
                            _ReceivedString = ""
                            RaiseEvent DataReceivedEventStr()    'Alles ok für dieses Gerät
                        End If

                End Select

                StartRead() 'wieder mit lesen beginnen

            Catch ex As Exception
                _MyState = _IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent TimeOutEvent()
            End Try
		End If
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
			_TimTimeout = New Thread(AddressOf TimTimeout_TimerEvent)
			_TimTimeout.Start()
			StartRead()
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
			Try
				_TimTimeout.Abort()
			Catch ex As Exception
			End Try
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
	Public Shadows Sub DoTransferAr(ByVal Command() As Byte, ByVal Length As Integer, ByVal SendFlag As Integer) Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferAr
        Try
            Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferAr, _MyState=" & _MyState.ToString)
            If _MyState = _IDLE And Not _Closing Then
                _MyState = _BUSY
                _TimeStamp = Now
                _ComMode = 1
                Array.Clear(_ReceiveBuffer, 0, _ReceiveBuffer.Length)
                _ArrayIndex = 0
                If SendFlag = 1 Then
                    Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferAr, StartWriteAr")
                    StartWriteAr(Command)
                End If
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
	End Sub

	''' <summary>
	''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
	''' </summary>
	''' <param name="Command">String der den Befehl beinhaltet</param>
	''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
	''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
	''' <remarks></remarks>
	Public Shadows Sub DoTransferStr(ByVal Command As String, ByVal Length As Integer, ByVal SendFlag As Integer) Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferStr

        Try
            Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferStr, _MyState=" & _MyState.ToString)
            If _MyState = _IDLE And Not _Closing Then
                _MyState = _BUSY
                _TimeStamp = Now
                _ComMode = 2
                _ReceivedString = ""
                If SendFlag = 1 Then
                    Trace.TraceWarning("ThermoIOControl_Ehternet, DoTransferStr, StartWriteStr")
                    StartWriteStr(Command)
                End If
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent TimeOutEvent()
        End Try
	End Sub

#End Region

End Class

#End Region