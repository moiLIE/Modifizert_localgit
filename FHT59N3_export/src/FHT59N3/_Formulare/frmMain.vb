Imports FHT59N3Core
Imports Thermo.Rmp.CM.Controls
Imports ThermoUtilities

Public Class frmMain

    Private _OriginMinimumSize As Size

    'determines whether any issues occured while storing temperatures to history. Only set when 
    'writing fails to prevent a message box on every error occurence
    Private m_writingToFilePossible = True

    'Deklarieren der Klasse in einer Form
    Public WithEvents _HotKey As New Thermo_GlobalHotkeys(Me)

    Public Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try

            If _ForcedEndProgram Then
                Exit Sub
            End If

            If (e.CloseReason = CloseReason.UserClosing) And (Not _MenuEntryExitClicked) Then
                e.Cancel = True
                Exit Sub
            End If
            'Nur ausführen wenn ich mich nicht gerade in der Startroutine befinde und dort abbrechen weil
            'schon eine Instanz läuft
            If (Not _Start) And (Not _EndProgram) Then
                Dim checkBoxResult1 As Boolean = _MyFHT59N3Par.KeepActiveHighVoltageOnExitGuiFlag
                Dim checkBoxResult2 As Boolean = _MyFHT59N3Par.KeepActiveEcoolerOnExitGuiFlag
                Dim dialogResult As MsgBoxResult = GUI_ShowExtendedMessageBox(MSG_WantToExit, ml_string(90, "Yes"), ml_string(91, "No"), "",
                                              ml_string(537, "Keep MCA high voltage turned on"), checkBoxResult1,
                                              ml_string(538, "Keep E-Cooler turned on"), checkBoxResult2,
                                              MYCOL_THERMOGREEN, Color.White)
                If dialogResult = MsgBoxResult.No Then
                    e.Cancel = True
                    _MenuEntryExitClicked = False
                Else
                    SYS_RemoveHotkeys()
                    SYS_ReleaseFilterDrucker()
                    SYS_StopSnmpCommunication()
                    SYS_StopN4242Cleanup()

                    GUI_SetMessage(MSG_MEASPROGCLOSED, MessageStates.GREEN)
                    _MyControlCenter.SYS_States.Maintenance = True

                    _MyFHT59N3Par.KeepActiveHighVoltageOnExitGuiFlag = checkBoxResult1
                    _MyFHT59N3Par.KeepActiveEcoolerOnExitGuiFlag = checkBoxResult2
                    SYS_WriteSettings() 'Einstellungen sichern

                    _MyControlCenter.MCA_StopMeasurement(False)
                    If Not checkBoxResult1 Then
                        _MyControlCenter.MCA_SetHVOff()
                    End If
                    _MyControlCenter.SPS_AlarmOff()
                    _MyControlCenter.SPS_ErrorOn()
                    _MyControlCenter.MDS_StopNetlog()
                    If Not checkBoxResult2 Then
                        _MyControlCenter.SPS_EcoolerOff()
                    End If
                    'Setzte die Entsprechende Flagge, damit sie ins Konfigurations-Binary geschrieben wird.
                    If Not checkBoxResult1 Then
                        _MyControlCenter.SYS_States.HVOff = True
                    End If
                    _EndProgram = True
                    ''Schreibe den allenfalls geänderten Ecooler/HV state ins Konfigurations-Binary
                    FHT59N3_ControlFunctions.SYS_SystemStateChangedHandler()
                    e.Cancel = True 'ich muss noch auf das setzen der ausgänge warten
                End If
            ElseIf _EndProgram Then
                Dim thisProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess
                thisProcess.Kill()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' workaround for reducing the minimum size of application during startup phase
            Me._OriginMinimumSize = Me.MinimumSize
            Me.MinimumSize = New Size(40, 60)
            Me.Height = 40
            Me.Width = 60
            Me.Update()

            GUI_ShowSplashScreen(True)

            _Start = True
            Dim currentDomain As AppDomain = AppDomain.CurrentDomain
            AddHandler currentDomain.UnhandledException, AddressOf MyErrorHandler
            AddHandler Application.ThreadException, AddressOf MyErrorHandler1
            If _MyProcesses.CheckForRunningInstanceOfMe Then 'wenn schon eine Instanz von mir läuft, dann beenden
                _MyProcesses.ShutdownMe()
            End If
            tmrStart.Enabled = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub tmrStart_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrStart.Tick
        Try
            tmrStart.Enabled = False

            'Wichtig, weil ansonsten der Splashscreen während der Initialisierung hinten liegt
            Me.SendToBack()

            SYS_InitializeProgramFunctions()

            ' finish startup phase
            _Start = False
            Me.MinimumSize = _OriginMinimumSize
            GUI_ShowSplashScreen(False)
            tmrSignals.Enabled = True
            tmrTrigger.Enabled = True

            If (Not _MyTemperatureRecorder Is Nothing) Then
                _MyTemperatureRecorder = New FHT59N3_DetectorTemperatureRecorder(_MyFHT59N3Par.Customer, _MyFHT59N3Par.StationName)
            End If
            _MyTemperatureRecorder.Initialize(_MyFHT59N3Par.SensorRecordingDirectory)
            tmrTemperatureRecording.Interval = 1000
            tmrTemperatureRecording.Enabled = True

            If _MyControlCenter.SYS_States.AlarmMode Then
                UcStatusSideBar.BtnQuitAlarm.Visible = True
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrSignals_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrSignals.Tick
        Try
            SYS_MonitorControl()
            GUI_ShowMonitorInformation()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrTrigger_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrTrigger.Tick
        Try
            _MyControlCenter.TriggerStatemachines()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnOperation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOperation.Click
        Try
            GUI_FillBackColors()
            frmOperationMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFile.Click
        Try
            GUI_FillBackColors()
            frmFileMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnMaintenance_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMaintenance.Click
        Try
            GUI_FillBackColors()
            frmMaintenanceMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAbout.Click
        Try
            GUI_FillBackColors()
            frmAbout.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnSpectrum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSpectrum.Click
        Try
            If _ShowSpectrum Then
                GUI_ShowForm(frmMeasScreen.PanelfrmMeasScreen)
                GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)
                BtnSpectrum.Text = ml_string(414, "Service")
            Else
                GUI_ShowForm(frmServiceScreen.PanelfrmServiceScreen)
                BtnSpectrum.Text = ml_string(105, "Actual Spectrum")
            End If
            _ShowSpectrum = Not _ShowSpectrum
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnStates_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            _MonitorStatesClicked = True
            frmStates.FillFormStates()
            frmStates.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

  

    Private Sub BtnExpandMode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExpandMode.Click
        Try
            If Not frmMeasScreen.SpectralDisplay.ExpandMode Then
                frmMeasScreen.SpectralDisplay.ExpandMode = True
            Else
                frmMeasScreen.SpectralDisplay.ExpandMode = False
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnResult_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnResult.Click
        Try
            If Not _ShowInfo Then
                _ShowInfo = True
                frmMeasScreen.SpectralDisplay.ShowInfo = _ShowInfo
                frmMeasScreen.RtbAnalyzeData.Visible = False
                BtnResult.Text = ml_string(307, "Show result")
            Else
                _ShowInfo = False
                frmMeasScreen.SpectralDisplay.ShowInfo = _ShowInfo
                frmMeasScreen.RtbAnalyzeData.Visible = True
                BtnResult.Text = ml_string(101, "Show info")
            End If
            GUI_ResizeMeasScreen()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

 

    Private Sub BtnDetectorTemperature_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDetectorTemperature.Click
        GUI_FillBackColors()
        frmDetectorTemperature.ShowDialog()
        frmDetectorTemperature.Dispose()

    End Sub

    Private Sub MyErrorHandler(ByVal sender As Object, ByVal e As System.UnhandledExceptionEventArgs)
        Trace.TraceError(e.ExceptionObject.ToString)
    End Sub

    Private Sub MyErrorHandler1(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        Trace.TraceError(e.Exception.Message & " " & e.Exception.StackTrace)
    End Sub

    'Eingang des Hotkey-Events abfragen
    Private Sub HotKeyPressed(ByVal HotKeyID As String) Handles _HotKey.HotKeyPressed
        Try
            SYS_ReceiveHotKey(HotKeyID)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmMain_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        If _MyFHT59N3Par.EnableCapturingDetectorTemperature Then
            Me.BtnDetectorTemperature.Show()
        Else
            Me.BtnDetectorTemperature.Hide()
        End If
    End Sub

    Private Sub tmrTemperatureRecording_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrTemperatureRecording.Tick
        If tmrTemperatureRecording.Interval = 1000 Then
            If _MyFHT59N3Par.CaptureCycleDetectorTemperature Mod 5 = 0 Then
                tmrTemperatureRecording.Interval = _MyTemperatureRecorder.GetSecondsUntilNextDetection() * 1000
            Else
                tmrTemperatureRecording.Interval = 60 * _MyFHT59N3Par.CaptureCycleDetectorTemperature * 1000
            End If
            Return
        End If
        Dim result As Boolean
        result = _MyTemperatureRecorder.StoreNewTempertaureEntry(_DetectorTemperaturValue)

        If result = False Then
            If m_writingToFilePossible = True Then
                GUI_ShowMessageBox(ml_string(577, "Storing detector temperature failed."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                m_writingToFilePossible = False
            End If
        Else
            m_writingToFilePossible = True
        End If
        If _MyFHT59N3Par.CaptureCycleDetectorTemperature Mod 5 = 0 Then
            tmrTemperatureRecording.Interval = (60 * (_MyFHT59N3Par.CaptureCycleDetectorTemperature - 5) + _MyTemperatureRecorder.GetSecondsUntilNextDetection()) * 1000
        Else
            tmrTemperatureRecording.Interval = 60 * _MyFHT59N3Par.CaptureCycleDetectorTemperature * 1000
        End If
    End Sub
End Class