Imports System.Linq
Imports FHT59N3Core

Public Class frmEditAlarmNuclides

    ''' <summary>
    ''' The encoded list entries containing the nuclide name and the alarm values separated 
    ''' by equal and semicolon
    ''' </summary>
    Private ReadOnly _nuclideAlarmValueList As List(Of String)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="frmEditAlarmNuclides"/> class.
    ''' </summary>
    ''' <param name="ListValue">The list value.</param>
    Sub New(ByRef ListValue As List(Of String))

        ' This call is required by the designer.
        InitializeComponent()

        _nuclideAlarmValueList = ListValue
    End Sub

    ''' <summary>
    ''' Handles the Load event of the frmEditAlarmNuclides control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub frmEditAlarmNuclides_Load(sender As System.Object, e As EventArgs) Handles MyBase.Load

        'Aufpassen falls die gespeicherten Nuklid-Alarm-Paare nicht in der Nuklidliste (mehr) existieren

        'Combobox mit Nukliden vorbefüllen

        Dim nuclideCol As DataGridViewComboBoxColumn = DataGridAlarmNuclides.Columns("ColumnNuclide")

        Dim nuclidesSorted As IList(Of NuclideLibraryParams) = _MyControlCenter.MCA_Nuclides.Nuclides.OrderBy(Function(d) d.Library.Name).
            Select(Function(f) f.Library).ToList()
        Dim nuclideNames As IList(Of String) = nuclidesSorted.Select(Function(d) d.Name).ToList()

        nuclideCol.DataSource = nuclidesSorted
        nuclideCol.DataPropertyName = "NuclidNumber"
        nuclideCol.DisplayMember = "Name"

        For Each nuclideAlarmValue As String In _nuclideAlarmValueList

            Dim entryParts As String() = nuclideAlarmValue.Split(New Char() {"="c, ";"c})
            Dim nuclideName = entryParts(0)
            Dim alarm1 = SYS_SetLocaleDecimalSeparator(entryParts(1))
            Dim alarm2 = If(entryParts.Length > 2, SYS_SetLocaleDecimalSeparator(entryParts(2)), Nothing)

            'Nur übernehmen wenn es den Nuklidnamen noch in der aktuellen Nuklidbibliothek gibt, ansonsten ComboBox-DataError!
            If (nuclideNames.Contains(nuclideName)) Then
                DataGridAlarmNuclides.Rows.Add(nuclideName, alarm1, alarm2)
            End If

        Next


    End Sub


    Private Function SaveChanges() As Boolean

        Dim newAlarmValueList As New List(Of String)
        Try


            Dim listRowCount As Integer = DataGridAlarmNuclides.Rows.Count
            Dim uniqueRowCount As Integer = DataGridAlarmNuclides.Rows.OfType(Of DataGridViewRow)().
                Select(Function(dgRow) dgRow.Cells("ColumnNuclide").Value).Distinct().Count()

            If (uniqueRowCount < listRowCount) Then
                Dim msg = ml_string("653", "There are duplicate entries in the list. This is not allowed. Please edit data.")
                GUI_ShowMessageBox(msg, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                Return False
            End If

            For Each row As DataGridViewRow In DataGridAlarmNuclides.Rows
                Dim nuclideName As String = "", alarm1 As String = "", alarm2 As String = ""
                Dim errorMsg As String = ""

                'TODO: auch prüfen auf doppelte Nuklide!
                Dim validRow = ValidateNuclideEntry(row, nuclideName, alarm1, alarm2, errorMsg)

                If Not validRow Then
                    Dim msg = ml_string("652", "One or more alarm values are not valid. Please check keyed in data.")
                    GUI_ShowMessageBox(msg, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                    Return False
                End If



                newAlarmValueList.Add(nuclideName + "=" + alarm1 +
                      If(String.IsNullOrEmpty(alarm2), "", ";" + alarm2))

            Next

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return False
        End Try

        'we are ByRef, therefore we readd new entries!
        _nuclideAlarmValueList.Clear()
        _nuclideAlarmValueList.AddRange(newAlarmValueList)

        Return True
    End Function

    ''' <summary>
    ''' Validates the nuclide entry.
    ''' </summary>
    ''' <param name="dgRow">The dg row.</param>
    ''' <param name="nuclideName">Name of the nuclide.</param>
    ''' <param name="alarm1">The alarm1.</param>
    ''' <param name="alarm2">The alarm2.</param>
    ''' <returns>true if valid</returns>
    Private Function ValidateNuclideEntry(dgRow As DataGridViewRow, ByRef nuclideName As String,
                                          ByRef alarm1 As String, ByRef alarm2 As String, ByRef errorMessage As String) As Boolean

        errorMessage = String.Empty

        nuclideName = CType(dgRow.Cells("ColumnNuclide").Value, String)
        alarm1 = CType(dgRow.Cells("ColumnAlarm1").Value, String)
        alarm2 = CType(dgRow.Cells("ColumnAlarm2").Value, String)

        If (Not String.IsNullOrEmpty(alarm1)) Then
            alarm1 = alarm1.Replace(",", ".")
        End If
        If (Not String.IsNullOrEmpty(alarm2)) Then
            alarm2 = alarm2.Replace(",", ".")
        End If

        Dim isValid As Boolean


        If ValidateNuclideName(nuclideName, errorMessage) And
            ValidateAlarmValue(alarm1, alarm2, errorMessage) Then
            isValid = True
        Else
            isValid = False
        End If

        Return isValid
    End Function

    ''' <summary>
    ''' Validates the threshold of the nuclide
    ''' </summary>
    ''' <param name="alarm1Value">nuclide threshold as string</param>
    ''' <returns>true if threshold is valid, false if threshold is invalid</returns>
    ''' <remarks></remarks>
    Private Function ValidateAlarmValue(alarm1Value As String, alarm2Value As String,
                                        ByRef errorMessage As String) As Boolean

        Dim hasError As Boolean = False
        Dim alarm1 As Double, alarm2 As Double

        If Not Double.TryParse(alarm1Value, alarm1) Then
            hasError = True
            errorMessage += ml_string(605, "Alarm value 1 is invalid.") + Environment.NewLine

        End If

        If Not String.IsNullOrEmpty(alarm2Value) AndAlso Not Double.TryParse(alarm2Value, alarm2) Then
            hasError = True
            errorMessage += ml_string(608, "Alarm value 2 is invalid") + Environment.NewLine

        End If

        If Not hasError AndAlso Not String.IsNullOrEmpty(alarm2Value) Then
            If (alarm1 > alarm2) Then
                hasError = True
                errorMessage += ml_string(609, "Alarm1 bigger than Alarm2") + Environment.NewLine
            End If
        End If

        Return Not hasError
    End Function

    ''' <summary>
    ''' Validates the name of the nuclide
    ''' </summary>
    ''' <param name="nuclideName">nuclide name as string</param>
    ''' <returns>true if name is valid, false if name is invalid</returns>
    ''' <remarks></remarks>
    Private Function ValidateNuclideName(nuclideName As String, ByRef errorMessage As String) As Boolean

        If IsNothing(nuclideName) OrElse nuclideName.Length = 0 Then
            errorMessage += ml_string(602, "Name must not be empty.") + Environment.NewLine
            Return False
        End If

        If _MyControlCenter.MCA_Nuclides.GetNuclide(nuclideName) Is Nothing Then
            errorMessage += String.Format(ml_string(604, "Name ""{0}"" is invalid."), nuclideName) + Environment.NewLine
            Return False
        Else
            Return True
        End If

    End Function

    

    ''' <summary>
    ''' Handles the Click event of the BtnSave control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub BtnSave_Click(sender As System.Object, e As EventArgs) Handles BtnSave.Click
        If (SaveChanges()) Then
            Close()
        End If
    End Sub

    ''' <summary>
    ''' Handles the Click event of the BtnCancel control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub BtnCancel_Click(sender As System.Object, e As EventArgs) Handles BtnCancel.Click
        Close()
    End Sub

    ''' <summary>
    ''' Handles the Click event of the BtnDelete control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub BtnDelete_Click(sender As System.Object, e As EventArgs) Handles BtnDelete.Click

        If (DataGridAlarmNuclides.SelectedRows.Count > 0) Then
            Dim dgRow = DataGridAlarmNuclides.SelectedRows(0)
            DataGridAlarmNuclides.Rows.Remove(dgRow)
        End If

    End Sub

    ''' <summary>
    ''' Handles the Click event of the BtnAdd control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub BtnAdd_Click(sender As System.Object, e As EventArgs) Handles BtnAdd.Click

        'Check last row whether it is valid before adding another empty row.

        Dim lastRowIdx As Integer = DataGridAlarmNuclides.Rows.GetLastRow(DataGridViewElementStates.None)

        Dim isValid As Boolean = True
        If (lastRowIdx > -1) Then
            Dim lastRow As DataGridViewRow = If(lastRowIdx > -1, DataGridAlarmNuclides.Rows(lastRowIdx), Nothing)
            Dim nn = "", a1 = "", a2 = "", errm = ""
            isValid = ValidateNuclideEntry(lastRow, nn, a1, a2, errm)
        End If

        If (isValid) Then
            Dim rowIdx As Integer = DataGridAlarmNuclides.Rows.Add()
            DataGridAlarmNuclides.ClearSelection()
            DataGridAlarmNuclides.Rows(rowIdx).Selected = True
            DataGridAlarmNuclides.Rows(rowIdx).Visible = True
        End If


    End Sub

    ''' <summary>
    ''' Handles the DataError event of the DataGridAlarmNuclides control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs"/> instance containing the event data.</param>
    Private Sub DataGridAlarmNuclides_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DataGridAlarmNuclides.DataError
        'noch unklar wie das Ereigniss behandelt werden soll.

    End Sub
End Class