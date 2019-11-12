#Region "Imports"
Imports ThermoInterfaces
Imports ThermoLogging
#End Region

Public Class ThermoProtocol_SimaticSPS_FHT59N3
    Implements ThermoProtocol_Interface


#Region "Private Eigenschaften"

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

    ''' <summary>
    ''' Event das irgendetwas schiefgelaufen ist
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("Bitte ThermoAspekte.ThermoAspekt_TraceAttributeOnInvocation benutzen!")> Public Event ErrorEvent(ByVal ex As Exception) Implements ThermoProtocol_Interface.ErrorEvent

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
    ''' spezielle Kommandos zusammenbauen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitProtocolParameters()
        Try
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub New()
        InitProtocolParameters()
    End Sub

    ''' <summary>
    ''' Überprüfung ob die Empfangenen Daten korrekt und vollständig im Sinne des Protokolls sind
    ''' </summary>
    ''' <param name="DataContainer"></param> 
    ''' <param name="Arguments">
    ''' Arg(0) = ReceiveBuffer As String
    ''' </param> 
    ''' <returns>
    ''' 1 = ok
    ''' -1 = not complete
    ''' </returns>
    ''' <remarks></remarks>
    Public Function IsReceiveReady(ByVal DataContainer As ThermoDataContainer_Interface, ByVal ParamArray Arguments() As Object) As Integer Implements ThermoInterfaces.ThermoProtocol_Interface.IsReceiveReady
        Try
            Dim ReceiveBuffer As String = CType(Arguments(0), String)
            'lets check whether we have to strip preambel from reply and the ending line-feed character... 
            Dim ReplyStripResult = StripPreambelAndPostambel(DataContainer, ReceiveBuffer)
            Return ReplyStripResult
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Checks whether a preambel has to be stripped away from the reply answer. Furthermore the ending line-feed is removed
    ''' </summary>
    ''' <param name="DataContainer">data container which is updated in case we needed to strip preambel</param>
    ''' <param name="ReceiveBuffer">the receive buffer from the SPS</param>
    ''' <returns>negative value in case of an error, zero or positive value on success or if no stripping was necessary</returns>
    ''' <remarks></remarks>
    Private Function StripPreambelAndPostambel(ByVal DataContainer As ThermoDataContainer_Interface, ByVal ReceiveBuffer As String)
        Dim IndexOfCr As Integer
        Dim Cr As String = vbCr 'SPS beendet jede Kommunikation mit Cr

        Dim PreambelReplyDigitalValues As String = "E"
        Dim PreambelReplyAnalogValues As String = "F"
        Dim PreambelReplyCalculatedValues As String = "G"

        Dim WhichPreambel As Integer = 0
        Dim PreambelOk As Boolean   'irgendeine gültige Prambel empfangen
        Dim startindex As Integer = -1

        IndexOfCr = ReceiveBuffer.IndexOf(Cr)
        PreambelOk = False

        If ReceiveBuffer.IndexOf(PreambelReplyDigitalValues) <> -1 Then
            PreambelOk = True
            startindex = ReceiveBuffer.IndexOf(PreambelReplyDigitalValues)
            WhichPreambel = 1
        ElseIf ReceiveBuffer.IndexOf(PreambelReplyAnalogValues) <> -1 Then
            PreambelOk = True
            startindex = ReceiveBuffer.IndexOf(PreambelReplyAnalogValues)
            WhichPreambel = 2
        ElseIf ReceiveBuffer.IndexOf(PreambelReplyCalculatedValues) <> -1 Then
            PreambelOk = True
            startindex = ReceiveBuffer.IndexOf(PreambelReplyCalculatedValues)
            WhichPreambel = 3
        End If

        If PreambelOk Then
            If IndexOfCr <> -1 Then
                'kein Protokollrahmen wird mitgeschickt!
                Dim TempReceive As String = ""
                TempReceive = ReceiveBuffer.Substring(startindex)

                If WhichPreambel = 1 Then
                    TempReceive = TempReceive.TrimStart(PreambelReplyDigitalValues.ToCharArray)
                ElseIf WhichPreambel = 2 Then
                    TempReceive = TempReceive.TrimStart(PreambelReplyAnalogValues.ToCharArray)
                Else
                    TempReceive = TempReceive.TrimStart(PreambelReplyCalculatedValues.ToCharArray)
                End If

                TempReceive = TempReceive.TrimEnd(Cr.ToCharArray)
                DataContainer.AnswerAsString = TempReceive
                DataContainer.AnswerLength = DataContainer.AnswerAsString.Length
                Return 1
            Else
                Return -1   'kein abschluss
            End If
        End If

        Return -1

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
            CommandString = CType(Arguments(0), String)
            If Arguments.Length = 2 Then
                ArgBuffer = CType(Arguments(1), String)
            End If
            If ArgBuffer <> "" Then
                NewCommand = CommandString & ArgBuffer & vbCr
            Else
                NewCommand = CommandString & vbCr
            End If
            DataContainer.Command = CommandString
            DataContainer.CommandAsString = NewCommand
            DataContainer.CommandLength = DataContainer.CommandAsString.Length
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region



End Class

