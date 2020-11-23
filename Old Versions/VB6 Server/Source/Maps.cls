VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Maps"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private mcolDisplay As Collection
Private Sub Class_Initialize()
    Set mcolDisplay = New Collection
End Sub
Public Function Count() As Long
    Count = mcolDisplay.Count
End Function
Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = mcolDisplay.[_NewEnum]
End Function
Public Function Item(ByVal Index As Variant) As MapDisplay
Attribute Item.VB_UserMemId = 0
    'find or build an object
    'then returnit as a result
    'change the 'Object' to the name of the object
    'ex: Item(ByVal Index As Variant) As Customer
    Set Item = mcolDisplay(Index)
End Function
Public Sub Load(Optional RecNum As Long)
    'enter optional parameters for this sub
    'ex: Load(Optional Name as string, Optional Phone as string)
    'load data from database
    Dim RS As Recordset
    Dim SQL As String
    Dim objDisplay As MapDisplay
    If RecNum > 0 Then
        If SQL = "" Then
            SQL = " where"
        Else
            SQL = SQL & " and"
        End If
        SQL = SQL & " MapRecNum = " & RecNum
    End If
    SQL = "select * from tblMaps" & SQL
    Set RS = MainDB.OpenRecordset(SQL)
    Do While Not RS.EOF
        Set objDisplay = New MapDisplay
        With objDisplay
            .ID = NZ(RS!MapID)
            .RecNum = NZ(RS!MapRecNum)
            .MapName = NZ(RS!MapName, True)
            .MapNotes = NZ(RS!MapNotes, True)
            .Units = NZ(RS!MapUnits)
            .Zoom = NZ(RS!MapZoom)
        End With
        mcolDisplay.Add objDisplay, Format$(objDisplay.ID) & "ID"
        Set objDisplay = Nothing
        RS.MoveNext
    Loop
    RS.Close
    Set RS = Nothing
End Sub



