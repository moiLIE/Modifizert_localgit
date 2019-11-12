<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMCAMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMCAMenu))
        Me.BtnHV = New System.Windows.Forms.Button
        Me.BtnReturn = New System.Windows.Forms.Button
        Me.BtnGainStabilizer = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'BtnHV
        '
        Me.BtnHV.AccessibleDescription = Nothing
        Me.BtnHV.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnHV, "BtnHV")
        Me.BtnHV.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnHV.BackgroundImage = Nothing
        Me.BtnHV.Name = "BtnHV"
        Me.BtnHV.UseVisualStyleBackColor = False
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
        'BtnGainStabilizer
        '
        Me.BtnGainStabilizer.AccessibleDescription = Nothing
        Me.BtnGainStabilizer.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnGainStabilizer, "BtnGainStabilizer")
        Me.BtnGainStabilizer.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnGainStabilizer.BackgroundImage = Nothing
        Me.BtnGainStabilizer.Name = "BtnGainStabilizer"
        Me.BtnGainStabilizer.UseVisualStyleBackColor = False
        '
        'frmMCAMenu
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnGainStabilizer)
        Me.Controls.Add(Me.BtnReturn)
        Me.Controls.Add(Me.BtnHV)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMCAMenu"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnHV As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnGainStabilizer As System.Windows.Forms.Button
End Class
