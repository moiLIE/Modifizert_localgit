'GEHT LEIDER SO NICHT RICHTIG - WAHRSCHEINLICH FEHLEN DIE EINSTELLUNGEN BAUDRATE, PARITY USW.
'KOMISCHERWEISE GEHT ES MIT DEM CF OHNE PROBLEME

'Option Strict Off

'Imports ThermoUtilities
'Imports System.Runtime.Interopservices

'''' <summary>
'''' Serielles PortObjekt, dem normalen aus dem Framework nachempfunden
'''' </summary>
'''' <remarks></remarks>
'Public Class ThermoSerialPort

'#Region "DLL Deklarationen"

'	<DllImport("kernel32.dll")> Private Shared Function _
'	CreateFile(<MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String, _
'	ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, _
'	ByVal lpSecurityAttributes As Integer, ByVal dwCreationDisposition As Integer, _
'	ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As Integer) As Integer
'	End Function

'	<DllImport("kernel32.dll")> Private Shared Function _
'	ReadFile(ByVal hFile As Integer, ByVal Buffer As Byte(), _
'	ByVal nNumberOfBytesToRead As Integer, ByRef lpNumberOfBytesRead As Integer, _
'	ByVal lpOverlapped As Integer) As Integer
'	End Function

'	<DllImport("kernel32.dll")> Private Shared Function _
'	WriteFile(ByVal hFile As Integer, ByVal Buffer As Byte(), _
'	ByVal nNumberOfBytesToWrite As Integer, ByRef lpNumberOfBytesWritten As Integer, _
'	ByVal lpOverlapped As Integer) As Integer
'	End Function

'	<DllImport("kernel32.dll")> Private Shared Function _
'	CloseHandle(ByVal hObject As Integer) As Integer
'	End Function



'#End Region

'#Region "Eigene Felder"

'	Private _MyComPort As String
'	Private _ComOpend As Boolean
'	Private _InputBuffer(65535) As Byte
'	Private _outfileHandler As Integer = 0
'	Private _numReadWrite As Integer
'	Private _BytesToRead As Integer
'	Private _DataReaded As Boolean
'	Private _t1 As System.Threading.Thread
'	Private _MyUtillities As New ThermoUtilities.ThermoUtilities
'	Private _ListenerMode As Boolean
'	Private _Closing As Boolean = False

'	Private _MyState As Integer
'	Private Const _IDLE As Integer = 0
'	Private Const _BUSY As Integer = 1


'#End Region

'#Region "Öffentliche Eigenschaften"

'	''' <summary>
'	''' Wieviele Bytes habe ich aktuell zum lesen 
'	''' </summary>
'	''' <value></value>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public ReadOnly Property BytesToRead() As Integer
'		Get
'			Return _BytesToRead
'		End Get
'	End Property

'	''' <summary>
'	''' Nur Lesen Zustand
'	''' </summary>
'	''' <value></value>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public ReadOnly Property ListenerMode() As Boolean
'		Get
'			Return _ListenerMode
'		End Get
'	End Property

'	''' <summary>
'	''' Daten angekommen
'	''' </summary>
'	''' <remarks></remarks>
'	Public Event DataReceived()

'	''' <summary>
'	''' Event das irgendetwas schiefgelaufen ist
'	''' </summary>
'	''' <remarks></remarks>
'	<Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As Exception)

'#End Region

'#Region "Private Methoden"

'	''' <summary>
'	''' Öffnen des Comport
'	''' </summary>
'	''' <param name="ComPort"></param>
'	''' <returns>0, wenn Fehler, 1 wenn ok</returns>
'	''' <remarks></remarks>
'	Private Function OpenComPort(ByVal ComPort As String) As Integer
'		Try
'			_MyComPort = ComPort
'			_outfileHandler = CreateFile(_MyComPort, &HC0000000, 0, 0, 3, 0, 0)

'			If _outfileHandler = -1 Then
'				_ComOpend = False
'				Return 0
'			Else
'				_ComOpend = True
'				_DataReaded = True
'				_Closing = False
'				_t1 = New Threading.Thread(AddressOf receiveLoop)
'				_t1.Start()
'				Return 1
'			End If
'		Catch ex As Exception
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'			End If
'			Return 0
'		End Try
'	End Function

'	''' <summary>
'	''' Schliessen des Comports
'	''' </summary>
'	''' <returns>0, wenn Fehler, 1 wenn ok</returns>
'	''' <remarks></remarks>
'	Private Function CloseComPort() As Integer
'		Try
'			If _ComOpend Then
'				_Closing = True
'				_t1.Abort()
'				CloseHandle(_outfileHandler)
'				Return 1
'			Else
'				Return 0
'			End If
'		Catch ex As Exception
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'				Return 0
'			Else
'				Return 1
'			End If
'		End Try
'	End Function

