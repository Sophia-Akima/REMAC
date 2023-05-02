Imports System.Drawing.Text

Public Class Logger
    Public Shared Sub Log(FormMain As FrmMain, ByVal text As String, Optional ByVal remac As Boolean = True)

        If FormMain.InvokeRequired Then

            If remac Then
                FormMain.BeginInvoke(Sub()
                                         FormMain.RtbOutput.AppendText(DateTime.Now.ToString("HH:mm:ss") & " REMAC: " & text & Environment.NewLine)
                                     End Sub)
            Else
                FormMain.BeginInvoke(Sub()
                                         FormMain.RtbOutput.AppendText(text & Environment.NewLine)
                                     End Sub)
            End If

        Else

            If remac Then
                FormMain.RtbOutput.AppendText(DateTime.Now.ToString("HH:mm:ss") & " REMAC: " & text & Environment.NewLine)
            Else
                FormMain.RtbOutput.AppendText(text & Environment.NewLine)
            End If
        End If
    End Sub
End Class
