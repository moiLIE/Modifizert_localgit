Public Class frmMsgBox

    Public MsgBoxReturnCode As Integer

    Protected Sub frmMsgBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Text = My.Application.Info.Title
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnCancel.Click
        Try
            MsgBoxReturnCode = MsgBoxResult.Cancel
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Protected Sub BtnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnOK.Click
        Try
            MsgBoxReturnCode = MsgBoxResult.Yes
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Protected Sub BtnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNo.Click
        Try
            MsgBoxReturnCode = MsgBoxResult.No
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Constructor for class frmMsgBox
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmMsgBox_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsgBox))
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        resources.ApplyResources(Me.BtnNo, "BtnNo")
        resources.ApplyResources(Me.BtnOK, "BtnOK")
    End Sub
End Class