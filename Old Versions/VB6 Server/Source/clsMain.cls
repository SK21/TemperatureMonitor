VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit

Private Type ObjProps
    dbType As String
    DbVersion As Long
    MaxDBsize As Long       'in K
    PulseTime As Long       'time in seconds before ControlBox attempts to reconnect
    MaxTemp As Long         'maximum sensor temp in C
    RecordInterval As Long  'minutes
    ControlBoxDelay As Long     'time delay in seconds between ControlBoxes
    AutoSave As Boolean     'auto save sensor report
    Days As Long
    Hrs As Long
    Location As String * 255
    StartSave As Date       'start time for auto-save
End Type

Event Valid(IsValid As Boolean)
Event DataEdited()
Event DatabaseLoaded()

Private mflgNew As Boolean
Private mflgDeleted As Boolean
Private mflgDirty As Boolean
Private mflgEditing As Boolean
Private WithEvents mobjValid As BrokenRules
Attribute mobjValid.VB_VarHelpID = -1
Private CurrentDbName As String
Private CurrentDbPassword As String
Private mudtProps As ObjProps
Private mudtSave As ObjProps

Public Sub ApplyEdit()
    If Not mflgEditing Then Err.Raise 445
    If mflgDeleted Then
        If Not mflgNew Then
            'code to delete the object's data goes here
            'no code since object won't be deleted
            mflgNew = True
        End If
        mflgDeleted = False
    ElseIf mflgDirty Or mflgNew Then
        If Not IsValid Then Err.Raise 445
        'save object to database if appropriate
        Save
        'save object state
        LSet mudtSave = mudtProps
        mflgNew = False
    End If
    mflgDirty = False
    mflgEditing = False
End Sub

Public Sub BeginEdit()
    If mflgEditing Then Err.Raise 445
    'save object state
    LSet mudtSave = mudtProps
    mflgEditing = True
End Sub

Public Property Get BrokenRules() As BrokenRules
    Set BrokenRules = mobjValid
End Property

Public Sub CancelEdit()
    If Not mflgEditing Then Err.Raise 445
    mflgEditing = False
    mflgDeleted = False
    mflgDirty = False
    'restore object state
    LSet mudtProps = mudtSave
End Sub

Private Sub Class_Initialize()
    mflgNew = True
    Set mobjValid = New BrokenRules
    LoadDefaults True
    CheckRules
End Sub

Private Sub CheckRules()
    mobjValid.RuleBroken "Database", (mudtProps.dbType <> LCase(AppName)), "A Database is required."
    mobjValid.RuleBroken "Version", (mudtProps.DbVersion <> CurrentDBversion), "The database version is invalid."
    If mudtProps.AutoSave Then
'        If (mudtProps.Days = 0 And mudtProps.Hrs = 0) Or Not DirExists(mudtProps.Location) Then
        If (mudtProps.Days = 0 And mudtProps.Hrs = 0) Then
            mobjValid.RuleBroken "AutoSave", True, "Invalid Auto-Save settings."
        Else
            mobjValid.RuleBroken "AutoSave", False
        End If
    Else
        mobjValid.RuleBroken "AutoSave", False
    End If
End Sub

Public Sub LoadDefaults(Optional Update As Boolean = True)
    mudtProps.MaxDBsize = 1000      'Kilobytes
    mudtProps.PulseTime = 60        'seconds
    mudtProps.MaxTemp = 35          'degrees C
    mudtProps.RecordInterval = 15   'minutes
    mudtProps.ControlBoxDelay = 5       'seconds
    mudtProps.AutoSave = False
    If Update Then ValueChanged
    CheckRules
End Sub

Private Sub Class_Terminate()
    On Error Resume Next
    MainDB.Close
End Sub

Public Property Get DataChanged() As Boolean
    DataChanged = mflgDirty
End Property

Public Property Get IsDeleted() As Boolean
    IsDeleted = mflgDeleted
End Property

Public Property Get IsDirty() As Boolean
    IsDirty = mflgDirty Or mflgNew
End Property

Public Property Get IsNew() As Boolean
    IsNew = mflgNew
End Property

Public Property Get IsValid() As Boolean
    IsValid = (mobjValid.Count = 0)
End Property

Private Sub mobjValid_BrokenRule()
    RaiseEvent Valid(False)
End Sub

Private Sub mobjValid_NoBrokenRules()
    RaiseEvent Valid(True)
End Sub

