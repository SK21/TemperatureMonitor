VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Storages"
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
Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = mcolDisplay.[_NewEnum]
End Function
Public Function Item(ByVal Index As Variant) As StorageDisplay
Attribute Item.VB_UserMemId = 0
    'find or build an object
    'then returnit as a result
    'change the 'Object' to the name of the object
    'ex: Item(ByVal Index As Variant) As Customer
    Set Item = mcolDisplay(Index)
End Function
Public Function IsItem(StorID As Long) As Boolean
    Dim ID As String
    Dim X As Long
    On Error GoTo ErrExit
    ID = Format$(StorID) & "ID"
    X = Item(ID).ID
    IsItem = True
    Exit Function
ErrExit:
    IsItem = False
End Function
Public Sub Load(Optional StorageNumber As Long, _
    Optional RecNum As Long, Optional LotNum As Long, Optional StorageType As GMStorageTypes, _
    Optional ExcludeScaleID As Long, Optional OrderByMap As Boolean)
    'enter optional parameters for this sub
    'ex: Load(Optional Name as string, Optional Phone as string)
    'load data from database
    'if lotnum is provided then the collection contains
    'only those locations that contain the lot and the
    'tonnes in each location.
    'StorageType: 0 - all, 1 - bins, 2 - warehouses
    'ExcludeScaleID is used together with LotNum for getting a list of
    'locations for the LotNum where the balance is not 0. The ScaleID is
    'excluded so lots with a 0 balance caused by the Scale Receipt are included.
    Dim RS As Recordset
    Dim SQL As String
    Dim objDisplay As StorageDisplay
    If LotNum = 0 Then
        SQL = ""
        If StorageNumber > 0 Then
            SQL = " where StorNum =" & StorageNumber
        End If
        If RecNum > 0 Then
            If SQL = "" Then
                SQL = " where"
            Else
                SQL = SQL & " and"
            End If
            SQL = SQL & " StorRecNum = " & RecNum
        End If
        Select Case StorageType
            Case 1
                'bin
                If SQL = "" Then
                    SQL = " where"
                Else
                    SQL = SQL & " and"
                End If
                SQL = SQL & " StorIsWarehouse = 0"
            Case 2
                'warehouse
                If SQL = "" Then
                    SQL = " where"
                Else
                    SQL = SQL & " and"
                End If
                SQL = SQL & " StorIsWarehouse = -1"
        End Select
        SQL = "select * from tblStorage" & SQL
        If OrderByMap Then
            SQL = SQL & " Order By StorMapID,StorRecNum"
        Else
            SQL = SQL & " order by StorNum"
        End If
        Set RS = MainDB.OpenRecordset(SQL)
        Do While Not RS.EOF
            Set objDisplay = New StorageDisplay
            With objDisplay
                .Description = NZ(RS!StorDescription, True)
                .ID = NZ(RS!StorID)
                .BinNumber = NZ(RS!StorNum)
                .Volume = NZ(RS!StorVolume)
                If NZ(RS!StorUnits) = 1 Then
                    .Units = "Bushels"
                    .UnitsID = 1
                Else
                    .Units = "Tonnes"
                    .UnitsID = 5
                End If
                .IsWarehouse = NZ(RS!StorIsWarehouse)
                .XPos = NZ(RS!StorXPos)
                .YPos = NZ(RS!StorYPos)
                .MapID = NZ(RS!StorMapID)
                .PositionSet = NZ(RS!StorPositionSet)
            End With
            mcolDisplay.Add objDisplay, Format$(objDisplay.ID) & "ID"
            Set objDisplay = Nothing
            RS.MoveNext
        Loop
        RS.Close
    End If
    Set RS = Nothing
End Sub

Public Function DropList() As String
'---------------------------------------------------------------------------------------
' Procedure : DropList
' Author    : David
' Date      : 3/26/2016
' Purpose   : builds a dropdown list for the dcGridControl
'             format is ID^Description|
'---------------------------------------------------------------------------------------
'
    Dim Obj As StorageDisplay
    DropList = ""
    For Each Obj In mcolDisplay
        DropList = DropList & Obj.ID & "^" & Obj.BinNumber & "  " & Obj.Description & "|"
    Next
    Set Obj = Nothing
End Function
