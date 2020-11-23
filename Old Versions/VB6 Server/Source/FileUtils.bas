Attribute VB_Name = "FileUtils"
Option Explicit

Public Function ConnectDatabase(Optional DBname As String = "") As Boolean
'---------------------------------------------------------------------------------------
' Procedure : ConnectDatabase
' Author    : David
' Date      : 17/Jan/2016
' Purpose   : sets DBconnected flag on connection to database
'---------------------------------------------------------------------------------------
    On Error GoTo ErrHandler
    If DBname = "" Then
        'check connection
        If Not DBconnected Then
            Set Prog = New clsMain
            'try last database
            DBname = AD.Element("LastDB")
            If OpenDB(DBname) Then
                DBconnected = True
            Else
                'let user select database
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
                DBconnected = OpenDB(DBname)
            End If
        End If
    Else
        'connect to specific database
        DBconnected = OpenDB(DBname)
    End If
    If DBconnected Then
        AD.DatabaseFullName = Prog.DatabaseFullName
    End If
    ConnectDatabase = DBconnected
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
    AD.DisplayError Err.Number, "FileUtils", "ConnectDatabase", Err.Description
    Resume ErrExit
End Function

Public Function CopyCurrentFile(NewLoc As String) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : CopyCurrentFile
' Author    : David
' Date      : 3/1/2011
' Purpose   : FileName ex: "DC2001",NewLoc ex: "C:\temp"
'---------------------------------------------------------------------------------------
'
    Dim FSO As FileSystemObject
    Dim Des As String
    On Error GoTo ErrHandler
    Set FSO = New FileSystemObject
    'copy database
    Des = NewLoc & "\" & Prog.DatabaseName & ".mdb"
    FSO.CopyFile Prog.DatabaseFullName, Des
    CopyCurrentFile = True
    On Error GoTo 0
ErrExit:
    Set FSO = Nothing
    Exit Function
ErrHandler:
    AD.DisplayError Err.Number, "FileUtils", "CopyCurrentFile", Err.Description
    Resume ErrExit
End Function

Public Function DatabaseName(PathName As String, Optional ReturnLocation As Boolean) As String
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

Public Function DirExists(DirName As String) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : DirExists
' Author    : dc
' Date      : 30/Dec/2009
' Purpose   : Tests if DirName exists
'---------------------------------------------------------------------------------------
    On Error GoTo ErrHandler
    Dim FSO As New FileSystemObject
    DirExists = False
    If FSO.FolderExists(DirName) Then
        DirExists = True
    End If
    Set FSO = Nothing
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
    AD.DisplayError Err.Number, "FileUtils", "DirExists", Err.Description
    Resume ErrExit
End Function

Public Function FileModified(FileName As String, DateModified As Date) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : FileModified
' Author    : XPMUser
' Date      : 12/20/2014
' Purpose   : returns the date a file was last modified
'---------------------------------------------------------------------------------------
    Dim F As File
    Dim FSO As FileSystemObject
    On Error GoTo ErrExit
    Set FSO = New FileSystemObject
    Set F = FSO.GetFile(FileName)
    DateModified = F.DateLastModified
    FileModified = True
    Set F = Nothing
    Set FSO = Nothing
ErrExit:
    Exit Function
End Function

Public Function LegalFileName(ByVal NewName As String) As Boolean
    Dim BadChars As String
    Dim i As Long
    Dim IsLegal As Boolean
    On Error GoTo ErrExit
    LegalFileName = False
    If NewName <> "" Then
        IsLegal = True
        BadChars = "\/:*?<>|& " & Chr$(34)
        'test for bad characters
        For i = 1 To Len(NewName)
          If InStr(BadChars, Mid(NewName, i, 1)) <> 0 Then
            IsLegal = False
            Exit For
          End If
        Next i
        'try to create a file with the name to check for errors
        If IsLegal Then
            NewName = AD.Folders(AppDatabase) & "\" & NewName & ".txt"
            Open NewName For Output As #1
            Close #1
            Kill NewName
        End If
        LegalFileName = IsLegal
    End If
ErrExit:
End Function

Private Function LoadDB(DBname As String, DBpassword As String) As Long
    Dim ER As Long
    On Error GoTo ErrHandler
    LoadDB = 0
    Prog.LoadDB DBname, DBpassword
ErrExit:
    Exit Function
ErrHandler:
    'convert err number to object error
    ER = (Err.Number And &HFFFF&)
    Select Case ER
        Case 1002, 1003, 1004, 1005, 1006
            LoadDB = ER
        Case Else
            AD.DisplayError Err.Number, "FileUtils", "LoadDb", Err.Description
    End Select
    Resume ErrExit
End Function

Private Function OpenDB(DBname As String) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : OpenDB
' Author    : David
' Date      : 17/Jan/2016
' Purpose   :
'---------------------------------------------------------------------------------------
    Dim DBpassword As String
    Dim C As Long
    Dim Rslt As Long
    On Error GoTo ErrHandler
    OpenDB = False
    If DBname <> "" Then
        For C = 1 To 3
            Rslt = LoadDB(DBname, DBpassword)
            Select Case Rslt
                Case 0
                    'database loaded
                    OpenDB = True
                    Exit For
                Case 1002
                    'bad password
                    frmPassword.Show vbModal
                    If frmPassword.Canceled Then
                        Unload frmPassword
                        Exit For
                    Else
                        DBpassword = frmPassword.txtPassword
                        Unload frmPassword
                        'continue to next loop to see if new password works
                    End If
                Case 1003
                    Call MsgBox("Wrong Database Type!", vbExclamation, App.Title)
                    Exit For
                Case 1004
                    'database version too low
'                    R = MsgBox("This database is from a different version." & Chr$(13) & "Would you like to update?", vbYesNo, AppName)
'                    If R = vbYes Then
'                        Prog.UpdateDatabaseVersion DBname, DBpassword
'                        'continue with loop
'                    Else
'                        Exit For
'                    End If
                    Prog.UpdateDatabaseVersion DBname, DBpassword
                Case 1005
                    'database version too high
                    Call MsgBox("Database Version is newer than current, can not load or update.", vbExclamation, App.Title)
                    Exit For
                Case 1006
                    'file doesn't exist
                    Call MsgBox("File doesn't exist.", vbExclamation, App.Title)
                    Exit For
            End Select
        Next C
    End If
ErrExit:
    On Error GoTo 0
    Exit Function
ErrHandler:
    AD.DisplayError Err.Number, "FileUtils", "OpenDB", Err.Description
    Resume ErrExit
End Function

