Public NotInheritable Class frmSplashScreen

    'TODO: Dieses Formular kann einfach als Begrüßungsbildschirm für die Anwendung festgelegt werden, indem Sie zur Registerkarte "Anwendung"
    '  des Projekt-Designers wechseln (Menü "Projekt", Option "Eigenschaften").


    Private Sub SplashScreen1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Richten Sie den Dialogtext zur Laufzeit gemäß den Assemblyinformationen der Anwendung ein.  

        'TODO: Passen Sie die Assemblyinformationen der Anwendung im Bereich "Anwendung" des Dialogfelds für die 
        '  Projekteigenschaften (im Menü "Projekt") an.

        'Anwendungstitel
        If My.Application.Info.Title <> "" Then
            ApplicationTitle.Text = My.Application.Info.Title
        Else
            'Wenn der Anwendungstitel fehlt, Anwendungsnamen ohne Erweiterung verwenden.
            ApplicationTitle.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If

        'Verwenden Sie zum Formatieren der Versionsinformationen den Text, der zur Entwurfszeit im Versionssteuerelement festgelegt wurde, als
        '  Formatierungszeichenfolge. Dies ermöglicht ggf. eine effektive Lokalisierung.
        '  Build- und Revisionsinformationen können durch Verwendung des folgenden Codes und durch Ändern 
        '  des Entwurfszeittextes des Versionssteuerelements in "Version {0}.{1:00}.{2}.{3}" oder einen ähnlichen Text eingeschlossen werden. Weitere Informationen erhalten Sie unter
        '  String.Format() in der Hilfe.
        '
        'Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)
        'Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor)

        Version.Text = String.Format("Version {0}.{1}.{2} ({3})", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)

        'Copyrightinformationen
        Copyright.Text = My.Application.Info.Copyright
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplashScreen))
    End Sub

  Private Sub frmSplashScreen_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
End Class
