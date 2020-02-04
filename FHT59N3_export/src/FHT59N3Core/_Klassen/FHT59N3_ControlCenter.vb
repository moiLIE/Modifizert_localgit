Imports System.Threading

Public Class FHT59N3_ControlCenter

#Region "Private Eigenschaften"

    Private _ComSPS As FHT59N3_SPSStatemachine
    Private _ComLynx As FHT59N3_LynxCommunication
    Private _ComDataServer As FHT59N3_MDSCommunication
    Private WithEvents _ComRemote As FHT59N3_RemoteControlWebServer
    Private _SPS_FilterstepStarted As Boolean = False
    Private _SPS_Filterstep1stStarted As Boolean = False  'used when filter printer is active
    Private _SPS_Filterstep2ndStarted As Boolean = False  'used when filter printer is active
    Private _ParLynx As New FHT59N3_LynxParams
    Private WithEvents _FHT59N3States As New FHT59N3_SystemStates
    Private _MCANuclides As New FHT59N3MCA_NuclideList
    Private _MCAAlarmNuclides As New FHT59N3MCA_AlarmNuclides
    Private _MCAPeaks As New FHT59N3MCA_Peaks


    Private _CP5_Connection As BAGiPAConnection.BAGCryoCooler


#End Region

#Region "Enums"

    Public Enum MCATypes
        Canberra_Lynx = 1
        Ortec_DspecPlus = 2
    End Enum
    Private _MCAType As MCATypes

    Public Enum MCAPolarity
        Positive = 0
        Negative = 1
    End Enum

    Public Enum MCAMode_ManAuto
        Manual = 0
        Automatic = 1
    End Enum

    Public Enum MCAMode_BLR
        Automatic = 0
        Hard = 1
        Medium = 2
        Soft = 3
    End Enum

    Public Enum MCAMode_Acq
        PHA = 0
        MSS = 1
        DLFC = 2
        List = 3
        Tlist = 4
    End Enum

    Public Enum MCAMode_StabRange
        Ge = 0
        NaI = 1
    End Enum

    Public Enum MCAMode_Stabilizer
        StabOFF = 0
        StabON = 1
        StabHOLD = 2
    End Enum

#End Region

#Region "Öffentliche Eigenschaften"

#Region "SPS"

    ''' <summary>
    ''' A filterstep with the configured length was started, no filterprint is done
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SPS_FilterstepStartedFlag() As Boolean
        Get
            Return _SPS_FilterstepStarted
        End Get
        Set(ByVal value As Boolean)
            _SPS_FilterstepStarted = value
        End Set
    End Property

    Public Property SPS_FilterstepStarted1stFlag() As Boolean
        Get
            Return _SPS_Filterstep1stStarted
        End Get
        Set(ByVal value As Boolean)
            _SPS_Filterstep1stStarted = value
        End Set
    End Property

    Public Property SPS_FilterstepStarted2ndFlag() As Boolean
        Get
            Return _SPS_Filterstep2ndStarted
        End Get
        Set(ByVal value As Boolean)
            _SPS_Filterstep2ndStarted = value
        End Set
    End Property

    Public Property SPS_NewDigValuesAvailable() As Boolean
        Get
            Return _ComSPS.MySPSCommunication.NewDigValuesAvailable
        End Get
        Set(ByVal value As Boolean)
            _ComSPS.MySPSCommunication.NewDigValuesAvailable = value
        End Set
    End Property

    Public Property SPS_NewAnaValuesAvailable() As Boolean
        Get
            Return _ComSPS.MySPSCommunication.NewAnaValuesAvailable
        End Get
        Set(ByVal value As Boolean)
            _ComSPS.MySPSCommunication.NewAnaValuesAvailable = value
        End Set
    End Property

    Public ReadOnly Property SPS_TransferState() As FHT59N3_SPSStatemachine.TransferStates
        Get
            Return _ComSPS.MyTransferState
        End Get
    End Property

    ' digital values from SPS

    Public ReadOnly Property SPS_HVOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigIn_HVOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_CloseDetectorHead() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigIn_CloseDetectorHead
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_RemoteMaintenance() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigIn_MaintenanceOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_PumpOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_PumpOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_AlarmRelaisOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_AlarmRelaisOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_FilterRippedOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_FilterRippedOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_ErrorOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_ErrorOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_HeatingOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_HeatingOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_MotorOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_MotorOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_BypassOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_BypassOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_DetectorHeadOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_DetectorHeadOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_MaintenanceOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_MaintenanceOnOff
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_EcoolerOnOff() As Boolean
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.DigOut_EcoolerOnOff
            End If
        End Get
    End Property

    ' SPS analog values

    Public ReadOnly Property SPS_N2Voltage() As Double
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_N2Voltage
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_Temperature() As Double
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_Temperature
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_PressureBezel() As Double
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_PressureBezel
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_PressureFilter() As Double
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_PressureFilter
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_PressureEnvironment() As Double
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_PressureEnv
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_DetectorTemperature() As Double
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_DetectorTemperatur
            End If
        End Get
    End Property

    Public ReadOnly Property CanberraDetectorTemperature() As Double
        Get
            Return _CP5_Connection.CP5_ReadTemperature()
        End Get
    End Property

    ''' <summary>
    ''' Gets the second temperature. New for V2.0.3 (June 2017)
    ''' </summary>
    ''' <value>
    ''' The second temperature from analog input on SPS.
    ''' </value>
    Public ReadOnly Property SPS_ExternalTemperature() As Double
        Get
            If Not _ComSPS Is Nothing Then
                'Nach Rücksprache mit Armin Silbermann wurde es auf bisherige Reserve (E+/E-) gelegt
                Return _ComSPS.MySPSCommunication.DataContainer.AnaIn_Reserved
            End If
        End Get
    End Property


    ' SPS calculated values

    Public ReadOnly Property SPS_AirThroughPutNormReceived() As Integer
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.Calculated_AirThroughPutNormReceived
            End If
        End Get
    End Property

    Public ReadOnly Property SPS_AirThroughPutWorkingReceived() As Integer
        Get
            If Not _ComSPS Is Nothing Then
                Return _ComSPS.MySPSCommunication.DataContainer.Calculated_AirThroughPutWorkingReceived
            End If
        End Get
    End Property

