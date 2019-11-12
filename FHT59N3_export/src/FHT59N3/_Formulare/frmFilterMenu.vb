Public Class frmFilterMenu

    Private Sub BtnSetFilterstep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSetFilterstep.Click
        Dim resultStr As String
        Try
            _FiltertstepTries = 0

            If (CheckBoxWithTimeStampPrint.Checked) Then
                'kleiner Vorschub, um Datum auf Filterband zu drucken, intern wird dann der restliche Vorschub veranlasst
                resultStr = SPS_SetFilterstepWithPrinter1()
            Else
                'regulärer Schritt mit normal konfigurierter Länge
                resultStr = SPS_SetFilterstep()
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnSetFilterChanged_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSetFilterChanged.Click
        Try
            SYS_SetFilterChanged()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnSetAvFiltersteps_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSetAvFiltersteps.Click
        Try
            frmFiltersteps.ShowDialog()
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Private Sub BtnReturn_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturn.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub


    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmFilterMenu_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFilterMenu))
        resources.ApplyResources(Me.BtnReturn, "BtnReturn")
        resources.ApplyResources(Me.BtnSetAvFiltersteps, "BtnSetAvFiltersteps")
        resources.ApplyResources(Me.BtnSetFilterChanged, "BtnSetFilterChanged")
        resources.ApplyResources(Me.BtnSetFilterstep, "BtnSetFilterstep")
        resources.ApplyResources(Me, "$this")
    End Sub

    Private Sub frmFilterMenu_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        If (Not _MyFHT59N3Par.EnablePaperrollPrinter) Then
            CheckBoxWithTimeStampPrint.Checked = False
            CheckBoxWithTimeStampPrint.Enabled = False
        Else
            CheckBoxWithTimeStampPrint.Checked = False
            CheckBoxWithTimeStampPrint.Enabled = True
        End If
    End Sub
End Class