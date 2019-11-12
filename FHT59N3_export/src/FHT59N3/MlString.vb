Imports System.Resources
Imports System.Reflection
Imports System.Globalization

Module MLString

  Private RootNamespace as String = "FHT59N3" 'MLHIDE
  Private ResMgr        as ResourceManager

  Sub New()
    If RootNamespace.Length > 0 Then
      ResMgr = New ResourceManager ( RootNamespace & ".MultiLang", [Assembly].GetExecutingAssembly ) 'MLHIDE
    Else
      ResMgr = New ResourceManager ( "MultiLang", [Assembly].GetExecutingAssembly ) 'MLHIDE
    End If
  End Sub

  Public Sub ml_UseCulture ( Byval ci as CultureInfo )
    System.Threading.Thread.CurrentThread.CurrentUICulture = ci 
  End Sub  

  Public Function ml_string(ByVal StringID As Integer, Optional ByVal Text As String = "") As String'MLHIDE

        Dim resTxt As String = ml_resource(StringID)
        If IsNothing(resTxt) Then
            resTxt = "@" + StringID.ToString() + " " + Text
        End If
        Return resTxt
  End Function

  Public Function ml_resource(ByVal StringID As Integer) As String'MLHIDE

        Dim resTxt As String = ResMgr.GetString("_" & StringID.ToString())
        Return resTxt

  End Function

    Public SupportedCultures() As String = {"en", "de"} 'MLHIDE
End Module

Friend Class ml_CategoryAttribute
  Inherits System.ComponentModel.CategoryAttribute
  Sub New ( ByVal StringID As Integer, ByVal text As String )
    MyBase.New ( ml_string ( StringID, text ) )
  End Sub
End Class

Friend Class ml_DisplayNameAttribute
  Inherits System.ComponentModel.DisplayNameAttribute
  Sub New ( ByVal StringID As Integer, ByVal text As String )
    MyBase.New ( ml_string ( StringID, text ) )
  End Sub
End Class

Friend Class ml_DescriptionAttribute
  Inherits System.ComponentModel.DescriptionAttribute
  Sub New ( ByVal StringID As Integer, ByVal text As String )
    MyBase.New ( ml_string ( StringID, text ) )
  End Sub
End Class
