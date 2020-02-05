Public Class UserControlStatusSideBar

    Public Sub New()

        InitializeComponent()

        PanelAlarm.Visible = False
    End Sub


    Private Sub BtnQuitAlarm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnQuitAlarm.Click
        Try
            SYS_SetAlarmModeOff() 'Controls GUI (alarm in frmMain), alarm flag and the SPS

            'könnte entfernt werden da bei nächster Prüfung sowieso neu aufgebaut wird!
            SYS_RemoveAllAlarmNuclidesFromList()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnROI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnROI.Click
        Try
            frmROIMenu.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub AddAlarmNuclideToList(alarmNuclide As String, alarmLevel As Integer, alarmLimit As Double)

        Try
            'BulletIndent set to 10-12...
            PanelAlarm.Visible = True
            TextAlarmNuclides.SelectionBullet = True
            Dim txt As String = String.Format("{0} (> {1:G4} Bq/m³, Alarm{2})", alarmNuclide, alarmLimit, alarmLevel)

            TextAlarmNuclides.AppendText(txt + Environment.NewLine)
            TextAlarmNuclides.SelectionBullet = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try

    End Sub


    Public Sub RemoveAllAlarmNuclidesFromList()
        Try
            TextAlarmNuclides.Clear()
            PanelAlarm.Visible = False

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try

    End Sub

End Class
