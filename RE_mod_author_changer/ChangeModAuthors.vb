Imports System.CodeDom
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports System.Xml
Imports SharpCompress.Archives.Rar

Public Class ChangeModAuthors
    Private Property RarExe As String
    Private MyPath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
    Public Sub New(ByVal _RarExe As String)
        RarExe = _RarExe
    End Sub

    Public Function Zip(FormMain As FrmMain, ByVal _ZipFile As String, ByVal Name As String)

        Logger.Log(FormMain, String.Format("{0}========== START FILE {1}{2}{1} ==========", Environment.NewLine, """", Path.GetFileName(_ZipFile)), False)
        Logger.Log(FormMain, String.Format("Extracting {0}{1}{0}", """", _ZipFile))
        Dim ExtractedFiles As New List(Of String)

        Using Archive As ZipArchive = ZipFile.Open(_ZipFile, ZipArchiveMode.Update)

            For Each ZipEntry In Archive.Entries
                If ZipEntry.FullName.ToLower.Contains("modinfo.ini") Then

                    Dim OutPath As String = Path.Combine(MyPath, ZipEntry.FullName)
                    Directory.CreateDirectory(Path.GetDirectoryName(OutPath))
                    ZipEntry.ExtractToFile(OutPath, True)
                    ExtractedFiles.Add(ZipEntry.FullName)

                    Logger.Log(FormMain, String.Format("Extraction complete: {0}{1}{0}", """", ZipEntry.FullName))

                    Dim Ini As New IniFileParser(OutPath)
                    Ini.SetValue("author", Name)

                    Logger.Log(FormMain, String.Format("Changed author for: {0}{1}{0}", """", ZipEntry.FullName))

                End If
            Next

            For Each File In ExtractedFiles
                Dim ArchiveEntry As ZipArchiveEntry = Archive.GetEntry(File)
                ArchiveEntry.Delete()
                Logger.Log(FormMain, String.Format("Deleted from archive: {0}{1}{0}", """", File))
                Archive.CreateEntryFromFile(File, File)
                IO.File.Delete(File)
                Logger.Log(FormMain, String.Format("Moved to archive: {0}{1}{0}", """", File))

                If Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(File)) Then
                    Dim DirToDelete = Path.GetDirectoryName(File).Split("\")(0)
                    Try
                        Logger.Log(FormMain, String.Format("Cleaning up directory {0}{1}{0}", """", DirToDelete))
                        Directory.Delete(DirToDelete, True)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                End If
            Next
        End Using

        Logger.Log(FormMain, String.Format("========== DONE FILE {1}{2}{1} ==========", Environment.NewLine, """", Path.GetFileName(_ZipFile)), False)
        Return Task.FromResult(0)
    End Function

    Public Function Rar(FormMain As FrmMain, ByVal _rarArchive As String, ByVal Name As String)
        Logger.Log(FormMain, String.Format("{0}========== START ARCHIVE {1}{2}{1} ==========", Environment.NewLine, """", Path.GetFileName(_rarArchive)), False)
        Dim FileList As New List(Of String)
        Using archive = RarArchive.Open(_rarArchive)
            For Each file In archive.Entries
                If Not file.IsDirectory And file.Key.ToLower.Contains("modinfo.ini") Then
                    Logger.Log(FormMain, String.Format("Extracting {0}{1}{0}", """", file.Key))
                    FileList.Add(file.Key)

                    Dim destination = Path.Combine(MyPath, file.Key)
                    If Not Directory.Exists(Path.GetDirectoryName(destination)) Then Directory.CreateDirectory(Path.GetDirectoryName(destination))

                    Using stream = file.OpenEntryStream()
                        Using outputStream = IO.File.Create(destination)
                            stream.CopyTo(outputStream)
                        End Using
                    End Using
                    Dim Ini As New IniFileParser(destination)
                    Ini.SetValue("author", Name)
                End If
            Next
        End Using

        For Each file In FileList
            Logger.Log(FormMain, String.Format("Changing author to {0}{3}{0} for {0}{2}{0}", """", MyPath, file, Name))
            Dim destination = Path.Combine(MyPath, file)
            Dim ShowRarOut As Boolean = My.Settings.ShowConsoleOutput
            Dim RarOutput As String
            Dim RarOutputShort As String
            Dim RarProcess As New Process()
            Dim RarProcessInfo As New ProcessStartInfo(RarExe, String.Format("m -r {0}{1}{0} {0}{2}{0}", """", _rarArchive, file)) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .CreateNoWindow = My.Settings.HidePopupWindow}
            Logger.Log(FormMain, String.Format("Moving {0}{1}{0} back to {0}{2}{0}", """", file, Path.GetFileName(_rarArchive)))
            RarOutput = StartRarProcessAndReturnOutput(FormMain, _rarArchive, RarProcessInfo)
            RarOutputShort = FormatRarOutput(RarOutput)

            If ShowRarOut Then Logger.Log(FormMain, RarOutputShort, False)

            If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Or RarOutput.Contains("100%") Then
                Logger.Log(FormMain, "rar.exe returned OK, Done, or 100%")
            Else
                Logger.Log(FormMain, "rar.exe did not return ok, likely an error. turn on Show all console output or uncheck hide CMD to help diagnose issue")
                Return 0
            End If

            If Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(file)) Then
                Dim DirToDelete = Path.GetDirectoryName(file).Split("\")(0)
                Try
                    Logger.Log(FormMain, String.Format("Cleaning up directory {0}{1}{0}", """", DirToDelete))
                    Directory.Delete(DirToDelete, True)
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
            Logger.Log(FormMain, String.Format("========== DONE FILE {1}{2}{1} ==========", Environment.NewLine, """", file), False)
        Next

    End Function

    Private Function StartRarProcessAndReturnOutput(FormMain As FrmMain, ByVal FileName As String, ByVal ProcStartInfo As ProcessStartInfo) As String

        Dim RarProcess As New Process()
        Dim RarOutput As String


        RarProcess.StartInfo = ProcStartInfo
        RarProcess.Start()

        If (Not RarProcess.WaitForExit(My.Settings.RarProcessTimeout)) Then
            RarProcess.Kill()
            Logger.Log(FormMain, Environment.NewLine & "================================================================================", False)
            Logger.Log(FormMain, "RAR.exe HAS EXCEEDED MAXIMUM WAIT TIME AND HAS BEEN KILLED!")
            Logger.Log(FormMain, "You might have to process this archive manually!")
            Logger.Log(FormMain, "RarProcStartInfo")
            Logger.Log(FormMain, String.Format("FileName: {0}", ProcStartInfo.FileName))
            Logger.Log(FormMain, String.Format("Arguments: {0}", ProcStartInfo.Arguments))
            Logger.Log(FormMain, "================================================================================" & Environment.NewLine, False)
            Return "err"
        End If


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



End Class
