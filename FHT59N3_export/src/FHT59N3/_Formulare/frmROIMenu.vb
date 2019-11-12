Public Class frmROIMenu

    Private Sub BtnNextROI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNextROI.Click
        Try
            GUI_NextROI()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnPrevROI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPrevROI.Click
        Try
            GUI_PrevROI()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAddROI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAddROI.Click
        Try
            GUI_AddROI()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnDeleteROI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDeleteROI.Click
        Try
            GUI_DeleteROI()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnDeleteAllROIs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDeleteAllROIs.Click
        Try
            GUI_DeleteAllROIs()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnSaveROIs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSaveROIs.Click
        Try
            GUI_SaveROIs()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnLoadROIs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLoadROIs.Click
        Try
            GUI_LoadROIs()
            Me.Close()
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

  Private Sub frmROIMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmROIMenu))
    resources.ApplyResources(Me.BtnAddROI, "BtnAddROI")
    resources.ApplyResources(Me.BtnDeleteAllROIs, "BtnDeleteAllROIs")
    resources.ApplyResources(Me.BtnDeleteROI, "BtnDeleteROI")
    resources.ApplyResources(Me.BtnLoadROIs, "BtnLoadROIs")
    resources.ApplyResources(Me.BtnNextROI, "BtnNextROI")
    resources.ApplyResources(Me.BtnPrevROI, "BtnPrevROI")
    resources.ApplyResources(Me.BtnReturn, "BtnReturn")
    resources.ApplyResources(Me.BtnSaveROIs, "BtnSaveROIs")
    resources.ApplyResources(Me, "$this")
  End Sub
End Class