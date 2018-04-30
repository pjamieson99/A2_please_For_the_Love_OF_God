Public Class Form1
    'This is the main form, It focuses on creating and organising everything while initiating the program

    Public Floor As CFloor 'floor of the world

    Dim NumOfAnimals As Integer = 3 'sets the number of animals and body parts
    Public NumOfLegs As Integer
    Public NumOfLayers As Integer = 2

    Dim NewBody As CBody 'sets newbody which is the body variable for offspring
    Dim Base(NumOfAnimals - 1) As CBody 'sets base which is the body of the creature being created at the start

    Dim BodyJoints(NumOfAnimals - 1) 'this variable is used to copy joints without updating the original joints value when the values  change
    Dim BodyLines(NumOfAnimals - 1) 'this bariable is used to copy legs without updating the original legs when the values change

    Public BestAnimal As CCreature 'this variable is assignes to the best animal of moving left ro right when it is called
    Public BestAnimalGeneration As Integer
    Dim Animal As New List(Of CCreature) 'this is a list of all current animals being used

    Dim Rnd As New Random 'allows creation of random values

    Dim TotalDistance As Double 'variable that records total distance moved by all animals of that generation

    Dim StartPos(14) As Integer 'these are used for the markers, and are used for the starting positions, the current positions, and the position relative to the creature the camera focuses on
    Dim CurrentPos(14) As Integer
    Dim CurrentStartPos(14) As Integer

    Dim Distance(14) As Integer 'these are the values displayed under the markers
    Dim DistanceNumber As Integer = 1

    Dim Generation As Integer = 1 'sets the current generation of the creatures

    Public SpeedTime As Boolean = True 'variable determines whether the program bothers to go through the timer
    Public RealSpeedTime As Boolean = True 'used to carry the value of speedtime when it is temporarily enabled when the menu is open to pause

    Dim MutationRate As Double = 15 'the percentage chance of a mutation occuring

    Dim PrevLegY As Integer 'part of leg above the bottom leg's position is set to thes variables
    Dim PrevLegX As Integer

    Public FurthestAnimalPos As Double = 0 'vairable finds the position of the furthest animal and the movement for the camera due to this



    Public CreatureDatabase As Database 'this is the database that retrieves and stores the creatures

    Public FindBestAnimal As Boolean = False 'chooses whether to run the best animal or continue with the current generation

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If NumOfAnimals = 2 Then
            NumOfAnimals = 4
        End If

        CreatureDatabase = New Database()

        CreatureDatabase.FindPrevIDs() 'looks in the database and finds the latest ID to allow the program to add on to the databse instead of overwrite

        SetMarkers() 'sets markers that will be used to show the distance a creature is travelling to the user.

        Floor = New CFloor(400)
        CreateAnimals() 'creates all the starting creatures 
    End Sub

    Sub SetMarkers()
        For x = 0 To Display.Width / 100 - 1
            Distance(x) = DistanceNumber 'creates markers to show a specific distance moved on the map
            DistanceNumber += 1

            StartPos(x) = 100 * x 'sets starting positions for the markers
            CurrentStartPos(x) = 100 * x
        Next

    End Sub

    Sub CreateAnimals()
        For n = 0 To NumOfAnimals - 1
            NumOfLegs = Rnd.Next(2, 10)
            Base(n) = New CBody(100, 50, Rnd.Next(120, 500), 50, NumOfLegs) ' creates a body with a random number of legs and length

            Dim Line(NumOfLegs - 1, NumOfLayers - 1) As CLeg 'creates a new line and joint with the new number of legs and layers
            Dim Joint(NumOfLegs - 1, NumOfLayers - 1) As CJoint

            For y = 0 To NumOfLayers - 1 ' creates legs and joints
                For x = 0 To NumOfLegs - 1
                    If y > 0 Then
                        PrevLegY = y - 1 ' gets leg above it's position to find length
                        PrevLegX = x
                        Line(x, y) = New CLeg(100 + Base(n).Diameter / (NumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, 1, 1, 1, 1, 1, Rnd)
                    Else
                        Line(x, y) = New CLeg(100 + Base(n).Diameter / (NumOfLegs - 1) * x, 50 + 100 * y, 1, 1, 1, 1, 1, Rnd)
                    End If
                    Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
                Next
            Next

            Dim TempLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg ' copies lines to clone vairables to avoid updating the original lines and body
            TempLine = Line.Clone()
            BodyLines(n) = TempLine.Clone
            BodyJoints(n) = Joint

            Animal.Add(New CCreature(Base(n), BodyLines(n), BodyJoints(n), Floor, CreatureDatabase.CreatureID, Generation)) ' creates the creature and writes it to the database
            CreatureDatabase.WriteCreature(Animal(n), Animal(n).Generation, Animal(n).NumOfLegs, Animal(n).NumOfLayers, CreatureDatabase.CreatureID)
            CreatureDatabase.CreatureID += 1
        Next
    End Sub

    Sub DrawBackground(g As Graphics, gen As Integer) 'draw the background
        Floor.Draw(g) 'draw the floor 
        g.FillRectangle(Brushes.Green, 0, Floor.ypos, Display.Width, Display.Height - Floor.ypos) 'create background colour
        g.FillRectangle(Brushes.SkyBlue, 0, 0, Display.Width, Floor.ypos)
        g.DrawString("Generation: " & gen, DefaultFont, Brushes.Black, New Point(0, 0)) 'show generation number
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick ' runs through program on repeat to create movement
        Display.Refresh()
    End Sub

    Private Sub Display_Paint(sender As Object, e As PaintEventArgs) Handles Display.Paint

        Dim g As Graphics
        g = e.Graphics

        Dim AnimalAlive As Boolean = False
        FurthestAnimalPos = 0

        If FindBestAnimal = True Then 'if the user has selected show best creature run the best creature
            DrawBackground(g, BestAnimalGeneration) 'draw background
            If BestAnimal.TimeCounter = 0 Then
                BestAnimal.Body.PrevCom.X = 0
                BestAnimal.Body.PrevCom.Y = 0

            End If
            If BestAnimal.TimeCounter < 500 And BestAnimal.DeadCounter < 125 Then 'if the animal is not dead then make its next move


                BestAnimal.NextMove(g)
                AnimalAlive = True
            Else
                FindBestAnimal = False
            End If

            FollowFurthestAnimal(g) 'keep the camera on the furthest animal

            If Not SpeedTime Then
                Display.Refresh() 'if speed up button is pressed, make time go as quick as possible - avoid timer
            End If

        Else
            DrawBackground(g, Generation) 'draw background

            For x = 0 To NumOfAnimals - 1
                If Animal(x).TimeCounter < 500 And Animal(x).DeadCounter < 125 Then 'if the animal is not dead then make its next move
                    Animal(x).NextMove(g)
                    AnimalAlive = True
                End If
            Next

            FollowFurthestAnimal(g) 'keep the camera focused on the furthest animal

            If Not AnimalAlive Then
                Generation += 1
                GeneticAlgorithm() 'complete genetic algorithm process 

                DistanceNumber = 1 ' reset the camera back to starting position
                For x = 0 To Display.Width / 100 - 1
                    Distance(x) = DistanceNumber
                    DistanceNumber += 1
                    CurrentStartPos(x) = StartPos(x)
                Next
            End If

        End If
        If Not SpeedTime Then
            Display.Refresh()
        End If
    End Sub



    Sub FollowFurthestAnimal(g As Graphics) ' keep camera focused on furthest moved animal by moving the distance markers
        For x = 0 To Display.Width / 100 - 1
            CurrentPos(x) = CurrentStartPos(x) - FurthestAnimalPos

            If CurrentPos(x) < 0 Then ' move marker to other side and make it count on from the next furthest marker
                CurrentPos(x) = Display.Width + CurrentPos(x)
                Distance(x) = Distance(x) + (CurrentPos.Count)
                CurrentStartPos(x) = CurrentPos(x) + FurthestAnimalPos
            ElseIf CurrentPos(x) > Display.Width Then ' move marker by distance animal moved
                CurrentPos(x) -= Display.Width
                Distance(x) = Distance(x) - (CurrentPos.Count)
                CurrentStartPos(x) = CurrentPos(x) + FurthestAnimalPos
            End If

            g.DrawLine(Pens.Black, New Point(CurrentPos(x), Floor.ypos), New Point(CurrentPos(x), Floor.ypos + 100)) 'draw the markers
            g.DrawString(Convert.ToString(Distance(x)), DefaultFont, Brushes.Black, New Point(CurrentPos(x), Floor.ypos + 103))
        Next
    End Sub

    Sub GeneticAlgorithm() ' completes the genetic algorithm
        AddDistance() 'records distance moved by all animals in database
        FindFitness() 'assigns fitness absed on distance moved
        KillWeakest() 'kills the weakest half of the population
        CrossOver() 'make remaining population reproduce
        For Each Creature In Animal ' reset creatures back to starting position
            Creature.Reset()
        Next
    End Sub

    Sub AddDistance()
        For x = 0 To NumOfAnimals - 1 ' records distance moved
            CreatureDatabase.QueryDatabase("UPDATE Creatures SET Distance = " & Animal(x).Body.CoM.X & " WHERE Creature_ID = " & Animal(x).ID, True)
        Next
    End Sub

    Sub FindFitness()
        TotalDistance = 0

        For n = 0 To NumOfAnimals - 1 'find total distance moved by all animals
            If Animal(n).Body.CoM.X > 0 Then
                TotalDistance += Animal(n).Body.CoM.X
            End If
        Next

        For n = 0 To NumOfAnimals - 1 'give all animals a percent of the total distance moved
            If Animal(n).Body.CoM.X > 0 Then
                Animal(n).Fitness = Animal(n).Body.CoM.X / TotalDistance * 100
            Else
                Animal(n).Fitness = 0
            End If
        Next

    End Sub

    Sub KillWeakest()
        Dim DeathPool As New List(Of CCreature)
        Dim ToDie As New List(Of CCreature)

        For X = 0 To NumOfAnimals - 1 'give animals a percent chance of surviving based on fitness
            For Y = 0 To Math.Min(100, (Animal(X).Fitness ^ (-1) * 100))
                DeathPool.Add(Animal(X))
            Next
        Next

        For x = 0 To NumOfAnimals / 2 - 1 ' choose half of population to remove.
            Dim SelectedCreature As CCreature = DeathPool(Math.Floor(Rnd.Next(DeathPool.Count)))
            While ToDie.Contains(SelectedCreature)
                SelectedCreature = DeathPool(Math.Floor(Rnd.Next(DeathPool.Count)))
            End While
            ToDie.Add(SelectedCreature)
        Next

        For Each DeadAnimal In ToDie 'remove dead animals
            Animal.Remove(DeadAnimal)
        Next
    End Sub

    Sub FindFurthestAnimal() 'find the furthest animal position movement from the previous move
        If FindBestAnimal = True Then 'follows the best animal specifically if the best animal is being shown
            If FurthestAnimalPos < BestAnimal.Body.CoM.X And BestAnimal.Body.CoM.X > 700 Then
                FurthestAnimalPos = BestAnimal.Body.CoM.X
            End If
        Else
            For x = 0 To NumOfAnimals - 1 'find the furthest animal
                If FurthestAnimalPos < Animal(x).Body.CoM.X And Animal(x).Body.CoM.X > 700 Then
                    FurthestAnimalPos = Animal(x).Body.CoM.X
                End If
            Next
        End If

        If FurthestAnimalPos <> 0 Then 'sets distance to the movement from the previous position
            FurthestAnimalPos -= 700
        End If
    End Sub

    Sub CrossOver()
        Dim MatingPool As New List(Of CCreature)

        For X = 0 To Animal.Count - 1 'give all animals a percent chance of mating based on fitness
            For Y = 0 To Animal(X).Fitness
                MatingPool.Add(Animal(X))
            Next
        Next

        For I = 0 To NumOfAnimals / 2 - 1

            Dim MotherIndex As Integer = 0
            Dim FatherIndex As Integer = 0
            Dim Parents As New List(Of CCreature)

            While MatingPool(MotherIndex) Is MatingPool(FatherIndex) 'choose mother and father
                MotherIndex = Math.Floor(Rnd.Next(MatingPool.Count))
                FatherIndex = Math.Floor(Rnd.Next(MatingPool.Count))
            End While

            Parents.Add(MatingPool(MotherIndex)) 'set mother and father as parents then start making child
            Parents.Add(MatingPool(FatherIndex))

            Dim NewNumOfLegs As Integer = Parents(Rnd.Next(Parents.Count)).NumOfLegs 'set number of legs to one of parents or random if mutated
            If Rnd.Next(101) < MutationRate Then
                NewNumOfLegs = Rnd.Next(2, 10)
            End If

            Dim NewBodyLength As Integer = Parents(Rnd.Next(Parents.Count)).Body.Diameter 'set the length of the body to one of parents or random if mutated
            If Rnd.Next(101) < MutationRate Then
                NewBodyLength = Rnd.Next(120, 500)
            End If

            NewBody = New CBody(100, 50, 100 + NewBodyLength, 50, NewNumOfLegs) 'set the new body

            Dim Line(NewNumOfLegs - 1, NumOfLayers - 1) As CLeg
            Dim Joint(NewNumOfLegs - 1, NumOfLayers - 1) As CJoint

            For y = 0 To NumOfLayers - 1

                For x = 0 To NewNumOfLegs - 1
                    Dim NewSpeed As Integer
                    Dim NewAngle As Integer
                    Dim NewClock As Integer
                    Dim NewGTClock As Integer
                    Dim NewDiameter As Integer

                    If Parents(0).NumOfLegs - 1 >= x And Parents(1).NumOfLegs - 1 >= x Then 'set new leg properties
                        NewSpeed = Parents(Rnd.Next(Parents.Count)).line(x, y).Speed
                        NewAngle = Parents(Rnd.Next(Parents.Count)).line(x, y).Angle
                        NewClock = Parents(Rnd.Next(Parents.Count)).line(x, y).Clock
                        NewGTClock = Parents(Rnd.Next(Parents.Count)).line(x, y).GoThroughClock
                        NewDiameter = Parents(Rnd.Next(Parents.Count)).line(x, y).Diameter
                    ElseIf Parents(0).NumOfLegs - 1 < x And Parents(1).NumOfLegs - 1 >= x Then
                        NewSpeed = Parents(1).line(x, y).Speed
                        NewAngle = Parents(1).line(x, y).Angle
                        NewClock = Parents(1).line(x, y).Clock
                        NewGTClock = Parents(1).line(x, y).GoThroughClock
                        NewDiameter = Parents(1).line(x, y).Diameter
                    ElseIf Parents(1).NumOfLegs - 1 < x And Parents(0).NumOfLegs - 1 >= x Then
                        NewSpeed = Parents(0).line(x, y).Speed
                        NewAngle = Parents(0).line(x, y).Angle
                        NewClock = Parents(0).line(x, y).Clock
                        NewGTClock = Parents(0).line(x, y).GoThroughClock
                        NewDiameter = Parents(0).line(x, y).Diameter
                    Else
                        NewSpeed = Rnd.Next(0, 10)
                        NewAngle = Rnd.Next(40, 200)
                        NewClock = Rnd.Next(0, 50)
                        NewGTClock = Rnd.Next(0, 50)
                        NewDiameter = Rnd.Next(30, 150)
                    End If

                    If Rnd.Next(101) < MutationRate Then 'give chance of mutations on all properties
                        NewSpeed = Math.Abs(Rnd.Next(-11, -1))
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewAngle = Rnd.Next(-180, -40)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewClock = Rnd.Next(0, 50)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewGTClock = Rnd.Next(0, 50)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewDiameter = Rnd.Next(30, 150)
                    End If

                    If y > 0 Then 'set the new legs 
                        PrevLegY = y - 1 'find the leg above this one to connect them
                        PrevLegX = x
                        Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, NewSpeed, NewClock, NewAngle, NewGTClock, NewDiameter, Rnd)
                    End If
                    Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + 100 * y, NewSpeed, NewClock, NewAngle, NewGTClock, NewDiameter)
                    Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
                Next

            Next

            Dim TempLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg
            TempLine = Line.Clone() 'clone the line to avoid updating it
            NumOfLegs = NewNumOfLegs

            Animal.Add(New CCreature(NewBody, Line.Clone(), Joint.Clone(), Floor, CreatureDatabase.CreatureID, Generation)) 'create the animal
            Animal(Animal.Count - 1).OrigLine = TempLine

            CreatureDatabase.WriteCreature(Animal(Animal.Count - 1), Animal(Animal.Count - 1).Generation, Animal(Animal.Count - 1).NumOfLegs, Animal(Animal.Count - 1).NumOfLayers, CreatureDatabase.CreatureID) 'add the animal to the database
            CreatureDatabase.CreatureID += 1
        Next

    End Sub

    Private Sub Menu_Box_Click(sender As Object, e As EventArgs) Handles Menu_Box.Click 'if the user clicks the menu
        Dim DialogForm As Form = New Menu_Form 'pause the program and show the menu

        RealSpeedTime = SpeedTime 'pause program and then show the dialogue
        If SpeedTime = False Then 'makes sure it pauses even when it is sped up and skips the timer by temporarily putting it on the slow down mode to pause it then speed it back up after
            SpeedTime = True
            Timer1.Stop()
            DialogForm.ShowDialog()
            Timer1.Start()
            SpeedTime = False
        Else
            Timer1.Stop()
            DialogForm.ShowDialog()
            Timer1.Start()
        End If
        SpeedTime = RealSpeedTime
    End Sub

End Class