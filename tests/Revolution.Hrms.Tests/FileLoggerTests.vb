Imports System.IO
Imports Revolution.Hrms.Common
Imports Xunit

Public Class FileLoggerTests

    <Fact>
    Public Sub LogError_WritesFileAndReturnsReference()
        Dim folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "hrms-log-" & Guid.NewGuid().ToString("N"))
        Try
            Dim logger As New FileLogger(folder)
            logger.Info("started")
            Dim reference = logger.LogError("test context", New InvalidOperationException("boom"))

            Assert.Equal(8, reference.Length)
            Dim files = Directory.GetFiles(folder, "*.log")
            Assert.Single(files)
            Dim text = File.ReadAllText(files(0))
            Assert.Contains("started", text)
            Assert.Contains(reference, text)
        Finally
            If Directory.Exists(folder) Then Directory.Delete(folder, True)
        End Try
    End Sub

End Class
