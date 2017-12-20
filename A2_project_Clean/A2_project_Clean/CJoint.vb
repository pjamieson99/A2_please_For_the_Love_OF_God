Public Class CJoint

    Private width As Integer
    Private diameter As Integer
    Public JYpos As Integer
    Public JXpos As Integer
    Dim DrawX As Integer

    Public PrevJoint As CJoint


    'set properties
    Public Sub New(x As Integer, y As Integer, w As Integer, d As Integer)

        width = w
        diameter = d
        JYpos = y
        JXpos = x


    End Sub


    Function Clone()
        Return Me.MemberwiseClone()
    End Function

    Sub PrevPoint()
        PrevJoint = New CJoint(JXpos, JYpos, width, diameter)
    End Sub
    'draw
    Public Sub Draw(g As Graphics, ColourPen As Pen)
        '    DrawX = JXpos - Form1.FurthestAnimalPos

        g.FillEllipse(New SolidBrush(ColourPen.Color), JXpos - 5, JYpos - 5, width, diameter)
    End Sub

    'rise the joint towards the line point 1
    Sub StickToLeg(p1 As PointF)
        JYpos = p1.Y
        JXpos = p1.X
    End Sub

End Class