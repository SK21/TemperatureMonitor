VERSION 5.00
Object = "{248DD890-BB45-11CF-9ABC-0080C7E7B78D}#1.0#0"; "MSWINSCK.OCX"
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "ComDlg32.ocx"
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "mscomctl.ocx"
Object = "{3CC26050-93EE-4497-B39F-C5F1BA7EAA84}#2.1#0"; "dcBinMaps.ocx"
Begin VB.Form frmStart 
   BackColor       =   &H8000000C&
   Caption         =   "TemperatureMonitor"
   ClientHeight    =   7575
   ClientLeft      =   165
   ClientTop       =   555
   ClientWidth     =   11655
   HelpContextID   =   10
   Icon            =   "frmStart.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   7575
   ScaleWidth      =   11655
   WindowState     =   2  'Maximized
   Begin VB.Timer TimerResize 
      Enabled         =   0   'False
      Interval        =   200
      Left            =   7560
      Top             =   2880
   End
   Begin VB.TextBox tbEvents 
      Height          =   3855
      Left            =   8760
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      ScrollBars      =   2  'Vertical
      TabIndex        =   3
      Top             =   1680
      Width           =   2535
   End
   Begin MSComctlLib.Toolbar Toolbar1 
      Align           =   1  'Align Top
      Height          =   660
      Left            =   0
      TabIndex        =   2
      Top             =   0
      Width           =   11655
      _ExtentX        =   20558
      _ExtentY        =   1164
      ButtonWidth     =   1032
      ButtonHeight    =   1005
      Appearance      =   1
      ImageList       =   "ImageList2"
      _Version        =   393216
      BeginProperty Buttons {66833FE8-8583-11D1-B16A-00C0F0283628} 
         NumButtons      =   10
         BeginProperty Button1 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Style           =   3
         EndProperty
         BeginProperty Button2 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Enable Network"
            ImageIndex      =   1
         EndProperty
         BeginProperty Button3 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Record data"
            ImageIndex      =   2
         EndProperty
         BeginProperty Button4 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Enable Notifications"
            ImageIndex      =   3
         EndProperty
         BeginProperty Button5 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Enable Maps"
            ImageIndex      =   4
         EndProperty
         BeginProperty Button6 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "File Information"
            ImageIndex      =   5
         EndProperty
         BeginProperty Button7 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Sensor Database"
            ImageIndex      =   8
         EndProperty
         BeginProperty Button8 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Refresh Readings"
            ImageIndex      =   6
         EndProperty
         BeginProperty Button9 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Bin Report"
            ImageIndex      =   7
         EndProperty
         BeginProperty Button10 {66833FEA-8583-11D1-B16A-00C0F0283628} 
            Object.ToolTipText     =   "Options"
            ImageIndex      =   13
         EndProperty
      EndProperty
   End
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   1000
      Left            =   6840
      Top             =   2280
   End
   Begin VB.PictureBox Picture1 
      Height          =   1095
      Left            =   5400
      ScaleHeight     =   1035
      ScaleWidth      =   1515
      TabIndex        =   1
      Top             =   3120
      Visible         =   0   'False
      Width           =   1575
   End
   Begin MSComDlg.CommonDialog ComDial 
      Left            =   5160
      Top             =   840
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin dcBinMaps.BinMaps BinMaps1 
      Height          =   3855
      Left            =   0
      TabIndex        =   0
      Top             =   675
      Width           =   4575
      _ExtentX        =   8070
      _ExtentY        =   6800
      MapType         =   1
      WidthPercent    =   60
   End
   Begin MSWinsockLib.Winsock sckServer 
      Index           =   0
      Left            =   7920
      Top             =   1920
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin MSComctlLib.ImageList ImageList2 
      Left            =   3480
      Top             =   5280
      _ExtentX        =   1005
      _ExtentY        =   1005
      BackColor       =   -2147483643
      ImageWidth      =   32
      ImageHeight     =   32
      MaskColor       =   12632256
      _Version        =   393216
      BeginProperty Images {2C247F25-8591-11D1-B16A-00C0F0283628} 
         NumListImages   =   13
         BeginProperty ListImage1 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":0442
            Key             =   ""
         EndProperty
         BeginProperty ListImage2 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":0894
            Key             =   ""
         EndProperty
         BeginProperty ListImage3 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":0CE6
            Key             =   ""
         EndProperty
         BeginProperty ListImage4 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":1138
            Key             =   ""
         EndProperty
         BeginProperty ListImage5 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":158A
            Key             =   ""
         EndProperty
         BeginProperty ListImage6 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":19DC
            Key             =   ""
         EndProperty
         BeginProperty ListImage7 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":1B36
            Key             =   ""
         EndProperty
         BeginProperty ListImage8 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":1F88
            Key             =   ""
         EndProperty
         BeginProperty ListImage9 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":23DA
            Key             =   ""
         EndProperty
         BeginProperty ListImage10 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":12C46
            Key             =   ""
         EndProperty
         BeginProperty ListImage11 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":2F2A2
            Key             =   ""
         EndProperty
         BeginProperty ListImage12 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":3B309
            Key             =   ""
         EndProperty
         BeginProperty ListImage13 {2C247F27-8591-11D1-B16A-00C0F0283628} 
            Picture         =   "frmStart.frx":3B463
            Key             =   ""
         EndProperty
      EndProperty
   End
   Begin VB.Menu mnuFile 
      Caption         =   "File"
      Begin VB.Menu mnuNew 
         Caption         =   "New"
      End
      Begin VB.Menu mnuFileOpen 
         Caption         =   "Open"
      End
      Begin VB.Menu MenuSaveAs 
         Caption         =   "Save As"
      End
      Begin VB.Menu mnuBackup 
         Caption         =   "Backup"
      End
      Begin VB.Menu mnuRestore 
         Caption         =   "Restore"
      End
      Begin VB.Menu mnuSeperator 
         Caption         =   "-"
      End
      Begin VB.Menu mnuPassword 
         Caption         =   "Change Password"
      End
      Begin VB.Menu MnuCompact 
         Caption         =   "Compact Database"
      End
      Begin VB.Menu mnuServer 
         Caption         =   "Enable Server"
         Checked         =   -1  'True
      End
      Begin VB.Menu mnuRecord 
         Caption         =   "Enable Recording"
         Checked         =   -1  'True
      End
      Begin VB.Menu MnuNotify 
         Caption         =   "Enable Notification"
         Checked         =   -1  'True
      End
      Begin VB.Menu mnuUseMaps 
         Caption         =   "Use Bin Maps"
         Checked         =   -1  'True
      End
      Begin VB.Menu mnuInfo 
         Caption         =   "File Information"
      End
      Begin VB.Menu mnuFileExit 
         Caption         =   "Exit"
      End
   End
   Begin VB.Menu mnuSensors 
      Caption         =   "Sensors"
   End
   Begin VB.Menu mnuEditBins 
      Caption         =   "Bins"
   End
   Begin VB.Menu mnuMaps 
      Caption         =   "Maps"
   End
   Begin VB.Menu mnuBins 
      Caption         =   "Bin Report"
   End
   Begin VB.Menu mnuOptions 
      Caption         =   "Options"
   End
   Begin VB.Menu mnuHelp 
      Caption         =   "Help"
      Begin VB.Menu mnuHelpHelp 
         Caption         =   "TemperatureMonitor Help"
      End
      Begin VB.Menu mnuLog 
         Caption         =   "Log"
      End
      Begin VB.Menu mnuHelpAbout 
         Caption         =   "About"
      End
   End
