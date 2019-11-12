<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNuclideLibrary
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNuclideLibrary))
        Me.BtnOk = New System.Windows.Forms.Button()
        Me.DataGridLibraryNuclides = New System.Windows.Forms.DataGridView()
        Me.BtnExport = New System.Windows.Forms.Button()
        Me.ColumnNuclideIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnNuclide = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnHalflive = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DataGridLibraryNuclides, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnOk
        '
        resources.ApplyResources(Me.BtnOk, "BtnOk")
        Me.BtnOk.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnOk.ImageKey = Global.FHT59N3.MultiLang._504
        Me.BtnOk.Name = "BtnOk"
        Me.BtnOk.UseVisualStyleBackColor = False
        '
        'DataGridLibraryNuclides
        '
        resources.ApplyResources(Me.DataGridLibraryNuclides, "DataGridLibraryNuclides")
        Me.DataGridLibraryNuclides.AllowUserToAddRows = False
        Me.DataGridLibraryNuclides.AllowUserToDeleteRows = False
        Me.DataGridLibraryNuclides.BackgroundColor = System.Drawing.SystemColors.Control
        Me.DataGridLibraryNuclides.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridLibraryNuclides.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColumnNuclideIndex, Me.ColumnNuclide, Me.ColumnHalflive, Me.ColumnNumber})
        Me.DataGridLibraryNuclides.MultiSelect = False
        Me.DataGridLibraryNuclides.Name = "DataGridLibraryNuclides"
        Me.DataGridLibraryNuclides.RowHeadersVisible = False
        Me.DataGridLibraryNuclides.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DataGridLibraryNuclides.RowTemplate.Height = 30
        Me.DataGridLibraryNuclides.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'BtnExport
        '
        resources.ApplyResources(Me.BtnExport, "BtnExport")
        Me.BtnExport.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnExport.ImageKey = Global.FHT59N3.MultiLang._504
        Me.BtnExport.Name = "BtnExport"
        Me.BtnExport.UseVisualStyleBackColor = False
        '
        'ColumnNuclideIndex
        '
        resources.ApplyResources(Me.ColumnNuclideIndex, "ColumnNuclideIndex")
        Me.ColumnNuclideIndex.Name = "ColumnNuclideIndex"
        Me.ColumnNuclideIndex.ToolTipText = Global.FHT59N3.MultiLang._504
        '
        'ColumnNuclide
        '
        Me.ColumnNuclide.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.ColumnNuclide, "ColumnNuclide")
        Me.ColumnNuclide.Name = "ColumnNuclide"
        Me.ColumnNuclide.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ColumnNuclide.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.ColumnNuclide.ToolTipText = Global.FHT59N3.MultiLang._504
        '
        'ColumnHalflive
        '
        Me.ColumnHalflive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.ColumnHalflive, "ColumnHalflive")
        Me.ColumnHalflive.Name = "ColumnHalflive"
        Me.ColumnHalflive.ToolTipText = Global.FHT59N3.MultiLang._504
        '
        'ColumnNumber
        '
        resources.ApplyResources(Me.ColumnNumber, "ColumnNumber")
        Me.ColumnNumber.Name = "ColumnNumber"
        Me.ColumnNumber.ToolTipText = Global.FHT59N3.MultiLang._504
        '
        'frmNuclideLibrary
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.BtnExport)
        Me.Controls.Add(Me.DataGridLibraryNuclides)
        Me.Controls.Add(Me.BtnOk)
        Me.Name = "frmNuclideLibrary"
        CType(Me.DataGridLibraryNuclides, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnOk As System.Windows.Forms.Button
    Friend WithEvents DataGridLibraryNuclides As System.Windows.Forms.DataGridView
    Friend WithEvents BtnExport As System.Windows.Forms.Button
    Friend WithEvents ColumnNuclideIndex As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnNuclide As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnHalflive As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnNumber As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
