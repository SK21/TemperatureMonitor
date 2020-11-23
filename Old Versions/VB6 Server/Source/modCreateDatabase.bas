Attribute VB_Name = "modCreateDatabase"
Dim NewDB As Database

Public Function CreatedNewDB(ByVal sDestDBPath As String, ByVal sDestDBPassword As String) As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Tables, Fields & Indexes
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:49 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

CreatedNewDB = False

On Error GoTo Err_Handler

If sDestDBPassword <> "" Then
    Set NewDB = Workspaces(0).CreateDatabase(sDestDBPath, dbLangGeneral & ";pwd=" & sDestDBPassword)
Else
    Set NewDB = Workspaces(0).CreateDatabase(sDestDBPath, dbLangGeneral)
End If

'Now call the functions for each table

Dim b As Boolean

b = CreatedNewTBLtblMaps
If b = False Then
    CreatedNewDB = False
    NewDB.Close
    Set NewDB = Nothing
    Exit Function
End If

b = CreatedNewTBLtblProps
If b = False Then
    CreatedNewDB = False
    NewDB.Close
    Set NewDB = Nothing
    Exit Function
End If

b = CreatedNewTBLtblRecords
If b = False Then
    CreatedNewDB = False
    NewDB.Close
    Set NewDB = Nothing
    Exit Function
End If

b = CreatedNewTBLtblSensors
If b = False Then
    CreatedNewDB = False
    NewDB.Close
    Set NewDB = Nothing
    Exit Function
End If

b = CreatedNewTBLtblStorage
If b = False Then
    CreatedNewDB = False
    NewDB.Close
    Set NewDB = Nothing
    Exit Function
End If

b = CreatedNewTBLtmpRecs
If b = False Then
    CreatedNewDB = False
    NewDB.Close
    Set NewDB = Nothing
    Exit Function
End If

NewDB.Close
Set NewDB = Nothing
CreatedNewDB = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
        MsgBox "Error Creating Copy Database." & vbCr & Err.Number & vbCr & Err.Description
            CreatedNewDB = False
            NewDB.Close

            Set NewDB = Nothing

            Exit Function
    End If
End Function

Private Function CreatedNewTBLtblMaps() As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Table:tblMaps
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:49 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

Dim TempTDef As TableDef
Dim TempField As Field
Dim TempIdx As Index

CreatedNewTBLtblMaps = False

On Error GoTo Err_Handler