End
Attribute VB_Name = "frmStart"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Const MaxSockets As Integer = 5
Const PortNum As Long = 1600
Const HBtime As Long = 9   'seconds between heartbeats signals, ControlBox disconnect is 60 seconds

Dim LastReading As Date
Dim LastHeartBeat As Date
Dim SocketCount As Long
Dim PrevAlarmOn As Boolean
Public RefreshReadings As Boolean
Dim HalfTime As Date
Dim HalfTimeRan As Boolean
Dim DailyJob As Date
Public ControlBoxDelay As Long

Public LoadOK As Boolean
Public SendTofrmSensorsSht As Boolean

Public Sub AddPackets(PKtype As PacketType, Optional NewVal As String = "", Optional SensorID As Long = 0)
    'add packets to the send collection
    Dim Sck As Winsock
    Dim Obj As clsSensor
    Dim SocketID As Integer
    Dim RomCode As String
    Dim StartTime As Date
    Dim Count As Long
    SocketID = 0
    RomCode = ""
    Count = 0
    On Error GoTo ErrHandler
    If SensorID = 0 Then
        'send to all sockets
        StartTime = Now
        For Each Sck In sckServer
            If Sck.Index <> 0 Then
                'delay packet to each socket by ControlBoxDelay time
                OP.Add Sck.Index, PKtype, NewVal, "", DateAdd("s", Count * ControlBoxDelay, StartTime)
                Count = Count + 1
            End If
        Next
    Else
        'send to one socket
        Set Obj = New clsSensor
        Obj.Load SensorID
        SocketID = Obj.Socket
        RomCode = Obj.RomCode
        Set Obj = Nothing
        OP.Add SocketID, PKtype, NewVal, RomCode, Now
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "AddPackets", Err.Description
     Resume ErrExit
