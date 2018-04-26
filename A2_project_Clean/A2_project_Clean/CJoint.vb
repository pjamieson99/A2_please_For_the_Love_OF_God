Public Class CJoint

    Private width As Integer 'size of the joints determined by these properties
    Private diameter As Integer
    Public JYpos As Integer
    Public JXpos As Integer
    Dim DrawX As Integer


    Public Sub New(x As Integer, y As Integer, w As Integer, d As Integer) 'set properties
        width = w
        diameter = d
        JYpos = y
        JXpos = x
    End Sub

    Function Clone() 'clones the joint to avoid updating the values
        Return Me.MemberwiseClone()
    End Function

    Public Sub Draw(g As Graphics, ColourPen As Pen) 'draw the joint
        g.FillEllipse(New SolidBrush(ColourPen.Color), Convert.ToSingle(JXpos - (diameter / 2)), Convert.ToSingle(JYpos - (diameter / 2)), width, diameter)
    End Sub

    Sub StickToLeg(p1 As PointF)  'connect the joint to the line's top point
        JYpos = p1.Y
        JXpos = p1.X
    End Sub

End Class