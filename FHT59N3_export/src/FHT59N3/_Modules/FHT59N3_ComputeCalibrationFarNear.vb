Module FHT59N3_ComputeCalibrationFarNear

    Public Function MCA_CalibrateEnergyGeometryFar() As Boolean
        Try
            'Energikalibrirung am Spektrum FERN.CNF
            'Kopi der Energi-koeff nach detebin und nah.cnf
            Dim asfWerter As New CanberraSequenceAnalyzerLib.SequenceAnalyzer
            Dim GroesE(0 To 1) As CanberraDataAccessLib.ParamCodes
            Dim Anz, bol, i, j, k, l, lRet As Integer
            Dim staig, konst As Double
            Dim Buffer As Object
            Dim ShellCommand As String
            Dim ShellArguments As String
            Dim energi() As Double
            Dim klage() As Double
            Dim kalLinie(0 To 4) As Double
            Dim IndicesList As New Dictionary(Of String, List(Of Integer))
            Dim MyNuclideList As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides)

            MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\EBIN.ctf", IndicesList)
            If MyNuclideList.Count = 0 Then
                Return False            'ebin.ctf ler oder felt
            End If
            _ActivityCS137_Far = MyNuclideList(IndicesList("CS-137")(0)).ActivitykBq
            _CertDate_Far = MyNuclideList(IndicesList("CS-137")(0)).CertificateDate

            If Not System.IO.File.Exists(_fernS) Then
                GUI_ShowMessageBox(ml_string(180, "Fern.cnf not found."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            GUI_CloseSpectrum(False, True)
            GUI_OpenSpectrum(_fernS, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)
            Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
            If Anz = 0 Then
                GUI_ShowMessageBox(ml_string(181, "No Lines in Spectrum FERN.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                GUI_CloseSpectrum(False, True)
                Return False
            End If
            ReDim energi(Anz + 1)
            ReDim klage(Anz + 1)
            GroesE(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
            GroesE(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCENTRD  'Kanallage
            For i = 1 To Anz
                Buffer = _SpectraFile.ParamArray(GroesE, i)
                energi(i) = Buffer(0)      'Energi
                klage(i) = Buffer(1)       'Kanallage
            Next i
            kalLinie(1) = 344.3        'keV fon 4 EU-152x-Linien
            kalLinie(2) = 778.9
            kalLinie(3) = 964.1
            kalLinie(4) = 1112.1
            bol = 0
            For i = 1 To Anz - 1
                For j = Anz To i + 6 Step -1
                    '            1286.2
                    staig = (1408 - 121.8) / (klage(j) - klage(i))
                    konst = 121.8 - staig * klage(i)
                    'If konst# > -5# And konst# < 25# Then 'Änderung auf Wunsch DWD für Version 2.1.57
                    If konst > -15 And konst < 25 Then
                        For k = 1 To 4
                            bol = 0
                            For l = 1 To Anz
                                If Math.Abs(kalLinie(k) - konst - klage(l) * staig) < 2.8 Then
                                    bol = 1
                                End If
                            Next l
                            If bol = 0 Then
                                Exit For
                            End If
                        Next k
                    End If
                    If bol Then
                        Exit For
                    End If
                Next j
                If bol Then
                    Exit For
                End If
            Next i
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_ECALTERMS) = 2
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET) = konst
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE) = staig
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD) = 0
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECALFAC1) = 0
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECALFAC2) = 0
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECALFAC3) = 0
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_ASTFBADCAL) = 0
            For i = 1 To Anz
                Buffer = _SpectraFile.ParamArray(GroesE, i)
                energi(i) = Buffer(0)     'Energi
                klage(i) = Buffer(1)      'Kanallage
                Buffer(0) = konst + staig * Buffer(1)
                _SpectraFile.ParamArray(GroesE, i) = Buffer
            Next i
            GUI_CloseSpectrum(True, True)
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\ECAL.exe"
            ShellArguments = _fernS & " /CERT=" & _CertificateDirectory & "\EBIN.CTF /NOTAIL /ORDER=2 /ETOL=2.8"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(182, "\ECAL -- fernS.cnf missglückt.! Rkkod: ") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\CALPLOT.exe"
            ShellArguments = _fernS & " /CAL=ENERGY /EDIT"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(183, "\CALPLOT -- fernS.cnf failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ' Energikoeffizienten nach detEbin und NAH.CNF kopiren
            GUI_OpenSpectrum(_fernS, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ECALTIME) = Now
            'Analüse mit Ausgabe "peakanalysis" nach fern.rpt (noie Datai) und auf Drucker
            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINKAL.ASF", , , , , _ReportFilesDirectory & "\Fern.rpt", ) 'FFkaAnzaige.RepFenst)
            'inn EBINKAL.ASF nur PEAKANALYSIS
            GUI_CloseSpectrum(False, True)
            SYS_SaveReportsToCollectionFile(_ReportFilesDirectory & "\Fern.rpt")
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\MOVEDATA.exe"
            ShellArguments = _fernS & " " & _naS & " /ECAL /OVERWRITE"     '& detEbin & " "  kopi des spektrums nach MIN.CNF
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                Trace.TraceInformation(ml_string(184, "\MOVEDATA - fern.cnf to nah.cnf failure.! Code:") & Format(lRet, "0"))
                GUI_ShowMessageBox(ml_string(184, "\MOVEDATA - fern.cnf to nah.cnf failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            _MyControlCenter.MCA_DisconnectFromDetector()
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\MOVEDATA.exe"
            ShellArguments = _fernS & " DET:EBIN /ECAL /OVERWRITE"     '& detEbin & " "  kopi des spektrums nach MIN.CNF
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            _MyControlCenter.MCA_ConnectToDetector()
            _MyControlCenter.MCA_SetDetectorParams(_MyFHT59N3Par.Customer, _MyFHT59N3Par.StationName, _MyFHT59N3Par.StationID)
            If lRet Then
                Trace.TraceInformation(ml_string(185, "\MOVEDATA - fern.cnf to DET:EBIN failure.! Code:") & Format(lRet, "0"))
                GUI_ShowMessageBox(ml_string(185, "\MOVEDATA - fern.cnf to DET:EBIN failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\REPORT.exe"
            ShellArguments = _fernS & " /TEMPLATE=" & _MyControlCenter.MCA_CtlFiles & "\ANALYS.TPL /SECTION=ENERGYCAL /NEWFILE"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(186, "\ENERG-REPORT -- fernS.rpt failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            SYS_SaveReportsToCollectionFile(_ReportFilesDirectory & "\Fern.rpt")

            'OpenSpectrum(fernS, SpectraTypes.OFFLINE, True)
            'ShowMessageBox(MSG_EnergyCalibOK, "OK", "", "", MYCOL_THERMOBLUE, Color.White)
            Return True

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function


    Public Function MCA_CalibrateEfficiencyGeometryFar() As Boolean
        Try
            'Effizienzkalibrirung inn Geometri FERN
            'Kopi fern.cnf nach fernkor.cnf,
            'inn fernkor Kopi der 1408-Linie als 1950-Linie
            'fernkor eff-kalibriren d.h. dort noie Energi- und fernEff-koeff
            Dim GroesF(0 To 2) As CanberraDataAccessLib.ParamCodes
            Dim CtfFile As New CanberraDataAccessLib.DataAccess
            Dim Anz As Integer
            Dim i As Integer
            Dim lRet&
            Dim ztfKor As String
            Dim energi() As Double
            Dim nettoFl() As Double
            Dim zRate() As Double
            Dim Buffer As Object
            Dim ShellCommand As String
            Dim ShellArguments As String

            'Holen der Mess- und Energiekalibrirdaten aus FERN.CNF
            If Not System.IO.File.Exists(_fernS) Then
                GUI_ShowMessageBox(ml_string(187, "Fern.cnf not found."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            GUI_CloseSpectrum(False, True)
            System.IO.File.Copy(_fernS, _fernkorS, True)
            'Suchen der 1408-keV-Linie inn fernkorCnf
            GUI_OpenSpectrum(_fernkorS, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)
            Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
            If Anz = 0 Then
                GUI_ShowMessageBox(ml_string(188, "No Lines in FERNKOR.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                GUI_CloseSpectrum(False, True)
                Return False
            End If
            ReDim energi(Anz + 1)
            ReDim nettoFl(Anz + 1)
            ReDim zRate(Anz + 1)
            GroesF(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
            GroesF(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSAREA    'Nettofläche
            GroesF(2) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCTSS    'Zälrate
            For i = 1 To Anz
                Buffer = _SpectraFile.ParamArray(GroesF, i%)
                energi(i) = Buffer(0)      'Energi
                nettoFl(i) = Buffer(1)     'Nettofläche
                zRate(i) = Buffer(2)       'Zälrate
                If Math.Abs(energi(i) - 1407.95) < 1.5 Then Exit For '1408-keV-Linie gefunden
            Next i
            If i > Anz Then
                GUI_ShowMessageBox(ml_string(189, "(1407.95keV+-1.5keV)-Line not found. Please check calibration spectrum FERN.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                GUI_CloseSpectrum(False, True)
                Return False
            End If

            '1408-keV-Liniensaz als 1950-keV-Liniensaz nach fernkorCnf schraiben
            _SpectraFile.InsertRecord(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK, Anz + 1)
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY, Anz + 1) = 1950
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_PSAREA, Anz + 1) = nettoFl(i) * _MyFHT59N3Par.Factor1950keV
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_PSCTSS, Anz + 1) = zRate(i) * _MyFHT59N3Par.Factor1950keV
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_DCALTIME) = Now
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SGEOMTRY) = "FERN"
            GUI_CloseSpectrum(True, True)

            ztfKor$ = _CertificateDirectory & "\EBINKOR.CTF"
            'EBINKOR.CTF wi ebin.ctf mitt 1950keV-Linie, deren Emissionswschlk = der der 1408keV glaicht
            'inn fernkor.cnf ist der 1950-keV-Linie Innhalt und Rate umm den Faktor fk1950# (<1!) ferklainert
            System.IO.File.Copy(_CertificateDirectory & "\EBIN.CTF", ztfKor$, True) 'in energiekalibirierung kann evtl. ebin.ctf geändert worden sein, muss also ebinkor anpassen
            CtfFile.Open(ztfKor, CanberraDataAccessLib.OpenMode.dReadWrite)
            Anz = CtfFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_CERTIF)
            CtfFile.InsertRecord(CanberraDataAccessLib.ClassCodes.CAM_CLS_CERTIF, Anz + 1)

            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFNUCL, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFNUCL, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFENER, Anz + 1) = 1950
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFRATE, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFRATE, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFABUN, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFABUN, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFACNVFAC, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFACNVFAC, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFASTMMASS, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFASTMMASS, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFASTMMERR, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFASTMMERR, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFECNVFAC, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFECNVFAC, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFERROR, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFERROR, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFQUANT, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_CTFQUANT, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_CTFFLAGS, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_CTFFLAGS, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_CTFNOASKINI, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_CTFNOASKINI, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_CTFTRACER, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_L_CTFTRACER, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFASTMMATER, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFASTMMATER, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFASTMMATRX, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFASTMMATRX, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFAUNITS, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFAUNITS, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFEUNITS, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFEUNITS, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFHLFUNITS, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_CTFHLFUNITS, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFDATE, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFDATE, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFHLFERR, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFHLFERR, Anz)
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFHLFLIFE, Anz + 1) = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFHLFLIFE, Anz)
            CtfFile.Flush()
            CtfFile.Close()

            'Berechnung des Wirkungsgradpolünoms
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\EFFCAL.exe"
            ShellArguments = _fernkorS & " /CERT=" & ztfKor$ & " /CROSSOVER=0 /ETOL=2 /USEPKRESULTS"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(190, "\EFFCAL -- fernkorS.rpt failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\CALPLOT.exe"
            ShellArguments = _fernkorS & " /CAL=DUAL /EDIT"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(191, "\EFF-CALPLOT -- fernkorS.cnf failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\REPORT.exe"
            ShellArguments = _fernkorS & " /TEMPLATE=" & _MyControlCenter.MCA_CtlFiles & "\ANALYS.TPL /SECTION=EFFCAL /NEWFILE"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(192, "\EFF-REPORT -- fernkor.rpt failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            SYS_SaveReportsToCollectionFile(_ReportFilesDirectory & "\Fernkor.rpt")

            Return True

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function


    Public Function MCA_CalibrateEfficiencyGeometryNear() As Boolean
        Try
            'Effizienzkalibrirung inn Geometri NA
            'Bestimmung der Zerfallsrate(661.6) inn Fernkor.cnf und Berechnung der Effizienz(661.6)
            'Bestimmung der Zerfallsrate(661.6) inn Nah.cnf und Berechnung der Effizienz(661.6)
            'Bestimmung des (datumskorrigirten) Ferhältnisses nah- zu fern-rate (=Korrekturfaktor)
            'Kopi der Datai ebinkor.ctf als ebinkor2.ctf und dort Aintrag Korrekturdiwisor-angepasster Zerfallsraten
            'Eff-kalibrirung an fernkor mittels ebinkor2.ctf d.h. nah-Kalibrirkoeff-Bestimmung
            'Kopi der eff-koeff nach detebin und nah.cnf
            Dim GroesN(0 To 1) As CanberraDataAccessLib.ParamCodes
            Dim CtfFile As New CanberraDataAccessLib.DataAccess
            Dim asfWerter As New CanberraSequenceAnalyzerLib.SequenceAnalyzer
            Dim Anz, i, lRet As Integer
            Dim eff_f, eff_n, kal_faktor As Double
            Dim ztfKor2 As String
            Dim energi() As Double
            Dim zRate() As Double
            Dim Buffer As Object
            Dim ShellCommand As String
            Dim ShellArguments As String
            Dim IndicesList As New Dictionary(Of String, List(Of Integer))
            Dim MyNuclideList As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides)

            MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\CS137.CTF", IndicesList)
            If MyNuclideList.Count = 0 Then
                Return False            'ebin.ctf ler oder felt
            End If
            _ActivityCS137_Near = MyNuclideList(IndicesList("CS-137")(0)).ActivitykBq
            _CertDate_Near = MyNuclideList(IndicesList("CS-137")(0)).CertificateDate

            'Holen der 662-keV-Linie Nettoinnhalt (Cs137) fern
            If Not System.IO.File.Exists(_fernkorS) Then
                GUI_ShowMessageBox(ml_string(193, "Fernkor.cnf not found."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            GUI_CloseSpectrum(False, True)
            GUI_OpenSpectrum(_fernkorS, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)
            Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
            If Anz = 0 Then
                GUI_ShowMessageBox(ml_string(194, "No Lines in FERNKOR.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                GUI_CloseSpectrum(False, True)
                Return False
            End If
            ReDim energi(Anz + 1)
            ReDim zRate(Anz + 1)
            GroesN(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
            GroesN(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCTSS    'Zälrate
            For i = 1 To Anz
                Buffer = _SpectraFile.ParamArray(GroesN, i)
                energi(i) = Buffer(0)      'Energi
                zRate(i) = Buffer(1)       'Zälrate
                If Math.Abs(energi(i) - 661.67) < 1.5 Then Exit For '661-keV-Linie gefunden
            Next i
            GUI_CloseSpectrum(False, True)
            If i > Anz Then
                GUI_ShowMessageBox(ml_string(195, "(661.67keV+-1.5keV)-Line not found. Please check calibration spectrum FERN.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            eff_f = zRate(i) / _ActivityCS137_Far

            'Holen der 662-keV-Linie Nettoinnhalt (Cs137) na
            GUI_OpenSpectrum(_naS, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SGEOMTRY) = "NAH"
            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINKAL.ASF", , , , , _ReportFilesDirectory & "\Nah.rpt", ) ' FFkaAnzaige.RepFenst)
            SYS_SaveReportsToCollectionFile(_ReportFilesDirectory & "\Nah.rpt")
            Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
            If Anz = 0 Then
                GUI_ShowMessageBox(ml_string(196, "Keine Linien im Spektrum FERNKOR.CNF gefunden."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                GUI_CloseSpectrum(False, True)
                Return False
            End If
            ReDim energi(Anz + 1)
            ReDim zRate(Anz + 1)
            GroesN(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
            GroesN(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCTSS    'Zälrate
            For i = 1 To Anz
                Buffer = _SpectraFile.ParamArray(GroesN, i)
                energi(i) = Buffer(0)      'Energi
                zRate(i) = Buffer(1)       'Zälrate
                If Math.Abs(energi(i) - 661.67) < 1.5 Then Exit For '661-keV-Linie gefunden
            Next i
            GUI_CloseSpectrum(False, True)
            If i > Anz Then
                GUI_ShowMessageBox(ml_string(197, "(661.67keV+-1.5keV)-Line not found. Please check calibration spectrum NAH.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            eff_n = zRate(i) / _ActivityCS137_Near

            kal_faktor = eff_n / eff_f / Math.Exp(0.6931471 / 30.17 / 365.25 * DateDiff("d", _CertDate_Far, _CertDate_Near))
            ztfKor2$ = _CertificateDirectory & "\Ebinkor2.ctf"
            System.IO.File.Copy(_CertificateDirectory & "\Ebinkor.ctf", ztfKor2$, True)
            CtfFile.Open(ztfKor2$, CanberraDataAccessLib.OpenMode.dReadWrite)
            Anz = CtfFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_CERTIF) ', CAM_F_CTFRATE
            ReDim zRate(Anz + 1)
            ReDim energi(Anz + 1)
            GroesN(0) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFRATE   'Emissionsrate
            GroesN(1) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFENER   'Energi
            For i = 1 To Anz
                Buffer = CtfFile.ParamArray(GroesN, i)
                energi#(i) = Buffer(1)      'Energi
                Buffer(0) = Buffer(0) / kal_faktor  'Zerfallsrate d.h qasi Emissionswschlk inn Ebinkor.ctf ernidrigt
                CtfFile.ParamArray(GroesN, i) = Buffer
            Next i
            CtfFile.Flush()
            CtfFile.Close()

            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\EFFCAL.exe"
            ShellArguments = _fernkorS & " /CERT=" & ztfKor2$ & " /CROSSOVER=0 /ETOL=2 /USEPKRESULTS"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(198, "\EFFCAL with fernkor.cnf! Code: ") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\CALPLOT.exe"
            ShellArguments = _fernkorS & " /CAL=DUAL /EDIT"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(199, "\NAH-CALPLOT fernkor failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If

            'Sichern der berechneten Energie- und Effizienzkoeffizienten nach NAH.cnf (da bei Programmstart von dieser .cnf explizit in DET:EBIN hochgeladen wird) 
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\MOVEDATA.exe"
            ShellArguments = _fernkorS & " " & _naS & " /ECAL /EFFCAL /OVERWRITE"     '& detEbin & " "  kopi des spektrums nach NAH.CNF
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                Trace.TraceInformation(ml_string(200, "\MOVEDATA - fernkor.cnf to nah.cnf failure.! Code:") & Format(lRet, "0"))
                GUI_ShowMessageBox(ml_string(200, "\MOVEDATA - fernkor.cnf to nah.cnf failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If

            'vom Detektor lösen, Energie/Effizienzkoeffizienten an Lynx übertragen, dann wieder mit Detektor verbinden
            _MyControlCenter.MCA_DisconnectFromDetector()
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\MOVEDATA.exe"
            ShellArguments = _fernkorS & " DET:EBIN /ECAL /EFFCAL /OVERWRITE"     '& detEbin & " "  kopi des spektrums nach DET:EBIN
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            _MyControlCenter.MCA_ConnectToDetector()
            _MyControlCenter.MCA_SetDetectorParams(_MyFHT59N3Par.Customer, _MyFHT59N3Par.StationName, _MyFHT59N3Par.StationID)
            If lRet Then
                Trace.TraceInformation(ml_string(201, "\MOVEDATA - fernkor.cnf to DET:EBIN failure.! Code:") & Format(lRet, "0"))
                GUI_ShowMessageBox(ml_string(201, "\MOVEDATA - fernkor.cnf to DET:EBIN failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ShellCommand = _MyControlCenter.MCA_ExeFiles & "\REPORT.exe"
            ShellArguments = _naS & " /TEMPLATE=" & _MyControlCenter.MCA_CtlFiles & "\ANALYS.TPL /SECTION=EFFCOR /NEWFILE"
            lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
            If lRet Then
                GUI_ShowMessageBox(ml_string(202, "\EFFCOR-REPORT -- fernkor.rpt failure.! Code:") & Format(lRet, "0"), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            SYS_SaveReportsToCollectionFile(_ReportFilesDirectory & "\Nah.rpt")

            GUI_OpenSpectrum(_fernkorS, SpectraTypes.OFFLINE, True, True, frmMeasScreen.SpectralDisplay)
            GUI_ShowMessageBox(MSG_EfficiencyCalibNearOK, "OK", "", "", MYCOL_THERMOGREEN, Color.White)

            Return True

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function


End Module
