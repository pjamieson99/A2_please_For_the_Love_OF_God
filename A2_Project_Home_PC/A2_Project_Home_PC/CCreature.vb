Public Class CCreature
    Public NumOfLegs As Integer
    Public NumOfLayers As Integer
    Public Body As CBody
    Public line(NumOfLegs - 1, NumOfLayers - 1) As CLeg
    Public Joint(NumOfLegs - 1, NumOfLayers - 1) As CJoint
    Dim Floor As CFloor
    Public OrigBody As CBody
    Public OrigLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg
    Public OrigJoint(NumOfLegs - 1, NumOfLayers - 1) As CJoint
    Public Fitness As Double
    Public DeadCounter As Integer
    Public OC As New List(Of PointF)
    Public TimeCounter As Integer
    Private SpeciesColours As New List(Of Color) From {Color.Red, Color.Orange, Color.Green, Color.Yellow, Color.Blue, Color.Indigo, Color.Violet, Color.White, Color.Chartreuse}

    Sub New(B As CBody, ByVal L(,) As CLeg, ByVal J(,) As CJoint, ByVal F As CFloor)
        NumOfLayers = Form1.NumOfLayers
        NumOfLegs = Form1.NumOfLegs

        Body = B
        line = L
        Joint = J
        Floor = F


        For x = 0 To NumOfLegs - 1

            Body.BodyPoints(line(x, 0).LP1)


        Next


        Dim OrigConnections As New List(Of PointF)(B.Connections)

        OC = OrigConnections
        OrigBody = B.Clone()
        OrigConnections = OrigBody.Clone().Connections
        OrigLine = line.Clone
        OrigJoint = Joint.Clone


        Fitness = 0




    End Sub

    Sub StartPos(ByVal Body, ByVal Line, ByVal Joint, ByVal floor)

    End Sub

    Sub Reset()
        OrigBody.Connections = OC
        Body = OrigBody.Clone()
        line = OrigLine.Clone()
        Joint = OrigJoint.Clone()
        TimeCounter = 0
        DeadCounter = 0
        Dim OrigConnections As New List(Of PointF)(Body.Connections)

        OC = OrigConnections
    End Sub

    Sub NextMove(g As Graphics)
        If Body.Connections(0).X <> 100 Then
            Console.ReadLine()
        End If
        TimeCounter += 1

        ' Set the colour based on the amount of legs
        Dim ColourBrush As New Pen(SpeciesColours(NumOfLegs - 1))



        'Sets previous points to variable used to calculate friction
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                line(x, y).OldPoint1 = line(x, y).LP1
                line(x, y).OldPoint2 = line(x, y).LP2
            Next
        Next

        'Find the side of the point touching the floor compared to the centre of mass
        Body.Rightside = False
        Body.Leftside = False
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1

                line(x, y).FindSideLegisOn(Body, Floor)

            Next
        Next




        'Find the pivot point for the body
        Body.Pivot.Y = 0
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1

                Body.FindPivot(line(x, y), Floor)

            Next
        Next

        'make the body fall over based on the pivot
        Body.FallOver(g, line)


        'check if max angle of legs has been reached then rotate legs.
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1

                line(x, y).AngleLock(Floor)
                line(x, y).NewPoints()

            Next
        Next



        'draw the floor


        'attach the legs to the body
        AttachLegs()

        'Find lowest leg past the floor
        Body.BodyRise = 0
        For x = 0 To NumOfLegs - 1
            line(x, 1).FindLowestLeg(line(x, 0), Body.BodyRise)
        Next




        'raise the body by the distance between the furthest leg and the floor
        Body.RaiseBody(Floor)


        'attach the legs to the body
        AttachLegs()



        'drop the body
        Body.Drop()




        'check if hit floor and if so stop falling
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                line(x, y).StopFalling(Body, Floor)
            Next
        Next



        'attach the legs
        AttachLegs()

        'find the furthest leg past the floor
        Body.BodyRise = 0
        For x = 0 To NumOfLegs - 1

            line(x, 0).Pivot = New Point(0, 0)
            line(x, 1).Pivot = New Point(0, 0)
            line(x, 1).FindLowestLeg(line(x, 0), Body.BodyRise)

        Next

        'raise the body by the distance between the furthest leg and the floor
        Body.RaiseBody(Floor)







        'attach the legs to the body
        AttachLegs()

        'Find the frictional value based on the different frictions from all legs
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                Body.FindFriction(line(x, y), Floor)
            Next
        Next

        'add the friction to the body that moves it by the distance moved by the leg in sum.
        Body.AddFriction()

        'attach the legs to the body
        AttachLegs()





        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                line(x, y).Draw(g, ColourBrush)
                Joint(x, y).StickToLeg(line(x, y).DrawPoint1)
                Joint(x, y).Draw(g, ColourBrush)
            Next
        Next
        Body.Draw(g, ColourBrush)

        IsAlive()

        If Body.Connections(0).X <> 100 Then
            Console.ReadLine()
        End If
    End Sub

    Sub AttachLegs()
        'attach top legs to body
        For x = 0 To NumOfLegs - 1
            Body.SetPoints(line(x, 0), x)
        Next

        'attach bottom legs to top legs
        For x = 0 To NumOfLegs - 1
            line(x, 1).AttachBottomLegs(x, Body.Connections(x), line(x, 0))
        Next
    End Sub

    Sub IsAlive()
        If Body.CoM.X <= Body.PrevCoM.X Then
            DeadCounter += 1
        Else
            DeadCounter = 0
            Body.PrevCoM = Body.CoM
        End If

        If DeadCounter = 100 Then
            Console.ReadLine()
        End If
    End Sub

End Class