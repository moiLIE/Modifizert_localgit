Imports FHT59N3Core
Imports ThermoUtilities
Imports ThermoProtocols

Public Module FHT59N3_SystemProperties
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_SystemProperties.vb 427 2017-09-06 09:13:48Z marcel.bender $
    ' Title: system properties
    '
    ' Description:
    ' global data storage (public variables)
    '  - fixed configurations values (time, steps)
    '  - parameters
    '  - calculated values (temperature, airflow)
    '
    ' Note: the calculated alarms are stored in FHT59N3_SystemStates
    ' -------------------------------------------------------------

#Region "Const"

    Public Const _AlarmCheckPoints = 5      'Abstand [min] der Alarmprüfungen
    Public Const _MinMeasTimeBeforeAlarmCheck = 600       'Mindestmessdauer [s] vor 1. Alarmprüfung

    Public Const _SpectrumAnalyzationDuration = 1
    Public Const _AirFlowMeanThreshold = 6    'only used internally for mean value calculation
    Public Const _TapeLength = 42000   'Bandlänge [mm]
    Public Const _TimeBetweenAirFlowMeas = 5 'Abstand zum Abholen der Luftdurchsatzwerte bai Kalibrirung
    Public Const _N2HeatingTime As Integer = 60 'wie lange soll der Widerstand bezeiht werden (in Sekunden)
    Public Const _N2HeatingSteps As Integer = 60 'aller wieviel minuten soll gemessen werden (in Minuten)
    Public Const _SignalTact As Integer = 5 'aller wieviel Sekunden sollen Digitale und Analoge Werte abgeholt werden?

    'TFHN-17
    Public Const _MinimumMeasurementTimeMinutes = 10 'untere Grenze in Minuten für die Auswertung vom Spektrum

#End Region

