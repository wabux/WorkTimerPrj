<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(main))
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.btnAnzeigen = New System.Windows.Forms.Button()
        Me.console = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.btnTimer = New System.Windows.Forms.Button()
        Me.btnNeu = New System.Windows.Forms.Button()
        Me.btnHide = New System.Windows.Forms.Button()
        Me.btnDBClear = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Visible = True
        '
        'btnAnzeigen
        '
        Me.btnAnzeigen.Location = New System.Drawing.Point(3, 8)
        Me.btnAnzeigen.Name = "btnAnzeigen"
        Me.btnAnzeigen.Size = New System.Drawing.Size(75, 23)
        Me.btnAnzeigen.TabIndex = 0
        Me.btnAnzeigen.Text = "Anzeigen"
        Me.btnAnzeigen.UseVisualStyleBackColor = True
        '
        'console
        '
        Me.console.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.console.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.console.Location = New System.Drawing.Point(3, 37)
        Me.console.Multiline = True
        Me.console.Name = "console"
        Me.console.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.console.Size = New System.Drawing.Size(363, 277)
        Me.console.TabIndex = 1
        '
        'Timer1
        '
        Me.Timer1.Interval = 60000
        '
        'btnTimer
        '
        Me.btnTimer.Location = New System.Drawing.Point(165, 8)
        Me.btnTimer.Name = "btnTimer"
        Me.btnTimer.Size = New System.Drawing.Size(75, 23)
        Me.btnTimer.TabIndex = 2
        Me.btnTimer.Text = "Timer"
        Me.btnTimer.UseVisualStyleBackColor = True
        '
        'btnNeu
        '
        Me.btnNeu.Location = New System.Drawing.Point(84, 8)
        Me.btnNeu.Name = "btnNeu"
        Me.btnNeu.Size = New System.Drawing.Size(75, 23)
        Me.btnNeu.TabIndex = 3
        Me.btnNeu.Text = "Neu"
        Me.btnNeu.UseVisualStyleBackColor = True
        '
        'btnHide
        '
        Me.btnHide.Location = New System.Drawing.Point(345, 8)
        Me.btnHide.Name = "btnHide"
        Me.btnHide.Size = New System.Drawing.Size(21, 23)
        Me.btnHide.TabIndex = 4
        Me.btnHide.Text = "H"
        Me.btnHide.UseVisualStyleBackColor = True
        '
        'btnDBClear
        '
        Me.btnDBClear.Location = New System.Drawing.Point(246, 8)
        Me.btnDBClear.Name = "btnDBClear"
        Me.btnDBClear.Size = New System.Drawing.Size(75, 23)
        Me.btnDBClear.TabIndex = 5
        Me.btnDBClear.Text = "DBClear"
        Me.btnDBClear.UseVisualStyleBackColor = True
        '
        'main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(369, 319)
        Me.Controls.Add(Me.btnDBClear)
        Me.Controls.Add(Me.btnHide)
        Me.Controls.Add(Me.btnNeu)
        Me.Controls.Add(Me.btnTimer)
        Me.Controls.Add(Me.console)
        Me.Controls.Add(Me.btnAnzeigen)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "WorkTime"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents btnAnzeigen As System.Windows.Forms.Button
    Friend WithEvents console As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnTimer As System.Windows.Forms.Button
    Friend WithEvents btnNeu As System.Windows.Forms.Button
    Friend WithEvents btnHide As System.Windows.Forms.Button
    Friend WithEvents btnDBClear As System.Windows.Forms.Button

End Class
