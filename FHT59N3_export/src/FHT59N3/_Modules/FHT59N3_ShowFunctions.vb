Imports FHT59N3Core
Imports FHT59N3Core.FHT59N3_SystemStates
Imports AxCanberraDataDisplayLib
Imports CanberraDataDisplayLib

Module FHT59N3_ShowFunctions
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_ShowFunctions.vb 434 2018-11-21 17:14:08Z burak.topbas $
    ' Title: show functions
    '
    ' Description:
    ' GUI library functions:
    '  - simple GUI tools (popups, splash)
    '  - fill data in GUI list controls
    '  - 
    ' -------------------------------------------------------------

    Public Enum EcoolerGuiStates
        UNCHANGED
        ACTIVATED
        DEACTIVATED
    End Enum


#Region "Forms"

    Public Sub GUI_FillBackColors()
        Try
            frmMain.BackColor = MYCOL_THERMOGREEN
            frmMain.PanelBottom.BackColor = MYCOL_THERMOGREEN
            frmMain.PanelRight.BackColor = MYCOL_THERMOGREEN
            frmMain.UcStatusSideBar.LabelClock.BackColor = MYCOL_THERMOGREEN
            frmMain.UcStatusSideBar.LabelClock.ForeColor = Color.White

            frmMain.UcStatusSideBar.TextAlarmNuclides.BackColor = MYCOL_THERMOGREEN

            frmParameter.BackColor = MYCOL_THERMOGREEN
            frmParameter.PropGridParameter.BackColor = MYCOL_THERMOGREEN
            frmParameter.PropGridParameter.HelpBackColor = MYCOL_THERMOGREEN
            frmParameter.PropGridParameter.HelpForeColor = Color.White
            frmParameter.PropGridParameter.CategoryForeColor = Color.White
            frmParameter.PropGridParameter.ViewBackColor = Color.White
            frmParameter.PropGridParameter.ViewForeColor = Color.Black
            frmParameter.PropGridParameter.LineColor = MYCOL_THERMOGREEN

            frmMCAParameter_HV.BackColor = MYCOL_THERMOGREEN
            frmStates.BackColor = MYCOL_THERMOGREEN
            frmMessages.BackColor = MYCOL_THERMOGREEN
            frmAirFlow.BackColor = MYCOL_THERMOGREEN
            frmOperationMenu.BackColor = MYCOL_THERMOGREEN
            frmMaintenanceMenu.BackColor = MYCOL_THERMOGREEN
            frmControlMenu.BackColor = MYCOL_THERMOGREEN
            frmCalibrationMenu.BackColor = MYCOL_THERMOGREEN
            frmMCAMenu.BackColor = MYCOL_THERMOGREEN
            frmFileMenu.BackColor = MYCOL_THERMOGREEN
            frmAbout.BackColor = MYCOL_THERMOGREEN
            frmAbout.TextBoxDescription.BackColor = MYCOL_THERMOGREEN
            frmResultFile.BackColor = MYCOL_THERMOGREEN
            frmCalibrationMeasControl.BackColor = MYCOL_THERMOGREEN
            frmCalibrationAnalyzeControl.BackColor = MYCOL_THERMOGREEN
            frmROIMenu.BackColor = MYCOL_THERMOGREEN
            frmFilterMenu.BackColor = MYCOL_THERMOGREEN
            frmFiltersteps.BackColor = MYCOL_THERMOGREEN
            frmDetectorTemperature.BackColor = MYCOL_THERMOGREEN
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub GUI_ShowForm(ByVal N_Panel As Panel)
        Try
            If Not _ActualVisiblePanel Is Nothing Then
                If _ActualVisiblePanel.Name = N_Panel.Name Then
                    Exit Sub
                End If
                _ActualVisiblePanel.Visible = False
            End If
            If Not frmMain.Controls.Contains(N_Panel) Then
                frmMain.Controls.Add(N_Panel)
            End If
            N_Panel.Visible = True
            _ActualVisiblePanel = N_Panel
            _ActualVisiblePanel.Dock = DockStyle.Fill
            _ActualVisiblePanel.BringToFront()
            Select Case N_Panel.Name
                Case "PanelfrmMeasScreen"
                    GUI_InitializeMeasScreen()
                Case "PanelfrmServiceScreen"
                    frmServiceScreen.GUI_FillServiceForm()
            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' Update the labels (shown states) and their colors in dialog "control" and in dialog "states"
    ''' </summary>
    Public Sub GUI_SetLabelsBecauseOfDigStates()
        Try
            If _MyControlCenter.SPS_PumpOnOff Then
                frmControlMenu.LblPump.Text = ml_string(54, "pumping")
                frmControlMenu.LblPump.BackColor = MYCOL_OK
                frmStates.LblPump.Text = ml_string(54, "pumping")
                frmStates.LblPump.BackColor = MYCOL_OK
            Else
                frmControlMenu.LblPump.Text = ml_string(56, "stopped")
                frmControlMenu.LblPump.BackColor = MYCOL_NotOK
                frmStates.LblPump.Text = ml_string(56, "stopped")
                frmStates.LblPump.BackColor = MYCOL_NotOK
            End If
            If _MyControlCenter.SPS_BypassOnOff Then
                frmControlMenu.LblFilterBypass.Text = ml_string(246, "unlocked / open")
                frmControlMenu.LblFilterBypass.BackColor = MYCOL_NotOK
                frmStates.LblFilterBypass.Text = ml_string(246, "unlocked / open")
                frmStates.LblFilterBypass.BackColor = MYCOL_NotOK
            Else
                frmControlMenu.LblFilterBypass.Text = ml_string(53, "locked / closed")
                frmControlMenu.LblFilterBypass.BackColor = MYCOL_OK
                frmStates.LblFilterBypass.Text = ml_string(53, "locked / closed")
                frmStates.LblFilterBypass.BackColor = MYCOL_OK
            End If
            If _MyControlCenter.SPS_MotorOnOff Then
                frmControlMenu.LblFilterstep.Text = ml_string(247, "running")
                frmControlMenu.LblFilterstep.BackColor = MYCOL_NotOK
                frmStates.LblFilterstep.Text = ml_string(247, "running")
                frmStates.LblFilterstep.BackColor = MYCOL_NotOK
                frmFilterMenu.BtnSetFilterstep.Enabled = False
                frmControlMenu.BtnFilterstep.Enabled = False
                frmFilterMenu.BtnSetFilterstep.BackColor = Color.Gray
                frmControlMenu.BtnFilterstep.BackColor = Color.Gray
            Else
                frmControlMenu.LblFilterstep.Text = ml_string(56, "stopped")
                frmControlMenu.LblFilterstep.BackColor = MYCOL_OK
                frmStates.LblFilterstep.Text = ml_string(56, "stopped")
                frmStates.LblFilterstep.BackColor = MYCOL_OK
                frmFilterMenu.BtnSetFilterstep.Enabled = True
                frmControlMenu.BtnFilterstep.Enabled = True
                frmFilterMenu.BtnSetFilterstep.BackColor = SystemColors.ButtonFace
                frmControlMenu.BtnFilterstep.BackColor = SystemColors.ButtonFace
            End If
            If _MyControlCenter.SPS_AlarmRelaisOnOff Then
                frmControlMenu.LblAlarmIndication.Text = ml_string(248, "on")
                frmControlMenu.LblAlarmIndication.BackColor = MYCOL_NotOK
                frmStates.LblAlarmIndication.Text = ml_string(248, "on")
                frmStates.LblAlarmIndication.BackColor = MYCOL_NotOK
            Else
                frmControlMenu.LblAlarmIndication.Text = ml_string(55, "off")
                frmControlMenu.LblAlarmIndication.BackColor = MYCOL_OK
                frmStates.LblAlarmIndication.Text = ml_string(55, "off")
                frmStates.LblAlarmIndication.BackColor = MYCOL_OK
            End If
            If _MyControlCenter.SPS_ErrorOnOff Then
                frmControlMenu.LblErrorIndication.Text = ml_string(248, "on")
                frmControlMenu.LblErrorIndication.BackColor = MYCOL_NotOK
                frmStates.LblErrorIndication.Text = ml_string(248, "on")
                frmStates.LblErrorIndication.BackColor = MYCOL_NotOK
            Else
                frmControlMenu.LblErrorIndication.Text = ml_string(55, "off")
                frmControlMenu.LblErrorIndication.BackColor = MYCOL_OK
                frmStates.LblErrorIndication.Text = ml_string(55, "off")
                frmStates.LblErrorIndication.BackColor = MYCOL_OK
            End If
            If _MyControlCenter.SPS_HeatingOnOff Then
                frmControlMenu.LblN2Fill.Text = ml_string(248, "on")
                frmControlMenu.LblN2Fill.BackColor = MYCOL_NotOK
                frmStates.LblN2Fill.Text = ml_string(248, "on")
                frmStates.LblN2Fill.BackColor = MYCOL_NotOK
            Else
                frmControlMenu.LblN2Fill.Text = ml_string(55, "off")
                frmControlMenu.LblN2Fill.BackColor = MYCOL_OK
                frmStates.LblN2Fill.Text = ml_string(55, "off")
                frmStates.LblN2Fill.BackColor = MYCOL_OK
            End If
            If _MyControlCenter.MCA_GetHVState Then
                frmControlMenu.LblHighVoltage.Text = ml_string(248, "on")
                frmControlMenu.LblHighVoltage.BackColor = MYCOL_OK
                frmStates.LblHighVoltage.Text = ml_string(248, "on")
                frmStates.LblHighVoltage.BackColor = MYCOL_OK
            Else
                frmControlMenu.LblHighVoltage.Text = ml_string(55, "off")
                frmControlMenu.LblHighVoltage.BackColor = MYCOL_NotOK
                frmStates.LblHighVoltage.Text = ml_string(55, "off")
                frmStates.LblHighVoltage.BackColor = MYCOL_NotOK
            End If
            If Not _MyFHT59N3Par.IsCanberraDetector Then
                If _MyControlCenter.SYS_States.EcoolerOff Then
                    frmControlMenu.LblEcoolerState.Text = ml_string(533, "not cooling")
                    frmControlMenu.LblEcoolerState.BackColor = MYCOL_NotOK
                Else
                    frmControlMenu.LblEcoolerState.Text = ml_string(534, "running")
                    frmControlMenu.LblEcoolerState.BackColor = MYCOL_OK
                End If
            Else 'Canberra
                frmControlMenu.LblEcoolerState.Text = ml_string(675, "Canberra")
                frmControlMenu.LblEcoolerState.BackColor = MYCOL_WARNING
            End If


        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_SetMenus()
        Try
            If Not _MyControlCenter.SYS_States.Maintenance Then 'Wartung aus
                frmMaintenanceMenu.BtnMaintenanceOn.Text = ml_string(74, "Maintenance On")
                frmMaintenanceMenu.BtnSysConfig.Enabled = False
                frmMaintenanceMenu.BtnSysConfig.BackColor = Color.Gray
                frmMaintenanceMenu.BtnMCAConfig.Enabled = False
                frmMaintenanceMenu.BtnMCAConfig.BackColor = Color.Gray
                frmMaintenanceMenu.BtnCalibration.Enabled = False
                frmMaintenanceMenu.BtnCalibration.BackColor = Color.Gray
                frmMaintenanceMenu.BtnControl.Enabled = False
                frmMaintenanceMenu.BtnControl.BackColor = Color.Gray
                frmMaintenanceMenu.BtnFilterTape.Enabled = False
                frmMaintenanceMenu.BtnFilterTape.BackColor = Color.Gray
            Else 'Wartung an
                frmMaintenanceMenu.BtnMaintenanceOn.Text = ml_string(249, "Maintenance Off")
                frmMaintenanceMenu.BtnSysConfig.Enabled = True
                frmMaintenanceMenu.BtnSysConfig.BackColor = SystemColors.ButtonFace
                frmMaintenanceMenu.BtnMCAConfig.Enabled = True
                frmMaintenanceMenu.BtnMCAConfig.BackColor = SystemColors.ButtonFace
                frmMaintenanceMenu.BtnCalibration.Enabled = True
                frmMaintenanceMenu.BtnCalibration.BackColor = SystemColors.ButtonFace
                frmMaintenanceMenu.BtnControl.Enabled = True
                frmMaintenanceMenu.BtnControl.BackColor = SystemColors.ButtonFace
                frmMaintenanceMenu.BtnFilterTape.Enabled = True
                frmMaintenanceMenu.BtnFilterTape.BackColor = SystemColors.ButtonFace
            End If


            If (Not _MyControlCenter.SYS_States.IntensiveMode) And (Not _MyControlCenter.SYS_States.AlarmMode) Then 'intensive mode off
                frmOperationMenu.BtnIntensOn.Enabled = True
                frmOperationMenu.BtnIntensOn.BackColor = SystemColors.ButtonFace
                frmOperationMenu.BtnIntensOff.Enabled = False
                frmOperationMenu.BtnIntensOff.BackColor = Color.Gray
            ElseIf (_MyControlCenter.SYS_States.IntensiveMode) Then 'Intensive mode on
                frmOperationMenu.BtnIntensOn.Enabled = False
                frmOperationMenu.BtnIntensOn.BackColor = Color.Gray
                frmOperationMenu.BtnIntensOff.Enabled = True
                frmOperationMenu.BtnIntensOff.BackColor = SystemColors.ButtonFace
            End If

            'Achtung: Button heißt 'Alarmausgang an/aus' nicht AlarmModus an/aus!
            frmOperationMenu.panelAlarmMode.Visible = _AlarmOutSwitchableWithoutMaintenance
            frmOperationMenu.panelIntensiveMode.Visible = _MyFHT59N3Par.IntensiveModeEnabled


        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_ShowMonitorInformation()
        Try
            frmMain.UcStatusSideBar.LabelClock.Text = Format(Now, "dd.MM.yyyy HH:mm:ss")

            If _MyControlCenter.SYS_States.SumStateLevel = FHT59N3_SystemStates.StateLevel.OK Then

                _MyControlCenter.SPS_ErrorOff()       'Summenstörung aus
            ElseIf _MyControlCenter.SYS_States.SumStateLevel = FHT59N3_SystemStates.StateLevel.WARNING Then

                _MyControlCenter.SPS_ErrorOff()       'Summenstörung aus
            ElseIf _MyControlCenter.SYS_States.SumStateLevel = FHT59N3_SystemStates.StateLevel.FATAL Then

                _MyControlCenter.SPS_ErrorOn()       'Summenstörung an
            End If

            frmMain.UcStatusSideBar.LblFilterSteps.Text = _MyFHT59N3Par.FilterSteps.ToString
            frmMain.UcStatusSideBar.LblAirFlowActual.Text = Format(_AirFlowActual, "0.00")
            frmMain.UcStatusSideBar.LblAirFlowMean.Text = Format(_AirFlowMean, "0.00")

            If _MyFHT59N3Par.N2FillThreshold > 0 Then
                frmMain.UcStatusSideBar.LbLN2.Visible = True
                frmMain.UcStatusSideBar.Label2.Visible = True
                frmMain.UcStatusSideBar.LbLN2.Text = Format(_N2FillValue, "0")
            Else
                frmMain.UcStatusSideBar.LbLN2.Visible = False
                frmMain.UcStatusSideBar.Label2.Visible = False
            End If

            frmMain.UcStatusSideBar.LblTemp.Text = Format(_Temperature, "0.00")

            frmMain.UcStatusSideBar.LblDetectTemp.Text = If(_DetectorTemperaturValue <> Double.MinValue,
                                            Format(_DetectorTemperaturValue, "0.00"), "--")
            If _MyFHT59N3Par.IsCanberraDetector Then
                'Show cooling power
                frmMain.UcStatusSideBar.LblCryoPowerName.Visible = True
                frmMain.UcStatusSideBar.LblCryoPowerValue.Visible = True
                frmMain.UcStatusSideBar.LblCryoPowerValue.Text = Format(_CryoPower, "0")
            Else
                'Hide cooling power
                frmMain.UcStatusSideBar.LblCryoPowerName.Visible = False
                frmMain.UcStatusSideBar.LblCryoPowerValue.Visible = False
            End If

            If _MyControlCenter.SYS_States.Maintenance Then
                frmMain.UcStatusSideBar.LblStatus.Text = ml_string(65, "Maintenance")
                frmMain.UcStatusSideBar.LblStatus.BackColor = MYCOL_WARNING
            Else
                If _MyControlCenter.SYS_States.AlarmMode Or _MyControlCenter.SYS_States.IntensiveMode Then
                    If _MyControlCenter.SYS_States.AlarmMode Then
                        frmMain.UcStatusSideBar.LblStatus.Text = "ALARM"
                        frmMain.UcStatusSideBar.LblStatus.BackColor = MYCOL_NotOK
                    ElseIf _MyControlCenter.SYS_States.IntensiveMode Then
                        frmMain.UcStatusSideBar.LblStatus.Text = ml_string(251, "Intensive Mode")
                        frmMain.UcStatusSideBar.LblStatus.BackColor = MYCOL_WARNING
                    End If
                Else
                    frmMain.UcStatusSideBar.LblStatus.Text = ml_string(252, "Measuring")
                    frmMain.UcStatusSideBar.LblStatus.BackColor = MYCOL_OK
                End If
            End If

            Select Case _ActualVisiblePanel.Name
                Case "PanelfrmMeasScreen"
                    GUI_FillStatesMeasScreen()

                    If Not _MsgListFilled Then
                        GUI_FillMsgMeasScreen()
                    End If

                    If Not _DigInOutListFilled Then
                        GUI_FillDigInOutListMeasScreen()
                    Else
                        GUI_UpdateDigInOutListMeasScreen()
                    End If
                Case "PanelfrmServiceScreen"
                    frmServiceScreen.GUI_UpdateServiceForm()
            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub GUI_InitializeMeasScreen()
        GUI_FillStatesMeasScreen()
        _MsgListFilled = False
        GUI_FillMsgMeasScreen()
        _DigInOutListFilled = False
        GUI_FillDigInOutListMeasScreen()
    End Sub

    Public Sub GUI_ResizeMeasScreen()
        Try
            With _InstanceOfFormMeasscreen
                .ListView1.Font = _LViewFont
                .ListView2.Font = _LViewFont
                .ListView3.Font = _LViewFont
                .Label1.Font = _LViewFont1
                .Label2.Font = _LViewFont1
                .Label3.Font = _LViewFont1
                If .RtbAnalyzeData.Visible = True Then
                    If _ShowSystemStateInMainDisplay Then
                        .Label1.Visible = True
                        .Label1.Top = 0
                        .Label2.Visible = True
                        .Label2.Top = 0
                        .Label3.Visible = True
                        .Label3.Top = 0
                        .RtbAnalyzeData.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2 / 3)
                        .ListView2.Visible = True
                        .ListView2.Left = 0
                        .ListView2.Top = .Label1.Height
                        .ListView2.Width = Math.Abs(.PanelfrmMeasScreen.Width / 4)
                        .ListView2.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2) - .Label1.Height
                        .ListView1.Visible = True
                        .ListView1.Left = .ListView2.Width
                        .ListView1.Top = .Label1.Height
                        .ListView1.Width = Math.Abs(.PanelfrmMeasScreen.Width / 2.5)
                        .ListView1.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2) - .Label1.Height
                        .ListView3.Visible = True
                        .ListView3.Left = .ListView1.Width + .ListView2.Width
                        .ListView3.Top = .Label1.Height
                        .ListView3.Width = .PanelfrmMeasScreen.Width - .ListView1.Width - .ListView2.Width
                        .ListView3.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2) - .Label1.Height
                        .SpectralDisplay.Height = .PanelfrmMeasScreen.Height - .RtbAnalyzeData.Height - .ListView1.Height - .Label1.Height
                        .Label1.Left = .ListView2.Left
                        .Label2.Left = .ListView1.Left
                        .Label3.Left = .ListView3.Left
                    Else
                        .RtbAnalyzeData.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2 / 3)
                        .SpectralDisplay.Top = 0
                        .SpectralDisplay.Height = .PanelfrmMeasScreen.Height - .RtbAnalyzeData.Height
                        .ListView1.Visible = False
                        .ListView2.Visible = False
                        .ListView3.Visible = False
                        .Label1.Visible = False
                        .Label2.Visible = False
                        .Label3.Visible = False
                    End If
                Else
                    If _ShowSystemStateInMainDisplay Then
                        .Label1.Visible = True
                        .Label1.Top = 0
                        .Label2.Visible = True
                        .Label2.Top = 0
                        .Label3.Visible = True
                        .Label3.Top = 0
                        .ListView2.Visible = True
                        .ListView2.Left = 0
                        .ListView2.Top = .Label1.Height
                        .ListView2.Width = Math.Abs(.PanelfrmMeasScreen.Width / 4)
                        .ListView2.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2) - .Label1.Height
                        .ListView1.Visible = True
                        .ListView1.Left = .ListView2.Width
                        .ListView1.Top = .Label1.Height
                        .ListView1.Width = Math.Abs(.PanelfrmMeasScreen.Width / 2.5)
                        .ListView1.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2) - .Label1.Height
                        .ListView3.Visible = True
                        .ListView3.Left = .ListView1.Width + .ListView2.Width
                        .ListView3.Top = .Label1.Height
                        .ListView3.Width = .PanelfrmMeasScreen.Width - .ListView1.Width - .ListView2.Width
                        .ListView3.Height = Math.Abs(.PanelfrmMeasScreen.Height / 2) - .Label1.Height
                        .SpectralDisplay.Height = .PanelfrmMeasScreen.Height - .ListView1.Height - .Label1.Height
                        .Label1.Left = .ListView2.Left
                        .Label2.Left = .ListView1.Left
                        .Label3.Left = .ListView3.Left
                    Else
                        .SpectralDisplay.Top = 0
                        .SpectralDisplay.Height = .PanelfrmMeasScreen.Height
                        .ListView1.Visible = False
                        .ListView2.Visible = False
                        .ListView3.Visible = False
                        .Label1.Visible = False
                        .Label2.Visible = False
                        .Label3.Visible = False
                    End If
                End If
            End With
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_FillStatesMeasScreen()
        Try
            If Not _MyControlCenter Is Nothing Then
                If _MyControlCenter.SYS_States.SumState <> _MyOldSystemState Then
                    Dim Msg As String = ""
                    If _MyControlCenter.SYS_States.Maintenance Then
                        Msg = Msg & (ml_string(65, "Maintenance")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.K40ToLow_NotFound Then
                        Msg = Msg & (ml_string(324, "K40 to low or not found")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.AirFlowLessThen1Cubic Then
                        Msg = Msg & (String.Format(ml_string(325, "Air flow less than {0} m³/h"), _MyFHT59N3Par.MinAirFlowAlert)) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic Then
                        Msg = Msg & (String.Format(ml_string(335, "Air flow greater than {0} m³/h"), _MyFHT59N3Par.MaxAirFlowAlert)) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.HVOff Then
                        Msg = Msg & (ml_string(326, "HV Off")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.NoFilterstep Then
                        Msg = Msg & (ml_string(327, "No filterstep investigated")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.BypassOpen Then
                        Msg = Msg & (ml_string(328, "Bypass open")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.AnalyzationCancelled Then
                        Msg = Msg & (ml_string(329, "Analyzation routine cancelled")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                        If _MyFHT59N3Par.EnableCapturingDetectorTemperature Then
                            Msg = Msg & (ml_string(523, "Recording detector temperature is defect")) & vbCrLf
                        Else
                            Msg = Msg & (ml_string(331, "N2 Filling is going low")) & vbCrLf
                        End If
                    End If
                    If _MyFHT59N3Par.EcoolerEnabled And _MyControlCenter.SYS_States.EcoolerOff Then
                        Msg = Msg & (ml_string(511, "The E-Cooler is deactivated")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.FilterHasToBeChanged Then
                        Msg = Msg & (ml_string(332, "Filter has to be changed")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.CheckTempPressure Then
                        Msg = Msg & (ml_string(333, "Check Temperature and Pressure")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.K40ToBig Then
                        Msg = Msg & (ml_string(334, "K40 to big")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.DataTransferError Then
                        Msg = Msg & (ml_string(336, "Data transfer error")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent Then
                        Msg = Msg & (ml_string(337, "Spectrum dead time is bigger than 20 percent")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.UpsOnBattery Then
                        Msg = Msg & (ml_string(591, "UPS operation (on battery)")) & vbCrLf
                    End If
                    If _MyControlCenter.SYS_States.SumState > 0 Then
                        frmMeasScreen.ListView2.BackColor = Color.Yellow
                        Msg = Msg & vbCrLf
                        Msg = Msg & vbCrLf
                        Msg = Msg & "_______________________" & vbCrLf
                    Else
                        frmMeasScreen.ListView2.BackColor = Color.White
                    End If
                    Msg = Msg & (ml_string(338, "Sum State") & ": " & _MyControlCenter.SYS_States.SumState.ToString)
                    frmMeasScreen.ListView2.Text = Msg
                    _MyOldSystemState = _MyControlCenter.SYS_States.SumState
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub GUI_FillMsgMeasScreen()
        Dim Msg As String, sDot As String, Jetzt As Date
        Try
            frmMeasScreen.ListView1.Clear()
            frmMeasScreen.ListView1.SmallImageList = frmMain.ImageList1
            frmMeasScreen.ListView1.View = View.Details
            frmMeasScreen.ListView1.Columns.Add("Messages", frmMeasScreen.ListView1.Width - 21, HorizontalAlignment.Center)
            frmMeasScreen.ListView1.HeaderStyle = ColumnHeaderStyle.None
            frmMeasScreen.ListView1.HideSelection = True
            Msg = ""
            sDot = "GreenDot"
            For i As Integer = 0 To MsgListStatus.Count - 1
                Msg = MsgListStatus(i)
                Select Case MsgListStatusOn(i)
                    Case MessageStates.GREEN
                        sDot = "GreenDot"
                    Case MessageStates.YELLOW
                        sDot = "YellowDot"
                    Case MessageStates.RED
                        sDot = "RedDot"
                End Select
                frmMeasScreen.ListView1.Items.Insert(0, MsgListDate(i).ToShortDateString & " " & Format$(MsgListDate(i), "HH:mm:ss") & " : " & Msg, sDot)
                _MsgListFilled = True
            Next
            If Msg = "" Then
                Msg = ml_string(309, "No message available")
                Jetzt = Now
                frmMeasScreen.ListView1.Items.Insert(0, Jetzt.ToShortDateString & " " & Jetzt.ToShortTimeString & " : " & Msg, sDot)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Add the given parameters into the list-view object within the measurement GUI control
    ''' </summary>
    ''' <param name="Msg"></param>
    ''' <param name="MsgDate"></param>
    ''' <param name="MsgState"></param>
    ''' <remarks></remarks>
    Private Sub GUI_UpdateMsgMeasScreen(ByVal Msg As String, ByVal MsgDate As Date, ByVal MsgState As MessageStates)
        Dim sDot As String
        Try
            sDot = "GreenDot"
            Select Case MsgState
                Case MessageStates.GREEN
                    sDot = "GreenDot"
                Case MessageStates.YELLOW
                    sDot = "YellowDot"
                Case MessageStates.RED
                    sDot = "RedDot"
            End Select
            If frmMeasScreen.ListView1.Items.Count >= MAXLISTENTRIES Then
                frmMeasScreen.ListView1.Items.RemoveAt(frmMeasScreen.ListView1.Items.Count - 1)
            End If
            frmMeasScreen.ListView1.Items.Insert(0, MsgDate.ToShortDateString & " " & Format$(MsgDate, "HH:mm:ss") & " : " & Msg, sDot)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_FillDigInOutListMeasScreen()
        Try
            If Not _MyControlCenter Is Nothing Then
                frmMeasScreen.ListView3.Clear()
                frmMeasScreen.ListView3.SmallImageList = frmMain.ImageList1
                frmMeasScreen.ListView3.View = View.Details
                frmMeasScreen.ListView3.Columns.Add("Messages", frmMeasScreen.ListView3.Width - 10, HorizontalAlignment.Center)
                frmMeasScreen.ListView3.HeaderStyle = ColumnHeaderStyle.None
                frmMeasScreen.ListView3.HideSelection = True

                If _MyControlCenter.SPS_MotorOnOff Then
                    frmMeasScreen.ListView3.Items.Add(ml_string(47, "Filterstep") & ": " & ml_string(247, "running"), "RedDot")
                Else
                    frmMeasScreen.ListView3.Items.Add(ml_string(47, "Filterstep") & ": " & ml_string(56, "stopped"), "GreenDot")
                End If
                If _MyControlCenter.SPS_BypassOnOff Then
                    frmMeasScreen.ListView3.Items.Add("Filter / Bypass" & ": " & ml_string(246, "unlocked / open"), "RedDot")
                Else
                    frmMeasScreen.ListView3.Items.Add("Filter / Bypass" & ": " & ml_string(53, "locked / closed"), "GreenDot")
                End If
                If _MyControlCenter.SPS_PumpOnOff Then
                    frmMeasScreen.ListView3.Items.Add(ml_string(49, "Pump") & ": " & ml_string(54, "pumping"), "GreenDot")
                Else
                    frmMeasScreen.ListView3.Items.Add(ml_string(49, "Pump") & ": " & ml_string(56, "stopped"), "RedDot")
                End If
                If _MyControlCenter.SPS_ErrorOnOff Then
                    frmMeasScreen.ListView3.Items.Add(ml_string(50, "Error indication") & ": " & ml_string(248, "on"), "RedDot")
                Else
                    frmMeasScreen.ListView3.Items.Add(ml_string(50, "Error indication") & ": " & ml_string(55, "off"), "GreenDot")
                End If
                If _MyControlCenter.SPS_AlarmRelaisOnOff Then
                    frmMeasScreen.ListView3.Items.Add(ml_string(57, "Alarm indication") & ": " & ml_string(248, "on"), "RedDot")
                Else
                    frmMeasScreen.ListView3.Items.Add(ml_string(57, "Alarm indication") & ": " & ml_string(55, "off"), "GreenDot")
                End If

                If Not _MyFHT59N3Par.IsCanberraDetector Then
                    If _MyFHT59N3Par.EcoolerEnabled Then
                        If _MyControlCenter.SPS_EcoolerOnOff Then
                            frmMeasScreen.ListView3.Items.Add(ml_string(650, "ECooler") & ": " & ml_string(248, "on"), "GreenDot")
                        Else
                            frmMeasScreen.ListView3.Items.Add(ml_string(650, "ECooler") & ": " & ml_string(55, "off"), "RedDot")
                        End If
                    Else
                        If _MyControlCenter.SPS_HeatingOnOff Then
                            frmMeasScreen.ListView3.Items.Add(ml_string(51, "N2 Fill") & ": " & ml_string(248, "on"), "RedDot")
                        Else
                            frmMeasScreen.ListView3.Items.Add(ml_string(51, "N2 Fill") & ": " & ml_string(55, "off"), "GreenDot")
                        End If
                    End If
                Else
                    If _MyControlCenter.CanberraCryoCoolerStatus Then
                        frmMeasScreen.ListView3.Items.Add(ml_string(650, "ECooler") & ": " & ml_string(675, "Canberra"), "GreenDot")
                    Else
                        frmMeasScreen.ListView3.Items.Add(ml_string(650, "ECooler") & ": " & ml_string(675, "Canberra"), "RedDot")
                    End If
                End If



                If _MyControlCenter.MCA_GetHVState Then
                        frmMeasScreen.ListView3.Items.Add(ml_string(59, "High Voltage") & ": " & ml_string(248, "on"), "GreenDot")
                    Else
                        frmMeasScreen.ListView3.Items.Add(ml_string(59, "High Voltage") & ": " & ml_string(55, "off"), "RedDot")
                    End If
                    _DigInOutListFilled = True
                End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_UpdateDigInOutListMeasScreen()
        Try
            _MyOldDigInOutState = _MyDigInOutState

            Dim stateLv As ListView = frmMeasScreen.ListView3


            If _MyControlCenter.SPS_MotorOnOff Then
                If stateLv.Items(0).ImageKey <> "RedDot" Then
                    stateLv.Items(0).Text = (ml_string(47, "Filterstep") & ": " & ml_string(247, "running"))
                    stateLv.Items(0).ImageKey = "RedDot"
                End If
            Else
                If stateLv.Items(0).ImageKey <> "GreenDot" Then
                    stateLv.Items(0).Text = (ml_string(47, "Filterstep") & ": " & ml_string(56, "stopped"))
                    stateLv.Items(0).ImageKey = "GreenDot"
                End If
            End If

            If _MyControlCenter.SPS_BypassOnOff Then
                If stateLv.Items(1).ImageKey <> "RedDot" Then
                    stateLv.Items(1).Text = ("Filter / Bypass" & ": " & ml_string(246, "unlocked / open"))
                    stateLv.Items(1).ImageKey = "RedDot"
                End If
            Else
                If stateLv.Items(1).ImageKey <> "GreenDot" Then
                    stateLv.Items(1).Text = ("Filter / Bypass" & ": " & ml_string(53, "locked / closed"))
                    stateLv.Items(1).ImageKey = "GreenDot"
                End If
            End If

            If _MyControlCenter.SPS_PumpOnOff Then

                If stateLv.Items(2).ImageKey <> "GreenDot" Then
                    stateLv.Items(2).Text = (ml_string(49, "Pump") & ": " & ml_string(54, "pumping"))
                    stateLv.Items(2).ImageKey = "GreenDot"
                End If

            Else
                If stateLv.Items(2).ImageKey <> "RedDot" Then
                    stateLv.Items(2).Text = (ml_string(49, "Pump") & ": " & ml_string(56, "stopped"))
                    stateLv.Items(2).ImageKey = "RedDot"
                End If
            End If

            If _MyControlCenter.SPS_ErrorOnOff Then

                If stateLv.Items(3).ImageKey <> "RedDot" Then
                    stateLv.Items(3).Text = (ml_string(50, "Error indication") & ": " & ml_string(248, "on"))
                    stateLv.Items(3).ImageKey = "RedDot"
                End If

            Else
                If stateLv.Items(3).ImageKey <> "GreenDot" Then
                    stateLv.Items(3).Text = (ml_string(50, "Error indication") & ": " & ml_string(55, "off"))
                    stateLv.Items(3).ImageKey = "GreenDot"
                End If
            End If

            If _MyControlCenter.SPS_AlarmRelaisOnOff Then
                If stateLv.Items(4).ImageKey <> "RedDot" Then
                    stateLv.Items(4).Text = (ml_string(57, "Alarm indication") & ": " & ml_string(248, "on"))
                    stateLv.Items(4).ImageKey = "RedDot"
                End If

            Else
                If stateLv.Items(4).ImageKey <> "GreenDot" Then
                    stateLv.Items(4).Text = (ml_string(57, "Alarm indication") & ": " & ml_string(55, "off"))
                    stateLv.Items(4).ImageKey = "GreenDot"
                End If

            End If

            If Not _MyFHT59N3Par.IsCanberraDetector Then
                If _MyFHT59N3Par.EcoolerEnabled Then
                    If _MyControlCenter.SPS_EcoolerOnOff Then
                        If stateLv.Items(5).ImageKey <> "GreenDot" Then
                            stateLv.Items(5).Text = (ml_string(650, "ECooler") & ": " & ml_string(248, "on"))
                            stateLv.Items(5).ImageKey = "GreenDot"
                        End If
                    Else
                        If stateLv.Items(5).ImageKey <> "RedDot" Then
                            stateLv.Items(5).Text = (ml_string(650, "ECooler") & ": " & ml_string(55, "off"))
                            stateLv.Items(5).ImageKey = "RedDot"
                        End If
                    End If
                Else
                    If _MyControlCenter.SPS_HeatingOnOff Then

                        If stateLv.Items(5).ImageKey <> "RedDot" Then
                            stateLv.Items(5).Text = (ml_string(51, "N2 Fill") & ": " & ml_string(248, "on"))
                            stateLv.Items(5).ImageKey = "RedDot"
                        End If
                    Else

                        If stateLv.Items(5).ImageKey <> "GreenDot" Then
                            stateLv.Items(5).Text = (ml_string(51, "N2 Fill") & ": " & ml_string(55, "off"))
                            stateLv.Items(5).ImageKey = "GreenDot"
                        End If
                    End If
                End If
            Else
                stateLv.Items(5).Text = (ml_string(650, "ECooler") & ": " & ml_string(675, "Canberra"))
                stateLv.Items(5).ImageKey = "YellowDot"
            End If



            If _MyControlCenter.MCA_GetHVState Then

                If stateLv.Items(6).ImageKey <> "GreenDot" Then
                    stateLv.Items(6).Text = (ml_string(59, "High Voltage") & ": " & ml_string(248, "on"))
                    stateLv.Items(6).ImageKey = "GreenDot"
                End If

            Else

                If stateLv.Items(6).ImageKey <> "RedDot" Then
                    stateLv.Items(6).Text = (ml_string(59, "High Voltage") & ": " & ml_string(55, "off"))
                    stateLv.Items(6).ImageKey = "RedDot"
                End If

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Dialog routine for safe changing the ecooler state
    ''' </summary>
    ''' <returns>EcoolerGuiStates</returns>
    ''' <remarks>opens at least one message box</remarks>
    Public Function GUI_SafetyChangeEcoolerState() As EcoolerGuiStates
        Dim questionResult1, questionResult2 As MsgBoxResult

        If _MyFHT59N3Par.IsCanberraDetector Then
            questionResult1 = GUI_ShowMessageBox(ml_string(670, "The Canberra Cryo Cooler has to be controlled in a seperate GUI. Do you want to open it?"), ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)
            If questionResult1 = MsgBoxResult.Yes Then
                Try
                    Process.Start(_MyFHT59N3Par.CryoCoolExecutable) '("""C:\Program Files (x86)\CANBERRA\CP5 Control Panel\CP5.exe""")
                Catch e As Exception
                    MsgBox(e.Message)
                End Try
            End If
        Else
            If (_DetectorTemperaturValue = Double.MinValue) Then
                'we don't know the temperature, let the ecooler untouched
                Return EcoolerGuiStates.UNCHANGED
            End If

            If _MyControlCenter.SYS_States.EcoolerOff Then
                'alarm shows that ecooler is off
                questionResult1 = GUI_ShowMessageBox(MSG_WantToSwitchOnEcooler, ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)

                Dim status As FHT59N3_EmergencyStopCoolingStates = _MyControlCenter.SYS_States.EmergencyStopCoolingState

                If questionResult1 = MsgBoxResult.Yes Then
                    ' TODO: check detector temperature state machine (just reingelaufen in Inbetriebnahme 1.BAG-Gerät 30.Nov.15 in Bern)
                    If (_DetectorTemperaturValue < _MyFHT59N3Par.CrystalTooWarmTempThreshold) Then
                        ' Activate e-cooler via SPS
                        _MyControlCenter.SPS_EcoolerOn()

                        Return EcoolerGuiStates.ACTIVATED
                        'es soll möglich sein den Mechanismus händisch zu überlisten indem der Status im Config.XML auf 'COOLING_PHASE_PREPARED' gesetzt wird!
                    ElseIf (_DetectorTemperaturValue < _MyFHT59N3Par.CrystalWarmedUpTempThreshold And status <> FHT59N3_EmergencyStopCoolingStates.COOLING_PHASE_PREPARED) Then
                        Dim helpString As String = String.Format(MSG_CannotActivateEcooler, _MyFHT59N3Par.CrystalWarmedUpTempThreshold, Convert.ToInt32(_DetectorTemperaturValue))
                        questionResult2 = GUI_ShowMessageBox(helpString, "", "", ml_string(37, "Return"), MYCOL_WARNING, Color.Black)
                        Return EcoolerGuiStates.UNCHANGED
                    Else
                        ' Activate e-cooler via SPS
                        _MyControlCenter.SPS_EcoolerOn()

                        Return EcoolerGuiStates.ACTIVATED
                    End If
                End If
            Else
                'no alarm, it means that ecooler is on
                questionResult1 = GUI_ShowMessageBox(MSG_WantToSwitchOffEcooler, ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White)

                If questionResult1 = MsgBoxResult.Yes Then
                    ' Deactivate e-cooler via SPS
                    _MyControlCenter.SPS_EcoolerOff()
                    Return EcoolerGuiStates.DEACTIVATED
                End If
            End If
        End If
        Return EcoolerGuiStates.UNCHANGED
    End Function

    ''' <summary>
    ''' Show a message box with text and up to 3 buttons
    ''' </summary>
    ''' <param name="Message">message to show</param>
    ''' <param name="OKText">1.Button: set the text like "Yes", "OK"</param>
    ''' <param name="NOText">2.Button: set the text like "No"</param>
    ''' <param name="CANCELText">3.Button: set the text like "Cancel"</param>
    ''' <param name="BackColor">The background color (instance of Color)</param>
    ''' <param name="LetterColor">The foreground color (instance of Color)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GUI_ShowMessageBox(ByVal Message As String, ByVal OKText As String,
                                       ByVal NOText As String, ByVal CANCELText As String,
                                       ByVal BackColor As Color, ByVal LetterColor As Color) As MsgBoxResult
        Try
            frmMsgBox.LabelMsg.Text = Message
            If OKText <> "" Then
                frmMsgBox.BtnOK.Text = OKText
                frmMsgBox.BtnOK.Visible = True
            Else
                frmMsgBox.BtnOK.Visible = False
            End If
            If NOText <> "" Then
                frmMsgBox.BtnNo.Text = NOText
                frmMsgBox.BtnNo.Visible = True
            Else
                frmMsgBox.BtnNo.Visible = False
            End If
            frmMsgBox.BtnCancel.Text = ""
            If CANCELText <> "" Then
                frmMsgBox.BtnCancel.Text = CANCELText
                frmMsgBox.BtnCancel.Visible = True
            Else
                frmMsgBox.BtnCancel.Visible = False
            End If
            frmMsgBox.BackColor = BackColor
            frmMsgBox.LabelMsg.ForeColor = LetterColor

            'Aus der Notwendigkeit geboren das Netlog32 gestartet wird und sich den 
            'Focus zuweist. Wenn dann beim Hochlauf des Monitors eine MessageBox erscheint
            'ist diese nicht fokussiert und kann auch mit Return-Taste nicht mehr quittiert werden!
            Dim ownProcId As Integer = Process.GetCurrentProcess().Id
            AppActivate(ownProcId)

            frmMsgBox.ShowDialog()

            Return frmMsgBox.MsgBoxReturnCode
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Function

    ''' <summary>
    ''' Show a message box with text, up to 2 checkboxes and up to 3 buttons
    ''' </summary>
    ''' <param name="Message">message to show</param>
    ''' <param name="OKText">1.Button: set the text like "Yes", "OK"</param>
    ''' <param name="NOText">2.Button: set the text like "No"</param>
    ''' <param name="CANCELText">3.Button: set the text like "Cancel"</param>
    ''' <param name="CheckBoxText1">1.Checkbox: set the text like "enable action 1"</param>
    ''' <param name="CheckBoxResult1">in/out: checkbox 1 is checked</param>
    ''' <param name="CheckBoxText2">2.Checkbox: set the text like "enable action 2"</param>
    ''' <param name="CheckBoxResult2">in/out: checkbox 2 is checked</param>
    ''' <param name="BackColor">The background color (instance of Color)</param>
    ''' <param name="LetterColor">The foreground color (instance of Color)</param>
    ''' <returns>values of enum MsgBoxResult</returns>
    ''' <remarks></remarks>
    Public Function GUI_ShowExtendedMessageBox(ByVal Message As String, ByVal OKText As String,
                                       ByVal NOText As String, ByVal CANCELText As String,
                                       ByVal CheckBoxText1 As String, ByRef CheckBoxResult1 As Boolean, ByVal CheckBoxText2 As String, ByRef CheckBoxResult2 As Boolean,
                                       ByVal BackColor As Color, ByVal LetterColor As Color) As MsgBoxResult
        Try
            frmExtendedMsgBox.LabelMsg.Text = Message

            If CheckBoxText1 <> "" Then
                frmExtendedMsgBox.CheckBox1.Text = CheckBoxText1
                frmExtendedMsgBox.CheckBox1.Visible = True
                frmExtendedMsgBox.CheckBox1.Checked = CheckBoxResult1
            Else
                frmExtendedMsgBox.CheckBox1.Visible = False
            End If
            If CheckBoxText2 <> "" Then
                frmExtendedMsgBox.CheckBox2.Text = CheckBoxText2
                frmExtendedMsgBox.CheckBox2.Visible = True
                frmExtendedMsgBox.CheckBox2.Checked = CheckBoxResult2
            Else
                frmExtendedMsgBox.CheckBox2.Visible = False
            End If
            If OKText <> "" Then
                frmExtendedMsgBox.BtnOK.Text = OKText
                frmExtendedMsgBox.BtnOK.Visible = True
            Else
                frmExtendedMsgBox.BtnOK.Visible = False
            End If
            If NOText <> "" Then
                frmExtendedMsgBox.BtnNo.Text = NOText
                frmExtendedMsgBox.BtnNo.Visible = True
            Else
                frmExtendedMsgBox.BtnNo.Visible = False
            End If
            frmExtendedMsgBox.BtnCancel.Text = ""
            If CANCELText <> "" Then
                frmExtendedMsgBox.BtnCancel.Text = CANCELText
                frmExtendedMsgBox.BtnCancel.Visible = True
            Else
                frmExtendedMsgBox.BtnCancel.Visible = False
            End If
            frmExtendedMsgBox.BackColor = BackColor
            frmExtendedMsgBox.LabelMsg.ForeColor = LetterColor
            frmExtendedMsgBox.CheckBox1.ForeColor = LetterColor
            frmExtendedMsgBox.CheckBox2.ForeColor = LetterColor
            frmExtendedMsgBox.ShowDialog()

            ' set return values
            CheckBoxResult1 = frmExtendedMsgBox.CheckBox1.Checked
            CheckBoxResult2 = frmExtendedMsgBox.CheckBox2.Checked

            Return frmExtendedMsgBox.MsgBoxReturnCode
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Function

    Public Sub GUI_ShowNextAnalyzationMinute()
        Try
            GUI_SetMessage(ml_string(253, "Next evaluation: ") & _AnalyzeMinuteDate.ToString("HH:mm") & " h", MessageStates.GREEN)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_ShowNextFilterStepTime()
        Try
            If Not _MyControlCenter.SYS_States.NoFilterstep Then
                GUI_SetMessage(ml_string(254, "Next tape transport: ") & _NextFilterStepMinuteDate.ToString("HH:mm") & " h", MessageStates.GREEN)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_ShowAnalyzationFile(ByVal File As String, ByVal RTB As System.Windows.Forms.RichTextBox)
        Try
            RTB.Text = System.IO.File.ReadAllText(File)
            frmMeasScreen.SpectralDisplay.ShowInfo = False
            frmMeasScreen.RtbAnalyzeData.Visible = True
            GUI_ResizeMeasScreen()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub GUI_ShowMonitorStatesInControlMenu(ByVal ShowAsMDIChild As Boolean)
        Try
            If ShowAsMDIChild Then
                frmControlMenu.IsMdiContainer = True
                frmControlMenu.WindowState = FormWindowState.Maximized
                frmStates.MdiParent = frmControlMenu
                frmStates.FillFormStates()
                frmStates.Show()
                frmStates.BtnClose.Visible = False
                frmStates.Left = frmControlMenu.LblFilterstep.Left + frmControlMenu.LblFilterstep.Width + 10
                frmStates.Top = frmControlMenu.LblFilterstep.Top
                frmStates.BackColor = MYCOL_THERMOGREEN
            Else
                frmControlMenu.IsMdiContainer = False
                frmControlMenu.WindowState = FormWindowState.Normal
                frmStates.MdiParent = Nothing
                frmStates.BtnClose.Visible = True
                frmStates.BackColor = MYCOL_THERMOGREEN
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' show a splash screen during starup phase and hide the application until full initialization
    ''' </summary>
    ''' <param name="Show">true: show splash screen, false: hide splash screen and show application</param>
    ''' <remarks>interoperates with class frmMain</remarks>
    Public Sub GUI_ShowSplashScreen(ByVal Show As Boolean)
        Try
            If Show Then
                frmSplashScreen.Show()
                frmMain.Size = frmSplashScreen.Size
                frmMain.Top = frmSplashScreen.Top
                frmMain.Left = frmSplashScreen.Left
            Else
                frmMain.WindowState = FormWindowState.Maximized
                Dim S As New System.Drawing.Size
                'If System.Threading.Thread.CurrentThread.CurrentUICulture.ToString.Contains("de") Then
                '    S.Width = 205
                '    S.Height = 163
                'Else
                '    S.Width = 185
                '    S.Height = 149
                'End If
                'frmMain.TableLayoutPanel1.Size = S
                frmSplashScreen.Close()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub GUI_CloseAllMenus()
        Try
            frmFilterMenu.Close()
            frmOperationMenu.Close()
            frmFileMenu.Close()
            frmMaintenanceMenu.Close()
            frmCalibrationMenu.Close()
            frmMCAMenu.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub GUI_NextROI()
        Try
            frmMeasScreen.SpectralDisplay.NextRoi()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_PrevROI()
        Try
            frmMeasScreen.SpectralDisplay.PreviousRoi()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_AddROI()
        Try
            frmMeasScreen.SpectralDisplay.AddRoi()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_DeleteROI()
        Try
            frmMeasScreen.SpectralDisplay.DeleteRoi()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_DeleteAllROIs()
        Try
            frmMeasScreen.SpectralDisplay.DeleteAllRois()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_SaveROIs()
        Try
            Dim result As DialogResult
            frmROIMenu.SaveFileDialog1.InitialDirectory = _MonitorConfigDirectory
            frmROIMenu.SaveFileDialog1.Filter = "ROI File (*.roi)|*.roi"
            frmROIMenu.SaveFileDialog1.FilterIndex = 1
            result = frmROIMenu.SaveFileDialog1.ShowDialog()
            If (result = DialogResult.OK) And (frmROIMenu.SaveFileDialog1.FileName <> "") Then
                frmMeasScreen.SpectralDisplay.SaveRois(frmROIMenu.SaveFileDialog1.FileName, True)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub GUI_LoadROIs()
        Try
            Dim result As DialogResult
            frmROIMenu.OpenFileDialog1.InitialDirectory = _MonitorConfigDirectory
            frmROIMenu.OpenFileDialog1.Filter = "ROI File (*.roi)|*.roi"
            frmROIMenu.OpenFileDialog1.FilterIndex = 1
            result = frmROIMenu.OpenFileDialog1.ShowDialog()
            If (result = DialogResult.OK) And (frmROIMenu.OpenFileDialog1.FileName <> "") Then
                frmMeasScreen.SpectralDisplay.LoadRois(frmROIMenu.OpenFileDialog1.FileName, True)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Messages"

    Public Sub GUI_SetMessage(ByVal MyMessage As String, ByVal Status As MessageStates)
        Try
            Dim DestinationFile As String = "FHT59N3Messages"
            Dim DestinationFileExtension As String = ".txt"
            Dim DestinationFileComplete As String
            Dim Msg As String = ""
            Dim DelMsgFile As Boolean = False
            Dim MsgDate As Date = Now

            If MsgListStatus.Count >= MAXLISTENTRIES Then
                MsgListStatus.RemoveAt(0)
                MsgListStatusOn.RemoveAt(0)
                MsgListDate.RemoveAt(0)
                DelMsgFile = True
            End If
            MsgListStatus.Add(MyMessage)
            MsgListStatusOn.Add(Status)
            MsgListDate.Add(MsgDate)
            isMsgListChanged = True
            GUI_UpdateMsgMeasScreen(MyMessage, MsgDate, Status)

            DestinationFileComplete = _AnalyzationFilesDirectory & "\" & DestinationFile & DestinationFileExtension
            If Not DelMsgFile Then
                Msg = Now.ToShortDateString & " " & Now.ToLongTimeString & "!/!" & MyMessage & "!/!" & CType(Status, Integer).ToString & vbCrLf
                My.Computer.FileSystem.WriteAllText(DestinationFileComplete, Msg, True)
            Else
                'Hier wird nur die Datei mit einer maximalen Anzahl von Zeilen (MAXLISTENTRIES) geschrieben
                If System.IO.File.Exists(DestinationFileComplete) Then System.IO.File.Delete(DestinationFileComplete)
                For i As Integer = 0 To MsgListStatus.Count - 1
                    Msg = MsgListDate(i).ToShortDateString & " " & MsgListDate(i).ToLongTimeString & "!/!" & MsgListStatus(i) & "!/!" & CType(MsgListStatusOn(i), Integer).ToString & vbCrLf
                    My.Computer.FileSystem.WriteAllText(DestinationFileComplete, Msg, True)
                Next
            End If

            'FHTNT-14: additionally create monthly
            If (_MyFHT59N3Par.GenerateMonthlySystemMessagesFiles) Then

                DestinationFileComplete = _AnalyzationFilesDirectory & "\" _
                    & DestinationFile & "_" & Year(Now) & "_" & Format(Month(Now), "00") _
                    & DestinationFileExtension
                My.Computer.FileSystem.WriteAllText(DestinationFileComplete, Msg, True)
            End If

            SYS_SaveMessageToCollectionFile(Now.ToShortDateString & " " & Now.ToLongTimeString & "!/!" & MyMessage & "!/!" & CType(Status, Integer).ToString & vbCrLf)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub GUI_ReadMessages()
        Dim DestinationFile As String = "FHT59N3Messages.txt"
        Dim Msg() As String
        Dim MsgParts() As String
        Dim M As String
        Dim MinIndex As Integer
        Dim Sep() As String = {"!/!"}
        Try
            DestinationFile = _AnalyzationFilesDirectory & "\" & DestinationFile
            If System.IO.File.Exists(DestinationFile) Then
                Msg = My.Computer.FileSystem.ReadAllText(DestinationFile).Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                MinIndex = Msg.Length - MAXLISTENTRIES - 1
                If MinIndex < 0 Then
                    MinIndex = 0
                End If
                For i As Integer = MinIndex To Msg.Length - 1
                    M = Msg(i)
                    MsgParts = M.Split(Sep, StringSplitOptions.RemoveEmptyEntries)
                    MsgListDate.Add(CDate(MsgParts(0)))
                    MsgListStatus.Add(MsgParts(1))
                    MsgListStatusOn.Add(CType(MsgParts(2), MessageStates))
                Next
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

