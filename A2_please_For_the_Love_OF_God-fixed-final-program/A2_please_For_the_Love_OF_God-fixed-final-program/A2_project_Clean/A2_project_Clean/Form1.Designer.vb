﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.components = New System.ComponentModel.Container()
        Me.Display = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Menu_Box = New System.Windows.Forms.Button()
        CType(Me.Display, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Display
        '
        Me.Display.BackColor = System.Drawing.Color.White
        Me.Display.Location = New System.Drawing.Point(12, 12)
        Me.Display.Name = "Display"
        Me.Display.Size = New System.Drawing.Size(1500, 550)
        Me.Display.TabIndex = 0
        Me.Display.TabStop = False
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 40
        '
        'Menu_Box
        '
        Me.Menu_Box.Location = New System.Drawing.Point(29, 515)
        Me.Menu_Box.Name = "Menu_Box"
        Me.Menu_Box.Size = New System.Drawing.Size(75, 23)
        Me.Menu_Box.TabIndex = 1
        Me.Menu_Box.Text = "Menu"
        Me.Menu_Box.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gray
        Me.ClientSize = New System.Drawing.Size(1534, 611)
        Me.Controls.Add(Me.Menu_Box)
        Me.Controls.Add(Me.Display)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.Display, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Display As PictureBox
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Menu_Box As Button
End Class
