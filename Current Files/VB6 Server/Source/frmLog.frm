VERSION 5.00
Object = "{3B7C8863-D78F-101B-B9B5-04021C009402}#1.2#0"; "RichTX32.ocx"
Begin VB.Form frmLog 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "TemperatureMonitor Log"
   ClientHeight    =   11775
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   8850
   Icon            =   "frmLog.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   11775
   ScaleWidth      =   8850
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton butClear 
      Caption         =   "Clear"
      Height          =   375
      Left            =   5760
      TabIndex        =   3
      Top             =   11280
      Width           =   855
   End
   Begin RichTextLib.RichTextBox RTB 
      Height          =   10935
      Left            =   120
      TabIndex        =   2
      Top             =   120
      Width           =   8535
      _ExtentX        =   15055
      _ExtentY        =   19288
      _Version        =   393217
      ReadOnly        =   -1  'True
      ScrollBars      =   3
      TextRTF         =   $"frmLog.frx":0442
   End
   Begin VB.CommandButton butPrint 
      Caption         =   "Print"
      Height          =   375
      Left            =   6840
      TabIndex        =   1
      Top             =   11280
      Width           =   855
   End
   Begin VB.CommandButton butSave 
      Caption         =   "OK"
      Height          =   375
      Left            =   7920
      TabIndex        =   0
      Top             =   11280
      Width           =   855
   End
End
Attribute VB_Name = "frmLog"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Private Sub butClear_Click()
    On Error GoTo ErrHandler
    RTB.Text = ""
    AD.EraseLog
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    AD.DisplayError Err.Number, "frmLog", "butClear_Click", Err.Description
    Resume ErrExit
End Sub

Private Sub butPrint_Click()
    On Error GoTo ErrHandler
    frmSelectPrinter.Show vbModal
    If Not frmSelectPrinter.Cancel Then
        PrintRTF RTB, AnInch / 2, AnInch / 2, AnInch / 2, AnInch / 2
    End If
    Unload frmSelectPrinter
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmLog", "DoPrint", Err.Description
     Resume ErrExit
End Sub

Private Sub butSave_Click()
    Unload Me
End Sub

Private Sub Form_Load()
    Dim Ln As String
    Dim Mes As String
    Dim ER As Long
    On Error GoTo ErrHandler
    AD.LoadFormData Me
    Open AD.Folders(AppCommon) & "\Log.txt" For Input As #1
    Do While Not EOF(1)
        Input #1, Ln
        Ln = Ln & vbNewLine
        Mes = Mes & Ln
    Loop
    Close #1
    RTB.TextRTF = Mes
    With RTB
        .SelStart = Len(.Text)
        .SelLength = 0
    End With
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
    ER = Err.Number
    Select Case ER
        Case 53
            'no file
            Call MsgBox("No data.", vbInformation, App.Title)
            Unload Me
        Case Else
            AD.DisplayError Err.Number, "frmLog", "Load", Err.Description
    End Select
    Resume ErrExit
End Sub

Private Sub Form_Unload(Cancel As Integer)
    AD.SaveFormData Me
End Sub
