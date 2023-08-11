VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsSensor"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit
Event Valid(IsValid As Boolean)
Event DataEdited()

Private mflgNew As Boolean
Private mflgDeleted As Boolean
Private mflgDirty As Boolean
Private mflgEditing As Boolean
Private WithEvents mobjValid As BrokenRules
Attribute mobjValid.VB_VarHelpID = -1

Private Type ObjProps
    ID As Long
    RecNum As Long
    RomCode As String * 30
    LastReading As Date
    LastTemp As Long
    BinID As Long           'ID not BinNumber or StorNum
    CableNumber As Long     'number between 1 and 16
    SensorNumber As Long    'number between 1 and 16
    Enabled As Boolean
    Socket As Integer
    MaxAlarm As Boolean
    Offset As Long
End Type

Private mudtProps As ObjProps
Private mudtSave As ObjProps

Public Sub ApplyEdit()
    Dim DL As String
    If Not mflgEditing Then Err.Raise 445
    If mflgDeleted Then
        If Not mflgNew Then
            'code to delete the object's data goes here
'            DL = "tblRecords,recSenID," & mudtProps.ID & ",false"
            If DependantData(DL) Then
                Err.Raise InputErr, "Sensor", "Dependant data."
            End If
            DeleteRecord MainDB, "tblSensors", mudtProps.ID, "senID"
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
    
    'Enter default values for new records
    mudtProps.RecNum = CCur(LastValue(MainDB, "tblSensors", "senRecNum")) + 1
    mudtProps.Enabled = True
    'initialize the list of broken rules
    CheckRules
End Sub

Private Sub CheckRules()
    mobjValid.RuleBroken "Number", Not ValidSensorNumber(), "Duplicate entry."
End Sub

Public Property Get DataChanged() As Boolean
    DataChanged = mflgDirty
End Property

Public Sub Delete()
    If Not mflgEditing Then Err.Raise 445
    mflgDeleted = True
    mflgDirty = True
End Sub

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
    SQL = "select * from tblSensors where senID = " & mudtProps.ID
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If mflgNew Then
            .AddNew
        Else
            .Edit
        End If
        !senRecNum = mudtProps.RecNum
        !senRomCode = TR(mudtProps.RomCode)
        !senLastReading = mudtProps.LastReading
        !senLastTemp = mudtProps.LastTemp
        !senStorID = mudtProps.BinID
        !senCableNumber = mudtProps.CableNumber
        !senSensorNumber = mudtProps.SensorNumber
        !senEnabled = mudtProps.Enabled
        !senSocket = mudtProps.Socket
        !senMaxAlarm = mudtProps.MaxAlarm
        !senOffset = mudtProps.Offset
        .Update
        'get ID from autoincrement field after file is saved
        If mflgNew Then
            .Bookmark = .LastModified
            mudtProps.ID = !SenID
        End If
        .Close
    End With
    Set RS = Nothing
End Sub

Public Sub Load(Optional SenID As Long = 0, Optional RomCode As String, _
    Optional BinID As Long, Optional CableNumber As Long, Optional SensorNumber As Long)
    Dim RS As Recordset
    Dim SQL As String
    If mflgEditing Then Err.Raise 445
    If Not mflgNew Then Err.Raise 445
    If RomCode = "" Then
        If BinID = 0 Then
            'load by sensor ID
            SQL = "select * from tblSensors where senID = " & SenID
        Else
            'load by BinID,CableNumber and SensorNumber
            SQL = "select * from tblSensors where senStorID = " & BinID
            SQL = SQL & " and senCableNumber = " & CableNumber
            SQL = SQL & " and senSensorNumber = " & SensorNumber
        End If
    Else
        'load by Rom Code
        SQL = "select * from tblSensors where senRomCode = '" & RomCode & "'"
    End If
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If Not .EOF Then
            mudtProps.ID = NZ(!SenID)
            mudtProps.RecNum = NZ(!senRecNum)
            mudtProps.RomCode = NZ(!senRomCode)
            mudtProps.LastReading = NZ(!senLastReading)
            mudtProps.LastTemp = NZ(!senLastTemp)
            mudtProps.BinID = NZ(!senStorID)
            mudtProps.CableNumber = NZ(!senCableNumber)
            mudtProps.SensorNumber = NZ(!senSensorNumber)
            mudtProps.Enabled = NZ(!senEnabled)
            mudtProps.Socket = NZ(!senSocket)
            mudtProps.MaxAlarm = NZ(!senMaxAlarm)
            mudtProps.Offset = NZ(!senOffset)
            mflgNew = False
            CheckRules
        End If
       .Close
    End With
    Set RS = Nothing
