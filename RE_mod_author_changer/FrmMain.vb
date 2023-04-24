Imports System.Drawing.Text
Imports System.IO
Imports MadMilkman.Ini
Public Class FrmMain
    Private ExePath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
    Private WinrarPath As String
    Private RarPath As String
    Private IniPath As String = ExePath & "\modinfo.ini"
    Private TestRar As String = ExePath & "\test.rar"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TxtWinrar.Text = My.Settings.WinrarPath
        TxtAuthor.Text = My.Settings.AuthorName
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

    Private Sub btnSetAuthorAll_Click(sender As Object, e As EventArgs) Handles btnSetAuthorAll.Click

        My.Settings.Save()
        btnSetAuthorAll.Enabled = False
        BtnBrowseWinrar.Enabled = False
        LstArchives.Enabled = False
        TxtWinrar.Enabled = False

        For Each item In LstArchives.Items
            Dim RarFile As String = item.ToString
            Dim RarProcess As New Process()
            Dim RarProcInfo As New ProcessStartInfo(RarPath, String.Format("lb {0}{1}{0}", """", RarFile)) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True
            }

            RarProcess.StartInfo = RarProcInfo
            RarProcess.Start()

            Dim RarOutput As String
            Using sr As StreamReader = RarProcess.StandardOutput
                RarOutput = sr.ReadToEnd
            End Using

            Dim RarOutputLines As String() = RarOutput.Split(Environment.NewLine)

            For Each line As String In RarOutputLines
                If line.ToLower.Contains("modinfo.ini") Then

                    WriteLn("extracting: " & line)

                    ' line in this scenario doubles as both the file we are extracting and it's
                    ' corresponding filename on disk

                    RarProcInfo.Arguments = String.Format("x -o+ {0}{1}{0} {0}{2}{0} {0}{3}{0}", """", RarFile, line, ExePath)
                    RarProcess.Start()
                    RarProcess.WaitForExit()

                    Using sr As StreamReader = RarProcess.StandardOutput
                        RarOutput = sr.ReadToEnd
                    End Using
                    If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Then
                        WriteLn("extraction returned OK")
                    Else
                        MessageBox.Show(RarOutput, "remac")
                    End If

                    WriteLn("updating author: " & line)

                    Dim Ini As New IniFileParser(line)
                    Ini.SetValue("author", TxtAuthor.Text)


                    RarProcInfo.Arguments = String.Format("m -r {0}{1}{0} {0}{2}{0}", """", RarFile, line)
                    WriteLn("adding updated modinfo.ini to archive")
                    RarProcess.Start()
                    RarProcess.WaitForExit()

                    Using sr As StreamReader = RarProcess.StandardOutput
                        RarOutput = sr.ReadToEnd
                    End Using
                    If RarOutput.Contains("OK") Or RarOutput.Contains("Done") Then
                        WriteLn("extraction returned OK")
                    Else
                        MessageBox.Show(RarOutput, "remac")
                    End If

                    If (Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(line))) Then
                        Try
                            Directory.Delete(Path.GetDirectoryName(line))
                        Catch ex As Exception
                            WriteLn("ERROR: " & ex.Message)
                        End Try
                    End If
                End If
            Next
        Next

        TxtWinrar.Enabled = True
        btnSetAuthorAll.Enabled = True
        BtnBrowseWinrar.Enabled = True
        LstArchives.Enabled = True
    End Sub
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
End Class
