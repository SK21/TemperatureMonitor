VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "StorageDisplay"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private Type StorageProps
    ID As Long
    BinNumber As String * 20
    Description As String * 50
    IsWarehouse As Boolean
    MapID As Long
    XPos As Single
    YPos As Single
    Volume As Currency
    Units As String
    PositionSet As Boolean
    UnitsID As Long
End Type

Private mudtProps As StorageProps
Public Property Get UnitsID() As Long
    UnitsID = mudtProps.UnitsID
End Property
Friend Property Let UnitsID(NewVal As Long)
    mudtProps.UnitsID = NewVal
End Property
Public Property Get PositionSet() As Boolean
    PositionSet = mudtProps.PositionSet
End Property
Friend Property Let PositionSet(NewVal As Boolean)
    mudtProps.PositionSet = NewVal
End Property
Public Property Get IsWarehouse() As Boolean
    IsWarehouse = mudtProps.IsWarehouse
End Property
Friend Property Let IsWarehouse(NewVal As Boolean)
    mudtProps.IsWarehouse = NewVal
End Property
Public Property Get MapID() As Long
    MapID = mudtProps.MapID
End Property
Friend Property Let MapID(NewVal As Long)
    mudtProps.MapID = NewVal
End Property
Public Property Get XPos() As Single
    XPos = mudtProps.XPos
End Property
Friend Property Let XPos(NewVal As Single)
    mudtProps.XPos = NewVal
End Property
Public Property Get YPos() As Single
    YPos = mudtProps.YPos
End Property
Friend Property Let YPos(NewVal As Single)
    mudtProps.YPos = NewVal
End Property
Public Property Get ID() As Long
    ID = mudtProps.ID
End Property
Friend Property Let ID(NewVal As Long)
    mudtProps.ID = NewVal
End Property
Public Property Get BinNumber() As String
    BinNumber = TR(mudtProps.BinNumber)
End Property
Friend Property Let BinNumber(NewVal As String)
    mudtProps.BinNumber = NewVal
End Property
Public Property Get Description() As String
    Description = TR(mudtProps.Description)
End Property
Friend Property Let Description(NewVal As String)
    mudtProps.Description = NewVal
End Property
Public Property Get Volume() As Currency
    Volume = mudtProps.Volume
End Property
Friend Property Let Volume(NewVal As Currency)
    mudtProps.Volume = NewVal
End Property
Public Property Get Units() As String
    Units = TR(mudtProps.Units)
End Property
Friend Property Let Units(NewVal As String)
    mudtProps.Units = NewVal
End Property
