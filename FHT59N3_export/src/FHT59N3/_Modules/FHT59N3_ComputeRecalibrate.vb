Module FHT59N3_ComputeRecalibrate

    Public Sub MCA_ReCalibrateEnergyWithNaturalLines(ByVal SpecType As Integer, ByVal DestinationReport As String)
        Try
            '----------------------------------------------------------------------
            'Energi-Nachkalibrierung an Hand natürlicher Linien
            '----------------------------------------------------------------------

            Dim NuclideParameter(0 To 3) As CanberraDataAccessLib.ParamCodes  '0=Energi, 1=Kanallage, 2=Nettofläche, 3=Fwhm
            Dim asfWerter As New CanberraSequenceAnalyzerLib.SequenceAnalyzer
            Dim SpectraFile_NearGeo = New CanberraDataAccessLib.DataAccess
            Dim NumberOfRecords As Integer
            Dim ECOFFSET As Double
            Dim ECSLOPE As Double
            Dim ECQUAD As Double
            Dim Buffer As Object            'speicher zur übernahme von ParamArray-satz
            Dim ShellCommand As String
            Dim ShellArguments As String
            Dim Energy() As Double
            Dim Channel() As Double
            Dim Fwhm() As Double
            Dim Failure() As Double
            Dim calibLines(8) As Double
            Dim k As Integer
            Dim l As Integer
            Dim foundEnergy() As Double
            Dim foundChannel() As Double
            Dim tolerance As Double
            Dim tolerancea As Double
            Dim toleranceb As Double
            Dim toleranceStr As String
            Dim phantom As Double
            Dim fitt(4, 5) As Double
            '(1,1)=Linienanzal, (1,2)=sK, (1,3)=sK2, (1,4)=sE
            '(2,1)=sK,  (2,2)=sK2, (2,3)=sK3, (2,4)=sEK
            '(3,1)=sK2, (3,2)=sK3, (3,3)=sK4, (3,4)=sEK2
            Dim filename As String
            Dim lRet As Integer

            _EREALOld = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL)
            NumberOfRecords = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
            Erase Energy
            Erase Channel
            Erase Failure
            Erase Fwhm
            ReDim Energy(NumberOfRecords + 1)
            ReDim Channel(NumberOfRecords + 1)
            ReDim Failure(NumberOfRecords + 1)
            ReDim Fwhm(NumberOfRecords + 1)
            NuclideParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
            NuclideParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCENTRD  'Kanallage
            NuclideParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCERR    '% der Fläche
            NuclideParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_F_PSDENERGY 'Unsicherhait inn Energi
            For i As Integer = 1 To NumberOfRecords
                Buffer = _SpectraFile.ParamArray(NuclideParameter, i)
                Energy(i) = Buffer(0)      'Energi
                Channel(i) = Buffer(1)       'Kanallage
                Failure(i) = Buffer(2)       '%-Feler
                Fwhm(i) = Buffer(3)        'Unsicherhait inn Energi
            Next
            calibLines(1) = 238.6
            calibLines(2) = 242
            calibLines(3) = 295.3
            calibLines(4) = 351.9
            calibLines(5) = 609.3
            calibLines(6) = 1120.3
            calibLines(7) = 1460.8
            calibLines(8) = 1764.5
            Erase foundEnergy
            ReDim foundEnergy(8)
            Erase foundChannel
            ReDim foundChannel(8)
            k = 0
            tolerancea = 0.0
            For i As Integer = 1 To NumberOfRecords
                For j As Integer = 1 To 8
                    phantom = Math.Abs(Energy(i) - calibLines(j))
                    If phantom < 1.5 And Failure(i) < 20 Then
                        If tolerancea < phantom Then tolerancea = phantom
                        foundEnergy(j) = calibLines(j)
                        foundChannel(j) = Channel(i)
                        k = k + 1
                        Exit For
                    End If
                Next
            Next
            filename = _CertificateDirectory + "\EbinNat8.ctf"
            If k < 8 Then
                '     238 keV             242 keV
                If foundEnergy(1) = 0 Or foundEnergy(2) = 0 Then
                    filename = _CertificateDirectory + "\EbinNat6.ctf"
                    If foundEnergy(1) = 0 And foundEnergy(2) > 0 Then
                        k = k - 1
                        foundEnergy(2) = 0
                    ElseIf foundEnergy(2) = 0 And foundEnergy(1) > 0 Then
                        k = k - 1
                        foundEnergy(1) = 0
                    End If
                End If
            End If
            tolerance = tolerancea + 0.25

            l = 0
            For j As Integer = 1 To 8
                If foundEnergy(j) > 0 Then
                    foundEnergy(l + 1) = foundEnergy(j)
                    foundChannel(l + 1) = foundChannel(j)
                    l = l + 1
                End If
            Next j

            If tolerance > 0.3 Then
                If k > 4 Then                'mindestens 5 Linien gefunden ?
                    If tolerance > 1.5 Then tolerance = 1.5
                    toleranceStr = Format(tolerance, "0.0").Replace(",", ".")
                    'Fitt nach Metode der klainsten Qadrate
                    '   a1         a2         a3         y
                    '(0,0)=k%,  (0,1)=sK,  (0,2)=sK2, (0,3)=sE
                    '(1,0)=sK,  (1,1)=sK2, (1,2)=sK3, (1,3)=sEK
                    '(2,0)=sK2, (2,1)=sK3, (2,2)=sK4, (2,3)=sEK2
                    For j As Integer = 0 To 4
                        For i As Integer = 0 To 3
                            fitt(i, j) = 0
                        Next i
                    Next j
                    fitt(1, 1) = k
                    For j As Integer = 1 To k
                        fitt(1, 4) = fitt(1, 4) + foundEnergy(j)                  'sE
                        fitt(2, 4) = fitt(2, 4) + foundEnergy(j) * foundChannel(j)  'sEK1
                        phantom = foundChannel(j) * foundChannel(j)
                        fitt(3, 4) = fitt(3, 4) + foundEnergy(j) * phantom        'sEK2
                        fitt(1, 2) = fitt(1, 2) + foundChannel(j)                  'sK1
                        fitt(1, 3) = fitt(1, 3) + phantom                        'sK2
                        fitt(2, 3) = fitt(2, 3) + foundChannel(j) * phantom        'sK3
                        fitt(3, 3) = fitt(3, 3) + phantom * phantom              'sK4
                    Next j
                    fitt(2, 1) = fitt(1, 2)           'sK1
                    fitt(2, 2) = fitt(1, 3)           'sK2
                    fitt(3, 1) = fitt(1, 3)           'sK2
                    fitt(3, 2) = fitt(2, 3)           'sK3
                    Multiply(1, 2, fitt(2, 3) / fitt(1, 3), fitt)           'multiplizirt Z1 mit fitt#(2,3)/fitt#(1,3) und subtrahirt Z2 fon Z1
                    'Zaile 1:  x x 0 y
                    Multiply(2, 3, fitt(3, 3) / fitt(2, 3), fitt)           'multiplizirt Z2 mit fitt#(3,3)/fitt#(2,3) und subtrahirt Z3 fon Z2
                    'Zaile 2:  x x 0 y
                    'Zaile 3:  x x x y
                    Multiply(1, 2, fitt(2, 2) / fitt(1, 2), fitt)           'multiplizirt Z1 mit fitt#(2,2)/fitt#(1,2) und subtrahirt Z2 fon Z1
                    'Zaile 1:  x 0 0 y
                    Multiply(1, 0, 1 / fitt(1, 1), fitt)           'berechnet inn Z1 a1=fitt#(1,4)/fitt#(1,1) (multiplizirt Z1 mit 1/fitt#(1,1))
                    'a1 = fitt#(1,4)-noi
                    'Zaile 1:  1 0 0 y
                    'sezt a1 inn Z2 ain und bringt Glid auf andere Saite des "="-Zaichens
                    fitt(2, 4) = fitt(2, 4) - fitt(2, 1) * fitt(1, 4)
                    fitt(2, 1) = 0
                    'Zaile 2:  0 x 0 y
                    'sezt a1 inn Z2 ain und bringt Glid auf andere Saite des "="-Zaichens
                    fitt(3, 4) = fitt(3, 4) - fitt(3, 1) * fitt(1, 4)
                    fitt(3, 1) = 0
                    'Zaile 3:  0 x x y
                    Multiply(2, 0, 1 / fitt(2, 2), fitt)           'berechnet inn Z2 a2=fitt#(2,4)/fitt#(2,2) (multiplizirt Z2 mit 1/fitt#(2,2))
                    'Zaile 2:  0 1 0 y   a2 = fitt#(2,4)-noi
                    'sezt a2 inn Z3 ain und bringt Glid auf andere Saite des "="-Zaichens
                    fitt(3, 4) = fitt(3, 4) - fitt(3, 2) * fitt(2, 4)
                    fitt(3, 2) = 0
                    'Zaile 3:  0 0 x y
                    Multiply(3, 0, 1 / fitt(3, 3), fitt)           'berechnet inn Z3 a3=fitt#(3,4)/fitt#(3,3) (multiplizirt Z3 mit 1/fitt#(3,3))
                    'Zaile 3:  0 0 1 y   a3 = fitt#(3,4)-noi
                    ECOFFSET = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET)          'alte Kalibrirung retten
                    ECSLOPE = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE)
                    ECQUAD = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD)
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET) = fitt(1, 4)  'noie Energikalibrirung sezen
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE) = fitt(2, 4)
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD) = fitt(3, 4)
                    _SpectraFile.Flush()
                    'besser? nochmals Liniensuche und Bestimmung der Tolleranz, danach Sezen inn ebinlin3.asf
                    If SpecType < 4 Then
                        If System.IO.File.Exists(DestinationReport) Then
                            System.IO.File.Delete(DestinationReport) 'gibt es Ergebniss.rpt, dann zu löschen
                        End If
                        asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN1.ASF", , , , , DestinationReport, ) ' FFkaAnzaige.RepFenst)
                    Else                   'für AP_
                        asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN2.ASF", , , , , , ) ' FFkaAnzaige.RepFenst)
                    End If
                    NumberOfRecords = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
                    If NumberOfRecords > 6 Then
                        Erase Energy
                        Erase Channel
                        Erase Failure
                        Erase Fwhm
                        ReDim Energy(NumberOfRecords + 1)
                        ReDim Channel(NumberOfRecords + 1)
                        ReDim Failure(NumberOfRecords + 1)
                        ReDim Fwhm(NumberOfRecords + 1)
                        NuclideParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_F_PSENERGY  'Energi der Linie
                        NuclideParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCENTRD  'Kanallage
                        NuclideParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_F_PSCERR    '% der Fläche
                        NuclideParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_F_PSDENERGY 'Unsicherhait inn Energi
                        For i As Integer = 1 To NumberOfRecords
                            Buffer = _SpectraFile.ParamArray(NuclideParameter, i)
                            Energy(i) = Buffer(0)      'Energi
                            Channel(i) = Buffer(1)       'Kanallage
                            Failure(i) = Buffer(2)       '%-Feler
                            Fwhm(i) = Buffer(3)        'Unsicherhait inn Energi
                        Next
                        k = 0
                        toleranceb = 0
                        For i As Integer = 1 To NumberOfRecords
                            For j As Integer = 1 To 8
                                phantom = Math.Abs(Energy(i) - Channel(j))
                                If phantom < 1.5 And Failure(i) < 20 Then
                                    If toleranceb < phantom Then toleranceb = phantom
                                    k = k + 1
                                    Exit For
                                End If
                            Next j
                        Next i
                        If toleranceb > tolerancea Then
                            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET) = ECOFFSET      'Kalibrirung nicht ferbessert
                            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE) = ECSLOPE
                            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD) = ECQUAD
                            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINLIN2.ASF", , , , , , ) ' FFkaAnzaige.RepFenst)
                        Else
                            tolerance = toleranceb + 0.25
                            If tolerance < 0.5 Then tolerance = 0.5
                            toleranceStr = Format(tolerance, "0.0").Replace(",", ".")
                            ECOFFSET = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET)   'noie Energikoeffizienten
                            ECSLOPE = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE)
                            ECQUAD = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD)
                            If SpecType = 2 Then
                                _MyControlCenter.MCA_SetECOffset(ECOFFSET) 'auch nach detEbin bringen
                                _MyControlCenter.MCA_SetECSlope(ECSLOPE)
                                _MyControlCenter.MCA_SetECQuad(ECQUAD)

                                'In V2.0.0 ist NAH_MISCH.cnf standard, aber noch testen ob nach alter Methode NAH.cnf aktualisiert
                                'werden muss
                                Dim spectrumFile As String = _na_mischS
                                If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                                    spectrumFile = _naS
                                End If

                                SpectraFile_NearGeo.Open(spectrumFile, CanberraDataAccessLib.OpenMode.dReadWrite)
                                SpectraFile_NearGeo.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET) = ECOFFSET
                                SpectraFile_NearGeo.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE) = ECSLOPE
                                SpectraFile_NearGeo.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD) = ECQUAD
                                SpectraFile_NearGeo.Flush()
                                SpectraFile_NearGeo.Close()
                                GUI_SetMessage(MSG_EnergyReCalibrated, MessageStates.GREEN)
                            End If
                            End If   'If-end tolb# > tola#
                    End If     'If-end Anz% > 6
                    ShellCommand = _MyControlCenter.MCA_ExeFiles & "\PARS.exe"
                    ShellArguments = _MyControlCenter.MCA_CtlFiles & "\EBINLIN3.ASF /TOLERANCE=" & toleranceStr
                    lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
                    If lRet Then
                        GUI_SetMessage(ShellCommand & " " & ShellArguments & Format(lRet, "0"), MessageStates.YELLOW)
                    End If
                End If 'If-end k% > 4
                _SpectraFile.Flush()
            Else
                ShellCommand = _MyControlCenter.MCA_ExeFiles & "\PARS.exe"
                ShellArguments = _MyControlCenter.MCA_CtlFiles & "\EBINLIN3.ASF /TOLERANCE=0.5"
                lRet = SYS_MyShell(ShellCommand, ShellArguments, ProcessWindowStyle.Hidden, True)
                If lRet Then
                    GUI_SetMessage(ShellCommand & " " & ShellArguments & Format(lRet, "0"), MessageStates.YELLOW)
                End If
            End If   'If-end tol# > 0.5

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub MCA_ReCalibrateK40(ByVal SpecType As Integer, ByVal K40Channel As Double)
        Try
            Dim ECSLOPE As Double
            Dim ECOFFSET As Double
            Dim ECQUAD As Double
            Dim SpectraFile_NearGeo = New CanberraDataAccessLib.DataAccess
            ECOFFSET = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECOFFSET)
            ECSLOPE = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE)
            ECQUAD = _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECQUAD)
            ECSLOPE = (1460.81 - ECOFFSET) / (K40Channel) - ECQUAD * K40Channel
            _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE) = ECSLOPE
            _SpectraFile.Flush()
            If (SpecType = 1) Or (SpecType = 2) Then
                _MyControlCenter.MCA_SetECSlope(ECSLOPE)

                'In V2.0.0 ist NAH_MISCH.cnf standard, aber noch testen ob nach alter Methode NAH.cnf aktualisiert
                'werden muss
                Dim spectrumFile As String = _na_mischS
                If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                    spectrumFile = _naS
                End If

                'Todo: hier wird nicht nach NAH.cnf und NAH_MISCH.cnf unterschieden!!
                SpectraFile_NearGeo.Open(spectrumFile, CanberraDataAccessLib.OpenMode.dReadWrite)
                SpectraFile_NearGeo.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ECSLOPE) = ECSLOPE
                SpectraFile_NearGeo.Flush()
                SpectraFile_NearGeo.Close()
            End If
            GUI_SetMessage(MSG_K40ReCalibrated, MessageStates.YELLOW)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub Multiply(ByVal mz As Integer, ByVal sz As Integer, ByVal Factor As Double, ByRef fitt(,) As Double)
        'mz=Zeilennr, deren Werte mit Factor muliplizirt werden
        'sz>0, dann Nr. der Zeile, die von mz subtrahiert wird
        'sz=0 zur Berechnung eines Koeffizienten
        'Factor ist Multiplikator
        Try
            For i As Integer = 1 To 4
                fitt(mz, i) = fitt(mz, i) * Factor
                If sz > 0 Then
                    fitt(mz, i) = fitt(mz, i) - fitt(sz, i)
                    If Math.Abs(fitt(mz, i)) < 0.000001 Then fitt(mz, i) = 0
                End If
            Next i%
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

End Module
