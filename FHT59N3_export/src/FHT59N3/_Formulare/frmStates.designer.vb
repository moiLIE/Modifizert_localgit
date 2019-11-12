<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStates
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmStates))
        Me.BtnClose = New System.Windows.Forms.Button
        Me.StateList = New ThermoControls.ThermoListViewEx
        Me.TimerCheckChanges = New System.Windows.Forms.Timer(Me.components)
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.BtnDown = New System.Windows.Forms.Button
        Me.BtnUp = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.TimerStart = New System.Windows.Forms.Timer(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.Label3 = New System.Windows.Forms.Label
        Me.LblHighVoltage = New System.Windows.Forms.Label
        Me.LblAlarmIndication = New System.Windows.Forms.Label
        Me.LblFilterstep = New System.Windows.Forms.Label
        Me.LblN2Fill = New System.Windows.Forms.Label
        Me.LblErrorIndication = New System.Windows.Forms.Label
        Me.LblPump = New System.Windows.Forms.Label
        Me.LblFilterBypass = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'BtnClose
        '
        Me.BtnClose.AccessibleDescription = Nothing
        Me.BtnClose.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnClose.BackgroundImage = Nothing
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'StateList
        '
        Me.StateList.AccessibleDescription = Nothing
        Me.StateList.AccessibleName = Nothing
        resources.ApplyResources(Me.StateList, "StateList")
        Me.StateList.BackgroundImage = Nothing
        Me.StateList.GridLineColor = System.Drawing.Color.Black
        Me.StateList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.StateList.ItemHighlightColor = System.Drawing.SystemColors.Highlight
        Me.StateList.ItemNotFocusedHighlighColor = System.Drawing.SystemColors.MenuBar
        Me.StateList.Name = "StateList"
        Me.StateList.UseCompatibleStateImageBehavior = False
        Me.StateList.UseDefaultGridLines = False
        '
        'TimerCheckChanges
        '
        Me.TimerCheckChanges.Interval = 1000
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
        'BtnDown
        '
        Me.BtnDown.AccessibleDescription = Nothing
        Me.BtnDown.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnDown, "BtnDown")
        Me.BtnDown.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnDown.BackgroundImage = Global.FHT59N3.My.Resources.Resources.stock_down
        Me.BtnDown.Font = Nothing
        Me.BtnDown.Name = "BtnDown"
        Me.BtnDown.UseVisualStyleBackColor = False
        '
        'BtnUp
        '
        Me.BtnUp.AccessibleDescription = Nothing
        Me.BtnUp.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnUp, "BtnUp")
        Me.BtnUp.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnUp.BackgroundImage = Global.FHT59N3.My.Resources.Resources.stock_up
        Me.BtnUp.Name = "BtnUp"
        Me.BtnUp.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Name = "Label1"
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = Nothing
        Me.Label2.AccessibleName = Nothing
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Name = "Label2"
        '
        'TimerStart
        '
        Me.TimerStart.Interval = 10
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        resources.ApplyResources(Me.ImageList1, "ImageList1")
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'LblHighVoltage
        '
        Me.LblHighVoltage.AccessibleDescription = Nothing
        Me.LblHighVoltage.AccessibleName = Nothing
        resources.ApplyResources(Me.LblHighVoltage, "LblHighVoltage")
        Me.LblHighVoltage.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblHighVoltage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblHighVoltage.Name = "LblHighVoltage"
        '
        'LblAlarmIndication
        '
        Me.LblAlarmIndication.AccessibleDescription = Nothing
        Me.LblAlarmIndication.AccessibleName = Nothing
        resources.ApplyResources(Me.LblAlarmIndication, "LblAlarmIndication")
        Me.LblAlarmIndication.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblAlarmIndication.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblAlarmIndication.Name = "LblAlarmIndication"
        '
        'LblFilterstep
        '
        Me.LblFilterstep.AccessibleDescription = Nothing
        Me.LblFilterstep.AccessibleName = Nothing
        resources.ApplyResources(Me.LblFilterstep, "LblFilterstep")
        Me.LblFilterstep.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblFilterstep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblFilterstep.Name = "LblFilterstep"
        '
        'LblN2Fill
        '
        Me.LblN2Fill.AccessibleDescription = Nothing
        Me.LblN2Fill.AccessibleName = Nothing
        resources.ApplyResources(Me.LblN2Fill, "LblN2Fill")
        Me.LblN2Fill.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblN2Fill.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblN2Fill.Name = "LblN2Fill"
        '
        'LblErrorIndication
        '
        Me.LblErrorIndication.AccessibleDescription = Nothing
        Me.LblErrorIndication.AccessibleName = Nothing
        resources.ApplyResources(Me.LblErrorIndication, "LblErrorIndication")
        Me.LblErrorIndication.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblErrorIndication.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblErrorIndication.Name = "LblErrorIndication"
        '
        'LblPump
        '
        Me.LblPump.AccessibleDescription = Nothing
        Me.LblPump.AccessibleName = Nothing
        resources.ApplyResources(Me.LblPump, "LblPump")
        Me.LblPump.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblPump.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblPump.Name = "LblPump"
        '
        'LblFilterBypass
        '
        Me.LblFilterBypass.AccessibleDescription = Nothing
        Me.LblFilterBypass.AccessibleName = Nothing
        resources.ApplyResources(Me.LblFilterBypass, "LblFilterBypass")
        Me.LblFilterBypass.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblFilterBypass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblFilterBypass.Name = "LblFilterBypass"
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = Nothing
        Me.Label4.AccessibleName = Nothing
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Name = "Label4"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = Nothing
        Me.Label5.AccessibleName = Nothing
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Name = "Label5"
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = Nothing
        Me.Label6.AccessibleName = Nothing
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Name = "Label6"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = Nothing
        Me.Label7.AccessibleName = Nothing
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Name = "Label7"
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = Nothing
        Me.Label8.AccessibleName = Nothing
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Name = "Label8"
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = Nothing
        Me.Label9.AccessibleName = Nothing
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label9.ForeColor = System.Drawing.Color.White
        Me.Label9.Name = "Label9"
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = Nothing
        Me.Label10.AccessibleName = Nothing
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label10.ForeColor = System.Drawing.Color.White
        Me.Label10.Name = "Label10"
        '
        'frmStates
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.LblHighVoltage)
        Me.Controls.Add(Me.LblAlarmIndication)
        Me.Controls.Add(Me.LblFilterstep)
        Me.Controls.Add(Me.LblN2Fill)
        Me.Controls.Add(Me.LblErrorIndication)
        Me.Controls.Add(Me.LblPump)
        Me.Controls.Add(Me.LblFilterBypass)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnDown)
        Me.Controls.Add(Me.BtnUp)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.StateList)
        Me.Controls.Add(Me.BtnClose)
        Me.Font = Nothing
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmStates"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents StateList As ThermoControls.ThermoListViewEx
    Friend WithEvents TimerCheckChanges As System.Windows.Forms.Timer
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents BtnDown As System.Windows.Forms.Button
    Friend WithEvents BtnUp As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TimerStart As System.Windows.Forms.Timer
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents LblHighVoltage As System.Windows.Forms.Label
    Friend WithEvents LblAlarmIndication As System.Windows.Forms.Label
    Friend WithEvents LblFilterstep As System.Windows.Forms.Label
    Friend WithEvents LblN2Fill As System.Windows.Forms.Label
    Friend WithEvents LblErrorIndication As System.Windows.Forms.Label
    Friend WithEvents LblPump As System.Windows.Forms.Label
    Friend WithEvents LblFilterBypass As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
End Class