End Sub

Private Sub ValueChanged()
    mflgDirty = True
    RaiseEvent DataEdited
End Sub

'****************** fields ***********************

Friend Property Let ID(NewVal As String)
    'this is a 'Friend' property to allow the collection class
    'to work properly
    mudtProps.ID = CCur(NewVal)
End Property

Public Property Get ID() As String
    ID = mudtProps.ID
End Property

Friend Property Let RecNum(NewVal As String)
    mudtProps.RecNum = CCur(NewVal)
End Property

Public Property Get RecNum() As String
    RecNum = mudtProps.RecNum
End Property

Public Property Get RomCode() As String
    RomCode = TR(mudtProps.RomCode)
End Property

Public Property Let RomCode(NewVal As String)
    If Not mflgEditing Then Err.Raise 383
    If TR(mudtProps.RomCode) <> TR(NewVal) Then
        If Len(NewVal) > Len(mudtProps.RomCode) Then
            Err.Raise InputErr, "", "Rom Code too long."
        End If
        mudtProps.RomCode = NewVal
        CheckRules
        ValueChanged
    End If
End Property

Public Property Get LastReading() As Date
    LastReading = mudtProps.LastReading
End Property

Public Property Let LastReading(NewVal As Date)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.LastReading <> NewVal Then
        mudtProps.LastReading = NewVal
        ValueChanged
    End If
End Property

Public Property Get LastTemp() As Long
    LastTemp = mudtProps.LastTemp
End Property