Private Sub Save()
    Dim SQL As String
    Dim RS As Recordset
    SQL = "select * from tblProps"
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If mflgNew Then
            .AddNew
        Else
            .Edit
        End If
        !dbType = mudtProps.dbType
        !DbVersion = mudtProps.DbVersion
        !dbMaxDBsize = mudtProps.MaxDBsize
        !dbPulseTime = mudtProps.PulseTime
        !dbMaxTemp = mudtProps.MaxTemp
        !dbRecordInterval = mudtProps.RecordInterval
        !dbControlBoxDelay = mudtProps.ControlBoxDelay
        !dbAutoSave = mudtProps.AutoSave
        !dbDays = mudtProps.Days
        !dbHrs = mudtProps.Hrs
        !dbLocation = mudtProps.Location
        !dbStartSave = mudtProps.StartSave
        .Update
        .Close
    End With
    Set RS = Nothing
End Sub

Private Function FileExists(DBname As String) As Boolean
    Dim FSO As FileSystemObject
    Set FSO = New FileSystemObject
    FileExists = FSO.FileExists(DBname)
    Set FSO = Nothing
End Function

Private Function CheckPassword(DBname As String, DBpassword As String) As Boolean
    Dim DB As Database
    On Error GoTo ErrExit
    CheckPassword = False
    Set DB = OpenDatabase(DBname, False, False, ";pwd=" & DBpassword)
    CheckPassword = True
ErrExit:
    Set DB = Nothing
End Function

Private Function CheckDatabaseType(DBname As String, DBpassword As String) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : CheckDatabase
' Author    : XPMUser
' Date      : 1/17/2016
' Purpose   : checks if database type is correct
'---------------------------------------------------------------------------------------
'
    Dim DB As Database
    Dim RS As Recordset
    On Error GoTo ErrExit
    CheckDatabaseType = False
    Set DB = OpenDatabase(DBname, False, False, ";pwd=" & DBpassword)
    Set RS = DB.OpenRecordset("Select * from tblProps")
    If LCase(RS!dbType) = LCase(AppName) Then
        CheckDatabaseType = True
    End If
ErrExit:
    Set DB = Nothing
    Set RS = Nothing
End Function

Private Function CheckDatabaseVersion(DBname As String, DBpassword As String) As Long
    Dim DB As Database
    Dim RS As Recordset
    On Error GoTo ErrExit
    CheckDatabaseVersion = 0
    Set DB = OpenDatabase(DBname, False, False, ";pwd=" & DBpassword)
    Set RS = DB.OpenRecordset("Select * from tblProps")
    CheckDatabaseVersion = RS!DbVersion
ErrExit:
    Set DB = Nothing
    Set RS = Nothing
End Function

Public Sub LoadDB(DBname As String, Optional DBpassword As String = "")
    Dim Ver As Long
    If Not FileExists(DBname) Then Err.Raise ErrFileNotFound
    If Not CheckPassword(DBname, DBpassword) Then Err.Raise ErrPassword
    If Not CheckDatabaseType(DBname, DBpassword) Then Err.Raise ErrDBtype
    Ver = CheckDatabaseVersion(DBname, DBpassword)
    If Ver < CurrentDBversion Then Err.Raise ErrLowVersion
    If Ver > CurrentDBversion Then Err.Raise ErrHighVersion
    Load DBname, DBpassword
End Sub

Private Sub Load(DBname As String, Optional DBpassword As String = "")
    Dim RS As Recordset
    Dim SQL As String
    If mflgEditing Then Err.Raise 445, "clsMain.Load"
'    If Not mflgNew Then Err.Raise 445, "clsMain.Load"
    CurrentDbName = DBname
    CurrentDbPassword = DBpassword
    'if database doesn't open, the error will raise up to
    'the calling procedure
    Set MainDB = OpenDatabase(DBname, False, False, ";pwd=" & DBpassword)
    SQL = "select * from tblProps"
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        mudtProps.dbType = LCase(NZ(!dbType, True))
        mudtProps.DbVersion = NZ(!DbVersion)
        mudtProps.MaxDBsize = NZ(!dbMaxDBsize)
        mudtProps.PulseTime = NZ(!dbPulseTime)
        mudtProps.MaxTemp = NZ(!dbMaxTemp)
        mudtProps.RecordInterval = NZ(!dbRecordInterval)
        mudtProps.ControlBoxDelay = NZ(!dbControlBoxDelay)
        mudtProps.AutoSave = NZ(!dbAutoSave)
        mudtProps.Days = NZ(!dbDays)
        mudtProps.Hrs = NZ(!dbHrs)
        mudtProps.Location = NZ(!dbLocation, True)
        mudtProps.StartSave = NZ(!dbStartSave)
        .Close
    End With
    CheckRules
    mflgNew = False
    RaiseEvent DatabaseLoaded
    Set RS = Nothing
End Sub

Private Sub ValueChanged()
    mflgDirty = True
    RaiseEvent DataEdited
End Sub

'****************** fields ***********************

