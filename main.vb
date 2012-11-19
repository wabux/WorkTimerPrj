Imports System.IO
Imports System.Linq
Imports System.Drawing
Imports Microsoft.Win32
Imports System.Threading
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Threading.Tasks

Imports Raven.Client
Imports Raven.Client.Document
Imports Raven.Client.Indexes
Imports Raven.Client.Embedded
Imports Raven.Storage.Esent
Imports Raven.Json.Linq
Imports Raven.Abstractions.Indexing
Imports Raven.Abstractions.Commands


'*****************************************************************************
NotInheritable Class MyNotifyIconApplication
    Dim documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
    Dim session As Document.DocumentSession = CType(documentStore.OpenSession(), DocumentSession)

    Private Sub New()
    End Sub
    Private Shared notico As NotifyIcon
    'test
    '==========================================================================
    Public Shared Sub Main(astrArg As String())
        Dim cm As ContextMenu
        Dim miCurr As MenuItem
        Dim iIndex As Integer = 0

        ' Kontextmenü erzeugen
        cm = New ContextMenu()

        ' Kontextmenüeinträge erzeugen
        miCurr = New MenuItem()
        miCurr.Index = System.Math.Max(System.Threading.Interlocked.Increment(iIndex), iIndex - 1)
        miCurr.Text = "&Aktion 1"
        ' Eigenen Text einsetzen
        AddHandler miCurr.Click, New System.EventHandler(AddressOf Action1Click)
        cm.MenuItems.Add(miCurr)

        ' Kontextmenüeinträge erzeugen
        miCurr = New MenuItem()
        miCurr.Index = System.Math.Max(System.Threading.Interlocked.Increment(iIndex), iIndex - 1)
        miCurr.Text = "&Beenden"
        AddHandler miCurr.Click, New System.EventHandler(AddressOf ExitClick)
        cm.MenuItems.Add(miCurr)

        ' NotifyIcon selbst erzeugen
        notico = New NotifyIcon()
        'notico.Icon = New Icon("Reminder.ico")
        'http://www.attilan.com/2006/08/accessing-embedded-resources-using.html
        Dim _assembly As Assembly = Assembly.GetExecutingAssembly
        Dim arrs() As String = _assembly.GetManifestResourceNames()
        Dim s As System.IO.Stream = _assembly.GetManifestResourceStream("WorkTimerPrj.Reminder.ico")
        notico.Icon = New Icon(_assembly.GetManifestResourceStream("WorkTimerPrj.Reminder.ico")) 'NotifyIconApplication
        ' Eigenes Icon einsetzen
        notico.Text = "Doppelklick mich!"
        ' Eigenen Text einsetzen
        notico.Visible = True
        notico.ContextMenu = cm
        AddHandler notico.DoubleClick, New EventHandler(AddressOf NotifyIconDoubleClick)

        'AddHandler Application.on

        ' Ohne Appplication.Run geht es nicht
        Application.Run()

        MessageBox.Show("Quit")

    End Sub

    Private Sub Starting()
        'Dim documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
        'MsgBox(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
        documentStore.DataDirectory = Path.Combine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData, "DB")
        'Using documentStore = New DocumentStore() With {.Url = "http://localhost:8080", .DefaultDatabase = "Test"}
        documentStore.Initialize()

    End Sub

    '==========================================================================
    Private Sub Closing(sender As [Object], e As EventArgs)
        MessageBox.Show("Quit")
        session.Dispose()
        documentStore.Dispose()
    End Sub

    '==========================================================================
    Private Shared Sub ExitClick(sender As [Object], e As EventArgs)
        notico.Dispose()
        Application.[Exit]()
    End Sub

    '==========================================================================
    Private Shared Sub Action1Click(sender As [Object], e As EventArgs)
        ' nur als Beispiel:
        ' new MyForm ().Show ();
    End Sub

    '==========================================================================
    Private Shared Sub NotifyIconDoubleClick(sender As [Object], e As EventArgs)
        ' Was immer du willst
    End Sub
End Class