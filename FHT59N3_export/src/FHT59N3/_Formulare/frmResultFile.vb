Imports Thermo.Rmp.CM.Controls

Public Class frmResultFile

    Private WithEvents Doc As New System.Drawing.Printing.PrintDocument
    Dim MyprintFont As New Font(_MyprintFontName, _MyprintFontSize, _MyprintFontStyle)
    Dim MyPrintersettings As System.Drawing.Printing.PrinterSettings
    Dim MyPagesettings As System.Drawing.Printing.PageSettings
    ' diese Variablen werden in den 
    ' DocumentPrint1-Ereignisprozeduren verwendet
    Dim initPrint As Boolean = False 'gibt an, ob die Initialisierung für den Druckprozess bereits durchgeführt worden ist
    Const headLines As Integer = 2   'Platz für Kopfzeilen
    Const footLines As Integer = 2   'Platz für Kopfzeilen
    Dim YPosLastLine As Single
    Dim filename1 As String

    Private Sub FrmDataFile_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            PageSetupDialog1.ShowNetwork = True
            PageSetupDialog1.EnableMetric = True 'sonst werden aus meinen millimetern immer Zoll gemacht!
            MyPagesettings = _MyPagesettings
            MyPrintersettings = _MyPrintersettings
            PageSetupDialog1.PageSettings = MyPagesettings
            PageSetupDialog1.PrinterSettings = MyPrintersettings
            RichTextBox1.Font = MyprintFont
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFont.Click
        Try
            Dim result As DialogResult
            FontDialog1.Font = MyprintFont
            FontDialog1.FixedPitchOnly = True
            FontDialog1.AllowSimulations = False
            result = FontDialog1.ShowDialog()
            If (result = DialogResult.OK) Then
                RichTextBox1.Font = FontDialog1.Font
                MyprintFont = FontDialog1.Font
                _MyprintFontName = MyprintFont.Name
                _MyprintFontSize = MyprintFont.Size
                _MyprintFontStyle = MyprintFont.Style
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnPageSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPageSettings.Click
        Try
            Dim result As DialogResult
            'Druckereinstellungen zuweisen
            PageSetupDialog1.PrinterSettings = MyPrintersettings
            'Seiteneinstellungen zuweisen
            PageSetupDialog1.PageSettings = MyPagesettings
            result = PageSetupDialog1.ShowDialog()
            If (result = DialogResult.OK) Then
                MyPrintersettings = PageSetupDialog1.PrinterSettings
                MyPagesettings = PageSetupDialog1.PageSettings
                _MyPrintersettings = MyPrintersettings
                _MyPagesettings = MyPagesettings
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnPageView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPageView.Click
        Try
            Doc.PrinterSettings = MyPrintersettings
            Doc.DefaultPageSettings = MyPagesettings
            PrintPreviewDialog1.Document = Doc
            PrintPreviewDialog1.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPrint.Click
        Try
            Doc.PrinterSettings = MyPrintersettings
            Doc.DefaultPageSettings = MyPagesettings
            Doc.Print()
            'PrintPreviewDialog1.Document = Doc
            'PrintPreviewDialog1.ShowDialog()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Try
            Me.Close()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    Public Sub DateiLesen(ByVal Filename As String)
        Try
            Dim allLines As String
            Dim sr As New IO.StreamReader(Filename)

            allLines = sr.ReadToEnd()
            sr.Close()
            RichTextBox1.Text = allLines
            Doc.DocumentName = Filename
            filename1 = _MyUtillities.HoleZeichenAusKette(Filename, _MyUtillities.SucheLetztesZeichen(Filename, "\") + 1)

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ' ========  Ereignisprozeduren für den Druckvorgang  ==========
    ' am Beginn des Druckvorgangs
    Private Sub Doc_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Doc.BeginPrint
        Try
            initPrint = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ' eine Seite drucken
    Private Sub Doc_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles Doc.PrintPage
        Try
            Dim gr As Graphics = e.Graphics
            Dim printrectf As RectangleF = RectangleF.op_Implicit(e.MarginBounds)
            Dim fnt As Font = MyprintFont
            Dim boldfnt As New Font(fnt, FontStyle.Bold)

            Static linesPerPage As Integer      'Zeilen pro Seite
            Static columnsPerLine As Integer    'Spalten pro Zeile
            Static pageNr As Integer                    'aktuelle Seitennummer (1 für erste Seite)
            Static maxPageNr As Integer             'Anzahl der Seiten des gesamten Ausdrucks
            Static lastPageNr As Integer            'letzte zu druckenden Seite
            Static lineNr As Integer             'aktuelle Zeilennummer (0 für erste Zeile)
            Static txtLines() As String             'enthält zeilenweise den zu druckenden Text

            ' Initialisierung nur beim ersten Aufruf
            ' dieser Prozedur
            If initPrint = False Then
                Dim sizf As SizeF
                Dim firstLineOfPage As New Collections.ArrayList()

                ' Int rundet ab, liefert aber Double; CInt wandelt in Integer um
                sizf = gr.MeasureString("W", fnt)
                linesPerPage = CInt(Int(printrectf.Height / sizf.Height)) - headLines - footLines
                sizf = gr.MeasureString("W", fnt, 0, StringFormat.GenericTypographic)
                columnsPerLine = CInt(Int(printrectf.Width / sizf.Width))
                sizf = gr.MeasureString("W", boldfnt)
                YPosLastLine = printrectf.Bottom - (footLines * (sizf.Height * 1.2F))
                txtLines = RichTextBox1.Lines

                ' gesamte Seitenanzahl ermitteln
                ' dazu muss das gesamte Dokument gleichsam gedruckt werden
                ' (natürlich, ohne tatsächlich Ausgaben durchzuführen);
                ' gleichzeitig die Nummer der ersten Zeile jeder Seite speichern
                lineNr = 0
                pageNr = 0
                firstLineOfPage.Insert(0, 0)    'ArrayList besteht darauf, dass es zuerst ein 0-Element gibt, bevor ein 1-Element eingefügt werden darf 
                Do
                    pageNr += 1
                    firstLineOfPage.Insert(pageNr, lineNr)
                    lineNr = PrintOnePage(gr, fnt, printrectf, txtLines, lineNr, linesPerPage, columnsPerLine, False)
                Loop Until lineNr >= txtLines.GetUpperBound(0)
                maxPageNr = pageNr

                ' erste und letzte Seite einstellen
                ' erste zu druckende Zeile einstellen
                If e.PageSettings.PrinterSettings.PrintRange = Printing.PrintRange.SomePages Then
                    pageNr = e.PageSettings.PrinterSettings.FromPage
                    lastPageNr = e.PageSettings.PrinterSettings.ToPage
                    If lastPageNr > maxPageNr Then lastPageNr = maxPageNr
                    If pageNr > maxPageNr Then pageNr = maxPageNr
                    lineNr = CInt(firstLineOfPage(pageNr))
                Else
                    pageNr = 1
                    lastPageNr = maxPageNr
                    lineNr = 0
                End If

                ' vermeiden, dass dieser Code nochmals ausgeführt wird
                initPrint = True
            End If

            ' diese Seite drucken: Kopfzeile und Seiteninhalt un  Fusszeile
            ' PrintOnePage liefert als Ergebnis die Zeilennummer 
            ' der nächsten Seite
            PrintHeadLine(gr, fnt, printrectf, pageNr, maxPageNr)
            lineNr = PrintOnePage(gr, fnt, printrectf, txtLines, _
             lineNr, linesPerPage, columnsPerLine, True)
            PrintFootLine(gr, fnt, printrectf, lineNr, linesPerPage)

            ' Seitennummer erhöhen, Ausdruck gegebenenfalls abbrechen
            pageNr += 1
            If pageNr <= lastPageNr Then
                e.HasMorePages = True
            Else
                e.HasMorePages = False
                Erase txtLines
            End If
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ' Ausdruck abschließen
    Private Sub Doc_EndPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Doc.EndPrint
        Try
            initPrint = False
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ' ========== Hilfsfunktionen ============
    ' Kopfzeile drucken
    Private Sub PrintHeadLine(ByVal gr As Graphics, _
     ByVal fnt As Font, ByVal printrectf As RectangleF, _
     ByVal pageNr As Integer, ByVal maxPageNr As Integer)

        Try

            ' für die Kopfzeile sind die ersten zwei Zeilen vorgesehen; siehe CalcLinesPerPage
            Dim s As String
            Dim y As Single
            Dim sizf As SizeF
            Dim rectf1 As RectangleF
            Dim boldfnt As New Font(fnt, FontStyle.Bold)
            Dim sf As New StringFormat(StringFormatFlags.NoWrap)

            ' links Seitennummer
            s = ml_string(311, "Seite ") & pageNr & " / " & maxPageNr
            sizf = gr.MeasureString(s, boldfnt)
            gr.DrawString(s, boldfnt, Brushes.Black, printrectf.X, printrectf.Y)

            ' rechts Dateiname
            s = filename1
            sf.Alignment = StringAlignment.Far
            sf.Trimming = StringTrimming.EllipsisPath
            rectf1 = New RectangleF( _
             printrectf.X + sizf.Width * 1.2!, printrectf.Y, _
             printrectf.Width - sizf.Width * 1.2!, sizf.Height)
            gr.DrawString(s, boldfnt, Brushes.Black, rectf1, sf)

            ' darunter Linie
            y = printrectf.Y + sizf.Height * 1.2F
            gr.DrawLine(Pens.Black, printrectf.X, y, printrectf.Right, y)

            'Aufräumarbeiten
            boldfnt.Dispose()
            sf.Dispose()
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ' Fusszeile drucken
    Private Sub PrintFootLine(ByVal gr As Graphics, _
    ByVal fnt As Font, ByVal printrectf As RectangleF, ByVal lines As Integer, ByVal linesPerpage As Integer)

        Try
            ' für die Kopfzeile sind die ersten zwei Zeilen vorgesehen; siehe CalcLinesPerPage
            Dim s As String
            Dim sizf As SizeF
            Dim rectf1 As RectangleF
            Dim boldfnt As New Font(fnt, FontStyle.Bold)
            Dim sf As New StringFormat(StringFormatFlags.NoWrap)

            ' darunter Linie
            gr.DrawLine(Pens.Black, printrectf.X, YPosLastLine, printrectf.Right, YPosLastLine)

            '' links Druckzeit
            's = Format(Now)
            'gr.DrawString(s, boldfnt, Brushes.Black, XPos, YPos)

            ' rechts Dateiname in nächster Zeile
            'YPos += lineheight
            s = "FHT59N3T"
            sf.Alignment = StringAlignment.Far
            sf.Trimming = StringTrimming.EllipsisPath
            rectf1 = New RectangleF( _
             printrectf.X + sizf.Width * 1.2!, YPosLastLine, _
             printrectf.Width - sizf.Width * 1.2!, sizf.Height)
            gr.DrawString(s, boldfnt, Brushes.Black, rectf1, sf)

            'Aufräumarbeiten
            boldfnt.Dispose()
            sf.Dispose()

        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Sub

    ' eine Seite drucken; liefert die Nummer der nächsten Zeile zurück
    Private Function PrintOnePage( _
     ByVal gr As Graphics, ByVal fnt As Font, _
     ByVal printrectf As RectangleF, ByVal txtLines() As String, _
     ByVal startlineNr As Integer, ByVal linesPerPage As Integer, _
     ByVal colsPerLine As Integer, ByVal reallyPrint As Boolean) _
     As Integer

        Try
            Dim s As String
            Dim lineNr As Integer = startlineNr
            Dim xpos1, xpos2, ypos As Double
            Dim lineheight As Double
            Dim sf As StringFormat = StringFormat.GenericTypographic
            Dim boldfnt As New Font(fnt, FontStyle.Bold)

            ' xpos1: X-Position für Zeilennummer
            ' xpos2: X-Position für Listing
            ' ypos : Y-Position der nächsten Zeile
            xpos1 = printrectf.X
            xpos2 = printrectf.X + _
             gr.MeasureString("88888", fnt, 0, sf).Width
            ypos = printrectf.Y + _
             gr.MeasureString("W", fnt).Height * headLines

            ' Schleife über Textzeilen (solange Platz auf der Seite ist)
            Do
                ' Zeilenumbruch für die nächste Zeile 
                ' (5 ist die Breite für Zeilennummer)
                s = UnTabify(txtLines(lineNr))
                s = WrapLine(s, colsPerLine - 5)
                ' "W", damit auch bei leeren Zeilen sinnvolles Ergebnis
                lineheight = gr.MeasureString("W" + s, fnt).Height
                ' ist noch genug Platz
                If ypos + lineheight < YPosLastLine Or lineNr = startlineNr Then
                    If reallyPrint Then
                        '' Zeilennummer drucken
                        'lineNrTxt = (lineNr + 1).ToString("####")
                        'gr.DrawString(lineNrTxt, boldfnt, Brushes.Black, xpos1, ypos)
                        ' Zeile drucken
                        'gr.DrawString(s, fnt, Brushes.Black, xpos2, ypos, sf)
                        gr.DrawString(s, fnt, Brushes.Black, xpos1, ypos, sf)
                    End If
                    ypos += lineheight
                    lineNr += 1
                Else
                    ' diese Zeile hat nicht mehr Platz; auf der nächsten Seite drucken
                    Exit Do
                End If
            Loop Until lineNr >= txtLines.GetUpperBound(0)

            ' Aufräumarbeiten
            boldfnt.Dispose()
            Return lineNr
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
        End Try
    End Function

    ' zerlegt eine lange Zeile in mehrere Zeilen und rückt 
    ' den Text ab der zweiten Zeile ein
    ' cols gibt an, wieviele Zeichen pro Zeile maximal angezeigt werden können
    Private Function WrapLine(ByVal s As String, _
     ByVal cols As Integer) As String
        Try
            Dim i, indent, pos As Integer
            Dim tmp As New System.Text.StringBuilder()
            ' bei kurzen Zeichenketten: kein Umbruch notwendig
            If Len(s) <= cols Then Return s
            ' Einrücktiefe ermitteln (max. halbe Seitenbreite)
            For i = 1 To Len(s)
                If Mid(s, i, 1) <> " " Then Exit For
            Next
            indent = i + 1  'um zwei Zeichen einrücken
            If indent > cols / 2 Then indent = CInt(cols / 2)
            ' Zeile zerlegen, ab der zweiten Zeile einrücken
            tmp.Append(Strings.Left(s, cols))
            pos = cols
            While pos < Len(s)
                tmp.Append(vbCrLf + Space(indent) + Mid(s, pos + 1, cols - indent))
                pos += cols - indent
            End While
            Return tmp.ToString
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return ""
        End Try
    End Function

    ' ersetzt Tabulatorzeichen durch die entsprechende Anzahl von Leerzeichen
    Private Function UnTabify(ByVal s As String) As String
        Try
            Const tabsize As Integer = 8
            Dim pos, realpos, n As Integer
            Dim tmp As New System.Text.StringBuilder()
            For pos = 1 To Len(s)
                If Mid(s, pos, 1) <> vbTab Then
                    tmp.Append(Mid(s, pos, 1))
                    realpos += 1
                Else
                    ' Anzahl der einzufügenden Leerzeichen berechnen
                    n = tabsize - ((pos - 1) Mod tabsize)
                    tmp.Append(Space(n))
                    realpos += n
                End If
            Next
            Return tmp.ToString
        Catch ex As Exception
            Trace.TraceError("Message: " & ex.Message & vbCrLf & "Stacktrace : " & ex.StackTrace)
            Return ""
        End Try
    End Function

    Private Sub RichTextBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RichTextBox1.Click
    End Sub


  Public Sub New
    InitializeComponent()
    ml_UpdateControls()
    AddHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub

  Private Sub frmResultFile_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
  RemoveHandler MlRuntime.MlRuntime.LanguageChanged, AddressOf ml_UpdateControls
  End Sub
  Protected Overridable Sub ml_UpdateControls()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmResultFile))
    resources.ApplyResources(Me.BtnClose, "BtnClose")
    resources.ApplyResources(Me.BtnFont, "BtnFont")
    resources.ApplyResources(Me.BtnPageSettings, "BtnPageSettings")
    resources.ApplyResources(Me.BtnPageView, "BtnPageView")
    resources.ApplyResources(Me.BtnPrint, "BtnPrint")
    resources.ApplyResources(Me, "$this")
    resources.ApplyResources(Me.OpenFileDialog1, "OpenFileDialog1")
  End Sub
End Class