#Region "Parameters"

    Public _FilterstepStartTime As Date

    Public _N2HeatingStarted As Date
    Public _TS_N2Heating As TimeSpan

    Public _N2HeatingTimePoint As Date
    Public _TS_N2HeatingTimePoint As TimeSpan

    Public Enum CalibTypes
        NoCalibration = 0
        CalibFar = 1
        CalibNear = 2
		'WKP mit CS137-Strahler (Fern+Nah-Kalibrierung)
        CalibRccCs137 = 3
		'WKP mit Mischstrahler (Geräte ab Nov15 für DWD und BAG)
        CalibRccMix = 4
        CalibFree = 5
        CalibNearMix = 6
        'Spektrum für Hintergrund aufnehmen
        CalibBackground = 7
    End Enum

    

    Public _CalibrationType As CalibTypes
    Public _LastCalibrationType As CalibTypes
    Public _NeedToManageNextCalibrationStep As Boolean = False

    Public Enum CalibAnalyzationTypes
        NoAnalyzation = 0
        EnergyFar = 1
        EfficiencyFar = 2
        EfficiencyNear = 3
        EfficiencyRccCs137 = 4
        EfficiencyRccMix = 5
        EnergyNearMix = 6
    End Enum
    Public _CalibAnalyzationType As CalibAnalyzationTypes

    Public _Measurement As Boolean   'Merker ob Messung läuft   
    Public _HourSumSpectrumIsNotEmpty As Boolean
    Public _ActualMeasurementTime As Integer
    Public _NumberOfFilterStepsPerDay As Integer
    Public _FilterStepMinutes() As Integer
    Public _AlarmThresholdFilterSteps As Integer
    Public _AirFlowSumCounter As Integer
    Public _AirFlowSum As Double
    Public _AirFlowMean As Double
    Public _AirFlowActual As Double
    Public _AirFlowMeasured As Double

    Public _TempZero As Double = -30            '(SPS digital value 0x0000 means -30, value 0x0FFF means +70)
    Public _DetectorTempZero As Double = -180   '(SPS digital value 0x0000 means ca -180°C, value 0x0FFF means +70°C, value 204 dec. means 0°C)

    Public _PressureFilter As Double            'Druckabfall an Band, accessed from SPS and calculated in SPS_AnalyzeAnalogStates
    Public _PressureBezel As Double             'Druckabfall an Blende, accessed from SPS and calculated in SPS_AnalyzeAnalogStates
    Public _PressureEnvironment As Double       'Umgebungsdruck, accessed from SPS and calculated in SPS_AnalyzeAnalogStates
    Public _Temperature As Double               'Temperatur (vermutlich -30°C bis +70°C), accessed from SPS and calculated in SPS_AnalyzeAnalogStates

    Public _ExternalTemperature As Double         'Zweite Sensortemperatur (nicht die Detektortemperatur!)
    Public _N2FillValue As Double               'N2 Füllstand, accessed from SPS and calculated in SPS_AnalyzeAnalogStates

    Public _DetectorTemperaturValue As Double _
                    = Double.MinValue           'Detectore temperature, accessed from SPS and calculated in SPS_AnalyzeAnalogStates

    Public _ASPSTR As String 'Messstartzait: jjmmttssnn
    Public _SSPRSTR2 As String 'Messstartzait für tagS: tt.mm.jj ss:nn
    Public _SSPRSTR3 As String 'Messstartzait für stdS: tt/mm/jj ss:nn
    Public _SSPRSTR4 As String 'Messendezait:  tt.mm.jj ss:nn
    Public _ASP1 As Double 'Anzal der Stunden sait 1.Jan.2000 biss Tagesnoibeginn(h)
    Public _ASP2 As Double 'gemittelter Luftdurchsatz
    Public _ASP3 As Double 'Anzahl der addierten Spektren
    Public _ASP4 As Double 'Bestaubungsspektrum = 1, kein Bestaubungsspektrum = 0 (Kalibrierung)
    Public _SpecCustomer As String = "" 'Customer Name aus Spektrum
    Public _SpecStationName As String = "" 'Station Name aus Spektrum
    Public _SpecStationID As String = "0" 'Station ID aus Spektrum

    'spektren dateien
    Public _minS As String
    Public _stdS As String
    Public _tagS As String
    Public _fernS As String
    Public _naS As String
    Public _na_mischS As String
    Public _fernkorS As String
    Public _nahKorS As String
    Public _WkpS As String
    Public _TempS As String

    'directories
    Public _CertificateDirectory As String
    Public _MonitorConfigDirectory As String
    Public _NuclideLibsDirectory As String
    Public _SpectraDirectory As String
    Public _AnalyzationFilesDirectory As String
    Public _AnalyzationN4242FilesDirectory As String
    Public _ReportFilesDirectory As String
    Public _SensorRecordingDirectory As String
    Public _LogfilesDirectory As String
    Public _AlarmfilesDirectory As String

    Public _AusgabeSammelinfo As String

    Public _EREALHour As Double
    Public _ELIVEHour As Double
    Public _ELIVE As Double
    Public _EREAL As Double
    Public _EREALOld As Double

    'hinzugefügt für ANSI4242-Auswertung
    Public _K40_FWHN As Double
    Public _K40AREA_PER_HOUR_ABS As Double
    Public _K40AREA_PER_HOUR_PERCENT As Double
    Public _K40AREA_DRIFT As Double

    Public _DoIhaveToExecuteStepsAfterFilterstep As Boolean = False
    Public _StepWidthTimeout As Integer = 300
    Public _SetNewFilterStep As Boolean = False
    Public _OldN2FillValue As Double = 0
    Public _NumberOfFilterStepsPerFilter As Integer
    Public _OldFilterStepmm As Integer = 0
    Public _ActivityCS137_Far As Double      'aktiwitaet des Cs137 imm mischstraler (fern)
    Public _ActivityCS137_Near As Double      'aktiwitaet des Cs137 imm ainzelstraler (na)
    Public _CertDate_Far As Date
    Public _CertDate_Near As Date

    Public _StartCalibration As Boolean = False
    Public _StartCalibAnalyzation As Boolean = False

    Public _LastMeasurementTimeMin As Integer

    Public _CalibAirFlow As Boolean = False
    Public _AirFlowSumCounterCalib As Integer
    Public _AirFlowTrue As Double
    Public _FactorBezelCalib As Double
    Public _NumberOfAirFlowMeasurementsCalib As Integer
    Public _StartOfAirFlowMeasurementCalib As Date

    'Datumsvergleich ist besser zu handeln
    Public _AnalyzeMinuteDate As Date
    Public _NextFilterStepMinuteDate As Date 'Zeitpunkt des naechsten Filterband Vorschubs
    Public _DayStartTimeDate As Date
    Public _AlarmCheckTimeDate As Date       'Zeitpunkt der nächsten Alarmpruefung

    Public _NetLogActive As Boolean = False
    Public _NetViewActive As Boolean = False
    Public _MyControlCenter As FHT59N3_ControlCenter

    Public _ApplPath As String                           'Applikationspfad
    Public _TimeZone As String = "UTC"
    Public _SubtractBkgSpectrum As Boolean
    Public _NetViewPath As String

    'Drucken (Ausdrucke)
    Public _MyprintFontName As String = "Courier New"
    Public _MyprintFontSize As Single = "12"
    Public _MyprintFontStyle As System.Drawing.FontStyle = FontStyle.Regular
    Public _MyPrintersettings As New System.Drawing.Printing.PrinterSettings
    Public _MyPagesettings As New System.Drawing.Printing.PageSettings
    Public WithEvents _MySerializer As New Config_File_Serialized   'Speichern der Druckerdaten als serielisiertes Objekt
    Public _MyUtillities As New ThermoUtilities.ThermoUtilities
    Public WithEvents _MyConfig As ThermoConfigFile_XML 'zum lesen meiner Konfiguration

    Public WithEvents _MyFHT59N3Par As New FHT59N3_SystemParams
    Public WithEvents _MyFHT59N3ParCopy As FHT59N3_SystemParams
    Public WithEvents _MyTemperatureRecorder As FHT59N3_DetectorTemperatureRecorder

    Public _MyEcoolerController As FHT59N3_EcoolerController
    Public _MyEcoolerControllerStarted As Boolean = False

    Public _IPLynx As String
    Public _SimulateLynxSystem As Boolean = False

    Public _MyTimeZoneCodes As New FHT59N3_TimeZoneCodes
    Public _ShowInfo As Boolean = False
    Public _Start As Boolean
    Public _MyProcesses As New ThermoUtilities.ThermoProcesses
    Public _EndProgram As Boolean = False
    Public _ForcedEndProgram As Boolean = False

    Public _FiltertstepTries As Integer = 0  'fail counter for filtersteps
    Public _Protocol As New ThermoDataControl_Protocol
    Public _RemoteMaintenance As Boolean = False
    Public _MenuEntryExitClicked As Boolean = False
    Public _MonitorStatesClicked As Boolean = False
    Public _WaitTimeBeforeK40Check As Integer = 1800
    Public _MyDigInOutState As Integer = -1
    Public _MyDigInOutStateOld As Integer = -1

    Public _TimeFilterstepEnded As Date

    Public _AirflowMeasErrorCount As Integer = 0

    'Unchangeable in program:
    Public _ReCalibrateK40LineIfNeeded As Boolean = False
    Public _ReCalibrateEnergyIfNeeded As Boolean = False

    'UPS Battery status
    'Wird in V2.0.2 nicht mehr verwendet: Public _UpsBatteryStatusLow As Boolean
    Public _UpsOnBattery As Boolean
    Public _TimeStampOnUpsBattery As DateTime = DateTime.Now
    Public _LastGuiMessageOutputOnUpsBattery As DateTime = DateTime.Now

#End Region

End Module
