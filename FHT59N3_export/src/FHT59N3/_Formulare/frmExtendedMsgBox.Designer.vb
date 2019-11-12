<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmExtendedMsgBox

    Inherits FHT59N3.frmMsgBox

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExtendedMsgBox))
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.CheckBox2 = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'LabelMsg
        '
        resources.ApplyResources(Me.LabelMsg, "LabelMsg")
        '
        'CheckBox1
        '
        Me.CheckBox1.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.CheckBox1, "CheckBox1")
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.UseVisualStyleBackColor = False
        '
        'CheckBox2
        '
        Me.CheckBox2.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.CheckBox2, "CheckBox2")
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.UseVisualStyleBackColor = False
        '
        'frmExtendedMsgBox
        '
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.CheckBox2)
        Me.Controls.Add(Me.CheckBox1)
        Me.Name = "frmExtendedMsgBox"
        Me.Controls.SetChildIndex(Me.BtnCancel, 0)
        Me.Controls.SetChildIndex(Me.BtnOK, 0)
        Me.Controls.SetChildIndex(Me.BtnNo, 0)
        Me.Controls.SetChildIndex(Me.CheckBox1, 0)
        Me.Controls.SetChildIndex(Me.CheckBox2, 0)
        Me.Controls.SetChildIndex(Me.LabelMsg, 0)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox

End Class