#End Region


#Region "MCA"

    Public ReadOnly Property MCAType() As MCATypes
        Get
            Return _MCAType
        End Get
    End Property

    Public ReadOnly Property MCA() As Object
        Get
            Select Case _MCAType
                Case MCATypes.Canberra_Lynx
                    Return _ComLynx.MyDetector

                Case MCATypes.Ortec_DspecPlus

            End Select

            Return _ComLynx.MyDetector
        End Get
    End Property

    Public Property MCA_Params() As Object
        Get
            Select Case _MCAType
                Case MCATypes.Canberra_Lynx
                    Return _ParLynx

                Case MCATypes.Ortec_DspecPlus

            End Select

            Return _ParLynx
        End Get

        Set(ByVal value As Object)
            Select Case _MCAType
                Case MCATypes.Canberra_Lynx
                    _ParLynx = value

                Case MCATypes.Ortec_DspecPlus

            End Select
        End Set
    End Property

    Public ReadOnly Property MCA_Nuclides() As FHT59N3MCA_NuclideList
        Get
            Return _MCANuclides
        End Get
    End Property

    Public ReadOnly Property MCA_AlarmNuclides() As FHT59N3MCA_AlarmNuclides
        Get
            Return _MCAAlarmNuclides
        End Get
    End Property

    Public ReadOnly Property MCA_Peaks() As FHT59N3MCA_Peaks
        Get
            Return _MCAPeaks
        End Get
    End Property

    Public ReadOnly Property MCA_AquireDone() As Boolean
        Get
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    Dim Status As Long

                    Status = _ComLynx.MyDetectorStatus
                    If (Status And CanberraDeviceAccessLib.DeviceStatus.aAcquireDone) = CanberraDeviceAccessLib.DeviceStatus.aAcquireDone Then
                        Return True
                    Else
                        Return False
                    End If

                Case MCATypes.Ortec_DspecPlus

            End Select
        End Get
    End Property

    Public ReadOnly Property MCA_RealMeasTime() As Integer
        Get
            Return _ComLynx.MyDetectorRealMeasTime
        End Get
    End Property

    Public ReadOnly Property MCA_LiveMeasTime() As Integer
        Get
            Return _ComLynx.MyDetectorLiveMeasTime
        End Get
    End Property

    Public Property MCA_ExeFiles() As String
        Get
            Return _ComLynx.GetEnvironmentVar("EXEFILES")
        End Get
        Set(ByVal value As String)
            _ComLynx.SetEnvironmentVar("EXEFILES", value)
        End Set
    End Property

    Public Property MCA_CtlFiles() As String
        Get
            Return _ComLynx.GetEnvironmentVar("ASEQFILES")
        End Get
        Set(ByVal value As String)
            _ComLynx.SetEnvironmentVar("ASEQFILES", value)
        End Set
    End Property

