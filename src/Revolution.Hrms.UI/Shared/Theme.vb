Imports System.Drawing

''' <summary>Colours and font from docs/DESIGN.md section 1.3.</summary>
Friend Module Theme

    Public ReadOnly Primary As Color = ColorTranslator.FromHtml("#1F4E79")
    Public ReadOnly Accent As Color = ColorTranslator.FromHtml("#2E8B57")
    Public ReadOnly Danger As Color = ColorTranslator.FromHtml("#B22222")
    Public ReadOnly PageBackground As Color = ColorTranslator.FromHtml("#F4F6F8")
    Public ReadOnly GridAlternateRow As Color = ColorTranslator.FromHtml("#EEF3F8")
    Public ReadOnly InputFocus As Color = ColorTranslator.FromHtml("#FFF8DC")

    Public Function BaseFont() As Font
        Return New Font("Segoe UI", 9.5F)
    End Function

End Module
