VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Begin VB.Form frmChangePassword 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Change Password"
   ClientHeight    =   2130
   ClientLeft      =   2835
   ClientTop       =   3480
   ClientWidth     =   4050
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   2130
   ScaleWidth      =   4050
   ShowInTaskbar   =   0   'False
   Begin VB.CommandButton cmdOK 
      Caption         =   "Close"
      Height          =   390
      Left            =   2760
      TabIndex        =   6
      Top             =   1200
      Width           =   1140
   End
   Begin VB.CommandButton cmdCancel 
      Caption         =   "Cancel"
      Height          =   390
      Left            =   1440
      TabIndex        =   5
      Top             =   1200
      Width           =   1140
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   4
      Top             =   1755
      Width           =   4050
      _ExtentX        =   7144
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
   Begin VB.TextBox textbox 
      Height          =   345
      IMEMode         =   3  'DISABLE
      Index           =   1
      Left            =   1530
      MaxLength       =   20
      TabIndex        =   2
      Top             =   735
      Width           =   2325
   End
   Begin VB.TextBox textbox 
      Height          =   345
      IMEMode         =   3  'DISABLE
      Index           =   0
      Left            =   1530
      MaxLength       =   20
      TabIndex        =   1
      Top             =   180
      Width           =   2325
   End
   Begin VB.Label lblLabels 
      Caption         =   "New Password"
      Height          =   270
      Index           =   0
      Left            =   120
      TabIndex        =   3
      Top             =   735
      Width           =   1080
   End
   Begin VB.Label lblLabels 
      Caption         =   "Old Password"
      Height          =   270
      Index           =   1
      Left            =   105
      TabIndex        =   0
      Top             =   180
      Width           =   1080
   End
End
Attribute VB_Name = "frmChangePassword"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Sub cmdCancel_Click()
    textbox(0) = ""
    textbox(1) = ""
    textbox(0).SetFocus
End Sub
Private Sub cmdOK_Click()
    Dim Done As Boolean
    Dim ER As Long
    On Error GoTo ErrHandler
    If cmdOK.Caption = "Save" Then
        Done = Prog.ChangePassword(textbox(0), textbox(1))
        If Done Then
            StatusBar1.SimpleText = "Password changed."
            textbox(0) = ""
            textbox(1) = ""
        Else
            StatusBar1.SimpleText = "Password not changed."
            'reload database
            DBconnected = False
            ConnectDatabase
        End If
    Else
        Unload Me
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    'convert err number to GrainManager object error
    ER = (Err.Number And &HFFFF&)
    Select Case ER
        Case 1001
            'GrainManager object input error
            Beep
            StatusBar1.SimpleText = Err.Description
        Case Else
            AD.DisplayError Err.Number, "frmChangePassword", "cmdOK_Click", Err.Description
    End Select
    Resume ErrExit
End Sub
Private Sub cmdOK_LostFocus()
        StatusBar1.SimpleText = ""
End Sub
Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    On Error GoTo ErrHandler
    Select Case KeyCode
        Case 38
            'up arrow
            SendKeys ("+{tab}")
            KeyCode = 0
        Case 40
            'down arrow
            SendKeys ("{tab}")
            KeyCode = 0
    End Select
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmChangePassword", "Form_KeyDown", Err.Description
    Resume ErrExit
End Sub
Private Sub Form_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        'enter
        SendKeys ("{tab}")
        KeyAscii = 0
    End If
End Sub
Private Sub Form_Load()
    AD.LoadFormData Me
    cmdCancel.Enabled = False
End Sub
Private Sub Form_Unload(Cancel As Integer)
    AD.SaveFormData Me
End Sub
Private Sub textBox_Change(Index As Integer)
    If textbox(0) = "" And textbox(1) = "" Then
        cmdOK.Caption = "Close"
        cmdCancel.Enabled = False
    Else
        cmdOK.Caption = "Save"
        cmdCancel.Enabled = True
    End If
End Sub
Private Sub textbox_GotFocus(Index As Integer)
    textbox(Index).SelStart = 0
    textbox(Index).SelLength = Len(textbox(Index).Text)
End Sub
