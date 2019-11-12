Imports System.Reflection
Imports AxCanberraDataDisplayLib


Public Class frmServiceScreen

    Dim KEY_AIR_FLOW_SPECTRUM As String = "1"
    Dim KEY_SPECTRUM_ADDED_COUNT As String = "2"
    Dim KEY_SPECTRUM_START_TIME As String = "3"
    Dim KEY_SPECTRUM_END_TIME As String = "4"
    Dim KEY_FILTER_PRESSURE As String = "5"
    Dim KEY_BEZEL_PRESSURE As String = "6"
    Dim KEY_ENVIRONMENT_PRESSURE As String = "7"
    Dim KEY_ACTUAL_AIR_FLOW As String = "8"
    Dim KEY_MEAN_AIR_FLOW As String = "9"
    Dim KEY_N2_FILL_VALUE As String = "10"
    Dim KEY_ENVIRONMENT_TEMPERATURE As String = "11"
    Dim KEY_ECOOLER_STATUS As String = "12"
    Dim KEY_EXTERNAL_TEMPERATURE As String = "13"


    Public Sub frmServiceScreen_Load()
        Try

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub frmServiceScreen_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            If Not e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
                Me.Visible = False
            End If
            'TimerCheckChanges.Enabled = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub New()
        InitializeComponent()

        ControlExtensions.EnableDoubleBuffered(ServiceList, True)

        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub



    Public Sub GUI_FillServiceForm()
        Try
            'Irgendeinstatus hat sich geändert, Liste neu aufbauen
            ServiceList.Items.Clear()


            Dim spectraDisplay As AxMvc = frmMeasScreen.SpectralDisplay
            Dim canRead As Boolean = _SpectraFileOpen AndAlso spectraDisplay.IsAccessible

            ' Zeile 'Luftdurchsatz'
            Dim Lvi As ListViewItem = New ListViewItem(ml_string(313, "Air Flow of Spectrum"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_AIR_FLOW_SPECTRUM
            ServiceList.Items.Add(Lvi)

            'Zeile 'Anzahl addierter Spalten'
            Lvi = New ListViewItem(ml_string(314, "Spectrum added"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_SPECTRUM_ADDED_COUNT
            ServiceList.Items.Add(Lvi)

            'Zeile 'Startzeit der Messung'
            Lvi = New ListViewItem(ml_string(315, "Start time"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_SPECTRUM_START_TIME
            ServiceList.Items.Add(Lvi)


            'Zeile 'Ende Zeit der Messung'
            Lvi = New ListViewItem(ml_string(316, "End time"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_SPECTRUM_END_TIME
            ServiceList.Items.Add(Lvi)


            Lvi = New ListViewItem(ml_string(317, "Filter pressure [hPa]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_FILTER_PRESSURE
            ServiceList.Items.Add(Lvi)

            Lvi = New ListViewItem(ml_string(318, "Bezel pressure [hPa]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_BEZEL_PRESSURE
            ServiceList.Items.Add(Lvi)

            Lvi = New ListViewItem(ml_string(319, "Environment pressure [hPa]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_ENVIRONMENT_PRESSURE
            ServiceList.Items.Add(Lvi)


            Lvi = New ListViewItem(ml_string(320, "Actual air flow [m³/h]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_ACTUAL_AIR_FLOW
            ServiceList.Items.Add(Lvi)

            Lvi = New ListViewItem(ml_string(321, "Mean air flow [m³/h]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_MEAN_AIR_FLOW
            ServiceList.Items.Add(Lvi)

            If _MyFHT59N3Par.N2FillThreshold > 0 Then
                Lvi = New ListViewItem(ml_string(322, "N2 filling [mV]"))
                Lvi.SubItems.Add("")
                Lvi.Name = KEY_N2_FILL_VALUE
                ServiceList.Items.Add(Lvi)
            End If

            Lvi = New ListViewItem(ml_string(323, "Temperature [°C]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_ENVIRONMENT_TEMPERATURE
            ServiceList.Items.Add(Lvi)

            Lvi = New ListViewItem(ml_string(660, "External Temperature [°C]"))
            Lvi.SubItems.Add("")
            Lvi.Name = KEY_EXTERNAL_TEMPERATURE
            ServiceList.Items.Add(Lvi)


            If _MyFHT59N3Par.EcoolerEnabled Then
                Lvi = New ListViewItem(ml_string(614, "ECooler state marker"))
                Lvi.SubItems.Add("")
                Lvi.Name = KEY_ECOOLER_STATUS
                ServiceList.Items.Add(Lvi)

            End If

            GUI_UpdateServiceForm()

            'Spezielle Einstellung um die Wert-Spalte zu "maximieren"....
            ServiceList.Columns(1).Width = -2


        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub GUI_UpdateServiceForm()
        Try

            'SuspendLayout()
            'ServiceList.BeginUpdate()

            Dim spectraDisplay As AxMvc = frmMeasScreen.SpectralDisplay
            Dim canRead As Boolean = _SpectraFileOpen AndAlso spectraDisplay.IsAccessible

            If (canRead) Then
                ServiceList.Items(KEY_AIR_FLOW_SPECTRUM).SubItems(1).Text = frmMeasScreen.SpectralDisplay.CurrentDataSource.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP2)
                ServiceList.Items(KEY_SPECTRUM_ADDED_COUNT).SubItems(1).Text = frmMeasScreen.SpectralDisplay.CurrentDataSource.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3)
                ServiceList.Items(KEY_SPECTRUM_START_TIME).SubItems(1).Text = frmMeasScreen.SpectralDisplay.CurrentDataSource.Param(CanberraDataAccessLib.ParamCodes.CAM_T_ASPSTR) & " " & _TimeZone
                ServiceList.Items(KEY_SPECTRUM_END_TIME).SubItems(1).Text = frmMeasScreen.SpectralDisplay.CurrentDataSource.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR4) & " " & _TimeZone
            Else
                ServiceList.Items(KEY_AIR_FLOW_SPECTRUM).SubItems(1).Text = "---"
                ServiceList.Items(KEY_SPECTRUM_ADDED_COUNT).SubItems(1).Text = "---"
                ServiceList.Items(KEY_SPECTRUM_START_TIME).SubItems(1).Text = "---"
                ServiceList.Items(KEY_SPECTRUM_END_TIME).SubItems(1).Text = "---"

            End If


            ServiceList.Items(KEY_FILTER_PRESSURE).SubItems(1).Text = Format(_PressureFilter, "0.00") + " (" + ConvertBitsToMillivolt(_MyControlCenter.SPS_PressureFilter) + " mV)"
            ServiceList.Items(KEY_BEZEL_PRESSURE).SubItems(1).Text = Format(_PressureBezel, "0.00") + " (" + ConvertBitsToMillivolt(_MyControlCenter.SPS_PressureBezel) + " mV)"
            ServiceList.Items(KEY_ENVIRONMENT_PRESSURE).SubItems(1).Text = Format(_PressureEnvironment, "0.00") + " (" + ConvertBitsToMillivolt(_MyControlCenter.SPS_PressureEnvironment) + " mV)"
            ServiceList.Items(KEY_ACTUAL_AIR_FLOW).SubItems(1).Text = Format(_AirFlowActual, "0.00")
            ServiceList.Items(KEY_MEAN_AIR_FLOW).SubItems(1).Text = Format(_AirFlowMean, "0.00")
            ServiceList.Items(KEY_ENVIRONMENT_TEMPERATURE).SubItems(1).Text = Format(_Temperature, "0.00") + " (" + ConvertBitsToMillivolt(_MyControlCenter.SPS_Temperature) + " mV)"
            ServiceList.Items(KEY_EXTERNAL_TEMPERATURE).SubItems(1).Text = Format(_ExternalTemperature, "0.00") + " (" + ConvertBitsToMillivolt(_MyControlCenter.SPS_ExternalTemperature) + " mV)"


            'Weitere konditionale Statuswerte:
            If _MyFHT59N3Par.N2FillThreshold > 0 Then
                ServiceList.Items(KEY_N2_FILL_VALUE).SubItems(1).Text = Format(_N2FillValue, "0.00")
            End If

            If _MyFHT59N3Par.EcoolerEnabled Then
                ServiceList.Items(KEY_ECOOLER_STATUS).SubItems(1).Text = _MyEcoolerController._EmergencyStopCoolingState.ToString()
            End If


        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        Finally
            'ServiceList.EndUpdate()
            'ResumeLayout()

        End Try
        
    End Sub

    Private Function ConvertBitsToMillivolt(bitValueDecimal As Double) As String
        Dim convertBitsMv As Double = 10000 / 4095
        Return CInt(convertBitsMv * bitValueDecimal).ToString()
    End Function

    Private Sub frmServiceScreen_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmServiceScreen))
    End Sub
End Class

Public Class ControlExtensions

    Public Shared Sub EnableDoubleBuffered(control As Control, enable As Boolean)
        Dim doubleBufferPropertyInfo = control.[GetType]().GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)
        doubleBufferPropertyInfo.SetValue(control, enable, Nothing)
    End Sub
End Class