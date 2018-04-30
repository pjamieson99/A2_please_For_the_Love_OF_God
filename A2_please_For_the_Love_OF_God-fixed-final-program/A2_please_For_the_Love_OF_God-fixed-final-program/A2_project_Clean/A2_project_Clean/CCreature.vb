Public Class CCreature
    Public NumOfLegs As Integer 'sets the number of legs the creature has
    Public NumOfLayers As Integer = 2

    Public Body As CBody 'gives the body, legs and joints as objects of the creature
    Public line(NumOfLegs - 1, NumOfLayers - 1) As CLeg
    Public Joint(NumOfLegs - 1, NumOfLayers - 1) As CJoint

    Dim Floor As CFloor 'passes floor class through to be used in the interaction between the creature and the ground

    Public OrigBody As CBody 'these variables keep record of the original values of the body parts of the creature so it can be reset
    Public OrigLine(Form1.NumOfLegs - 1, Form1.NumOfLayers - 1) As CLeg
    Public OrigJoint(NumOfLegs - 1, NumOfLayers - 1) As CJoint
    Public OrigConnections As New List(Of PointF)
    Dim OC As New List(Of PointF)

    Public Fitness As Double 'this is the fitness value of the creature that determines its chance of survival and reproduction

    Public DeadCounter As Integer 'this keeps track of whether the creature is making progress, to kill it off if it doesnt make progress within a set time

    Public TimeCounter As Integer 'this keeps track of the time limit that the creature has to move
    Private SpeciesColours As New List(Of Color) From {Color.Red, Color.Orange, Color.Green, Color.Yellow, Color.Blue, Color.Indigo, Color.Violet, Color.White, Color.Chartreuse} 'this variable is to determine the colour of the animal based on which species it is
    Public ID As Integer 'This is the creature's ID for the database
    Public Generation As Integer 'this is the generation number that it is
    Public ColourBrush As Pen 'this is a tool used to determine the colour of the brush


    Sub New(B As CBody, ByVal L(,) As CLeg, ByVal J(,) As CJoint, ByVal F As CFloor, CID As Integer, g As Integer)
        NumOfLayers = Form1.NumOfLayers 'set the number of legs the creature has
        NumOfLegs = Form1.NumOfLegs

        Body = B 'assigns the body part values
        line = L
        Joint = J
        Floor = F

        ID = CID 'sets the generation and id of the creature
        Generation = g

        For x = 0 To NumOfLegs - 1 'create the connections between the body and the legs
            Body.BodyPoints(line(x, 0).LP1)
        Next


        For y = 0 To NumOfLayers - 1 'sets the original values of the animal so it can be reset to those values
            For x = 0 To NumOfLegs - 1
                OrigLine(x, y) = line(x, y).Clone()
            Next
        Next
        Dim OC As New List(Of PointF)(B.Connections)
        OrigConnections = OC
        OrigBody = B.Clone()
        OC = OrigBody.Clone().Connections
        OrigJoint = Joint.Clone

        Fitness = 0 'sets the fitness value to be used later in determining likelihood to survive and reproduce
        ColourBrush = New Pen(SpeciesColours(NumOfLegs - 1)) 'assigns the colour to the creature depending on the number of legs(species) it has


    End Sub

    Sub Reset() 'resets the creature to its original values
        OrigBody.Connections = OrigConnections 'sets the original body parts
        Body = OrigBody.Clone()
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                line(x, y) = OrigLine(x, y).Clone()
            Next
        Next
        OC = New List(Of PointF)(Body.Connections)
        OrigConnections = OC
        Joint = OrigJoint.Clone()

        TimeCounter = 0 'resets the counters that determine whethe it is dead
        DeadCounter = 0
    End Sub

    Sub NextMove(g As Graphics) 'performs next frame of animal movement
        TimeCounter += 1 'increases the time counting how long the creature has been alive

        For y = 0 To NumOfLayers - 1 'Sets previous points to variable used to calculate friction
            For x = 0 To NumOfLegs - 1
                line(x, y).OldPoint1 = New Point(line(x, y).LPx1, line(x, y).LPy1)
                line(x, y).OldPoint2 = New Point(line(x, y).LPx2, line(x, y).LPy2)
            Next
        Next

        Body.Rightside = False 'Find the side of the point touching the floor compared to the centre of mass
        Body.Leftside = False
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                line(x, y).FindSideLegisOn(Body, Floor)
            Next
        Next

        Body.Pivot.Y = 0  'Find the pivot point for the body
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                Body.FindPivot(line(x, y), Floor)
            Next
        Next
        Body.FallOver(g, line) 'make the body fall over based on the pivot



        For y = 0 To NumOfLayers - 1 'check if max angle of legs has been reached then rotate legs.
            For x = 0 To NumOfLegs - 1
                line(x, y).AngleLock(Floor)
                line(x, y).NewPoints()


            Next
        Next

        AttachLegs() 'attach the legs to the body

        Body.BodyRise = 0 'Find lowest leg past the floor
        For x = 0 To NumOfLegs - 1
            line(x, 1).FindLowestLeg(line(x, 0), Body.BodyRise)
        Next

        Body.RaiseBody(Floor) 'raise the body by the distance between the furthest leg and the floor

        AttachLegs() 'attach the legs to the body

        Body.Drop() 'drop the body

        For y = 0 To NumOfLayers - 1 'check if the legs have hit the floor and if so stop falling
            For x = 0 To NumOfLegs - 1
                line(x, y).StopFalling(Body, Floor)
            Next
        Next

        AttachLegs() 'attach the legs

        Body.BodyRise = 0 'find the furthest leg past the floor
        For x = 0 To NumOfLegs - 1
            line(x, 0).Pivot = New Point(0, 0)
            line(x, 1).Pivot = New Point(0, 0)
            line(x, 1).FindLowestLeg(line(x, 0), Body.BodyRise)
        Next

        Body.RaiseBody(Floor) 'raise the body by the distance between the furthest leg and the floor

        AttachLegs() 'attach the legs to the body

        Body.Friction = 0 'Find the frictional value based on the different frictions from all legs
        Body.NegativeFriction = 0
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                Body.FindFriction(line(x, y), Floor)
            Next
        Next

        Body.AddFriction() 'add the friction to the body that moves it by the distance moved by the leg in sum.


        AttachLegs() 'attach the legs to the body

        Form1.FindFurthestAnimal() 'finds the furthest animal forwards to be used to keep the camera focused on them
        Draw(ColourBrush, g) 'draws the animal

        IsAlive() 'updates whether the creature has made progress 
    End Sub

    Sub AttachLegs()

        For x = 0 To NumOfLegs - 1 'attach top legs to body
            Body.SetPoints(line(x, 0), x)
        Next

        For x = 0 To NumOfLegs - 1 'attach bottom legs to top legs
            line(x, 1).AttachBottomLegs(x, Body.Connections(x), line(x, 0), line(x, 1).Diameter)
        Next
    End Sub

    Sub IsAlive() 'updates whether the creature has made progress and if so counts towards the time limit before they die
        If Body.CoM.X <= Body.PrevCom.X Then 'if no progress made count towards death
            DeadCounter += 1
        Else 'if progress made reset this timer
            DeadCounter = 0
            Body.PrevCom = Body.CoM
        End If


    End Sub


    Sub Draw(ColourBrush As Pen, g As Graphics) 'draws the creature
        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1 'draws the different body parts
                line(x, y).Draw(g, ColourBrush)
                Joint(x, y).StickToLeg(line(x, y).DrawPoint1)
                Joint(x, y).Draw(g, ColourBrush)
            Next
        Next
        Body.Draw(g, ColourBrush)
    End Sub
End Class