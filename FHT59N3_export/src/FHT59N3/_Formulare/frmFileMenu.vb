Public Class frmFileMenu

    Private Sub BtnAnalyzeSpectrum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnalyzeSpectrum.Click
        Try
            GUI_CloseAllMenus()
            OpenFileDialog1.InitialDirectory = _SpectraDirectory
            OpenFileDialog1.Filter = ml_string(304, "Spectras (*.cnf)|*.cnf")
            OpenFileDialog1.FilterIndex = 1
            OpenFileDialog1.ShowDialog()

            Dim analyzeOption = If(checkBoxCheckAlarms.Checked, FHT59N3_ComputeSpectrumAnalyze.AnalyzeOptions.ForceAlarmCheck,
                                   FHT59N3_ComputeSpectrumAnalyze.AnalyzeOptions.None)

            Dim analyzer = New FHT59N3_ComputeSpectrumAnalyze
            analyzer.MCA_AnalyzeSpectrum(OpenFileDialog1.FileName, SPECTYPE_SONDERAUSWERTUNG, analyzeOption)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnShowSpectrum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnShowSpectrum.Click
        Try
            GUI_CloseAllMenus()
            OpenFileDialog1.InitialDirectory = _SpectraDirectory
            OpenFileDialog1.Filter = ml_string(304, "Spectras (*.cnf)|*.cnf")
            OpenFileDialog1.FilterIndex = 1
            OpenFileDialog1.ShowDialog()
            If OpenFileDialog1.FileName <> "" Then
                GUI_OpenSpectrum(OpenFileDialog1.FileName, SpectraTypes.OFFLINE, True, True, frmMeasScreen.SpectralDisplay)
            End If
            GUI_ShowForm(frmMeasScreen.PanelfrmMeasScreen)
            frmMain.BtnSpectrum.Text = ml_string(105, "Actual Spectrum")
            _ShowSpectrum = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnShowResultFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnShowResultFile.Click
        Try
            GUI_CloseAllMenus()
            Dim result As DialogResult
            OpenFileDialog1.InitialDirectory = _AnalyzationFilesDirectory
            OpenFileDialog1.Filter = ml_string(305, "Results (*.dat)|*.dat")
            OpenFileDialog1.FilterIndex = 1
            result = OpenFileDialog1.ShowDialog()
            If (result = DialogResult.OK) Then
                frmResultFile.DateiLesen(OpenFileDialog1.FileName)
                frmResultFile.ShowDialog()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturn.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmFileMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileMenu))
    resources.ApplyResources(Me.BtnAnalyzeSpectrum, "BtnAnalyzeSpectrum")
    resources.ApplyResources(Me.BtnReturn, "BtnReturn")
    resources.ApplyResources(Me.BtnShowResultFile, "BtnShowResultFile")
    resources.ApplyResources(Me.BtnShowSpectrum, "BtnShowSpectrum")
    resources.ApplyResources(Me, "$this")
  End Sub
End Class