End Sub

Private Sub BinMaps1_ToopTip(TipText As String)
'    ShowStatus TipText
End Sub

Private Sub CheckAlarms()
    Dim AlarmOn As Boolean
    Dim Col As New clsSensors
    On Error GoTo ErrHandler
    If DBconnected Then
        Col.Load ssBinNumberAsc
        AlarmOn = Col.CheckAlarms(Prog.MaxTemp)
        Set Col = Nothing
    End If
    If MnuNotify.Checked Then UpdateStatusFile AlarmOn
    If mnuUseMaps.Checked Then UpdateMaps
    'reset time
    HalfTimeRan = True
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "CheckAlarms", Err.Description
     Resume ErrExit
End Sub

Private Sub CheckDeadSockets()
'---------------------------------------------------------------------------------------
' Procedure : CheckDeadSockets
' Author    : David
' Date      : 1/1/2012
' Purpose   : check if any sockets are not connected
'---------------------------------------------------------------------------------------
'
    Dim Sck As Winsock
    On Error GoTo ErrHandler
    For Each Sck In sckServer
        If Sck.State = sckClosed Or Sck.State = sckError Or Sck.State = sckClosing Then
            sckServer_Close (Sck.Index)
        End If
    Next
    CountSockets
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "CheckDeadSockets", Err.Description
    Resume ErrExit
End Sub

Private Function CheckSensor(RomCode As String, Sckt As Integer, ID As Long) As String
'---------------------------------------------------------------------------------------
' Procedure : CheckSensor
' Author    : David
' Date      : 3/24/2016
' Purpose   : adds new sensors and updates socket on existing sensors. Returns name.
'---------------------------------------------------------------------------------------
'
    Dim Obj As clsSensor
    On Error GoTo ErrHandler
    Set Obj = New clsSensor
    Obj.Load 0, RomCode
    With Obj
        .BeginEdit
        .RomCode = RomCode
        .Socket = Sckt
        .ApplyEdit
        CheckSensor = .Name
        ID = .ID
    End With
    Set Obj = Nothing
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "CheckSensor", Err.Description
     Resume ErrExit
End Function

Private Sub CheckServerStatus()
    On Error GoTo ErrHandler
    If mnuServer.Checked And sckServer(0).State = sckClosed Then
        'initialize the port on which to listen
        sckServer(0).LocalPort = PortNum
        'Start listening for a ControlBox connection request
        sckServer(0).Listen
        LogWrite "Server Listening ..."
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    Select Case Err.Number
        Case 10048
            'port is in use
        Case Else
            AD.DisplayError Err.Number, "frmStart", "CheckServerStatus", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub CloseSockets()
    Dim S As Winsock
    On Error GoTo ErrHandler
    For Each S In sckServer
        If S.State <> sckClosed Then S.Close
    Next S
    Set S = Nothing
    CountSockets
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "CloseSockets", Err.Description
     Resume ErrExit
End Sub

Private Sub CountSockets()
    Dim Sck As Winsock
    Dim TmpCount As Long
    On Error GoTo ErrHandler
    For Each Sck In sckServer
        If Sck.Index <> 0 And Sck.State <> sckClosed And Sck.State <> sckError And Sck.State <> sckClosing Then
            TmpCount = TmpCount + 1
        End If
    Next
    If SocketCount <> TmpCount Then
        SocketCount = TmpCount
        ShowBoxes
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "CountSockets", Err.Description
     Resume ErrExit
End Sub

Private Sub DoAutoReport()
    Dim Hrs As Long
    Dim Interval As Long
    On Error GoTo ErrHandler
    Interval = Prog.Days * 24 + Prog.Hours
    Hrs = DateDiff("d", Prog.StartSave, Now) * 24
    Hrs = Hrs + DatePart("h", Now) - DatePart("h", Prog.StartSave)
    If Hrs > Interval Then
        With Prog
            .BeginEdit
            .StartSave = Now
            .ApplyEdit
        End With
        Load frmSensorsSht
        frmSensorsSht.DoPrint
        Unload frmSensorsSht
        FileCopy AD.Folders(AppCommon) & "\Sensors.pdf", Prog.Location & "\Sensors.pdf"
        AD.SaveToLog "Sensor Report Saved.", , , , adTimeStamp
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.SaveToLog Err.Description, "frmStart", "DoReport", Err.Number, adFull
    Resume ErrExit
