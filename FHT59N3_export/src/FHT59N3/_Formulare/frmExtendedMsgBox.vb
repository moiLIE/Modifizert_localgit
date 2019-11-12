Public Class frmExtendedMsgBox

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
        Me.ml_UpdateExtendedControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateExtendedControls
    End Sub

    Private Sub frmExtendedMsgBox_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateExtendedControls
    End Sub

    Private Sub ml_UpdateExtendedControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExtendedMsgBox))

        resources.ApplyResources(Me.CheckBox1, "CheckBox1")
        resources.ApplyResources(Me.CheckBox2, "CheckBox2")
    End Sub

End Class
