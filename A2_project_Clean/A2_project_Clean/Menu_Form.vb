Public Class Menu_Form

    Public Sub KeyPressed(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.B Then
            Form1.Drawing = Not Form1.Drawing
        End If


    End Sub

    Private Sub Save_Button_Click(sender As Object, e As EventArgs) Handles Save_Button.Click
        Form1.Connection.Close()
        Form1.Connection.Open()
    End Sub

    Private Sub Menu_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MyBase.Location = New Point(Form1.Location.X + Form1.Width / 2 - MyBase.Width / 2, Form1.Location.Y + Form1.Height / 2 - MyBase.Height / 2)
        Continue_Button.Location = New Point(MyBase.ClientSize.Width / 2 - Continue_Button.Width / 2, 20)
        Save_Button.Location = New Point(MyBase.ClientSize.Width / 2 - Save_Button.Width / 2, 60)
        Show_Creature.Location = New Point(MyBase.ClientSize.Width / 2 - Show_Creature.Width / 2, 100)
    End Sub

    Private Sub Show_Creature_Click(sender As Object, e As EventArgs) Handles Show_Creature.Click
        Dim Creature As List(Of String)
        Dim Body As List(Of String)
        Dim Legs As List(Of String)

        Dim RowCount As Integer = 0
        Dim NumOfColumns As Integer = 12

        Dim Base As CBody



        Creature = Form1.QueryDatabase("SELECT * FROM Creatures WHERE Distance = (SELECT MAX(Distance) FROM Creatures)", False, 12)

        Legs = Form1.QueryDatabase("SELECT * FROM Legs WHERE Creature_ID = '" & Creature(0) & "'", False, 12)

        Body = Form1.QueryDatabase("SELECT * FROM Body WHERE Creature_ID = " & Creature(0), False, 6)

        Dim Line(Body(5) - 1, 1) As CLeg
        Dim Joint(Body(5) - 1, 1) As CJoint
        For y = 0 To 1
            For x = 0 To Body(5) - 1

                Line(x, y) = New CLeg(Legs(RowCount * NumOfColumns + 10), Legs(RowCount * NumOfColumns + 8), Legs(RowCount * NumOfColumns + 7), Legs(RowCount * NumOfColumns + 2), Legs(RowCount * NumOfColumns + 4), Legs(RowCount * NumOfColumns + 3), Legs(RowCount * NumOfColumns + 5))

                Joint(x, y) = New CJoint(Line(x, y).LP2.X, Line(x, y).LP2.Y, 10, 10)
                RowCount += 1
            Next
        Next
        Base = New CBody(Body(3), Body(1), Body(4), Body(2), Body(5))


        Form1.NumOfLegs = Body(5)

        Form1.BestAnimal = New CCreature(Base, Line, Joint, Form1.Floor, Creature(0), Creature(1))
        Form1.FindBestAnimal = True
        Me.Hide()

    End Sub
End Class