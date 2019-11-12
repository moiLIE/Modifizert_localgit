Public Class frmCalibrationMenu


    Private Sub frmCalibrationMenu_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Falls wir kein Hintergrundspektrum verwenden brauchen wir auch nicht das Hintergrundspektrum aufnehmen
        If (Not _SubtractBkgSpectrum) Then
            'BtnBackgroundMeas.Visible = False
        End If

    End Sub

    Private Sub BtnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturn.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BTnMeasFar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTnMeasFar.Click
        Try
            If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                _CalibrationType = CalibTypes.CalibFar
            Else
                _CalibrationType = CalibTypes.CalibNearMix
            End If

            _StartCalibration = True
            GUI_CloseAllMenus()


        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub BtnRCC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnRCC.Click
        Try

			'alle Ger‰te ab 2.0.0 haben defaultm‰ﬂig die Mischstrahler-WKP...
			_CalibrationType = CalibTypes.CalibRccMix
            If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                _CalibrationType = CalibTypes.CalibRccCs137
            End If

            _StartCalibration = True
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnMeasFree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMeasFree.Click
        Try
            _CalibrationType = CalibTypes.CalibFree
            _StartCalibration = True
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnBackgroundMeas_Click(sender As System.Object, e As System.EventArgs) Handles BtnBackgroundMeas.Click
        Try
            _CalibrationType = CalibTypes.CalibBackground
            _StartCalibration = True
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAirFlow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAirFlow.Click
        Try
            frmAirFlow.ShowDialog()
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnEnergyFar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEnergyFar.Click
        Try
            _StartCalibAnalyzation = True
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStop.Click
        Try
            MCA_StopCalibrationMeasurement()
            GUI_CloseAllMenus()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmCalibrationMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalibrationMenu))
    resources.ApplyResources(Me.BtnAirFlow, "BtnAirFlow")
    resources.ApplyResources(Me.BtnEfficiencyFar, "BtnEfficiencyFar")
    resources.ApplyResources(Me.BtnEfficiencyNear, "BtnEfficiencyNear")
    resources.ApplyResources(Me.BtnEnergyFar, "BtnEnergyFar")
    resources.ApplyResources(Me.BTnMeasFar, "BTnMeasFar")
    resources.ApplyResources(Me.BtnMeasFree, "BtnMeasFree")
    resources.ApplyResources(Me.BtnRCC, "BtnRCC")
    resources.ApplyResources(Me.BtnReturn, "BtnReturn")
    resources.ApplyResources(Me.BtnStop, "BtnStop")
    resources.ApplyResources(Me, "$this")
    resources.ApplyResources(Me.GroupBox1, "GroupBox1")
    resources.ApplyResources(Me.GroupBox2, "GroupBox2")
  End Sub

    Private Sub BtnEfficiencyFar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEfficiencyFar.Click

    End Sub

    Private Sub BtnEfficiencyNear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEfficiencyNear.Click

    End Sub

  
End Class