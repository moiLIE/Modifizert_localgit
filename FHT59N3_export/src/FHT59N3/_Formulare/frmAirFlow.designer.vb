<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAirFlow
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAirFlow))
        Me.BtnClose = New System.Windows.Forms.Button
        Me.BtnApply = New System.Windows.Forms.Button
        Me.BtnStartMeas = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.TBox_NumberValues = New System.Windows.Forms.TextBox
        Me.TBox_TrueAirflow = New System.Windows.Forms.TextBox
        Me.TBox_Result = New System.Windows.Forms.TextBox
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
        'BtnApply
        '
        Me.BtnApply.AccessibleDescription = Nothing
        Me.BtnApply.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnApply, "BtnApply")
        Me.BtnApply.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnApply.BackgroundImage = Nothing
        Me.BtnApply.Name = "BtnApply"
        Me.BtnApply.UseVisualStyleBackColor = False
        '
        'BtnStartMeas
        '
        Me.BtnStartMeas.AccessibleDescription = Nothing
        Me.BtnStartMeas.AccessibleName = Nothing
        resources.ApplyResources(Me.BtnStartMeas, "BtnStartMeas")
        Me.BtnStartMeas.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnStartMeas.BackgroundImage = Nothing
        Me.BtnStartMeas.Name = "BtnStartMeas"
        Me.BtnStartMeas.UseVisualStyleBackColor = False
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
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'TBox_NumberValues
        '
        Me.TBox_NumberValues.AccessibleDescription = Nothing
        Me.TBox_NumberValues.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_NumberValues, "TBox_NumberValues")
        Me.TBox_NumberValues.BackgroundImage = Nothing
        Me.TBox_NumberValues.Name = "TBox_NumberValues"
        '
        'TBox_TrueAirflow
        '
        Me.TBox_TrueAirflow.AccessibleDescription = Nothing
        Me.TBox_TrueAirflow.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_TrueAirflow, "TBox_TrueAirflow")
        Me.TBox_TrueAirflow.BackgroundImage = Nothing
        Me.TBox_TrueAirflow.Name = "TBox_TrueAirflow"
        '
        'TBox_Result
        '
        Me.TBox_Result.AccessibleDescription = Nothing
        Me.TBox_Result.AccessibleName = Nothing
        resources.ApplyResources(Me.TBox_Result, "TBox_Result")
        Me.TBox_Result.BackgroundImage = Nothing
        Me.TBox_Result.Name = "TBox_Result"
        '
        'frmAirFlow
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BackgroundImage = Nothing
        Me.ControlBox = False
        Me.Controls.Add(Me.TBox_Result)
        Me.Controls.Add(Me.TBox_TrueAirflow)
        Me.Controls.Add(Me.TBox_NumberValues)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnStartMeas)
        Me.Controls.Add(Me.BtnApply)
        Me.Controls.Add(Me.BtnClose)
        Me.Font = Nothing
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAirFlow"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents BtnApply As System.Windows.Forms.Button
    Friend WithEvents BtnStartMeas As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TBox_NumberValues As System.Windows.Forms.TextBox
    Friend WithEvents TBox_TrueAirflow As System.Windows.Forms.TextBox
    Friend WithEvents TBox_Result As System.Windows.Forms.TextBox
End Class
