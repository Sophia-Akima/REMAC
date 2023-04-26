Public Class FrmSettings
    Private Sub FrmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CbHidePopupWindow.Checked = My.Settings.HidePopupWindow
        CbShowConsoleOutput.Checked = My.Settings.ShowConsoleOutput
        CbCheckForUpdates.Checked = My.Settings.AutoUpdate
        If My.Settings.RarProcessTimeout >= 1000 Then NumRarTimeout.Value = My.Settings.RarProcessTimeout
    End Sub

    Private Sub SaveSettings()
        My.Settings.HidePopupWindow = CbHidePopupWindow.Checked
        My.Settings.ShowConsoleOutput = CbShowConsoleOutput.Checked
        My.Settings.RarProcessTimeout = NumRarTimeout.Value
        My.Settings.AutoUpdate = CbCheckForUpdates.Checked
    End Sub
    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        SaveSettings()
    End Sub

    Private Sub BtnSaveAndClose_Click(sender As Object, e As EventArgs) Handles BtnSaveAndClose.Click
        SaveSettings()
        Me.Close()
    End Sub

    Private Sub BtnCloseWithoutSave_Click(sender As Object, e As EventArgs) Handles BtnCloseWithoutSave.Click
        Me.Close()
    End Sub
End Class