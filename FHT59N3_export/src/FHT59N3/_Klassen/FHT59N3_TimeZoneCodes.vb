Public Class FHT59N3_TimeZoneCodes

    Private _TimeZoneCodes As New Dictionary(Of Integer, String)

    Private Sub InitializeTimeZoneCodes()
        Try
            _TimeZoneCodes.Clear()
            _TimeZoneCodes.Add(-12, "IDLW")
            _TimeZoneCodes.Add(-11, "NT")
            _TimeZoneCodes.Add(-10, "AHST/CAT/HST")
            _TimeZoneCodes.Add(-9, "YST")
            _TimeZoneCodes.Add(-8, "PST")
            _TimeZoneCodes.Add(-7, "MST")
            _TimeZoneCodes.Add(-6, "CST")
            _TimeZoneCodes.Add(-5, "EST")
            _TimeZoneCodes.Add(-4, "AST")
            _TimeZoneCodes.Add(-2, "AT")
            _TimeZoneCodes.Add(-1, "WAT")
            _TimeZoneCodes.Add(0, "UTC/GMT")
            _TimeZoneCodes.Add(1, "CET/MEZ")
            _TimeZoneCodes.Add(2, "CEST/MESZ/EET")
            _TimeZoneCodes.Add(3, "MSK/BGT")
            _TimeZoneCodes.Add(4, "IOT")
            _TimeZoneCodes.Add(5, "EIT")
            _TimeZoneCodes.Add(6, "ICT")
            _TimeZoneCodes.Add(7, "WAST")
            _TimeZoneCodes.Add(8, "CCT")
            _TimeZoneCodes.Add(9, "JST")
            _TimeZoneCodes.Add(10, "EAST/GST")
            _TimeZoneCodes.Add(12, "NZST/IDLE")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub New()
        Try
            InitializeTimeZoneCodes()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Function GetTimeZoneCode(ByVal UTCDiff As Integer) As String
        Try
            If _TimeZoneCodes.ContainsKey(UTCDiff) Then
                Return _TimeZoneCodes(UTCDiff)
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try

        Return ml_string(289, "UTC (No valid time zone code available)")
    End Function

End Class
