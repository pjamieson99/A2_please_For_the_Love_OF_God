Imports System.Data.OleDb

Public Class Database
    Public CreatureID As Integer = 0 'used to give each creature an ID

    Dim DatabaseDirectory As String = Environment.CurrentDirectory() & "\Database1.accdb" 'the file path of the database
    Public Connection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & DatabaseDirectory) 'create the connection to the database
    Dim Query As New OleDbCommand("", Connection) 'this is the query command that will be used to retrieve and put data in the database

    Sub New()
        Connection.Open()
        FindPrevIDs()
    End Sub
    Sub FindPrevIDs() 'finds the previous last IDs from the database and sets the new ID for the next creature
        Dim CreatureIds As List(Of String) = QueryDatabase("SELECT Creature_ID FROM Creatures ORDER BY Creature_ID DESC", False, 1) 'finds last creature ID

        If CreatureIds.Count <> 0 Then 'finds next creature id on the database
            CreatureID = CreatureIds(0) + 1
        End If

        If CreatureID = 0 Then 'if database is empty manually set the next creature ID
            CreatureID += 1
        End If


    End Sub

    Public Function QueryDatabase(QueryText As String, Write As Boolean, Optional ColumnCount As Integer = 1) As List(Of String) 'provides access and interaciton with database
        Dim Result As New List(Of String)

        If Not Write Then 'checks whether it is reading or writing to database
            Query.CommandText = QueryText

            Using DataReader = Query.ExecuteReader 'executes command given and stores result - for read

                While DataReader.Read() 'adds to the list of results the all the values that are read
                    For X = 0 To ColumnCount - 1
                        Result.Add(DataReader(X).ToString)
                    Next
                End While

            End Using

        Else ' executes command to database for writing
            Query.CommandText = QueryText
            Query.ExecuteNonQuery()
        End If

        Return Result
    End Function

    Public Sub WriteCreature(Animal As CCreature, GenNum As Integer, NumberOfLegs As Integer, NumberOfLayers As Integer, Creature_ID As Integer) ' writes a newly created creature to a database

        QueryDatabase("INSERT INTO Body([BPY1], [BPY2], [BPX1], [BPX2], [NumOfLegs]) VALUES (" & Animal.Body.BPy1 & "," & Animal.Body.BPy2 & "," & Animal.Body.BPx1 & "," & Animal.Body.BPx2 & "," & Form1.NumOfLegs & ")", True) 'writes the body properties into database
        QueryDatabase("INSERT INTO Creatures(Generation) VALUES (" & GenNum & ")", True) ' inserts the generation number

        For y = 0 To Form1.NumOfLayers - 1 'inputs all properties of legs of creature
            For x = 0 To Form1.NumOfLegs - 1
                Dim LegClock As Integer = Animal.line(x, y).Clock
                Dim LegSpeed As Integer = Animal.line(x, y).Speed
                Dim LegAngle As Integer = Animal.line(x, y).Angle
                Dim LegGoThroughClock As Integer = Animal.line(x, y).GoThroughClock
                Dim LegDiameter As Integer = Animal.line(x, y).Diameter
                QueryDatabase("INSERT INTO Legs([Speed], [Clock], [GoThroughClock], [Angle], [Diameter], [Creature_ID], [LPY1], [LPY2], [LPX1], [LPX2]) VALUES (" & LegSpeed & "," & LegClock & "," & LegGoThroughClock & "," & LegAngle & "," & LegDiameter & "," & Animal.ID & "," & Animal.line(x, y).LPy1 & "," & Animal.line(x, y).LPy2 & "," & Animal.line(x, y).LPx1 & "," & Animal.line(x, y).LPx2 & ")", True)
            Next
        Next

    End Sub
End Class
