VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Object = "{AA2080D5-36CF-435E-826D-3A123F24101A}#11.0#0"; "dcGridControl.ocx"
Begin VB.Form frmSensorsSht 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Sensors"
   ClientHeight    =   8865
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   18135
   HelpContextID   =   20
   Icon            =   "frmSensorsSht.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   8865
   ScaleWidth      =   18135
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton butPrint 
      Caption         =   "Print"
      Height          =   390
      Left            =   14280
      TabIndex        =   11
      ToolTipText     =   "Print report."
      Top             =   7905
      Width           =   1000
   End
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   1000
      Left            =   12000
      Top             =   120
   End
   Begin VB.CommandButton butRefresh 
      Caption         =   "Reload"
      Enabled         =   0   'False
      Height          =   390
      Left            =   12960
      TabIndex        =   7
      ToolTipText     =   "Refresh sensor list."
      Top             =   7200
      Width           =   1000
   End
   Begin VB.CommandButton butClose 
      Caption         =   "Close"
      Height          =   390
      Left            =   16920
      TabIndex        =   13
      Top             =   7905
      Width           =   1000
   End
   Begin VB.CommandButton butWrite 
      Caption         =   "Write"
      Enabled         =   0   'False
      Height          =   390
      Left            =   15600
      TabIndex        =   9
      ToolTipText     =   "Write to selected sensor."
      Top             =   7200
      Width           =   1000
   End
   Begin VB.CommandButton butCheck 
      Caption         =   "Check"
      Enabled         =   0   'False
      Height          =   390
      Left            =   14280
      TabIndex        =   8
      ToolTipText     =   "Check values stored in selected sensor."
      Top             =   7200
      Width           =   1000
   End
   Begin VB.TextBox tbEvents 
      Height          =   5535
      Left            =   12480
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      ScrollBars      =   2  'Vertical
      TabIndex        =   5
      Top             =   960
      Width           =   5495
   End
   Begin VB.CommandButton butWriteAll 
      Caption         =   "Write to All"
      Height          =   390
      Left            =   16920
      TabIndex        =   10
      ToolTipText     =   "Write to all sensors in database."
      Top             =   7200
      Width           =   1000
   End
   Begin VB.CommandButton butCancel 
      Caption         =   "Cancel"
      Enabled         =   0   'False
      Height          =   390
      Left            =   15600
      TabIndex        =   12
      Top             =   7905
      Width           =   1000
   End
   Begin MSComctlLib.StatusBar StatusBar1 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   14
      Top             =   8490
      Width           =   18135
      _ExtentX        =   31988
      _ExtentY        =   661
      Style           =   1
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   1
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
         EndProperty
      EndProperty
   End
   Begin dcGridControl.dcGrid Grd 
      Height          =   7700
      Left            =   240
      TabIndex        =   2
      Top             =   600
      Width           =   12060
      _ExtentX        =   21273
      _ExtentY        =   13573
      Cols            =   3
      Rows            =   30
      focus           =   -1  'True
      LockColour      =   12648447
      GridColour      =   12632256
      BackColorFixed  =   16777152
      ShowZeros       =   -1  'True
   End
   Begin MSComctlLib.ProgressBar ProgressBar1 
      Height          =   375
      Left            =   12480
      TabIndex        =   6
      Top             =   6720
      Width           =   5490
      _ExtentX        =   9684
      _ExtentY        =   661
      _Version        =   393216
      Appearance      =   1
   End
   Begin VB.Label Label3 
      Alignment       =   2  'Center
      AutoSize        =   -1  'True
      Caption         =   "Sensor Database"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   -1  'True
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Left            =   240
      TabIndex        =   0
      Top             =   120
      Width           =   11055
   End
   Begin VB.Label Label2 
      Alignment       =   2  'Center
      AutoSize        =   -1  'True
      Caption         =   "Read/Write Sensors"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   -1  'True
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Left            =   13695
      TabIndex        =   1
      Top             =   120
      Width           =   2145
   End
   Begin VB.Label LbSelected 
      AutoSize        =   -1  'True
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Left            =   14500
      TabIndex        =   4
      Top             =   600
      Width           =   45
   End
   Begin VB.Label Label1 
      AutoSize        =   -1  'True
      Caption         =   "Selected Sensor :"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Index           =   1
      Left            =   12480
      TabIndex        =   3
      Top             =   600
      Width           =   1860
   End
