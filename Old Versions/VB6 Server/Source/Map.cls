VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Map"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Event Valid(IsValid As Boolean)
Event DataEdited()

Private mflgNew As Boolean
Private mflgDeleted As Boolean
Private mflgDirty As Boolean
Private mflgEditing As Boolean
Private WithEvents mobjValid As BrokenRules
Attribute mobjValid.VB_VarHelpID = -1

Private Type MapProps
    ID As Long
    RecNum As Long
    Name As String * 50
    Notes As String * 5000
    Units As Long
    Zoom As Long
End Type

Private mudtProps As MapProps
Private mudtSave As MapProps

Public Sub ApplyEdit()
    Dim DL As String    'dependant data list
    DL = "tblStorage,StorMapID," & mudtProps.ID & ",false"
    If Not mflgEditing Then Err.Raise 445
    If mflgDeleted Then
        If Not mflgNew Then
            'code to delete the object's data goes here
            If DependantData(DL) Then
                Err.Raise InputErr, "", "Dependant data."
            End If
            DeleteRecord MainDB, "tblMaps", mudtProps.ID, "MapID"
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
    mudtProps.RecNum = CCur(LastValue(MainDB, "tblMaps", "MapRecNum")) + 1
    mudtProps.Zoom = 100
    mudtProps.Units = CCur(LastValue(MainDB, "tblMaps", "MapUnits"))
    If mudtProps.Units <> 1 And mudtProps.Units <> 5 Then
        mudtProps.Units = 1
    End If
    'mobjValid.RuleBroken "RuleName", True
    mobjValid.RuleBroken "Map", (Trim(mudtProps.Name) = ""), "A Map name is required."
End Sub

Public Property Get DataChanged() As Boolean
    DataChanged = mflgDirty
End Property

Public Sub Delete()
    If Not mflgEditing Then Err.Raise 445
    mflgDeleted = True
    mflgDirty = True
End Sub

Friend Property Let ID(NewVal As String)
    'this is a 'Friend' property to allow the collection class
    'to work properly
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

Public Sub Load(MapID As Long)
    Dim RS As Recordset
    Dim SQL As String
    If mflgEditing Then Err.Raise 445
    If Not mflgNew Then Err.Raise 445
    SQL = "select * from tblMaps where MapID = " & MapID
    Set RS = MainDB.OpenRecordset(SQL)
    With RS
        If Not .EOF Then
            mudtProps.ID = NZ(!MapID)
            mudtProps.RecNum = NZ(!MapRecNum)
            mudtProps.Name = NZ(!MapName, True)
            mudtProps.Notes = NZ(!MapNotes, True)
            mudtProps.Units = NZ(!MapUnits)
            mudtProps.Zoom = NZ(!MapZoom)
            mobjValid.RuleBroken "Map", (Trim(mudtProps.Name) = ""), "A Map name is required."
            mflgNew = False
        End If
        .Close
    End With
    Set RS = Nothing
End Sub
Public Property Get MapZoom() As Long
    MapZoom = mudtProps.Zoom
End Property
Public Property Let MapZoom(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If NewVal <> mudtProps.Zoom Then
        If NewVal >= 0 And NewVal < 251 Then
            mudtProps.Zoom = NewVal
            ValueChanged
        End If
    End If
End Property
Public Property Get MapUnits() As Long
    MapUnits = mudtProps.Units
End Property
Public Property Let MapUnits(NewVal As Long)
    If Not mflgEditing Then Err.Raise 383
    If NewVal <> mudtProps.Units Then
        If NewVal = 1 Or NewVal = 5 Then
            mudtProps.Units = NewVal
            ValueChanged
        End If
    End If
End Property
Public Property Get MapName() As String
    MapName = TR(mudtProps.Name)
End Property

Public Property Let MapName(NewVal As String)
    If Not mflgEditing Then Err.Raise 383
    If TR(mudtProps.Name) <> TR(NewVal) Then
        If Len(NewVal) > Len(mudtProps.Name) Then
            Err.Raise InputErr, "Map", "Map name too long."
        End If
        mudtProps.Name = TR(NewVal)
        ValueChanged
        mobjValid.RuleBroken "Map", (Trim(mudtProps.Name) = ""), "A Map name is required."
    End If
End Property

Public Property Get MapNotes() As String
    MapNotes = TR(mudtProps.Notes)
End Property

Public Property Let MapNotes(NewVal As String)
    If Not mflgEditing Then Err.Raise 383
    If TR(mudtProps.Notes) <> TR(NewVal) Then
        If Len(NewVal) > Len(mudtProps.Notes) Then
            Err.Raise InputErr, "Map", "Map notes too long."
        End If
        mudtProps.Notes = TR(NewVal)
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
    SQL = "select * from tblMaps where MapID = " & mudtProps.ID
    Set RS = MainDB.OpenRecordset(SQL)
    If mflgNew Then
        RS.AddNew
    Else
        RS.Edit
    End If
    With RS
        !MapRecNum = mudtProps.RecNum
        !MapName = TR(mudtProps.Name)
        !MapNotes = TR(mudtProps.Notes)
        !MapUnits = mudtProps.Units
        !MapZoom = mudtProps.Zoom
        .Update
        'get ID from autoincrement field after file is saved
        If mflgNew Then
            .Bookmark = .LastModified
            mudtProps.ID = !MapID
        End If
        .Close
    End With
    Set RS = Nothing
End Sub

Private Sub ValueChanged()
    mflgDirty = True
    RaiseEvent DataEdited
End Sub

