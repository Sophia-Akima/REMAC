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
            ChangeModAuthors.Log(String.Format("UPDATEFILE detected, attempting update cleanup"))
            Dim UpdateClean = Await FinalizeUpdate()
        End If

        If My.Settings.AutoUpdate Then
            Dim UpdateResult = Await CheckForUpdate(ProgramVersion)
        End If

    End Sub

    Private Function FinalizeUpdate() As Task(Of Integer)
        Dim UpdateEXE As String = ExePath & "\REMAC_auto_updater.exe"
        Dim UpdateDLL As String = ExePath & "\REMAC_auto_updater.dll"
        Dim UpdateEXETMP As String = ExePath & "\REMAC_auto_updater.exe.tmp"
        Dim UpdateDLLTMP As String = ExePath & "\REMAC_auto_updater.dll.tmp"
        Dim UpdateEXEOLD As String = ExePath & "\REMAC_auto_updater.exe.old"
        Dim UpdateDLLOLD As String = ExePath & "\REMAC_auto_updater.dll.old"

        While IsProcessRunning("REMAC_auto_updater")

        End While

        Try
            ChangeModAuthors.Log("Moving old update executable")
            If File.Exists(UpdateEXE) Then File.Move(UpdateEXE, UpdateEXEOLD)
            ChangeModAuthors.Log("Replacing with a new executable")
            If File.Exists(UpdateEXETMP) Then File.Move(UpdateEXETMP, UpdateEXE)
            ChangeModAuthors.Log("Deleting old executable")
            If File.Exists(UpdateEXEOLD) Then File.Delete(UpdateEXEOLD)

            ChangeModAuthors.Log("Moving old update dll")
            If File.Exists(UpdateDLL) Then File.Move(UpdateDLL, UpdateDLLOLD)
            ChangeModAuthors.Log("Replacing with a new dll")
            If File.Exists(UpdateDLLTMP) Then File.Move(UpdateDLLTMP, UpdateDLL)
            ChangeModAuthors.Log("Deleting old dll")
            If File.Exists(UpdateDLLOLD) Then File.Delete(UpdateDLLOLD)

            ChangeModAuthors.Log("Deleting UPDATEFILE")
            If File.Exists(ExePath & "\UPDATEFILE") Then File.Delete(ExePath & "\UPDATEFILE")

        Catch ex As Exception
            MessageBox.Show(String.Format("An error has occured cleaning the update files.{0}{0}You may have to run as administrator.{0}{0}Alternatively you may clean the files yourself, just delete UPDATEFILE and replace AutoUpdater.exe with AutoUpdater.exe.tmp{0}{0}{1}", Environment.NewLine, ex.Message), "REMAC - Error")
            Return Task.FromResult(1)
        End Try

        ChangeModAuthors.Log("Update cleanup complete")
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
                Dim UpdateProcessInfo As New ProcessStartInfo("REMAC_auto_updater.exe", String.Format("{0} {1}", UpdateURL, ExePath))
                If Not File.Exists(ExePath & "\UPDATEFILE") Then File.Create(ExePath & "\UPDATEFILE")
                UpdateProcess.StartInfo = UpdateProcessInfo
                UpdateProcess.Start()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Function IsProcessRunning(processName As String) As Boolean
        Dim processes() As Process = Process.GetProcessesByName(processName)
        Return processes.Length > 0
    End Function
End Class
