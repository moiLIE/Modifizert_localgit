<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCalibrationMeasControl
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalibrationMeasControl))
        Me.BtnStartCalib = New System.Windows.Forms.Button
        Me.BtnClose = New System.Windows.Forms.Button
        Me.TBox_DateTime = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Lbl_CommandText = New System.Windows.Forms.Label
        Me.TBox_MeasTime = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'BtnStartCalib
        '
        Me.BtnStartCalib.AccessibleDescription = Nothing
        Me.BtnStartCalib.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnStartCalib, "BtnStartCalib")
        Me.BtnStartCalib.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnStartCalib.BackgroundImage = Nothing
        Me.BtnStartCalib.Name = "BtnStartCalib"
        Me.BtnStartCalib.UseVisualStyleBackColor = False
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
        'TBox_MeasTime
        '
        Me.TBox_MeasTime.AccessibleDescription = Nothing
        Me.TBox_MeasTime.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_MeasTime, "TBox_MeasTime")
        Me.TBox_MeasTime.BackgroundImage = Nothing
        Me.TBox_MeasTime.Name = "TBox_MeasTime"
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'frmCalibrationMeasControl
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.TBox_MeasTime)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Lbl_CommandText)
        Me.Controls.Add(Me.TBox_DateTime)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnStartCalib)
        Me.Controls.Add(Me.BtnClose)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCalibrationMeasControl"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnStartCalib As System.Windows.Forms.Button
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents TBox_DateTime As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Lbl_CommandText As System.Windows.Forms.Label
    Friend WithEvents TBox_MeasTime As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
