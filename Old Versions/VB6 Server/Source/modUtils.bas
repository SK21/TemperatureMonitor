Attribute VB_Name = "modUtils"
Option Explicit
Public Declare Function timeGetTime Lib "winmm.dll" () As Long
Public Declare Function LockWindowUpdate Lib "user32" (ByVal hwndLock As Long) As Long
Private StartTime As Long
Private WaitStart As Long
Private BreakOut As Long

Function BuildArray(ByRef DataString As String, _
    Ds() As String, Optional Seperator As String = "") As Boolean
'---------------------------------------------------------------------------------------
' Procedure : BuildArray
' Author    : dc
' Date      : 14/Dec/2009
' Purpose   : take a string with comma seperated data and
'             create an array. Ex: "A,BC,DEF" would return
'             DS(0)= "A", DS(1)= "BC", DS(2)= "DEF"
'             Returns True if there is at least one item.
'             Can take a different seperator than a comma.
'---------------------------------------------------------------------------------------
    Dim C As Long
    Dim S As String
    Dim F As String
    Dim Sep As String
    If Seperator = "" Then
        Sep = ","
    Else
        Sep = Seperator
    End If
    BuildArray = False
    C = -1
    S = NZ(DataString, True)
    If S <> "" Then
        Do While InStr(S, Sep) <> 0
            C = C + 1
            ReDim Preserve Ds(C)
            F = Left$(S, InStr(S, Sep) - 1)
            Ds(C) = F
            S = Right$(S, Len(S) - Len(F) - 1)
        Loop
        C = C + 1
        ReDim Preserve Ds(C)
        Ds(C) = S
    End If
    If C > -1 Then BuildArray = True
End Function

Function ConvertDate(DT As String, Optional Delimiter As String = "-") As Date
'---------------------------------------------------------------------------------------
' Procedure : ConvertDate
' Author    : XPMUser
' Date      : 4/3/2016
' Purpose   : converts date string in the format mm-dd-yy to a date
'---------------------------------------------------------------------------------------
'
    Dim P() As String
    On Error GoTo ErrExit
    ConvertDate = 0
    P = Split(DT, Delimiter)
    ConvertDate = DateSerial(P(2), P(0), P(1))
ErrExit:
End Function

Public Sub DeleteRecord(DB As Database, Table As String, IDvalue As Variant, _
    IDfield As String, Optional IdIsString As Boolean = False)
    Dim RS As Recordset
    Dim SQL As String
    SQL = "select * from " & Table & " where "
    If IdIsString Then
        SQL = SQL & IDfield & " = '" & TR(IDvalue) & "'"
    Else
        SQL = SQL & IDfield & " = " & IDvalue
    End If
    Set RS = DB.OpenRecordset(SQL)
    If Not RS.EOF Then RS.Delete
    Set RS = Nothing
End Sub

Public Function DependantData(DataList As String) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : DependantData
' Author    : David
' Date      : 14/Feb/2010
' Purpose   : check if any tables depend on the current ID
'             The data list is in the form:
'             "Table,Field,Value,IsString"
'             Each check is entered following in the string
'             and is seperated in code.
'ex: DataList = "tblScaleDetails,SdBagID," & mudtProps.ID & ",false"
'---------------------------------------------------------------------------------------
    Dim FD As Boolean
    Dim AR() As String
    Dim TBL As Long
    Dim IsString As Boolean
    DependantData = True
    FD = False
    If BuildArray(DataList, AR) Then
        For TBL = 0 To UBound(AR) Step 4
            IsString = (LCase(AR(TBL + 3)) = "true")
            If ValueFound(MainDB, AR(TBL + 0), AR(TBL + 1), _
                AR(TBL + 2), IsString) Then
                FD = True
                Exit For
            End If
        Next TBL
    End If
    DependantData = FD
End Function

Public Function ElapsedTime() As Currency
    'in seconds
    ElapsedTime = (timeGetTime - StartTime) / 1000
    StartTime = timeGetTime
End Function

