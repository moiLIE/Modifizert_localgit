Imports System.Globalization
Imports FHT59N3Core

Public Module FHT59N3_DataFunctions
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_DataFunctions.vb 424 2017-07-19 16:11:06Z marcel.bender $
    ' Title: data functions
    '
    ' Description:
    ' Processing of data files containing configuration data:
    '  - read/write Canberra configuration
    '  - read/write system settings
    '  - create working directory structure
    ' -------------------------------------------------------------

#Region "MCA"

    ' ----------------------------------------------------------------------
    ' zaig_ztfQell prüft zunächst den Innhalt der Zertifikatsdatai ctfqel auf
    ' Plausibilität und zaigt anschlisend den Innhalt an. Bai falschen
    ' Angaben ist di Möglichkait ainer Noiaingabe der Aktiwität und/oder
    ' des Referenzdatums forgeseen.
    ' ----------------------------------------------------------------------
    Public Function MCA_ReadCanberraCtfFile(ByVal File As String, ByRef IndicesList As Dictionary(Of String, List(Of Integer))) As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides)
        Dim CtfFile As New CanberraDataAccessLib.DataAccess
        Dim NuklidParameter(0 To 3) As CanberraDataAccessLib.ParamCodes  '0=Nuklidname, 1=Energi, 2=Emissionsrate, 3=-warschlk
        Dim CertificateDate As Date
        Dim Buffer As Object            'speicher zur übernahme von ParamArray-satz
        Dim anzLinien As Integer
        Dim ret As Integer
        Dim NuclideList As New Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides)
        Try
            ret = CtfFile.FileExists(File)
            If ret = 0 Then
                ret = GUI_ShowMessageBox(ml_string(221, "Certificate File ") & File & ml_string(222, " does not exist."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return Nothing
            End If
            CtfFile.Open(File, CanberraDataAccessLib.OpenMode.dReadWrite)
            anzLinien = CtfFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_CERTIF)
            If anzLinien = 0 Then
                ret = GUI_ShowMessageBox(ml_string(221, "Certificate File ") & File & ml_string(223, " does not contain any data."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                CtfFile.Close()
                CtfFile = Nothing
                Return Nothing
            End If
            NuklidParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_T_CTFNUCL
            NuklidParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFENER
            NuklidParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFRATE
            NuklidParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFABUN
            CertificateDate = CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFDATE)
            'holt Säze aus Zertifikatsdatai
            IndicesList.Clear()
            For i As Integer = 1 To anzLinien
                Buffer = CtfFile.ParamArray(NuklidParameter, i)
                Dim Nuclide As New FHT59N3Core.FHT59N3MCA_CertificateNuclides
                Nuclide.Name = CType(Buffer(0), String)
                Nuclide.Energy = CType(Buffer(1), Double)
                Nuclide.EmissionProbability = CType(Buffer(3), Double)
                Nuclide.ActivitykBq = Math.Round(CType(Buffer(2), Double) / CType(Buffer(3), Double) / 10, 2)
                Nuclide.CertificateDate = CertificateDate
                NuclideList.Add(i, Nuclide)
                If Not IndicesList.ContainsKey(Nuclide.Name) Then
                    Dim IndList As New List(Of Integer)
                    IndList.Add(i)
                    IndicesList.Add(Nuclide.Name, IndList)
                Else
                    IndicesList(Nuclide.Name).Add(i)
                End If
            Next i
            CtfFile.Close()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        Return NuclideList
    End Function

    Public Function MCA_CheckCanberraCtfFile(ByVal NuclideList As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides), ByVal IndicesList As Dictionary(Of String, List(Of Integer))) As List(Of FHT59N3Core.FHT59N3MCA_CertificateActivities)
        'auf unstimmigkeiten prüfen
        Dim ActivitiesList As New List(Of FHT59N3Core.FHT59N3MCA_CertificateActivities)
        Try
            For Each Nuclide As String In IndicesList.Keys
                Dim NewActEntry As New FHT59N3Core.FHT59N3MCA_CertificateActivities
                NewActEntry.Name = Nuclide
                NewActEntry.ActivitykBq = 0
                NewActEntry.OK = True
                For i As Integer = 0 To IndicesList(Nuclide).Count - 2
                    Dim Index As Integer = IndicesList(Nuclide)(i)
                    Dim IndexNext As Integer = IndicesList(Nuclide)(i + 1)
                    If NuclideList(Index).ActivitykBq <> NuclideList(IndexNext).ActivitykBq Then
                        If Not NewActEntry.ProbEnergies.Contains(NuclideList(Index).Energy) Then
                            NewActEntry.ProbEnergies.Add(NuclideList(Index).Energy)
                        End If
                        If Not NewActEntry.ProbEnergies.Contains(NuclideList(IndexNext).Energy) Then
                            NewActEntry.ProbEnergies.Add(NuclideList(IndexNext).Energy)
                        End If
                        NewActEntry.OK = False
                    End If
                Next
                If NewActEntry.OK Then
                    NewActEntry.ActivitykBq = NuclideList(IndicesList(Nuclide)(0)).ActivitykBq
                End If
                NewActEntry.CertificateDate = NuclideList(IndicesList(Nuclide)(0)).CertificateDate
                ActivitiesList.Add(NewActEntry)
            Next
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        Return ActivitiesList

    End Function

    Public Sub MCA_WriteDataToCanberraCtfFile(ByVal File As String, ByVal CertificateDate As Date, ByVal NuclideList As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides))
        Dim CtfFile As New CanberraDataAccessLib.DataAccess
        Dim NuklidParameter(0 To 3) As CanberraDataAccessLib.ParamCodes  '0=Nuklidname, 1=Energie, 2=Emissionsrate, 3=-warschlk
        Dim Buffer As Object            'speicher zur übernahme von ParamArray-satz
        Dim anzLinien As Integer
        Dim ret As Integer
        Try
            ret = CtfFile.FileExists(File)
            If ret = 0 Then
                ret = GUI_ShowMessageBox(ml_string(221, "Certificate File ") & File & ml_string(222, " does not exist."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Exit Sub
            End If
            CtfFile.Open(File, CanberraDataAccessLib.OpenMode.dReadWrite)
            anzLinien = CtfFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_CERTIF)
            If anzLinien = 0 Then
                ret = GUI_ShowMessageBox(ml_string(221, "Certificate File ") & File & ml_string(223, " does not contain any data."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                CtfFile.Close()
                CtfFile = Nothing
                Exit Sub
            End If
            NuklidParameter(0) = CanberraDataAccessLib.ParamCodes.CAM_T_CTFNUCL
            NuklidParameter(1) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFENER
            NuklidParameter(2) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFRATE
            NuklidParameter(3) = CanberraDataAccessLib.ParamCodes.CAM_F_CTFABUN
            CtfFile.Param(CanberraDataAccessLib.ParamCodes.CAM_X_CTFDATE) = CertificateDate
            Buffer = CtfFile.ParamArray(NuklidParameter, anzLinien) 'buffer initialisieren
            For i As Integer = 1 To anzLinien
                Buffer(0) = NuclideList(i).Name
                Buffer(1) = NuclideList(i).Energy
                Buffer(2) = Math.Round(NuclideList(i).ActivitykBq * NuclideList(i).EmissionProbability * 10, 2)
                Buffer(3) = NuclideList(i).EmissionProbability
                CtfFile.ParamArray(NuklidParameter, i) = Buffer
            Next i
            CtfFile.Flush()
            CtfFile.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Function MCA_GetNuclidsFromBib() As Boolean
        Dim ebinNlb As New CanberraDataAccessLib.DataAccess
        Dim nukGroes(0 To 2) As CanberraDataAccessLib.ParamCodes   '0=NCLNAME, 1=NCLHLFLIFE
        Dim ret As Integer
        Dim NucCount As Integer
        Dim Buffer As Object
        Dim FileName As String
        Try
            FileName = _NuclideLibsDirectory & "\EBIN.NLB"
            ret = ebinNlb.FileExists(FileName)
            If ret = 0 Then
                ret = GUI_ShowMessageBox(_NuclideLibsDirectory & "\EBIN.NLB" & " fehlt.", "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                Return False
            End If
            ebinNlb.Open(FileName, CanberraDataAccessLib.OpenMode.dReadOnly)
            NucCount = ebinNlb.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_NUCL) ', CAM_T_NCLNAME

            nukGroes(0) = CanberraDataAccessLib.ParamCodes.CAM_T_NCLNAME
            nukGroes(1) = CanberraDataAccessLib.ParamCodes.CAM_X_NCLHLFLIFE
            nukGroes(2) = CanberraDataAccessLib.ParamCodes.CAM_L_NLNUCL

            For i As Integer = 1 To NucCount
                Buffer = ebinNlb.ParamArray(nukGroes, i)
                Dim Nuclide As New FHT59N3MCA_Nuclide
                Nuclide.Library.Name = CType(Buffer(0), String)             'Nuklidnamen
                Nuclide.Library.NuclideHalfLife = CType(Buffer(1), Double) / 3600.0#   'Halbwertsdauer inn h
                Nuclide.Library.DecayConstant = 0.69315 / Nuclide.Library.NuclideHalfLife
                Nuclide.Library.NuclidNumber = CType(Buffer(2), Integer)
                _MyControlCenter.MCA_Nuclides.AddNuclide(Nuclide)
            Next i
            ebinNlb.Close()
            ebinNlb = Nothing
            Dim Nuclide1 As New FHT59N3MCA_Nuclide
            Nuclide1.Library.Name = "PO-218"
            Nuclide1.Library.DecayConstant = 818.14                 'Zerfallskonstante des Po218
            Nuclide1.Library.NuclideHalfLife = 0.69315 / Nuclide1.Library.DecayConstant
            _MyControlCenter.MCA_Nuclides.AddNuclide(Nuclide1)
            Return True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

#End Region

#Region "System"

    Private writeSettingsLock As New Object

    ''' <summary>
    ''' Einstellungen speichern (Thermo config file)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SYS_WriteSettings()
        SyncLock writeSettingsLock
            Try
                With _MyConfig

                    'Station
                    .WriteMySetting("Station", "Customer", _MyFHT59N3Par.Customer)
                    .WriteMySetting("Station", "StationName", _MyFHT59N3Par.StationName)
                    .WriteMySetting("Station", "StationID", _MyFHT59N3Par.StationID)
                    .WriteMySetting("Station", "StationSerialNumber", _MyFHT59N3Par.StationSerialNumber)

                    'Detector
                    .WriteMySetting("Detector", "IsCanberra", _MyFHT59N3Par.IsCanberraDetector)
                    .WriteMySetting("Detector", "CP5ComPort", _MyFHT59N3Par.CP5Com)
                    '.WriteMySetting("Detector", "iPATemperatureJAR", _MyFHT59N3Par.iPA_TemperatureJARpath)
                    '.WriteMySetting("Detector", "CanberraCryoCoolExe", _MyFHT59N3Par.CryoCoolExecutable)
                    '.WriteMySetting("Detector", "CanberraTemperatureLog", _MyFHT59N3Par.iPATemperatureLog)

                    'Measurement
                    .WriteMySetting("Measurement", "DayStartTime", _MyFHT59N3Par.DayStartTime.ToString)
                    .WriteMySetting("Measurement", "FilterTimeH", _MyFHT59N3Par.FilterTimeh.ToString)
                    .WriteMySetting("Measurement", "MeasurementTimeMin", _MyFHT59N3Par.MeasurementTimemin.ToString)
                    .WriteMySetting("Measurement", "FilterStepMM", _MyFHT59N3Par.FilterStepmm.ToString)
                    .WriteMySetting("Measurement", "FilterStepIM", _MyFHT59N3Par.FilterStepim.ToString)
                    .WriteMySetting("Measurement", "FilterStepsAvailable", _MyFHT59N3Par.FilterSteps.ToString)
                    .WriteMySetting("Measurement", "N2Threshold", _MyFHT59N3Par.N2FillThreshold.ToString)
                    .WriteMySetting("Measurement", "FactorBezel", String.Format("{0:N3}", _MyFHT59N3Par.FactorBezel))
                    .WriteMySetting("Measurement", "Factor1950keV", _MyFHT59N3Par.Factor1950keV.ToString)
                    .WriteMySetting("Measurement", "K40MinArea", _MyFHT59N3Par.K40MinArea)
                    .WriteMySetting("Measurement", "K40FWHM", _MyFHT59N3Par.K40FWHM)
                    .WriteMySetting("Measurement", "WorkingAirFlow", _MyFHT59N3Par.AirFlowWorking.ToString)
                    .WriteMySetting("Measurement", "AirFlowSetPoint", _MyFHT59N3Par.AirFlowSetPoint.ToString)
                    .WriteMySetting("Measurement", "MinAirFlowAlert", _MyFHT59N3Par.MinAirFlowAlert.ToString)
                    .WriteMySetting("Measurement", "MaxAirFlowAlert", _MyFHT59N3Par.MaxAirFlowAlert.ToString)

                    'SPS
                    .WriteMySetting("SPS", "SerialPort", _MyFHT59N3Par.SPSCom)
                    .WriteMySetting("SPS", "SPSConnectionType", _MyFHT59N3Par.SPSConnectionType)
                    .WriteMySetting("SPS", "IpNetworkAddress", _MyFHT59N3Par.SPSIpNetworkAddress)
                    .WriteMySetting("SPS", "IpNetworkPort", _MyFHT59N3Par.SPSIpNetworkPort.ToString)

                    'Filterband Drucker
                    .WriteMySetting("PaperrollPrinter", "SerialPort", _MyFHT59N3Par.PrinterSerialPort)
                    .WriteMySetting("PaperrollPrinter", "PrinterPositionToMeasureCenterDistanceMM", _MyFHT59N3Par.PrinterPositionToMeasureCenterDistanceMM)
                    .WriteMySetting("PaperrollPrinter", "DustationDiameterMM", _MyFHT59N3Par.DustationDiameterMM)
                    .WriteMySetting("PaperrollPrinter", "EnablePaperrollPrinter", _MyFHT59N3Par.EnablePaperrollPrinter)


                    'Alarmnuclides
                    For i As Integer = 0 To _MyFHT59N3Par.AlarmNuclideConfigStrings.Count - 1
                        .WriteMySetting("AlarmNuclides", "Nuclide" & i, _MyFHT59N3Par.AlarmNuclideConfigStrings(i))
                    Next
                    .WriteMySetting("AlarmNuclides", "NumberOfNuclides", _MyFHT59N3Par.AlarmNuclideConfigStrings.Count.ToString)

                    'Flags
                    .WriteMySetting("Flags", "SubtractBackground", _SubtractBkgSpectrum.ToString)
                    .WriteMySetting("Flags", "NetViewActive", _NetViewActive.ToString)
                    .WriteMySetting("Flags", "NetLogActive", _NetLogActive.ToString)
                    .WriteMySetting("Flags", "DisplayPoints", _MyFHT59N3Par.DisplayPoints.ToString)
                    .WriteMySetting("Flags", "AlarmOutSwitchableWithoutMaintenance", _AlarmOutSwitchableWithoutMaintenance.ToString)
                    'start new in V2.0.2:
                    .WriteMySetting("Flags", "AlarmModeSettings", FlaggedEnumToString(_MyFHT59N3Par.AlarmModeSettings))
                    .WriteMySetting("Flags", "IntensiveModeEnabled", _MyFHT59N3Par.IntensiveModeEnabled.ToString)
                    'end new in V2.0.2
                    .WriteMySetting("Flags", "ShowSystemStateInMainDisplay", _ShowSystemStateInMainDisplay.ToString)
                    .WriteMySetting("Flags", "EnableEmergencyStopDetect", _MyFHT59N3Par.EnableEmergencyStopDetect.ToString)
                    .WriteMySetting("Flags", "EnableCapturingDetectorTemperature", _MyFHT59N3Par.EnableCapturingDetectorTemperature.ToString)
                    .WriteMySetting("Flags", "EmergencyStopCoolingState", _MyControlCenter.SYS_States.EmergencyStopCoolingState.ToString)

                    'Miscellaneous (Misc)
                    .WriteMySetting("Misc", "NetViewPath", _NetViewPath)
                    .WriteMySetting("Misc", "Language", _MyFHT59N3Par.Language)
                    .WriteMySetting("Misc", "IPLynx", _IPLynx)
                    .WriteMySetting("Misc", "SimulateLynxSystem", _SimulateLynxSystem)
                    .WriteMySetting("Misc", "TempZero", _TempZero.ToString)
                    .WriteMySetting("Misc", "WaitTimeBeforeK40Check", _WaitTimeBeforeK40Check.ToString)
                    .WriteMySetting("Misc", "CaptureCycleDetectorTemperature", _MyFHT59N3Par.CaptureCycleDetectorTemperature.ToString)
                    .WriteMySetting("Misc", "CrystalTooWarmTempThreshold", _MyFHT59N3Par.CrystalTooWarmTempThreshold.ToString)
                    .WriteMySetting("Misc", "CrystalWarmedUpTempThreshold", _MyFHT59N3Par.CrystalWarmedUpTempThreshold.ToString)
                    .WriteMySetting("Misc", "KeepActiveHighVoltageOnExitGuiFlag", _MyFHT59N3Par.KeepActiveHighVoltageOnExitGuiFlag)
                    .WriteMySetting("Misc", "KeepActiveEcoolerOnExitGuiFlag", _MyFHT59N3Par.KeepActiveEcoolerOnExitGuiFlag)
                    .WriteMySetting("Misc", "CalibrationType", _MyFHT59N3Par.CalibrationType)
                    .WriteMySetting("Misc", "AirFlowThroughPutCalculationMode", _MyFHT59N3Par.AirFlowThroughPutCalculationMode)
                    .WriteMySetting("Misc", "ZeroDegreesVoltage", Format(_MyFHT59N3Par.ZeroDegreesVoltage, "0.00"))
                    .WriteMySetting("Misc", "ZeroVoltsTemperature", Format(_MyFHT59N3Par.ZeroVoltsTemperature, "0.00"))
                    .WriteMySetting("Misc", "SuppressTimeSecondsAirflowTooLess", _SuppressTimeAirflowTooLess)

                    'System
                    .WriteMySetting("System", "CertificateDirectory", _MyFHT59N3Par.CertificateDirectory)
                    .WriteMySetting("System", "NuclideLibsDirectory", _MyFHT59N3Par.NuclideLibsDirectory)
                    .WriteMySetting("System", "SpectraDirectory", _MyFHT59N3Par.SpectraDirectory)
                    .WriteMySetting("System", "AnalyzationFilesDirectory", _MyFHT59N3Par.AnalyzationFilesDirectory)
                    .WriteMySetting("System", "AlarmfilesDirectory", _MyFHT59N3Par.AlarmfilesDirectory)
                    .WriteMySetting("System", "ReportFilesDirectory", _MyFHT59N3Par.ReportFilesDirectory)
                    .WriteMySetting("System", "SensorRecordingDirectory", _MyFHT59N3Par.SensorRecordingDirectory)
                    .WriteMySetting("System", "LogfilesDirectory", _MyFHT59N3Par.LogfilesDirectory)

                    'Uninterruptible Power Supply
                    .WriteMySetting("UninterruptiblePowerSupply", "IsUpsAvailable", _MyFHT59N3Par.IsUpsAvailable.ToString)
                    .WriteMySetting("UninterruptiblePowerSupply", "IpAddressOfUpsSnmp", _MyFHT59N3Par.IpAddressOfUpsSnmp)
                    .WriteMySetting("UninterruptiblePowerSupply", "PortOfUpsSnmp", _MyFHT59N3Par.PortOfUpsSnmp)
                    .WriteMySetting("UninterruptiblePowerSupply", "SnmpCommunity", _MyFHT59N3Par.SnmpCommunity)


                    .WriteMySetting("AnalysisAndReporting", "GenerateAnsiN4242", _MyFHT59N3Par.GenerateAnsiN4242)

                    Dim ansiSettingsString As String = FlaggedEnumToString(_MyFHT59N3Par.AnsiN4242Settings)
                    .WriteMySetting("AnalysisAndReporting", "AnsiN4242Settings", ansiSettingsString)
                    .WriteMySetting("AnalysisAndReporting", "GenerateMonthlySystemMessageFiles", _MyFHT59N3Par.GenerateMonthlySystemMessagesFiles)

                    'Soll in Kunden-Konfigurationsdatei explizit nicht auftauchen...
                    '.WriteMySetting("Debugging", "LynxCommandsDelayTime", _MyFHT59N3Par.LynxCommandsDelayTime)
                    '.WriteMySetting("Debugging", "LynxCommandsShowMessageBox", _MyFHT59N3Par.LynxCommandsShowMessageBox)


                    'Remove unused XML configuration tags (created during development of 2.0.0)
                    .DeleteMySetting("Misc", "TurnOffHighVoltageOnExitGuiFlag")
                    .DeleteMySetting("Misc", "TurnOffEcoolerOnExitGuiFlag")
                    .DeleteMySetting("Misc", "FarOnlyCalibration")
                    .DeleteMySetting("System", "McaDataRecordingDirectory")

                    .WriteSettingsFile(My.Application.Info.Version.ToString)

                End With
            Catch ex As Exception
                Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            End Try
        End SyncLock
    End Sub


    ''' <summary>
    ''' Einstellungen aus XML Datei einlesen (Thermo config file)
    ''' </summary>
    ''' <param name="LanguageAttributeOnly">true: only language attribute is retrieved, false: other attributes are retrieved</param>
    ''' <remarks></remarks>
    Public Sub SYS_ReadSettings(ByVal LanguageAttributeOnly As Boolean)
        Try
            Dim Separator As String = SYS_GetDecimalSeparator()
            Dim NotSeparator As String
            If Separator = "." Then NotSeparator = "," Else NotSeparator = "."

            With _MyConfig
                .ReadSettingsFile()

                _MyFHT59N3Par.Language = .ReadMySetting("Misc", "Language", "German (de)")

                'Miscellaneous
                _NetViewPath = .ReadMySetting("Misc", "NetViewPath", "C:\Program Files\Thermo\NetView32\")
                _IPLynx = .ReadMySetting("Misc", "IPLynx", "127.0.0.1")
                _SimulateLynxSystem = CBool(.ReadMySetting("Misc", "SimulateLynxSystem", "False"))

                _TempZero = CInt(.ReadMySetting("Misc", "TempZero", "-30"))
                _WaitTimeBeforeK40Check = CInt(.ReadMySetting("Misc", "WaitTimeBeforeK40Check", "1800"))

                Dim calibrationTypeString As String
                calibrationTypeString = .ReadMySetting("Misc", "CalibrationType", "NearAndFarCalibration")
                Dim airFlowThroughPutCalculationModeString As String
                airFlowThroughPutCalculationModeString = .ReadMySetting("Misc", "AirFlowThroughPutCalculationMode", "ByTiaSps")

                Dim zeroDegreesVoltageString As String
                zeroDegreesVoltageString = .ReadMySetting("Misc", "ZeroDegreesVoltage", "499,33")
                Dim zeroVoltsTemperatureString As String
                zeroVoltsTemperatureString = .ReadMySetting("Misc", "ZeroVoltsTemperature", "-246,99")


                If Not LanguageAttributeOnly Then

                    '######## Im Programm änderbare Parameter #######

                    'Station
                    _MyFHT59N3Par.Customer = .ReadMySetting("Station", "Customer", "Default")
                    _MyFHT59N3Par.StationName = .ReadMySetting("Station", "StationName", "Default")
                    _MyFHT59N3Par.StationID = .ReadMySetting("Station", "StationID", "0")
                    _MyFHT59N3Par.StationSerialNumber = .ReadMySetting("Station", "StationSerialNumber", "00000000")


                    'Detector
                    _MyFHT59N3Par.IsCanberraDetector = CBool(.ReadMySetting("Detector", "IsCanberra", "False"))
                    _MyFHT59N3Par.CP5Com = .ReadMySetting("Detector", "CP5ComPort", "COM3")
                    '_MyFHT59N3Par.iPA_TemperatureJARpath = .ReadMySetting("Detector", "iPATemperatureJAR", "C:\FHT59N3\iPA_Temperature\")
                    '_MyFHT59N3Par.CryoCoolExecutable = .ReadMySetting("Detector", "CanberraCryoCoolExe", "")
                    '_MyFHT59N3Par.iPATemperatureLog = .ReadMySetting("Detector", "CanberraTemperatureLog", "C:\FHT59N3\iPA_Temperature\TemperatureLog.txt")

                    'Measurement
                    _MyFHT59N3Par.DayStartTime = CType(.ReadMySetting("Measurement", "DayStartTime", "6"), Integer)
                    _MyFHT59N3Par.FilterTimeh = CType(.ReadMySetting("Measurement", "FilterTimeH", "4"), Integer)
                    _MyFHT59N3Par.MeasurementTimemin = CType(.ReadMySetting("Measurement", "MeasurementTimeMin", "10"), Integer)
                    _MyFHT59N3Par.FilterStepmm = CType(.ReadMySetting("Measurement", "FilterStepMM", "120"), Integer)
                    _MyFHT59N3Par.FilterStepim = CType(.ReadMySetting("Measurement", "FilterStepIM", "240"), Integer)
                    _MyFHT59N3Par.FilterSteps = CType(.ReadMySetting("Measurement", "FilterStepsAvailable", "300"), Integer)
                    _MyFHT59N3Par.N2FillThreshold = CType(.ReadMySetting("Measurement", "N2Threshold", "3000"), Integer)

                    _MyFHT59N3Par.FactorBezel = CDbl(.ReadMySetting("Measurement", "FactorBezel", "1.0").Replace(NotSeparator, Separator))
                    _MyFHT59N3Par.Factor1950keV = CDbl(.ReadMySetting("Measurement", "Factor1950keV", "0.9").Replace(NotSeparator, Separator))
                    _MyFHT59N3Par.K40MinArea = CDbl(.ReadMySetting("Measurement", "K40MinArea", "0").Replace(NotSeparator, Separator))
                    _MyFHT59N3Par.K40FWHM = CDbl(.ReadMySetting("Measurement", "K40FWHM", "0").Replace(NotSeparator, Separator))

                    _MyFHT59N3Par.AirFlowWorking = CBool(.ReadMySetting("Measurement", "WorkingAirFlow", "True"))
                    _MyFHT59N3Par.AirFlowSetPoint = CType(.ReadMySetting("Measurement", "AirFlowSetPoint", "10"), Integer)
                    _MyFHT59N3Par.MinAirFlowAlert = CType(.ReadMySetting("Measurement", "MinAirFlowAlert", "6"), Integer)
                    _MyFHT59N3Par.MaxAirFlowAlert = CType(.ReadMySetting("Measurement", "MaxAirFlowAlert", "12"), Integer)

                    'Miscellaneous
                    _MyFHT59N3Par.CaptureCycleDetectorTemperature = CInt(.ReadMySetting("Misc", "CaptureCycleDetectorTemperature", "10"))
                    _MyFHT59N3Par.CrystalTooWarmTempThreshold = CInt(.ReadMySetting("Misc", "CrystalTooWarmTempThreshold", "-150"))
                    _MyFHT59N3Par.CrystalWarmedUpTempThreshold = CInt(.ReadMySetting("Misc", "CrystalWarmedUpTempThreshold", "15"))
                    _MyFHT59N3Par.KeepActiveHighVoltageOnExitGuiFlag = CBool(.ReadMySetting("Misc", "KeepActiveHighVoltageOnExitGuiFlag", "False"))
                    _MyFHT59N3Par.KeepActiveEcoolerOnExitGuiFlag = CBool(.ReadMySetting("Misc", "KeepActiveEcoolerOnExitGuiFlag", "True"))

                    _SuppressTimeAirflowTooLess = CInt(.ReadMySetting("Misc", "SuppressTimeSecondsAirflowTooLess", "0"))

                    _MyFHT59N3Par.CalibrationType = _
                        DirectCast([Enum].Parse(GetType(FHT59N3_SystemParams.CalibrationTypeEnum), calibrationTypeString), FHT59N3_SystemParams.CalibrationTypeEnum)

                    _MyFHT59N3Par.AirFlowThroughPutCalculationMode = _
                        DirectCast([Enum].Parse(GetType(FHT59N3_SystemParams.AirFlowThroughPutCalculationModeEnum), airFlowThroughPutCalculationModeString), FHT59N3_SystemParams.AirFlowThroughPutCalculationModeEnum)

                    _MyFHT59N3Par.ZeroDegreesVoltage = Double.Parse(zeroDegreesVoltageString.Replace(NotSeparator, Separator))
                    _MyFHT59N3Par.ZeroVoltsTemperature = Double.Parse(zeroVoltsTemperatureString.Replace(NotSeparator, Separator))

                    'Flags
                    _MyFHT59N3Par.DisplayPoints = CBool(.ReadMySetting("Flags", "DisplayPoints", "False"))
                    _MyFHT59N3Par.EnableEmergencyStopDetect = CBool(.ReadMySetting("Flags", "EnableEmergencyStopDetect", "True"))
                    _MyFHT59N3Par.EnableCapturingDetectorTemperature = CBool(.ReadMySetting("Flags", "EnableCapturingDetectorTemperature", "True"))

                    Dim defaultAlarmModeSettings As AlarmModeSettings = FHT59N3.AlarmModeSettings.Normal

                    Dim alarmModeSettingsStr As String = .ReadMySetting("Flags", "AlarmModeSettings", FlaggedEnumToString(defaultAlarmModeSettings))
                    Dim alarmModeSettings As AlarmModeSettings = StringToFlaggedEnum(Of AlarmModeSettings)(alarmModeSettingsStr)
                    _MyFHT59N3Par.AlarmModeSettings = alarmModeSettings

                    _MyFHT59N3Par.IntensiveModeEnabled = CBool(.ReadMySetting("Flags", "IntensiveModeEnabled", "True"))

                    'SPS
                    _MyFHT59N3Par.SPSCom = .ReadMySetting("SPS", "SerialPort", "COM1")
                    _MyFHT59N3Par.SPSUseSTXEXTProtocol = .ReadMySetting("SPS", "UseExtendedStxEtxProtocol", "True")
                    _MyFHT59N3Par.SPSOutputChecksumError = .ReadMySetting("SPS", "OutputChecksumError", "False")
                    _MyFHT59N3Par.SPSConnectionType = .ReadMySetting("SPS", "SPSConnectionType", "serial (RS-232)")
                    _MyFHT59N3Par.SPSIpNetworkAddress = .ReadMySetting("SPS", "IpNetworkAddress", "192.168.1.1")
                    _MyFHT59N3Par.SPSIpNetworkPort = .ReadMySetting("SPS", "IpNetworkPort", 5001)

                    'Filterband Drucker
                    _MyFHT59N3Par.PrinterSerialPort = .ReadMySetting("PaperrollPrinter", "SerialPort", "COM2")
                    _MyFHT59N3Par.PrinterPositionToMeasureCenterDistanceMM = CInt(.ReadMySetting("PaperrollPrinter", "PrinterPositionToMeasureCenterDistanceMM", "180"))
                    _MyFHT59N3Par.DustationDiameterMM = CInt(.ReadMySetting("PaperrollPrinter", "DustationDiameterMM", "50"))
                    _MyFHT59N3Par.EnablePaperrollPrinter = CBool(.ReadMySetting("PaperrollPrinter", "EnablePaperrollPrinter", "True"))


                    '######### Ende #########


                    '######### Im Programm NICHT änderbare Parameter ########

                    'Flags
                    _SubtractBkgSpectrum = CBool(.ReadMySetting("Flags", "SubtractBackground", "True"))
                    _NetViewActive = CBool(.ReadMySetting("Flags", "NetViewActive", "True"))
                    _NetLogActive = CBool(.ReadMySetting("Flags", "NetLogActive", "True"))

                    _AlarmOutSwitchableWithoutMaintenance = CBool(.ReadMySetting("Flags", "AlarmOutSwitchableWithoutMaintenance", "False"))
                    _ShowSystemStateInMainDisplay = CBool(.ReadMySetting("Flags", "ShowSystemStateInMainDisplay", "True"))


                    'Unchangeable in program
                    _ReCalibrateK40LineIfNeeded = CBool(.ReadMySetting("Flags", "ReCalibrateK40LineIfNeeded", "False"))
                    _ReCalibrateEnergyIfNeeded = CBool(.ReadMySetting("Flags", "ReCalibrateEnergyIfNeeded", "False"))

                    'System
                    _MyFHT59N3Par.CertificateDirectory = .ReadMySetting("System", "CertificateDirectory", "")
                    _MyFHT59N3Par.NuclideLibsDirectory = .ReadMySetting("System", "NuclideLibsDirectory", "")
                    _MyFHT59N3Par.SpectraDirectory = .ReadMySetting("System", "SpectraDirectory", "")
                    _MyFHT59N3Par.AnalyzationFilesDirectory = .ReadMySetting("System", "AnalyzationFilesDirectory", "")
                    _MyFHT59N3Par.AnalyzationN4242FilesDirectory = .ReadMySetting("System", "AnalyzationN4242FilesDirectory", "")
                    _MyFHT59N3Par.AlarmfilesDirectory = .ReadMySetting("System", "AlarmfilesDirectory", "")
                    _MyFHT59N3Par.ReportFilesDirectory = .ReadMySetting("System", "ReportFilesDirectory", "")
                    _MyFHT59N3Par.SensorRecordingDirectory = .ReadMySetting("System", "SensorRecordingDirectory", "")
                    _MyFHT59N3Par.LogfilesDirectory = .ReadMySetting("System", "LogfilesDirectory", "")

                    'Uninterruptible Power Supply
                    _MyFHT59N3Par.IsUpsAvailable = CBool(.ReadMySetting("UninterruptiblePowerSupply", "IsUpsAvailable", "False"))
                    _MyFHT59N3Par.IpAddressOfUpsSnmp = .ReadMySetting("UninterruptiblePowerSupply", "IpAddressOfUpsSnmp", "127.0.0.1")
                    _MyFHT59N3Par.PortOfUpsSnmp = .ReadMySetting("UninterruptiblePowerSupply", "PortOfUpsSnmp", 161)
                    _MyFHT59N3Par.SnmpCommunity = .ReadMySetting("UninterruptiblePowerSupply", "SnmpCommunity", "public")

                    'ResultAnalysis
                    _MyFHT59N3Par.GenerateAnsiN4242 = .ReadMySetting("AnalysisAndReporting", "GenerateAnsiN4242", "True")


                    Dim ansiSettingsAsString = .ReadMySetting("AnalysisAndReporting", "AnsiN4242Settings", "")
                    _MyFHT59N3Par.AnsiN4242Settings = StringToFlaggedEnum(Of AnsiN4242Settings)(ansiSettingsAsString)
                    _MyFHT59N3Par.GenerateMonthlySystemMessagesFiles = .ReadMySetting("AnalysisAndReporting", "GenerateMonthlySystemMessageFiles", "True")

                    'nur zum Untersuchen eines Fehlers, wird nur eingelesen aber nicht geschrieben (reiner Entwicklungsschalter)...
                    _MyFHT59N3Par.LynxCommandsDelayTime = .ReadMySetting("Debugging", "LynxCommandsDelayTime", "0")
                    _MyFHT59N3Par.LynxCommandsShowMessageBox = .ReadMySetting("Debugging", "LynxCommandsShowMessageBox", "False")

                    '######### Ende #########

                End If
            End With
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_ReadAlarmSettings()

        Try

            Dim alarmNuclides As FHT59N3MCA_AlarmNuclides = _MyControlCenter.MCA_AlarmNuclides

            'Important to flush (see how AddNuclide works!)
            alarmNuclides.Clear()

            With _MyConfig
                .ReadSettingsFile()

                'Alarmnuclides
                Dim NuclideCount As Integer = CType(.ReadMySetting("AlarmNuclides", "NumberOfNuclides", "0"), Integer)

                _MyFHT59N3Par.AlarmNuclideConfigStrings.Clear()

                For i As Integer = 0 To NuclideCount - 1
                    Dim Entry As String = .ReadMySetting("AlarmNuclides", "Nuclide" & i, "")
                    _MyFHT59N3Par.AlarmNuclideConfigStrings.Add(Entry)

                    Dim AlarmNuclide As New FHT59N3MCA_AlarmNuclide
                    Dim Str() As String = Entry.Split("=;".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                    AlarmNuclide.Name = Str(0)
                    AlarmNuclide.AlarmValue1 = Double.Parse(Str(1), NumberStyles.Any, CultureInfo.InvariantCulture)
                    AlarmNuclide.AlarmValue2 = If(Str.Length > 2, Double.Parse(Str(2), NumberStyles.Any, CultureInfo.InvariantCulture), AlarmNuclide.AlarmValue1)

                    alarmNuclides.AddNuclide(AlarmNuclide)
                Next

            End With

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_ReadECoolerMemorized()

        Try
            With _MyConfig
                .ReadSettingsFile()

                'e-Cooler state machine
                Dim HelpString As String = .ReadMySetting("Flags", "EmergencyStopCoolingState", "UNKNOWN")
                If HelpString IsNot Nothing And HelpString.Length > 0 Then
                    If HelpString = "COOLING_ACTIVE" Then
                        _MyControlCenter.SYS_States.EmergencyStopCoolingState = FHT59N3Core.FHT59N3_SystemStates.FHT59N3_EmergencyStopCoolingStates.COOLING_ACTIVE
                    ElseIf HelpString = "COOLING_PHASE_PREPARED" Then
                        _MyControlCenter.SYS_States.EmergencyStopCoolingState = FHT59N3Core.FHT59N3_SystemStates.FHT59N3_EmergencyStopCoolingStates.COOLING_PHASE_PREPARED
                    ElseIf HelpString = "WARMING_PHASE_FORCED" Then
                        _MyControlCenter.SYS_States.EmergencyStopCoolingState = FHT59N3Core.FHT59N3_SystemStates.FHT59N3_EmergencyStopCoolingStates.WARMING_PHASE_FORCED
                    ElseIf HelpString = "UNKNOWN" Then
                        _MyControlCenter.SYS_States.EmergencyStopCoolingState = FHT59N3Core.FHT59N3_SystemStates.FHT59N3_EmergencyStopCoolingStates.UNKNOWN
                    End If
                Else
                    _MyControlCenter.SYS_States.EmergencyStopCoolingState = FHT59N3Core.FHT59N3_SystemStates.FHT59N3_EmergencyStopCoolingStates.UNKNOWN
                End If

            End With

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_CreateWorkingDirectoryStructure()
        Try
            Dim DirectoriesWithoutPermission As String = ""
            Dim MyDirectories As New List(Of String)
            Dim MCADrive As String
            MCADrive = "C" '_MyControlCenter.MCA_ExeFiles.Split(":".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(0)
            MyDirectories.Clear()
            If _MyFHT59N3Par.CertificateDirectory <> "" Then
                _CertificateDirectory = _MyFHT59N3Par.CertificateDirectory
            Else
                _CertificateDirectory = MCADrive & ":\FHT59N3\Configuration\Certificates"
                _MyFHT59N3Par.CertificateDirectory = _CertificateDirectory
            End If
            MyDirectories.Add(_CertificateDirectory)
            If _MyFHT59N3Par.NuclideLibsDirectory <> "" Then
                _NuclideLibsDirectory = _MyFHT59N3Par.NuclideLibsDirectory
            Else
                _NuclideLibsDirectory = MCADrive & ":\FHT59N3\Configuration\NuclideLibs"
                _MyFHT59N3Par.NuclideLibsDirectory = _NuclideLibsDirectory
            End If
            MyDirectories.Add(_NuclideLibsDirectory)
            If _MyFHT59N3Par.SpectraDirectory <> "" Then
                _SpectraDirectory = _MyFHT59N3Par.SpectraDirectory
            Else
                _SpectraDirectory = MCADrive & ":\FHT59N3\Data\Spectra"
                _MyFHT59N3Par.SpectraDirectory = _SpectraDirectory
            End If
            MyDirectories.Add(_SpectraDirectory)

            If _MyFHT59N3Par.AnalyzationFilesDirectory <> "" Then
                _AnalyzationFilesDirectory = _MyFHT59N3Par.AnalyzationFilesDirectory
            Else
                _AnalyzationFilesDirectory = MCADrive & ":\FHT59N3\Data\Results"
                _MyFHT59N3Par.AnalyzationFilesDirectory = _AnalyzationFilesDirectory
            End If

            If _MyFHT59N3Par.AnalyzationN4242FilesDirectory <> "" Then
                _AnalyzationN4242FilesDirectory = _MyFHT59N3Par.AnalyzationN4242FilesDirectory
            Else
                _AnalyzationN4242FilesDirectory = MCADrive & ":\FHT59N3\Data\N42.42"
                _MyFHT59N3Par.AnalyzationN4242FilesDirectory = _AnalyzationN4242FilesDirectory
            End If
            MyDirectories.Add(_AnalyzationN4242FilesDirectory)

            If _MyFHT59N3Par.AlarmfilesDirectory <> "" Then
                _AlarmfilesDirectory = _MyFHT59N3Par.AlarmfilesDirectory
            Else
                _AlarmfilesDirectory = MCADrive & ":\FHT59N3\Data\Results\Alarmchecks"
                _MyFHT59N3Par.AlarmfilesDirectory = _AlarmfilesDirectory
            End If
            MyDirectories.Add(_AlarmfilesDirectory)

            If _MyFHT59N3Par.ReportFilesDirectory <> "" Then
                _ReportFilesDirectory = _MyFHT59N3Par.ReportFilesDirectory
            Else
                _ReportFilesDirectory = MCADrive & ":\FHT59N3\Data\Reports"
                _MyFHT59N3Par.ReportFilesDirectory = _ReportFilesDirectory
            End If
            MyDirectories.Add(_ReportFilesDirectory)

            If _MyFHT59N3Par.SensorRecordingDirectory <> "" Then
                _SensorRecordingDirectory = _MyFHT59N3Par.SensorRecordingDirectory
            Else
                _SensorRecordingDirectory = MCADrive & ":\FHT59N3\Data\SensorRecording"
                _MyFHT59N3Par.SensorRecordingDirectory = _SensorRecordingDirectory
            End If
            MyDirectories.Add(_SensorRecordingDirectory)

            If _MyFHT59N3Par.LogfilesDirectory <> "" Then
                _LogfilesDirectory = _MyFHT59N3Par.LogfilesDirectory
            Else
                _LogfilesDirectory = MCADrive & ":\FHT59N3\Data\LogFiles"
                _MyFHT59N3Par.LogfilesDirectory = _LogfilesDirectory
            End If
            MyDirectories.Add(_LogfilesDirectory)

            'Erstellen wenn nicht vorhanden
            For Each Dir As String In MyDirectories
                If Not System.IO.Directory.Exists(Dir) Then
                    System.IO.Directory.CreateDirectory(Dir)
                End If
            Next

            'Schreibzugriff testen
            For Each Dir As String In MyDirectories
                Dim ThisFile As System.IO.FileStream
                Try
                    ThisFile = System.IO.File.Create(Dir & "\Test_1234_qwertz_5678_asdfg_90.txt_test")
                    ThisFile.Close()
                    ThisFile = Nothing
                    If System.IO.File.Exists(Dir & "\Test_1234_qwertz_5678_asdfg_90.txt_test") Then
                        System.IO.File.Delete(Dir & "\Test_1234_qwertz_5678_asdfg_90.txt_test")
                    End If
                Catch ex As Exception
                    DirectoriesWithoutPermission = DirectoriesWithoutPermission & Dir & vbCrLf
                End Try
            Next
            If DirectoriesWithoutPermission <> "" Then
                GUI_ShowMessageBox(MSG_NoAccessToDirectories & vbCrLf & DirectoriesWithoutPermission, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

   

    ''' <summary>
    ''' If coonfiguration has been changed or read from COnfig file, then based on the configuration other working parameters have been set.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SYS_SetDerivedWorkParamsFromConfig()
        Try
            Dim j As Integer
            Dim FilterTimeOk As Boolean = True
            Dim MeasurementTimeOk As Boolean = True
            Dim TempFilterStepSize As Integer

            _minS = _SpectraDirectory & "\MIN.CNF"                     'Kurzdauer-Ainzelspektrum
            _stdS = _SpectraDirectory & "\STD.CNF"                     'Filterstanddauer-Spektrum
            _tagS = _SpectraDirectory & "\TAG.CNF"                     'Tagesspektrum
            _fernS = _SpectraDirectory & "\FERN.CNF"
            _fernkorS = _SpectraDirectory & "\FERNKOR.CNF"
            _nahKorS = _SpectraDirectory & "\NAHKOR.CNF"
            _naS = _SpectraDirectory & "\NAH.CNF"
            _na_mischS = _SpectraDirectory & "\NAH_MISCH.cnf"
            _WkpS = _SpectraDirectory & "\Wkp.cnf"
            _TempS = _SpectraDirectory & "\spektrum.cnf"
            _AusgabeSammelinfo = _AnalyzationFilesDirectory & "\" & "InfoCollection.dat" 'Neu Ver2157

            'TFHN-17
            If Not _MyControlCenter Is Nothing And _MyControlCenter.SYS_States.IntensiveMode Then
                TempFilterStepSize = _MyFHT59N3Par.FilterStepim
            Else
                TempFilterStepSize = _MyFHT59N3Par.FilterStepmm
            End If

            'hier pro Filterschritt 2 mm dazurechnen für die Abklingkurve des Schrittmotors, somit etwas weniger Filterschritte und eine konservative Haltung
            'lieber zu früh Signalisieren als zu spät!
            _NumberOfFilterStepsPerFilter = _TapeLength / (TempFilterStepSize + 2)

            If _OldFilterStepmm > 0 Then
                _MyFHT59N3Par.FilterSteps = _MyFHT59N3Par.FilterSteps * _OldFilterStepmm / TempFilterStepSize
            End If
            _OldFilterStepmm = TempFilterStepSize

            'Überprügung ob Filterstanddauer ein ganzzahliger Teiler von 24 ist
            'dies geschiet hier und nicht in der Parameter Klasse, weil sonst immer kreuzweise nachgeschaut werden müsste, damit keine
            'unzulässigen Kombinationen von Filterstands- und Messdauer zustande kommen
            If _MyFHT59N3Par.FilterTimeh > 24 Then
                _MyFHT59N3Par.FilterTimeh = 24
                FilterTimeOk = False
            End If
            Do   'Suche nach ganzzaligem Teiler von 24
                If _MyFHT59N3Par.FilterTimeh < 1 Then _MyFHT59N3Par.FilterTimeh = 1
                If (24 \ _MyFHT59N3Par.FilterTimeh) * _MyFHT59N3Par.FilterTimeh = 24 Then
                    Exit Do 'fistand% ganzzaliger Tailer
                End If
                _MyFHT59N3Par.FilterTimeh = _MyFHT59N3Par.FilterTimeh - 1
                FilterTimeOk = False
            Loop
            If Not FilterTimeOk Then
                GUI_ShowMessageBox(MSG_FilterTimeOutOfRange & _MyFHT59N3Par.FilterTimeh.ToString & "!!", "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            End If

            'Überprügung ob Messdauer ein ganzzahliger Teiler der Filterstandsdauer in Minuten ist
            'dies geschiet hier und nicht in der Parameter Klasse, weil sonst immer kreuzweise nachgeschaut werden müsste, damit keine
            'unzulässigen Kombinationen von Filterstands- und Messdauer zustande kommen
            If _MyFHT59N3Par.MeasurementTimemin < _MinimumMeasurementTimeMinutes Then
                _MyFHT59N3Par.MeasurementTimemin = _MinimumMeasurementTimeMinutes 'mindestens 10 min
                MeasurementTimeOk = False
            End If
            'Auswerteabstand ganzzaliger Teiler der Filterstanddauer:
            If _MyFHT59N3Par.MeasurementTimemin > _MyFHT59N3Par.FilterTimeh * 60 Then
                _MyFHT59N3Par.MeasurementTimemin = _MyFHT59N3Par.FilterTimeh * 60
                MeasurementTimeOk = False
            End If
            Dim k As Integer = _MyFHT59N3Par.FilterTimeh * 60 \ _MyFHT59N3Par.MeasurementTimemin
            Do While (k * _MyFHT59N3Par.MeasurementTimemin <> _MyFHT59N3Par.FilterTimeh * 60)
                _MyFHT59N3Par.MeasurementTimemin = _MyFHT59N3Par.MeasurementTimemin + 1
                k = _MyFHT59N3Par.FilterTimeh * 60 \ _MyFHT59N3Par.MeasurementTimemin
                MeasurementTimeOk = False
            Loop ' do-while-end */
            If Not MeasurementTimeOk Then
                GUI_ShowMessageBox(MSG_MeasurementTimeOutOfRange & _MyFHT59N3Par.MeasurementTimemin.ToString & "!!", "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            End If

            'Tagesminuten für die Filterschritte innerhalb eines Tages ausrechnen
            j = 1
            _NumberOfFilterStepsPerDay = 24 \ _MyFHT59N3Par.FilterTimeh
            ReDim _FilterStepMinutes(_NumberOfFilterStepsPerDay)
            For i As Integer = 0 To 23 Step _MyFHT59N3Par.FilterTimeh       'Filterwechselzeiten (min)
                _FilterStepMinutes(j) = (i + _MyFHT59N3Par.FilterTimeh + _MyFHT59N3Par.DayStartTime) * 60
                If _FilterStepMinutes(j) >= 1440 Then
                    _FilterStepMinutes(j) = _FilterStepMinutes(j) - 1440
                End If
                If _FilterStepMinutes(j) < 0 Then
                    _FilterStepMinutes(j) = 1440 - _FilterStepMinutes(j)
                End If
                j = j + 1
            Next i
            _NumberOfFilterStepsPerDay = j - 1                    'Anzal möglicher Filterwechsel/tag

            _AlarmThresholdFilterSteps = _NumberOfFilterStepsPerDay * 3 + 1      'Grenzwert für "Filterband get zu Ende"
            _AirFlowSumCounter = 0
            _AirFlowSum = 0
            _AirFlowMean = 0
            _AirFlowActual = 0

            If _MyFHT59N3Par.DisplayPoints Then
                frmMeasScreen.SpectralDisplay.DisplayPoints = True
                frmMCAParameter_GainStabiAdd.SpectralDisplay.DisplayPoints = True
            Else
                frmMeasScreen.SpectralDisplay.DisplayPoints = False
                frmMCAParameter_GainStabiAdd.SpectralDisplay.DisplayPoints = True
            End If

            SYS_ReadAlarmSettings()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub SYS_SaveRCCResults(ByVal Energy As Double, ByVal Efficiency As String)
        Try
            Dim File As String = _AnalyzationFilesDirectory & "\fht59n.wkp"
            Dim Lines() As String
            Dim FileContent As String = ""
            If System.IO.File.Exists(File) Then
                Lines = My.Computer.FileSystem.ReadAllText(File).Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                'Header
                FileContent = Lines(0) & vbCrLf
                'Neue Zeile gleich nach dem Header
                FileContent = FileContent & Format(Now, "dd.MM.yyyy") & ";" & Format(Now, "HH:mm:ss") & ";" & Format(_LastMeasurementTimeMin, "0") & ";" & Format(Energy, "0.0") & ";" & Efficiency & vbCrLf
                'Rest anhängen
                For i As Integer = 1 To Lines.Length - 1
                    FileContent = FileContent & Lines(i) & vbCrLf
                Next
            Else
                FileContent = ml_string(245, "Date;Time;Measurementtime[s];Energy[keV],Efficiency[%]") & vbCrLf
                FileContent = FileContent & Format(Now, "dd.MM.yyyy") & ";" & Format(Now, "HH:mm:ss") & ";" & Format(_LastMeasurementTimeMin, "0") & ";" & Format(Energy, "0.0") & ";" & Efficiency & vbCrLf
            End If
            My.Computer.FileSystem.WriteAllText(File, FileContent, False)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SYS_SaveMessageToCollectionFile(ByVal MSG As String)
        Try
            My.Computer.FileSystem.WriteAllText(_AusgabeSammelinfo, MSG, True)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_SaveReportsToCollectionFile(ByVal ReportFile As String)
        Try
            If System.IO.File.Exists(ReportFile) Then
                Dim RepFileContent As String = My.Computer.FileSystem.ReadAllText(ReportFile)
                My.Computer.FileSystem.WriteAllText(_AusgabeSammelinfo, RepFileContent, True)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

#End Region


    Private Function FlaggedEnumToString(flaggedEnumValue As [Enum]) As String
        Return flaggedEnumValue.ToString()
    End Function

    Private Function StringToFlaggedEnum(Of TEnum)(flaggedEnumString As String) As TEnum

        Try
            If String.IsNullOrEmpty(flaggedEnumString) Then
                'Gib ersten Enum-Eintrag zurück, der sollte immer der None-Value sein
                Return CType(Nothing, TEnum)
            End If

            Dim parsedEnum As TEnum = [Enum].Parse(GetType(TEnum), flaggedEnumString)
            Return parsedEnum
        Catch ex As Exception
            Return CType(Nothing, TEnum)
        End Try

    End Function

End Module
