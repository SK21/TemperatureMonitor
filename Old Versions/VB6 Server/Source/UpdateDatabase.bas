Attribute VB_Name = "UpdateDatabase"
Option Explicit

Private Function CopyData(DBnew As Database, DBold As Database) As Boolean
    Dim TDnew As TableDef
    Dim TDold As TableDef
    Dim TBL As String
    Dim Found As Boolean
    Dim FLDnew As DAO.Field
    Dim FLDold As DAO.Field
    Dim RSnew As Recordset
    Dim RSold As Recordset
    Dim Flds() As String
    Dim fldCount As Long
    Dim F As Long
    CopyData = False
    For Each TDnew In DBnew.TableDefs
        TBL = TDnew.Name
        If Not SysTable(TBL) Then
            'check if table exists in old database
            Found = False
            For Each TDold In DBold.TableDefs
                If TDold.Name = TBL Then
                    Found = True
                    Exit For
                End If
            Next
            If Found Then
                'make a list of compatable fields
                fldCount = 0
                For Each FLDnew In TDnew.Fields
                    Found = False
                    For Each FLDold In TDold.Fields
                        If FLDold.Name = FLDnew.Name And FLDold.Type = FLDnew.Type Then
                            Found = True
                            Exit For
                        End If
                    Next
                    If Found Then
                        fldCount = fldCount + 1
                        ReDim Preserve Flds(fldCount)
                        Flds(fldCount) = FLDnew.Name
                    End If
                Next
                'copy data
                Set RSnew = DBnew.OpenRecordset(TBL)
                Set RSold = DBold.OpenRecordset(TBL)
                Do Until RSold.EOF
                    RSnew.AddNew
                    For F = 1 To fldCount
                        RSnew(Flds(F)) = RSold(Flds(F))
                    Next F
                    RSnew.Update
                    RSold.MoveNext
                Loop
            End If
        End If
    Next
    Set RSnew = Nothing
    Set RSold = Nothing
    CopyData = True
End Function

Public Function CreateRequired(DBnew As Database, CurrentVersion As Long) As Boolean
    Dim RS As Recordset
    Set RS = DBnew.OpenRecordset("tblProps")
    If RS.EOF Then
        RS.AddNew
    Else
        RS.Edit
    End If
    RS("dbType") = AppName
    RS("dbVersion") = CurrentVersion
    RS.Update
    Set RS = Nothing
    CreateRequired = True
End Function

Private Function FreeDBname(Target As String, FilePath As String) As String
    Dim F As String
    Dim C As Integer
    Dim NewName As String
    FreeDBname = ""
    NewName = FilePath & Target & ".mdb"
    Do
        F = Dir(NewName)
        If F = "" Then
            FreeDBname = NewName
        Else
            C = C + 1
            If C > 100 Then Exit Do
            NewName = FilePath & Target & C & ".mdb"
        End If
    Loop Until F = ""
End Function

Private Function SysTable(TableName As String) As Boolean
    SysTable = True
    If Len(TableName) > 3 Then
        If LCase(Left$(TableName, 4)) <> "msys" Then
            SysTable = False
        End If
    End If
End Function

Public Function UpdateDB(DBname As String, DBpassword As String, CurrentVersion As Long) As Boolean
    Dim NwDB As String
    Dim OldDB As String
    Dim DataPath As String
    Dim DBnew As Database
    Dim DBold As Database
    'create temp db of new version
    'to use as source
    UpdateDB = False
    DataPath = Left$(DBname, InStrRev(DBname, "\"))
    OldDB = FreeDBname("OldDB", DataPath)
    NwDB = FreeDBname("NewDB", DataPath)
    If CreatedNewDB(NwDB, DBpassword) Then
        Set DBnew = OpenDatabase(NwDB)
        FileCopy DBname, OldDB
        Set DBold = OpenDatabase(OldDB, False, False, ";pwd=" & DBpassword)
        If CopyData(DBnew, DBold) Then
            CreateRequired DBnew, CurrentVersion
            Set DBold = Nothing
            Set DBnew = Nothing
            UpdateDB = True
            Kill DBname
            Name NwDB As DBname
            Kill OldDB
        Else
            Kill NwDB
            Kill OldDB
        End If
    End If
    Set DBold = Nothing
    Set DBnew = Nothing
End Function

