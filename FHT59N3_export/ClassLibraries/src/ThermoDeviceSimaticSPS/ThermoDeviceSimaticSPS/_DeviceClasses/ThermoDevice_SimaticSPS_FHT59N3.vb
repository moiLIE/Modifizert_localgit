#Region "Imports"

Imports ThermoUtilities
Imports ThermoInterfaces

#End Region

''' <summary>
''' Geräteklasse für das Gerät FH40G
''' </summary>
''' <remarks>
''' Hier ist die Gerätelogik hinterlegt (Codieren / Decodieren von Daten zu / vom Gerät im Sinne des Protokolls XChannel)
''' </remarks>
Public Class ThermoDevice_SimaticSPS_FHT59N3
    Implements ThermoInterfaces.ThermoDevice_Interface

#Region "Felder Allgemein"

    Private Const _DataFormat As String = "String"
    Private _DataContainer As ThermoDeviceDataContainer_SimaticSPS_FHT59N3
    Private _MyUtilities As New ThermoUtilities.ThermoUtilities
    Private WithEvents _Protocol As ThermoProtocol_Interface
    Private _IsSTX_Protocol As Boolean
    Private _HistorySetList As String()

#End Region

#Region "Öffentliche Felder"

    ''' <summary>
    ''' In welchem Format nimmt das Gerät Daten an ("String", "Array")
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DataFormat() As String Implements ThermoInterfaces.ThermoDevice_Interface.DataFormat
        Get
            Return _DataFormat
        End Get
    End Property

    ''' <summary>
    ''' Eine Instanz der Daten_FHT681 Klasse.
    ''' </summary>
    ''' <value>
    ''' Daten_FH40G
    ''' </value>
    ''' <returns></returns>
    ''' <remarks>
    ''' In Felder dieser Instanz werden die decodierten Daten geschrieben
    ''' </remarks>
    Public Overloads Property DataContainer() As ThermoDataContainer_Interface Implements ThermoDevice_Interface.DataContainer
        Get
            Return CType(_DataContainer, ThermoDataContainer_Interface)
        End Get
        Set(ByVal value As ThermoDataContainer_Interface)
            _DataContainer = CType(value, ThermoDeviceDataContainer_SimaticSPS_FHT59N3)
        End Set
    End Property

    ''' <summary>
    ''' Event das irgendetwas schiefgelaufen ist
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Shadows Event ErrorEvent(ByVal ex As Exception) Implements ThermoDevice_Interface.ErrorEvent

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <remarks>
    ''' This function is available for compatibitly reasons only.
    ''' In July 2013 a safe data transfer protocol with STX, BCC and ETX has been introduced.
    ''' It is requested to use this new protocol!
    ''' </remarks>
    Sub New(ByVal Protocol As ThermoProtocol_Interface)
        Try
            _Protocol = Protocol
            _IsSTX_Protocol = True
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub


    ''' <summary>
    ''' Codiert den Befehl der an die SPS gesendet wird
    ''' </summary>
    ''' <param name="DataContainer"></param> 
    ''' <param name="Arguments">
    ''' Args(0) = CommandString As String
    ''' Args(1) = ParamArray Arguments() As Object (Parameter als String Übergeben, Alle zahlen werden schon
    ''' als string übergeben, auch die im E-Format!
    ''' </param> 
    ''' <returns>
    ''' true wenn ok
    ''' false wenn nicht ok
    ''' in .CommandAsString des Datencontainer den codierten Befehl als String
    ''' </returns>
    ''' <remarks></remarks>
    Public Shadows Function CodeCommand(ByVal DataContainer As ThermoDataContainer_Interface, ByVal ParamArray Arguments() As Object) As Boolean Implements ThermoDevice_Interface.CodeCommand
        Dim CommandString As String
        Dim ArgBuffer As String = ""

        Try
            _DataContainer = DataContainer
            CommandString = Arguments(0)
            If _IsSTX_Protocol Then 'data transfer protocol introduced in July 2013
                Select Case CommandString
                    Case "BT"        'filter step
                        ArgBuffer = ArgBuffer & " " & _DataContainer.Filterstep_mm.ToString("X4")
                    Case "ZG"        'bezel factor with 1 parameter
                        Dim valueAsDouble As Double
                        valueAsDouble = _DataContainer.FactorBezel * 4095 / 5
                        ArgBuffer = ArgBuffer & " " & CInt(valueAsDouble).ToString("X4")
                    Case "TG"        'Throughput Air norm with 1 parameter
                        ArgBuffer = ArgBuffer & " " & (_DataContainer.ThroughputAirNorm).ToString("X4")
                    Case "SG"        'Throughput Air operation (working) with 1 parameter
                        ArgBuffer = ArgBuffer & " " & (_DataContainer.ThroughputAirOperation).ToString("X4")
                End Select
            Else
                Select Case CommandString
                    Case "1"
                        ArgBuffer = ArgBuffer & " " & _DataContainer.Filterstep_mm.ToString
                End Select
            End If

            Dim newArguments() As Object = {CommandString, ArgBuffer}
            _Protocol.BuildProtocolFrame(DataContainer, newArguments)

            Return True
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DataContainer"></param>
    ''' <param name="Arguments"></param>
    ''' <returns>
    ''' 1 = Decode data successful
    ''' -1 = "?" = NACK received
    ''' -2 = error occured while decoding (e.g. not as many values returned than expected, type mismatch)
    ''' </returns>
    ''' <remarks></remarks>
    Public Function DecodeData(ByVal DataContainer As ThermoDataContainer_Interface, ByVal ParamArray Arguments() As Object) As Integer Implements ThermoDevice_Interface.DecodeData
        Dim SendCommand As String
        Dim nRet As Integer = 1 'positiv vorbesetzen, da Schreibbefehle überhaupt nicht dekodiert werden

        Try
            _DataContainer = DataContainer
            SendCommand = _DataContainer.Command

            'parsed _ReceiveData-Array je nach Anfangswort...
            If _IsSTX_Protocol Then
                nRet = DecodeData_STX(SendCommand, DataContainer.AnswerAsString)
            Else
                nRet = DecodeData_Plain(SendCommand, DataContainer.AnswerAsString)
            End If

        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            nRet = -10
        End Try

        Return nRet
    End Function

    REM Rückantworten der SPS auswerten für gesichertes Protokoll (STX/ETX Präambel/Postambel, BC, Quittierung ACK/NAK)
    Private Function DecodeData_STX(ByVal SendCommand As String, ByRef receiveBuffer As String)

        'define later als global constants...
        Dim STX As String = Chr(2)
        Dim ETX As String = Chr(3)
        Dim ACK As String = Chr(6)
        Dim NAK As String = Chr(21)
        Dim ETB As String = Chr(23)

        Dim HexString As String = ""
        Dim nSensorState As Integer = 0
        Dim nOutputState As Integer = 0
        Dim tmpCommand As String = ""
        Dim successfulDecodingStatus As Integer = 1
        Dim received As String = receiveBuffer

        ' this is normal reply, so just return...
        If (received.Contains("ACK") Or received.Contains("NAK") Or received.Contains("ETB")) Then
            Return 1
        End If


        Dim ReceivedDataTokens As String() = received.Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries)

        Select Case SendCommand

            Case "MR"   'read analog values

                If ReceivedDataTokens.Length = 6 Then    'Number of data values received, old SPS SW version (2013)
                    HexString = "&H" & ReceivedDataTokens(0)  'A+/A-
                    _DataContainer.AnaIn_Temperature = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(1)  'B+/B-
                    _DataContainer.AnaIn_PressureFilter = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(2)  'C+/C-
                    _DataContainer.AnaIn_PressureBezel = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(3)  'D+/D-
                    _DataContainer.AnaIn_PressureEnv = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(4)  'E+/E-, unused
                    _DataContainer.AnaIn_Reserved = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(5)  'F+/F-
                    _DataContainer.AnaIn_N2Voltage = CType(HexString, Integer)
                    successfulDecodingStatus = 1

                ElseIf ReceivedDataTokens.Length = 8 Then    'Number of data values received, new SPS SW version (2014)
                    HexString = "&H" & ReceivedDataTokens(0)  'A+/A-
                    _DataContainer.AnaIn_Temperature = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(1)  'B+/B-
                    _DataContainer.AnaIn_PressureFilter = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(2)  'C+/C-
                    _DataContainer.AnaIn_PressureBezel = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(3)  'D+/D-
                    _DataContainer.AnaIn_PressureEnv = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(4)  'E+/E-, unused
                    _DataContainer.AnaIn_Reserved = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(5)  'F+/F-
                    _DataContainer.AnaIn_N2Voltage = CType(HexString, Integer)
                    _DataContainer.AnaIn_DetectorTemperatur = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(6)  'G+/G-
                    _DataContainer.AnaIn_ReservedG = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(7)  'H+/H-
                    _DataContainer.AnaIn_ReservedH = CType(HexString, Integer)
                    successfulDecodingStatus = 1
                Else
                    successfulDecodingStatus = -1
                End If

            Case "CR"   'read calculated values
                If ReceivedDataTokens.Length = 8 Then
                    HexString = "&H" & ReceivedDataTokens(0)
                    _DataContainer.Calculated_AirThroughPutNormReceived = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(1)
                    _DataContainer.Calculated_AirThroughPutWorkingReceived = CType(HexString, Integer)

                    successfulDecodingStatus = 1
                Else
                    successfulDecodingStatus = -1
                End If

            Case "SR"   'read digital values
                If ReceivedDataTokens.Length = 2 Then    'Number of data values received
                    HexString = "&H" & ReceivedDataTokens(0) 'first value represents sensor signals
                    nSensorState = CType(HexString, Integer)
                    HexString = "&H" & ReceivedDataTokens(1) 'second value represents output status
                    nOutputState = CType(HexString, Integer)

                    ' sensors
                    If (nSensorState And 1) = 1 Then
                        _DataContainer.DigIn_HVOnOff = True
                    Else
                        _DataContainer.DigIn_HVOnOff = False
                    End If
                    If (nSensorState And 2) = 2 Then
                        _DataContainer.DigIn_CloseDetectorHead = True
                    Else
                        _DataContainer.DigIn_CloseDetectorHead = False
                    End If
                    If (nSensorState And 8) = 8 Then
                        _DataContainer.DigIn_MaintenanceOnOff = True
                    Else
                        _DataContainer.DigIn_MaintenanceOnOff = False
                    End If

                    ' outputs
                    If (nOutputState And 1) = 1 Then  'Bit0
                        _DataContainer.DigOut_PumpOnOff = True
                    Else
                        _DataContainer.DigOut_PumpOnOff = False
                    End If
                    If (nOutputState And 2) = 2 Then  'Bit1
                        _DataContainer.DigOut_AlarmRelaisOnOff = True
                    Else
                        _DataContainer.DigOut_AlarmRelaisOnOff = False
                    End If
                    If (nOutputState And 4) = 4 Then  'Bit2
                        _DataContainer.DigOut_FilterRippedOnOff = True
                    Else
                        _DataContainer.DigOut_FilterRippedOnOff = False
                    End If
                    If (nOutputState And 8) = 8 Then  'Bit3
                        _DataContainer.DigOut_ErrorOnOff = True
                    Else
                        _DataContainer.DigOut_ErrorOnOff = False
                    End If
                    If (nOutputState And 16) = 16 Then  'Bit4
                        _DataContainer.DigOut_HeatingOnOff = True
                    Else
                        _DataContainer.DigOut_HeatingOnOff = False
                    End If
                    If (nOutputState And 32) = 32 Then  'Bit5
                        _DataContainer.DigOut_MotorOnOff = True
                    Else
                        _DataContainer.DigOut_MotorOnOff = False
                    End If
                    If (nOutputState And 64) = 64 Then  'Bit6 (new SW: ecooler)
                        _DataContainer.DigOut_EcoolerOnOff = True
                    Else
                        _DataContainer.DigOut_EcoolerOnOff = False
                    End If
                    If (nOutputState And 128) = 128 Then  'Bit7
                        _DataContainer.DigOut_BypassOnOff = True
                    Else
                        _DataContainer.DigOut_BypassOnOff = False
                    End If
                    If (nOutputState And 256) = 256 Then  'Bit8
                        _DataContainer.DigOut_DetectorHeadOnOff = True
                    Else
                        _DataContainer.DigOut_DetectorHeadOnOff = False
                    End If
                    If (nOutputState And 512) = 512 Then  'Bit9
                        _DataContainer.DigOut_MaintenanceOnOff = True
                    Else
                        _DataContainer.DigOut_MaintenanceOnOff = False
                    End If
                    successfulDecodingStatus = 1
                Else
                    successfulDecodingStatus = -1
                End If

            Case "VR"   'read version number
                If ReceivedDataTokens.Length = 1 Then    'Number of data values received
                    _DataContainer.VersionNumber = ReceivedDataTokens(0)
                    successfulDecodingStatus = 1
                Else
                    successfulDecodingStatus = -1
                End If

        End Select
        Return successfulDecodingStatus
    End Function


    REM Rückantworten der SPS auswerten für Plain-Protokoll (kein STX/ETX/BC/ACK/NAK)
    Private Function DecodeData_Plain(ByVal command As String, ByRef receiveBuffer As String)

        Dim HexString As String = ""
        Dim nSensorState As Integer = 0
        Dim nOutputState As Integer = 0
        Dim successfulDecodingStatus As Integer = 1
        Dim ReceivedData = receiveBuffer.Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries)

        Select Case command

            'decode analog values....
            Case "F"
                If ReceivedData.Length = 6 Then
                    _DataContainer.AnaIn_Temperature = CType(ReceivedData(0), Single)
                    _DataContainer.AnaIn_PressureFilter = CType(ReceivedData(1), Single)
                    _DataContainer.AnaIn_PressureBezel = CType(ReceivedData(2), Single)
                    _DataContainer.AnaIn_PressureEnv = CType(ReceivedData(3), Single)
                    _DataContainer.AnaIn_Reserved = CType(ReceivedData(4), Single)
                    _DataContainer.AnaIn_N2Voltage = CType(ReceivedData(5), Single)
                    successfulDecodingStatus = 1
                Else
                    successfulDecodingStatus = -1
                End If

                'decode digital in/out values....
            Case "E"
                If ReceivedData.Length = 12 Then
                    If ReceivedData(0) = "0" Then
                        _DataContainer.DigIn_HVOnOff = False
                    Else
                        _DataContainer.DigIn_HVOnOff = True
                    End If
                    If ReceivedData(1) = "0" Then
                        _DataContainer.DigIn_CloseDetectorHead = False
                    Else
                        _DataContainer.DigIn_CloseDetectorHead = True
                    End If
                    If ReceivedData(2) = "0" Then
                        _DataContainer.DigIn_MaintenanceOnOff = False
                    Else
                        _DataContainer.DigIn_MaintenanceOnOff = True
                    End If
                    If ReceivedData(3) = "0" Then
                        _DataContainer.DigOut_PumpOnOff = False
                    Else
                        _DataContainer.DigOut_PumpOnOff = True
                    End If
                    If ReceivedData(4) = "0" Then
                        _DataContainer.DigOut_AlarmRelaisOnOff = False
                    Else
                        _DataContainer.DigOut_AlarmRelaisOnOff = True
                    End If
                    If ReceivedData(5) = "0" Then
                        _DataContainer.DigOut_FilterRippedOnOff = False
                    Else
                        _DataContainer.DigOut_FilterRippedOnOff = True
                    End If
                    If ReceivedData(6) = "0" Then
                        _DataContainer.DigOut_ErrorOnOff = False
                    Else
                        _DataContainer.DigOut_ErrorOnOff = True
                    End If
                    If ReceivedData(7) = "0" Then
                        _DataContainer.DigOut_HeatingOnOff = False
                    Else
                        _DataContainer.DigOut_HeatingOnOff = True
                    End If
                    If ReceivedData(8) = "0" Then
                        _DataContainer.DigOut_MotorOnOff = False
                    Else
                        _DataContainer.DigOut_MotorOnOff = True
                    End If
                    If ReceivedData(9) = "0" Then
                        _DataContainer.DigOut_BypassOnOff = False
                    Else
                        _DataContainer.DigOut_BypassOnOff = True
                    End If
                    If ReceivedData(10) = "0" Then
                        _DataContainer.DigOut_DetectorHeadOnOff = False
                    Else
                        _DataContainer.DigOut_DetectorHeadOnOff = True
                    End If
                    If ReceivedData(11) = "0" Then
                        _DataContainer.DigOut_MaintenanceOnOff = False
                    Else
                        _DataContainer.DigOut_MaintenanceOnOff = True
                    End If
                    successfulDecodingStatus = 1
                Else
                    successfulDecodingStatus = -1
                End If
        End Select

        Return successfulDecodingStatus
    End Function


    ''' <summary>
    ''' Blockchecksumme bilden. Später in ThermoUtilities auslagern. Da Versionierung unbekannt wird vorerst mit 
    ''' duplikatem Code gearbeitet
    ''' </summary>
    ''' <param name="Input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BuildBCC(ByVal Input As String) As String
        Try
            Dim ByteArray(Input.Length) As Byte
            Dim BCC As Integer = 0
            ByteArray = System.Text.Encoding.ASCII.GetBytes(Input)
            For i As Integer = 0 To ByteArray.Length - 1
                BCC = BCC + ByteArray(i)
            Next
            BCC = BCC Mod 256
            Dim HexBCC As String = Hex(BCC)
            HexBCC = _MyUtilities.FillHexString(HexBCC)
            Return HexBCC
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return ""
        End Try
    End Function


#End Region

End Class
