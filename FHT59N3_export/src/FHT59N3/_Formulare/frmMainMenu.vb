Public Class frmMainMenu

    Private Sub BtnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturn.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnOperations_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOperations.Click
    End Sub

    Private Sub BtnAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAbout.Click
    End Sub

    Private Sub BtnMaintenance_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMaintenance.Click
    End Sub

    Private Sub BtnFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFile.Click
    End Sub

  Protected Overridable Sub ml_UpdateControls()
    Me.Text = ml_string ( 64, "FHT59N3 - Main menu" )
    BtnAbout.Text = ml_string ( 67, "About" )
    BtnFile.Text = ml_string ( 68, "File" )
    BtnMaintenance.Text = ml_string ( 65, "Maintenance" )
    BtnOperations.Text = ml_string ( 66, "Operations" )
    BtnReturn.Text = ml_string ( 37, "Return" )
  End Sub

  Public Sub New
    InitializeComponent()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmMainMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
End Class