End
Attribute VB_Name = "frmSensorsSht"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Dim ErrCol As Long
Dim Talking As Boolean  'read/write with sensors
Dim SenSort As SensorSort
Dim ExitCount As Long   '# of attempts to exit form, ensures a form can be closed

Const WaitTime = 10        'for reading/writing to sensor
Dim StartTime As Date
Public SensorData As Long       'data stored on sensor, comes from frmStart/ProcessData
Public SensorTemp As Long       'sensor temperature from frmStart/ProcessData
Dim Requests As clsPackets      'collection of packet requests to be sent to frmStart/AddPackets
Dim Waiting As Boolean          'waiting for read/write to complete
Dim PreviousRequest As clsPacket
Dim modSensorID As Long 'ID of sensor beging checked or written to

Private Sub AddRecords()
    Dim Col As clsSensors
    Dim Obj As clsSensor
    Dim C As Long
    On Error GoTo ErrHandler
    Grd.ClearRecords
    Set Col = New clsSensors
    Col.Load SenSort
    C = Col.Count + Grd.FixedRows + 1
    If C > Grd.Rows Then Grd.Rows = C
    For Each Obj In Col
        With Grd
            .AddRecord 1, Obj.BinID, Obj.ID
            .AddRecord 2, Obj.CableNumber, Obj.ID
            .AddRecord 3, Obj.SensorNumber, Obj.ID
            .AddRecord 4, TrueFalse(Obj.Enabled), Obj.ID
            .AddRecord 5, Obj.Offset, Obj.ID
            .AddRecord 6, Obj.CalTemp, Obj.ID
            .AddRecord 7, Obj.LastReading, Obj.ID
            .AddRecord 8, Obj.RomCode, Obj.ID
        End With
    Next
    Set Obj = Nothing
    Set Col = Nothing
    LbSelected = Grd.Cell(Grd.Row, 8)
    SetButtons
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "AddRecords", Err.Description
     Resume ErrExit
End Sub

Private Sub BuildHeaders()
    On Error GoTo ErrHandler
    With Grd
        .AddHeader 0, "ID", 0, , , CT_Number
        .AddHeader 1, "Bin", 2600, "^", , CT_Combo
        .AddHeader 2, "Cable #", 800, "^", , CT_Number
        .AddHeader 3, "Sensor #", 900, "^", , CT_Number
        .AddHeader 4, "Enabled", 800, "^", , CT_Combo
        .AddHeader 5, "Offset", 800, "^", , CT_Number
        .AddHeader 6, "Last Temp", 1100, "^", , CT_Number
        .AddHeader 7, "Last Reading", 1800, "^", , CT_DateTime
        .AddHeader 8, "Rom Code", 2700, "^", , CT_String
    End With
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "BuildHeaders", Err.Description
     Resume ErrExit
End Sub

Private Sub BuildLists()
    Dim Col As Storages
    Dim Obj As StorageDisplay
    'bins
    On Error GoTo ErrHandler
    Set Col = New Storages
    Col.Load
    Grd.DropList(1) = Col.DropList
    'enabled
    Grd.BuildList 4, -1, "True"
    Grd.BuildList 4, 0, "False"
    Set Obj = Nothing
    Set Col = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "BuildLists", Err.Description
     Resume ErrExit
End Sub

