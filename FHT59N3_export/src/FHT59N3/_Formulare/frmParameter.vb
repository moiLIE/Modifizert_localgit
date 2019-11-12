Imports Thermo.Rmp.CM.Controls

Public Class frmParameter

    Private Sub frmParameter_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'PropGridParameter.ViewForeColor = Color.FromArgb(1, 1, 1)
            _MyFHT59N3ParCopy = _MyFHT59N3Par.CopyMe
            PropGridParameter.SelectedObject = _MyFHT59N3ParCopy
            PropGridParameter.PropertySort = PropertySort.Categorized
           

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAccept.Click
        Try
            Dim ret As MsgBoxResult
            ret = GUI_ShowMessageBox(ml_string(308, "Do you really want to save these settings?"), ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)
            If ret = MsgBoxResult.Yes Then
                _MyFHT59N3Par = _MyFHT59N3ParCopy.CopyMe
                SYS_WriteSettings()
                SYS_CheckIfLanguageChanged()
                'in DataFunctions...
                SYS_SetDerivedWorkParamsFromConfig()
                'in ControlFunctions...
                SYS_SetDerivedSPSParamsFromConfig()

                SYS_SynchronizeNextFilterStepTime(MinutesOfDayNow(), "Parameterchange")
                SYS_SynchronizeNextDayStart()
                Me.Close()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmParameter_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmParameter))
    resources.ApplyResources(Me.BtnAccept, "BtnAccept")
    resources.ApplyResources(Me.BtnClose, "BtnClose")
        resources.ApplyResources(Me, "$this")
  End Sub

    Private Sub BtnShowNuclideLibrary_Click(sender As System.Object, e As System.EventArgs) Handles BtnShowNuclideLibrary.Click

        Dim frm As New frmNuclideLibrary
        frm.ShowDialog(Me)

    End Sub
End Class