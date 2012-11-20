Public Class WorkTime
    Inherits Object
    Private _StartEvent As String = ""

    Sub New()
        Me.StartTime = DateTime.Now
        Me.EndTime = DateTime.Now
        Me.StartEvent = ""
    End Sub

    'Sub New(Optional StartEvent As String = "unknwon")
    '    Me.StartTime = DateTime.Now
    '    Me.EndTime = DateTime.Now
    '    Me.StartEvent = StartEvent
    'End Sub

    Sub New(ByVal StartTime As DateTime, Optional StartEvent As String = "")
        Me.StartTime = StartTime
        Me.EndTime = DateTime.Now
        Me.StartEvent = StartEvent
    End Sub

    Sub New(ByVal StartTime As DateTime, ByVal EndTime As DateTime, Optional StartEvent As String = "")
        Me.StartTime = StartTime
        Me.EndTime = EndTime
        Me.StartEvent = StartEvent
    End Sub


    Public Property StartTime As DateTime

    Public Property EndTime As DateTime

    Public Property StartEvent As String
        Get
            Return _StartEvent
        End Get
        Set(value As String)
            _StartEvent = value
        End Set
    End Property

    Public ReadOnly Property WorkingHours As TimeSpan
        Get
            Return New TimeSpan(Me.EndTime.Ticks - Me.StartTime.Ticks)
        End Get
    End Property



    'Public Overrides Function ToString() As String
    '    Return MyBase.ToString()
    'End Function
    Public Overrides Function ToString() As String
        Return String.Format("{0:ddd}: {4,-8} {1:d} {1:t} - {2:t} = {3:hh}:{3:mm}", Me.StartTime, Me.StartTime, Me.EndTime, Me.WorkingHours, _StartEvent)
    End Function
End Class
