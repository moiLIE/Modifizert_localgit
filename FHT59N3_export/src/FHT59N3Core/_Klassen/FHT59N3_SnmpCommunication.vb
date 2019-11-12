Imports System.Net
Imports System.Threading

Imports Lextm.SharpSnmpLib
Imports Lextm.SharpSnmpLib.Messaging

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Enum SnmpRetrieveResult
    Undefined
    ValidData
    UpsmonError
    UpsConnectionError
End Enum

' -------------------------------------------------------------
' $Id:  $
' Title: support for accessing the UPS SNMP nodes
'
' Description:
' Contains a thread which polls the UPS SNMP nodes. The result
' can be accessed via GetUpsBatteryStatus() and GetUpsOnBattery()
' -------------------------------------------------------------
Public Class FHT59N3_SnmpCommunication



    ''' <summary>
    ''' Timeout for the TCP access to the local SNMP server
    ''' </summary>
    ''' <remarks></remarks>
    Private Const SNMP_READ_TIMEOUT_IN_MS As Int32 = 2000

    ''' <summary>
    ''' thread cycle time for accessing the SNMP node
    ''' </summary>
    ''' <remarks></remarks>
    Private Const THREAD_SLEEP_IN_MS As Int32 = 5000

    ''' <summary>
    ''' Node to read the UPS model (defined by GENEREX): upsIdent.upsIdentModelName
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UID_OID_IDENT_MODELNAME As String = ".1.3.6.1.4.1.1356.1.1.1.0"

    ''' <summary>
    ''' Node to read the Battery status: upsBattery.upsBatteryStatus
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UPS_OID_BATTERY_STATUS As String = "1.3.6.1.4.1.1356.1.2.1.0"

    ''' <summary>
    ''' 'upsBatteryStatus: 0 if not connected with UPS, otherwise: unknown=1, batteryNormal=2, batteryLow=3
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UPS_BATTERY_STATUS_LOW As Int32 = 3

    ''' <summary>
    ''' Node to read the current UPS status: upsOutput.upsOutputStatus
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UPS_OID_OUTPUT_STATUS As String = "1.3.6.1.4.1.1356.1.4.1.0"

    ''' <summary>
    ''' upsOutputStatus: 0 if not connected with UPS, otherwise: unknown=1, onLine=2, onBattery=3, onByPass=4, shutdown=5
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UPS_OUTPUTSTATUS_ON_BATTERY As Int32 = 3

    Private Const UPS_DEFAULT_INTEGER As String = "2686924"
    Private Const UPS_NO_DATA_FOUND As String = "NoSuchObject"

    Private ReadOnly _ADDRESS As String = "127.0.0.1"
    Private ReadOnly _PORT As Int16 = 161
    Private ReadOnly _COMMUNITY_STRING As String = "public"

    Private snmpThread As Thread
    Private snmpThreadRunning As Boolean
    Private ReadOnly snmpReadCycleLockObject As Object = New Object()  'locking object

    Public Const SNMP_RETRIEVE_TIMEOUT_SECONDS As Integer = 15

    Private _isUpsOnBattery As Boolean

    Public Property LastRetrieve As ChangableObject(Of SnmpRetrieveResult) = New ChangableObject(Of SnmpRetrieveResult) _
            With {.Value = SnmpRetrieveResult.Undefined}

    Private Sub New()

    End Sub

    Public Sub New(ByVal ipAddressOfSnmp As String, ByVal portOfSnmp As Int16, ByVal snmpCommunity As String)
        _ADDRESS = ipAddressOfSnmp
        _PORT = portOfSnmp
        _COMMUNITY_STRING = snmpCommunity
    End Sub


    ''' <summary>
    ''' Checks if ups is on battery
    ''' </summary>
    ''' <returns>true if UPS is on battery</returns>
    ''' <remarks></remarks>
    Public Function GetUpsOnBattery() As Boolean
        Return _isUpsOnBattery
        'Zum Testen: Return True
    End Function


    ''' <summary>
    ''' The thread which communicates with the snmp agent
    ''' </summary>
    ''' <remarks>Every 1000ms a SNMP node request for the battery status is sent</remarks>
    Private Sub ThreadTask()

        Dim onBatteryValid As SnmpRetrieveResult
        Dim onBattery As Int32
        Dim batteryStatus As Int32
        Dim _isUpsBatteryStatusLow As Boolean

        Do
            Try

                LastRetrieve.Value = GetObjectData(UPS_OID_BATTERY_STATUS, batteryStatus)

                If LastRetrieve.Value = SnmpRetrieveResult.ValidData Then
                    If batteryStatus = UPS_BATTERY_STATUS_LOW Then
                        _isUpsBatteryStatusLow = True
                    Else
                        _isUpsBatteryStatusLow = False
                    End If
                End If

                onBatteryValid = GetObjectData(UPS_OID_OUTPUT_STATUS, onBattery)
                If onBatteryValid = SnmpRetrieveResult.ValidData Then
                    If onBattery = UPS_OUTPUTSTATUS_ON_BATTERY Then
                        _isUpsOnBattery = True
                    Else
                        _isUpsOnBattery = False
                    End If
                End If

                SyncLock snmpReadCycleLockObject
                    Monitor.Wait(snmpReadCycleLockObject, THREAD_SLEEP_IN_MS)
                End SyncLock
            Catch ex As Exception
                LastRetrieve.Value = SnmpRetrieveResult.UpsConnectionError
            End Try

            'Hier kann von außen gezwungen werden die Abfrageschleife zu verlassen.
            If Not snmpThreadRunning Then
                Exit Do
            End If
        Loop

    End Sub


    ''' <summary>
    ''' Start the thread which communicate with the snmp agent
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartThread()
        If IsNothing(snmpThread) Then
            snmpThread = New Thread(AddressOf ThreadTask)

            snmpThread.IsBackground = True
            snmpThread.Name = "SNMP supervision for UPS"
            snmpThread.Start()
            snmpThreadRunning = True
        End If
    End Sub

    ''' <summary>
    ''' Stops the thread which communicate with the snmp agent
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopThread()
        If Not IsNothing(snmpThread) Then
            snmpThreadRunning = False

            SyncLock snmpReadCycleLockObject
                'Monitor.Wait aufwecken...
                Monitor.Pulse(snmpReadCycleLockObject)
            End SyncLock

            snmpThread.Join(500)

            If snmpThread.IsAlive Then
                snmpThread.Abort()
            End If
        End If
    End Sub


    ''' <summary>
    ''' Gets the data of a snmp object
    ''' </summary>
    ''' <param name="objectId">the object id</param>
    ''' <param name="objectValue">the value of the object</param>
    ''' <returns>Enum of object validation</returns>
    ''' <remarks></remarks>
    Private Function GetObjectData(objectId As String, ByRef objectValue As String) As SnmpRetrieveResult
        Dim result As New List(Of Variable)
        Dim returnedData As String
        Try
            result = Messenger.Get(VersionCode.V2,
                                   New IPEndPoint(IPAddress.Parse(_ADDRESS), _PORT),
                                   New OctetString(_COMMUNITY_STRING),
                                   New List(Of Variable)(New Variable() {New Variable(New ObjectIdentifier(objectId))}),
                                   SNMP_READ_TIMEOUT_IN_MS)
            returnedData = result.Item(0).Data.ToString

            If (IsNothing(result) Or IsNothing(result.Item(0)) Or IsNothing(result.Item(0).Data) Or returnedData = UPS_NO_DATA_FOUND) Then
                Return SnmpRetrieveResult.UpsmonError
            ElseIf returnedData = String.Empty Or returnedData = UPS_DEFAULT_INTEGER Then
                Return SnmpRetrieveResult.UpsConnectionError
            Else
                objectValue = returnedData
                Return SnmpRetrieveResult.ValidData
            End If
        Catch ex As Exception
            'Trace.TraceError("Error in UPS SNMP GetObjectData: " + ex.Message + vbCrLf & "Stacktrace : " + ex.StackTrace)
            Return SnmpRetrieveResult.UpsConnectionError
        End Try
    End Function
End Class