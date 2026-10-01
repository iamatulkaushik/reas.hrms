Imports System.Windows.Forms

''' <summary>
''' Base class for module forms. Gives every form the same hotkeys (docs/DESIGN.md 1.5):
''' F2 New, F3 Find, F5 Refresh, Ctrl+S Save, Ctrl+P Print, Ctrl+E Export, Esc Close, Enter next field.
''' Override the OnXxx methods in the form that needs them.
''' </summary>
Public Class BaseForm
    Inherits Form

    Public Sub New()
        KeyPreview = True
        Font = Theme.BaseFont()
        BackColor = Theme.PageBackground
        StartPosition = FormStartPosition.CenterParent
    End Sub

    Protected Overridable Sub OnNewRecord()
    End Sub

    Protected Overridable Sub OnFind()
    End Sub

    Protected Overridable Sub OnRefreshData()
    End Sub

    Protected Overridable Sub OnSave()
    End Sub

    Protected Overridable Sub OnPrint()
    End Sub

    Protected Overridable Sub OnExport()
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.F2
                OnNewRecord()
                Return True
            Case Keys.F3
                OnFind()
                Return True
            Case Keys.F5
                OnRefreshData()
                Return True
            Case Keys.Control Or Keys.S
                OnSave()
                Return True
            Case Keys.Control Or Keys.P
                OnPrint()
                Return True
            Case Keys.Control Or Keys.E
                OnExport()
                Return True
            Case Keys.Escape
                Close()
                Return True
            Case Keys.Enter
                If IsEnterNavigable(ActiveControl) Then
                    SelectNextControl(ActiveControl, True, True, True, True)
                    Return True
                End If
        End Select
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' Enter moves to the next field, except in multi-line text boxes, buttons and grids.
    Private Shared Function IsEnterNavigable(control As Control) As Boolean
        If control Is Nothing Then Return False
        Dim box = TryCast(control, TextBox)
        If box IsNot Nothing Then Return Not box.Multiline
        Return TypeOf control Is ComboBox _
            OrElse TypeOf control Is MaskedTextBox _
            OrElse TypeOf control Is NumericUpDown _
            OrElse TypeOf control Is DateTimePicker
    End Function

End Class