Private Sub butCancel_Click()
    On Error GoTo ErrHandler
    Timer1.Enabled = False
    ProgressBar1.Value = 0
    Talking = False
    SetButtons
    Set Requests = New clsPackets   'erase left-over data
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "butCancel_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butCheck_Click()
    On Error GoTo ErrHandler
    If Grd.RecordChanged Then GridSaveRecord
    Talking = True
    SetButtons
    LogWrite
    LogWrite "Reading sensor  " & LbSelected
    frmStart.AddPackets pdGetUserData, , Grd.RecordID
    frmStart.AddPackets pdGetTemperatures, , Grd.RecordID
    modSensorID = Grd.RecordID
    SensorTemp = -99
    PreviousRequest.PKtype = pdGetUserData
    StartTime = Now
    ProgressBar1.Value = 0
    Timer1.Enabled = True
    SensorData = -1
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "butCheck_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butClose_Click()
    Unload Me
End Sub

Private Sub butPrint_Click()
    On Error GoTo ErrHandler
    Screen.MousePointer = vbHourglass
    Grd.PrintReport
    On Error GoTo 0
ErrExit:
    Screen.MousePointer = vbDefault
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmSensorsSht", "butPrint_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub butRefresh_Click()
    On Error GoTo ErrHandler
    If Grd.RecordChanged Then GridSaveRecord
    AddRecords
    LbSelected = Grd.Cell(Grd.Row, 8)
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "butRefresh_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butWrite_Click()
    Dim Obj As clsSensor
    Dim NewVal As Long
    On Error GoTo ErrHandler
    On Error GoTo ErrHandler
    If Grd.RecordChanged Then GridSaveRecord
    Talking = True
    SetButtons
    Set Obj = New clsSensor
    Obj.Load Grd.RecordID
    LogWrite
    LogWrite "Writing to sensor " & Obj.RomCode
    NewVal = MakeUserData(Val(Obj.BinNumber), Val(Obj.CableNumber), Val(Obj.SensorNumber))
    frmStart.AddPackets pdSetUserData, CStr(NewVal), Obj.ID
    PreviousRequest.PKtype = pdSetUserData
    PreviousRequest.RomCode = Obj.RomCode
    PreviousRequest.SensorID = Obj.ID
    StartTime = Now
    ProgressBar1.Value = 0
    Timer1.Enabled = True
    SensorData = -1
    Set Obj = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "butWrite_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butWriteAll_Click()
    Dim Col As clsSensors
    Dim Obj As clsSensor
    Dim NewVal As Long
    On Error GoTo ErrHandler
    If Grd.RecordChanged Then GridSaveRecord
    Set Requests = New clsPackets
    Talking = True
    SetButtons
    Set Col = New clsSensors
    Col.Load ssBinNumberAsc
    For Each Obj In Col
        NewVal = MakeUserData(Val(Obj.BinNumber), Val(Obj.CableNumber), Val(Obj.SensorNumber))
        Requests.Add 0, pdSetUserData, CStr(NewVal), Obj.RomCode, , , Obj.ID
    Next
    If Requests.Count > 0 Then
        'get new request
        LogWrite
        LogWrite "Writing to sensor " & Requests(1).RomCode
        frmStart.AddPackets Requests(1).PKtype, Requests(1).Value, Requests(1).SensorID
        modSensorID = -1
        PreviousRequest.PKtype = Requests(1).PKtype
        PreviousRequest.SensorID = Requests(1).SensorID
        PreviousRequest.RomCode = Requests(1).RomCode
        Requests.Remove 1
        StartTime = Now
        ProgressBar1.Value = 0
        Timer1.Enabled = True
        SensorData = -1
    End If
    Set Col = Nothing
    Set Obj = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "butWriteAll_Click", Err.Description
     Resume ErrExit
End Sub

Public Sub DoPrint()
    Grd.PrintReport False
End Sub

