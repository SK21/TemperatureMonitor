VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Storage"
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

Private Type StorageProps
    ID As Long
    RecNum As Long
    BinNumber As Long
    Description As String * 50
    IsWarehouse As Boolean
    MapID As Long
    XPos As Single
    YPos As Single
    Volume As Currency
    UnitsID As Long
    PositionSet As Boolean
End Type

Private mudtProps As StorageProps
Private mudtSave As StorageProps
Public Sub ApplyEdit()
    Dim DL As String
    DL = "tblSensors,senStorID," & mudtProps.ID & ",false"
    If Not mflgEditing Then Err.Raise 445
    If mflgDeleted Then
        If Not mflgNew Then
            'code to delete the object's data goes here
            DL = "tblSensors,senStorID," & mudtProps.ID & ",false"
            If DependantData(DL) Then
                Err.Raise InputErr, "", "Dependant data."
            End If
            DeleteRecord MainDB, "tblStorage", mudtProps.ID, "StorID"
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
    'if we know any rules are broken on startup
    'then add code here to initialize the list
    'of broken rules
    'Enter Default values for new records
    mudtProps.RecNum = CCur(LastValue(MainDB, "tblStorage", "StorRecNum")) + 1
    mudtProps.BinNumber = CCur(LastValue(MainDB, "tblStorage", "StorNum", "StorRecNum")) + 1
    mudtProps.MapID = CLng(LastValue(MainDB, "tblStorage", "StorMapID", "StorRecNum"))
    mudtProps.UnitsID = CLng(LastValue(MainDB, "tblStorage", "StorUnits", "StorRecNum"))
    If mudtProps.UnitsID = 0 Then mudtProps.UnitsID = 1
    'mobjValid.RuleBroken "RuleName", True
    mobjValid.RuleBroken "Description", True, "No Description."
    mobjValid.RuleBroken "UnitsID", (mudtProps.UnitsID = 0), "Invalid Units."
End Sub
Public Property Get PositionSet() As Boolean
    PositionSet = mudtProps.PositionSet
End Property
Public Property Get DataChanged() As Boolean
    DataChanged = mflgDirty
End Property

Public Sub Delete()
    If Not mflgEditing Then Err.Raise 445
    mflgDeleted = True
    mflgDirty = True
End Sub

Public Property Get Description() As String
    Description = TR(mudtProps.Description)
End Property

Public Property Let Description(NewVal As String)
    If Not mflgEditing Then Err.Raise 383
    If TR(mudtProps.Description) <> TR(NewVal) Then
        If Len(NewVal) > Len(mudtProps.Description) Then
            Err.Raise InputErr, "", "Description too long."
        End If
        mudtProps.Description = TR(NewVal)
        mobjValid.RuleBroken "Description", (Len(TR(NewVal)) = 0)
        ValueChanged
    End If
End Property

Friend Property Let ID(NewVal As String)
    mudtProps.ID = CCur(NewVal)
End Property

Public Property Get ID() As String
    ID = mudtProps.ID
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

Public Property Get BinNumber() As String
    BinNumber = TR(mudtProps.BinNumber)
End Property

Public Property Let BinNumber(NewVal As String)
    'this is a number 1 - 256
    Dim V As Long
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Storage", ErrNum
    End If
    V = CLng(NewVal)
    If mudtProps.BinNumber <> V Then
        If V < 1 Or V > 256 Then
            Err.Raise InputErr, "Storage", "Invalid bin number. Range 1 - 256."
        End If
        'check if unique
        If ValueFound(MainDB, "tblStorage", "StorNum", V) Then
            Err.Raise InputErr, "", "Bin number repeated."
        End If
        mudtProps.BinNumber = V
        ValueChanged
    End If
End Property

