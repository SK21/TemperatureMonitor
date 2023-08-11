VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsAppData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit
'Revisions:
' 06-Mar-17 Version 1.0 - initial version


' Folder locations:
' 1. Database (user data)                   CSIDL_COMMON_DOCUMENTS\AppName
' 2. documents for database                 CSIDL_COMMON_DOCUMENTS\AppName\database name
' 3. backup (backup database)               CSIDL_COMMON_DOCUMENTS\AppName\Backup
' 4. Common (for appdata and temp files)    CSIDL_COMMON_DOCUMENTS\AppName\Common
' requires FSO class
Const CSIDL_COMMON_DOCUMENTS = &H2E

Public Enum adMessageType
    adFull                  'full error message
    adShort                 'just description
    adTimeStamp             'includes time stamp
End Enum

Public Enum AppFldrs
    AppDocuments
    AppDatabase
    AppBackup
    AppCommon
End Enum

Private Type FolderLocations
    Documents As String
    Database As String
    Backup As String
    Common As String
End Type

Const MaxLogSize = 250000  'in bytes

Private iData() As String
Private iName() As String
Private iCount As Long
Private ErrorTime As Date
Private LastError As Long
Private Fldrs As FolderLocations
Private FSO As FileSystemObject
Private modUseDocumentsFolder
Private modDBname As String
Private modStatusSaved As Boolean
Private modStatusMessage As String
Private modStatusFileName As String
Private modUseTimeStamp As Boolean

Private Declare Function SHGetSpecialFolderLocation Lib "shell32" (ByVal hWndOwner As Long, ByVal nFolder As Long, PIDL As Long) As Long
Private Declare Function SHGetPathFromIDList Lib "shell32" Alias "SHGetPathFromIDListA" (ByVal PIDL As Long, ByVal pszPath As String) As Long

Private Sub CheckDocumentsFolder()
    If modUseDocumentsFolder Then
        'locate documents folder
        Fldrs.Documents = Fldrs.Database & "\" & DatabaseName(modDBname)
        If Not FSO.FolderExists(Fldrs.Documents) Then MkDirs (Fldrs.Documents)
    End If
End Sub

Private Sub CheckLogSize()
'---------------------------------------------------------------------------------------
' Procedure : CheckLogSize
' Author    : David
' Date      : 2/7/2012
' Purpose   : check if log size is too big and clip if necessary
'---------------------------------------------------------------------------------------
'
    Dim Ln() As String
    Dim L As Long
    Dim ST As Long
    Dim NewL As Long
    On Error GoTo ErrExit
    If FileLen(Fldrs.Common & "\Log.txt") > MaxLogSize Then
        Open Fldrs.Common & "\Log.txt" For Input As #1
        Do While Not EOF(1)
            L = L + 1
            ReDim Preserve Ln(L)
            Line Input #1, Ln(L)
        Loop
        Close #1
        Open Fldrs.Common & "\LogNew.txt" For Output As #1
        'start 10% from beginning and write new file
        ST = L * 0.1
        For NewL = ST To L
            Print #1, Ln(NewL)
        Next NewL
        Close #1
        Kill Fldrs.Common & "\Log.txt"
        Name Fldrs.Common & "\LogNew.txt" As Fldrs.Common & "\Log.txt"
    End If
ErrExit:
    Exit Sub
End Sub

Private Sub Class_Initialize()
    Dim Pth As String
    Dim DL As String
    Pth = CommonDocs
    If Pth = "" Then Pth = App.Path
    Set FSO = New FileSystemObject
    'locate common folder
    Fldrs.Common = Pth & "\" & AppName & "\Common"
    If Not FSO.FolderExists(Fldrs.Common) Then MkDirs (Fldrs.Common)
    GetAppData
    'locate backup folder
    Fldrs.Backup = Pth & "\" & AppName & "\Backup"
    If Not FSO.FolderExists(Fldrs.Backup) Then MkDirs (Fldrs.Backup)
    'locate database
    'check if location has been saved from previous use
    DL = Element("DataLocation")
    If Not FSO.FolderExists(DL) Then
        DL = Pth & "\" & AppName
    End If
    Fldrs.Database = DL
    If Not FSO.FolderExists(Fldrs.Database) Then MkDirs (Fldrs.Database)
    Element("DataLocation") = DL
    'get last database
    DL = Element("LastDB")
    If DL <> "" Then DatabaseFullName = DL
    modUseTimeStamp = True
    modStatusFileName = Fldrs.Common & "\Status.txt"
    EraseOldStatus
