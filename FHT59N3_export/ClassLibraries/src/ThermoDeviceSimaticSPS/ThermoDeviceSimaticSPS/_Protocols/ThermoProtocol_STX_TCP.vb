Imports ThermoInterfaces
Imports ThermoLogging

Public Class ThermoProtocol_STX_TCP
    Implements ThermoProtocol_Interface

    Private NUL As String = Chr(0)
    Private STX As String = Chr(2)
    Private ETX As String = Chr(3)
    Private ACK As String = Chr(6)
    Private NAK As String = Chr(21)
    Private ETB As String = Chr(23)

    Private _MyUtilities As New ThermoUtilities.ThermoUtilities

    'Logging
    Private _MyTraceHandler As TraceHandler


    ''' <summary>
    ''' fürs debuggen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TraceHandler() As ThermoLogging.TraceHandler Implements ThermoProtocol_Interface.TraceHandler
        Get
            Return _MyTraceHandler
        End Get
        Set(ByVal value As ThermoLogging.TraceHandler)
            _MyTraceHandler = value
        End Set
    End Property

    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As System.Exception) Implements ThermoInterfaces.ThermoProtocol_Interface.ErrorEvent

    ''' <summary>
    ''' Debug Infos!
    ''' </summary>
    ''' <param name="Msg"></param>
    ''' <param name="debug"></param>
    ''' <remarks></remarks>
    Public Event NewMessage(ByVal Msg As String, ByVal debug As Boolean) Implements ThermoProtocol_Interface.NewMessage


    ''' <summary>
    ''' Blockchecksumme bilden
    ''' </summary>
    ''' <param name="Input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BuildBCCInteger(ByVal Input As String) As Integer
        Try
            Dim ByteArray(Input.Length) As Byte
            Dim BCC As Integer = 0
            ByteArray = System.Text.Encoding.ASCII.GetBytes(Input)
            For i As Integer = 0 To ByteArray.Length - 1
                BCC = BCC + ByteArray(i)
            Next
            BCC = BCC Mod 256
            Return BCC
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return ""
        End Try
    End Function

    Private Sub WriteLog(ByVal EntryType As TraceEventType, ByVal LogEntry As String)
        'If Not _MyTraceHandler Is Nothing Then
        '    _MyTraceHandler.WriteLog(EntryType, 0, LogEntry)
        'End If
        'Trace.TraceInformation(LogEntry)
    End Sub

    ''' <summary>
    ''' Überprüfung ob die empfangenen Daten korrekt und vollständig im Sinne des Protokolls sind
    ''' </summary>
    ''' <param name="DataContainer"></param> 
    ''' <param name="Arguments">
    ''' Arg(0) = ReceiveBuffer As String
    ''' </param> 
    ''' <returns>
    '''  1 = wenn ok
    ''' -1 = wenn nicht ok
    ''' -2 = Checksum error
    ''' -3 = ETB received
    ''' -4 = NAK received
    ''' </returns>
    ''' <remarks></remarks>
    Public Function IsReceiveReady(ByVal DataContainer As ThermoDataContainer_Interface, ByVal ParamArray Arguments() As Object) As Integer Implements ThermoInterfaces.ThermoProtocol_Interface.IsReceiveReady
        Try
            Dim ErrorString As String = ""
            Dim ReceiveBuffer As String = ""

            ReceiveBuffer = CType(Arguments(0), String)
            WriteLog(TraceEventType.Information, "ThermoProtocol_STX, IsReceiveReady, Receivebuffer: " & ReceiveBuffer)
            Dim IndexOfSTX As Integer = ReceiveBuffer.IndexOf(STX)
            Dim IndexOfETX As Integer = ReceiveBuffer.IndexOf(ETX)
            Dim IndexOfNAK As Integer = ReceiveBuffer.IndexOf(NAK)
            Dim IndexOfACK As Integer = ReceiveBuffer.IndexOf(ACK)

            Dim IndexOfETB As Integer = ReceiveBuffer.IndexOf(ETB)

            Dim EndMarker = IndexOfNAK
            If (EndMarker = -1) Then
                EndMarker = IndexOfACK
            End If
            If (EndMarker = -1) Then
                EndMarker = IndexOfETB
            End If

            WriteLog(TraceEventType.Information, "ThermoProtocol_STX_TCP, IsReceiveReady: " & "Bel: " & IndexOfSTX.ToString & " End: " & EndMarker.ToString)

            'how is nak recognized???

            'we have the case of reply from S7-1200
            If (IndexOfSTX <> -1) And (EndMarker <> -1) Then
                ReceiveBuffer = ReceiveBuffer.Substring(IndexOfSTX + 1, EndMarker - IndexOfSTX - 1)
                StripPreambelAndPostambel(ReceiveBuffer)

                'we only have ACK/NAK/ETB, detect...
                If (ReceiveBuffer.Length = 0) Then
                    'handle NAK before ACK!
                    If (IndexOfNAK <> -1) Then 'NAK empfangen
                        DataContainer.AnswerAsString = "NAK"
                        DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                        Return -4
                    ElseIf (IndexOfACK <> -1) Then 'ACK empfangen
                        DataContainer.AnswerAsString = "ACK"
                        DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                        Return 1

                    ElseIf (IndexOfETB <> -1) Then 'ETB empfangen
                        DataContainer.AnswerAsString = "ETB"
                        DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                        Return -3
                    Else 'nix ordentliches
                        Return -1
                    End If
                End If

                    DataContainer.AnswerAsString = ReceiveBuffer
                    DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                    WriteLog(TraceEventType.Information, "ThermoProtocol_STX_TCP, IsReceiveReady, ReceiveBuffer: " & ReceiveBuffer)
                    Return 1
                Else
                    'nix gescheites
                    Return -1


                End If
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return -1
        End Try
    End Function


    ''' <summary>
    ''' Checks whether a preambel has to be stripped away from the reply answer. In STX-ETX format we do not expect line-feed 
    ''' as done for plain protocol
    ''' </summary>
    ''' <param name="ReceiveBuffer">the receive buffer from the SPS</param>
    ''' <returns>negative value in case of an error, zero or positive value on success or if no stripping was necessary</returns>
    ''' <remarks></remarks>
    Private Function StripPreambelAndPostambel(ByRef ReceiveBuffer As String)

        Dim PreambelReplyDigitalValues As String = "E "
        Dim PreambelReplyAnalogValues As String = "F "

        'stupid to do it hardcoded, but we have GW1/GW0 which are three chars! Check GW1/GW0 before GW!!!
        Dim Prefixes As String() = {"MRF", "SRE", "CRG",
                                    "BT", "BS", "BG", "CG", "CS", "HG", "HS", "PG", "PS", "AG",
                                    "AS", "YG", "YS", "EG", "ES", "GW1", "GW0", "GW", "TG", "VR", "ZG"}

        For Each Prefix As String In Prefixes
            If (ReceiveBuffer.StartsWith(Prefix)) Then
                ReceiveBuffer = ReceiveBuffer.TrimStart(Prefix.ToCharArray)
                Exit For
            End If
        Next

        'don't know why sometimes we have spaces or NUL chars in ACK/NACK-reply, ask Mr. Ahrens
        ReceiveBuffer = ReceiveBuffer.Trim.Replace(Chr(0), "")


        'at the moment we do not have any condition where call with return fail status...
        Return 1
    End Function




    ''' <summary>
    ''' Protokoll zusammennageln
    ''' </summary>
    '''<param name="DataContainer"></param> 
    ''' <param name="Arguments">
    ''' Args(0) = CommandString As String
    ''' Args(1) = Optional ByVal ArgBuffer As String = ""
    ''' </param> 
    ''' <remarks></remarks>
    Public Sub BuildProtocolFrame(ByVal DataContainer As ThermoDataContainer_Interface, ByVal ParamArray Arguments() As Object) Implements ThermoInterfaces.ThermoProtocol_Interface.BuildProtocolFrame
        Try
            Dim NewCommand As String
            Dim CommandString As String
            Dim ArgBuffer As String = ""

            Dim Address As String = ""
            Dim BCC As String = ""
            CommandString = CType(Arguments(0), String)
            If Arguments.Length = 2 Then
                ArgBuffer = CType(Arguments(1), String)
            End If
            If ArgBuffer <> "" Then
                NewCommand = STX & CommandString & ArgBuffer
            Else
                NewCommand = STX & CommandString
            End If
            BCC = Hex(BuildBCCInteger(NewCommand))
            BCC = _MyUtilities.FillHexString(BCC)
            NewCommand = NUL & NewCommand & BCC & ETX
            Do While NewCommand.Length < 100
                NewCommand = NewCommand & NUL
            Loop

            DataContainer.Command = CommandString
            DataContainer.CommandAsString = NewCommand
            DataContainer.CommandLength = DataContainer.CommandAsString.Length
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Protokoll um einen gegebenen String herumbauen
    ''' </summary>
    '''<param name="DataContainer"></param> 
    ''' <param name="Command">Befehl, vollständig zusammengebaut</param>
    ''' <remarks></remarks>
    Public Sub BuildProtocolFrame(ByVal DataContainer As ThermoDataContainer_Interface, ByVal Command As String)
        Try
            Dim NewCommand As String
            Dim BCC As String = ""
            Dim CommandString As String = ""
            CommandString = Command.Substring(2, 2)
            '01HR
            '^^^^
            '0123
            NewCommand = STX & Command
            BCC = Hex(BuildBCCInteger(NewCommand))
            BCC = _MyUtilities.FillHexString(BCC)
            NewCommand = NewCommand & BCC & ETX
            DataContainer.Command = CommandString
            DataContainer.CommandAsString = NewCommand
            DataContainer.CommandLength = DataContainer.CommandAsString.Length
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub


End Class


