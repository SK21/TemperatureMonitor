VERSION 5.00
Begin VB.Form frmSelectPrinter 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Select Printer"
   ClientHeight    =   1155
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   3795
   Icon            =   "frmSelectPrinter.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1155
   ScaleWidth      =   3795
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton butSave 
      Caption         =   "OK"
      Height          =   375
      Left            =   2760
      TabIndex        =   2
      TabStop         =   0   'False
      Top             =   600
      Width           =   855
   End
   Begin VB.CommandButton butCancel 
      Caption         =   "Cancel"
      Height          =   375
      Left            =   1800
      TabIndex        =   1
      TabStop         =   0   'False
      Top             =   600
      Width           =   855
   End
   Begin VB.ComboBox Combo1 
      Height          =   315
      Left            =   120
      Style           =   2  'Dropdown List
      TabIndex        =   0
      Top             =   120
      Width           =   3495
   End
End
Attribute VB_Name = "frmSelectPrinter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public Cancel As Boolean

Private Sub butCancel_Click()
    Cancel = True
    Me.Hide
End Sub

Private Sub butSave_Click()
    Cancel = False
    Me.Hide
End Sub

Private Sub Combo1_Click()
    Set Printer = Printers(Combo1.ListIndex)
End Sub

Private Sub Form_Load()
    AD.LoadFormData Me
    LoadPrinters
End Sub

Private Sub LoadPrinters()
    Dim pr As Printer
    On Error GoTo ErrHandler
    For Each pr In Printers
       Combo1.AddItem pr.DeviceName
    Next pr
    Combo1.Text = Printer.DeviceName
    On Error GoTo 0
ErrExit:
    Exit Sub
ErrHandler:
     AD.DisplayError Err.Number, "frmSelectPrinter", "LoadPrinters", Err.Description
     Resume ErrExit
End Sub

Private Sub Form_Unload(Cancel As Integer)
    AD.SaveFormData Me
End Sub
