Public Class frmMCAMenu

    Private Sub BtnHV_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHV.Click
        Try
            GUI_CloseAllMenus()
            frmMCAParameter_HV.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnGainStabilizer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGainStabilizer.Click
        Try
            GUI_CloseAllMenus()
            Dim F As New frmMCAParameter_GainStabiAdd '.ShowDialog()
            F.BackColor = MYCOL_THERMOGREEN
            F.ShowDialog()
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


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmMCAMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMCAMenu))
    resources.ApplyResources(Me.BtnGainStabilizer, "BtnGainStabilizer")
    resources.ApplyResources(Me.BtnHV, "BtnHV")
    resources.ApplyResources(Me.BtnReturn, "BtnReturn")
    resources.ApplyResources(Me, "$this")
  End Sub
End Class