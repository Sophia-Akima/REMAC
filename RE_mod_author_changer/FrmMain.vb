Imports System.Drawing.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Public Class FrmMain
    Private ExePath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
    Private WinrarPath As String
    Private RarPath As String
    Private IniPath As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = String.Format("{0} {1}", Me.Text, FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion)
        TxtWinrar.Text = My.Settings.WinrarPath
        TxtAuthor.Text = My.Settings.AuthorName
        If (String.IsNullOrEmpty(TxtWinrar.Text)) Then
            btnSetAuthorAll.Enabled = False
        End If
    End Sub

    Private Sub BtnBrowseWinrar_Click(sender As Object, e As EventArgs) Handles BtnBrowseWinrar.Click
        If (FbdWinrar.ShowDialog() = DialogResult.OK) Then
            TxtWinrar.Text = FbdWinrar.SelectedPath
        End If
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Settings.Save()
    End Sub

    Private Sub LstArchives_DragDrop(sender As Object, e As DragEventArgs) Handles LstArchives.DragDrop
        For Each f As String In e.Data.GetData(DataFormats.FileDrop)
            If (Path.GetExtension(f) = ".rar") Then
                LstArchives.Items.Add(f)
            End If
        Next
    End Sub

    Private Sub LstArchives_DragOver(sender As Object, e As DragEventArgs) Handles LstArchives.DragOver
        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            e.Effect = DragDropEffects.Link
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        Dim SelectedItems = New ListBox.SelectedObjectCollection(LstArchives)
        SelectedItems = LstArchives.SelectedItems
        If (Not LstArchives.SelectedIndex = -1) Then
            For i As Integer = SelectedItems.Count - 1 To 0 Step -1
                LstArchives.Items.Remove(SelectedItems(i))
            Next
        End If
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        LstArchives.Items.Clear()
    End Sub

    Private Async Sub btnSetAuthorAll_Click(sender As Object, e As EventArgs) Handles btnSetAuthorAll.Click

        My.Settings.Save()
        btnSetAuthorAll.Enabled = False
        BtnBrowseWinrar.Enabled = False
        BtnSettings.Enabled = False
        LstArchives.Enabled = False
        TxtWinrar.Enabled = False

        WriteLn("Task UpdateModInfo start")
        Dim result = Await UpdateModinfoRar()
        WriteLn(Environment.NewLine & "Task UpdateModInfo finished")

        TxtWinrar.Enabled = True
        btnSetAuthorAll.Enabled = True
        BtnBrowseWinrar.Enabled = True
        BtnSettings.Enabled = True
        LstArchives.Enabled = True
    End Sub
    Private Function UpdateModinfoZip() As Task(Of Integer)

        Return Task.FromResult(0)
    End Function

    Private Function UpdateModinfoRar() As Task(Of Integer)
        For Each item In LstArchives.Items
            Dim RarOutputFull As String
            Dim RarFile As String = item.ToString
            Dim RarProcess As New Process()
            Dim RarProcInfo As New ProcessStartInfo(RarPath, String.Format("lb {0}{1}{0}", """", RarFile)) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .CreateNoWindow = My.Settings.HidePopupWindow
            }

            RarProcess.StartInfo = RarProcInfo
            RarProcess.Start()

            Dim RarOutput As String
            Using sr As StreamReader = RarProcess.StandardOutput
                RarOutput = sr.ReadToEnd
            End Using

            If My.Settings.ShowConsoleOutput Then RtbOutput.Invoke(Sub()
                                                                       RarOutputFull = Regex.Replace(RarOutput, "[^\P{C}\n]+", String.Empty)
                                                                       RarOutputFull = Regex.Replace(RarOutputFull, "^[\s\t]*\r?\n", String.Empty, RegexOptions.Multiline).Trim
                                                                       WriteLn(Environment.NewLine & "----------rar.exe " & RarProcInfo.Arguments, False)
                                                                       WriteLn(RarOutputFull, False)
                                                                       WriteLn("----------end rar.exe----------" & Environment.NewLine, False)
                                                                   End Sub)

            Dim RarOutputLines As String() = RarOutput.Split(Environment.NewLine)

            For Each line As String In RarOutputLines
                If line.ToLower.Contains("modinfo.ini") Then

                    RtbOutput.Invoke(Sub()
                                         WriteLn(String.Format("{2}---------- Starting file {0}{1}{0} ----------", """", line, Environment.NewLine), False)
                                         WriteLn(String.Format("Extracting: {0}{1}{0}", """", line))
                                     End Sub)


                    ' line in this scenario doubles as both the file we are extracting and it's
                    ' corresponding filename on disk when extracted
                    RarProcInfo.Arguments = String.Format("x -o+ {0}{1}{0} {0}{2}{0} {0}{3}{0}", """", RarFile, line, ExePath)
                    RarProcess.Start()
                    RarProcess.WaitForExit()

                    Using sr As StreamReader = RarProcess.StandardOutput
                        RarOutput = sr.ReadToEnd
                    End Using
                    If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Or RarOutput.Contains("100%") Then
                        RtbOutput.Invoke(Sub()
                                             If My.Settings.ShowConsoleOutput Then
                                                 RarOutputFull = Regex.Replace(RarOutput, "[^\P{C}\n]+", String.Empty)
                                                 RarOutputFull = Regex.Replace(RarOutputFull, "^[\s\t]*\r?\n", String.Empty, RegexOptions.Multiline).Trim
                                                 WriteLn(Environment.NewLine & "----------rar.exe " & RarProcInfo.Arguments, False)
                                                 WriteLn(RarOutputFull, False)
                                                 WriteLn("----------end rar.exe----------" & Environment.NewLine, False)
                                             End If
                                             WriteLn("Extraction returned OK")
                                         End Sub)
                    Else
                        MessageBox.Show(RarOutput, "remac")
                    End If

                    RtbOutput.Invoke(Sub()
                                         WriteLn(String.Format("Updating author for: {0}{1}{0}", """", line))
                                     End Sub)

                    Dim Ini As New IniFileParser(line)
                    Ini.SetValue("author", TxtAuthor.Text)


                    RarProcInfo.Arguments = String.Format("m -r {0}{1}{0} {0}{2}{0}", """", RarFile, line)
                    RtbOutput.Invoke(Sub()
                                         WriteLn(String.Format("Adding updated {0}{1}{0} to archive", """", line))
                                     End Sub)
                    RarProcess.Start()
                    RarProcess.WaitForExit()

                    Using sr As StreamReader = RarProcess.StandardOutput
                        RarOutput = sr.ReadToEnd
                    End Using

                    If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Or RarOutput.Contains("100%") Then
                        RtbOutput.Invoke(Sub()
                                             If My.Settings.ShowConsoleOutput Then
                                                 RarOutputFull = Regex.Replace(RarOutput, "[^\P{C}\n]+", String.Empty)
                                                 RarOutputFull = Regex.Replace(RarOutputFull, "^[\s\t]*\r?\n", String.Empty, RegexOptions.Multiline).Trim
                                                 WriteLn(Environment.NewLine & "----------rar.exe " & RarProcInfo.Arguments, False)
                                                 WriteLn(RarOutputFull, False)
                                                 WriteLn("----------end rar.exe----------" & Environment.NewLine, False)
                                             End If
                                             WriteLn(String.Format("Adding {0}{1}{0} returned OK", """", line))
                                             WriteLn(String.Format("---------- Finished file {0}{1}{0} ----------", """", line), False)
                                         End Sub)
                    Else
                        MessageBox.Show(RarOutput, "remac")
                    End If

                    If (Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(line))) Then
                        Try
                            Directory.Delete(Path.GetDirectoryName(line))
                        Catch ex As Exception
                            RtbOutput.Invoke(Sub()
                                                 WriteLn("-----------------------")
                                                 WriteLn("ERROR: " & ex.Message)
                                                 WriteLn("-----------------------")
                                             End Sub)
                        End Try
                    End If
                End If
            Next
        Next
        Return Task.FromResult(0)
    End Function

    Private Sub WriteLn(ByVal text As String, Optional ByVal remac As Boolean = True)
        If remac Then
            RtbOutput.AppendText(DateTime.Now.ToString("HH:mm:ss") & " remac: " & text & Environment.NewLine)
        Else
            RtbOutput.AppendText(text & Environment.NewLine)
        End If
    End Sub

    Private Sub TxtAuthor_TextChanged(sender As Object, e As EventArgs) Handles TxtAuthor.TextChanged
        My.Settings.AuthorName = TxtAuthor.Text
    End Sub

    Private Sub TxtWinrar_TextChanged(sender As Object, e As EventArgs) Handles TxtWinrar.TextChanged
        My.Settings.WinrarPath = TxtWinrar.Text
        WinrarPath = TxtWinrar.Text
        RarPath = WinrarPath & "\rar.exe"
        If (Not File.Exists(RarPath)) Then
            btnSetAuthorAll.Enabled = False
            WriteLn("ERROR: Rar.exe not found at " & RarPath)
        Else
            btnSetAuthorAll.Enabled = True
        End If
    End Sub

    Private Sub ClearToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem1.Click
        RtbOutput.Clear()
    End Sub

    Private Sub AddFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFilesToolStripMenuItem.Click
        Try
            If OfdRars.ShowDialog = DialogResult.OK Then
                For Each s As String In OfdRars.FileNames
                    LstArchives.Items.Add(s)
                Next
            End If
        Catch ex As Exception
            WriteLn(ex.Message)
        End Try
    End Sub

    Private Sub TxtAuthor_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtAuthor.KeyDown
        If e.KeyCode = Keys.Enter Then
            If btnSetAuthorAll.Enabled Then
                btnSetAuthorAll.PerformClick()
            End If
        End If
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles BtnSettings.Click
        FrmSettings.ShowDialog()
    End Sub
End Class
