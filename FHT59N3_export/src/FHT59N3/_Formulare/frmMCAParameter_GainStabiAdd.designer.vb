<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMCAParameter_GainStabiAdd
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMCAParameter_GainStabiAdd))
        Me.BtnAccept = New System.Windows.Forms.Button()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Num_GainULD = New System.Windows.Forms.NumericUpDown()
        Me.Num_GainLLD = New System.Windows.Forms.NumericUpDown()
        Me.Num_GainFineGain = New System.Windows.Forms.NumericUpDown()
        Me.Lbl_GainULDMax = New System.Windows.Forms.Label()
        Me.Lbl_GainULDMin = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Lbl_GainLLDMax = New System.Windows.Forms.Label()
        Me.Lbl_GainLLDMin = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Lbl_GainFineGainMax = New System.Windows.Forms.Label()
        Me.Lbl_GainFineGainMin = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.CBox_GainCoarseGain = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.CBox_GainInputPolarity = New System.Windows.Forms.ComboBox()
        Me.CBox_GainLLDMode = New System.Windows.Forms.ComboBox()
        Me.CBox_GainConversionGain = New System.Windows.Forms.ComboBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Num_ADSPoleZero = New System.Windows.Forms.NumericUpDown()
        Me.Num_ADSShapingFlatTop = New System.Windows.Forms.NumericUpDown()
        Me.Num_ADSShapingRiseTime = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Lbl_ADSPoleZeroMax = New System.Windows.Forms.Label()
        Me.Lbl_ADSPoleZeroMin = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Lbl_ADSShapingFlatTopMax = New System.Windows.Forms.Label()
        Me.Lbl_ADSShapingFlatTopMin = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Lbl_ADSShapingRiseTimeMax = New System.Windows.Forms.Label()
        Me.Lbl_ADSShapingRiseTimeMin = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.CBox_ADSBLRMode = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.CBox_ADSAquisitionMode = New System.Windows.Forms.ComboBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Num_DSRatio = New System.Windows.Forms.NumericUpDown()
        Me.Num_DSSpacing = New System.Windows.Forms.NumericUpDown()
        Me.Num_DSWindow = New System.Windows.Forms.NumericUpDown()
        Me.Num_DSCentroid = New System.Windows.Forms.NumericUpDown()
        Me.CBox_DSDivider = New System.Windows.Forms.ComboBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.CBox_DSMode = New System.Windows.Forms.ComboBox()
        Me.CBox_DSRatioMode = New System.Windows.Forms.ComboBox()
        Me.CBox_DSRange = New System.Windows.Forms.ComboBox()
        Me.Lbl_DSRatioMax = New System.Windows.Forms.Label()
        Me.Lbl_DSRatioMin = New System.Windows.Forms.Label()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.Lbl_DSSpacingMax = New System.Windows.Forms.Label()
        Me.Lbl_DSSpacingMin = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.Lbl_DSWindowMax = New System.Windows.Forms.Label()
        Me.Lbl_DSWindowMin = New System.Windows.Forms.Label()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Lbl_DSCentroidMax = New System.Windows.Forms.Label()
        Me.Lbl_DSCentroidMin = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.BtnSet = New System.Windows.Forms.Button()
        Me.BtnReset = New System.Windows.Forms.Button()
        Me.BtnExpandMode = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SpectralDisplay = New AxCanberraDataDisplayLib.AxMvc()
        Me.GroupBox2.SuspendLayout()
        CType(Me.Num_GainULD, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_GainLLD, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_GainFineGain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox3.SuspendLayout()
        CType(Me.Num_ADSPoleZero, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_ADSShapingFlatTop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_ADSShapingRiseTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        CType(Me.Num_DSRatio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_DSSpacing, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_DSWindow, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Num_DSCentroid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SpectralDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnAccept
        '
        Me.BtnAccept.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAccept, "BtnAccept")
        Me.BtnAccept.Name = "BtnAccept"
        Me.BtnAccept.UseVisualStyleBackColor = False
        '
        'BtnClose
        '
        Me.BtnClose.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Num_GainULD)
        Me.GroupBox2.Controls.Add(Me.Num_GainLLD)
        Me.GroupBox2.Controls.Add(Me.Num_GainFineGain)
        Me.GroupBox2.Controls.Add(Me.Lbl_GainULDMax)
        Me.GroupBox2.Controls.Add(Me.Lbl_GainULDMin)
        Me.GroupBox2.Controls.Add(Me.Label25)
        Me.GroupBox2.Controls.Add(Me.Lbl_GainLLDMax)
        Me.GroupBox2.Controls.Add(Me.Lbl_GainLLDMin)
        Me.GroupBox2.Controls.Add(Me.Label21)
        Me.GroupBox2.Controls.Add(Me.Lbl_GainFineGainMax)
        Me.GroupBox2.Controls.Add(Me.Lbl_GainFineGainMin)
        Me.GroupBox2.Controls.Add(Me.Label17)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.CBox_GainCoarseGain)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.CBox_GainInputPolarity)
        Me.GroupBox2.Controls.Add(Me.CBox_GainLLDMode)
        Me.GroupBox2.Controls.Add(Me.CBox_GainConversionGain)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'Num_GainULD
        '
        Me.Num_GainULD.DecimalPlaces = 1
        Me.Num_GainULD.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        resources.ApplyResources(Me.Num_GainULD, "Num_GainULD")
        Me.Num_GainULD.Name = "Num_GainULD"
        '
        'Num_GainLLD
        '
        Me.Num_GainLLD.DecimalPlaces = 1
        Me.Num_GainLLD.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        resources.ApplyResources(Me.Num_GainLLD, "Num_GainLLD")
        Me.Num_GainLLD.Name = "Num_GainLLD"
        '
        'Num_GainFineGain
        '
        Me.Num_GainFineGain.DecimalPlaces = 5
        Me.Num_GainFineGain.Increment = New Decimal(New Integer() {1, 0, 0, 327680})
        resources.ApplyResources(Me.Num_GainFineGain, "Num_GainFineGain")
        Me.Num_GainFineGain.Name = "Num_GainFineGain"
        '
        'Lbl_GainULDMax
        '
        resources.ApplyResources(Me.Lbl_GainULDMax, "Lbl_GainULDMax")
        Me.Lbl_GainULDMax.Name = "Lbl_GainULDMax"
        '
        'Lbl_GainULDMin
        '
        resources.ApplyResources(Me.Lbl_GainULDMin, "Lbl_GainULDMin")
        Me.Lbl_GainULDMin.Name = "Lbl_GainULDMin"
        '
        'Label25
        '
        resources.ApplyResources(Me.Label25, "Label25")
        Me.Label25.Name = "Label25"
        '
        'Lbl_GainLLDMax
        '
        resources.ApplyResources(Me.Lbl_GainLLDMax, "Lbl_GainLLDMax")
        Me.Lbl_GainLLDMax.Name = "Lbl_GainLLDMax"
        '
        'Lbl_GainLLDMin
        '
        resources.ApplyResources(Me.Lbl_GainLLDMin, "Lbl_GainLLDMin")
        Me.Lbl_GainLLDMin.Name = "Lbl_GainLLDMin"
        '
        'Label21
        '
        resources.ApplyResources(Me.Label21, "Label21")
        Me.Label21.Name = "Label21"
        '
        'Lbl_GainFineGainMax
        '
        resources.ApplyResources(Me.Lbl_GainFineGainMax, "Lbl_GainFineGainMax")
        Me.Lbl_GainFineGainMax.Name = "Lbl_GainFineGainMax"
        '
        'Lbl_GainFineGainMin
        '
        resources.ApplyResources(Me.Lbl_GainFineGainMin, "Lbl_GainFineGainMin")
        Me.Lbl_GainFineGainMin.Name = "Lbl_GainFineGainMin"
        '
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'CBox_GainCoarseGain
        '
        Me.CBox_GainCoarseGain.FormattingEnabled = True
        Me.CBox_GainCoarseGain.Items.AddRange(New Object() {resources.GetString("CBox_GainCoarseGain.Items"), resources.GetString("CBox_GainCoarseGain.Items1"), resources.GetString("CBox_GainCoarseGain.Items2")})
        resources.ApplyResources(Me.CBox_GainCoarseGain, "CBox_GainCoarseGain")
        Me.CBox_GainCoarseGain.Name = "CBox_GainCoarseGain"
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        '
        'Label13
        '
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Name = "Label13"
        '
        'CBox_GainInputPolarity
        '
        Me.CBox_GainInputPolarity.FormattingEnabled = True
        Me.CBox_GainInputPolarity.Items.AddRange(New Object() {resources.GetString("CBox_GainInputPolarity.Items"), resources.GetString("CBox_GainInputPolarity.Items1")})
        resources.ApplyResources(Me.CBox_GainInputPolarity, "CBox_GainInputPolarity")
        Me.CBox_GainInputPolarity.Name = "CBox_GainInputPolarity"
        '
        'CBox_GainLLDMode
        '
        Me.CBox_GainLLDMode.FormattingEnabled = True
        Me.CBox_GainLLDMode.Items.AddRange(New Object() {resources.GetString("CBox_GainLLDMode.Items"), resources.GetString("CBox_GainLLDMode.Items1")})
        resources.ApplyResources(Me.CBox_GainLLDMode, "CBox_GainLLDMode")
        Me.CBox_GainLLDMode.Name = "CBox_GainLLDMode"
        '
        'CBox_GainConversionGain
        '
        Me.CBox_GainConversionGain.FormattingEnabled = True
        Me.CBox_GainConversionGain.Items.AddRange(New Object() {resources.GetString("CBox_GainConversionGain.Items"), resources.GetString("CBox_GainConversionGain.Items1"), resources.GetString("CBox_GainConversionGain.Items2")})
        resources.ApplyResources(Me.CBox_GainConversionGain, "CBox_GainConversionGain")
        Me.CBox_GainConversionGain.Name = "CBox_GainConversionGain"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Num_ADSPoleZero)
        Me.GroupBox3.Controls.Add(Me.Num_ADSShapingFlatTop)
        Me.GroupBox3.Controls.Add(Me.Num_ADSShapingRiseTime)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.Button1)
        Me.GroupBox3.Controls.Add(Me.Lbl_ADSPoleZeroMax)
        Me.GroupBox3.Controls.Add(Me.Lbl_ADSPoleZeroMin)
        Me.GroupBox3.Controls.Add(Me.Label28)
        Me.GroupBox3.Controls.Add(Me.Lbl_ADSShapingFlatTopMax)
        Me.GroupBox3.Controls.Add(Me.Lbl_ADSShapingFlatTopMin)
        Me.GroupBox3.Controls.Add(Me.Label31)
        Me.GroupBox3.Controls.Add(Me.Lbl_ADSShapingRiseTimeMax)
        Me.GroupBox3.Controls.Add(Me.Lbl_ADSShapingRiseTimeMin)
        Me.GroupBox3.Controls.Add(Me.Label34)
        Me.GroupBox3.Controls.Add(Me.Label18)
        Me.GroupBox3.Controls.Add(Me.CBox_ADSBLRMode)
        Me.GroupBox3.Controls.Add(Me.Label14)
        Me.GroupBox3.Controls.Add(Me.CBox_ADSAquisitionMode)
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        '
        'Num_ADSPoleZero
        '
        resources.ApplyResources(Me.Num_ADSPoleZero, "Num_ADSPoleZero")
        Me.Num_ADSPoleZero.Name = "Num_ADSPoleZero"
        '
        'Num_ADSShapingFlatTop
        '
        Me.Num_ADSShapingFlatTop.DecimalPlaces = 1
        Me.Num_ADSShapingFlatTop.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        resources.ApplyResources(Me.Num_ADSShapingFlatTop, "Num_ADSShapingFlatTop")
        Me.Num_ADSShapingFlatTop.Name = "Num_ADSShapingFlatTop"
        '
        'Num_ADSShapingRiseTime
        '
        Me.Num_ADSShapingRiseTime.DecimalPlaces = 1
        Me.Num_ADSShapingRiseTime.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        resources.ApplyResources(Me.Num_ADSShapingRiseTime, "Num_ADSShapingRiseTime")
        Me.Num_ADSShapingRiseTime.Name = "Num_ADSShapingRiseTime"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Lbl_ADSPoleZeroMax
        '
        resources.ApplyResources(Me.Lbl_ADSPoleZeroMax, "Lbl_ADSPoleZeroMax")
        Me.Lbl_ADSPoleZeroMax.Name = "Lbl_ADSPoleZeroMax"
        '
        'Lbl_ADSPoleZeroMin
        '
        resources.ApplyResources(Me.Lbl_ADSPoleZeroMin, "Lbl_ADSPoleZeroMin")
        Me.Lbl_ADSPoleZeroMin.Name = "Lbl_ADSPoleZeroMin"
        '
        'Label28
        '
        resources.ApplyResources(Me.Label28, "Label28")
        Me.Label28.Name = "Label28"
        '
        'Lbl_ADSShapingFlatTopMax
        '
        resources.ApplyResources(Me.Lbl_ADSShapingFlatTopMax, "Lbl_ADSShapingFlatTopMax")
        Me.Lbl_ADSShapingFlatTopMax.Name = "Lbl_ADSShapingFlatTopMax"
        '
        'Lbl_ADSShapingFlatTopMin
        '
        resources.ApplyResources(Me.Lbl_ADSShapingFlatTopMin, "Lbl_ADSShapingFlatTopMin")
        Me.Lbl_ADSShapingFlatTopMin.Name = "Lbl_ADSShapingFlatTopMin"
        '
        'Label31
        '
        resources.ApplyResources(Me.Label31, "Label31")
        Me.Label31.Name = "Label31"
        '
        'Lbl_ADSShapingRiseTimeMax
        '
        resources.ApplyResources(Me.Lbl_ADSShapingRiseTimeMax, "Lbl_ADSShapingRiseTimeMax")
        Me.Lbl_ADSShapingRiseTimeMax.Name = "Lbl_ADSShapingRiseTimeMax"
        '
        'Lbl_ADSShapingRiseTimeMin
        '
        resources.ApplyResources(Me.Lbl_ADSShapingRiseTimeMin, "Lbl_ADSShapingRiseTimeMin")
        Me.Lbl_ADSShapingRiseTimeMin.Name = "Lbl_ADSShapingRiseTimeMin"
        '
        'Label34
        '
        resources.ApplyResources(Me.Label34, "Label34")
        Me.Label34.Name = "Label34"
        '
        'Label18
        '
        resources.ApplyResources(Me.Label18, "Label18")
        Me.Label18.Name = "Label18"
        '
        'CBox_ADSBLRMode
        '
        resources.ApplyResources(Me.CBox_ADSBLRMode, "CBox_ADSBLRMode")
        Me.CBox_ADSBLRMode.FormattingEnabled = True
        Me.CBox_ADSBLRMode.Items.AddRange(New Object() {resources.GetString("CBox_ADSBLRMode.Items"), resources.GetString("CBox_ADSBLRMode.Items1"), resources.GetString("CBox_ADSBLRMode.Items2")})
        Me.CBox_ADSBLRMode.Name = "CBox_ADSBLRMode"
        '
        'Label14
        '
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Name = "Label14"
        '
        'CBox_ADSAquisitionMode
        '
        resources.ApplyResources(Me.CBox_ADSAquisitionMode, "CBox_ADSAquisitionMode")
        Me.CBox_ADSAquisitionMode.FormattingEnabled = True
        Me.CBox_ADSAquisitionMode.Items.AddRange(New Object() {resources.GetString("CBox_ADSAquisitionMode.Items"), resources.GetString("CBox_ADSAquisitionMode.Items1"), resources.GetString("CBox_ADSAquisitionMode.Items2")})
        Me.CBox_ADSAquisitionMode.Name = "CBox_ADSAquisitionMode"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Num_DSRatio)
        Me.GroupBox4.Controls.Add(Me.Num_DSSpacing)
        Me.GroupBox4.Controls.Add(Me.Num_DSWindow)
        Me.GroupBox4.Controls.Add(Me.Num_DSCentroid)
        Me.GroupBox4.Controls.Add(Me.CBox_DSDivider)
        Me.GroupBox4.Controls.Add(Me.Label48)
        Me.GroupBox4.Controls.Add(Me.Label49)
        Me.GroupBox4.Controls.Add(Me.Label50)
        Me.GroupBox4.Controls.Add(Me.Label51)
        Me.GroupBox4.Controls.Add(Me.CBox_DSMode)
        Me.GroupBox4.Controls.Add(Me.CBox_DSRatioMode)
        Me.GroupBox4.Controls.Add(Me.CBox_DSRange)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSRatioMax)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSRatioMin)
        Me.GroupBox4.Controls.Add(Me.Label47)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSSpacingMax)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSSpacingMin)
        Me.GroupBox4.Controls.Add(Me.Label38)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSWindowMax)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSWindowMin)
        Me.GroupBox4.Controls.Add(Me.Label41)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSCentroidMax)
        Me.GroupBox4.Controls.Add(Me.Lbl_DSCentroidMin)
        Me.GroupBox4.Controls.Add(Me.Label44)
        resources.ApplyResources(Me.GroupBox4, "GroupBox4")
        Me.GroupBox4.ForeColor = System.Drawing.Color.White
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.TabStop = False
        '
        'Num_DSRatio
        '
        Me.Num_DSRatio.DecimalPlaces = 2
        Me.Num_DSRatio.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        resources.ApplyResources(Me.Num_DSRatio, "Num_DSRatio")
        Me.Num_DSRatio.Name = "Num_DSRatio"
        '
        'Num_DSSpacing
        '
        resources.ApplyResources(Me.Num_DSSpacing, "Num_DSSpacing")
        Me.Num_DSSpacing.Name = "Num_DSSpacing"
        '
        'Num_DSWindow
        '
        resources.ApplyResources(Me.Num_DSWindow, "Num_DSWindow")
        Me.Num_DSWindow.Name = "Num_DSWindow"
        '
        'Num_DSCentroid
        '
        resources.ApplyResources(Me.Num_DSCentroid, "Num_DSCentroid")
        Me.Num_DSCentroid.Name = "Num_DSCentroid"
        '
        'CBox_DSDivider
        '
        Me.CBox_DSDivider.FormattingEnabled = True
        Me.CBox_DSDivider.Items.AddRange(New Object() {resources.GetString("CBox_DSDivider.Items"), resources.GetString("CBox_DSDivider.Items1")})
        resources.ApplyResources(Me.CBox_DSDivider, "CBox_DSDivider")
        Me.CBox_DSDivider.Name = "CBox_DSDivider"
        '
        'Label48
        '
        resources.ApplyResources(Me.Label48, "Label48")
        Me.Label48.Name = "Label48"
        '
        'Label49
        '
        resources.ApplyResources(Me.Label49, "Label49")
        Me.Label49.Name = "Label49"
        '
        'Label50
        '
        resources.ApplyResources(Me.Label50, "Label50")
        Me.Label50.Name = "Label50"
        '
        'Label51
        '
        resources.ApplyResources(Me.Label51, "Label51")
        Me.Label51.Name = "Label51"
        '
        'CBox_DSMode
        '
        Me.CBox_DSMode.FormattingEnabled = True
        resources.ApplyResources(Me.CBox_DSMode, "CBox_DSMode")
        Me.CBox_DSMode.Name = "CBox_DSMode"
        '
        'CBox_DSRatioMode
        '
        Me.CBox_DSRatioMode.FormattingEnabled = True
        Me.CBox_DSRatioMode.Items.AddRange(New Object() {resources.GetString("CBox_DSRatioMode.Items"), resources.GetString("CBox_DSRatioMode.Items1")})
        resources.ApplyResources(Me.CBox_DSRatioMode, "CBox_DSRatioMode")
        Me.CBox_DSRatioMode.Name = "CBox_DSRatioMode"
        '
        'CBox_DSRange
        '
        Me.CBox_DSRange.FormattingEnabled = True
        Me.CBox_DSRange.Items.AddRange(New Object() {resources.GetString("CBox_DSRange.Items"), resources.GetString("CBox_DSRange.Items1"), resources.GetString("CBox_DSRange.Items2")})
        resources.ApplyResources(Me.CBox_DSRange, "CBox_DSRange")
        Me.CBox_DSRange.Name = "CBox_DSRange"
        '
        'Lbl_DSRatioMax
        '
        resources.ApplyResources(Me.Lbl_DSRatioMax, "Lbl_DSRatioMax")
        Me.Lbl_DSRatioMax.Name = "Lbl_DSRatioMax"
        '
        'Lbl_DSRatioMin
        '
        resources.ApplyResources(Me.Lbl_DSRatioMin, "Lbl_DSRatioMin")
        Me.Lbl_DSRatioMin.Name = "Lbl_DSRatioMin"
        '
        'Label47
        '
        resources.ApplyResources(Me.Label47, "Label47")
        Me.Label47.Name = "Label47"
        '
        'Lbl_DSSpacingMax
        '
        resources.ApplyResources(Me.Lbl_DSSpacingMax, "Lbl_DSSpacingMax")
        Me.Lbl_DSSpacingMax.Name = "Lbl_DSSpacingMax"
        '
        'Lbl_DSSpacingMin
        '
        resources.ApplyResources(Me.Lbl_DSSpacingMin, "Lbl_DSSpacingMin")
        Me.Lbl_DSSpacingMin.Name = "Lbl_DSSpacingMin"
        '
        'Label38
        '
        resources.ApplyResources(Me.Label38, "Label38")
        Me.Label38.Name = "Label38"
        '
        'Lbl_DSWindowMax
        '
        resources.ApplyResources(Me.Lbl_DSWindowMax, "Lbl_DSWindowMax")
        Me.Lbl_DSWindowMax.Name = "Lbl_DSWindowMax"
        '
        'Lbl_DSWindowMin
        '
        resources.ApplyResources(Me.Lbl_DSWindowMin, "Lbl_DSWindowMin")
        Me.Lbl_DSWindowMin.Name = "Lbl_DSWindowMin"
        '
        'Label41
        '
        resources.ApplyResources(Me.Label41, "Label41")
        Me.Label41.Name = "Label41"
        '
        'Lbl_DSCentroidMax
        '
        resources.ApplyResources(Me.Lbl_DSCentroidMax, "Lbl_DSCentroidMax")
        Me.Lbl_DSCentroidMax.Name = "Lbl_DSCentroidMax"
        '
        'Lbl_DSCentroidMin
        '
        resources.ApplyResources(Me.Lbl_DSCentroidMin, "Lbl_DSCentroidMin")
        Me.Lbl_DSCentroidMin.Name = "Lbl_DSCentroidMin"
        '
        'Label44
        '
        resources.ApplyResources(Me.Label44, "Label44")
        Me.Label44.Name = "Label44"
        '
        'BtnSet
        '
        Me.BtnSet.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnSet, "BtnSet")
        Me.BtnSet.Name = "BtnSet"
        Me.BtnSet.UseVisualStyleBackColor = False
        '
        'BtnReset
        '
        Me.BtnReset.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnReset, "BtnReset")
        Me.BtnReset.Name = "BtnReset"
        Me.BtnReset.UseVisualStyleBackColor = False
        '
        'BtnExpandMode
        '
        Me.BtnExpandMode.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnExpandMode.BackgroundImage = Global.FHT59N3.My.Resources.Resources.Zoom
        resources.ApplyResources(Me.BtnExpandMode, "BtnExpandMode")
        Me.BtnExpandMode.Name = "BtnExpandMode"
        Me.BtnExpandMode.UseVisualStyleBackColor = False
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'SpectralDisplay
        '
        resources.ApplyResources(Me.SpectralDisplay, "SpectralDisplay")
        Me.SpectralDisplay.Name = "SpectralDisplay"
        Me.SpectralDisplay.OcxState = CType(resources.GetObject("SpectralDisplay.OcxState"), System.Windows.Forms.AxHost.State)
        '
        'frmMCAParameter_GainStabiAdd
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnExpandMode)
        Me.Controls.Add(Me.SpectralDisplay)
        Me.Controls.Add(Me.BtnReset)
        Me.Controls.Add(Me.BtnSet)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.BtnAccept)
        Me.Controls.Add(Me.BtnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMCAParameter_GainStabiAdd"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.Num_GainULD, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_GainLLD, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_GainFineGain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.Num_ADSPoleZero, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_ADSShapingFlatTop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_ADSShapingRiseTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.Num_DSRatio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_DSSpacing, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_DSWindow, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Num_DSCentroid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SpectralDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnAccept As System.Windows.Forms.Button
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents CBox_GainInputPolarity As System.Windows.Forms.ComboBox
    Friend WithEvents CBox_GainLLDMode As System.Windows.Forms.ComboBox
    Friend WithEvents CBox_GainConversionGain As System.Windows.Forms.ComboBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents CBox_GainCoarseGain As System.Windows.Forms.ComboBox
    Friend WithEvents Lbl_GainLLDMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_GainLLDMin As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Lbl_GainFineGainMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_GainFineGainMin As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Lbl_GainULDMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_GainULDMin As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents CBox_ADSAquisitionMode As System.Windows.Forms.ComboBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents CBox_ADSBLRMode As System.Windows.Forms.ComboBox
    Friend WithEvents Lbl_ADSPoleZeroMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_ADSPoleZeroMin As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Lbl_ADSShapingFlatTopMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_ADSShapingFlatTopMin As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Lbl_ADSShapingRiseTimeMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_ADSShapingRiseTimeMin As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSRatioMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSRatioMin As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSSpacingMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSSpacingMin As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSWindowMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSWindowMin As System.Windows.Forms.Label
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSCentroidMax As System.Windows.Forms.Label
    Friend WithEvents Lbl_DSCentroidMin As System.Windows.Forms.Label
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents CBox_DSMode As System.Windows.Forms.ComboBox
    Friend WithEvents CBox_DSRatioMode As System.Windows.Forms.ComboBox
    Friend WithEvents CBox_DSRange As System.Windows.Forms.ComboBox
    Friend WithEvents CBox_DSDivider As System.Windows.Forms.ComboBox
    Friend WithEvents BtnSet As System.Windows.Forms.Button
    Friend WithEvents BtnReset As System.Windows.Forms.Button
    Friend WithEvents SpectralDisplay As AxCanberraDataDisplayLib.AxMvc
    Friend WithEvents BtnExpandMode As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Num_DSCentroid As NumericUpDown
    Friend WithEvents Num_DSWindow As NumericUpDown
    Friend WithEvents Num_DSSpacing As NumericUpDown
    Friend WithEvents Num_DSRatio As NumericUpDown
    Friend WithEvents Num_GainFineGain As NumericUpDown
    Friend WithEvents Num_GainLLD As NumericUpDown
    Friend WithEvents Num_GainULD As NumericUpDown
    Friend WithEvents Num_ADSShapingRiseTime As NumericUpDown
    Friend WithEvents Num_ADSShapingFlatTop As NumericUpDown
    Friend WithEvents Num_ADSPoleZero As NumericUpDown
End Class
