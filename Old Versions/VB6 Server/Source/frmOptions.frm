VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Begin VB.Form frmOptions 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Options"
   ClientHeight    =   4875
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   4380
   HelpContextID   =   30
   Icon            =   "frmOptions.frx":0000
   KeyPreview      =   -1  'True
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   4875
   ScaleWidth      =   4380
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.TextBox textbox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   5
      Left            =   3400
      MaxLength       =   50
      TabIndex        =   5
      Tag             =   "3"
      Top             =   2265
      Width           =   495
   End
   Begin VB.TextBox textbox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   4
      Left            =   2235
      MaxLength       =   50
      TabIndex        =   4
      Tag             =   "3"
      Top             =   2265
      Width           =   495
   End
   Begin VB.TextBox textbox 
      Alignment       =   2  'Center
      Height          =   570
      Index           =   2
      Left            =   120
      MaxLength       =   255
      MultiLine       =   -1  'True
      TabIndex        =   7
      Tag             =   "3"
      Top             =   3240
      Width           =   4095
   End
   Begin VB.CommandButton butLocation 
      Caption         =   "Save Location :"
      Height          =   375
      Left            =   120
      TabIndex        =   6
      Top             =   2760
      Width           =   1335
   End
   Begin VB.CheckBox CkSaveReport 
      Alignment       =   1  'Right Justify
      Caption         =   "Auto-save Sensor Report"
      Height          =   255
      Left            =   120
      TabIndex        =   3
      Top             =   1800
      Width           =   3200
   End
   Begin VB.TextBox textbox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   1
      Left            =   3120
      MaxLength       =   50
      TabIndex        =   1
      Tag             =   "3"
      Top             =   600
      Width           =   1095
   End
   Begin VB.CommandButton butDefaults 
      Caption         =   "Load Defaults"
      Height          =   375
      Left            =   120
      TabIndex        =   15
      TabStop         =   0   'False
      Top             =   3960
      Width           =   1335
   End
   Begin VB.TextBox textbox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   3
      Left            =   3120
      MaxLength       =   50
      TabIndex        =   2
      Tag             =   "3"
      Top             =   1080
      Width           =   1095
   End
   Begin VB.TextBox textbox 
      Alignment       =   2  'Center
      Height          =   285
      Index           =   0
      Left            =   3120
      MaxLength       =   50
      TabIndex        =   0
      Tag             =   "3"
      Top             =   120
      Width           =   1095
   End
   Begin VB.CommandButton butCancel 
      Caption         =   "Cancel"
      Height          =   375
      Left            =   1980
      TabIndex        =   16
      TabStop         =   0   'False
      Top             =   3960
      Width           =   855
   End
   Begin VB.CommandButton butSave 
      Caption         =   "Save"
      Height          =   375
      Left            =   3360
      TabIndex        =   8
      Top             =   3960
      Width           =   855
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   17
      Top             =   4500
      Width           =   4380
      _ExtentX        =   7726
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
   Begin VB.Label Label4 
      AutoSize        =   -1  'True
      Caption         =   "Hrs"
      Height          =   195
      Left            =   3975
      TabIndex        =   14
      Top             =   2310
      Width           =   240
   End
   Begin VB.Label Label3 
      AutoSize        =   -1  'True
      Caption         =   "Days"
      Height          =   195
      Left            =   2805
      TabIndex        =   13
      Top             =   2310
      Width           =   360
   End
   Begin VB.Label Label2 
      Caption         =   "Save Interval"
      Height          =   255
      Left            =   120
      TabIndex        =   12
      Top             =   2280
      Width           =   975
   End
   Begin VB.Line Line 
      X1              =   120
      X2              =   4200
      Y1              =   1560
      Y2              =   1560
   End
   Begin VB.Label Label1 
      Caption         =   "Client Delay (seconds)"
      Height          =   255
      Left            =   120
      TabIndex        =   10
      ToolTipText     =   "Send Delay between Clients"
      Top             =   600
      Width           =   2055
   End
   Begin VB.Label Label6 
      Caption         =   "Alarm Temperature"
      Height          =   255
      Left            =   120
      TabIndex        =   11
      Top             =   1080
      Width           =   2055
   End
   Begin VB.Label Label12 
      Caption         =   "Record Inverval  (minutes)"
      Height          =   255
      Left            =   120
      TabIndex        =   9
      Top             =   120
      Width           =   2055
   End
End
Attribute VB_Name = "frmOptions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private locLoadOK As Boolean

Private Function ApplyEdit() As Boolean
    On Error GoTo ErrHandler
    Dim r As Long
    Dim Mes As String
    ApplyEdit = False
    If Prog.IsValid Then
        Prog.ApplyEdit
        ApplyEdit = True
    Else
        For r = 1 To Prog.BrokenRules.Count
            Mes = Mes & Prog.BrokenRules.RuleDescription(r) & " "
        Next r
        StatusBar1.SimpleText = Mes
        Beep
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmInvoices", "ApplyEdit", Err.Description
     Resume ErrExit
