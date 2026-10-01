Imports System.Windows.Forms
Imports Revolution.Hrms.Common

''' <summary>
''' MDI shell: menu, toolbar, status bar (docs/DESIGN.md 1.2).
''' Menus and toolbar are disabled placeholders. The role-based menu builder (Phase 1)
''' enables what the signed-in user may see, and each module adds its own items.
''' </summary>
Public Class frmMain
    Inherits Form

    Private ReadOnly _config As AppConfig
    Private ReadOnly _menu As New MenuStrip()
    Private ReadOnly _toolbar As New ToolStrip()
    Private ReadOnly _status As New StatusStrip()
    Private ReadOnly _lblUser As New ToolStripStatusLabel("User: -")
    Private ReadOnly _lblCompany As New ToolStripStatusLabel("Company: -")
    Private ReadOnly _lblFinancialYear As New ToolStripStatusLabel()
    Private ReadOnly _lblServer As New ToolStripStatusLabel()
    Private ReadOnly _lblRole As New ToolStripStatusLabel("Role: -")

    Public Sub New(config As AppConfig)
        _config = config

        Text = "Revolution HRMS"
        IsMdiContainer = True
        WindowState = FormWindowState.Maximized
        Font = Theme.BaseFont()
        BackColor = Theme.PageBackground

        BuildMenu()
        BuildToolbar()
        BuildStatusBar()

        ' Same order as the designer: status, toolbar, then menu on top.
        Controls.Add(_status)
        Controls.Add(_toolbar)
        Controls.Add(_menu)
        MainMenuStrip = _menu
    End Sub

    ''' <summary>Called after login (Phase 1) to show who is signed in.</summary>
    Public Sub ShowSession(userName As String, companyName As String, roleName As String)
        _lblUser.Text = "User: " & userName
        _lblCompany.Text = "Company: " & companyName
        _lblRole.Text = "Role: " & roleName
    End Sub

    ''' <summary>Opens a module form inside the shell.</summary>
    Public Sub ShowChild(child As Form)
        child.MdiParent = Me
        child.Show()
    End Sub

    Private Sub BuildMenu()
        Dim file As New ToolStripMenuItem("&File")
        Dim switchCompany As New ToolStripMenuItem("Switch company") With {.Enabled = False}
        Dim logOut As New ToolStripMenuItem("Log out") With {.Enabled = False}
        Dim exitItem As New ToolStripMenuItem("E&xit")
        AddHandler exitItem.Click, Sub(sender, e) Close()
        file.DropDownItems.AddRange(New ToolStripItem() {switchCompany, logOut, New ToolStripSeparator(), exitItem})
        _menu.Items.Add(file)

        For Each title In {"Masters", "Attendance", "Payroll", "Statutory", "Career", "Reports", "Admin"}
            _menu.Items.Add(New ToolStripMenuItem(title) With {.Enabled = False})
        Next
    End Sub

    Private Sub BuildToolbar()
        For Each title In {"New", "Save", "Delete", "Find", "Print", "Export"}
            _toolbar.Items.Add(New ToolStripButton(title) With {.Enabled = False})
        Next
    End Sub

    Private Sub BuildStatusBar()
        _lblFinancialYear.Text = "FY " & DateHelpers.FinancialYearLabel(Date.Today)
        _lblServer.Text = "Server: " & _config.ServerDisplayName
        _status.Items.AddRange(New ToolStripItem() {_lblUser, _lblCompany, _lblFinancialYear, _lblServer, _lblRole})
        For Each item As ToolStripStatusLabel In _status.Items
            item.BorderSides = ToolStripStatusLabelBorderSides.Right
            item.Padding = New Padding(6, 0, 6, 0)
        Next
    End Sub

End Class