Function LastValue(LvDB As Database, LvTable As String, _
    LvField As String, Optional LvSortField As String = "", _
    Optional LvWhereClause As String = "") As String
    'LvWhereClause example: for Order object returning the
    'Record Number, "OrderType = 1" would return the last
    'Cleaning Order record number
    Dim RS As Recordset
    Dim SQL As String
    Dim IsString As Boolean
    If LvSortField = "" Then LvSortField = LvField
    SQL = "select * from " & LvTable
    If LvWhereClause <> "" Then
        SQL = SQL & " " & LvWhereClause & " "
    End If
    If LvSortField <> "" Then SQL = SQL & " order by " & LvSortField
    Set RS = LvDB.OpenRecordset(SQL)
    If RS(LvField).Type = dbMemo Or RS(LvField).Type = dbText Then
        LastValue = ""
        IsString = True
    Else
        LastValue = 0
        IsString = False
    End If
    With RS
        If Not .EOF Then
            .MoveLast
            LastValue = NZ(RS(LvField), IsString)
        End If
        .Close
    End With
    Set RS = Nothing
End Function

Function NZ(NullValue As Variant, Optional IsString As Boolean = False) As Variant
'---------------------------------------------------------------------------------------
' Procedure : NZ
' Author    : David
' Date      : 19-Apr-09
' Purpose   : replace a null number with 0 or a null string with ""
'---------------------------------------------------------------------------------------
    NZ = NullValue
    If IsNull(NZ) Then
        If IsString Then
            NZ = ""
        Else
            NZ = 0
        End If
    End If
End Function

Public Function ReplaceWith(ByVal Str As String, OldPrt As String, NewPrt As String) As String
'---------------------------------------------------------------------------------------
' Procedure : ReplaceWith
' Author    : David
' Date      : 1/6/2013
' Purpose   : replace all instances of OldPrt with NewPrt in Str and return the new string
'---------------------------------------------------------------------------------------
'
    Dim P As Long
    Do
        P = InStr(Str, OldPrt)
        If P > 0 Then
            Str = Left$(Str, P - 1) & NewPrt & Right$(Str, Len(Str) - (P - 1) - Len(OldPrt))
        End If
    Loop While P > 0
    ReplaceWith = Str
End Function

Function RoundToHalf(Num As Currency) As Currency
    Dim F As Currency
    On Error GoTo ErrHandler
    F = Num - Int(Num)
    If F > 0.75 Then
        RoundToHalf = Int(Num) + 1
    ElseIf F > 0.25 Then
        RoundToHalf = Int(Num) + 0.5
    Else
        RoundToHalf = Int(Num)
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "modUtils", "RoundToHalf", Err.Description
     Resume ErrExit
End Function

Public Function SensorID(Optional MacAddress As String, Optional BinNumber As _
    Long, Optional SensorNumber As Long) As Long
    
    Dim RS As Recordset
    Dim SQL As String
    If MacAddress = "" Then
        SQL = "select * from tblSensors where senStorID = " & BinNumber
        SQL = SQL & " and senNumber = " & SensorNumber
    Else
        SQL = "select * from tblSensors where senMac = '" & TR(MacAddress) & "'"
    End If
    Set RS = MainDB.OpenRecordset(SQL)
    If Not RS.EOF Then SensorID = RS!SenID
    Set RS = Nothing
End Function

Function ShowForm(FormName As String) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : ShowForm
' Author    : David
' Date      : 07/Feb/2010
' Purpose   :
'---------------------------------------------------------------------------------------
    On Error GoTo ErrHandler
    ShowForm = False
    FormName = LCase(FormName)
    Select Case FormName
        Case "frmsaveas"
            Load frmSaveAs
            ShowForm = frmSaveAs.LoadOK
            If ShowForm Then
                frmSaveAs.Show vbModal
            Else
                Unload frmSaveAs
            End If
        Case "frmnewdatabase"
            Load frmNewDatabase
            ShowForm = frmNewDatabase.LoadOK
            If ShowForm Then
                frmNewDatabase.Show vbModal
            Else
                Unload frmNewDatabase
            End If
        Case "frmstart"
            Load frmStart
            ShowForm = frmStart.LoadOK
            If ShowForm Then
                frmStart.Show
            Else
                Unload frmStart
            End If
        Case "frmpassword"
            Load frmPassword
            ShowForm = frmPassword.LoadOK
            If ShowForm Then
                frmPassword.Show vbModal
            Else
                Unload frmPassword
            End If
    End Select
