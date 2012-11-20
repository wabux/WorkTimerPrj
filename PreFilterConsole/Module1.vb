Imports System.IO
Imports System.Linq
Imports System.Drawing
Imports Microsoft.Win32
Imports System.Threading
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports System.Security.Permissions

Imports Raven.Client
Imports Raven.Client.Document
Imports Raven.Client.Indexes
Imports Raven.Client.Embedded
Imports Raven.Storage.Esent
Imports Raven.Json.Linq
Imports Raven.Abstractions.Indexing
Imports Raven.Abstractions.Commands


Module Module1
    'Implements IMessageFilter

    Private documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
    Private session As Document.DocumentSession ' = CType(documentStore.OpenSession(), DocumentSession)
    Private CurentWorkTime As WorkTime = New WorkTime
    Private WithEvents Timer1 As System.Windows.Forms.Timer
    Private _text As String
    Private notico As NotifyIcon
    'Shared MyNotifyIconApplication As IMessageFilter


    Sub Main()
        Dim cm As ContextMenu
        Dim miCurr As MenuItem
        Dim iIndex As Integer = 0

        documentStore.DataDirectory = Path.Combine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData, "DB")
        documentStore.Initialize()
        session = CType(documentStore.OpenSession(), DocumentSession)
        ReadEndTimeEntry()

        ' Dim Timer1 As System.Windows.Forms.Timer
        Timer1 = New System.Windows.Forms.Timer
        Timer1.Interval = 30000
        AddHandler Timer1.Tick, New System.EventHandler(AddressOf TimerTick)

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
        notico.Text = String.Format("{0:hh}:{0:mm}", CurentWorkTime.WorkingHours)
        ' Eigenen Text einsetzen
        notico.Visible = True
        notico.ContextMenu = cm
        AddHandler notico.DoubleClick, New EventHandler(AddressOf NotifyIconDoubleClick)

        'AddHandler Application.on
        Timer1.Start()
        ' Ohne Appplication.Run geht es nicht
        Application.AddMessageFilter(MeasureItemEventArgs.PreFilterMessage)  'MyNotifyIconApplication.

        Application.Run()

        MyNotifyIconApplication.Closing()


    End Sub

End Module
