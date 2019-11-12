#Region "Imports"

Imports ADONTEC.Comm
Imports ADONTEC.Comm.SuperCom
Imports System.Threading

#End Region

''' <summary>
''' Serial Class based on the ADONTEC SuperCom API
''' </summary>
''' <remarks></remarks>
Public Class SerialPort

#Region "Private Eigenschaften"

    Private _Com As Integer
    Private _MyUtillities As New ThermoUtilities.ThermoUtilities
    Private _CheckingThread As System.Threading.Thread
    Private _ReceivedBytesThreshold As Integer
    Private _BytesToRead As Integer
    Private _PortName As String
    Private _BaudRate As Integer
    Private _Parity As ADONTEC.Comm.Parity = ADONTEC.Comm.Parity.None
    Private _DataBits As Integer = 8
    Private _StopBits As ADONTEC.Comm.StopBits = ADONTEC.Comm.StopBits.One
    Private _Handshake As ADONTEC.Comm.Handshake = ADONTEC.Comm.Handshake.None
    Private _DtrEnable As Boolean = False
    Private _RtsEnable As Boolean = False
    Private _Disposed As Boolean
    Private _IsOpen As Boolean
    Private _InBuffer(0) As Byte
    Private _OutBuffer(0) As Byte
    Private _BytesToWrite As Integer
    Private _AsnycTransfer As Boolean

#End Region

#Region "Öffentliche Eigenschaften"

    Public Property PortName() As String
        Get
            Return _PortName
        End Get
        Set(ByVal value As String)
            _PortName = value
            Dim Digits As String = ""
            Dim i As Integer = _PortName.Length
            While (i > 0)
                i -= 1
                If Char.IsDigit(_PortName(i)) Then
                    Digits = PortName(i) + Digits
                Else
                    Exit While
                End If
            End While
            If Digits.Length > 0 Then
                _Com = Convert.ToInt32(Digits) - 1
            End If
        End Set
    End Property

    Public ReadOnly Property BytesToRead() As Integer
        Get
            _BytesToRead = ComBufCount(_Com, DIR_INC)  ' how many bytes are available in the COM input buffer
            Return _BytesToRead
        End Get
    End Property

    Public Property Baudrate() As Integer
        Get
            Return _BaudRate
        End Get
        Set(ByVal value As Integer)
            _BaudRate = value
        End Set
    End Property

    Public Property Parity() As System.IO.Ports.Parity
        Get
            Select Case _Parity

                Case ADONTEC.Comm.Parity.Even
                    Return IO.Ports.Parity.Even

                Case ADONTEC.Comm.Parity.Mark
                    Return IO.Ports.Parity.Mark

                Case ADONTEC.Comm.Parity.None
                    Return IO.Ports.Parity.None

                Case ADONTEC.Comm.Parity.Odd
                    Return IO.Ports.Parity.Odd

                Case ADONTEC.Comm.Parity.Space
                    Return IO.Ports.Parity.Space

            End Select
        End Get
        Set(ByVal value As System.IO.Ports.Parity)
            Select Case value

                Case IO.Ports.Parity.Even
                    _Parity = ADONTEC.Comm.Parity.Even

                Case IO.Ports.Parity.Mark
                    _Parity = ADONTEC.Comm.Parity.Mark

                Case IO.Ports.Parity.None
                    _Parity = ADONTEC.Comm.Parity.None

                Case IO.Ports.Parity.Odd
                    _Parity = ADONTEC.Comm.Parity.Odd

                Case IO.Ports.Parity.Space
                    _Parity = ADONTEC.Comm.Parity.Space

            End Select
        End Set
    End Property

    Public Property DataBits() As Integer
        Get
            Return _DataBits
        End Get
        Set(ByVal value As Integer)
            _DataBits = value
        End Set
    End Property

    Public Property StopBits() As System.IO.Ports.StopBits
        Get
            Select Case _StopBits

                Case ADONTEC.Comm.StopBits.None
                    Return IO.Ports.StopBits.None

                Case ADONTEC.Comm.StopBits.One
                    Return IO.Ports.StopBits.One

                Case ADONTEC.Comm.StopBits.OnePointFive
                    Return IO.Ports.StopBits.OnePointFive

                Case ADONTEC.Comm.StopBits.Two
                    Return IO.Ports.StopBits.Two

            End Select
        End Get
        Set(ByVal value As System.IO.Ports.StopBits)
            Select Case value

                Case IO.Ports.StopBits.None
                    _StopBits = ADONTEC.Comm.StopBits.None

                Case IO.Ports.StopBits.One
                    _StopBits = ADONTEC.Comm.StopBits.One

                Case IO.Ports.StopBits.OnePointFive
                    _StopBits = ADONTEC.Comm.StopBits.OnePointFive

                Case IO.Ports.StopBits.Two
                    _StopBits = ADONTEC.Comm.StopBits.Two

            End Select
        End Set
    End Property

    Public Property Handshake() As System.IO.Ports.Handshake
        Get
            Select Case _Handshake

                Case ADONTEC.Comm.Handshake.None
                    Return IO.Ports.Handshake.None

                Case ADONTEC.Comm.Handshake.RequestToSend
                    Return IO.Ports.Handshake.RequestToSend

                Case ADONTEC.Comm.Handshake.RequestToSendXOnXOff
                    Return IO.Ports.Handshake.RequestToSendXOnXOff

            End Select
        End Get
        Set(ByVal value As System.IO.Ports.Handshake)
            Select Case value

                Case IO.Ports.Handshake.None
                    _Handshake = ADONTEC.Comm.Handshake.None

                Case IO.Ports.Handshake.RequestToSend
                    _Handshake = ADONTEC.Comm.Handshake.RequestToSend

                Case IO.Ports.Handshake.RequestToSendXOnXOff
                    _Handshake = ADONTEC.Comm.Handshake.RequestToSendXOnXOff

                Case IO.Ports.Handshake.XOnXOff

            End Select

        End Set
    End Property

    Public Property DtrEnable() As Boolean
        Get
            Return _DtrEnable
        End Get
        Set(ByVal value As Boolean)
            _DtrEnable = value
        End Set
    End Property

    Public Property RtsEnable() As Boolean
        Get
            Return _RtsEnable
        End Get
        Set(ByVal value As Boolean)
            _RtsEnable = value
        End Set
    End Property

    Public Property ReceivedBytesThreshold() As Integer
        Get
            Return _ReceivedBytesThreshold
        End Get
        Set(ByVal value As Integer)
            _ReceivedBytesThreshold = value
        End Set
    End Property

    Public ReadOnly Property IsOpen() As Boolean
        Get
            Return _IsOpen
        End Get
    End Property

    Public ReadOnly Property BytesToWrite() As Integer
        Get
            _BytesToWrite = ComBufCount(_Com, DIR_OUT)
            Return _BytesToWrite
        End Get
    End Property

    Public Event DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs)

    Public Event ErrorReceived(ByVal sender As Object, ByVal e As Integer)

