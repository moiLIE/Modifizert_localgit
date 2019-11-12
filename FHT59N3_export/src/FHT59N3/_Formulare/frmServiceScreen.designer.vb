<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmServiceScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmServiceScreen))
        Me.PanelfrmServiceScreen = New System.Windows.Forms.Panel()
        Me.LabelHint = New System.Windows.Forms.Label()
        Me.ServiceList = New ThermoControls.ThermoListViewEx()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.PanelfrmServiceScreen.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelfrmServiceScreen
        '
        Me.PanelfrmServiceScreen.Controls.Add(Me.LabelHint)
        Me.PanelfrmServiceScreen.Controls.Add(Me.ServiceList)
        resources.ApplyResources(Me.PanelfrmServiceScreen, "PanelfrmServiceScreen")
        Me.PanelfrmServiceScreen.Name = "PanelfrmServiceScreen"
        '
        'LabelHint
        '
        resources.ApplyResources(Me.LabelHint, "LabelHint")
        Me.LabelHint.Name = "LabelHint"
        '
        'ServiceList
        '
        resources.ApplyResources(Me.ServiceList, "ServiceList")
        Me.ServiceList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.ServiceList.GridLineColor = System.Drawing.Color.Silver
        Me.ServiceList.GridLines = True
        Me.ServiceList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ServiceList.ItemHighlightColor = System.Drawing.SystemColors.Highlight
        Me.ServiceList.ItemNotFocusedHighlighColor = System.Drawing.SystemColors.MenuBar
        Me.ServiceList.Name = "ServiceList"
        Me.ServiceList.UseCompatibleStateImageBehavior = False
        Me.ServiceList.UseDefaultGridLines = False
        Me.ServiceList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        resources.ApplyResources(Me.ColumnHeader1, "ColumnHeader1")
        '
        'ColumnHeader2
        '
        resources.ApplyResources(Me.ColumnHeader2, "ColumnHeader2")
        '
        'frmServiceScreen
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.PanelfrmServiceScreen)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmServiceScreen"
        Me.PanelfrmServiceScreen.ResumeLayout(False)
        Me.PanelfrmServiceScreen.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelfrmServiceScreen As System.Windows.Forms.Panel
    Friend WithEvents ServiceList As ThermoControls.ThermoListViewEx
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents LabelHint As System.Windows.Forms.Label
End Class
