
Imports System.CodeDom
Imports System.Collections.Generic
Imports System.Data.SqlTypes
Imports System.IO
Imports System.Linq
Imports System.Text


''' <summary>
''' Simple CSV export
''' Example:
'''   CsvExport myExport = new CsvExport();
'''   myExport.AddRow();
'''   myExport["Region"] = "New York, USA";
'''   myExport["Sales"] = 100000;
'''   myExport["Date Opened"] = new DateTime(2003, 12, 31);
'''   myExport.AddRow();
'''   myExport["Region"] = "Sydney \"in\" Australia";
'''   myExport["Sales"] = 50000;
'''   myExport["Date Opened"] = new DateTime(2005, 1, 1, 9, 30, 0);
''' Then you can do any of the following three output options:
'''   string myCsv = myExport.Export();
'''   myExport.ExportToFile("Somefile.csv");
'''   byte[] myCsvData = myExport.ExportToBytes();
''' </summary>
Public Class CsvExport
    ''' <summary>
    ''' To keep the ordered list of column names
    ''' </summary>
    Private ReadOnly m_fields As New List(Of String)()

    ''' <summary>
    ''' The list of rows
    ''' </summary>
    Private ReadOnly m_rows As New List(Of Dictionary(Of String, Object))()



    ''' <summary>
    ''' Initializes a new instance of the <see cref="Jitbit.Utils.CsvExport"/> class.
    ''' </summary>
    ''' <param name="columnSeparator">
    ''' The string used to separate columns in the output.
    ''' By default this is a comma so that the generated output is a CSV file.
    ''' </param>
    ''' <param name="includeColumnSeparatorPreamble">
    ''' Whether to include the preamble that declares which column separator is used in the output.
    ''' By default this is <c>true</c> so that Excel can open the generated CSV
    ''' without asking the user to specify the delimiter used in the file.
    ''' </param>
    Public Sub New(Optional columnSeparator As String = ",", Optional includeColumnSeparatorPreamble As Boolean = False)
        Me.ColumnSeparator = columnSeparator
        Me.IncludeColumnSeparatorPreamble = includeColumnSeparatorPreamble
    End Sub

    ''' <summary>
    ''' Whether to include the preamble that declares which column separator is used in the output
    ''' </summary>
    Public Property IncludeColumnSeparatorPreamble() As Boolean
        Get
            Return m_IncludeColumnSeparatorPreamble
        End Get
        Set(value As Boolean)
            m_IncludeColumnSeparatorPreamble = value
        End Set
    End Property
    Private m_IncludeColumnSeparatorPreamble As Boolean

    ''' <summary>
    ''' The string used to separate columns in the output
    ''' </summary>
    Public Property ColumnSeparator() As String
        Get
            Return m_ColumnSeparator
        End Get
        Set(value As String)
            m_ColumnSeparator = value
        End Set
    End Property
    Private m_ColumnSeparator As String

    ''' <summary>
    ''' Gets or sets the optional comment line added before the column header row.
    ''' </summary>
    ''' <value>
    ''' The comment line.
    ''' </value>
    Public Property CommentLine() As String
        Get
            Return m_CommentLine
        End Get
        Set(value As String)
            m_CommentLine = value
        End Set
    End Property
    Private m_CommentLine As String

    ''' <summary>
    ''' The current row
    ''' </summary>
    Private ReadOnly Property CurrentRow() As Dictionary(Of String, Object)
        Get
            Return m_rows(m_rows.Count - 1)
        End Get
    End Property

    ''' <summary>
    ''' Set a value on this column
    ''' </summary>
    ''' <value>
    ''' The <see cref="System.Object"/>.
    ''' </value>
    ''' <param name="field">The field.</param>
    ''' <returns>value of column</returns>
    Default Public WriteOnly Property Item(field As String) As Object
        Set(value As Object)
            ' Keep track of the field names, because the dictionary loses the ordering
            SetField(field)
            CurrentRow(field) = value
        End Set
    End Property

    ''' <summary>
    ''' Sets the field.
    ''' </summary>
    ''' <param name="fieldName">Name of the field.</param>
    Public Sub SetField(fieldName As String)
        If Not m_fields.Contains(fieldName) Then
            m_fields.Add(fieldName)
        End If
    End Sub

    ''' <summary>
    ''' Clears the rows.
    ''' </summary>
    Public Sub ClearRows()
        m_rows.Clear()
    End Sub

    ''' <summary>
    ''' Call this before setting any fields on a row
    ''' </summary>
    Public Sub AddRow()
        m_rows.Add(New Dictionary(Of String, Object)())
    End Sub

    ''' <summary>
    ''' Add a list of typed objects, maps object properties to CsvFields
    ''' </summary>
    ''' <typeparam name="T">the list type</typeparam>
    ''' <param name="list">The list.</param>
    Public Sub AddRows(Of T)(list As IEnumerable(Of T))
        If list.Any() Then

            For Each obj As Object In list
                AddRow()
                Dim values = obj.GetType().GetProperties()
                For Each v As Object In values
                    Me(v.Name) = v.GetValue(obj, Nothing)
                Next
            Next
        End If
    End Sub

    ''' <summary>
    ''' Converts a value to how it should output in a csv file
    ''' If it has a comma, it needs surrounding with double quotes
    ''' Eg Sydney, Australia -&gt; "Sydney, Australia"
    ''' Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
    ''' Eg "Dangerous Dan" McGrew -&gt; """Dangerous Dan"" McGrew"
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <param name="columnSeparator">The string used to separate columns in the output.
    ''' By default this is a comma so that the generated output is a CSV document.</param>
    ''' <returns>formatted value</returns>
    Public Shared Function MakeValueCsvFriendly(value As Object, Optional columnSeparator As String = ",") As String
        If value Is Nothing Then
            Return ""
        End If
        If TypeOf value Is INullable AndAlso DirectCast(value, INullable).IsNull Then
            Return ""
        End If
        If TypeOf value Is DateTime Then
            If DirectCast(value, DateTime).TimeOfDay.TotalSeconds = 0 Then
                Return DirectCast(value, DateTime).ToString("yyyy-MM-dd")
            End If
            Return DirectCast(value, DateTime).ToString("yyyy-MM-dd HH:mm:ss")
        End If
        Dim output As String = value.ToString().Trim()
        If output.Contains(columnSeparator) OrElse output.Contains("""") OrElse output.Contains(vbLf) OrElse output.Contains(vbCr) Then
            output = """"c + output.Replace("""", """""") + """"c
        End If

        'cropping value for stupid Excel
        If output.Length > 30000 Then
            If output.EndsWith("""") Then
                output = output.Substring(0, 30000)
                'rare situation when cropped line ends with a '"'
                If output.EndsWith("""") AndAlso Not output.EndsWith("""""") Then
                    'add another '"' to escape it
                    output += """"
                End If
                output += """"
            Else
                output = output.Substring(0, 30000)
            End If
        End If
        Return output
    End Function



    ''' <summary>
    ''' Output all rows as a CSV returning a string
    ''' </summary>
    ''' <returns>full csv file as string</returns>
    Public Function ExportFile() As String
        Dim sb As New StringBuilder()

        For Each line As String In Me.ExportToLines()
            sb.AppendLine(line)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Exports to a file
    ''' </summary>
    ''' <param name="path">The path.</param>
    Public Overridable Sub ExportFile(path As String)
        File.WriteAllLines(path, Me.ExportToLines(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' Exports as raw UTF8 bytes
    ''' </summary>
    ''' <returns>the csv file as byte array</returns>
    Public Function ExportToBytes() As Byte()
        Dim data = Encoding.UTF8.GetBytes(ExportFile())
        Return Encoding.UTF8.GetPreamble().Concat(data).ToArray()
    End Function

    ''' <summary>
    ''' Outputs all rows as a CSV, returning one string at a time
    ''' </summary>
    ''' <returns>list of csv lines</returns>
    Private Function ExportToLines() As IEnumerable(Of String)
        Dim list As New List(Of String)
        If IncludeColumnSeparatorPreamble Then
            list.Add(Convert.ToString("sep=") & ColumnSeparator)
        End If

        ' the optional comment line
        If Not String.IsNullOrEmpty(CommentLine) Then
            list.Add(MakeValueCsvFriendly(CommentLine))
        End If

        ' The header
        list.Add(String.Join(ColumnSeparator, m_fields))

        ' The rows
        For Each row As Dictionary(Of String, Object) In m_rows
            For Each k As String In m_fields.Where(Function(f) Not row.ContainsKey(f))
                row(k) = Nothing
            Next
            list.Add(String.Join(ColumnSeparator, m_fields.[Select](Function(field) MakeValueCsvFriendly(row(field), ColumnSeparator))))
        Next
        Return list
    End Function
End Class