#End Region

#Region "Spectras"

    Public Sub GUI_OpenSpectrum(ByVal Source As Object, ByVal SpectrumType As SpectraTypes, ByVal ShowSpectrum As Boolean, ByVal ShowInfo As Boolean, ByVal Display As AxCanberraDataDisplayLib.AxMvc)
        Try
            If SpectrumType = SpectraTypes.ONLINE Then
                _SpectraFileOpen = True
                GUI_CloseSpectrum(False, ShowSpectrum)
                If ShowSpectrum Then
                    Select Case _MyControlCenter.MCAType
                        Case FHT59N3_ControlCenter.MCATypes.Canberra_Lynx
                            Display.CurrentDataSource = CType(Source, CanberraDeviceAccessLib.DeviceAccess)
                            
                            frmMain.Text = "FHT59N3T - EBIN Lynx"

                        Case FHT59N3_ControlCenter.MCATypes.Ortec_DspecPlus
                    End Select
                End If
            ElseIf SpectrumType = SpectraTypes.OFFLINE Then
                'Assume we have vald file name and try to open
                _SpectraFileOpen = False
                GUI_CloseSpectrum(False, ShowSpectrum)
                _SpectraFile = New CanberraDataAccessLib.DataAccess
                _SpectraFile.Open(CType(Source, String), CanberraDataAccessLib.OpenMode.dReadWrite)
                _SpectraFileOpen = True
                If ShowSpectrum Then
                    Display.CurrentDataSource = _SpectraFile
                    Dim Filename() As String = CType(Source, String).Split("\".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                    frmMain.Text = "FHT59N3T - " & Filename(Filename.Length - 1)
                    
                End If
            End If
            frmMeasScreen.SpectralDisplay.DisplayUpdateRate = 3000
            frmMeasScreen.SpectralDisplay.ShowSpectralPeakInformation = False

            If Not _MyFHT59N3Par.DisplayPoints Then
                frmMeasScreen.SpectralDisplay.DisplayType = DisplayTypes.SpectroscopyPlot
                frmMeasScreen.SpectralDisplay.DisplayPoints = False
            End If

            If ShowInfo Then
                frmMeasScreen.SpectralDisplay.ShowInfo = ShowInfo
                frmMCAParameter_GainStabiAdd.SpectralDisplay.ShowInfo = ShowInfo
                frmMeasScreen.RtbAnalyzeData.Visible = False
                If SpectrumType = SpectraTypes.OFFLINE Then
                    frmMeasScreen.SpectralDisplay.ShowSpectralPeakInformation = True
                End If
            Else
                frmMeasScreen.SpectralDisplay.ShowInfo = ShowInfo
                frmMCAParameter_GainStabiAdd.SpectralDisplay.ShowInfo = ShowInfo
                frmMeasScreen.RtbAnalyzeData.Visible = True
            End If
            GUI_ResizeMeasScreen()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub GUI_CloseSpectrum(ByVal Flush As Boolean, ByVal UnShow As Boolean)
        Try
            If Not _SpectraFile Is Nothing Then
                If Flush Then
                    _SpectraFile.Flush()
                End If
                _SpectraFile.Close()
                _SpectraFile = Nothing
            End If
            If UnShow Then
                frmMeasScreen.SpectralDisplay.Close(CanberraDataDisplayLib.CloseOptions.NoUpdate)
                frmMeasScreen.SpectralDisplay.CloseAll()
                frmMCAParameter_GainStabiAdd.SpectralDisplay.Close(CanberraDataDisplayLib.CloseOptions.NoUpdate)
                frmMCAParameter_GainStabiAdd.SpectralDisplay.CloseAll()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

#End Region

End Module
