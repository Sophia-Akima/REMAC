Imports System.Drawing.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.IO.Compression
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
End Class
