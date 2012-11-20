Imports System.Linq
Imports Raven.Abstractions.Indexing
Imports Raven.Client
Imports Raven.Client.Document
Imports Raven.Client.Indexes
Imports Raven.Client.Embedded
Imports Raven.Storage.Esent
Imports Raven.Json.Linq
Imports Raven.Abstractions.Commands
Imports System.IO
Imports Microsoft.Win32
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Reflection
Imports System.Security.Permissions




'IEnumerable<InstallationSummary> installationSummaries =
'  QueryAndCacheEtags(session => session.Advanced.LuceneQuery<InstallationSummary>()
'  .Include(x => x.ApplicationServerId)
'  .Include(x => x.ApplicationWithOverrideVariableGroup.ApplicationId)
'  .Include(x => x.ApplicationWithOverrideVariableGroup.CustomVariableGroupId)
'  .OrderByDescending(summary => summary.InstallationStart)
'  .Take(numberToRetrieve)).Cast<InstallationSummary>().ToList();

'Dim installationSummaries As IEnumerable(Of InstallationSummary) = QueryAndCacheEtags(Function(session) session.Advanced.LuceneQuery(Of InstallationSummary)().Include(Function(x) x.ApplicationServerId).Include(Function(x) x.ApplicationWithOverrideVariableGroup.ApplicationId).Include(Function(x) x.ApplicationWithOverrideVariableGroup.CustomVariableGroupId).OrderByDescending(Function(summary) summary.InstallationStart).Take(numberToRetrieve)).Cast(Of InstallationSummary)().ToList()
<SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
Public Class frmWindow
    Implements IMessageFilter

    Dim CurentWorkTime As WorkTime = New WorkTime

    Dim myFactory As New Tasks.TaskFactory(TaskCreationOptions.AttachedToParent Or TaskCreationOptions.PreferFairness, _
                                           TaskContinuationOptions.ExecuteSynchronously Or TaskContinuationOptions.PreferFairness)

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles btnAnzeigen.Click
        console.Clear()
        Me.ShowEntrys()
    End Sub


    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        console.Clear()
        Task.Factory.StartNew(Sub()
                                  Me.SetEndTimeEntry()
                              End Sub)
    End Sub

    Private Sub SetEndTimeEntry(Optional ByVal StartEvent As String = "") '(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Dim CurentWorkTimeReadfromDB As Boolean = False
        Using documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
            'MsgBox(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
            documentStore.DataDirectory = Path.Combine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData, "DB")
            'Using documentStore = New DocumentStore() With {.Url = "http://localhost:8080", .DefaultDatabase = "Test"}
            documentStore.Initialize()

            Using session As Document.DocumentSession = CType(documentStore.OpenSession(), DocumentSession)
                CurentWorkTime.EndTime = DateTime.Now
                'session.Store(CurentWorkTime)
                'session.SaveChanges()

                'Dim results = From WTime In session.Query(Of WorkTime)() Where (WTime.EndTime > DateTime.Now.AddMinutes(-1))

                Dim results = From WTime In session.Query(Of WorkTime)() _
                .Take(1) _
                .OrderByDescending(Function(WTime) WTime.EndTime)
                '.Distinct _
                '.ToArray()

                For Each li As WorkTime In results
                    Dim tspn As TimeSpan = New TimeSpan(li.EndTime.Ticks - li.StartTime.Ticks)

                    Dim listr As String = li.ToString
                    If console.InvokeRequired Then
                        console.Invoke(Sub()
                                           console.AppendText(listr)
                                           console.AppendText(vbCrLf)
                                       End Sub)
                    Else
                        console.AppendText(li.ToString)
                        console.AppendText(vbCrLf)
                    End If


                    CurentWorkTime = li
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
            End Using
        End Using
    End Sub

    Private Sub main_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
        AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf OnPowerModeChanged
        Application.AddMessageFilter(Me)
    End Sub

    Private Sub btnTimer_Click(sender As System.Object, e As System.EventArgs) Handles btnTimer.Click
        Timer1_Tick(Me, New System.EventArgs)
    End Sub

    Private Sub MakeNewEntry(Optional ByVal StartEvent As String = "")
        Dim CurentWorkTimeReadfromDB As Boolean = False
        Using documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
            'MsgBox(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
            documentStore.DataDirectory = Path.Combine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData, "DB")
            'Using documentStore = New DocumentStore() With {.Url = "http://localhost:8080", .DefaultDatabase = "Test"}
            documentStore.Initialize()

            Using session As Document.DocumentSession = CType(documentStore.OpenSession(), DocumentSession)

                CurentWorkTime = New WorkTime(DateTime.Now, StartEvent)
                CurentWorkTime.EndTime = DateAndTime.Now
                session.Store(CurentWorkTime)

                'console.Clear()
                Dim tspn As TimeSpan = New TimeSpan(CurentWorkTime.EndTime.Ticks - CurentWorkTime.StartTime.Ticks)
                'console.AppendText(CurentWorkTime.StartTime & " - " & CurentWorkTime.EndTime & " - " & tspn.Hours & ":" & tspn.Minutes)
                'console.AppendText(String.Format("{0:ddd}: {1:d} {1:t} - {2:t} = {3:hh}:{3:mm}", CurentWorkTime.StartTime, CurentWorkTime.StartTime, CurentWorkTime.EndTime, tspn))
                console.AppendText(CurentWorkTime.ToString)
                console.AppendText(vbCrLf)
                session.SaveChanges()
            End Using
        End Using
    End Sub

    Private Sub ShowEntrys()
        Me.ShowEntrysTask()
        'Task.Factory.StartNew(Sub()
        '                          Me.ShowEntrysTask()
        '                      End Sub)
    End Sub

    Private Sub ShowEntrysTask()
        Dim CurentWorkTimeReadfromDB As Boolean = False
        Using documentStore As Raven.Client.Embedded.EmbeddableDocumentStore = New EmbeddableDocumentStore()
            documentStore.DataDirectory = Path.Combine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData, "DB")
            documentStore.Initialize()

            Using session As Document.DocumentSession = CType(documentStore.OpenSession(), DocumentSession)

                'Dim results = session.Query(Of WorkTime)()

                'If CurentWorkTime.StartTime.Month = DateTime.Now.Month Then  True

                Dim results = From WTime In session.Query(Of WorkTime)() _
                .Take(100) _
                .OrderByDescending(Function(WTime) WTime.EndTime) _
                .Where(Function(WTime) WTime.StartTime >= (DateTime.Now.AddMonths(-2)))
                '.ToArray()

                'console.Clear()
                Try
                    For Each li As WorkTime In results
                        Dim tspn As TimeSpan = New TimeSpan(li.EndTime.Ticks - li.StartTime.Ticks)
                        If console.InvokeRequired Then
                            console.Invoke(Sub()
                                               'console.AppendText(li.StartTime & " - " & li.EndTime & " - " & tspn.Hours & ":" & tspn.Minutes)
                                               'console.AppendText(String.Format("{0:ddd}: {1:d} {1:t} - {2:t} = {3:hh}:{3:mm}", li.StartTime, li.StartTime, li.EndTime, tspn))
                                               'Dim AppendText As MethodInfo = TextBox.GetMethod("AppendText", New Type() {GetType(String)})
                                               'console.Invoke(CType(AppendText.Invoke(New Object, New Object() {li.ToString}), TextBox))
                                               console.AppendText(li.ToString)
                                               console.AppendText(vbCrLf)

                                           End Sub)
                        Else
                            console.AppendText(li.ToString)
                            console.AppendText(vbCrLf)
                        End If
                        CurentWorkTime = li
                        CurentWorkTimeReadfromDB = True
                    Next
                Catch ex As Exception

                End Try

            End Using
        End Using
    End Sub

    Private Sub OnPowerModeChanged(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)
        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                Me.MakeNewEntry("Resume")
                Timer1.Enabled = True

            Case Microsoft.Win32.PowerModes.Suspend
                Me.SetEndTimeEntry("Suspend")
                Timer1.Enabled = False

            Case Microsoft.Win32.PowerModes.StatusChange
                Me.SetEndTimeEntry("StatusChange")
                Timer1.Enabled = False

        End Select
    End Sub

    Private Sub SessionEnding(ByVal sender As System.Object, ByVal e As Microsoft.Win32.SessionEndingEventArgs)
        Select Case e.Reason
            Case Microsoft.Win32.SessionEndReasons.Logoff
                'logoff
                Me.MakeNewEntry("Logoff")
                Timer1.Enabled = False
            Case Microsoft.Win32.SessionEndReasons.SystemShutdown
                'shutdown
                Me.MakeNewEntry("Shutdown")
                Timer1.Enabled = False
        End Select
    End Sub

    Private Sub btnNeu_Click(sender As System.Object, e As System.EventArgs) Handles btnNeu.Click
        Me.MakeNewEntry()
        Me.ShowEntrys()
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
    End Sub

    Private Sub main_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
