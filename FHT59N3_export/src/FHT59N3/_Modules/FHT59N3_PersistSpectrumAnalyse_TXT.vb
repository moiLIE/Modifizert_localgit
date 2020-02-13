Imports System.Globalization
Imports Mustache
Imports FHT59N3Core
Imports FHT59N3Core.FHT59N3MCA_NuclideList
Imports ExterneOpenSource.Mustage
Imports System.Xml
Imports System.Text
Imports System.IO
Imports System.IO.Compression
Imports System.Linq

Module FHT59N3_Persist
    Public Const WRITE_HEADER As Integer = 1
    Public Const APPEND_MESSAGE As Integer = 2
    Public Const APPEND_RESULTS As Integer = 3
    Public Const APPEND_STATE_INFOS As Integer = 4

    Public Const SPECTYPE_ALARMPRUEFUNG As Integer = 0
    Public Const SPECTYPE_BISHERIGER_FILTERSTAND As Integer = 1
    Public Const SPECTYPE_FILTERSTAND As Integer = 2
    Public Const SPECTYPE_TAGESAUSWERTUNG As Integer = 3
    Public Const SPECTYPE_SONDERAUSWERTUNG As Integer = 4
    Public Const SPECTYPE_WARTUNG As Integer = 5

    Public Const FILETYPE_SPECANALYSIS_TEXTFILE = 0
    Public Const FILETYPE_SPECANALYSIS_ANSIN4242 = 1
End Module

