Imports System.Globalization
Imports Revolution.Hrms.Common
Imports Xunit

Public Class DateHelpersTests

    <Theory>
    <InlineData("2026-03-31", "2025-04-01", "2025-26")>
    <InlineData("2026-04-01", "2026-04-01", "2026-27")>
    <InlineData("2026-10-01", "2026-04-01", "2026-27")>
    <InlineData("2027-01-15", "2026-04-01", "2026-27")>
    Public Sub FinancialYear_RunsAprilToMarch(input As String, expectedStart As String, expectedLabel As String)
        Dim value = Date.Parse(input, CultureInfo.InvariantCulture)
        Assert.Equal(Date.Parse(expectedStart, CultureInfo.InvariantCulture), DateHelpers.FinancialYearStart(value))
        Assert.Equal(expectedLabel, DateHelpers.FinancialYearLabel(value))
    End Sub

    <Fact>
    Public Sub FinancialYearEnd_IsThirtyFirstMarch()
        Dim value = New Date(2026, 10, 1)
        Assert.Equal(New Date(2027, 3, 31), DateHelpers.FinancialYearEnd(value))
    End Sub

    <Fact>
    Public Sub ToDisplay_UsesDayMonthYear()
        Assert.Equal("01-10-2026", DateHelpers.ToDisplay(New Date(2026, 10, 1)))
    End Sub

End Class
