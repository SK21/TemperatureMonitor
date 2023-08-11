VERSION 5.00
Object = "{D9794188-4A19-4061-A8C1-BCC9E392E6DB}#3.1#0"; "dcCombo.ocx"
Object = "{5E9E78A0-531B-11CF-91F6-C2863C385E30}#1.0#0"; "MSFlxGrd.ocx"
Object = "{BDC217C8-ED16-11CD-956C-0000C04E4C0A}#1.1#0"; "TabCtl32.ocx"
Object = "{86CF1D34-0C5F-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomct2.ocx"
Object = "{65E121D4-0C60-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSChrt20.ocx"
Begin VB.Form frmBinReport 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Bin Reports"
   ClientHeight    =   11880
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   17025
   Icon            =   "frmBinReport.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   11880
   ScaleWidth      =   17025
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.PictureBox Picture 
      BorderStyle     =   0  'None
      Height          =   855
      Left            =   14640
      ScaleHeight     =   855
      ScaleWidth      =   1935
      TabIndex        =   17
      Top             =   200
      Width           =   1935
      Begin VB.OptionButton OptScale 
         Caption         =   "by hour"
         Height          =   375
         Index           =   0
         Left            =   720
         TabIndex        =   19
         Top             =   360
         Width           =   1215
      End
      Begin VB.OptionButton OptScale 
         Caption         =   "by day"
         Height          =   375
         Index           =   1
         Left            =   720
         TabIndex        =   18
         Top             =   0
         Value           =   -1  'True
         Width           =   1215
      End
      Begin VB.Label Label1 
         Caption         =   "Sort:"
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
         Index           =   3
         Left            =   0
         TabIndex        =   20
         Top             =   50
         Width           =   900
      End
   End
   Begin VB.OptionButton Opt 
      Caption         =   "Single"
      Height          =   375
      Index           =   0
      Left            =   8280
      TabIndex        =   12
      Top             =   195
      Value           =   -1  'True
      Width           =   975
   End
   Begin VB.OptionButton Opt 
      Caption         =   "Range"
      Height          =   375
      Index           =   1
      Left            =   8280
      TabIndex        =   11
      Top             =   720
      Width           =   975
   End
   Begin VB.CommandButton butMonth 
      Caption         =   "This Month"
      Height          =   375
      Left            =   12240
      TabIndex        =   10
      Top             =   195
      Width           =   1200
   End
   Begin VB.CommandButton butWeek 
      Caption         =   "This Week"
      Height          =   375
      Left            =   10920
      TabIndex        =   9
      Top             =   720
      Width           =   1200
   End
   Begin VB.CommandButton butToday 
      Caption         =   "Today"
      Height          =   375
      Left            =   10920
      TabIndex        =   8
      Top             =   195
      Width           =   1200
   End
   Begin VB.CommandButton butUpdate 
      Caption         =   "Update"
      Height          =   375
      Left            =   12240
      TabIndex        =   6
      Top             =   720
      Width           =   1200
   End
   Begin dcCombo.dcComboControl Combo 
      Height          =   375
      Index           =   0
      Left            =   1080
      TabIndex        =   1
      Top             =   180
      Width           =   4935
      _ExtentX        =   8705
      _ExtentY        =   661
      HideAdd         =   -1  'True
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
   End
   Begin TabDlg.SSTab SSTab1 
      Height          =   10335
      Left            =   240
      TabIndex        =   2
      Top             =   1320
      Width           =   16575
      _ExtentX        =   29236
      _ExtentY        =   18230
      _Version        =   393216
      Tabs            =   2
      TabsPerRow      =   2
      TabHeight       =   520
      TabCaption(0)   =   "Graph"
      TabPicture(0)   =   "frmBinReport.frx":0442
      Tab(0).ControlEnabled=   -1  'True
      Tab(0).Control(0)=   "Label1(1)"
      Tab(0).Control(0).Enabled=   0   'False
      Tab(0).Control(1)=   "MSChart1"
      Tab(0).Control(1).Enabled=   0   'False
      Tab(0).Control(2)=   "Combo(1)"
      Tab(0).Control(2).Enabled=   0   'False
      Tab(0).ControlCount=   3
      TabCaption(1)   =   "Table"
      TabPicture(1)   =   "frmBinReport.frx":045E
      Tab(1).ControlEnabled=   0   'False
      Tab(1).Control(0)=   "Grid"
      Tab(1).ControlCount=   1
      Begin MSFlexGridLib.MSFlexGrid Grid 
         Height          =   9375
         Left            =   -74760
         TabIndex        =   3
         Top             =   720
         Width           =   16095
         _ExtentX        =   28390
         _ExtentY        =   16536
         _Version        =   393216
      End
      Begin dcCombo.dcComboControl Combo 
         Height          =   375
         Index           =   1
         Left            =   8400
         TabIndex        =   4
         Top             =   600
         Width           =   735
         _ExtentX        =   1296
         _ExtentY        =   661
         HideAdd         =   -1  'True
         BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
      End
      Begin MSChart20Lib.MSChart MSChart1 
         Height          =   9135
         Left            =   120
         OleObjectBlob   =   "frmBinReport.frx":047A
         TabIndex        =   21
         Top             =   1080
         Width           =   16215
      End
      Begin VB.Label Label1 
         AutoSize        =   -1  'True
         Caption         =   "Sensor"
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
         Left            =   7440
         TabIndex        =   5
         Top             =   660
         Width           =   750
      End
   End
   Begin MSComCtl2.DTPicker DTP 
      Height          =   285
      Index           =   0
      Left            =   9360
      TabIndex        =   7
      Top             =   240
      Width           =   1200
      _ExtentX        =   2117
      _ExtentY        =   503
      _Version        =   393216
      CustomFormat    =   "dd-MMM-yy"
      Format          =   16646147
      CurrentDate     =   42393
   End
   Begin MSComCtl2.DTPicker DTP 
      Height          =   285
      Index           =   1
      Left            =   9360
      TabIndex        =   13
      Top             =   765
      Width           =   1200
      _ExtentX        =   2117
      _ExtentY        =   503
      _Version        =   393216
      CustomFormat    =   "dd-MMM-yy"
      Format          =   16646147
      CurrentDate     =   42393
   End
   Begin dcCombo.dcComboControl Combo 
      Height          =   375
      Index           =   2
      Left            =   1080
      TabIndex        =   15
      Top             =   660
      Width           =   735
      _ExtentX        =   1296
      _ExtentY        =   661
      HideAdd         =   -1  'True
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
   End
   Begin VB.Label Label1 
      AutoSize        =   -1  'True
      Caption         =   "Cable"
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
      Index           =   2
      Left            =   240
      TabIndex        =   16
      Top             =   720
      Width           =   630
   End
   Begin VB.Label Label3 
      AutoSize        =   -1  'True
      Caption         =   "Date:"
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
      Left            =   7440
      TabIndex        =   14
      Top             =   240
      Width           =   570
   End
   Begin VB.Label Label1 
      AutoSize        =   -1  'True
      Caption         =   "Bin"
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
      Index           =   0
      Left            =   240
      TabIndex        =   0
      Top             =   240
      Width           =   345
   End
