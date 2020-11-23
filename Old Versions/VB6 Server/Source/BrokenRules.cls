VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "BrokenRules"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit
Event BrokenRule()
Event NoBrokenRules()
Private BrokenCount As Long
Private BrokenName() As String
Private BrokenDescription() As String
Public Property Get RuleName(RuleNum As Long) As String
    If RuleNum < 1 Or RuleNum > BrokenCount Then Exit Property
    RuleName = BrokenName(RuleNum)
End Property
Public Sub RuleBroken(Rule As String, _
    IsBroken As Boolean, Optional Description As String)
    On Error GoTo HandleError
    If IsBroken Then
        Add Rule, Description
    Else
        Remove Rule
    End If
HandleError:

End Sub
Public Property Get Count() As Integer
    Count = BrokenCount
End Property
Private Sub Remove(Rule As String)
    Dim R As Long
    Dim CR As Long
    For R = 1 To BrokenCount
        If LCase(BrokenName(R)) = LCase(Rule) Then
            BrokenCount = BrokenCount - 1
            For CR = R To BrokenCount
                BrokenName(CR) = BrokenName(CR + 1)
                BrokenDescription(CR) = BrokenDescription(CR + 1)
            Next CR
            Exit For
        End If
    Next R
    If Count = 0 Then RaiseEvent NoBrokenRules
End Sub
Private Sub Add(Rule As String, Optional Description As String)
    Dim R As Long
    Dim RuleFound As Boolean
    Dim FoundRule As Long
    For R = 1 To BrokenCount
        If LCase(BrokenName(R)) = LCase(Rule) Then
            RuleFound = True
            FoundRule = R
            Exit For
        End If
    Next R
    If Not RuleFound Then
        BrokenCount = BrokenCount + 1
        ReDim Preserve BrokenName(BrokenCount)
        ReDim Preserve BrokenDescription(BrokenCount)
        BrokenName(BrokenCount) = Rule
        BrokenDescription(BrokenCount) = Description
        RaiseEvent BrokenRule
    Else
        'update description
        BrokenDescription(FoundRule) = Description
    End If
End Sub
Public Property Get RuleDescription(RuleNum As Long) As String
    If RuleNum < 1 Or RuleNum > BrokenCount Then Exit Property
    RuleDescription = BrokenDescription(RuleNum)
End Property
