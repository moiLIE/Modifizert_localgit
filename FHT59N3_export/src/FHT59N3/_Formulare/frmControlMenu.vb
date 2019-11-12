Public Class frmControlMenu

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Try
            GUI_ShowMonitorStatesInControlMenu(False)
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Click on button "filter step" revoked: A request for a "normal" filterstep is sent to SPS
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnFilterstep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFilterstep.Click
        Dim resultStr As String
        Try
            _FiltertstepTries = 0
            resultStr = SPS_SetFilterstep()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnFilterBypass_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFilterBypass.Click
        Try
            SPS_SetBypass()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnPump_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPump.Click
        Try
            SPS_SetPump()
            'Trace.TraceInformation(": BtnPump_Click: Pump On / Off!")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnErrorIndication_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnErrorIndication.Click
        Try
            SPS_SetErrorIndication()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAlarm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAlarm.Click
        Try
            SPS_SetAlarm()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnN2Fill_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnN2Fill.Click
        Try
            SPS_SetHeating()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnHV_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHV.Click
        Try
            MCA_SetHighVoltage()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAirFlow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAirFlow.Click
        Try
            GUI_ShowMonitorStatesInControlMenu(True)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnEcoolerCtl_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEcoolerCtl.Click
        Try
            GUI_SafetyChangeEcoolerState()

            'deactivate button for about 5 seconds (expected feedback time from SPS)
            Me.BtnEcoolerCtl.Enabled = False
            Me.TimerForEcoolerButton.Enabled = True
            Me.TimerForEcoolerButton.Start()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls

        If _MyFHT59N3Par.EcoolerEnabled Then
            BtnN2Fill.Enabled = False
            LblN2Fill.Enabled = False
        End If
    End Sub

    Private Sub frmControlMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmControlMenu))
        resources.ApplyResources(Me.BtnAirFlow, "BtnAirFlow")
        resources.ApplyResources(Me.BtnAlarm, "BtnAlarm")
        resources.ApplyResources(Me.BtnErrorIndication, "BtnErrorIndication")
        resources.ApplyResources(Me.BtnFilterstep, "BtnFilterstep")
        resources.ApplyResources(Me.BtnHV, "BtnHV")
        resources.ApplyResources(Me.BtnEcoolerCtl, "BtnEcoolerCtl")
        resources.ApplyResources(Me.BtnN2Fill, "BtnN2Fill")
        resources.ApplyResources(Me.BtnPump, "BtnPump")
        resources.ApplyResources(Me.Button7, "Button7")
        resources.ApplyResources(Me, "$this")
        resources.ApplyResources(Me.LblAlarmIndication, "LblAlarmIndication")
        resources.ApplyResources(Me.LblErrorIndication, "LblErrorIndication")
        resources.ApplyResources(Me.LblFilterBypass, "LblFilterBypass")
        resources.ApplyResources(Me.LblFilterstep, "LblFilterstep")
        resources.ApplyResources(Me.LblHighVoltage, "LblHighVoltage")
        resources.ApplyResources(Me.LblEcoolerState, "LblEcoolerState")
        resources.ApplyResources(Me.LblN2Fill, "LblN2Fill")
        resources.ApplyResources(Me.LblPump, "LblPump")

        If _MyFHT59N3Par.EcoolerEnabled And _MyFHT59N3Par.EnableEmergencyStopDetect Then
            Me.BtnEcoolerCtl.Show()
            Me.BtnEcoolerCtl.Enabled = True
            Me.LblEcoolerState.Show()
        Else
            Me.BtnEcoolerCtl.Hide()
            Me.LblEcoolerState.Hide()
        End If
    End Sub

    Private Sub TimerForEcoolerButton_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerForEcoolerButton.Tick
        Me.TimerForEcoolerButton.Stop()
        Me.TimerForEcoolerButton.Enabled = False
        Me.BtnEcoolerCtl.Enabled = True
    End Sub
End Class