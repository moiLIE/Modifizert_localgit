Public Class frmMessages
    Private ScrollIndex As Integer = 0
    Private isListItemChanged As Integer = 0


    Private Sub FillMsgForm()
        Try
            Dim Msg As String, sDot As String, Jetzt As Date

            isListItemChanged = 1
            ImageList1.Images.Add("GreenDot", CType(_MyResouceManager.GetObject("GreenDot"), System.Drawing.Image))
            ImageList1.Images.Add("YellowDot", CType(_MyResouceManager.GetObject("YellowDot"), System.Drawing.Image))
            ImageList1.Images.Add("RedDot", CType(_MyResouceManager.GetObject("RedDot"), System.Drawing.Image))

            ListView1.Clear()
            ListView1.SmallImageList = ImageList1
            ListView1.View = View.Details
            ListView1.Columns.Add("Messages", ListView1.Width - 21, HorizontalAlignment.Center)
            ListView1.HeaderStyle = ColumnHeaderStyle.None
            ListView1.HideSelection = True
            Msg = ""
            sDot = "GreenDot"
            For i As Integer = 0 To MsgListStatus.Count - 1
                Msg = MsgListStatus(i)
                Select Case MsgListStatusOn(i)
                    Case MessageStates.GREEN
                        sDot = "GreenDot"
                    Case MessageStates.YELLOW
                        sDot = "YellowDot"
                    Case MessageStates.RED
                        sDot = "RedDot"
                End Select
                ListView1.Items.Add(MsgListDate(i).ToShortDateString & " " & Format$(MsgListDate(i), "HH:mm:ss") & " : " & Msg, sDot)
            Next
            If Msg = "" Then
                Msg = ml_string(309, "No message available")
                Jetzt = Now
                ListView1.Items.Add(Jetzt.ToShortDateString & " " & Jetzt.ToShortTimeString & " : " & Msg, sDot)
            End If
            isListItemChanged = 0
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Try
            TimerCheckChanges.Enabled = False
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub frmMessages_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            FillMsgForm()
            TimerStart.Enabled = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnUp.Click
        Try
            isListItemChanged = 1
            ListView1.Items(ScrollIndex).Selected = False
            ListView1.Items(ScrollIndex).BackColor = ListView1.BackColor

            ScrollIndex = ScrollIndex - 1
            If ScrollIndex < 0 Then ScrollIndex = 0
            ListView1.Items(ScrollIndex).Selected = True
            ListView1.Items(ScrollIndex).BackColor = MYCOL_ENTRYFOCUS

            ListView1.Items(ScrollIndex).EnsureVisible()
            isListItemChanged = 0
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDown.Click
        Try
            isListItemChanged = 1
            ListView1.Items(ScrollIndex).Selected = False
            ListView1.Items(ScrollIndex).BackColor = ListView1.BackColor

            ScrollIndex = ScrollIndex + 1
            If ScrollIndex > ListView1.Items.Count - 1 Then ScrollIndex = ListView1.Items.Count - 1
            ListView1.Items(ScrollIndex).Selected = True
            ListView1.Items(ScrollIndex).BackColor = MYCOL_ENTRYFOCUS

            ListView1.Items(ScrollIndex).EnsureVisible()
            isListItemChanged = 0
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub ListView1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.Click
        Try
            If isListItemChanged = 0 Then
                ListView1.Items(ScrollIndex).Selected = False
                ListView1.Items(ScrollIndex).BackColor = ListView1.BackColor

                ScrollIndex = ListView1.SelectedIndices.Item(0)  'ListView1.SelectedIndices.Count 'CType(ListView1.FocusedItem, Integer)
                If ScrollIndex > ListView1.Items.Count - 1 Then ScrollIndex = ListView1.Items.Count - 1
                If ScrollIndex < 0 Then ScrollIndex = 0
                ListView1.Items(ScrollIndex).Selected = True
                ListView1.Items(ScrollIndex).BackColor = MYCOL_ENTRYFOCUS

                ListView1.Items(ScrollIndex).EnsureVisible()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerStart_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerStart.Tick
        'Timer verwenden, damit der letzte Eintrag auch ganz unten gelistet wird
        Try
            TimerStart.Enabled = False
            isListItemChanged = 1
            ScrollIndex = ListView1.Items.Count - 1
            ListView1.Items(ScrollIndex).Selected = True
            ListView1.Items(ScrollIndex).BackColor = MYCOL_ENTRYFOCUS

            ListView1.Items(ScrollIndex).EnsureVisible()
            isListItemChanged = 0
            TimerCheckChanges.Enabled = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerCheckChanges_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerCheckChanges.Tick
        Try
            If isMsgListChanged Then
                isMsgListChanged = False
                FillMsgForm()
                isListItemChanged = 1
                ScrollIndex = ListView1.Items.Count - 1
                ListView1.Items(ScrollIndex).Selected = True
                ListView1.Items(ScrollIndex).BackColor = MYCOL_ENTRYFOCUS
                ListView1.Items(ScrollIndex).EnsureVisible()
                isListItemChanged = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


   Public Sub New
     InitializeComponent()
     ml_UpdateControls()
     AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
   End Sub

   Private Sub frmMessages_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
   RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
   End Sub
   Protected Overridable Sub ml_UpdateControls()
     Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMessages))
     resources.ApplyResources(Me.BtnClose, "BtnClose")
     resources.ApplyResources(Me, "$this")
   End Sub
 End Class