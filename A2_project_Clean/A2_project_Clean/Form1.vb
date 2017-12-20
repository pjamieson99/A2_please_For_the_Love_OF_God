Imports System.Data.OleDb

Public Class Form1
    Dim DatabaseDirectory As String = Environment.CurrentDirectory() & "\Database1.accdb"
    Public Connection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & DatabaseDirectory)
    Dim Query As New OleDbCommand("", Connection)

    Public Floor As CFloor
    Dim NumOfAnimals As Integer = 20
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
    Public Drawing As Boolean = True
    Dim MutationRate As Double = 15
    Dim PrevLegY As Integer
    Dim PrevLegX As Integer
    Public FurthestAnimalPos As Double = 0
    Dim NewBody As CBody
    Dim Distance(14) As Integer
    Dim Count As Integer = 1

    Dim LegCount As Integer = 1
    Dim CreatureID As Integer = 0

    Dim Pause As Boolean = False
    Public FindBestAnimal As Boolean = False
    Public BestAnimal As CCreature

    'Dim PrevAnimal As New List(Of CCreature)

    Public Function QueryDatabase(QueryText As String, Write As Boolean, Optional ColumnCount As Integer = 1) As List(Of String)

        Dim Result As New List(Of String)

        '  Connection.Open()
        If Not Write Then
            Query.CommandText = QueryText
            Using DataReader = Query.ExecuteReader
                While DataReader.Read()
                    '      Result.Add(DataReader(0).ToString)
                    For X = 0 To ColumnCount - 1
                        Result.Add(DataReader(X).ToString)
                    Next
                End While
            End Using
        Else
            Query.CommandText = QueryText
            Query.ExecuteNonQuery()

        End If
        '  Connection.Close()

        Return Result

    End Function


    Private Function ReadCreature(Creature_ID As Integer) As List(Of String)

        Dim Result As New List(Of String)
        'Connection.Open()
        Result = QueryDatabase("SELECT * FROM Creatures WHERE Creature_ID = " & Creature_ID, False, 12)
        'Connection.Close()

        For x = 2 To 11
            Dim CurrentLeg As List(Of String) = QueryDatabase("SELECT * FROM Legs WHERE Leg_ID = " & Result(x), False, 6)
            For Each P In CurrentLeg
                Result.Add(P)
            Next
        Next



        Return Result

    End Function

    Public Sub WriteCreature(Animal As CCreature, GenNum As Integer, NumberOfLegs As Integer, NumberOfLayers As Integer, Creature_ID As Integer)

        Dim Leg_ID(19) As Integer
        Dim NumOfLegIDs(19) As Integer


        For x = 1 To 20
            Leg_ID(x - 1) = LegCount + x

        Next
        For x = 1 To 20
            NumOfLegIDs(x - 1) = x
        Next


        'Connection.Open()
        QueryDatabase("INSERT INTO Body([BPY1], [BPY2], [BPX1], [BPX2], [NumOfLegs]) VALUES (" & Animal.Body.BPy1 & "," & Animal.Body.BPy2 & "," & Animal.Body.BPx1 & "," & Animal.Body.BPx2 & "," & NumOfLegs & ")", True)



        QueryDatabase("INSERT INTO Creatures(Generation) VALUES (" & GenNum & ")", True)

        For x = 0 To 19
            QueryDatabase("UPDATE Creatures SET Leg_" & NumOfLegIDs(x) & "_ID = " & Leg_ID(x) & " WHERE Creature_ID = " & Creature_ID, True)
            'QueryDatabase("INSERT INTO Creatures(Leg_" & NumOfLegIDs(x) & "_ID) VALUES (" & Leg_ID(x) & ")", True)
        Next

        LegCount += 20


        For y = 0 To NumOfLayers - 1
            For x = 0 To NumOfLegs - 1
                Dim LegClock As Integer = Animal.line(x, y).Clock
                Dim LegSpeed As Integer = Animal.line(x, y).Speed
                Dim LegAngle As Integer = Animal.line(x, y).Angle
                Dim LegGoThroughClock As Integer = Animal.line(x, y).GoThroughClock
                Dim LegDiameter As Integer = Animal.line(x, y).Diameter


                QueryDatabase("INSERT INTO Legs([Speed], [Clock], [GoThroughClock], [Angle], [Diameter], [True], [Creature_ID], [LPY1], [LPY2], [LPX1], [LPX2]) VALUES (" & LegSpeed & "," & LegClock & "," & LegGoThroughClock & "," & LegAngle & "," & LegDiameter & "," & 1 & "," & Animal.ID & "," & Animal.line(x, y).LPy1 & "," & Animal.line(x, y).LPy2 & "," & Animal.line(x, y).LPx1 & "," & Animal.line(x, y).LPx2 & ")", True)
            Next
        Next
        For x = 19 To NumberOfLegs * NumberOfLayers Step -1
            QueryDatabase("INSERT INTO Legs([Speed], [Clock], [GoThroughClock], [Angle], [Diameter], [True], [Creature_ID], [LPY1], [LPY2], [LPX1], [LPX2]) VALUES (" & 0 & "," & 0 & "," & 0 & "," & 0 & "," & 0 & "," & 0 & "," & Animal.ID & "," & 0 & "," & 0 & "," & 0 & "," & 0 & ")", True)

        Next
        'Connection.Close()
    End Sub


    Sub FindPrevIDs()
        ' Connection.Open()

        Dim Ids As List(Of String) = QueryDatabase("SELECT Creature_ID FROM Creatures ORDER BY Creature_ID DESC", False, 1)
        If Ids.Count <> 0 Then
            CreatureID = Ids(0) + 1
        End If
        'Connection.Close()

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Connection.Open()

        'ReadCreature(1)

        FindPrevIDs()

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

                        Line(x, y) = New CLeg(100 + Base(n).Diameter / (NumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, 1, 1, 1, 1, 1, Rnd)

                    Else
                        Line(x, y) = New CLeg(100 + Base(n).Diameter / (NumOfLegs - 1) * x, 50 + 100 * y, 1, 1, 1, 1, 1, Rnd)

                    End If


                    Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)

                Next
            Next
            Dim TempLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg
            TempLine = Line.Clone()
            BodyLines(n) = TempLine
            BodyJoints(n) = Joint

            Animal.Add(New CCreature(Base(n), BodyLines(n), BodyJoints(n), Floor, CreatureID, Generation))

            WriteCreature(Animal(n), Animal(n).Generation, Animal(n).NumOfLegs, Animal(n).NumOfLayers, CreatureID)

            CreatureID += 1
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

        If FindBestAnimal = True Then

            If BestAnimal.TimeCounter < 500 And BestAnimal.DeadCounter < 125 Then


                BestAnimal.NextMove(g)
                AnimalAlive = True

            End If

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


            If Not Drawing Then
                Display.Refresh()
            End If

        Else


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
                Generation += 1
                GeneticAlgorithm()
                'genetic algorithm

                Count = 1
                For x = 0 To Display.Width / 100 - 1
                    Distance(x) = Count
                    Count += 1
                    CurrentStartPos(x) = StartPos(x)
                Next
            End If






        End If
        If Not Drawing Then
            Display.Refresh()
        End If
    End Sub




    Function CheckFloor(floor As CFloor, point As Double)

        If floor.ypos - 4 <= point Then
            Return True
        Else
            Return False
        End If
    End Function



    Sub GeneticAlgorithm()
        AddDistance()
        FindFitness()
        KillWeakest()
        CrossOver()
        For Each Creature In Animal
            Creature.Reset()
        Next
    End Sub

    Sub AddDistance()
        'Connection.Open()
        For x = 0 To NumOfAnimals - 1
            QueryDatabase("UPDATE Creatures SET Distance = " & Animal(x).Body.CoM.X & " WHERE Creature_ID = " & Animal(x).ID, True)
        Next
        'Connection.Close()
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

    Sub FindFurthestAnimal()
        For x = 0 To NumOfAnimals - 1
            If FurthestAnimalPos < Animal(x).Body.CoM.X And Animal(x).Body.CoM.X > 700 Then
                FurthestAnimalPos = Animal(x).Body.CoM.X
            End If
        Next
        If FurthestAnimalPos <> 0 Then
            FurthestAnimalPos -= 700
        End If
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
            While MatingPool(MotherIndex) Is MatingPool(FatherIndex)
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
                    Dim NewDiameter As Integer
                    If Parents(0).NumOfLegs - 1 >= x And Parents(1).NumOfLegs - 1 >= x Then
                        NewSpeed = Parents(Rnd.Next(Parents.Count)).line(x, y).Speed
                        NewAngle = Parents(Rnd.Next(Parents.Count)).line(x, y).Angle
                        NewClock = Parents(Rnd.Next(Parents.Count)).line(x, y).Clock
                        NewGTClock = Parents(Rnd.Next(Parents.Count)).line(x, y).GoThroughClock
                        NewGTClock = Parents(Rnd.Next(Parents.Count)).line(x, y).Diameter

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
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewSpeed = Rnd.Next(0, 10)
                    End If
                    If Rnd.Next(101) < MutationRate Then
                        NewAngle = Rnd.Next(40, 200)
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
                    'Line(x, y) = New CLeg(100 + 100 * x, 50 + 100 * y, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Speed, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Clock, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).Angle, Parents(Math.Floor(Rnd.Next(Parents.Count))).line(x, y).GoThroughClock)

                    If y > 0 Then
                        If x = 0 Then
                            PrevLegY = y - 1
                            PrevLegX = NewNumOfLegs - 1
                        Else
                            PrevLegY = y
                            PrevLegX = x - 1
                        End If

                        Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + Line(PrevLegX, PrevLegY).Diameter, NewSpeed, NewClock, NewAngle, NewGTClock, NewDiameter, Rnd)
                    End If

                    Line(x, y) = New CLeg(100 + NewBody.Diameter / (NewNumOfLegs - 1) * x, 50 + 100 * y, NewSpeed, NewClock, NewAngle, NewGTClock, NewDiameter)
                    Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
                Next
            Next

            Dim TempLine(NumOfLegs - 1, NumOfLayers - 1) As CLeg
            TempLine = Line.Clone()
            NumOfLegs = NewNumOfLegs


            Animal.Add(New CCreature(NewBody, Line.Clone(), Joint.Clone(), Floor, CreatureID, Generation))
            Animal(Animal.Count - 1).OrigLine = TempLine

            WriteCreature(Animal(Animal.Count - 1), Animal(Animal.Count - 1).Generation, Animal(Animal.Count - 1).NumOfLegs, Animal(Animal.Count - 1).NumOfLayers, CreatureID)
            CreatureID += 1
        Next

    End Sub


    Public Sub KeyPressed(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown, Menu_Box.KeyDown

        If e.KeyCode = Keys.B Then
            Drawing = Not Drawing
        End If


    End Sub

    Private Sub Menu_Box_Click(sender As Object, e As EventArgs) Handles Menu_Box.Click
        Dim DialogForm As Form = New Menu_Form
        Timer1.Stop()
        DialogForm.ShowDialog()
        Timer1.Start()
    End Sub

    'Private Sub Form1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress

    '    If e.KeyChar = Keys.B Then
    '        Drawing = Not Drawing
    '    End If



    'End Sub
End Class