End Sub

Private Sub EnableButtons()
'---------------------------------------------------------------------------------------
' Procedure : EnableButtons
' Author    : XPMUser
' Date      : 12/17/2016
' Purpose   : enable buttons depending on database connection
'---------------------------------------------------------------------------------------
'
    Dim b As Long
    On Error GoTo ErrHandler
    With Toolbar1
        For b = 2 To 9
            .Buttons(b).Enabled = DBconnected
        Next b
    End With
    mnuSensors.Enabled = DBconnected
    mnuEditBins.Enabled = DBconnected
    mnuMaps.Enabled = DBconnected
    mnuBins.Enabled = DBconnected
    mnuOptions.Enabled = DBconnected
    mnuBackup.Enabled = DBconnected
    MnuCompact.Enabled = DBconnected
    mnuInfo.Enabled = DBconnected
    MenuSaveAs.Enabled = DBconnected
    mnuServer.Enabled = DBconnected
    mnuRecord.Enabled = DBconnected
    MnuNotify.Enabled = DBconnected
    mnuUseMaps.Enabled = DBconnected
    mnuPassword.Enabled = DBconnected
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "EnableButtons", Err.Description
     Resume ErrExit
End Sub

Private Sub Form_Load()
    Dim VL As String
    AD.LoadFormData Me
    ConnectDatabase
    'get previous DailyJob date
    VL = AD.Element("DailyJob")
    If VL <> "" Then DailyJob = VL
    'get previous maps preference
    VL = AD.Element("ShowMaps")
    If VL = "" Then
        mnuUseMaps.Checked = False
    Else
        mnuUseMaps.Checked = CBool(VL)
    End If
    CheckDeadSockets
    mnuServer.Checked = True
    UpdateButtons
    CheckServerStatus
    UpdateMaps
    HalfTime = Now
    CheckAlarms
    ControlBoxDelay = Prog.ControlBoxDelay
    Timer1.Enabled = DBconnected
    EnableButtons
    ShowBoxes
    LoadOK = True
End Sub

Private Sub Form_Resize()
    On Error GoTo ErrHandler
    ResizeControls
    'verify controls have been resized after 1 second
    TimerResize.Enabled = True
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "Form_Resize", Err.Description
     Resume ErrExit
End Sub

Private Sub Form_Unload(Cancel As Integer)
    Dim FF As Form
    Dim C As Long
    For Each FF In Forms
        C = C + 1
        If C > 100 Then Exit For
        Unload FF
    Next FF
    Timer1.Enabled = False
    AD.SaveFormData Me
    AD.Element("ShowMaps") = mnuUseMaps.Checked
    AD.Element("DailyJob") = DailyJob
    SaveMaps BinMaps1
End Sub

Private Function IsLoaded(ByVal Control As Control) As Boolean
    Dim S As String
    On Error Resume Next
    S = Control.Name 'Try to fetch control's name.
    IsLoaded = (Err.Number = 0)
End Function

Private Sub LogWrite(Optional ByVal Mes As String = "", Optional SaveToLog As Boolean = True)
    Dim L As Long
    On Error GoTo ErrHandler
    L = Len(tbEvents)
    If L > 20000 Then
        tbEvents.Text = Right$(tbEvents.Text, 15000)
    End If
    If Mes <> "" Then
        If SaveToLog Then
            AD.SaveToLog Mes, , , , adTimeStamp
        End If
        Mes = Format(Now, "hh:mm:ss  AM/PM") & "    " & Mes
    End If
    tbEvents.Text = tbEvents.Text & Mes & vbNewLine
    tbEvents.SelStart = Len(tbEvents.Text)
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "LogWrite", Err.Description
     Resume ErrExit
End Sub

Private Sub MenuSaveAs_Click()
    ShowForm ("frmSaveAs")
End Sub

