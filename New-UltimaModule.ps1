param(
    [string]$RootPath = "C:\Valheim Modding\Code\UltimaValheim",
    [string]$SolutionSubDir = "UltimaValheim",
    [string]$SolutionFileName = "UltimaValheim.sln"
)

Write-Host "Ultima Valheim Module Generator"
Write-Host "----------------------------------"
$ModuleName = Read-Host "Enter the new module name (e.g., Skills, Magic, Combat)"
if (-not $ModuleName) { Write-Host "No module name provided. Exiting."; exit }

$projectName = "UltimaValheim$ModuleName"
$namespace = "UltimaValheim.$ModuleName"
$templatePath = Join-Path $RootPath "UltimaValheimTemplate"
$newProjectPath = Join-Path $RootPath $projectName
$coreReferencePath = Join-Path $RootPath "UltimaValheimCore\UltimaValheimCore\UltimaValheimCore.csproj"
$buildsPath = Join-Path $RootPath "Builds"
$solutionPath = Join-Path (Join-Path $RootPath $SolutionSubDir) $SolutionFileName

if (-not (Test-Path $templatePath)) {
    Write-Host "Template not found at $templatePath"
    Write-Host "Run Create-UltimaTemplate.ps1 first to generate it."
    exit
}

if (Test-Path $newProjectPath) {
    Write-Host "Module already exists: $newProjectPath"
    $overwrite = Read-Host "Do you want to overwrite it? (y/n)"
    if ($overwrite -ne "y") { exit }
    Remove-Item -Recurse -Force $newProjectPath
}

Write-Host "Creating module: $projectName..."
Copy-Item -Recurse -Path $templatePath -Destination $newProjectPath -Force

Rename-Item "$newProjectPath\UltimaValheimTemplate.csproj" "$projectName.csproj" -ErrorAction SilentlyContinue
Rename-Item "$newProjectPath\UltimaModulePlugin.cs" "$projectName`Plugin.cs" -ErrorAction SilentlyContinue

Get-ChildItem -Path $newProjectPath -Recurse -Include *.cs, *.csproj, *.json | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    $content = $content -replace [regex]::Escape("UltimaValheim.Template"), $namespace
    $content = $content -replace "UltimaValheimTemplate", $projectName
    $content = $content -replace "com\.valheim\.ultima\.template", "com.valheim.ultima.$($ModuleName.ToLower())"
    $content = $content -replace "Ultima Valheim Module Template", "Ultima Valheim $ModuleName"
    Set-Content $_.FullName -Value $content -Encoding UTF8
}

$csprojPath = Join-Path $newProjectPath "$projectName.csproj"

$csprojXML = @"
<Project Sdk='Microsoft.NET.Sdk'>
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <OutputPath>..\Builds\$$(ProjectName)\</OutputPath>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include='$coreReferencePath' />
  </ItemGroup>

  <Target Name='PostBuild' AfterTargets='Build'>
    <Copy 
      SourceFiles='$$(TargetPath)'
      DestinationFolder='$buildsPath\$$(ProjectName)'
      SkipUnchangedFiles='true' />
  </Target>
</Project>
"@

[System.IO.File]::WriteAllText($csprojPath, $csprojXML, [System.Text.Encoding]::UTF8)

Write-Host "Adding project to Visual Studio solution..."
if (Test-Path $solutionPath) {
    $solutionDir = Split-Path $solutionPath
    $relProjectPath = $newProjectPath.Replace($solutionDir + "\", "")
    $relProjectPath = "$relProjectPath\$projectName.csproj"
    & dotnet sln "$solutionPath" add "$relProjectPath" | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Added $projectName to $SolutionFileName"
    } else {
        Write-Host "Could not automatically add project. Add it manually in Visual Studio."
    }
} else {
    Write-Host "Solution file not found at $solutionPath"
}

Write-Host ""
Write-Host "New module created successfully!"
Write-Host "Path: $newProjectPath"
Write-Host "Namespace: $namespace"
Write-Host ""
Write-Host "Open $SolutionFileName in Visual Studio and build your new module."
