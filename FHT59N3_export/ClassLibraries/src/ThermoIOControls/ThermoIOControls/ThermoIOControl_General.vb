#Region "Imports"
Imports ThermoInterfaces
Imports System.Net.Sockets
Imports System.Threading
Imports System.IO.Ports
#End Region

''' <summary>
''' This class is a summary of the RS232 and the Ethernet classes and can handle a data transfer
''' with or without events
''' </summary>
''' <remarks></remarks>
Public Class ThermoIOControl_General

#Region "Private Eigenschaften"

    Private _Socket As System.Net.Sockets.Socket    'Socket
    Private _MyRS232_Win As System.IO.Ports.SerialPort 'serielle Schnittstelle
    Private _MyRS232_SuperCom As SerialPort 'serielle Schnittstelle via OCX ADCOM Supercom
    Private _MyRS232ParityError As Boolean = False
    Private _DataContainer As ThermoDataContainer_Interface 'Datencontainer, IsReceiveready
    Private _Protocol As ThermoProtocol_Interface    'Irgendeine Klasse die dieses Interface implementiert, egal welche!; IsReceiveReady
    Private _ReadBuffer(100) As Byte   'Puffer für die Leseop
    Private _WriteBuffer(1024) As Byte 'Puffer für die Schreibop
    Private _ReceiveBuffer(100) As Byte 'Gesamtpuffer, wird bei bedraf erweitert
    Private _ReceivedString As String   'empfangener string
    Private _TimeoutThread As Thread   'timeout verwalter
    Private _TimeStamp As DateTime
    Private _Timeout As Integer
    Private _read As Integer = 0
    Private _send As Integer = 0
    Private _length As Integer = 0
    Private _ArrayIndex As Integer = 0
    Private _MyInstanceAlive As Boolean 'False, wenn gerade versucht wird (in DIspose) die Instanz zu schließen,
    'dann darf nicht mehr auf die Threads (Tineout etc) zugegriffen werden. Ansonsten wird nicht sauber entladen.
    Private _ErrorCounter As Integer = 0
    Private _IsTimeout As Boolean = False
    Private _CheckParityError As Boolean = True
    Private _LockDataReceivedEvent As Boolean = False

    'Flags die die Events ersetzen
    Private _DisconnectedFlag As Boolean

    Private _MyState As Integer 'wo stehe ich?
    Public Enum MyStates
        IDLE = 101
        BUSY = 102
    End Enum

    Private _ComMode As Integer
    Public Enum ComMode
        CM_ARRAY = 1001
        CM_STRING = 1002
    End Enum

    Private _InterfaceMode As Integer
    Public Enum InterfaceMode
        IM_RS232 = 10001
        IM_Ethernet = 10002
    End Enum

    Private _TransferMode As Integer
    Public Enum TransferMode
        TM_Asynchron = 100001
        TM_Synchron = 100002
    End Enum

    Private _ComClass As ComClassMode
    Private Enum ComClassMode
        CCM_WIN = 1000001
        CCM_SUPERCOM = 1000002
    End Enum
#End Region

#Region "Öffentliche Eigenschaften"

    Public Enum ResultsOfDatatransfer
        TRANSFER_BUSY = 0
        TRANSFER_OK = 1

        TRANSFER_ERR_TIMEOUT = -1
        TRANSFER_ERR_CHECKSUM = -2
        TRANSFER_ERR_PARITY = -3
        TRANSFER_ERR_NAK = -4
        TRANSFER_ERR_SPECIALNAK = -5
        TRANSFER_ERR_EXCEPTION = -10
    End Enum

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
    ''' Serielle Schnittstelle Windows
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MyRS232() As System.IO.Ports.SerialPort
        Set(ByVal value As System.IO.Ports.SerialPort)
            _MyRS232_Win = value
        End Set
    End Property

    ''' <summary>
    ''' Serielle Schnittstelle SuperCom
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MyRS232_SuperCom() As SerialPort
        Set(ByVal value As SerialPort)
            _MyRS232_SuperCom = value
        End Set
    End Property

    Public Property CheckParityError() As Boolean
        Get
            Return _CheckParityError
        End Get
        Set(ByVal value As Boolean)
            _CheckParityError = value
        End Set
    End Property

    Public Property LockDataReceivedEvent() As Boolean
        Get
            Return _LockDataReceivedEvent
        End Get
        Set(ByVal value As Boolean)
            _LockDataReceivedEvent = value
        End Set
    End Property

    ''' <summary>
    ''' Sende und Empfange ich Byte Array´s(1) oder String´s(2)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MyComMode() As Integer
        Get
            Return _ComMode
        End Get
        Set(ByVal value As Integer)
            _ComMode = value
        End Set
    End Property

    ''' <summary>
    ''' Kommuniziere ich über Ethernet oder RS232
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MyInterfaceMode() As Integer
        Get
            Return _InterfaceMode
        End Get
    End Property

    ''' <summary>
    ''' Asynchroner Transfer (mit Events) oder Synchroner Transfer (Polling)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MyTransferMode() As Integer
        Get
            Return _TransferMode
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
            Return _Timeout
        End Get
        Set(ByVal value As Integer)
            _Timeout = value
        End Set
    End Property

    ''' <summary>
    ''' Gerät ist nicht mehr verbunden
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DisconnectedFlag() As Boolean
        Get
            Return _DisconnectedFlag
        End Get
    End Property

    ''' <summary>
    ''' Event das Daten vom seriellen Port empfangen wurden (Byte Array)
    ''' </summary>
    ''' <remarks></remarks>
    Public Event DataReceivedEventAr(ByVal ReceivedBytes As Integer)

    ''' <summary>
    ''' Event das Daten vom seriellen Port empfangen wurden (String)
    ''' </summary>
    ''' <remarks></remarks>
    Public Event DataReceivedEventStr(ByVal ReceivedChars As Integer)

    ''' <summary>
    ''' Event dass Fehler beim Empfang von Daten aufgetreten (String und Array)
    ''' </summary>
    ''' <remarks>Bitte das ENUM ResultsOfDatatransfer verwenden</remarks>
    Public Event DataErrorEvent(ByVal ErrNumber As Integer)

    ''' <summary>
    ''' Verbindung unterbrochen
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Disconnected()

