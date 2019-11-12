'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH						 #
'# Author: Thomas Kuschel														 #	
'#####################################################'

#Region "Imports"

Imports ThermoInterfaces
Imports ThermoProtocols
Imports ThermoLogging

#End Region

''' <summary>
''' eine Klasse die viele unabhängige Statusmaschinen triggern können soll. Das Problem bei einer Statusmaschine und
''' vielen Geräten ist, wenn ein Gerät nicht mehr antwortet. Die Statusmaschine ist so geschrieben, das erst eine
''' neue Runde von Befehlen gesetzt und ausgeführt werden kann, wenn alle Befehle für alle Geräte abgearbeitet wurden.
''' Das ist unter umständen eine Bremse!
''' Dieser über diesen Container können die Statusmaschinen unabhängig getriggert werden.
''' Dieser Container macht aber generell nur im Ethernet-Mode Sinn, das sich im seriellen Fall nix Beschleunigen
''' lässt.
''' </summary>
''' <remarks></remarks>
Public Class ThermoStateMachineContainer

    '#Region "Private Eigenschaften"

    '	Private _StateMachines As New Dictionary(Of String, ThermoStateMachine)	'liste der Statusmaschinen
    '	Private _Index As Integer	'Index der Statusmaschinen
    '	Private _IndexList As New Dictionary(Of Integer, String) 'welche Statusmaschine behandelt welche IP

    '    'Logging
    '    Private _MyTraceHandler As TraceHandler

    '#End Region

    '#Region "Öffentliche Eigenschaften"

    '	''' <summary>
    '	''' Statusmaschine des Gerätes!
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <value></value>
    '	''' <returns></returns>
    '	''' <remarks></remarks>
    '	Public ReadOnly Property Statemachine(ByVal IPAddress As String) As ThermoStateMachine
    '		Get
    '			If _StateMachines.ContainsKey(IPAddress) Then
    '				Return _StateMachines(IPAddress)
    '			Else
    '				Return Nothing
    '			End If
    '		End Get
    '	End Property

    '	Public ReadOnly Property IndexList(ByVal Index As Integer) As String 'welche Statusmaschine behandelt welche IP
    '		Get
    '			Dim RetStr As String = ""
    '			If _IndexList.ContainsKey(Index) Then
    '				RetStr = _IndexList(Index)
    '			End If
    '			Return RetStr
    '		End Get
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

    '	''' <summary>
    '	''' Event das irgendetwas schiefgelaufen ist
    '	''' </summary>
    '	''' <remarks></remarks>
    '    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As Exception)

    '	''' <summary>
    '	''' Alle Befehle abgearbeitet, für ein Gerät
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="ReturnListStringEth"></param>
    '	''' <remarks></remarks>
    '	Public Event CommandsDoneStr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListStringEth As List(Of String))

    '	''' <summary>
    '	''' Alle Befehle abgearbeitet, für ei Gerät
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="ReturnListArrayEth"></param>
    '	''' <remarks></remarks>
    '	Public Event CommandsDoneAr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListArrayEth As List(Of Byte()))

    '	''' <summary>
    '	''' Alle Befehle abgearbeitet, für ein Gerät
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="ReturnStringEth"></param>
    '	''' <remarks></remarks>
    '	Public Event CommandsDoneStrAsync(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnStringEth As String)

    '	''' <summary>
    '	''' Alle Befehle abgearbeitet, für ei Gerät
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="ReturnArrayEth"></param>
    '	''' <remarks></remarks>
    '	Public Event CommandsDoneArAsync(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnArrayEth As Byte())

    '	''' <summary>
    '	''' Meldung das alle Befehle für alle Geräte abgearbeitet wurden; kommt nur im Ethernet-Mode
    '	''' </summary>
    '	''' <param name="Index "></param>
    '	''' <remarks></remarks>
    '	Public Event AllWorkDone(ByVal Index As Integer)

    '	''' <summary>
    '	''' eine Meldung vom Gerät empfangen (Autosend!)
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnListStringEth"></param>
    '	''' <remarks></remarks>
    '	Public Event CommandReceivedStr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListStringEth As List(Of String))

    '	''' <summary>
    '	''' eine Meldung vom Gerät empfangen (Autosend!)
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnListArrayEth"></param>
    '	''' <remarks></remarks>
    '	Public Event CommandReceivedAr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListArrayEth As List(Of Byte()))

    '	''' <summary>
    '	''' Gerät wurde verbunden
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <remarks></remarks>
    '	Public Event DeviceConnected(ByVal Index As Integer, ByVal IPAddress As String, ByVal Connected As Boolean)

    '	''' <summary>
    '	''' Verbindung mit Gerät unterbrochen
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <remarks></remarks>
    '	Public Event DeviceDisconnected(ByVal Index As Integer, ByVal IPAddress As String)

    '	''' <summary>
    '	''' Nachricht schicken
    '	''' </summary>
    '	''' <param name="Msg"></param>
    '	''' <param name="debug"></param>
    '	''' <remarks></remarks>
    '	Public Event NewMessage(ByVal Msg As String, ByVal debug As Boolean)

    '#End Region

    '#Region "Private Methoden"

    '    '''' <summary>
    '    '''' ErrorEvent der Statusmaschinen
    '    '''' </summary>
    '    '''' <param name="ex"></param>
    '    '''' <remarks></remarks>
    '    'Private Sub ErrorInSubClass(ByVal ex As Exception)
    '    '	RaiseEvent ErrorEvent(ex)
    '    'End Sub

    '	''' <summary>
    '	''' Nachricht schicken
    '	''' </summary>
    '	''' <param name="Msg"></param>
    '	''' <param name="debug"></param>
    '	''' <remarks></remarks>
    '	Private Sub MsgHandler(ByVal Msg As String, ByVal debug As Boolean)
    '		RaiseEvent NewMessage(Msg, debug)
    '	End Sub

    '	''' <summary>
    '	''' Befehl ausgeführt
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnListStringEth"></param>
    '	''' <remarks></remarks>
    '	Private Sub CDoneStr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListStringEth As List(Of String))
    '		RaiseEvent CommandsDoneStr(Index, IPAddress, ReturnListStringEth)
    '	End Sub

    '	''' <summary>
    '	''' Befehl ausgeführt
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnListArrayEth"></param>
    '	''' <remarks></remarks>
    '	Private Sub CDoneAr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListArrayEth As List(Of Byte()))
    '		RaiseEvent CommandsDoneAr(Index, IPAddress, ReturnListArrayEth)
    '	End Sub

    '	''' <summary>
    '	''' Befehl ausgeführt
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnStringEth"></param>
    '	''' <remarks></remarks>
    '	Private Sub CDoneStrAsnyc(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnStringEth As String)
    '		RaiseEvent CommandsDoneStrAsync(Index, IPAddress, ReturnStringEth)
    '	End Sub

    '	''' <summary>
    '	''' Befehl ausgeführt
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnArrayEth"></param>
    '	''' <remarks></remarks>
    '	Private Sub CDoneArAsync(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnArrayEth As Byte())
    '		RaiseEvent CommandsDoneArAsync(Index, IPAddress, ReturnArrayEth)
    '	End Sub

    '	''' <summary>
    '	''' alle Befehle ausgeführt
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <remarks></remarks>
    '	Private Sub AWorkDone(ByVal Index As Integer)
    '		RaiseEvent AllWorkDone(Index)
    '	End Sub

    '	''' <summary>
    '	''' Daten vom Gerät erhalten (Autosend)
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnListStringEth"></param>
    '	''' <remarks></remarks>
    '	Private Sub CReceivedStr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListStringEth As List(Of String))
    '		RaiseEvent CommandReceivedStr(Index, IPAddress, ReturnListStringEth)
    '	End Sub

    '	''' <summary>
    '	''' Daten vom Gerät erhalten (Autosend)
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="ReturnListArrayEth"></param>
    '	''' <remarks></remarks>
    '	Private Sub CReceivedAr(ByVal Index As Integer, ByVal IPAddress As String, ByVal ReturnListArrayEth As List(Of Byte()))
    '		RaiseEvent CommandReceivedAr(Index, IPAddress, ReturnListArrayEth)
    '	End Sub

    '	''' <summary>
    '	''' Gerät verbunden
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="Connected"></param>
    '	''' <remarks></remarks>
    '	Private Sub DevConnected(ByVal Index As Integer, ByVal IPAddress As String, ByVal Connected As Boolean)
    '		RaiseEvent DeviceConnected(Index, IPAddress, Connected)
    '	End Sub

    '	''' <summary>
    '	''' Gerät nicht verbunden
    '	''' </summary>
    '	''' <param name="Index"></param>
    '	''' <param name="IPAddress"></param>
    '	''' <remarks></remarks>
    '	Private Sub DevDisconnected(ByVal Index As Integer, ByVal IPAddress As String)
    '		RaiseEvent DeviceDisconnected(Index, IPAddress)
    '	End Sub

    '#End Region

    '#Region "Öffentliche Methoden"

    '	''' <summary>
    '	''' Konstruktor
    '	''' </summary>
    '	''' <remarks></remarks>
    '    Public Sub New(ByVal TraceHandler As ThermoLogging.TraceHandler)
    '        Try
    '            _Index = 0
    '            _MyTraceHandler = TraceHandler
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '    ''' <summary>
    '    ''' Alles kaputtmachen
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '	Public Sub Dispose()
    '        Try
    '            For Each IP As String In _StateMachines.Keys
    '                _StateMachines(IP).StopMachine()
    '                _StateMachines(IP).DeleteDeviceEth(IP)
    '                _StateMachines(IP).Dispose()
    '            Next
    '            _StateMachines.Clear()
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '	''' <summary>
    '	''' Neue Maschine hinzu
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="Port"></param>
    '	''' <param name="ProtocolAssembly"></param>
    '	''' <param name="ProtocolName"></param>
    '	''' <param name="DataContainerAssembly"></param>
    '	''' <param name="DataContainerName"></param>
    '	''' <param name="Timeout"></param>
    '	''' <remarks></remarks>
    '    Public Sub AddMachine(ByVal Interval As Integer, ByVal IPAddress As String, ByVal Port As Integer, ByVal ProtocolAssembly As String, ByVal ProtocolName As String, ByVal DataContainerAssembly As String, ByVal DataContainerName As String, ByVal Timeout As Integer, ByVal Repeats As Integer, ByVal AliveCheckIntervall As Integer, ByVal ConnectionTimeout As Integer, Optional ByVal ReceiveDataWithoutQuestion As Boolean = False)
    '        Try
    '            If Not _StateMachines.ContainsKey(IPAddress) Then
    '                _Index = _Index + 1
    '                Dim NewMachine As New ThermoStateMachine(Interval, _Index, ProtocolAssembly, ProtocolName, DataContainerAssembly, DataContainerName, Timeout, Repeats, True, ConnectionTimeout, _MyTraceHandler, AliveCheckIntervall, ReceiveDataWithoutQuestion)
    '                'AddHandler NewMachine.ErrorEvent, AddressOf ErrorInSubClass
    '                AddHandler NewMachine.CommandsDoneStr, AddressOf CDoneStr
    '                AddHandler NewMachine.CommandsDoneAr, AddressOf CDoneAr
    '                AddHandler NewMachine.CommandsDoneStrAsync, AddressOf CDoneStrAsnyc
    '                AddHandler NewMachine.CommandsDoneArAsync, AddressOf CDoneArAsync
    '                AddHandler NewMachine.AllWorkDone, AddressOf AWorkDone
    '                AddHandler NewMachine.CommandReceivedStr, AddressOf CReceivedStr
    '                AddHandler NewMachine.CommandReceivedAr, AddressOf CReceivedAr
    '                AddHandler NewMachine.DeviceConnected, AddressOf DevConnected
    '                AddHandler NewMachine.DeviceDisconnected, AddressOf DevDisconnected
    '                AddHandler NewMachine.NewMessage, AddressOf MsgHandler
    '                _IndexList.Add(_Index, IPAddress)
    '                _StateMachines.Add(IPAddress, NewMachine)
    '                _StateMachines(IPAddress).AddDeviceEth(IPAddress, Port)
    '                _StateMachines(IPAddress).StartMachine()
    '            Else
    '                Dim exe As New Exception("Device with this ID: " & IPAddress & "is already existing")
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '	''' <summary>
    '	''' Maschine löschen
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <remarks></remarks>
    '	Public Sub DeleteMachine(ByVal IPAddress As String)
    '        Try
    '            If _StateMachines.ContainsKey(IPAddress) Then
    '                _StateMachines(IPAddress).StopMachine()
    '                _StateMachines(IPAddress).DeleteDeviceEth(IPAddress)
    '                _StateMachines.Remove(IPAddress)
    '            Else
    '                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '	''' <summary>
    '	''' Neue Befehlsliste setzten (strings)
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="CommandList"></param>
    '	''' <param name="CommandListUncoded"></param>
    '	''' <remarks></remarks>
    '	Public Sub PutNewListEth(ByVal IPAddress As String, ByVal CommandList As List(Of String), ByVal CommandListUncoded As List(Of String))
    '        Try
    '            If _StateMachines.ContainsKey(IPAddress) Then
    '                _StateMachines(IPAddress).PutNewListEth(IPAddress, CommandList, CommandListUncoded)
    '            Else
    '                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '	''' <summary>
    '	''' Neue Befehlsliste setzten (array of byte)
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="CommandList"></param>
    '	''' <param name="CommandListUncoded"></param>
    '	''' <remarks></remarks>
    '	Public Sub PutNewListEth(ByVal IPAddress As String, ByVal CommandList As List(Of Byte()), ByVal CommandListUncoded As List(Of String))
    '        Try
    '            If _StateMachines.ContainsKey(IPAddress) Then
    '                _StateMachines(IPAddress).PutNewListEth(IPAddress, CommandList, CommandListUncoded)
    '            Else
    '                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '	''' <summary>
    '	''' Signal an eine Maschine; alle Maschinen werden unabhängig gesteuert
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="Signal"></param>
    '	''' <remarks></remarks>
    '	Public Function SetSignal(ByVal IPAddress As String, ByVal Signal As ThermoStateMachine.Signals) As Boolean
    '        Try
    '            Dim ret As Boolean
    '            If _StateMachines.ContainsKey(IPAddress) Then
    '                ret = _StateMachines(IPAddress).SetSignal(Signal)
    '                Return ret
    '            Else
    '                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Function

    '	''' <summary>
    '	''' genau einen Befehl an genau ein Device senden
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="Command"></param>
    '	''' <remarks></remarks>
    '	Public Sub SendAsyncCommandStrEth(ByVal IPAddress As String, ByVal Command As String, ByVal CommandUncoded As String)
    '        Try
    '            If _StateMachines.ContainsKey(IPAddress) Then
    '                _StateMachines(IPAddress).SendAsyncCommandStrEth(IPAddress, Command, CommandUncoded)
    '            Else
    '                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '	''' <summary>
    '	''' genau einen Befehl an genau ein Device senden
    '	''' </summary>
    '	''' <param name="IPAddress"></param>
    '	''' <param name="Command"></param>
    '	''' <remarks></remarks>
    '	Public Sub SendAsyncCommandArEth(ByVal IPAddress As String, ByVal Command As Byte(), ByVal CommandUncoded As String)
    '        Try
    '            If _StateMachines.ContainsKey(IPAddress) Then
    '                _StateMachines(IPAddress).SendAsyncCommandArEth(IPAddress, Command, CommandUncoded)
    '            Else
    '                Dim exe As New Exception("No Device with this ID: " & IPAddress)
    '                'RaiseEvent ErrorEvent(exe)
    '                Trace.TraceError(exe.Message & " " & exe.StackTrace)
    '            End If
    '        Catch ex As Exception
    '            Trace.TraceError(ex.Message & " " & ex.StackTrace)
    '        End Try
    '    End Sub

    '#End Region

End Class
