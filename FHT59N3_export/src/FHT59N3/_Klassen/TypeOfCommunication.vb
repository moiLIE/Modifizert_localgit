''' <summary>
''' DropDownListe für das PropertyGrid
''' </summary>
''' <remarks>names of "type of communication" known by application</remarks>
Friend Class TypeOfCommunication
    Inherits System.ComponentModel.StringConverter

    Private _List As New List(Of String)

    Public Overloads Overrides Function GetStandardValues(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.ComponentModel.TypeConverter.StandardValuesCollection
        _List.Clear()
        _List.Add(ml_string(550, "serial (RS-232)"))
        _List.Add(ml_string(551, "network (TCP/IP)"))
        Return New StandardValuesCollection(_List)
    End Function

    Public Overloads Overrides Function GetStandardValuesSupported(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

    Public Overloads Overrides Function GetStandardValuesExclusive(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

End Class