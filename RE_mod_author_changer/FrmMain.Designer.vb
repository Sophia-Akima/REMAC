<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(FrmMain))
        TxtWinrar = New TextBox()
        Label1 = New Label()
        BtnBrowseWinrar = New Button()
        FbdWinrar = New FolderBrowserDialog()
        LstArchives = New ListBox()
        CmsLstArchives = New ContextMenuStrip(components)
        AddFilesToolStripMenuItem = New ToolStripMenuItem()
        ClearToolStripMenuItem = New ToolStripMenuItem()
        ClearAllToolStripMenuItem = New ToolStripMenuItem()
        TxtAuthor = New TextBox()
        Label2 = New Label()
        btnSetAuthorAll = New Button()
        RtbOutput = New RichTextBox()
        CmsRtbOutput = New ContextMenuStrip(components)
        ClearToolStripMenuItem1 = New ToolStripMenuItem()
        OfdRars = New OpenFileDialog()
        BtnSettings = New Button()
        CmsLstArchives.SuspendLayout()
        CmsRtbOutput.SuspendLayout()
        SuspendLayout()
        ' 
        ' TxtWinrar
        ' 
        TxtWinrar.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        TxtWinrar.Location = New Point(12, 27)
        TxtWinrar.Name = "TxtWinrar"
        TxtWinrar.Size = New Size(533, 23)
        TxtWinrar.TabIndex = 4
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(12, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(161, 15)
        Label1.TabIndex = 1
        Label1.Text = "WinRAR installation directory"
        ' 
        ' BtnBrowseWinrar
        ' 
        BtnBrowseWinrar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        BtnBrowseWinrar.Location = New Point(551, 27)
        BtnBrowseWinrar.Name = "BtnBrowseWinrar"
        BtnBrowseWinrar.Size = New Size(30, 23)
        BtnBrowseWinrar.TabIndex = 2
        BtnBrowseWinrar.Text = "..."
        BtnBrowseWinrar.UseVisualStyleBackColor = True
        ' 
        ' FbdWinrar
        ' 
        FbdWinrar.InitialDirectory = "C:\Program Files\"
        FbdWinrar.RootFolder = Environment.SpecialFolder.ProgramFiles
        FbdWinrar.SelectedPath = "C:\Program Files\"
        FbdWinrar.ShowNewFolderButton = False
        ' 
        ' LstArchives
        ' 
        LstArchives.AllowDrop = True
        LstArchives.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        LstArchives.ContextMenuStrip = CmsLstArchives
        LstArchives.FormattingEnabled = True
        LstArchives.ItemHeight = 15
        LstArchives.Location = New Point(12, 56)
        LstArchives.Name = "LstArchives"
        LstArchives.SelectionMode = SelectionMode.MultiExtended
        LstArchives.Size = New Size(598, 229)
        LstArchives.TabIndex = 3
        ' 
        ' CmsLstArchives
        ' 
        CmsLstArchives.Items.AddRange(New ToolStripItem() {AddFilesToolStripMenuItem, ClearToolStripMenuItem, ClearAllToolStripMenuItem})
        CmsLstArchives.Name = "CmsLstArchives"
        CmsLstArchives.Size = New Size(149, 70)
        ' 
        ' AddFilesToolStripMenuItem
        ' 
        AddFilesToolStripMenuItem.Name = "AddFilesToolStripMenuItem"
        AddFilesToolStripMenuItem.Size = New Size(148, 22)
        AddFilesToolStripMenuItem.Text = "Add Files"
        ' 
        ' ClearToolStripMenuItem
        ' 
        ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        ClearToolStripMenuItem.Size = New Size(148, 22)
        ClearToolStripMenuItem.Text = "Clear Selected"
        ' 
        ' ClearAllToolStripMenuItem
        ' 
        ClearAllToolStripMenuItem.Name = "ClearAllToolStripMenuItem"
        ClearAllToolStripMenuItem.Size = New Size(148, 22)
        ClearAllToolStripMenuItem.Text = "Clear All"
        ' 
        ' TxtAuthor
        ' 
        TxtAuthor.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        TxtAuthor.Location = New Point(12, 306)
        TxtAuthor.Name = "TxtAuthor"
        TxtAuthor.Size = New Size(517, 23)
        TxtAuthor.TabIndex = 0
        ' 
        ' Label2
        ' 
        Label2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Label2.AutoSize = True
        Label2.Location = New Point(12, 288)
        Label2.Name = "Label2"
        Label2.Size = New Size(102, 15)
        Label2.TabIndex = 5
        Label2.Text = "New author name"
        ' 
        ' btnSetAuthorAll
        ' 
        btnSetAuthorAll.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnSetAuthorAll.Location = New Point(535, 306)
        btnSetAuthorAll.Name = "btnSetAuthorAll"
        btnSetAuthorAll.Size = New Size(75, 23)
        btnSetAuthorAll.TabIndex = 6
        btnSetAuthorAll.Text = "Set All"
        btnSetAuthorAll.UseVisualStyleBackColor = True
        ' 
        ' RtbOutput
        ' 
        RtbOutput.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        RtbOutput.ContextMenuStrip = CmsRtbOutput
        RtbOutput.Font = New Font("NSimSun", 9.75F, FontStyle.Regular, GraphicsUnit.Point)
        RtbOutput.Location = New Point(12, 335)
        RtbOutput.Name = "RtbOutput"
        RtbOutput.ReadOnly = True
        RtbOutput.Size = New Size(598, 224)
        RtbOutput.TabIndex = 7
        RtbOutput.Text = ""
        ' 
        ' CmsRtbOutput
        ' 
        CmsRtbOutput.Items.AddRange(New ToolStripItem() {ClearToolStripMenuItem1})
        CmsRtbOutput.Name = "CmsRtbOutput"
        CmsRtbOutput.Size = New Size(102, 26)
        ' 
        ' ClearToolStripMenuItem1
        ' 
        ClearToolStripMenuItem1.Name = "ClearToolStripMenuItem1"
        ClearToolStripMenuItem1.Size = New Size(101, 22)
        ClearToolStripMenuItem1.Text = "Clear"
        ' 
        ' OfdRars
        ' 
        OfdRars.Filter = "RAR Archive (*.rar)|*.rar"
        OfdRars.Multiselect = True
        ' 
        ' BtnSettings
        ' 
        BtnSettings.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        BtnSettings.Image = CType(resources.GetObject("BtnSettings.Image"), Image)
        BtnSettings.Location = New Point(587, 27)
        BtnSettings.Name = "BtnSettings"
        BtnSettings.Size = New Size(23, 23)
        BtnSettings.TabIndex = 8
        BtnSettings.UseVisualStyleBackColor = True
        ' 
        ' FrmMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(622, 571)
        Controls.Add(BtnSettings)
        Controls.Add(RtbOutput)
        Controls.Add(btnSetAuthorAll)
        Controls.Add(Label2)
        Controls.Add(TxtAuthor)
        Controls.Add(LstArchives)
        Controls.Add(BtnBrowseWinrar)
        Controls.Add(Label1)
        Controls.Add(TxtWinrar)
        Name = "FrmMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "REMAC"
        CmsLstArchives.ResumeLayout(False)
        CmsRtbOutput.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents TxtWinrar As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnBrowseWinrar As Button
    Friend WithEvents FbdWinrar As FolderBrowserDialog
    Friend WithEvents LstArchives As ListBox
    Friend WithEvents CmsLstArchives As ContextMenuStrip
    Friend WithEvents ClearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TxtAuthor As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btnSetAuthorAll As Button
    Friend WithEvents RtbOutput As RichTextBox
    Friend WithEvents CmsRtbOutput As ContextMenuStrip
    Friend WithEvents ClearToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents AddFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OfdRars As OpenFileDialog
    Friend WithEvents BtnSettings As Button
End Class
