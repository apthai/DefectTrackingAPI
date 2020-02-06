set mypath=%cd%

dotnet script %mypath%\PocosGenerator.csx -- output:ModelsAuth.cs namespace:com.apthai.DefectAPI.Model.DefectAPISync config:..\appsettings.json connectionstring:ConnectionStrings:DefaultAuthorizeConnection dapper:true