Private Sub mnuBackup_Click()
'---------------------------------------------------------------------------------------
' Procedure : mnuBackup_Click
' Author    : David
' Date      : 3/6/2012
' Purpose   :
'---------------------------------------------------------------------------------------
'
    Dim BK As String
    On Error GoTo ErrHandler
    BK = AD.Folders(AppBackup)
    If CopyCurrentFile(BK) Then
        LogWrite "Data backed-up."
    Else
        LogWrite "Data could not be backed-up."
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "mnuBackup_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub mnuBins_Click()
    frmBinReport.Show vbModal
End Sub

Private Sub MnuCompact_Click()
    Dim CompactWorked As Boolean
    'compact & repair database
    On Error GoTo ErrHandler
    CompactWorked = Prog.CompactDatabase
ErrExit:
    If CompactWorked Then
        LogWrite "Database Compacted."
    Else
        Beep
        LogWrite "Failed to Compact database."
        DBconnected = False
    End If
    Exit Sub
ErrHandler:
    Select Case Err.Number
        Case 3356
            'locked, do nothing
        Case Else
            AD.SaveToLog Err.Description, "frmStart", "MnuCompact_Click", Err.Number, adTimeStamp
    End Select
    Resume ErrExit
End Sub

Private Sub mnuEditBins_Click()
    frmStorageSht.Show vbModal
End Sub

Private Sub mnuFileExit_Click()
    Unload Me
End Sub

Private Sub mnuFileOpen_Click()
'---------------------------------------------------------------------------------------
' Procedure : mnuFileOpen_Click
' Author    : David
' Date      : 12/5/2010
' Purpose   :
'---------------------------------------------------------------------------------------
    Dim DBname As String
    On Error GoTo ErrHandler
    CloseSockets
    With frmStart.ComDial
        .FileName = ""  'prevents using previous directory
'                    .CancelError = True 'raises error cdlCancel if user presses the cancel button
        .InitDir = AD.Folders(AppDatabase)
        .Filter = "Database files|*.mdb"
        .Flags = cdlOFNHideReadOnly + cdlOFNFileMustExist
        .DialogTitle = "Open"
        .ShowOpen
        DBname = .FileName
    End With
    If ConnectDatabase(DBname) Then
        UpdateMaps
        CheckServerStatus
    Else
        LogWrite "File not opened."
        ConnectDatabase 'to original file
    End If
    Timer1.Enabled = DBconnected
    EnableButtons
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "mnuFileOpen_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub mnuHelpAbout_Click()
    On Error GoTo ErrHandler
    frmAbout.Show vbModal
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "mnuHelpAbout_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub mnuHelpHelp_Click()
    SendKeys "{F1}"
End Sub

Private Sub mnuInfo_Click()
    frmInfo.Show vbModal
End Sub

Private Sub mnuLog_Click()
    On Error GoTo ErrExit
    frmLog.Show vbModal
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "mnuLog_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub mnuMaps_Click()
    On Error GoTo ErrHandler
    frmBinMapsSht.Show vbModal
    UpdateMaps
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "mnuMaps_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub mnuNew_Click()
    ShowForm "frmNewDatabase"
    UpdateMaps
End Sub

Private Sub MnuNotify_Click()
    MnuNotify.Checked = Not MnuNotify.Checked
    UpdateButtons
End Sub

Private Sub mnuOptions_Click()
    On Error GoTo ErrHandler
    frmOptions.Show vbModal
    CheckAlarms
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "mnuOptions_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub mnuPassword_Click()
    frmChangePassword.Show vbModal
End Sub

Private Sub mnuRecord_Click()
    mnuRecord.Checked = Not mnuRecord.Checked
    UpdateButtons
End Sub