End Sub

Private Sub Class_Terminate()
    Set FSO = Nothing
End Sub

Public Property Get CommonDocs() As String
    Dim Rtn As Long
    Dim Buffer As String
    Dim ID As Long
    Buffer = Space$(260)
    Rtn = SHGetSpecialFolderLocation(0&, CSIDL_COMMON_DOCUMENTS, ID)
    If Rtn = 0 Then
        If SHGetPathFromIDList(ID, Buffer) Then
          CommonDocs = TrimNull(Buffer)
        End If
    End If
End Property

Public Property Let DatabaseFullName(NewVal As String)
    'requires the full path to the database file
    modDBname = NewVal
    'locate the database folder
    Fldrs.Database = DatabaseName(NewVal, True)
    If Not FSO.FolderExists(Fldrs.Database) Then MkDirs (Fldrs.Database)
    Element("DataLocation") = Fldrs.Database
    Element("LastDB") = NewVal
    CheckDocumentsFolder
End Property

Public Property Get DatabaseFullName() As String
    DatabaseFullName = modDBname
End Property

Private Function DatabaseName(ByVal PathName As String, Optional ReturnLocation As Boolean) As String
    'returns the file name out of a path
    'ex: C:\GrainManager\GrainManagerdata.mdb
    'would return 'GrainManagerdata'
    'if ReturnLocation then the return
    'value would be 'C:\GrainManager'
    Dim P As Long
    Dim ST As Long
    Dim EN As Long
    ST = 0
    EN = Len(PathName) + 1
    For P = Len(PathName) To 1 Step -1
        If Mid$(PathName, P, 1) = "." Then EN = P
        If Mid$(PathName, P, 1) = "\" Then
            ST = P
            Exit For
        End If
    Next P
    If ReturnLocation Then
        DatabaseName = Mid$(PathName, 1, ST - 1)
    Else
        DatabaseName = Mid$(PathName, ST + 1, EN - ST - 1)
    End If
End Function

Public Sub DisplayError(ByVal ErrorNum As Long, Module As String, _
    Procedure As String, _
    Optional Description As String = "")
    Dim Skip As Boolean
    On Error GoTo ErrExit
    SaveToLog Description, Module, Procedure, ErrorNum, adFull
    If ErrorTime = 0 Then ErrorTime = Now
    'check if mutiple errors of the same type should be skipped
    If ErrorNum <> 0 Then
        If ErrorNum = LastError Then
            If Abs(DateDiff("s", ErrorTime, Now)) < 4 Then Skip = True
        End If
    End If
    If Not Skip Then
        MsgBox "Error Number:" & vbTab & ErrorNum & Chr$(13) _
                & "Module:     " & vbTab & Module & Chr$(13) _
                & "Procedure:" & vbTab & Procedure & Chr$(13) _
                & "Description:" & vbTab & Description & Chr$(13)
        ErrorTime = Now
        LastError = ErrorNum
    End If
ErrExit:
    Exit Sub
End Sub

Public Property Get Element(DataName As String) As String
    Dim X As Long
    For X = 1 To iCount
        If LCase(DataName) = LCase(iName(X)) Then
            Element = iData(X)
            Exit For
        End If
    Next X
End Property

Public Property Let Element(DataName As String, NewVal As String)
    Dim X As Long
    Dim Found As Boolean
    Dim Changed As Boolean
    For X = 1 To iCount
        If LCase(DataName) = LCase(iName(X)) Then
            If LCase(iData(X)) <> LCase(NewVal) Then
                Changed = True
                iData(X) = NewVal
            End If
            Found = True
            Exit For
        End If
    Next X
    If Not Found Then
        'add line
        iCount = iCount + 1
        ReDim Preserve iName(iCount)
        ReDim Preserve iData(iCount)
        iName(iCount) = DataName
        iData(iCount) = NewVal
        Changed = True
    End If
    If Changed Then SaveAppData
End Property

Public Sub EraseLog()
    Kill Fldrs.Common & "\Log.txt"
End Sub

Private Sub EraseOldStatus()
    Dim FN As Integer
    If Dir(modStatusFileName) <> "" Then Kill modStatusFileName
    FN = FreeFile
    Open modStatusFileName For Output As #FN
    Close #FN
    SetAttr modStatusFileName, vbNormal
