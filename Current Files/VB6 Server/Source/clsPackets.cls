VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsPackets"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit

Private mCol As Collection

Public Function Add(SocketID As Integer, PKtype As PacketType, Value As String, _
    RomCode As String, Optional TimeStamp As Date, Optional sKey As String, _
    Optional SensorID As Long) As clsPacket
    Dim objNew As clsPacket
    Set objNew = New clsPacket
    With objNew
        .SocketID = SocketID
        .PKtype = PKtype
        .Value = Value
        .RomCode = RomCode
        .TimeStamp = TimeStamp
        .SensorID = SensorID
    End With
    If Len(sKey) = 0 Then
        mCol.Add objNew
    Else
        mCol.Add objNew, sKey
    End If
    Set Add = objNew
    Set objNew = Nothing
End Function

Private Sub Class_Initialize()
    Set mCol = New Collection
End Sub

Private Sub Class_Terminate()
    Set mCol = Nothing
End Sub

Public Function Count() As Long
    Count = mCol.Count
End Function

Public Function Item(ByVal Index As Variant) As clsPacket
Attribute Item.VB_UserMemId = 0
    Set Item = mCol(Index)
End Function

Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = mCol.[_NewEnum]
End Function

Public Sub Remove(vntIndexKey As Variant)
    mCol.Remove vntIndexKey
End Sub

Public Sub StreamAdd(StreamData As String, Sckt As Integer)
    'add packets to collection from stream of data received over socket from ControlBoxes
    Dim Chunks() As String
    Dim Count As Long
    Dim C As Long
    Dim SC() As String  'sub-chunks
    Chunks = Split(StreamData, BeginPacket)
    Count = UBound(Chunks)
    For C = 0 To Count
        SC = Split(Chunks(C), "|")
        'check for correctly formed packet
        If UBound(SC) = 3 Then Add Sckt, Val(SC(0)), SC(1), TR(SC(2))
    Next C
End Sub