ErrExit:
    Exit Function
ErrHandler:
    AD.DisplayError Err.Number, "Utils", "ShowForm", Err.Description
    Resume ErrExit
End Function

Public Sub ShowProgress(ProgBar As ProgressBar, NewVal As Long)
    'prevent error 380 where new value is greater than max
    On Error GoTo ErrHandler
    If NewVal > 0 Then
        If NewVal > ProgBar.Max Then
            ProgBar.Value = ProgBar.Max
        Else
            ProgBar.Value = NewVal
        End If
    End If
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "modUtils", "ShowProgress", Err.Description
     Resume ErrExit
End Sub

Public Function TimesUp(Optional WaitSecs As Long) As Boolean
    On Error GoTo ErrHandler
    If WaitSecs = 0 Then
        WaitStart = timeGetTime
        TimesUp = False
        BreakOut = 0
    Else
        TimesUp = ((timeGetTime - WaitStart) > (WaitSecs * 1000))
    End If
    'breaks out of function after about 30 seconds to
    'prevent being stuck in a loop
    BreakOut = BreakOut + 1
    If BreakOut > 30000000 Then
        TimesUp = True
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "modUtils", "Timer", Err.Description
     Resume ErrExit
End Function

Function ToAccessDate(DateVal As Date) As String
'---------------------------------------------------------------------------------------
' Procedure : ToAccessDate
' Author    : David
' Date      : 02/01/2013
' Purpose   : Takes a date stored in a date data type and returns a string
'             literal of the date in the format M/D/Y which Access 97 uses
'             for queries.
'---------------------------------------------------------------------------------------
'
    ToAccessDate = "#" & Format(DateVal, "MM/DD/YYYY") & "#"
End Function

Public Function TR(Str As Variant, Optional RemoveCRLF As Boolean = False, _
    Optional RemoveTrailingCRLF As Boolean = True) As String
'---------------------------------------------------------------------------------------
' Procedure : TR
' Date      : 15/Mar/2010
' Purpose   : removes leading and trailing spaces and
' sets a string to "" if it starts with asc(0). This occurs
' when a fixed-length string has been created, but no value
' has been assigned to it yet.
' 17/Jan/2012
' added option to remove CRLF
' Dec 26, 2014
' added option to remove trailing CRLF
'---------------------------------------------------------------------------------------
    Dim P As Long
    Dim CH As Long
    If Len(Str) > 0 Then
        If Asc(Left$(Str, 1)) = 0 Then
            TR = ""
        Else
            TR = Trim$(Str)
        End If
    End If
    If RemoveTrailingCRLF Then
        For P = Len(TR) To 1 Step -1
            CH = Asc(Mid$(TR, P, 1))
            If CH = 10 Or CH = 13 Then
                TR = Left$(TR, Len(TR) - 1)
            Else
                Exit For
            End If
        Next P
    End If
    If RemoveCRLF Then
        TR = ReplaceWith(TR, Chr$(10), " ")
        TR = ReplaceWith(TR, Chr$(13), " ")
    End If
End Function

Function ValueFound(DB As Database, Table As String, Field As String, _
    SearchValue As Variant, Optional IsString As Boolean = False) As Boolean
    Dim RS As Recordset
    Dim SQL As String
    ValueFound = False
    SQL = "select * from " & Table & " where "
    If IsString Then
        SQL = SQL & Field & " = '" & TR(SearchValue) & "'"
    Else
        SQL = SQL & Field & " = " & SearchValue
    End If
    Set RS = DB.OpenRecordset(SQL)
    If Not RS.EOF Then ValueFound = True
    Set RS = Nothing
End Function

Function ZeroDate(DateVal As Variant) As Boolean
'---------------------------------------------------------------------------------------
' Procedure : ZeroDate
' Author    : David
' Date      : 1/2/2013
' Purpose   : returns true if the date portion of the variable is 0
'---------------------------------------------------------------------------------------
'
    Dim D As Date
    If IsDate(DateVal) Then
        D = DateVal
        ZeroDate = (Int(D) = 0)
    Else
        ZeroDate = True
    End If
End Function

