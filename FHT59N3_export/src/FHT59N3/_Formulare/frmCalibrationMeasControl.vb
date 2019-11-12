Imports Thermo.Rmp.CM.Controls

Public Class frmCalibrationMeasControl

    Private Sub frmCalibrationMeasControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            TBox_DateTime.Text = Format(Now, "dd.MM.yy HH:mm")

            Select Case _CalibrationType

                Case CalibTypes.CalibFar
                    Me.Text = ml_string(294, "FHT59N3T - Calibration geometry FAR")
                    Lbl_CommandText.Text = ml_string(295, "Please put the MIXED source in geometry FAR.")
                    TBox_MeasTime.Text = 40
                    BtnStartCalib.Text = ml_string(296, "Start Calibration")
                Case CalibTypes.CalibNear
                    Me.Text = ml_string(297, "FHT59N3T - Calibration geometry NEAR")
                    Lbl_CommandText.Text = ml_string(298, "Please put the CS-137 source in geometry NEAR.")
                    TBox_MeasTime.Text = 5
                    BtnStartCalib.Text = ml_string(296, "Start Calibration")
                Case CalibTypes.CalibNearMix
                    Me.Text = ml_string(297, "FHT59N3T - Calibration geometry NEAR")
                    Lbl_CommandText.Text = ml_string(564, "Please put the MIXED source in geometry NEAR.")
                    TBox_MeasTime.Text = 40
                    BtnStartCalib.Text = ml_string(296, "Start Calibration")
                Case CalibTypes.CalibRccCs137
                    Me.Text = ml_string(299, "FHT59N3T - Calibration RCC")
                    Lbl_CommandText.Text = ml_string(298, "Please put the CS-137 source in geometry NEAR.")
                    TBox_MeasTime.Text = 5
                    BtnStartCalib.Text = ml_string(300, "Start RCC")
                Case CalibTypes.CalibRccMix
                    Me.Text = ml_string(299, "FHT59N3T - Calibration RCC")
                    Lbl_CommandText.Text = ml_string(564, "Please put the MIXED source in geometry NEAR.")
                    TBox_MeasTime.Text = 5
                    BtnStartCalib.Text = ml_string(300, "Start RCC")
                Case CalibTypes.CalibFree
                    Me.Text = ml_string(301, "FHT59N3T - Free Measurement")
                    Lbl_CommandText.Text = ml_string(302, "Please specify the measurement time.")
                    TBox_MeasTime.Text = 5
                    BtnStartCalib.Text = ml_string(14, "Start measurement")
                Case CalibTypes.CalibBackground
                    Me.Text = ml_string(630, "FHT59N3T - Measurement of background spectrum")
                    Lbl_CommandText.Text = ml_string(632, "Please specify the measurement time for the background spectrum.")
                    TBox_MeasTime.Text = 180
                    BtnStartCalib.Text = ml_string(14, "Start measurement")

            End Select

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStartCalib.Click
        Try
            'Kalibriermessung starten
            If IsNumeric(TBox_MeasTime.Text) Then
                MCA_StartCalibrationMeasurement(CType(TBox_MeasTime.Text, Integer))
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

    Private Sub TBox_DateTime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_DateTime.Click
    End Sub

    Private Sub TBox_MeasTime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_MeasTime.Click
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmCalibrationMeasControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalibrationMeasControl))
    resources.ApplyResources(Me.BtnClose, "BtnClose")
    resources.ApplyResources(Me.BtnStartCalib, "BtnStartCalib")
    resources.ApplyResources(Me, "$this")
    resources.ApplyResources(Me.Label1, "Label1")
    resources.ApplyResources(Me.Label3, "Label3")
  End Sub
End Class