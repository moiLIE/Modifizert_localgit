<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmControlMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmControlMenu))
        Me.Button7 = New System.Windows.Forms.Button()
        Me.BtnFilterstep = New System.Windows.Forms.Button()
        Me.BtnFilterBypass = New System.Windows.Forms.Button()
        Me.BtnPump = New System.Windows.Forms.Button()
        Me.BtnErrorIndication = New System.Windows.Forms.Button()
        Me.BtnN2Fill = New System.Windows.Forms.Button()
        Me.BtnAirFlow = New System.Windows.Forms.Button()
        Me.LblFilterBypass = New System.Windows.Forms.Label()
        Me.LblPump = New System.Windows.Forms.Label()
        Me.LblErrorIndication = New System.Windows.Forms.Label()
        Me.LblN2Fill = New System.Windows.Forms.Label()
        Me.LblFilterstep = New System.Windows.Forms.Label()
        Me.LblAlarmIndication = New System.Windows.Forms.Label()
        Me.BtnAlarm = New System.Windows.Forms.Button()
        Me.BtnHV = New System.Windows.Forms.Button()
        Me.LblHighVoltage = New System.Windows.Forms.Label()
        Me.LblEcoolerState = New System.Windows.Forms.Label()
        Me.BtnEcoolerCtl = New System.Windows.Forms.Button()
        Me.TimerForEcoolerButton = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'Button7
        '
        Me.Button7.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.Button7, "Button7")
        Me.Button7.Name = "Button7"
        Me.Button7.UseVisualStyleBackColor = False
        '
        'BtnFilterstep
        '
        Me.BtnFilterstep.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnFilterstep, "BtnFilterstep")
        Me.BtnFilterstep.Name = "BtnFilterstep"
        Me.BtnFilterstep.UseVisualStyleBackColor = False
        '
        'BtnFilterBypass
        '
        Me.BtnFilterBypass.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnFilterBypass, "BtnFilterBypass")
        Me.BtnFilterBypass.Name = "BtnFilterBypass"
        Me.BtnFilterBypass.UseVisualStyleBackColor = False
        '
        'BtnPump
        '
        Me.BtnPump.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnPump, "BtnPump")
        Me.BtnPump.Name = "BtnPump"
        Me.BtnPump.UseVisualStyleBackColor = False
        '
        'BtnErrorIndication
        '
        Me.BtnErrorIndication.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnErrorIndication, "BtnErrorIndication")
        Me.BtnErrorIndication.Name = "BtnErrorIndication"
        Me.BtnErrorIndication.UseVisualStyleBackColor = False
        '
        'BtnN2Fill
        '
        Me.BtnN2Fill.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnN2Fill, "BtnN2Fill")
        Me.BtnN2Fill.Name = "BtnN2Fill"
        Me.BtnN2Fill.UseVisualStyleBackColor = False
        '
        'BtnAirFlow
        '
        Me.BtnAirFlow.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAirFlow, "BtnAirFlow")
        Me.BtnAirFlow.Name = "BtnAirFlow"
        Me.BtnAirFlow.UseVisualStyleBackColor = False
        '
        'LblFilterBypass
        '
        Me.LblFilterBypass.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblFilterBypass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblFilterBypass, "LblFilterBypass")
        Me.LblFilterBypass.Name = "LblFilterBypass"
        '
        'LblPump
        '
        Me.LblPump.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblPump.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblPump, "LblPump")
        Me.LblPump.Name = "LblPump"
        '
        'LblErrorIndication
        '
        Me.LblErrorIndication.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblErrorIndication.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblErrorIndication, "LblErrorIndication")
        Me.LblErrorIndication.Name = "LblErrorIndication"
        '
        'LblN2Fill
        '
        Me.LblN2Fill.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblN2Fill.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblN2Fill, "LblN2Fill")
        Me.LblN2Fill.Name = "LblN2Fill"
        '
        'LblFilterstep
        '
        Me.LblFilterstep.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblFilterstep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblFilterstep, "LblFilterstep")
        Me.LblFilterstep.Name = "LblFilterstep"
        '
        'LblAlarmIndication
        '
        Me.LblAlarmIndication.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblAlarmIndication.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblAlarmIndication, "LblAlarmIndication")
        Me.LblAlarmIndication.Name = "LblAlarmIndication"
        '
        'BtnAlarm
        '
        Me.BtnAlarm.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAlarm, "BtnAlarm")
        Me.BtnAlarm.Name = "BtnAlarm"
        Me.BtnAlarm.UseVisualStyleBackColor = False
        '
        'BtnHV
        '
        Me.BtnHV.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnHV, "BtnHV")
        Me.BtnHV.Name = "BtnHV"
        Me.BtnHV.UseVisualStyleBackColor = False
        '
        'LblHighVoltage
        '
        Me.LblHighVoltage.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblHighVoltage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblHighVoltage, "LblHighVoltage")
        Me.LblHighVoltage.Name = "LblHighVoltage"
        '
        'LblEcoolerState
        '
        Me.LblEcoolerState.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblEcoolerState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblEcoolerState, "LblEcoolerState")
        Me.LblEcoolerState.Name = "LblEcoolerState"
        '
        'BtnEcoolerCtl
        '
        Me.BtnEcoolerCtl.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnEcoolerCtl, "BtnEcoolerCtl")
        Me.BtnEcoolerCtl.Name = "BtnEcoolerCtl"
        Me.BtnEcoolerCtl.UseVisualStyleBackColor = False
        '
        'TimerForEcoolerButton
        '
        Me.TimerForEcoolerButton.Interval = 6000
        '
        'frmControlMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.LblEcoolerState)
        Me.Controls.Add(Me.BtnEcoolerCtl)
        Me.Controls.Add(Me.LblHighVoltage)
        Me.Controls.Add(Me.BtnHV)
        Me.Controls.Add(Me.LblAlarmIndication)
        Me.Controls.Add(Me.BtnAlarm)
        Me.Controls.Add(Me.LblFilterstep)
        Me.Controls.Add(Me.LblN2Fill)
        Me.Controls.Add(Me.LblErrorIndication)
        Me.Controls.Add(Me.LblPump)
        Me.Controls.Add(Me.LblFilterBypass)
        Me.Controls.Add(Me.BtnAirFlow)
        Me.Controls.Add(Me.BtnN2Fill)
        Me.Controls.Add(Me.BtnErrorIndication)
        Me.Controls.Add(Me.BtnPump)
        Me.Controls.Add(Me.BtnFilterBypass)
        Me.Controls.Add(Me.BtnFilterstep)
        Me.Controls.Add(Me.Button7)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmControlMenu"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents BtnFilterstep As System.Windows.Forms.Button
    Friend WithEvents BtnFilterBypass As System.Windows.Forms.Button
    Friend WithEvents BtnPump As System.Windows.Forms.Button
    Friend WithEvents BtnErrorIndication As System.Windows.Forms.Button
    Friend WithEvents BtnN2Fill As System.Windows.Forms.Button
    Friend WithEvents BtnAirFlow As System.Windows.Forms.Button
    Friend WithEvents LblFilterBypass As System.Windows.Forms.Label
    Friend WithEvents LblPump As System.Windows.Forms.Label
    Friend WithEvents LblErrorIndication As System.Windows.Forms.Label
    Friend WithEvents LblN2Fill As System.Windows.Forms.Label
    Friend WithEvents LblFilterstep As System.Windows.Forms.Label
    Friend WithEvents LblAlarmIndication As System.Windows.Forms.Label
    Friend WithEvents BtnAlarm As System.Windows.Forms.Button
    Friend WithEvents BtnHV As System.Windows.Forms.Button
    Friend WithEvents LblHighVoltage As System.Windows.Forms.Label
    Friend WithEvents LblEcoolerState As System.Windows.Forms.Label
    Friend WithEvents BtnEcoolerCtl As System.Windows.Forms.Button
    Friend WithEvents TimerForEcoolerButton As System.Windows.Forms.Timer
End Class