End Sub

Public Property Get FileModified() As Date
    Dim F As File
    Set F = FSO.GetFile(modDBname)
    FileModified = F.DateLastModified
    Set F = Nothing
End Property

Public Property Get FileSize() As Long
'---------------------------------------------------------------------------------------
' Procedure : FileSize
' Author    : XPMUser
' Date      : 1/16/2016
' Purpose   : returns database size in Kilobytes
'---------------------------------------------------------------------------------------
'
    Dim F As File
    Set F = FSO.GetFile(modDBname)
    FileSize = F.Size / 1024
    Set F = Nothing
End Property

Public Property Get Folders(FolderType As AppFldrs) As String
    Select Case FolderType
        Case AppFldrs.AppBackup
            Folders = Fldrs.Backup
        Case AppFldrs.AppCommon
            Folders = Fldrs.Common
        Case AppFldrs.AppDatabase
            Folders = Fldrs.Database
        Case AppFldrs.AppDocuments
            Folders = Fldrs.Documents
    End Select
End Property

Private Sub GetAppData()
'---------------------------------------------------------------------------------------
' Procedure : GetAppData
' Author    : David
' Date      : 12/4/2010
' Purpose   :
'---------------------------------------------------------------------------------------
    On Error GoTo ErrHandler
    iCount = 0
    Open Fldrs.Common & "\AppData.txt" For Input As #1
    Do While Not EOF(1)
        iCount = iCount + 1
        ReDim Preserve iData(iCount)
        ReDim Preserve iName(iCount)
        Input #1, iName(iCount), iData(iCount)
    Loop
    Close #1
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    Select Case Err.Number
        Case 53
            'file not found
            'do nothing
        Case Else
            Err.Raise Err.Number
    End Select
    Resume ErrExit
End Sub

Private Sub GetFormData(Frm As Form, LeftPos As Long, TopPos As Long, State As Long, frmWidth As Long, frmHeight As Long)
    Dim DN As String
    DN = "Form_" & Frm.Name
    LeftPos = Val(Element(DN & "_Left"))
    TopPos = Val(Element(DN & "_Top"))
    State = Val(Element(DN & "_State"))
    frmWidth = Val(Element(DN & "_Width"))
    frmHeight = Val(Element(DN & "_Height"))
End Sub

Public Function InIDE() As Boolean
    'returns true if in IDE mode
    InIDE = CBool(App.LogMode = 0)
End Function

Public Sub LoadFormData(Frm As Form)
    Dim lp As Long
    Dim TP As Long
    Dim ST As Long
    Dim Wd As Long
    Dim Ht As Long
    GetFormData Frm, lp, TP, ST, Wd, Ht
    If (lp + Frm.Width > Screen.Width) Or lp = 0 Then
        lp = (Screen.Width - Frm.Width) / 2
    End If
    If (TP + Frm.Height > Screen.Height) Or TP = 0 Then
        TP = (Screen.Height - Frm.Height) / 2
    End If
    'don't resize fixed border windows
    If Frm.BorderStyle <> 3 And ST = vbNormal Then
        If Wd > 0 Then Frm.Width = Wd
        If Ht > 0 Then Frm.Height = Ht
    End If
    Frm.Left = lp
    Frm.Top = TP
    Frm.WindowState = ST
End Sub

