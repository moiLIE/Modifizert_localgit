Public Class frmAbout

    Private Sub frmAbout_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Dim ApplicationTitle As String

            If My.Application.Info.Title <> "" Then
                ApplicationTitle = My.Application.Info.Title
            Else
                ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
            End If
            Me.Text = String.Format(ml_string(339, "About {0}"), ApplicationTitle)
            ' Initialisieren Sie den gesamten Text, der im Infofeld angezeigt wird.
            ' TODO: Passen Sie die Assemblyinformationen der Anwendung im Bereich "Anwendung" des Dialogfelds für die 
            '    Projekteigenschaften (im Menü "Projekt") an.
            Me.LabelProductName.Text = My.Application.Info.ProductName
            Me.LabelVersion.Text = String.Format("Version {0}.{1}.{2} ({3})", My.Application.Info.Version.Major, My.Application.Info.Version.Minor,
                                                 My.Application.Info.Version.Build, My.Application.Info.Version.Revision)
            Me.LabelCopyright.Text = My.Application.Info.Copyright
            Me.LabelCompanyName.Text = My.Application.Info.CompanyName
            Me.TextBoxDescription.Text = My.Application.Info.Description
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub New()
        Try
            ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
            InitializeComponent()
            ml_UpdateControls()

            ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub


  Private Sub frmAbout_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
    resources.ApplyResources(Me.Button1, "Button1")
    resources.ApplyResources(Me, "$this")
  End Sub
End Class