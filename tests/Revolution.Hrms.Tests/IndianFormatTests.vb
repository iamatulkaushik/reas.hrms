Imports System.Globalization
Imports Revolution.Hrms.Common
Imports Xunit

Public Class IndianFormatTests

    <Theory>
    <InlineData("0", "₹ 0.00")>
    <InlineData("5", "₹ 5.00")>
    <InlineData("999.5", "₹ 999.50")>
    <InlineData("1000", "₹ 1,000.00")>
    <InlineData("12345.678", "₹ 12,345.68")>
    <InlineData("123456", "₹ 1,23,456.00")>
    <InlineData("12345678.9", "₹ 1,23,45,678.90")>
    <InlineData("-1234.5", "-₹ 1,234.50")>
    <InlineData("0.005", "₹ 0.01")>
    <InlineData("2.675", "₹ 2.68")>
    Public Sub ToRupees_FormatsWithIndianGrouping(input As String, expected As String)
        Dim amount = Decimal.Parse(input, CultureInfo.InvariantCulture)
        Assert.Equal(expected, IndianFormat.ToRupees(amount))
    End Sub

End Class