Private Sub Form_Load()
    On Error GoTo ErrHandler
    AD.LoadFormData Me
    Set Requests = New clsPackets
    Set PreviousRequest = New clsPacket
    ProgressBar1.Min = 0
    ProgressBar1.Max = WaitTime
    ExitCount = 0
    SenSort = LastSort
    SetupGrid
    frmStart.SendTofrmSensorsSht = True
    Talking = False
    SetButtons
    LbSelected = Grd.Cell(Grd.Row, 8)
    Grd.FontsFolder = AD.Folders(AppDatabase) & "\Fonts"
    Grd.PrintFolder = AD.Folders(AppCommon)
    Grd.Title = "Sensors"
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "Form_Load", Err.Description
     Resume ErrExit
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ExitCount = ExitCount + 1
    'check if still communicating with sensors
    If Talking Then
        Cancel = True
        butCancel_Click
        Beep
    End If
    If Grd.RecordChanged Then GridSaveRecord
    If ExitCount > 3 Then Cancel = False
    If Not Cancel Then
        AD.SaveFormData Me
        AD.Element("SensorSort") = SenSort
        frmStart.SendTofrmSensorsSht = False
        Set Requests = Nothing
        Set PreviousRequest = Nothing
        Timer1.Enabled = False
    End If
End Sub

Sub GetUserData(NewData As Long, BinNum As Long, CableNum As Long, SensorNum As Long)
    On Error GoTo ErrHandler
    If NewData > -1 Then
        'good data
        BinNum = ShiftRight(NewData, 8)    'shift right to lower 8 bits
        CableNum = NewData And 240     'remove top 8 and lower 4 bits, 0000 0000 1111 0000
        CableNum = ShiftRight(CableNum, 4) 'shift right to lower 4 bits
        SensorNum = NewData And 15     'remove top 12 bits, 0000 0000 0000 1111
        'convert to base 1
        BinNum = BinNum + 1
        CableNum = CableNum + 1
        SensorNum = SensorNum + 1
    Else
        'not valid data
        BinNum = 0
        CableNum = 0
        SensorNum = 0
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "GetUserData", Err.Description
     Resume ErrExit
End Sub

Private Sub Grd_Click(r As Long, C As Long)
    Dim Tmp As SensorSort
    On Error GoTo ErrHandler
    'check if header clicked
    If r = 0 Then
        'sort by selected column
        Select Case C
            Case 8
                Tmp = ssRomCodeAsc
                If SenSort = Tmp Then
                    SenSort = ssRomCodeDes
                Else
                    SenSort = Tmp
                End If
                AddRecords
            Case 7
                Tmp = ssLastDateAsc
                If SenSort = Tmp Then
                    'invert sort
                    SenSort = ssLastDateDes
                Else
                    SenSort = Tmp
                End If
                AddRecords
            Case 6
                Tmp = ssLastTempAsc
                If SenSort = Tmp Then
                    SenSort = ssLastTempDes
                Else
                    SenSort = Tmp
                End If
                AddRecords
            Case 1
                Tmp = ssBinNumberAsc
                If SenSort = Tmp Then
                    SenSort = ssBinNumberDes
                Else
                    SenSort = Tmp
                End If
                AddRecords
        End Select
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "Grd_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub Grd_DeleteRecord(ID As Long)
    Dim ER As Long
    Dim Obj As clsSensor
    On Error GoTo ErrHandler
    Select Case MsgBox("Delete record?", vbOKCancel Or vbQuestion Or vbDefaultButton1, App.Title)
        Case vbOK
            Set Obj = New clsSensor
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
            AD.DisplayError Err.Number, "frmSensorsSht", "Grd_DeleteRecord", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub Grd_RowColChange(CurrentRow As Long, CurrentCol As Long)
    On Error GoTo ErrHandler
    LbSelected = Grd.Cell(Grd.Row, 8)
    SetButtons
    If CurrentCol <> ErrCol Then StatusBar1.SimpleText = ""
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "Grd_RowColChange", Err.Description
     Resume ErrExit
End Sub

