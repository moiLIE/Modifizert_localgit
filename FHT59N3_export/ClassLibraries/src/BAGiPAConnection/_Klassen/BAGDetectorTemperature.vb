Public Class BAGDetectorTemperature
    'Private Shared _LastSucessfullReadTime As DateTime

    'Timeout if the newest value readout by the Java programm is older than this value
    'unit: Minutes
    Private Const _TimeOut As Double = 10

    Private Const _TemperatureFileLocaton As String = "C:\FHT59N3\iPA_Temperature\TemperatureLog.txt"

    Private Const _InvalidTemperature As Double = Double.MinValue


    Public ReadOnly Property iPA_ReadTemperature(ByVal path As String) As Double
        'returns the current Temperature or ?? if unable to read it.
        Get
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
                    Return _InvalidTemperature
                End If
                Return JavaReadoutTemperature
            Catch ex As Exception
                Return _InvalidTemperature
            End Try
        End Get
    End Property

End Class