Private Sub mnuRestore_Click()
'---------------------------------------------------------------------------------------
' Procedure : mnuRestore_Click
' Author    : David
' Date      : 3/5/2012
' Purpose   :
'---------------------------------------------------------------------------------------
'
    Dim FSO As FileSystemObject
    Dim FL As File
    Dim DT As String
    Dim Tm As String
    Dim P As Long
    Dim Src As String
    Dim Des As String
    Dim DocSrc As String
    Dim IsCurrent As Boolean
    On Error GoTo ErrHandler
    With frmStart.ComDial
        .FileName = ""  'prevents using previous directory
        .CancelError = True 'raises error cdlCancel if user presses the cancel button
        .InitDir = AD.Folders(AppBackup)
        .Filter = "Database files|*.mdb"
        .Flags = cdlOFNHideReadOnly + cdlOFNFileMustExist
        .DialogTitle = "File Restore"
        .ShowOpen
        Src = .FileName
    End With
    If Src <> "" Then
        'remove the .mdb
        DocSrc = Left$(Src, Len(Src) - 4)
        Des = AD.Folders(AppDatabase) & "\" & DatabaseName(Src)
        'check if restoring current database
        IsCurrent = (LCase(DatabaseName(Src)) = LCase(Prog.DatabaseName))
        Set FSO = New FileSystemObject
        Set FL = FSO.GetFile(Src)
        DT = FL.DateLastModified
        P = InStr(DT, " ")
        Tm = Right$(DT, Len(DT) - P)
        DT = Left$(DT, P - 1)
        DT = Format(DT, "medium date")
        Select Case MsgBox("Do you want to restore this database that was backed-up on " & DT & " " & Tm & " ?", vbYesNo Or vbQuestion Or vbSystemModal Or vbDefaultButton1, App.Title)
            Case vbYes
                If IsCurrent Then
                    'close database
                    Set Prog = Nothing
                    Set Prog = New clsMain
                End If
                'copy database
                FSO.CopyFile Src, Des & ".mdb"
                'copy document folder
                If FSO.FolderExists(DocSrc) Then
                    FSO.CopyFolder DocSrc, Des
                End If
                If IsCurrent Then
                    'reopen database
                    ConnectDatabase Des & ".mdb"
                End If
                LogWrite "Database restored."
            Case vbNo
                LogWrite "Database not restored."
        End Select
    End If
    On Error GoTo 0
ErrExit:
    Set FSO = Nothing
    Set FL = Nothing
    Exit Sub
ErrHandler:
    Select Case Err.Number
        Case cdlCancel
'            CancelOpen = True
        Case Else
            AD.DisplayError Err.Number, "frmStart", "mnuRestore_Click", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub mnuSensors_Click()
    On Error GoTo ErrHandler
    frmSensorsSht.Show vbModal
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "mnuSensors_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub mnuServer_Click()
    On Error GoTo ErrHandler
    mnuServer.Checked = Not mnuServer.Checked
    UpdateButtons
    If Not mnuServer.Checked Then CloseSockets
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "mnuServer_Click", Err.Description
     Resume ErrExit
End Sub

Private Sub mnuUseMaps_Click()
    mnuUseMaps.Checked = Not mnuUseMaps.Checked
    UpdateButtons
    UpdateMaps
End Sub

Private Sub ProcessData(ND As String, Sckt As Integer)
    Dim IP As clsPackets 'incoming packets
    Dim Pckt As clsPacket
    Dim SenName As String
    Dim SensorID As Long
    Dim Mes As String
    On Error GoTo ErrHandler
    Set IP = New clsPackets
    IP.StreamAdd ND, Sckt
    For Each Pckt In IP
        With Pckt
            SenName = CheckSensor(.RomCode, .SocketID, SensorID)
            Select Case .PKtype
                Case PacketType.pdGetTemperatures
                    Mes = SenName & "  Temperature = " & .Value
                    LogWrite Mes
                    If mnuRecord.Checked Then SaveTemp SensorID, .Value
                    'send temp to frmSensorsSht if it is being checked
                    If SendTofrmSensorsSht And SensorID = frmSensorsSht.SensorID Then
                        frmSensorsSht.SensorTemp = CLng(.Value)
                    End If
                Case PacketType.pdGetUserData
                    If SendTofrmSensorsSht Then
                        frmSensorsSht.SensorData = CLng(.Value)
                    Else
                        Mes = SenName & "  SensorData = " & .Value
                    End If
            End Select
        End With
    Next
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "ProcessData", Err.Description
     Resume ErrExit
End Sub

Private Sub ResizeControls()
    On Error GoTo ErrHandler
    BinMaps1.Resize
    With tbEvents
        .Top = BinMaps1.Top
        .Height = BinMaps1.Height
        .Left = BinMaps1.Left + BinMaps1.Width
        .Width = Me.Width - BinMaps1.Width - 150
    End With
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "ResizeControls", Err.Description
     Resume ErrExit
End Sub

Private Sub SaveTemp(SensorID As Long, NewVal As String)
    Dim Obj As clsRecord
    Dim objSensor As clsSensor
    On Error GoTo ErrHandler
    Set Obj = New clsRecord
    With Obj
        .BeginEdit
        .SensorID = SensorID
        .Temperature = CLng(NewVal)
        .recDate = Now
        .ApplyEdit
    End With
    Set objSensor = New clsSensor
    With objSensor
        .Load SensorID
        .BeginEdit
        .LastReading = Now
        .LastTemp = CLng(NewVal)
        .ApplyEdit
    End With
