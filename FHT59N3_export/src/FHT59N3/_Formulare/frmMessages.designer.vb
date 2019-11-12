<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMessages
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMessages))
        Me.BtnClose = New System.Windows.Forms.Button
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnUp = New System.Windows.Forms.Button
        Me.TimerStart = New System.Windows.Forms.Timer(Me.components)
        Me.TimerCheckChanges = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'BtnClose
        '
        Me.BtnClose.AccessibleDescription = Nothing
        Me.BtnClose.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.BackgroundImage = Nothing
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'ListView1
        '
        Me.ListView1.AccessibleDescription = Nothing
        Me.ListView1.AccessibleName = Nothing
        resources.ApplyResources(Me.ListView1, "ListView1")
        Me.ListView1.BackColor = System.Drawing.Color.White
        Me.ListView1.BackgroundImage = Nothing
        Me.ListView1.ForeColor = System.Drawing.Color.Black
        Me.ListView1.FullRowSelect = True
        Me.ListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListView1.Name = "ListView1"
        Me.ListView1.ShowGroups = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        resources.ApplyResources(Me.ImageList1, "ImageList1")
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'BtnDown
        '
        Me.BtnDown.AccessibleDescription = Nothing
        Me.BtnDown.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnDown, "BtnDown")
        Me.BtnDown.BackgroundImage = Global.FHT59N3.My.Resources.Resources.stock_down
        Me.BtnDown.Font = Nothing
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.UseVisualStyleBackColor = True
        '
        'BtnUp
        '
        Me.BtnUp.AccessibleDescription = Nothing
        Me.BtnUp.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnUp, "BtnUp")
        Me.BtnUp.BackgroundImage = Global.FHT59N3.My.Resources.Resources.stock_up
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.UseVisualStyleBackColor = True
        '
        'TimerStart
        '
        Me.TimerStart.Interval = 10
        '
        'TimerCheckChanges
        '
        Me.TimerCheckChanges.Interval = 1000
        '
        'frmMessages
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.BtnClose)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMessages"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents TimerStart As System.Windows.Forms.Timer
    Friend WithEvents TimerCheckChanges As System.Windows.Forms.Timer
End Class
