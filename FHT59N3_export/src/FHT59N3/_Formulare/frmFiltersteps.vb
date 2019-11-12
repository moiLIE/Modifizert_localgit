Imports Thermo.Rmp.CM.Controls

Public Class frmFiltersteps

    Private Sub frmCalibrationMeasControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            _MyFHT59N3ParCopy = _MyFHT59N3Par.CopyMe
            TBox_Filtersteps.Text = _MyFHT59N3ParCopy.FilterSteps.ToString
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAccept.Click
        Try
            If IsNumeric(TBox_Filtersteps.Text) Then
                _MyFHT59N3ParCopy.FilterSteps = CType(TBox_Filtersteps.Text, Integer)
                _MyFHT59N3Par = _MyFHT59N3ParCopy.CopyMe
                SYS_WriteSettings()
                Me.Close()
            Else
                GUI_ShowMessageBox(ml_string(303, "Please type a correct numerical value in."), "OK", "", "", MYCOL_THERMOGREEN, Color.White)
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

    Private Sub TBox_Filtersteps_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_Filtersteps.Click
    End Sub


    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmCalibrationMeasControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFiltersteps))
        resources.ApplyResources(Me.BtnAccept, "BtnAccept")
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        resources.ApplyResources(Me.Label1, "Label1")
        resources.ApplyResources(Me, "$this")
    End Sub

    Private Sub frmFiltersteps_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub
End Class