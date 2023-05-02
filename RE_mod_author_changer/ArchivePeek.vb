Imports SharpCompress.Archives.Rar
Imports SharpCompress.Archives.Zip
Imports SharpCompress.Common
Imports System.IO

Public Class ArchivePeek
    Private Property ArchivePath As String

    Private WorkingDir As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
    Public Sub New(ByVal _ArchivePath As String)
        ArchivePath = _ArchivePath
    End Sub
    Public Function ListAuthorsRar(_Form As FrmMain, ByVal NameToCheck As String, ByVal ListAll As Boolean) As Integer

        Logger.Log(_Form, String.Format("Checking {0}{1}{0}", """", ArchivePath))

        Using archive = RarArchive.Open(ArchivePath)
            For Each entry In archive.Entries
                If Not entry.IsDirectory And entry.Key.ToLower.Contains("modinfo.ini") Then

                    Dim destination = Path.Combine(WorkingDir, "modinfochkrar.ini")
                    Using stream = entry.OpenEntryStream()
                        Using outputStream = File.Create(destination)
                            stream.CopyTo(outputStream)
                        End Using
                    End Using

                    Dim Ini As New IniFileParser(destination)
                    Dim Author As String = Ini.GetValue("author").ToLower
                    If String.IsNullOrEmpty(Author) Then Author = "NULL"
                    NameToCheck = NameToCheck.ToLower
                    File.Delete(destination)
                    If Author = NameToCheck And Not ListAll Then
                        Logger.Log(_Form, String.Format("MATCH: {0}{2}{0} : {0}{1}{0}", """", entry.Key, Author))
                    ElseIf Author.Contains(NameToCheck) And Not ListAll Then
                        Logger.Log(_Form, String.Format("CONTAINS: {0}{2}{0} : {0}{1}{0}", """", entry.Key, Author))
                    End If
                    If ListAll Then Logger.Log(_Form, String.Format("Author: {0}{2}{0} : {0}{1}{0}", """", entry.Key, Author))
                End If
            Next
        End Using
        Return 0
    End Function

    Public Function ListAuthorsZip(_Form As FrmMain, ByVal NameToCheck As String, ByVal ListAll As Boolean)
        Logger.Log(_Form, String.Format("Checking {0}{1}{0}", """", ArchivePath))

        Using archive = ZipArchive.Open(ArchivePath)
            For Each ZipEntry In archive.Entries
                Dim FullPath = ZipEntry.Key.ToString
                Dim FileName = Path.GetFileName(FullPath)


                If Not ZipEntry.IsDirectory And FileName.ToLower.Contains("modinfo.ini") Then
                    Dim Destination = Path.Combine(WorkingDir, "modinfochkzip.ini")
                    Using stream = ZipEntry.OpenEntryStream()
                        Using outputStream = File.Create(Destination)
                            stream.CopyTo(outputStream)
                        End Using
                    End Using

                    Dim Ini As New IniFileParser(Destination)
                    Dim Author As String = Ini.GetValue("author").ToLower
                    If String.IsNullOrEmpty(Author) Then Author = "NULL"
                    NameToCheck = NameToCheck.ToLower
                    File.Delete(Destination)

                    If Author = NameToCheck And Not ListAll Then
                        Logger.Log(_Form, String.Format("MATCH: {0}{2}{0} : {0}{1}{0}", """", FullPath, Author))
                    ElseIf Author.Contains(NameToCheck) And Not ListAll Then
                        Logger.Log(_Form, String.Format("CONTAINS: {0}{2}{0} : {0}{1}{0}", """", FullPath, Author))
                    End If
                    If ListAll Then Logger.Log(_Form, String.Format("Author: {0}{2}{0} : {0}{1}{0}", """", FullPath, Author))
                End If
            Next
        End Using
        Return 0
    End Function

End Class
