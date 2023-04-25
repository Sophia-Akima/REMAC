Public Class FrmSettings
    Private Sub FrmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CbHidePopupWindow.Checked = My.Settings.HidePopupWindow
        CbShowConsoleOutput.Checked = My.Settings.ShowConsoleOutput
    End Sub

    Private Sub SaveSettings()
        My.Settings.HidePopupWindow = CbHidePopupWindow.Checked
        My.Settings.ShowConsoleOutput = CbShowConsoleOutput.Checked
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