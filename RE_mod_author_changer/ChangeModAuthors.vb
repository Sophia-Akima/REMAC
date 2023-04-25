Imports System.CodeDom
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports System.Xml

Public Class ChangeModAuthors
    Private Property RarExe As String
    Private MyPath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
    Public Sub New(ByVal _RarExe As String)
        RarExe = _RarExe
    End Sub

    Public Function Zip(ByVal _ZipFile As String) As Task(Of Integer)

        Log(String.Format("{0}========== START FILE {1}{2}{1} ==========", Environment.NewLine, """", Path.GetFileName(_ZipFile)), False)
        Log(String.Format("Extracting {0}{1}{0}", """", _ZipFile))
        Dim ExtractedFiles As New List(Of String)

        Using Archive As ZipArchive = ZipFile.Open(_ZipFile, ZipArchiveMode.Update)

            For Each ZipEntry In Archive.Entries
                If ZipEntry.FullName.ToLower.Contains("modinfo.ini") Then

                    Dim OutPath As String = Path.Combine(MyPath, ZipEntry.FullName)
                    Directory.CreateDirectory(Path.GetDirectoryName(OutPath))
                    ZipEntry.ExtractToFile(OutPath, True)
                    ExtractedFiles.Add(ZipEntry.FullName)

                    Log(String.Format("Extract {0}{1}{0} complete", """", ZipEntry.FullName))

                    Dim Ini As New IniFileParser(OutPath)
                    Ini.SetValue("author", FrmMain.TxtAuthor.Text)

                    Log(String.Format("Changing mod author for {0}{1}{0} complete", """", ZipEntry.FullName))

                End If
            Next

            For Each File In ExtractedFiles
                Dim ArchiveEntry As ZipArchiveEntry = Archive.GetEntry(File)
                ArchiveEntry.Delete()
                Log(String.Format("Deleted {0}{1}{0} from archive", """", File))
                Archive.CreateEntryFromFile(File, File)
                IO.File.Delete(File)
                Log(String.Format("Moved {0}{1}{0} to archive", """", File))

                If Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(File)) Then
                    Dim DirToDelete = Path.GetDirectoryName(File).Split("\")(0)
                    Try
                        Log(String.Format("Cleaning up directory {0}{1}{0}", """", DirToDelete))
                        Directory.Delete(DirToDelete, True)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                End If

                Log(String.Format("Deleted directory {0}{1}{0}", """", Path.GetDirectoryName(File).Split("\")(0)))
            Next
        End Using

        Log(String.Format("========== DONE FILE {1}{2}{1} ==========", Environment.NewLine, """", Path.GetFileName(_ZipFile)), False)
        Return Task.FromResult(0)
    End Function
    Public Function Rar(ByVal RarArchive As String) As Task(Of Integer)
        Dim ShowRarOut As Boolean = My.Settings.ShowConsoleOutput
        Dim RarOutput As String
        Dim RarOutputShort As String
        Dim RarProcess As New Process()
        Dim RarProcessInfo As New ProcessStartInfo(RarExe, String.Format("lb {0}{1}{0}", """", RarArchive)) With {
            .UseShellExecute = False,                                   ' This initial argument is simply to LOOK
            .RedirectStandardOutput = True,                             ' at the files BASIC information (just the location)
            .CreateNoWindow = My.Settings.HidePopupWindow}

        'This will return the filenames
        RarOutput = StartRarProcessAndReturnOutput(RarArchive, RarProcessInfo)
        RarOutputShort = FormatRarOutput(RarOutput)
        If ShowRarOut Then Log(RarOutputShort, False)

        For Each File As String In RarOutput.Split(Environment.NewLine)
            If File.ToLower.Contains("modinfo.ini") Then
                Log(String.Format("{0}========== START FILE {1}{2}{1} ==========", Environment.NewLine, """", File), False)
                Log(String.Format("Extracting {0}{1}{0}", """", File))

                RarProcessInfo.Arguments = String.Format("x -o+ {0}{1}{0} {0}{2}{0} {0}{3}{0}", """", RarArchive, File, MyPath)
                RarOutput = StartRarProcessAndReturnOutput(RarArchive, RarProcessInfo)
                RarOutputShort = FormatRarOutput(RarOutput)
                If ShowRarOut Then Log(RarOutputShort, False)

                If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Or RarOutput.Contains("100%") Then
                    Log("rar.exe returned OK, Done, or 100%")
                Else Return Task.FromResult(1)
                End If

                Log(String.Format("Changing mod author for {0}{1}\{2}{0}", """", MyPath, File))
                Dim Ini As New IniFileParser(File)
                Ini.SetValue("author", FrmMain.TxtAuthor.Text)

                Log(String.Format("Moving {0}{1}{0} back to {0}{2}{0}", """", File, Path.GetFileName(RarArchive)))
                RarProcessInfo.Arguments = String.Format("m -r {0}{1}{0} {0}{2}{0}", """", RarArchive, File)
                RarOutput = StartRarProcessAndReturnOutput(RarArchive, RarProcessInfo)
                RarOutputShort = FormatRarOutput(RarOutput)
                If ShowRarOut Then Log(RarOutputShort, False)

                If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Or RarOutput.Contains("100%") Then
                    Log("rar.exe returned OK, Done, or 100%")
                Else Return Task.FromResult(1)
                End If

                If Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(File)) Then
                    Dim DirToDelete = Path.GetDirectoryName(File).Split("\")(0)
                    Try
                        Log(String.Format("Cleaning up directory {0}{1}{0}", """", DirToDelete))
                        Directory.Delete(DirToDelete, True)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                End If
                Log(String.Format("========== DONE FILE {1}{2}{1} ==========", Environment.NewLine, """", File), False)
            End If
        Next

        Return Task.FromResult(0)
    End Function

    Private Function StartRarProcessAndReturnOutput(ByVal FileName As String, ByVal ProcStartInfo As ProcessStartInfo) As String
        Dim RarProcess As New Process()
        Dim RarOutput As String

        RarProcess.StartInfo = ProcStartInfo
        RarProcess.Start()
        RarProcess.WaitForExit()

        Using sr As StreamReader = RarProcess.StandardOutput
            RarOutput = sr.ReadToEnd
        End Using

        Return RarOutput
    End Function

    Private Function FormatRarOutput(ByVal Output As String)
        Dim OutputNoSquares = Regex.Replace(Output, "[^\P{C}\n]+", String.Empty)
        Dim OutputNoEmptyLines = Regex.Replace(OutputNoSquares, "^[\s\t]*\r?\n", String.Empty, RegexOptions.Multiline).Trim
        Dim Fluff0 As String = String.Format("{0}========== RAR.exe START =========={0}{1}", Environment.NewLine, OutputNoEmptyLines)
        Dim Fluff1 As String = String.Format("{1}{0}========== RAR.exe DONE =========={0}", Environment.NewLine, Fluff0)
        Return Fluff1
    End Function

    Public Shared Sub Log(ByVal text As String, Optional ByVal remac As Boolean = True)
        If remac Then
            FrmMain.RtbOutput.Invoke(Sub()
                                         FrmMain.RtbOutput.AppendText(DateTime.Now.ToString("HH:mm:ss") & " REMAC: " & text & Environment.NewLine)
                                     End Sub)
        Else
            FrmMain.RtbOutput.AppendText(text & Environment.NewLine)
        End If
    End Sub
End Class
