Imports System.Drawing

Public Class frmStates

    Private ScrollIndex As Integer = 0
    Private isListItemChanged As Integer = 0

    Private Sub frmStates_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ImageList1.Images.Add("GreenDot", CType(_MyResouceManager.GetObject("GreenDot"), System.Drawing.Image))
        ImageList1.Images.Add("YellowDot", CType(_MyResouceManager.GetObject("YellowDot"), System.Drawing.Image))
        ImageList1.Images.Add("RedDot", CType(_MyResouceManager.GetObject("RedDot"), System.Drawing.Image))
    End Sub

    Public Sub FillFormStates()
        Try
            If _MonitorStatesClicked Then
                Label4.Visible = True
                Label5.Visible = True
                Label6.Visible = True
                Label7.Visible = True
                Label8.Visible = True
                Label9.Visible = True
                Label10.Visible = True
                LblFilterstep.Visible = True
                LblFilterBypass.Visible = True
                LblPump.Visible = True
                LblErrorIndication.Visible = True
                LblAlarmIndication.Visible = True
                LblN2Fill.Visible = True
                LblHighVoltage.Visible = True
                StateList.Width = 432
            Else
                Label4.Visible = False
                Label5.Visible = False
                Label6.Visible = False
                Label7.Visible = False
                Label8.Visible = False
                Label9.Visible = False
                Label10.Visible = False
                LblFilterstep.Visible = False
                LblFilterBypass.Visible = False
                LblPump.Visible = False
                LblErrorIndication.Visible = False
                LblAlarmIndication.Visible = False
                LblN2Fill.Visible = False
                LblHighVoltage.Visible = False
                StateList.Width = 721
            End If
            FillStatesForm()
            FillMsgForm()
            TimerStart.Enabled = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub FillStatesForm()
        Try
            Dim Lvi As New ListViewItem
            'Irgendeinstatus hat sich geändert, Liste neu aufbauen
            StateList.Items.Clear()
            StateList.Columns.Clear()
            StateList.View = View.Details
            StateList.Columns.Add("C1", 100, HorizontalAlignment.Left)
            If _MonitorStatesClicked Then
                StateList.Columns.Add("C2", CType(Math.Abs(StateList.Width), Integer) - 10 - 95, HorizontalAlignment.Left)
            Else
                StateList.Columns.Add("C2", CType(Math.Abs(StateList.Width), Integer) - 10 - 100, HorizontalAlignment.Left)
            End If
            StateList.GridLines = True
            StateList.GridLineColor = Color.Black

            Lvi = New ListViewItem("1")
            Lvi.SubItems.Add(ml_string(65, "Maintenance"))
            If _MyControlCenter.SYS_States.Maintenance Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("2")
            Lvi.SubItems.Add(ml_string(324, "K40 too low or not found"))
            If _MyControlCenter.SYS_States.K40ToLow_NotFound Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("4")
            Lvi.SubItems.Add(String.Format(ml_string(325, "Air flow less than {0} m³/h"), _MyFHT59N3Par.MinAirFlowAlert))
            If _MyControlCenter.SYS_States.AirFlowLessThen1Cubic Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("8")
            Lvi.SubItems.Add(ml_string(326, "HV Off"))
            If _MyControlCenter.SYS_States.HVOff Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("16")
            Lvi.SubItems.Add(ml_string(327, "No filterstep investigated"))
            If _MyControlCenter.SYS_States.NoFilterstep Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("32")
            Lvi.SubItems.Add(ml_string(328, "Bypass open"))
            If _MyControlCenter.SYS_States.BypassOpen Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("64")
            Lvi.SubItems.Add(ml_string(329, "Analyzation routine cancelled"))
            If _MyControlCenter.SYS_States.AnalyzationCancelled Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)
            Lvi = New ListViewItem("128")
            Lvi.SubItems.Add(String.Format(ml_string(335, "Air flow greater than {0} m³/h"), _MyFHT59N3Par.MaxAirFlowAlert))
            If _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("256")
            If _MyFHT59N3Par.EcoolerEnabled Then
                If _MyControlCenter.SYS_States.EcoolerOff Then
                    Lvi.SubItems.Add(ml_string(511, "The E-Cooler is deactivated"))
                    Lvi.BackColor = Color.Red
                    Lvi.SubItems(1).BackColor = Color.Red
                Else
                    Lvi.SubItems.Add(ml_string(535, "The E-Cooler is cooling"))
                    Lvi.BackColor = StateList.BackColor
                    Lvi.SubItems(1).BackColor = StateList.BackColor
                End If
            Else
                Lvi.SubItems.Add(ml_string(536, "An E-Cooler is not present"))
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
                Lvi.SubItems(1).ForeColor = SystemColors.GrayText
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("512")
            Lvi.SubItems.Add(ml_string(337, "Spectrum dead time is bigger than 20 percent"))
            If _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("1024")
            If _MyFHT59N3Par.EnableCapturingDetectorTemperature Then
                If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                    Lvi.SubItems.Add(ml_string(523, "Recording detector temperature is defect"))
                Else
                    Lvi.SubItems.Add(ml_string(522, "Recording detector temperature is running"))
                End If
            Else
                Lvi.SubItems.Add(ml_string(331, "N2 Filling is going low"))
            End If
            If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("2048")
            Lvi.SubItems.Add(ml_string(332, "Filter has to be changed"))
            If _MyControlCenter.SYS_States.FilterHasToBeChanged Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("4096")
            Lvi.SubItems.Add(ml_string(333, "Check Temperature and Pressure"))
            If _MyControlCenter.SYS_States.CheckTempPressure Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("8196")
            Lvi.SubItems.Add(ml_string(334, "K40 too big"))
            If _MyControlCenter.SYS_States.K40ToBig Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("16384")
            Lvi.SubItems.Add(ml_string(336, "Data transfer error"))
            If _MyControlCenter.SYS_States.DataTransferError Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            Lvi = New ListViewItem("32768")
            Lvi.SubItems.Add(ml_string(591, "UPS operation (on battery)"))
            If _MyControlCenter.SYS_States.UpsOnBattery Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            Else
                Lvi.BackColor = StateList.BackColor
                Lvi.SubItems(1).BackColor = StateList.BackColor
            End If
            StateList.Items.Add(Lvi)

            'Leerzeile
            Lvi = New ListViewItem("")
            Lvi.SubItems.Add("")
            StateList.Items.Add(Lvi)

            'Summenzeile
            Lvi = New ListViewItem(_MyControlCenter.SYS_States.SumState.ToString)
            Lvi.SubItems.Add(ml_string(338, "Sum State"))
            If _MyControlCenter.SYS_States.SumStateLevel = FHT59N3Core.FHT59N3_SystemStates.StateLevel.OK Then
                Lvi.BackColor = Color.Green
                Lvi.SubItems(1).BackColor = Color.Green
            ElseIf _MyControlCenter.SYS_States.SumStateLevel = FHT59N3Core.FHT59N3_SystemStates.StateLevel.WARNING Then
                Lvi.BackColor = Color.Yellow
                Lvi.SubItems(1).BackColor = Color.Yellow
            ElseIf _MyControlCenter.SYS_States.SumStateLevel = FHT59N3Core.FHT59N3_SystemStates.StateLevel.FATAL Then
                Lvi.BackColor = Color.Red
                Lvi.SubItems(1).BackColor = Color.Red
            End If
            StateList.Items.Add(Lvi)

            TimerCheckChanges.Enabled = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub FillMsgForm()
        Try
            Dim Msg As String, sDot As String, Jetzt As Date

            isListItemChanged = 1

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
                ListView1.Items.Insert(0, MsgListDate(i).ToShortDateString & " " & Format$(MsgListDate(i), "HH:mm:ss") & " : " & Msg, sDot)
            Next
            If Msg = "" Then
                Msg = ml_string(309, "No message available")
                Jetzt = Now
                ListView1.Items.Add(Jetzt.ToShortDateString & " " & Jetzt.ToShortTimeString & " : " & Msg, sDot)
            End If
            SelectItem(0)
            isListItemChanged = 0
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub UpdateStatesForm()
        Try
            StateList.BeginUpdate()

            '1
            If _MyControlCenter.SYS_States.Maintenance Then
                StateList.Items(0).SubItems(0).BackColor = Color.Red
                StateList.Items(0).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(0).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(0).SubItems(1).BackColor = StateList.BackColor
            End If
            '2
            If _MyControlCenter.SYS_States.K40ToLow_NotFound Then
                StateList.Items(1).SubItems(0).BackColor = Color.Red
                StateList.Items(1).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(1).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(1).SubItems(1).BackColor = StateList.BackColor
            End If
            '4
            If _MyControlCenter.SYS_States.AirFlowLessThen1Cubic Then
                StateList.Items(2).SubItems(0).BackColor = Color.Red
                StateList.Items(2).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(2).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(2).SubItems(1).BackColor = StateList.BackColor
            End If
            '8
            If _MyControlCenter.SYS_States.HVOff Then
                StateList.Items(3).SubItems(0).BackColor = Color.Red
                StateList.Items(3).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(3).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(3).SubItems(1).BackColor = StateList.BackColor
            End If
            '16
            If _MyControlCenter.SYS_States.NoFilterstep Then
                StateList.Items(4).SubItems(0).BackColor = Color.Red
                StateList.Items(4).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(4).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(4).SubItems(1).BackColor = StateList.BackColor
            End If
            '32
            If _MyControlCenter.SYS_States.BypassOpen Then
                StateList.Items(5).SubItems(0).BackColor = Color.Red
                StateList.Items(5).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(5).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(5).SubItems(1).BackColor = StateList.BackColor
            End If
            '64
            If _MyControlCenter.SYS_States.AnalyzationCancelled Then
                StateList.Items(6).SubItems(0).BackColor = Color.Red
                StateList.Items(6).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(6).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(6).SubItems(1).BackColor = StateList.BackColor
            End If
            '128
            If _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic Then
                StateList.Items(7).SubItems(0).BackColor = Color.Red
                StateList.Items(7).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(7).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(7).SubItems(1).BackColor = StateList.BackColor
            End If

            ' starting with bit 0x100 (256)
            If _MyFHT59N3Par.EcoolerEnabled Then
                If _MyControlCenter.SYS_States.EcoolerOff Then
                    StateList.Items(8).SubItems(0).BackColor = Color.Red
                    StateList.Items(8).SubItems(1).BackColor = Color.Red
                    StateList.Items(8).SubItems(1).Text = ml_string(511, "The E-Cooler is deactivated")
                Else
                    StateList.Items(8).SubItems(0).BackColor = StateList.BackColor
                    StateList.Items(8).SubItems(1).BackColor = StateList.BackColor
                    StateList.Items(8).SubItems(1).Text = ml_string(535, "The E-Cooler is cooling")
                End If
            Else
                StateList.Items(8).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(8).SubItems(1).BackColor = StateList.BackColor
                StateList.Items(8).SubItems(1).Text = ml_string(536, "An E-Cooler is not present")
            End If

            '512
            If _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent Then
                StateList.Items(9).SubItems(0).BackColor = Color.Red
                StateList.Items(9).SubItems(1).BackColor = Color.Red
            Else
                StateList.Items(9).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(9).SubItems(1).BackColor = StateList.BackColor
            End If

            '1024 - Warning levels
            If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                StateList.Items(10).SubItems(0).BackColor = Color.Yellow
                StateList.Items(10).SubItems(1).BackColor = Color.Yellow
                If _MyFHT59N3Par.EnableCapturingDetectorTemperature Then
                    StateList.Items(10).SubItems(1).Text = ml_string(523, "Recording detector temperature is defect")
                End If
            Else
                StateList.Items(10).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(10).SubItems(1).BackColor = StateList.BackColor
                If _MyFHT59N3Par.EnableCapturingDetectorTemperature Then
                    StateList.Items(10).SubItems(1).Text = ml_string(522, "Recording detector temperature is running")
                End If
            End If
            '2048
            If _MyControlCenter.SYS_States.FilterHasToBeChanged Then
                StateList.Items(11).SubItems(0).BackColor = Color.Yellow
                StateList.Items(11).SubItems(1).BackColor = Color.Yellow
            Else
                StateList.Items(11).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(11).SubItems(1).BackColor = StateList.BackColor
            End If
            '4096
            If _MyControlCenter.SYS_States.CheckTempPressure Then
                StateList.Items(12).SubItems(0).BackColor = Color.Yellow
                StateList.Items(12).SubItems(1).BackColor = Color.Yellow
            Else
                StateList.Items(12).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(12).SubItems(1).BackColor = StateList.BackColor
            End If
            '8196
            If _MyControlCenter.SYS_States.K40ToBig Then
                StateList.Items(13).SubItems(0).BackColor = Color.Yellow
                StateList.Items(13).SubItems(1).BackColor = Color.Yellow
            Else
                StateList.Items(13).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(13).SubItems(1).BackColor = StateList.BackColor
            End If
            '16384
            If _MyControlCenter.SYS_States.DataTransferError Then
                StateList.Items(14).SubItems(0).BackColor = Color.Yellow
                StateList.Items(14).SubItems(1).BackColor = Color.Yellow
            Else
                StateList.Items(14).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(14).SubItems(1).BackColor = StateList.BackColor
            End If
            '32768
            If _MyControlCenter.SYS_States.UpsOnBattery Then
                StateList.Items(15).SubItems(0).BackColor = Color.Yellow
                StateList.Items(15).SubItems(1).BackColor = Color.Yellow
            Else
                StateList.Items(15).SubItems(0).BackColor = StateList.BackColor
                StateList.Items(15).SubItems(1).BackColor = StateList.BackColor
            End If

            'Summenzeile (bei Update)
            StateList.Items(17).SubItems(0).Text = _MyControlCenter.SYS_States.SumState.ToString
            If _MyControlCenter.SYS_States.SumStateLevel = FHT59N3Core.FHT59N3_SystemStates.StateLevel.OK Then
                StateList.Items(17).SubItems(0).BackColor = Color.Green
                StateList.Items(17).SubItems(1).BackColor = Color.Green
            ElseIf _MyControlCenter.SYS_States.SumStateLevel = FHT59N3Core.FHT59N3_SystemStates.StateLevel.WARNING Then
                StateList.Items(17).SubItems(0).BackColor = Color.Yellow
                StateList.Items(17).SubItems(1).BackColor = Color.Yellow
            ElseIf _MyControlCenter.SYS_States.SumStateLevel = FHT59N3Core.FHT59N3_SystemStates.StateLevel.FATAL Then
                StateList.Items(17).SubItems(0).BackColor = Color.Red
                StateList.Items(17).SubItems(1).BackColor = Color.Red
            End If

            StateList.EndUpdate()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub UpdateMsgForm()
        Try
            If isMsgListChanged Then
                isMsgListChanged = False
                FillMsgForm()
                isListItemChanged = 1
                SelectItem(0)
                isListItemChanged = 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Try
            TimerCheckChanges.Enabled = False
            _MonitorStatesClicked = False
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerCheckChanges_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerCheckChanges.Tick
        Try
            UpdateStatesForm()
            UpdateMsgForm()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerStart_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerStart.Tick
        Try
            'Timer verwenden, damit der letzte Eintrag auch ganz unten gelistet wird
            TimerStart.Enabled = False
            isListItemChanged = 1
            SelectItem(0)
            isListItemChanged = 0
            TimerCheckChanges.Enabled = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnUp.Click
        Try
            isListItemChanged = 1
            SelectItem(ScrollIndex - 1)
            isListItemChanged = 0
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDown.Click
        Try
            isListItemChanged = 1
            SelectItem(ScrollIndex + 1)
            isListItemChanged = 0
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub ListView1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.Click
        Try
            If isListItemChanged = 0 Then
                SelectItem(ListView1.SelectedIndices.Item(0))  'ListView1.SelectedIndices.Count 'CType(ListView1.FocusedItem, Integer)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub SelectItem(ByVal Index As Integer)
        Try
            ListView1.Items(ScrollIndex).Selected = False
            ListView1.Items(ScrollIndex).BackColor = ListView1.BackColor
            ScrollIndex = Index  'ListView1.SelectedIndices.Count 'CType(ListView1.FocusedItem, Integer)
            If ScrollIndex > ListView1.Items.Count - 1 Then ScrollIndex = ListView1.Items.Count - 1
            If ScrollIndex < 0 Then ScrollIndex = 0
            ListView1.Items(ScrollIndex).Selected = True
            ListView1.Items(ScrollIndex).BackColor = MYCOL_ENTRYFOCUS
            ListView1.Items(ScrollIndex).EnsureVisible()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmStates_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmStates))
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        resources.ApplyResources(Me, "$this")
        resources.ApplyResources(Me.Label1, "Label1")
        resources.ApplyResources(Me.Label10, "Label10")
        resources.ApplyResources(Me.Label2, "Label2")
        resources.ApplyResources(Me.Label3, "Label3")
        resources.ApplyResources(Me.Label4, "Label4")
        resources.ApplyResources(Me.Label5, "Label5")
        resources.ApplyResources(Me.Label6, "Label6")
        resources.ApplyResources(Me.Label7, "Label7")
        resources.ApplyResources(Me.Label8, "Label8")
        resources.ApplyResources(Me.Label9, "Label9")
        resources.ApplyResources(Me.LblAlarmIndication, "LblAlarmIndication")
        resources.ApplyResources(Me.LblErrorIndication, "LblErrorIndication")
        resources.ApplyResources(Me.LblFilterBypass, "LblFilterBypass")
        resources.ApplyResources(Me.LblFilterstep, "LblFilterstep")
        resources.ApplyResources(Me.LblHighVoltage, "LblHighVoltage")
        resources.ApplyResources(Me.LblN2Fill, "LblN2Fill")
        resources.ApplyResources(Me.LblPump, "LblPump")
    End Sub
End Class