
''' <summary>
''' Hüllklasse die erlaubt Wertänderungen zu erkennen. Dabei wird auch der Änderungszeitstempel gesetzt für eine
''' spätere Auswertung. Momentan wird Klasse nur für SNMP-Verbindungsüberwachung genutzt.
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class ChangableObject(Of T)

    Private _value As T

    Public Property OldValue As T

    Public Property Value As T
        Get
            Return _value
        End Get
        Set(newValue As T)
            OldValue = _value
            _value = newValue

            If (Not IsNothing(newValue) AndAlso Not newValue.Equals(OldValue)) Then
                HasChanged = True
                ChangeTimeStamp = DateTime.Now
            End If

        End Set
    End Property

    Public Property ChangeTimestamp As DateTime

    ''' <summary>
    ''' Gets or sets a value indicating whether this instance has changed.
    ''' </summary>
    ''' <value>
    ''' <c>true</c> if this instance has changed; otherwise, <c>false</c>.
    ''' </value>
    Public Property HasChanged As Boolean

    ''' <summary>
    ''' Changes the timestamp older than.
    ''' </summary>
    ''' <param name="seconds">The seconds.</param>
    ''' <returns></returns>
    Public Function ChangeTimestampOlderThan(seconds As Integer) As Boolean
        Dim deltaSeconds As Double = (DateTime.Now - ChangeTimestamp).TotalSeconds

        Dim olderThan As Boolean = deltaSeconds > seconds
        Return olderThan
    End Function

End Class
