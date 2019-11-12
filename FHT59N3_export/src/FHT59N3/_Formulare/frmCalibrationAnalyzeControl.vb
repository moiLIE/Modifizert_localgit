Imports Thermo.Rmp.CM.Controls
Imports FHT59N3Core

Public Class frmCalibrationAnalyzeControl

    Private IndicesList As New Dictionary(Of String, List(Of Integer))
    Private MyNuclideList As Dictionary(Of Integer, FHT59N3Core.FHT59N3MCA_CertificateNuclides)
    Private MyNuclideActivities As List(Of FHT59N3Core.FHT59N3MCA_CertificateActivities)
    Private _ChangingData As Boolean = False

    Public ReturnValue As Integer = -1

    Private Sub frmCalibrationMeasControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            FillView()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnChange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnChange.Click
        Try

            Select Case _CalibAnalyzationType

                Case CalibAnalyzationTypes.EnergyFar
                    TBox_ActNuc1.Enabled = True
                    TBox_ActNuc2.Enabled = True
                    TBox_ActNuc3.Enabled = True

                Case CalibAnalyzationTypes.EnergyNearMix
                    TBox_ActNuc1.Enabled = True
                    TBox_ActNuc2.Enabled = True
                    TBox_ActNuc3.Enabled = True

                Case CalibAnalyzationTypes.EfficiencyFar

                Case CalibAnalyzationTypes.EfficiencyNear
                    TBox_ActNuc1.Enabled = True

                Case CalibAnalyzationTypes.EfficiencyRccMix, CalibAnalyzationTypes.EfficiencyRccCs137
                    TBox_ActNuc1.Enabled = True

            End Select

            ChangeView(True)

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnAnalyze_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnalyze.Click
        Try

            If _ChangingData Then


                Select Case _CalibAnalyzationType

                    Case CalibAnalyzationTypes.EnergyFar
                        MyNuclideActivities(0).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc1.Text), Double)
                        MyNuclideActivities(1).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc2.Text), Double)
                        MyNuclideActivities(2).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc3.Text), Double)
                        For i As Integer = 1 To MyNuclideList.Count
                            For j As Integer = 0 To MyNuclideActivities.Count - 1
                                If MyNuclideActivities(j).Name = MyNuclideList(i).Name Then
                                    MyNuclideList(i).ActivitykBq = MyNuclideActivities(j).ActivitykBq
                                End If
                            Next
                        Next
                        MCA_WriteDataToCanberraCtfFile(_CertificateDirectory & "\EBIN.ctf", CType(TBox_DateTime.Text, Date), MyNuclideList)

                    Case CalibAnalyzationTypes.EnergyNearMix
                        MyNuclideActivities(0).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc1.Text), Double)
                        MyNuclideActivities(1).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc2.Text), Double)
                        MyNuclideActivities(2).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc3.Text), Double)
                        For i As Integer = 1 To MyNuclideList.Count
                            For j As Integer = 0 To MyNuclideActivities.Count - 1
                                If MyNuclideActivities(j).Name = MyNuclideList(i).Name Then
                                    MyNuclideList(i).ActivitykBq = MyNuclideActivities(j).ActivitykBq
                                End If
                            Next
                        Next
                        MCA_WriteDataToCanberraCtfFile(_CertificateDirectory & "\EBIN_NAH_MISCH.ctf", CType(TBox_DateTime.Text, Date), MyNuclideList)

                    Case CalibAnalyzationTypes.EfficiencyFar

                    Case CalibAnalyzationTypes.EfficiencyNear, CalibAnalyzationTypes.EfficiencyRccCs137

                        MyNuclideActivities(0).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc1.Text), Double)
                        For i As Integer = 1 To MyNuclideList.Count
                            For j As Integer = 0 To MyNuclideActivities.Count - 1
                                If MyNuclideActivities(j).Name = MyNuclideList(i).Name Then
                                    MyNuclideList(i).ActivitykBq = MyNuclideActivities(j).ActivitykBq
                                End If
                            Next
                        Next
                        MCA_WriteDataToCanberraCtfFile(_CertificateDirectory & "\CS137.ctf", CType(TBox_DateTime.Text, Date), MyNuclideList)

                    Case CalibAnalyzationTypes.EfficiencyRccMix

                        'Index von CS-137 in der Nah-Misch-Zertifikat-Datei ist 2...
                        MyNuclideActivities(2).ActivitykBq = CType(SYS_SetUnitedStatesDecimalSeparator(TBox_ActNuc1.Text), Double)
                        For i As Integer = 1 To MyNuclideList.Count
                            For j As Integer = 0 To MyNuclideActivities.Count - 1
                                If MyNuclideActivities(j).Name = MyNuclideList(i).Name Then
                                    MyNuclideList(i).ActivitykBq = MyNuclideActivities(j).ActivitykBq
                                End If
                            Next
                        Next
                        MCA_WriteDataToCanberraCtfFile(_CertificateDirectory & "\EBIN_NAH_MISCH.ctf", CType(TBox_DateTime.Text, Date), MyNuclideList)


                End Select

                ChangeView(False)

            Else

                Me.Close()

            End If

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Try
            If _ChangingData Then

                ChangeView(False)
                FillView()

            Else

                ReturnValue = -1
                Me.Close()

            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub FillView()
        Try
            Lbl_Prob.Visible = False
            TBox_Prob.Visible = False
            BtnChange.Visible = False

            TBox_ActNuc1.Enabled = False
            TBox_ActNuc2.Enabled = False
            TBox_ActNuc3.Enabled = False

            Select Case _CalibAnalyzationType

                Case CalibAnalyzationTypes.EnergyFar
                    Me.Text = "FHT59N3T - EBIN.ctf"
                    Lbl_CommandText.Text = ml_string(292, "Please CHECK the certificate data of EBIN.CTF.") '"Have you measured with MIXED source in geometry FAR?" & vbCrLf & 
                    MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\EBIN.ctf", IndicesList)
                    If Not IndicesList.Count <= 0 Then
                        MyNuclideActivities = MCA_CheckCanberraCtfFile(MyNuclideList, IndicesList)
                        If Not MyNuclideActivities Is Nothing Then
                            TBox_DateTime.Text = Format(MyNuclideActivities(0).CertificateDate, "dd.MM.yy HH:mm")
                            Lbl_ActNuc1.Text = MyNuclideActivities(0).Name & " [kBq]"
                            TBox_ActNuc1.Text = MyNuclideActivities(0).ActivitykBq.ToString
                            Lbl_ActNuc2.Text = MyNuclideActivities(1).Name & " [kBq]"
                            TBox_ActNuc2.Text = MyNuclideActivities(1).ActivitykBq.ToString
                            Lbl_ActNuc3.Text = MyNuclideActivities(2).Name & " [kBq]"
                            TBox_ActNuc3.Text = MyNuclideActivities(2).ActivitykBq.ToString
                            If Not MyNuclideActivities(0).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(0).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            If Not MyNuclideActivities(1).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(1).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(1).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(1).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            If Not MyNuclideActivities(2).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(2).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(2).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(2).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            BtnChange.Visible = True
                            ReturnValue = 1
                        End If
                    End If
                    Lbl_ActNuc1.Visible = True
                    TBox_ActNuc1.Visible = True
                    Lbl_ActNuc2.Visible = True
                    TBox_ActNuc2.Visible = True
                    Lbl_ActNuc3.Visible = True
                    TBox_ActNuc3.Visible = True

                Case CalibAnalyzationTypes.EnergyNearMix
                    Me.Text = "FHT59N3T - EBIN_NAH_MISCH.ctf"
                    Lbl_CommandText.Text = ml_string(578, "Please CHECK the certificate data of EBIN_NAH_MISCH.CTF.") '"Have you measured with MIXED source in geometry FAR?" & vbCrLf & 
                    MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\EBIN_NAH_MISCH.ctf", IndicesList)
                    If Not IndicesList.Count <= 0 Then
                        MyNuclideActivities = MCA_CheckCanberraCtfFile(MyNuclideList, IndicesList)
                        If Not MyNuclideActivities Is Nothing Then
                            TBox_DateTime.Text = Format(MyNuclideActivities(0).CertificateDate, "dd.MM.yy HH:mm")
                            Lbl_ActNuc1.Text = MyNuclideActivities(0).Name & " [kBq]"
                            TBox_ActNuc1.Text = MyNuclideActivities(0).ActivitykBq.ToString
                            Lbl_ActNuc2.Text = MyNuclideActivities(1).Name & " [kBq]"
                            TBox_ActNuc2.Text = MyNuclideActivities(1).ActivitykBq.ToString
                            Lbl_ActNuc3.Text = MyNuclideActivities(2).Name & " [kBq]"
                            TBox_ActNuc3.Text = MyNuclideActivities(2).ActivitykBq.ToString
                            If Not MyNuclideActivities(0).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(0).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            If Not MyNuclideActivities(1).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(1).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(1).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(1).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            If Not MyNuclideActivities(2).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(2).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(2).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(2).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            BtnChange.Visible = True
                            ReturnValue = 1
                        End If
                    End If
                    BtnChange.Visible = True
                    Lbl_ActNuc1.Visible = True
                    TBox_ActNuc1.Visible = True
                    Lbl_ActNuc2.Visible = True
                    TBox_ActNuc2.Visible = True
                    Lbl_ActNuc3.Visible = True
                    TBox_ActNuc3.Visible = True

                Case CalibAnalyzationTypes.EfficiencyFar

                Case CalibAnalyzationTypes.EfficiencyNear
                    Me.Text = "FHT59N3T - CS137.ctf"
                    Lbl_CommandText.Text = ml_string(293, "Please CHECK the certificate data of CS137.CTF.") '"Have you measured with CS-137 source in geometry NEAR?" & vbCrLf & 
                    MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\CS137.ctf", IndicesList)
                    If Not IndicesList.Count <= 0 Then
                        MyNuclideActivities = MCA_CheckCanberraCtfFile(MyNuclideList, IndicesList)
                        If Not MyNuclideActivities Is Nothing Then
                            TBox_DateTime.Text = Format(MyNuclideActivities(0).CertificateDate, "dd.MM.yy HH:mm")
                            Lbl_ActNuc1.Text = MyNuclideActivities(0).Name & " [kBq]"
                            TBox_ActNuc1.Text = MyNuclideActivities(0).ActivitykBq.ToString
                            If Not MyNuclideActivities(0).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(0).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            BtnChange.Visible = True
                            ReturnValue = 1
                        End If
                    End If
                    Lbl_ActNuc2.Visible = False
                    TBox_ActNuc2.Visible = False
                    Lbl_ActNuc3.Visible = False
                    TBox_ActNuc3.Visible = False

                Case CalibAnalyzationTypes.EfficiencyRccCs137
                    Me.Text = "FHT59N3T - CS137.ctf"
                    Lbl_CommandText.Text = ml_string(293, "Please CHECK the certificate data of CS137.CTF.") '"Have you measured with CS-137 source in geometry NEAR?" & vbCrLf & 
                    MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\CS137.ctf", IndicesList)
                    If Not IndicesList.Count <= 0 Then
                        MyNuclideActivities = MCA_CheckCanberraCtfFile(MyNuclideList, IndicesList)
                        If Not MyNuclideActivities Is Nothing Then
                            TBox_DateTime.Text = Format(MyNuclideActivities(0).CertificateDate, "dd.MM.yy HH:mm")
                            Lbl_ActNuc1.Text = MyNuclideActivities(0).Name & " [kBq]"
                            TBox_ActNuc1.Text = MyNuclideActivities(0).ActivitykBq.ToString
                            If Not MyNuclideActivities(0).OK Then
                                TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).Name & ": "
                                For i As Integer = 0 To MyNuclideActivities(0).ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & MyNuclideActivities(0).ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            BtnChange.Visible = True
                            ReturnValue = 1
                        End If
                    End If
                    Lbl_ActNuc2.Visible = False
                    TBox_ActNuc2.Visible = False
                    Lbl_ActNuc3.Visible = False
                    TBox_ActNuc3.Visible = False

                Case CalibAnalyzationTypes.EfficiencyRccMix
                    Me.Text = "FHT59N3T - EBIN_NAH_MISCH.ctf"
                    Lbl_CommandText.Text = ml_string(578, "Please CHECK the certificate data of EBIN_NAH_MISCH.CTF.") '"Have you measured with MIXED source in geometry FAR?" & vbCrLf & 
                    MyNuclideList = MCA_ReadCanberraCtfFile(_CertificateDirectory & "\EBIN_NAH_MISCH.ctf", IndicesList)
                    If Not IndicesList.Count <= 0 Then
                        MyNuclideActivities = MCA_CheckCanberraCtfFile(MyNuclideList, IndicesList)
                        If Not MyNuclideActivities Is Nothing Then

                            Dim CS137_Index_in_NearMix_Certificate As Integer = 2
                            Dim cs137Nuclide As FHT59N3MCA_CertificateActivities = MyNuclideActivities(CS137_Index_in_NearMix_Certificate)

                            TBox_DateTime.Text = Format(cs137Nuclide.CertificateDate, "dd.MM.yy HH:mm")
                            Lbl_ActNuc1.Text = cs137Nuclide.Name & " [kBq]"
                            TBox_ActNuc1.Text = cs137Nuclide.ActivitykBq.ToString
                            If Not cs137Nuclide.OK Then
                                TBox_Prob.Text = TBox_Prob.Text & cs137Nuclide.Name & ": "
                                For i As Integer = 0 To cs137Nuclide.ProbEnergies.Count - 1
                                    TBox_Prob.Text = TBox_Prob.Text & cs137Nuclide.ProbEnergies(i).ToString & "; "
                                Next
                                TBox_Prob.Text = TBox_Prob.Text & vbCrLf
                                Lbl_Prob.Visible = True
                                TBox_Prob.Visible = True
                            End If
                            BtnChange.Visible = True
                            ReturnValue = 1
                        End If
                    End If
                    Lbl_ActNuc2.Visible = False
                    TBox_ActNuc2.Visible = False
                    Lbl_ActNuc3.Visible = False
                    TBox_ActNuc3.Visible = False


            End Select

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub ChangeView(ByVal ChangingData As Boolean)
        Try
            If ChangingData Then
                BtnChange.Visible = False
                _ChangingData = True
                BtnAnalyze.Text = ml_string(453, "Save")
            Else
                BtnChange.Visible = True
                _ChangingData = False
                BtnAnalyze.Text = ml_string(21, "Resume")
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TBox_ActNuc1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_ActNuc1.Click
    End Sub

    Private Sub TBox_ActNuc2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_ActNuc2.Click
    End Sub

    Private Sub TBox_ActNuc3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_ActNuc3.Click
    End Sub

    Private Sub TBox_DateTime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBox_DateTime.Click
    End Sub

    Public Sub New()
        InitializeComponent()
        ml_UpdateControls()
        AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Private Sub frmCalibrationAnalyzeControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
    End Sub

    Protected Overridable Sub ml_UpdateControls()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalibrationAnalyzeControl))
        resources.ApplyResources(Me.BtnAnalyze, "BtnAnalyze")
        resources.ApplyResources(Me.BtnChange, "BtnChange")
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        resources.ApplyResources(Me.Label1, "Label1")
        resources.ApplyResources(Me.Lbl_Prob, "Lbl_Prob")
        resources.ApplyResources(Me, "$this")
    End Sub

End Class