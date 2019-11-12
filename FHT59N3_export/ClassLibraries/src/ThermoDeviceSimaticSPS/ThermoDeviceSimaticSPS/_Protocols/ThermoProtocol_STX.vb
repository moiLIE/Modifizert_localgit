#Region "Imports"
Imports ThermoInterfaces
Imports ThermoLogging
#End Region


Public Class ThermoProtocol_STX
    Implements ThermoProtocol_Interface

#Region "Private Eigenschaften"

    Private STX As String = Chr(2)
    Private ETX As String = Chr(3)
    Private ACK As String = Chr(6)
    Private NAK As String = Chr(21)
    Private ETB As String = Chr(23)

    Private _MyUtilities As New ThermoUtilities.ThermoUtilities

    'Logging
    Private _MyTraceHandler As TraceHandler

#End Region

#Region "Öffentliche Eigenschaften"

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

#End Region

#Region "Private Methoden"

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

    ''' <summary>
    ''' Die Funktion prüft einen String daraufhin, ob er ein korrektes Format erfüllt.
    ''' </summary>
    ''' <returns>
    ''' 1, wenn ok, -1, wenn Checksumme falsch (BCC!)
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ValidateSPSBlockCode(ByRef ResponseData As String, ByVal StripBlockCode As Boolean) As Integer


        Try
            'BCC aus dem String auslesen: 2 zeichen vor dem ETX ist die Checksumme (hex)
            Dim IdxBC = ResponseData.Length - 2

            Dim PayloadWithoutBCC = ResponseData.Substring(0, IdxBC)
            Dim BCCAsString As String = ResponseData.Substring(IdxBC)

            'übergebenen BCC in eine Integerzahl wandeln
            Dim GivenBCC As Integer = CType("&H" & BCCAsString, Byte)

            'we expect that ResponseData doesn't have STX/ETX but for calculation we take it into account...
            Dim CalculatedBCC As Integer = BuildBCCInteger(STX & PayloadWithoutBCC)

            'we must after after we fully calculated payload! 
            If (StripBlockCode) Then
                ResponseData = PayloadWithoutBCC
            End If

            'passt?
            If GivenBCC <> CalculatedBCC Then
                Trace.TraceError("Block code check failed for received data from SPS!")
                Return -1
            End If
            Return 1

        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Function

#Region "Logging"

    Private Sub WriteLog(ByVal EntryType As TraceEventType, ByVal LogEntry As String)
        'If Not _MyTraceHandler Is Nothing Then
        '    _MyTraceHandler.WriteLog(EntryType, 0, LogEntry)
        'End If
        'Trace.TraceInformation(LogEntry)
    End Sub

#End Region

#End Region

#Region "Öffentliche Methoden"

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

            Dim IndexOfSTX As Integer = -1
            Dim IndexOfETX As Integer = -1

            ReceiveBuffer = CType(Arguments(0), String)
            WriteLog(TraceEventType.Information, "ThermoProtocol_STX, IsReceiveReady, Receivebuffer: " & ReceiveBuffer)
            IndexOfSTX = ReceiveBuffer.LastIndexOf(STX)
            IndexOfETX = ReceiveBuffer.LastIndexOf(ETX)

            WriteLog(TraceEventType.Information, "ThermoProtocol_STX, IsReceiveReady: " & "Bel: " & IndexOfSTX.ToString & " Etx: " & IndexOfETX.ToString)

            If (IndexOfSTX <> -1) And (IndexOfETX <> -1) Then
                Dim LengthPayloadWithBC As Integer = IndexOfETX - IndexOfSTX + 1
                If LengthPayloadWithBC > 0 Then
                    ReceiveBuffer = ReceiveBuffer.Substring(IndexOfSTX, LengthPayloadWithBC)
                    WriteLog(TraceEventType.Information, "ThermoProtocol_STX, IsReceiveReady, Receivebuffer2: " & ReceiveBuffer)



                    ReceiveBuffer = ReceiveBuffer.Replace(STX, "")
                    Dim IdxETX = ReceiveBuffer.IndexOf(ETX)
                    'there is a bug in the STX/ETX reply from the SPS where BC-code AND additional ETX is send multiple times AFTER 
                    'the first ETX, therefore we use indexof
                    ReceiveBuffer = ReceiveBuffer.Substring(0, IdxETX)

                    'Doesn't work as SPS sends rubbish AFTER ETX was already sent: 

                    'only set final AnswerAsString in case blockcode-comparison was o.k.
                    If ValidateSPSBlockCode(ReceiveBuffer, True) = 1 Then
                        StripPreambelAndPostambel(ReceiveBuffer)
                        DataContainer.AnswerAsString = ReceiveBuffer
                        DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                        WriteLog(TraceEventType.Information, "ThermoProtocol_STX, IsReceiveReady, ReceiveBuffer: " & ReceiveBuffer)
                        Return 1
                    Else 'fehler in der checksumme
                        Return -2
                    End If
                Else 'Länge zwischen den Indizes ist 0
                    Return -1
                End If
            Else 'kein STX oder ETX, aber vielleicht ACK oder NAK oder ETB?

                Dim IndexOfACK As Integer = ReceiveBuffer.LastIndexOf(ACK)
                Dim IndexOfNAK As Integer = ReceiveBuffer.LastIndexOf(NAK)
                Dim IndexOfETB As Integer = ReceiveBuffer.LastIndexOf(ETB)

                If (IndexOfACK <> -1) Then 'ACK empfangen
                    DataContainer.AnswerAsString = "ACK"
                    DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                    Return 1
                ElseIf (IndexOfNAK <> -1) Then 'NAK empfangen
                    DataContainer.AnswerAsString = "NAK"
                    DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                    Return -4
                ElseIf (IndexOfETB <> -1) Then 'ETB empfangen
                    DataContainer.AnswerAsString = "ETB"
                    DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                    Return -3
                Else 'nix ordentliches
                    Return -1
                End If
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

        If (ReceiveBuffer.StartsWith(PreambelReplyDigitalValues)) Then
            ReceiveBuffer = ReceiveBuffer.TrimStart(PreambelReplyDigitalValues.ToCharArray)
        ElseIf (ReceiveBuffer.StartsWith(PreambelReplyAnalogValues)) Then
            ReceiveBuffer = ReceiveBuffer.TrimStart(PreambelReplyAnalogValues.ToCharArray)
        End If

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
            NewCommand = NewCommand & BCC & ETX
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

#End Region

End Class


