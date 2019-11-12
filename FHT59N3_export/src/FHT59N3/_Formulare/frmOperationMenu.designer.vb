<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOperationMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOperationMenu))
        Me.BtnReturn = New System.Windows.Forms.Button()
        Me.BtnIntensOn = New System.Windows.Forms.Button()
        Me.BtnIntensOff = New System.Windows.Forms.Button()
        Me.BtnExitProg = New System.Windows.Forms.Button()
        Me.BtnAlarmOutOn = New System.Windows.Forms.Button()
        Me.BtnAlarmOutOff = New System.Windows.Forms.Button()
        Me.flowLayout = New System.Windows.Forms.FlowLayoutPanel()
        Me.panelIntensiveMode = New System.Windows.Forms.Panel()
        Me.panelAlarmMode = New System.Windows.Forms.Panel()
        Me.panelExitAndConfirm = New System.Windows.Forms.Panel()
        Me.flowLayout.SuspendLayout()
        Me.panelIntensiveMode.SuspendLayout()
        Me.panelAlarmMode.SuspendLayout()
        Me.panelExitAndConfirm.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnReturn
        '
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnIntensOn
        '
        Me.BtnIntensOn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnIntensOn, "BtnIntensOn")
        Me.BtnIntensOn.Name = "BtnIntensOn"
        Me.BtnIntensOn.UseVisualStyleBackColor = False
        '
        'BtnIntensOff
        '
        Me.BtnIntensOff.BackColor = System.Drawing.Color.Gray
        resources.ApplyResources(Me.BtnIntensOff, "BtnIntensOff")
        Me.BtnIntensOff.Name = "BtnIntensOff"
        Me.BtnIntensOff.UseVisualStyleBackColor = False
        '
        'BtnExitProg
        '
        Me.BtnExitProg.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnExitProg, "BtnExitProg")
        Me.BtnExitProg.Name = "BtnExitProg"
        Me.BtnExitProg.UseVisualStyleBackColor = False
        '
        'BtnAlarmOutOn
        '
        Me.BtnAlarmOutOn.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAlarmOutOn, "BtnAlarmOutOn")
        Me.BtnAlarmOutOn.Name = "BtnAlarmOutOn"
        Me.BtnAlarmOutOn.UseVisualStyleBackColor = False
        '
        'BtnAlarmOutOff
        '
        Me.BtnAlarmOutOff.BackColor = System.Drawing.Color.Gray
        resources.ApplyResources(Me.BtnAlarmOutOff, "BtnAlarmOutOff")
        Me.BtnAlarmOutOff.Name = "BtnAlarmOutOff"
        Me.BtnAlarmOutOff.UseVisualStyleBackColor = False
        '
        'flowLayout
        '
        resources.ApplyResources(Me.flowLayout, "flowLayout")
        Me.flowLayout.Controls.Add(Me.panelAlarmMode)
        Me.flowLayout.Controls.Add(Me.panelIntensiveMode)
        Me.flowLayout.Controls.Add(Me.panelExitAndConfirm)
        Me.flowLayout.Name = "flowLayout"
        '
        'panelIntensiveMode
        '
        resources.ApplyResources(Me.panelIntensiveMode, "panelIntensiveMode")
        Me.panelIntensiveMode.Controls.Add(Me.BtnIntensOff)
        Me.panelIntensiveMode.Controls.Add(Me.BtnIntensOn)
        Me.panelIntensiveMode.Name = "panelIntensiveMode"
        '
        'panelAlarmMode
        '
        resources.ApplyResources(Me.panelAlarmMode, "panelAlarmMode")
        Me.panelAlarmMode.Controls.Add(Me.BtnAlarmOutOff)
        Me.panelAlarmMode.Controls.Add(Me.BtnAlarmOutOn)
        Me.panelAlarmMode.Name = "panelAlarmMode"
        '
        'panelExitAndConfirm
        '
        Me.panelExitAndConfirm.Controls.Add(Me.BtnReturn)
        Me.panelExitAndConfirm.Controls.Add(Me.BtnExitProg)
        resources.ApplyResources(Me.panelExitAndConfirm, "panelExitAndConfirm")
        Me.panelExitAndConfirm.Name = "panelExitAndConfirm"
        '
        'frmOperationMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.flowLayout)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOperationMenu"
        Me.flowLayout.ResumeLayout(False)
        Me.flowLayout.PerformLayout()
        Me.panelIntensiveMode.ResumeLayout(False)
        Me.panelAlarmMode.ResumeLayout(False)
        Me.panelExitAndConfirm.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnIntensOn As System.Windows.Forms.Button
    Friend WithEvents BtnIntensOff As System.Windows.Forms.Button
    Friend WithEvents BtnExitProg As System.Windows.Forms.Button
    Friend WithEvents BtnAlarmOutOn As System.Windows.Forms.Button
    Friend WithEvents BtnAlarmOutOff As System.Windows.Forms.Button
    Friend WithEvents flowLayout As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents panelIntensiveMode As System.Windows.Forms.Panel
    Friend WithEvents panelAlarmMode As System.Windows.Forms.Panel
    Friend WithEvents panelExitAndConfirm As System.Windows.Forms.Panel
End Class
