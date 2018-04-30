Public Class CLeg
    'This is the leg class, it provides the properties and methods used by the legs 

    Public Speed As Double 'properties of legs - speed = how fast it rotates
    Public Clock As Double 'clock = how long it will stay still once making a full rotation
    Public Angle As Double 'angle = how large one rotation is
    Public GoThroughClock As Double 'gives different starting positions of the legs
    Public Diameter As Double 'diameter = length of the leg

    Public LXpos As Double 'these are the original - non-rotated leg positions. they are used for the rotation matrix to work properly
    Public LYpos As Double

    Public LPx1 As Double 'these are the point positions of the leg
    Public LPx2 As Double
    Public LPy1 As Double
    Public LPy2 As Double

    Public LP1 As PointF 'these are repeats of the point positions, they are used as non-updated versions of them in the rotation matrix function 
    Public LP2 As PointF

    Public DrawPoint1 As PointF 'this sets the poitns of the legs to integer values as a point that can be drawn
    Public drawPoint2 As PointF

    Private increase As Double = 1 'increases the angle that is rotated
    Private AngleRotate As Double = 1

    Public Down As Boolean = False 'used to choose whether to rotate clockwise ort anti-clockwise
    Public Up As Boolean = True


    Public Pivot As PointF 'sets pivot as to what rotates

    Public OldPoint1 As PointF 'keeps record of the old position of the leg to be used to find movement from previous state
    Public OldPoint2 As PointF




    Public Sub New(x As Integer, y As Integer, s As Integer, c As Integer, a As Integer, gc As Integer, d As Integer, Optional rnd As Random = Nothing) 'creates a new leg
        'set properties
        LXpos = x
        LYpos = y
        LPx1 = LXpos
        LPy1 = LYpos
        LPx2 = LXpos

        If Not IsNothing(rnd) Then 'creates random properties if the creature isn't already assigned properties
            Speed = Math.Abs(rnd.Next(-11, -1))
            Clock = rnd.Next(5, 100)
            Angle = rnd.Next(-180, -40)
            GoThroughClock = rnd.Next(0, 100)
            Diameter = rnd.Next(30, 150)
        Else 'if creature has already been assigned properties then set them to those values
            Speed = s
            Clock = c
            Angle = a
            GoThroughClock = gc
            Diameter = d
        End If

        LPy2 = LYpos + Diameter 'sets the origin points
        LP1 = New Point(LXpos, LYpos)
        LP2 = New Point(LXpos, LYpos + Diameter)
    End Sub

    Function Clone() 'allows leg to be clones so it doesn't get updated
        Return Me.MemberwiseClone()
    End Function

    Sub AngleLock(floor As CFloor) 'sets the limit to how much a leg can rotate

        If Angle / 2 <= Speed * AngleRotate Then 'sets clockwise limit
            GoThroughClock += 1

            If GoThroughClock >= Clock Then
                GoThroughClock = 0
                Down = True
                Up = False
            Else
                Down = False
                Up = False
            End If

        ElseIf Angle / 2 <= Speed * -AngleRotate And AngleRotate < 0 Then 'sets anti-clockwise limit
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

        LP1.X = LPx1 'sets the non - updated values to be used in the function
        LP1.Y = LPy1
        LP2.X = LPx2
        LP2.Y = LPy2

        If Up = True Then 'rotate points using rotation matrix

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

    End Sub

    Sub AttachBottomLegs(x As Integer, Connection As PointF, Line2 As CLeg, TopLegLength As Integer) 'attaches the bottom legs to the top legs
        LPx2 += Line2.LPx2 - LPx1
        LPx1 = Line2.LPx2
        LPy2 += Line2.LPy2 - LPy1
        LPy1 = Line2.LPy2

        LYpos = Connection.Y
        LYpos = Line2.LYpos + TopLegLength
        LXpos = Connection.X
        LXpos = Line2.LXpos
    End Sub

    Sub FindLowestLeg(Line2 As CLeg, ByRef BodyRise As Double) 'finds lowest leg past the floor and finds the difference from the floor to that leg
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

    Sub FindSideLegisOn(Body As CBody, Floor As CFloor) 'this finds out what sides of the body are touching the floor, finds whether it is falling or not

        If Floor.CheckFloor(Floor, LPy1) = True And LPx1 < Body.CoM.X - 2 Then 'finds if target leg is touching floor and what side it is, prevents falling on that side
            Body.Leftside = True
            Body.RightMomentum = 0.1
        ElseIf Floor.CheckFloor(Floor, LPy1) = True And LPx1 > Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.LeftMomentum = 0.1
        ElseIf Floor.CheckFloor(Floor, LPy1) = True And LPx1 >= Body.CoM.X - 2 And LPx1 <= Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.Leftside = True
            Body.LeftMomentum = 0.1
            Body.RightMomentum = 0.1
        End If

        If Floor.CheckFloor(Floor, LPy1) = True Then 'finds the pivot that is closest to the centre of mass out of its top and bottom point
            Pivot.X = LPx1
            Pivot.Y = LPy1

            If Floor.CheckFloor(Floor, LPy2) = True And LPx2 < Body.CoM.X - 2 Then
                Body.Leftside = True
                Body.RightMomentum = 0.1

                If LPy2 > Pivot.X Then
                    Pivot.X = LPx2
                    Pivot.Y = LPy2
                End If

            ElseIf Floor.CheckFloor(Floor, LPy2) = True And LPx2 > Body.CoM.X + 2 Then
                Body.Rightside = True
                Body.LeftMomentum = 0.1

                If LPy2 < Pivot.X Then
                    Pivot.X = LPx2
                    Pivot.Y = LPy2
                End If

            ElseIf Floor.CheckFloor(Floor, LPy2) = True And LPx2 >= Body.CoM.X - 2 And LPx2 <= Body.CoM.X + 2 Then
                Body.Rightside = True
                Body.Leftside = True
                Body.LeftMomentum = 0.1
                Body.RightMomentum = 0.1
            End If

        ElseIf Floor.CheckFloor(Floor, LPy2) = True And LPx2 < Body.CoM.X - 2 Then
            Body.Leftside = True
            Body.RightMomentum = 0.1
            Pivot.X = LPx2
            Pivot.Y = LPy2
        ElseIf Floor.CheckFloor(Floor, LPy2) = True And LPx2 > Body.CoM.X + 2 Then
            Body.Rightside = True
            Body.LeftMomentum = 0.1
            Pivot.X = LPx2
            Pivot.Y = LPy2
        ElseIf Floor.CheckFloor(Floor, LPy2) = True And LPx2 >= Body.CoM.X - 2 And LPx2 <= Body.CoM.X + 2 Then 'finds closest pivot and checks which side the point is on
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

    Sub StopFalling(Body As CBody, Floor As CFloor) 'if the leg is touching the floor then the body stops falling

        If Floor.CheckFloor(Floor, LPy1) = True Or Floor.CheckFloor(Floor, LPy2) Then
            Body.Yspeed = 0
        End If

    End Sub

    Sub Draw(g As Graphics, ColourBrush As Pen) 'draws the leg
        DrawPoint1.X = LPx1 - Form1.FurthestAnimalPos
        DrawPoint1.Y = LPy1
        drawPoint2.X = LPx2 - Form1.FurthestAnimalPos
        drawPoint2.Y = LPy2
        g.DrawLine(ColourBrush, DrawPoint1.X, DrawPoint1.Y, drawPoint2.X, drawPoint2.Y)
    End Sub

    Sub FallOver(BodyPivot As PointF, ang As Double) 'this moves the angle of the leg relative to the motion of the body falling over
        LP1.X = LPx1 'sets the non - updated values to be used in the function
        LP1.Y = LPy1
        LP2.X = LPx2
        LP2.Y = LPy2

        'rotation matrix used to rotate points by given angle

        LPx1 = ((LP1.X - BodyPivot.X) * Math.Cos((ang) * Math.PI / 180) + ((LP1.Y - BodyPivot.Y) * -(Math.Sin((ang) * Math.PI / 180)))) + BodyPivot.X
        LPy1 = ((LP1.X - BodyPivot.X) * Math.Sin((ang) * Math.PI / 180) + ((LP1.Y - BodyPivot.Y) * Math.Cos((ang) * Math.PI / 180))) + BodyPivot.Y
        LPx2 = ((LP2.X - BodyPivot.X) * Math.Cos((ang) * Math.PI / 180) + ((LP2.Y - BodyPivot.Y) * -(Math.Sin((ang) * Math.PI / 180)))) + BodyPivot.X
        LPy2 = ((LP2.X - BodyPivot.X) * Math.Sin((ang) * Math.PI / 180) + ((LP2.Y - BodyPivot.Y) * Math.Cos((ang) * Math.PI / 180))) + BodyPivot.Y

    End Sub
End Class