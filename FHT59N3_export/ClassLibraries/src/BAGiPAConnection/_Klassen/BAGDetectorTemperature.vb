﻿Public Class BAGDetectorTemperature
    'Private Shared _LastSucessfullReadTime As DateTime

    'Timeout if the newest value readout by the Java programm is older than this value
    'unit: Minutes
    Private Const _TimeOut As Double = 10
    Private Const _ReadbackCycle As Integer = 2 'minutes
    Private Const _InvalidTemperature As Double = Double.MinValue

    'Default Values:
    Private Const _TemperatureFileLocaton As String = "C:\FHT59N3\iPA_Temperature\TemperatureLog.txt"
    Private Const _JarFileLocation As String = "C:\FHT59N3\iPA_Temperature\ipATemperatureLogger.jar"

    Private _LastTemperature As Double
    Private _Last_Temperature_Readback As DateTime

    Public ReadOnly Property iPA_ReadTemperature(ByVal jarFile As String, ByVal Logfile As String, ByVal ComPort As String) As Double
        'returns the current Temperature or _InvalidTemperature if unable to read it.
        Get
            ''Readout the file once every _ReadbackCycle. 
            If (DateTime.Now - _Last_Temperature_Readback).TotalMinutes > _ReadbackCycle Then
                _Last_Temperature_Readback = DateTime.Now

                Try 'read in temperature
                    Dim JavaReadoutTime As DateTime
                    Dim JavaReadoutTemperature As Double

                    If jarFile = "" Then
                        jarFile = _JarFileLocation
                    End If

                    If Logfile = "" Then
                        Logfile = _TemperatureFileLocaton
                    End If

                    'Trigger the Java Programm that interfaces with the iPA:
                    Using JavaProcess As Process = Process.Start("java", " -jar " + jarFile + " " + Logfile + " " + ComPort)
                        JavaProcess.WaitForExit(10000) 'Wait for maximal 10s
                        If Not JavaProcess.HasExited Then
                            'Kill it if it hang up.
                            'Attetion, this might cause the Virtual COM Port to end up in an illdefined state.
                            'However, as this is only the temperature readback, and the HV inhibit is done directly between iPA and CP5
                            ' and not via this software, this should be fine.
                            JavaProcess.Kill()
                        End If
                    End Using

                    Using FileReader As New FileIO.TextFieldParser(Logfile)
                        FileReader.TextFieldType = FileIO.FieldType.Delimited
                        FileReader.SetDelimiters(",")
                        Dim firstRow As String() = FileReader.ReadFields()
                        'Time: firstRow(0)
                        'Temperature: firstRow(1)
                        JavaReadoutTime = DateTime.ParseExact(firstRow(0), "dd.MM.yyyy HH:mm:ss", Globalization.DateTimeFormatInfo.InvariantInfo)
                        JavaReadoutTemperature = Val(firstRow(1))
                    End Using
                    'Check if the value is sufficiently recent
                    If (DateTime.Now - JavaReadoutTime).TotalMinutes > _TimeOut Then
                        _LastTemperature = _InvalidTemperature
                    Else
                        _LastTemperature = JavaReadoutTemperature
                    End If
                Catch ex As Exception
                    _LastTemperature = _InvalidTemperature
                End Try
            End If
            Return _LastTemperature

        End Get
    End Property

End Class
