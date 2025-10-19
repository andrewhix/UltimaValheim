param(
    [string]$RootPath = "C:\Valheim Modding\Code\UltimaValheim"
)

$projectName = "UltimaValheimTemplate"
$projectPath = Join-Path $RootPath $projectName
$coreReferencePath = Join-Path $RootPath "UltimaValheimCore\UltimaValheimCore\UltimaValheimCore.csproj"
$buildsPath = Join-Path $RootPath "Builds"

Write-Host "Creating clean template project at $projectPath..."

# Create directories
New-Item -ItemType Directory -Force -Path $projectPath | Out-Null
New-Item -ItemType Directory -Force -Path "$projectPath\Package" | Out-Null
New-Item -ItemType Directory -Force -Path "$projectPath\Properties" | Out-Null

# AssemblyInfo.cs
@"
using System.Reflection;

[assembly: AssemblyTitle(""Ultima Valheim Module Template"")]
[assembly: AssemblyDescription(""Template for creating new Ultima Valheim modules."")]
[assembly: AssemblyVersion(""0.1.0.0"")]
[assembly: AssemblyFileVersion(""0.1.0.0"")]
"@ | Set-Content "$projectPath\Properties\AssemblyInfo.cs" -Encoding UTF8

# manifest.json
@"
{
  ""name"": ""Ultima Valheim Module Template"",
  ""version_number"": ""0.1.0"",
  ""website_url"": ""https://github.com/UltimaValheim"",
  ""description"": ""Template for creating new Ultima Valheim modules."",
  ""dependencies"": [
    ""BepInEx-BepInExPack_Valheim"",
    ""com.valheim.ultima.core""
  ]
}
"@ | Set-Content "$projectPath\Package\manifest.json" -Encoding UTF8

# README.md
@"
# Ultima Valheim Module Template

This is the base project template for creating new Ultima Valheim modules.
Each module references UltimaValheimCore and registers with its lifecycle.
"@ | Set-Content "$projectPath\README.md" -Encoding UTF8

# ModuleBase.cs
@"
using UltimaValheim.Core;

namespace UltimaValheim.Template
{
    public class ModuleBase : ICoreModule
    {
        private readonly string _moduleName;

        public ModuleBase(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void OnCoreReady()
        {
            CoreAPI.Log.LogInfo($""[{_moduleName}] Core ready — module initialization complete."");
        }
    }
}
"@ | Set-Content "$projectPath\ModuleBase.cs" -Encoding UTF8

# UltimaModulePlugin.cs
@"
using BepInEx;
using UltimaValheim.Core;

namespace UltimaValheim.Template
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInDependency(""com.valheim.ultima.core"")]
    public class UltimaModulePlugin : BaseUnityPlugin
    {
        public const string ModGuid = ""com.valheim.ultima.template"";
        public const string ModName = ""Ultima Valheim Module Template"";
        public const string ModVersion = ""0.1.0"";

        private ModuleBase _module;

        private void Awake()
        {
            CoreAPI.Log.LogInfo($""[{ModName}] Initializing..."");
            _module = new ModuleBase(ModName);
            CoreAPI.RegisterModule(_module);
            CoreAPI.Log.LogInfo($""[{ModName}] Registered successfully with Core."");
        }
    }
}
"@ | Set-Content "$projectPath\UltimaModulePlugin.cs" -Encoding UTF8

# .csproj
@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <OutputPath>..\Builds\$$(ProjectName)\</OutputPath>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include=""$coreReferencePath"" />
  </ItemGroup>

  <Target Name=""PostBuild"" AfterTargets=""Build"">
    <Copy 
      SourceFiles=""$$(TargetPath)""
      DestinationFolder=""$buildsPath\$$(ProjectName)""
      SkipUnchangedFiles=""true"" />
  </Target>
</Project>
"@ | Set-Content "$projectPath\$projectName.csproj" -Encoding UTF8

Write-Host "UltimaValheimTemplate recreated successfully at:"
Write-Host "  $projectPath"
