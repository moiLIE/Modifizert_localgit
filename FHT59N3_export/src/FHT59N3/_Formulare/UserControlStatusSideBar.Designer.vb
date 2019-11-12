<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlStatusSideBar
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UserControlStatusSideBar))
        Me.BtnQuitAlarm = New System.Windows.Forms.Button()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LbLN2 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.LblAirFlowActual = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblFilterSteps = New System.Windows.Forms.Label()
        Me.LblCursorIndex = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LblAirFlowMean = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LblTemp = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LblDetectTemp = New System.Windows.Forms.Label()
        Me.LabelClock = New System.Windows.Forms.Label()
        Me.BtnROI = New System.Windows.Forms.Button()
        Me.PanelAlarm = New System.Windows.Forms.Panel()
        Me.LabelAlarmNuclideTitle = New System.Windows.Forms.Label()
        Me.TextAlarmNuclides = New System.Windows.Forms.RichTextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.PanelAlarm.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnQuitAlarm
        '
        Me.BtnQuitAlarm.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnQuitAlarm, "BtnQuitAlarm")
        Me.BtnQuitAlarm.Name = "BtnQuitAlarm"
        Me.BtnQuitAlarm.UseVisualStyleBackColor = False
        '
        'LblStatus
        '
        Me.LblStatus.BackColor = System.Drawing.Color.WhiteSmoke
        Me.LblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LblStatus, "LblStatus")
        Me.LblStatus.Name = "LblStatus"
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.LbLN2, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.LblAirFlowActual, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label9, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label20, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LblFilterSteps, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LblCursorIndex, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LblAirFlowMean, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.LblTemp, 1, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.LblDetectTemp, 1, 7)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'LbLN2
        '
        resources.ApplyResources(Me.LbLN2, "LbLN2")
        Me.LbLN2.ForeColor = System.Drawing.Color.White
        Me.LbLN2.Name = "LbLN2"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Name = "Label6"
        '
        'LblAirFlowActual
        '
        resources.ApplyResources(Me.LblAirFlowActual, "LblAirFlowActual")
        Me.LblAirFlowActual.ForeColor = System.Drawing.Color.White
        Me.LblAirFlowActual.Name = "LblAirFlowActual"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.ForeColor = System.Drawing.Color.White
        Me.Label9.Name = "Label9"
        '
        'Label20
        '
        resources.ApplyResources(Me.Label20, "Label20")
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label20, 2)
        Me.Label20.ForeColor = System.Drawing.Color.White
        Me.Label20.Name = "Label20"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Name = "Label4"
        '
        'LblFilterSteps
        '
        resources.ApplyResources(Me.LblFilterSteps, "LblFilterSteps")
        Me.LblFilterSteps.ForeColor = System.Drawing.Color.White
        Me.LblFilterSteps.Name = "LblFilterSteps"
        '
        'LblCursorIndex
        '
        resources.ApplyResources(Me.LblCursorIndex, "LblCursorIndex")
        Me.LblCursorIndex.Name = "LblCursorIndex"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Name = "Label1"
        '
        'LblAirFlowMean
        '
        resources.ApplyResources(Me.LblAirFlowMean, "LblAirFlowMean")
        Me.LblAirFlowMean.ForeColor = System.Drawing.Color.White
        Me.LblAirFlowMean.Name = "LblAirFlowMean"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'LblTemp
        '
        resources.ApplyResources(Me.LblTemp, "LblTemp")
        Me.LblTemp.ForeColor = System.Drawing.Color.White
        Me.LblTemp.Name = "LblTemp"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Name = "Label5"
        '
        'LblDetectTemp
        '
        resources.ApplyResources(Me.LblDetectTemp, "LblDetectTemp")
        Me.LblDetectTemp.ForeColor = System.Drawing.Color.White
        Me.LblDetectTemp.Name = "LblDetectTemp"
        '
        'LabelClock
        '
        Me.LabelClock.BackColor = System.Drawing.Color.WhiteSmoke
        resources.ApplyResources(Me.LabelClock, "LabelClock")
        Me.LabelClock.ForeColor = System.Drawing.Color.Black
        Me.LabelClock.Name = "LabelClock"
        '
        'BtnROI
        '
        Me.BtnROI.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnROI, "BtnROI")
        Me.BtnROI.Name = "BtnROI"
        Me.BtnROI.UseVisualStyleBackColor = False
        '
        'PanelAlarm
        '
        Me.PanelAlarm.Controls.Add(Me.TextAlarmNuclides)
        Me.PanelAlarm.Controls.Add(Me.LabelAlarmNuclideTitle)
        resources.ApplyResources(Me.PanelAlarm, "PanelAlarm")
        Me.PanelAlarm.Name = "PanelAlarm"
        '
        'LabelAlarmNuclideTitle
        '
        Me.LabelAlarmNuclideTitle.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.LabelAlarmNuclideTitle, "LabelAlarmNuclideTitle")
        Me.LabelAlarmNuclideTitle.ForeColor = System.Drawing.Color.White
        Me.LabelAlarmNuclideTitle.Name = "LabelAlarmNuclideTitle"
        '
        'TextAlarmNuclides
        '
        resources.ApplyResources(Me.TextAlarmNuclides, "TextAlarmNuclides")
        Me.TextAlarmNuclides.BackColor = System.Drawing.SystemColors.Control
        Me.TextAlarmNuclides.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextAlarmNuclides.ForeColor = System.Drawing.Color.LightYellow
        Me.TextAlarmNuclides.Name = "TextAlarmNuclides"
        Me.TextAlarmNuclides.Text = Global.FHT59N3.MultiLang._504
        '
        'UserControlStatusSideBar
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PanelAlarm)
        Me.Controls.Add(Me.BtnQuitAlarm)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.LabelClock)
        Me.Controls.Add(Me.BtnROI)
        Me.Name = "UserControlStatusSideBar"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.PanelAlarm.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnQuitAlarm As System.Windows.Forms.Button
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LbLN2 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents LblAirFlowActual As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LblFilterSteps As System.Windows.Forms.Label
    Friend WithEvents LblCursorIndex As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LblAirFlowMean As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents LblTemp As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LblDetectTemp As System.Windows.Forms.Label
    Friend WithEvents LabelClock As System.Windows.Forms.Label
    Friend WithEvents BtnROI As System.Windows.Forms.Button
    Friend WithEvents PanelAlarm As System.Windows.Forms.Panel
    Friend WithEvents LabelAlarmNuclideTitle As System.Windows.Forms.Label
    Friend WithEvents TextAlarmNuclides As System.Windows.Forms.RichTextBox

End Class
