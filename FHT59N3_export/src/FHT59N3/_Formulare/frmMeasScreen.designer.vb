<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMeasScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMeasScreen))
        Me.PanelfrmMeasScreen = New System.Windows.Forms.Panel()
        Me.SpectralDisplay = New AxCanberraDataDisplayLib.AxMvc()
        Me.RtbAnalyzeData = New System.Windows.Forms.RichTextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ListView3 = New System.Windows.Forms.ListView()
        Me.ListView2 = New System.Windows.Forms.RichTextBox()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ContextMenuStripMsgList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteMessageListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PanelfrmMeasScreen.SuspendLayout()
        CType(Me.SpectralDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStripMsgList.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelfrmMeasScreen
        '
        resources.ApplyResources(Me.PanelfrmMeasScreen, "PanelfrmMeasScreen")
        Me.PanelfrmMeasScreen.Controls.Add(Me.SpectralDisplay)
        Me.PanelfrmMeasScreen.Controls.Add(Me.RtbAnalyzeData)
        Me.PanelfrmMeasScreen.Controls.Add(Me.Label3)
        Me.PanelfrmMeasScreen.Controls.Add(Me.Label2)
        Me.PanelfrmMeasScreen.Controls.Add(Me.Label1)
        Me.PanelfrmMeasScreen.Controls.Add(Me.ListView3)
        Me.PanelfrmMeasScreen.Controls.Add(Me.ListView2)
        Me.PanelfrmMeasScreen.Controls.Add(Me.ListView1)
        Me.PanelfrmMeasScreen.Name = "PanelfrmMeasScreen"
        '
        'SpectralDisplay
        '
        resources.ApplyResources(Me.SpectralDisplay, "SpectralDisplay")
        Me.SpectralDisplay.Name = "SpectralDisplay"
        Me.SpectralDisplay.OcxState = CType(resources.GetObject("SpectralDisplay.OcxState"), System.Windows.Forms.AxHost.State)
        '
        'RtbAnalyzeData
        '
        resources.ApplyResources(Me.RtbAnalyzeData, "RtbAnalyzeData")
        Me.RtbAnalyzeData.ForeColor = System.Drawing.Color.Black
        Me.RtbAnalyzeData.Name = "RtbAnalyzeData"
        Me.RtbAnalyzeData.Text = ""
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Name = "Label2"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Name = "Label1"
        '
        'ListView3
        '
        resources.ApplyResources(Me.ListView3, "ListView3")
        Me.ListView3.BackColor = System.Drawing.Color.White
        Me.ListView3.ForeColor = System.Drawing.Color.Black
        Me.ListView3.FullRowSelect = True
        Me.ListView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListView3.Name = "ListView3"
        Me.ListView3.ShowGroups = False
        Me.ListView3.UseCompatibleStateImageBehavior = False
        '
        'ListView2
        '
        resources.ApplyResources(Me.ListView2, "ListView2")
        Me.ListView2.BackColor = System.Drawing.Color.White
        Me.ListView2.ForeColor = System.Drawing.Color.Black
        Me.ListView2.Name = "ListView2"
        Me.ListView2.Text = ""
        '
        'ListView1
        '
        resources.ApplyResources(Me.ListView1, "ListView1")
        Me.ListView1.BackColor = System.Drawing.Color.White
        Me.ListView1.ContextMenuStrip = Me.ContextMenuStripMsgList
        Me.ListView1.ForeColor = System.Drawing.Color.Black
        Me.ListView1.FullRowSelect = True
        Me.ListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListView1.Name = "ListView1"
        Me.ListView1.ShowGroups = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'ContextMenuStripMsgList
        '
        resources.ApplyResources(Me.ContextMenuStripMsgList, "ContextMenuStripMsgList")
        Me.ContextMenuStripMsgList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteMessageListToolStripMenuItem})
        Me.ContextMenuStripMsgList.Name = "ContextMenuStripMsgList"
        '
        'DeleteMessageListToolStripMenuItem
        '
        resources.ApplyResources(Me.DeleteMessageListToolStripMenuItem, "DeleteMessageListToolStripMenuItem")
        Me.DeleteMessageListToolStripMenuItem.Name = "DeleteMessageListToolStripMenuItem"
        '
        'frmMeasScreen
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.PanelfrmMeasScreen)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmMeasScreen"
        Me.PanelfrmMeasScreen.ResumeLayout(False)
        Me.PanelfrmMeasScreen.PerformLayout()
        CType(Me.SpectralDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStripMsgList.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelfrmMeasScreen As System.Windows.Forms.Panel
    Friend WithEvents SpectralDisplay As AxCanberraDataDisplayLib.AxMvc
    Friend WithEvents RtbAnalyzeData As System.Windows.Forms.RichTextBox
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents ListView2 As System.Windows.Forms.RichTextBox
    Friend WithEvents ListView3 As System.Windows.Forms.ListView
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ContextMenuStripMsgList As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteMessageListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
