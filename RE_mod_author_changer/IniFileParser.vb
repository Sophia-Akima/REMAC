Imports System.IO

Public Class IniFileParser

    Private ReadOnly _dictionary As New Dictionary(Of String, String)()
    Private ReadOnly _filename As String

    Public Sub New(ByVal filename As String)
        _filename = filename
        ReadFile(_filename)
    End Sub

    Public Function GetValue(ByVal key As String) As String
        If _dictionary.ContainsKey(key) Then
            Return _dictionary(key)
        Else
            Return Nothing
        End If
    End Function

    Public Sub SetValue(ByVal key As String, ByVal value As String)
        _dictionary(key) = value
        SaveToFile(_filename)
    End Sub

    Private Sub ReadFile(ByVal filename As String)
        Dim lines() As String = File.ReadAllLines(filename)
        For Each line As String In lines
            Dim parts() As String = line.Split("="c)
            If parts.Length = 2 Then
                Dim key As String = parts(0).Trim()
                Dim value As String = parts(1).Trim()
                _dictionary(key) = value
            End If
        Next
    End Sub

    Private Sub SaveToFile(ByVal filename As String)
        Dim lines As New List(Of String)
        For Each pair As KeyValuePair(Of String, String) In _dictionary
            Dim line As String = pair.Key & "=" & pair.Value
            lines.Add(line)
        Next
        File.WriteAllLines(filename, lines.ToArray())
    End Sub

End Class
