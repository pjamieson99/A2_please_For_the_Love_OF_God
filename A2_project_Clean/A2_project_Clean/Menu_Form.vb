Public Class Menu_Form 'this form is the menu option that allows you to pause and configure the program

    Private Sub Save_Button_Click(sender As Object, e As EventArgs) Handles Save_Button.Click 'this saves the current data about the creatures to the database by closing then opening the databse again
        Form1.CreatureDatabase.Connection.Close()
        Form1.CreatureDatabase.Connection.Open()
    End Sub

    Private Sub Menu_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'loads the menu when clicked
        If Form1.RealSpeedTime = False Then 'updates buttom display of the changing speed based on what the speed is going at
            Speed_Up.Text = "Slow Down"
        Else
            Speed_Up.Text = "Speed Up"
        End If

        MyBase.Location = New Point(Form1.Location.X + Form1.Width / 2 - MyBase.Width / 2, Form1.Location.Y + Form1.Height / 2 - MyBase.Height / 2) 'sets location of buttons
        Continue_Button.Location = New Point(MyBase.ClientSize.Width / 2 - Continue_Button.Width / 2, 20)
        Save_Button.Location = New Point(MyBase.ClientSize.Width / 2 - Save_Button.Width / 2, 60)
        Show_Creature.Location = New Point(MyBase.ClientSize.Width / 2 - Show_Creature.Width / 2, 100)
        Speed_Up.Location = New Point(MyBase.ClientSize.Width / 2 - Speed_Up.Width / 2, 140)

    End Sub

    Private Sub Show_Creature_Click(sender As Object, e As EventArgs) Handles Show_Creature.Click 'when the show best creature button is clicked it activates this function
        Dim Creature As List(Of String) 'these are the body parts of the creature retrieving variables that recieve the info in a string
        Dim Body As List(Of String)
        Dim Legs As List(Of String)

        Dim RowCount As Integer = 0 'these set the number of column data needs to be selected from out of the 12,  and the rowcount can specify from which row
        Dim NumOfColumns As Integer = 11

        Creature = Form1.CreatureDatabase.QueryDatabase("SELECT * FROM Creatures WHERE Distance = (SELECT MAX(Distance) FROM Creatures)", False, 3) 'retrieves the info from the database to be used
        Legs = Form1.CreatureDatabase.QueryDatabase("SELECT * FROM Legs WHERE Creature_ID = '" & Creature(0) & "'", False, 11)
        Body = Form1.CreatureDatabase.QueryDatabase("SELECT * FROM Body WHERE Creature_ID = " & Creature(0), False, 6)

        Dim Base As CBody 'this is the creature's body parts
        Dim Line(Body(5) - 1, 1) As CLeg
        Dim Joint(Body(5) - 1, 1) As CJoint

        For y = 0 To 1
            For x = 0 To Body(5) - 1 'sets the legs and joints from the data retrieved
                Line(x, y) = New CLeg(Legs(RowCount * NumOfColumns + 9), Legs(RowCount * NumOfColumns + 7), Legs(RowCount * NumOfColumns + 6), Legs(RowCount * NumOfColumns + 2), Legs(RowCount * NumOfColumns + 4), Legs(RowCount * NumOfColumns + 3), Legs(RowCount * NumOfColumns + 5))
                Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
                RowCount += 1
            Next
        Next

        Form1.NumOfLegs = Body(5) 'sets the number of legs and the generation of the creature while enabling the show best creature mode
        Form1.BestAnimalGeneration = Convert.ToInt32(Creature(1))
        Form1.FindBestAnimal = True

        Base = New CBody(Body(3), Body(1), Body(4), Body(2), Body(5)) 'sets the body and the creates the animal
        Form1.BestAnimal = New CCreature(Base, Line, Joint, Form1.Floor, Creature(0), Creature(1))


        Me.Hide()

    End Sub

    Private Sub Speed_Up_Click(sender As Object, e As EventArgs) Handles Speed_Up.Click 'this speeds up or slows down the program
        Form1.RealSpeedTime = Not Form1.RealSpeedTime 'changes the mode form either speed up to speed down or speed down to speed up

        If Form1.RealSpeedTime = False Then 'changes the text based on whether it is being sped up
            Speed_Up.Text = "Slow Down"
        Else
            Speed_Up.Text = "Speed Up"
        End If

    End Sub
End Class