Set TempTDef = NewDB.CreateTableDef("tblMaps")
    Set TempField = TempTDef.CreateField("MapID", 4)
        TempField.Attributes = 17
        TempField.Required = False
        TempField.OrdinalPosition = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("MapRecNum", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 1
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("MapName", 10)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 2
        TempField.Size = 100
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("MapNotes", 12)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 3
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("MapUnits", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 4
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("MapZoom", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 5
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "MapID"
        Set TempField = .CreateField("MapID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "MapRecNum"
        Set TempField = .CreateField("MapRecNum")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "PrimaryKey"
        TempIdx.Primary = True
        .Unique = True
        Set TempField = .CreateField("MapID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

NewDB.TableDefs.Append TempTDef
NewDB.TableDefs.Refresh

'Done, Close the objects
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

CreatedNewTBLtblMaps = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
            MsgBox "Error Creating Database Table: tblMaps" & vbCr & Err.Number & vbCr & Err.Description
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

    CreatedNewTBLtblMaps = False
    Exit Function
    End If
End Function
Private Function CreatedNewTBLtblProps() As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Table:tblProps
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:49 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

Dim TempTDef As TableDef
Dim TempField As Field
Dim TempIdx As Index

CreatedNewTBLtblProps = False

On Error GoTo Err_Handler

Set TempTDef = NewDB.CreateTableDef("tblProps")
    Set TempField = TempTDef.CreateField("ID", 4)
        TempField.Attributes = 17
        TempField.Required = False
        TempField.OrdinalPosition = 1
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbType", 10)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 2
        TempField.Size = 50
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbVersion", 10)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 3
        TempField.Size = 50
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbMaxDBsize", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 4
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbPulseTime", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 5
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbMaxTemp", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 6
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbRecordInterval", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 7
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbControlBoxDelay", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 8
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbAutoSave", 1)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 9
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbDays", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 10
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbHrs", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 11
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbLocation", 10)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 12
        TempField.Size = 255
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("dbStartSave", 8)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 13
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "PrimaryKey"
        TempIdx.Primary = True
        .Unique = True
        Set TempField = .CreateField("ID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

NewDB.TableDefs.Append TempTDef
NewDB.TableDefs.Refresh

'Done, Close the objects
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

CreatedNewTBLtblProps = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
            MsgBox "Error Creating Database Table: tblProps" & vbCr & Err.Number & vbCr & Err.Description
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

    CreatedNewTBLtblProps = False
    Exit Function
    End If
End Function
Private Function CreatedNewTBLtblRecords() As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Table:tblRecords
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:49 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

Dim TempTDef As TableDef
Dim TempField As Field
Dim TempIdx As Index

CreatedNewTBLtblRecords = False

On Error GoTo Err_Handler

Set TempTDef = NewDB.CreateTableDef("tblRecords")
    Set TempField = TempTDef.CreateField("recID", 4)
        TempField.Attributes = 17
        TempField.Required = False
        TempField.OrdinalPosition = 1
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("recNum", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 2
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("recSenID", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 3
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("recTemp", 6)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 4
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("recHumidity", 6)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 5
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("recDate", 8)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 6
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "PrimaryKey"
        TempIdx.Primary = True
        .Unique = True
        Set TempField = .CreateField("recID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "recID"
        Set TempField = .CreateField("recID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "recNum"
        Set TempField = .CreateField("recNum")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "senID"
        Set TempField = .CreateField("recSenID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

NewDB.TableDefs.Append TempTDef
NewDB.TableDefs.Refresh

'Done, Close the objects
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

CreatedNewTBLtblRecords = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
            MsgBox "Error Creating Database Table: tblRecords" & vbCr & Err.Number & vbCr & Err.Description
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

    CreatedNewTBLtblRecords = False
    Exit Function
    End If
End Function
Private Function CreatedNewTBLtblSensors() As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Table:tblSensors
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:50 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

Dim TempTDef As TableDef
Dim TempField As Field
Dim TempIdx As Index

CreatedNewTBLtblSensors = False

On Error GoTo Err_Handler

Set TempTDef = NewDB.CreateTableDef("tblSensors")
    Set TempField = TempTDef.CreateField("senID", 4)
        TempField.Attributes = 17
        TempField.Required = False
        TempField.OrdinalPosition = 1
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senRecNum", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 2
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senRomCode", 10)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 3
        TempField.Size = 50
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senLastReading", 8)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 4
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senLastTemp", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 5
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senStorID", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 6
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senCableNumber", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 7
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senSensorNumber", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 8
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senEnabled", 1)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 9
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senSocket", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 10
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senMaxAlarm", 1)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 11
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senOffset", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 12
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "ID"
        Set TempField = .CreateField("senID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "PrimaryKey"
        TempIdx.Primary = True
        .Unique = True
        Set TempField = .CreateField("senID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "senCableNumber"
        Set TempField = .CreateField("senCableNumber")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "senRecNum"
        Set TempField = .CreateField("senRecNum")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "senRomCode"
        Set TempField = .CreateField("senRomCode")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "senStorID"
        Set TempField = .CreateField("senStorID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

NewDB.TableDefs.Append TempTDef
NewDB.TableDefs.Refresh

'Done, Close the objects
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

CreatedNewTBLtblSensors = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
            MsgBox "Error Creating Database Table: tblSensors" & vbCr & Err.Number & vbCr & Err.Description
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

    CreatedNewTBLtblSensors = False
    Exit Function
    End If
End Function
Private Function CreatedNewTBLtblStorage() As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Table:tblStorage
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:50 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

Dim TempTDef As TableDef
Dim TempField As Field
Dim TempIdx As Index

CreatedNewTBLtblStorage = False

On Error GoTo Err_Handler

Set TempTDef = NewDB.CreateTableDef("tblStorage")
    Set TempField = TempTDef.CreateField("StorId", 4)
        TempField.Attributes = 17
        TempField.Required = False
        TempField.OrdinalPosition = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorRecNum", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 1
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorNum", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 2
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorDescription", 10)
        TempField.Attributes = 2
        TempField.Required = False
        TempField.OrdinalPosition = 3
        TempField.Size = 50
        TempField.AllowZeroLength = True
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorIsWarehouse", 1)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 4
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorMapID", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 5
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorXPos", 6)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 6
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorYPos", 6)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 7
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorVolume", 6)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 8
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorUnits", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 9
        TempField.DefaultValue = 0
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("StorPositionSet", 1)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 10
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "PrimaryKey"
        TempIdx.Primary = True
        .Unique = True
        Set TempField = .CreateField("StorId")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "storId"
        Set TempField = .CreateField("StorId")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "StorMapID"
        Set TempField = .CreateField("StorMapID")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "StorNum"
        Set TempField = .CreateField("StorNum")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

    Set TempIdx = TempTDef.CreateIndex
    With TempIdx
        .Name = "StorRecNum"
        Set TempField = .CreateField("StorRecNum")
        .Fields.Append TempField
    End With
    TempTDef.Indexes.Append TempIdx
    TempTDef.Indexes.Refresh

NewDB.TableDefs.Append TempTDef
NewDB.TableDefs.Refresh

'Done, Close the objects
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

CreatedNewTBLtblStorage = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
            MsgBox "Error Creating Database Table: tblStorage" & vbCr & Err.Number & vbCr & Err.Description
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

    CreatedNewTBLtblStorage = False
    Exit Function
    End If
End Function
Private Function CreatedNewTBLtmpRecs() As Boolean

'''''''''''''''''''''''''''''''''''''''
'Database Table:tmpRecs
'Copied from: C:\Documents and Settings\All Users\Documents\TemperatureMonitor\TMbase.mdb
'On: 2/4/2017 10:16:50 PM
'Copied via rcSmithDBCopy ver:1.6
'REQUIRES:  Reference to MS DAO in VB project
'NOTE NOTE NOTE:  Code does *not* check Validity of Destination Path!!
'''''''''''''''''''''''''''''''''''''''

Dim TempTDef As TableDef
Dim TempField As Field
Dim TempIdx As Index

CreatedNewTBLtmpRecs = False

On Error GoTo Err_Handler

Set TempTDef = NewDB.CreateTableDef("tmpRecs")
    Set TempField = TempTDef.CreateField("recDate", 8)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 1
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("senSensorNumber", 4)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 2
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

    Set TempField = TempTDef.CreateField("recTemp", 6)
        TempField.Attributes = 1
        TempField.Required = False
        TempField.OrdinalPosition = 3
    TempTDef.Fields.Append TempField
    TempTDef.Fields.Refresh

NewDB.TableDefs.Append TempTDef
NewDB.TableDefs.Refresh

'Done, Close the objects
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

CreatedNewTBLtmpRecs = True

Exit Function

Err_Handler:
    If Err.Number <> 0 Then
        'Alert & Close Objects, could be altered to Raise the error
            MsgBox "Error Creating Database Table: tmpRecs" & vbCr & Err.Number & vbCr & Err.Description
    Set TempTDef = Nothing
    Set TempField = Nothing
    Set TempIdx = Nothing

    CreatedNewTBLtmpRecs = False
    Exit Function
    End If
End Function