#If DEBUG Then
#Else
        Me.Hide()
#End If
        ShowEntrys()
    End Sub

    Private Sub btnHide_Click(sender As System.Object, e As System.EventArgs) Handles btnHide.Click
        Me.Hide()
    End Sub

    Private Sub main_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Select Case e.CloseReason
            Case CloseReason.None
            Case CloseReason.ApplicationExitCall
            Case CloseReason.FormOwnerClosing
            Case CloseReason.MdiFormClosing
            Case CloseReason.TaskManagerClosing
            Case CloseReason.UserClosing
            Case CloseReason.WindowsShutDown
        End Select

        console.AppendText(DateTime.Now & e.CloseReason.ToString)
        console.AppendText(vbCrLf)
    End Sub


    'Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    '    Dim oMessage As System.Windows.Forms.Message
    '    Dim WM_QUERYENDSESSION = &H11
    '    Dim WM_ENDSESSION = &H16
    '    Dim ENDSESSION_CLOSEAPP = &H1
    '    Dim ENDSESSION_LOGOFF = &H80000000

    '    oMessage = m

    '    Select Case oMessage.Msg
    '        Case WM_ENDSESSION
    '            If oMessage.LParam = ENDSESSION_LOGOFF Then
    '                'system is logging off

    '            End If
    '        Case WM_QUERYENDSESSION
    '            If oMessage.LParam = ENDSESSION_LOGOFF Then

    '            End If

    '    End Select

    '    MyBase.WndProc(m)
    'End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Const SC_RESTORE As Integer = &HF120
        Const SC_SCREENSAVE As Integer = &HF140
        Const WM_SYSCOMMAND As Integer = &H112, SC_MONITORPOWER As Integer = &HF170
        Dim m_needToClose As Boolean = False

        If m.Msg = WM_SYSCOMMAND Then
            'Intercept System Command

            ' notice the 0xFFF0 mask, it's because the system can use the 4 low order bits of the wParam value as stated in the MSDN library article about WM_SYSCOMMAND.

            If (m.WParam.ToInt32() And &HFFF0) = SC_MONITORPOWER Then

                'Intercept Monitor Power Message
                Me.Text = "SC_SCREENSAVE"
                m_needToClose = True

            End If
        End If

        MyBase.WndProc(m)

        If m.Msg = WM_SYSCOMMAND Then
            Select Case m.WParam.ToInt32
                Case SC_SCREENSAVE
                    Me.Text = "SC_SCREENSAVE"

            End Select
        End If
        MyBase.WndProc(m)

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
                    Me.Text = "WM_LBUTTONDBLCLK"
            End Select

        End If



        If (m.Msg = KEY_PRESSED) Then
            Select Case m.WParam.ToInt32()
                Case Keys.F2
                    Me.Text = "Keys.F2xx"
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
            Me.Text = "SC_MONITORPOWER"
        End If

        If m.Msg = WM_SYSCOMMAND And ((m.WParam.ToInt32 And &HFFF0) = SC_SCREENSAVE) Then
            Me.Text = "SC_SCREENSAVE"
        End If

        If m.Msg = WM_SYSCOMMAND Then
            Select Case m.WParam.ToInt32
                Case SC_SCREENSAVE
                    Me.Text = "SC_SCREENSAVE"

                Case SC_RESTORE
                    Me.Text = "SC_RESTORE"

                Case SC_MAXIMIZE
                    Me.Text = "SC_MAXIMIZE"

                Case SC_HOTKEY
                    Me.Text = "SC_HOTKEY"

                Case SC_MONITORPOWER
                    Me.Text = "SC_MONITORPOWER"

            End Select
        End If

        If m.Msg = SC_SCREENSAVE Then
            Me.Text = "SC_SCREENSAVE"
        End If


        Select Case m.Msg
            Case WM_SYSCOMMAND
                If m.WParam.ToInt32 = SC_SCREENSAVE Then
                    Me.Text = "SC_SCREENSAVE"
                    Return True
                End If

                'Handled := TRUE; // disable startup of screensavers
        End Select
        Return False
    End Function
End Class