End
Attribute VB_Name = "frmBinReport"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Declare Function LockWindowUpdate Lib "USER32" (ByVal hwndLock As Long) As Long
Dim Starting As Boolean
Dim ByHour As Boolean

Private Sub butMonth_Click()
    On Error GoTo ErrHandler
    DTP(0).Value = DateAdd("m", -1, Now)
    DTP(1).Value = Now
    Opt(1).Value = True
    DTP(1).Enabled = True
    butUpdate_Click
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "butMonth_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butToday_Click()
    On Error GoTo ErrHandler
    DTP(0).Value = Now
    DTP(1).Value = Now
    Opt(0).Value = True
    DTP(1).Enabled = False
    butUpdate_Click
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "butToday_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butUpdate_Click()
    On Error GoTo ErrHandler
    If Not Starting Then
        LoadGraph
        LoadGrid
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "butUpdate_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub butWeek_Click()
    On Error GoTo ErrHandler
    DTP(0).Value = DateAdd("ww", -1, Now)
    DTP(1).Value = Now
    Opt(1).Value = True
    DTP(1).Enabled = True
    butUpdate_Click
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "butWeek_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub Combo_Change(Index As Integer)
    butUpdate_Click
End Sub

Private Sub ComboLoad()
    On Error GoTo ErrHandler
    Dim X As Long
    Dim Obj As StorageDisplay
    Dim Col As Storages
    'bins
    Combo(0).Clear
    Set Col = New Storages
    Col.Load , , , GMSTBins
    For Each Obj In Col
        Combo(0).AddItem Obj.BinNumber & "  " & Obj.Description, Obj.ID
    Next
    Set Obj = Nothing
    Set Col = Nothing
    'Sensors(1) and Cables(2)
    Combo(1).Clear
    Combo(2).Clear
    For X = 1 To 16
        Combo(1).AddItem Str(X), X
        Combo(2).AddItem Str(X), X
    Next X
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmBinReport", "ComboLoad", Err.Description
    Resume ErrExit
