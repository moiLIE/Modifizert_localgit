Public Class frmOperationMenu



    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmOperationMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmOperationMenu_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        SetAlarmButtonGuiStates(_MyControlCenter.SYS_States.AlarmMode)

    End Sub

    Private Sub frmOperationMenu_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        'Größe reduzieren wenn Alarmmodus und/oder Intensivmodus nicht freigeschalten worden sind...

        'das FlowLayoutPanel berechnet seine Höhe anhand der Kinder wenn folgende Bedingungen
        'erfüllt sind:
        '- Die Kinder (normallerweise Panels) müssen auf AutoSize stehen
        '- Das FlowLayoutPanel muss auf AutoSize stehen und die FlowDirection von LeftToRight (nicht TopBottom!)
        '- Das FlowLayoutPanel ist als Dock.Top im Form gesetzt
        flowLayout.PerformLayout()

        Dim newSize As Size = flowLayout.Size
        'newSize.Height = irgendwelche Berechnungen
        ClientSize = newSize
    End Sub


    Private Sub BtnIntensOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnIntensOn.Click
        Try
            GUI_CloseAllMenus()
            SYS_SetIntensiveModeOn()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnIntensOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnIntensOff.Click
        Try
            GUI_CloseAllMenus()
            SYS_SetIntensiveModeOff()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAlarmOutOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAlarmOutOn.Click
        Try
            GUI_CloseAllMenus()

            SYS_SetAlarmModeOn() 'Controls GUI (alarm in frmMain), alarm flag and the SPS

            SetAlarmButtonGuiStates(True)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAlarmOutOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAlarmOutOff.Click
        Try
            GUI_CloseAllMenus()
            SYS_SetAlarmModeOff() 'Controls GUI (alarm in frmMain), alarm flag and the SPS

            SetAlarmButtonGuiStates(False)
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

    Private Sub BtnExitProg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExitProg.Click
        Try
            GUI_CloseAllMenus()
            _MenuEntryExitClicked = True
            frmMain.Close()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOperationMenu))
        resources.ApplyResources(Me.BtnAlarmOutOff, "BtnAlarmOutOff")
        resources.ApplyResources(Me.BtnAlarmOutOn, "BtnAlarmOutOn")
        resources.ApplyResources(Me.BtnExitProg, "BtnExitProg")
        resources.ApplyResources(Me.BtnIntensOff, "BtnIntensOff")
        resources.ApplyResources(Me.BtnIntensOn, "BtnIntensOn")
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        resources.ApplyResources(Me, "$this")
    End Sub

    Private Sub SetAlarmButtonGuiStates(ByVal alarmIsPresent As Boolean)

        If alarmIsPresent Then
            BtnAlarmOutOn.Enabled = False
            BtnAlarmOutOn.BackColor = Color.Gray
            BtnAlarmOutOff.Enabled = True
            BtnAlarmOutOff.BackColor = SystemColors.ButtonFace
        Else
            BtnAlarmOutOn.Enabled = True
            BtnAlarmOutOn.BackColor = SystemColors.ButtonFace
            BtnAlarmOutOff.Enabled = False
            BtnAlarmOutOff.BackColor = Color.Gray
        End If

        Me.Update()
    End Sub

  
End Class