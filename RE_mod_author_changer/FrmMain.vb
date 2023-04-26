Imports System.Drawing.Text
Imports System.IO
Imports System.IO.Compression
Imports System.Threading.Tasks
Imports System.Net.Http

Public Class FrmMain
    Private ExePath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
    Private WinrarPath As String
    Private RarPath As String
    Private IniPath As String
    Private ReadyForUpdate As Boolean = False
    Private UpdateURL As String

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim ProgramVersion As String = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion

        Me.Text = String.Format("{0} {1}", Me.Text, ProgramVersion)
        TxtWinrar.Text = My.Settings.WinrarPath
        TxtAuthor.Text = My.Settings.AuthorName
        If (String.IsNullOrEmpty(TxtWinrar.Text)) Then
            btnSetAuthorAll.Enabled = False
        End If

        If File.Exists(ExePath & "\UPDATEFILE") Then
            Dim UpdateClean = Await FinalizeUpdate()
        End If

        If My.Settings.AutoUpdate Then
            Dim UpdateResult = Await CheckForUpdate(ProgramVersion)
        End If

    End Sub

    Private Function FinalizeUpdate() As Task(Of Integer)
        Dim UpdateEXE As String = ExePath & "\AutoUpdater.exe"
        Try
            While ProgramIsRunning(UpdateEXE)
                'this is probably a terrible thing to do
                'but I really don't know or care
            End While
        Catch ex As Exception
            MessageBox.Show(String.Format("An error has occured cleaning the update files.{0}{0}You may have to run as administrator.{0}{0}Alternatively you may clean the files yourself, just delete UPDATEFILE and replace AutoUpdater.exe with AutoUpdater.exe.tmp{0}{0}{1}", Environment.NewLine, ex.Message), "REMAC - Error")
            Return Task.FromResult(1)
        End Try


        Try
            MoveFile(UpdateEXE, UpdateEXE & ".old")
            MoveFile(UpdateEXE & ".tmp", UpdateEXE)
            File.Delete(UpdateEXE & ".old")
            File.Delete(ExePath & "\UPDATEFILE")
            Return Task.FromResult(0)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return Task.FromResult(1)
        End Try
        Return Task.FromResult(0)
    End Function

    Private Function CheckForUpdate(ByVal ProgramVersion As String) As Task(Of Integer)
        Try
            Dim OctoClient As New Octokit.GitHubClient(New Octokit.ProductHeaderValue("REMAC"))
            Dim Releases As IReadOnlyList(Of Octokit.Release) = OctoClient.Repository.Release.GetAll("Sophia-Akima", "REMAC").Result
            Dim NewestRelease As Octokit.Release = Releases(0)
            UpdateURL = NewestRelease.Assets(0).BrowserDownloadUrl

            If Not ProgramVersion.Equals(NewestRelease.TagName) Then
                If MessageBox.Show(String.Format("Update available from version {0} to {1} from Github. Would you like to automatically update now?",
                                              ProgramVersion, NewestRelease.TagName), "REMAC", MessageBoxButtons.OKCancel) = DialogResult.OK Then
                    ReadyForUpdate = True
                    Me.Close()
                End If
            End If
        Catch ex As Exception

        End Try
        Return Task.FromResult(0)
    End Function

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
            If (Path.GetExtension(f) = ".rar" Or Path.GetExtension(f) = ".zip") Then
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

        Dim ChangeModAuthors As New ChangeModAuthors(RarPath)

        For Each item In LstArchives.Items
            Select Case Path.GetExtension(item)
                Case ".rar"
                    Dim result = Await ChangeModAuthors.Rar(item)
                Case ".zip"
                    Dim result = Await ChangeModAuthors.Zip(item)
                Case Else
                    MessageBox.Show("unknown extension type")
            End Select
        Next

        TxtWinrar.Enabled = True
        btnSetAuthorAll.Enabled = True
        BtnBrowseWinrar.Enabled = True
        BtnSettings.Enabled = True
        LstArchives.Enabled = True
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
            ChangeModAuthors.Log("ERROR: Rar.exe not found at " & RarPath)
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
            ChangeModAuthors.Log(ex.Message)
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

    Private Sub FrmMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If ReadyForUpdate Then
            Try
                Dim UpdateProcess As New Process()
                Dim UpdateProcessInfo As New ProcessStartInfo("AutoUpdater.exe", String.Format("{0} {1}", UpdateURL, ExePath))
                If Not File.Exists(ExePath & "\UPDATEFILE") Then File.Create(ExePath & "\UPDATEFILE")
                UpdateProcess.StartInfo = UpdateProcessInfo
                UpdateProcess.Start()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Function ProgramIsRunning(FullPath As String) As Boolean
        Dim FilePath As String = Path.GetDirectoryName(FullPath)
        Dim FileName As String = Path.GetFileNameWithoutExtension(FullPath).ToLower()
        Dim isRunning As Boolean = False

        Dim pList As Process() = Process.GetProcessesByName(FileName)

        For Each p As Process In pList
            If p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase) Then
                isRunning = True
                Exit For
            End If
        Next

        Return isRunning
    End Function

    Private Async Sub MoveFile(sourceFile As String, destinationFile As String)
        Try
            Using sourceStream As FileStream = File.Open(sourceFile, FileMode.Open)
                Using destinationStream As FileStream = File.Create(destinationFile)
                    Await sourceStream.CopyToAsync(destinationStream)
                    sourceStream.Close()
                    File.Delete(sourceFile)
                End Using
            End Using
        Catch ioex As IOException
            MessageBox.Show("An IOException occured during move, " & ioex.Message)
        Catch ex As Exception
            MessageBox.Show("An Exception occured during move, " & ex.Message)
        End Try
    End Sub
End Class