Private Sub Grd_ValidateRecord(r As Long, ErrorCol As Long)
    Dim Obj As clsSensor
    Dim ER As Long
    Dim Col As Long
    Dim Rule As Long
    Dim OffsetChanged As Boolean
    On Error GoTo ErrHandler
    Set Obj = New clsSensor
    With Obj
        .Load Val(Grd.Cell(r, 0))
        .BeginEdit
        Col = 1
        .BinID = Grd.Cell(r, Col)
        Col = 2
        .CableNumber = Grd.Cell(r, Col)
        Col = 3
        .SensorNumber = Grd.Cell(r, Col)
        Col = 4
        .Enabled = Grd.Cell(r, Col)
        Col = 5
        OffsetChanged = (.Offset <> Grd.Cell(r, Col))
        .Offset = Grd.Cell(r, Col)
        If .IsValid Then
            .ApplyEdit
            Grd.Cell(r, 0) = Obj.ID
            ErrCol = -1
            If OffsetChanged Then butRefresh_Click  'reload to reflect changes
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
            AD.DisplayError Err.Number, "frmSensorsSht", "Grd_ValidateRecord", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub GridSaveRecord()
    'check if record should be saved before loosing focus to another control on the form
    On Error GoTo ErrHandler
    If Not Grd.CloseRecord Then Grd.CancelEdit
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "GridSaveRecord", Err.Description
     Resume ErrExit
End Sub

Private Function LastSort() As SensorSort
    'in case wrong data
    On Error Resume Next
    LastSort = Val(AD.Element("SensorSort"))
End Function

Private Sub LogWrite(Optional Mes As String = "")
    Dim L As Long
    On Error GoTo ErrHandler
    L = Len(tbEvents)
    If L > 5000 Then
        tbEvents.Text = Right$(tbEvents.Text, 1000)
    End If
    If Mes <> "" Then Mes = Format(Now, "hh:mm:ss  AM/PM") & "    " & Mes
    tbEvents.Text = tbEvents.Text & Mes & vbNewLine
    tbEvents.SelStart = Len(tbEvents.Text)
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "LogWrite", Err.Description
     Resume ErrExit
End Sub

Function MakeUserData(BinNum As Long, CableNum As Long, SensorNum As Long) As Long
    'NewData stored in 16 bit number
    '15,14,13,12,11,10,9,8      7,6,5,4     3,2,1,0
    '11111111                   1111        1111
    'bin num                    cable       sensor
    'on the sensor, bin 0-255, cable 0-15, sensor 0-15
    'the user enters, bin 1-256, cable 1-16, sensor 1-16
    Dim BinBits As Long
    Dim CableBits As Long
    Dim SensorBits As Long
    On Error GoTo ErrHandler
    'convert to base 0
    BinNum = BinNum - 1
    CableNum = CableNum - 1
    SensorNum = SensorNum - 1
    If BinNum < 0 Then BinNum = 0
    If BinNum > 255 Then BinNum = 255
    If CableNum < 0 Then CableNum = 0
    If CableNum > 15 Then CableNum = 15
    If SensorNum < 0 Then SensorNum = 0
    If SensorNum > 15 Then SensorNum = 15
    'set bits
    BinBits = ShiftLeft(BinNum, 8)       'move to top 8 bits
    CableBits = ShiftLeft(CableNum, 4)   'move to bits 7 - 4
    SensorBits = SensorNum              'kee
    MakeUserData = BinBits Or CableBits Or SensorBits
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "MakeNewData", Err.Description
     Resume ErrExit
End Function

Public Property Get SensorID() As Long
    'sensor being read
    SensorID = modSensorID
End Property

