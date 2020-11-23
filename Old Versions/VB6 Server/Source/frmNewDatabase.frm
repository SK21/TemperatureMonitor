VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Begin VB.Form frmNewDatabase 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "New File"
   ClientHeight    =   3450
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   4770
   Icon            =   "frmNewDatabase.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3450
   ScaleWidth      =   4770
   ShowInTaskbar   =   0   'False
   Begin VB.CommandButton cmdOK 
      Caption         =   "Close"
      Height          =   390
      Left            =   3480
      TabIndex        =   11
      Top             =   2520
      Width           =   1140
   End
   Begin VB.CommandButton cmdCancel 
      Caption         =   "Cancel"
      Height          =   390
      Left            =   2160
      TabIndex        =   10
      Top             =   2520
      Width           =   1140
   End
   Begin VB.CommandButton butSelect 
      Caption         =   "Select All"
      Height          =   390
      Left            =   840
      TabIndex        =   9
      Top             =   2520
      Width           =   1140
   End
   Begin VB.TextBox TextBox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   1
      Left            =   1800
      MaxLength       =   20
      TabIndex        =   3
      Top             =   600
      Width           =   2775
   End
   Begin VB.CheckBox ckBox 
      Caption         =   "Historical Records"
      Height          =   375
      Index           =   2
      Left            =   120
      TabIndex        =   6
      Top             =   2040
      Value           =   1  'Checked
      Width           =   2415
   End
   Begin VB.CheckBox ckBox 
      Caption         =   "Sensors"
      Height          =   375
      Index           =   0
      Left            =   120
      TabIndex        =   4
      Top             =   1560
      Value           =   1  'Checked
      Width           =   1815
   End
   Begin VB.CheckBox ckBox 
      Caption         =   "Bins"
      Height          =   375
      Index           =   1
      Left            =   2640
      TabIndex        =   5
      Top             =   1560
      Value           =   1  'Checked
      Width           =   1575
   End
   Begin VB.TextBox TextBox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   0
      Left            =   1800
      MaxLength       =   20
      TabIndex        =   1
      Top             =   120
      Width           =   2775
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   8
      Top             =   3075
      Width           =   4770
      _ExtentX        =   8414
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
   Begin VB.Label Label3 
      Caption         =   "Password"
      Height          =   255
      Left            =   240
      TabIndex        =   2
      Top             =   600
      Width           =   1215
   End
   Begin VB.Label Label2 
      Alignment       =   2  'Center
      Caption         =   "Copy from Current File :"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   -1  'True
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   120
      TabIndex        =   7
      Top             =   1080
      Width           =   4455
   End
   Begin VB.Label Label1 
      Caption         =   "New File Name"
      Height          =   255
      Left            =   240
      TabIndex        =   0
      Top             =   120
      Width           =   1215
   End
End
Attribute VB_Name = "frmNewDatabase"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public LoadOK As Boolean
Dim AllSelected As Boolean
Dim DataChanged As Boolean
Const Boxes As Long = 3

Private Function ApplyEdit() As Boolean
    Dim CK As Long
    Dim TBL(Boxes - 1) As Boolean
    Dim Done As Boolean
    Dim ER As Long
    On Error GoTo ErrHandler
    If LegalFileName(textbox(0)) Then
        For CK = 0 To Boxes - 1
            If ckBox(CK) = 1 And ckBox(CK).Enabled Then
                TBL(CK) = True
            Else
                TBL(CK) = False
            End If
        Next CK
        Done = Prog.CreateDatabase(textbox(0), TBL, AD.Element("DataLocation"), textbox(1))
        If Done Then
            StatusBar1.SimpleText = "File created."
            Beep
            AD.Element("LastDB") = Prog.DatabaseFullName
            ResetStatus
        Else
            StatusBar1.SimpleText = "Could not create file."
            Beep
            'reload database
            DBconnected = False
            ConnectDatabase
        End If
        Else
            StatusBar1.SimpleText = "Not a legal file name."
            Beep
        End If
        ApplyEdit = Done
