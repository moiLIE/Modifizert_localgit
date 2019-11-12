<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFileMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileMenu))
        Me.BtnAnalyzeSpectrum = New System.Windows.Forms.Button()
        Me.BtnReturn = New System.Windows.Forms.Button()
        Me.BtnShowSpectrum = New System.Windows.Forms.Button()
        Me.BtnShowResultFile = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.checkBoxCheckAlarms = New System.Windows.Forms.CheckBox()
        Me.toolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'BtnAnalyzeSpectrum
        '
        Me.BtnAnalyzeSpectrum.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAnalyzeSpectrum, "BtnAnalyzeSpectrum")
        Me.BtnAnalyzeSpectrum.Name = "BtnAnalyzeSpectrum"
        Me.BtnAnalyzeSpectrum.UseVisualStyleBackColor = False
        '
        'BtnReturn
        '
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnShowSpectrum
        '
        Me.BtnShowSpectrum.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnShowSpectrum, "BtnShowSpectrum")
        Me.BtnShowSpectrum.Name = "BtnShowSpectrum"
        Me.BtnShowSpectrum.UseVisualStyleBackColor = False
        '
        'BtnShowResultFile
        '
        Me.BtnShowResultFile.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnShowResultFile, "BtnShowResultFile")
        Me.BtnShowResultFile.Name = "BtnShowResultFile"
        Me.BtnShowResultFile.UseVisualStyleBackColor = False
        '
        'OpenFileDialog1
        '
        resources.ApplyResources(Me.OpenFileDialog1, "OpenFileDialog1")
        '
        'checkBoxCheckAlarms
        '
        resources.ApplyResources(Me.checkBoxCheckAlarms, "checkBoxCheckAlarms")
        Me.checkBoxCheckAlarms.Name = "checkBoxCheckAlarms"
        Me.toolTip.SetToolTip(Me.checkBoxCheckAlarms, resources.GetString("checkBoxCheckAlarms.ToolTip"))
        Me.checkBoxCheckAlarms.UseVisualStyleBackColor = True
        '
        'frmFileMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.checkBoxCheckAlarms)
        Me.Controls.Add(Me.BtnShowResultFile)
        Me.Controls.Add(Me.BtnShowSpectrum)
        Me.Controls.Add(Me.BtnReturn)
        Me.Controls.Add(Me.BtnAnalyzeSpectrum)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFileMenu"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnAnalyzeSpectrum As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnShowSpectrum As System.Windows.Forms.Button
    Friend WithEvents BtnShowResultFile As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents checkBoxCheckAlarms As System.Windows.Forms.CheckBox
    Friend WithEvents toolTip As System.Windows.Forms.ToolTip
End Class