Public Property Let LastTemp(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.LastTemp <> NewVal Then
        mudtProps.LastTemp = NewVal
        ValueChanged
    End If
End Property

Public Property Get BinID() As String
    'ID not BinNumber or StorNum
    BinID = mudtProps.BinID
End Property

Public Property Let BinID(NewVal As String)
    'ID not BinNumber or StorNum
    Dim NV As Long
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "BinTest", "A number is required."
    End If
    NV = CLng(NewVal)
    If NV <> mudtProps.BinID Then
        'check NewVal exists
        If Not ValueFound(MainDB, "tblStorage", "StorID", NV) Then
            Err.Raise InputErr, "clsSensor", "Bin not found."
        End If
        mudtProps.BinID = NV
        CheckRules
        ValueChanged
    End If
End Property

Public Property Get CableNumber() As Long
    CableNumber = mudtProps.CableNumber
End Property

Public Property Let CableNumber(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.CableNumber <> NewVal Then
        If NewVal < 1 Or NewVal > 16 Then Err.Raise InputErr, "clsSensor", "Invalid cable number. Range 1 - 16."
        mudtProps.CableNumber = NewVal
        ValueChanged
        CheckRules
    End If
End Property

Public Property Get SensorNumber() As Long
    SensorNumber = mudtProps.SensorNumber
End Property

Public Property Let SensorNumber(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.SensorNumber <> NewVal Then
        If NewVal < 1 Or NewVal > 16 Then Err.Raise InputErr, "clsSensor", "Invalid sensor number. Range 1 - 16."
        mudtProps.SensorNumber = NewVal
        CheckRules
        ValueChanged
    End If
End Property

Public Property Get Enabled() As Boolean
    Enabled = mudtProps.Enabled
End Property

Public Property Let Enabled(NewVal As Boolean)
    If Not mflgEditing Then Err.Raise 383
    If NewVal <> mudtProps.Enabled Then
        mudtProps.Enabled = NewVal
        ValueChanged
    End If
End Property

Public Property Get Socket() As Integer
    Socket = mudtProps.Socket
End Property

Public Property Let Socket(NewVal As Integer)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.Socket <> NewVal Then
        mudtProps.Socket = NewVal
        ValueChanged
    End If
End Property

Public Property Get MaxAlarm() As Boolean
    MaxAlarm = mudtProps.MaxAlarm
End Property

Public Property Let MaxAlarm(NewVal As Boolean)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.MaxAlarm <> NewVal Then
        mudtProps.MaxAlarm = NewVal
        ValueChanged
    End If
End Property

Public Property Get Offset() As Long
    Offset = mudtProps.Offset
End Property

Public Property Let Offset(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If mudtProps.Offset <> NewVal Then
        If NewVal < -50 Or NewVal > 50 Then Err.Raise InputErr, "clsSensor", "Invalid sensor number. Range -50 to 50."
        mudtProps.Offset = NewVal
        ValueChanged
    End If
End Property

'****************** other properties ***********************

Public Function ValidSensorNumber() As Boolean
'---------------------------------------------------------------------------------------
' Procedure : ValidSensorNumber
' Author    : XPMUser
' Date      : 12/16/2015
' Purpose   : make sure sensor number is only used once per cable
'---------------------------------------------------------------------------------------
'
    Dim SQL As String
    Dim RS As Recordset
    Dim Count As Long
    If mudtProps.BinID = 0 Then
        'ignore check, return valid
        ValidSensorNumber = True
    Else
        SQL = "select * from tblSensors where senCableNumber = " & mudtProps.CableNumber
        SQL = SQL & " and senSensorNumber = " & mudtProps.SensorNumber
        SQL = SQL & " and senStorID = " & mudtProps.BinID
        Set RS = MainDB.OpenRecordset(SQL)
        Do Until RS.EOF
            Count = Count + 1
            RS.MoveNext
        Loop
        ValidSensorNumber = (Count < 2)
        Set RS = Nothing
    End If
End Function

Public Function BinNumber() As String
    Dim Bin As Storage
    If ValueFound(MainDB, "tblStorage", "StorID", mudtProps.BinID) Then
        Set Bin = New Storage
        Bin.Load (mudtProps.BinID)
        BinNumber = Bin.BinNumber
        Set Bin = Nothing
    End If
End Function

Public Function ValidMacAddress(NewMac As String) As Boolean
    Dim SQL As String
    Dim RS As Recordset
    Dim Count As Long
    If NewMac = "" Then
        ValidMacAddress = False
    Else
        SQL = "select * from tblSensors where senMac = '" & NewMac & "'"
        Set RS = MainDB.OpenRecordset(SQL)
        Do Until RS.EOF
            Count = Count + 1
            RS.MoveNext
        Loop
        If mflgNew Then
            'new object, should be none in db
            ValidMacAddress = (Count = 0)
        Else
            'editing an object, should be only 1 in db
            ValidMacAddress = (Count = 1)
        End If
    End If
    Set RS = Nothing
End Function

Public Function BinDescription() As String
    Dim Obj As Storage
    If mudtProps.BinID > 0 Then
        Set Obj = New Storage
        Obj.Load mudtProps.BinID
        BinDescription = Obj.BinNumber & "  " & Obj.Description
    End If
    Set Obj = Nothing
End Function


Public Sub ClearRecords()
    Dim RS As Recordset
    Dim SQL As String
    SQL = "select * from tblRecords where recSenID = " & mudtProps.ID
    Set RS = MainDB.OpenRecordset(SQL)
    Do Until RS.EOF
        RS.Delete
        RS.MoveNext
    Loop
    Set RS = Nothing
End Sub

Public Sub CheckAlarms(MaxTemp As Long)
    Dim SQL As String
    Dim RS As Recordset
    SQL = "select * from tblRecords where recSenID = " & mudtProps.ID
    SQL = SQL & " order by recDate"
    Set RS = MainDB.OpenRecordset(SQL)
    If Not RS.EOF Then
        RS.MoveLast
        'maxtemp, check last record
        mudtProps.MaxAlarm = (NZ(RS!recTemp) > MaxTemp)
    Else
        mudtProps.MaxAlarm = False
    End If
    If IsValid Then Save
    Set RS = Nothing
End Sub

Public Function Name() As String
    Name = BinDescription
    If Name = "" Then
        Name = "ID:  " & mudtProps.RomCode & "  "
    Else
        Name = "Bin: " & Name & "  "
    End If
    Name = Name & "Cable: " & mudtProps.CableNumber & "  "
    Name = Name & "Sensor: " & mudtProps.SensorNumber
End Function

Public Function CalTemp() As Long
Attribute CalTemp.VB_Description = "LastTemp + Offset"
    CalTemp = mudtProps.LastTemp + mudtProps.Offset
End Function
