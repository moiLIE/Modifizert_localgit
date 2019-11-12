Imports System.Windows.Forms.DataVisualization.Charting
Imports FHT59N3Core
Imports SharpGraphLib



Public Class frmDetectorTemperature

    Private Sub frmDetectorTemperature_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        comboBoxShownRange.Items.Clear()
        comboBoxShownRange.Items.Add(New ShownDateRange() With {.DurationHours = 8, .Name = ml_string(640, "8 Hours")})
        comboBoxShownRange.Items.Add(New ShownDateRange() With {.DurationHours = 24, .Name = ml_string(641, "24 Hours")})
        comboBoxShownRange.Items.Add(New ShownDateRange() With {.DurationHours = (24 * 7), .Name = ml_string(642, "One week")})
        comboBoxShownRange.Items.Add(New ShownDateRange() With {.DurationHours = (24 * 30), .Name = ml_string(643, "Four weeks")})
        comboBoxShownRange.Items.Add(New ShownDateRange() With {.DurationHours = 999999, .Name = ml_string(644, "All data")})

        TimerRefreshUi_Tick(Me, New EventArgs())
        TimerRefreshUi.Enabled = True

    End Sub

    Private Sub frmDetectorTemperature_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        For idx As Integer = comboBoxShownRange.Items.Count - 1 To 0 Step -1
            Dim rangeItem As ShownDateRange = comboBoxShownRange.Items(idx)
            If _MyTemperatureRecorder.DisplayPeriodMinutes >= (rangeItem.DurationHours * 60) Then
                comboBoxShownRange.SelectedItem = rangeItem
                Exit For
            End If
        Next
    End Sub

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ml_UpdateControls()
        ' Add any initialization after the InitializeComponent() call.
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls

        'InteractiveGraphViewer.MinGapBetweenGraphs = _MyFHT59N3Par.CaptureCycleDetectorTemperature * 60 + 5
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDetectorTemperature))
        Me.lblMeasureStartText.Text = ml_resource(552)
        Me.lblMeasureEndText.Text = ml_resource(553)
        Me.grpBoxCurrentValues.Text = ml_resource(556)
        Me.lblLastRetrievedTempText.Text = ml_resource(557)
        Me.lblMinTempText.Text = ml_resource(558)
        Me.lblMaxTempText.Text = ml_resource(559)
        Me.btnClose.Text = ml_resource(560)
        Me.lblAverageTempText.Text = ml_resource(606)
        Me.grpBoxFurtherInformation.Text = ml_resource(607)

        resources.ApplyResources(Me, "$this")
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub TimerRefreshUi_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerRefreshUi.Tick

        RefreshGraph()

    End Sub

    Private Sub RefreshGraph()
        Dim displayPeriodMinutes As Integer = _MyTemperatureRecorder.DisplayPeriodMinutes
        Dim periodStart As New DateTime(_MyTemperatureRecorder.MostRecentTimeStamp.AddMinutes(-displayPeriodMinutes - 1).Ticks, DateTimeKind.Unspecified)
        Dim periodEnd = _MyTemperatureRecorder.MostRecentTimeStamp

        Dim lineChart As Series = ChartLineTemperature.Series.Item(0)
        lineChart.XValueType = If(displayPeriodMinutes <= 480, ChartValueType.Time, ChartValueType.DateTime)

        lineChart.Points.Clear()
        _MyTemperatureRecorder.FillXYBeginningFrom(periodStart, periodEnd, Sub(dt As DateTime, tempVal As Integer)
                                                                               lineChart.Points.AddXY(dt, tempVal)
                                                                           End Sub)

        'Alles raus, hat nicht getaugt: InteractiveGraphViewer.AddGraph(graph, graphColor, 2, "Temperature")

        lblMeasureStart.Text = _MyTemperatureRecorder.OldestTimeStamp
        lblMeasureStop.Text = _MyTemperatureRecorder.MostRecentTimeStamp

        Dim periodMinTemp As Double = _MyTemperatureRecorder.GetMaxTempFromPeriod(TimeSpan.FromMinutes(_MyTemperatureRecorder.DisplayPeriodMinutes))
        Dim periodMaxTemp As Double = _MyTemperatureRecorder.GetMinTempFromPeriod(TimeSpan.FromMinutes(_MyTemperatureRecorder.DisplayPeriodMinutes))
        Dim latestTemp As Double = _MyTemperatureRecorder.GetLatestTemp()
        Dim averageTemp As Double = _MyTemperatureRecorder.GetAverageTemp()

        If latestTemp = Double.MinValue Then
            lblLastTemperature.Text = "--"
        Else
            lblLastTemperature.Text = latestTemp.ToString("F0") & "°C"
        End If

        If periodMaxTemp = Double.MinValue Then
            lblPeriodMaxTemp.Text = "--"
        Else
            lblPeriodMaxTemp.Text = periodMinTemp.ToString("F0") & "°C"
        End If

        If periodMinTemp = Double.MinValue Then
            lblPeriodMinTemp.Text = "--"
        Else
            lblPeriodMinTemp.Text = periodMaxTemp.ToString("F0") & "°C"
        End If

        If averageTemp = Double.MinValue Then
            lblAverageTemp.Text = "--"
        Else
            lblAverageTemp.Text = averageTemp.ToString("F0") & "°C"
        End If



    End Sub


    Private Sub frmDetectorTemperature_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        TimerRefreshUi.Enabled = False
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLastRetrievedTempText.Click

    End Sub


    Private Sub comboBoxShownRange_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles comboBoxShownRange.SelectedIndexChanged

        If (Not IsNothing(comboBoxShownRange.SelectedItem)) Then
            Dim dateRange As ShownDateRange = comboBoxShownRange.SelectedItem
            'is displayed in next timer cycle
            _MyTemperatureRecorder.DisplayPeriodMinutes = dateRange.DurationHours * 60
        End If

        RefreshGraph()

    End Sub

    
End Class


Public Class GraphTemplate
    Public ReadOnly Func As Graph.MathFunction
    Protected Description As String

    Public Overrides Function ToString() As String
        Return Description
    End Function

    Public Sub New(ByVal desc As String, ByVal func As Graph.MathFunction)
        Me.Func = func
        Me.Description = desc
    End Sub

End Class


Public Class ShownDateRange

    Public DurationHours As Integer

    Public Name As String = ""

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class