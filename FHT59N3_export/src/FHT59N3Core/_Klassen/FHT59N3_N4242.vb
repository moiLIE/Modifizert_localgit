Imports System.IO
Imports System.Threading

Public Class FHT59N3_N4242_FileCleanup

    Private Const THREADCYCLE__IN_MS As Int32 = 15 * 1000

    Private Const MAXIMUM_AGE_FILES_IN_DAYS As Int32 = 30

    Private fileCleanupThread As Thread
    Private fileCleanupLockObject As Object = New Object()  'locking object
    Private fileCleanupThreadRunning As Boolean

    Private fileDirectoryToClean As String

    Public Sub New(ByVal fileDirectory As String)
        fileDirectoryToClean = fileDirectory

    End Sub

    ''' <summary>
    ''' Start the thread of ANSI N42.42 agent
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartThread()
        If IsNothing(fileCleanupThread) Then
            fileCleanupThread = New Thread(AddressOf ThreadTask)

            fileCleanupThread.IsBackground = True
            fileCleanupThread.Name = "ANSI N42.42 file cleanup"
            fileCleanupThreadRunning = True
            fileCleanupThread.Start()
        End If
    End Sub

    ''' <summary>
    ''' The thread cleans up
    ''' </summary>
    ''' <remarks>Every 15 seconds old files will be deleted</remarks>
    Private Sub ThreadTask()
        Do
            SyncLock fileCleanupLockObject
                Monitor.Wait(fileCleanupLockObject, THREADCYCLE__IN_MS)
            End SyncLock

            If (Not fileDirectoryToClean Is Nothing) Then
                Helpers.DeleteOldFiles(fileDirectoryToClean, MAXIMUM_AGE_FILES_IN_DAYS)
            End If

            If Not fileCleanupThreadRunning Then Exit Do
        Loop
    End Sub

    ''' <summary>
    ''' Stops the thread which communicate with the snmp agent
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopThread()

        If Not IsNothing(fileCleanupThread) Then
            fileCleanupThreadRunning = False
            SyncLock fileCleanupLockObject
                'Monitor.Wait aufwecken...
                Monitor.Pulse(fileCleanupLockObject)
            End SyncLock

            'sicherheitshalber noch etwas warten
            Thread.Sleep(500)

            If fileCleanupThread.IsAlive Then
                fileCleanupThread.Abort()
            End If
        End If
    End Sub


    Class Helpers

        Private Const RetriesOnError As Integer = 3
        Private Const DelayOnRetry As Integer = 1000

        Public Shared Sub DeleteOldFiles(folderPath As String, maximumAgeInDays As UInteger, ParamArray filesToExclude As String())

            Try
                Dim minimumDate As DateTime = DateTime.Now.AddDays(-maximumAgeInDays)
                For Each Path As String In Directory.EnumerateFiles(folderPath)
                    If IsExcluded(Path, filesToExclude) Then
                        Continue For
                    End If

                    DeleteFileIfOlderThan(Path, minimumDate)
                Next
            Catch ex As Exception
                'TODO: add to logger
            End Try

           
        End Sub

        Private Shared Function DeleteFileIfOlderThan(path As String, [date] As DateTime) As Boolean
            For i As Integer = 0 To RetriesOnError - 1
                Try
                    Dim file As New FileInfo(path)
                    If file.CreationTime < [date] Then
                        file.Delete()
                    End If

                    Return True
                Catch generatedExceptionName As IOException
                    System.Threading.Thread.Sleep(DelayOnRetry)
                Catch generatedExceptionName As UnauthorizedAccessException
                    System.Threading.Thread.Sleep(DelayOnRetry)
                End Try
            Next

            Return False
        End Function

        Private Shared Function IsExcluded(item As String, exclusions As String()) As Boolean
            For Each exclusion As String In exclusions
                If item.Equals(exclusion, StringComparison.CurrentCultureIgnoreCase) Then
                    Return True
                End If
            Next

            Return False
        End Function

    End Class

End Class
