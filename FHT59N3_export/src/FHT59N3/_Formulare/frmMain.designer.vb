<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tmrTrigger = New System.Windows.Forms.Timer(Me.components)
        Me.PanelBottom = New System.Windows.Forms.Panel()
        Me.BtnDetectorTemperature = New System.Windows.Forms.Button()
        Me.BtnSpectrum = New System.Windows.Forms.Button()
        Me.BtnResult = New System.Windows.Forms.Button()
        Me.BtnExpandMode = New System.Windows.Forms.Button()
        Me.BtnAbout = New System.Windows.Forms.Button()
        Me.BtnFile = New System.Windows.Forms.Button()
        Me.BtnMaintenance = New System.Windows.Forms.Button()
        Me.BtnOperation = New System.Windows.Forms.Button()
        Me.tmrSignals = New System.Windows.Forms.Timer(Me.components)
        Me.PanelTop = New System.Windows.Forms.Panel()
        Me.LabelTitle = New System.Windows.Forms.Label()
        Me.PanelRight = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.UcStatusSideBar = New FHT59N3.UserControlStatusSideBar()
        Me.tmrStart = New System.Windows.Forms.Timer(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.tmrTemperatureRecording = New System.Windows.Forms.Timer(Me.components)
        Me.PanelBottom.SuspendLayout()
        Me.PanelTop.SuspendLayout()
        Me.PanelRight.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrTrigger
        '
        Me.tmrTrigger.Interval = 300
        '
        'PanelBottom
        '
        Me.PanelBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.PanelBottom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PanelBottom.Controls.Add(Me.BtnDetectorTemperature)
        Me.PanelBottom.Controls.Add(Me.BtnSpectrum)
        Me.PanelBottom.Controls.Add(Me.BtnResult)
        Me.PanelBottom.Controls.Add(Me.BtnExpandMode)
        Me.PanelBottom.Controls.Add(Me.BtnAbout)
        Me.PanelBottom.Controls.Add(Me.BtnFile)
        Me.PanelBottom.Controls.Add(Me.BtnMaintenance)
        Me.PanelBottom.Controls.Add(Me.BtnOperation)
        resources.ApplyResources(Me.PanelBottom, "PanelBottom")
        Me.PanelBottom.Name = "PanelBottom"
        '
        'BtnDetectorTemperature
        '
        Me.BtnDetectorTemperature.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnDetectorTemperature, "BtnDetectorTemperature")
        Me.BtnDetectorTemperature.Name = "BtnDetectorTemperature"
        Me.BtnDetectorTemperature.UseVisualStyleBackColor = False
        '
        'BtnSpectrum
        '
        Me.BtnSpectrum.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnSpectrum, "BtnSpectrum")
        Me.BtnSpectrum.Name = "BtnSpectrum"
        Me.BtnSpectrum.UseVisualStyleBackColor = False
        '
        'BtnResult
        '
        Me.BtnResult.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnResult, "BtnResult")
        Me.BtnResult.Name = "BtnResult"
        Me.BtnResult.UseVisualStyleBackColor = False
        '
        'BtnExpandMode
        '
        Me.BtnExpandMode.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnExpandMode, "BtnExpandMode")
        Me.BtnExpandMode.Image = Global.FHT59N3.My.Resources.Resources.Zoom
        Me.BtnExpandMode.Name = "BtnExpandMode"
        Me.BtnExpandMode.UseVisualStyleBackColor = False
        '
        'BtnAbout
        '
        Me.BtnAbout.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnAbout, "BtnAbout")
        Me.BtnAbout.Name = "BtnAbout"
        Me.BtnAbout.UseVisualStyleBackColor = False
        '
        'BtnFile
        '
        Me.BtnFile.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnFile, "BtnFile")
        Me.BtnFile.Name = "BtnFile"
        Me.BtnFile.UseVisualStyleBackColor = False
        '
        'BtnMaintenance
        '
        Me.BtnMaintenance.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnMaintenance, "BtnMaintenance")
        Me.BtnMaintenance.Name = "BtnMaintenance"
        Me.BtnMaintenance.UseVisualStyleBackColor = False
        '
        'BtnOperation
        '
        Me.BtnOperation.BackColor = System.Drawing.SystemColors.ButtonFace
        resources.ApplyResources(Me.BtnOperation, "BtnOperation")
        Me.BtnOperation.Name = "BtnOperation"
        Me.BtnOperation.UseVisualStyleBackColor = False
        '
        'tmrSignals
        '
        Me.tmrSignals.Interval = 500
        '
        'PanelTop
        '
        Me.PanelTop.BackColor = System.Drawing.Color.Black
        resources.ApplyResources(Me.PanelTop, "PanelTop")
        Me.PanelTop.Controls.Add(Me.LabelTitle)
        Me.PanelTop.Name = "PanelTop"
        '
        'LabelTitle
        '
        Me.LabelTitle.BackColor = System.Drawing.Color.Black
        Me.LabelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LabelTitle, "LabelTitle")
        Me.LabelTitle.ForeColor = System.Drawing.Color.White
        Me.LabelTitle.Name = "LabelTitle"
        '
        'PanelRight
        '
        Me.PanelRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelRight.Controls.Add(Me.Panel1)
        resources.ApplyResources(Me.PanelRight, "PanelRight")
        Me.PanelRight.Name = "PanelRight"
        '
        'Panel1
        '
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Controls.Add(Me.UcStatusSideBar)
        Me.Panel1.Name = "Panel1"
        '
        'UcStatusSideBar
        '
        resources.ApplyResources(Me.UcStatusSideBar, "UcStatusSideBar")
        Me.UcStatusSideBar.Name = "UcStatusSideBar"
        '
        'tmrStart
        '
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        resources.ApplyResources(Me.ImageList1, "ImageList1")
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'tmrTemperatureRecording
        '
        Me.tmrTemperatureRecording.Interval = 1000
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ControlBox = False
        Me.Controls.Add(Me.PanelRight)
        Me.Controls.Add(Me.PanelTop)
        Me.Controls.Add(Me.PanelBottom)
        Me.ForeColor = System.Drawing.Color.Black
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.IsMdiContainer = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMain"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.PanelBottom.ResumeLayout(False)
        Me.PanelTop.ResumeLayout(False)
        Me.PanelRight.ResumeLayout(False)
        Me.PanelRight.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrTrigger As System.Windows.Forms.Timer

    Friend WithEvents PanelBottom As System.Windows.Forms.Panel
    Friend WithEvents BtnSpectrum As System.Windows.Forms.Button
    Friend WithEvents BtnAbout As System.Windows.Forms.Button
    Friend WithEvents tmrSignals As System.Windows.Forms.Timer
    Friend WithEvents PanelTop As System.Windows.Forms.Panel
    Friend WithEvents LabelTitle As System.Windows.Forms.Label
    Friend WithEvents PanelRight As System.Windows.Forms.Panel
    Friend WithEvents BtnExpandMode As System.Windows.Forms.Button
    Friend WithEvents BtnResult As System.Windows.Forms.Button
    Friend WithEvents tmrStart As System.Windows.Forms.Timer
    Friend WithEvents BtnOperation As System.Windows.Forms.Button
    Friend WithEvents BtnFile As System.Windows.Forms.Button
    Friend WithEvents BtnMaintenance As System.Windows.Forms.Button
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents BtnDetectorTemperature As System.Windows.Forms.Button
    Friend WithEvents tmrTemperatureRecording As System.Windows.Forms.Timer
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents UcStatusSideBar As FHT59N3.UserControlStatusSideBar
End Class