ErrExit:
    Set Obj = Nothing
    Set objSensor = Nothing
    Exit Sub
ErrHandler:
    AD.SaveToLog Err.Description, "frmStart", "SaveTemp", Err.Number, adFull
    Resume ErrExit
End Sub

Private Sub sckServer_Close(Index As Integer)
    On Error GoTo ErrHandler
    ' If not the listening socket then ...
    If Index > 0 Then
        ' Make sure the connection is closed
        If sckServer(Index).State <> sckClosed Then sckServer(Index).Close
        ' Unload the control
        Unload sckServer(Index)
        LogWrite "Socket " & CStr(Index) & " - Connection closed"
    End If
    CountSockets
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "sckServer_Close", Err.Description
    Resume ErrExit
End Sub

Private Sub sckServer_ConnectionRequest(Index As Integer, ByVal requestID As Long)
    ' Accept the connection on the next socket control in the control array
    Dim ID As Integer
    Dim Sck As Winsock
    On Error GoTo ErrHandler
    ' Ignore request if not the server socket.
    If Index = 0 Then
        CheckDeadSockets
        ' Look for an available socket control index
        ID = 1
        For Each Sck In sckServer
            ' Skip index 0 as this is the listening socket
            ' this accounts for sockets that may have been
            ' unloaded. Instead of sockets 0,1,2,3 in the collection
            ' it may be 0,1,3. The next socket selected would be 2.
            If Sck.Index > 0 Then
                If ID < Sck.Index Then Exit For
                ID = ID + 1
            End If
        Next Sck
        ' check for maximum # of controls
        If ID <= MaxSockets Then
            ' Load new control and accept connection
            Load sckServer(ID)
            sckServer(ID).LocalPort = 0
            sckServer(ID).Accept requestID
            ' Indicate status
            LogWrite "Socket " & CStr(ID) & " accepted a connection"
            CountSockets
        End If
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "sckServer_ConnectionRequest", Err.Description
    Resume ErrExit
End Sub

Private Sub sckServer_DataArrival(Index As Integer, ByVal bytesTotal As Long)
    Dim strData As String
    On Error GoTo ErrHandler
    'socket 0 is only for making connections
    If Index > 0 Then
        sckServer(Index).GetData strData, vbString
        ProcessData strData, Index
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "sckServer_DataArrival", Err.Description
     Resume ErrExit
End Sub

