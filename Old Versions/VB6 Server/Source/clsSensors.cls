VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsSensors"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private modCol As Collection
Private Sub Class_Initialize()
    Set modCol = New Collection
End Sub
Public Function Count() As Long
    Count = modCol.Count
End Function
Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    'set procedure attributes ID = -4, Hidden checked
    Set NewEnum = modCol.[_NewEnum]
End Function
Public Function Item(ByVal Index As Variant) As clsSensor
Attribute Item.VB_UserMemId = 0
    'set procedure attributes ID = default
    'find or build an object
    'then returnit as a result
    'change the 'Object' to the name of the object
    'ex: Item(ByVal Index As Variant) As Customer
    Set Item = modCol(Index)
End Function
Public Sub Load(Optional SortBy As SensorSort = ssRomCodeAsc, Optional SensorID _
    As Long, Optional BinID As Long, Optional CableNumber As Long)
    'enter optional parameters for this sub
    'ex: Load(Optional Name as string, Optional Phone as string)
    'load data from database
    Dim RS As Recordset
    Dim SQL As String
    Dim SQLSelect As String
    Dim modObject As clsSensor
    SQLSelect = _
        "Select senID,senRecNum,senRomCode,senLastReading,senLastTemp,senStorID"
    SQLSelect = SQLSelect & _
        ",senCableNumber,senSensorNumber,senEnabled,senSocket,senMaxAlarm"
    SQLSelect = SQLSelect & ",StorNum"
    SQLSelect = SQLSelect & _
        " From tblSensors Left Join tblStorage On tblSensors.senStorID = tblStorage.StorID"
    SQL = ""
    If SensorID > 0 Then
        SQL = SQL & " where senID = " & SensorID
    End If
    If BinID > 0 Then
        If SQL = "" Then
            SQL = SQL & " where "
        Else
            SQL = SQL & " and "
        End If
        SQL = SQL & "senStorID = " & BinID
    End If
    If CableNumber > 0 Then
        If SQL = "" Then
            SQL = SQL & " where "
        Else
            SQL = SQL & " and "
        End If
        SQL = SQL & "senCableNumber = " & CableNumber
    End If
    SQL = SQLSelect & SQL
    Select Case SortBy
        Case SensorSort.ssBinNumberAsc
            SQL = SQL & " order by StorNum,senCableNumber,senSensorNumber"
        Case SensorSort.ssLastDateAsc
            SQL = SQL & _
                " order by senLastReading,senStorID,senCableNumber,senSensorNumber"
        Case SensorSort.ssLastTempAsc
            SQL = SQL & _
                " order by senLastTemp,senStorID,senCableNumber,senSensorNumber"
        Case SensorSort.ssRomCodeAsc
            SQL = SQL & " order by senRomCode"
        Case SensorSort.ssBinNumberDes
            SQL = SQL & " order by StorNum Desc,senCableNumber,senSensorNumber"
        Case SensorSort.ssLastDateDes
            SQL = SQL & _
                " order by senLastReading Desc,senStorID,senCableNumber,senSensorNumber"
        Case SensorSort.ssLastTempDes
            SQL = SQL & _
                " order by senLastTemp Desc,senStorID,senCableNumber,senSensorNumber"
        Case SensorSort.ssRomCodeDes
            SQL = SQL & " order by senRomCode Desc"
    End Select
    Set RS = MainDB.OpenRecordset(SQL)
    Do While Not RS.EOF
        Set modObject = New clsSensor
        modObject.Load RS!SenID
        modCol.Add modObject, Format$(modObject.ID) & "ID"
        Set modObject = Nothing
        RS.MoveNext
    Loop
    RS.Close
    Set RS = Nothing
    Set modObject = Nothing
End Sub

Public Function CheckAlarms(MaxTemp As Long) As Boolean
    Dim modObject As clsSensor
    CheckAlarms = False
    For Each modObject In modCol
        modObject.CheckAlarms MaxTemp
        CheckAlarms = CheckAlarms Or modObject.MaxAlarm
    Next
    Set modObject = Nothing
End Function

