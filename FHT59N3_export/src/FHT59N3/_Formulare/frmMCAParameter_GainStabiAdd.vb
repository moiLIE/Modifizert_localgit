Public Class frmMCAParameter_GainStabiAdd

    Dim _LynxParCopy As FHT59N3Core.FHT59N3_LynxParams
    Dim _Initialized As Boolean = False

    Private Sub ShowSettings()
        Try
            _LynxParCopy = CType(_MyControlCenter.MCA_Params, FHT59N3Core.FHT59N3_LynxParams).CopyMe

            CBox_ADSAquisitionMode.Items.Clear()
            For Each Acq As String In _LynxParCopy.Acq.Keys
                CBox_ADSAquisitionMode.Items.Add(Acq)
            Next
            For Each Acq As String In _LynxParCopy.Acq.Keys
                If _LynxParCopy.ADC_AcquisitionMode = _LynxParCopy.Acq(Acq) Then
                    CBox_ADSAquisitionMode.Text = Acq
                End If
            Next

            CBox_ADSBLRMode.Items.Clear()
            For Each Blr As String In _LynxParCopy.BLR.Keys
                CBox_ADSBLRMode.Items.Add(Blr)
            Next
            For Each Blr As String In _LynxParCopy.BLR.Keys
                If _LynxParCopy.AMP_BLRMode = _LynxParCopy.BLR(Blr) Then
                    CBox_ADSBLRMode.Text = Blr
                End If
            Next

            CBox_GainInputPolarity.Items.Clear()
            For Each Pol As String In _LynxParCopy.Polarity.Keys
                CBox_GainInputPolarity.Items.Add(Pol)
            Next
            For Each Pol As String In _LynxParCopy.Polarity.Keys
                If _LynxParCopy.AMP_InputPolarity = _LynxParCopy.Polarity(Pol) Then
                    CBox_GainInputPolarity.Text = Pol
                End If
            Next

            CBox_GainLLDMode.Items.Clear()
            For Each Lld As String In _LynxParCopy.ManAuto.Keys
                CBox_GainLLDMode.Items.Add(Lld)
            Next
            For Each Lld As String In _LynxParCopy.ManAuto.Keys
                If _LynxParCopy.ADC_LLDMode = _LynxParCopy.ManAuto(Lld) Then
                    CBox_GainLLDMode.Text = Lld
                End If
            Next

            CBox_GainConversionGain.Items.Clear()
            For Each Conv As String In _LynxParCopy.ConversionGainL.Keys
                CBox_GainConversionGain.Items.Add(Conv)
            Next
            For Each Conv As String In _LynxParCopy.ConversionGainL.Keys
                If _LynxParCopy.ADC_ConversionGain = _LynxParCopy.ConversionGainL(Conv) Then
                    CBox_GainConversionGain.Text = Conv
                End If
            Next

            CBox_GainCoarseGain.Items.Clear()
            For Each Coa As String In _LynxParCopy.CoarseGainL.Keys
                CBox_GainCoarseGain.Items.Add(Coa)
            Next
            For Each Coa As String In _LynxParCopy.CoarseGainL.Keys
                If _LynxParCopy.AMP_CoarseGain = _LynxParCopy.CoarseGainL(Coa) Then
                    CBox_GainCoarseGain.Text = Coa
                End If
            Next

            CBox_DSRange.Items.Clear()
            For Each Rang As String In _LynxParCopy.StabRange.Keys
                CBox_DSRange.Items.Add(Rang)
            Next
            For Each Rang As String In _LynxParCopy.StabRange.Keys
                If _LynxParCopy.STAB_UseNaI = _LynxParCopy.StabRange(Rang) Then
                    CBox_DSRange.Text = Rang
                End If
            Next

            CBox_DSRatioMode.Items.Clear()
            For Each Rm As String In _LynxParCopy.ManAuto.Keys
                CBox_DSRatioMode.Items.Add(Rm)
            Next
            For Each Rm As String In _LynxParCopy.ManAuto.Keys
                If _LynxParCopy.STAB_GainRatioAutoMode = _LynxParCopy.ManAuto(Rm) Then
                    CBox_DSRatioMode.Text = Rm
                End If
            Next

            CBox_DSDivider.Items.Clear()
            For Each Mul As String In _LynxParCopy.MultiplierL.Keys
                CBox_DSDivider.Items.Add(Mul)
            Next
            For Each Mul As String In _LynxParCopy.MultiplierL.Keys
                If _LynxParCopy.STAB_Multiplier = _LynxParCopy.MultiplierL(Mul) Then
                    CBox_DSDivider.Text = Mul
                End If
            Next

            CBox_DSMode.Items.Clear()
            For Each Sm As String In _LynxParCopy.Stabilizer.Keys
                CBox_DSMode.Items.Add(Sm)
            Next
            For Each Sm As String In _LynxParCopy.Stabilizer.Keys
                If _LynxParCopy.STAB_StabMode = _LynxParCopy.Stabilizer(Sm) Then
                    CBox_DSMode.Text = Sm
                End If
            Next

            Num_ADSShapingRiseTime.Minimum = CType(_LynxParCopy.FilterRiseTimeMinMax(0) / CType(_LynxParCopy.FilterRiseTimeMinMax(2), Integer), Double)
            Lbl_ADSShapingRiseTimeMin.Text = Format(_LynxParCopy.FilterRiseTimeMinMax(0) / CType(_LynxParCopy.FilterRiseTimeMinMax(2), Integer), CType(_LynxParCopy.FilterRiseTimeMinMax(3), String)) & CType(_LynxParCopy.FilterRiseTimeMinMax(4), String)
            Num_ADSShapingRiseTime.Maximum = CType(_LynxParCopy.FilterRiseTimeMinMax(1) / CType(_LynxParCopy.FilterRiseTimeMinMax(2), Integer), Double)
            Lbl_ADSShapingRiseTimeMax.Text = Format(_LynxParCopy.FilterRiseTimeMinMax(1) / CType(_LynxParCopy.FilterRiseTimeMinMax(2), Integer), CType(_LynxParCopy.FilterRiseTimeMinMax(3), String)) & CType(_LynxParCopy.FilterRiseTimeMinMax(4), String)
            Num_ADSShapingRiseTime.Value = CType(_LynxParCopy.AMP_FilterRiseTime, Double)

            Num_ADSShapingFlatTop.Minimum = CType(_LynxParCopy.FilterFlatTopMinMax(0) / CType(_LynxParCopy.FilterFlatTopMinMax(2), Integer), Double)
            Lbl_ADSShapingFlatTopMin.Text = Format(_LynxParCopy.FilterFlatTopMinMax(0) / CType(_LynxParCopy.FilterFlatTopMinMax(2), Integer), CType(_LynxParCopy.FilterFlatTopMinMax(3), String)) & CType(_LynxParCopy.FilterFlatTopMinMax(4), String)
            Num_ADSShapingFlatTop.Maximum = CType(_LynxParCopy.FilterFlatTopMinMax(1) / CType(_LynxParCopy.FilterFlatTopMinMax(2), Integer), Double)
            Lbl_ADSShapingFlatTopMax.Text = Format(_LynxParCopy.FilterFlatTopMinMax(1) / CType(_LynxParCopy.FilterFlatTopMinMax(2), Integer), CType(_LynxParCopy.FilterFlatTopMinMax(3), String)) & CType(_LynxParCopy.FilterFlatTopMinMax(4), String)
            Num_ADSShapingFlatTop.Value = CType(_LynxParCopy.AMP_FilterFlatTop, Double)

            Num_ADSPoleZero.Minimum = CType(_LynxParCopy.AmplifierPoleZeroMinMax(0) / CType(_LynxParCopy.AmplifierPoleZeroMinMax(2), Integer), Double)
            Lbl_ADSPoleZeroMin.Text = Format(_LynxParCopy.AmplifierPoleZeroMinMax(0) / CType(_LynxParCopy.AmplifierPoleZeroMinMax(2), Integer), CType(_LynxParCopy.AmplifierPoleZeroMinMax(3), String)) & CType(_LynxParCopy.AmplifierPoleZeroMinMax(4), String)
            Num_ADSPoleZero.Maximum = CType(_LynxParCopy.AmplifierPoleZeroMinMax(1) / CType(_LynxParCopy.AmplifierPoleZeroMinMax(2), Integer), Double)
            Lbl_ADSPoleZeroMax.Text = Format(_LynxParCopy.AmplifierPoleZeroMinMax(1) / CType(_LynxParCopy.AmplifierPoleZeroMinMax(2), Integer), CType(_LynxParCopy.AmplifierPoleZeroMinMax(3), String)) & CType(_LynxParCopy.AmplifierPoleZeroMinMax(4), String)
            Num_ADSPoleZero.Value = CType(_LynxParCopy.AMP_PoleZero, Double)

            Num_GainFineGain.Minimum = CType(_LynxParCopy.AmplifierFineGainMinMax(0) / CType(_LynxParCopy.AmplifierFineGainMinMax(2), Integer), Double)
            Lbl_GainFineGainMin.Text = Format(_LynxParCopy.AmplifierFineGainMinMax(0) / CType(_LynxParCopy.AmplifierFineGainMinMax(2), Integer), CType(_LynxParCopy.AmplifierFineGainMinMax(3), String)) & CType(_LynxParCopy.AmplifierFineGainMinMax(4), String)
            Num_GainFineGain.Maximum = CType(_LynxParCopy.AmplifierFineGainMinMax(1) / CType(_LynxParCopy.AmplifierFineGainMinMax(2), Integer), Double)
            Lbl_GainFineGainMax.Text = Format(_LynxParCopy.AmplifierFineGainMinMax(1) / CType(_LynxParCopy.AmplifierFineGainMinMax(2), Integer), CType(_LynxParCopy.AmplifierFineGainMinMax(3), String)) & CType(_LynxParCopy.AmplifierFineGainMinMax(4), String)
            Num_GainFineGain.Value = CType(_LynxParCopy.AMP_FineGain, Double)

            Num_GainLLD.Minimum = CType(_LynxParCopy.ADCLLDMinMax(0) / CType(_LynxParCopy.ADCLLDMinMax(2), Integer), Double)
            Lbl_GainLLDMin.Text = Format(_LynxParCopy.ADCLLDMinMax(0) / CType(_LynxParCopy.ADCLLDMinMax(2), Integer), CType(_LynxParCopy.ADCLLDMinMax(3), String)) & CType(_LynxParCopy.ADCLLDMinMax(4), String)
            Num_GainLLD.Maximum = CType(_LynxParCopy.ADCLLDMinMax(1) / CType(_LynxParCopy.ADCLLDMinMax(2), Integer), Double)
            Lbl_GainLLDMax.Text = Format(_LynxParCopy.ADCLLDMinMax(1) / CType(_LynxParCopy.ADCLLDMinMax(2), Integer), CType(_LynxParCopy.ADCLLDMinMax(3), String)) & CType(_LynxParCopy.ADCLLDMinMax(4), String)
            Num_GainLLD.Value = CType(_LynxParCopy.ADC_LLD, Double)

            Num_GainULD.Minimum = CType(_LynxParCopy.ADCULDMinMax(0) / CType(_LynxParCopy.ADCULDMinMax(2), Integer), Double)
            Lbl_GainULDMin.Text = Format(_LynxParCopy.ADCULDMinMax(0) / CType(_LynxParCopy.ADCULDMinMax(2), Integer), CType(_LynxParCopy.ADCULDMinMax(3), String)) & CType(_LynxParCopy.ADCULDMinMax(4), String)
            Num_GainULD.Maximum = CType(_LynxParCopy.ADCULDMinMax(1) / CType(_LynxParCopy.ADCULDMinMax(2), Integer), Double)
            Lbl_GainULDMax.Text = Format(_LynxParCopy.ADCULDMinMax(1) / CType(_LynxParCopy.ADCULDMinMax(2), Integer), CType(_LynxParCopy.ADCULDMinMax(3), String)) & CType(_LynxParCopy.ADCULDMinMax(4), String)
            Num_GainULD.Value = CType(_LynxParCopy.ADC_ULD, Double)

            Num_DSCentroid.Minimum = CType(_LynxParCopy.STABCentroidMinMax(0) / CType(_LynxParCopy.STABCentroidMinMax(2), Integer), Double)
            Lbl_DSCentroidMin.Text = Format(_LynxParCopy.STABCentroidMinMax(0) / CType(_LynxParCopy.STABCentroidMinMax(2), Integer), CType(_LynxParCopy.STABCentroidMinMax(3), String)) & CType(_LynxParCopy.STABCentroidMinMax(4), String)
            Num_DSCentroid.Maximum = CType(_LynxParCopy.STABCentroidMinMax(1) / CType(_LynxParCopy.STABCentroidMinMax(2), Integer), Double)
            Lbl_DSCentroidMax.Text = Format(_LynxParCopy.STABCentroidMinMax(1) / CType(_LynxParCopy.STABCentroidMinMax(2), Integer), CType(_LynxParCopy.STABCentroidMinMax(3), String)) & CType(_LynxParCopy.STABCentroidMinMax(4), String)
            Num_DSCentroid.Value = CType(_LynxParCopy.STAB_Centroid, Double)

            Num_DSWindow.Minimum = CType(_LynxParCopy.STABWindowMinMax(0) / CType(_LynxParCopy.STABWindowMinMax(2), Integer), Double)
            Lbl_DSWindowMin.Text = Format(_LynxParCopy.STABWindowMinMax(0) / CType(_LynxParCopy.STABWindowMinMax(2), Integer), CType(_LynxParCopy.STABWindowMinMax(3), String)) & CType(_LynxParCopy.STABWindowMinMax(4), String)
            Num_DSWindow.Maximum = CType(_LynxParCopy.STABWindowMinMax(1) / CType(_LynxParCopy.STABWindowMinMax(2), Integer), Double)
            Lbl_DSWindowMax.Text = Format(_LynxParCopy.STABWindowMinMax(1) / CType(_LynxParCopy.STABWindowMinMax(2), Integer), CType(_LynxParCopy.STABWindowMinMax(3), String)) & CType(_LynxParCopy.STABWindowMinMax(4), String)
            Num_DSWindow.Value = CType(_LynxParCopy.STAB_Window, Double)

            Num_DSSpacing.Minimum = CType(_LynxParCopy.STABSpacingMinMax(0) / CType(_LynxParCopy.STABSpacingMinMax(2), Integer), Double)
            Lbl_DSSpacingMin.Text = Format(_LynxParCopy.STABSpacingMinMax(0) / CType(_LynxParCopy.STABSpacingMinMax(2), Integer), CType(_LynxParCopy.STABSpacingMinMax(3), String)) & CType(_LynxParCopy.STABSpacingMinMax(4), String)
            Num_DSSpacing.Maximum = CType(_LynxParCopy.STABSpacingMinMax(1) / CType(_LynxParCopy.STABSpacingMinMax(2), Integer), Double)
            Lbl_DSSpacingMax.Text = Format(_LynxParCopy.STABSpacingMinMax(1) / CType(_LynxParCopy.STABSpacingMinMax(2), Integer), CType(_LynxParCopy.STABSpacingMinMax(3), String)) & CType(_LynxParCopy.STABSpacingMinMax(4), String)
            Num_DSSpacing.Value = CType(_LynxParCopy.STAB_Spacing, Double)

            Num_DSRatio.Minimum = CType(_LynxParCopy.STABRatioMinMax(0) / CType(_LynxParCopy.STABRatioMinMax(2), Integer), Double)
            Lbl_DSRatioMin.Text = Format(_LynxParCopy.STABRatioMinMax(0) / CType(_LynxParCopy.STABRatioMinMax(2), Integer), CType(_LynxParCopy.STABRatioMinMax(3), String)) & CType(_LynxParCopy.STABRatioMinMax(4), String)
            Num_DSRatio.Maximum = CType(_LynxParCopy.STABRatioMinMax(1) / CType(_LynxParCopy.STABRatioMinMax(2), Integer), Double)
            Lbl_DSRatioMax.Text = Format(_LynxParCopy.STABRatioMinMax(1) / CType(_LynxParCopy.STABRatioMinMax(2), Integer), CType(_LynxParCopy.STABRatioMinMax(3), String)) & CType(_LynxParCopy.STABRatioMinMax(4), String)
            Num_DSRatio.Value = CType(_LynxParCopy.STAB_WindowRatio, Double)

            _Initialized = True
            _MyControlCenter.MCA_StartMeasurement(60, True, False)
            GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, True, SpectralDisplay)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub frmMCAParameter_GainStabiAdd_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            ShowSettings()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_ADSAquisitionMode_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_ADSAquisitionMode.Leave
        Try
            If Not CBox_ADSAquisitionMode.Items.Contains(CBox_ADSAquisitionMode.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_ADSAquisitionMode.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_ADSBLRMode_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_ADSBLRMode.Leave
        Try
            If Not CBox_ADSBLRMode.Items.Contains(CBox_ADSBLRMode.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_ADSBLRMode.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_GainInputPolarity_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_GainInputPolarity.Leave
        Try
            If Not CBox_GainInputPolarity.Items.Contains(CBox_GainInputPolarity.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_GainInputPolarity.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_GainLLDMode_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_GainLLDMode.Leave
        Try
            If Not CBox_GainLLDMode.Items.Contains(CBox_GainLLDMode.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_GainLLDMode.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_GainConversionGain_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_GainConversionGain.Leave
        Try
            If Not CBox_GainConversionGain.Items.Contains(CBox_GainConversionGain.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_GainConversionGain.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_GainCoarseGain_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_GainCoarseGain.Leave
        Try
            If Not CBox_GainCoarseGain.Items.Contains(CBox_GainCoarseGain.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_GainCoarseGain.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_DSRange_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_DSRange.Leave
        Try
            If Not CBox_DSRange.Items.Contains(CBox_DSRange.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_DSRange.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_DSRatioMode_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_DSRatioMode.Leave
        Try
            If Not CBox_DSRatioMode.Items.Contains(CBox_DSRatioMode.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_DSRatioMode.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_DSDivider_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_DSDivider.Leave
        Try
            If Not CBox_DSDivider.Items.Contains(CBox_DSDivider.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_DSDivider.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub CBox_DSMode_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBox_DSMode.Leave
        Try
            If Not CBox_DSMode.Items.Contains(CBox_DSMode.Text) Then
                GUI_ShowMessageBox(FHT59N3Core.FHT59N3_LynxParams.MSG_SelectOneItemOfList, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                CBox_DSMode.SelectedIndex = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReset.Click
        Try
            BtnReset.Enabled = False
            Me.Cursor = Cursors.WaitCursor
            _MyControlCenter.MCA_StopMeasurement(False)
            GUI_CloseSpectrum(False, True)
            ShowSettings()
            BtnSet.Enabled = True
            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSet.Click
        Try
            BtnSet.Enabled = False
            Me.Cursor = Cursors.WaitCursor
            _LynxParCopy.AMP_InputPolarity = _LynxParCopy.Polarity(CBox_GainInputPolarity.Text)
            _LynxParCopy.AMP_CoarseGain = _LynxParCopy.CoarseGainL(CBox_GainCoarseGain.Text)
            _LynxParCopy.AMP_FineGain = CType(Num_GainFineGain.Value, Double)
            _LynxParCopy.AMP_PoleZero = CType(Num_ADSPoleZero.Value, Double)
            _LynxParCopy.AMP_BLRMode = _LynxParCopy.BLR(CBox_ADSBLRMode.Text)
            _LynxParCopy.AMP_FilterRiseTime = CType(Num_ADSShapingRiseTime.Value, Double)
            _LynxParCopy.AMP_FilterFlatTop = CType(Num_ADSShapingFlatTop.Value, Double)
            _LynxParCopy.ADC_AcquisitionMode = _LynxParCopy.Acq(CBox_ADSAquisitionMode.Text)
            _LynxParCopy.ADC_LLDMode = _LynxParCopy.ManAuto(CBox_GainLLDMode.Text)
            _LynxParCopy.ADC_LLD = CType(Num_GainLLD.Value, Double)
            _LynxParCopy.ADC_ULD = CType(Num_GainULD.Value, Double)
            _LynxParCopy.ADC_ConversionGain = _LynxParCopy.ConversionGainL(CBox_GainConversionGain.Text)
            _LynxParCopy.STAB_Centroid = CType(Num_DSCentroid.Value, Double)
            _LynxParCopy.STAB_Window = CType(Num_DSWindow.Value, Double)
            _LynxParCopy.STAB_Spacing = CType(Num_DSSpacing.Value, Double)
            _LynxParCopy.STAB_Multiplier = _LynxParCopy.MultiplierL(CBox_DSDivider.Text)
            _LynxParCopy.STAB_WindowRatio = CType(Num_DSRatio.Value, Double)
            _LynxParCopy.STAB_UseNaI = _LynxParCopy.StabRange(CBox_DSRange.Text)
            _LynxParCopy.STAB_GainRatioAutoMode = _LynxParCopy.ManAuto(CBox_DSRatioMode.Text)
            _LynxParCopy.STAB_StabMode = _LynxParCopy.Stabilizer(CBox_DSMode.Text)
            _MyControlCenter.MCA_StopMeasurement(False)
            GUI_CloseSpectrum(False, True)
            _MyControlCenter.MCA_SetAmplifierParams(_LynxParCopy.AMP_InputPolarity, _LynxParCopy.AMP_CoarseGain, _LynxParCopy.AMP_FineGain, _LynxParCopy.AMP_PoleZero, _LynxParCopy.AMP_BLRMode, _LynxParCopy.AMP_FilterRiseTime, _LynxParCopy.AMP_FilterFlatTop)
            _MyControlCenter.MCA_SetADCParams(_LynxParCopy.ADC_AcquisitionMode, _LynxParCopy.ADC_LLDMode, _LynxParCopy.ADC_LLD, _LynxParCopy.ADC_ULD, _LynxParCopy.ADC_ConversionGain)
            _MyControlCenter.MCA_SetStabilizerParameters(_LynxParCopy.STAB_Centroid, _LynxParCopy.STAB_Window, _LynxParCopy.STAB_Spacing, _LynxParCopy.STAB_Multiplier, _LynxParCopy.STAB_WindowRatio, _LynxParCopy.STAB_UseNaI, _LynxParCopy.STAB_GainRatioAutoMode)
            _MyControlCenter.MCA_SetStabilizerMode(_LynxParCopy.STAB_StabMode)
            _MyControlCenter.MCA_StartMeasurement(60, True, False)
            GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, True, SpectralDisplay)
            BtnSet.Enabled = True
            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAccept.Click
        Try
            Dim ret As MsgBoxResult
            ret = GUI_ShowMessageBox(ml_string(308, "Do you really want to save these settings?"), ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)
            If ret = MsgBoxResult.Yes Then
                BtnAccept.Enabled = False
                Me.Cursor = Cursors.WaitCursor
                _LynxParCopy.AMP_InputPolarity = _LynxParCopy.Polarity(CBox_GainInputPolarity.Text)
                _LynxParCopy.AMP_CoarseGain = _LynxParCopy.CoarseGainL(CBox_GainCoarseGain.Text)
                _LynxParCopy.AMP_FineGain = CType(Num_GainFineGain.Value, Double)
                _LynxParCopy.AMP_PoleZero = CType(Num_ADSPoleZero.Value, Double)
                _LynxParCopy.AMP_BLRMode = _LynxParCopy.BLR(CBox_ADSBLRMode.Text)
                _LynxParCopy.AMP_FilterRiseTime = CType(Num_ADSShapingRiseTime.Value, Double)
                _LynxParCopy.AMP_FilterFlatTop = CType(Num_ADSShapingFlatTop.Value, Double)
                _LynxParCopy.ADC_AcquisitionMode = _LynxParCopy.Acq(CBox_ADSAquisitionMode.Text)
                _LynxParCopy.ADC_LLDMode = _LynxParCopy.ManAuto(CBox_GainLLDMode.Text)
                _LynxParCopy.ADC_LLD = CType(Num_GainLLD.Value, Double)
                _LynxParCopy.ADC_ULD = CType(Num_GainULD.Value, Double)
                _LynxParCopy.ADC_ConversionGain = _LynxParCopy.ConversionGainL(CBox_GainConversionGain.Text)
                _LynxParCopy.STAB_Centroid = CType(Num_DSCentroid.Value, Double)
                _LynxParCopy.STAB_Window = CType(Num_DSWindow.Value, Double)
                _LynxParCopy.STAB_Spacing = CType(Num_DSSpacing.Value, Double)
                _LynxParCopy.STAB_Multiplier = _LynxParCopy.MultiplierL(CBox_DSDivider.Text)
                _LynxParCopy.STAB_WindowRatio = CType(Num_DSRatio.Value, Double)
                _LynxParCopy.STAB_UseNaI = _LynxParCopy.StabRange(CBox_DSRange.Text)
                _LynxParCopy.STAB_GainRatioAutoMode = _LynxParCopy.ManAuto(CBox_DSRatioMode.Text)
                _LynxParCopy.STAB_StabMode = _LynxParCopy.Stabilizer(CBox_DSMode.Text)
                _MyControlCenter.MCA_Params = _LynxParCopy.CopyMe
                CType(_MyControlCenter.MCA_Params, FHT59N3Core.FHT59N3_LynxParams).SaveMeBinary(_MonitorConfigDirectory & "\")
                _MyControlCenter.MCA_StopMeasurement(False)
                GUI_CloseSpectrum(False, True)
                _MyControlCenter.MCA_SetAllMeasParams()
                BtnAccept.Enabled = True
                Me.Cursor = Cursors.Default
                Me.Close()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Try
            BtnClose.Enabled = False
            Me.Cursor = Cursors.WaitCursor
            _MyControlCenter.MCA_SetAllMeasParams() 'wurden die Parameter verstellt, und ich gehe mit "Cancel" raus, dann einfach die alten Parameter wieder herstellen
            'dies kann über diesen Befehl passieren, da noch nicht aus der Kopie in den echten Parametersatz gespeichert wurde
            _MyControlCenter.MCA_StopMeasurement(False)
            GUI_CloseSpectrum(False, True)
            BtnClose.Enabled = True
            Me.Cursor = Cursors.Default
            SpectralDisplay.Close()
            SpectralDisplay.CloseAll()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnExpandMode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExpandMode.Click
        Try
            If Not SpectralDisplay.ExpandMode Then
                SpectralDisplay.ExpandMode = True
            Else
                SpectralDisplay.ExpandMode = False
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmMCAParameter_GainStabiAdd_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        _MyControlCenter.MCA_SetAutoPoleZero()
        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Label1.Text = _MyControlCenter.MCA_GetPoleZeroValue.ToString
        'Timer1.Enabled = _MyControlCenter.MCA_GetPoleZeroBusyState
    End Sub

  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMCAParameter_GainStabiAdd))
    resources.ApplyResources(Me.BtnAccept, "BtnAccept")
    resources.ApplyResources(Me.BtnClose, "BtnClose")
    resources.ApplyResources(Me.BtnReset, "BtnReset")
    resources.ApplyResources(Me.BtnSet, "BtnSet")
    resources.ApplyResources(Me.Button1, "Button1")
    resources.ApplyResources(Me, "$this")
    resources.ApplyResources(Me.GroupBox2, "GroupBox2")
    resources.ApplyResources(Me.GroupBox3, "GroupBox3")
    resources.ApplyResources(Me.GroupBox4, "GroupBox4")
    resources.ApplyResources(Me.Label10, "Label10")
    resources.ApplyResources(Me.Label11, "Label11")
    resources.ApplyResources(Me.Label12, "Label12")
    resources.ApplyResources(Me.Label14, "Label14")
    resources.ApplyResources(Me.Label17, "Label17")
    resources.ApplyResources(Me.Label18, "Label18")
    resources.ApplyResources(Me.Label21, "Label21")
    resources.ApplyResources(Me.Label25, "Label25")
    resources.ApplyResources(Me.Label38, "Label38")
    resources.ApplyResources(Me.Label41, "Label41")
    resources.ApplyResources(Me.Label44, "Label44")
    resources.ApplyResources(Me.Label47, "Label47")
    resources.ApplyResources(Me.Label48, "Label48")
    resources.ApplyResources(Me.Label49, "Label49")
    resources.ApplyResources(Me.Label50, "Label50")
    resources.ApplyResources(Me.Label51, "Label51")
  End Sub
End Class