#End Region

#Region "System"

    Public ReadOnly Property SYS_States() As FHT59N3_SystemStates
        Get
            Return _FHT59N3States
        End Get
    End Property

    Public Event SystemStateChanged()

#End Region

#Region "Remote"

    Public Event CommandReceived(ByVal ClientNumber As String, ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface)

#End Region

#End Region

#Region "Öffentliche Methoden"

#Region "Allgemein"

    ''' <summary>
    ''' Constructor for 
    ''' </summary>
    ''' <param name="ComPortSPS"></param>
    ''' <param name="NetworkAddress"></param>
    ''' <param name="RemotePort"></param>
    ''' <param name="SPSConnectionType"></param>
    ''' <param name="UseStxEtxProtocolSPS"></param>
    ''' <param name="MCAType"></param>
    ''' <param name="IPMCA"></param>
    ''' <param name="LogFilePath"></param>
    ''' <param name="SimulateLynxSystem"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal ComPortSPS As String, ByVal NetworkAddress As String, ByVal RemotePort As UInt16, ByVal SPSConnectionType As String,
                   ByVal UseStxEtxProtocolSPS As Boolean,
                   ByVal MCAType As MCATypes, ByVal IPMCA As String,
                   ByVal LogFilePath As String, ByVal SimulateLynxSystem As Boolean, ByRef BeforeLynxCommandSub As Action(Of String))
        Try

            _MCAType = MCAType
            If (SPSConnectionType Is Nothing Or SPSConnectionType.Contains("serial") Or SPSConnectionType.Contains("seriell")) Then
                _ComSPS = New FHT59N3_SPSStatemachine(ComPortSPS, UseStxEtxProtocolSPS)
            Else
                _ComSPS = New FHT59N3_SPSStatemachine(NetworkAddress, RemotePort, UseStxEtxProtocolSPS)
            End If
            _ComSPS.InitSPS()

            Select Case _MCAType
                Case MCATypes.Canberra_Lynx
                    _ComLynx = New FHT59N3_LynxCommunication(IPMCA, SimulateLynxSystem)
                    _ComLynx.BeforeLynxCommandSub = BeforeLynxCommandSub


                    _ComLynx.SetGenieEnvironment()
                Case MCATypes.Ortec_DspecPlus
            End Select

        Catch ex As Exception
            Trace.TraceError("Error communicating to Lynx: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try

        'Die Schnittstelle zu Netview starten...
        Try
            _ComDataServer = New FHT59N3_MDSCommunication
        Catch ex As Exception
            Trace.TraceError("Error starting communication to NetView: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try


        'Den Remoteschnittstellen-Server starten...
        Try
            _ComRemote = New FHT59N3_RemoteControlWebServer(LogFilePath)
        Catch ex As Exception
            Trace.TraceError("Error starting remote control webserver: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
        'Das Interface zum CryoCooler starten...
        If True Then
            _CP5_Connection = New BAGiPAConnection.BAGCryoCooler("COM3")
        End If
    End Sub

    Public Sub Dispose()
        Try
            _ComSPS.Dispose()
            _ComLynx.Dispose()
            _ComRemote.Dispose()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub TriggerStatemachines()
        Try
            _ComSPS.DoMyJobs()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "SPS"

    ' activation/deactivation of signals to the SPS

    Public Sub SPS_SetFilterstep(ByVal StepWidth_mm As Integer)
        Try
            _SPS_FilterstepStarted = True  'used for feedback control in FHT59N3_ControlFunctions.vb

            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_SetFilterstep, StepWidth_mm, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub SPS_DetectorheadOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_DetectorHeadOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_DetectorHeadOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_DetectorheadOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_HeatingOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_HeatingOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_HeatingOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_HeatingOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_PumpOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_PumpOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_PumpOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_PumpOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>sendet NUR NUR NUR den SPS Befehl</remarks>
    Public Sub SPS_AlarmOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_AlarmOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_AlarmOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_AlarmOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_BypassOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_BypassOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_BypassOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_BypassOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_ErrorOn()
        Try
            _ComSPS.PutCommand(FHT59N3_SPSStatemachine.Commands.CM_ErrorOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_ErrorOff()
        Try
            _ComSPS.PutCommand(FHT59N3_SPSStatemachine.Commands.CM_ErrorOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_EcoolerOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_EcoolerOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_EcoolerOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_EcoolerOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Set the Airflow bezel factor for the SPS
    ''' </summary>
    ''' <param name="FactorBezel"></param>
    ''' <remarks></remarks>
    Public Sub SPS_SetAirflowBezelFactor(FactorBezel As Double)
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_SetFactorBezel, FactorBezel, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_SetAirflowSetPoint(AirFlowMode As Boolean, AirFlowSetPoint As Integer)
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_SetAirFlowSetpoint, AirFlowMode, AirFlowSetPoint, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub



    ' more complex SPS requests

    Public Sub SPS_GetDigStates()
        Try
            _ComSPS.PutCommand(FHT59N3_SPSStatemachine.Commands.CM_GetDigStates, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_GetAnaStates()
        Try
            _ComSPS.PutCommand(FHT59N3_SPSStatemachine.Commands.CM_GetAnaStates, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Gets the calculations from SPS, available since SPS via TCP/IP
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SPS_GetCalculations()
        Try
            _ComSPS.PutCommand(FHT59N3_SPSStatemachine.Commands.CM_GetCalculations, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_MaintenanceOn()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_MaintenanceOn, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub SPS_MaintenanceOff()
        Try
            _ComSPS.PushCommand(FHT59N3_SPSStatemachine.Commands.CM_MainteneanceOff, 0, 0, 0, 0)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "MCA"

    Public Sub MCA_StartMeasurement(ByVal MeasurementTimemin As Integer, ByVal CountToRealTime As Boolean, ByVal WithAir As Boolean)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.StartMeasurement(MeasurementTimemin, CountToRealTime, WithAir)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_ReStartMeasurement(Optional ByVal MeasurementTimemin As Integer = 0)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.ReStartMeasurement(MeasurementTimemin)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_StopMeasurement(ByVal Pause As Boolean)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.StopMeasurement(Pause)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetParamsForActualSpectrum(ByVal SUNITS As String, ByVal ASP2 As Double, ByVal ASP3 As Integer, ByVal ASP4 As Integer, ByVal SSPRSTR4 As String)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetParamsForActualSpectrum(SUNITS, ASP2, ASP3, ASP4, SSPRSTR4)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_GetParamsFromActualSpectrum(ByRef ASPSTR As String, ByRef SSPRSTR3 As String)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.GetParamsFromActualSpectrum(ASPSTR, SSPRSTR3)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SaveSpectrum(ByVal FileName As String, ByVal OverWrite As Boolean)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SaveSpectrum(FileName, OverWrite)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_ClearDetector()
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.ClearDetector()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetDetectorParams(ByVal Customer As String, ByVal StationName As String, ByVal StationID As String)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetDetectorParams(Customer, StationName, StationID)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetHVParams(ByVal Range As Integer, ByVal Limit As Integer, ByVal Voltage As Integer, ByVal DetectorPolarity As MCAPolarity, ByVal InhibitPolarity As MCAPolarity)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetHVParams(Range, Limit, Voltage, DetectorPolarity, InhibitPolarity)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetHVOn()
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetHVOn()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetHVOff()
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetHVOff()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Gets the state of high voltage via "Lynx" or other external components
    ''' </summary>
    ''' <returns>true: HV is activated, false: HV is off</returns>
    ''' <remarks></remarks>
    Public Function MCA_GetHVState() As Boolean
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    Return _ComLynx.GetHVState()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function MCA_GetHVInhibitState() As Boolean
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    Return _ComLynx.GetHVInhibitState()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Sub MCA_SetAmplifierParams(ByVal InputPolarity As MCAPolarity, ByVal CoarseGain As Double, ByVal FineGain As Double, ByVal PoleZero As Integer, ByVal BLRMode As MCAMode_BLR, ByVal FilterRiseTime As Double, ByVal FilterFlatTop As Double)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetAmplifierParams(InputPolarity, CoarseGain, FineGain, PoleZero, BLRMode, FilterRiseTime, FilterFlatTop)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetADCParams(ByVal AcquisitionMode As MCAMode_Acq, ByVal LLDMode As MCAMode_ManAuto, ByVal LLD As Double, ByVal ULD As Double, ByVal ConversionGain As Integer)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetADCParams(AcquisitionMode, LLDMode, LLD, ULD, ConversionGain)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetStabilizerParameters(ByVal Centroid As Integer, ByVal Window As Integer, ByVal Spacing As Integer, ByVal Multiplier As Integer, ByVal WindowRatio As Double, ByVal UseNaI As MCAMode_StabRange, ByVal GainRatioAutoMode As MCAMode_ManAuto)
        Try
            Select Case _MCAType
                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetStabilizerParameters(Centroid, Window, Spacing, Multiplier, WindowRatio, UseNaI, GainRatioAutoMode)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetStabilizerMode(ByVal StabMode As MCAMode_Stabilizer)
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.SetStabilizerMode(StabMode)

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetAllMeasParams()
        Try
            MCA_SetHVParams(_ParLynx.HV_Range, _ParLynx.HV_Limit, _ParLynx.HV_Voltage, _ParLynx.HV_DetectorPolarity, _ParLynx.HV_InhibitPolarity)
            MCA_SetAmplifierParams(_ParLynx.AMP_InputPolarity, _ParLynx.AMP_CoarseGain, _ParLynx.AMP_FineGain, _ParLynx.AMP_PoleZero, _ParLynx.AMP_BLRMode, _ParLynx.AMP_FilterRiseTime, _ParLynx.AMP_FilterFlatTop)
            MCA_SetADCParams(_ParLynx.ADC_AcquisitionMode, _ParLynx.ADC_LLDMode, _ParLynx.ADC_LLD, _ParLynx.ADC_ULD, _ParLynx.ADC_ConversionGain)
            MCA_SetStabilizerParameters(_ParLynx.STAB_Centroid, _ParLynx.STAB_Window, _ParLynx.STAB_Spacing, _ParLynx.STAB_Multiplier, _ParLynx.STAB_WindowRatio, _ParLynx.STAB_UseNaI, _ParLynx.STAB_GainRatioAutoMode)
            MCA_SetStabilizerMode(_ParLynx.STAB_StabMode)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub



    Public Sub MCA_ConnectToDetector()
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.ConnectDetector()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_DisconnectFromDetector()
        Try
            Select Case _MCAType

                Case MCATypes.Canberra_Lynx
                    _ComLynx.DisconnectFromDetector()

                Case MCATypes.Ortec_DspecPlus

            End Select
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetAutoPoleZero()
        Try
            _ComLynx.SetAutoPoleZero()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Function MCA_GetPoleZeroValue() As Integer
        Try
            Return _ComLynx.GetPoleZeroValue
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Function MCA_GetPoleZeroBusyState() As Boolean
        Try
            Return _ComLynx.GetPoleZeroBusyState
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    Public Sub MCA_SetECSlope(ByVal ECSLOPE As Double)
        Try
            _ComLynx.SetECSlope(ECSLOPE)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetECOffset(ByVal ECOFFSET As Double)
        Try
            _ComLynx.SetECOffset(ECOFFSET)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MCA_SetECQuad(ByVal ECQUAD As Double)
        Try
            _ComLynx.SetECQuad(ECQUAD)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Monitor Data Server"

    Public Function MDS_StartOrRestartNetlog(ByVal NetViewPath As String, ByVal NetViewActive As Boolean) As String
        Try
            Return _ComDataServer.StartOrRestartNetlog(NetViewPath, NetViewActive)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return ""
        End Try
    End Function

    Public Sub MDS_StopNetlog()
        Try
            If Not _ComDataServer Is Nothing Then
                _ComDataServer.StopNetlog()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub MDS_SaveAnalyzationResultsToDataServer(ByVal MCANuclides As FHT59N3MCA_NuclideList, ByVal MyFHT59N3States As FHT59N3_SystemStates, ByVal AirFlowMean As Single, ByVal SpecType As Integer, Optional ByVal DeleteValues As Boolean = False)
        Try
            _ComDataServer.SaveAnalyzationResultsToDataServer(MCANuclides, MyFHT59N3States, AirFlowMean, SpecType, DeleteValues)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Remote"

    Private Sub CommandReceivedHandler(ByVal ClientNumber As String, ByVal DataContainer As ThermoInterfaces.ThermoDataContainer_Interface) Handles _ComRemote.CommandReceived
        RaiseEvent CommandReceived(ClientNumber, DataContainer)
    End Sub

#End Region

#Region "System"

    Public Sub SYS_RestoreStatesFromFile(ByVal Path As String)
        Try
            Dim States As FHT59N3_SystemStates = _FHT59N3States.RestoreMeFromFileBinary(Path)
            If Not States Is Nothing Then
                _FHT59N3States = States
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Private Sub SystemStateChangedHandler() Handles _FHT59N3States.StateChanged
        RaiseEvent SystemStateChanged()
    End Sub

#End Region

#End Region

    Sub ForceCommandStackFlush()
        _ComSPS.ForceCommandStackFlush()
    End Sub

    Sub WaitTillStackEmpty(ByVal secondsToWait As Integer)
        _ComSPS.WaitTillStackEmpty(secondsToWait)
    End Sub


End Class
