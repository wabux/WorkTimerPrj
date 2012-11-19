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
    Shared documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
    Shared session As Document.DocumentSession ' = CType(documentStore.OpenSession(), DocumentSession)
    Shared CurentWorkTime As WorkTime = New WorkTime
    Shared WithEvents Timer1 As System.Windows.Forms.Timer
    ' Shared Timer1 = New System.Windows.Forms.Timer

    Private Sub New()
    End Sub

    Private Shared notico As NotifyIcon

    '==========================================================================
    Public Shared Sub Main(astrArg As String())
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

    Private Sub MakeNewEntry(Optional ByVal StartEvent As String = "")
        CurentWorkTime = New WorkTime(DateTime.Now, StartEvent)
        CurentWorkTime.EndTime = DateAndTime.Now
        session.Store(CurentWorkTime)
        session.SaveChanges()
    End Sub

    Private Shared Sub ReadEndTimeEntry(Optional ByVal StartEvent As String = "")
        Dim CurentWorkTimeReadfromDB As Boolean = False

        CurentWorkTime.EndTime = DateTime.Now

        Dim results = From WTime In session.Query(Of WorkTime)() _
        .Take(1) _
        .OrderByDescending(Function(WTime) WTime.EndTime)
        '.Distinct _
        '.ToArray()

        For Each li As WorkTime In results
            Dim tspn As TimeSpan = New TimeSpan(li.EndTime.Ticks - li.StartTime.Ticks)
            CurentWorkTime = li
        Next
        If CurentWorkTime.EndTime.Date = DateAndTime.Now.Date Then
            CurentWorkTime.EndTime = DateAndTime.Now
            If StartEvent <> String.Empty Then CurentWorkTime.StartEvent = StartEvent
            If Not CurentWorkTimeReadfromDB Then session.Store(CurentWorkTime)
        ElseIf New TimeSpan(CurentWorkTime.EndTime.Date.Ticks - DateAndTime.Now.Date.Ticks).TotalDays >= 1 Then
            session.Delete(CurentWorkTime)
            CurentWorkTime = New WorkTime
            CurentWorkTime.EndTime = DateAndTime.Now
            CurentWorkTime.StartEvent = "overday"
        Else
            CurentWorkTime = New WorkTime
            CurentWorkTime.EndTime = DateAndTime.Now
            CurentWorkTime.StartEvent = "overnight"
        End If
        session.Store(CurentWorkTime)
        session.SaveChanges()
    End Sub

    Private Shared Sub SetEndTimeEntry(Optional ByVal StartEvent As String = "") '(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Dim CurentWorkTimeReadfromDB As Boolean = False
        CurentWorkTime.EndTime = DateTime.Now

        Dim results = From WTime In session.Query(Of WorkTime)() _
            .Take(1) _
            .OrderByDescending(Function(WTime) WTime.EndTime)
        '.Distinct _
        '.ToArray()

        For Each li As WorkTime In results
            CurentWorkTimeReadfromDB = True
        Next

        If CurentWorkTime.EndTime.Date = DateAndTime.Now.Date Then
            CurentWorkTime.EndTime = DateAndTime.Now
            If StartEvent <> String.Empty Then CurentWorkTime.StartEvent = StartEvent
            If Not CurentWorkTimeReadfromDB Then session.Store(CurentWorkTime)
        ElseIf New TimeSpan(CurentWorkTime.EndTime.Date.Ticks - DateAndTime.Now.Date.Ticks).TotalDays >= 1 Then
            session.Delete(CurentWorkTime)
            CurentWorkTime = New WorkTime
            CurentWorkTime.EndTime = DateAndTime.Now
            CurentWorkTime.StartEvent = "overday"
        Else
            CurentWorkTime = New WorkTime
            CurentWorkTime.EndTime = DateAndTime.Now
            CurentWorkTime.StartEvent = "overnight"
        End If
        session.Store(CurentWorkTime)
        session.SaveChanges()
    End Sub

    Private Sub OnPowerModeChanged(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)
        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                Me.MakeNewEntry("Resume")
                Timer1.Enabled = True

            Case Microsoft.Win32.PowerModes.Suspend
                MyNotifyIconApplication.SetEndTimeEntry("Suspend")
                Timer1.Enabled = False

            Case Microsoft.Win32.PowerModes.StatusChange
                MyNotifyIconApplication.SetEndTimeEntry("StatusChange")
                Timer1.Enabled = False

        End Select
    End Sub

    Private Shared Sub TimerTick(sender As System.Object, e As System.EventArgs)
        MyNotifyIconApplication.SetEndTimeEntry()
        MyNotifyIconApplication.notico.Text = String.Format("{0:hh}:{0:mm}", CurentWorkTime.WorkingHours)
    End Sub
End Class