'	''' <summary>
'	''' Daten schreiben
'	''' </summary>
'	''' <param name="DataArray"></param>
'	''' <param name="Length"></param>
'	''' <returns>0, wenn Fehler, 1 wenn ok</returns>
'	''' <remarks></remarks>
'	Private Function SendData(ByVal DataArray() As Byte, ByVal Length As Integer) As Integer
'		Try
'			If (_MyState = _IDLE And Not _ListenerMode) Then
'				Dim retCode As Boolean
'				ReDim _InputBuffer(65535)
'				_BytesToRead = 0
'				_DataReaded = True
'				retCode = WriteFile(_outfileHandler, DataArray, Length, _numReadWrite, 0)
'				If retCode = True Then
'					_MyState = _BUSY
'					Return 1
'				Else
'					Return 0
'				End If
'			Else
'				Return 0
'			End If
'		Catch ex As Exception
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'			End If
'			Return 0
'		End Try
'	End Function

'	''' <summary>
'	''' Kontinuierlich lesen
'	''' </summary>
'	''' <remarks></remarks>
'	Private Sub receiveLoop()
'		Try

'			Dim inbuff(65535) As Byte
'			Dim Length As Integer
'			'---receive the message through the serial port
'			Dim retCode As Integer = ReadFile(_outfileHandler, _InputBuffer, _InputBuffer.Length, _numReadWrite, 0)

'			While True
'				If retCode = 0 Then	'Or stopThread
'					'---either error or stop is requested
'					Exit While
'				Else
'					If (_MyState = _BUSY Or _ListenerMode = True) And _DataReaded And Not _Closing Then	'nur wenn geschrieben, und aktuelle Daten abgeholt wurden wieder neu lesen
'						ReDim inbuff(65535)
'						retCode = ReadFile(_outfileHandler, inbuff, inbuff.Length, _numReadWrite, 0)
'						Length = _numReadWrite
'						If Length <> 0 Then
'							_BytesToRead = _BytesToRead + Length
'							Array.Copy(inbuff, 0, _InputBuffer, 0, Length)
'							_DataReaded = False
'							RaiseEvent DataReceived()
'						End If
'					End If
'				End If
'				System.Threading.Thread.Sleep(10)	'siehe Application.DoEvents
'			End While
'		Catch ex As Exception
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'			End If
'		End Try
'	End Sub

'#End Region

'#Region "Öffentliche Methoden"

'	''' <summary>
'	''' Konstruktor
'	''' </summary>
'	''' <param name="ListenerMode">Soll der serielle Port nur abgehört werden (GPS)</param>
'	''' <remarks></remarks>
'	Public Sub New(ByVal ListenerMode As Boolean)
'		_ListenerMode = ListenerMode
'	End Sub

'	''' <summary>
'	''' Thread anhalten
'	''' </summary>
'	''' <remarks></remarks>
'	Public Sub Dispose()
'		CloseComPort()
'	End Sub

'	''' <summary>
'	''' Port öffnen
'	''' </summary>
'	''' <param name="PortName"></param>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public Function Open(ByVal PortName As String) As Integer
'		Return OpenComPort(PortName)
'	End Function

'	''' <summary>
'	''' Port schliessen
'	''' </summary>
'	''' <remarks></remarks>
'	Public Function Close() As Integer
'		Return CloseComPort()
'	End Function

'	''' <summary>
'	''' Zeichen das alle Daten vom Client abgeholt worden und ich neu schreiben kann
'	''' </summary>
'	''' <remarks></remarks>
'	Public Sub ResetPortState()
'		_MyState = _IDLE
'	End Sub

'	''' <summary>
'	''' Daten auf die Leitung schreiben
'	''' </summary>
'	''' <param name="DataArray"></param>
'	''' <param name="Length"></param>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public Function Write(ByVal DataArray() As Byte, ByVal Length As Integer) As Integer
'		Return SendData(DataArray, Length)
'	End Function

'	''' <summary>
'	''' Daten auf die Leitung schreiben
'	''' </summary>
'	''' <param name="Data"></param>
'	''' <param name="Length"></param>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public Function Write(ByVal Data As String, ByVal Length As Integer) As Integer
'		Try
'			Dim DataArray() As Byte
'			DataArray = _MyUtillities.StringToByteArray(Data, 0, Length - 1)
'			Return SendData(DataArray, Length)
'		Catch ex As Exception
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'			End If
'		End Try
'	End Function

'	''' <summary>
'	''' Liets in ein ByteArray
'	''' </summary>
'	''' <param name="Buffer"></param>
'	''' <param name="Offset"></param>
'	''' <param name="Count"></param>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public Function Read(ByRef Buffer() As Byte, ByVal Offset As Integer, ByVal Count As Integer) As Integer
'		Try
'			Array.Copy(_InputBuffer, 0, Buffer, Offset, Count)
'			_DataReaded = True
'			_BytesToRead = 0
'			Return Count
'		Catch ex As Exception
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'			End If
'		End Try
'	End Function

'	''' <summary>
'	''' Liest einen String
'	''' </summary>
'	''' <returns></returns>
'	''' <remarks></remarks>
'	Public Function ReadExisting() As String
'		Dim RetString As String = ""
'		Try
'			RetString = _MyUtillities.ByteArrayToString(_InputBuffer, 0, _BytesToRead)
'			_DataReaded = True
'			_BytesToRead = 0
'			Return RetString
'		Catch ex As Exception
'			Return RetString
'			If Not ex.Message = "ThreadAbortException" Then	'ignorieren, weil hat trotzdem alles geklappt
'				RaiseEvent ErrorEvent(ex)
'			End If
'		End Try
'	End Function

'#End Region

'End Class
