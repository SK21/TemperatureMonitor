Attribute VB_Name = "modMain"
Option Explicit
Public Const AppName = "TemperatureMonitor"
Public Const VersionDate As String = "06-Mar-17"
Public AD As clsAppData

'for manifest file
Private Declare Function LoadLibraryA Lib "kernel32.dll" (ByVal lpLibFileName As String) As Long
Private Declare Function FreeLibrary Lib "kernel32.dll" (ByVal hLibModule As Long) As Long

Private Type tagInitCommonControlsEx
   lngSize As Long
   lngICC As Long
End Type

Private Declare Function InitCommonControlsEx Lib "comctl32.dll" (iccex As tagInitCommonControlsEx) As Boolean
Private Const ICC_USEREX_CLASSES = &H200

'for help file
Private Declare Function HtmlHelp Lib "HHCtrl.ocx" Alias "HtmlHelpA" (ByVal hWndCaller As Long, ByVal pszFile As String, ByVal uCommand As Long, dwData As Any) As Long

Const HH_DISPLAY_TOPIC As Long = 0
Const HH_HELP_CONTEXT As Long = &HF

'other
Public Const CurrentDBversion As Long = 19
Public Const BeginPacket As String = "^"
Public Const MaxRecords As Long = 250000    'maximum # of temperature records

'error descriptions
Public Const ErrNum = "A number is required."
Public Const ErrDate = "A date is required."
Public Const ErrBoo = "True/False is required."
Public Const InputErr = vbObjectError + 1001    'global error #
Public Const ErrPassword = vbObjectError + 1002
Public Const ErrDBtype = vbObjectError + 1003     'wrong database type
Public Const ErrLowVersion = vbObjectError + 1004 'database version too low
Public Const ErrHighVersion = vbObjectError + 1005 'database version too high
Public Const ErrFileNotFound = vbObjectError + 1006  'database file doesn't exist

Public Enum GMStorageTypes
    GMSTall = 0
    GMSTBins = 1
    GMSTWarehouses = 2
End Enum

Public Enum SensorTypes
    stTemperature
End Enum

'// packet description:
'// start,packet type,break,data,break,sensor Rom Code,break
'// packet types:                                                  examples:
'// 0  heartbeat to signal still connected                         ^0|||       heartbeat
'// 1  command sensors to report either 0 or a specific board      ^1||0|      all sensors report
'// 2  set userdata                                                ^2|NewValue|RomCode|     set userdata for sensor
'// 3  get userdata                                                ^3|NewValue|RomCode|     get userdata for sensor

'// data saved on the sensor:
'// 16 bit data
'// 11111111  1111  1111
'// bin       cable sensor
'// 0-255     0-15  0-15

Public Enum PacketType
    pdHeartBeat
    pdGetTemperatures
    pdSetUserData
    pdGetUserData
End Enum

Public Enum SensorSort
    ssRomCodeAsc
    ssRomCodeDes
    ssLastDateAsc
    ssLastDateDes
    ssLastTempAsc
    ssLastTempDes
    ssBinNumberAsc
    ssBinNumberDes
End Enum

Public LastFolder As String
Public MainDB As DAO.Database
Public Prog As clsMain
Public DBconnected As Boolean
Public OP As clsPackets 'outgoing packets


Public Sub DisplayHelp(ID As Long, mForm As Form)
    On Error GoTo ErrHandler
    HtmlHelp mForm.hWnd, AppName & "Help.chm", HH_HELP_CONTEXT, ByVal ID
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "modMain", "DisplayHelp", Err.Description
    Resume ErrExit
End Sub

Private Function InitCommonControlsVB() As Boolean
    'for use with manifest files
    On Error Resume Next
    Dim iccex As tagInitCommonControlsEx
    ' Ensure CC available:
    With iccex
        .lngSize = LenB(iccex)
        .lngICC = ICC_USEREX_CLASSES
    End With
    InitCommonControlsEx iccex
    InitCommonControlsVB = (Err.Number = 0)
    On Error GoTo 0
End Function

Sub Main()
    'don't edit this sub use the Start sub
    Dim hMod As Long
    hMod = LoadLibraryA("shell32.dll") ' patch to prevent XP crashes when VB usercontrols present
    InitCommonControlsVB
    Start
    If hMod Then FreeLibrary hMod
End Sub

Public Sub Start()
    If Not App.PrevInstance Then
        App.HelpFile = App.Path & "\" & AppName & "Help.chm"
        Set OP = New clsPackets
        Set AD = New clsAppData
        LastFolder = AD.Folders(AppDatabase)
        frmStart.Show
    End If
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "modMain", "Start", Err.Description
    Resume ErrExit
End Sub

