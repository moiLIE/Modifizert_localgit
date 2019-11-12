<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMainMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMainMenu))
        Me.BtnMaintenance = New System.Windows.Forms.Button
        Me.BtnOperations = New System.Windows.Forms.Button
        Me.BtnReturn = New System.Windows.Forms.Button
        Me.BtnAbout = New System.Windows.Forms.Button
        Me.BtnFile = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'BtnMaintenance
        '
        Me.BtnMaintenance.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnMaintenance, "BtnMaintenance")
        Me.BtnMaintenance.Name = "BtnMaintenance"
        Me.BtnMaintenance.UseVisualStyleBackColor = False
        '
        'BtnOperations
        '
        Me.BtnOperations.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnOperations, "BtnOperations")
        Me.BtnOperations.Name = "BtnOperations"
        Me.BtnOperations.UseVisualStyleBackColor = False
        '
        'BtnReturn
        '
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnAbout
        '
        Me.BtnAbout.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAbout, "BtnAbout")
        Me.BtnAbout.Name = "BtnAbout"
        Me.BtnAbout.UseVisualStyleBackColor = False
        '
        'BtnFile
        '
        Me.BtnFile.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnFile, "BtnFile")
        Me.BtnFile.Name = "BtnFile"
        Me.BtnFile.UseVisualStyleBackColor = False
        '
        'frmMainMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnFile)
        Me.Controls.Add(Me.BtnAbout)
        Me.Controls.Add(Me.BtnReturn)
        Me.Controls.Add(Me.BtnOperations)
        Me.Controls.Add(Me.BtnMaintenance)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMainMenu"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnMaintenance As System.Windows.Forms.Button
    Friend WithEvents BtnOperations As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnAbout As System.Windows.Forms.Button
    Friend WithEvents BtnFile As System.Windows.Forms.Button
End Class