Class FHT59N3_PersistSpectrumAnalyse_TXT
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="WhatToDo">1=Write Header, 2=Append a message, 3=Append results, 4=Append state infos</param>
    ''' <param name="Source"></param>
    ''' <param name="SpecType"></param>
    ''' <param name="Msg"></param>
    ''' <remarks></remarks>
    Public Function SYS_SaveAnalyzationResultsToFile(ByVal WhatToDo As Integer, ByVal Source As String, ByVal SpecType As Integer, ByVal Msg As String, Optional ByVal Customer As String = "", Optional ByVal StationName As String = "") As String
        Try
            'ehemals Teil von auswertg
            Static DestinationFile As String = ""

            Select Case WhatToDo

                Case WRITE_HEADER

                    CalculateDatDestinationFilePath(SpecType, Source, DestinationFile)

                    'Übernahme aus CalculateDestinationFilePath da dort falsch platziert...
                    If SpecType = SPECTYPE_ALARMPRUEFUNG Then
                        _ASP2 = _AirFlowMean               'gemittelter Luftdurchsaz
                    End If

                    Dim HeaderContent As String = WriteHeaderToTextFile(SpecType, DestinationFile, Source, StationName, Customer)

                    If SpecType > SPECTYPE_ALARMPRUEFUNG Then 'Alarmprüfungen nicht in die Sammelinfo schreiben
                        SYS_SaveMessageToCollectionFile(HeaderContent)
                    End If

                Case APPEND_MESSAGE

                    My.Computer.FileSystem.WriteAllText(DestinationFile, Msg, True)

                    If SpecType > SPECTYPE_ALARMPRUEFUNG Then 'Alarmprüfungen nicht in die Sammelinfo schreiben
                        SYS_SaveMessageToCollectionFile(Msg)
                    End If

                Case APPEND_RESULTS

                    'Schreiben in Meldungsdatei wird pro Nuklid gemacht, daher in Methode selbst
                    WriteAnalysisResultsTextFile(SpecType, DestinationFile)


                Case APPEND_STATE_INFOS

                    Dim SystemStateText = WriteSystemStatesToTextFile(DestinationFile)

                    If SpecType > SPECTYPE_ALARMPRUEFUNG Then 'Alarmprüfungen nicht in die Sammelinfo schreiben
                        SYS_SaveMessageToCollectionFile(SystemStateText)
                    End If

            End Select

            Return DestinationFile

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return ""
        End Try
    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SpecType"></param>
    ''' <param name="Source"></param>
    ''' <param name="DestinationFile"></param>
    ''' <remarks></remarks>
    Public Sub CalculateDatDestinationFilePath(ByVal SpecType As Integer, ByVal Source As String, ByRef DestinationFile As String)

        Select Case SpecType 'erzoigt zildat=Name der Ergebnissdatai
            'Sonderauswertung
            Case SPECTYPE_SONDERAUSWERTUNG
                Dim SourceParts() As String = Source.Split("\".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                DestinationFile = SourceParts(SourceParts.Length - 1)
                DestinationFile = Left(DestinationFile, DestinationFile.Length - 3) & "tmp"

                'Tagesspektrum-Daten
            Case SPECTYPE_TAGESAUSWERTUNG
                DestinationFile = "T_" & Format(Now, "MM") & Format(Now, "dd") & Format(Now, "HH") & ".dat"

            Case SPECTYPE_FILTERSTAND
                DestinationFile = "B_" & Format(Now, "dd") & Format(Now, "HH") & Format(Now, "mm") & ".dat"

                'bisherige Filterstanddauer
            Case SPECTYPE_BISHERIGER_FILTERSTAND
                DestinationFile = "S_" & Format(Now, "dd") & Format(Now, "HH") & Format(Now, "mm") & ".dat"

                'Alarmprüfung
            Case SPECTYPE_ALARMPRUEFUNG
                DestinationFile = "AP_" & Format(Now, "HH") & Format(Now, "mm") & ".dat"
            Case Else
                'NOP
        End Select

        If SpecType > SPECTYPE_ALARMPRUEFUNG Then
            DestinationFile = _AnalyzationFilesDirectory & "\" & DestinationFile
        Else
            DestinationFile = _AlarmfilesDirectory & "\" & DestinationFile 'Alarmprüfung in einen eigenen Ordner speichern.
        End If

        If System.IO.File.Exists(DestinationFile) Then
            System.IO.File.Delete(DestinationFile) 'gibt es Ergebniss.dat, dann zu löschen
        End If
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SpecType"></param>
    ''' <param name="DestinationFile"></param>
    ''' <param name="Customer"></param>
    ''' <param name="StationName"></param>
    ''' <remarks></remarks>
    Public Function WriteHeaderToTextFile(ByVal SpecType As Integer, ByVal DestinationFile As String, ByVal Source As String, ByVal Customer As String, ByVal StationName As String)

        Dim DFile As String = ""
        Dim ThisCustomer As String = ""
        Dim ThisStationName As String = ""

        Dim NormOrWorkingCubic As String = "Norm-m3"
        If _MyFHT59N3Par.AirFlowWorking Then
            NormOrWorkingCubic = ml_string(225, "Operation-m³")
        End If

        If SpecType > SPECTYPE_ALARMPRUEFUNG Then
            'schraiben: 2 Kopf- und Infosäze nach *.dat
            If SpecType <> SPECTYPE_TAGESAUSWERTUNG Then
                If Customer <> "" Then
                    ThisCustomer = Customer
                Else
                    ThisCustomer = _MyFHT59N3Par.Customer
                End If
                If StationName <> "" Then
                    ThisStationName = StationName
                Else
                    ThisStationName = _MyFHT59N3Par.StationName
                End If
                DFile = DFile & _SSPRSTR4 & ":00 " & _TimeZone & ";" & ThisCustomer & ";" & ThisStationName & vbCrLf
            Else 'Beim Tagessummenspektrum kann es sonst vorkommen das da eine 02:00 oder sowas drinsteht.
                DFile = DFile & Format(Now, "dd.MM.yy HH") & ":00:00 " & _TimeZone & ";" & _MyFHT59N3Par.Customer & ";" & _MyFHT59N3Par.StationName & vbCrLf
            End If

            If SpecType < SPECTYPE_SONDERAUSWERTUNG Then
                Select Case SpecType

                    Case SPECTYPE_TAGESAUSWERTUNG '2.Infozaile: "Luftdurchsaz"
                        DFile = DFile & ml_string(226, "Spectrum of sum of day;Total meas. time (h): ") & Format(_ELIVEHour * _ASP3, "0.0") & vbCrLf
                        DFile = DFile & ml_string(227, "averaged meas. time (h): ") & Format(_ELIVEHour, "0.0") & ml_string(228, ";Air flow (m3/h): ") & Format(_AirFlowMean / _ASP3, "0.0") & "*" & Format(_ASP3, "0") & ";" & NormOrWorkingCubic & vbCrLf

                    Case SPECTYPE_BISHERIGER_FILTERSTAND, SPECTYPE_FILTERSTAND
                        'infozlr% = 1
                        DFile = DFile & ml_string(229, "Spectrum of dustation period;Total meas. time (min): ") & Format(_ELIVE / 60, "0.0") & ml_string(228, ";Air flow (m3/h): ") & Format(_AirFlowMean, "0.0") & ";" & NormOrWorkingCubic & vbCrLf
                    Case Else
                        'NOP
                End Select
            Else 'merker=4
                DFile = DFile & ml_string(230, "Evaluation of the spectrum ") & Source & vbCrLf
                If _ASP4 = 0 Then
                    DFile = DFile & ml_string(231, "Spectrum of sum of day;Start of dustation: ") & _SSPRSTR2 & ":00 " & _TimeZone & ";" & vbCrLf
                End If
                If _ASP4 = 1 Then
                    DFile = DFile & ml_string(232, "Spectrum of dustation;Start of dustation: ") & _SSPRSTR3 & ":00 " & _TimeZone & ";" & vbCrLf
                End If
                If _ASP3 = 1 Then 'Filterstanddauer
                    DFile = DFile & ml_string(233, "Meas.time (h): ") & Format(_ELIVEHour, "0.0") & vbCrLf
                    DFile = DFile & ml_string(234, "Air flow (m3/h): ") & Format(_AirFlowMean, "0.0") & ";" & NormOrWorkingCubic & vbCrLf
                Else 'Tagessumme
                    DFile = DFile & ml_string(235, "Overall meas.time (h): ") & Format(_ELIVEHour * _ASP3, "0.0") & vbCrLf
                    DFile = DFile & ml_string(236, "Average meas.time (h): ") & Format(_ELIVEHour, "0.0") & ml_string(228, ";Air flow (m3/h): ") & Format(_AirFlowMean / _ASP3, "0.0") & "*" & Format(_ASP3, "0") & ";" & NormOrWorkingCubic & vbCrLf
                End If    'if-end ASP3#<>1
            End If      'if-end merker% < 4
        Else 'AP_*.dat
            DFile = DFile & ml_string(237, "Alarm check (") & Format(Now, "dd.MM.yy HH:mm:ss") & ml_string(238, ");Total meas. time (min): ") & Format(_ELIVE / 60, "0.0") & ml_string(228, ";Air flow (m3/h): ") & Format(_AirFlowMean, "0.0") & ";" & NormOrWorkingCubic & vbCrLf
        End If        'If-end merker% > 0

        My.Computer.FileSystem.WriteAllText(DestinationFile, DFile, False)

        Return DFile
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SpecType"></param>
    ''' <param name="DestinationFile"></param>
    ''' <remarks></remarks>
    Public Function WriteAnalysisResultsTextFile(ByVal SpecType As String, ByVal DestinationFile As String)
        'Ausgabe der Messergebnisse -------------------------------
        My.Computer.FileSystem.WriteAllText(DestinationFile, ml_string(239, "Nuclide     ActivConc.(Bq/m3)   Error(%)  DecisionLimit(Bq/m3)") & vbCrLf, True)
        If SpecType > SPECTYPE_ALARMPRUEFUNG Then 'Alarmprüfungen nicht in die Sammelinfo schreiben
            SYS_SaveMessageToCollectionFile(ml_string(239, "Nuclide     ActivConc.(Bq/m3)   Error(%)  DecisionLimit(Bq/m3)") & vbCrLf)
        End If
        Dim ret As Integer
        Dim Conc As String
        Dim DetErr As String
        Dim DetLimit As String
        Dim AlarmNuclide As Boolean

        For n As Integer = 1 To _MyControlCenter.MCA_Nuclides.NuclideCount

            Dim nuclide As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n)

            If nuclide.SpectrumAnalysis.Concentration_Bqm3 > nuclide.SpectrumAnalysis.DetectionLimit_Bqm3 Then

                Conc = " " & Format(nuclide.SpectrumAnalysis.Concentration_Bqm3, "0.0000")
                If Conc.Length > 7 Then Conc = SYS_ConvertNumber(nuclide.SpectrumAnalysis.Concentration_Bqm3)
                Conc = " " & Conc.ToUpper
                ret = InStr(Conc, "E")

                If ret > 0 Then
                    Conc = Left(Conc, ret - 1) & " " & Right(Conc, Conc.Length - ret + 1)
                End If

                Conc = Conc.Replace(",", ".")
                If Conc.Length < 16 Then Conc = Conc & Space(16 - Conc.Length)
                DetErr = " " & Format(nuclide.SpectrumAnalysis.DetectionError_Percent, "0.0")
                If DetErr.Length > 6 Then DetErr = SYS_ConvertNumber(nuclide.SpectrumAnalysis.DetectionError_Percent)
                DetErr = " " & DetErr.ToUpper
                ret = InStr(DetErr, "E")
                If ret > 0 Then
                    DetErr = Left(DetErr, ret - 1) & " " & Right(DetErr, DetErr.Length - ret + 1)
                End If
                DetErr = DetErr.Replace(",", ".")
                If DetErr.Length < 14 Then DetErr = DetErr & Space(14 - DetErr.Length)
            Else
                Conc = "  0             "
                DetErr = "  0           "
            End If   'if-end nwtm#(n%)<=0


            If nuclide.SpectrumAnalysis.DetectionLimit_Bqm3 > 0 Then
                DetLimit = SYS_ConvertNumber(nuclide.SpectrumAnalysis.DetectionLimit_Bqm3)
                DetLimit = " " & DetLimit.ToUpper
                ret = InStr(DetLimit, "E")
                If ret > 0 Then
                    DetLimit = Left(DetLimit, ret - 1) & " " & Right(DetLimit, DetLimit.Length - ret + 1)
                End If
                DetLimit = DetLimit.Replace(",", ".")
                If DetLimit.Length < 16 Then DetLimit = DetLimit & Space(16 - DetLimit.Length)
            Else
                DetLimit = "  0             "
            End If

            If SpecType < SPECTYPE_SONDERAUSWERTUNG Then
                AlarmNuclide = False                 'Merker: ist's Alarmnuklid oder Pb-212 oder Pb-214 ?
                For j As Integer = 1 To _MyControlCenter.MCA_AlarmNuclides.AlarmNuclideCount
                    If _MyControlCenter.MCA_AlarmNuclides.Nuclide_ByNumber(j).Name = nuclide.Library.Name Then
                        AlarmNuclide = True
                    End If
                Next j%
            End If      'if-end merker%<4

            If nuclide.SpectrumAnalysis.Concentration_Bqm3 > 0 Or AlarmNuclide Or SpecType > SPECTYPE_FILTERSTAND Then
                Dim Str As String
                Dim Name As String = nuclide.Library.Name

                If Trim(nuclide.Library.Name) <> "K-40" Then
                    Str = Name & Space(9 - Name.Length) & ";" & Conc & ";" & DetErr & ";" & DetLimit & ";" & vbCrLf
                Else
                    Str = Name & Space(9 - Name.Length) & ";" & Left(Conc, 14) & ";Bq" & DetErr & ";" & Left(DetLimit, 14) & ";Bq" & vbCrLf
                End If  'if-end nkln$(n%)="K-40"

                My.Computer.FileSystem.WriteAllText(DestinationFile, Str, True)

                If SpecType > SPECTYPE_ALARMPRUEFUNG Then 'Alarmprüfungen nicht in die Sammelinfo schreiben
                    SYS_SaveMessageToCollectionFile(Str)
                End If

            End If    'if-end nwtm#(n%) > 0# Or alnuk% = 1
        Next n      'for-end n%=1 to anznuk%

        Return ""
    End Function


    Public Function WriteSystemStatesToTextFile(ByVal DestinationFile As String)
        '----------------------------------------------------------------------
        'Durchsuchen der Staten auf Anormalitäten und Eintragen in zildat
        '----------------------------------------------------------------------
        Dim DFile As String = ""
        If _MyControlCenter.SYS_States.Maintenance Then
            DFile = DFile & ml_string(240, "Monitor in Maintenance") & vbCrLf
        End If
        If _MyControlCenter.SYS_States.K40ToLow_NotFound Then
            DFile = DFile & ml_string(179, "K40-peak too small") & vbCrLf
        End If
        If _MyControlCenter.SYS_States.AnalyzationCancelled Then
            DFile = DFile & ml_string(241, "Evaluation stopped") & vbCrLf
        End If
        If _MyControlCenter.SYS_States.N2FillingGoingLow Then
            If _MyFHT59N3Par.EnableCapturingDetectorTemperature Then
                DFile = DFile & MSG_RecordingDetectorTemperaturIsDefect & vbCrLf
            Else
                DFile = DFile & MSG_N2NearEnd & vbCrLf
            End If
        End If
        If _MyControlCenter.SYS_States.FilterHasToBeChanged Then
            DFile = DFile & MSG_TapeNearEnd & vbCrLf
        End If
        If _MyControlCenter.SYS_States.CheckTempPressure Then
            DFile = DFile & MSG_CheckTempPressure & vbCrLf
        End If
        If _MyControlCenter.SYS_States.K40ToBig Then
            DFile = DFile & ml_string(242, "K40-FWHM too broad") & vbCrLf
        End If
        If _MyControlCenter.SYS_States.AirFlowLessThen1Cubic Then
            DFile = DFile & String.Format(ml_string(325, "Air flow less than {0} m³/h"), _MyFHT59N3Par.MinAirFlowAlert) & vbCrLf
        End If
        If _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic Then
            DFile = DFile & String.Format(ml_string(335, "Air flow greater than {0} m³/h"), _MyFHT59N3Par.MaxAirFlowAlert) & vbCrLf
        End If
        If _MyControlCenter.SYS_States.DataTransferError Then
            DFile = DFile & ml_string(244, "Communication problems with SPS") & vbCrLf
        End If
        If _MyControlCenter.SYS_States.UpsOnBattery Then
            DFile = DFile & ml_string(591, "UPS operation (on battery)") & vbCrLf
        End If

        My.Computer.FileSystem.WriteAllText(DestinationFile, DFile, True)
        Return DFile
    End Function
End Class