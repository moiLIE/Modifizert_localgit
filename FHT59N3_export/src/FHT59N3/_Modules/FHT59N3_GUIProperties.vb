Module FHT59N3_GUIProperties

#Region "Misc"

    Public _ActualVisiblePanel As Panel
    Public _LeftHandOperation As Boolean = False
    Public _MyResouceManager As New Resources.ResourceManager(frmMain.GetType().Namespace + ".Resources", frmMain.GetType().Assembly)

    Public _AlarmOutSwitchableWithoutMaintenance As Boolean = False
    Public _ShowSpectrum As Boolean = False
    Public _ShowSystemStateInMainDisplay As Boolean = True
    Public _SuppressTimeAirflowTooLess As Integer = 0

    Public _LViewFont As New Font("Microsoft Sans Serif", 12, FontStyle.Regular)
    Public _LViewFont1 As New Font("Microsoft Sans Serif", 10, FontStyle.Regular)
    Public _MyOldDigInOutState As Integer = -1
    Public _DigInOutListFilled As Boolean = False
    Public _MsgListFilled As Boolean = False
    Public _MyOldSystemState As Integer = -1

    Public _InstanceOfFormMeasscreen As frmMeasScreen

#End Region

#Region "Colors"

    Public MYCOL_EXPECT_INPUT As Color = System.Drawing.Color.FromArgb(189, 203, 228)
    Public MYCOL_THERMOGREEN As Color = System.Drawing.Color.FromArgb(70, 140, 70)
    Public MYCOL_ENTRYFOCUS As Color = System.Drawing.Color.FromArgb(192, 255, 255)
    Public MYCOL_OK As Color = System.Drawing.Color.FromArgb(0, 192, 0)
    Public MYCOL_NotOK As Color = Color.Red
    Public MYCOL_WARNING As Color = Color.Yellow
    Public MYCOL_MESSAGE_CRITICAL As Color = System.Drawing.Color.FromArgb(255, 219, 0)

#End Region

#Region "Spectrum"

    Public Enum SpectraTypes
        ONLINE = 1
        OFFLINE = 2
    End Enum

    Public _SpectraFile As CanberraDataAccessLib.DataAccess
    Public _SpectraFileOpen As Boolean = False

#End Region

