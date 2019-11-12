
Imports System.IO
Imports Mustache

Public Class FHT59N3_PersistSpectrumAnalyse_CSV

    ''' <summary>
    ''' Systems the write template file CSV.
    ''' </summary>
    ''' <param name="templateData">The template data.</param>
    ''' <param name="SpecType">Type of the spec.</param>
    Public Sub SYS_WriteTemplateFileCsv(ByRef templateData As IDictionary(Of String, Object), ByVal SpecType As Integer)

        Try

            Dim destinationFile As String = CalculateCsvDestinationFilePath()
            Dim fileExists As Boolean = File.Exists(destinationFile)
            Dim sep As String = vbTab

            templateData("outputHeader") = Not fileExists
            templateData("sep") = sep

            Dim compiler As New FormatCompiler()
            Dim templateCsvFile As String = Global.FHT59N3.My.Resources.Resources.TEMPLATE_SPECTRA_ANALYSIS_CSV
            templateCsvFile = templateCsvFile.Replace(vbCrLf, "")
            templateCsvFile = templateCsvFile.Replace(sep, "")

            Dim generator As Generator = compiler.Compile(templateCsvFile)
            Dim fileContent As String = generator.Render(templateData)

            fileContent = fileContent.Replace(".", ",")

            File.AppendAllText(destinationFile, fileContent)

        Catch ex As Exception

        End Try


    End Sub


    ''' <summary>
    ''' Calculates the CSV output path and file name (monthly file!)
    ''' </summary>
    ''' <returns>path and file name of the monthly CSV file</returns>
    Public Function CalculateCsvDestinationFilePath() As String

        Dim destinationFile As String = _AnalyzationFilesDirectory + "\" + String.Format("LongTerm_{0}_{1}.csv",
                                                                                         Format(Now, "yyyy"), Format(Now, "MM"))
        Return destinationFile
    End Function

End Class
