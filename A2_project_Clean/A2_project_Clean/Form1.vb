Public Class Form1
    Dim Floor As CFloor
    Dim NumOfAnimals As Integer = 100
    Dim Base(NumOfAnimals - 1) As CBody
    Dim checking As Boolean
    Public NumOfLegs As Integer = 4
    Public NumOfLayers As Integer = 2

    Dim Joint(NumOfLegs - 1, NumOfLayers - 1) As CJoint
    Dim BodyLines(NumOfAnimals - 1)
    Dim BodyJoints(NumOfAnimals - 1)
    Dim Rnd As New Random
    Dim TotalDistance As Double
    Dim Animal As New List(Of CCreature)
    Dim ChosenParents(1) As Integer
    Dim PrevAnimal As New List(Of CCreature)

    Dim StartPos(14) As Integer
    Dim CurrentPos(14) As Integer
    Dim CurrentStartPos(14) As Integer
    Dim Generation As Integer = 1
    Dim Drawing As Boolean = True
    Dim MutationRate As Double = 10
    Dim PrevLegY As Integer
    Dim PrevLegX As Integer
    Public FurthestAnimalPos As Double = 0
    Dim NewBody As CBody
    Dim Distance(14) As Integer
    Dim Count As Integer = 1



    'Dim PrevAnimal As New List(Of CCreature)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For x = 0 To Display.Width / 100 - 1
            Distance(x) = Count
            Count += 1

        Next

        'create floor object
        Floor = New CFloor(400)

        For x = 0 To Display.Width / 100 - 1
            StartPos(x) = 100 * x
            CurrentStartPos(x) = 100 * x

        Next

        For n = 0 To NumOfAnimals - 1
            NumOfLegs = Rnd.Next(2, 10)
            Base(n) = New CBody(100, 50, Rnd.Next(120, 500), 50, NumOfLegs)

            Dim Line(NumOfLegs - 1, NumOfLayers - 1) As CLeg
            Dim Joint(NumOfLegs - 1, NumOfLayers - 1) As CJoint
            For y = 0 To NumOfLayers - 1
                For x = 0 To NumOfLegs - 1
                    If y > 0 Then
                        If x = 0 Then
                            PrevLegY = y - 1
                            PrevLegX = NumOfLegs - 1
                        Else
                            PrevLegY = y
                            PrevLegX = x - 1
                        End If

                        Line(x, y) = New CLeg(100 + Base(n).Diameter / (NumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, 1, 1, 1, 1, Rnd)

                    Else
                        Line(x, y) = New CLeg(100 + Base(n).Diameter / (NumOfLegs - 1) * x, 50 + 100 * y, 1, 1, 1, 1, Rnd)

                    End If


                    Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)

                Next
            Next
            Dim TempLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg
            TempLine = Line.Clone()
            BodyLines(n) = TempLine
            BodyJoints(n) = Joint

            Animal.Add(New CCreature(Base(n), BodyLines(n), BodyJoints(n), Floor))

        Next




    End Sub




    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'keep display refreshing, loop through display
        Display.Refresh()
    End Sub




    Private Sub Display_Paint(sender As Object, e As PaintEventArgs) Handles Display.Paint

        Dim g As Graphics
        g = e.Graphics
        Floor.Draw(g)

        g.FillRectangle(Brushes.Green, 0, Floor.ypos, Display.Width, Display.Height - Floor.ypos)
        g.FillRectangle(Brushes.SkyBlue, 0, 0, Display.Width, Floor.ypos)

        g.DrawString("Generation: " & Generation, DefaultFont, Brushes.Black, New Point(0, 0))

        Dim AnimalAlive As Boolean = False
        FurthestAnimalPos = 0

        For x = 0 To NumOfAnimals - 1
            If FurthestAnimalPos < Animal(x).Body.CoM.X And Animal(x).Body.CoM.X > 700 Then
                FurthestAnimalPos = Animal(x).Body.CoM.X
            End If
        Next
        If FurthestAnimalPos <> 0 Then
            FurthestAnimalPos -= 700
        End If
        For x = 0 To NumOfAnimals - 1
            If Animal(x).TimeCounter < 500 And Animal(x).DeadCounter < 125 Then


                Animal(x).NextMove(g)
                AnimalAlive = True

            End If

        Next
        For x = 0 To Display.Width / 100 - 1
            CurrentPos(x) = CurrentStartPos(x) - FurthestAnimalPos

            If CurrentPos(x) < 0 Then
                CurrentPos(x) = Display.Width + CurrentPos(x)

                Distance(x) = Distance(x) + (CurrentPos.Count)
                CurrentStartPos(x) = CurrentPos(x) + FurthestAnimalPos
            ElseIf CurrentPos(x) > Display.Width Then
                CurrentPos(x) -= Display.Width
                Distance(x) = Distance(x) - (CurrentPos.Count)

                CurrentStartPos(x) = CurrentPos(x) + FurthestAnimalPos
            End If
            g.DrawLine(Pens.Black, New Point(CurrentPos(x), Floor.ypos), New Point(CurrentPos(x), Floor.ypos + 100))
            g.DrawString(Convert.ToString(Distance(x)), DefaultFont, Brushes.Black, New Point(CurrentPos(x), Floor.ypos + 103))
        Next

        If Not AnimalAlive Then
            GeneticAlgorithm()
            'genetic algorithm
            Generation += 1
            Count = 1
            For x = 0 To Display.Width / 100 - 1
                Distance(x) = Count
                Count += 1
                CurrentStartPos(x) = StartPos(x)
            Next
        End If




        If Not Drawing Then
            Display.Refresh()
        End If


    End Sub




    Function CheckFloor(floor As CFloor, point As Double)

        If floor.ypos - 1 <= point Then
            Return True
        Else
            Return False
        End If
    End Function



    Sub GeneticAlgorithm()
        FindFitness()
        KillWeakest()
        CrossOver()
        For Each Creature In Animal
            Creature.Reset()
        Next
    End Sub

    Sub KillWeakest()
        Dim DeathPool As New List(Of CCreature)

        For X = 0 To NumOfAnimals - 1
            For Y = 0 To Math.Min(100, (Animal(X).Fitness ^ (-1) * 100))
                DeathPool.Add(Animal(X))
            Next
        Next

        Dim ToDie As New List(Of CCreature)

        For x = 0 To NumOfAnimals / 2 - 1
            Dim SelectedCreature As CCreature = DeathPool(Math.Floor(Rnd.Next(DeathPool.Count)))
            While ToDie.Contains(SelectedCreature)
                SelectedCreature = DeathPool(Math.Floor(Rnd.Next(DeathPool.Count)))
            End While
            ToDie.Add(SelectedCreature)
        Next

        For Each DeadAnimal In ToDie
            Animal.Remove(DeadAnimal)
        Next


    End Sub


    Sub FindFitness()
        TotalDistance = 0
        For n = 0 To NumOfAnimals - 1
            If Animal(n).Body.CoM.X > 0 Then
                TotalDistance += Animal(n).Body.CoM.X
            End If
        Next
        For n = 0 To NumOfAnimals - 1
            If Animal(n).Body.CoM.X > 0 Then
                Animal(n).Fitness = Animal(n).Body.CoM.X / TotalDistance * 100
            Else
                Animal(n).Fitness = 0
            End If


        Next

    End Sub


    'Sub CrossOver()
    '    Dim MatingPool As New List(Of CCreature)

    '    For X = 0 To Animal.Count - 1
    '        For Y = 0 To Animal(X).Fitness
    '            MatingPool.Add(Animal(X))
    '        Next
    '    Next

    '    For I = 0 To NumOfAnimals / 2 - 1
    '        Dim MotherIndex As Integer = 0
    '        Dim FatherIndex As Integer = 0
    '        Dim Parents As New List(Of CCreature)
    '        While MotherIndex = FatherIndex
    '            MotherIndex = Math.Floor(Rnd.Next(MatingPool.Count))
    '            FatherIndex = Math.Floor(Rnd.Next(MatingPool.Count))
    '        End While
    '        Parents.Add(MatingPool(MotherIndex))
    '        Parents.Add(MatingPool(FatherIndex))




    '        Dim NewNumOfLegs As Integer = Parents(Rnd.Next(Parents.Count)).NumOfLegs
    '        If Rnd.Next(101) < MutationRate Then
    '            NewNumOfLegs = Rnd.Next(2, 10)
    '        End If

    '        Dim NewBodyLength As Integer = (Parents(0).Body.Diameter + Parents(1).Body.Diameter) / 2
    '        If Rnd.Next(101) < MutationRate Then
    '            NewBodyLength = Rnd.Next(120, 500)
    '        End If

    '        NewBody = New CBody(100, 50, 100 + NewBodyLength, 50, NewNumOfLegs)

    '        Dim Line(NewNumOfLegs - 1, NumOfLayers - 1) As CLeg
    '        Dim Joint(NewNumOfLegs - 1, NumOfLayers - 1) As CJoint
    '        For y = 0 To NumOfLayers - 1
    '            For x = 0 To NewNumOfLegs - 1
    '                Dim NewSpeed As Integer
    '                Dim NewAngle As Integer
    '                Dim NewClock As Integer
    '                Dim NewGTClock As Integer
    '                If Parents(0).NumOfLegs - 1 >= x And Parents(1).NumOfLegs - 1 >= x Then
    '                    NewSpeed = (Parents(0).line(x, y).Speed + Parents(1).line(x, y).Speed) / 2
    '                    NewAngle = (Parents(0).line(x, y).Angle + Parents(1).line(x, y).Angle) / 2
    '                    NewClock = (Parents(0).line(x, y).Clock + Parents(1).line(x, y).Clock) / 2
    '                    NewGTClock = (Parents(0).line(x, y).GoThroughClock + Parents(1).line(x, y).GoThroughClock) / 2
    '                ElseIf Parents(0).NumOfLegs - 1 < x And Parents(1).NumOfLegs - 1 >= x Then
    '                    NewSpeed = Parents(1).line(x, y).Speed
    '                    NewAngle = Parents(1).line(x, y).Angle
    '                    NewClock = Parents(1).line(x, y).Clock
    '                    NewGTClock = Parents(1).line(x, y).GoThroughClock
    '                ElseIf Parents(1).NumOfLegs - 1 < x And Parents(0).NumOfLegs - 1 >= x Then
    '                    NewSpeed = Parents(0).line(x, y).Speed
    '                    NewAngle = Parents(0).line(x, y).Angle
    '                    NewClock = Parents(0).line(x, y).Clock
    '                    NewGTClock = Parents(0).line(x, y).GoThroughClock
    '                Else
    '                    NewSpeed = Rnd.Next(1, 20)
    '                    NewAngle = Rnd.Next(50, 180)
    '                    NewClock = Rnd.Next(5, 50)
    '                    NewGTClock = Rnd.Next(5, 50)
    '                End If
    '                If Rnd.Next(101) < MutationRate Then
    '                    NewSpeed = Rnd.Next(1, 20)
    '                End If
    '                If Rnd.Next(101) < MutationRate Then
    '                    NewAngle = Rnd.Next(50, 180)
    '                End If
    '                If Rnd.Next(101) < MutationRate Then
    '                    NewClock = Rnd.Next(5, 50)
    '                End If
    '                If Rnd.Next(101) < MutationRate Then
    '                    NewGTClock = Rnd.Next(5, 50)
    '                End If
    '                'Line(x, y) = New CLeg(100 + 100 * x, 50 + 100 * y, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Speed, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Clock, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Angle, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).GoThroughClock)

    '                If y > 0 Then
    '                    If x = 0 Then
    '                        PrevLegY = y - 1
    '                        PrevLegX = NewNumOfLegs - 1
    '                    Else
    '                        PrevLegY = y
    '                        PrevLegX = x - 1
    '                    End If

    '                    Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, NewSpeed, NewClock, NewAngle, NewGTClock, Rnd)
    '                End If

    '                Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + 100 * y, NewSpeed, NewClock, NewAngle, NewGTClock)
    '                Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
    '            Next
    '        Next
    '        NumOfLegs = NewNumOfLegs
    '        Animal.Add(New CCreature(NewBody, Line.Clone(), Joint.Clone(), Floor))



    '    Next

    'End Sub

    Sub CrossOver()
        Dim MatingPool As New List(Of CCreature)

        For X = 0 To Animal.Count - 1
            For Y = 0 To Animal(X).Fitness
                MatingPool.Add(Animal(X))
            Next
        Next

        For I = 0 To NumOfAnimals / 2 - 1
            Dim MotherIndex As Integer = 0
            Dim FatherIndex As Integer = 0
            Dim Parents As New List(Of CCreature)
            While MotherIndex = FatherIndex
                MotherIndex = Math.Floor(Rnd.Next(MatingPool.Count))
                FatherIndex = Math.Floor(Rnd.Next(MatingPool.Count))
            End While
            Parents.Add(MatingPool(MotherIndex))
            Parents.Add(MatingPool(FatherIndex))




            Dim NewNumOfLegs As Integer = Parents(Rnd.Next(Parents.Count)).NumOfLegs
            If Rnd.Next(101) < MutationRate Then
                NewNumOfLegs = Rnd.Next(2, 10)
            End If

            Dim NewBodyLength As Integer = Parents(Rnd.Next(Parents.Count)).Body.Diameter
            If Rnd.Next(101) < MutationRate Then
                NewBodyLength = Rnd.Next(120, 500)
            End If

            NewBody = New CBody(100, 50, 100 + NewBodyLength, 50, NewNumOfLegs)

            Dim Line(NewNumOfLegs - 1, NumOfLayers - 1) As CLeg
            Dim Joint(NewNumOfLegs - 1, NumOfLayers - 1) As CJoint
            For y = 0 To NumOfLayers - 1
                For x = 0 To NewNumOfLegs - 1
                    Dim NewSpeed As Integer
                    Dim NewAngle As Integer
                    Dim NewClock As Integer
                    Dim NewGTClock As Integer
                    If Parents(0).NumOfLegs - 1 >= x And Parents(1).NumOfLegs - 1 >= x Then
                        NewSpeed = Parents(Rnd.Next(Parents.Count)).line(x, y).Speed
                        NewAngle = Parents(Rnd.Next(Parents.Count)).line(x, y).Angle
                        NewClock = Parents(Rnd.Next(Parents.Count)).line(x, y).Clock
                        NewGTClock = Parents(Rnd.Next(Parents.Count)).line(x, y).GoThroughClock
                    ElseIf Parents(0).NumOfLegs - 1 < x And Parents(1).NumOfLegs - 1 >= x Then
                        NewSpeed = Parents(1).line(x, y).Speed
                        NewAngle = Parents(1).line(x, y).Angle
                        NewClock = Parents(1).line(x, y).Clock
                        NewGTClock = Parents(1).line(x, y).GoThroughClock
                    ElseIf Parents(1).NumOfLegs - 1 < x And Parents(0).NumOfLegs - 1 >= x Then
                        NewSpeed = Parents(0).line(x, y).Speed
                        NewAngle = Parents(0).line(x, y).Angle
                        NewClock = Parents(0).line(x, y).Clock
                        NewGTClock = Parents(0).line(x, y).GoThroughClock
                    Else
                        NewSpeed = Rnd.Next(0, 10)
                        NewAngle = Rnd.Next(40, 200)
                        NewClock = Rnd.Next(0, 100)
                        NewGTClock = Rnd.Next(0, 100)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewSpeed = Rnd.Next(0, 10)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewAngle = Rnd.Next(40, 200)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewClock = Rnd.Next(0, 100)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewGTClock = Rnd.Next(0, 100)
                    End If
                    'Line(x, y) = New CLeg(100 + 100 * x, 50 + 100 * y, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Speed, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Clock, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Angle, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).GoThroughClock)

                    If y > 0 Then
                        If x = 0 Then
                            PrevLegY = y - 1
                            PrevLegX = NewNumOfLegs - 1
                        Else
                            PrevLegY = y
                            PrevLegX = x - 1
                        End If

                        Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, NewSpeed, NewClock, NewAngle, NewGTClock, Rnd)
                    End If

                    Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + 100 * y, NewSpeed, NewClock, NewAngle, NewGTClock)
                    Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
                Next
            Next

            Dim TempLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg
            TempLine = Line.Clone()
            NumOfLegs = NewNumOfLegs

            Animal.Add(New CCreature(NewBody, Line.Clone(), Joint.Clone(), Floor))
            Animal(Animal.Count - 1).OrigLine = TempLine


        Next

    End Sub




    Public Sub KeyPressed(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.B Then
            Drawing = Not Drawing
        End If

    End Sub

End Class