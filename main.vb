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



'*****************************************************************************
NotInheritable Class MyNotifyIconApplication
    Implements IMessageFilter

    Shared documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
    Shared session As Document.DocumentSession ' = CType(documentStore.OpenSession(), DocumentSession)
    Shared CurentWorkTime As WorkTime = New WorkTime
    Shared WithEvents Timer1 As System.Windows.Forms.Timer
    Shared _text As Object
    'Shared MyNotifyIconApplication As IMessageFilter


    Private Sub New()
    End Sub

    Private Shared notico As NotifyIcon

    Private Shared Property text
        Get
            Return notico.Text
        End Get
        Set(value)
            _text = value
            notico.Text = String.Format("{0:hh}:{0:mm}", _text)
        End Set
    End Property


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
        Application.AddMessageFilter(MeasureItemEventArgs.PreFilterMessage)  'MyNotifyIconApplication.

        Application.Run()

        MyNotifyIconApplication.Closing()

    End Sub

    Private Sub Starting()
        'Dim documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
        'MsgBox(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
        documentStore.DataDirectory = Path.Combine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData, "DB")
        'Using documentStore = New DocumentStore() With {.Url = "http://localhost:8080", .DefaultDatabase = "Test"}
        documentStore.Initialize()

    End Sub

    '==========================================================================
    Private Shared Sub Closing()
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


    <SecurityPermission(SecurityAction.Demand)> _
    Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean Implements System.Windows.Forms.IMessageFilter.PreFilterMessage

        'http://msdn.microsoft.com/en-us/library/windows/desktop/ms646360(v=vs.85).aspx

        Const WM_NCLBUTTONDOWN As Integer = &HA1
        Const WM_LBUTTONDBLCLK As Integer = &H203
        Const WM_SYSCOMMAND As Integer = &H112
        Const SC_RESTORE As Integer = &HF120
        Const SC_SCREENSAVE As Integer = &HF140
        Const WM_KEYDOWN As Integer = &H100
        Const SC_MAXIMIZE As Integer = &HF030
        Const KEY_PRESSED As Integer = &H1000
        Const SC_HOTKEY As Integer = &HF150
        Const SC_MONITORPOWER As Integer = &HF170


        'WM_LBUTTONDOWN, WM_LBUTTONUP, WM_LBUTTONDOWN, WM_LBUTTONUP
        If m.Msg = WM_NCLBUTTONDOWN Then
            Select Case m.WParam.ToInt32()
                Case WM_LBUTTONDBLCLK
                    MyNotifyIconApplication.text = "WM_LBUTTONDBLCLK"
            End Select

        End If



        If (m.Msg = KEY_PRESSED) Then
            Select Case m.WParam.ToInt32()
                Case Keys.F2
                    MyNotifyIconApplication.text = "Keys.F2xx"
            End Select
        End If

        If (m.Msg = WM_KEYDOWN) Then
            Select Case m.WParam.ToInt32()
                Case Keys.F2
                    '  Me.Text = "Keys.F2"
            End Select
        End If


        'Make sure you check the wParam or lParam (i forget which one)... 
        'it is more likely that your application DOES receive WM_SYSCOMMAND but you 
        '        don() 't decode the SC_SCREENSAVE correctly. 
        'Check the docs - the lower 4 bits of the parameter are used internally, so 
        'you must do 
        'case WM_SYSCOMMAND: 
        '  if (wParam & 0xfff0 == SC_SCREENSAVE) 
        '  { 
        '    // whatever 
        '  } 
        '  break; 
        If m.Msg = WM_SYSCOMMAND And (m.WParam.ToInt32() And &HFFF0) = SC_MONITORPOWER Then
            MyNotifyIconApplication.text = "SC_MONITORPOWER"
        End If

        If m.Msg = WM_SYSCOMMAND And ((m.WParam.ToInt32 And &HFFF0) = SC_SCREENSAVE) Then
            MyNotifyIconApplication.text = "SC_SCREENSAVE"
        End If

        If m.Msg = WM_SYSCOMMAND Then
            Select Case m.WParam.ToInt32
                Case SC_SCREENSAVE
                    MyNotifyIconApplication.text = "SC_SCREENSAVE"

                Case SC_RESTORE
                    MyNotifyIconApplication.text = "SC_RESTORE"

                Case SC_MAXIMIZE
                    MyNotifyIconApplication.text = "SC_MAXIMIZE"

                Case SC_HOTKEY
                    MyNotifyIconApplication.text = "SC_HOTKEY"

                Case SC_MONITORPOWER
                    MyNotifyIconApplication.text = "SC_MONITORPOWER"

            End Select
        End If

        If m.Msg = SC_SCREENSAVE Then
            MyNotifyIconApplication.text = "SC_SCREENSAVE"
        End If


        Select Case m.Msg
            Case WM_SYSCOMMAND
                If m.WParam.ToInt32 = SC_SCREENSAVE Then
                    MyNotifyIconApplication.text = "SC_SCREENSAVE"
                    Return True
                End If

                'Handled := TRUE; // disable startup of screensavers
        End Select
        Return False
    End Function


    Protected Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Const SC_RESTORE As Integer = &HF120
        Const SC_SCREENSAVE As Integer = &HF140
        Const WM_SYSCOMMAND As Integer = &H112, SC_MONITORPOWER As Integer = &HF170
        Dim m_needToClose As Boolean = False

        If m.Msg = WM_SYSCOMMAND Then
            'Intercept System Command

            ' notice the 0xFFF0 mask, it's because the system can use the 4 low order bits of the wParam value as stated in the MSDN library article about WM_SYSCOMMAND.


            If (m.WParam.ToInt32() And &HFFF0) = SC_MONITORPOWER Then

                'Intercept Monitor Power Message
                Me.text = "SC_SCREENSAVE"
                m_needToClose = True

            End If
        End If

        'MyBase.WndProc(m)

        If m.Msg = WM_SYSCOMMAND Then
            Select Case m.WParam.ToInt32
                Case SC_SCREENSAVE
                    Me.text = "SC_SCREENSAVE"

            End Select
        End If
        'MyBase.WndProc(m)

    End Sub

End Class