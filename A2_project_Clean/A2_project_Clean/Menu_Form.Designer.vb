<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Menu_Form
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Continue_Button = New System.Windows.Forms.Button()
        Me.Save_Button = New System.Windows.Forms.Button()
        Me.Show_Creature = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Continue_Button
        '
        Me.Continue_Button.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Continue_Button.Location = New System.Drawing.Point(113, 30)
        Me.Continue_Button.Name = "Continue_Button"
        Me.Continue_Button.Size = New System.Drawing.Size(75, 23)
        Me.Continue_Button.TabIndex = 0
        Me.Continue_Button.Text = "Continue"
        Me.Continue_Button.UseVisualStyleBackColor = True
        '
        'Save_Button
        '
        Me.Save_Button.Location = New System.Drawing.Point(113, 84)
        Me.Save_Button.Name = "Save_Button"
        Me.Save_Button.Size = New System.Drawing.Size(75, 23)
        Me.Save_Button.TabIndex = 1
        Me.Save_Button.Text = "Save"
        Me.Save_Button.UseVisualStyleBackColor = True
        '
        'Show_Creature
        '
        Me.Show_Creature.Location = New System.Drawing.Point(85, 137)
        Me.Show_Creature.Name = "Show_Creature"
        Me.Show_Creature.Size = New System.Drawing.Size(133, 23)
        Me.Show_Creature.TabIndex = 3
        Me.Show_Creature.Text = "Show The best creature"
        Me.Show_Creature.UseVisualStyleBackColor = True
        '
        'Menu_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 219)
        Me.Controls.Add(Me.Show_Creature)
        Me.Controls.Add(Me.Save_Button)
        Me.Controls.Add(Me.Continue_Button)
        Me.Name = "Menu_Form"
        Me.Text = "Menu_Form"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Continue_Button As Button
    Friend WithEvents Save_Button As Button
    Friend WithEvents Show_Creature As Button
End Class