#Region "Message"
    Public Const MAXLISTENTRIES As Integer = 300
    Public isMsgListChanged As Boolean = False
    Public MsgListStatus As New List(Of String)
    Public MsgListStatusOn As New List(Of Integer)
    '0=Error-Status ging weg
    '1=Error-Status kommt
    '2=Alarm-Status ging weg
    '3=Alarm-Status kommt
    Public MsgListDate As New List(Of Date)

    Public MSG_MEASPROGSTARTED = ml_string(340, "Measurement program started.")
    Public MSG_MEASPROGCLOSED = ml_string(341, "Measurement program closed.")
    Public MSG_SPSTRANSFERERROR = ml_string(342, "Communication problems with SPS!")
    Public MSG_SPSONLINE = ml_string(601, "SPS is online again.")

    Public MSG_UPS_STATUS_ONBATTERY = ml_string(591, "UPS operation (on battery)")
    Public MSG_UPS_SNMP_NOT_REACHABLE = ml_string(592, "The monitoring software for UPS is not available!")
    Public MSG_UPS_SNMP_REACHABLE = ml_string(593, "UPS monitoring started, UPS status available")

    Public MSG_UPS_STATUS_PREPARING_SHUTDOWN = ml_string(619, "Preparing shutdown ({0} min)")
    Public MSG_UPS_STATUS_ONLINE = ml_string(611, "UPS operation (on line)")
    Public MSG_UPS_ACTION_SHUTDOWN = ml_string(612, "Application shutdown due to low UPS battery")



    Public MSG_SPSHVON = ml_string(343, "High voltage switched on")
    Public MSG_SPSHVOFF = ml_string(344, "High voltage switched off")
    Public MSG_DeadTime = ml_string(345, "Deadtime > 20%")
    Public MSG_DeadTimeOK = ml_string(346, "Deadtime normal.")
    Public MSG_TapeTransportDemanded = ml_string(347, "Tape transport demanded")
    Public MSG_NoTapeTransport = ml_string(348, "Tape not transported, torn?")
    Public MSG_TapeTransport = ml_string(349, "Tape transport ended")
    Public MSG_TapeNearEnd = ml_string(350, "Tape near its end")
    Public MSG_TapeChanged = ml_string(351, "Tape changed.")
    Public MSG_AlarmModeON = ml_string(425, "Alarm mode switched on")
    Public MSG_AlarmModeOFF = ml_string(426, "Alarm mode switched off")
    Public MSG_IntensiveModeON = ml_string(427, "Intensive mode switched on")
    Public MSG_IntensiveModeOFF = ml_string(428, "Intensive mode switched off")
    Public MSG_K40NotFound = ml_string(353, "K40-1460.8-keV-line not found.")
    Public MSG_K40DevToBig = ml_string(354, "Deviation of K40-1461-keV-line too large.")
    Public MSG_K40NotFoundOrTooSmall = ml_string(355, "K40-1460.8-keV-line too small (or not found).")
    Public MSG_K40ReCalibrated = ml_string(502, "K40-1460.8-keV-line recalibrated.")
    Public MSG_IdentifyingWithoutLines = ml_string(356, "Identifying: Analysis interrupted. Without peaks!!")
    Public MSG_BI214Recalib = ml_string(357, "BI-214 found, PB-214 is missing! Energy recalibration?")
    Public MSG_StartOfMaintenance = ml_string(358, "Start of maintenance")
    Public MSG_EndOfMaintenance = ml_string(359, "End of maintenance")
    Public MSG_RemoteMaintenance = ml_string(360, "Remote maintenance requested")
    Public MSG_TapeTorn = ml_string(361, "Tape torn, Bypass open")
    Public MSG_BypassOpen = ml_string(328, "Bypass open")
    Public MSG_BypassClosed = ml_string(362, "Bypass closed")
    Public MSG_CheckTempPressure = ml_string(363, "Check Temp./Pressure!")
    Public MSG_TempPressureOK = ml_string(364, "Temp./Pressure Ok")
    Public MSG_AirFlowLessThan1Cubic = ml_string(365, "Air flow < {0} m³/h!")
    Public MSG_AirFlowGreaterThan12Cubic = ml_string(366, "Air flow > {0} m³/h!")
    Public MSG_AirFlowOK = ml_string(367, "Air flow Ok!")
    Public MSG_N2ValueUnpl = ml_string(368, "Value of N2-filling-level unplausible (")
    Public MSG_N2NearEnd = ml_string(369, "Nitrogen near its end!")
    Public MSG_N2Refilled = ml_string(370, "Nitrogen refilled.")
    Public MSG_StepWidthToSmall = ml_string(371, "Step width too small. Set to 60 mm!!")
    Public MSG_MeasureTimeToSmall = ml_string(579, "Measure time too small. Set to 10 min!!")
    Public MSG_WantToExit = ml_string(372, "Do you really want to exit program?")
    Public MSG_EfficiencyCalibNearOK = ml_string(373, "Energy and efficiency calibration was successfull!")
    Public MSG_NoSpectrasAdded = ml_string(374, "Identifying: Analysis interrupted. No spectrum were added!")
    Public MSG_WantToSave = ml_string(375, "Do you want to save the measured spectrum?")
    Public MSG_DayStartTimeOutOfRange = ml_string(376, "Day start time out of range. Set to 0!!")
    Public MSG_FilterstepsOutOfRange = ml_string(377, "Number of Filtersteps out of range. Set to 1!!")
    Public MSG_N2ThresholdOutOfRange = ml_string(378, "N2 Threshold out of range. Set to 0!!")
    Public MSG_FilterTimeOutOfRange = ml_string(379, "Filter time out of range. Set to ")
    Public MSG_MeasurementTimeOutOfRange = ml_string(380, "Measurement time out of range. Set to ")
    Public MSG_ProgRestart = ml_string(386, "Program has to be restarted!")

    Public MSG_MinAirFlowAlertOfRange = ml_string(505, "The allowed range for the minimal air flow must be between 1 and 20 (m³/h)")
    Public MSG_MaxAirFlowAlertOfRange = ml_string(506, "The allowed range for the maximal air flow must be between 5 and 25 (m³/h)")
    Public MSG_MinHigherMaxAlertOfRange = ml_string(610, "The minimum range must be smaller than the maximum range")

    Public MSG_CaptureCycleDetectorTemperatureOfRange = ml_string(516, "The allowed range for the capture cycle of detector temperature must be between 1 and 60 (minutes)")
    Public MSG_EmergencyStopDetectOnlyWithTemperaruteDetection = ml_string(519, "The e-cooler emergency stop detection can only be activated if detector temperature is captured.")

    Public MSG_RecordingDetectorTemperaturIsRunning = ml_string(522, "Recording detector temperature is running")
    Public MSG_RecordingDetectorTemperaturIsDefect = ml_string(523, "Recording detector temperature is defect")

    Public MSG_NoAccessToDirectories = ml_string(451, "You are not allowed to access the following directories:")
    Public MSG_EnergyReCalibrated = ml_string(503, "Energy was recalibrated")
    Public MSG_SPSCHECKSUMRERROR = ml_string(504, "Checksum error received from SPS.")
    Public MSG_NoValidSPSIpNetworkAddress = ml_string(543, "The entered network address for TCP/IP to SPS is invalid")
    Public MSG_WrongCrystalTooWarmTempThreshold = ml_string(526, "Wrong value for temperature threshold 'Crystal too warm'")
    Public MSG_WrongCrystalWarmedUpTempThreshold = ml_string(529, "Wrong value for temperature threshold 'Crystal warmed up'")

    Public MSG_WantToSwitchOffEcooler = ml_string(530, "Do you really want to deactivate the E-cooler?")
    Public MSG_WantToSwitchOnEcooler = ml_string(531, "Do you really want to activate the E-cooler?")
    Public MSG_CannotActivateEcooler = ml_string(532, "The E-cooler cannot be activated at the moment...")

    Public MSG_FilterPrinterPortNotAvailable = ml_string(588, "The serial port {0} ist not available for filter printer!")

    Public MSG_SavedBackgroundMeasurement = ml_string(631, "The background spectrum was saved (in backgnd.cnf file)")

    Public MSG_SpectrumNotAddedToDaySpectrum = ml_string(651, "Spectrum was not added to day spectrum due to fatal status: ")

    Public MSG_DetectorChange = ml_string(664, "Attention: Switching the detector model might require additional changes to the system. Please follow the corresponding manual carefully.")
    Public MSG_TestCryoCool = ml_string(667, "Test whether the Cryo-Cool control is linked correctly?")

    Public Enum MessageStates
        GREEN = 0
        YELLOW = 1
        RED = 2
    End Enum

#End Region

End Module