Public Sub Load(StorageID As Long)
    Dim RS As Recordset
    Dim SQL As String
    If mflgEditing Then Err.Raise 445
    If Not mflgNew Then Err.Raise 445
    SQL = "select * from tblStorage where StorID = " & StorageID
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If Not .EOF Then
            mudtProps.ID = NZ(!StorID)
            mudtProps.RecNum = NZ(!StorRecNum)
            mudtProps.BinNumber = NZ(!StorNum)
            mudtProps.Description = NZ(!StorDescription, True)
            mudtProps.IsWarehouse = NZ(!StorIsWarehouse)
            mudtProps.MapID = NZ(!StorMapID)
            mudtProps.XPos = NZ(!StorXPos)
            mudtProps.YPos = NZ(!StorYPos)
            mudtProps.Volume = NZ(!StorVolume)
            mudtProps.UnitsID = NZ(!StorUnits)
            mudtProps.PositionSet = NZ(!StorPositionSet)
            mobjValid.RuleBroken "Description", False
            mobjValid.RuleBroken "UnitsID", (mudtProps.UnitsID = 0), "Invalid Units."
            mflgNew = False
        End If
        .Close
    End With
    Set RS = Nothing
End Sub

Public Property Get MapID() As Long
    MapID = mudtProps.MapID
End Property

Public Property Let MapID(NewVal As Long)
    Dim NV As Long
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Storage", ErrNum
    End If
    NV = CCur(NewVal)
    If mudtProps.MapID <> NV Then
        If NV <> 0 Then
            If Not ValueFound(MainDB, "tblMaps", "MapID", NV) Then
                Err.Raise InputErr, "", "Map not found."
            End If
        End If
        mudtProps.MapID = NV
        ValueChanged
    End If
End Property

Private Sub mobjValid_BrokenRule()
    RaiseEvent Valid(False)
End Sub

Private Sub mobjValid_NoBrokenRules()
    RaiseEvent Valid(True)
End Sub

Friend Property Let RecNum(NewVal As String)
    mudtProps.RecNum = CCur(NewVal)
End Property

Public Property Get RecNum() As String
    RecNum = mudtProps.RecNum
End Property

Private Sub Save()
    Dim SQL As String
    Dim RS As Recordset
    SQL = "select * from tblStorage where StorID = " & mudtProps.ID
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If mflgNew Then
            .AddNew
        Else
            .Edit
        End If
        !StorRecNum = mudtProps.RecNum
        !StorNum = TR(mudtProps.BinNumber)
        !StorDescription = TR(mudtProps.Description)
        !StorIsWarehouse = mudtProps.IsWarehouse
        !StorMapID = mudtProps.MapID
        !StorXPos = mudtProps.XPos
        !StorYPos = mudtProps.YPos
        !StorVolume = mudtProps.Volume
        !StorUnits = mudtProps.UnitsID
        !StorPositionSet = mudtProps.PositionSet
        .Update
        'get ID from autoincrement field after file is saved
        If mflgNew Then
            .Bookmark = .LastModified
            mudtProps.ID = !StorID
        End If
        .Close
    End With
    Set RS = Nothing
End Sub

Private Sub ValueChanged()
    mflgDirty = True
    RaiseEvent DataEdited
End Sub

Public Property Get XPos() As Currency
    XPos = mudtProps.XPos
End Property

Public Property Let XPos(NewVal As Currency)
    Dim NV As Currency
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Storage", ErrNum
    End If
    NV = CCur(NewVal)
    If mudtProps.XPos <> NV Then
        mudtProps.XPos = NV
        mudtProps.PositionSet = True
        ValueChanged
    End If
End Property

Public Property Get YPos() As Currency
    YPos = mudtProps.YPos
End Property

Public Property Let YPos(NewVal As Currency)
    Dim NV As Currency
    If Not mflgEditing Then Err.Raise 383
    If Not IsNumeric(NewVal) Then
        Err.Raise InputErr, "Storage", ErrNum
    End If
    NV = CCur(NewVal)
    If mudtProps.YPos <> NV Then
        mudtProps.YPos = NV
        mudtProps.PositionSet = True
        ValueChanged
    End If
End Property

