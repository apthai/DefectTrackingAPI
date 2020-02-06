set mypath=%cd%

dotnet script %mypath%\PocosGenerator.csx -- output:DefectModels.cs namespace:com.apthai.DefectAPI.Model.DefectAPI config:..\appsettings.json connectionstring:ConnectionStrings:DefaultConnection dapper:true