End Sub

Private Function CurrentSensorID(BinID As Long, CableNumber As Long, SensorNumber As Long) As Long
    Dim Obj As clsSensor
    On Error GoTo ErrHandler
    Set Obj = New clsSensor
    Obj.Load , , BinID, CableNumber, SensorNumber
    If Obj.IsNew Then
        CurrentSensorID = 0
    Else
        CurrentSensorID = Obj.ID
    End If
    Set Obj = Nothing
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "CurrentSensorID", Err.Description
     Resume ErrExit
End Function

Private Function DateDisplay(ReadDate As String) As String
    'change "2016/09/01  14" to "01-Sep-16  2 PM"
    Dim L As Long
    Dim DV As String
    Dim HV As String
    Dim H As Long
    On Error GoTo ErrHandler
    DV = ""
    HV = ""
    L = Len(ReadDate)
    If L > 10 Then L = 10
    DV = Left$(ReadDate, L)
    DV = " " & Format(DV, "dd-mmm-yy")
    L = Len(ReadDate)
    If L > 10 Then
        H = Val(Right$(ReadDate, L - 10))
        If H = 0 Then
            HV = "  12 AM"
        ElseIf H = 12 Then
            HV = "  12 PM"
        ElseIf H < 12 Then
            HV = "  " & Format(H) & " AM"
        Else
            HV = "  " & Format(H - 12) & " PM"
        End If
        'align single digits
        If Len(HV) < 7 Then HV = "  " & HV
    End If
    DateDisplay = DV & HV
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "DateDisplay", Err.Description
     Resume ErrExit
End Function

Private Sub DeleteTable(TBname)
    On Error Resume Next
    MainDB.Execute "DROP TABLE " & TBname
End Sub

Private Sub Form_Load()
    On Error GoTo ErrHandler
    AD.LoadFormData Me
    Starting = True
    ByHour = False
    ComboLoad
    butMonth_Click
    SetupGrid
    If Not GetPreviousSensor Then
        Combo(0).MoveToRecord , 0
        Combo(1).RecordID = 0
        Combo(2).RecordID = 0
    End If
    Starting = False
    With MSChart1.Backdrop.Fill
        .Style = VtFillStyleBrush
        .Brush.Style = VtBrushStyleSolid
        .Brush.FillColor.Set 240, 240, 255
    End With
'    SSTab1.BackColor = 16773360
    butUpdate_Click
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "Form_Load", Err.Description
     Resume ErrExit
End Sub

Private Sub Form_Unload(Cancel As Integer)
    On Error Resume Next
    AD.Element("ReportSensorID") = CurrentSensorID(Combo(0).RecordID, Combo(2).RecordID, Combo(1).RecordID)
    AD.SaveFormData Me
End Sub

