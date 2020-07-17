$VERSION = "4.0.0-preview01"
$SOLUTION_PATH = "C:\.NetStandardGit\LogicBuilder\LogicBuilder.DataComponents"

#Write-Host "Project Path: $SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj"
#Write-Host "Package Path: $SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg"

$PROJECT = "LogicBuilder.Data"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.Domain"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.Structures"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.Kendo.ExpressionExtensions"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.Expressions.Utils"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.Expressions.EntityFrameworkCore"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.EntityFrameworkCore.SqlServer"
msbuild /t:pack "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj" /p:Configuration=Release
nuget add "$SOLUTION_PATH\$($PROJECT)\bin\Release\$($PROJECT).$($VERSION).nupkg" -source C:\LocalNuget\packages

$PROJECT = "LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests"
dotnet test "$SOLUTION_PATH\$($PROJECT)\$($PROJECT).csproj"
