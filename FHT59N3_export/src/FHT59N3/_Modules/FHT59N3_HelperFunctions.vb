Imports System.Globalization
Imports System.Diagnostics

Module FHT59N3_HelperFunctions
    ' -------------------------------------------------------------
    ' $Id: FHT59N3_HelperFunctions.vb 405 2017-06-08 15:04:16Z marcel.bender $
    ' Title: helper functions
    '
    ' Description:
    '  - data conversion
    '  - simple wrppers
    ' -------------------------------------------------------------

    Public Function SYS_MyShell(ByVal ShellCommand As String, ByVal ShellArguments As String,
                                ByVal WindowStyle As Diagnostics.ProcessWindowStyle, ByVal WaitForExit As Boolean) As Integer
        Try
            Dim procEC As Integer = -1
            Dim procID As Integer
            Dim newProc As Diagnostics.Process
            Dim newProcStartInfo As New Diagnostics.ProcessStartInfo
            newProcStartInfo.FileName = ShellCommand
            newProcStartInfo.Arguments = ShellArguments
            newProcStartInfo.WindowStyle = WindowStyle
            If WindowStyle = ProcessWindowStyle.Hidden Then
                newProcStartInfo.CreateNoWindow = True
            End If
            newProcStartInfo.UseShellExecute = False
            newProc = Diagnostics.Process.Start(newProcStartInfo)
            procID = newProc.Id
            If WaitForExit Then
                newProc.WaitForExit()
                If newProc.HasExited Then
                    procEC = newProc.ExitCode
                End If
            End If
            Return procEC
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return -1
        End Try
    End Function

    Public Function SYS_GetDecimalSeparator() As String
        Try
            Dim Result As String = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
            Return Result
        Catch ex As Exception
            Return "."
        End Try
        Return "."
    End Function

    Public Function SYS_SetUnitedStatesDecimalSeparator(ByVal DecimalAsText As String) As String
        Try
            Dim Separator As String = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
            Dim NotSeparator As String
            If Separator = "." Then NotSeparator = "," Else NotSeparator = "."
            Return DecimalAsText.Replace(NotSeparator, Separator)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return "0"
        End Try
    End Function

    Public Function SYS_SetLocaleDecimalSeparator(ByVal DecimalAsText As String) As String
        Try
            Dim Separator As String = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator
            Dim NotSeparator As String
            If Separator = "." Then NotSeparator = "," Else NotSeparator = "."
            Return DecimalAsText.Replace(NotSeparator, Separator)
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return "0"
        End Try
    End Function

    Public Function SYS_ConvertNumber(ByVal Number As Object) As String
        Try
            Dim St As String
            St = Format(Number, "0.###0E+00")
            If Len(St) <= 10 Then
                St = " " + St
            End If
            Return St.Replace(",", ".")
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace) 'MLHIDE
            Return ""
        End Try
    End Function

    Public Function SYS_ConvertBoolToInt(ByVal Value As Boolean) As Integer
        Try
            If Value = True Then
                Return 1
            Else
                Return 0
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

End Module
