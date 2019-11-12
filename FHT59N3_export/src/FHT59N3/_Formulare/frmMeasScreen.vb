Public Class frmMeasScreen

    Public _FormLoaded As Boolean = False

    Public Sub frmMeasScreen_Load()
        Try
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub frmMeasScreen_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            If Not e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
                Me.Visible = False
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub PanelfrmMeasScreen_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles PanelfrmMeasScreen.Resize, PanelfrmMeasScreen.SizeChanged
        Try
            _InstanceOfFormMeasscreen = Me
            GUI_ResizeMeasScreen()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub RtbAnalyzeData_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RtbAnalyzeData.Enter
        Try
            Me.SpectralDisplay.Focus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub ListView2_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView2.Enter
        Try
            Me.SpectralDisplay.Focus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmMeasScreen_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub


  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMeasScreen))
    resources.ApplyResources(Me.Label1, "Label1")
    resources.ApplyResources(Me.Label2, "Label2")
    resources.ApplyResources(Me.Label3, "Label3")
  End Sub

    Private Sub DeleteMessageListToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DeleteMessageListToolStripMenuItem.Click

        Dim ret As MsgBoxResult = GUI_ShowMessageBox(ml_string(618, "Delete messages? The system messages are also saved in the 'info collection' file."), ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)
        If ret = MsgBoxResult.Yes Then
            MsgListStatus.Clear()
            ListView1.Items.Clear()
        End If
    End Sub
End Class