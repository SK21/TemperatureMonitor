VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsRecord"
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
    SenID As Long
    Temp As Currency
    Humidity As Currency
    recDate As Date
End Type

Private mudtProps As ObjProps
Private mudtSave As ObjProps

Public Sub ApplyEdit()
    Dim DL As String
    If Not mflgEditing Then Err.Raise 445
    If mflgDeleted Then
        If Not mflgNew Then
            'code to delete the object's data goes here
'            DL = "tblLots,LotCropID," & mudtProps.ID & ",false"
'            DL = DL & ",tblSettings,SetCropID," & mudtProps.ID & ",false"
            If DependantData(DL) Then
                Err.Raise InputErr, "Record", "Dependant data."
            End If
            DeleteRecord MainDB, "tblRecords", mudtProps.ID, "recID"
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
    mudtProps.RecNum = CCur(LastValue(MainDB, "tblRecords", "recNum", "recNum")) + 1
    mudtProps.recDate = Now
    
    'initialize the list of broken rules
    CheckRules
End Sub
Private Sub CheckRules()
    mobjValid.RuleBroken "Date", Not IsDate(mudtProps.recDate), "Invalid date."
    mobjValid.RuleBroken "Sensor", Not ValueFound(MainDB, "tblSensors", "senID", mudtProps.SenID), "Invalid Sensor."
    mobjValid.RuleBroken "Temperature", (mudtProps.Temp < -50 Or mudtProps.Temp > 50), "Invalid temperature."
    mobjValid.RuleBroken "Humidity", (mudtProps.Humidity < 0 Or mudtProps.Humidity > 100), "Invalid humidity."
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
    SQL = "select * from tblRecords where recID = " & mudtProps.ID
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If mflgNew Then
            .AddNew
        Else
            .Edit
        End If
        !recSenID = mudtProps.SenID
        !recTemp = mudtProps.Temp
        !RecNum = mudtProps.RecNum
        !recHumidity = mudtProps.Humidity
        !recDate = mudtProps.recDate
        .Update
        'get ID from autoincrement field after file is saved
        If mflgNew Then
            .Bookmark = .LastModified
            mudtProps.ID = !recID
        End If
        .Close
    End With
    Set RS = Nothing
End Sub

Public Sub Load(recID As Long)
    Dim RS As Recordset
    Dim SQL As String
    If mflgEditing Then Err.Raise 445
    If Not mflgNew Then Err.Raise 445
    SQL = "select * from tblRecords where recID = " & recID
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If Not .EOF Then
            mudtProps.ID = NZ(!recID)
            mudtProps.RecNum = NZ(!RecNum)
            mudtProps.SenID = NZ(!recSenID)
            mudtProps.Temp = NZ(!recTemp)
            mudtProps.Humidity = NZ(!recHumidity)
            mudtProps.recDate = NZ(!recDate)
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

Public Property Get SensorID() As String
    SensorID = mudtProps.SenID
End Property

Public Property Let SensorID(NewVal As String)
    Dim NV As Currency
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Record", "A number is required."
    End If
    NV = CCur(NewVal)
    If mudtProps.SenID <> NV Then
        If Not ValueFound(MainDB, "tblSensors", "senID", NV) Then
            Err.Raise InputErr, "Record", "Invalid Sensor."
        End If
        mudtProps.SenID = NV
        CheckRules
        ValueChanged
    End If
End Property

Public Property Get Temperature() As String
    Temperature = mudtProps.Temp
End Property

Public Property Let Temperature(NewVal As String)
    Dim NV As Currency
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Record", "A number is required."
    End If
    NV = CCur(NewVal)
    If mudtProps.Temp <> NV Then
        If NV < -50 Or NV > 50 Then
            Err.Raise InputErr, "Record", "Invalid Temperature."
        End If
        mudtProps.Temp = NV
        CheckRules
        ValueChanged
    End If
End Property

Public Property Get Humidity() As String
    Humidity = mudtProps.Temp
End Property

Public Property Let Humidity(NewVal As String)
    Dim NV As Currency
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Record", "A number is required."
    End If
    NV = CCur(NewVal)
    If mudtProps.Humidity <> NV Then
        If NV < 0 Or NV > 100 Then
            Err.Raise InputErr, "Record", "Invalid Humidity."
        End If
        mudtProps.Humidity = NV
        CheckRules
        ValueChanged
    End If
End Property

Public Property Get recDate() As Date
    recDate = mudtProps.recDate
End Property

Public Property Let recDate(NewVal As Date)
    If Not mflgEditing Then Err.Raise 383
    If ZeroDate(NewVal) Then Err.Raise InputErr, "Record", ErrDate
    If mudtProps.recDate <> NewVal Then
       mudtProps.recDate = NewVal
       CheckRules
       ValueChanged
    End If
End Property

'****************** other properties ***********************
