VERSION 5.00
Begin VB.Form frmInfo 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "File Info"
   ClientHeight    =   3045
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   10185
   Icon            =   "frmInfo.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3045
   ScaleWidth      =   10185
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton butSave 
      Caption         =   "OK"
      Height          =   375
      Left            =   9120
      TabIndex        =   12
      Top             =   2520
      Width           =   855
   End
   Begin VB.Label LbInfo 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Index           =   5
      Left            =   2040
      TabIndex        =   11
      Top             =   2520
      Width           =   5355
   End
   Begin VB.Label Lb 
      Caption         =   "Backup File Date"
      Height          =   255
      Index           =   5
      Left            =   240
      TabIndex        =   10
      Top             =   2520
      Width           =   1335
   End
   Begin VB.Label LbInfo 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Index           =   4
      Left            =   2040
      TabIndex        =   9
      Top             =   960
      Width           =   5355
   End
   Begin VB.Label LbInfo 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Index           =   3
      Left            =   2040
      TabIndex        =   8
      Top             =   600
      Width           =   2175
   End
   Begin VB.Label LbInfo 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Index           =   2
      Left            =   2040
      TabIndex        =   7
      Top             =   240
      Width           =   7995
   End
   Begin VB.Label LbInfo 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   510
      Index           =   1
      Left            =   2040
      TabIndex        =   6
      Top             =   1920
      Width           =   7995
   End
   Begin VB.Label LbInfo 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   510
      Index           =   0
      Left            =   2040
      TabIndex        =   1
      Top             =   1320
      Width           =   7995
   End
   Begin VB.Label Lb 
      Caption         =   "Database Date"
      Height          =   255
      Index           =   4
      Left            =   240
      TabIndex        =   5
      Top             =   960
      Width           =   1335
   End
   Begin VB.Label Lb 
      Caption         =   "Database Size (KB)"
      Height          =   255
      Index           =   3
      Left            =   240
      TabIndex        =   4
      Top             =   600
      Width           =   1695
   End
   Begin VB.Label Lb 
      Caption         =   "Backup Folder"
      Height          =   255
      Index           =   1
      Left            =   240
      TabIndex        =   2
      Top             =   1920
      Width           =   1335
   End
   Begin VB.Label Lb 
      Caption         =   "Database Folder"
      Height          =   255
      Index           =   0
      Left            =   240
      TabIndex        =   0
      Top             =   1320
      Width           =   1335
   End
   Begin VB.Label Lb 
      Caption         =   "Database Name"
      Height          =   255
      Index           =   2
      Left            =   240
      TabIndex        =   3
      Top             =   240
      Width           =   1335
   End
End
Attribute VB_Name = "frmInfo"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Sub butSave_Click()
    Unload Me
End Sub
Private Sub Form_Load()
    Dim DT As Date
    On Error GoTo ErrHandler
    AD.LoadFormData Me
    LbInfo(0).Caption = AD.Folders(AppDatabase)
    LbInfo(1).Caption = AD.Folders(AppBackup)
    LbInfo(2).Caption = Prog.DatabaseName
    LbInfo(3).Caption = Format(AD.FileSize, "###,###,##0")
    DT = AD.FileModified
    LbInfo(4).Caption = Format(DT, "medium date") & "    " & Format(DT, "medium time")
    LbInfo(5).Caption = BackupDate
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmInfo", "Form_Load", Err.Description
     Resume ErrExit
End Sub
Private Sub Form_Unload(Cancel As Integer)
    AD.SaveFormData Me
End Sub
Private Function BackupDate() As String
    Dim BackupName As String
    Dim DT As Date
    On Error GoTo ErrHandler
    BackupName = AD.Folders(AppBackup) & "\" & Prog.DatabaseName & ".mdb"
    If FileModified(BackupName, DT) Then
        BackupDate = Format(DT, "medium date") & "    " & Format(DT, "medium time")
    Else
        BackupDate = ""
    End If
    On Error GoTo 0
ErrExit:
    Exit Function
ErrHandler:
     AD.DisplayError Err.Number, "frmInfo", "BackupDate", Err.Description
     Resume ErrExit
End Function