End Function

Private Sub butCancel_Click()
    On Error GoTo ErrHandler
    Prog.CancelEdit
    Prog.BeginEdit
    UpdateForm
    butSave.Caption = "Close"
    butCancel.Enabled = False
    textbox(0).SetFocus
    DoReportSettings
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "butCancel_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub butDefaults_Click()
    On Error GoTo ErrHandler
    Prog.LoadDefaults
    CheckChanged
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmOptions", "butDefaults_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butLocation_Click()
    On Error GoTo ErrHandler
    Dim BR As UnicodeBrowseFolders
    Set BR = New UnicodeBrowseFolders
    BR.Flags = BIF_NEWDIALOGSTYLe
    BR.InitialDirectory = Prog.Location
    BR.ShowBrowseForFolder Me.hWnd
    If BR.SelectedFolder <> "" Then
        textbox(2) = BR.SelectedFolder
        textbox_Validate 2, 0
    End If
    Set BR = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "butLocation_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub butSave_Click()
    On Error GoTo ErrHandler
    If butSave.Caption = "Close" Then
        Unload Me
    Else
        If ApplyEdit Then
            Unload Me
        End If
    End If
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "butSave_Click", Err.Description
    Resume ErrExit
End Sub

Private Function CancelExit() As Boolean
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
     AD.DisplayError Err.Number, "frmOptions", "CancelExit", Err.Description
     Resume ErrExit
End Function

Private Sub CheckChanged()
    On Error GoTo ErrHandler
    If Prog.DataChanged Then
        UpdateForm
        butSave.Caption = "Save"
        butCancel.Enabled = True
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "CheckChanged", Err.Description
    Resume ErrExit
End Sub

Private Sub CkSaveReport_Click()
    StatusBar1.SimpleText = ""
    If CkSaveReport.Value = 0 Then
        Prog.AutoSave = False
    Else
        Prog.AutoSave = True
    End If
    DoReportSettings
    CheckChanged
End Sub

Private Sub DoReportSettings()
    On Error GoTo ErrHandler
    textbox(4).Enabled = (CkSaveReport.Value = 1)
    textbox(5).Enabled = (CkSaveReport.Value = 1)
    textbox(2).Enabled = (CkSaveReport.Value = 1)
    butLocation.Enabled = (CkSaveReport.Value = 1)
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "DoReportSettings", Err.Description
    Resume ErrExit
End Sub

Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    On Error GoTo ErrHandler
    Select Case KeyCode
        Case 38
            'up arrow
            SendKeys ("+{tab}")
        Case 40
            'down arrow
            SendKeys ("{tab}")
    End Select
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "Form_KeyDown", Err.Description
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
    butSave.Caption = "Close"
    butCancel.Enabled = False
    StatusBar1.SimpleText = ""
    Prog.BeginEdit
    UpdateForm
    DoReportSettings
    locLoadOK = True
ErrExit:
End Sub

Private Sub Form_Unload(Cancel As Integer)
    On Error Resume Next
    ValidateControls
    If Err = 380 Then
        'control validation event set to cancel
        Cancel = True
    Else
        If Prog.DataChanged Then Cancel = CancelExit
    End If
    If Not Cancel Then
        AD.SaveFormData Me
        Prog.CancelEdit
        frmStart.ControlBoxDelay = Prog.ControlBoxDelay
    End If
End Sub

Private Sub textbox_GotFocus(Index As Integer)
    textbox(Index).SelStart = 0
    textbox(Index).SelLength = Len(textbox(Index).Text)
End Sub

Private Sub textbox_Validate(Index As Integer, Cancel As Boolean)
    On Error GoTo ErrHandler
    Dim ER As Long
    StatusBar1.SimpleText = ""
    With Prog
        Select Case Index
            Case 0
                .RecordInterval = CLng(textbox(Index))
            Case 1
                .ControlBoxDelay = CLng(textbox(Index))
            Case 2
                .Location = textbox(Index)
            Case 3
                .MaxTemp = CLng(textbox(Index))
            Case 4
                .Days = CLng(textbox(Index))
            Case 5
                .Hours = CLng(textbox(Index))
        End Select
    End With
    CheckChanged
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
            Cancel = True
            UpdateForm
            textbox_GotFocus Index
        Case Else
            AD.DisplayError Err.Number, "frmOptions", "textbox_Validate", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub UpdateForm()
    On Error GoTo ErrHandler
    With Prog
        textbox(0).Text = .RecordInterval
        textbox(1).Text = .ControlBoxDelay
        textbox(2).Text = .Location
        textbox(3).Text = .MaxTemp
        textbox(4).Text = .Days
        textbox(5).Text = .Hours
        If .AutoSave Then
            CkSaveReport.Value = 1
        Else
            CkSaveReport.Value = 0
        End If
    End With
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmOptions", "UpdateForm", Err.Description
    Resume ErrExit
End Sub

