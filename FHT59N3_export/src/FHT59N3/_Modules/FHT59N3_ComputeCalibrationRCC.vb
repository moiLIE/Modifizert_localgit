Imports FHT59N3Core

Module FHT59N3_ComputeCalibrationRCC


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CalibrationType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MCA_CalibrateEfficiencyRCC(ByVal CalibrationType As CalibTypes) As Boolean
        Try
            Dim GroesW(0 To 1) As CanberraDataAccessLib.ParamCodes
            Dim wkpEnergi As Double

            'Daten aus Zertifikat-Datei
            Dim Activity_CS137_in_CTF As Double
            Dim CertDate As Date


            'berechnete Daten aus temp.cnf (gerade aufgenommenes WKP-Spektrum)
            Dim EffWKP As String = "_UNDEFINED"
            Dim wkp_cs As Double
            'berechnete Daten aus nah.cnf/nah_misch.cnf (zuletzt gemessene Nah-Spektrum von voller Kalibrierung)
            Dim na_cs As Double
            Dim EffNear As String = "_UNDEFINED"

            Dim validResult As Boolean
            validResult = GetDataFromCertificateFile(CalibrationType, Activity_CS137_in_CTF, CertDate)
            If (Not validResult) Then
                Exit Function
            End If

            validResult = GetEfficiencyFromWkpSpectrum(CertDate, Activity_CS137_in_CTF, wkpEnergi, wkp_cs, EffWKP)
            If (Not validResult) Then
                Exit Function
            End If

            validResult = GetEfficiencyFromNearSpectrum(CalibrationType, CertDate, Activity_CS137_in_CTF, na_cs, EffNear)
            If (Not validResult) Then
                Exit Function
            End If

            Dim DiffEffWkpNear As String
            DiffEffWkpNear = Format(Math.Abs(wkp_cs - na_cs) / 10, "0.000")

            Dim meldg As String
            meldg = ml_string(207, "Efficiency extracted from the actual spectrum") & vbCrLf
            meldg = meldg & ml_string(208, " at 661.7 keV is: ") & EffWKP & " %"

            If (CalibrationType = CalibTypes.CalibRccCs137) Then
                meldg = meldg & vbCrLf & ml_string(1209, "extracted from NAH.cnf: ") & EffNear & " %"
            ElseIf (CalibrationType = CalibTypes.CalibRccMix) Then
                meldg = meldg & vbCrLf & ml_string(1210, "extracted from NAH_MISCH.cnf: ") & EffNear & " %"
            End If

            meldg = meldg & vbCrLf & ml_string(210, "the difference is: ") & DiffEffWkpNear
            meldg = meldg & vbCrLf & vbCrLf & ml_string(211, "Do you accept the new value?")
            If GUI_ShowMessageBox(meldg, ml_string(90, "Yes"), ml_string(212, "No"), "", MYCOL_THERMOGREEN, Color.White) = MsgBoxResult.Yes Then
                System.IO.File.Copy(_TempS, _WkpS, True)                 'kopirt spektrum.cnf nach x.cnf
                SYS_SaveRCCResults(wkpEnergi, EffWKP)
            Else
                Exit Function
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CalibrationType"></param>
    ''' <param name="Activity_CS137_in_CTF"></param>
    ''' <param name="CertDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetDataFromCertificateFile(ByVal CalibrationType As CalibTypes, ByRef Activity_CS137_in_CTF As Double,
                                                ByRef CertDate As Date) As Boolean
        Dim CertificateFileName As String

        'für CalibRccMix...
        CertificateFileName = "EBIN_NAH_MISCH.CTF"
        If (CalibrationType = CalibTypes.CalibRccCs137) Then
            CertificateFileName = "CS137.CTF"
        End If

        Dim IndicesList As New Dictionary(Of String, List(Of Integer))
        Dim MyNuclideList As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides)

        MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\" & CertificateFileName, IndicesList)
        If MyNuclideList.Count = 0 Then

            Return False            'ebin.ctf ler oder felt
        End If

        Dim cs137Idx = IndicesList("CS-137")(0)
        Activity_CS137_in_CTF = MyNuclideList(cs137Idx).ActivitykBq
        CertDate = MyNuclideList(cs137Idx).CertificateDate

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CertDate"></param>
    ''' <param name="Activity_CS137_in_CTF"></param>
    ''' <param name="wkpEnergi"></param>
    ''' <param name="EffWKP"></param>
    ''' <param name="EffWKPAsString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetEfficiencyFromWkpSpectrum(ByVal CertDate As Date, ByVal Activity_CS137_in_CTF As Double,
                                                 ByRef wkpEnergi As Double, ByRef EffWKP As Double, ByRef EffWKPAsString As String) As Boolean

        Dim GroesW(0 To 1) As CanberraDataAccessLib.ParamCodes
        Dim Anz As Integer
        Dim i As Integer
        Dim energi() As Double
        Dim zRate() As Double
        Dim Buffer() As Object
        Dim diff As Double

        'Bestimmen der Effizienz an 661.67 keV aus wkp-Spektrum
        GUI_OpenSpectrum(_TempS, SpectraTypes.OFFLINE, False, False, Nothing)
        Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK) ', CAM_F_PSENERGY
        ReDim energi(Anz + 1)
        ReDim zRate(Anz + 1)
        GroesW(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
        GroesW(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCTSS    'ips
        For i = 1 To Anz
            Buffer = _SpectraFile.ParamArray(GroesW, i)
            energi(i) = Buffer(0)       'Energi
            zRate(i) = Buffer(1)       'ips
            If Math.Abs(energi(i) - 661.67) < 2 Then Exit For
        Next i
        If i > Anz Then
            GUI_ShowMessageBox(ml_string(203, "(661.7keV+-2keV)-Line not found.") & vbCrLf & ml_string(204, "Please check calibration spectrum."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            GUI_CloseSpectrum(False, True)
            Return False
        End If
        GUI_CloseSpectrum(False, True)
        wkpEnergi = energi(i)
        diff = DateDiff("d", CertDate, Now)   'bestimmen der Effizienz an 661.67 keV aus wkp-Spektrum
        'ActivityCS137CTF = Anfangsaktiwität des cs-137-Stralers aus CS137.ctf, der für wkp benuzt wird
        EffWKP = zRate(i) * Math.Exp(0.69315 * diff / (365.25 * 30.17)) / (0.8512 * Activity_CS137_in_CTF)
        EffWKPAsString = Format(EffWKP / 10, "0.000")     'Effizienz an 661.67 keV aus wkp-Spektrum zum Referenzdatum

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CalibrationType"></param>
    ''' <param name="CertDate"></param>
    ''' <param name="Activity_CS137_in_CTF"></param>
    ''' <param name="EffNear"></param>
    ''' <param name="EffNearAsString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetEfficiencyFromNearSpectrum(ByVal CalibrationType As CalibTypes, ByVal CertDate As Date,
                                                  ByVal Activity_CS137_in_CTF As Double, ByRef EffNear As Double,
                                                  ByRef EffNearAsString As String) As Boolean

        Dim GroesW(0 To 1) As CanberraDataAccessLib.ParamCodes
        Dim Anz As Integer
        Dim i As Integer
        Dim energi() As Double
        Dim zRate() As Double
        Dim Buffer() As Object
        Dim diff As Double
        Dim NearDate As Date

        Dim spectrumFile As String

        If (CalibrationType = CalibTypes.CalibRccCs137) Then
            spectrumFile = _naS
        ElseIf (CalibrationType = CalibTypes.CalibRccMix) Then
            spectrumFile = _na_mischS
        Else
            Return False
        End If

        'Holen der Effizienz an 661.67 keV aus nah.cnf/nah_misch.cnf
        NearDate = FileDateTime(spectrumFile)  'nah.cnfs Zaitstempel
        GUI_OpenSpectrum(spectrumFile, SpectraTypes.OFFLINE, False, False, Nothing)
        Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK) ', CAM_F_PSENERGY
        If Anz = 0 Then
            GUI_ShowMessageBox(ml_string(205, "No Lines in NAH.CNF."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            GUI_CloseSpectrum(False, True)
            Return False
        End If

        'Bestimmen der Effizienz an 661.67 keV aus nah.cnf/nah_misch.cnf
        ReDim energi(Anz + 1)
        ReDim zRate(Anz + 1)
        GroesW(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
        GroesW(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCTSS    'ips
        For i = 1 To Anz
            Buffer = _SpectraFile.ParamArray(GroesW, i)
            energi(i) = Buffer(0)       'Energi
            zRate(i) = Buffer(1)       'ips
            If Math.Abs(energi(i) - 661.67) < 2 Then Exit For
        Next i
        If i > Anz Then
            GUI_ShowMessageBox(ml_string(206, "(661.7keV+-2keV)-Line not found in CNF file."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            GUI_CloseSpectrum(False, True)
            Return False
        End If
        GUI_CloseSpectrum(False, True)

        diff = DateDiff("d", CertDate, NearDate)   'Bestimmen der Effizienz an 661.67 keV aus nah.cnf,nah_misch.cnf
        EffNear = zRate(i) * Math.Exp(0.69315 * diff / (365.25 * 30.17)) / (0.8512 * Activity_CS137_in_CTF)
        EffNearAsString = " " & Format(EffNear / 10, "0.000")      'Effizienz an 661.67 keV aus nah.cnf zum Referenzdatum

        Return True
    End Function



End Module
