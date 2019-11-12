Imports System.IO
Imports System.Linq
Imports FHT59N3Core

Public Class frmNuclideLibrary

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    ''' <summary>
    ''' Handles the Load event of the frmNuclideLibrary control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub frmNuclideLibrary_Load(sender As System.Object, e As EventArgs) Handles MyBase.Load

        For n As Integer = 1 To _MyControlCenter.MCA_Nuclides.NuclideCount
            Dim nuclide As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n)
            DataGridLibraryNuclides.Rows.Add(nuclide.Library.IndexNr, nuclide.Library.Name, nuclide.Library.NuclideHalfLife, nuclide.Library.NuclidNumber)
        Next

    End Sub


    ''' <summary>
    ''' Handles the Click event of the BtnCancel control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub BtnClose_Click(sender As System.Object, e As EventArgs) Handles BtnOk.Click
        Close()
    End Sub

    ''' <summary>
    ''' Handles the DataError event of the DataGridAlarmNuclides control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs"/> instance containing the event data.</param>
    Private Sub DataGridAlarmNuclides_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DataGridLibraryNuclides.DataError
        'noch unklar wie das Ereigniss behandelt werden soll.

    End Sub

    Private Sub BtnExport_Click(sender As System.Object, e As System.EventArgs) Handles BtnExport.Click

        Try
            Dim csvExport As New CsvExport()
            csvExport.ColumnSeparator = "\t"
            csvExport.IncludeColumnSeparatorPreamble = True

            For n As Integer = 1 To _MyControlCenter.MCA_Nuclides.NuclideCount
                csvExport.AddRow()
                Dim nuclide As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n)
                csvExport.Item("Index") = nuclide.Library.IndexNr
                csvExport.Item("Name") = nuclide.Library.Name
                csvExport.Item("Halflife") = nuclide.Library.NuclideHalfLife
                csvExport.Item("NuclidNumber") = nuclide.Library.NuclidNumber
            Next

            Dim filePath As String = _NuclideLibsDirectory + "\" + "Ebin.nlb.csv"
            csvExport.ExportFile(filePath)

            frmMsgBox.LabelMsg.Text = String.Format(ml_string("654", "Export nuclide library ..."), filePath)
            frmMsgBox.ShowDialog(Me)
        Catch ex As Exception
            Trace.TraceError("Error while exporting nuclide library information, due to: " + ex.Message)
            frmMsgBox.LabelMsg.Text = String.Format(ml_string("655", ex.Message))
            frmMsgBox.ShowDialog(Me)
        End Try


      

    End Sub
End Class