Private Function GetPreviousSensor() As Boolean
    Dim VL As String
    Dim Obj As clsSensor
    On Error GoTo ErrHandler
    GetPreviousSensor = False
    VL = AD.Element("ReportSensorID")
    If VL <> "" Then
        'load sensor
        Set Obj = New clsSensor
        Obj.Load Val(VL)
        If Not Obj.IsNew Then
            Combo(0).MoveToRecord Obj.BinID
            Combo(2).MoveToRecord Obj.CableNumber
            Combo(1).MoveToRecord Obj.SensorNumber
            GetPreviousSensor = True
        End If
        Set Obj = Nothing
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "GetPreviousSensor", Err.Description
     Resume ErrExit
End Function

Private Sub LoadGraph()
    Dim RS As Recordset
    Dim SQL As String
    Dim StDate As Date
    Dim EndDate As Date
    Dim DateFmt As String
    Dim Col As Long
    Dim Row As Long
    Dim V As Variant
    Dim Tmp As String
    Dim GD() As Variant
    On Error GoTo ErrHandler
    
    'make tmp table
    SQL = "SELECT recDate, senSensorNumber, recTemp INTO tmpRecs"
    SQL = SQL & " FROM (tblSensors LEFT JOIN tblRecords ON tblSensors.senID = tblRecords.recSenID) RIGHT JOIN tblStorage ON tblSensors.senStorID = tblStorage.StorId"
    SQL = SQL & " GROUP BY recDate,senSensorNumber,recTemp,StorID,senCableNumber"
    SQL = SQL & " Having StorID = " & Combo(0).RecordID
    SQL = SQL & " And senCableNumber = " & Combo(2).RecordID
    SQL = SQL & " And senSensorNumber = " & Combo(1).RecordID
    'date
    StDate = DTP(0).Value
    EndDate = DTP(1).Value
    If StDate = EndDate Or Opt(0) Then
        'single date
        'end date is beginning of next day
        EndDate = DateAdd("d", 1, StDate)
    Else
        'date range
        'end date is beginning of next day after user selected end date
        EndDate = DateAdd("d", 1, EndDate)
    End If
    SQL = SQL & " And recDate >= " & ToAccessDate(StDate) & " And recDate < " & ToAccessDate(EndDate)
    SQL = SQL & " Order By StorID,recDate,senCableNumber,senSensorNumber"
    DeleteTable "tmpRecs"   'delete old table, if any
    MainDB.Execute SQL
    
    'make crosstab recordset
    If ByHour Then
        DateFmt = Chr$(34) & "yyyy/mm/dd  hh" & Chr$(34)
    Else
        'by day
        DateFmt = Chr$(34) & "yyyy/mm/dd" & Chr$(34)
    End If
    SQL = "TRANSFORM Avg(tmpRecs.recTemp) AS AvgOfrecTemp"
    SQL = SQL & " SELECT Format([recdate]," & DateFmt & ") AS ReadDate"
    SQL = SQL & " FROM tmpRecs"
    SQL = SQL & " GROUP BY Format([recdate]," & DateFmt & ")"
    SQL = SQL & " PIVOT tmpRecs.senSensorNumber In (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)"
    Set RS = MainDB.OpenRecordset(SQL)
    
    'fill graph
    If RS.EOF Then
        MSChart1.Visible = False
    Else
        MSChart1.Visible = True
        RS.MoveLast
        ReDim GD(RS.RecordCount, 1)
        GD(0, 1) = "sensor"
        Col = Combo(1).RecordID
        RS.MoveFirst
        Do Until RS.EOF
            V = RS(Col)
            If Not IsNull(V) Then
                Tmp = CCur(V)
                Row = Row + 1
                GD(Row, 0) = DateDisplay(NZ(RS!ReadDate))
                GD(Row, 1) = Tmp
            End If
            RS.MoveNext
        Loop
        MSChart1.ChartData = GD
    End If
    Set RS = Nothing
    On Error GoTo 0
ErrExit:
    LockWindowUpdate (0)
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "LoadGrid", Err.Description
     Resume ErrExit
End Sub

