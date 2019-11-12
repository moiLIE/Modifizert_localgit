Imports Thermo.Rmp.CM.Controls

Public Class frmMCAParameter_HV

    Dim _LynxParCopy As FHT59N3Core.FHT59N3_LynxParams

    Private Sub frmMCAParameter_HV_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            _LynxParCopy = CType(_MyControlCenter.MCA_Params, FHT59N3Core.FHT59N3_LynxParams).CopyMe
            CBox_HVRange.Items.Clear()
            For Each HVr As String In _LynxParCopy.HVRange.Keys
                CBox_HVRange.Items.Add(HVr)
            Next
            CBox_HVRange.Text = _LynxParCopy.HV_Range.ToString
            CBox_HVPolarity.Items.Clear()
            CBox_HVInhibitPolarity.Items.Clear()
            For Each HVp As String In _LynxParCopy.Polarity.Keys
                CBox_HVPolarity.Items.Add(HVp)
                CBox_HVInhibitPolarity.Items.Add(HVp)
            Next
            For Each HVp As String In _LynxParCopy.Polarity.Keys
                If _LynxParCopy.HV_DetectorPolarity = _LynxParCopy.Polarity(HVp) Then
                    CBox_HVPolarity.Text = HVp
                End If
                If _LynxParCopy.HV_InhibitPolarity = _LynxParCopy.Polarity(HVp) Then
                    CBox_HVInhibitPolarity.Text = HVp
                End If
            Next
            HSB_HVVoltage.Maximum = _LynxParCopy.HV_Limit
            HSB_HVVoltage.Value = _LynxParCopy.HV_Voltage
            Lbl_HVVoltage.Text = HSB_HVVoltage.Value.ToString
            TBox_HVLimit.Text = _LynxParCopy.HV_Limit.ToString
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TBox_HVLimit_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_HVLimit.TextChanged
        Try
            If IsNumeric(TBox_HVLimit.Text) And IsNumeric(CBox_HVRange.Text) Then
                If CType(TBox_HVLimit.Text, Integer) > CType(CBox_HVRange.Text, Integer) Then
                    TBox_HVLimit.Text = CBox_HVRange.Text
                End If
                Label7.Text = TBox_HVLimit.Text
                HSB_HVVoltage.Maximum = CType(TBox_HVLimit.Text, Integer)
                Lbl_HVVoltage.Text = HSB_HVVoltage.Value.ToString
            Else
                TBox_HVLimit.Text = CBox_HVRange.Text
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_HVRange_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_HVRange.SelectedIndexChanged
        Try
            If IsNumeric(TBox_HVLimit.Text) And IsNumeric(CBox_HVRange.Text) Then
                If CType(TBox_HVLimit.Text, Integer) > CType(CBox_HVRange.Text, Integer) Then
                    TBox_HVLimit.Text = CBox_HVRange.Text
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_HVRange_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_HVRange.Leave
        Try
            If Not CBox_HVRange.Items.Contains(CBox_HVRange.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_HVRange.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_HVPolarity_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_HVPolarity.Leave
        Try
            If Not CBox_HVPolarity.Items.Contains(CBox_HVPolarity.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_HVPolarity.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_HVInhibitPolarity_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_HVInhibitPolarity.Leave
        Try
            If Not CBox_HVInhibitPolarity.Items.Contains(CBox_HVInhibitPolarity.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_HVInhibitPolarity.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub HSB_HVVoltage_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles HSB_HVVoltage.Scroll
        Try
            Lbl_HVVoltage.Text = HSB_HVVoltage.Value.ToString
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAccept.Click
        Try
            Dim ret As MsgBoxResult
            ret = GUI_ShowMessageBox(ml_string(308, "Do you really want to save these settings?"), ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)
            If ret = MsgBoxResult.Yes Then
                _LynxParCopy.HV_Range = CType(CBox_HVRange.Text, Integer)
                _LynxParCopy.HV_Limit = CType(TBox_HVLimit.Text, Integer)
                _LynxParCopy.HV_Voltage = CType(Lbl_HVVoltage.Text, Integer)
                _LynxParCopy.HV_DetectorPolarity = _LynxParCopy.Polarity(CBox_HVPolarity.Text)
                _LynxParCopy.HV_InhibitPolarity = _LynxParCopy.Polarity(CBox_HVInhibitPolarity.Text)
                _MyControlCenter.MCA_Params = _LynxParCopy.CopyMe
                CType(_MyControlCenter.MCA_Params, FHT59N3Core.FHT59N3_LynxParams).SaveMeBinary(_MonitorConfigDirectory & "\")
                _MyControlCenter.MCA_SetAllMeasParams()
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

    Private Sub TBox_HVLimit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_HVLimit.Click
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmMCAParameter_HV_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMCAParameter_HV))
    resources.ApplyResources(Me.BtnAccept, "BtnAccept")
    resources.ApplyResources(Me.BtnClose, "BtnClose")
    resources.ApplyResources(Me.CBox_HVInhibitPolarity, "CBox_HVInhibitPolarity")
    resources.ApplyResources(Me.CBox_HVPolarity, "CBox_HVPolarity")
    resources.ApplyResources(Me, "$this")
    resources.ApplyResources(Me.GroupBox1, "GroupBox1")
    resources.ApplyResources(Me.Label1, "Label1")
    resources.ApplyResources(Me.Label2, "Label2")
    resources.ApplyResources(Me.Label3, "Label3")
    resources.ApplyResources(Me.Label4, "Label4")
    resources.ApplyResources(Me.Label5, "Label5")
  End Sub
End Class