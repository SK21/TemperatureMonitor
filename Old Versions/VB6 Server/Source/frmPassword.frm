VERSION 5.00
Begin VB.Form frmPassword 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Password"
   ClientHeight    =   1320
   ClientLeft      =   2835
   ClientTop       =   3480
   ClientWidth     =   3750
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   779.9
   ScaleMode       =   0  'User
   ScaleWidth      =   3521.047
   ShowInTaskbar   =   0   'False
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Height          =   390
      Left            =   2475
      TabIndex        =   3
      Top             =   720
      Width           =   1140
   End
   Begin VB.CommandButton cmdCancel 
      Caption         =   "Cancel"
      Height          =   390
      Left            =   1200
      TabIndex        =   2
      Top             =   720
      Width           =   1140
   End
   Begin VB.TextBox txtPassword 
      Height          =   345
      IMEMode         =   3  'DISABLE
      Left            =   1290
      MaxLength       =   20
      TabIndex        =   1
      Top             =   165
      Width           =   2325
   End
   Begin VB.Label lblLabels 
      Caption         =   "&Password:"
      Height          =   270
      Index           =   1
      Left            =   105
      TabIndex        =   0
      Top             =   180
      Width           =   1080
   End
End
Attribute VB_Name = "frmPassword"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public LoadOk As Boolean
Public Canceled As Boolean
Private Function ApplyEdit() As Boolean
    On Error GoTo ErrHandler
    If Len(txtPassword) > 20 Then
        MsgBox "Password too long.", , "TemperatureMonitor"
        txtPassword.SetFocus
        SendKeys "{Home}+{End}"
    Else
        ApplyEdit = True
        Canceled = False
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmPassword", "ApplyEdit", Err.Description
     Resume ErrExit
End Function
Private Sub cmdCancel_Click()
    Canceled = True
    Me.Hide
End Sub
Private Sub cmdOK_Click()
    If ApplyEdit Then
        Canceled = False
        Me.Hide
    End If
End Sub
Private Sub Form_Activate()
    txtPassword.SetFocus
    txtPassword = ""
End Sub
Private Sub Form_Load()
    AD.LoadFormData Me
    LoadOk = True
End Sub
Private Sub Form_Unload(Cancel As Integer)
    AD.SaveFormData Me
End Sub