#End Region

#Region "Private Methoden"

#Region "General"

    ''' <summary>
    ''' Timeout für Empfang von Daten der Schnittstelle überwachen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TimTimeout_TimerEvent()
        Dim dt As DateTime
        Dim DataAvailable As Boolean = False
        Do
            Try

                If _MyState = MyStates.BUSY Then    'solange busy (=Daten wurden angefordert), wird TimeOut geprüft.
                    dt = Now
                    Dim ts As TimeSpan = dt.Subtract(_TimeStamp)
                    If ts.TotalMilliseconds > _Timeout Then
                        _MyState = MyStates.IDLE
                        _IsTimeout = True
                        RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT)   'TimeOut aufgetreten, => Event feuern
                    End If

                End If

                Thread.Sleep(10)

            Catch ex As Exception
                _MyState = MyStates.IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
            End Try

        Loop While _MyInstanceAlive = True
    End Sub

#End Region

#Region "Ethernet_Asynchron"

    ''' <summary>
    ''' Asynchron schreiben
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <remarks></remarks>
    Private Sub StartWriteAr(ByVal Data() As Byte)
        Try
            If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                Array.Copy(Data, _WriteBuffer, Data.Length)
                Try
                    _Socket.BeginSend(_WriteBuffer, 0, _WriteBuffer.Length, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf WriteCallback), _Socket)
                Catch ex As Exception
                    _MyState = MyStates.IDLE
                    RaiseEvent Disconnected()
                End Try
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
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
            If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _WriteBuffer = System.Text.Encoding.ASCII.GetBytes(Data)
                Try
                    _Socket.BeginSend(_WriteBuffer, 0, _WriteBuffer.Length, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf WriteCallback), _Socket)
                Catch ex As Exception
                    _MyState = MyStates.IDLE
                    RaiseEvent Disconnected()
                End Try
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
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
            If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
                Try
                    _send = _Socket.EndSend(ar)
                Catch ex As Exception
                    _MyState = MyStates.IDLE
                    RaiseEvent Disconnected()
                End Try
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' asynchron lesen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartRead()
        Try
            If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
                'Die nächste Zeile ist auskommentiert: Diese bringt beim anzeigen der Werte in einem Formular mit BeginInvoke einen Fehler
                'Beim Vorgang zum Rückgängigmachen wurde ein Kontext gefunden, der sich vom Kontext des entsprechenden Set-Vorgangs unterschied. Möglicherweise war der Kontext für den Thread \"Set\" und wurde nicht zurückgesetzt (rückgängig gemacht).
                'System.Windows.Forms.Application.DoEvents() 'WICHTIG. Sonst kann es bei Verbindungsverlust durch dauerndes StartRead zu einem Hänger kommen
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                Try
                    _Socket.BeginReceive(_ReadBuffer, 0, _ReadBuffer.Length, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ReadCallback), _Socket)
                Catch ex As Exception
                    _MyState = MyStates.IDLE
                    RaiseEvent Disconnected()
                End Try
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' lesen beendet
    ''' </summary>
    ''' <param name="ar"></param>
    ''' <remarks></remarks>
    Private Sub ReadCallback(ByVal ar As System.IAsyncResult)
        If (_MyInstanceAlive) And (Not _IsTimeout) And (Not _LockDataReceivedEvent) Then
            Try

                Select Case _ComMode

                    Case ComMode.CM_ARRAY

                        Try
                            _read = _Socket.EndReceive(ar)
                        Catch ex As Exception
                            Trace.TraceError(ex.Message & " " & ex.StackTrace)
                        End Try

                        _length = Array.IndexOf(_ReadBuffer, Nothing)
                        If _length = -1 Then _length = _ReadBuffer.Length
                        If _ArrayIndex + _length >= _ReceiveBuffer.Length Then   'wenn der bisherige Speicherplatz nicht reicht
                            ReDim Preserve _ReceiveBuffer(_ReceiveBuffer.Length + _length)
                        End If
                        Array.ConstrainedCopy(_ReadBuffer, 0, _ReceiveBuffer, _ArrayIndex, _length)
                        _ArrayIndex = _ArrayIndex + _length
                        Try 'wenn der Socket schon closed ist, kann es trotzdem sein das wir noch hierher kommen!
                            If _Socket.Available > 0 And _length <> 0 Then  'wurde überhaupt was gelesen und gibt es noch mehr???
                                StartRead()
                                Exit Sub
                            End If
                        Catch ex As Exception
                            _MyState = MyStates.IDLE
                        End Try

                        Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex}
                        Dim RetRecvReady As Integer = 0
                        RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                        If RetRecvReady = 1 Then 'Empfang ok???
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataReceivedEventAr(_ArrayIndex)        'Alles ok für dieses Gerät

                        ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM)
                        ElseIf RetRecvReady = -4 Then 'NAK Received
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_NAK)
                        ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK)
                        End If

                    Case ComMode.CM_STRING

                        Try
                            _read = _Socket.EndReceive(ar)
                        Catch ex As System.Net.Sockets.SocketException
                            Trace.TraceError(ex.Message & " " & ex.StackTrace)
                        End Try
                        _length = Array.IndexOf(_ReadBuffer, Nothing)
                        If _length = -1 Then _length = _ReadBuffer.Length
                        _ReceivedString = _ReceivedString & System.Text.Encoding.ASCII.GetString(_ReadBuffer, 0, _length)
                        If _ReceivedString = "" Then 'WICHTIG. Sonst kann es bei Verbindungsverlust durch dauerndes StartRead zu einem Hänger kommen
                            _ErrorCounter = _ErrorCounter + 1
                            If _ErrorCounter >= 20 Then 'nach 50 sinnlosen Aktionen auf der Leitung fliegt er raus -> wenn ein Client einfach stirbt ohne den socket korrekt zu schliessen
                                RaiseEvent Disconnected()
                            End If
                        Else
                            _ErrorCounter = _ErrorCounter - 1
                        End If
                        If _ErrorCounter < 0 Then _ErrorCounter = 0
                        Try 'wenn der Socket schon closed ist, kann es trotzdem sein das wir noch hierher kommen!
                            If _Socket.Available > 0 And _length <> 0 Then  'wurde überhaupt was gelesen und gibt es noch mehr???
                                StartRead()
                                Exit Sub
                            End If
                        Catch ex As Exception
                            _MyState = MyStates.IDLE
                        End Try

                        Dim Arguments() As Object = {_ReceivedString}
                        Dim RetRecvReady As Integer = 0
                        RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                        If RetRecvReady = 1 Then 'Empfang ok???
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            Dim RetVal As Integer = _ReceivedString.Length
                            _ReceivedString = ""
                            RaiseEvent DataReceivedEventStr(RetVal)    'Alles ok für dieses Gerät

                        ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            _ReceivedString = ""
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM)
                        ElseIf RetRecvReady = -4 Then 'NAK Received
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            _ReceivedString = ""
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_NAK)
                        ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            _ReceivedString = ""
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK)
                        End If

                End Select

                StartRead() 'wieder mit lesen beginnen

            Catch ex As Exception
                _MyState = MyStates.IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
            End Try
        End If
    End Sub

#End Region

#Region "Ethernet_Synchron"

    ''' <summary>
    ''' Synchron schreiben
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <remarks></remarks>
    Private Sub WriteAr(ByVal Data() As Byte)
        Try
            If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                Array.Copy(Data, _WriteBuffer, Data.Length)
                Try
                    If (Not _Socket Is Nothing) Then
                        _Socket.Send(_WriteBuffer)
                    Else
                        Trace.TraceError("_Socket in WriteAr is null")
                    End If
                Catch ex As Exception
                    _MyState = MyStates.IDLE
                    _DisconnectedFlag = True
                    Trace.TraceError(ex.Message & " " & ex.StackTrace)
                End Try
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
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
            If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
                ReDim _WriteBuffer(Data.Length)
                Array.Clear(_WriteBuffer, 0, _WriteBuffer.Length)
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _WriteBuffer = System.Text.Encoding.ASCII.GetBytes(Data)
                Try
                    If (Not _Socket Is Nothing) Then
                        _Socket.Send(_WriteBuffer)
                    Else
                        Trace.TraceError("_Socket in WriteStr is null")
                    End If
                Catch ex As Exception
                    _MyState = MyStates.IDLE
                    _DisconnectedFlag = True
                    Trace.TraceError(ex.Message & " " & ex.StackTrace)
                End Try
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "RS232_Asynchron"

    ''' <summary>
    ''' Event von der RS232 Schnittstelle
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs)
        Dim DataRead As Integer
        Dim BytesToRead As Integer
        If (_MyInstanceAlive) And (Not _IsTimeout) And (Not _LockDataReceivedEvent) Then
            Try

                Select Case _ComMode

                    Case ComMode.CM_ARRAY

                        Select Case _ComClass
                            Case ComClassMode.CCM_WIN
                                BytesToRead = _MyRS232_Win.BytesToRead
                            Case ComClassMode.CCM_SUPERCOM
                                BytesToRead = _MyRS232_SuperCom.BytesToRead
                        End Select
                        ReDim _ReadBuffer(BytesToRead) 'Eingangspuffer so groß machen wie gebraucht
                        Select Case _ComClass
                            Case ComClassMode.CCM_WIN
                                DataRead = _MyRS232_Win.Read(_ReadBuffer, 0, BytesToRead)
                            Case ComClassMode.CCM_SUPERCOM
                                DataRead = _MyRS232_SuperCom.Read(_ReadBuffer, 0, BytesToRead)
                        End Select
                        If _ArrayIndex + DataRead >= _ReceiveBuffer.Length Then   'wenn der bisherige Speicherplatz nicht reicht
                            ReDim Preserve _ReceiveBuffer(_ReceiveBuffer.Length + DataRead)
                        End If
                        Array.ConstrainedCopy(_ReadBuffer, 0, _ReceiveBuffer, _ArrayIndex, DataRead)
                        _ArrayIndex = _ArrayIndex + DataRead

                        If _MyRS232ParityError Then
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_PARITY)
                        End If

                        'Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex - 1} 'Anzahl Bytes uebergeben, diese = Index - 1
                        Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex}
                        Dim RetRecvReady As Integer = 0
                        RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                        If RetRecvReady = 1 Then 'Empfang ok???
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataReceivedEventAr(_ArrayIndex)        'Alles ok für dieses Gerät
                        ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM)
                        ElseIf RetRecvReady = -4 Then 'NAK Received
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_NAK)
                        ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK)
                        End If

                    Case ComMode.CM_STRING

                        Select Case _ComClass
                            Case ComClassMode.CCM_WIN
                                _ReceivedString += _MyRS232_Win.ReadExisting
                            Case ComClassMode.CCM_SUPERCOM
                                _ReceivedString += _MyRS232_SuperCom.ReadExisting
                        End Select

                        If _MyRS232ParityError Then
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_PARITY)
                        End If

                        Dim Arguments() As Object = {_ReceivedString}
                        Dim RetRecvReady As Integer = 0
                        RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                        If RetRecvReady = 1 Then 'Empfang ok???
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            Dim RetVal As Integer = _ReceivedString.Length
                            _ReceivedString = ""
                            RaiseEvent DataReceivedEventStr(RetVal)    'Alles ok für dieses Gerät
                        ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            _ReceivedString = ""
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM)
                        ElseIf RetRecvReady = -4 Then 'NAK Received
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            _ReceivedString = ""
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_NAK)
                        ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                            _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                            _MyState = MyStates.IDLE
                            _ReceivedString = ""
                            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK)
                        End If

                End Select

            Catch ex As Exception
                _MyState = MyStates.IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Check for Errors, especially Parity Errors
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RS232ErrorReceived_Win(ByVal sender As Object, ByVal e As System.IO.Ports.SerialErrorReceivedEventArgs)
        If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
            Try
                Select Case e.EventType

                    Case SerialError.RXParity
                        If _CheckParityError Then
                            _MyRS232ParityError = True
                        End If

                End Select
            Catch ex As Exception
                _MyState = MyStates.IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Check for Errors, especially Parity Errors
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RS232ErrorReceived_SuperCom(ByVal sender As Object, ByVal e As Integer)
        If _MyInstanceAlive Then    'nur wenn Instanz nicht gerade geschlossen wurde, neuen Transfer anstoßen
            Try
                Select Case e

                    Case ADONTEC.Comm.SuperCom.PARITY_ERROR
                        If _CheckParityError Then
                            _MyRS232ParityError = True
                        End If

                End Select
            Catch ex As Exception
                _MyState = MyStates.IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
            End Try
        End If
    End Sub

#End Region

#Region "RS232_AND_TCP_Synchron"

    ''' <summary>
    ''' Timeout überwachen
    ''' </summary>
    ''' <returns>
    ''' 0 = no timeout
    ''' 1 = timeout
    ''' 2 = timeout but new data available
    ''' </returns>
    ''' <remarks>_TimeStamp wird als Zeitreferenz genommen</remarks>
    Private Function IsTimeOut() As Boolean
        Dim dt As DateTime
        Dim ReturnValue As Boolean = False

        'Diese Funktion wird nur bei synchroner Übertragung aufgerufen.
        Try
            'TimeStamp wurde beim Senden des Befehls gesetzt (in Funktion DoTransferString bzw. DoTransferArray)
            dt = Now
            Dim ts As TimeSpan = dt.Subtract(_TimeStamp)
            If (ts.TotalMilliseconds > _Timeout) Then
                ReturnValue = True  'wenn TimeOut, dann liefer die Funktion True zurück
            End If
            Return ReturnValue

        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return True 'Exception wird wie TimeOut behandelt
        End Try
    End Function

