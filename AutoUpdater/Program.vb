Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Xml

Module Program
    Sub Main(args As String())
        Dim MyPath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
        Try
            Console.WriteLine("Updating REMAC")

            If args.Count < 1 Then
                Console.WriteLine("There are not enough arguments, closing REMAC auto updater")
                Console.ReadLine()
                Environment.Exit(0)
            End If

            Dim UpdateURL = args(0)
            Dim UpdatePath = args(1)
            Dim HttpClient As New HttpClient
            Dim Response As HttpResponseMessage = HttpClient.GetAsync(UpdateURL).Result
            Response.EnsureSuccessStatusCode()

            Using ContentStream As Stream = Response.Content.ReadAsStreamAsync().Result
                Using FS As New FileStream("update.zip", FileMode.Create, FileAccess.Write, FileShare.None)
                    ContentStream.CopyTo(FS)
                End Using
            End Using

            Console.WriteLine("update.zip downloaded, extracting")

            Using Archive As ZipArchive = ZipFile.Open("update.zip", ZipArchiveMode.Read)
                For Each ZipEntry In Archive.Entries

                    Dim ZipName As String = ZipEntry.FullName
                    Dim ZipPath As String = Path.GetFullPath(String.Format("{0}\{1}", MyPath, ZipEntry.FullName))
                    Dim ZipDir As String = Path.GetDirectoryName(ZipPath)

                    If Not Directory.Exists(ZipDir) Then
                        Directory.CreateDirectory(ZipDir)
                    End If

                    If Not ZipName.ToLower.Contains("autoupdater.exe") And Path.HasExtension(ZipName) Then
                        Console.WriteLine("Extracting " & String.Format("{0}\{1}", MyPath, ZipPath))
                        ZipEntry.ExtractToFile(ZipPath, True)
                    ElseIf ZipName.ToLower.Contains("autoupdater.exe") And Path.HasExtension(ZipName) Then
                        Dim TempExe As String = String.Format("{0}\{1}.tmp", MyPath, ZipPath)
                        Console.WriteLine(String.Format("Extracting {0}\{1}.tmp", MyPath, ZipPath))
                    End If
                Next
            End Using

            File.Delete("update.zip")

            Dim UpdateProcess As New Process()
            Dim UpdateProcessInfo As New ProcessStartInfo("RE_mod_author_changer.exe")
            UpdateProcess.StartInfo = UpdateProcessInfo
            UpdateProcess.Start()
            Environment.Exit(0)
            Console.WriteLine("Everything is complete aside from starting a new process")
            Console.ReadLine()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.ReadLine()
            Environment.Exit(0)
        End Try
    End Sub
End Module
