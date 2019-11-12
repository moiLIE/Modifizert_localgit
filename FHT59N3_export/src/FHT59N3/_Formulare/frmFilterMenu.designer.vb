<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFilterMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFilterMenu))
        Me.BtnSetFilterChanged = New System.Windows.Forms.Button()
        Me.BtnSetFilterstep = New System.Windows.Forms.Button()
        Me.BtnReturn = New System.Windows.Forms.Button()
        Me.BtnSetAvFiltersteps = New System.Windows.Forms.Button()
        Me.CheckBoxWithTimeStampPrint = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'BtnSetFilterChanged
        '
        resources.ApplyResources(Me.BtnSetFilterChanged, "BtnSetFilterChanged")
        Me.BtnSetFilterChanged.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnSetFilterChanged.Name = "BtnSetFilterChanged"
        Me.BtnSetFilterChanged.UseVisualStyleBackColor = False
        '
        'BtnSetFilterstep
        '
        resources.ApplyResources(Me.BtnSetFilterstep, "BtnSetFilterstep")
        Me.BtnSetFilterstep.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnSetFilterstep.Name = "BtnSetFilterstep"
        Me.BtnSetFilterstep.UseVisualStyleBackColor = False
        '
        'BtnReturn
        '
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        Me.BtnReturn.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnReturn.Name = "BtnReturn"
        Me.BtnReturn.UseVisualStyleBackColor = False
        '
        'BtnSetAvFiltersteps
        '
        resources.ApplyResources(Me.BtnSetAvFiltersteps, "BtnSetAvFiltersteps")
        Me.BtnSetAvFiltersteps.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnSetAvFiltersteps.Name = "BtnSetAvFiltersteps"
        Me.BtnSetAvFiltersteps.UseVisualStyleBackColor = False
        '
        'CheckBoxWithTimeStampPrint
        '
        resources.ApplyResources(Me.CheckBoxWithTimeStampPrint, "CheckBoxWithTimeStampPrint")
        Me.CheckBoxWithTimeStampPrint.Name = "CheckBoxWithTimeStampPrint"
        Me.CheckBoxWithTimeStampPrint.UseVisualStyleBackColor = True
        '
        'frmFilterMenu
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.CheckBoxWithTimeStampPrint)
        Me.Controls.Add(Me.BtnSetFilterChanged)
        Me.Controls.Add(Me.BtnSetFilterstep)
        Me.Controls.Add(Me.BtnReturn)
        Me.Controls.Add(Me.BtnSetAvFiltersteps)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFilterMenu"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnSetFilterChanged As System.Windows.Forms.Button
    Friend WithEvents BtnSetFilterstep As System.Windows.Forms.Button
    Friend WithEvents BtnReturn As System.Windows.Forms.Button
    Friend WithEvents BtnSetAvFiltersteps As System.Windows.Forms.Button
    Friend WithEvents CheckBoxWithTimeStampPrint As System.Windows.Forms.CheckBox
End Class
