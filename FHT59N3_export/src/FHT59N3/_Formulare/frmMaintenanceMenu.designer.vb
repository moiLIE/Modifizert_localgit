<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMaintenanceMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMaintenanceMenu))
        Me.BtnSysConfig = New System.Windows.Forms.Button()
        Me.BtnControl = New System.Windows.Forms.Button()
        Me.BtnReturn = New System.Windows.Forms.Button()
        Me.BtnCalibration = New System.Windows.Forms.Button()
        Me.BtnMCAConfig = New System.Windows.Forms.Button()
        Me.BtnMaintenanceOn = New System.Windows.Forms.Button()
        Me.BtnFilterTape = New System.Windows.Forms.Button()
        Me.BtnEcooler = New System.Windows.Forms.Button()
        Me.TimerForEcoolerButton = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'BtnSysConfig
        '
        Me.BtnSysConfig.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnSysConfig, "BtnSysConfig")
        Me.BtnSysConfig.Name = "BtnSysConfig"
        Me.BtnSysConfig.UseVisualStyleBackColor = False
        '
        'BtnControl
        '
        Me.BtnControl.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnControl, "BtnControl")
        Me.BtnControl.Name = "BtnControl"
        Me.BtnControl.UseVisualStyleBackColor = False
        '
        'BtnReturn
        '
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnCalibration
        '
        Me.BtnCalibration.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnCalibration, "BtnCalibration")
        Me.BtnCalibration.Name = "BtnCalibration"
        Me.BtnCalibration.UseVisualStyleBackColor = False
        '
        'BtnMCAConfig
        '
        Me.BtnMCAConfig.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnMCAConfig, "BtnMCAConfig")
        Me.BtnMCAConfig.Name = "BtnMCAConfig"
        Me.BtnMCAConfig.UseVisualStyleBackColor = False
        '
        'BtnMaintenanceOn
        '
        Me.BtnMaintenanceOn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnMaintenanceOn, "BtnMaintenanceOn")
        Me.BtnMaintenanceOn.Name = "BtnMaintenanceOn"
        Me.BtnMaintenanceOn.UseVisualStyleBackColor = False
        '
        'BtnFilterTape
        '
        Me.BtnFilterTape.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnFilterTape, "BtnFilterTape")
        Me.BtnFilterTape.Name = "BtnFilterTape"
        Me.BtnFilterTape.UseVisualStyleBackColor = False
        '
        'BtnEcooler
        '
        Me.BtnEcooler.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnEcooler, "BtnEcooler")
        Me.BtnEcooler.Image = Global.FHT59N3.My.Resources.Resources.cooling_on_32
        Me.BtnEcooler.Name = "BtnEcooler"
        Me.BtnEcooler.UseVisualStyleBackColor = False
        '
        'TimerForEcoolerButton
        '
        Me.TimerForEcoolerButton.Interval = 5000
        '
        'frmMaintenanceMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnEcooler)
        Me.Controls.Add(Me.BtnFilterTape)
        Me.Controls.Add(Me.BtnMaintenanceOn)
        Me.Controls.Add(Me.BtnMCAConfig)
        Me.Controls.Add(Me.BtnCalibration)
        Me.Controls.Add(Me.BtnReturn)
        Me.Controls.Add(Me.BtnControl)
        Me.Controls.Add(Me.BtnSysConfig)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMaintenanceMenu"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnSysConfig As System.Windows.Forms.Button
    Friend WithEvents BtnControl As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnCalibration As System.Windows.Forms.Button
    Friend WithEvents BtnMCAConfig As System.Windows.Forms.Button
    Friend WithEvents BtnMaintenanceOn As System.Windows.Forms.Button
    Friend WithEvents BtnFilterTape As System.Windows.Forms.Button
    Friend WithEvents BtnEcooler As System.Windows.Forms.Button
    Friend WithEvents TimerForEcoolerButton As System.Windows.Forms.Timer
End Class
