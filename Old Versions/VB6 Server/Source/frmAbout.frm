VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Begin VB.Form frmAbout 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "About"
   ClientHeight    =   2730
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   6120
   ControlBox      =   0   'False
   Icon            =   "frmAbout.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   2730
   ScaleWidth      =   6120
   ShowInTaskbar   =   0   'False
   Begin VB.CommandButton butSave 
      Caption         =   "OK"
      Height          =   375
      Left            =   5040
      TabIndex        =   4
      Top             =   1800
      Width           =   855
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   3
      Top             =   2355
      Width           =   6120
      _ExtentX        =   10795
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
   Begin VB.PictureBox picIcon 
      AutoSize        =   -1  'True
      ClipControls    =   0   'False
      Height          =   540
      Left            =   240
      Picture         =   "frmAbout.frx":0442
      ScaleHeight     =   337.12
      ScaleMode       =   0  'User
      ScaleWidth      =   337.12
      TabIndex        =   0
      Top             =   240
      Width           =   540
   End
   Begin VB.Label LbIP 
      Alignment       =   2  'Center
      Caption         =   "IP Address"
      Height          =   255
      Left            =   240
      TabIndex        =   5
      Top             =   1200
      Width           =   5655
   End
   Begin VB.Line Line 
      X1              =   240
      X2              =   5880
      Y1              =   1680
      Y2              =   1680
   End
   Begin VB.Label LabelVersion 
      Alignment       =   2  'Center
      Caption         =   "Version"
      Height          =   255
      Left            =   240
      TabIndex        =   2
      Top             =   720
      Width           =   5655
   End
   Begin VB.Label Label1 
      Alignment       =   2  'Center
      AutoSize        =   -1  'True
      Caption         =   "TemperatureMonitor"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   12
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   300
      Left            =   1860
      TabIndex        =   1
      Top             =   240
      Width           =   2475
   End
End
Attribute VB_Name = "frmAbout"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public LoadOK As Boolean
Private Sub butSave_Click()
'---------------------------------------------------------------------------------------
' Procedure : butSave_Click
' Author    : David
' Date      : 12/6/2010
' Purpose   :
'---------------------------------------------------------------------------------------
    On Error GoTo ErrHandler
    Unload Me
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmAbout", "butSave_Click", Err.Description
    Resume ErrExit
End Sub
Private Sub Form_Load()
    AD.LoadFormData Me
    LabelVersion = "Version " & App.Major & "." & App.Minor & "." & App.Revision
    LbIP = "This computer's local network IP address is: " & IPaddress
    LoadOK = True
End Sub
Private Sub Form_Unload(Cancel As Integer)
    AD.SaveFormData Me
End Sub
Private Function IPaddress() As String
    On Error Resume Next
    IPaddress = frmStart.sckServer(0).LocalIP
End Function

