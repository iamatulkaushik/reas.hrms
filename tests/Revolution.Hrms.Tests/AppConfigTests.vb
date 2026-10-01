Imports System.IO
Imports Revolution.Hrms.Common
Imports Xunit

Public Class AppConfigTests

    <Fact>
    Public Sub Load_ReadsConnectionStringAndDefaultsLogFolder()
        Dim path = System.IO.Path.GetTempFileName()
        Try
            File.WriteAllText(path, "{ ""ConnectionString"": ""Server=PC1,14330;Database=HRMS_Data;"", ""LogFolder"": """" }")
            Dim config = AppConfig.Load(path)
            Assert.Equal("Server=PC1,14330;Database=HRMS_Data;", config.ConnectionString)
            Assert.Equal(AppConfig.DefaultLogFolder(), config.LogFolder)
            Assert.Equal("PC1,14330", config.ServerDisplayName)
        Finally
            File.Delete(path)
        End Try
    End Sub

    <Fact>
    Public Sub ServerDisplayName_NeverShowsPassword()
        Dim path = System.IO.Path.GetTempFileName()
        Try
            File.WriteAllText(path, "{ ""ConnectionString"": ""Server=PC1;User Id=app;Password=secret;"" }")
            Dim config = AppConfig.Load(path)
            Assert.DoesNotContain("secret", config.ServerDisplayName)
        Finally
            File.Delete(path)
        End Try
    End Sub

    <Fact>
    Public Sub Load_MissingFile_Throws()
        Dim path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") & ".json")
        Assert.Throws(Of FileNotFoundException)(Sub() AppConfig.Load(path))
    End Sub

End Class
