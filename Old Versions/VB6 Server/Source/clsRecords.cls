VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsRecords"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit

Private mcolDisplay As Collection

Private Sub Class_Initialize()
    Set mcolDisplay = New Collection
End Sub

Public Function Count() As Long
    Count = mcolDisplay.Count
End Function

Public Function Item(ByVal Index As Variant) As clsRecord
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
    Dim objDisplay As clsRecord
    If RecNum > 0 Then
        If SQL = "" Then
            SQL = " where"
        Else
            SQL = SQL & " and"
        End If
        SQL = SQL & " recNum = " & RecNum
    End If
    SQL = "select * from tblRecords" & SQL
    SQL = SQL & " order by recNum"
    Set RS = MainDB.OpenRecordset(SQL)
    Do While Not RS.EOF
        Set objDisplay = New clsRecord
        objDisplay.Load RS!recID
        mcolDisplay.Add objDisplay, Format$(objDisplay.ID) & "ID"
        Set objDisplay = Nothing
        RS.MoveNext
    Loop
    RS.Close
    Set RS = Nothing
End Sub

Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = mcolDisplay.[_NewEnum]
End Function

