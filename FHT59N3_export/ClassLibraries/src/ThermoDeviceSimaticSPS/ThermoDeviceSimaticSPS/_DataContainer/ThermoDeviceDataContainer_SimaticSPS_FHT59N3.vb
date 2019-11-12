Public Class ThermoDeviceDataContainer_SimaticSPS_FHT59N3
    Implements ThermoInterfaces.ThermoDataContainer_Interface

    ''' <summary>
    ''' 'mit Protocol
    ''' </summary>
    Private ReadOnly _CommandAsArray(1000) As Byte


    ''' <summary>
    ''' ohne protocol
    ''' </summary>
    Private ReadOnly _AnswerAsArray(1000) As Byte


    ''' <summary>
    ''' zu versendendes Kommando
    ''' </summary>
    ''' <value></value>
    Public Property Command As String Implements ThermoInterfaces.ThermoDataContainer_Interface.Command


    Public Property CommandAsString As String Implements ThermoInterfaces.ThermoDataContainer_Interface.CommandAsString


    Public Property CommandAsArray() As Byte() Implements ThermoInterfaces.ThermoDataContainer_Interface.CommandAsArray
        Get
            Return _CommandAsArray
        End Get
        Set(ByVal value As Byte())
            value.CopyTo(_CommandAsArray, 0)
        End Set
    End Property

    Public Property CommandLength As Integer Implements ThermoInterfaces.ThermoDataContainer_Interface.CommandLength


    Public Property AnswerAsString As String Implements ThermoInterfaces.ThermoDataContainer_Interface.AnswerAsString


    Public Property AnswerAsArray() As Byte() Implements ThermoInterfaces.ThermoDataContainer_Interface.AnswerAsArray
        Get
            Return _AnswerAsArray
        End Get
        Set(ByVal value As Byte())
            value.CopyTo(_AnswerAsArray, 0)
        End Set
    End Property

    Public Property AnswerLength As Integer Implements ThermoInterfaces.ThermoDataContainer_Interface.AnswerLength

	'seit SPS 2.0
    Public Property Filterstep_mm As Integer
	'seit SPS 2.0
    Public Property FactorBezel As Double
	'seit SPS 2.0
    Public Property ThroughputAirNorm As Integer
	'seit SPS 2.0
    Public Property ThroughputAirOperation As Integer


    Public Property DigIn_HVOnOff As Boolean

    Public Property DigIn_CloseDetectorHead As Boolean

    Public Property DigIn_MaintenanceOnOff As Boolean


    ''' <summary>
    ''' Bit 0 (0000 0001) 0x01
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_PumpOnOff As Boolean

    ''' <summary>
    ''' Bit 1 (0000 0010) 0x02
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_AlarmRelaisOnOff As Boolean

    ''' <summary>
    ''' Bit 2 (0000 0100) 0x04
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_FilterRippedOnOff As Boolean

    ''' <summary>
    ''' Bit 3 (0000 1000) 0x08
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_ErrorOnOff As Boolean

    ''' <summary>
    ''' Bit 4 (0001 0000) 0x10
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_HeatingOnOff As Boolean

    ''' <summary>
    ''' Bit 5 (0010 0000) 0x20
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_MotorOnOff As Boolean

    ''' <summary>
    ''' Bit 7 (1000 0000) 0x80
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_BypassOnOff As Boolean

    ''' <summary>
    ''' Bit 8 (0000 0001 0000 0000) 0x0100
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_DetectorHeadOnOff As Boolean

    ''' <summary>
    ''' Activate/Deactivate the electrical line to E-cooler
    ''' Bit 6 (0100 0000) 0x40
    ''' </summary>
    ''' <value>true = cooling, false = not cooling</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_EcoolerOnOff As Boolean

    ''' <summary>
    ''' Bit 9 (0000 0010 0000 0000) 0x0200
    ''' </summary>
    ''' <value>
    ''' true = open
    ''' false = closed
    ''' </value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DigOut_MaintenanceOnOff As Boolean


    Public Property AnaIn_Temperature As Single

    Public Property AnaIn_PressureFilter As Single

    Public Property AnaIn_PressureBezel As Single

    Public Property AnaIn_N2Voltage As Single

    Public Property AnaIn_PressureEnv As Single

    Public Property AnaIn_Reserved As Single

    Public Property AnaIn_DetectorTemperatur As Single

    Public Property AnaIn_ReservedG As Single

    Public Property AnaIn_ReservedH As Single


    Public Property Calculated_AirThroughPutNormReceived As Integer

    Public Property Calculated_AirThroughPutWorkingReceived As Integer

    Public Property VersionNumber As String

End Class

