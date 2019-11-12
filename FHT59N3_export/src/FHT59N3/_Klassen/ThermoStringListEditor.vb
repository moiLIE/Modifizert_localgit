Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Windows.Forms.Design
Imports Thermo.Rmp.CM.Controls

Friend Class ThermoStringListEditor
    Inherits UITypeEditor

    ' Wird gebraucht um das Steuerelement dann azuzeigen.
    Private edSvc As IWindowsFormsEditorService

    ''' <summary>
    ''' Shows the list entries for the editor
    ''' </summary>
    Private WithEvents MyList As New ListBox

    ''' <summary>
    ''' Tracked X Coordinate
    ''' </summary>
    Private _MyListX As Integer

    ''' <summary>
    ''' Tracked Y Coordinate
    ''' </summary>
    Private _MyListY As Integer

    ''' <summary>
    ''' My context menu
    ''' </summary>
    Private ReadOnly MyContextMenu As New ContextMenu


    ''' <summary>
    ''' The edit item click event handler instance
    ''' </summary>
    Private ReadOnly EditItem_Click_EventHandlerInstance As New EventHandler(AddressOf EditItem_Click)
    
    ''' <summary>
    ''' The list values (currently only used for alarm nuclide entries, like Nuclidename=Alarmvalue
    ''' </summary>
    Private ListValue As List(Of String)

    Public Sub New()
        Try
            MyContextMenu.MenuItems.Add(ml_string(594, "Edit Item"), EditItem_Click_EventHandlerInstance)
            MyList.ContextMenu = MyContextMenu

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Overrides Function GetEditStyle( _
                                           ByVal context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Try
            ' Überprüfung auf das vorhandensein und Rückgabe des Stils, was 
            ' angezeigt werden soll. In diesem Fall ein DropDown
            If Not context Is Nothing AndAlso Not context.Instance Is Nothing Then
                Return UITypeEditorEditStyle.DropDown
            End If
            ' Ist nichts vorhanden ,dann machen wir an dieser Stelle nichts.
            Return UITypeEditorEditStyle.None
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Overrides Function EditValue( _
                                        ByVal context As ITypeDescriptorContext, _
                                        ByVal provider As System.IServiceProvider, _
                                        ByVal value As [Object]) As [Object]
        Try

            If (Not (context Is Nothing) And _
                Not (context.Instance Is Nothing) And _
                Not (provider Is Nothing)) Then

                MyList.DataSource = value
                ListValue = CType(value, List(Of String))

                edSvc = CType(provider.GetService( _
                    GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
                If Not (edSvc Is Nothing) Then
                    edSvc.DropDownControl(MyList)
                End If

            End If
            Return ListValue
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return Nothing
        End Try
    End Function

    Private Sub EditItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try

            Dim dlg As frmEditAlarmNuclides = New frmEditAlarmNuclides(ListValue)
            dlg.ShowDialog()

            'Perform update of ListValue entries!
            '...ListValue.Add(MyFormNuclideNameTextBox.Text + "=" + alarmLimit)

            edSvc.CloseDropDown()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

 

    Private Sub MyList_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyList.MouseMove
        Try
            _MyListX = e.X
            _MyListY = e.Y
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub MyList_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MyList.Click
        Try
            Dim Pos As New Point(_MyListX, _MyListY)
            MyContextMenu.Show(MyList, Pos)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


End Class