#End Region

#End Region

#Region "Öffentliche Methoden"

#Region "Construction"

    ''' <summary>
    ''' Konstruktor TCP/IP
    ''' </summary>
    ''' <param name="Eth">Handle für einen geöffneten! Socket</param>
    ''' <param name="Timeout">Timeout für die Abfragen</param>
    ''' <param name="Protocol">Gerätelogik die IsReceiveReady implementiert</param>
    ''' <remarks></remarks>
    Sub New(ByVal Eth As Socket, ByVal Timeout As Integer, ByVal Protocol As ThermoProtocol_Interface, ByVal DataContainer As ThermoDataContainer_Interface, ByVal AsnycTransfer As Boolean)
        Try
            _InterfaceMode = InterfaceMode.IM_Ethernet
            _MyInstanceAlive = True
            _Socket = Eth
            _Timeout = Timeout
            _Protocol = Protocol
            _DataContainer = DataContainer
            _MyState = MyStates.IDLE
            _TimeStamp = Now

            If AsnycTransfer Then
                _TransferMode = TransferMode.TM_Asynchron
                _TimeoutThread = New Thread(AddressOf TimTimeout_TimerEvent)
                _TimeoutThread.Name = "monitor for asynch. comm TCP"
                _TimeoutThread.Start()
                StartRead()
            Else
                _TransferMode = TransferMode.TM_Synchron
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Konstruktor seriell via .NET
    ''' </summary>
    ''' <param name="SP">Handle für einen Seriellen Port (aus dem .NET Framework</param>
    ''' <param name="Timeout">Timeout für die Abfragen</param>
    ''' <param name="Protocol">Gerätelogik die IsReceiveReady implementiert</param> 
    ''' <remarks></remarks>
    Sub New(ByVal SP As System.IO.Ports.SerialPort, ByVal Timeout As Integer, ByVal Protocol As ThermoProtocol_Interface, ByVal DataContainer As ThermoDataContainer_Interface, ByVal AsnycTransfer As Boolean)
        Try
            _InterfaceMode = InterfaceMode.IM_RS232
            _ComClass = ComClassMode.CCM_WIN
            _MyInstanceAlive = True
            _MyRS232_Win = SP
            _Timeout = Timeout
            _Protocol = Protocol
            _DataContainer = DataContainer
            _MyState = MyStates.IDLE
            _TimeStamp = Now
            If AsnycTransfer Then
                _TransferMode = TransferMode.TM_Asynchron
                _MyRS232_Win.ReceivedBytesThreshold = 1
                _MyRS232ParityError = False
                AddHandler _MyRS232_Win.DataReceived, AddressOf DataReceived
                _TimeoutThread = New Thread(AddressOf TimTimeout_TimerEvent)
                _TimeoutThread.Name = "monitor for asynch. comm serial"
                _TimeoutThread.Start()
            Else
                _TransferMode = TransferMode.TM_Synchron
            End If
            AddHandler _MyRS232_Win.ErrorReceived, AddressOf RS232ErrorReceived_Win
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Konstruktor für RS232 via ADONTEC
    ''' </summary>
    ''' <param name="SP">Handle für einen Seriellen Port (aus ThermoIOControls</param>
    ''' <param name="Timeout">Timeout für die Abfragen</param>
    ''' <param name="Protocol">Gerätelogik die IsReceiveReady implementiert</param> 
    ''' <remarks></remarks>
    Sub New(ByVal SP As SerialPort, ByVal Timeout As Integer, ByVal Protocol As ThermoProtocol_Interface, ByVal DataContainer As ThermoDataContainer_Interface, ByVal AsnycTransfer As Boolean)
        Try
            _InterfaceMode = InterfaceMode.IM_RS232
            _ComClass = ComClassMode.CCM_SUPERCOM
            _MyInstanceAlive = True
            _MyRS232_SuperCom = SP
            _Timeout = Timeout
            _Protocol = Protocol
            _DataContainer = DataContainer
            _MyState = MyStates.IDLE
            _TimeStamp = Now
            If AsnycTransfer Then
                _TransferMode = TransferMode.TM_Asynchron
                _MyRS232_SuperCom.ReceivedBytesThreshold = 1
                _MyRS232ParityError = False
                AddHandler _MyRS232_SuperCom.DataReceived, AddressOf DataReceived
                _TimeoutThread = New Thread(AddressOf TimTimeout_TimerEvent)
                _TimeoutThread.Name = "monitor for asynch. comm serial"
                _TimeoutThread.Start()
            Else
                _TransferMode = TransferMode.TM_Synchron
            End If
            AddHandler _MyRS232_SuperCom.ErrorReceived, AddressOf RS232ErrorReceived_SuperCom
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' alles zumachen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose()
        Try
            _MyInstanceAlive = False    'Setzen, damit andere Threads blockiert werden können, weil Instanz gerade geschlossen wird

            Try
                If Not _TimeoutThread Is Nothing Then
                    _TimeoutThread.Abort()
                End If
            Catch ex As Exception
            End Try
            If Not _MyRS232_Win Is Nothing Then
                _MyRS232_Win.Dispose()
            End If
            If Not _MyRS232_SuperCom Is Nothing Then
                _MyRS232_SuperCom.Dispose()
            End If
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Transfer"

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">Bytestream der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Function DoTransferAr(ByVal Command() As Byte, ByVal Length As Integer, ByVal SendFlag As Integer, ByVal SetBusy As Boolean) As Boolean
        Try
            Dim RetVal As Boolean = False
            If _MyState = MyStates.IDLE And (_MyInstanceAlive = True) Then
                _IsTimeout = False
                _TimeStamp = Now
                If SetBusy Then _MyState = MyStates.BUSY
                _ComMode = ComMode.CM_ARRAY
                ReDim _ReceiveBuffer(1024)  '
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _ArrayIndex = 0
                Select Case _InterfaceMode

                    Case InterfaceMode.IM_Ethernet

                        If SendFlag = 1 Then
                            If _TransferMode = TransferMode.TM_Asynchron Then
                                StartWriteAr(Command)
                            ElseIf _TransferMode = TransferMode.TM_Synchron Then
                                WriteAr(Command)
                            End If
                        End If

                    Case InterfaceMode.IM_RS232

                        Select Case _ComClass

                            Case ComClassMode.CCM_WIN
                                _MyRS232_Win.DiscardOutBuffer() 'Löscht Output Buffer
                                _MyRS232_Win.DiscardInBuffer()  'Löscht Input Buffer
                                _MyRS232ParityError = False
                                If SendFlag = 1 Then
                                    If _TransferMode = TransferMode.TM_Asynchron Then
                                        _MyRS232_Win.Write(Command, 0, Length)          'Sendet Length Bytes beginnend ab Position 0 im Feld SendArray
                                    ElseIf _TransferMode = TransferMode.TM_Synchron Then
                                        _MyRS232_Win.Write(Command, 0, Length)          'Sendet Length Bytes beginnend ab Position 0 im Feld SendArray
                                    End If
                                End If

                            Case ComClassMode.CCM_SUPERCOM
                                _MyRS232_SuperCom.DiscardOutBuffer() 'Löscht Output Buffer
                                _MyRS232_SuperCom.DiscardInBuffer()  'Löscht Input Buffer
                                _MyRS232ParityError = False
                                If SendFlag = 1 Then
                                    If _TransferMode = TransferMode.TM_Asynchron Then
                                        _MyRS232_SuperCom.Write(Command, 0, Length)          'Sendet Length Bytes beginnend ab Position 0 im Feld SendArray
                                    ElseIf _TransferMode = TransferMode.TM_Synchron Then
                                        _MyRS232_SuperCom.Write(Command, 0, Length)          'Sendet Length Bytes beginnend ab Position 0 im Feld SendArray
                                    End If
                                End If

                        End Select

                End Select
                RetVal = True
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
        End Try
    End Function

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">String der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden (0/1)</param> 
    ''' <remarks></remarks>
    Public Function DoTransferStr(ByVal Command As String, ByVal Length As Integer, ByVal SendFlag As Integer, ByVal SetBusy As Boolean) As Boolean
        Try
            Dim RetVal As Boolean = False
            If _MyState = MyStates.IDLE And (_MyInstanceAlive = True) Then
                _IsTimeout = False
                _TimeStamp = Now
                If SetBusy Then _MyState = MyStates.BUSY
                _ComMode = ComMode.CM_STRING
                Array.Clear(_ReadBuffer, 0, _ReadBuffer.Length)
                _ReceivedString = ""
                Select Case _InterfaceMode

                    Case InterfaceMode.IM_Ethernet

                        If SendFlag = 1 Then
                            _TimeStamp = Now
                            If _TransferMode = TransferMode.TM_Asynchron Then
                                StartWriteStr(Command)
                            ElseIf _TransferMode = TransferMode.TM_Synchron Then
                                WriteStr(Command)
                            End If
                        End If

                    Case InterfaceMode.IM_RS232

                        Select Case _ComClass

                            Case ComClassMode.CCM_WIN
                                _MyRS232_Win.DiscardOutBuffer() 'Löscht Output Buffer
                                _MyRS232_Win.DiscardInBuffer()  'Löscht Input Buffer
                                _MyRS232ParityError = False
                                If SendFlag = 1 Then
                                    If _TransferMode = TransferMode.TM_Asynchron Then
                                        _MyRS232_Win.Write(Command)
                                    ElseIf _TransferMode = TransferMode.TM_Synchron Then
                                        _MyRS232_Win.Write(Command)
                                    End If
                                End If

                            Case ComClassMode.CCM_SUPERCOM
                                _MyRS232_SuperCom.DiscardOutBuffer() 'Löscht Output Buffer
                                _MyRS232_SuperCom.DiscardInBuffer()  'Löscht Input Buffer
                                _MyRS232ParityError = False
                                If SendFlag = 1 Then
                                    If _TransferMode = TransferMode.TM_Asynchron Then
                                        _MyRS232_SuperCom.Write(Command)
                                    ElseIf _TransferMode = TransferMode.TM_Synchron Then
                                        _MyRS232_SuperCom.Write(Command)
                                    End If
                                End If

                        End Select

                End Select
                RetVal = True
            End If
            Return RetVal
        Catch ex As Exception
            _MyState = MyStates.IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent DataErrorEvent(ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION)
        End Try
    End Function

#End Region

#Region "Synchrones lesen"

    ''' <summary>
    ''' Read the data synchron
    ''' </summary>
    ''' <returns>
    ''' >0 = Number of received Bytes
    ''' 0 = Busy
    ''' -1 = TimeOut
    ''' -2 = Checksum error
    ''' -3 = parity error
    ''' -4 = NAK received
    ''' -5 = special nak received
    ''' -10 = Exception error occured
    ''' </returns>
    ''' <remarks>please use the ENUM ResultsOfDatatransfer</remarks>
    Public Function ReadDataSynchron(ByRef ReceivedData As Object) As Integer
        Dim DataRead As Integer
        Dim BytesToRead As Integer
        If _MyInstanceAlive Then
            Try

                Select Case _InterfaceMode
                    Case InterfaceMode.IM_Ethernet  'datentransfer via Ethernet (Network)

                        If (_Socket Is Nothing) Then
                            Return ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION
                        End If

                        If (_Socket.Connected = False) Then
                            Trace.TraceError("_Socket is NOT connected in ReadDataSynchron()")
                            Return ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION
                        End If

                        Dim Available As Integer = _Socket.Available
                        Dim numberOfBytesReceived As Integer = 0

                        If Available > 0 Then   '???

                            Try
                                numberOfBytesReceived = _Socket.Receive(_ReadBuffer)
                            Catch ex As System.Net.Sockets.SocketException
                                numberOfBytesReceived = 0
                                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                            End Try

                            Select Case _ComMode

                                Case ComMode.CM_ARRAY   'binäre Übertagung (z.B. FHT681)
                                    _length = Array.IndexOf(_ReadBuffer, Nothing)   'anzahl der eben gelesenen Bytes

                                    If _length = -1 Then _length = _ReadBuffer.Length '???
                                    If _ArrayIndex + _length >= _ReceiveBuffer.Length Then   'wenn der bisherige Speicherplatz nicht reicht
                                        ReDim Preserve _ReceiveBuffer(_ReceiveBuffer.Length + _length)
                                    End If

                                    Array.ConstrainedCopy(_ReadBuffer, 0, _ReceiveBuffer, _ArrayIndex, _length) '??
                                    _ArrayIndex = _ArrayIndex + _length

                                    Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex}
                                    Dim RetRecvReady As Integer = 0
                                    'Protokollframe überprüfen (Checksumme, etc)
                                    RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                                    If RetRecvReady = 1 Then 'Empfang mit allen Prüfungen ok???
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return _ArrayIndex
                                    ElseIf RetRecvReady = -2 Then 'Checksummenfehler bei Empfang
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                                    ElseIf RetRecvReady = -4 Then 'NAK Received
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_NAK
                                    ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK
                                    End If

                                Case ComMode.CM_STRING
                                    '$bdr: temporary disable null end recognition
                                    '_length = Array.IndexOf(_ReadBuffer, Nothing)
                                    'If _length = -1 Then _length = _ReadBuffer.Length
                                    _length = Available

                                    If (_length > numberOfBytesReceived) Then
                                        Return 0
                                    End If

                                    _ReceivedString = _ReceivedString & System.Text.Encoding.ASCII.GetString(_ReadBuffer, 0, _length)
                                    If _ReceivedString = "" Then 'WICHTIG. Sonst kann es bei Verbindungsverlust durch dauerndes StartRead zu einem Hänger kommen
                                        _ErrorCounter = _ErrorCounter + 1
                                        If _ErrorCounter >= 20 Then 'nach 20 sinnlosen Aktionen auf der Leitung fliegt er raus -> wenn ein Client einfach stirbt ohne den socket korrekt zu schliessen
                                            _DisconnectedFlag = True
                                        End If
                                    Else
                                        _ErrorCounter = _ErrorCounter - 1
                                    End If
                                    If _ErrorCounter < 0 Then _ErrorCounter = 0

                                    Dim Arguments() As Object = {_ReceivedString}
                                    Dim RetRecvReady As Integer = 0
                                    RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                                    If RetRecvReady = 1 Then 'Empfang ok???
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        Dim RetVal As Integer = _ReceivedString.Length
                                        _ReceivedString = ""
                                        Return RetVal
                                    ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        _ReceivedString = ""
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                                    ElseIf RetRecvReady = -4 Then 'NAK Received
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        _ReceivedString = ""
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_NAK
                                    ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        _ReceivedString = ""
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK
                                    End If

                            End Select

                        End If

                    Case InterfaceMode.IM_RS232

                        Select Case _ComClass

                            Case ComClassMode.CCM_WIN
                                BytesToRead = _MyRS232_Win.BytesToRead

                            Case ComClassMode.CCM_SUPERCOM
                                BytesToRead = _MyRS232_SuperCom.BytesToRead

                        End Select

                        If BytesToRead > 0 Then

                            Select Case _ComMode

                                Case ComMode.CM_ARRAY

                                    ReDim _ReadBuffer(BytesToRead) 'Eingangspuffer so groß machen wie gebraucht
                                    Select Case _ComClass
                                        Case ComClassMode.CCM_WIN
                                            DataRead = _MyRS232_Win.Read(_ReadBuffer, 0, BytesToRead)
                                        Case ComClassMode.CCM_SUPERCOM
                                            DataRead = _MyRS232_SuperCom.Read(_ReadBuffer, 0, BytesToRead)
                                    End Select
                                    If _ArrayIndex + DataRead >= _ReceiveBuffer.Length Then   'wenn der bisherige Speicherplatz nicht reicht
                                        ReDim Preserve _ReceiveBuffer(_ReceiveBuffer.Length + DataRead)
                                    End If
                                    Array.ConstrainedCopy(_ReadBuffer, 0, _ReceiveBuffer, _ArrayIndex, DataRead)
                                    _ArrayIndex = _ArrayIndex + DataRead

                                    'Prüfung der Parität wird 
                                    If _MyRS232ParityError Then
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        Return -3
                                    End If

                                    Dim Arguments() As Object = {_ReceiveBuffer, _ArrayIndex}
                                    Dim RetRecvReady As Integer = 0
                                    RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                                    If RetRecvReady = 1 Then 'Empfang ok???
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return _ArrayIndex
                                    ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                                    ElseIf RetRecvReady = -4 Then 'NAK Received
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_NAK
                                    ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceiveBuffer
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK
                                    End If

                                Case ComMode.CM_STRING

                                    Select Case _ComClass
                                        Case ComClassMode.CCM_WIN
                                            _ReceivedString += _MyRS232_Win.ReadExisting
                                        Case ComClassMode.CCM_SUPERCOM
                                            _ReceivedString += _MyRS232_SuperCom.ReadExisting
                                    End Select

                                    If _MyRS232ParityError Then
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        Return -3
                                    End If

                                    Dim Arguments() As Object = {_ReceivedString}
                                    Dim RetRecvReady As Integer = 0
                                    RetRecvReady = _Protocol.IsReceiveReady(_DataContainer, Arguments)
                                    If RetRecvReady = 1 Then 'Empfang ok???
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        Dim RetVal As Integer = _ReceivedString.Length
                                        _ReceivedString = ""
                                        Return RetVal
                                    ElseIf RetRecvReady = -2 Then 'Checksummenfehler
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        _ReceivedString = ""
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_CHECKSUM
                                    ElseIf RetRecvReady = -4 Then 'NAK Received
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        _ReceivedString = ""
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_NAK
                                    ElseIf RetRecvReady = -5 Then 'Special NAK Received (eg. for FHT681)
                                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                                        _MyState = MyStates.IDLE
                                        ReceivedData = _ReceivedString
                                        _ReceivedString = ""
                                        Return ResultsOfDatatransfer.TRANSFER_ERR_SPECIALNAK
                                    End If

                            End Select

                        End If

                End Select

                If IsTimeOut() Then
                    _MyState = MyStates.IDLE
                    Trace.TraceError("timeout in ReadDataSynchron()")
                    Return ResultsOfDatatransfer.TRANSFER_ERR_TIMEOUT
                Else
                    Return 0
                End If

            Catch ex As Exception
                _MyState = MyStates.IDLE
                Trace.TraceError(ex.Message & " " & ex.StackTrace)
                Return ResultsOfDatatransfer.TRANSFER_ERR_EXCEPTION
            End Try
        End If
    End Function

#End Region

#End Region

End Class