Private Sub SetButtons()
    On Error GoTo ErrHandler
    butCancel.Enabled = Talking
    butClose.Enabled = Not Talking
    Grd.Enabled = Not Talking
    butWriteAll.Enabled = Not Talking
    butWrite.Enabled = Grd.RecordID > 0 And Not Talking
    butCheck.Enabled = Grd.RecordID > 0 And Not Talking
    butRefresh.Enabled = Not Talking
    butPrint.Enabled = Not Talking
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "SetButtons", Err.Description
     Resume ErrExit
End Sub

Private Sub SetupGrid()
    Dim r As Long
    On Error GoTo ErrHandler
    With Grd
        .Reset
        .Rows = 20
        .Cols = 9
        .AllowNewRow = True
    End With
    BuildLists
    AddRecords
    BuildHeaders
    'locked cells
    For r = Grd.FixedRows To Grd.Rows - 1
        Grd.CellLocked(r, 6) = True
        Grd.CellLocked(r, 7) = True
        Grd.CellLocked(r, 8) = True
    Next r
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "SetupGrid", Err.Description
     Resume ErrExit
End Sub

Function ShiftLeft(ByVal lngNumber As Long, ByVal intNumBits As Integer) As Long
    '--------------
    'BIT SHIFT LEFT
    '--------------
    On Error GoTo ErrHandler
    ShiftLeft = lngNumber * 2 ^ intNumBits
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "ShiftLeft", Err.Description
     Resume ErrExit
End Function

Function ShiftRight(ByVal lngNumber As Long, ByVal intNumBits As Integer) As Long
    '--------------
    'BIT SHIFT RIGHT
    '--------------
    On Error GoTo ErrHandler
    ShiftRight = lngNumber \ 2 ^ intNumBits 'note the integer division op
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "ShiftRight", Err.Description
     Resume ErrExit
End Function

Private Sub Timer1_Timer()
    Dim Seconds As Long
    Dim BinNum As Long
    Dim CableNum As Long
    Dim SensorNum As Long
    Dim Tmp As String
    On Error GoTo ErrHandler
    Seconds = DateDiff("s", StartTime, Now)
    ShowProgress ProgressBar1, Seconds
    Waiting = (Seconds < WaitTime)
    If Not Waiting Then
        If PreviousRequest.PKtype = pdGetUserData Then
            'show results
            GetUserData SensorData, BinNum, CableNum, SensorNum
            Tmp = ""
            If SensorTemp > -99 Then Tmp = "  Temp: " & SensorTemp 'check for valid temp data
            LogWrite "On Sensor>  Bin: " & BinNum & "  Cable: " & CableNum & "  Sensor: " & SensorNum & Tmp
            If Requests.Count > 0 Then
                'get new request
                LogWrite
                LogWrite "Writing to sensor " & Requests(1).RomCode
                frmStart.AddPackets Requests(1).PKtype, Requests(1).Value, Requests(1).SensorID
                PreviousRequest.PKtype = Requests(1).PKtype
                PreviousRequest.SensorID = Requests(1).SensorID
                PreviousRequest.RomCode = Requests(1).RomCode
                Requests.Remove 1
                StartTime = Now
                ProgressBar1.Value = 0
                SensorData = -1
            Else
                'done
                butCancel_Click
            End If
        Else
            'previous write request
            'request sensor data results
            LogWrite
            LogWrite "Reading sensor " & PreviousRequest.RomCode
            frmStart.AddPackets pdGetUserData, , PreviousRequest.SensorID
            frmStart.AddPackets pdGetTemperatures, , PreviousRequest.SensorID
            modSensorID = PreviousRequest.SensorID
            SensorTemp = -99
            PreviousRequest.PKtype = pdGetUserData
            StartTime = Now
            ProgressBar1.Value = 0
        End If
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "Timer1_Timer", Err.Description
     Resume ErrExit
End Sub

Private Function TrueFalse(Val As Boolean) As String
    On Error GoTo ErrHandler
    If Val Then
        TrueFalse = "True"
    Else
        TrueFalse = "False"
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmSensorsSht", "TrueFalse", Err.Description
     Resume ErrExit
End Function

