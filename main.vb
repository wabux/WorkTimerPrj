Imports System.Windows.Forms
Imports System.Drawing
Imports System.Reflection

'*****************************************************************************
NotInheritable Class MyNotifyIconApplication
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

        ' Ohne Appplication.Run geht es nicht
        Application.Run()
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