''' <summary>
''' DropDownListe für das PropertyGrid
''' </summary>
''' <remarks>names of "serial ports" located in computer</remarks>
Friend Class ComportList
    Inherits System.ComponentModel.StringConverter

    Dim _List As New List(Of String)

    Public Overloads Overrides Function GetStandardValues(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.ComponentModel.TypeConverter.StandardValuesCollection
        _List.Clear()
        For Each seport As String In My.Computer.Ports.SerialPortNames
            _List.Add(seport)
        Next
        Return New StandardValuesCollection(_List)
    End Function

    Public Overloads Overrides Function GetStandardValuesSupported(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

    Public Overloads Overrides Function GetStandardValuesExclusive(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

End Class