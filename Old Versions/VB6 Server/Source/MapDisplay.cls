VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "MapDisplay"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit
Private Type objProps
    ID As Long
    RecNum As Long
    Name As String * 50
    Notes As String * 5000
    Units As Long
    Zoom As Long
End Type
Private mudtProps As objProps
Public Property Get ID() As Long
    ID = mudtProps.ID
End Property
Friend Property Let ID(NewVal As Long)
    mudtProps.ID = NewVal
End Property
Public Property Get RecNum() As Long
    RecNum = mudtProps.RecNum
End Property
Friend Property Let RecNum(NewVal As Long)
    mudtProps.RecNum = NewVal
End Property
Public Property Get MapName() As String
    MapName = TR(mudtProps.Name)
End Property
Friend Property Let MapName(NewVal As String)
    mudtProps.Name = NewVal
End Property
Public Property Get MapNotes() As String
    MapNotes = TR(mudtProps.Notes)
End Property
Friend Property Let MapNotes(NewVal As String)
    mudtProps.Notes = NewVal
End Property
Public Property Get Units() As Long
    Units = mudtProps.Units
End Property
Friend Property Let Units(NewVal As Long)
    mudtProps.Units = NewVal
End Property
Public Property Get Zoom() As Long
    Zoom = mudtProps.Zoom
End Property
Friend Property Let Zoom(NewVal As Long)
    mudtProps.Zoom = NewVal
End Property
