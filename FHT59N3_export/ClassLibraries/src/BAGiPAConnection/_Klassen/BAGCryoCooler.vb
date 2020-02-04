Public Class BAGCryoCooler

    Private _CP5Port As IO.Ports.SerialPort
    Private _CP5_Portnumber As String

    Private _Last_CoolerStatus As Boolean
    Private _LastPower As Double
    Private _LastTemperature As Double
    Private _LastColdTipTemp As Double
    Private _LastCompressorTemp As Double
    Private _LastControllerTemp As Double
    Private _LastErrorStatus As Double

    Private _Last_Readback As DateTime
    Private Const _ReadbackCycle As Integer = 30 'in seconds

    Private Const _readbackFailedValue As Double = Double.MinValue


    Private Sub _InitCP5()
        ' Create a new SerialPort object with default settings.
        _CP5Port = New IO.Ports.SerialPort()

        Dim Portname As String = _CP5_Portnumber

        _CP5Port.PortName = Portname
        _CP5Port.BaudRate = 9600
        _CP5Port.Parity = IO.Ports.Parity.None
        _CP5Port.DataBits = 8
        _CP5Port.StopBits = 1
        _CP5Port.Handshake = IO.Ports.Handshake.None

        _CP5Port.ReadTimeout = 500
        _CP5Port.WriteTimeout = 500

        Try
            _CP5Port.Open()
        Catch ex As Exception
            _CP5Port = Nothing
        End Try
    End Sub


    Public Sub _ReleaseCP5()
        If Not _CP5Port Is Nothing Then
            If _CP5Port.IsOpen Then
                _CP5Port.Close()
            End If

            _CP5Port = Nothing
        End If
    End Sub

    Private Sub _UpdateValues()
        If (DateTime.Now - _Last_Readback).TotalSeconds > _ReadbackCycle Then
            _Last_Readback = DateTime.Now

            Try ' update the values
                _ReadoutValues()
            Catch ex As Exception ' default  the values
                _ResetValues()
                _ReleaseCP5()
            End Try
        End If
    End Sub

    Private Sub _ReadoutValues()
        _InitCP5()
        UpdateColdTipTemp()
        UpdateCompressorTemp()
        UpdateControllerTemp()
        UpdateDetectorTemperature()
        UpdateErrorStatus()
        UpdatePower()
        UpdateStatus()
        _ReleaseCP5()
    End Sub

    Private Sub _ResetValues()
        _Last_CoolerStatus = False
        _LastPower = 0
        _LastTemperature = _readbackFailedValue
        _LastColdTipTemp = _readbackFailedValue
        _LastCompressorTemp = _readbackFailedValue
        _LastControllerTemp = _readbackFailedValue
        _LastErrorStatus = "Readback failed."
    End Sub



    Private Sub UpdateDetectorTemperature()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("TEMPC")
            Dim Readback As String = _CP5Port.ReadLine()
            _LastTemperature = Val(Readback)
            Exit Sub
        End If
        _LastTemperature = _readbackFailedValue
    End Sub

    Public Sub UpdatePower()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("PWR")
            Dim Readback As String = _CP5Port.ReadLine()
            _LastPower = Val(Readback)
            Exit Sub
        End If
        _LastPower = 0
    End Sub
    Public Sub UpdateStatus()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("REMOTE")
            Dim Readback As String = _CP5Port.ReadLine()
            If Readback = "1" Then
                _Last_CoolerStatus = True
                Exit Sub
            End If
        End If
            _Last_CoolerStatus = False
    End Sub


    Private Sub UpdateColdTipTemp()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("TWE")
            Dim Readback As String = _CP5Port.ReadLine()
            _LastColdTipTemp = Val(Readback)
            Exit Sub
        End If
        _LastColdTipTemp = _readbackFailedValue
    End Sub

    Private Sub UpdateCompressorTemp()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("TCO")
            Dim Readback As String = _CP5Port.ReadLine()
            _LastCompressorTemp = Val(Readback)
            Exit Sub
        End If
        _LastCompressorTemp = _readbackFailedValue
    End Sub
    Private Sub UpdateControllerTemp()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("TBRD")
            Dim Readback As String = _CP5Port.ReadLine()
            _LastControllerTemp = Val(Readback)
            Exit Sub
        End If
        _LastControllerTemp = _readbackFailedValue
    End Sub
    Private Sub UpdateErrorStatus()
        If Not _CP5Port Is Nothing Then
            _CP5Port.WriteLine("ERROR")
            Dim Readback As String = _CP5Port.ReadLine()
            _LastErrorStatus = Val(Readback)
            Exit Sub
        End If
        _LastErrorStatus = "Readback failed."
    End Sub


    'Constructor
    Public Sub New(ByVal ComPortCP5 As String)
        _CP5_Portnumber = ComPortCP5
        _ResetValues()
    End Sub

    'Readback Parameter
    Public ReadOnly Property CP5_ReadTemperature() As Double
        Get
            _UpdateValues()
            Return _LastTemperature
        End Get
    End Property
    Public ReadOnly Property CP5_ReadPower() As Double
        Get
            _UpdateValues()
            Return _LastPower
        End Get
    End Property
    Public ReadOnly Property CP5_Status() As Double
        Get
            _UpdateValues()
            Return _Last_CoolerStatus
        End Get
    End Property

    Public ReadOnly Property CP5_ColdTipTemp() As Double
        Get
            _UpdateValues()
            Return _LastColdTipTemp
        End Get
    End Property

    Public ReadOnly Property CP5_CompressorTemp() As Double
        Get
            _UpdateValues()
            Return _LastCompressorTemp
        End Get
    End Property

    Public ReadOnly Property CP5_ControllerTemp() As Double
        Get
            _UpdateValues()
            Return _LastControllerTemp
        End Get
    End Property
    Public ReadOnly Property CP5_GetErrorStatus() As Double
        Get
            _UpdateValues()
            Return _LastErrorStatus
        End Get
    End Property


    'Control Functions
    Public Sub CP5_ON()
        Try
            _InitCP5()
            If Not _CP5Port Is Nothing Then
                _CP5Port.WriteLine("REMOTE 1")
                Dim Readback As String = _CP5Port.ReadLine()
                'Readback should be "y"
            End If
            _ReleaseCP5()
        Catch ex As Exception
            _ReleaseCP5()
        End Try
    End Sub
    Public Sub CP5_OFF()
        Try
            _InitCP5()
            If Not _CP5Port Is Nothing Then
                _CP5Port.WriteLine("REMOTE 0")
                Dim Readback As String = _CP5Port.ReadLine()
                'Readback should be "y"
            End If
            _ReleaseCP5()
        Catch ex As Exception
            _ReleaseCP5()
        End Try
    End Sub
End Class