ErrExit:
    Exit Function
ErrHandler:
    'convert err number to GrainManager object error
    ER = (Err.Number And &HFFFF&)
    Select Case ER
        Case 1001
            'GrainManager object input error
            Beep
            StatusBar1.SimpleText = Err.Description
        Case Else
            AD.DisplayError Err.Number, "frmNewDatabase", "ApplyEdit", Err.Description
    End Select
    Resume ErrExit
End Function

Private Sub butSelect_Click()
    Dim V As Long
    Dim CK As Long
    On Error GoTo ErrHandler
    If AllSelected Then
        AllSelected = False
        V = 0
        butSelect.Caption = "Select All"
    Else
        AllSelected = True
        V = 1
        butSelect.Caption = "Unselect All"
    End If
    For CK = 0 To Boxes - 1
        ckBox(CK).Value = V
    Next CK
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmNewDatabase", "butSelect_Click", Err.Description
    Resume ErrExit
End Sub

Private Function CancelExit() As Boolean
'---------------------------------------------------------------------------------------
' Procedure : CancelExit
' Author    : XPMUser
' Date      : 24/Jan/2015
' Purpose   : checks if users wants to save changes before exiting the form
'---------------------------------------------------------------------------------------
'
    On Error GoTo ErrHandler
    Select Case MsgBox("Do you want to save the changes?", vbYesNoCancel Or _
        vbExclamation Or vbDefaultButton1, App.Title)
        Case vbYes
            'attempt to save before exit
            CancelExit = Not ApplyEdit
        Case vbNo
            'exit without saving
            CancelExit = False
        Case vbCancel
            'cancel exit
            CancelExit = True
    End Select
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmDocs", "CancelExit", Err.Description
     Resume ErrExit
End Function

Private Sub cmdCancel_Click()
    ResetStatus
End Sub

Private Sub cmdOK_Click()
    On Error GoTo ErrHandler
    If cmdOK.Caption = "Save" Then
        ApplyEdit
    Else
        Unload Me
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmNewDatabase", "cmdOK_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub cmdOK_LostFocus()
    StatusBar1.SimpleText = ""
End Sub

Private Sub EnableChecks()
    Dim HasCurrent As Boolean
    Dim X As Long
    'enables copy from tables only
    'if there is a current database
    HasCurrent = Prog.IsValid
    For X = 0 To Boxes - 1
        ckBox(X).Enabled = HasCurrent
    Next X
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
    On Error GoTo ErrExit
    AD.LoadFormData Me
    LoadOK = False
    EnableChecks
    ResetStatus
    LoadOK = True
ErrExit:
End Sub

Private Sub Form_Unload(Cancel As Integer)
    If DataChanged Then Cancel = CancelExit
    If Not Cancel Then AD.SaveFormData Me
End Sub

Private Sub ResetStatus()
    Dim CK As Long
    On Error GoTo ErrHandler
    For CK = 0 To 1
        textbox(CK) = ""
    Next CK
    For CK = 0 To Boxes - 1
        ckBox(CK).Value = 0
    Next CK
    cmdCancel.Enabled = False
    cmdOK.Caption = "Close"
    DataChanged = False
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmNewDatabase", "ResetStatus", Err.Description
    Resume ErrExit
End Sub

Private Sub textBox_Change(Index As Integer)
    On Error GoTo ErrHandler
    DataChanged = (textbox(0) <> "" Or textbox(1) <> "")
    If DataChanged Then
        cmdCancel.Enabled = True
        cmdOK.Caption = "Save"
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmNewDatabase", "textBox_Change", Err.Description
     Resume ErrExit
End Sub

Private Sub textbox_GotFocus(Index As Integer)
    textbox(Index).SelStart = 0
    textbox(Index).SelLength = Len(textbox(Index).Text)
End Sub