Private Sub sckServer_Error(Index As Integer, ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    AD.SaveToLog Description, "frmStart", "sckServer_Error" & "(" & Index & ")", CLng(Number), adFull
    CancelDisplay = True
End Sub

Public Sub SendPackets()
    Dim ID As Long
    Dim SentPacket As Boolean
    On Error GoTo ErrHandler
    SentPacket = False
    If OP.Count > 0 Then
        For ID = OP.Count To 1 Step -1
            If OP(ID).TimeStamp <= Now Then
                'send packet
                If mnuServer.Checked Then
                    CheckDeadSockets
                    If OP(ID).SocketID > 0 And IsLoaded(sckServer(OP(ID).SocketID)) Then
                        sckServer(OP(ID).SocketID).SendData OP(ID).StreamData
                        SentPacket = True
                    End If
                End If
                OP.Remove ID
                If SentPacket Then Exit For    'only do one packet per timer loop (one second)
            End If
        Next ID
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.SaveToLog Err.Description, "frmStart", "SendPacket", Err.Number, adFull
     Resume ErrExit
End Sub

Private Sub ShowBoxes()
    On Error GoTo ErrHandler
    Dim Mes As String
    Mes = SocketCount & " Controlboxes connected"
    Me.Caption = "TemperatureMonitor - [ " & Mes & " ]"
    AD.SaveToLog Mes, , , , adTimeStamp
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmStart", "ShowBoxes", Err.Description
    Resume ErrExit
End Sub

Private Sub Timer1_Timer()
    On Error GoTo ErrHandler
    CheckServerStatus
    'recorddata
    If (mnuRecord.Checked And DateDiff("n", LastReading, Now) > Prog.RecordInterval) Or RefreshReadings Then
        RefreshReadings = False
        AddPackets pdGetTemperatures
        LastReading = Now
        HalfTime = DateAdd("n", Prog.RecordInterval / 2, LastReading)
        HalfTimeRan = False
    End If
    'heartbeat
    If DateDiff("s", LastHeartBeat, Now) > HBtime Then
'        LogWrite "Heartbeat time: " & DateDiff("s", LastHeartBeat, Now)
        LastHeartBeat = Now
        AddPackets PacketType.pdHeartBeat
    End If
    If Now > HalfTime And Not HalfTimeRan Then
        'run 1/2 way in-between reads
        'also updates mouse-over values
        CheckAlarms
    End If
    If DateDiff("d", DailyJob, Now) > 1 Then
        'check tblRecords size
        Prog.Shrink
        'compact database
        MnuCompact_Click
        DailyJob = Now
    End If
    If Prog.AutoSave Then DoAutoReport
    SendPackets
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "Timer1_Timer", Err.Description
     Resume ErrExit
End Sub

Private Sub TimerResize_Timer()
'---------------------------------------------------------------------------------------
' Procedure : TimerResize_Timer
' Author    : XPMUser
' Date      : 1/24/2017
' Purpose   : verify controls have been resized
'---------------------------------------------------------------------------------------
'
    On Error GoTo ErrHandler
    TimerResize.Enabled = False
    ResizeControls
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "TimerResize_Timer", Err.Description
     Resume ErrExit
End Sub

Private Sub Toolbar1_ButtonClick(ByVal Button As MSComctlLib.Button)
    On Error GoTo ErrHandler
    Select Case Button.Index
        Case 2
            'server
            mnuServer_Click
        Case 3
            'record
            mnuRecord_Click
        Case 4
            'notifications
            MnuNotify_Click
        Case 5
            'maps
            mnuUseMaps_Click
        Case 6
            'info
            mnuInfo_Click
        Case 7
            'sensor database
            frmSensorsSht.Show vbModal
            Toolbar1.Buttons(7).Value = tbrUnpressed
        Case 8
            'refresh
            LogWrite "Refreshing .... "
            RefreshReadings = True
            Toolbar1.Buttons(8).Value = tbrUnpressed
        Case 9
            'bin report
            frmBinReport.Show vbModal
            Toolbar1.Buttons(9).Value = tbrUnpressed
        Case 10
            'options
            frmOptions.Show vbModal
            CheckAlarms
            Toolbar1.Buttons(10).Value = tbrUnpressed
    End Select
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "Toolbar1_ButtonClick", Err.Description
     Resume ErrExit
End Sub

Private Sub UpdateButtons()
    On Error GoTo ErrHandler
    With Toolbar1
        If mnuServer.Checked Then
            .Buttons(2).Value = tbrPressed
        Else
            .Buttons(2).Value = tbrUnpressed
        End If
        If mnuRecord.Checked Then
            .Buttons(3).Value = tbrPressed
        Else
            .Buttons(3).Value = tbrUnpressed
        End If
        If MnuNotify.Checked Then
            .Buttons(4).Value = tbrPressed
        Else
            .Buttons(4).Value = tbrUnpressed
        End If
        If mnuUseMaps.Checked Then
            .Buttons(5).Value = tbrPressed
        Else
            .Buttons(5).Value = tbrUnpressed
        End If
    End With
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "UpdateButtons", Err.Description
     Resume ErrExit
End Sub

Private Sub UpdateMaps()
    On Error GoTo ErrHandler
    If DBconnected Then
        ConnectMaps BinMaps1, mnuUseMaps.Checked
    Else
        mnuUseMaps.Checked = False
        BinMaps1.Visible = False
    End If
ErrExit:
    Exit Sub
ErrHandler:
    'stop using maps if there is an error
    AD.DisplayError Err.Number, "frmStart", "UpdateMaps", Err.Description
    Call MsgBox("Maps will be disabled.", vbInformation Or vbSystemModal, App.Title)
    mnuUseMaps.Checked = False
    BinMaps1.Visible = False
    Resume ErrExit
End Sub

Private Sub UpdateStatusFile(AlarmOn As Boolean)
    Dim Mes As String
    On Error GoTo ErrHandler
    If PrevAlarmOn <> AlarmOn Then
        PrevAlarmOn = AlarmOn
        If AlarmOn Then
            Mes = "Bin Temperature Alarm"
        End If
    Else
        Mes = ""
    End If
    'run this sub even if Mes="" just to check if there is any old
    'data that should be sent
    AD.SaveStatus Mes
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmStart", "UpdateStatusFile", Err.Description
     Resume ErrExit
End Sub

