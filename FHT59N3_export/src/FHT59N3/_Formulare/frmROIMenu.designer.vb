<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmROIMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmROIMenu))
        Me.BtnAddROI = New System.Windows.Forms.Button
        Me.BtnReturn = New System.Windows.Forms.Button
        Me.BtnDeleteROI = New System.Windows.Forms.Button
        Me.BtnDeleteAllROIs = New System.Windows.Forms.Button
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.BtnLoadROIs = New System.Windows.Forms.Button
        Me.BtnSaveROIs = New System.Windows.Forms.Button
        Me.BtnPrevROI = New System.Windows.Forms.Button
        Me.BtnNextROI = New System.Windows.Forms.Button
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.SuspendLayout()
        '
        'BtnAddROI
        '
        Me.BtnAddROI.AccessibleDescription = Nothing
        Me.BtnAddROI.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnAddROI, "BtnAddROI")
        Me.BtnAddROI.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnAddROI.BackgroundImage = Nothing
        Me.BtnAddROI.Name = "BtnAddROI"
        Me.BtnAddROI.UseVisualStyleBackColor = False
        '
        'BtnReturn
        '
        Me.BtnReturn.AccessibleDescription = Nothing
        Me.BtnReturn.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnReturn.BackgroundImage = Nothing
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnDeleteROI
        '
        Me.BtnDeleteROI.AccessibleDescription = Nothing
        Me.BtnDeleteROI.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnDeleteROI, "BtnDeleteROI")
        Me.BtnDeleteROI.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnDeleteROI.BackgroundImage = Nothing
        Me.BtnDeleteROI.Name = "BtnDeleteROI"
        Me.BtnDeleteROI.UseVisualStyleBackColor = False
        '
        'BtnDeleteAllROIs
        '
        Me.BtnDeleteAllROIs.AccessibleDescription = Nothing
        Me.BtnDeleteAllROIs.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnDeleteAllROIs, "BtnDeleteAllROIs")
        Me.BtnDeleteAllROIs.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnDeleteAllROIs.BackgroundImage = Nothing
        Me.BtnDeleteAllROIs.Name = "BtnDeleteAllROIs"
        Me.BtnDeleteAllROIs.UseVisualStyleBackColor = False
        '
        'OpenFileDialog1
        '
        resources.ApplyResources(Me.OpenFileDialog1, "OpenFileDialog1")
        '
        'BtnLoadROIs
        '
        Me.BtnLoadROIs.AccessibleDescription = Nothing
        Me.BtnLoadROIs.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnLoadROIs, "BtnLoadROIs")
        Me.BtnLoadROIs.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnLoadROIs.BackgroundImage = Nothing
        Me.BtnLoadROIs.Name = "BtnLoadROIs"
        Me.BtnLoadROIs.UseVisualStyleBackColor = False
        '
        'BtnSaveROIs
        '
        Me.BtnSaveROIs.AccessibleDescription = Nothing
        Me.BtnSaveROIs.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnSaveROIs, "BtnSaveROIs")
        Me.BtnSaveROIs.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnSaveROIs.BackgroundImage = Nothing
        Me.BtnSaveROIs.Name = "BtnSaveROIs"
        Me.BtnSaveROIs.UseVisualStyleBackColor = False
        '
        'BtnPrevROI
        '
        Me.BtnPrevROI.AccessibleDescription = Nothing
        Me.BtnPrevROI.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnPrevROI, "BtnPrevROI")
        Me.BtnPrevROI.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnPrevROI.BackgroundImage = Nothing
        Me.BtnPrevROI.Name = "BtnPrevROI"
        Me.BtnPrevROI.UseVisualStyleBackColor = False
        '
        'BtnNextROI
        '
        Me.BtnNextROI.AccessibleDescription = Nothing
        Me.BtnNextROI.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnNextROI, "BtnNextROI")
        Me.BtnNextROI.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnNextROI.BackgroundImage = Nothing
        Me.BtnNextROI.Name = "BtnNextROI"
        Me.BtnNextROI.UseVisualStyleBackColor = False
        '
        'SaveFileDialog1
        '
        resources.ApplyResources(Me.SaveFileDialog1, "SaveFileDialog1")
        '
        'frmROIMenu
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnPrevROI)
        Me.Controls.Add(Me.BtnNextROI)
        Me.Controls.Add(Me.BtnLoadROIs)
        Me.Controls.Add(Me.BtnSaveROIs)
        Me.Controls.Add(Me.BtnDeleteAllROIs)
        Me.Controls.Add(Me.BtnDeleteROI)
        Me.Controls.Add(Me.BtnReturn)
        Me.Controls.Add(Me.BtnAddROI)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmROIMenu"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnAddROI As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnDeleteROI As System.Windows.Forms.Button
    Friend WithEvents BtnDeleteAllROIs As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents BtnLoadROIs As System.Windows.Forms.Button
    Friend WithEvents BtnSaveROIs As System.Windows.Forms.Button
    Friend WithEvents BtnPrevROI As System.Windows.Forms.Button
    Friend WithEvents BtnNextROI As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
End Class