Public Property Get dbType() As String
    dbType = mudtProps.dbType
End Property

Public Property Get DbVersion() As String
    DbVersion = mudtProps.DbVersion
End Property

Public Property Get MaxDBsize() As Long
    MaxDBsize = mudtProps.MaxDBsize
End Property

Public Property Let MaxDBsize(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.MaxDBsize <> NewVal Then
        If NewVal < 250 Or NewVal > 5000 Then Err.Raise InputErr, "clsMain", "Invalid Maximum database size. (250 - 5000) KB."
        mudtProps.MaxDBsize = NewVal
        ValueChanged
    End If
End Property

Public Property Get PulseTime() As Long
    PulseTime = mudtProps.PulseTime
End Property

Public Property Let PulseTime(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.PulseTime <> NewVal Then
        If NewVal < 10 Or NewVal > 3600 Then Err.Raise InputErr, "clsMain", "Invalid Pulse Time. (10 - 3600) seconds."
        mudtProps.PulseTime = NewVal
        ValueChanged
    End If
End Property

Public Property Get ControlBoxDelay() As Long
    ControlBoxDelay = mudtProps.ControlBoxDelay
End Property

Public Property Let ControlBoxDelay(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.ControlBoxDelay <> NewVal Then
        If NewVal < 0 Or NewVal > 20 Then Err.Raise InputErr, "clsMain", "Invalid ControlBox Delay. Range is 0 - 20."
        mudtProps.ControlBoxDelay = NewVal
        ValueChanged
    End If
End Property

Public Property Get MaxTemp() As Long
    MaxTemp = mudtProps.MaxTemp
End Property

Public Property Let MaxTemp(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.MaxTemp <> NewVal Then
        If NewVal < -40 Or NewVal > 40 Then Err.Raise InputErr, "clsMain", "Invalid maximum temperature. (-40 - 40) degrees C."
        mudtProps.MaxTemp = NewVal
        ValueChanged
    End If
End Property

Public Property Get RecordInterval() As Long
    RecordInterval = mudtProps.RecordInterval
End Property

Public Property Let RecordInterval(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.RecordInterval <> NewVal Then
        If NewVal < 10 Or NewVal > 10000 Then Err.Raise InputErr, "clsMain", "Invalid Record Interval. (10 - 10,000) minutes."
        mudtProps.RecordInterval = NewVal
        ValueChanged
    End If
End Property

Public Property Get AutoSave() As Boolean
    AutoSave = mudtProps.AutoSave
End Property

Public Property Let AutoSave(NewVal As Boolean)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.AutoSave <> NewVal Then
        mudtProps.AutoSave = NewVal
        ValueChanged
        CheckRules
        mudtProps.StartSave = Now
    End If
End Property

Public Property Get Days() As Long
    Days = mudtProps.Days
End Property

Public Property Let Days(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.Days <> NewVal Then
        If NewVal < 0 Or NewVal > 31 Then Err.Raise InputErr, "clsMain", "Invalid Days value. (0-31)"
        mudtProps.Days = NewVal
        ValueChanged
        CheckRules
    End If
End Property

Public Property Get Hours() As Long
    Hours = mudtProps.Hrs
End Property

Public Property Let Hours(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.Hrs <> NewVal Then
        If NewVal < 0 Or NewVal > 24 Then Err.Raise InputErr, "clsMain", "Invalid Hours value. (0-24)"
        mudtProps.Hrs = NewVal
        ValueChanged
        CheckRules
    End If
End Property

Public Property Get Location() As String
    Location = TR(mudtProps.Location)
End Property

Public Property Let Location(NewVal As String)
    If Not mflgEditing Then Err.Raise 383
    If TR(mudtProps.Location) <> NewVal Then
        If Len(NewVal) > 255 Then Err.Raise InputErr, "clsMain", "Location too long. Max. 255."
        If Not DirExists(NewVal) Then Err.Raise InputErr, "clsMain", "Invalid file location."
        mudtProps.Location = NewVal
        ValueChanged
        CheckRules
    End If
End Property

Public Property Get StartSave() As Date
    StartSave = mudtProps.StartSave
End Property

Public Property Let StartSave(NewVal As Date)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.StartSave <> NewVal Then
        mudtProps.StartSave = NewVal
        ValueChanged
    End If
End Property

'****************** other properties ***********************

Public Function ChangePassword(OldPassword As String, NewPassword As String) As Boolean
    Dim DBchange As Database
    If OldPassword <> CurrentDbPassword Then Err.Raise InputErr, , "Invalid Old Password."
    'close current database
    Set MainDB = Nothing
    'have to set to exclusive with the True parameter
    'for NewPassword method to work
    Set DBchange = OpenDatabase(CurrentDbName, True, False, ";pwd=" & CurrentDbPassword)
    DBchange.NewPassword CurrentDbPassword, NewPassword
    Set DBchange = Nothing
    mflgNew = True
    Load CurrentDbName, NewPassword
    ChangePassword = True
End Function

Public Sub Shrink()
    Dim RS As Recordset
    Dim SQL As String
    Dim RecCount As Long
    Dim CutPercent As Long
    Dim ID As Long
    RecCount = 0
    SQL = "select * from tblRecords"
    Set RS = MainDB.OpenRecordset(SQL)
    If Not RS.EOF Then
        RS.MoveLast
        RecCount = RS.RecordCount
    End If
    If RecCount > MaxRecords Then
        'reset size to 90% of max
        'find cutoff % from start that leaves correct size
        CutPercent = 100 - Int((MaxRecords * 0.9) / RecCount * 100)
        'find cutoff ID
        SQL = "select top " & CutPercent & " percent * from tblrecords order by recID"
        Set RS = MainDB.OpenRecordset(SQL)
        If Not RS.EOF Then
            RS.MoveLast
            ID = RS!recID
            'delete records up to cutoff ID
            SQL = "delete * from tblrecords where RecID < " & ID
            MainDB.Execute SQL, dbFailOnError
        End If
    End If
    Set RS = Nothing
End Sub

Public Function CompactDatabase() As Boolean
    Dim Temp As String
    CompactDatabase = False
    'get the store folder
    Temp = MainDB.Name
    Temp = Left$(Temp, InStrRev(Temp, "\")) & "temp.mdb"
    'close any open database connections
    Workspaces(0).Close
    'erase temp name if exists
    If Dir(Temp) <> "" Then Kill Temp
    If CurrentDbPassword = "" Then
        DBEngine.CompactDatabase CurrentDbName, Temp
    Else
        DBEngine.CompactDatabase CurrentDbName, Temp, , , CurrentDbPassword
    End If
    'check if new compacted file has been created
    If Dir(Temp) <> "" Then
        'file has been created
        Kill CurrentDbName
        Name Temp As CurrentDbName
        CompactDatabase = True
    End If
    mflgNew = True
    Load CurrentDbName, CurrentDbPassword
End Function

Public Function CreateDatabase(DBname As String, TBL() As Boolean, _
    Path As String, Optional DBpassword As String) As Boolean
    
    Dim NewPath As String
    Dim RS As Recordset
    Dim DBnew As Database
    Dim RSnew As Recordset
    
    NewPath = Path
    DBname = NewPath & "\" & DBname & ".mdb"
    'check new doesn't overwrite existing
    If Dir(DBname) <> "" Or DBname = "" Then Err.Raise InputErr, , "Invalid file name."
    'create a new blank database
    If Not CreatedNewDB(DBname, DBpassword) Then Err.Raise InputErr, , "Could not create new file."
    'create reference to new database
    Set DBnew = OpenDatabase(DBname, False, False, ";pwd=" & DBpassword)
    'edit tblProps
    Set RS = DBnew.OpenRecordset("tblProps")
    With RS
        If .EOF Then
            .AddNew
        Else
            .Edit
        End If
        !dbType = AppName
        !DbVersion = CurrentDBversion
        .Update
    End With
    Set RS = Nothing
    Set RSnew = Nothing
    Set DBnew = Nothing
    mflgNew = True
    Load DBname, DBpassword
    LoadDefaults False
    Save
    CreateDatabase = True
End Function

Public Function DatabaseFullName() As String
    DatabaseFullName = CurrentDbName
End Function

Public Function DatabaseName() As String
    'returns the file name out of a path
    'ex: C:\GrainManager\GrainManagerdata.mdb
    'would return 'GrainManagerdata'
    Dim P As Long
    Dim ST As Long
    Dim EN As Long
    ST = 0
    EN = Len(CurrentDbName) + 1
    For P = Len(CurrentDbName) To 1 Step -1
        If Mid$(CurrentDbName, P, 1) = "." Then EN = P
        If Mid$(CurrentDbName, P, 1) = "\" Then
            ST = P
            Exit For
        End If
    Next P
    DatabaseName = Mid$(CurrentDbName, ST + 1, EN - ST - 1)
End Function

Public Function UpdateDatabaseVersion(DBname As String, DBpassword As String)
    Dim Ver As Long
    Ver = CheckDatabaseVersion(DBname, DBpassword)
    'only update older versions, do nothing if same version
    If Ver < CurrentDBversion Then
'        'close references to database file
'        Workspaces(0).Close
        UpdateDB DBname, DBpassword, CurrentDBversion
'        mflgNew = True
'        Load CurrentDbName, CurrentDbPassword
    End If
End Function

