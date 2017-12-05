Public Class CLeg
    Public Speed As Double
    Public Clock As Double
    Public Angle As Double
    Public GoThroughClock As Double

    Public LXpos As Double
    Public LYpos As Double

    Public LPx1 As Double
    Public LPx2 As Double
    Public LPy1 As Double
    Public LPy2 As Double

    Public LP1 As PointF
    Public LP2 As PointF
    Public DrawPoint1 As PointF
    Public drawPoint2 As PointF

    Private increase As Double = 1
    Private AngleRotate As Double = 1

    Public Down As Boolean = False
    Public Up As Boolean = True

    Public LeftSide As Boolean
    Public rightSide As Boolean
    Public Pivot As PointF

    Public OldPoint1 As PointF
    Public OldPoint2 As PointF

    Public OldAngle As Double
    Public BodyAngle As Double

    Public Diameter As Double


    Public Sub New(x As Integer, y As Integer, s As Integer, c As Integer, a As Integer, gc As Integer, d As Integer, Optional rnd As Random = Nothing)
        'set properties
        LXpos = x
        LYpos = y
        LPx1 = LXpos
        LPy1 = LYpos
        LPx2 = LXpos

        If Not IsNothing(rnd) Then
            Speed = rnd.Next(0, 10)
            Clock = rnd.Next(5, 100)
            Angle = rnd.Next(40, 180)
            GoThroughClock = rnd.Next(0, 100)
            Diameter = rnd.Next(30, 150)
        Else
            Speed = s
            Clock = c
            Angle = a
            GoThroughClock = gc
            Diameter = d
        End If
        LPy2 = LYpos + Diameter
        LP1 = New Point(LXpos, LYpos)
        LP2 = New Point(LXpos, LYpos + Diameter)


    End Sub

    Function Clone()
        Return Me.MemberwiseClone()
    End Function




    Sub AngleLock(floor As CFloor)

        If Angle / 2 <= Speed * AngleRotate Then
            GoThroughClock += 1

            If GoThroughClock >= Clock Then
                GoThroughClock = 0
                Down = True
                Up = False
            Else
                Down = False
                Up = False
            End If

        ElseIf Angle / 2 <= Speed * -AngleRotate And AngleRotate < 0 Then
            GoThroughClock += 1

            If GoThroughClock >= Clock Then
                GoThroughClock = 0
                Down = False
                Up = True
            Else
                Down = False
                Up = False
            End If
        End If

    End Sub

    Sub NewPoints()

        LP1.X = LPx1
        LP1.Y = LPy1
        LP2.X = LPx2
        LP2.Y = LPy2

        If Up = True Then
            'rotate points using rotation matrix
            AngleRotate += 1
            increase = 1
            LPx1 = ((LP1.X - LXpos) * Math.Cos((Speed * increase) * Math.PI / 180) + ((LP1.Y - LYpos) * -(Math.Sin((Speed * increase) * Math.PI / 180)))) + LXpos
            LPy1 = ((LP1.X - LXpos) * Math.Sin((Speed * increase) * Math.PI / 180) + ((LP1.Y - LYpos) * Math.Cos((Speed * increase) * Math.PI / 180))) + LYpos
            LPx2 = ((LP2.X - LXpos) * Math.Cos((Speed * increase) * Math.PI / 180) + ((LP2.Y - LYpos) * -(Math.Sin((Speed * increase) * Math.PI / 180)))) + LXpos
            LPy2 = ((LP2.X - LXpos) * Math.Sin((Speed * increase) * Math.PI / 180) + ((LP2.Y - LYpos) * Math.Cos((Speed * increase) * Math.PI / 180))) + LYpos

        ElseIf Down = True Then
            AngleRotate -= 1
            increase = -1
            LPx1 = ((LP1.X - LXpos) * Math.Cos((Speed * increase) * Math.PI / 180) + ((LP1.Y - LYpos) * -(Math.Sin((Speed * increase) * Math.PI / 180)))) + LXpos
            LPy1 = ((LP1.X - LXpos) * Math.Sin((Speed * increase) * Math.PI / 180) + ((LP1.Y - LYpos) * Math.Cos((Speed * increase) * Math.PI / 180))) + LYpos
            LPx2 = ((LP2.X - LXpos) * Math.Cos((Speed * increase) * Math.PI / 180) + ((LP2.Y - LYpos) * -(Math.Sin((Speed * increase) * Math.PI / 180)))) + LXpos
            LPy2 = ((LP2.X - LXpos) * Math.Sin((Speed * increase) * Math.PI / 180) + ((LP2.Y - LYpos) * Math.Cos((Speed * increase) * Math.PI / 180))) + LYpos

        End If
        OldAngle = Angle
    End Sub





    Sub AttachBottomLegs(x As Integer, Connection As PointF, Line2 As CLeg, TopLegLength As Integer)

        LPx2 += Line2.LPx2 - LPx1
        LPx1 = Line2.LPx2
        LPy2 += Line2.LPy2 - LPy1
        LPy1 = Line2.LPy2

        LYpos = Connection.Y
        LYpos = Line2.LYpos + TopLegLength
        LXpos = Connection.X
        LXpos = Line2.LXpos

    End Sub

    Sub FindLowestLeg(Line2 As CLeg, ByRef BodyRise As Double)

        If BodyRise <= LPy2 Then
            BodyRise = LPy2
        End If
        If BodyRise <= LPy1 Then
            BodyRise = LPy1
        End If
        If BodyRise <= Line2.LPy1 Then
            BodyRise = Line2.LPy1
        End If
    End Sub

    Sub FindSideLegisOn(Body As CBody, Floor As CFloor)


        'need to put this function in body instead
        If Form1.CheckFloor(Floor, LPy1) = True And LPx1 < Body.CoM.X - 2 Then
            Body.Leftside = True
            Body.RightMomentum = 0.1

        ElseIf Form1.CheckFloor(Floor, LPy1) = True And LPx1 > Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.LeftMomentum = 0.1

        ElseIf Form1.CheckFloor(Floor, LPy1) = True And LPx1 >= Body.CoM.X - 2 And LPx1 <= Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.Leftside = True
            Body.LeftMomentum = 0.1
            Body.RightMomentum = 0.1

        End If

        If Form1.CheckFloor(Floor, LPy1) = True Then
            Pivot.X = LPx1
            Pivot.Y = LPy1
            Body.Pivot = Pivot

            If Form1.CheckFloor(Floor, LPy2) = True And LPx2 < Body.CoM.X - 2 Then
                Body.Leftside = True
                Body.RightMomentum = 0.1
                If LPy2 > Pivot.X Then
                    Pivot.X = LPx2
                    Pivot.Y = LPy2
                    Body.Pivot = Pivot
                End If
            ElseIf Form1.CheckFloor(Floor, LPy2) = True And LPx2 > Body.CoM.X + 2 Then
                Body.Rightside = True
                Body.LeftMomentum = 0.1
                If LPy2 < Pivot.X Then
                    Pivot.X = LPx2
                    Pivot.Y = LPy2
                    Body.Pivot = Pivot
                End If
            ElseIf Form1.CheckFloor(Floor, LPy2) = True And LPx2 >= Body.CoM.X - 2 And LPx2 <= Body.CoM.X + 2 Then
                Body.Rightside = True
                Body.Leftside = True
                Body.LeftMomentum = 0.1
                Body.RightMomentum = 0.1
            End If

        ElseIf Form1.CheckFloor(Floor, LPy2) = True And LPx2 < Body.CoM.X - 2 Then

            Body.Leftside = True
            Body.RightMomentum = 0.1
            Pivot.X = LPx2
            Pivot.Y = LPy2
        ElseIf Form1.CheckFloor(Floor, LPy2) = True And LPx2 > Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.LeftMomentum = 0.1
            Pivot.X = LPx2
            Pivot.Y = LPy2
        ElseIf Form1.CheckFloor(Floor, LPy2) = True And LPx2 >= Body.CoM.X - 2 And LPx2 <= Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.Leftside = True
            Body.LeftMomentum = 0.1
            Body.RightMomentum = 0.1
            Pivot.X = LPx2
            Pivot.Y = LPy2
        Else
            Pivot.X = 0
            Pivot.Y = 0
        End If

    End Sub

    Sub StopFalling(Body As CBody, Floor As CFloor)

        If Form1.CheckFloor(Floor, LPy1) = True Or Form1.CheckFloor(Floor, LPy2) Then
            Body.Yspeed = 0
        End If

    End Sub

    Sub Draw(g As Graphics, ColourBrush As Pen)
        DrawPoint1.X = LPx1 - Form1.FurthestAnimalPos
        DrawPoint1.Y = LPy1
        drawPoint2.X = LPx2 - Form1.FurthestAnimalPos
        drawPoint2.Y = LPy2
        g.DrawLine(ColourBrush, DrawPoint1.X, DrawPoint1.Y, drawPoint2.X, drawPoint2.Y)
    End Sub

    Sub FallOver(pivot As PointF, angle As Double)
        LP1.X = LPx1
        LP1.Y = LPy1
        LP2.X = LPx2
        LP2.Y = LPy2

        LPx1 = ((LP1.X - pivot.X) * Math.Cos((angle) * Math.PI / 180) + ((LP1.Y - pivot.Y) * -(Math.Sin((angle) * Math.PI / 180)))) + pivot.X
        LPy1 = ((LP1.X - pivot.X) * Math.Sin((angle) * Math.PI / 180) + ((LP1.Y - pivot.Y) * Math.Cos((angle) * Math.PI / 180))) + pivot.Y
        LPx2 = ((LP2.X - pivot.X) * Math.Cos((angle) * Math.PI / 180) + ((LP2.Y - pivot.Y) * -(Math.Sin((angle) * Math.PI / 180)))) + pivot.X
        LPy2 = ((LP2.X - pivot.X) * Math.Sin((angle) * Math.PI / 180) + ((LP2.Y - pivot.Y) * Math.Cos((angle) * Math.PI / 180))) + pivot.Y

    End Sub
End Class