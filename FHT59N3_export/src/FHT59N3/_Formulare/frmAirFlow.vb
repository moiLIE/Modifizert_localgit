Imports Thermo.Rmp.CM.Controls

Public Class frmAirFlow

    ''' <summary>
    ''' Start measurement
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnStartMeas_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStartMeas.Click
        Try
            If IsNumeric(TBox_NumberValues.Text) Then
                If IsNumeric(TBox_TrueAirflow.Text) Then
                    _NumberOfAirFlowMeasurementsCalib = CType(TBox_NumberValues.Text, Integer)
                    _AirFlowTrue = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_TrueAirflow.Text), Double)
                    'Wahrer Luftdurchsatz wird in Betriebs-m³ gemessen, die Druck und Durchflussmessung kann aber nur Norm-m³
                    'also umrechnen in Morm-m³
                    Trace.TraceInformation("Luftdurchsatz-Kalib: Eingegebener Wert: " & _AirFlowTrue.ToString)
                    _AirFlowTrue = _AirFlowTrue / ((1013 / _PressureEnvironment) * ((_Temperature + 273.15) / 273.15))
                    Trace.TraceInformation("Luftdurchsatz-Kalib: Wert in Norm-m³: " & _AirFlowTrue.ToString)
                    _StartOfAirFlowMeasurementCalib = Now
                    _AirFlowSumCounterCalib = _NumberOfAirFlowMeasurementsCalib
                    _FactorBezelCalib = 0
                    _CalibAirFlow = True

                    TBox_NumberValues.Enabled = False
                    BtnStartMeas.Enabled = False
                    BtnApply.Enabled = False
                    BtnClose.Enabled = False
                    Me.Cursor = Cursors.WaitCursor
                Else
                    GUI_ShowMessageBox(ml_string(290, "Please type a correct numerical value for true airflow in."), "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                End If
            Else
                GUI_ShowMessageBox(ml_string(291, "Please type a correct numerical value for number of measurements in."), "OK", "", "", MYCOL_THERMOGREEN, Color.White)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnApply.Click
        Try
            _MyFHT59N3Par.FactorBezel = _FactorBezelCalib

            'update configuration file
            SYS_WriteSettings()
            _MyControlCenter.SPS_SetAirflowBezelFactor(_FactorBezelCalib)

            Me.Close()
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

    Private Sub TBox_NumberValues_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_NumberValues.Click
    End Sub

    Private Sub TBox_TrueAirflow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_TrueAirflow.Click
    End Sub

    Private Sub TBox_Result_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_Result.Click
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmAirFlow_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAirFlow))
    resources.ApplyResources(Me.BtnApply, "BtnApply")
    resources.ApplyResources(Me.BtnClose, "BtnClose")
    resources.ApplyResources(Me.BtnStartMeas, "BtnStartMeas")
    resources.ApplyResources(Me, "$this")
    resources.ApplyResources(Me.Label1, "Label1")
    resources.ApplyResources(Me.Label2, "Label2")
    resources.ApplyResources(Me.Label3, "Label3")
  End Sub
End Class