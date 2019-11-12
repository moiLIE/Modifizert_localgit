<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCalibrationMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalibrationMenu))
        Me.BtnRCC = New System.Windows.Forms.Button()
        Me.BTnMeasFar = New System.Windows.Forms.Button()
        Me.BtnReturn = New System.Windows.Forms.Button()
        Me.BtnMeasFree = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.BtnBackgroundMeas = New System.Windows.Forms.Button()
        Me.BtnStop = New System.Windows.Forms.Button()
        Me.BtnAirFlow = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.BtnEfficiencyNear = New System.Windows.Forms.Button()
        Me.BtnEfficiencyFar = New System.Windows.Forms.Button()
        Me.BtnEnergyFar = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnRCC
        '
        Me.BtnRCC.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnRCC, "BtnRCC")
        Me.BtnRCC.ForeColor = System.Drawing.Color.Black
        Me.BtnRCC.Name = "BtnRCC"
        Me.BtnRCC.UseVisualStyleBackColor = False
        '
        'BTnMeasFar
        '
        Me.BTnMeasFar.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BTnMeasFar, "BTnMeasFar")
        Me.BTnMeasFar.ForeColor = System.Drawing.Color.Black
        Me.BTnMeasFar.Name = "BTnMeasFar"
        Me.BTnMeasFar.UseVisualStyleBackColor = False
        '
        'BtnReturn
        '
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnMeasFree
        '
        Me.BtnMeasFree.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnMeasFree, "BtnMeasFree")
        Me.BtnMeasFree.ForeColor = System.Drawing.Color.Black
        Me.BtnMeasFree.Name = "BtnMeasFree"
        Me.BtnMeasFree.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.BtnBackgroundMeas)
        Me.GroupBox1.Controls.Add(Me.BtnStop)
        Me.GroupBox1.Controls.Add(Me.BtnAirFlow)
        Me.GroupBox1.Controls.Add(Me.BtnRCC)
        Me.GroupBox1.Controls.Add(Me.BtnMeasFree)
        Me.GroupBox1.Controls.Add(Me.BTnMeasFar)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'BtnBackgroundMeas
        '
        Me.BtnBackgroundMeas.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnBackgroundMeas, "BtnBackgroundMeas")
        Me.BtnBackgroundMeas.ForeColor = System.Drawing.Color.Black
        Me.BtnBackgroundMeas.Name = "BtnBackgroundMeas"
        Me.BtnBackgroundMeas.UseVisualStyleBackColor = False
        '
        'BtnStop
        '
        Me.BtnStop.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnStop, "BtnStop")
        Me.BtnStop.ForeColor = System.Drawing.Color.Black
        Me.BtnStop.Name = "BtnStop"
        Me.BtnStop.UseVisualStyleBackColor = False
        '
        'BtnAirFlow
        '
        Me.BtnAirFlow.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAirFlow, "BtnAirFlow")
        Me.BtnAirFlow.ForeColor = System.Drawing.Color.Black
        Me.BtnAirFlow.Name = "BtnAirFlow"
        Me.BtnAirFlow.UseVisualStyleBackColor = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.BtnEfficiencyNear)
        Me.GroupBox2.Controls.Add(Me.BtnEfficiencyFar)
        Me.GroupBox2.Controls.Add(Me.BtnEnergyFar)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'BtnEfficiencyNear
        '
        Me.BtnEfficiencyNear.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnEfficiencyNear, "BtnEfficiencyNear")
        Me.BtnEfficiencyNear.ForeColor = System.Drawing.Color.Black
        Me.BtnEfficiencyNear.Name = "BtnEfficiencyNear"
        Me.BtnEfficiencyNear.UseVisualStyleBackColor = False
        '
        'BtnEfficiencyFar
        '
        Me.BtnEfficiencyFar.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnEfficiencyFar, "BtnEfficiencyFar")
        Me.BtnEfficiencyFar.ForeColor = System.Drawing.Color.Black
        Me.BtnEfficiencyFar.Name = "BtnEfficiencyFar"
        Me.BtnEfficiencyFar.UseVisualStyleBackColor = False
        '
        'BtnEnergyFar
        '
        Me.BtnEnergyFar.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnEnergyFar, "BtnEnergyFar")
        Me.BtnEnergyFar.ForeColor = System.Drawing.Color.Black
        Me.BtnEnergyFar.Name = "BtnEnergyFar"
        Me.BtnEnergyFar.UseVisualStyleBackColor = False
        '
        'frmCalibrationMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.BtnReturn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCalibrationMenu"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnRCC As System.Windows.Forms.Button
    Friend WithEvents BTnMeasFar As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnMeasFree As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents BtnEfficiencyNear As System.Windows.Forms.Button
    Friend WithEvents BtnEfficiencyFar As System.Windows.Forms.Button
    Friend WithEvents BtnEnergyFar As System.Windows.Forms.Button
    Friend WithEvents BtnAirFlow As System.Windows.Forms.Button
    Friend WithEvents BtnStop As System.Windows.Forms.Button
    Friend WithEvents BtnBackgroundMeas As System.Windows.Forms.Button
End Class
