<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMsgBox

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsgBox))

        Me.BtnCancel = New System.Windows.Forms.Button
        Me.BtnOK = New System.Windows.Forms.Button
        Me.LabelMsg = New System.Windows.Forms.Label
        Me.BtnNo = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'BtnCancel
        '
        Me.BtnCancel.AccessibleDescription = Nothing
        Me.BtnCancel.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnCancel.BackgroundImage = Nothing
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = False
        '
        'BtnOK
        '
        Me.BtnOK.AccessibleDescription = Nothing
        Me.BtnOK.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnOK.BackgroundImage = Nothing
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = False
        '
        'LabelMsg
        '
        Me.LabelMsg.AccessibleDescription = Nothing
        Me.LabelMsg.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelMsg, "LabelMsg")
        Me.LabelMsg.BackColor = System.Drawing.Color.Transparent
        Me.LabelMsg.Name = "LabelMsg"
        '
        'BtnNo
        '
        Me.BtnNo.AccessibleDescription = Nothing
        Me.BtnNo.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnNo, "BtnNo")
        Me.BtnNo.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnNo.BackgroundImage = Nothing
        Me.BtnNo.Name = "BtnNo"
        Me.BtnNo.UseVisualStyleBackColor = False
        '
        'frmMsgBox
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnNo)
        Me.Controls.Add(Me.LabelMsg)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.BtnCancel)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMsgBox"
        Me.ResumeLayout(False)

    End Sub

    Protected Friend WithEvents BtnCancel As System.Windows.Forms.Button
    Protected Friend WithEvents BtnOK As System.Windows.Forms.Button
    Protected Friend WithEvents LabelMsg As System.Windows.Forms.Label
    Protected Friend WithEvents BtnNo As System.Windows.Forms.Button

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
