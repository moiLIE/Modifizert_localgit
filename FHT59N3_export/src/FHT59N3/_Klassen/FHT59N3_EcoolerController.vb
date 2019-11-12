Imports FHT59N3Core.FHT59N3_SystemStates

Public Class FHT59N3_EcoolerController
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_EcoolerController.vb 307 2015-11-23 13:15:18Z marcel.bender $
    ' Title: controller for eColler state machine
    '
    ' Description:
    ' The state machine is responsible for emergecny shutdown
    ' of ecooler when crystal is too warm
    ' -------------------------------------------------------------

    Private _EnableEmergencyStopDetect As Boolean
    Private _EnableCapturingDetectorTemperature As Boolean
    Private _CrystalTooWarmTempThreshold As Integer
    Private _CrystalWarmedUpTempThreshold As Integer

    Private _detectorTemperaturPrevious As Double
    Public _EmergencyStopCoolingState As FHT59N3_EmergencyStopCoolingStates
    Private _ControllerEnabled As Boolean

    ' Um 'Zappelt' und möglicher erneuter Notabschaltung zu verhindern, gehen wir
    ' erst später wieder in Normalzustand (COOLING_ACTIVE)
    Private Const RESERVE_TEMP_CELSUIS_TO_NORMAL_COOLING = -4


    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks>not visible outside this class</remarks>
    Private Sub New()
        _EmergencyStopCoolingState = FHT59N3_EmergencyStopCoolingStates.UNKNOWN
        _ControllerEnabled = False
    End Sub

    ''' <summary>
    ''' This constructor setups the configured values for the eCooler state machine
    ''' </summary>
    ''' <param name="enableEmergencyStopDetect">true: eCooler exists and can be controlled</param>
    ''' <param name="enableCapturingDetectorTemperature">true: the detector temperatur is available</param>
    ''' <param name="crystalTooWarmTempThreshold">threshold "crystalTooWarmTemperature"</param>
    ''' <param name="crystalWarmedUpTempThreshold">threshold "crystalWarmedUpTemperature"</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal enableEmergencyStopDetect As Boolean, ByVal enableCapturingDetectorTemperature As Boolean,
                   ByVal crystalTooWarmTempThreshold As Integer, ByVal crystalWarmedUpTempThreshold As Integer)
        Me.New()

        _EnableEmergencyStopDetect = enableEmergencyStopDetect
        _EnableCapturingDetectorTemperature = enableCapturingDetectorTemperature
        _CrystalTooWarmTempThreshold = crystalTooWarmTempThreshold
        _CrystalWarmedUpTempThreshold = crystalWarmedUpTempThreshold

    End Sub

    Public Sub Start()
        _ControllerEnabled = True
    End Sub

    Public Sub Stopp()
        _ControllerEnabled = False
    End Sub

    Public Function IsECoolerTemperaturOk(ByVal detectorTemperaturCurrent As Double)

        If Not _ControllerEnabled Or Not _EnableEmergencyStopDetect Or Not _EnableCapturingDetectorTemperature Then
            Return False
        End If

        If detectorTemperaturCurrent = Double.MinValue Or detectorTemperaturCurrent = Double.MaxValue Then
            Return False
        End If

        If detectorTemperaturCurrent < _CrystalTooWarmTempThreshold Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' This is the "callback" functions for time-events or timer-ticks. It must be called regularly
    ''' </summary>
    ''' <param name="eCoolerRunningCurrent">current state of ecooler (false=off, true=running)</param>
    ''' <param name="detectorTemperaturCurrent">current detector temperatur</param>
    ''' <param name="emergencyStopCoolingState">returned state of state machine</param>
    ''' <returns>true if ecooler shall be turned on (conditions met), false if ecooler shall be shut off (due to conditions)</returns>
    ''' <remarks></remarks>
    Public Function CheckValuesOnTick(ByVal eCoolerRunningCurrent As Boolean, ByVal detectorTemperaturCurrent As Double,
                                 ByRef emergencyStopCoolingState As FHT59N3_EmergencyStopCoolingStates) As Boolean

        Dim shallECoolerRun As Boolean = eCoolerRunningCurrent

        If Not _ControllerEnabled Or Not _EnableEmergencyStopDetect Or Not _EnableCapturingDetectorTemperature Then
            ' all activity is disabled, no changes on data
            _EmergencyStopCoolingState = emergencyStopCoolingState
            _detectorTemperaturPrevious = detectorTemperaturCurrent
            Return eCoolerRunningCurrent
        End If


        'wir sind im gesicherten Kühlbereich....
        If detectorTemperaturCurrent < _CrystalTooWarmTempThreshold Then

            Dim backToCoolingStateTemperature As Double = (_CrystalTooWarmTempThreshold + RESERVE_TEMP_CELSUIS_TO_NORMAL_COOLING)

            'wir bauen ein paar Grad Sicherheit ein damit mögliche 'Toggler' bei der Diskretisierung nicht wieder
            'sofort zur Notabschaltung führt! (Aufgetreten bei TFS Erlangen am 24.2.2016 bei versehentlichem Stromaus)
            If detectorTemperaturCurrent < backToCoolingStateTemperature Then
                If _EmergencyStopCoolingState <> FHT59N3_EmergencyStopCoolingStates.COOLING_ACTIVE Then
                    'Systemmeldung generieren, die Kühlung ist wieder im Normalbereich
                    Dim txt As String = String.Format(ml_string(617, "ECooler has reached cooling temperature"),
                                                      detectorTemperaturCurrent, backToCoolingStateTemperature)
                    GUI_SetMessage(txt, MessageStates.GREEN)
                End If

                ' cooling seems to be running properly
                _EmergencyStopCoolingState = FHT59N3_EmergencyStopCoolingStates.COOLING_ACTIVE
                shallECoolerRun = eCoolerRunningCurrent
            End If

            'wir sind unterhalb der Abschaltschwelle geraten....
        ElseIf detectorTemperaturCurrent < _CrystalWarmedUpTempThreshold Then

            If Not _EmergencyStopCoolingState = FHT59N3_EmergencyStopCoolingStates.COOLING_PHASE_PREPARED Then

                If _EmergencyStopCoolingState <> FHT59N3_EmergencyStopCoolingStates.WARMING_PHASE_FORCED Then
                    'Systemmeldung generieren, die Kühlung ist nicht ausreichend
                    Dim txt As String = String.Format(ml_string(615, "Emergency detect of ECooler"), detectorTemperaturCurrent, _CrystalTooWarmTempThreshold)
                    GUI_SetMessage(txt, MessageStates.RED)
                End If

                ' emergency shut down
                _EmergencyStopCoolingState = FHT59N3_EmergencyStopCoolingStates.WARMING_PHASE_FORCED

                shallECoolerRun = False
            Else
                ' we are now in a new cooling phase
                shallECoolerRun = eCoolerRunningCurrent
            End If

        Else

            If _EmergencyStopCoolingState <> FHT59N3_EmergencyStopCoolingStates.COOLING_PHASE_PREPARED Then
                'Systemmeldung generieren, Aufwärmtemperatur wurde erreicht
                Dim txt As String = String.Format(ml_string(616, "ECooler has warmed up"), detectorTemperaturCurrent, _CrystalWarmedUpTempThreshold)
                GUI_SetMessage(txt, MessageStates.YELLOW)
            End If

            _EmergencyStopCoolingState = FHT59N3_EmergencyStopCoolingStates.COOLING_PHASE_PREPARED
            shallECoolerRun = eCoolerRunningCurrent

        End If

        emergencyStopCoolingState = _EmergencyStopCoolingState
        _detectorTemperaturPrevious = detectorTemperaturCurrent

        Return shallECoolerRun

    End Function


End Class