Private Sub LoadGrid()
    Dim RS As Recordset
    Dim SQL As String
    Dim StDate As Date
    Dim EndDate As Date
    Dim DateFmt As String
    Dim Col As Long
    Dim Row As Long
    Dim V As Variant
    Dim Tmp As String
    On Error GoTo ErrHandler
    
    'make tmp table
    SQL = "SELECT recDate, senSensorNumber, recTemp INTO tmpRecs"
    SQL = SQL & " FROM (tblSensors LEFT JOIN tblRecords ON tblSensors.senID = tblRecords.recSenID) RIGHT JOIN tblStorage ON tblSensors.senStorID = tblStorage.StorId"
    SQL = SQL & " GROUP BY recDate,senSensorNumber,recTemp,StorID,senCableNumber"
    SQL = SQL & " Having StorID = " & Combo(0).RecordID
    SQL = SQL & " And senCableNumber = " & Combo(2).RecordID
    'date
    StDate = DTP(0).Value
    EndDate = DTP(1).Value
    If StDate = EndDate Or Opt(0) Then
        'single date
        'end date is beginning of next day
        EndDate = DateAdd("d", 1, StDate)
    Else
        'date range
        'end date is beginning of next day after user selected end date
        EndDate = DateAdd("d", 1, EndDate)
    End If
    SQL = SQL & " And recDate >= " & ToAccessDate(StDate) & " And recDate < " & ToAccessDate(EndDate)
    SQL = SQL & " Order By StorID,recDate,senCableNumber,senSensorNumber"
    DeleteTable "tmpRecs"   'delete old table, if any
    MainDB.Execute SQL
    
    'make crosstab recordset
    If ByHour Then
        DateFmt = Chr$(34) & "yyyy/mm/dd  hh" & Chr$(34)
    Else
        'by day
        DateFmt = Chr$(34) & "yyyy/mm/dd" & Chr$(34)
    End If
    SQL = "TRANSFORM Avg(tmpRecs.recTemp) AS AvgOfrecTemp"
    SQL = SQL & " SELECT Format([recdate]," & DateFmt & ") AS ReadDate"
    SQL = SQL & " FROM tmpRecs"
    SQL = SQL & " GROUP BY Format([recdate]," & DateFmt & ")"
    SQL = SQL & " PIVOT tmpRecs.senSensorNumber In (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)"
    Set RS = MainDB.OpenRecordset(SQL)
    
    'fill grid
    Grid.Clear
    SetupGrid
    LockWindowUpdate (Grid.hWnd)
    Do Until RS.EOF
        Row = Row + 1
        'check for enough rows
        If (Grid.Rows - 1) < Row Then Grid.Rows = Row + 1
        Grid.TextMatrix(Row, 0) = DateDisplay(NZ(RS!ReadDate))
        For Col = 1 To 16
            V = RS(Col)
            If IsNull(V) Then
                Tmp = ""
            Else
                Tmp = Format(CCur(V), "0")
            End If
            Grid.TextMatrix(Row, Col) = Tmp
        Next Col
        RS.MoveNext
    Loop
    Set RS = Nothing
    On Error GoTo 0
ErrExit:
    LockWindowUpdate (0)
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "LoadGrid", Err.Description
     Resume ErrExit
End Sub

Private Sub Opt_Click(Index As Integer)
    If Index = 0 Then
        'single
        DTP(1).Enabled = False
    Else
        'range
        DTP(1).Enabled = True
    End If
End Sub

Private Sub OptScale_Click(Index As Integer)
    ByHour = OptScale(0).Value
    butUpdate_Click
End Sub

Private Sub SetupGrid()
    Dim C As Long
    On Error GoTo ErrHandler
    With Grid
        .Cols = 17
        .FixedCols = 0
        .TextMatrix(0, 0) = "Date"
        .ColAlignment(0) = flexAlignCenterCenter
        .ColWidth(0) = 1600
        For C = 1 To 16
            .TextMatrix(0, C) = "Sensor " & Str(C)
            .ColAlignment(C) = flexAlignCenterCenter
            .ColWidth(C) = 900
        Next C
        .Rows = 40
    End With
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmBinReport", "SetupGrid", Err.Description
     Resume ErrExit
End Sub

