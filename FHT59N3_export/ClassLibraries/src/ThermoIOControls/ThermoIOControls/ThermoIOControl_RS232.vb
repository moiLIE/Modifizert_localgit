'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

Option Strict Off

#Region "Imports"
'$ Imports
Imports System.IO.Ports
Imports ThermoInterfaces
#End Region

#Region "Schnittstelle"
''' <summary>
''' </summary>
''' <remarks></remarks>
Public Class ThermoIOControl_RS232
    Implements ThermoInterfaces.ThermoIOControls_Interface

#Region "Eigenschaften RS232"
    '$ Eigenschaften RS232

    Private Const _IDLE As Integer = 0
    Private Const _BUSY As Integer = 1

    Private _MyRS232 As System.IO.Ports.SerialPort 'serielle Schnittstelle
    Private _DataContainer As ThermoDataContainer_Interface 'DatenContainer, ISReceiveReady
    Private WithEvents _Protocol As ThermoProtocol_Interface        'Irgendeine Klasse die dieses Interface implementiert, egal welche!, IsReceiveReady
    Private _MyState As Integer 'wo bin ich?
    Private WithEvents _TimTimeout As New System.Timers.Timer   'Timeout Verwalter
    Private _TimeStamp As DateTime
    Private _Timeout As Integer
    Private _ReceiveBuffer(1000) As Byte 'eingangspuffer
    Private _ReceivedString As String   'empfangener String (im Stringmode)
    Private Shared _DataRead As Integer 'gelesene Datenmenge -> evtl. Probleme wegen shared


    Private _ComMode As Integer
    Private Const _Array As Integer = 1
    Private Const _String As Integer = 2

#End Region

#Region "Öffentliche Eigenschaften"

    ''' <summary>
    ''' Empfangspuffer für die Daten von der seriellen Schnittstelle
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReceiveBuffer() As Array
        Get
            Dim Buffer(1000) As Byte
            _ReceiveBuffer.CopyTo(Buffer, 0)
            Return Buffer
        End Get
        Set(ByVal value As Array)
            value.CopyTo(_ReceiveBuffer, 0)
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
    ''' Serielle Schnittstelle
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MyRS232() As System.IO.Ports.SerialPort
        Set(ByVal value As System.IO.Ports.SerialPort)
            _MyRS232 = value
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

    Public Event Disconnected() Implements ThermoInterfaces.ThermoIOControls_Interface.Disconnected

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

#End Region

