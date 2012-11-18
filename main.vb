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




'IEnumerable<InstallationSummary> installationSummaries =
'  QueryAndCacheEtags(session => session.Advanced.LuceneQuery<InstallationSummary>()
'  .Include(x => x.ApplicationServerId)
'  .Include(x => x.ApplicationWithOverrideVariableGroup.ApplicationId)
'  .Include(x => x.ApplicationWithOverrideVariableGroup.CustomVariableGroupId)
'  .OrderByDescending(summary => summary.InstallationStart)
'  .Take(numberToRetrieve)).Cast<InstallationSummary>().ToList();

'Dim installationSummaries As IEnumerable(Of InstallationSummary) = QueryAndCacheEtags(Function(session) session.Advanced.LuceneQuery(Of InstallationSummary)().Include(Function(x) x.ApplicationServerId).Include(Function(x) x.ApplicationWithOverrideVariableGroup.ApplicationId).Include(Function(x) x.ApplicationWithOverrideVariableGroup.CustomVariableGroupId).OrderByDescending(Function(summary) summary.InstallationStart).Take(numberToRetrieve)).Cast(Of InstallationSummary)().ToList()
Public Class main

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
        If e.Mode = Microsoft.Win32.PowerModes.Resume Then
            Me.MakeNewEntry("Resume")
            Timer1.Enabled = True
        ElseIf e.Mode = Microsoft.Win32.PowerModes.Suspend Then
            Me.SetEndTimeEntry("Suspend")
            Timer1.Enabled = False
        ElseIf e.Mode = Microsoft.Win32.PowerModes.StatusChange Then
            Me.SetEndTimeEntry("StatusChange")
            Timer1.Enabled = False
        End If
    End Sub



    Private Sub SessionEnding(ByVal sender As System.Object, ByVal e As Microsoft.Win32.SessionEndingEventArgs)
        Select Case e.Reason
            Case Microsoft.Win32.SessionEndReasons.Logoff
                'logoff
                console.AppendText(DateTime.Now & " - SessionEndReasons.Logoff")
                console.AppendText(vbCrLf)
                Timer1.Enabled = False
            Case Microsoft.Win32.SessionEndReasons.SystemShutdown
                'shutdown
                console.AppendText(DateTime.Now & " - SessionEndReasons.SystemShutdown")
                console.AppendText(vbCrLf)
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
End Class
