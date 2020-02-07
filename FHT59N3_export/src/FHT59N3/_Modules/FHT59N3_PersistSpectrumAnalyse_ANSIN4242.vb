Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports System.Xml
Imports FHT59N3Core
Imports Mustache

Class FHT59N3_PersistSpectrumAnalyse_ANSIN4242
    Public Sub SYS_StoreCurrentSpectrumResultsAnsiN4242(ByRef templateData As IDictionary(Of String, Object), ByVal SpecType As Integer)

        Try

            'add template placeholders for <InstrumentInformation> tag
            SetTemplateTagsInstrumentInformation(SpecType, templateData)

            SetTemplateTagsSensorInformation(SpecType, templateData)

            'add template placeholders for <Spectrum> tag
            SetTemplateTagsSpectrum(templateData)

            'add template placeholders for <AnalysisResults> tag
            SetTemplateTagsNuclidAnalysis(templateData)

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try

    End Sub

   


    Public Sub SYS_StoreCalibrationDataAnsiN4242(ByRef templateData As IDictionary(Of String, Object), ByVal CalibrationType As FHT59N3_SystemParams.CalibrationTypeEnum)

        Try

            Dim spectrumFile As String = _na_mischS
            If (CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration) Then
                spectrumFile = _naS
            End If

            'Holen der Energie/Effizienz aus nah.cnf/nah_misch.cnf
            GUI_OpenSpectrum(spectrumFile, SpectraTypes.OFFLINE, False, False, Nothing)

            'add template placeholders for the <Calibration> tags (inside Spectrum and outside)
            SetTemplateTagsCalibration(templateData)

            GUI_CloseSpectrum(False, True)


        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try

    End Sub



    Public Sub SYS_WriteTemplateFileAnsiN4242(ByRef templateData As IDictionary(Of String, Object), ByVal SpecType As Integer)

        Try
            Dim compiler As New FormatCompiler()
            Dim templateAnsi4242 As String = Global.FHT59N3.My.Resources.Resources.TEMPLATE_SPECTRA_ANALYSIS_ANSI4242
            Dim generator As Generator = compiler.Compile(templateAnsi4242)
            Dim ansi4242FileContent As String = generator.Render(templateData)

            StripComments(ansi4242FileContent)

            Dim plainXml As Boolean = _MyFHT59N3Par.AnsiN4242Settings.HasFlag(AnsiN4242Settings.GeneratePlainXml)
            If plainXml Then
                Dim DestinationFile As String = CalculateAnsiN4242DestinationFilePath(SpecType,
                                                                                      _MyFHT59N3Par.StationSerialNumber, ".xml")
                My.Computer.FileSystem.WriteAllText(DestinationFile, ansi4242FileContent, False)
            Else
                Dim DestinationFile As String = CalculateAnsiN4242DestinationFilePath(SpecType,
                                                                                      _MyFHT59N3Par.StationSerialNumber, ".xml.gz")
                Dim GzipBytes As Array = CompressASCII(ansi4242FileContent)
                My.Computer.FileSystem.WriteAllBytes(DestinationFile, GzipBytes, False)

            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try

    End Sub

   

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SpecType"></param>
    ''' <param name="templateData"></param>
    ''' <remarks></remarks>
    Public Sub SetTemplateTagsInstrumentInformation(ByVal SpecType As String, ByRef templateData As IDictionary(Of String, Object))

        'add data for <Measurement></Measurement> and <InstrumentInformation></InstrumentInformation>
        templateData.Add("measurement_id", Guid.NewGuid)

        templateData.Add("serial_id", _MyFHT59N3Par.StationSerialNumber)
        templateData.Add("station_name", _MyFHT59N3Par.StationName)

        Dim vers As System.Version = My.Application.Info.Version
        Dim fullProgramVersion As String = String.Format("{0}.{1}.{2} ({3})", vers.Major, vers.Minor,
                                            vers.Build, vers.Revision)

        templateData.Add("program_version", fullProgramVersion)

        SetFileType(SpecType, templateData)

        'vermutlich ein fester Wert
        templateData.Add("type_instrument_mode", "Measure")

        ' kann folgende Werte annehmen: Normal, Alarm, Intensive and Maintenance
        Dim measurementMode = "Normal"
        Dim sysStates As FHT59N3_SystemStates = _MyControlCenter.SYS_States
        If (sysStates.AlarmMode) Then
            measurementMode = "Alarm"
        ElseIf (sysStates.IntensiveMode) Then
            measurementMode = "Intensive"
        ElseIf (sysStates.Maintenance) Then
            measurementMode = "Maintenance"
        End If

        templateData.Add("measurement_mode", measurementMode)

        templateData.Add("systemstate_summary", _MyControlCenter.SYS_States.SumState)
        templateData.Add("systemstate_enumeration", GetSystemStateSummary())

        'Sensordaten hinzufügen

    End Sub

    Private Sub SetTemplateTagsSensorInformation(specType As Integer, templateData As IDictionary(Of String, Object))

        'Mal sehen ob wir einfach die Aktualwerte nehmen können, evtl geht das nur in den Alarm/Zwischenauswertungen da sich beim
        'Filterwechsel ja alles kurzfristig ändern und keine Aussagekraft hat.

        'Dont take e.g. direct _MyControlCenter.SPS_PressureFilter as it is only the raw sampling value!

        templateData.Add("sensor_temperature_detector", GetDecimalThreeDigit(_DetectorTemperaturValue))
        templateData.Add("sensor_temperature_after_filter", GetDecimalThreeDigit(_Temperature))
        templateData.Add("sensor_temperature_external", GetDecimalThreeDigit(_ExternalTemperature))
        templateData.Add("sensor_pressure_environment", GetDecimalThreeDigit(_PressureEnvironment))
        templateData.Add("sensor_pressure_bezel", GetDecimalThreeDigit(_PressureBezel))
        templateData.Add("sensor_pressure_filter", GetDecimalThreeDigit(_PressureFilter))


    End Sub

    Private Sub SetTemplateTagsSpectrum(ByRef templateData As IDictionary(Of String, Object))

        Dim sysParams As FHT59N3_SystemParams = _MyFHT59N3Par

        Dim startTime As String = If(_SSPRSTR3 <> "", _SSPRSTR3 & ":00", Now.ToString)
        Dim startTimeOffset As DateTimeOffset = New DateTimeOffset(startTime)
        Dim startTimeUtc As Date = startTimeOffset.ToUniversalTime().UtcDateTime
        Dim endTimeUtc As Date = If(_SSPRSTR4 <> "", _SSPRSTR4 & ":00", Now.ToString)

        templateData.Add("start_measurement_time_utc", Format(startTimeUtc, "yyyy-MM-ddTHH:mm:00Z"))
        templateData.Add("end_measurement_time_utc", Format(endTimeUtc, "yyyy-MM-ddTHH:mm:00Z"))
        templateData.Add("start_measurement_time_utc_plain", Format(startTimeUtc, "yyyy-MM-dd HH:mm:00"))
        templateData.Add("end_measurement_time_utc_plain", Format(endTimeUtc, "yyyy-MM-dd HH:mm:00"))


        'Zeit laut ISO 8601...
        templateData.Add("realtime", GetDecimalThreeDigit(_EREAL))
        templateData.Add("livetime", GetDecimalThreeDigit(_ELIVE))

        templateData.Add("air_flow_type", If(sysParams.AirFlowWorking, "stp_p", "stp_n"))
        templateData.Add("air_flow_value", GetDecimal(_AirFlowMean))

        templateData.Add("air_volume_filtered", GetDecimal(_AirFlowMean * (_EREAL / 3600)))

        templateData.Add("k40_fwhm_peak", GetDecimal(_K40_FWHN))
        templateData.Add("k40_fwhm_areaPerHour_abs", GetDecimal(_K40AREA_PER_HOUR_ABS))
        templateData.Add("k40_fwhm_areaPerHour_percent", GetDecimal(_K40AREA_PER_HOUR_PERCENT))
        'kann negativ sein...
        templateData.Add("k40_fwhm_drift", GetDecimal(_K40AREA_DRIFT))
        templateData.Add("sara_k40_fwhm_peak_correction", "0.998")


        templateData.Add("air_pressure_environment", GetDecimal(_PressureEnvironment))
        templateData.Add("air_temperature_after_filter", GetDecimal(_Temperature))

        CreateChannelData(templateData)

    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="templateData"></param>
    ''' <remarks></remarks>
    Private Sub SetTemplateTagsNuclidAnalysis(ByRef templateData As IDictionary(Of String, Object))
        'add data for <AnalysisResults></AnalysisResults>
        Dim analysisNuclidData As IList(Of Dictionary(Of String, Object)) = New List(Of Dictionary(Of String, Object))()
        templateData.Add("analysis_nuclids", analysisNuclidData)

        Dim nuclideList As FHT59N3MCA_NuclideList = _MyControlCenter.MCA_Nuclides

        templateData.Add("maxViolatedAlarmLevel_AllNuclides", nuclideList.AlarmMaxViolocatedLevel.ToString())

        'Schleife über alle gefundenen Nuklide der letzten Auswertung...
        For n As Integer = 1 To nuclideList.NuclideCount
            Dim nuclide As FHT59N3MCA_Nuclide = nuclideList.GetNuclide(n)

            Dim activity As Double = nuclide.SpectrumAnalysis.Activity
            Dim concBqm3 As Double = nuclide.SpectrumAnalysis.Concentration_Bqm3
            Dim detectLimitBqm3 As Double = nuclide.SpectrumAnalysis.DetectionLimit_Bqm3
            Dim uncertainty As Double = nuclide.SpectrumAnalysis.DetectionError_Percent
            Dim correctionFactor As Double = nuclide.SpectrumAnalysis.NuclideCorrectionFactor
            Dim nuclidenumber As Integer = nuclide.SpectrumAnalysis.SpectrumNuclideNumber

            Dim showEmpty As Boolean = _MyFHT59N3Par.AnsiN4242Settings.HasFlag(AnsiN4242Settings.ShowEmptyAnalyzedNuclids)

            Dim showNuclid = (showEmpty Or concBqm3 > 0) AndAlso nuclide.Library.Name <> "K-40"
            'Dim showNuclid = concBqm3 > detectLimitBqm3 And mcaNuclide.Name <> "K-40"

            Dim nuclidData As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
            nuclidData.Add("showNuclid", If(showNuclid, "True", Nothing))

            nuclidData.Add("nuclid_remark", "")
            'am einfachsten als Camelcase behandeln, konvertiert z.B. CO-60 nach Co-60
            nuclidData.Add("nuclid_name", StrConv(nuclide.Library.Name, VbStrConv.ProperCase))
            nuclidData.Add("nuclid_number", nuclide.Library.NuclidNumber)

            nuclidData.Add("nuclid_confidence_ind", "")
            nuclidData.Add("nuclid_confidence_desc", "")

            nuclidData.Add("nuclid_activity", GetDoubleScientific(activity))
            nuclidData.Add("nuclid_activity_concentration", GetDoubleScientific(concBqm3))
            nuclidData.Add("nuclid_detection_limit", GetDoubleScientific(detectLimitBqm3))
            nuclidData.Add("nuclid_uncertainty", GetDoubleScientific(uncertainty))

            nuclidData.Add("nuclid_correction_factor", GetDecimalDotted(correctionFactor))

            nuclidData.Add("nuclid_activity_plain", GetDecimalDotted(activity))
            nuclidData.Add("nuclid_activity_concentration_plain", GetDecimalDotted(concBqm3))
            nuclidData.Add("nuclid_detection_limit_plain", GetDecimalDotted(detectLimitBqm3))
            nuclidData.Add("nuclid_uncertainty_plain", GetDecimalDotted(uncertainty))
            'nuclidData.Add("nuclid_decision_limit", GetDoubleScientific(...))

            'Momentan nur eine Linie, alle Nebenlinien kann ich momentan noch nicht gescheit erkennen...
            'Dim peaksForNuclide As List(Of FHT59N3MCA_Peak) = _MyControlCenter.MCA_Peaks.GetPeakByNuclidEnergy(mcaNuclide.KeyLineEnergy)
            Dim peaksForNuclide As IEnumerable(Of FHT59N3MCA_Peak) = _MyControlCenter.MCA_Peaks.PeakList.Where(Function(d)
                                                                                                                   'Return CInt(nuclide.SpectrumAnalysis.KeyLineEnergy) = CInt(d.PeakEnergy)
                                                                                                                   Return d.NuclideNumber = nuclidenumber
                                                                                                               End Function)

            '0 falls kein Peak, wird von Template-Engine dann nicht genutzt...
            nuclidData.Add("isPeak", If(peaksForNuclide.Count > 0, "true", Nothing))
            If (peaksForNuclide.Count > 0) Then
                Dim NuclidPeaks As IList(Of Dictionary(Of String, String)) = New List(Of Dictionary(Of String, String))()
                Dim PeaksToRemove As IList(Of FHT59N3MCA_Peak) = New List(Of FHT59N3MCA_Peak)()
                nuclidData.Add("nuclide_peaks", NuclidPeaks)
                For Each peak As FHT59N3MCA_Peak In peaksForNuclide
                    'For m As Integer = 0 To peaksForNuclide.Count - 1
                    Dim peakData As Dictionary(Of String, String) = New Dictionary(Of String, String)()
                    'Dim peak As FHT59N3MCA_Peak = peaksForNuclide.ElementAt(m)
                    peakData.Add("peak_channel", peak.PeakChannel)
                    peakData.Add("peak_energy", GetDecimal(peak.PeakEnergy))
                    peakData.Add("peak_area", GetDecimal(peak.PeakArea))
                    peakData.Add("peak_fwhm", GetDecimalThreeDigit(peak.PeakFwhm))

                    peakData.Add("peak_areaerr", GetDecimal(peak.PeakAreaErr))
                    peakData.Add("peak_bckg", GetDecimalThreeDigit(peak.PeakBckg))
                    peakData.Add("peak_bckgerr", GetDecimalThreeDigit(peak.PeakBckgErr))

                    Dim peaktype As String = "misc"
                    If peak.IsKeyLine Then
                        peaktype = "key"
                    ElseIf peak.UseWtm Then
                        peaktype = "wtm"
                    End If
                    peakData.Add("peak_type", peaktype)

                    'Add the peak to the list of peaks associated with the nuclide 
                    NuclidPeaks.Add(peakData)

                    PeaksToRemove.Add(peak)
                Next

                For Each peak As FHT59N3MCA_Peak In PeaksToRemove
                    'ist nur eine temporäre Liste, daher die bereits ausgegebenen Peaks rausnehmen, damit die "not assigned peaks" übrigbleiben
                    _MyControlCenter.MCA_Peaks.PeakList.Remove(peak)
                Next

            End If

            'Wert wird immer ausgegeben
            nuclidData.Add("isAlarmLimitExceeded", If(nuclide.SpectrumAnalysis.ExceededAlarmLevel > 0, "True", "False"))
            nuclidData.Add("maxViolatedAlarmLevel", nuclide.SpectrumAnalysis.ExceededAlarmLevel.ToString())

            'Alarmlimit wird ausgegeben wenn ein Alarm definiert ist
            nuclidData.Add("isAlarmLimitDefined", If(nuclide.SpectrumAnalysis.Alarm1Limit > 0 Or nuclide.SpectrumAnalysis.Alarm2Limit > 0, "True", Nothing))
            nuclidData.Add("alarm1_limit_configured", GetDoubleScientific(nuclide.SpectrumAnalysis.Alarm1Limit))
            nuclidData.Add("alarm1_limit_2hour_hysterese", GetDoubleScientific(nuclide.SpectrumAnalysis.Alarm1LimitCurrent))
            nuclidData.Add("alarm2_limit_configured", GetDoubleScientific(nuclide.SpectrumAnalysis.Alarm2Limit))
            nuclidData.Add("alarm2_limit_2hour_hysterese", GetDoubleScientific(nuclide.SpectrumAnalysis.Alarm2LimitCurrent))

            analysisNuclidData.Add(nuclidData)

        Next n

        'Kleine Umsortierung um natürliche Nuklide am Anfang der Liste zu haben (wegen CSV-Export)
        'Try
        'For Each nuclideEntry As Dictionary(Of String, Object) In New List(Of Dictionary(Of String, Object))(analysisNuclidData)
        ''Object->String might not work anymore ?
        'Dim name As String = ToString(nuclideEntry("nuclid_name"))
        'If (name = "Pb-214" Or name = "Bi-214" Or name = "K-40") Then
        'analysisNuclidData.Remove(nuclideEntry)
        'analysisNuclidData.Insert(0, nuclideEntry)
        'End If
        'Next
        'Catch ex As Exception
        'MsgBox(ex.Message)
        'End Try

        Dim notAssignedPeaks As IList(Of Dictionary(Of String, String)) = New List(Of Dictionary(Of String, String))()
        templateData.Add("not_assigned_peaks", notAssignedPeaks)

        'Schleife über alle gefundenen Nuklide der letzten Auswertung...
        For Each remainingPeak As FHT59N3MCA_Peak In _MyControlCenter.MCA_Peaks.PeakList
            Dim peakData As Dictionary(Of String, String) = New Dictionary(Of String, String)()

            peakData.Add("peak_channel", remainingPeak.PeakChannel)
            peakData.Add("peak_energy", GetDecimal(remainingPeak.PeakEnergy))

            peakData.Add("peak_grosscounts", GetDecimal(remainingPeak.GrossCounts))
            peakData.Add("peak_area", GetDecimal(remainingPeak.PeakArea))
            peakData.Add("peak_fwhm", GetDecimal(remainingPeak.PeakFwhm))
            'peakData.Add("nuclid_name", remainingPeak.NuclideName)

            peakData.Add("peak_areaerr", GetDecimal(remainingPeak.PeakAreaErr))
            peakData.Add("peak_bckg", GetDecimalThreeDigit(remainingPeak.PeakBckg))
            peakData.Add("peak_bckgerr", GetDecimalThreeDigit(remainingPeak.PeakBckgErr))
            'peakData.Add("peak_rchisq", GetDecimalThreeDigit(remainingPeak.PeakRChiSq))

            notAssignedPeaks.Add(peakData)
        Next

    End Sub


    ''' <summary>
    ''' add data for <Calibration></Calibration> tags
    ''' </summary>
    ''' <param name="templateData"></param>
    ''' <remarks></remarks>
    Private Sub SetTemplateTagsCalibration(ByRef templateData As IDictionary(Of String, Object))

        'auf Defaultwert setzen falls ja mal die Kalibrierdaten nicht gelesen werden können sollten...
        templateData("energy_calibration_coefficients_spaced") = ""
        templateData("fwhm_calibration_coefficients_spaced") = ""
        templateData("efficiency_calibration_xypoints") = ""

        'naS bzw. naMisch laden und auswerten...

        Try
            Dim energyCoefficients() As Single = _SpectraFile.Calibrations.Energy.Curve
            Dim energyCoefficientValues As New List(Of String)
            For energyIdx As Integer = 0 To energyCoefficients.Length - 1
                Dim coeff As Single = energyCoefficients(energyIdx)
                If (coeff <> 0) Then
                    energyCoefficientValues.Add(GetDoubleScientific(energyCoefficients(energyIdx)))
                End If
            Next
            templateData("energy_calibration_coefficients_spaced") = String.Join(" ", energyCoefficientValues)

            Dim fwhnPoints() As Single = _SpectraFile.Calibrations.Fwhm.Curve
            Dim fwhnValues As New List(Of String)
            For fwhnIdx As Single = 0 To fwhnPoints.Length - 1
                fwhnValues.Add(GetDoubleScientific(fwhnPoints(fwhnIdx)))
            Next
            templateData("fwhm_calibration_coefficients_spaced") = String.Join(" ", fwhnValues)


            Dim efficiencyPoints(,) As Single = _SpectraFile.Calibrations.Efficiency.EfficiencyPoints
            Dim calibXyPoints As IList(Of Dictionary(Of String, String)) = New List(Of Dictionary(Of String, String))

            '2-dim Array, etwas unüblich gegliedert...
            For effIdx As Integer = 0 To efficiencyPoints.GetUpperBound(1)
                Dim x As Single = efficiencyPoints(0, effIdx)
                Dim y As Single = efficiencyPoints(1, effIdx)
                Dim e As Single = efficiencyPoints(2, effIdx)
                calibXyPoints.Add(CreateMap(GetDecimal(x), GetDoubleScientific(y), GetDoubleScientific(e)))
            Next
            templateData("efficiency_calibration_xypoints") = calibXyPoints

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try

    End Sub


    Private Function CreateMap(ByVal ParamArray ValueArgs() As String) As Dictionary(Of String, String)

        Dim Dict As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Dim i As Integer
        For i = 0 To UBound(ValueArgs)
            Dim key As String = "Item" & (i + 1)
            Dict.Add(key, ValueArgs(i))
        Next

        CreateMap = Dict
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="templateData"></param>
    ''' <remarks></remarks>
    Private Sub CreateChannelData(ByRef templateData As IDictionary(Of String, Object))
        Dim channelList As List(Of String) = New List(Of String)()
        Dim testMode = False

        If (testMode) Then

            Dim numberOfRecords As Integer = 4096
            For n As Integer = 0 To numberOfRecords
                channelList.Add(0)
            Next

        Else
            Dim numberOfRecords As Integer = 4094
            Dim spek As Object = _SpectraFile.Spectrum(0&, 4095&)
            For n As Integer = 0 To numberOfRecords
                Dim countRate As Double = spek(n)
                channelList.Add(countRate)
            Next

        End If

        Dim channelData = String.Join(" ", channelList.ToArray())
        templateData.Add("channel_data", channelData)
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSystemStateSummary()

        Dim activeSystemStates As List(Of String) = New List(Of String)()
        Dim sysStates As FHT59N3_SystemStates = _MyControlCenter.SYS_States

        If (sysStates.Maintenance) Then
            activeSystemStates.Add("MaintenanceOn")
        End If
        If (sysStates.K40ToLow_NotFound) Then
            activeSystemStates.Add("K40NotDetected")
        End If
        If (sysStates.AirFlowLessThen1Cubic) Then
            activeSystemStates.Add("AirFlowBelowLimit")
        End If
        If (sysStates.HVOff) Then
            activeSystemStates.Add("McaHighVoltageOff")
        End If
        If (sysStates.NoFilterstep) Then
            activeSystemStates.Add("NoFilterStep")
        End If
        If (sysStates.BypassOpen) Then
            activeSystemStates.Add("BypassOpen")
        End If
        If (sysStates.AnalyzationCancelled) Then
            activeSystemStates.Add("AnalyzationCancelled")
        End If
        If (sysStates.AirFlowGreaterThen12Cubic) Then
            activeSystemStates.Add("AirFlowAboveLimit")
        End If
        If (sysStates.EcoolerOff) Then
            activeSystemStates.Add("EcoolerOff")
        End If
        If (sysStates.SpectrumDeadTimeBigger20Percent) Then
            activeSystemStates.Add("SpectrumDeadTimeBigger20Percent")
        End If
        If (sysStates.N2FillingGoingLow) Then
            activeSystemStates.Add("InvalidDetectorTemperature")
        End If
        If (sysStates.FilterHasToBeChanged) Then
            activeSystemStates.Add("FilterHasToBeChanged")
        End If
        If (sysStates.CheckTempPressure) Then
            activeSystemStates.Add("InvalidAirTempOrPressure")
        End If
        If (sysStates.K40ToBig) Then
            activeSystemStates.Add("K40ToBig")
        End If
        If (sysStates.DataTransferError) Then
            activeSystemStates.Add("DataTransferErrorPlc")
        End If
        If (sysStates.UpsOnBattery) Then
            activeSystemStates.Add("UpsOnBattery")
        End If

        Return String.Join(",", activeSystemStates.ToArray)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SpecType"></param>
    ''' <param name="SerialNumber"></param>
    ''' <param name="FileEnding"></param>
    ''' <remarks></remarks>
    Public Function CalculateAnsiN4242DestinationFilePath(ByVal SpecType As Integer, ByVal SerialNumber As String,
                                                          ByRef FileEnding As String)

        Dim DestinationFile As String = "_UNDEFINED_ANSIN4242_FILENAME"
        Dim CurrentTime As DateTime = Date.UtcNow
        Dim TimeZoneChar As String = "Z"
        Dim BaseFileName As String = "fht" & SerialNumber & "_" & Format(CurrentTime, "yyyy-MM-ddTHH_mm_00") & _
                                     TimeZoneChar

        Select Case SpecType
            'Tagesspektrum-Daten
            Case SPECTYPE_TAGESAUSWERTUNG
                DestinationFile = BaseFileName & "-" & "day" & FileEnding

                'Filterstanddauer
            Case SPECTYPE_FILTERSTAND
                DestinationFile = BaseFileName & "-" & "period" & FileEnding

                'bisherige Filterstanddauer
            Case SPECTYPE_BISHERIGER_FILTERSTAND
                DestinationFile = BaseFileName & "-" & "partperiod" & FileEnding

                'Alarmprüfung
            Case SPECTYPE_ALARMPRUEFUNG
                DestinationFile = BaseFileName & "-" & "alarm" & FileEnding

                'Sonderauswertung 
            Case SPECTYPE_SONDERAUSWERTUNG
                DestinationFile = BaseFileName & "-" & "userrequested" & FileEnding

                'andere Fälle (sollte aktuell nicht genutzt sein)
            Case Else
                DestinationFile = BaseFileName & "-" & "undefined" & FileEnding

        End Select

        DestinationFile = _AnalyzationN4242FilesDirectory & "\" & DestinationFile

        If System.IO.File.Exists(DestinationFile) Then
            System.IO.File.Delete(DestinationFile)
        End If

        Return DestinationFile
    End Function


    Private Sub SetFileType(ByVal SpecType As String, ByRef templateData As IDictionary(Of String, Object))

        Dim fileType As String = "_UNDEFINED"
        Select Case SpecType
            Case SPECTYPE_ALARMPRUEFUNG
                fileType = "AlarmCheck"
            Case SPECTYPE_BISHERIGER_FILTERSTAND
                fileType = "SpectrumOfMeasurementPeriod"
            Case SPECTYPE_FILTERSTAND
                fileType = "SpectrumOfDustationPeriod"
            Case SPECTYPE_TAGESAUSWERTUNG
                fileType = "SpectrumOfTheDay"
            Case SPECTYPE_SONDERAUSWERTUNG
                fileType = "UserForcedAnalysis"
        End Select

        templateData.Add("file_type", fileType)
    End Sub



    Sub StripComments(ByRef xmlFileContent As String)
        Dim xmlDoc As XmlDocument = New XmlDocument
        xmlDoc.LoadXml(xmlFileContent)
        Dim list As XmlNodeList = xmlDoc.SelectNodes("//comment()")
        For Each node As XmlNode In list
            node.ParentNode.RemoveChild(node)
        Next
        xmlFileContent = ToUtf8IndentedString(xmlDoc)
    End Sub

    Function ToUtf8IndentedString(doc As XmlDocument) As String

        Dim settings As XmlWriterSettings = New XmlWriterSettings() With
        {
            .Encoding = Encoding.UTF8,
            .Indent = True
        }

        Using stringWriter = New StringWriter(New StringBuilder())
            Using xWriter = XmlWriter.Create(stringWriter, settings)
                doc.WriteContentTo(xWriter)
            End Using
            Return stringWriter.ToString()
        End Using

    End Function

    Function CompressASCII(str As String) As Byte()
        Dim bytes As Byte() = Encoding.ASCII.GetBytes(str)
        Using ms As New MemoryStream
            Using gzStream As New GZipStream(ms, CompressionMode.Compress)
                gzStream.Write(bytes, 0, bytes.Length)
            End Using
            Return ms.ToArray
        End Using
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDoubleScientific(ByVal value As Double)

        Dim numberAsString As String = SYS_ConvertNumber(value)
        'SYS_ConvertNumber liefert führendes Leerzeichen, weg damit...
        numberAsString = numberAsString.Trim.ToUpper
        numberAsString = numberAsString.Replace(",", ".")
        Return numberAsString
    End Function

    Private Function GetDecimal(ByVal value As Double)
        Dim numberAsString As String = Format(value, "0.00")
        numberAsString = numberAsString.Replace(",", ".")
        Return numberAsString
    End Function

    Private Function GetDecimalDotted(ByVal value As Double)
        If (IsNothing(value)) Then
            Return ""
        End If
        Return value.ToString().Replace(",", ".")
    End Function

    Private Function GetDecimalThreeDigit(ByVal value As Double)
        Dim numberAsString As String = Format(value, "0.000")
        numberAsString = numberAsString.Replace(",", ".")
        Return numberAsString
    End Function

    Public Class EfficiencyCalib
        Public Property pointx As String
        Public Property pointy As String
        Public Property pointy_err As String

        Sub New(ByVal Item1 As String, ByVal Item2 As String, ByVal Item3 As String)
            pointx = Item1
            pointy = Item2
            pointy_err = Item3
        End Sub


    End Class
End Class
