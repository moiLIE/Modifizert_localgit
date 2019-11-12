Public NotInheritable Class MlRuntime
    
  Private Sub New
    'Prevent instantiation with a private constructor.
  End Sub
    
  Public Shared Event LanguageChanged()
  
  Public Shared Sub BroadcastLanguageChanged()
    RaiseEvent LanguageChanged()
  End Sub  

End Class
