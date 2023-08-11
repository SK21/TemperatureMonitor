Attribute VB_Name = "modMaps"
Option Explicit
Private MapsInitialized As Boolean
Private mDBname As String
Private MapList As ListObject
Private MapInit As Boolean

Private Sub AddSensors(BM As BinMaps)
    Dim Col As clsSensors
    Dim Obj As clsSensor
    On Error GoTo ErrHandler
    BM.ResetBalances
    Set Col = New clsSensors
    Col.Load ssBinNumberAsc
    For Each Obj In Col
        BM.AddSensor Obj.ID, Obj.SensorNumber, Obj.BinID, Obj.LastTemp, Obj.MaxAlarm
    Next
    Set Col = Nothing
    Set Obj = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "modMaps", "AddSensors", Err.Description
     Resume ErrExit
End Sub

Private Sub CheckBins(BM As BinMaps)
'---------------------------------------------------------------------------------------
' Procedure : CheckBins
' Author    : David
' Date      : 4/3/2011
' Purpose   :
' Checks if any bins have been added or deleted
'---------------------------------------------------------------------------------------

    Dim lp As Long
    Dim BinID As Long
    Dim objStorages As Storages
    Dim objStorage As StorageDisplay
    Dim MapID As Long
    On Error GoTo ErrHandler
    Set objStorages = New Storages
    objStorages.Load
    'check for deleted bins
    For lp = 1 To BM.StorCount
        BinID = BM.StorID(lp)
        If Not objStorages.IsItem(BinID) Then
            BM.UnloadStor BinID
        End If
    Next lp
    'check for new bins
    For Each objStorage In objStorages
        With objStorage
            If Not BM.BinLoaded(.ID) Then
                BM.AddStor .ID, .BinNumber, .IsWarehouse, .MapID, .XPos, .YPos, .Volume, .PositionSet
            End If
            'check for bin moved to other map
            MapID = BM.BinMapID(.ID)
            If MapID <> 0 And MapID <> .MapID Then
                BM.UnloadStor .ID
                BM.AddStor .ID, .BinNumber, .IsWarehouse, .MapID, .XPos, .YPos, .Volume, .PositionSet
            End If
        End With
    Next
    Set objStorages = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "modMaps", "CheckBins", Err.Description
    Resume ErrExit
End Sub

Private Sub CheckDatabase(BM As BinMaps)
'---------------------------------------------------------------------------------------
' Procedure : CheckDatabase
' Author    : David
' Date      : 2/13/2012
' Purpose   : checks if database has changed to a different database. If so reset map control.
'---------------------------------------------------------------------------------------
'

    On Error GoTo ErrHandler
    If mDBname <> Prog.DatabaseFullName Then
        mDBname = Prog.DatabaseFullName
        BM.Reset
        MapsInitialized = False
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "modMaps", "CheckDatabase", Err.Description
    Resume ErrExit
End Sub

Private Sub CheckMaps(BM As BinMaps)
'---------------------------------------------------------------------------------------
' Procedure : CheckMaps
' Author    : XPMUser
' Date      : 12/6/2014
' Purpose   : checks if any maps have been added or deleted. Resets if so.
'---------------------------------------------------------------------------------------
    Dim objStors As Storages
    Dim objStor As StorageDisplay
    Dim NewList As ListObject
    Dim DoReset As Boolean
    Dim lp As Long
    On Error GoTo ErrHandler
    Set NewList = New ListObject
    Set objStors = New Storages
    objStors.Load , , , , , True
    For Each objStor In objStors
        'make list of unique map ID's
        NewList.Add objStor.MapID
    Next
    If Not MapInit Then
        'init map list, reset BinMap object
        Set MapList = NewList
        MapInit = True
        DoReset = True
    Else
        'check if each map on new list is on old list
        If MapList.Count <> NewList.Count Then
            'count not same, reset
            DoReset = True
            Set MapList = NewList
        Else
            For lp = 1 To MapList.Count
                If MapList.ID(lp) <> NewList.ID(lp) Then
                    DoReset = True
                    Set MapList = NewList
                    Exit For
                End If
            Next lp
        End If
    End If
    If DoReset Then
        BM.Reset
        MapsInitialized = False
    End If
    Set NewList = Nothing
    Set objStors = Nothing
    Set objStor = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "modMaps", "CheckMaps", Err.Description
     Resume ErrExit
End Sub

Public Sub ConnectMaps(BM As BinMaps, ShowMaps As Boolean)
'---------------------------------------------------------------------------------------
' Procedure : ConnectMaps
' Author    : David
' Date      : 2/13/2012
' Purpose   : show maps. If none in database then control will be hidden.
'---------------------------------------------------------------------------------------
'
    Dim objStorages As Storages
    Dim objStorage As StorageDisplay
    Dim objMap As MapDisplay
    Dim objMaps As Maps
    On Error GoTo ErrHandler
    CheckDatabase BM
    CheckMaps BM
    Set objMaps = New Maps
    objMaps.Load
    BM.Visible = False
    If objMaps.Count > 0 Then
        BM.Visible = ShowMaps
        If ShowMaps Then
            If Not MapsInitialized Then
                'add maps
                For Each objMap In objMaps
                    With objMap
                        BM.AddMap .ID, .MapName, .Units, .Zoom
                    End With
                Next
                'add bins
                Set objStorages = New Storages
                objStorages.Load , , , , , True
                For Each objStorage In objStorages
                    With objStorage
                        BM.AddStor .ID, .BinNumber, .IsWarehouse, .MapID, .XPos, .YPos, .Volume, .PositionSet
                    End With
                Next
                MapsInitialized = True
            End If
            AddSensors BM
            CheckBins BM
            BM.Update
        End If
    End If
    Set objMap = Nothing
    Set objMaps = Nothing
    Set objStorage = Nothing
    Set objStorages = Nothing
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "modMaps", "ConnectMaps", Err.Description
    Resume ErrExit
End Sub

Public Sub SaveMaps(BM As BinMaps)
    Dim objStor As Storage
    Dim objMap As Map
    Dim lp As Long
    Dim ID As Long
    Dim XPos As Single
    Dim YPos As Single
    Dim BinLP As Long
    Dim BinID As Long
    'save map data
    On Error GoTo ErrHandler
    For lp = 1 To BM.MapCount
        ID = BM.MapID(lp)
        Set objMap = New Map
        objMap.Load ID
        objMap.BeginEdit
        objMap.MapZoom = BM.MapZoom(lp)
        objMap.ApplyEdit
        Set objMap = Nothing
    Next lp
    'save bin data
    For BinLP = 1 To BM.StorCount
        BinID = BM.StorID(BinLP)
        If BM.BinLoaded(BinID) Then
            BM.BinLocation BinLP, XPos, YPos
            Set objStor = New Storage
            With objStor
                .Load BinID
                .BeginEdit
                .XPos = XPos
                .YPos = YPos
                .ApplyEdit
            End With
            Set objStor = Nothing
        End If
    Next BinLP
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "modMaps", "SaveMaps", Err.Description
    Resume ErrExit
End Sub