Private Function MkDirs(ByVal Folder As String) As Boolean
   Dim F() As String
   Dim attr As Long
   Dim first As Long
   Dim i As Long

   ' Split incoming folder into subfolders.
   F = Split(Folder, "\")
   For i = 1 To UBound(F)
      F(i) = F(i - 1) & "\" & F(i)
   Next i

   ' If the input path is UNC, the first Attribute
   ' will be empty and the second "\", so we need
   ' to adjust where we start creating folders.
   If F(0) = "" And UBound(F) > 0 Then
      If F(1) = "\" Then
         first = 4  'fourth Attribute is first path.
      End If
   End If

   ' Use errors to signal need to take action.
   On Error Resume Next
   For i = first To UBound(F)
      ' Check if this level already exists.
      attr = GetAttr(F(i))
      If Err.Number Then
         ' Folder likely doesn't exist,
         ' clear error and create.
         Err.Clear
         MkDir F(i)
         If Err.Number Then Exit For
      End If
   Next i

   ' Return success?
   MkDirs = CBool(GetAttr(Folder) And vbDirectory)
End Function

Private Sub SaveAppData()
'---------------------------------------------------------------------------------------
' Procedure : SaveAppData
' Author    : David
' Date      : 12/4/2010
' Purpose   :
'---------------------------------------------------------------------------------------
    Dim C As Long
    Dim Nm As String
    Nm = Fldrs.Common & "\AppData.txt"
    Open Fldrs.Common & "\AppDataNew.txt" For Output As #1
    For C = 1 To iCount
        If iData(C) <> "" Then
            Write #1, iName(C), iData(C)
        End If
    Next C
    Close #1
    On Error Resume Next
    Kill Nm
    On Error GoTo 0
    Name Fldrs.Common & "\AppDataNew.txt" As Nm
End Sub

Public Sub SaveFormData(Frm As Form)
    Dim DN As String
    DN = "Form_" & Frm.Name
    If Frm.WindowState <> vbMinimized Then
        Element(DN & "_Left") = Frm.Left
        Element(DN & "_Top") = Frm.Top
        Element(DN & "_State") = Frm.WindowState
        Element(DN & "_Width") = Frm.Width
        Element(DN & "_Height") = Frm.Height
    End If
End Sub

Public Sub SaveStatus(Message As String)
    If modUseTimeStamp And Message <> "" Then
        Message = Format(Now, "dd-mmm-yy hh:mm:ss AM/PM") & ":  " & Message
    End If
    If modStatusSaved Then
        'save new message
        modStatusMessage = Message
    Else
        'add new message to old message and save
        modStatusMessage = modStatusMessage & Message
    End If
    If modStatusMessage <> "" Then
        modStatusSaved = SaveStatusToFile
    End If
End Sub

Private Function SaveStatusToFile() As Boolean
    Dim FN As Integer
    On Error GoTo ErrExit
    SaveStatusToFile = False
    FN = FreeFile
    If (GetAttr(modStatusFileName) And vbArchive) = 0 Then
        'file has been read, overwrite
        Open modStatusFileName For Output Lock Read Write As #FN
    Else
        'file has not been read, append
        Open modStatusFileName For Append Lock Read Write As #FN
    End If
    Print #FN, modStatusMessage & vbNewLine
    Close #FN
    SaveStatusToFile = True
ErrExit:
End Function

Public Sub SaveToLog(Message As String, Optional Module As String, _
    Optional Procedure As String, Optional ErrorNum As Long, _
    Optional MessageType As adMessageType = adShort)
    On Error GoTo ErrExit
    Select Case MessageType
        Case adMessageType.adFull
            Open Fldrs.Common & "\Log.txt" For Append As #1
            Write #1, " ----------- Error -----------"
            Write #1, Format(Now, " dd-MMM-yy h:mm:ss AM/PM")
            Write #1, " Description: " & Message
            Write #1, " Module: " & Module
            Write #1, " Procedure: " & Procedure
            Write #1, " Error #: " & ErrorNum
            Write #1, " -----------------------------"
            Close #1
        Case adMessageType.adTimeStamp
            Open Fldrs.Common & "\Log.txt" For Append As #1
            Write #1, Format(Now, " dd-MMM-yy h:mm:ss AM/PM") & "     " & Message
            Close #1
        Case Else
            Open Fldrs.Common & "\Log.txt" For Append As #1
            Write #1, Message
            Close #1
    End Select
    CheckLogSize
ErrExit:
    Exit Sub
End Sub

Public Property Get StatusFileName() As String
    StatusFileName = modStatusFileName
End Property

Public Property Let StatusFileName(NewVal As String)
    modStatusFileName = NewVal
End Property

Private Function TrimNull(ByVal StrZ As String) As String
   TrimNull = Left$(StrZ, InStr(StrZ & vbNullChar, vbNullChar) - 1)
End Function

Public Property Get UseDocumentsFolder() As Boolean
    UseDocumentsFolder = modUseDocumentsFolder
End Property

Public Property Let UseDocumentsFolder(NewVal As Boolean)
    modUseDocumentsFolder = NewVal
    Element("UseDocumentsFolder") = NewVal
    CheckDocumentsFolder
End Property

Public Property Get UseTimeStamp() As Boolean
    UseTimeStamp = modUseTimeStamp
End Property

Public Property Let UseTimeStamp(NewVal As Boolean)
    modUseTimeStamp = NewVal
End Property

