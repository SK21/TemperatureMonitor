VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Object = "{AA2080D5-36CF-435E-826D-3A123F24101A}#11.0#0"; "dcGridControl.ocx"
Begin VB.Form frmBinMapsSht 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "BinMaps"
   ClientHeight    =   4575
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   9120
   Icon            =   "frmBinMapsSht.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   4575
   ScaleWidth      =   9120
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin dcGridControl.dcGrid Grd 
      Height          =   3975
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   8895
      _ExtentX        =   15690
      _ExtentY        =   7011
      Cols            =   3
      Rows            =   28
      focus           =   -1  'True
      LockColour      =   12648447
      GridColour      =   12632256
      BackColorFixed  =   16777152
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   1
      Top             =   4200
      Width           =   9120
      _ExtentX        =   16087
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
End
Attribute VB_Name = "frmBinMapsSht"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Dim ErrCol As Long
Dim ExitCount As Long   '# of attempts to exit form, ensures a form can be closed

Private Sub AddRecords()
    Dim Col As Maps
    Dim Obj As MapDisplay
    Dim C As Long
    On Error GoTo ErrHandler
    Grd.ClearRecords
    Set Col = New Maps
    Col.Load
    C = Col.Count + Grd.FixedRows + 1
    If C > Grd.Rows Then Grd.Rows = C
    For Each Obj In Col
        With Grd
            .AddRecord 1, Obj.MapName, Obj.ID
            .AddRecord 2, Obj.MapNotes, Obj.ID
        End With
    Next
    Set Obj = Nothing
    Set Col = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinMapsSht", "AddRecords", Err.Description
     Resume ErrExit
End Sub

Private Sub BuildHeaders()
    On Error GoTo ErrHandler
    With Grd
        .AddHeader 0, "ID", 0, , , CT_Number
        .AddHeader 1, "Map Name", 2500, "<", , CT_String
        .AddHeader 2, "Notes", 3000, "<", , CT_String
    End With
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinMapsSht", "BuildHeaders", Err.Description
     Resume ErrExit
End Sub

Private Sub BuildLists()
    'no lists
End Sub

Private Sub Form_DblClick()
    Grd.PrintReport
End Sub

Private Sub Form_Load()
    On Error GoTo ErrHandler
    AD.LoadFormData Me
    ExitCount = 0
    SetupGrid
    Grd.FontsFolder = AD.Folders(AppDatabase) & "\Fonts"
    Grd.PrintFolder = AD.Folders(AppCommon)
    Grd.Title = "Maps"
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinMapsSht", "Form_Load", Err.Description
     Resume ErrExit
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ExitCount = ExitCount + 1
    If Grd.RecordChanged Then Cancel = GridCancelExit
    If ExitCount > 3 Then Cancel = False
    If Not Cancel Then
        AD.SaveFormData Me
    End If
End Sub

Private Function GridCancelExit() As Boolean
    'checks if user wants to save changes to grid
    Dim Done As Boolean
    On Error GoTo ErrHandler
    Done = False
    Do While Not Done
        Select Case MsgBox("Do you want to save changes to last record?", vbYesNoCancel Or vbExclamation Or vbDefaultButton1, App.Title)
           Case vbYes
                GridCancelExit = False
                Done = Grd.CloseRecord
           Case vbNo
                GridCancelExit = False
                Done = True
           Case vbCancel
                GridCancelExit = True
                Done = True
        End Select
    Loop
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, Me.Name, "GridCancelExit", Err.Description
     Resume ErrExit
End Function

Private Sub Grd_DeleteRecord(ID As Long)
    Dim ER As Long
    Dim Obj As Map
    On Error GoTo ErrHandler
    Select Case MsgBox("Delete record?", vbOKCancel Or vbQuestion Or vbDefaultButton1, App.Title)
        Case vbOK
            Set Obj = New Map
            Obj.Load ID
            Obj.BeginEdit
            Obj.Delete
            Obj.ApplyEdit
            AddRecords
            Set Obj = Nothing
    End Select
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    'convert err number to object error
    ER = (Err.Number And &HFFFF&)
    Select Case ER
        Case 1001
            'object input error
            Beep
            StatusBar1.SimpleText = Err.Description
        Case Else
            AD.DisplayError Err.Number, "frmBinMapsSht", "Grd_DeleteRecord", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub Grd_RowColChange(CurrentRow As Long, CurrentCol As Long)
    On Error GoTo ErrHandler
    If CurrentCol <> ErrCol Then StatusBar1.SimpleText = ""
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinMapsSht", "Grd_RowColChange", Err.Description
     Resume ErrExit
End Sub

Private Sub Grd_ValidateRecord(r As Long, ErrorCol As Long)
    Dim Obj As Map
    Dim ER As Long
    Dim Col As Long
    Dim Rule As Long
    On Error GoTo ErrHandler
    Set Obj = New Map
    With Obj
        .Load Val(Grd.Cell(r, 0))
        .BeginEdit
        Col = 1
        .MapName = Grd.Cell(r, Col)
        Col = 2
        .MapNotes = Grd.Cell(r, Col)
        If .IsValid Then
            .ApplyEdit
            Grd.Cell(r, 0) = Obj.ID
            ErrCol = -1
        Else
            Beep
            StatusBar1.SimpleText = ""
            For Rule = 1 To Obj.BrokenRules.Count
                StatusBar1.SimpleText = StatusBar1.SimpleText & "  " & Obj.BrokenRules.RuleDescription(Rule)
            Next Rule
            ErrorCol = 1
            ErrCol = 1
        End If
    End With
    On Error GoTo 0
ErrExit:
    Set Obj = Nothing
    Exit Sub
ErrHandler:
    'convert err number to object error
    ER = (Err.Number And &HFFFF&)
    Select Case ER
        Case 1001
            'object input error
            Beep
            StatusBar1.SimpleText = Err.Description
            ErrorCol = Col
            ErrCol = Col
        Case Else
            AD.DisplayError Err.Number, "frmBinMapsSht", "Grd_ValidateRecord", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub SetupGrid()
    On Error GoTo ErrHandler
    With Grd
        .Reset
        .Rows = 20
        .Cols = 3
        .AllowNewRow = True
    End With
    BuildLists
    AddRecords
    BuildHeaders
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinMapsSht", "SetupGrid", Err.Description
     Resume ErrExit
End Sub
