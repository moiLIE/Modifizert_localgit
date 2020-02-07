Imports System.IO.Directory
Imports System.IO.File
Imports System.Linq
Imports FHT59N3Core
Imports FHT59N3Core.FHT59N3MCA_NuclideList

' -------------------------------------------------------------
' $Id: FHT59N3_ComputeSpectrumAnalyze.vb 430 2017-09-11 10:47:47Z marcel.bender $
' Title: compute and calculation functions
'
' Description:
' compute:
'  - Spectrum
'  - Calibration of components
' -------------------------------------------------------------
Class FHT59N3_ComputeSpectrumAnalyze

    'Optionen die mitgegeben werden können zur Auswertung
    Public Enum AnalyzeOptions
        None
        ForceAlarmCheck
    End Enum

    Protected persistSpectrumTxt As New FHT59N3_PersistSpectrumAnalyse_TXT
    Protected persistSpectrumAnsiN4242 As New FHT59N3_PersistSpectrumAnalyse_ANSIN4242
    Private nuclide1 As NuclideSpectrumParams
    Private nuclide2 As NuclideSpectrumParams
    Private nuclide3 As NuclideSpectrumParams

    ''' <summary>
    ''' auswertg wertet Spektrum aus (inkl. Sonderauswertungen, sie qelle)
    ''' wenn SpecType=4, dann asp4=1 wi STD, =0 wi tag
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <param name="SpecType">4=sonder, 3=TAG.CNF, 2=STD.CNF(Filterstanddauer), 1=STD.CNF (Zwischenauswertung), 0=MIN.CNF (Alarmprüfung)</param>
    ''' <remarks></remarks>
    Public Sub MCA_AnalyzeSpectrum(Source As String, SpecType As Integer, Optional AnalyzeOptions As AnalyzeOptions = AnalyzeOptions.None)
        'ehemals auswertg

        Dim quellBackCnf As New CanberraDataAccessLib.DataAccess
        Dim asfWerter As New CanberraSequenceAnalyzerLib.SequenceAnalyzer

        Dim DestinationReport As String = ""

        Dim rettELIVE As Double
        Dim rettEREAL As Double
        Dim abortAnalysis As Boolean

        'Erkennung Fehlerfall wenn ELIVE=0 war
        Dim ELIVEEREAL As Boolean = False

        Try

            LoadAndShowSpectrumFile(Source, SpecType, ELIVEEREAL)
            Dim invalidSpectrumFile As Boolean = _SpectraFile Is Nothing
            If (invalidSpectrumFile) Then
                Exit Sub
            End If

            SetSpectrumAnalysisParameters(SpecType, DestinationReport, rettELIVE, rettEREAL)

            EnsureNoK40StateOnWaitTime()

            _MyControlCenter.SYS_States.AnalyzationCancelled = False 'Auswertungsabbbruch

            'Header speichern
            persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(WRITE_HEADER, Source, SpecType, "", _SpecCustomer, _SpecStationName)
            If ELIVEEREAL Then
                ELIVEEREAL = False
                persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, "ELIVE=0; EREAL=" & Format(_EREAL, "0.0") & ml_string(172, "; set ELIVE=EREAL ") & vbCrLf)
            End If

            CheckAirFlowBoundaries(Source, SpecType)

            abortAnalysis = PerformK40Search(asfWerter, Source, SpecType, rettELIVE, rettEREAL)
            If (abortAnalysis AndAlso SpecType <> SPECTYPE_SONDERAUSWERTUNG) Then
                GUI_CloseSpectrum(True, False)
                persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, ml_string(613, "Spectrum analysis aborted - K40 line not found") & vbCrLf)
                Exit Sub
            End If

            AnalyseFullSpectrum(asfWerter, Source, SpecType, DestinationReport)

            PerformRecalibrateNaturalLinesIfNecessary(SpecType, DestinationReport)

            abortAnalysis = AlertAnalysisWithoutLines(Source, SpecType, rettELIVE, rettEREAL)
            If (abortAnalysis) Then
                Exit Sub
            End If

            PrepareSubtractBackground(asfWerter, quellBackCnf)

            IdentifyLines(asfWerter)

            WriteFatalStatus(Source, SpecType)

            'Jetzt wird gerechnet...

            FlushNuclidParametersEmpty()

            'Messdauer inn Stunden
            _EREALHour = _EREAL / 3600

            SetNuclideCorrectionFactor()

            SetAlarmNuclidThresholds()

            abortAnalysis = CalculateNuclidEnergyAndPeaks(Source, SpecType, rettELIVE, rettEREAL)
            If (abortAnalysis) Then
                Exit Sub
            End If

            CalculateNuclidActivityAndDetectionLimit()
            CalculateConcentrationAndDetectLimit()

            If SpecType < SPECTYPE_TAGESAUSWERTUNG Or AnalyzeOptions = AnalyzeOptions.ForceAlarmCheck Then
                PerformAlarmDetection()
            End If

            AlertPB214Recalibration(SpecType)

            If (SpecType = SPECTYPE_TAGESAUSWERTUNG) Or (SpecType = SPECTYPE_SONDERAUSWERTUNG And _ASP4 = 0) Then 'wenn Tagesspektrum
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = rettELIVE
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = rettEREAL
            End If

            'Ansi N4242 generieren wenn Spektrum noch offen (da darauf direkt zugegriffen wird)
            'momentan erstellen wir die N4242-Dateien immer
            Dim templateData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
            If (_MyFHT59N3Par.GenerateAnsiN4242) Then
                persistSpectrumAnsiN4242.SYS_StoreCurrentSpectrumResultsAnsiN4242(templateData, SpecType)
            End If

            'reguläres Schließen von der aktuellen Spektrum-Datei
            GUI_CloseSpectrum(True, False)

            'Alles in Monats-Csv-Datei protokollieren, benötigt Template-Map der ANSI N42.42-Generierung
            Dim csvExport As New FHT59N3_PersistSpectrumAnalyse_CSV
            csvExport.SYS_WriteTemplateFileCsv(templateData, _MyFHT59N3Par.CalibrationType)

            If (_MyFHT59N3Par.GenerateAnsiN4242) Then
                'lädt NAH.cnf bzw. NAH_MISCH.cnf um die FWHN/Energie/Effizienzkoeffizienten auszulesen...
                persistSpectrumAnsiN4242.SYS_StoreCalibrationDataAnsiN4242(templateData, _MyFHT59N3Par.CalibrationType)
                persistSpectrumAnsiN4242.SYS_WriteTemplateFileAnsiN4242(templateData, SpecType)
            End If

            CopySpectrumIfAlarmed(Source, SpecType)

            AdjustTc99()

            'Messergebnisse ausgeben
            Dim DestFile = persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_RESULTS, Source, SpecType, "")


            If SpecType = SPECTYPE_SONDERAUSWERTUNG Then
                GUI_OpenSpectrum(Source, SpectraTypes.OFFLINE, True, False, frmMeasScreen.SpectralDisplay)
            End If

            If SpecType = SPECTYPE_FILTERSTAND Or SpecType = SPECTYPE_TAGESAUSWERTUNG Then
                _MyControlCenter.MDS_SaveAnalyzationResultsToDataServer(_MyControlCenter.MCA_Nuclides, _MyControlCenter.SYS_States, _AirFlowMean, SpecType)
            End If

            'Füllt die Richtextbox
            GUI_ShowAnalyzationFile(DestFile, frmMeasScreen.RtbAnalyzeData)

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            GUI_CloseSpectrum(False, False)
            If Not quellBackCnf Is Nothing Then
                quellBackCnf.Close()
            End If
        End Try
    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <param name="SpecType"></param>
    ''' <param name="ELIVEEREAL"></param>
    ''' <remarks></remarks>
    Private Sub LoadAndShowSpectrumFile(ByVal Source As String, ByVal SpecType As Integer, ByRef ELIVEEREAL As Boolean)
        '##############################
        'Parameter aus Spektrum abholen
        '##############################

        GUI_OpenSpectrum(Source, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)

        _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_SQUANT) = 1         'aus CAM_CLS_SAMP
        If SpecType = SPECTYPE_ALARMPRUEFUNG Then
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3) = 1         'Anzal addirter Spektren
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR4) = _SSPRSTR4
        End If
        _ELIVE = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE)        'aus CAM_CLS_ACQP
        _EREAL = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL)
        If _ELIVE = 0 Then        'das gab's: EREAL>0, ELIVE=0 !!!!
            ELIVEEREAL = True
            _ELIVE = _EREAL
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL)
        End If
        _ASP2 = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP2)         'Luftdurchsaz
        _ASP3 = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3)         'Anzal addirter Spektren
        _ASP4 = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP4)         'ASP4=1 dann wi STD, =0 dann wi TAG
        _ASPSTR = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_ASPSTR)      'Messstartzait: jjmmttssnn
        _SSPRSTR2 = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR2)  'Messstartzait für tagS: tt.mm.jj ss:nn
        _SSPRSTR3 = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR3)  'Messstartzait für stdS: tt/mm/jj ss:nn
        _SSPRSTR4 = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR4)  'Messendezait:  tt.mm.jj ss:nn
        _SpecCustomer = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC1)  'Customer aus Spektrum
        _SpecStationName = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SLOCTN)  'Stationname aus Spektrum


        'weder tagS noch stdS
        Dim isNoDayOrIntermediateMeasurement = (_ASP4 <> 0 And _ASP4 <> 1)
        Dim couldBeCalibrationSpectrum = (_ASP3 = 0)
        Dim noMeasurementTime = _ELIVE = 0

        'Spektrum vermutlich nicht waehrend Bestaubung gemessen...
        If isNoDayOrIntermediateMeasurement Or couldBeCalibrationSpectrum Or noMeasurementTime Then
            'GUI_CloseSpectrum(False, False)
            Exit Sub
        End If
    End Sub


    Private Sub SetSpectrumAnalysisParameters(ByVal SpecType As Integer, ByRef DestinationReport As String,
                                              ByRef rettELIVE As Double, ByRef rettEREAL As Double)
        '####################################
        'Parameter für die Bearbeitung setzen
        '####################################

        Select Case SpecType

            Case SPECTYPE_SONDERAUSWERTUNG                            'Sonderauswertung
                If _ASP4 = 0 Then 'wi TAG.CNF: Messdauern gemittelt, Luftdurchsaz blaibt
                    rettELIVE = _ELIVE                                 'Tagesspektrum
                    rettEREAL = _EREAL

                    If _ASP3 = 0.0 Then
                        _ASP3 = 1
                    End If

                    _ELIVE = _ELIVE / _ASP3         'gemittelte Messdauer
                    _EREAL = _EREAL / _ASP3         'Summenluftdurchsaz blaibt
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = _ELIVE
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = _EREAL
                    _SpectraFile.Flush()
                Else     'wi STD.CNF: elive und ereal blaiben, Luftdurchsaz gemittelt
                    _ASP2 = _ASP2 / _ASP3         'gemittelter Luftdurchsaz, Messdauer blaibt
                    _ASP3 = 1                    'so tun, als obb auf ainmal gemessen
                End If   'if-end asp4# = 0#

            Case SPECTYPE_TAGESAUSWERTUNG                            'Tagesspektrum
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STYPE) = ml_string(170, "Spectrum of sum of day")
                rettELIVE = _ELIVE
                rettEREAL = _EREAL
                _ELIVE = _ELIVE / _ASP3         'gemittelte Messdauer
                _EREAL = _EREAL / _ASP3         'Summenluftdurchsaz blaibt
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = _ELIVE  'rettELIVE wird nach Auswertung zurückgeschriben
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = _EREAL
                _SpectraFile.Flush()
                DestinationReport = _ReportFilesDirectory & "\" & "T_" & Format(Now, "MM") & Format(Now, "dd") & Format(Now, "HH") & ".rpt"    'Linien

            Case SPECTYPE_BISHERIGER_FILTERSTAND, SPECTYPE_FILTERSTAND                         'Filterstanddauer
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STYPE) = ml_string(171, "Spectrum of dustation period")
                _ASP2 = _ASP2 / _ASP3         'gemittelter Luftdurchsaz, Messdauer blaibt
                _ASP3 = 1                    'so tun, als obb auf ainmal gemessen
                If SpecType = 1 Then
                    DestinationReport = _ReportFilesDirectory & "\" & "S_" & Format(Now, "dd") & Format(Now, "HH") & ".rpt"  'Linien
                Else
                    DestinationReport = _ReportFilesDirectory & "\" & "B_" & Format(Now, "dd") & Format(Now, "HH") & ".rpt"  'Linien
                End If

            Case SPECTYPE_ALARMPRUEFUNG
                _ASP2 = _AirFlowMean               'gemittelter Luftdurchsaz

            Case Else
                'NOP
        End Select

        _ELIVEHour = _ELIVE / 3600
        _AirFlowMean = _ASP2                        'gemittelter Luftdurchsatz

    End Sub




    Public Function CalculateNuclidEnergyAndPeaks(ByVal Source As String, ByVal SpecType As Integer, ByRef rettELIVE As Double,
                                          ByRef rettEREAL As Double)




        Dim NuclideChannel(10) As CanberraDataAccessLib.ParamCodes
        NuclideChannel(0) = CanberraDataAccessLib.ParamCodes.CAM_L_NLNUCL   'Nuklidnr in Nukliddatei
        NuclideChannel(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY 'Energi der Linie
        NuclideChannel(2) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCENTRD 'Kanallage
        NuclideChannel(3) = CanberraDataAccessLib.ParamCodes.CAM_F_PSAREA   'Nettofläche
        NuclideChannel(4) = CanberraDataAccessLib.ParamCodes.CAM_F_PSFWHM   'Fwhm
        NuclideChannel(5) = CanberraDataAccessLib.ParamCodes.CAM_L_NLFKEYLINE  '1=Clüssellinie, 0=sonst
        'zum testen
        NuclideChannel(6) = CanberraDataAccessLib.ParamCodes.CAM_F_NLMDANET  'netto counts 
        'Additional Fields: 
        NuclideChannel(7) = CanberraDataAccessLib.ParamCodes.CAM_F_PSDAREA   'Error on Area
        NuclideChannel(8) = CanberraDataAccessLib.ParamCodes.CAM_F_PSBACKGND 'Backgrond
        NuclideChannel(9) = CanberraDataAccessLib.ParamCodes.CAM_F_PSDBACK   'Error on Background
        NuclideChannel(10) = CanberraDataAccessLib.ParamCodes.CAM_L_NLFNOUSEWTM    'Don't use the line in weighted average?


        ' CAM_F_NLMDANETERR

        _MyControlCenter.MCA_Peaks.PeakList.Clear()

        Dim NumberOfPeaks As Integer = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
        For idx As Integer = 1 To NumberOfPeaks
            Dim ParamBuffer = _SpectraFile.ParamArray(NuclideChannel, idx)
            Dim NuclideNumber = CType(ParamBuffer(0), Integer) 'Irgendeine komische Nuklidnr
            Dim PeakEnergy = CType(ParamBuffer(1), Double) 'Energie der Linie
            Dim PeakChannel As Integer = CType(ParamBuffer(2), Integer) 'Kanallage
            Dim PeakArea = CType(ParamBuffer(3), Double) 'Nettofläche
            Dim PeakFwhm = CType(ParamBuffer(4), Double) 'Fwhm
            Dim IsKeyLine = CType(ParamBuffer(5), Integer) 'ist die Schlüssellinie?
            Dim GrossCounts = CType(ParamBuffer(6), Integer) 'Gross counts
            Dim PeakAreaErr = CType(ParamBuffer(7), Double)  'Error on Area
            Dim PeakBckg = CType(ParamBuffer(8), Double)  'Backgrond
            Dim PeakBckgErr = CType(ParamBuffer(9), Double)  'Error on Backgrond
            Dim PeakUseWtm = Not (CType(ParamBuffer(10), Boolean))  'Use the line in weighted average?

            Dim peak As FHT59N3MCA_Peak = New FHT59N3MCA_Peak()
            peak.NuclideNumber = NuclideNumber

            peak.PeakEnergy = PeakEnergy
            peak.PeakChannel = PeakChannel
            peak.IsKeyLine = IsKeyLine
            peak.GrossCounts = GrossCounts

            peak.PeakArea = PeakArea
            peak.PeakFwhm = PeakFwhm

            peak.PeakAreaErr = PeakAreaErr
            peak.PeakBckg = PeakBckg
            peak.PeakBckgErr = PeakBckgErr
            'peak.PeakRChiSq = PeakRChiSq

            peak.UseWtm = PeakUseWtm


            _MyControlCenter.MCA_Peaks.PeakList.Add(peak)
        Next

        Dim NuclideParameter(3) As CanberraDataAccessLib.ParamCodes
        NuclideParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_L_NLPEAK      '0=Linie nicht zugeordnet bzw. zurüchgewisen, >0 ferwendet
        NuclideParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_L_NLNUCL      'Nuklidnr inn Nukliddatai, di der Linie NLPEAK zugeordnet
        NuclideParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_L_NLFKEYLINE  '1=Clüssellinie, 0=sonst
        NuclideParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_F_NLENERGY    'Energilage

        Dim NumberOfRecords As Integer = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_NLINES)
        For idx As Integer = 1 To NumberOfRecords            'prüft, obb Clüssellinien für Identifizirung benuzt

            Dim ParamBuffer = _SpectraFile.ParamArray(NuclideParameter, idx)
            Dim PeakLineIdentified = CType(ParamBuffer(0), Integer)      'Liniennr
            Dim NuclideNumber = CType(ParamBuffer(1), Integer)   'Nuklidnr
            Dim KeyLine = CType(ParamBuffer(2), Integer)         'Clüssellinienmerker
            Dim Energy = CType(ParamBuffer(3), Integer)

            Dim nucl As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(NuclideNumber)

            If Not IsNothing(nucl) Then
                If KeyLine > 0 And PeakLineIdentified > 0 Then
                    nucl.SpectrumAnalysis.KeyLineFound = True
                    nucl.SpectrumAnalysis.KeyLineEnergy = Energy
                End If
            End If

        Next idx

        NumberOfRecords = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_NUCL)

        If NumberOfRecords = 0 Then
            persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, MSG_IdentifyingWithoutLines & vbCrLf)             '3.Infozaile: inn Identifizirung abbgebrochen, one Linien

            Dim DestFile = persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_STATE_INFOS, Source, SpecType, "") 'Status melden  'Fatalstatenausgabe als Info-Zailen
            GUI_ShowAnalyzationFile(DestFile, frmMeasScreen.RtbAnalyzeData)
            GUI_SetMessage(MSG_IdentifyingWithoutLines, MessageStates.YELLOW)   'changed by TFHN-15

            _MyControlCenter.SYS_States.AnalyzationCancelled = True

            If (SpecType = SPECTYPE_TAGESAUSWERTUNG) Or (SpecType = SPECTYPE_SONDERAUSWERTUNG And _ASP4 = 0) Then
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = rettELIVE
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = rettEREAL
            End If
            GUI_CloseSpectrum(True, False)

        End If

        Return (NumberOfRecords = 0)

    End Function

    '----------------------------------------------------------------------
    'folge: Berechnung des Faktors für den Zerfall zwaier Folgenuklide
    'folg3: Berechnung des Faktors für den Zerfall inns 3. Folgenuklid
    '----------------------------------------------------------------------
    Private Sub MCA_ComputeFactorSuccessorNuclide3(ByVal n1 As String, ByVal n2 As String, Optional ByVal n3 As String = Nothing)
        Try
            Dim exp1, exp2, exp3, decayTime1, decayTime2, decayTime3, zf1, zf2, zf3 As Double

            'Vorbereitung
            Dim nucl1 As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n1)
            decayTime1 = nucl1.Library.DecayConstant * _EREALHour
            exp1 = 1 - Math.Exp(-decayTime1)

            Dim nucl2 As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n2)
            decayTime2 = nucl2.Library.DecayConstant * _EREALHour
            exp2 = 1 - Math.Exp(-decayTime2)

            Dim nucl3 As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n3)
            If Not IsNothing(n3) Then
                decayTime3 = nucl3.Library.DecayConstant * _EREALHour
                exp3 = 1 - Math.Exp(-decayTime3)
            End If

            'Berechnungen
            zf1 = decayTime1 * (1 - exp2 / decayTime2) / decayTime2
            zf2 = 1 - exp1 / decayTime1 - exp2 / decayTime2 - (exp2 - exp1) / (decayTime1 - decayTime2)
            nucl2.SpectrumAnalysis.NuclideCorrectionFactor = zf1 / (zf1 + zf2)

            If Not IsNothing(n3) Then

                zf3 = ((exp3 - exp2) / (decayTime3 - decayTime2) + (exp3 - exp1) / (decayTime1 - decayTime3)) * decayTime2 / (decayTime2 - decayTime1) + (exp3 - exp2) / (decayTime2 - decayTime3)
                zf3 = 1 - zf3 - (exp2 - exp1) / (decayTime1 - decayTime2) - exp3 / decayTime3 - exp2 / decayTime2 - exp1 / decayTime1
                zf1 = decayTime1 * (1 - exp3 / decayTime3) / decayTime3
                zf2 = decayTime1 * (1 - exp2 / decayTime2 - exp3 / decayTime3 - (exp3 - exp2) / (decayTime2 - decayTime3)) / decayTime2
                nucl3.SpectrumAnalysis.NuclideCorrectionFactor = zf1 / (zf1 + zf2 + zf3)
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub CheckAirFlowBoundaries(ByVal Source As String, ByVal SpecType As Integer)

        If _AirFlowMean < _MyFHT59N3Par.MinAirFlowAlert Then
            _MyControlCenter.SYS_States.AirFlowLessThen1Cubic = True 'Luftdurchsatz zu klein
            _AirFlowMean = _AirFlowMeanThreshold * 1.5                      'Ersazwert
            persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, Source & ml_string(173, " Air flow (<<) set to ") & Format(_AirFlowMean, "0.0") & " m3/h." & vbCrLf)
        Else
            _MyControlCenter.SYS_States.AirFlowLessThen1Cubic = False      'Luftdurchsatz verglichen mit Grenzwert (früher: 1)
        End If    'if-end mitluft#<1#

        'Obergrenze nicht mehr prüfen für Spektrumauswertung da nicht relevant (wenn nicht durch Anzahl von Spektren kalkuliert dann führt das 
        'zu falschen Ergebnissen in Tagesspektrum-Auswertung da durch Ersatzwert ein viel zu niedriger Luftdurchsatz eingeht)

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EnsureNoK40StateOnWaitTime()

        If _EREAL > _WaitTimeBeforeK40Check Then '1800 Then 'SpecType > 0 And
            _MyControlCenter.SYS_States.K40ToLow_NotFound = False 'K40-Innhalt nicht überprüfen wenn Alarmprüfung Neu in V 2.1.60
            _MyControlCenter.SYS_States.K40ToBig = False 'K40-hwb nicht überprüfen wenn Alarmprüfung Neu in V 2.1.60
        End If
    End Sub



    Private Function PerformK40Search(ByRef asfWerter As CanberraSequenceAnalyzerLib.SequenceAnalyzer,
                                 ByVal Source As String, ByVal SpecType As Integer,
                                 ByRef rettELIVE As Double, ByRef rettEREAL As Double)

        Dim fernCnf As New CanberraDataAccessLib.DataAccess

        Dim ECOFFSET As Double
        Dim ECSLOPE As Double
        Dim ECQUAD As Double
        Dim ECALFAC1 As Double
        Dim Buffer As Object            'speicher zur übernahme von ParamArray-satz
        Dim kLage As Double
        Dim nettoFl As Double
        Dim fwhm As Double
        Dim k40Area As Double
        Dim k40Areah As Double
        Dim k40fwhm As String = "0"
        Dim k40Channel As Double
        Dim k40Diff As Double
        Dim k40Drift As Double
        Dim k40AreaProz As Double
        'Dim ChannelNumber As Integer
        'Dim NuclideNumber As Integer
        'Dim KeyLine As Integer
        Dim Energy As Double
        Dim NumberOfRecords As Integer
        Dim MaxK40Failure As Double
        Dim i
        Dim NuclideParameter(0 To 3) As CanberraDataAccessLib.ParamCodes  '0=Energi, 1=Kanallage, 2=Nettofläche, 3=Fwhm

        'Globalen Wert löschen damit auffällig wird wenn der Wert nicht korrekt berechnet werden konnte
        _K40_FWHN = 0
        _K40AREA_PER_HOUR_ABS = 0
        _K40AREA_PER_HOUR_PERCENT = 0
        _K40AREA_DRIFT = 0

        'K-40-Liniensuche imm Beraich 2820 biss 3020
        asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN0.ASF", , , , , , ) ' FFkaAnzaige.RepFenst)

        'Bestimmung fon k40abw, k40hbw und k40inh
        If _EREAL > _WaitTimeBeforeK40Check Then '1800 Then 'SpecType > 0 And

            _MyControlCenter.SYS_States.K40ToLow_NotFound = True
            MaxK40Failure = 19.9        'max. erlaubter k-40-Linien-Fersaz
            NumberOfRecords = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)

            If NumberOfRecords = 0 Then
                _MyControlCenter.SYS_States.AnalyzationCancelled = True
                persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, Format(SpecType, "0") & " " & MSG_K40NotFound & vbCrLf)
                Dim DestFile = persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_STATE_INFOS, Source, SpecType, "") 'Status melden  'Fatalstatenausgabe als Info-Zailen
                GUI_ShowAnalyzationFile(DestFile, frmMeasScreen.RtbAnalyzeData)

                GUI_SetMessage(MSG_K40NotFound, MessageStates.YELLOW)

                If (SpecType = SPECTYPE_TAGESAUSWERTUNG) Or (SpecType = SPECTYPE_SONDERAUSWERTUNG And _ASP4 = 0) Then
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = rettELIVE
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = rettEREAL
                End If
                'Spektrum wird im Aufrufer geschlossen
                Return True
            Else
                NuclideParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY 'Energi der Linie
                NuclideParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCENTRD 'Kanallage
                NuclideParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_F_PSAREA   'Nettofläche
                NuclideParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_F_PSFWHM   'Fwhm

                For i = 1 To NumberOfRecords
                    Buffer = _SpectraFile.ParamArray(NuclideParameter, i)
                    Energy = CType(Buffer(0), Double)     'Energi
                    kLage = CType(Buffer(1), Double)     'Kanallage
                    nettoFl = CType(Buffer(2), Double)    'Nettofläche
                    fwhm = CType(Buffer(3), Double)       'Fwhm

                    'K40MinArea ist pro Stunde gerechnet!
                    If (nettoFl - _ELIVEHour * _ASP3 * _MyFHT59N3Par.K40MinArea) > 0 Then
                        k40Diff = Energy - 1460.81
                        If Math.Abs(k40Diff) < Math.Abs(MaxK40Failure) Then
                            _MyControlCenter.SYS_States.K40ToLow_NotFound = False
                            k40Area = nettoFl
                            k40Channel = kLage

                            'global zwischenspeichern damit Dateiausgabe (z.B. ANSI 4242) nochmals
                            'darauf zugreifen kann
                            _K40_FWHN = fwhm

                            k40fwhm = Format(fwhm, "0.00")
                            If Len(k40fwhm) > 4 Then
                                k40fwhm = Left(k40fwhm, 4)
                            End If
                            MaxK40Failure = k40Diff
                        End If
                    End If
                Next i

                If Not _MyControlCenter.SYS_States.K40ToLow_NotFound Then
                    k40Areah = k40Area / _ELIVEHour / _ASP3

                    If SpecType < SPECTYPE_SONDERAUSWERTUNG Then
                        'Neu in V. 2.1.60
                        fernCnf.Open(_fernS, CanberraDataAccessLib.OpenMode.dReadOnly)
                        ECOFFSET = fernCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET)
                        ECSLOPE = fernCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE)
                        ECQUAD = fernCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD)
                        ECALFAC1 = fernCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECALFAC1)
                        fernCnf.Close()
                        k40Drift = ECOFFSET + ECSLOPE * k40Channel + ECQUAD * k40Channel ^ 2 + ECALFAC1 * k40Channel ^ 3 - 1460.81
                        k40AreaProz = k40Areah * 100.0 / _MyFHT59N3Par.K40MinArea

                        _K40AREA_PER_HOUR_ABS = k40Areah
                        _K40AREA_PER_HOUR_PERCENT = k40AreaProz
                        _K40AREA_DRIFT = k40Drift

                        If _MyFHT59N3Par.K40FWHM < CSng(k40fwhm) Then
                            _MyControlCenter.SYS_States.K40ToBig = True 'K40 zu brait
                        Else
                            _MyControlCenter.SYS_States.K40ToBig = False 'K40 zu brait
                        End If
                        persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, ml_string(174, "K40-FWHM: ") & k40fwhm & ml_string(175, " keV  -DIFF(NAH): ") & Format(k40Drift, "0.0") & ml_string(176, " keV  -AREA/h: ") & Format(k40Areah, "0.0") & " = " & Format(k40AreaProz, "0.0") & " %" & vbCrLf)
                    Else     'merker=4
                        persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, ml_string(177, "K40-FWHM: ") & k40fwhm & ml_string(178, " keV  -AREA/h: ") & Format(k40Areah, "0.0") & vbCrLf)
                    End If   'if-end merker% < 4
                    If Math.Abs(MaxK40Failure) > 0.5 And Math.Abs(MaxK40Failure) < 20 Then
                        If _ReCalibrateK40LineIfNeeded Then
                            MCA_ReCalibrateK40(SpecType, MaxK40Failure)
                        End If
                    ElseIf Math.Abs(MaxK40Failure) > 19.9 Then
                        _MyControlCenter.SYS_States.AnalyzationCancelled = True  'Abwaichung zu gros
                        persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, Format(SpecType, "0") & " " & MSG_K40DevToBig & vbCrLf)
                        Dim DestFile = persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_STATE_INFOS, Source, SpecType, "") 'Status melden  'Fatalstatenausgabe als Info-Zailen
                        GUI_ShowAnalyzationFile(DestFile, frmMeasScreen.RtbAnalyzeData)
                        GUI_SetMessage(MSG_K40DevToBig, MessageStates.YELLOW)

                        If (SpecType = SPECTYPE_TAGESAUSWERTUNG) Or (SpecType = SPECTYPE_SONDERAUSWERTUNG And _ASP4 = 0) Then
                            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = rettELIVE
                            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = rettEREAL
                        End If
                        'Spektrum wird im Aufrufer geschlossen
                        Return True
                    End If  'if-end ABS(k40abw)>0.5 & ABS(k40abw)<20
                Else      'gStatus(2)<>0
                    _MyControlCenter.SYS_States.AnalyzationCancelled = True
                    persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, Format(SpecType, "0") & " " & MSG_K40NotFound & vbCrLf)
                    Dim DestFile = persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_STATE_INFOS, Source, SpecType, "") 'Status melden  'Fatalstatenausgabe als Info-Zailen
                    GUI_ShowAnalyzationFile(DestFile, frmMeasScreen.RtbAnalyzeData)
                    GUI_SetMessage(MSG_K40NotFound, MessageStates.YELLOW)

                    If (SpecType = SPECTYPE_TAGESAUSWERTUNG) Or (SpecType = SPECTYPE_SONDERAUSWERTUNG And _ASP4 = 0) Then
                        _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = rettELIVE
                        _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = rettEREAL
                    End If
                    'Spektrum wird im Aufrufer geschlossen
                    Return True
                End If    'If-end gStatus(2) = 0
            End If      'If-end NumberOfRecords = 0
        End If        'If-end merker% > 0

        'wir machen in der Verarbeitung weiter
        Return False

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="asfWerter"></param>
    ''' <param name="Source"></param>
    ''' <param name="SpecType"></param>
    ''' <param name="DestinationReport"></param>
    ''' <remarks></remarks>
    Private Sub AnalyseFullSpectrum(ByRef asfWerter As CanberraSequenceAnalyzerLib.SequenceAnalyzer,
                                    ByVal Source As String, ByVal SpecType As Integer, ByRef DestinationReport As String)

        'Liniensuche inn foller Braite + Identifizirung
        If SpecType = SPECTYPE_SONDERAUSWERTUNG Then
            Dim b As String
            Dim i As Integer = InStr(Source, ".")
            b = Left$(Source, i)
            Do
                i = InStr(b, "\")
                If i = 0 Then Exit Do
                b = Right$(b, Len(b) - i)
            Loop
            Dim DestinationReport1 As String = _ReportFilesDirectory & "\" & b$ & "rpt"
            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN1.ASF", , , , , DestinationReport1, ) ' FFkaAnzaige.RepFenst)
            For i = 5 To Len(Source)
                If Left$(Right$(Source, i), 1) = "\" Then Exit For
            Next i
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_DETNAME) = Right$(Source, i - 1)
        Else
            If SpecType > SPECTYPE_ALARMPRUEFUNG And SpecType < SPECTYPE_SONDERAUSWERTUNG Then
                If System.IO.File.Exists(DestinationReport) Then
                    System.IO.File.Delete(DestinationReport) 'gibt es Ergebniss.rpt, dann zu löschen
                End If
            End If
            If SpecType > 0 Then    'für std.cnf und tag.cnf
                asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN1.ASF", , , , , DestinationReport, ) ' FFkaAnzaige.RepFenst)
            Else                   'für AP_
                asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN2.ASF", , , , , , ) ' FFkaAnzaige.RepFenst)
            End If
        End If    'If-end merker% = 4


    End Sub

    Private Sub PerformRecalibrateNaturalLinesIfNecessary(ByVal SpecType As Integer, ByRef DestinationReport As String)

        Dim NumberOfRecords As Integer = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK) 'lohnt sich der Aufwand überhaupt?
        If NumberOfRecords > 6 Then
            'Nachkalibreirung nicht bei Alarmprüfung, frühestens nach 1/3-filterstanddauer, danach alle 30 min
            If ((SpecType = SPECTYPE_SONDERAUSWERTUNG) And (_EREAL / 60 > _MyFHT59N3Par.FilterTimeh * 20)) Or
                ((SpecType > SPECTYPE_ALARMPRUEFUNG) And (_EREAL / 60 > _MyFHT59N3Par.FilterTimeh * 20) And (_EREAL - _EREALOld > 1790)) Then 'nach Filterstanddauer
                If _ReCalibrateEnergyIfNeeded Then
                    MCA_ReCalibrateEnergyWithNaturalLines(SpecType, DestinationReport)
                End If
            End If
        End If

    End Sub

    Private Function AlertAnalysisWithoutLines(ByVal Source As String, ByVal SpecType As Integer, ByRef rettELIVE As Double, ByRef rettEREAL As Double)

        Dim NumberOfRecords = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK) 'lohnt sich der Aufwand überhaupt?
        If NumberOfRecords = 0 Then

            persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, MSG_IdentifyingWithoutLines & vbCrLf)             '3.Infozaile: inn Identifizirung abbgebrochen, one Linien
            Dim DestFile As String = persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_STATE_INFOS, Source, SpecType, "") 'Status melden  'Fatalstatenausgabe als Info-Zailen
            GUI_ShowAnalyzationFile(DestFile, frmMeasScreen.RtbAnalyzeData)
            GUI_SetMessage(MSG_IdentifyingWithoutLines, MessageStates.YELLOW)  'changed by TFHN-15
            _MyControlCenter.SYS_States.AnalyzationCancelled = True

            If (SpecType = SPECTYPE_TAGESAUSWERTUNG) Or (SpecType = SPECTYPE_SONDERAUSWERTUNG And _ASP4 = 0) Then
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = rettELIVE
                _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = rettEREAL
            End If
            GUI_CloseSpectrum(True, False)

            Return True
        End If

        Return False

    End Function

    Private Sub PrepareSubtractBackground(ByRef asfWerter As CanberraSequenceAnalyzerLib.SequenceAnalyzer,
                                          ByRef quellBackCnf As CanberraDataAccessLib.DataAccess)

        Dim i
        Dim Help
        Dim peakzaehl

        'Untergrund
        'Dim help As Double       'Hilfsvariable
        'Dim peakzaehl As Double  'Peak-Nettofläche
        Dim backCnf As String    'Backgroundspektren


        '### - Code für Background -> auf Wunsch des DWD Neu in V2157 'nicht vor der K40 suche, sonst ist die weg nach dem Untergrund und wir produzieren 'einen Fehler
        If _SubtractBkgSpectrum Then

            'Peaksuche mit Untergrundspektrum ausführen
            backCnf = _SpectraDirectory & "\BACKGND.CNF"
            quellBackCnf.Open(backCnf, CanberraDataAccessLib.OpenMode.dReadWrite)
            asfWerter.Analyze(quellBackCnf, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN1.asf", , , , , , ) 'FFkaAnzaige.RepFenst)

            'Im Untergrundspektrum Peakflächen mit Anzahl aufsummierter Spektren multiplizieren
            Dim NumberOfRecords As Integer = quellBackCnf.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK) 'peaks
            For i = 1 To NumberOfRecords
                Help = quellBackCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_PSORIGAREA, i)
                peakzaehl = Help * _ASP3
                quellBackCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_PSORIGAREA, i) = peakzaehl
            Next i

            quellBackCnf.Flush()
            quellBackCnf.Close()
            quellBackCnf = Nothing

        End If
        '### - Ende

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="asfWerter"></param>
    ''' <remarks></remarks>
    Private Sub IdentifyLines(ByRef asfWerter As CanberraSequenceAnalyzerLib.SequenceAnalyzer)


        If (_SpectraFile Is Nothing) Then
            MessageBox.Show("_SpectraFile object in IdentifyLines() is nothing", "Fatal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Trace.TraceInformation("TRACE: Performing analysis on spectrum '" + _SpectraFile.Name + "'")
        End If

        'TODO: remove this part after finding the problem !!!!!
        Dim doesFileExist As Boolean = System.IO.Directory.Exists(_MyControlCenter.MCA_CtlFiles)
        If (Not doesFileExist) Then
            MessageBox.Show("control directry " + _MyControlCenter.MCA_CtlFiles + " does not exist")
        End If

        doesFileExist = System.IO.File.Exists(_MyControlCenter.MCA_CtlFiles & "\EBINLIN3_BACK.ASF")
        If (Not doesFileExist) Then
            MessageBox.Show("control file " + "\EBINLIN3_BACK.ASF" + " does not exist")
        End If





        'Identifizierung
        If _SubtractBkgSpectrum Then 'Untergrund V 2.1.57
            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN3_BACK.ASF", , , , , , ) ' FFkaAnzaige.RepFenst)
        Else
            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN3.ASF", , , , , , ) ' FFkaAnzaige.RepFenst)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <param name="SpecType"></param>
    ''' <remarks></remarks>
    Private Sub WriteFatalStatus(ByVal Source As String, ByVal SpecType As Integer)
        If SpecType > SPECTYPE_ALARMPRUEFUNG Then
            If SpecType < SPECTYPE_TAGESAUSWERTUNG Then
                persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_STATE_INFOS, Source, SpecType, "") 'Status melden  'Fatalstatenausgabe als Info-Zailen
            Else
                If _MyControlCenter.SYS_States.K40ToLow_NotFound Then
                    persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, ml_string(179, "K40-peak too small") & vbCrLf) 'K40-Linie zu klain
                End If
            End If
        End If        'If-end merker% > 0
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FlushNuclidParametersEmpty()

        For n As Integer = 1 To _MyControlCenter.MCA_Nuclides.NuclideCount                  'Nuklidanzal inn Datai

            Dim nuclide As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(n)
            nuclide.SpectrumAnalysis.NuclideCorrectionFactor = 1
            nuclide.SpectrumAnalysis.KeyLineFound = 0
            nuclide.SpectrumAnalysis.Concentration_Bqm3 = 0

        Next n%

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetNuclideCorrectionFactor()

        MCA_ComputeFactorSuccessorNuclide3("SR-92", "Y-92") 'SR-92,Y-92
        MCA_ComputeFactorSuccessorNuclide3("ZR-97", "NB-97") 'ZR-97,NB-97
        MCA_ComputeFactorSuccessorNuclide3("MO-99", "TC-99M") 'MO-99,TC-99M
        MCA_ComputeFactorSuccessorNuclide3("TE-132", "I-132") 'TE-132,I-132
        MCA_ComputeFactorSuccessorNuclide3("PB-212", "BI-212", "TL-208") 'PB-212,BI-212,TL-208
        MCA_ComputeFactorSuccessorNuclide3("PO-218", "PB-214", "BI-214") 'PO-218,PB-214,BI-214

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAlarmNuclidThresholds()

        For n As Integer = 1 To _MyControlCenter.MCA_AlarmNuclides.AlarmNuclideCount
            'Schwelle bai Messdauern < 2h fergrösern
            Dim currentNuclid As FHT59N3MCA_AlarmNuclide = _MyControlCenter.MCA_AlarmNuclides.Nuclide_ByNumber(n)

            If _ELIVE < 7000 Then
                currentNuclid.AlarmValue1WithHysterese = 7200 / _ELIVE * Math.Sqrt(7200 / _ELIVE) * currentNuclid.AlarmValue1
                currentNuclid.AlarmValue2WithHysterese = 7200 / _ELIVE * Math.Sqrt(7200 / _ELIVE) * currentNuclid.AlarmValue2
            Else
                currentNuclid.AlarmValue1WithHysterese = currentNuclid.AlarmValue1
                currentNuclid.AlarmValue2WithHysterese = currentNuclid.AlarmValue2
            End If


        Next n%
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateNuclidActivityAndDetectionLimit()
        Dim NuclideParameter(0 To 3) As CanberraDataAccessLib.ParamCodes  '0=Energi, 1=Kanallage, 2=Nettofläche, 3=Fwhm
        Dim NumberOfRecords As Integer = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_NUCL) 'NUCL
        Dim i

        NuclideParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_G_NCLWTMEAN
        NuclideParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_G_NCLWTMERR
        NuclideParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_G_NCLMDA
        NuclideParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_G_NCLDECAY  'überschraibt lediglich CAM_F_PSFWHM, wird nicht benötigt
        For i = 1 To NumberOfRecords          'holt Säze
            Dim nuclide As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(i)

            Dim Buffer = _SpectraFile.ParamArray(NuclideParameter, i)

            nuclide.SpectrumAnalysis.Activity = CType(Buffer(0), Double)
            nuclide.SpectrumAnalysis.DetectError = CType(Buffer(1), Double)
            nuclide.SpectrumAnalysis.DetectionLimit = CType(Buffer(2), Double)
        Next i

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateConcentrationAndDetectLimit()

        Dim NumberOfRecords As Integer = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_NUCL) 'NUCL

        Dim zerfall As Double
        Dim umrech As Double

        For nuclidIdx As Integer = 1 To NumberOfRecords

            Dim nuclide As FHT59N3MCA_Nuclide = _MyControlCenter.MCA_Nuclides.GetNuclide(nuclidIdx)

            If nuclide.SpectrumAnalysis.Activity > 0 Then
                nuclide.SpectrumAnalysis.DetectionError_Percent = nuclide.SpectrumAnalysis.DetectError * 100 / nuclide.SpectrumAnalysis.Activity
                If nuclide.SpectrumAnalysis.DetectionError_Percent > 99.95 Then    'Nuklide mit mer als 100% Feler ignoriren
                    nuclide.SpectrumAnalysis.Activity = 0
                    nuclide.SpectrumAnalysis.DetectionError_Percent = 0
                    nuclide.SpectrumAnalysis.KeyLineFound = False
                End If
            End If
            'Umrechnungsfaktor für Umrechnung von µCi in Bq bestimmen
            If nuclide.Library.Name = "K-40" Then
                umrech = 37000
            Else
                Dim decayTime As Double = nuclide.Library.DecayConstant * _EREALHour
                zerfall = 1 - Math.Exp(-decayTime)
                If nuclide.Library.NuclideHalfLife > 80 Then
                    umrech = 2 * 37000 / (_AirFlowMean * _EREALHour)
                Else
                    umrech = 37000.0 * zerfall / (_AirFlowMean * (_EREALHour - zerfall / nuclide.Library.DecayConstant))
                End If
                If nuclide.Library.Name = "LA-140" Or nuclide.Library.Name = "TE-129" Then
                    umrech = umrech * 2 * (1 - zerfall / decayTime) / decayTime
                End If
            End If    'if-end nkln(n%)="K-40"

            Dim totalCorrectionFactor As Double = umrech * nuclide.SpectrumAnalysis.NuclideCorrectionFactor

            'Aktiwitätskonzentrazion nur dann zuwaisen, wenn Schlüssellinie gefunden wurde
            If nuclide.SpectrumAnalysis.KeyLineFound Then
                nuclide.SpectrumAnalysis.Concentration_Bqm3 = nuclide.SpectrumAnalysis.Activity * totalCorrectionFactor 'Umrechnung fon uCi/m3 nach Bq/m3
            Else
                nuclide.SpectrumAnalysis.Concentration_Bqm3 = 0
            End If
            nuclide.SpectrumAnalysis.DetectionLimit_Bqm3 = nuclide.SpectrumAnalysis.DetectionLimit * totalCorrectionFactor

        Next nuclidIdx

        'for-end n%=1 to anznuk%
    End Sub

    Private Sub PerformAlarmDetection()

        'dargestellte Liste immer neu aufbauen weil "leere" Nuklide in der Spektrumdatei nicht mehr enthalten sind wenn 
        SYS_RemoveAllAlarmNuclidesFromList()

        Dim NumberOfRecords As Integer = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_NUCL) 'NUCL

        Dim nuclideList As FHT59N3MCA_NuclideList = _MyControlCenter.MCA_Nuclides
        Dim alarmNuclides As List(Of FHT59N3MCA_AlarmNuclide) = _MyControlCenter.MCA_AlarmNuclides.Nuclides().ToList()

        'Marker für Summenstatus für Überschreitung der Nuklid-Alarme zurücksetzen
        nuclideList.AlarmMaxViolocatedLevel = 0

        'Jetzt die Alarmprüfung....
        For nuclidIdx As Integer = 1 To NumberOfRecords
            Dim nuclide As FHT59N3MCA_Nuclide = nuclideList.GetNuclide(nuclidIdx)

            'Marker für Überschreitung des Nuklid-Alarms auf jeden Fall zurücksetzen
            nuclide.SpectrumAnalysis.ExceededAlarmLevel = 0

            Dim alarmNuclid As FHT59N3MCA_AlarmNuclide = alarmNuclides.FirstOrDefault(Function(d)
                                                                                          Return nuclide.Library.Name = d.Name
                                                                                      End Function)
            Dim foundAlarmNuclid As Boolean = Not IsNothing(alarmNuclid)
            If foundAlarmNuclid Then

                nuclide.SpectrumAnalysis.Alarm1LimitCurrent = alarmNuclid.AlarmValue1WithHysterese
                nuclide.SpectrumAnalysis.Alarm1Limit = alarmNuclid.AlarmValue1

                nuclide.SpectrumAnalysis.Alarm2LimitCurrent = alarmNuclid.AlarmValue2WithHysterese
                nuclide.SpectrumAnalysis.Alarm2Limit = alarmNuclid.AlarmValue2

                If nuclide.SpectrumAnalysis.Concentration_Bqm3 > 0 Then

                    Dim traceTxt As String = String.Format("Alarm nuclid '{0}' checked: Concentration_Bqm3: {1} <-> Alarm1(with hysterese): {2}. Alarm1: {3}",
                                                           alarmNuclid.Name,
                                                           nuclide.SpectrumAnalysis.Concentration_Bqm3,
                                                           alarmNuclid.AlarmValue1WithHysterese,
                                                           alarmNuclid.AlarmValue1)

                    Trace.TraceError(traceTxt)


                    Dim exceededAlarmValue As Double = 0

                    If (nuclide.SpectrumAnalysis.Concentration_Bqm3 >= alarmNuclid.AlarmValue1WithHysterese) Then

                        nuclide.SpectrumAnalysis.SetMaxAlarmLevel(1)
                        nuclideList.SetGlobalMaxAlarmLevel(1)

                        exceededAlarmValue = alarmNuclid.AlarmValue1WithHysterese

                        'Wir schalten den bisherigen Alarm(modus) immer schon bei Überschreitung der Alarm1-Schwelle an.
                        'Dadurch sind wir kompatibel mit bisherigen Nutzern die nur Alarm1 kennen
                        If (Not _MyControlCenter.SYS_States.AlarmMode) Then
                            SYS_SetAlarmModeOn()
                        End If

                    End If
                    If (nuclide.SpectrumAnalysis.Concentration_Bqm3 >= alarmNuclid.AlarmValue2WithHysterese) Then

                        'Wenn Alarm1=Alarm2 ist dann wird Alarm2 nicht genutzt (Migrationsverhalten nach Einführung Alarm2-Schwelle). 
                        'Dann wird auch nur Überschreitung des Alarm1 angezeigt
                        Dim hasConfiguredAlarm2 As Boolean = alarmNuclid.AlarmValue2WithHysterese > alarmNuclid.AlarmValue1WithHysterese
                        If (hasConfiguredAlarm2) Then
                            nuclide.SpectrumAnalysis.SetMaxAlarmLevel(2)
                            nuclideList.SetGlobalMaxAlarmLevel(2)
                            exceededAlarmValue = alarmNuclid.AlarmValue2WithHysterese
                        End If
                    End If

                    If (nuclide.SpectrumAnalysis.ExceededAlarmLevel > 0) Then
                        SYS_AddAlarmNuclideToList(nuclide.Library.Name, nuclide.SpectrumAnalysis.ExceededAlarmLevel, exceededAlarmValue)
                    End If

                End If
            End If
        Next nuclidIdx

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SpecType"></param>
    ''' <remarks></remarks>
    Private Sub AlertPB214Recalibration(ByVal SpecType As Integer)

        Dim nuclNameBi214 As String = "BI-214"
        Dim nuclNamePb214 As String = "PB-214"

        If SpecType = SPECTYPE_TAGESAUSWERTUNG And _MyControlCenter.MCA_Nuclides.GetNuclide(nuclNameBi214).SpectrumAnalysis.Concentration_Bqm3 > 0 And
            _MyControlCenter.MCA_Nuclides.GetNuclide(nuclNamePb214).SpectrumAnalysis.Concentration_Bqm3 = 0 Then

            persistSpectrumTxt.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", SpecType, MSG_BI214Recalib & vbCrLf)
            GUI_SetMessage(MSG_BI214Recalib, MessageStates.YELLOW)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AdjustTc99()


        'ainen Messwert fon TC-99m nur dann anzaigen, wenn auch Mo-99 forhanden
        If _MyControlCenter.MCA_Nuclides.GetNuclide("MO-99").SpectrumAnalysis.Concentration_Bqm3 = 0 Then

            _MyControlCenter.MCA_Nuclides.GetNuclide("TC-99M").SpectrumAnalysis.Concentration_Bqm3 = 0
            _MyControlCenter.MCA_Nuclides.GetNuclide("TC-99M").SpectrumAnalysis.DetectionError_Percent = 0

        End If
    End Sub

    Private Sub CopySpectrumIfAlarmed(ByVal Source As String, ByVal SpecType As Integer)

        'V2.1.59 -> Änderungswunsch vom DWD Herr Naatz am 30.11.2007

        Dim alarmModeValid As Boolean = _MyControlCenter.SYS_States.AlarmMode

        If SpecType < SPECTYPE_TAGESAUSWERTUNG AndAlso alarmModeValid Then
            System.IO.File.Copy(Source, _SpectraDirectory & "\!!" & Format(Now, "MM") & Format(Now, "dd") & Format(Now, "HH") & Format(Now, "mm") & ".CNF", True)
        End If

    End Sub

End Class
