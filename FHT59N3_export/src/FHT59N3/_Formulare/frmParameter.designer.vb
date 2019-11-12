<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmParameter
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmParameter))
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.PropGridParameter = New System.Windows.Forms.PropertyGrid()
        Me.BtnAccept = New System.Windows.Forms.Button()
        Me.BtnShowNuclideLibrary = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'BtnClose
        '
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnClose.ImageKey = Global.FHT59N3.MultiLang._504
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'PropGridParameter
        '
        resources.ApplyResources(Me.PropGridParameter, "PropGridParameter")
        Me.PropGridParameter.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.PropGridParameter.CommandsForeColor = System.Drawing.Color.White
        Me.PropGridParameter.Name = "PropGridParameter"
        Me.PropGridParameter.PropertySort = System.Windows.Forms.PropertySort.Categorized
        Me.PropGridParameter.ToolbarVisible = False
        '
        'BtnAccept
        '
        resources.ApplyResources(Me.BtnAccept, "BtnAccept")
        Me.BtnAccept.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnAccept.ImageKey = Global.FHT59N3.MultiLang._504
        Me.BtnAccept.Name = "BtnAccept"
        Me.BtnAccept.UseVisualStyleBackColor = False
        '
        'BtnShowNuclideLibrary
        '
        resources.ApplyResources(Me.BtnShowNuclideLibrary, "BtnShowNuclideLibrary")
        Me.BtnShowNuclideLibrary.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.BtnShowNuclideLibrary.ImageKey = Global.FHT59N3.MultiLang._504
        Me.BtnShowNuclideLibrary.Name = "BtnShowNuclideLibrary"
        Me.BtnShowNuclideLibrary.UseVisualStyleBackColor = False
        '
        'frmParameter
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnShowNuclideLibrary)
        Me.Controls.Add(Me.BtnAccept)
        Me.Controls.Add(Me.PropGridParameter)
        Me.Controls.Add(Me.BtnClose)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmParameter"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents PropGridParameter As System.Windows.Forms.PropertyGrid
    Friend WithEvents BtnAccept As System.Windows.Forms.Button
    Friend WithEvents BtnShowNuclideLibrary As System.Windows.Forms.Button
End Class
