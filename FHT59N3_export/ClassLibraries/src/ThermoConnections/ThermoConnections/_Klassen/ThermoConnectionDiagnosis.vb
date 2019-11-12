''' <summary>
''' Klasse für Verbindungsdiagnose im Datenkollektor
''' </summary>
''' <remarks></remarks>
Public Class ThermoConnectionDiagnosis

#Region "Private Eigenschaften"

	'Diagnose
    Private _CounterTotalQueries As Long
    Private _CounterTotalQueriesOld As Long
    Private _CounterGoodQueries As Long
	Private _CounterRepeats As Long
    Private _CounterTimeouts As Long
    Private _CounterParityError As Long
    Private _CounterChecksumError As Long
    Private _CounterNAKError As Long
    Private _AliveCheckTime As Date
    Private _AliveCheckIntervall As Integer

	'Statistik
	Private _GoodQuerieStat As Double
	Private _RepeatStat As Double
    Private _TimeoutStat As Double
    Private _ParityErrorStat As Double
    Private _ChecksumErrorStat As Double
    Private _NAKErrorStat As Double

	'Zur Rückgabe
	Private _DiagString As String

#End Region

#Region "Öffentliche Eigenschaften"

	''' <summary>
	''' Totale übertragungen
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CounterTotalQueries() As Long
		Get
			Return _CounterTotalQueries
		End Get
		Set(ByVal value As Long)
			If _CounterTotalQueries < 9223372036854775000 Then
				_CounterTotalQueries = value
			Else
				_CounterTotalQueries = 0
			End If
		End Set
	End Property

	''' <summary>
	''' Gute Übertragungen
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CounterGoodQueries() As Long
		Get
			Return _CounterGoodQueries
		End Get
		Set(ByVal value As Long)
			If _CounterGoodQueries < 9223372036854775000 Then
				_CounterGoodQueries = value
			Else
				_CounterGoodQueries = 0
			End If
		End Set
	End Property

	''' <summary>
	''' Wiederholungen
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CounterRepeats() As Long
		Get
			Return _CounterRepeats
		End Get
		Set(ByVal value As Long)
			If _CounterRepeats < 9223372036854775000 Then
				_CounterRepeats = value
			Else
				_CounterRepeats = 0
			End If
		End Set
	End Property

	''' <summary>
	''' Timeouts
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
    Public Property CounterTimeouts() As Long
        Get
            Return _CounterTimeouts
        End Get
        Set(ByVal value As Long)
            If _CounterTimeouts < 9223372036854775000 Then
                _CounterTimeouts = value
            Else
                _CounterTimeouts = 0
            End If
        End Set
    End Property

    Public Property CounterParityError() As Long
        Get
            Return _CounterParityError
        End Get
        Set(ByVal value As Long)
            If _CounterParityError < 9223372036854775000 Then
                _CounterParityError = value
            Else
                _CounterParityError = 0
            End If
        End Set
    End Property

    Public Property CounterChecksumError() As Long
        Get
            Return _CounterChecksumError
        End Get
        Set(ByVal value As Long)
            If _CounterChecksumError < 9223372036854775000 Then
                _CounterChecksumError = value
            Else
                _CounterChecksumError = 0
            End If
        End Set
    End Property

    Public Property CounterNAKError() As Long
        Get
            Return _CounterNAKError
        End Get
        Set(ByVal value As Long)
            If _CounterNAKError < 9223372036854775000 Then
                _CounterNAKError = value
            Else
                _CounterNAKError = 0
            End If
        End Set
    End Property

	Public ReadOnly Property GoodQuerieStat() As Double
		Get
			CalculateStatistics()
			Return _GoodQuerieStat
		End Get
	End Property

	Public ReadOnly Property RepeatStat() As Double
		Get
			CalculateStatistics()
			Return _RepeatStat
		End Get
	End Property

	Public ReadOnly Property TimeoutStat() As Double
		Get
			CalculateStatistics()
			Return _TimeoutStat
		End Get
	End Property

	''' <summary>
	''' Alle Daten in einem String
	''' </summary>
	''' <returns>CounterTotal;CounterGood;CounterTimeouts;CounterRepeats;GoodQuerieStat;TimeOutStat;RepeatStat</returns>
	''' <remarks></remarks>
	Public ReadOnly Property DiagString() As String
		Get
			BuildDiagstring()
			Return _DiagString
		End Get
	End Property

    ''' <summary>
    ''' der Datencollector noch
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Alive() As Boolean
        Get
            If (Now.Subtract(_AliveCheckTime).TotalMilliseconds >= _AliveCheckIntervall) Then 'nur alle 10 sekunden die Verbindung prüfen
                _AliveCheckTime = Now
                Dim Ret As Boolean
                If (_CounterTotalQueries = _CounterTotalQueriesOld) Then
                    Ret = False 'ich bin Busy, sollte also den Counter erhöht haben
                Else
                    _CounterTotalQueriesOld = _CounterTotalQueries
                    Ret = True
                End If
                Return Ret
            Else
                Return True
            End If
        End Get
    End Property

#End Region

#Region "Private Methoden"

	''' <summary>
	''' eine kleine Statistik
	''' </summary>
	''' <remarks></remarks>
    Private Sub CalculateStatistics()
        Try
            _GoodQuerieStat = (_CounterGoodQueries / _CounterTotalQueries) * 100
            _TimeoutStat = (_CounterTimeouts / _CounterTotalQueries) * 100
            _RepeatStat = (_CounterRepeats / _CounterTotalQueries) * 100
            _ParityErrorStat = (_CounterParityError / _CounterTotalQueries) * 100
            _ChecksumErrorStat = (_CounterChecksumError / _CounterTotalQueries) * 100
            _NAKErrorStat = (_CounterNAKError / _CounterTotalQueries) * 100
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

	''' <summary>
	''' Diagnosestring ausgeben
	''' </summary>
	''' <remarks></remarks>
    Private Sub BuildDiagstring()
        Try
            CalculateStatistics()
            _DiagString = _CounterTotalQueries.ToString & ";" & _CounterGoodQueries.ToString & ";" & _CounterTimeouts.ToString & ";" & _CounterRepeats.ToString & ";" & _
            Format(_GoodQuerieStat, "0.00") & "%;" & Format(_TimeoutStat, "0.00") & "%;" & Format(_RepeatStat, "0.00") & "%" & _
            Format(_ParityErrorStat, "0.00") & "%" & Format(_ChecksumErrorStat, "0.00") & "%" & Format(_NAKErrorStat, "0.00") & "%"
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="AliveCheckIntervall"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal AliveCheckIntervall As Integer)
        Try
            _AliveCheckTime = Now
            _AliveCheckIntervall = AliveCheckIntervall
        Catch ex As Exception
            Trace.TraceError(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

End Class
