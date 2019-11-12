<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDetectorTemperature
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDetectorTemperature))
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.TimerRefreshUi = New System.Windows.Forms.Timer(Me.components)
        Me.grpBoxCurrentValues = New System.Windows.Forms.GroupBox()
        Me.lblPeriodMaxTemp = New System.Windows.Forms.Label()
        Me.lblMaxTempText = New System.Windows.Forms.Label()
        Me.lblPeriodMinTemp = New System.Windows.Forms.Label()
        Me.lblMinTempText = New System.Windows.Forms.Label()
        Me.lblLastTemperature = New System.Windows.Forms.Label()
        Me.lblLastRetrievedTempText = New System.Windows.Forms.Label()
        Me.lblMeasureEndText = New System.Windows.Forms.Label()
        Me.lblAverageTemp = New System.Windows.Forms.Label()
        Me.lblMeasureStartText = New System.Windows.Forms.Label()
        Me.lblAverageTempText = New System.Windows.Forms.Label()
        Me.lblMeasureStart = New System.Windows.Forms.Label()
        Me.lblMeasureStop = New System.Windows.Forms.Label()
        Me.grpBoxFurtherInformation = New System.Windows.Forms.GroupBox()
        Me.comboBoxShownRange = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ChartLineTemperature = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.grpBoxCurrentValues.SuspendLayout()
        Me.grpBoxFurtherInformation.SuspendLayout()
        CType(Me.ChartLineTemperature, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'TimerRefreshUi
        '
        Me.TimerRefreshUi.Interval = 10000
        '
        'grpBoxCurrentValues
        '
        Me.grpBoxCurrentValues.Controls.Add(Me.lblPeriodMaxTemp)
        Me.grpBoxCurrentValues.Controls.Add(Me.lblMaxTempText)
        Me.grpBoxCurrentValues.Controls.Add(Me.lblPeriodMinTemp)
        Me.grpBoxCurrentValues.Controls.Add(Me.lblMinTempText)
        Me.grpBoxCurrentValues.Controls.Add(Me.lblLastTemperature)
        Me.grpBoxCurrentValues.Controls.Add(Me.lblLastRetrievedTempText)
        resources.ApplyResources(Me.grpBoxCurrentValues, "grpBoxCurrentValues")
        Me.grpBoxCurrentValues.Name = "grpBoxCurrentValues"
        Me.grpBoxCurrentValues.TabStop = False
        '
        'lblPeriodMaxTemp
        '
        Me.lblPeriodMaxTemp.BackColor = System.Drawing.Color.White
        Me.lblPeriodMaxTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblPeriodMaxTemp, "lblPeriodMaxTemp")
        Me.lblPeriodMaxTemp.Name = "lblPeriodMaxTemp"
        '
        'lblMaxTempText
        '
        resources.ApplyResources(Me.lblMaxTempText, "lblMaxTempText")
        Me.lblMaxTempText.Name = "lblMaxTempText"
        '
        'lblPeriodMinTemp
        '
        Me.lblPeriodMinTemp.BackColor = System.Drawing.Color.White
        Me.lblPeriodMinTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblPeriodMinTemp, "lblPeriodMinTemp")
        Me.lblPeriodMinTemp.Name = "lblPeriodMinTemp"
        '
        'lblMinTempText
        '
        resources.ApplyResources(Me.lblMinTempText, "lblMinTempText")
        Me.lblMinTempText.Name = "lblMinTempText"
        '
        'lblLastTemperature
        '
        Me.lblLastTemperature.BackColor = System.Drawing.Color.White
        Me.lblLastTemperature.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblLastTemperature, "lblLastTemperature")
        Me.lblLastTemperature.Name = "lblLastTemperature"
        '
        'lblLastRetrievedTempText
        '
        resources.ApplyResources(Me.lblLastRetrievedTempText, "lblLastRetrievedTempText")
        Me.lblLastRetrievedTempText.Name = "lblLastRetrievedTempText"
        '
        'lblMeasureEndText
        '
        Me.lblMeasureEndText.ForeColor = System.Drawing.Color.Blue
        resources.ApplyResources(Me.lblMeasureEndText, "lblMeasureEndText")
        Me.lblMeasureEndText.Name = "lblMeasureEndText"
        '
        'lblAverageTemp
        '
        Me.lblAverageTemp.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.lblAverageTemp, "lblAverageTemp")
        Me.lblAverageTemp.Name = "lblAverageTemp"
        '
        'lblMeasureStartText
        '
        Me.lblMeasureStartText.ForeColor = System.Drawing.Color.Blue
        resources.ApplyResources(Me.lblMeasureStartText, "lblMeasureStartText")
        Me.lblMeasureStartText.Name = "lblMeasureStartText"
        '
        'lblAverageTempText
        '
        Me.lblAverageTempText.ForeColor = System.Drawing.Color.Blue
        resources.ApplyResources(Me.lblAverageTempText, "lblAverageTempText")
        Me.lblAverageTempText.Name = "lblAverageTempText"
        '
        'lblMeasureStart
        '
        Me.lblMeasureStart.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.lblMeasureStart, "lblMeasureStart")
        Me.lblMeasureStart.Name = "lblMeasureStart"
        '
        'lblMeasureStop
        '
        Me.lblMeasureStop.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.lblMeasureStop, "lblMeasureStop")
        Me.lblMeasureStop.Name = "lblMeasureStop"
        '
        'grpBoxFurtherInformation
        '
        resources.ApplyResources(Me.grpBoxFurtherInformation, "grpBoxFurtherInformation")
        Me.grpBoxFurtherInformation.Controls.Add(Me.lblMeasureEndText)
        Me.grpBoxFurtherInformation.Controls.Add(Me.lblMeasureStop)
        Me.grpBoxFurtherInformation.Controls.Add(Me.lblAverageTemp)
        Me.grpBoxFurtherInformation.Controls.Add(Me.lblMeasureStart)
        Me.grpBoxFurtherInformation.Controls.Add(Me.lblMeasureStartText)
        Me.grpBoxFurtherInformation.Controls.Add(Me.lblAverageTempText)
        Me.grpBoxFurtherInformation.Name = "grpBoxFurtherInformation"
        Me.grpBoxFurtherInformation.TabStop = False
        '
        'comboBoxShownRange
        '
        Me.comboBoxShownRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBoxShownRange.FormattingEnabled = True
        resources.ApplyResources(Me.comboBoxShownRange, "comboBoxShownRange")
        Me.comboBoxShownRange.Name = "comboBoxShownRange"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'ChartLineTemperature
        '
        ChartArea1.Name = "ChartArea"
        Me.ChartLineTemperature.ChartAreas.Add(ChartArea1)
        Legend1.Name = "Legend1"
        Me.ChartLineTemperature.Legends.Add(Legend1)
        resources.ApplyResources(Me.ChartLineTemperature, "ChartLineTemperature")
        Me.ChartLineTemperature.Name = "ChartLineTemperature"
        Series1.ChartArea = "ChartArea"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series1.EmptyPointStyle.Color = System.Drawing.Color.Red
        Series1.Legend = "Legend1"
        Series1.Name = "Series"
        Series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time
        Series1.YValuesPerPoint = 2
        Series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Me.ChartLineTemperature.Series.Add(Series1)
        '
        'frmDetectorTemperature
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.SystemColors.Control
        resources.ApplyResources(Me, "$this")
        Me.ControlBox = False
        Me.Controls.Add(Me.ChartLineTemperature)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.grpBoxFurtherInformation)
        Me.Controls.Add(Me.comboBoxShownRange)
        Me.Controls.Add(Me.grpBoxCurrentValues)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmDetectorTemperature"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.grpBoxCurrentValues.ResumeLayout(False)
        Me.grpBoxFurtherInformation.ResumeLayout(False)
        CType(Me.ChartLineTemperature, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents TimerRefreshUi As System.Windows.Forms.Timer
    Friend WithEvents grpBoxCurrentValues As System.Windows.Forms.GroupBox
    Friend WithEvents lblPeriodMaxTemp As System.Windows.Forms.Label
    Friend WithEvents lblMaxTempText As System.Windows.Forms.Label
    Friend WithEvents lblPeriodMinTemp As System.Windows.Forms.Label
    Friend WithEvents lblMinTempText As System.Windows.Forms.Label
    Friend WithEvents lblLastTemperature As System.Windows.Forms.Label
    Friend WithEvents lblLastRetrievedTempText As System.Windows.Forms.Label
    Friend WithEvents lblMeasureEndText As System.Windows.Forms.Label
    Friend WithEvents lblAverageTemp As System.Windows.Forms.Label
    Friend WithEvents lblMeasureStartText As System.Windows.Forms.Label
    Friend WithEvents lblAverageTempText As System.Windows.Forms.Label
    Friend WithEvents lblMeasureStart As System.Windows.Forms.Label
    Friend WithEvents lblMeasureStop As System.Windows.Forms.Label
    Friend WithEvents grpBoxFurtherInformation As System.Windows.Forms.GroupBox
    Friend WithEvents comboBoxShownRange As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ChartLineTemperature As System.Windows.Forms.DataVisualization.Charting.Chart
End Class
