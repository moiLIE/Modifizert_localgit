Public Class BAGDetectorTemperature
    'Private Shared _LastSucessfullReadTime As DateTime

    'Timeout if the newest value readout by the Java programm is older than this value
    'unit: Minutes
    Private Const _TimeOut As Double = 10

    Private Const _TemperatureFileLocaton As String = "C:\FHT59N3\iPA_Temperature\TemperatureLog.txt"

    '#TODO
    Private Const _InvalidTemperature As Double = 0


    Public ReadOnly Property iPA_ReadTemperature() As Double
        'returns the current Temperature or ?? if unable to read it.
        Get
            Try 'read in temperature
                Dim JavaReadoutTime As DateTime
                Dim JavaReadoutTemperature As Double

                Using FileReader As New FileIO.TextFieldParser(_TemperatureFileLocaton)
                    FileReader.TextFieldType = FileIO.FieldType.Delimited
                    FileReader.SetDelimiters(",")
                    Dim firstRow As String() = FileReader.ReadFields()
                    'Time: firstRow(0)
                    'Temperature: firstRow(1)
                    JavaReadoutTime = DateTime.ParseExact(firstRow(0), "dd.MM.yyyy HH:mm:ss", Globalization.DateTimeFormatInfo.InvariantInfo)
                    If Not Double.TryParse(firstRow(1), JavaReadoutTemperature) Then
                        Return _InvalidTemperature
                    End If
                End Using
                'Check if the value is sufficiently recent
                If (DateTime.Now - JavaReadoutTime).TotalMinutes > _TimeOut Then
                Return _InvalidTemperature
            End If

                Return JavaReadoutTemperature
            Catch ex As Exception
                Return _InvalidTemperature
            End Try
        End Get
    End Property

End Class
