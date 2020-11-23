VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsPacket"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit
Private Type ObjProps
    SocketID As Integer
    PKtype As PacketType
    Value As String
    RomCode As String
    TimeStamp As Date
    SensorID As Long
End Type

Dim Obj As ObjProps

Public Property Get PKtype() As PacketType
    PKtype = Obj.PKtype
End Property

Public Property Let PKtype(NewVal As PacketType)
    Obj.PKtype = NewVal
End Property

Public Property Get RomCode() As String
    RomCode = Obj.RomCode
End Property

Public Property Let RomCode(NewVal As String)
    Obj.RomCode = NewVal
End Property

Public Property Get TimeStamp() As Date
    TimeStamp = Obj.TimeStamp
End Property

Public Property Let TimeStamp(NewVal As Date)
    Obj.TimeStamp = NewVal
End Property

Public Property Get SocketID() As Integer
    SocketID = Obj.SocketID
End Property

Public Property Let SocketID(NewVal As Integer)
    Obj.SocketID = NewVal
End Property

Public Property Get Value() As String
    Value = Obj.Value
End Property

Public Property Let Value(NewVal As String)
    Obj.Value = NewVal
End Property

Public Property Get SensorID() As Long
    SensorID = Obj.SensorID
End Property

Public Property Let SensorID(NewVal As Long)
    Obj.SensorID = NewVal
End Property

Public Function StreamData() As String
    'returns the packet data as a stream of data the ControlBoxes can recognize
    StreamData = BeginPacket & Obj.PKtype & "|" & Obj.Value & "|" & Obj.RomCode & "|"
End Function
