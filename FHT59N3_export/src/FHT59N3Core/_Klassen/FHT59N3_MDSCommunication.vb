Public Class FHT59N3_MDSCommunication

#Region "Private Eigenschfaten"

    Private WithEvents _MyDataServer As New ThermoActivexWrapper.ThermoMonitorDataServer_Wrapper(128) 'Verbindung zum Netview
    Private _NetViewLogin As Boolean = False
    Private _NetLogProc As Diagnostics.Process

#End Region

#Region "Öffentliche Eigenschaften"

    Public ReadOnly Property NetViewLogin() As Boolean
        Get
            Return _NetViewLogin
        End Get
    End Property

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Wenn Netlog nicht läuft wird es neu gestartet
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartOrRestartNetlog(ByVal NetViewPath As String, ByVal NetViewActive As Boolean) As String
        Dim newProcStartInfo As New Diagnostics.ProcessStartInfo
        Try
            newProcStartInfo.FileName = NetViewPath & "\NetLog32.exe"


            If NetViewActive Then
                'wenn keine Parameter mitgegeben wird NetLog mit der Lizenzierung von NetView gestartet
                newProcStartInfo.Arguments = ""
            Else
                'starte NetLog32 in limitierten (lokalen) Modus...
                newProcStartInfo.Arguments = "MON8000"
            End If

            ' NetLog32.exe doesnt't understand newProcStartInfo.WindowStyle = ProcessWindowStyle.Hidden
            newProcStartInfo.WindowStyle = ProcessWindowStyle.Minimized

            newProcStartInfo.UseShellExecute = True

            If IO.File.Exists(newProcStartInfo.FileName) Then
                _NetLogProc = Diagnostics.Process.Start(newProcStartInfo)

                _NetViewLogin = True
                Return ""
            Else
                Return "File not found: " + newProcStartInfo.FileName
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return "could not start " + newProcStartInfo.FileName
        End Try
    End Function

    Public Sub StopNetlog()
        Try
            _NetViewLogin = False

            If Not _NetLogProc Is Nothing Then
                If Not _NetLogProc.HasExited Then
                    _NetLogProc.Kill()
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Daten ans Netview schicken
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveAnalyzationResultsToDataServer(ByVal MCANuclides As FHT59N3MCA_NuclideList, ByVal MyFHT59N3States As FHT59N3_SystemStates, ByVal AirFlowMean As Single, ByVal SpecType As Integer, Optional ByVal DeleteValues As Boolean = False)
        Try
            Dim Conc As Double
            If _NetViewLogin Then

                'Konzentrationen schreiben, oder löschen
                For n As Integer = 1 To MCANuclides.NuclideCount
                    If Not DeleteValues Then

                        Dim nuclide As FHT59N3MCA_Nuclide = MCANuclides.GetNuclide(n)

                        If nuclide.SpectrumAnalysis.Concentration_Bqm3 > nuclide.SpectrumAnalysis.DetectionLimit_Bqm3 Then
                            Conc = nuclide.SpectrumAnalysis.Concentration_Bqm3
                        Else
                            Conc = 0
                        End If
                        _MyDataServer.SetChannel((n + (SpecType - 2) * 64) - 1, Conc, MyFHT59N3States.SumState, 0)
                        Trace.TraceInformation("_MyDataServer.SetChannel: " & "Ch: " & ((n + (SpecType - 2) * 64) - 1).ToString & " Value: " & Format(Conc, "0.0000") & " SumState: " & MyFHT59N3States.SumState.ToString)
                    Else
                        _MyDataServer.SetChannel(n - 1, 0, 0, 0)
                        _MyDataServer.SetChannel((n + 64) - 1, 0, 0, 0)
                    End If
                Next
                'Luftdurchsatz schreiben oder löschen
                If Not DeleteValues Then
                    _MyDataServer.SetChannel(MCANuclides.NuclideCount + (SpecType - 2) * 64 - 1, AirFlowMean, MyFHT59N3States.SumState, 0)
                    Trace.TraceInformation("_MyDataServer.SetChannel: " & "Ch: " & (MCANuclides.NuclideCount + (SpecType - 2) * 64 - 1).ToString & " AirFlowMean: " & Format(AirFlowMean, "0.0000") & " SumState: " & MyFHT59N3States.SumState.ToString)
                Else
                    _MyDataServer.SetChannel(MCANuclides.NuclideCount - 1, 0, 0, 0)
                    _MyDataServer.SetChannel(MCANuclides.NuclideCount + 64 - 1, 0, 0, 0)
                End If
                _MyDataServer.WriteValuesToDataServer()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

End Class
