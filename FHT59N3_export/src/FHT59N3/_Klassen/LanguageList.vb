''' <summary>
''' DropDownListe für das PropertyGrid
''' </summary>
''' <remarks>names of "languages" known by application</remarks>
Friend Class LanguageList
    Inherits System.ComponentModel.StringConverter

    Dim _List As New List(Of String)

    Public Overloads Overrides Function GetStandardValues(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.ComponentModel.TypeConverter.StandardValuesCollection
        _List.Clear()
        _List.Add(ml_string(384, "German (de)"))
        _List.Add(ml_string(385, "English (en)"))
        Return New StandardValuesCollection(_List)
    End Function

    Public Overloads Overrides Function GetStandardValuesSupported(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

    Public Overloads Overrides Function GetStandardValuesExclusive(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

End Class