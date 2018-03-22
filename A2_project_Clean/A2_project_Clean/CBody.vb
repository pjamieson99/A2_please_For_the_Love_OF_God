Public Class CBody
    Public CoM As PointF 'this defines the centre of mass
    Public PrevCom As PointF 'this is used to determine whether the creature is making progress based on whether its new centre of mass at the end of the fram has made progress from its previous centre of mass

    Public Bypos1 As Double 'these are the original, un-rotated points of the body
    Public Bypos2 As Double
    Public Bxpos1 As Double
    Public Bxpos2 As Double

    Public Yspeed As Double = 0 'this is the speed at which the body is falling down due to gravity

    Public BPx1 As Double 'these are the accurate  points of the body 
    Public BPx2 As Double
    Public BPy1 As Double
    Public BPy2 As Double



    Public AngleIncrease As Double = 0 'this is the angle at which the body rotates

    Public BodyRise As Double = 0 'this is used to the raise the body from falling through the floor

    Public LeftMomentum As Double = 0.1 'this is the momentum at which the body falls in a direction
    Public RightMomentum As Double = 0.1

    Public Pivot As PointF 'this is the pivot in which the body falls over - the closest point to the x position of the CoM that touches the floor

    Public Leftside As Boolean 'variables used in determining whether their are points touching the floor on the right and left side of the body
    Public Rightside As Boolean

    Public Diameter As Double 'sets length of body

    Public Friction As Double 'friction values that find the distance moved by a creature due to the friction
    Public NegativeFriction As Double

    Dim NumOfLegs As Integer 'sets the number of legs attached to this body
    Dim NumOfLayers As Integer

    Public Connections As New List(Of PointF) 'these are the connections of the legs to the body - the points at which they connect
    Public StaticConnections As New List(Of PointF) 'these are the positions of the connections of the legs to the body that allow the rotation matrix to use the connections values that arent updating them to update them

    Dim DrawX1 As Double 'these are the points of the body that are drawn onto the screen
    Dim DrawX2 As Double


    Public Sub New(x1, y1, x2, y2, n) 'creates a new body based on values given to it
        Bypos1 = y1 'sets the point positions of body that are used to not update during rotation matrix
        Bxpos1 = x1
        Bypos2 = y2
        Bxpos2 = x2

        CoM = New Point((x1 + x2) / 2, (y1 + y2) / 2) 'sets centre of mass
        PrevCom = New Point((x1 + x2) / 2, (y1 + y2) / 2) 'sets previous centre of mass

        BPx1 = Bxpos1 'sets the points that can be rotated
        BPx2 = Bxpos2
        BPy1 = Bypos1
        BPy2 = Bypos2

        Diameter = BPx2 - BPx1 'sets the diameter of the body

        NumOfLegs = n 'sets the number of legs attached to the body
        NumOfLayers = 2
    End Sub

    Sub BodyPoints(P1Legs As PointF) 'creates connections of where the legs are attached to the body
        Connections.Add(P1Legs)
        StaticConnections.Add(P1Legs)
    End Sub

    Public Sub SetPoints(line As CLeg, x As Integer) 'attaches the connected position of the leg to the body
        line.LP1 = Connections(x) 'attaches top points of leg to body

        line.LPx2 += Connections(x).X - line.LPx1 'moves the bottom part of the leg by how much the top part moved when attached ffrom where it was previously
        line.LPy2 += Connections(x).Y - line.LPy1

        line.LYpos = Connections(x).Y 'sets the unrotatable points to the current position of the attached leg
        line.LXpos = Connections(x).X

        line.LPy1 = Connections(x).Y 'sets the rotatable points to the attached part of the leg
        line.LPx1 = Connections(x).X
    End Sub

    Sub RaiseBody(Floor As CFloor) 'stops creature form falling through floor -keeps creature above the floor

        If Floor.ypos <= BodyRise Then
            BPy1 -= (BodyRise - Floor.ypos) 'moves body point positions up by distance under the floor the creature is
            BPy2 -= (BodyRise - Floor.ypos)

            For x = 0 To NumOfLegs - 1 'sets the connections
                Connections(x) = New PointF(Connections(x).X, Connections(x).Y - (BodyRise - Floor.ypos))
                StaticConnections(x) = New PointF(StaticConnections(x).X, StaticConnections(x).Y - (BodyRise - Floor.ypos))
            Next

            Bypos1 -= (BodyRise - Floor.ypos) 'moves the rotation matrix points up as well
            Bypos2 -= (BodyRise - Floor.ypos)

            ResetCoM() 'updates position of the centre of mass
        End If

    End Sub

    Sub ResetCoM() 'updates the current position of the centre of mass based on the current position of th ebody
        CoM = New Point((BPx1 + BPx2) / 2, (BPy1 + BPy2) / 2)
    End Sub

    Sub FindPivot(Line As CLeg, floor As CFloor) 'finds pivot closest to the body
        If Leftside = True And Rightside = False And floor.CheckFloor(floor, Line.Pivot.Y) = True Then 'checks whether body is dalling to the right

            If Pivot.X < Line.Pivot.X Or floor.CheckFloor(floor, Pivot.Y) = False Then 'finds closest pivot to centre of mass
                Pivot = Line.Pivot
            End If

        ElseIf Leftside = False And Rightside = True And floor.CheckFloor(floor, Line.Pivot.Y) = True Then 'checks if body is falling to the left

            If Pivot.X > Line.Pivot.X Or floor.CheckFloor(floor, Pivot.Y) = False Then 'finds closes pivot to centre of mass
                Pivot = Line.Pivot
            End If

        End If
    End Sub

    Sub FallRight(Pivot As PointF, g As Graphics) 'if body is falling right it uses this function
        AngleIncrease = 1 * RightMomentum 'updates how fast the body is falling
        RightMomentum -= 0.2

        For x = 0 To NumOfLegs - 1 'uses rotation matrix on all points of body and connections of leg to body
            Dim SetX = ((StaticConnections(x).X - Pivot.X) * Math.Cos(AngleIncrease * Math.PI / 180)) + ((StaticConnections(x).Y - Pivot.Y) * -(Math.Sin(AngleIncrease * Math.PI / 180))) + Pivot.X
            Dim SetY = ((StaticConnections(x).X - Pivot.X) * Math.Sin(AngleIncrease * Math.PI / 180)) + ((StaticConnections(x).Y - Pivot.Y) * Math.Cos(AngleIncrease * Math.PI / 180)) + Pivot.Y
            Connections(x) = New PointF(SetX, SetY)
        Next

        BPx1 = ((Bxpos1 - Pivot.X) * Math.Cos(AngleIncrease * Math.PI / 180)) + ((Bypos1 - Pivot.Y) * -(Math.Sin(AngleIncrease * Math.PI / 180))) + Pivot.X
        BPy1 = ((Bxpos1 - Pivot.X) * Math.Sin(AngleIncrease * Math.PI / 180)) + ((Bypos1 - Pivot.Y) * Math.Cos(AngleIncrease * Math.PI / 180)) + Pivot.Y
        BPx2 = ((Bxpos2 - Pivot.X) * Math.Cos(AngleIncrease * Math.PI / 180)) + ((Bypos2 - Pivot.Y) * -(Math.Sin(AngleIncrease * Math.PI / 180))) + Pivot.X
        BPy2 = ((Bxpos2 - Pivot.X) * Math.Sin(AngleIncrease * Math.PI / 180)) + ((Bypos2 - Pivot.Y) * Math.Cos(AngleIncrease * Math.PI / 180)) + Pivot.Y


    End Sub

    Sub FallLeft(Pivot As PointF, G As Graphics) 'if body is falling left it uses this function
        AngleIncrease = 1 * LeftMomentum 'updates how fast the body is falling
        LeftMomentum += 0.2

        For x = 0 To NumOfLegs - 1 'uses rotation matrix on all points of body and connections of leg to body
            Dim SetX = ((StaticConnections(x).X - Pivot.X) * Math.Cos(AngleIncrease * Math.PI / 180)) + ((StaticConnections(x).Y - Pivot.Y) * -(Math.Sin(AngleIncrease * Math.PI / 180))) + Pivot.X
            Dim SetY = ((StaticConnections(x).X - Pivot.X) * Math.Sin(AngleIncrease * Math.PI / 180)) + ((StaticConnections(x).Y - Pivot.Y) * Math.Cos(AngleIncrease * Math.PI / 180)) + Pivot.Y
            Connections(x) = New PointF(SetX, SetY)
        Next

        BPx1 = ((Bxpos1 - Pivot.X) * Math.Cos(AngleIncrease * Math.PI / 180)) + ((Bypos1 - Pivot.Y) * -(Math.Sin(AngleIncrease * Math.PI / 180))) + Pivot.X
        BPy1 = ((Bxpos1 - Pivot.X) * Math.Sin(AngleIncrease * Math.PI / 180)) + ((Bypos1 - Pivot.Y) * Math.Cos(AngleIncrease * Math.PI / 180)) + Pivot.Y
        BPx2 = ((Bxpos2 - Pivot.X) * Math.Cos(AngleIncrease * Math.PI / 180)) + ((Bypos2 - Pivot.Y) * -(Math.Sin(AngleIncrease * Math.PI / 180))) + Pivot.X
        BPy2 = ((Bxpos2 - Pivot.X) * Math.Sin(AngleIncrease * Math.PI / 180)) + ((Bypos2 - Pivot.Y) * Math.Cos(AngleIncrease * Math.PI / 180)) + Pivot.Y

    End Sub
    Sub FallOver(g As Graphics, line(,) As CLeg) 'chooses which direction to fall over
        Bxpos1 = BPx1 'sets the unupdates points during the program
        Bxpos2 = BPx2
        Bypos1 = BPy1
        Bypos2 = BPy2

        For x = 0 To NumOfLegs - 1 ' sets the undupdatated connections of leg to body
            StaticConnections(x) = Connections(x)
        Next

        If Leftside = True And Rightside = False Then 'falls in a direction based on whether legs are touching the floor on either side
            FallLeft(Pivot, g)
        ElseIf Leftside = False And Rightside = True Then
            FallRight(Pivot, g)
        End If

        ResetCoM() 'updates the position of the centre of mass based on the new position of the body

        For y = 0 To NumOfLayers - 1 'makes lines fall in the same direction with the same speed based on the pivot the body falls over oon
            For x = 0 To NumOfLegs - 1
                line(x, y).FallOver(Pivot, AngleIncrease)
            Next
        Next

    End Sub

    Sub Drop() 'this function causes gravity to take effect on the body whenever it is not touching the floor
        Bypos1 += Yspeed 'moves the body down by the acceleration of gravity
        Bypos2 += Yspeed
        For x = 0 To NumOfLegs - 1 '
            Connections(x) = New PointF(Connections(x).X, Connections(x).Y + Yspeed)
            StaticConnections(x) = New PointF(StaticConnections(x).X, StaticConnections(x).Y + Yspeed)
        Next
        BPy1 += Yspeed
        BPy2 += Yspeed

        Yspeed += 1 'increases the amount the body falls over per frame to add momentum to the fall

        ResetCoM() '#updates the position of the centre of mass based on the new body position
    End Sub

    Function Clone() 'allows the object to be clones so that the original doesn't get updated
        Return Me.MemberwiseClone()
    End Function

    Sub FindFriction(Line As CLeg, Floor As CFloor) 'implements friction into the program

        If Line.OldPoint1.X - Line.LPx1 > Friction And Floor.CheckFloor(Floor, Line.LPy1) = True And Floor.CheckFloor(Floor, Line.OldPoint1.Y) Then 'checks if the target line is touching the floor and then finds the greatest forward movement for each leg's top position
            Friction = (Line.OldPoint1.X - Line.LPx1)
        End If

        If Line.OldPoint2.X - Line.LPx2 > Friction And Floor.CheckFloor(Floor, Line.LPy2) = True And Floor.CheckFloor(Floor, Line.OldPoint2.Y) Then 'checks if the target line is touching the floor and then finds the greatest forward movement for each leg's bottom position
            Friction = (Line.OldPoint2.X - Line.LPx2)
        End If

        If Line.OldPoint1.X - Line.LPx1 < NegativeFriction And Floor.CheckFloor(Floor, Line.LPy1) = True And Floor.CheckFloor(Floor, Line.OldPoint1.Y) Then 'checks if the target line is touching the floor and then finds the greatest backwards movement for each leg's top position
            NegativeFriction = (Line.OldPoint1.X - Line.LPx1)
        End If

        If Line.OldPoint2.X - Line.LPx2 < NegativeFriction And Floor.CheckFloor(Floor, Line.LPy2) = True And Floor.CheckFloor(Floor, Line.OldPoint2.Y) Then 'checks if the target line is touching the floor and then finds the greatest forward movement for each leg's bottom position
            NegativeFriction = (Line.OldPoint2.X - Line.LPx2)
        End If

    End Sub

    Sub AddFriction() 'moves the body by the determined fricitonal value


        BPx1 += Friction + NegativeFriction 'moves body points by the sum of forwards movement and backwards movement to give a frictional value that these points are then moved by
        BPx2 += Friction + NegativeFriction
        For x = 0 To NumOfLegs - 1
            Connections(x) = New PointF(Connections(x).X + Friction + NegativeFriction, Connections(x).Y)
            StaticConnections(x) = New PointF(StaticConnections(x).X + Friction + NegativeFriction, StaticConnections(x).Y)
        Next
        Bxpos1 += Friction + NegativeFriction
        Bxpos2 += Friction + NegativeFriction

        ResetCoM() 'updates the centre of mass


    End Sub

    Sub Draw(g As Graphics, ColourPen As Pen) 'draws the body
        DrawX1 = BPx1 - Form1.FurthestAnimalPos 'moves the position back by the amount the camera has moved forwards to keep the creature in perspective to the movement of the camera
        DrawX2 = BPx2 - Form1.FurthestAnimalPos
        g.DrawLine(ColourPen, Convert.ToInt32(DrawX1), Convert.ToInt32(BPy1), Convert.ToInt32(DrawX2), Convert.ToInt32(BPy2))

    End Sub


End Class