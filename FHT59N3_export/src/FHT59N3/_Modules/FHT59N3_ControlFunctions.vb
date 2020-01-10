Imports System.Globalization
Imports System.IO
Imports System.IO.Ports
Imports System.Linq

Imports FHT59N3Core
Imports ThermoInterfaces
Imports ThermoLogging
Imports ThermoUtilities
Imports FHT59N3Core.FHT59N3_SystemStates
Imports System.Threading
Imports System.Threading.Tasks

Public Module FHT59N3_ControlFunctions
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_ControlFunctions.vb 434 2018-11-21 17:14:08Z burak.topbas $
    ' Title: control functions
    '
    ' Description:
    ' control of hardware and processing:
    '  - SPS
    '  - MCA
    '  - SYS
    ' -------------------------------------------------------------

    Private Const _OK As String = "OK"
    Private Const _ERROR As String = "ERROR (FHT59N3T)"
    Private Const _FUNC_DISABLED As String = "FUNCTION DISABLED"

    Private Const _COMMANDDOESNOTEXIST As String = " [-1] [Command does not exist]"
    Private Const _NOTENOUGHARGUMENTS As String = " [-2] [The given number of arguments is incorrect. Expected argument count is:]"
    Private Const _WRONGARGUMENT As String = " [-3] [Argument is wrong]"
    Private Const _FILTERSTEPINPROGRESS As String = " [-4] [Filterstep in progress]"
    Private Const _MinimalPlausibleTemperature As Double = -200
    Private Const _MaximalPlausibleTemperature As Double = 50
    Private Const _FactorFilterPrinterTimeMultiply As Integer = 3

    Private _FilterPrinterPort As SerialPort

    Private MediumCycle_LastCheck As DateTime = DateTime.Now

    Private _Last_Canberra_Detector_Temperature_Readback As DateTime

    ''' <summary>
    ''' Der E-Cooler muss beim Hochlauf des Programms einmalig gesetzt werden.
    ''' Dies ist nötig da die SPS keine Remanenz für den Digitalout hat. Wird die Anlage stromlos geschaltet
    ''' und fährt (automatisch) wieder hoch so sollte bei gültiger Detektortemperatur der E-Cooler sofort eingeschaltet
    ''' werden
    ''' </summary>
    ''' <remarks></remarks>
    Private _OneShotECoolerInit As Boolean = False

    Dim snmp As FHT59N3_SnmpCommunication
    Dim n4242Cleanup As FHT59N3_N4242_FileCleanup

#Region "SPS"

    Public Function SPS_SetMaintenanceOn() As String
        Try
            'schaltet Wartung ein
            _MyControlCenter.SYS_States.Maintenance = True
            MCA_StopMeasurement(False)
            _MyControlCenter.SPS_MaintenanceOn()

            GUI_SetMessage(MSG_StartOfMaintenance, MessageStates.YELLOW)
            _MyControlCenter.SPS_AlarmOff() 'Alarm aus
            _MyControlCenter.SPS_PumpOff() 'Pumpe aus
            _MyControlCenter.SPS_DetectorHeadOff()       'Baipass öffnen, Band frai
            GUI_SetMenus()
            _MyControlCenter.MDS_SaveAnalyzationResultsToDataServer(_MyControlCenter.MCA_Nuclides, _MyControlCenter.SYS_States, _AirFlowMean, 0, True) 'löschen! 
            Return _OK & " [MaintenanceOn]"
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    Public Function SPS_SetMaintenanceOff() As String
        Try
            _MyControlCenter.SYS_States.Maintenance = False
            _MyControlCenter.SPS_MaintenanceOff() 'nicht Wartung
            'Pumpe erst anschalten wenn Bandtransport ok, wird in digZustände sowieso schon gemacht V 2.1.60
            _MyControlCenter.SPS_DetectorheadOn()    'Band fest, Baipass geclossen
            GUI_SetMessage(MSG_EndOfMaintenance, MessageStates.GREEN)
            MCA_SetPreset()

            Dim minOfDayNow As Integer = MinutesOfDayNow()
            SYS_SynchronizeNextAnalyzationTime(minOfDayNow, "maintenance off")   'bestimmt nächste Auswerteminute
            SYS_SynchronizeNextAlarmCheckTime(minOfDayNow, "maintenance off")
            SYS_SynchronizeNextFilterStepTime(minOfDayNow, "maintenance off")
            SYS_SynchronizeNextDayStart()

            SYS_SetNextFilterStepAndStartMeasurement()           'Bandschritt-Forderung

            GUI_SetMenus()
            _MyControlCenter.MDS_SaveAnalyzationResultsToDataServer(_MyControlCenter.MCA_Nuclides, _MyControlCenter.SYS_States, _AirFlowMean, 0, True) 'löschen!
            Return _OK & " [MaintenanceOff]"
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    Public Function SPS_SetPump() As String
        Try
            If _MyControlCenter.SPS_PumpOnOff Then
                _MyControlCenter.SPS_PumpOff()
                Return _OK & " [PumpOff]"
                'Trace.TraceInformation(": SPS_SetPump: Pump Off!")
            Else
                _MyControlCenter.SPS_PumpOn()
                Return _OK & " [PumpOn]"
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    'Public Function SPS_SetEcooler() As String
    '    Try
    '        If _MyControlCenter.SPS_EcoolerOnOff Then
    '            _MyControlCenter.SPS_EcoolerOff()
    '            Return _OK & " [EcoolerOff]"
    '        Else
    '            _MyControlCenter.SPS_EcoolerOn()
    '            Return _OK & " [EcoolerOn]"
    '        End If
    '    Catch ex As Exception
    '        Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
    '        Return _ERROR
    '    End Try
    'End Function

    Public Function SPS_SetBypass() As String
        Try
            If _MyControlCenter.SPS_BypassOnOff Then
                If Not _MyControlCenter.SPS_MotorOnOff Then
                    _MyControlCenter.SPS_DetectorHeadOff() 'im falle eines filterschrittes darf ich den messkopf nicht schliessen!
                    Return _OK & " [Closed]"
                Else
                    Return _ERROR & _FILTERSTEPINPROGRESS
                End If
            Else
                _MyControlCenter.SPS_DetectorheadOn()
                Return _OK & " [Open]"
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    ''' <summary>
    ''' If coonfiguration has been changed or read from Config file, then based on the configuration certain SPS parameters
    ''' have to be set (again), see also SYS_SetDerivedWorkParamsFromConfig()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SYS_SetDerivedSPSParamsFromConfig()

        If _MyFHT59N3Par.AirFlowThroughPutCalculationMode = FHT59N3_SystemParams.AirFlowThroughPutCalculationModeEnum.ByTiaSps Then
            SPS_SetAirFlowParameters(_MyFHT59N3Par.FactorBezel, _MyFHT59N3Par.AirFlowWorking, _MyFHT59N3Par.AirFlowSetPoint)
        End If

    End Sub

    ''' <summary>
    ''' Set the Airflow bezel factor and other values of airflow calculation for the "control center"
    ''' </summary>
    ''' <param name="FactorBezel">Airflow bezel factor</param>
    ''' <param name="AirFlowMode"></param>
    ''' <param name="AirFlowSetPoint"></param>
    ''' <returns>_OK  or  _ERROR</returns>
    ''' <remarks>Only SPS with TCP/IP supports the airflow calculation</remarks>
    Public Function SPS_SetAirFlowParameters(ByVal FactorBezel As Double, ByVal AirFlowMode As Boolean, ByVal AirFlowSetPoint As Integer)
        Try
            _MyControlCenter.SPS_SetAirflowBezelFactor(FactorBezel)
            _MyControlCenter.SPS_SetAirflowSetPoint(AirFlowMode, AirFlowSetPoint)
            Return _OK & " [SetAirflowBezelFactor]"
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    ''' <summary>
    ''' Application function "set filter step" - A complete filter step without printing is done
    ''' </summary>
    ''' <returns>OK string or error string</returns>
    ''' <remarks>uses configuration parameters and calls _MyControlCenter.SPS_SetFilterstep
    ''' The wait on finish is done in the loop SPS_AnalyzeDigitalStates()</remarks>
    Public Function SPS_SetFilterstep() As String
        Try
            Dim StepWidthmm As Integer
            Dim Steps As Integer

            Dim alarmModeValid As Boolean = ChangeFilterTimeInAlarmMode()
            If alarmModeValid Or _MyControlCenter.SYS_States.IntensiveMode Then
                StepWidthmm = _MyFHT59N3Par.FilterStepim        'Mehr Schrittweite im Alarm-/Intensivbetrieb (TFHN-17)
                Steps = 2
                _StepWidthTimeout = 200
            Else
                StepWidthmm = _MyFHT59N3Par.FilterStepmm
                Steps = 1
                _StepWidthTimeout = 100
            End If

            If Not _MyControlCenter.SPS_MotorOnOff Then
                _MyControlCenter.SPS_SetFilterstep(StepWidthmm)
                _FilterstepStartTime = Now
                _MyFHT59N3Par.FilterSteps = _MyFHT59N3Par.FilterSteps - Steps
                Return _OK & " [Filterstep started]"
            Else
                Return _ERROR & _FILTERSTEPINPROGRESS
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function


    ''' <summary>
    ''' Application function "set filter step" phase 1 for filter printing
    ''' </summary>
    ''' <returns>OK string or error string</returns>
    ''' <remarks>uses configuration parameters and calls _MyControlCenter.SPS_SetFilterstep
    ''' The wait on finish is done in the loop SPS_AnalyzeDigitalStates()</remarks>
    Public Function SPS_SetFilterstepWithPrinter1() As String
        Try
            Dim StepWidthmm As Integer
            Dim Steps As Integer

            Dim alarmModeValid As Boolean = ChangeFilterTimeInAlarmMode()
            If alarmModeValid Or _MyControlCenter.SYS_States.IntensiveMode Then
                StepWidthmm = _MyFHT59N3Par.FilterStepim        'Mehr Schrittweite im Alarm-/Intensivbetrieb (TFHN-17)
                Steps = 2
                _StepWidthTimeout = 200
            Else
                StepWidthmm = _MyFHT59N3Par.FilterStepmm
                Steps = 1
                _StepWidthTimeout = 100
            End If

            ' the 1st half of moving 
            StepWidthmm = StepWidthmm - (StepWidthmm / 2) - _MyFHT59N3Par.PrinterPositionOffsetMM

            If Not _MyControlCenter.SPS_MotorOnOff Then
                _MyControlCenter.SPS_FilterstepStarted1stFlag = True

                'asynchronous call (processed by backend state machine)
                _MyControlCenter.SPS_SetFilterstep(StepWidthmm)
                _FilterstepStartTime = Now

                Return _OK & " [1st Filterstep started]"
            Else
                Return _ERROR & _FILTERSTEPINPROGRESS
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    ''' <summary>
    ''' Application function "set filter step" phase 2 for filter printing
    ''' </summary>
    ''' <returns>OK string or error string</returns>
    ''' <remarks>uses configuration parameters and calls _MyControlCenter.SPS_SetFilterstep
    ''' The wait on finish is done in the loop SPS_AnalyzeDigitalStates()</remarks>
    Public Function SPS_SetFilterstepWithPrinter2() As String
        Try
            Dim StepWidthmm As Integer
            Dim Steps As Integer

            Dim alarmModeValid As Boolean = ChangeFilterTimeInAlarmMode()
            If alarmModeValid Or _MyControlCenter.SYS_States.IntensiveMode Then
                StepWidthmm = _MyFHT59N3Par.FilterStepim        'Mehr Schrittweite im Alarm-/Intensivbetrieb (TFHN-17)
                Steps = 2
                _StepWidthTimeout = 200
            Else
                StepWidthmm = _MyFHT59N3Par.FilterStepmm
                Steps = 1
                _StepWidthTimeout = 100
            End If

            ' the 2nd half of moving 
            StepWidthmm = StepWidthmm - (StepWidthmm / 2) + _MyFHT59N3Par.PrinterPositionOffsetMM

            If Not _MyControlCenter.SPS_MotorOnOff Then
                _MyControlCenter.SPS_FilterstepStarted2ndFlag = True

                'asynchronous call (processed by backend state machine)
                _MyControlCenter.SPS_SetFilterstep(StepWidthmm)
                _FilterstepStartTime = Now

                _MyFHT59N3Par.FilterSteps = _MyFHT59N3Par.FilterSteps - Steps

                Return _OK & " [2nd Filterstep started and printed]"
            Else
                Return _ERROR & _FILTERSTEPINPROGRESS
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function


    Public Sub SPS_SetAlarm()
        Try
            If _MyControlCenter.SPS_AlarmRelaisOnOff Then
                _MyControlCenter.SPS_AlarmOff()
            Else
                _MyControlCenter.SPS_AlarmOn()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SPS_SetErrorIndication()
        Try
            If _MyControlCenter.SPS_ErrorOnOff Then
                _MyControlCenter.SPS_ErrorOff()
            Else
                _MyControlCenter.SPS_ErrorOn()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SPS_SetHeating()
        Try
            _MyControlCenter.SPS_HeatingOn()
            _N2HeatingStarted = Now
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub SPS_GetDigStates()
        Try
            _MyControlCenter.SPS_GetDigStates()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub


    Private Sub SPS_AnalyzeDigitalStates()
        Try
            Dim TS As TimeSpan
            Dim _TS_Filterstep As TimeSpan
            Dim TimeOut As Boolean = False
            Dim BypassClosedAfterFilterstep As Boolean = False
            Dim resultStr As String

            If _MyControlCenter.SPS_NewDigValuesAvailable Then

                _MyControlCenter.SPS_NewDigValuesAvailable = False

                If Not _MyControlCenter.SPS_MotorOnOff And _MyControlCenter.SPS_FilterstepStartedFlag Then
                    'Motor angehalten, filterbandschritt wurde von mir ausgelöst
                    _TS_Filterstep = Now.Subtract(_FilterstepStartTime)

                    If _TS_Filterstep.TotalSeconds >= 5 Then 'noch ein bissel warten

                        If (Not _MyControlCenter.SPS_FilterstepStarted1stFlag And Not _MyControlCenter.SPS_FilterstepStarted2ndFlag) Then
                            ' normal filter moval
                            _MyControlCenter.SPS_FilterstepStartedFlag = False
                            'In Verantwortung der SPS: _MyControlCenter.SPS_BypassOff()
                            _MyControlCenter.SYS_States.NoFilterstep = False
                            _TimeFilterstepEnded = Now
                            BypassClosedAfterFilterstep = True 'Merken das ich den bypass schliessen wollte, sonst geht die Pumpe gleich aus, denn diese
                            'Entscheidung wird aufgrund der AKTUELLEN Statusbits getroffen und diese sagen Bypass=offen
                            'Trace.TraceInformation(": SPS_AnalyzeDigitalStates: FilterstepOK: _DoIhaveToExecuteStepsAfterFilterstep=" & _DoIhaveToExecuteStepsAfterFilterstep.ToString)
                            If _DoIhaveToExecuteStepsAfterFilterstep Then
                                SYS_ExecuteStepsAfterFilterstep()
                            End If

                        ElseIf _MyControlCenter.SPS_FilterstepStarted1stFlag Then
                            ' filter moval before filter printing
                            _MyControlCenter.SPS_FilterstepStarted1stFlag = False

                            _MyControlCenter.SPS_FilterstepStartedFlag = False
                            _MyControlCenter.SYS_States.NoFilterstep = False
                            _TimeFilterstepEnded = Now

                            SYS_FilterBedrucken()

                            SPS_SetFilterstepWithPrinter2()

                        ElseIf _MyControlCenter.SPS_FilterstepStarted2ndFlag Then
                            ' filter moval after filter printing
                            _MyControlCenter.SPS_FilterstepStarted2ndFlag = False

                            _MyControlCenter.SPS_FilterstepStartedFlag = False
                            'In Verantwortung der SPS: _MyControlCenter.SPS_BypassOff()
                            _MyControlCenter.SYS_States.NoFilterstep = False
                            _TimeFilterstepEnded = Now
                            BypassClosedAfterFilterstep = True 'Merken das ich den bypass schliessen wollte, sonst geht die Pumpe gleich aus, denn diese
                            'Entscheidung wird aufgrund der AKTUELLEN Statusbits getroffen und diese sagen Bypass=offen
                            'Trace.TraceInformation(": SPS_AnalyzeDigitalStates: FilterstepOK: _DoIhaveToExecuteStepsAfterFilterstep=" & _DoIhaveToExecuteStepsAfterFilterstep.ToString)
                            If _DoIhaveToExecuteStepsAfterFilterstep Then
                                SYS_ExecuteStepsAfterFilterstep()
                            End If

                        End If

                    End If
                End If

                ' Check on Error (timeout etc.!)
                If _MyControlCenter.SPS_FilterstepStartedFlag Then

                    TS = Now.Subtract(_FilterstepStartTime)
                    If (TS.TotalSeconds > _StepWidthTimeout) Then
                        TimeOut = True
                    End If
                    If TimeOut Then 'Or _MyControlCenter.SPS_FilterRippedOnOff '(nur auswerten wenn Schrittfiltermechanik dran)
                        GUI_SetMessage(MSG_NoTapeTransport, MessageStates.RED)
                        _DoIhaveToExecuteStepsAfterFilterstep = False
                        'Trace.TraceInformation(": SPS_AnalyzeDigitalStates: FilterstepTimedOut: _DoIhaveToExecuteStepsAfterFilterstep=" & _DoIhaveToExecuteStepsAfterFilterstep.ToString)
                        _MyControlCenter.SPS_FilterstepStartedFlag = False
                        _TimeFilterstepEnded = Now
                        _MyControlCenter.SYS_States.NoFilterstep = True
                        'Filterband wurde als nicht transportiert angegegeben, und Messung nicht gestartet wegen Transferfehler
                        'sobald transferfehler weg ist, neu versuchen
                        _SetNewFilterStep = True
                    End If
                End If

                If _MyControlCenter.MCA_GetHVState And _MyControlCenter.SYS_States.HVOff Then
                    GUI_SetMessage(MSG_SPSHVON, MessageStates.GREEN)
                    _MyControlCenter.SYS_States.HVOff = False
                ElseIf Not _MyControlCenter.MCA_GetHVState And Not _MyControlCenter.SYS_States.HVOff Then
                    GUI_SetMessage(MSG_SPSHVOFF, MessageStates.RED)
                    _MyControlCenter.SYS_States.HVOff = True
                End If

                If _MyControlCenter.SPS_BypassOnOff And (Not _MyControlCenter.SYS_States.BypassOpen) And (Not _MyControlCenter.SPS_FilterstepStartedFlag And (Now.Subtract(_TimeFilterstepEnded).TotalSeconds > 20)) Then
                    _MyControlCenter.SYS_States.BypassOpen = True
                    GUI_SetMessage(MSG_BypassOpen, MessageStates.YELLOW)
                ElseIf Not _MyControlCenter.SPS_BypassOnOff And _MyControlCenter.SYS_States.BypassOpen Then
                    _MyControlCenter.SYS_States.BypassOpen = False
                    GUI_SetMessage(MSG_BypassClosed, MessageStates.GREEN)
                End If

                If _MyControlCenter.SPS_FilterRippedOnOff And Not _MyControlCenter.SYS_States.NoFilterstep Then      'wenn Band gerissen
                    _MyControlCenter.SYS_States.NoFilterstep = True          'Bandriss gemeldet
                    If _FiltertstepTries < 3 Then
                        _FiltertstepTries += 1 '3 mal wiederholen
                        'Ab und zu kommt es vor dass die SPS Fiterbandriss meldet,
                        'obwohl das Filterband io ist. Das führt dann dazu das die Pumpe stehen bleibt.
                        'Also lieber nochmal versuchen und auf Nummer sicher gehen
                        resultStr = SPS_SetFilterstep()
                    Else
                        _MyControlCenter.SPS_PumpOff()
                        _MyControlCenter.SPS_BypassOn() 'Bypass öffnen im Falle eines Filterbandrisses!
                        GUI_SetMessage(MSG_TapeTorn, MessageStates.RED)
                    End If
                ElseIf Not _MyControlCenter.SPS_FilterRippedOnOff And _MyControlCenter.SYS_States.NoFilterstep Then      'wenn Band gerissen
                    _MyControlCenter.SYS_States.NoFilterstep = False
                End If

                If _MyControlCenter.SPS_EcoolerOnOff And _MyControlCenter.SYS_States.EcoolerOff Then
                    ' clear ecooler alarm (SPS says it is running)
                    _MyControlCenter.SYS_States.EcoolerOff = False
                ElseIf Not _MyControlCenter.SPS_EcoolerOnOff And Not _MyControlCenter.SYS_States.EcoolerOff Then
                    ' raise ecooler alarm (SPS says it is off)

                    'Kriterium hinzufügen bei dem der E-Cooler-Status nicht gesetzt ist
                    'falls der E-Cooler garnicht über die SPS geschaltet wird:
                    If (_MyFHT59N3Par.EnableEmergencyStopDetect) Then
                        _MyControlCenter.SYS_States.EcoolerOff = True
                    End If

                End If

                If _MyControlCenter.SPS_RemoteMaintenance And Not _RemoteMaintenance Then
                    GUI_SetMessage(MSG_RemoteMaintenance, MessageStates.YELLOW)
                    _RemoteMaintenance = True
                    SPS_SetMaintenanceOn()
                ElseIf Not _MyControlCenter.SPS_RemoteMaintenance And _RemoteMaintenance Then
                    _RemoteMaintenance = False
                    SPS_SetMaintenanceOff()
                End If

                'TODO: Kann raus, da Kurti in der SPS sowieso den Bypass öffnet wenn ein Filterbandriss ansteht!
                'If (Not _MyControlCenter.SYS_States.Maintenance) And (Not _MyControlCenter.SPS_FilterstepStarted) And (Not BypassClosedAfterFilterstep) And (_MyControlCenter.SPS_BypassOnOff Or _MyControlCenter.SPS_FilterRippedOnOff) Then
                '    _MyControlCenter.SPS_PumpOff() 'Pumpe auscalten, wenn imm Normalbetrib Riss oder Baipass offen
                '    Trace.TraceInformation(": SPS_SetMaintenance: Pump Off!: Maintenance=" & _MyControlCenter.SYS_States.Maintenance.ToString & ": FilterStepStarted=" & _MyControlCenter.SPS_FilterstepStarted.ToString & ": BypassOnOff=" & _MyControlCenter.SPS_BypassOnOff.ToString & ": FilterRippedOnOff=" & _MyControlCenter.SPS_FilterRippedOnOff.ToString)
                'End If

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub SPS_BuildDigInOutState()
        Try
            Dim BitArray(6) As Integer
            If _MyControlCenter.SPS_MotorOnOff Then
                BitArray(0) = 1
            Else
                BitArray(0) = 0
            End If
            If _MyControlCenter.SPS_BypassOnOff Then
                BitArray(1) = 1
            Else
                BitArray(1) = 0
            End If
            If _MyControlCenter.SPS_PumpOnOff Then
                BitArray(2) = 1
            Else
                BitArray(2) = 0
            End If
            If _MyControlCenter.SPS_ErrorOnOff Then
                BitArray(3) = 1
            Else
                BitArray(3) = 0
            End If
            If _MyControlCenter.SPS_AlarmRelaisOnOff Then
                BitArray(4) = 1
            Else
                BitArray(4) = 0
            End If
            If _MyControlCenter.SPS_HeatingOnOff Then
                BitArray(5) = 1
            Else
                BitArray(5) = 0
            End If
            If _MyControlCenter.MCA_GetHVState Then
                BitArray(6) = 1
            Else
                BitArray(6) = 0
            End If
            _MyDigInOutState = BitArray(0) + BitArray(1) + BitArray(2) + BitArray(3) + BitArray(4) + BitArray(5) + BitArray(6)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub SPS_GetCalculations()
        Try
            If _MyFHT59N3Par.AirFlowThroughPutCalculationMode = FHT59N3_SystemParams.AirFlowThroughPutCalculationModeEnum.ByTiaSps Then
                'invoke the sending of command "caluculations request"
                _MyControlCenter.SPS_GetCalculations()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub


    Private Sub SPS_GetAnaStates()
        Try
            _MyControlCenter.SPS_GetAnaStates()
            _TS_N2Heating = Now.Subtract(_N2HeatingStarted)
            If (_MyFHT59N3Par.N2FillThreshold > 0) And (Not _MyControlCenter.SPS_HeatingOnOff) Then
                'im Falle der N2 Kühlung und wenn die Heizung nicht gerade läuft
                _TS_N2HeatingTimePoint = Now.Subtract(_N2HeatingTimePoint)
                'alle N2HeatingSteps Minuten die Heizung einschalten und Wert neu messen
                If _TS_N2HeatingTimePoint.TotalMinutes >= _N2HeatingSteps Then
                    _N2HeatingTimePoint = Now
                    SPS_SetHeating()
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub SPS_AnalyzeAnalogStates()
        Try

            If _MyControlCenter.SPS_NewAnaValuesAvailable Then

                _MyControlCenter.SPS_NewAnaValuesAvailable = False

                _PressureFilter = _MyControlCenter.SPS_PressureFilter * 500 / 4095           'Druckabfall an Band; Messbereich 0-500mbar; 12BitWert 0-4095
                _PressureBezel = _MyControlCenter.SPS_PressureBezel * 100 / 4095             'Druckabfall an Blende; Messbereich 0-100mbar; 12BitWert 0-4095
                _Temperature = _MyControlCenter.SPS_Temperature * 100 / 4095 + _TempZero     'Temperatur (-30°C bis +70°C)

                'Neu seit V2.0.3 (Juni 2017)
                _ExternalTemperature = _MyControlCenter.SPS_ExternalTemperature * 100 / 4095 'Externe Sensortemperatur (0° bis +100°C)

                'druckU# = 1013.0# * Exp(-hoeheNN# / 8000.0#) 'Normaldruck inn hoeheNN#
                _PressureEnvironment = (_MyControlCenter.SPS_PressureEnvironment * 1000 / 4095) + 500 'Umgebunsgdruck; Messbereich 500-1500mbar=1000mbar; Offset 500mbar; 12BitWert 0-4095

                '####################################################################
                'Luftdurchsatz
                '####################################################################
                _AirFlowMeasured = (Math.Sqrt(_PressureBezel * (_PressureEnvironment - _PressureFilter) / (_Temperature + 273.15)))  'momentaner Luftdurchsatz

                'norm air flow
                If (_MyFHT59N3Par.AirFlowThroughPutCalculationMode = FHT59N3_SystemParams.AirFlowThroughPutCalculationModeEnum.ByMonitorProgram) Then
                    _AirFlowActual = _MyFHT59N3Par.FactorBezel * _AirFlowMeasured
                Else
                    'Abholen von SPS Variablen
                    _AirFlowActual = CDbl(_MyControlCenter.SPS_AirThroughPutNormReceived()) / 4095 * 25
                End If


                If _MyFHT59N3Par.AirFlowWorking Then 'Norm kubik in betriebs kubik umrechnen
                    If (_MyFHT59N3Par.AirFlowThroughPutCalculationMode = FHT59N3_SystemParams.AirFlowThroughPutCalculationModeEnum.ByMonitorProgram) Then
                        _AirFlowActual = (1013 / _PressureEnvironment) * ((_Temperature + 273.15) / 273.15) * _AirFlowActual
                    Else
                        'Abholen von SPS Variablen
                        _AirFlowActual = CDbl(_MyControlCenter.SPS_AirThroughPutWorkingReceived()) / 4095 * 25
                    End If
                End If

                If Not _MyControlCenter.SPS_BypassOnOff Then 'wenn bypass geschlossen, dann wert zum Luftdurchsatz dazu addieren
                    _AirFlowSumCounter = _AirFlowSumCounter + 1
                    _AirFlowSum = _AirFlowSum + _AirFlowActual
                    _AirFlowMean = _AirFlowSum / _AirFlowSumCounter
                End If

                'Trace.TraceInformation("_MyControlCenter.SPS_PressureFilter: " & _MyControlCenter.SPS_PressureFilter.ToString & " : _MyControlCenter.SPS_PressureBezel: " & _MyControlCenter.SPS_PressureBezel.ToString & " : _MyControlCenter.SPS_PressureEnvironment: " & _MyControlCenter.SPS_PressureEnvironment.ToString & " : _MyControlCenter.SPS_Temperature: " & _MyControlCenter.SPS_Temperature.ToString)
                'Trace.TraceInformation("_PressureFilter: " & _PressureFilter.ToString & " : _PressureBezel: " & _PressureBezel.ToString & " : _PressureEnvironment: " & _PressureEnvironment.ToString & " : _Temperature: " & _Temperature.ToString)
                'Trace.TraceInformation("_AirFlowMeasured: " & _AirFlowMeasured.ToString & " : _AirFlowActual: " & _AirFlowActual.ToString & " : _AirFlowWorking: " & _MyFHT59N3Par.AirFlowWorking.ToString)
                'Trace.TraceInformation("_AirFlowSumCounter: " & _AirFlowSumCounter.ToString & " : _AirFlowSum: " & _AirFlowSum.ToString & " : _AirFlowMean: " & _AirFlowMean.ToString)

                'Changed for TFHN-21: defined by Mr. Silbermann on 12.11.2015: we use fixed value due to changed bezel geometry 
                Dim PressureBezelDelta As Double = 5

                Dim PressureFilterLimitReached As Boolean = ((250 - _PressureFilter) * (_PressureFilter - 40) < 0)
                Dim PressureBezelLimitReached As Boolean = ((_PressureBezel - PressureBezelDelta) * (100 - _PressureBezel) < 0)
                Dim TempLimitReached As Boolean = (_Temperature > (70 + _TempZero))
                Dim NoFilterStep As Boolean = (Not _MyControlCenter.SPS_FilterstepStartedFlag And (Now.Subtract(_TimeFilterstepEnded).TotalSeconds > 30))
                'doch nicht abklemmen in Wartungsmodus: Dim NoMaintenance As Boolean = Not _MyControlCenter.SYS_States.Maintenance

                'ALT: If (((250 - _PressureFilter) * (_PressureFilter - 40) < 0) Or ((_PressureBezel - 15) * (100 - _PressureBezel) < 0) Or (_Temperature > (70 + _TempZero))) And (Not _MyControlCenter.SPS_FilterstepStartedFlag And (Now.Subtract(_TimeFilterstepEnded).TotalSeconds > 20)) Then 'Nicht wenn gerade ein Filterschritt durchgeführt wird!

                If ((PressureFilterLimitReached Or PressureBezelLimitReached Or TempLimitReached) And (NoFilterStep)) Then 'Nicht wenn gerade ein Filterschritt durchgeführt wird!
                    _AirflowMeasErrorCount += 1
                    Trace.TraceInformation("_AirflowMeasErrorCount: " & _AirflowMeasErrorCount.ToString)
                    If _AirflowMeasErrorCount >= 10 Then
                        Trace.TraceInformation("_AirflowMeasErrorCount: Real error within pressure measurement")
                        _AirflowMeasErrorCount = 0
                        If Not _MyControlCenter.SYS_States.CheckTempPressure Then
                            _MyControlCenter.SYS_States.CheckTempPressure = True
                            GUI_SetMessage(MSG_CheckTempPressure, MessageStates.YELLOW) 'Temp./Druck pruefen
                        End If
                    End If
                Else      'd.h. ainzelgrösen nicht auserhalb des erwarteten
                    _AirflowMeasErrorCount = 0
                    If _MyControlCenter.SYS_States.CheckTempPressure Then
                        _MyControlCenter.SYS_States.CheckTempPressure = False
                        GUI_SetMessage(MSG_TempPressureOK, MessageStates.GREEN)
                    End If
                End If    'If-end (250# - wert(5)) * (490 ...


                If _AirFlowActual < _MyFHT59N3Par.MinAirFlowAlert Then  'momentanen Luftdurchsatz auf Minimum prüfen

                    'Nicht prüfen wenn im filterschritt (insbesondere wichtig bei Regelung des SINAMICS durch SPS Ahrens)
                    If NoFilterStep Then
                        If (Not _MyControlCenter.SYS_States.AirFlowLessThen1Cubic) Then

                            'Neues Feature in Version 2.0.3 (Wunsch BAK): Es soll eine Zeit einstellbar sein in der die Meldung 'Luftdurchsatz zu klein'
                            'nicht wiederholt auftaucht. Insbesondere wenn der Luftfilter sich im unteren Grenzbereich befindet und "mal under, mal über"
                            'der Grenze toggelt.

                            'muss am Schluß noch als Bedingung rein: Wunsch Armin Silbermann: 
                            '_MyControlCenter.SYS_States.Maintenance Or
                            If (_MyControlCenter.SYS_States._AirFlowLessThen1Cubic.ChangeTimestampOlderThan(_SuppressTimeAirflowTooLess)) Then

                                _MyControlCenter.SYS_States.AirFlowLessThen1Cubic = True
                                GUI_SetMessage(String.Format(MSG_AirFlowLessThan1Cubic, _MyFHT59N3Par.MinAirFlowAlert), MessageStates.RED)
                            End If
                        End If
                    End If
                Else   'momentaner Luftdurchsatz > WARNLUFT#
                    If _MyControlCenter.SYS_States.AirFlowLessThen1Cubic Then

                        Dim changeTimeStampTooOld As Boolean = _MyControlCenter.SYS_States._AirFlowLessThen1Cubic.ChangeTimestampOlderThan(_SuppressTimeAirflowTooLess)

                        _MyControlCenter.SYS_States.AirFlowLessThen1Cubic = False

                        'muss zurückgesetzt werden sonst müsste nach einer Normalisierung (von niedrig zu normal)
                        'wieder(die) 'suppress time' abgewartet werden bis erneuter Abfall gemeldet werden würde
                        If (changeTimeStampTooOld) Then
                            _MyControlCenter.SYS_States._AirFlowLessThen1Cubic.ChangeTimestamp = DateTime.MinValue
                        End If



                        If (Not _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic) Then

                            GUI_SetMessage(MSG_AirFlowOK, MessageStates.GREEN)

                        End If
                    End If
                End If




                If _AirFlowActual > _MyFHT59N3Par.MaxAirFlowAlert Then  'momentanen Luftdurchsatz auf Maximum prüfen

                    'Nicht prüfen wenn im filterschritt oder im wartungsmodus (insbesondere wichtig bei Regelung des SINAMICS durch SPS Ahrens)
                    If NoFilterStep Then
                        If (Not _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic) Then
                            _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic = True
                            GUI_SetMessage(String.Format(MSG_AirFlowGreaterThan12Cubic, _MyFHT59N3Par.MaxAirFlowAlert), MessageStates.RED)
                        End If
                    End If
                Else   'momentaner Luftdurchsatz > WARNLUFT#
                    If _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic Then
                        _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic = False
                        If (Not _MyControlCenter.SYS_States.AirFlowLessThen1Cubic) Then
                            GUI_SetMessage(MSG_AirFlowOK, MessageStates.GREEN)
                        End If
                    End If
                End If




                If Not _MyFHT59N3Par.EnableCapturingDetectorTemperature Then

                    ' cooling is done with N2
                    If (_MyFHT59N3Par.N2FillThreshold > 0) And (_MyControlCenter.SPS_HeatingOnOff) And (_TS_N2Heating.TotalSeconds >= _N2HeatingTime) Then
                        _N2FillValue = _MyControlCenter.SPS_N2Voltage 'Rowert: Stickstofffüllstand
                        'Heizung nach einer gewissen Zeit ausschalten
                        _MyControlCenter.SPS_HeatingOff()
                        If _N2FillValue < 1000 Then       'Wert plausibel?
                            If _OldN2FillValue > 1000 Then
                                GUI_SetMessage(MSG_N2ValueUnpl & Format(_OldN2FillValue, "0") & ")!", MessageStates.YELLOW)
                            End If
                        ElseIf _N2FillValue > _MyFHT59N3Par.N2FillThreshold Then
                            If Not _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                _MyControlCenter.SYS_States.N2FillingGoingLow = True
                                GUI_SetMessage(MSG_N2NearEnd, MessageStates.YELLOW)
                            End If
                        Else
                            If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                _MyControlCenter.SYS_States.N2FillingGoingLow = False
                                GUI_SetMessage(MSG_N2Refilled, MessageStates.GREEN)
                            End If
                        End If
                        _OldN2FillValue = _N2FillValue 'Rowert: Stickstofffüllstand
                    Else
                        _MyControlCenter.SYS_States.N2FillingGoingLow = False
                    End If  'If-end N2_Fuell% > 0 And gStatus(35) > 0 And

                ElseIf Not _MyFHT59N3Par.IsCanberraDetector Then
                    ' cooling is done via e-cooler (see spec 1.0c chapter 4.3.1)
                    ' get the temperature from the SPS buffer/logic (12 bit = 0 ... 4095)
                    Dim spsTemperatureAsDigitalValue As Double = _MyControlCenter.SPS_DetectorTemperature

                        'nasty workaround: Nachdem die Temperaturaufzeichnung erst nach dem Timout geschrieben wird,
                        'wird in der ersten Periode nichts aufgezeichnet, somit ergibt es keine richtige Darstellung
                        'des aufzeichnungszeitraumes. Hierfür wird ein erster Initialwert beim lesen geschrieben
                        'um eine Ausgangsbasis für Temperaturerfassungszeitraum zu haben.
                        If _MyTemperatureRecorder.Temperatures.Count = 0 Then
                            _MyTemperatureRecorder.StoreNewTempertaureEntry(_DetectorTemperaturValue)
                        End If




                        _DetectorTemperaturValue = ConvertDigitalValueToDetectorTemperature(spsTemperatureAsDigitalValue)

                        ' The detector temperature recording needs also the information if spsTemperatureAsDigitalValue has min/max value (0x0000 or 0xffff)
                        If spsTemperatureAsDigitalValue = 0 Or spsTemperatureAsDigitalValue = 4095 Then
                            If Not _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                ' alarm appears
                                _MyControlCenter.SYS_States.N2FillingGoingLow = True
                                GUI_SetMessage(MSG_RecordingDetectorTemperaturIsDefect, MessageStates.YELLOW)
                            End If
                            ' for detector temperature recording, that temperature sensor is not online
                            _DetectorTemperaturValue = Double.MinValue
                        Else
                            If _MyFHT59N3Par.EcoolerEnabled Then
                                If _DetectorTemperaturValue < _MinimalPlausibleTemperature Or _DetectorTemperaturValue > _MaximalPlausibleTemperature Then
                                    If Not _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                        ' alarm appears
                                        _MyControlCenter.SYS_States.N2FillingGoingLow = True
                                        GUI_SetMessage(MSG_RecordingDetectorTemperaturIsDefect, MessageStates.YELLOW)
                                    End If
                                Else
                                    If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                        ' alarm disappears
                                        _MyControlCenter.SYS_States.N2FillingGoingLow = False
                                        GUI_SetMessage(MSG_RecordingDetectorTemperaturIsRunning, MessageStates.GREEN)
                                    End If
                                End If
                            End If
                        End If

                        ' The detector temperature recording must be informed that SPS communication failed
                        If _MyControlCenter.SYS_States.DataTransferError Then
                            _DetectorTemperaturValue = Double.MinValue
                        End If
                    End If

                Else
                    ' The detector temperature recording must be informed that SPS communication failed
                    If _MyControlCenter.SYS_States.DataTransferError Then
                        _DetectorTemperaturValue = Double.MinValue
                    End If
                End If

            'Read Temperature of the Canberra detector
            If _MyFHT59N3Par.EnableCapturingDetectorTemperature And _MyFHT59N3Par.IsCanberraDetector Then

                'Only Read the temperature once in a while:
                If (DateTime.Now - _Last_Canberra_Detector_Temperature_Readback).TotalMinutes > _MyFHT59N3Par.CaptureCycleDetectorTemperature / 3 Then
                    _Last_Canberra_Detector_Temperature_Readback = DateTime.Now

                    ' Read the temperature
                    _DetectorTemperaturValue = _MyControlCenter.iPA_DetectorTemperature(_MyFHT59N3Par.iPATemperatureLog)

                    ' The detector temperature recording needs also the information if the temperature readback failed
                    If _DetectorTemperaturValue = Double.MinValue Then
                        If Not _MyControlCenter.SYS_States.N2FillingGoingLow Then
                            ' alarm appears
                            _MyControlCenter.SYS_States.N2FillingGoingLow = True
                            GUI_SetMessage(MSG_RecordingDetectorTemperaturIsDefect, MessageStates.YELLOW)
                        End If
                    Else
                        If True Then '_MyFHT59N3Par.EcoolerEnabled Then
                            If _DetectorTemperaturValue < _MinimalPlausibleTemperature Or _DetectorTemperaturValue > _MaximalPlausibleTemperature Then
                                If Not _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                    ' alarm appears
                                    _MyControlCenter.SYS_States.N2FillingGoingLow = True
                                    GUI_SetMessage(MSG_RecordingDetectorTemperaturIsDefect, MessageStates.YELLOW)
                                End If
                            Else
                                If _MyControlCenter.SYS_States.N2FillingGoingLow Then
                                    ' alarm disappears
                                    _MyControlCenter.SYS_States.N2FillingGoingLow = False
                                    GUI_SetMessage(MSG_RecordingDetectorTemperaturIsRunning, MessageStates.GREEN)
                                End If
                            End If
                        End If
                    End If


                    'nasty workaround: Nachdem die Temperaturaufzeichnung erst nach dem Timout geschrieben wird,
                    'wird in der ersten Periode nichts aufgezeichnet, somit ergibt es keine richtige Darstellung
                    'des aufzeichnungszeitraumes. Hierfür wird ein erster Initialwert beim lesen geschrieben
                    'um eine Ausgangsbasis für Temperaturerfassungszeitraum zu haben.
                    If _MyTemperatureRecorder.Temperatures.Count = 0 Then
                        _MyTemperatureRecorder.StoreNewTempertaureEntry(_DetectorTemperaturValue)
                    End If
                End If
            End If

        Catch ex As Exception
            Trace.TraceError("MCA_CheckIfMeasurementDone crashed: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Function ConvertDigitalValueToDetectorTemperature(ByVal digiValue As Double) As Double
        If _MyFHT59N3Par.IsCanberraDetector Then
            'Unnecessary for iPA readback
            Return digiValue
        End If
        'Ortec detector:
        Dim temperature As Double = Double.MinValue
        Dim voltage As Double = Double.MinValue
        Dim samples As Double = 409
        Dim voltageRange As Double = 1000
        Dim samplesPerVolt As Double = samples / voltageRange
        voltage = digiValue / samplesPerVolt
        temperature = ((_MyFHT59N3Par.ZeroVoltsTemperature * (-1) * voltage) / _MyFHT59N3Par.ZeroDegreesVoltage) + _MyFHT59N3Par.ZeroVoltsTemperature
        'temperature = (voltage - 499.33) / 2.0217

        Return temperature
    End Function


    Private Sub SPS_CheckAndControlEcoolerEmergency()
        Dim currentEcoolerCoolingState As FHT59N3_EmergencyStopCoolingStates = _MyControlCenter.SYS_States.EmergencyStopCoolingState
        Dim currentEcoolerIsRunning As Boolean = Not _MyControlCenter.SYS_States.EcoolerOff
        Dim shallECoolerRun As Boolean

        If _MyEcoolerController IsNot Nothing Then
            If Not _MyEcoolerControllerStarted And _DetectorTemperaturValue > Double.MinValue Then
                _MyEcoolerController.Start()
                _MyEcoolerControllerStarted = True
            End If

            shallECoolerRun = _MyEcoolerController.CheckValuesOnTick(currentEcoolerIsRunning, _DetectorTemperaturValue,
                                                   _MyControlCenter.SYS_States.EmergencyStopCoolingState)

            'liefert False falls nicht alle Rahmenparameter passen (Ecooler-Prüfung aktiv, Temp im Bereich usw.)...
            Dim hasNormalECoolerTemp = _MyEcoolerController.IsECoolerTemperaturOk(_DetectorTemperaturValue)

            If currentEcoolerIsRunning And Not shallECoolerRun Then
                ' emergency shutdown of ecooler
                _MyControlCenter.SPS_EcoolerOff()
                'Initialisierung ist auch in diesem Fall beendet, der ECooler-Zustand wurde explizit gesetzt
                _OneShotECoolerInit = True
            ElseIf ((Not _OneShotECoolerInit) And hasNormalECoolerTemp) Then
                _MyControlCenter.SPS_EcoolerOn()
                _OneShotECoolerInit = True
            End If


            If currentEcoolerCoolingState <> _MyControlCenter.SYS_States.EmergencyStopCoolingState Then
                SYS_WriteSettings()
            End If
        End If
    End Sub


    Private Sub SPS_CheckTransferState()
        Try
            If (_MyControlCenter.SPS_TransferState = FHT59N3_SPSStatemachine.TransferStates.TS_ERROR) And (Not _MyControlCenter.SYS_States.DataTransferError) Then
                _MyControlCenter.SYS_States.DataTransferError = True
                GUI_SetMessage(MSG_SPSTRANSFERERROR, MessageStates.RED)

            ElseIf _MyControlCenter.SPS_TransferState = FHT59N3_SPSStatemachine.TransferStates.TS_OK Then

                If (_MyControlCenter.SYS_States.DataTransferError) Then
                    GUI_SetMessage(MSG_SPSONLINE, MessageStates.GREEN)
                End If
                _MyControlCenter.SYS_States.DataTransferError = False

                If _SetNewFilterStep Then
                    _SetNewFilterStep = False
                    SYS_SetNextFilterStepAndStartMeasurement()
                End If

            ElseIf _MyControlCenter.SPS_TransferState = FHT59N3_SPSStatemachine.TransferStates.TS_NAK Then

                _MyControlCenter.SYS_States.DataTransferError = True
                If (_MyFHT59N3Par.SPSOutputChecksumError) Then
                    GUI_SetMessage(MSG_SPSCHECKSUMRERROR, MessageStates.RED)
                End If
                Trace.TraceWarning("Checksum error received from SPS!")

            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

#End Region

#Region "MCA"

    Private Sub MCA_StartNormalMeasurement(ByVal MeasurementTimemin As Integer, Optional ByVal CountToRealTime As Boolean = True)
        Try
            _LastMeasurementTimeMin = MeasurementTimemin
            _MyControlCenter.MCA_StartMeasurement(MeasurementTimemin, CountToRealTime, True)
            GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)
            _Measurement = True
            GUI_ShowNextAnalyzationMinute()
            GUI_ShowNextFilterStepTime()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub MCA_RestartNormalMeasurement(ByVal ShowNextAnalyzationMinute As Boolean, Optional ByVal MeasurementTimemin As Integer = 0)
        Try
            If MeasurementTimemin > 0 Then
                _LastMeasurementTimeMin = MeasurementTimemin
            End If
            _MyControlCenter.MCA_ReStartMeasurement(MeasurementTimemin)
            _Measurement = True
            If ShowNextAnalyzationMinute Then
                GUI_ShowNextAnalyzationMinute()
                GUI_ShowNextFilterStepTime()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub MCA_StopMeasurement(ByVal Pause As Boolean)
        Try
            _MyControlCenter.MCA_StopMeasurement(Pause)
            _Measurement = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub MCA_StartCalibrationMeasurement(ByVal MeasurementTimemin As Integer)
        Try
            'Kalibriermessung starten
            'Um welche Messung es geht wird sich schon in frmCalibrationMenu gemerkt
            _LastMeasurementTimeMin = MeasurementTimemin
            _MyControlCenter.MCA_StopMeasurement(False)

            If (_CalibrationType = CalibTypes.CalibBackground) Then
                SPS_SetFilterstep()
                Dim maxWait As Integer = 10 * 1000
                Dim elapsed As Integer = 0
                While (elapsed < maxWait)
                    elapsed += 100
                    Thread.Sleep(100)
                    Application.DoEvents()
                End While
            End If

            _MyControlCenter.MCA_StartMeasurement(MeasurementTimemin, False, False)
            GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, True, frmMeasScreen.SpectralDisplay)
            _Measurement = True
            _NeedToManageNextCalibrationStep = True
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub




    Public Sub MCA_StopCalibrationMeasurement()
        Try
            'TODO_PSA: Check whether spectrum is deleted here after free calib
            If _LastCalibrationType = CalibTypes.CalibFree Or _LastCalibrationType = CalibTypes.CalibBackground Then
                MCA_StopMeasurement(True)
            Else
                MCA_StopMeasurement(False)
            End If

            _CalibrationType = CalibTypes.NoCalibration
            _LastCalibrationType = CalibTypes.NoCalibration
            _CalibAnalyzationType = CalibAnalyzationTypes.NoAnalyzation
            _NeedToManageNextCalibrationStep = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Function MCA_SetHighVoltage() As String
        Try
            If _MyControlCenter.MCA_GetHVState Then
                _MyControlCenter.MCA_SetHVOff()
                Return _OK & " [HVOff]"
            Else
                _MyControlCenter.MCA_SetHVOn()
                Return _OK & " [HVOn]"
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    Private Sub MCA_CheckIfMeasurementDone()
        Try
            'ehemals sub takterNormal_Timer!

            _SSPRSTR4 = Format(Now, "dd.MM.yy HH:mm")
            If (_MyControlCenter.MCA_LiveMeasTime = 0) Or (Not _Measurement) Then
                Exit Sub
            End If

            If Not _MyControlCenter.SYS_States.Maintenance Then 'ich bin in der normalen Messung

                'Alarmprüfung
                'nach einem filterschritt mindestens 10 min warten bis zur ersten Alarmprüfung
                If _MyControlCenter.MCA_RealMeasTime > _MinMeasTimeBeforeAlarmCheck Then     '1.Alarmprüfung frühestens nach _MinMeasTimeBeforeAlarmCheck s
                    If Now >= _AlarmCheckTimeDate Then
                        MCA_StopMeasurement(True) 'Pause
                        _MyControlCenter.MCA_SaveSpectrum(_minS, True)
                        MCA_RestartNormalMeasurement(False)      'weiter messen

                        Dim analyzer = New FHT59N3_ComputeSpectrumAnalyze
                        analyzer.MCA_AnalyzeSpectrum(_minS, 0)          'Alarmprüfung

                        Try
                            'FHTNT-56: Beim neuen Genie2K 3.3 kann es anscheinend vorkommen das die Spektrumanzeige nach einer Auswertung
                            'schwarz bleibt. Daher wird proforma eine "Auffrischung" gemacht. Das flackert zwar aber besser als ein schwarzes Spektrum
                            GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)
                        Catch ex As Exception

                        End Try
                        SYS_SynchronizeNextAlarmCheckTime(MinutesOfDayNow(), Nothing)
                    End If
                Else
                    SYS_SynchronizeNextAlarmCheckTime(MinutesOfDayNow(), Nothing)
                End If

                'wenn Auswertezeitpunkt größer als Filterwechselzeit wird nie Filterschritt ausgelöst! 
                If (Now >= _AnalyzeMinuteDate) Or (_MyControlCenter.MCA_AquireDone) Then

                    Trace.TraceWarning("MCA_CheckIfMeasurementDone: detected analyze trigger!")

                    'MessendeZeitpunkt erreicht
                    Dim MeasurementStopped As Boolean = False
                    If (Now < _AnalyzeMinuteDate) And (_MyControlCenter.MCA_AquireDone) Then
                        'manchmal ist der Lynx angeblich schon fertig, weil er die voreingestellte Zeit nicht richtig verstanden hat
                        Trace.TraceError("_MyControlCenter.MCA_AquireDone ist TRUE, obwohl der Analyse-Zeitpunkt ({0}) noch nicht erreicht ist", _AnalyzeMinuteDate)
                        MCA_StopMeasurement(False) 'Messung hier komplett stoppen
                        MeasurementStopped = True
                    End If

                    If Not _MyControlCenter.MCA_AquireDone Then
                        MCA_StopMeasurement(True)
                    End If

                    _Measurement = False
                    MCA_ExecutePreAnalyzationSteps(MeasurementStopped)
                Else
                    Exit Sub                     'Messung noch nicht beendet
                End If

            Else 'ich bin in der Kalibrierung oder der WKP

                If (_MyControlCenter.MCA_LiveMeasTime >= _LastMeasurementTimeMin * 60) Or (_MyControlCenter.MCA_AquireDone) Then
                    If Not _MyControlCenter.MCA_AquireDone Then
                        MCA_StopMeasurement(True)
                    End If
                    _Measurement = False
                    MCA_ManageCalibrationSteps(False)
                Else
                    Exit Sub        'Messung noch nicht beendet
                End If

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' Initiert Filterband-Vorschub, wenn die Zeit erreicht ist. Es werden abhängig von den Messzeiten/Filterzeiten
    ''' Aktionen veranlasst.
    ''' </summary>
    ''' <param name="MeasurementStopped">If TRUE: MCA_StartNormalMeasurement</param>
    ''' <remarks></remarks>
    Public Sub MCA_ExecutePreAnalyzationSteps(ByVal MeasurementStopped As Boolean)
        Try
            Trace.TraceWarning("Entering MCA_ExecutePreAnalyzationSteps")
            'ehemals ferwaltg!

            Dim cnfNB As New CanberraDataAccessLib.DataAccess
            Dim cnfNT As New CanberraDataAccessLib.DataAccess
            Dim stdCnf As New CanberraDataAccessLib.DataAccess
            Dim tagCnf As New CanberraDataAccessLib.DataAccess

            Dim NB As String
            Dim NT As String


            If _MyControlCenter.MCA_RealMeasTime / _MyControlCenter.MCA_LiveMeasTime > 1.2 Then
                If Not _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent Then
                    GUI_SetMessage(MSG_DeadTime, MessageStates.YELLOW)
                    _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent = True
                End If
            Else
                If _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent Then
                    GUI_SetMessage(MSG_DeadTimeOK, MessageStates.GREEN)
                    _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent = False
                End If
            End If

            _MyControlCenter.MCA_GetParamsFromActualSpectrum(_ASPSTR, _SSPRSTR3)
            _MyControlCenter.MCA_SetParamsForActualSpectrum("m³/h", _AirFlowMean, 1, 1, _SSPRSTR4)
            'kopieren des detEbin nach stdS mit ELIVE, EREAL, SSPRSTR3(Bestaubungsstart), SSPRSTR4(Messende)
            _MyControlCenter.MCA_SaveSpectrum(_stdS, True)

            Trace.TraceWarning("MCA_ExecutePreAnalyzationSteps: current: " + Now.ToString("dd.MM.yy HH:mm"))
            Trace.TraceWarning("MCA_ExecutePreAnalyzationSteps: _AnalyzeMinuteDate: " + _AnalyzeMinuteDate.ToString("dd.MM.yy HH:mm"))
            Trace.TraceWarning("MCA_ExecutePreAnalyzationSteps: _NextFilterSteptime: " + _NextFilterStepMinuteDate.ToString("dd.MM.yy HH:mm"))

            Dim analyzer = New FHT59N3_ComputeSpectrumAnalyze

            If _NextFilterStepMinuteDate <> _AnalyzeMinuteDate Then
                Trace.TraceWarning("MCA_ExecutePreAnalyzationSteps: mache Auswertung")

                'Mache die Auswertung
                If _MyControlCenter.MCA_RealMeasTime >= _ActualMeasurementTime * 60 Then
                    _MyControlCenter.MCA_ClearDetector()
                    _EREALOld = 0
                End If
                GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)
                _Measurement = True
                GUI_SetMessage(ml_string(214, "Restart:   ") & Format(Now, "dd.MM.yy HH:mm") & " h", MessageStates.GREEN)
                SYS_SynchronizeNextAnalyzationTime(MinutesOfDayNow(), "MCA_ExecutePreAnalyzationSteps nur Auswertung")   'bestimmt nächste Auswerteminute

                If MeasurementStopped Then
                    MCA_StartNormalMeasurement(_ActualMeasurementTime, False)
                Else
                    MCA_RestartNormalMeasurement(True, _ActualMeasurementTime)
                End If
                analyzer.MCA_AnalyzeSpectrum(_stdS, 1)                  'abgelaufene Filterstanddauer
                GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)

            Else
                Trace.TraceWarning("MCA_ExecutePreAnalyzationSteps: mache Filterwechsel und Tagesspektrum")

                'fwxlmin% = auswmin%
                Dim NoFilterStep_Old As Boolean = _MyControlCenter.SYS_States.NoFilterstep 'neu in V. 2.1.60 'merken, bei der nächsten Zeile kann sich dieser Status ändern

                SYS_SetNextFilterStepAndStartMeasurement()   'Bandschritt fordern (= 5 3 1 ) + Messstart

                Dim minOfDayNow As Integer = MinutesOfDayNow()
                SYS_SynchronizeNextAnalyzationTime(minOfDayNow, "ExecutePreAnalyzationSteps mit Filter")   'bestimmt nächste Auswerteminute
                SYS_SynchronizeNextAlarmCheckTime(minOfDayNow, "ExecutePreAnalyzationSteps mit Filter")
                SYS_SynchronizeNextFilterStepTime(minOfDayNow, "ExecutePreAnalyzationSteps mit Filter")   '2  V2.1.5            'bestimmt nächste Auswerteminute + Fwxlminute

                NB = "B_" & Format(Now, "dd") & Format(Now, "HH")
                'aufgenommenes Spektrum retten, vor dem Untergrundabzug V2.1.57
                System.IO.File.Copy(_stdS, _SpectraDirectory & "\" & NB & "_bak.cnf", True)
                analyzer.MCA_AnalyzeSpectrum(_stdS, 2)                  'abbgelaufene Filterstanddauer
                GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)

                System.IO.File.Copy(_stdS, _SpectraDirectory & "\" & NB & ".cnf", True)
                cnfNB.Open(_SpectraDirectory & "\" & NB & ".cnf", CanberraDataAccessLib.OpenMode.dReadWrite)
                cnfNB.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STITLE) = NB & ".cnf"
                cnfNB.Flush()
                cnfNB.Close()
                'Spektrum nur addiren, wenn Messwerte gültig
                'neu in V. 2.1.60: Ausnahme: Wenn Bandtransportsteuerung gerade erst aufgetreten ist, dann noch addieren

                AddSpectrumToDaySpectrum(stdCnf, tagCnf, NoFilterStep_Old)


                If Now >= _DayStartTimeDate Then                      'Tageswechsel?
                    SYS_SynchronizeNextDayStart()
                    If _HourSumSpectrumIsNotEmpty = True Then 'nur wenn das Tagessummensp. nicht leer ist
                        analyzer.MCA_AnalyzeSpectrum(_tagS, 3)                           'TAG.CNF auszuwerten
                        GUI_OpenSpectrum(_MyControlCenter.MCA, SpectraTypes.ONLINE, True, False, frmMeasScreen.SpectralDisplay)
                        NT = "T_" & Format(Now, "MM") & Format(Now, "dd") & Format(Now, "HH")
                        System.IO.File.Copy(_tagS, _SpectraDirectory & "\" & NT & ".cnf", True)
                        cnfNT.Open(_SpectraDirectory & "\" & NT & ".cnf", CanberraDataAccessLib.OpenMode.dReadWrite)
                        cnfNT.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STITLE) = NB & ".cnf"
                        cnfNT.Flush()
                        cnfNT.Close()
                    Else
                        analyzer.SYS_SaveAnalyzationResultsToFile(WRITE_HEADER, "", 3, "")
                        analyzer.SYS_SaveAnalyzationResultsToFile(APPEND_MESSAGE, "", 3, MSG_NoSpectrasAdded)
                        GUI_SetMessage(MSG_NoSpectrasAdded, MessageStates.RED)
                    End If
                    If System.IO.File.Exists(_AusgabeSammelinfo) Then
                        System.IO.File.Copy(_AusgabeSammelinfo, _AnalyzationFilesDirectory & "\Info_" & Format(Now, "dd") & Format(Now, "MM") & ".dat", True) 'neu in ver 2157 dwd
                        System.IO.File.Delete(_AusgabeSammelinfo)
                    End If
                    _HourSumSpectrumIsNotEmpty = False 'Tagesspektrum beginnen
                    _ASP3 = 0
                    tagCnf.Open(_tagS, CanberraDataAccessLib.OpenMode.dReadWrite)
                    tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3) = 0
                    tagCnf.Flush()
                    tagCnf.Close()
                End If      'if-end ntg%=noiertg% */
            End If        'if-end fwxlmin%\=auswmin% */

            Dim alarmModeValid As Boolean = ChangeFilterTimeInAlarmMode()
            If _MyControlCenter.SYS_States.IntensiveMode Or alarmModeValid Then

                _NextFilterStepMinuteDate = _AnalyzeMinuteDate
                GUI_ShowNextAnalyzationMinute()
                GUI_ShowNextFilterStepTime()
            End If

            cnfNB = Nothing
            cnfNT = Nothing

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Adds the spectrum to day spectrum.
    ''' </summary>
    ''' <param name="stdCnf">The standard CNF.</param>
    ''' <param name="tagCnf">The tag CNF.</param>
    ''' <param name="NoFilterStep_Old">if set to <c>true</c> [no filter step old].</param>
    Private Sub AddSpectrumToDaySpectrum(ByRef stdCnf As CanberraDataAccessLib.DataAccess,
                                         ByRef tagCnf As CanberraDataAccessLib.DataAccess,
                                         NoFilterStep_Old As Boolean)

        Dim u As Integer
        Dim rmd, eli As Object
        Dim spek As Object, spekc As Object

        'wegmaskieren aller Warnstatus! Nur wenn kein einziger fataler Fehler entstanden ist, das Spektrum dem Tagesspektrum zuordnen...
        Dim fatalErrorSumState As Integer = _MyControlCenter.SYS_States.SumState And 1023
        If (fatalErrorSumState = 0) Or (_MyControlCenter.SYS_States.SumState = 16 And NoFilterStep_Old = False) Then

            If _HourSumSpectrumIsNotEmpty = False Then
                System.IO.File.Copy(_stdS, _tagS, True)
                tagCnf.Open(_tagS, CanberraDataAccessLib.OpenMode.dReadWrite)
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR2) = _SSPRSTR3 'Bestaubungsstartzait für tagS
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP1) = _ASP1        'Bestaubungsstart (h) nach 1.1.00 00:00 für tagS
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP4) = 0           'TagessummenSpektrum
                _HourSumSpectrumIsNotEmpty = True                          'Merker:das nächste Mal Spektrenaddizion
            Else    'if tagMerker%<>0 dann STD.CNF zu TAG.CNF addiren
                tagCnf.Open(_tagS, CanberraDataAccessLib.OpenMode.dReadWrite)
                stdCnf.Open(_stdS, CanberraDataAccessLib.OpenMode.dReadWrite)
                eli = tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE)
                eli = eli + stdCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE)                 ' + stdS-Echtmessdauer
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE) = eli
                eli = tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL)
                eli = eli + stdCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL)                 ' + stdS-Realmessdauer=stdS-Bestaubungsdauer
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL) = eli
                rmd = tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP2)
                rmd = rmd + stdCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP2)               ' + Mittelwert des Luftdurchsazes inn stdS
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP2) = rmd
                _ASP3 = tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3)
                _ASP3 = _ASP3 + 1                'Anzal addirter Spektren
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3) = _ASP3
                _ASPSTR = tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_T_ASPSTR)
                _SSPRSTR2 = tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR2)
                tagCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR4) = _SSPRSTR4  'Messendezait
                spek = tagCnf.Spectrum(0&, 4095&)        'Spektrenaddizion tagS=tagS+stdS
                spekc = stdCnf.Spectrum(0&, 4095&)
                stdCnf.Close()
                For u = 0 To 4094
                    spek(u) = spek(u) + spekc(u)
                Next u
                tagCnf.Spectrum(0&, 4095&) = spek
            End If  'If-end tagMerker% = 0
            tagCnf.Flush()
            tagCnf.Close()
            _ASP1 = 0
        Else
            GUI_SetMessage(MSG_SpectrumNotAddedToDaySpectrum + fatalErrorSumState.ToString(),
                           MessageStates.YELLOW)
        End If    'If-end statfat = 0


    End Sub

    Public Sub MCA_SetPreset()
        Try
            'ehemals sezForwal

            Dim qellCnf As New CanberraDataAccessLib.DataAccess
            Dim ret As Integer
            Dim dinDatum As Double
            ret = qellCnf.FileExists(_tagS)
            If ret = 0 Then
                GUI_ShowMessageBox(_tagS & ml_string(215, " fehlt."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            Else
                qellCnf.Open(_tagS, CanberraDataAccessLib.OpenMode.dReadOnly)
                _ASP1 = qellCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP1)  'Anzal der Stunden sait 1.Jan.2000 biss Tagesnoibeginn(h)
                _ASP3 = qellCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_F_ASP3)
                _SSPRSTR2 = qellCnf.Param(CanberraDataAccessLib.ParamCodes.CAM_T_SSPRSTR2) 'Bestaubungsstartzait
                qellCnf.Close()
                If _ASP3 > 0 Then
                    _HourSumSpectrumIsNotEmpty = True
                Else
                    _HourSumSpectrumIsNotEmpty = False
                End If
                dinDatum = DateDiff("h", "01.01.2000 " & Format(_MyFHT59N3Par.DayStartTime, "00") & ":00:00", Now)
                'tagesspektrum beginnen oder fortsezen ?
                If (dinDatum - _ASP1) > (24 - _MyFHT59N3Par.FilterTimeh / 2) Or (dinDatum < _ASP1) Then 'Or (_LastCalibAnalazationType > CalibAnalyzationTypes.NoAnalyzation)
                    '_LastCalibAnalazationType = CalibAnalyzationTypes.NoAnalyzation     'Merker für ausgefürte Kalibrirrutine zurücksezen
                    _HourSumSpectrumIsNotEmpty = False 'Tagesspektrum beginnen
                    _ASP1 = 0        'Merker: Anzal der Stunden sait 1.Jan.2000 sezen
                    _SSPRSTR2 = 0
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_ManageCalibrationSteps(ByVal StartOfCalibration As Boolean)
        Try
            If StartOfCalibration Then
                frmCalibrationMeasControl.ShowDialog()
            Else
                _NeedToManageNextCalibrationStep = False
                _LastCalibrationType = _CalibrationType

                'freie messung oder hintergrundmessung gemacht
                If _LastCalibrationType = CalibTypes.CalibFree Then

                    MCA_StopCalibrationMeasurement()
                    If GUI_ShowMessageBox(MSG_WantToSave, ml_string(90, "Yes"), ml_string(91, "No"), "", MYCOL_THERMOGREEN, Color.White) = MsgBoxResult.Yes Then
                        Dim SaveDialog As New SaveFileDialog
                        SaveDialog.InitialDirectory = _SpectraDirectory
                        SaveDialog.Filter = "Spectras (*.cnf)|*.cnf"
                        SaveDialog.FilterIndex = 1
                        SaveDialog.ShowDialog()
                        If SaveDialog.FileName <> "" Then
                            _MyControlCenter.MCA_SaveSpectrum(SaveDialog.FileName, True)
                        End If
                    End If
                ElseIf _LastCalibrationType = CalibTypes.CalibBackground Then

                    MCA_StopCalibrationMeasurement()

                    Dim backCnf = _SpectraDirectory & "\BACKGND.CNF"
                    If File.Exists(backCnf) Then
                        File.Copy(backCnf, backCnf + ".bak", True)
                    End If

                    'backup existing background.cnf before saving...
                    _MyControlCenter.MCA_SaveSpectrum(backCnf, True)

                    GUI_ShowMessageBox(MSG_SavedBackgroundMeasurement, "OK", "", "", MYCOL_THERMOGREEN, Color.White)
                Else

                    MCA_ExecutePreAnalyzationStepsCalib()
                    If _LastCalibrationType = CalibTypes.CalibFar Then 'auch noch in Nah Geometrie messen
                        _CalibrationType = CalibTypes.CalibNear
                        frmCalibrationMeasControl.ShowDialog()
                    ElseIf _LastCalibrationType = CalibTypes.CalibNear Then 'fertig, Auswertung starten
                        MCA_StopCalibrationMeasurement()
                        _CalibrationType = CalibTypes.NoCalibration
                        _LastCalibrationType = CalibTypes.NoCalibration
                        MCA_StartCalibrationAnalyzation()
                    ElseIf _LastCalibrationType = CalibTypes.CalibNearMix Then
                        MCA_StopCalibrationMeasurement()
                        _CalibrationType = CalibTypes.NoCalibration
                        _LastCalibrationType = CalibTypes.NoCalibration
                        '                        frmCalibrationMeasControl.ShowDialog()
                        'da der frmCalibrationMeasControl nicht ein zweites Mal aufgerufen wird
                        'muss das Starten der Auswertung hier händisch gesetzt werden.
                        _StartCalibAnalyzation = True

                    ElseIf _LastCalibrationType = CalibTypes.CalibRccCs137 Then
                        MCA_StopCalibrationMeasurement()
                        _CalibrationType = CalibTypes.NoCalibration
                        _LastCalibrationType = CalibTypes.NoCalibration
                        _CalibAnalyzationType = CalibAnalyzationTypes.EfficiencyRccCs137
                        frmCalibrationAnalyzeControl.ShowDialog()
                        If frmCalibrationAnalyzeControl.ReturnValue = 1 Then
                            MCA_CalibrateEfficiencyRCC(CalibTypes.CalibRccCs137)
                        End If


                    ElseIf _LastCalibrationType = CalibTypes.CalibRccMix Then
                        MCA_StopCalibrationMeasurement()
                        _CalibrationType = CalibTypes.NoCalibration
                        _LastCalibrationType = CalibTypes.NoCalibration
                        _CalibAnalyzationType = CalibAnalyzationTypes.EfficiencyRccMix
                        frmCalibrationAnalyzeControl.ShowDialog()
                        If frmCalibrationAnalyzeControl.ReturnValue = 1 Then
                            MCA_CalibrateEfficiencyRCC(CalibTypes.CalibRccMix)
                        End If
                    End If



                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_ExecutePreAnalyzationStepsCalib()
        Try
            Dim asfWerter As New CanberraSequenceAnalyzerLib.SequenceAnalyzer
            'Dim GroesW(0 To 1) As CanberraDataAccessLib.ParamCodes  '0=Energi, 1=Kanallage
            Dim Anz As Integer
            Dim FileName As String = ""

            _MyControlCenter.MCA_SetDetectorParams(_MyFHT59N3Par.Customer, _MyFHT59N3Par.StationName, _MyFHT59N3Par.StationID)
            _MyControlCenter.MCA_SetParamsForActualSpectrum("m³/h", 0, 0, 0, Format(Now, "dd.MM.yy") & " " & Format(Now, "HH:mm"))

            If System.IO.File.Exists(_TempS) Then
                System.IO.File.Delete(_TempS) 'alte Datai spektrum.cnf gelöscht
            End If
            _MyControlCenter.MCA_SaveSpectrum(_TempS, True)

            GUI_OpenSpectrum(_TempS, SpectraTypes.OFFLINE, False, False, frmMeasScreen.SpectralDisplay)

            Select Case _CalibrationType
                Case CalibTypes.CalibFar
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STYPE) = ml_string(216, "Calibrationspectrum")
                    FileName = _fernS

                Case CalibTypes.CalibNear, CalibTypes.CalibNearMix
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STYPE) = ml_string(216, "Calibrationspectrum")

                    FileName = _na_mischS
                    If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                        FileName = _naS
                    End If

                Case CalibTypes.CalibRccCs137, CalibTypes.CalibRccMix
                    _SpectraFile.Param(CanberraDataAccessLib.ParamCodes.CAM_T_STYPE) = ml_string(217, "RCC-Spectrum")
            End Select
            _SpectraFile.Flush()

            'Auswertung
            asfWerter.Analyze(_SpectraFile, , _MyControlCenter.MCA_CtlFiles & "\EBINKAL.ASF", , , , , _ReportFilesDirectory & "\Ebin.rpt", ) ' FFkaAnzaige.RepFenst)
            'inn EBINKAL.ASF nur PEAKANALYSIS
            SYS_SaveReportsToCollectionFile(_ReportFilesDirectory & "\Ebin.rpt")
            Anz = _SpectraFile.NumberOfRecords(CanberraDataAccessLib.ClassCodes.CAM_CLS_PEAK)
            If Anz = 0 Then
                GUI_ShowMessageBox(ml_string(218, "No Lines found in spectrum."), "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                GUI_CloseSpectrum(False, True)
                Exit Sub
            End If

            GUI_CloseSpectrum(True, True)

            If _CalibrationType <> CalibTypes.CalibRccCs137 And _CalibrationType <> CalibTypes.CalibRccMix Then
                System.IO.File.Copy(_TempS, FileName, True)                 'kopirt spektrum.cnf nach x.cnf
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_StartCalibrationAnalyzation()
        Try
            'Kalibrierauswertung starten
            If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                _CalibAnalyzationType = CalibAnalyzationTypes.EnergyFar
                frmCalibrationAnalyzeControl.ShowDialog()
                If frmCalibrationAnalyzeControl.ReturnValue = 1 Then

                    If MCA_CalibrateEnergyGeometryFar() Then
                        _CalibAnalyzationType = CalibAnalyzationTypes.EfficiencyFar
                        If MCA_CalibrateEfficiencyGeometryFar() Then
                            _CalibAnalyzationType = CalibAnalyzationTypes.EfficiencyNear
                            frmCalibrationAnalyzeControl.ShowDialog()
                            If frmCalibrationAnalyzeControl.ReturnValue = 1 Then
                                If MCA_CalibrateEfficiencyGeometryNear() Then
                                    _CalibAnalyzationType = CalibAnalyzationTypes.NoAnalyzation
                                End If
                            End If
                        End If
                    End If

                End If
            Else
                'der fall wenn keine NAH&FERN-Kalibrierung eingestellt ist (also Einzelstrahler-Misch-Kalibrierung wie für BAG)...
                _CalibAnalyzationType = CalibAnalyzationTypes.EnergyNearMix
                frmCalibrationAnalyzeControl.ShowDialog()
                If frmCalibrationAnalyzeControl.ReturnValue = 1 Then
                    If MCA_CalibrateEnergyGeometryNearMix() Then
                        If MCA_CalibrateEfficiencyGeometryNearMix() Then
                            _CalibAnalyzationType = CalibAnalyzationTypes.NoAnalyzation
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

#End Region

#Region "System"

    ''' <summary>
    ''' Initialize all program functions
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SYS_InitializeProgramFunctions()
        Try
            Dim resultString As String
            Dim ret As Integer

            _MonitorConfigDirectory = "C:\FHT59N3\Configuration\MonitorConfiguration"
            _MyConfig = New ThermoUtilities.ThermoConfigFile_XML(_MonitorConfigDirectory & "\FHT59N3_Config.xml")

            'Sprache auslesen und setzen
            SYS_ReadSettings(True)
            SYS_SetLanguage()

            _MyFHT59N3Par = New FHT59N3_SystemParams

            'alle anderen Konfigurationswerte einlesen
            SYS_ReadSettings(False)

            _MyFHT59N3Par.WasInitialized = True

            SYS_CreateWorkingDirectoryStructure()

            Dim Listeners As System.Diagnostics.TraceListenerCollection = Trace.Listeners
            For i As Integer = 0 To Listeners.Count - 1
                If Listeners(i).ToString = "ThermoLogging.ThermoLogging_TextWriterTraceListener" Then
                    Dim L As ThermoLogging_TextWriterTraceListener
                    Dim FileNameParts() As String
                    Dim FileName As String
                    L = CType(Listeners(i), ThermoLogging_TextWriterTraceListener)
                    FileNameParts = L.FileName.Split("\".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                    If FileNameParts.Length > 0 Then
                        FileName = FileNameParts(FileNameParts.Length - 1)
                        L.FileName = _LogfilesDirectory & "\" & FileName
                    End If
                End If
            Next

            frmMain.ImageList1.Images.Add("GreenDot", CType(_MyResouceManager.GetObject("GreenDot"), System.Drawing.Image))
            frmMain.ImageList1.Images.Add("YellowDot", CType(_MyResouceManager.GetObject("YellowDot"), System.Drawing.Image))
            frmMain.ImageList1.Images.Add("RedDot", CType(_MyResouceManager.GetObject("RedDot"), System.Drawing.Image))

            'niemand will die Shortcuts, aber aus Kompatibilitätsgründen wird bei DWD nichts geändert...
            If _MyFHT59N3Par.Customer = "DWD" Then
                SYS_AddHotkeys()
            End If


            _ApplPath = Application.StartupPath & "\"

            _TimeZone = _MyTimeZoneCodes.GetTimeZoneCode(System.TimeZone.CurrentTimeZone.GetUtcOffset(Now).Hours)

            GUI_FillBackColors()
            GUI_ShowForm(frmMeasScreen.PanelfrmMeasScreen)


            _MyControlCenter = New FHT59N3_ControlCenter(_MyFHT59N3Par.SPSCom,
                                                         _MyFHT59N3Par.SPSIpNetworkAddress,
                                                         _MyFHT59N3Par.SPSIpNetworkPort,
                                                         _MyFHT59N3Par.SPSConnectionType,
                                                         _MyFHT59N3Par.SPSUseSTXEXTProtocol,
                                                         FHT59N3_ControlCenter.MCATypes.Canberra_Lynx,
                                                         _IPLynx,
                                                         _LogfilesDirectory,
                                                         _SimulateLynxSystem,
                                                         Sub(msg As String)
                                                             InterruptedTracing(msg)
                                                         End Sub)

            AddHandler _MyControlCenter.CommandReceived, AddressOf SYS_RemoteCommandReceivedHandler
            AddHandler _MyControlCenter.SystemStateChanged, AddressOf SYS_SystemStateChangedHandler

            SYS_ReadAlarmSettings()
            SYS_ReadECoolerMemorized()

            'Implizit ergänzte Konfigurationswerte wieder rausschreiben
            SYS_WriteSettings()

            _N2HeatingTimePoint = Now.AddMinutes(-(2 * _N2HeatingSteps))
            '_LastCalibAnalazationType = CalibAnalyzationTypes.NoAnalyzation
            _CalibrationType = CalibTypes.NoCalibration
            _LastCalibrationType = CalibTypes.NoCalibration
            _CalibAnalyzationType = CalibAnalyzationTypes.NoAnalyzation

            _MyEcoolerController = New FHT59N3_EcoolerController(_MyFHT59N3Par.EnableEmergencyStopDetect,
                                                                 _MyFHT59N3Par.EnableCapturingDetectorTemperature,
                                                                 _MyFHT59N3Par.CrystalTooWarmTempThreshold,
                                                                 _MyFHT59N3Par.CrystalWarmedUpTempThreshold)

            _MyTemperatureRecorder = New FHT59N3_DetectorTemperatureRecorder(_MyFHT59N3Par.Customer, _MyFHT59N3Par.StationName)

            'also calls SYS_ReadAlarmSettings
            SYS_SetDerivedWorkParamsFromConfig()

            Dim minOfDaynow As Integer = MinutesOfDayNow()
            SYS_SynchronizeNextAnalyzationTime(minOfDaynow, "init program functions")   'bestimmt nächste Auswerteminute
            SYS_SynchronizeNextAlarmCheckTime(minOfDaynow, "init program functions")
            SYS_SynchronizeNextFilterStepTime(minOfDaynow, "init program functions")

            SYS_SynchronizeNextDayStart()

            SYS_StartSnmpCommunication()

            SYS_StartN4242Cleanup()

            Dim Par As FHT59N3Core.FHT59N3_LynxParams = CType(_MyControlCenter.MCA_Params, FHT59N3Core.FHT59N3_LynxParams).RestoreMeFromFileBinary(_MonitorConfigDirectory & "\")
            If Not Par Is Nothing Then
                _MyControlCenter.MCA_Params = Par
                _MyControlCenter.MCA_SetAllMeasParams()
            Else
                'TODO MELDUNG MACHEN
            End If

            _MyControlCenter.SYS_RestoreStatesFromFile(_MonitorConfigDirectory & "\")

            _MyControlCenter.SYS_States.K40ToLow_NotFound = False
            _MyControlCenter.SYS_States.AirFlowLessThen1Cubic = False
            _MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic = False
            _MyControlCenter.SYS_States.NoFilterstep = False
            _MyControlCenter.SYS_States.AnalyzationCancelled = False
            _MyControlCenter.SYS_States.CheckTempPressure = False
            _MyControlCenter.SYS_States.K40ToBig = False
            _MyControlCenter.SYS_States.DataTransferError = False
            _MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent = False
            _MyControlCenter.SYS_States.EcoolerOff = False
            _MyControlCenter.SYS_States.UpsOnBattery = False

            GUI_ReadMessages()
            GUI_SetMessage(MSG_MEASPROGSTARTED, MessageStates.GREEN)

            GUI_SetMenus()

            SYS_InitFilterDrucker()

            If Not MCA_GetNuclidsFromBib() Then
                frmMain.Close()
            End If

            _MyControlCenter.MCA_SetHVOn()

            If (_NetLogActive) Then
                resultString = _MyControlCenter.MDS_StartOrRestartNetlog(_NetViewPath, _NetViewActive)
                If resultString <> "" Then
                    ret = GUI_ShowMessageBox(resultString, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                End If
            End If


            _MyControlCenter.MCA_DisconnectFromDetector()


            Dim spectrumFile As String = _na_mischS
            If _MyFHT59N3Par.CalibrationType = FHT59N3_SystemParams.CalibrationTypeEnum.NearAndFarCalibration Then
                spectrumFile = _naS
            End If

            REM BDR: takes a few seconds...
            ret = SYS_MyShell(_MyControlCenter.MCA_ExeFiles & "\MOVEDATA.exe", spectrumFile & " DET:EBIN /ECAL /EFFCAL /OVERWRITE", ProcessWindowStyle.Hidden, True)
            If ret Then
                Dim ml As String = ml_string(219, "Transfer calibration ({0}) via MOVEDATA to detector (DET:EBIN) failed. Error code: {1}")
                Dim msg As String = String.Format(ml, spectrumFile, Format(ret, "0"))
                GUI_ShowMessageBox(msg, "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
            End If
            REM BDR: takes a few seconds
            _MyControlCenter.MCA_ConnectToDetector()
            _MyControlCenter.MCA_SetDetectorParams(_MyFHT59N3Par.Customer, _MyFHT59N3Par.StationName, _MyFHT59N3Par.StationID)

            If _MyControlCenter.SYS_States.Maintenance Then
                SPS_SetMaintenanceOn()
            Else
                SPS_SetMaintenanceOff()
            End If


            SYS_SetDerivedSPSParamsFromConfig()

            frmMain.FormBorderStyle = FormBorderStyle.Sizable
            frmMain.PanelTop.Visible = False
            frmMain.MaximizeBox = True
            frmMain.MinimizeBox = True
            frmMain.ControlBox = True

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub



    ''' <summary>
    ''' This is the most significant activity which is called cyclic (currently every second)
    ''' </summary>
    ''' <remarks>Don't call any functions which might block!</remarks>
    Public Sub SYS_MonitorControl()
        Try

            Dim PerformCycleCheck As Boolean = (DateTime.Now - MediumCycle_LastCheck).TotalSeconds > 1
            If PerformCycleCheck Then
                MediumCycle_LastCheck = DateTime.Now
            End If

            SYS_EvaluateSnmpStatus()

            SPS_BuildDigInOutState()

            SPS_GetDigStates()
            SPS_GetAnaStates()
            SPS_GetCalculations()

            SPS_AnalyzeDigitalStates()
            GUI_SetLabelsBecauseOfDigStates()

            SPS_AnalyzeAnalogStates()
            SPS_CheckTransferState()
            SPS_CheckAndControlEcoolerEmergency()

            If _CalibAirFlow Then
                SYS_ManageAirFlowCalibration()
            End If



            'Wenn Lynx nicht dran hängt (Entwicklung) gibts jede Sekunde eine Exception, daher konditional ausführen... 
            If Not _SimulateLynxSystem Then

                If PerformCycleCheck Then
                    MCA_CheckIfMeasurementDone()
                End If

            End If


            If _StartCalibration Then
                _StartCalibration = False
                MCA_ManageCalibrationSteps(True)
            End If
            If _StartCalibAnalyzation Then
                _StartCalibAnalyzation = False
                MCA_StartCalibrationAnalyzation()
            End If

            If PerformCycleCheck Then
                GUI_SetMenus()
            End If

            If _EndProgram Then
                ' almost all services were stopped in frmMain_FormClosing
                _MyControlCenter.Dispose()
                frmMain.Close()
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    '----------------------------------------------------------------------
    'Dwd,Jord: Fwechsel imm Normalbetrib: Bandtransportforderung an SPS, anschlisend Messstart
    'Nloe:     Fwechsel: Bandtransport-Forderung an SPS, anschlisend Messstart
    '----------------------------------------------------------------------
    Private Sub SYS_SetNextFilterStepAndStartMeasurement()
        Dim resultStr As String
        Try
            'ehemals fwechsel!
            MCA_StopMeasurement(False)

            GUI_SetMessage(MSG_TapeTransportDemanded, MessageStates.GREEN)
            _DoIhaveToExecuteStepsAfterFilterstep = True
            'Trace.TraceInformation(": SYS_SetNextFilterStepAndStartMeasurement: _DoIhaveToExecuteStepsAfterFilterstep=" & _DoIhaveToExecuteStepsAfterFilterstep.ToString)

            _AirFlowSumCounter = 0
            _AirFlowSum = 0
            _EREALOld = 0

            _FiltertstepTries = 0

            If (Not _MyFHT59N3Par.EnablePaperrollPrinter) Then
                'regulärer Schritt mit konfigurierte Länge
                resultStr = SPS_SetFilterstep()
            Else
                'kleiner Vorschub, um Datum auf Filterband zu drucken, intern wird dann der restliche Vorschub veranlasst
                _MyControlCenter.SPS_FilterstepStarted1stFlag = False
                _MyControlCenter.SPS_FilterstepStarted2ndFlag = False
                resultStr = SPS_SetFilterstepWithPrinter1()
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_ExecuteStepsAfterFilterstep()
        Try
            'ehemals Teil von fwechsel!
            Dim s$, st%, mi%, se%

            'Flag zum Neustart (Prozeduren nach Filterbandwechsel) zurücksetzen
            _DoIhaveToExecuteStepsAfterFilterstep = False

            'Trace.TraceInformation(": SYS_ExecuteStepsAfterFilterstep:  _DoIhaveToExecuteStepsAfterFilterstep=" & _DoIhaveToExecuteStepsAfterFilterstep.ToString)

            GUI_SetMessage(MSG_TapeTransport, MessageStates.GREEN)

            If _MyFHT59N3Par.FilterSteps < _AlarmThresholdFilterSteps Then
                If Not _MyControlCenter.SYS_States.FilterHasToBeChanged Then
                    GUI_SetMessage(MSG_TapeNearEnd, MessageStates.YELLOW)
                    _MyControlCenter.SYS_States.FilterHasToBeChanged = True
                End If
            Else
                If _MyControlCenter.SYS_States.FilterHasToBeChanged Then
                    GUI_SetMessage(MSG_TapeChanged, MessageStates.GREEN)
                    _MyControlCenter.SYS_States.FilterHasToBeChanged = False
                End If
            End If

            _ActualMeasurementTime = (1.5 * _MyFHT59N3Par.FilterTimeh * 60 + 60) \ 1

            'Trace.TraceInformation(": SYS_ExecuteStepsAfterFilterstep:  Try to set the pump on!")

            GUI_SetMessage(ml_string(220, "New start at ") & Format(Now, "dd.MM.yy HH:mm") & " h", MessageStates.GREEN)

            _MyControlCenter.SPS_PumpOn()

            'If (_CalibrationType = CalibTypes.CalibBackground And Not _Measurement) Then
            'MCA_StartCalibrationMeasurement()
            'Return
            'End If

            MCA_StartNormalMeasurement(_ActualMeasurementTime)

            If _ASP1 = 0 Then               'Tagesspektrum beginnen ?
                s$ = Format(Now, "dd.MM.yyyy HH:mm:ss")
                st% = Val(Format(Now, "HH"))
                mi% = Val(Format(Now, "mm"))
                se% = Val(Format(Now, "ss"))
                st% = st% + ((mi% * 60 + se%) + 1800) \ 3600
                If st% < (_MyFHT59N3Par.DayStartTime - _MyFHT59N3Par.FilterTimeh \ 2) Then
                    If Right(s$, 8) = "00:00:00" Then Mid$(s$, 17&, 1) = "1"
                    s$ = DateAdd("d", -1, s$)
                End If
                s$ = Left$(s$, Len(s$) - 8) & Format(_MyFHT59N3Par.DayStartTime, "00") & ":00:00"
                _ASP1 = DateDiff("h", "01.01.2000 " & Format(_MyFHT59N3Par.DayStartTime, "00") & ":00:00", s$)
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    ''' <summary>
    ''' Minutes of day of current time.
    ''' </summary>
    ''' <returns></returns>
    Public Function MinutesOfDayNow() As Integer
        Dim actualMinuteOfDay As Integer = 60 * Now.Hour + Now.Minute
        Return actualMinuteOfDay
    End Function


    Public Function DateFromDayMinutes(minutesOfDay As Integer) As DateTime
        Dim currDate = New System.DateTime(Now.Year, Now.Month, Now.Day, minutesOfDay \ 60, minutesOfDay Mod 60, 0)
        Return currDate
    End Function

    ''' <summary>
    ''' Bestimmung des Zaitpunkts für nächsten Filterwechsel aus den Zaitpunkten
    ''' möglicher Filterwechsel: fwxl.i (i=1 bis fwxlanz)
    ''' Bai Noistart Sünkronisirung: fistand/2 ... fistand ... 1.5 fistand
    ''' </summary>
    ''' <param name="actualMinuteOfDay">The actual minute of day.</param>
    ''' <param name="skipToOverNextIfLargerHalfFilterTime">if set to <c>true</c> [skip to over next if larger half filter time].</param><remarks>
    ''' _NextFilterStepMinute und _NextFilterStepMinuteDate werden gesetzt
    ''' </remarks>
    Public Sub SYS_SynchronizeNextFilterStepTime(actualMinuteOfDay As Integer, callInfo As String, Optional skipToOverNextIfLargerHalfFilterTime As Boolean = True)
        Try
            'ehemals suenkron1!

            Dim actualMinuteOfDayOrig = actualMinuteOfDay

            Dim alarmModeValid As Boolean = ChangeFilterTimeInAlarmMode()

            Dim nextFilterStepMinute As Integer
            If _MyControlCenter.SYS_States.IntensiveMode Or alarmModeValid Then

                _NextFilterStepMinuteDate = _AnalyzeMinuteDate

            Else


                nextFilterStepMinute = 1440

                Dim j As Integer
                For i As Integer = 1 To _NumberOfFilterStepsPerDay         'sucht die der Tagesminute nächste Filterwechselzeit
                    If Math.Abs(_FilterStepMinutes(i) - actualMinuteOfDay) < nextFilterStepMinute Then
                        nextFilterStepMinute = Math.Abs(_FilterStepMinutes(i) - actualMinuteOfDay)
                        j = i
                    End If
                Next i

                If actualMinuteOfDay >= _FilterStepMinutes(j) Then
                    j = j + 1
                    If j > _NumberOfFilterStepsPerDay Then j = 1
                    nextFilterStepMinute = _FilterStepMinutes(j)
                Else
                    nextFilterStepMinute = _FilterStepMinutes(j)
                End If

                If actualMinuteOfDay > nextFilterStepMinute Then
                    actualMinuteOfDay = actualMinuteOfDay - 1440
                End If

                If skipToOverNextIfLargerHalfFilterTime AndAlso ((nextFilterStepMinute - actualMinuteOfDay) < (_MyFHT59N3Par.FilterTimeh * 60) / 2) Then
                    j = j + 1                  'wenn Abstand Filterwechselzait-jezt < Filterstanddauer/2
                    If j > _NumberOfFilterStepsPerDay Then j = 1
                    nextFilterStepMinute = _FilterStepMinutes(j)         'nächste Filterwechselzait (Tagesminute)
                End If

                If nextFilterStepMinute > actualMinuteOfDayOrig Then
                    _NextFilterStepMinuteDate = DateFromDayMinutes(nextFilterStepMinute)
                Else 'bin über die Tagesgrenze hinaus, somit wäre _AnalyzeMinute < als _ActualMinuteOfDay und ich würde sofort auswerten
                    _NextFilterStepMinuteDate = DateFromDayMinutes(nextFilterStepMinute)
                    _NextFilterStepMinuteDate = _NextFilterStepMinuteDate.AddDays(1)
                End If

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' Bestimmung des Zaitpunkts der nächsten Auswertung
    ''' </summary>
    ''' <param name="actualMinuteOfDay">The actual minute of day.</param>
    ''' <param name="callInfo">The call information.</param><remarks>
    ''' setzt _AnalyzeMinute und _AnalyzeMinuteDate
    ''' </remarks>
    Public Sub SYS_SynchronizeNextAnalyzationTime(actualMinuteOfDay As Integer, callInfo As String)
        Try


            Dim analyzePeriod = _MyFHT59N3Par.MeasurementTimemin
            'ehemals suenkron2!

            Dim analyzeMinute As Integer = ((actualMinuteOfDay + analyzePeriod / 2) \ analyzePeriod) * analyzePeriod  'nächster Messstopp, Auswertezait
            If analyzeMinute < actualMinuteOfDay Then
                analyzeMinute = analyzeMinute + analyzePeriod
            End If
            If analyzeMinute - actualMinuteOfDay < analyzePeriod / 2 Then
                analyzeMinute = analyzeMinute + analyzePeriod
            End If

            If analyzeMinute > 1440 - 1 Then
                analyzeMinute = analyzeMinute - 1440
            End If

            If analyzeMinute > actualMinuteOfDay Then
                _AnalyzeMinuteDate = DateFromDayMinutes(analyzeMinute)
            Else 'bin über die Tagesgrenze hinaus, somit wäre _AnalyzeMinute < als _ActualMinuteOfDay und ich würde sofort auswerten
                _AnalyzeMinuteDate = DateFromDayMinutes(analyzeMinute)
                _AnalyzeMinuteDate = _AnalyzeMinuteDate.AddDays(1)
            End If
            Trace.TraceWarning("SYS_SynchronizeNextAnalyzationTime(" + callInfo + "): " + _AnalyzeMinuteDate.ToString("dd.MM.yy HH:mm"))
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' Berechne die naechste Zeit, wann man in der Alarmpruefung ist
    ''' </summary>
    ''' <param name="actualMinuteOfDay">The actual minute of day.</param>
    ''' <param name="callInfo">The call information.</param><remarks>
    ''' setzt _AlarmCheckTimeDate
    ''' </remarks>
    Public Sub SYS_SynchronizeNextAlarmCheckTime(actualMinuteOfDay As Integer, callInfo As String)
        Try
            Dim AlarmCheckTimeMinute As Integer

            'Dim actualMinuteOfDay As Integer = 60 * Now.Hour + Now.Minute
            If actualMinuteOfDay Mod _AlarmCheckPoints > 0 Then
                AlarmCheckTimeMinute = (actualMinuteOfDay + 2 * _AlarmCheckPoints) - (actualMinuteOfDay Mod _AlarmCheckPoints)
            Else
                AlarmCheckTimeMinute = actualMinuteOfDay + _AlarmCheckPoints
            End If

            AlarmCheckTimeMinute = (1440 + AlarmCheckTimeMinute) Mod 1440
            If AlarmCheckTimeMinute = (_AnalyzeMinuteDate.Hour * 60 + _AnalyzeMinuteDate.Minute) Then
                AlarmCheckTimeMinute = (1440 + AlarmCheckTimeMinute + _AlarmCheckPoints) Mod 1440
            End If
            If AlarmCheckTimeMinute > actualMinuteOfDay Then
                _AlarmCheckTimeDate = DateFromDayMinutes(AlarmCheckTimeMinute)
            Else 'bin über die Tagesgrenze hinaus, somit wäre _AnalyzeMinute < als _ActualMinuteOfDay und ich würde sofort auswerten
                _AlarmCheckTimeDate = DateFromDayMinutes(AlarmCheckTimeMinute)
                _AlarmCheckTimeDate = _AlarmCheckTimeDate.AddDays(1)
            End If

            If Not IsNothing(callInfo) Then
                Trace.TraceWarning("SYS_SynchronizeNextAlarmCheckTime(" + callInfo + "): " + _AlarmCheckTimeDate.ToString("dd.MM.yy HH:mm"))
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>setzt _DayStartTimeDate</remarks>
    Public Sub SYS_SynchronizeNextDayStart()
        Try
            If Now.Hour < _MyFHT59N3Par.DayStartTime Then
                _DayStartTimeDate = New System.DateTime(Now.Year, Now.Month, Now.Day, _MyFHT59N3Par.DayStartTime, 0, 0)
            Else 'bin über die Tagesgrenze hinaus, somit wäre _AnalyzeMinute < als _ActualMinuteOfDay und ich würde sofort auswerten
                Dim Tomorrow As Date = Now.AddDays(1)
                _DayStartTimeDate = New System.DateTime(Tomorrow.Year, Tomorrow.Month, Tomorrow.Day, _MyFHT59N3Par.DayStartTime, 0, 0)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_ManageAirFlowCalibration()
        Try
            Dim TS As New TimeSpan
            If _NumberOfAirFlowMeasurementsCalib > 0 Then                     '>0 ist auch Merker für Kalibrirung
                TS = Now.Subtract(_StartOfAirFlowMeasurementCalib)
                If TS.TotalSeconds >= _TimeBetweenAirFlowMeas Then
                    If Not _MyControlCenter.SPS_BypassOnOff Then 'wenn bypass geschlossen, dann wert zum Luftdurchsatz dazu addieren
                        _AirFlowSumCounterCalib = _AirFlowSumCounterCalib - 1
                        If _AirFlowSumCounterCalib = 0 Then
                            _FactorBezelCalib = (_FactorBezelCalib + _AirFlowTrue / _AirFlowMeasured) / _NumberOfAirFlowMeasurementsCalib
                            frmAirFlow.TBox_Result.Text = Format(_FactorBezelCalib, "0.000")
                            frmAirFlow.TBox_NumberValues.Text = Format(_NumberOfAirFlowMeasurementsCalib, "0")
                            _NumberOfAirFlowMeasurementsCalib = 0
                            _CalibAirFlow = False
                            frmAirFlow.TBox_NumberValues.Enabled = True
                            frmAirFlow.BtnStartMeas.Enabled = True
                            frmAirFlow.BtnApply.Enabled = True
                            frmAirFlow.BtnClose.Enabled = True
                            frmAirFlow.Cursor = Cursors.Default
                        Else
                            _FactorBezelCalib = _FactorBezelCalib + _AirFlowTrue / _AirFlowMeasured
                            frmAirFlow.TBox_NumberValues.Text = Format(_AirFlowSumCounterCalib, "0")
                        End If
                        _StartOfAirFlowMeasurementCalib = Now
                    Else
                        _MyControlCenter.SPS_BypassOff()
                    End If
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' Set the internal state machines to "filter changed", restart filter steps counter
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SYS_SetFilterChanged()
        Try
            _MyFHT59N3Par.FilterSteps = _NumberOfFilterStepsPerFilter
            GUI_SetMessage(MSG_TapeChanged, MessageStates.GREEN)

            _MyControlCenter.SYS_States.FilterHasToBeChanged = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_CheckIfLanguageChanged()
        Try
            If Not _Start Then
                Dim Changed As Boolean = False
                If _MyFHT59N3Par.Language.Contains("de") And System.Threading.Thread.CurrentThread.CurrentUICulture.ToString.Contains("en") Then
                    Changed = True
                ElseIf _MyFHT59N3Par.Language.Contains("en") And System.Threading.Thread.CurrentThread.CurrentUICulture.ToString.Contains("de") Then
                    Changed = True
                End If
                If Changed Then
                    GUI_ShowMessageBox(MSG_ProgRestart, "Ok", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                    GUI_SetMessage(MSG_MEASPROGCLOSED, MessageStates.GREEN)
                    _MyControlCenter.SYS_States.Maintenance = True
                    SYS_WriteSettings() 'Einstellungen sichern

                    _MyControlCenter.MCA_StopMeasurement(False)
                    _MyControlCenter.MCA_SetHVOff()
                    _MyControlCenter.SPS_AlarmOff()
                    _MyControlCenter.SPS_ErrorOn()

                    If _NetLogActive Then
                        '_MyControlCenter.MDS_StopNetlog()
                    End If

                    _Start = True
                    _EndProgram = True
                    _MenuEntryExitClicked = True

                    frmMain.Close()
                End If
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_SetLanguage()
        Try
            If _MyFHT59N3Par.Language.Contains("de") Then
                ml_UseCulture(New System.Globalization.CultureInfo("de"))
            ElseIf _MyFHT59N3Par.Language.Contains("en") Then
                ml_UseCulture(New System.Globalization.CultureInfo("en"))
            Else
                ml_UseCulture(New System.Globalization.CultureInfo("en"))
            End If
            MlRuntime.MlRuntime.BroadcastLanguageChanged()

            MSG_MEASPROGSTARTED = ml_string(340, "Measurement program started.")
            MSG_MEASPROGCLOSED = ml_string(341, "Measurement program closed.")
            MSG_SPSTRANSFERERROR = ml_string(342, "Communication problems with SPS!")
            MSG_SPSONLINE = ml_string(601, "SPS is online again.")
            MSG_SPSHVON = ml_string(343, "High voltage switched on")
            MSG_SPSHVOFF = ml_string(344, "High voltage switched off")
            MSG_DeadTime = ml_string(345, "Deadtime > 20%")
            MSG_DeadTimeOK = ml_string(346, "Deadtime normal.")
            MSG_TapeTransportDemanded = ml_string(347, "Tape transport demanded")
            MSG_NoTapeTransport = ml_string(348, "Tape not transported, torn?")
            MSG_TapeTransport = ml_string(349, "Tape transport ended")
            MSG_TapeNearEnd = ml_string(350, "Tape near its end")
            MSG_TapeChanged = ml_string(351, "Tape changed.")
            MSG_AlarmModeON = ml_string(352, "Alarm modus switched on")
            MSG_AlarmModeOFF = ml_string(426, "Alarm mode switched off")

            MSG_K40NotFound = ml_string(353, "K40-1460.8-keV-line not found.")
            MSG_K40DevToBig = ml_string(354, "Deviation of K40-1461-keV-line too large.")
            MSG_K40NotFoundOrTooSmall = ml_string(355, "K40-1460.8-keV-line too small (or not found).")
            MSG_K40ReCalibrated = ml_string(502, "K40-1460.8-keV-line recalibrated.")
            MSG_IdentifyingWithoutLines = ml_string(356, "Identifying: Analysis interrupted. Without peaks!!")
            MSG_BI214Recalib = ml_string(357, "BI-214 found, PB-214 is missing! Energy recalibration?")
            MSG_StartOfMaintenance = ml_string(358, "Start of maintenance")
            MSG_EndOfMaintenance = ml_string(359, "End of maintenance")
            MSG_RemoteMaintenance = ml_string(360, "Remote maintenance requested")
            MSG_TapeTorn = ml_string(450, "Tape torn, Bypass open")
            MSG_BypassOpen = ml_string(328, "Bypass open")
            MSG_BypassClosed = ml_string(362, "Bypass closed")
            MSG_CheckTempPressure = ml_string(363, "Check Temp./Pressure!")
            MSG_TempPressureOK = ml_string(364, "Temp./Pressure Ok")

            MSG_AirFlowLessThan1Cubic = ml_string(365, "Air flow < {0} m³/h!")
            MSG_AirFlowGreaterThan12Cubic = ml_string(366, "Air flow > {0} m³/h!")
            MSG_AirFlowOK = ml_string(367, "Air flow Ok!")
            MSG_MinAirFlowAlertOfRange = ml_string(505, "The allowed range for the minimal air flow must be between 1 and 10 (m³/h)")
            MSG_MaxAirFlowAlertOfRange = ml_string(506, "The allowed range for the maximal air flow must be between 10 and 20 (m³/h)")

            MSG_N2ValueUnpl = ml_string(368, "Value of N2-filling-level unplausible (")
            MSG_N2NearEnd = ml_string(369, "Nitrogen near its end!")
            MSG_N2Refilled = ml_string(370, "Nitrogen refilled.")
            MSG_StepWidthToSmall = ml_string(371, "Step width too small. Set to 60 mm!!")
            MSG_WantToExit = ml_string(372, "Do you really want to exit program?")
            MSG_EfficiencyCalibNearOK = ml_string(373, "Energy and efficiency calibration was successfull!")
            MSG_NoSpectrasAdded = ml_string(374, "Identifying: Analysis interrupted. No spectrum were added!")
            MSG_WantToSave = ml_string(375, "Do you want to save the measured spectrum?")
            MSG_DayStartTimeOutOfRange = ml_string(376, "Day start time out of range. Set to 0!!")
            MSG_FilterstepsOutOfRange = ml_string(377, "Number of Filtersteps out of range. Set to 1!!")
            MSG_N2ThresholdOutOfRange = ml_string(378, "N2 Threshold out of range. Set to 0!!")
            MSG_FilterTimeOutOfRange = ml_string(379, "Filter time out of range. Set to ")
            MSG_MeasurementTimeOutOfRange = ml_string(380, "Measurement time out of range. Set to ")
            MSG_ProgRestart = ml_string(386, "Program has to be restarted!")
            MSG_NoAccessToDirectories = ml_string(451, "You are not allowed to access the following directories:")
            MSG_EnergyReCalibrated = ml_string(503, "Energy was recalibrated")

            MSG_CaptureCycleDetectorTemperatureOfRange = ml_string(516, "The allowed range for the capture cycle of detector temperature must be between 1 and 60 (minutes)")
            MSG_EmergencyStopDetectOnlyWithTemperaruteDetection = ml_string(519, "The e-cooler emergency stop detection can only be activated if detector temperature is captured.")

            MSG_RecordingDetectorTemperaturIsRunning = ml_string(522, "Recording detector temperature is running")
            MSG_RecordingDetectorTemperaturIsDefect = ml_string(523, "Recording detector temperature is defect")
            MSG_WrongCrystalTooWarmTempThreshold = ml_string(526, "Wrong value for temperature threshold 'Crystal too warm'")
            MSG_WrongCrystalWarmedUpTempThreshold = ml_string(529, "Wrong value for temperature threshold 'Crystal warmed up'")

            MSG_WantToSwitchOffEcooler = ml_string(530, "Do you really want to deactivate the E-cooler?")
            MSG_WantToSwitchOnEcooler = ml_string(531, "Do you really want to activate the E-cooler?")
            MSG_CannotActivateEcooler = ml_string(532, "The E-cooler cannot be activated at the moment...")

            MSG_FilterPrinterPortNotAvailable = ml_string(588, "The serial port {0} ist not available for filter printer!")

            MSG_SpectrumNotAddedToDaySpectrum = ml_string(651, "Spectrum was not added to day spectrum due to fatal status: ")

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Function SYS_SetIntensiveModeOn() As String
        Try
            If Not _MyFHT59N3Par.IntensiveModeEnabled Then
                Return _FUNC_DISABLED
            End If

            If Not _MyControlCenter.SYS_States.IntensiveMode Then
                ' TFHN-11: Do some actions ONLY when not running in intensive mode before
                ' do not call _MyControlCenter.SPS_AlarmOn()
                _MyControlCenter.SYS_States.IntensiveMode = True

                _NextFilterStepMinuteDate = _AnalyzeMinuteDate

                GUI_ShowNextFilterStepTime()
                GUI_SetMenus()
                GUI_SetMessage(MSG_IntensiveModeON, MessageStates.YELLOW)
            End If
            Return _OK & " [IntensiveModeOn]"
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    Public Function SYS_SetIntensiveModeOff() As String
        Try
            If Not _MyFHT59N3Par.IntensiveModeEnabled Then
                Return _FUNC_DISABLED
            End If

            If (_MyControlCenter.SYS_States.IntensiveMode) Or (_MyControlCenter.SYS_States.AlarmMode) Then
                _MyControlCenter.SYS_States.IntensiveMode = False
                frmMain.UcStatusSideBar.BtnQuitAlarm.Visible = False
                _MyControlCenter.SYS_States.AlarmMode = False

                Try
                    'früher hierdrin gemacht, also bleibts vorerst so...
                    _MyControlCenter.SPS_AlarmOff()

                    GUI_SetMessage(MSG_IntensiveModeOFF, MessageStates.GREEN)
                    'unnötig: GUI_SetMessage(MSG_AlarmModeOFF, MessageStates.GREEN)

                    'Filterband Vorschub soll wieder normal erfolgen
                    Dim minOfDayNow As Integer = MinutesOfDayNow()
                    SYS_SynchronizeNextAlarmCheckTime(minOfDayNow, "set intensive mode off")
                    SYS_SynchronizeNextAnalyzationTime(minOfDayNow, "set intensive mode off")   'bestimmt nächste Auswerteminute
                    SYS_SynchronizeNextFilterStepTime(minOfDayNow, "set intensive mode off", skipToOverNextIfLargerHalfFilterTime:=False)

                    'die Auswertezeit darf nicht 'nach' der nächsten Filterwechselzeit liegen
                    'sonst wird der Filterschritt nie ausgelöst, siehe MCA_CheckIfMeasurementDone(..)
                    If (_AnalyzeMinuteDate > _NextFilterStepMinuteDate) Then
                        _AnalyzeMinuteDate = _NextFilterStepMinuteDate
                    End If

                    GUI_ShowNextAnalyzationMinute()
                    GUI_ShowNextFilterStepTime()
                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
                End Try


                GUI_SetMenus()

            End If
            Return _OK & " [IntensiveModeOff]"
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return _ERROR
        End Try
    End Function

    Public Sub SYS_SystemStateChangedHandler()
        Try
            _MyControlCenter.SYS_States.SaveMeBinary(_MonitorConfigDirectory & "\")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    ''' <summary>
    ''' Alarm-Mode einschalten: Filterbandwechsel erfolgt bei jeder Analyse
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SYS_SetAlarmModeOn()
        Try

            _MyControlCenter.SYS_States.AlarmMode = True

            _MyControlCenter.SPS_AlarmOn()

            frmMain.UcStatusSideBar.BtnQuitAlarm.Visible = True
            GUI_SetMessage(MSG_AlarmModeON, MessageStates.RED)


            If ChangeFilterTimeInAlarmMode() Then
                _NextFilterStepMinuteDate = _AnalyzeMinuteDate
            End If

            GUI_ShowNextFilterStepTime()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_SetAlarmModeOff()
        Try
            'früher hierdrin gemacht, also bleibts vorerst so...
            frmMain.UcStatusSideBar.BtnQuitAlarm.Visible = False

            _MyControlCenter.SYS_States.AlarmMode = False
            _MyControlCenter.SPS_AlarmOff()

            GUI_SetMessage(MSG_AlarmModeOFF, MessageStates.GREEN)

            'Filterband Vorschub soll wieder normal erfolgen
            Dim minOfDayNow As Integer = MinutesOfDayNow()
            SYS_SynchronizeNextAlarmCheckTime(minOfDayNow, "set alarm mode off")
            SYS_SynchronizeNextAnalyzationTime(minOfDayNow, "set alarm mode off")   'bestimmt nächste Auswerteminute
            SYS_SynchronizeNextFilterStepTime(minOfDayNow, "set alarm mode off", skipToOverNextIfLargerHalfFilterTime:=False)

            'die Auswertezeit darf nicht 'nach' der nächsten Filterwechselzeit liegen
            'sonst wird der Filterschritt nie ausgelöst, siehe MCA_CheckIfMeasurementDone(..)
            If (_AnalyzeMinuteDate > _NextFilterStepMinuteDate) Then
                _AnalyzeMinuteDate = _NextFilterStepMinuteDate
            End If

            GUI_ShowNextAnalyzationMinute()
            GUI_ShowNextFilterStepTime()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_AddAlarmNuclideToList(alarmNuclide As String, alarmLevel As Integer, nuclidThreshold As Double)

        frmMain.UcStatusSideBar.AddAlarmNuclideToList(alarmNuclide, alarmLevel, nuclidThreshold)

    End Sub


    Public Sub SYS_RemoveAllAlarmNuclidesFromList()

        frmMain.UcStatusSideBar.RemoveAllAlarmNuclidesFromList()

    End Sub

    'Registrieren der Hotkeys
    Public Sub SYS_AddHotkeys()
        Try
            frmMain._HotKey.AddHotKey(Keys.N, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "NextROI")
            frmMain._HotKey.AddHotKey(Keys.V, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "LastROI")
            frmMain._HotKey.AddHotKey(Keys.H, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "SetROI")
            frmMain._HotKey.AddHotKey(Keys.O, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "DelROI")
            frmMain._HotKey.AddHotKey(Keys.A, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "DelAllROIs")
            frmMain._HotKey.AddHotKey(Keys.S, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "SaveROIs")
            frmMain._HotKey.AddHotKey(Keys.L, Thermo_GlobalHotkeys.MODKEY.MOD_CONTROL, "LoadROIs")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Public Sub SYS_RemoveHotkeys()
        Try
            'Entfernen der Hotkeys
            frmMain._HotKey.RemoveHotKey("NextROI")
            frmMain._HotKey.RemoveHotKey("LastROI")
            frmMain._HotKey.RemoveHotKey("SetROI")
            frmMain._HotKey.RemoveHotKey("DelROI")
            frmMain._HotKey.RemoveHotKey("DelAllROIs")
            frmMain._HotKey.RemoveHotKey("SaveROIs")
            frmMain._HotKey.RemoveHotKey("LoadROIs")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    'Eingang des Hotkey-Events abfragen
    Public Sub SYS_ReceiveHotKey(ByVal HotKeyID As String)
        Try
            Select Case HotKeyID
                Case "NextROI"
                    GUI_NextROI()
                Case "LastROI"
                    GUI_PrevROI()
                Case "SetROI"
                    GUI_AddROI()
                Case "DelROI"
                    GUI_DeleteROI()
                Case "DelAllROIs"
                    GUI_DeleteAllROIs()
                Case "SaveROIs"
                    GUI_SaveROIs()
                Case "LoadROIs"
                    GUI_LoadROIs()
            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

    Private Sub SYS_InitFilterDrucker()
        ' Create a new SerialPort object with default settings.
        _FilterPrinterPort = New SerialPort()

        If (_MyFHT59N3Par.EnablePaperrollPrinter) Then
            _FilterPrinterPort.PortName = _MyFHT59N3Par.PrinterSerialPort
            _FilterPrinterPort.BaudRate = 300
            _FilterPrinterPort.Parity = Parity.Even
            _FilterPrinterPort.DataBits = 7
            _FilterPrinterPort.StopBits = 2
            _FilterPrinterPort.Handshake = Handshake.None

            Try
                _FilterPrinterPort.Open()
            Catch ex As Exception
                GUI_ShowMessageBox(String.Format(MSG_FilterPrinterPortNotAvailable, _MyFHT59N3Par.PrinterSerialPort) _
                    + vbCrLf + ex.Message,
                    "OK", "", "", MYCOL_MESSAGE_CRITICAL, Color.Black)
                _FilterPrinterPort = Nothing
            End Try
        End If
    End Sub

    Public Sub SYS_ReleaseFilterDrucker()
        If Not _FilterPrinterPort Is Nothing Then
            _FilterPrinterPort.Close()
            _FilterPrinterPort = Nothing
        End If
    End Sub

    Public Sub SYS_FilterBedrucken()
        Dim myNowString As String
        Dim currentDate As DateTime = DateTime.Now
        Dim printingDate As DateTime
        Dim advanceHours As Integer

        If (_MyFHT59N3Par.EnablePaperrollPrinter) Then
            If Not _FilterPrinterPort Is Nothing Then
                advanceHours = _FactorFilterPrinterTimeMultiply * _MyFHT59N3Par.FilterTimeh
                printingDate = currentDate.AddHours(advanceHours)

                'BAG möchte ein geändertes Format (xx ddmmyy hhmm wobei xx Stationsname)
                Dim stationId As String = "" + _MyFHT59N3Par.StationID
                'Die numerische Stations-ID kann länger als zwei Zeichen sein, daher nehmen 
                'wir nur die letzten zwei Ziffern, das genügt zur Eindeutigkeit
                If stationId.Length > 2 Then
                    stationId = stationId.Substring(stationId.Length - 2)
                End If
                myNowString = stationId + " "
                myNowString += printingDate.ToString("ddMMyy HHmm", CultureInfo.InvariantCulture)

                Try

                    'wir warten jetzt vorher und nachher, sicher ist sicher
                    Thread.Sleep(5000)
                    _FilterPrinterPort.Write("P" + myNowString + vbCr)
                    Thread.Sleep(5000)

                Catch ex As Exception
                    Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
                End Try
            Else
                Trace.TraceError("The paperroll printer is activated, but there is no serial port active")
            End If




        End If
    End Sub

    Private Sub SYS_StartSnmpCommunication()
        Dim ipAddressOfSnmp As String
        Dim portOfSnmp As Int16
        Dim snmpCommunity As String
        Dim isUpsAvailable As Boolean

        ipAddressOfSnmp = _MyFHT59N3Par.IpAddressOfUpsSnmp
        portOfSnmp = _MyFHT59N3Par.PortOfUpsSnmp
        snmpCommunity = _MyFHT59N3Par.SnmpCommunity
        isUpsAvailable = _MyFHT59N3Par.IsUpsAvailable

        snmp = New FHT59N3_SnmpCommunication(ipAddressOfSnmp, portOfSnmp, snmpCommunity)
        If isUpsAvailable Then
            snmp.LastRetrieve.HasChanged = False
            snmp.StartThread()
        End If

    End Sub

    Public Sub SYS_StopSnmpCommunication()
        If Not snmp Is Nothing Then
            snmp.StopThread()
        End If
    End Sub

    Public Sub SYS_StartN4242Cleanup()
        n4242Cleanup = New FHT59N3_N4242_FileCleanup(_MyFHT59N3Par.AnalyzationN4242FilesDirectory)
        n4242Cleanup.StartThread()
    End Sub

    Public Sub SYS_StopN4242Cleanup()
        If Not n4242Cleanup Is Nothing Then
            Try
                n4242Cleanup.StopThread()
            Catch ex As ApplicationException
                Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            End Try
        End If
    End Sub

    Private Sub SYS_EvaluateSnmpStatus()

        'Es soll keine UPS-Überwachung verwendet werden
        If snmp Is Nothing Or Not _MyFHT59N3Par.IsUpsAvailable Then
            _TimeStampOnUpsBattery = DateTime.Now
            _LastGuiMessageOutputOnUpsBattery = DateTime.MaxValue
            Return
        End If

        'Nochmals prüfen ob UPS verfügbar...
        If (snmp.LastRetrieve.HasChanged) Then
            If (snmp.LastRetrieve.Value = SnmpRetrieveResult.ValidData) Then
                GUI_SetMessage(MSG_UPS_SNMP_REACHABLE, MessageStates.GREEN)
                snmp.LastRetrieve.HasChanged = False
            ElseIf (snmp.LastRetrieve.Value = SnmpRetrieveResult.UpsConnectionError) Then

                If (snmp.LastRetrieve.ChangeTimestampOlderThan(FHT59N3_SnmpCommunication.SNMP_RETRIEVE_TIMEOUT_SECONDS)) Then
                    GUI_SetMessage(MSG_UPS_SNMP_NOT_REACHABLE, MessageStates.RED)
                    snmp.LastRetrieve.HasChanged = False
                End If
                Return
            End If
        End If


        Dim secondsPerMinute As Integer = 60
        'Shutdown des Rechners soll 10 Minuten nach der Erkennung USV Batterie-Betrieb erfolgen...
        Dim timeoutTimeSpan As Integer = 10 * secondsPerMinute

        'erstmal den aktuellen Status holen/synchronisieren bevor Aktionen ausgeführt werden...
        Dim upsOnBatteryOld As Boolean = _MyControlCenter.SYS_States.UpsOnBattery

        Dim isOnBattery As Boolean = snmp.GetUpsOnBattery
        _MyControlCenter.SYS_States.UpsOnBattery = isOnBattery

        If Not isOnBattery Then
            _TimeStampOnUpsBattery = DateTime.Now
        End If
        Dim timeDiffToLast As Single = DateTime.Now.Subtract(_TimeStampOnUpsBattery).TotalMilliseconds / 1000

        If isOnBattery And timeDiffToLast > timeoutTimeSpan Then
            SYS_UPS_ShutdownComputer()
        End If


        'Meldungen ausgeben falls sich was am Zustand geändert hat...
        If _MyControlCenter.SYS_States.UpsOnBattery Then

            If (_MyControlCenter.SYS_States.UpsOnBattery <> upsOnBatteryOld) Then
                _TimeStampOnUpsBattery = DateTime.Now
                GUI_SetMessage(MSG_UPS_STATUS_ONBATTERY, MessageStates.YELLOW)
                _LastGuiMessageOutputOnUpsBattery = DateTime.Now
            End If

            'Alle volle Minute eine Meldung ausgeben 
            Dim diffToLastOutput As Integer = DateTime.Now.Subtract(_LastGuiMessageOutputOnUpsBattery).TotalSeconds
            If diffToLastOutput > secondsPerMinute Then
                Dim remainingTimeMinutes As Integer = (timeoutTimeSpan - DateTime.Now.Subtract(_TimeStampOnUpsBattery).TotalSeconds) / 60
                GUI_SetMessage(String.Format(MSG_UPS_STATUS_PREPARING_SHUTDOWN, remainingTimeMinutes), MessageStates.YELLOW)
                _LastGuiMessageOutputOnUpsBattery = DateTime.Now
            End If

        End If

        If Not _MyControlCenter.SYS_States.UpsOnBattery And (_MyControlCenter.SYS_States.UpsOnBattery <> upsOnBatteryOld) Then
            GUI_SetMessage(MSG_UPS_STATUS_ONLINE, MessageStates.GREEN)
        End If

    End Sub

    Public Sub SYS_UPS_ShutdownComputer()

        Trace.TraceInformation("Started to shutdown computer due to timeout after UPS battery status.")
        GUI_SetMessage(MSG_UPS_ACTION_SHUTDOWN, MessageStates.RED)

        SYS_ForcedShutdownByUPS()

    End Sub



    Public Sub SYS_ForcedShutdownByUPS()

        Try
            'Änderung 15.08.2016: Zustandsvariable die verhindert das die frmMain.FormClosing durchlaufen wird wo beim händischen Herunterfahren
            'des Programms andere Aktionen durchgeführt werden.
            _ForcedEndProgram = True

            'Relevante Aktionen zuerst!
            _MyControlCenter.MCA_StopMeasurement(False)
            _MyControlCenter.MCA_SetHVOff()

            'we must shutdown immediately, try to push only this commands
            _MyControlCenter.ForceCommandStackFlush()

            'Änderung 3.8.2016: Zustand des E-Coolers so lassen wie er ist (bisher wurde E-Cooler explizit ausgeschaltet)

            _MyControlCenter.SPS_ErrorOn()
            _MyControlCenter.WaitTillStackEmpty(2)

            'Wartung nicht setzen! _MyControlCenter.SYS_States.Maintenance = True
            'KeepActive-Flags nicht ändern, wir forcieren das runterfahren
            'Einstellungen sichern
            SYS_WriteSettings()

            SYS_RemoveHotkeys()
            SYS_ReleaseFilterDrucker()
            SYS_StopSnmpCommunication()
            SYS_StopN4242Cleanup()
            _MyControlCenter.MDS_StopNetlog()

            'bis V2.0.1: sehr forciertes Ausschalten der Anwendung!
            'Dim thisProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess
            'thisProcess.Kill()
        Finally
            'ab V2.0.2: Festlegung Hr. Silbermann: hartes Ausschalten nach 10-Minuten-Timeout
            'Gibts auf jedem Windows 7 und höher, erfordert natürlich die entsprechende Benutzerberechtigung...
            Dim psi = New ProcessStartInfo("shutdown", "/s /t 0")
            psi.CreateNoWindow = True
            psi.UseShellExecute = False
            Process.Start(psi)
        End Try

    End Sub

    Private Function ChangeFilterTimeInAlarmMode() As Boolean
        Dim alarmModeValid As Boolean = _MyControlCenter.SYS_States.AlarmMode AndAlso Not _MyFHT59N3Par.AlarmModeSettings.HasFlag(AlarmModeSettings.DontChangeFilterTime)
        Return alarmModeValid
    End Function

    Private Sub InterruptedTracing(MethodCall As String)

        Trace.TraceInformation(MethodCall)
        If _MyFHT59N3Par.LynxCommandsShowMessageBox Then
            MessageBox.Show(MethodCall)
        End If
        If _MyFHT59N3Par.LynxCommandsDelayTime > 0 Then
            Thread.Sleep(_MyFHT59N3Par.LynxCommandsDelayTime)
        End If
    End Sub


#End Region

#Region "Remote"

    ''' <summary>
    ''' Remote access function
    ''' </summary>
    ''' <param name="ClientNumber"></param>
    ''' <param name="DataContainer">The datacontainer (structure) contains the command etc.</param>
    ''' <remarks></remarks>
    Private Sub SYS_RemoteCommandReceivedHandler(ByVal ClientNumber As String, ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface)
        Dim sep As String = "#"
        Dim ReceiveBuffer As String
        Dim Arguments() As Object = {}
        Dim Command As String
        Dim i As Integer = 0

        Try

            ReceiveBuffer = DataContainer.AnswerAsString
            Arguments = ReceiveBuffer.Split(sep.ToCharArray)
            If Arguments.Length > 0 Then

                Command = CType(Arguments(0), String)

                Select Case Command

                    Case "SetIntensiveMode"
                        If Arguments.Length = 2 Then
                            Dim OnOff As String = CType(Arguments(1), String)
                            If OnOff = "On" Then
                                _Protocol.BuildProtocolFrame(DataContainer, SYS_SetIntensiveModeOn)
                            ElseIf OnOff = "Off" Then
                                _Protocol.BuildProtocolFrame(DataContainer, SYS_SetIntensiveModeOff)
                            Else
                                _Protocol.BuildProtocolFrame(DataContainer, _ERROR & _WRONGARGUMENT)
                            End If
                        Else
                            _Protocol.BuildProtocolFrame(DataContainer, _ERROR & _NOTENOUGHARGUMENTS & " 1")
                        End If

                    Case "SetMaintenance"
                        If Arguments.Length = 2 Then
                            Dim OnOff As String = CType(Arguments(1), String)
                            If OnOff = "On" Then
                                _Protocol.BuildProtocolFrame(DataContainer, SPS_SetMaintenanceOn())
                            ElseIf OnOff = "Off" Then
                                _Protocol.BuildProtocolFrame(DataContainer, SPS_SetMaintenanceOff())
                            Else
                                _Protocol.BuildProtocolFrame(DataContainer, _ERROR & _WRONGARGUMENT)
                            End If
                        Else
                            _Protocol.BuildProtocolFrame(DataContainer, _ERROR & _NOTENOUGHARGUMENTS & " 1")
                        End If

                    Case "SetFilterstep"
                        _FiltertstepTries = 0   'fail counter zuruecksetzen
                        _Protocol.BuildProtocolFrame(DataContainer, SPS_SetFilterstep)

                    Case "SetBypass"
                        _Protocol.BuildProtocolFrame(DataContainer, SPS_SetBypass())

                    Case "SetPump"
                        _Protocol.BuildProtocolFrame(DataContainer, SPS_SetPump())

                    Case "SetHV"
                        _Protocol.BuildProtocolFrame(DataContainer, MCA_SetHighVoltage())

                    Case "GetMonitorState"
                        Dim Sb As New System.Text.StringBuilder
                        Dim ReturnString As String = ""
                        'Bender, November 2015: 
                        'ACHTUNG: Das Protokoll darf nur nach Rücksprache mit DWD geändert werden. Daher sind auch
                        'neue Statuszustände wie 'UPS-Betrieb' NICHT enthalten

                        'GeräteDaten: 
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_RemoteMaintenance).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_PumpOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_AlarmRelaisOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_FilterRippedOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_ErrorOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_HeatingOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_MotorOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_BypassOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_DetectorHeadOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_MaintenanceOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.Maintenance).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.K40ToLow_NotFound).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.AirFlowLessThen1Cubic).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.HVOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.NoFilterstep).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.BypassOpen).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.AnalyzationCancelled).ToString)
                        Sb.Append(";")
                        'ehemals SYS_States.PressureToLow. Gibt es in V2.0.0 nicht mehr (wurde umdefiniert)
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.AirFlowGreaterThen12Cubic).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.N2FillingGoingLow).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.FilterHasToBeChanged).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.CheckTempPressure).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.K40ToBig).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.AirFlowLessThen1Cubic).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.DataTransferError).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.SpectrumDeadTimeBigger20Percent).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.MeasValuesCorrupt).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.IntensiveMode).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SYS_States.AlarmMode).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_MyControlCenter.SPS_MotorOnOff).ToString)
                        Sb.Append(";")
                        Sb.Append(SYS_ConvertBoolToInt(_Measurement).ToString)
                        Sb.Append("$")
                        Sb.Append(Format(_MyControlCenter.SPS_N2Voltage, "0.###0E+00")) '0
                        Sb.Append(";")
                        Sb.Append(Format(_MyControlCenter.SPS_Temperature, "0.###0E+00")) '1
                        Sb.Append(";")
                        Sb.Append(Format(_MyControlCenter.SPS_PressureBezel, "0.###0E+00")) '2
                        Sb.Append(";")
                        Sb.Append(Format(_MyControlCenter.SPS_PressureFilter, "0.###0E+00")) '3
                        Sb.Append(";")
                        Sb.Append(Format(_MyControlCenter.SPS_PressureEnvironment, "0.###0E+00")) '4
                        Sb.Append(";")
                        Sb.Append(Format(_MyFHT59N3Par.FilterSteps, "0.###0E+00")) '5
                        Sb.Append(";")
                        Sb.Append(Format(_AirFlowActual, "0.###0E+00")) '6
                        Sb.Append(";")
                        Sb.Append(Format(_AirFlowMean, "0.###0E+00")) '7
                        Sb.Append(";")
                        Sb.Append(Format(_N2FillValue, "0.###0E+00")) '8
                        Sb.Append(";")
                        Sb.Append(Format(_Temperature, "0.###0E+00")) '9
                        Sb.Append(";")
                        Sb.Append(Format(_PressureFilter, "0.###0E+00")) '10
                        Sb.Append(";")
                        Sb.Append(Format(_PressureBezel, "0.###0E+00")) '11
                        Sb.Append(";")
                        Sb.Append(Format(_PressureEnvironment, "0.###0E+00")) '12
                        Sb.Append("$")
                        ReturnString = Sb.ToString
                        _Protocol.BuildProtocolFrame(DataContainer, ReturnString)
                    Case Else
                        _Protocol.BuildProtocolFrame(DataContainer, _ERROR & _COMMANDDOESNOTEXIST & " [" & Command & "]")

                End Select

            Else
                _Protocol.BuildProtocolFrame(DataContainer, _ERROR)
            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
        End Try
    End Sub

#End Region

End Module