#Region "Private Methoden"
    '$ Private Methoden

    '''' <summary>
    '''' Fehlerbehandlung
    '''' </summary>
    '''' <param name="ex"></param>
    '''' <remarks></remarks>
    'Private Sub ErrorHandler(ByVal ex As Exception) Handles _Protocol.ErrorEvent
    '    RaiseEvent ErrorEvent(ex)
    'End Sub

    ''' <summary>
    ''' Timeout überwachen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TimTimeout_TimerEvent(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles _TimTimeout.Elapsed
        Dim dt As DateTime

        Try

            If _MyState = _BUSY Then

                If _MyRS232.BytesToRead = 0 Then 'nachschauen ob wirklich nix da ist

                    dt = Now
                    Dim ts As TimeSpan = dt.Subtract(_TimeStamp)
                    If ts.TotalMilliseconds > _Timeout Then
                        _MyState = _IDLE
                        RaiseEvent TimeOutEvent()
                    End If
                End If
            End If

        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try

    End Sub

    ''' <summary>
    ''' Event von der RS232 Schnittstelle
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs)
        Dim ret As Integer

        Try

            Select Case _ComMode

                Case _Array

                    ret = _MyRS232.Read(_ReceiveBuffer, _DataRead, _MyRS232.BytesToRead)
                    _DataRead += ret

                    Dim Arguments() As Object = {ReceiveBuffer, _DataRead}
                    If _Protocol.IsReceiveReady(_DataContainer, Arguments) = 1 Then          'Empfang ok???
                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                        _MyState = _IDLE
                        _TimTimeout.Stop()
                        RaiseEvent DataReceivedEventAr()        'Alles ok für dieses Gerät
                    End If

                Case _String

                    _ReceivedString += _MyRS232.ReadExisting

                    Dim Arguments() As Object = {_ReceivedString}
                    If _Protocol.IsReceiveReady(_DataContainer, Arguments) = 1 Then  'Empfang ok???
                        _TimeStamp = Now '! sonst funkt der timeout evtl. doch noch dazwischen
                        _MyState = _IDLE
                        _TimTimeout.Stop()
                        _ReceivedString = ""
                        RaiseEvent DataReceivedEventStr()    'Alles ok für dieses Gerät
                    End If

            End Select

        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent TimeOutEvent()
        End Try

    End Sub

#End Region

#Region "Öffentliche Methoden"
    '$ Öffentliche Methoden

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="SP">Handle für einen Seriellen Port</param>
    ''' <param name="Timeout">Timeout für die Abfragen</param>
    ''' <param name="Protocol">Gerätelogik die IsReceiveReady implementiert</param> 
    ''' <remarks></remarks>
    Sub New(ByVal SP As System.IO.Ports.SerialPort, ByVal Timeout As Integer, ByVal Protocol As ThermoProtocol_Interface, ByVal DataContainer As ThermoDataContainer_Interface)
        Try
            _MyRS232 = SP
            _Timeout = Timeout
            _Protocol = Protocol
            _DataContainer = DataContainer
            _MyState = _IDLE
            _TimTimeout.Interval = 1
            _TimeStamp = Now
            AddHandler _MyRS232.DataReceived, AddressOf DataReceived
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
        _TimTimeout.Stop()
        _TimTimeout = Nothing
    End Sub

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">Bytestream der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden</param> 
    ''' <remarks></remarks>
    Public Shadows Sub DoTransferAr(ByVal Command() As Byte, ByVal Length As Integer, ByVal SendFlag As Integer) Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferAr
        Try
            If (_MyState = _IDLE) Then
                _MyState = _BUSY
                _ComMode = 1
                _MyRS232.DiscardOutBuffer() 'Löscht Output Buffer
                _MyRS232.DiscardInBuffer()  'Löscht Input Buffer
                _DataRead = 0
                Array.Clear(_ReceiveBuffer, 0, _ReceiveBuffer.Length)
                _TimeStamp = Now
                If SendFlag = 1 Then
                    _MyRS232.Write(Command, 0, Length)          'Sendet Length Bytes beginnend ab Position 0 im Feld SendArray
                End If
                _TimTimeout.Start()
            End If
        Catch ex As Exception
            _MyState = _IDLE
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            RaiseEvent TimeOutEvent()
        End Try
    End Sub

    ''' <summary>
    ''' Schnittstelle wird bereinigt, Transfer wird neu gestartet
    ''' </summary>
    ''' <param name="Command">String der den Befehl beinhaltet</param>
    ''' <param name="Length">Tatsächliche Länge des Befehls im Array</param> 
    ''' <param name="SendFlag">Soll der Befehl tatsächlich gesendet werden</param> 
    ''' <remarks></remarks>
    Public Shadows Sub DoTransferStr(ByVal Command As String, ByVal Length As Integer, ByVal SendFlag As Integer) Implements ThermoInterfaces.ThermoIOControls_Interface.DoTransferStr
        Try
            If (_MyState = _IDLE) Then
                _MyState = _BUSY
                _ComMode = 2
                _MyRS232.DiscardOutBuffer() 'Löscht Output Buffer
                _MyRS232.DiscardInBuffer()  'Löscht Input Buffer
                _ReceivedString = ""
                _TimeStamp = Now
                If SendFlag = 1 Then
                    _MyRS232.Write(Command)         'Sendet Length Bytes beginnend ab Position 0 im Feld SendArray
                End If
                _TimTimeout.Start()
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

