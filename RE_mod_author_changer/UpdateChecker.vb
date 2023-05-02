Imports System.IO
Imports System.Xml

Public Class UpdateChecker
    Public Shared Function CheckForUpdate(ByVal ProgramVersion As String)
        Try
            Dim OctoClient As New Octokit.GitHubClient(New Octokit.ProductHeaderValue("REMAC"))
            Dim Releases As IReadOnlyList(Of Octokit.Release) = OctoClient.Repository.Release.GetAll("Sophia-Akima", "REMAC").Result
            Dim NewestRelease As Octokit.Release = Releases(0)
            Dim UpdateURL = NewestRelease.Assets(0).BrowserDownloadUrl

            If Not ProgramVersion.Equals(NewestRelease.TagName) Then
                If MessageBox.Show(String.Format("Update available from version {0} to {1} from Github. Would you like to automatically update now?",
                                              ProgramVersion, NewestRelease.TagName), "REMAC", MessageBoxButtons.OKCancel) = DialogResult.OK Then
                    Return UpdateURL
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return False
    End Function

    Public Shared Function IsValidURL(ByVal url As String) As Boolean
        Return Uri.IsWellFormedUriString(url, UriKind.Absolute)
    End Function

    Public Shared Function FinalizeUpdate(FormMain As FrmMain)
        Dim ExePath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
        Dim UpdateEXE As String = Path.Combine(ExePath, "REMAC_auto_updater.exe")
        Dim UpdateDLL As String = Path.Combine(ExePath, "REMAC_auto_updater.dll")
        Dim UpdateEXETMP As String = Path.Combine(ExePath, "REMAC_auto_updater.exe.tmp")
        Dim UpdateDLLTMP As String = Path.Combine(ExePath, "REMAC_auto_updater.dll.tmp")
        Dim UpdateEXEOLD As String = Path.Combine(ExePath, "REMAC_auto_updater.exe.old")
        Dim UpdateDLLOLD As String = Path.Combine(ExePath, "REMAC_auto_updater.dll.old")

        Try
            Logger.Log(FormMain, "Moving old update executable")
            If File.Exists(UpdateEXE) Then File.Move(UpdateEXE, UpdateEXEOLD)
            Logger.Log(FormMain, "Replacing with a new executable")
            If File.Exists(UpdateEXETMP) Then File.Move(UpdateEXETMP, UpdateEXE)
            Logger.Log(FormMain, "Deleting old executable")
            If File.Exists(UpdateEXEOLD) Then File.Delete(UpdateEXEOLD)

            Logger.Log(FormMain, "Moving old update dll")
            If File.Exists(UpdateDLL) Then File.Move(UpdateDLL, UpdateDLLOLD)
            Logger.Log(FormMain, "Replacing with a new dll")
            If File.Exists(UpdateDLLTMP) Then File.Move(UpdateDLLTMP, UpdateDLL)
            Logger.Log(FormMain, "Deleting old dll")
            If File.Exists(UpdateDLLOLD) Then File.Delete(UpdateDLLOLD)

            Logger.Log(FormMain, "Deleting UPDATEFILE")
            If File.Exists(ExePath & "\UPDATEFILE") Then File.Delete(ExePath & "\UPDATEFILE")
        Catch ex As Exception
            MessageBox.Show(String.Format("An error has occured cleaning the update files.{0}{0}You may have to run as administrator.{0}{0}Alternatively you may clean the files yourself, just delete UPDATEFILE and replace AutoUpdater.exe with AutoUpdater.exe.tmp{0}{0}{1}", Environment.NewLine, ex.Message), "REMAC - Error")
        End Try

        Logger.Log(FormMain, "Update cleanup complete")
    End Function

End Class
