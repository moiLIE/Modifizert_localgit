<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditAlarmNuclides
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditAlarmNuclides))
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.DataGridAlarmNuclides = New System.Windows.Forms.DataGridView()
        Me.ColumnNuclide = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.ColumnAlarm1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnAlarm2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BtnAdd = New System.Windows.Forms.Button()
        Me.BtnDelete = New System.Windows.Forms.Button()
        CType(Me.DataGridAlarmNuclides, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnSave
        '
        resources.ApplyResources(Me.BtnSave, "BtnSave")
        Me.BtnSave.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.UseVisualStyleBackColor = False
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = False
        '
        'DataGridAlarmNuclides
        '
        Me.DataGridAlarmNuclides.AllowUserToAddRows = False
        Me.DataGridAlarmNuclides.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.DataGridAlarmNuclides, "DataGridAlarmNuclides")
        Me.DataGridAlarmNuclides.BackgroundColor = System.Drawing.SystemColors.Control
        Me.DataGridAlarmNuclides.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridAlarmNuclides.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColumnNuclide, Me.ColumnAlarm1, Me.ColumnAlarm2})
        Me.DataGridAlarmNuclides.MultiSelect = False
        Me.DataGridAlarmNuclides.Name = "DataGridAlarmNuclides"
        Me.DataGridAlarmNuclides.RowHeadersVisible = False
        Me.DataGridAlarmNuclides.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DataGridAlarmNuclides.RowTemplate.Height = 30
        Me.DataGridAlarmNuclides.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'ColumnNuclide
        '
        Me.ColumnNuclide.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.ColumnNuclide, "ColumnNuclide")
        Me.ColumnNuclide.Name = "ColumnNuclide"
        '
        'ColumnAlarm1
        '
        Me.ColumnAlarm1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.ColumnAlarm1, "ColumnAlarm1")
        Me.ColumnAlarm1.Name = "ColumnAlarm1"
        '
        'ColumnAlarm2
        '
        Me.ColumnAlarm2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.ColumnAlarm2, "ColumnAlarm2")
        Me.ColumnAlarm2.Name = "ColumnAlarm2"
        '
        'BtnAdd
        '
        resources.ApplyResources(Me.BtnAdd, "BtnAdd")
        Me.BtnAdd.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnAdd.Name = "BtnAdd"
        Me.BtnAdd.UseVisualStyleBackColor = False
        '
        'BtnDelete
        '
        resources.ApplyResources(Me.BtnDelete, "BtnDelete")
        Me.BtnDelete.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.UseVisualStyleBackColor = False
        '
        'frmEditAlarmNuclides
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnAdd)
        Me.Controls.Add(Me.DataGridAlarmNuclides)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnSave)
        Me.Name = "frmEditAlarmNuclides"
        CType(Me.DataGridAlarmNuclides, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnSave As System.Windows.Forms.Button
    Friend WithEvents BtnCancel As System.Windows.Forms.Button
    Friend WithEvents DataGridAlarmNuclides As System.Windows.Forms.DataGridView
    Friend WithEvents BtnAdd As System.Windows.Forms.Button
    Friend WithEvents BtnDelete As System.Windows.Forms.Button
    Friend WithEvents ColumnNuclide As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents ColumnAlarm1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnAlarm2 As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
