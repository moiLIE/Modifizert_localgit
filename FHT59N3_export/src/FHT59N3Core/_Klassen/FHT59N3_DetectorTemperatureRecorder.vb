' -------------------------------------------------------------
' $Id: FHT59N3_DetectorTemperatureRecorder.vb 97 2014-07-18 10:55:43Z psa $
' Title: Detector demperature history recorder
'
' Description:
' Management class for storing, loading and recording of 
' crystal temperatures for display and investigations
' -------------------------------------------------------------


Public Class FHT59N3_DetectorTemperatureRecorder

    ''' <summary>
    ''' Defines whether the data was initially loaded from disk
    ''' </summary>
    ''' <remarks></remarks>
    Private m_wasLoaded As Boolean

    'first element in temperature history (oldest)
    Private m_firstKey As Nullable(Of Long)

    'last element in temperature history (most recent)
    Private m_lastKey As Nullable(Of Long)

    'temperature history, contains all recorded temperatures with time stamps as keys 
    '(time stamps are converted from DateTime objects with short year notation)
    Private ReadOnly m_temperatures As New SortedDictionary(Of Long, Double)

    'period for displaying the graph and calculating locale minimums and maximums in minutes
    Private m_displayPeriodMinutes As Integer = 480

    'file name for history
    Private m_temperatureHistFileName As String = "DetectorTemperaturHistory.txt"

    'maximum count of possible entries in temperature history
    Private m_maxTemperatureEntryCount As Integer = 43200

    'customer (e.g. DWD, BAG)
    Private ReadOnly m_customer As String

    'station name (e.g. Erlangen, Offenbach)
    Private ReadOnly m_stationName As String

    Private Sub New()

    End Sub

    Public Sub New(customer As String, stationName As String)
        Me.New()

        m_customer = customer
        m_stationName = stationName
    End Sub


    'period for displaying the graph and calculating locale minimums and maximums in minutes
    Public Property DisplayPeriodMinutes() As Long
        Get
            Return m_displayPeriodMinutes
        End Get
        Set(value As Long)
            m_displayPeriodMinutes = value
        End Set
    End Property

    ''' <summary>
    ''' temperature history, contains all recorded temperatures with time stamps as keys 
    ''' (time stamps are converted from DateTime objects with short year notation)
    ''' </summary>
    Public ReadOnly Property Temperatures() As SortedDictionary(Of Long, Double)
        Get
            Return m_temperatures
        End Get
    End Property

    'function used to evaluate Y-axis value out of given X-axis value for 
    'temperature history graph
    Public Function EvaluateTemperature(ByVal x As Long) As Double

        If m_temperatures.ContainsKey(x) Then
            Return m_temperatures.Item(x)
        ElseIf m_temperatures.ContainsKey(x - 1) Then
            Return m_temperatures.Item(x - 1)
        Else
            Return Double.MinValue
        End If
    End Function


    ''' <summary>
    ''' Initialization, loading and parsing history file
    ''' </summary>
    ''' <param name="temperatureHistoryDirectory" >Name of file for temperature history</param>
    ''' <remarks></remarks>
    Public Function Initialize(ByVal temperatureHistoryDirectory As String) As Boolean

        If Not m_wasLoaded Then
            If temperatureHistoryDirectory.EndsWith("\") Then
                m_temperatureHistFileName = temperatureHistoryDirectory + "DetectorTemperaturHistory.txt"
            Else
                m_temperatureHistFileName = temperatureHistoryDirectory + "\" + "DetectorTemperaturHistory.txt"
            End If

            ReadTemperaturesFromFile(m_temperatureHistFileName)
            m_wasLoaded = True
        End If

    End Function


    ''' <summary>
    ''' Loads and parses temperature history from given file
    ''' </summary>
    ''' <param name="fileName">History file name</param>
    ''' <remarks></remarks>
    Private Sub ReadTemperaturesFromFile(ByVal fileName As String)
        Dim currentLine As String
        Dim headerLinesCount As Integer = 5
        Dim index As Integer = 0

        If System.IO.File.Exists(fileName) Then
            Dim txtFileReader As New System.IO.StreamReader(fileName)
            m_temperatures.Clear()

            'ignore header lines
            While Not txtFileReader.EndOfStream
                If index < headerLinesCount Then
                    txtFileReader.ReadLine()
                    index += 1
                Else
                    Exit While
                End If
            End While

            While Not txtFileReader.EndOfStream
                currentLine = txtFileReader.ReadLine()
                Dim keyValue As String() = currentLine.Split(vbTab)
                If keyValue.Length = 2 Then
                    Dim timeStamp As DateTime
                    Dim currentTemperature As Double

                    If DateTime.TryParse(keyValue(0), timeStamp) Then
                        If Double.TryParse(keyValue(1), currentTemperature) Then

                            m_temperatures.Add(timeStamp.Ticks, currentTemperature)
                        End If
                    End If
                End If

            End While

            txtFileReader.Close()
        End If
    End Sub


    ''' <summary>
    ''' generates header for temperature history file
    ''' </summary>
    ''' <returns>Generated header string </returns>
    ''' <remarks></remarks>
    Private Function GenerateHeader() As String
        Dim header As String

        Dim nowTime As String = DateTime.Now.ToString("yy.MM.dd hh:mm:ss")
        Dim measureStart As String = ConvertLongToDateTime(m_firstKey).ToString("dd.MM.yy HH:mm:ss")
        Dim measureEnd As String = ConvertLongToDateTime(m_lastKey).ToString("dd.MM.yy HH:mm:ss")
        Dim minMeasuredTemp As String = Math.Round(GetMinTempFromPeriod(TimeSpan.FromMinutes(DisplayPeriodMinutes)), 2)
        Dim maxMeasuredTemp As String = Math.Round(GetMaxTempFromPeriod(TimeSpan.FromMinutes(DisplayPeriodMinutes)), 2)

        'watch out: the file reader expects exactly 5 rows header!
        'row 1
        header = nowTime + " CET/MET;" + m_customer + ";" + m_stationName + " " + vbNewLine
        ' row 2
        header += "Detektortemperaturaufzeichnung " + vbNewLine
        'row 3
        header += "Messzeitraum: " + measureStart + " / " + measureEnd + vbNewLine
        'row 4
        header += "Minimale Temperatur 8 Std: " + minMeasuredTemp + " °C, "
        header += "Maximale Temperatur 8 Std: " + maxMeasuredTemp + " °C" + vbNewLine
        'row 5
        header += "Messzeitpunkt" + vbTab + "Temperatur" + vbNewLine

        Return header
    End Function


    ''' <summary>
    ''' stores temperature history into given file
    ''' </summary>
    ''' <param name="fileName">History file name</param>
    ''' <remarks></remarks>
    Private Function WriteTemperaturesToFile(ByVal fileName As String) As Boolean

        If Not System.IO.File.Exists(fileName) Then
            Try
                System.IO.File.Create(fileName).Dispose()
            Catch dirNotFoundEx As System.IO.DirectoryNotFoundException
                ' Since the error while creating the directory already occured during
                ' initialization the sub will be exited here
                Exit Function
            End Try
        End If
        Try
            Dim objWriter As New System.IO.StreamWriter(fileName)
            Dim temperatureValue As String

            objWriter.Flush()
            objWriter.Write(GenerateHeader())

            For Each timeStamp As Long In m_temperatures.Keys
                temperatureValue = Math.Round(m_temperatures.Item(timeStamp), 2)
                objWriter.WriteLine(ConvertLongToDateTime(timeStamp).ToString("dd.MM.yy HH:mm:ss") & vbTab & temperatureValue)
            Next
            objWriter.Close()

        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function


    ''' <summary>
    ''' converts given DateTime object into its long representation (converted value has short year representation)
    ''' </summary>
    ''' <param name="dateTime">DateTime containing the time to be converted</param>
    ''' <returns>Long data type representation of datetime</returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertDateTimeToLong(ByVal dateTime As DateTime) As Long
        Return dateTime.Ticks
    End Function

    ' 
    ''' <summary>
    ''' converts a long value (with short year representation) into DateTime object
    ''' </summary>
    ''' <param name="longValue">Long data type representation of datetime</param>
    ''' <returns>DateTime containing the time to be converted</returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertLongToDateTime(ByVal longValue As Long) As DateTime
        Return New DateTime(longValue)
    End Function


    ''' <summary>
    ''' retrieves oldest time stamp from recorded history
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property OldestTimeStamp() As DateTime
        Get
            If m_firstKey Is Nothing Then
                Return New DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified)
            End If
            Return ConvertLongToDateTime(m_firstKey)
        End Get
    End Property


    ''' <summary>
    ''' retrieves most recent time stamp from recorded history
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property MostRecentTimeStamp() As DateTime
        Get
            If m_lastKey Is Nothing Then
                Return New DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified)
            End If
            Return ConvertLongToDateTime(m_lastKey)
        End Get
    End Property

    ''' <summary>
    ''' calculates local maximal temperature within given period
    ''' </summary>
    ''' <param name="period">max time period</param>
    ''' <returns>found temperature</returns>
    ''' <remarks></remarks>
    Public Function GetMaxTempFromPeriod(ByVal period As TimeSpan) As Double
        Dim maxTemp As Double
        Dim oldestTime As DateTime
        Dim currentTemp As Double
        Dim timeStamps As Long()

        ReDim timeStamps(m_temperatures.Count - 1)
        m_temperatures.Keys.CopyTo(timeStamps, 0)

        oldestTime = DateTime.Now - period

        If timeStamps.Length <> 0 AndAlso (Not IsNothing(m_lastKey)) Then
            maxTemp = m_temperatures.Item(m_lastKey)
        Else
            maxTemp = Double.MinValue
        End If


        For i As Integer = m_temperatures.Count - 1 To 0 Step -1
            If ConvertLongToDateTime(timeStamps(i)) < oldestTime Then
                Return maxTemp
            Else
                currentTemp = m_temperatures.Item(timeStamps(i))
                If maxTemp < currentTemp Then
                    maxTemp = currentTemp
                End If
            End If
        Next
        Return maxTemp
    End Function


    ''' <summary>
    ''' calculates local minimal temperature within given period
    ''' </summary>
    ''' <param name="period">duration</param>
    ''' <returns>max temperature</returns>
    ''' <remarks></remarks>
    Public Function GetMinTempFromPeriod(ByVal period As TimeSpan) As Double
        Dim minTemp As Double
        Dim timeStamps As Long()
        Dim oldestTime As DateTime
        Dim currentTemp As Double

        oldestTime = DateTime.Now - period

        ReDim timeStamps(m_temperatures.Count - 1)
        m_temperatures.Keys.CopyTo(timeStamps, 0)

        If timeStamps.Length <> 0 AndAlso (Not IsNothing(m_firstKey)) Then
            minTemp = m_temperatures.Item(m_firstKey)
        Else
            minTemp = Double.MinValue
        End If

        For i As Integer = m_temperatures.Count - 1 To 0 Step -1
            If ConvertLongToDateTime(timeStamps(i)) < oldestTime Then
                Return minTemp
            Else
                currentTemp = m_temperatures.Item(timeStamps(i))
                If minTemp > currentTemp Then
                    minTemp = currentTemp
                End If
            End If
        Next


        Return minTemp
    End Function


    ''' <summary>
    ''' retrieves max temperature from recorded history
    ''' </summary>
    ''' <returns>maximal temperature from history</returns>
    ''' <remarks></remarks>
    Public Function GetOverallMaxTemp()
        Dim maxTemp As Double
        UpdateFirstLastKeysInDict()
        If m_temperatures.Count <> 0 Then
            maxTemp = m_temperatures.Item(m_firstKey)
        Else
            maxTemp = Double.MinValue
        End If

        For Each currentTemp As Double In m_temperatures.Values
            If maxTemp < currentTemp Then
                maxTemp = currentTemp
            End If
        Next
        Return maxTemp
    End Function


    ''' <summary>
    ''' retrieves min temperature from recorded history
    ''' </summary>
    ''' <returns>minimal temperature from whole history</returns>
    ''' <remarks></remarks>
    Public Function GetOverallMinTemp()
        Dim minTemp As Double
        UpdateFirstLastKeysInDict()
        If m_temperatures.Count <> 0 Then
            minTemp = m_temperatures.Item(m_firstKey)
        Else
            minTemp = Double.MinValue
        End If

        For Each currentTemp As Double In m_temperatures.Values
            If minTemp > currentTemp Then
                minTemp = currentTemp
            End If
        Next
        Return minTemp
    End Function


    ''' <summary>
    ''' retrieves latest temperature from recorded history
    ''' </summary>
    ''' <returns>latest temperature from whole history</returns>
    ''' <remarks></remarks>
    Public Function GetLatestTemp()
        Dim latestTemp As Double
        UpdateFirstLastKeysInDict()
        If m_temperatures.Count <> 0 Then
            latestTemp = m_temperatures.Item(m_lastKey)
        Else
            latestTemp = Double.MinValue
        End If
        Return latestTemp
    End Function


    ''' <summary>
    ''' retrieves average temperature from recorded history
    ''' </summary>
    ''' <returns>average temperature from whole history</returns>
    ''' <remarks></remarks>
    Public Function GetAverageTemp()
        Dim averageTemp As Double
        UpdateFirstLastKeysInDict()
        If m_temperatures.Count <> 0 Then
            For Each temp As Double In m_temperatures.Values
                averageTemp += temp
            Next
            averageTemp /= m_temperatures.Count
        Else
            averageTemp = Double.MinValue
        End If
        Return averageTemp
    End Function

    ''' <summary>
    ''' sets 2 internal variables with most recent and oldest key in temperatures history
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateFirstLastKeysInDict()
        m_firstKey = Nothing
        m_lastKey = Nothing

        For Each value As Long In m_temperatures.Keys
            If m_firstKey Is Nothing Then
                m_firstKey = value
            End If
            m_lastKey = value
        Next
    End Sub


    ''' <summary>
    ''' rolling appender helper. Removes oldest entries from history to shrink the history to valid size
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RemoveOldestEntries()
        UpdateFirstLastKeysInDict()
        While m_temperatures.Count >= m_maxTemperatureEntryCount
            m_temperatures.Remove(m_firstKey)
            UpdateFirstLastKeysInDict()
        End While
    End Sub


    ''' <summary>
    ''' adds new temperature entry together with new time stamp into temperature history cache and writes history into history file
    ''' </summary>
    ''' <param name="temperature">actual temperature value to be stored</param>
    ''' <remarks></remarks>
    Public Function StoreNewTempertaureEntry(ByVal temperature As Double) As Boolean

        Dim currentTimeStampAsLong As Long
        Dim currentDate As DateTime
        currentDate = New DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified)
        UpdateFirstLastKeysInDict()

        If temperature <> Double.MinValue Then
            currentTimeStampAsLong = currentDate.Ticks
            RemoveOldestEntries()
            If Not m_temperatures.ContainsKey(currentTimeStampAsLong) Then
                m_temperatures.Add(currentTimeStampAsLong, temperature)
            End If

            UpdateFirstLastKeysInDict()

            Return WriteTemperaturesToFile(m_temperatureHistFileName)
        End If
        Return True
    End Function

    ''' <summary>
    ''' Calculates the time in seconds until the next full 5 minutes
    ''' </summary>
    ''' <returns>time in seconds</returns>
    ''' <remarks></remarks>
    Public Function GetSecondsUntilNextDetection() As Integer
        Return (60 - DateTime.Now.Second + (4 - DateTime.Now.Minute Mod 5) * 60)
    End Function

    ''' <summary>
    ''' Gets all temperature values which are younger than the given period start date
    ''' </summary>
    ''' <param name="periodStart"></param>
    ''' <remarks></remarks>
    Public Sub FillXYBeginningFrom(periodStart As DateTime, periodEnd As DateTime, onMatching As Action(Of DateTime, Integer))

        For Each tempEntry As KeyValuePair(Of Long, Double) In Temperatures
            Dim dt As DateTime = New DateTime(tempEntry.Key)

            Dim isWithinRangeLow As Boolean = (periodStart - dt).TotalSeconds <= 0
            Dim isWithinRangeHigh As Boolean = (periodEnd - dt).TotalSeconds >= 0

            If isWithinRangeLow And isWithinRangeHigh Then
                Dim temperatureValue As Integer = tempEntry.Value

                onMatching(dt, temperatureValue)

            End If
        Next


    End Sub


End Class


Public Class XyValue
    Public Property Timestamp As DateTime
    Public Property Value As Integer
End Class
