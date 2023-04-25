<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        CbHidePopupWindow = New CheckBox()
        BtnSaveAndClose = New Button()
        BtnSave = New Button()
        BtnCloseWithoutSave = New Button()
        CbShowConsoleOutput = New CheckBox()
        SuspendLayout()
        ' 
        ' CbHidePopupWindow
        ' 
        CbHidePopupWindow.AutoSize = True
        CbHidePopupWindow.Location = New Point(12, 12)
        CbHidePopupWindow.Name = "CbHidePopupWindow"
        CbHidePopupWindow.Size = New Size(198, 19)
        CbHidePopupWindow.TabIndex = 0
        CbHidePopupWindow.Text = "Hide CMD popups (much faster)"
        CbHidePopupWindow.UseVisualStyleBackColor = True
        ' 
        ' BtnSaveAndClose
        ' 
        BtnSaveAndClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        BtnSaveAndClose.Location = New Point(12, 181)
        BtnSaveAndClose.Name = "BtnSaveAndClose"
        BtnSaveAndClose.Size = New Size(197, 23)
        BtnSaveAndClose.TabIndex = 1
        BtnSaveAndClose.Text = "Save and close"
        BtnSaveAndClose.UseVisualStyleBackColor = True
        ' 
        ' BtnSave
        ' 
        BtnSave.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        BtnSave.Location = New Point(12, 152)
        BtnSave.Name = "BtnSave"
        BtnSave.Size = New Size(197, 23)
        BtnSave.TabIndex = 2
        BtnSave.Text = "Save"
        BtnSave.UseVisualStyleBackColor = True
        ' 
        ' BtnCloseWithoutSave
        ' 
        BtnCloseWithoutSave.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        BtnCloseWithoutSave.Location = New Point(12, 210)
        BtnCloseWithoutSave.Name = "BtnCloseWithoutSave"
        BtnCloseWithoutSave.Size = New Size(197, 23)
        BtnCloseWithoutSave.TabIndex = 3
        BtnCloseWithoutSave.Text = "Close without saving"
        BtnCloseWithoutSave.UseVisualStyleBackColor = True
        ' 
        ' CbShowConsoleOutput
        ' 
        CbShowConsoleOutput.AutoSize = True
        CbShowConsoleOutput.Location = New Point(12, 37)
        CbShowConsoleOutput.Name = "CbShowConsoleOutput"
        CbShowConsoleOutput.Size = New Size(153, 19)
        CbShowConsoleOutput.TabIndex = 4
        CbShowConsoleOutput.Text = "Show all console output"
        CbShowConsoleOutput.UseVisualStyleBackColor = True
        ' 
        ' FrmSettings
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(221, 238)
        Controls.Add(CbShowConsoleOutput)
        Controls.Add(BtnCloseWithoutSave)
        Controls.Add(BtnSave)
        Controls.Add(BtnSaveAndClose)
        Controls.Add(CbHidePopupWindow)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        MaximizeBox = False
        Name = "FrmSettings"
        StartPosition = FormStartPosition.CenterParent
        Text = "REMAC Settings"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents CbHidePopupWindow As CheckBox
    Friend WithEvents BtnSaveAndClose As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents BtnCloseWithoutSave As Button
    Friend WithEvents CbShowConsoleOutput As CheckBox
End Class
