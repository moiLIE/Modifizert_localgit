<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmResultFile
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmResultFile))
        Me.PrintPreviewDialog1 = New System.Windows.Forms.PrintPreviewDialog
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox
        Me.PageSetupDialog1 = New System.Windows.Forms.PageSetupDialog
        Me.FontDialog1 = New System.Windows.Forms.FontDialog
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.BtnClose = New System.Windows.Forms.Button
        Me.BtnPrint = New System.Windows.Forms.Button
        Me.BtnFont = New System.Windows.Forms.Button
        Me.BtnPageSettings = New System.Windows.Forms.Button
        Me.BtnPageView = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'PrintPreviewDialog1
        '
        Me.PrintPreviewDialog1.AccessibleDescription = Nothing
        Me.PrintPreviewDialog1.AccessibleName = Nothing
        resources.ApplyResources(Me.PrintPreviewDialog1, "PrintPreviewDialog1")
        Me.PrintPreviewDialog1.BackgroundImage = Nothing
        Me.PrintPreviewDialog1.Font = Nothing
        Me.PrintPreviewDialog1.Name = "PrintPreviewDialog1"
        '
        'RichTextBox1
        '
        Me.RichTextBox1.AcceptsTab = True
        Me.RichTextBox1.AccessibleDescription = Nothing
        Me.RichTextBox1.AccessibleName = Nothing
        resources.ApplyResources(Me.RichTextBox1, "RichTextBox1")
        Me.RichTextBox1.BackgroundImage = Nothing
        Me.RichTextBox1.Font = Nothing
        Me.RichTextBox1.Name = "RichTextBox1"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        resources.ApplyResources(Me.OpenFileDialog1, "OpenFileDialog1")
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
        'BtnPrint
        '
        Me.BtnPrint.AccessibleDescription = Nothing
        Me.BtnPrint.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnPrint, "BtnPrint")
        Me.BtnPrint.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnPrint.BackgroundImage = Nothing
        Me.BtnPrint.Name = "BtnPrint"
        Me.BtnPrint.UseVisualStyleBackColor = False
        '
        'BtnFont
        '
        Me.BtnFont.AccessibleDescription = Nothing
        Me.BtnFont.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnFont, "BtnFont")
        Me.BtnFont.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnFont.BackgroundImage = Nothing
        Me.BtnFont.Name = "BtnFont"
        Me.BtnFont.UseVisualStyleBackColor = False
        '
        'BtnPageSettings
        '
        Me.BtnPageSettings.AccessibleDescription = Nothing
        Me.BtnPageSettings.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnPageSettings, "BtnPageSettings")
        Me.BtnPageSettings.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnPageSettings.BackgroundImage = Nothing
        Me.BtnPageSettings.Name = "BtnPageSettings"
        Me.BtnPageSettings.UseVisualStyleBackColor = False
        '
        'BtnPageView
        '
        Me.BtnPageView.AccessibleDescription = Nothing
        Me.BtnPageView.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnPageView, "BtnPageView")
        Me.BtnPageView.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnPageView.BackgroundImage = Nothing
        Me.BtnPageView.Name = "BtnPageView"
        Me.BtnPageView.UseVisualStyleBackColor = False
        '
        'frmResultFile
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnPageView)
        Me.Controls.Add(Me.BtnPageSettings)
        Me.Controls.Add(Me.BtnFont)
        Me.Controls.Add(Me.BtnPrint)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.RichTextBox1)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmResultFile"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PrintPreviewDialog1 As System.Windows.Forms.PrintPreviewDialog
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents PageSetupDialog1 As System.Windows.Forms.PageSetupDialog
    Friend WithEvents FontDialog1 As System.Windows.Forms.FontDialog
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents BtnPrint As System.Windows.Forms.Button
    Friend WithEvents BtnFont As System.Windows.Forms.Button
    Friend WithEvents BtnPageSettings As System.Windows.Forms.Button
    Friend WithEvents BtnPageView As System.Windows.Forms.Button
End Class
