Public Class BAGDetectorTemperature
    'Private Shared _LastSucessfullReadTime As DateTime

    'Timeout if the newest value readout by the Java programm is older than this value
    'unit: Minutes
    Private Const _TimeOut As Double = 10
    Private Const _TemperatureFileLocaton As String = "C:\FHT59N3\iPA_Temperature\TemperatureLog.txt"
    Private Const _ReadbackCycle As Integer = 1 'minutes
    Private Const _InvalidTemperature As Double = Double.MinValue

    Private _LastTemperature As Double
    Private _Last_Temperature_Readback As DateTime

    Public ReadOnly Property iPA_ReadTemperature(ByVal path As String) As Double
        'returns the current Temperature or _InvalidTemperature if unable to read it.
        Get
            ''Readout the file once every _ReadbackCycle. Java anyway only updates it once in 5 min.
            If (DateTime.Now - _Last_Temperature_Readback).TotalMinutes > _ReadbackCycle Then
                _Last_Temperature_Readback = DateTime.Now

                Try 'read in temperature
                    Dim JavaReadoutTime As DateTime
                    Dim JavaReadoutTemperature As Double

                    If path = "" Then
                        path = _TemperatureFileLocaton
                    End If

                    Using FileReader As New FileIO.TextFieldParser(path)
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
