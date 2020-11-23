VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Begin VB.Form frmSaveAs 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Save As"
   ClientHeight    =   1560
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   4680
   Icon            =   "frmSaveAs.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1560
   ScaleWidth      =   4680
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdOK 
      Caption         =   "Close"
      Height          =   390
      Left            =   3360
      TabIndex        =   4
      Top             =   600
      Width           =   1140
   End
   Begin VB.CommandButton cmdCancel 
      Caption         =   "Cancel"
      Height          =   390
      Left            =   2040
      TabIndex        =   3
      Top             =   600
      Width           =   1140
   End
   Begin VB.TextBox TextBox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   0
      Left            =   1680
      MaxLength       =   20
      TabIndex        =   0
      Top             =   120
      Width           =   2775
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   2
      Top             =   1185
      Width           =   4680
      _ExtentX        =   8255
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
   Begin VB.Label Label1 
      Caption         =   "New name for file"
      Height          =   255
      Left            =   120
      TabIndex        =   1
      Top             =   120
      Width           =   1455
   End
End
Attribute VB_Name = "frmSaveAs"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public LoadOK As Boolean
Dim Edited As Boolean
Private Function ApplyEdit() As Boolean
    Dim OldName As String
    Dim DataLocation As String
    Dim FSO As FileSystemObject
    On Error GoTo ErrHandler
    Set FSO = New FileSystemObject
    ApplyEdit = False
    OldName = Prog.DatabaseName
    DataLocation = AD.Folders(AppDatabase)
    Set Prog = Nothing
    Set Prog = New clsMain
    'rename database
    Name DataLocation & "\" & OldName & ".mdb" As DataLocation & "\" & textbox(0) & ".mdb"
    'rename documents folder
    If FSO.FolderExists(DataLocation & "\" & OldName) Then
        Name DataLocation & "\" & OldName As DataLocation & "\" & textbox(0)
    End If
    ConnectDatabase DataLocation & "\" & textbox(0) & ".mdb"
    StatusBar1.SimpleText = "Database renamed."
    Beep
    ApplyEdit = True
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmSaveAs", "ApplyEdit", Err.Description
     Resume ErrExit
End Function
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
     AD.DisplayError Err.Number, "frmSaveAs", "CancelExit", Err.Description
     Resume ErrExit
End Function
Private Sub cmdCancel_Click()
    Initilize
End Sub
Private Sub cmdOK_Click()
    On Error GoTo ErrHandler
    If cmdOK.Caption = "Close" Then
        Unload Me
    Else
        If ApplyEdit Then Initilize
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmSaveAs", "cmdOK_Click", Err.Description
    Resume ErrExit
End Sub
Private Sub cmdOK_LostFocus()
    StatusBar1.SimpleText = ""
End Sub
Private Function Exists() As Boolean
'---------------------------------------------------------------------------------------
' Procedure : NotExists
' Author    : David
' Date      : 3/6/2012
' Purpose   : checks if a file name or folder name already exists
'---------------------------------------------------------------------------------------
'
    Dim FSO As FileSystemObject
    Dim DataLocation As String
    On Error GoTo ErrHandler
    Set FSO = New FileSystemObject
    DataLocation = AD.Folders(AppDatabase)
    If FSO.FileExists(DataLocation & "\" & textbox(0) & ".mdb") Then
        Exists = True
    Else
        If FSO.FolderExists(DataLocation & "\" & textbox(0)) Then
            Exists = True
        End If
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
    AD.DisplayError Err.Number, "frmSaveAs", "NotExists", Err.Description
    Resume ErrExit
End Function
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
    AD.DisplayError Err.Number, "frmSaveAs", "Form_KeyDown", Err.Description
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
    Initilize
    LoadOK = True
ErrExit:
End Sub
Private Sub Form_Unload(Cancel As Integer)
    On Error Resume Next
    ValidateControls
    If Err = 380 Then
        'control validation event set to cancel
        Cancel = True
    Else
        If Edited Then Cancel = CancelExit
    End If
    If Not Cancel Then
        AD.SaveFormData Me
    End If
End Sub
Private Sub Initilize()
    Dim CK As Long
    On Error GoTo ErrHandler
    For CK = 0 To 0
        textbox(CK) = ""
    Next CK
    SetStatusEdited False
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmSaveAs", "Initilize", Err.Description
    Resume ErrExit
End Sub
Private Sub SetStatusEdited(NewVal As Boolean)
    On Error GoTo ErrHandler
    Edited = NewVal
    cmdCancel.Enabled = NewVal
    If Edited Then
        cmdOK.Caption = "Save"
    Else
        cmdOK.Caption = "Close"
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSaveAs", "SetStatusEdited", Err.Description
     Resume ErrExit
End Sub
Private Sub textbox_GotFocus(Index As Integer)
    textbox(Index).SelStart = 0
    textbox(Index).SelLength = Len(textbox(Index).Text)
End Sub
Private Sub textbox_Validate(Index As Integer, Cancel As Boolean)
    On Error GoTo ErrHandler
    If textbox(Index) <> "" Then
        If LegalFileName(textbox(Index)) Then
            If Exists Then
                StatusBar1.SimpleText = "File/Folder exists."
                Beep
                Cancel = True
                textbox_GotFocus (Index)
            Else
                SetStatusEdited True
            End If
        Else
            StatusBar1.SimpleText = "Not a legal file name."
            Beep
            Cancel = True
            textbox_GotFocus (Index)
        End If
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSaveAs", "TextBox_Validate", Err.Description
     Resume ErrExit
End Sub