#End Region

#Region "Private Methoden"

    Private Sub CheckForNewData()
        Try
            Do
                Try
                    If _IsOpen Then
                        _BytesToRead = ComBufCount(_Com, DIR_INC)  ' how many
                        _BytesToWrite = ComBufCount(_Com, DIR_OUT)
                        If _BytesToRead > _ReceivedBytesThreshold Then
                            RaiseEvent DataReceived(Me, Nothing)
                        End If
                        If (RS_LineState(_Com) And PARITY_ERROR) <> 0 Then
                            RaiseEvent ErrorReceived(Me, PARITY_ERROR)
                        End If
                    End If
                    Thread.Sleep(10)
                Catch ex As Exception
                    'If RS_LineState(_Com) = BREAK_DETECT Then
                    '    MsgBox("BREAK Detect")
                    'End If
                    Trace.TraceError(ex.Message & " " & ex.StackTrace)
                End Try
            Loop Until _Disposed
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="AsnycTransfer"> = true, if transfer procedure is controlled by events </param>
    ''' <remarks></remarks>
    Public Sub New(ByVal AsnycTransfer As Boolean)
        Try
            _Disposed = False
            _IsOpen = False
            _AsnycTransfer = AsnycTransfer
            'Try
            '    If Not _CheckingThread Is Nothing Then
            '        _CheckingThread.Abort()
            '    End If
            'Catch ex As Exception
            'End Try
            '_CheckingThread = New Threading.Thread(AddressOf CheckForNewData)
            '_CheckingThread.Start()
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Dispose()
        Try
            _Disposed = True
            _IsOpen = False
            ComReset(_Com)  ' release comm port
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Open()
        Try
            If Not _CheckingThread Is Nothing Then
                _CheckingThread.Abort()
            End If
        Catch ex As Exception
        End Try
        'Jetzt kann ich davon ausgehen, dass der Thread beendet ist.

        Try
            ComInit(_Com) ' init port
            If ComResult(_Com) <> scOK Then
                Throw New Exception("Can not open Com port")
                Exit Sub
            End If
            ComSetState(_Com, _BaudRate, CType(_DataBits, Byte), CType(_StopBits, Byte), CType(_Parity, Byte), SIGNAL_NONE)
            If _DtrEnable Then
                ComDTROn(_Com)
            Else
                ComDTROff(_Com)
            End If
            If _RtsEnable Then
                ComRTSOn(_Com)
            Else
                ComRTSOff(_Com)
            End If
            'Zyklische Thread-Funktion starten, wenn Datenübertragung xventgesteuert sein soll
            If _AsnycTransfer Then
                _CheckingThread = New Threading.Thread(AddressOf CheckForNewData)
                _CheckingThread.Start()
            End If
            _IsOpen = True
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Close()

        Try
            If Not _CheckingThread Is Nothing Then
                _CheckingThread.Abort()
            End If
        Catch ex As Exception
        End Try
        'Jetzt kann ich davon ausgehen, dass der Thread beendet ist.

        'Jetzt SChnittstelle wirklich schließen.
        Try
            ComReset(_Com)  ' release comm port
            _IsOpen = False
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <param name="offset"></param>
    ''' <param name="count"></param>
    ''' <returns>Number of bytes read</returns>
    ''' <remarks></remarks>
    Public Function Read(ByRef buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        Try
            If (RS_LineState(_Com) And PARITY_ERROR) <> 0 Then
                RaiseEvent ErrorReceived(Me, PARITY_ERROR)  'Mitteilung an die darüberliegende Schicht, dass Paritätsfehler aufgetreten
            End If
            _BytesToRead = ComBufCount(_Com, DIR_INC)  ' how many
            ReDim _InBuffer(_BytesToRead - 1)
            ReDim buffer(_BytesToRead - 1)
            Dim ret As Integer
            ret = ComReadEx(_Com, _InBuffer, _BytesToRead)  ' read
            Array.Copy(_InBuffer, 0, buffer, offset, _BytesToRead)
            Return ret
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

    Public Function ReadExisting() As String
        Try
            Dim RetString As String = ""
            Dim DataRead As Integer

            If (RS_LineState(_Com) And PARITY_ERROR) <> 0 Then
                RaiseEvent ErrorReceived(Me, PARITY_ERROR)  'Mitteilung an die darüberliegende Schicht, dass Paritätsfehler aufgetreten
            End If

            _BytesToRead = ComBufCount(_Com, DIR_INC)  ' how many
            ReDim _InBuffer(_BytesToRead)  'Eingangspuffer so groß machen wie gebraucht
            DataRead = ComReadEx(_Com, _InBuffer, _BytesToRead)  ' read
            RetString = _MyUtillities.ByteArrayToString(_InBuffer, 0, DataRead - 1)
            Return RetString
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return ""
        End Try
    End Function

    Public Function ReadChar() As Integer
        Try
            Dim S As String
            S = ReadExisting()
            Return Asc(S(0))
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

    Public Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        Try
            ReDim _OutBuffer(count - 1)
            Array.Copy(buffer, offset, _OutBuffer, 0, count)
            ComWriteEx(_Com, _OutBuffer, _OutBuffer.Length) ' send
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub Write(ByVal text As String)
        Try
            Dim Buffer(text.Length - 1) As Byte ' alloc buffer
            Array.Copy(_MyUtillities.StringToByteArray(text, 0, text.Length - 1), 0, Buffer, 0, Buffer.Length)
            ReDim _OutBuffer(Buffer.Length - 1)
            Array.Copy(Buffer, 0, _OutBuffer, 0, Buffer.Length)
            ComWriteEx(_Com, _OutBuffer, _OutBuffer.Length) ' send
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub DiscardInBuffer()
        Try
            _BytesToRead = 0
            Array.Clear(_InBuffer, 0, _InBuffer.Length)
            ReDim _InBuffer(0)
            ComBufClear(_Com, DIR_INC)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Public Sub DiscardOutBuffer()
        Try
            _BytesToWrite = 0
            Array.Clear(_OutBuffer, 0, _OutBuffer.Length)
            ReDim _OutBuffer(0)
            ComBufClear(_Com, DIR_OUT)
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

End Class
