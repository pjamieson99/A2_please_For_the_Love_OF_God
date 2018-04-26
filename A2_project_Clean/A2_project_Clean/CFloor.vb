Public Class CFloor 'this is the floor object that the creatures will stand on
    Public ypos As Integer
    Public Sub New(y As Integer)
        'set property
        ypos = y
    End Sub
    'draw floor
    Public Sub Draw(g As Graphics) 'draw the floor
        g.DrawLine(Pens.Black, 0, ypos, 10000, ypos)
    End Sub

    Function CheckFloor(floor As CFloor, point As Double) 'check if a point has collided with the floor
        If floor.ypos - 2 <= point Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
