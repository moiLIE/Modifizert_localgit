<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCalibrationAnalyzeControl
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalibrationAnalyzeControl))
        Me.BtnAnalyze = New System.Windows.Forms.Button
        Me.BtnClose = New System.Windows.Forms.Button
        Me.TBox_DateTime = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Lbl_CommandText = New System.Windows.Forms.Label
        Me.TBox_ActNuc1 = New System.Windows.Forms.TextBox
        Me.Lbl_ActNuc1 = New System.Windows.Forms.Label
        Me.TBox_ActNuc2 = New System.Windows.Forms.TextBox
        Me.Lbl_ActNuc2 = New System.Windows.Forms.Label
        Me.TBox_ActNuc3 = New System.Windows.Forms.TextBox
        Me.Lbl_ActNuc3 = New System.Windows.Forms.Label
        Me.TBox_Prob = New System.Windows.Forms.TextBox
        Me.Lbl_Prob = New System.Windows.Forms.Label
        Me.BtnChange = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'BtnAnalyze
        '
        Me.BtnAnalyze.AccessibleDescription = Nothing
        Me.BtnAnalyze.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnAnalyze, "BtnAnalyze")
        Me.BtnAnalyze.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnAnalyze.BackgroundImage = Nothing
        Me.BtnAnalyze.Name = "BtnAnalyze"
        Me.BtnAnalyze.UseVisualStyleBackColor = False
        '
        'BtnClose
        '
        Me.BtnClose.AccessibleDescription = Nothing
        Me.BtnClose.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnClose.BackgroundImage = Nothing
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'TBox_DateTime
        '
        Me.TBox_DateTime.AccessibleDescription = Nothing
        Me.TBox_DateTime.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_DateTime, "TBox_DateTime")
        Me.TBox_DateTime.BackgroundImage = Nothing
        Me.TBox_DateTime.Name = "TBox_DateTime"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Name = "Label1"
        '
        'Lbl_CommandText
        '
        Me.Lbl_CommandText.AccessibleDescription = Nothing
        Me.Lbl_CommandText.AccessibleName = Nothing
        resources.ApplyResources(Me.Lbl_CommandText, "Lbl_CommandText")
        Me.Lbl_CommandText.ForeColor = System.Drawing.Color.White
        Me.Lbl_CommandText.Name = "Lbl_CommandText"
        '
        'TBox_ActNuc1
        '
        Me.TBox_ActNuc1.AccessibleDescription = Nothing
        Me.TBox_ActNuc1.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_ActNuc1, "TBox_ActNuc1")
        Me.TBox_ActNuc1.BackgroundImage = Nothing
        Me.TBox_ActNuc1.Name = "TBox_ActNuc1"
        '
        'Lbl_ActNuc1
        '
        Me.Lbl_ActNuc1.AccessibleDescription = Nothing
        Me.Lbl_ActNuc1.AccessibleName = Nothing
        resources.ApplyResources(Me.Lbl_ActNuc1, "Lbl_ActNuc1")
        Me.Lbl_ActNuc1.ForeColor = System.Drawing.Color.White
        Me.Lbl_ActNuc1.Name = "Lbl_ActNuc1"
        '
        'TBox_ActNuc2
        '
        Me.TBox_ActNuc2.AccessibleDescription = Nothing
        Me.TBox_ActNuc2.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_ActNuc2, "TBox_ActNuc2")
        Me.TBox_ActNuc2.BackgroundImage = Nothing
        Me.TBox_ActNuc2.Name = "TBox_ActNuc2"
        '
        'Lbl_ActNuc2
        '
        Me.Lbl_ActNuc2.AccessibleDescription = Nothing
        Me.Lbl_ActNuc2.AccessibleName = Nothing
        resources.ApplyResources(Me.Lbl_ActNuc2, "Lbl_ActNuc2")
        Me.Lbl_ActNuc2.ForeColor = System.Drawing.Color.White
        Me.Lbl_ActNuc2.Name = "Lbl_ActNuc2"
        '
        'TBox_ActNuc3
        '
        Me.TBox_ActNuc3.AccessibleDescription = Nothing
        Me.TBox_ActNuc3.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_ActNuc3, "TBox_ActNuc3")
        Me.TBox_ActNuc3.BackgroundImage = Nothing
        Me.TBox_ActNuc3.Name = "TBox_ActNuc3"
        '
        'Lbl_ActNuc3
        '
        Me.Lbl_ActNuc3.AccessibleDescription = Nothing
        Me.Lbl_ActNuc3.AccessibleName = Nothing
        resources.ApplyResources(Me.Lbl_ActNuc3, "Lbl_ActNuc3")
        Me.Lbl_ActNuc3.ForeColor = System.Drawing.Color.White
        Me.Lbl_ActNuc3.Name = "Lbl_ActNuc3"
        '
        'TBox_Prob
        '
        Me.TBox_Prob.AccessibleDescription = Nothing
        Me.TBox_Prob.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_Prob, "TBox_Prob")
        Me.TBox_Prob.BackgroundImage = Nothing
        Me.TBox_Prob.ForeColor = System.Drawing.Color.Red
        Me.TBox_Prob.Name = "TBox_Prob"
        '
        'Lbl_Prob
        '
        Me.Lbl_Prob.AccessibleDescription = Nothing
        Me.Lbl_Prob.AccessibleName = Nothing
        resources.ApplyResources(Me.Lbl_Prob, "Lbl_Prob")
        Me.Lbl_Prob.ForeColor = System.Drawing.Color.Red
        Me.Lbl_Prob.Name = "Lbl_Prob"
        '
        'BtnChange
        '
        Me.BtnChange.AccessibleDescription = Nothing
        Me.BtnChange.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnChange, "BtnChange")
        Me.BtnChange.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnChange.BackgroundImage = Nothing
        Me.BtnChange.Name = "BtnChange"
        Me.BtnChange.UseVisualStyleBackColor = False
        '
        'frmCalibrationAnalyzeControl
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnChange)
        Me.Controls.Add(Me.Lbl_Prob)
        Me.Controls.Add(Me.TBox_Prob)
        Me.Controls.Add(Me.TBox_ActNuc3)
        Me.Controls.Add(Me.Lbl_ActNuc3)
        Me.Controls.Add(Me.TBox_ActNuc2)
        Me.Controls.Add(Me.Lbl_ActNuc2)
        Me.Controls.Add(Me.TBox_ActNuc1)
        Me.Controls.Add(Me.Lbl_ActNuc1)
        Me.Controls.Add(Me.Lbl_CommandText)
        Me.Controls.Add(Me.TBox_DateTime)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnAnalyze)
        Me.Controls.Add(Me.BtnClose)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCalibrationAnalyzeControl"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnAnalyze As System.Windows.Forms.Button
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents TBox_DateTime As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Lbl_CommandText As System.Windows.Forms.Label
    Friend WithEvents TBox_ActNuc1 As System.Windows.Forms.TextBox
    Friend WithEvents Lbl_ActNuc1 As System.Windows.Forms.Label
    Friend WithEvents TBox_ActNuc2 As System.Windows.Forms.TextBox
    Friend WithEvents Lbl_ActNuc2 As System.Windows.Forms.Label
    Friend WithEvents TBox_ActNuc3 As System.Windows.Forms.TextBox
    Friend WithEvents Lbl_ActNuc3 As System.Windows.Forms.Label
    Friend WithEvents TBox_Prob As System.Windows.Forms.TextBox
    Friend WithEvents Lbl_Prob As System.Windows.Forms.Label
    Friend WithEvents BtnChange As System.Windows.Forms.Button
End Class
