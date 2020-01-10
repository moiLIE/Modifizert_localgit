Public Class frmMaintenanceMenu

    Private Sub BtnMaintenanceOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMaintenanceOn.Click
        Try
            If _MyControlCenter.SYS_States.Maintenance Then
                SPS_SetMaintenanceOff()
            Else
                SPS_SetMaintenanceOn()
            End If
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnSysConfig_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSysConfig.Click
        Try
            GUI_CloseAllMenus()
            frmParameter.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnFilterTape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFilterTape.Click
        Try
            GUI_CloseAllMenus()
            frmFilterMenu.BackColor = MYCOL_THERMOGREEN
            frmFilterMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnMCAConfig_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMCAConfig.Click
        Try
            frmMCAMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnCalibration_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCalibration.Click
        Try
            frmCalibrationMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnControl_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnControl.Click
        Try
            GUI_CloseAllMenus()
            frmControlMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturn.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnEcooler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEcooler.Click
        Dim state As EcoolerGuiStates = GUI_SafetyChangeEcoolerState()

        'show the new state (the real state acknowledged from SPS can take some seconds)
        If (state = EcoolerGuiStates.ACTIVATED) Then
            Me.BtnEcooler.Image = Global.FHT59N3.My.Resources.Resources.cooling_on_32
        ElseIf (state = EcoolerGuiStates.DEACTIVATED) Then
            Me.BtnEcooler.Image = Global.FHT59N3.My.Resources.Resources.cooling_off_32
        End If

        'deactivate button for about 5 seconds (expected feedback time from SPS)
        Me.BtnEcooler.Enabled = False
        Me.TimerForEcoolerButton.Enabled = True
        Me.TimerForEcoolerButton.Start()

    End Sub

    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmMaintenanceMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMaintenanceMenu))
        resources.ApplyResources(Me.BtnCalibration, "BtnCalibration")
        resources.ApplyResources(Me.BtnControl, "BtnControl")
        resources.ApplyResources(Me.BtnFilterTape, "BtnFilterTape")
        resources.ApplyResources(Me.BtnMaintenanceOn, "BtnMaintenanceOn")
        resources.ApplyResources(Me.BtnEcooler, "BtnEcooler")
        resources.ApplyResources(Me.BtnMCAConfig, "BtnMCAConfig")
        resources.ApplyResources(Me.BtnSysConfig, "BtnSysConfig")
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        resources.ApplyResources(Me, "$this")
    End Sub

    Private Sub frmMaintenanceMenu_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If (_MyFHT59N3Par.EcoolerEnabled And _MyFHT59N3Par.EnableEmergencyStopDetect) Or _MyFHT59N3Par.IsCanberraDetector Then
            Me.BtnEcooler.Enabled = True
        Else
            Me.BtnEcooler.Enabled = False
        End If

        If _MyControlCenter.SYS_States.EcoolerOff Then
            'alarm shows that ecooler is off
            Me.BtnEcooler.Image = Global.FHT59N3.My.Resources.Resources.cooling_off_32
        Else
            Me.BtnEcooler.Image = Global.FHT59N3.My.Resources.Resources.cooling_on_32
        End If

    End Sub

    Private Sub TimerForEcoolerButton_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerForEcoolerButton.Tick
        Me.TimerForEcoolerButton.Stop()
        Me.TimerForEcoolerButton.Enabled = False
        Me.BtnEcooler.Enabled = True
    End Sub
End Class