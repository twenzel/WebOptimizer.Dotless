#tool "dotnet:?package=GitVersion.Tool&version=5.8.1"
#tool "nuget:?package=NuGet.CommandLine&version=5.11.0"

var target = Argument("target", "Default");
var nugetApiKey = Argument("nugetApiKey", EnvironmentVariable("nugetApiKey"));

//////////////////////////////////////////////////////////////////////
//    Build Variables
/////////////////////////////////////////////////////////////////////
var solution = "./WebOptimizer.Dotless.sln";
var project = "./src/WebOptimizer.Dotless.csproj";
var outputDirRoot = new DirectoryPath("./buildArtifacts/").MakeAbsolute(Context.Environment);
var outputDirPublished = outputDirRoot.Combine("Published");
var outputDirTemp = outputDirRoot.Combine("Temp");
var packageOutputDir = outputDirPublished.Combine("Package");

var outputDirTests = outputDirTemp.Combine("Tests/");

var nugetPublishFeed = "https://api.nuget.org/v3/index.json";


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
	.Description("Removes the output directory")
	.Does(() => {
	  
	EnsureDirectoryDoesNotExist(outputDirRoot, new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});	
	CreateDirectory(outputDirRoot);
	CreateDirectory(outputDirPublished);
});

GitVersion versionInfo = null;
Task("Version")
	.Description("Retrieves the current version from the git repository")
	.Does(() => {
		
		versionInfo = GitVersion(new GitVersionSettings {
			UpdateAssemblyInfo = false
		});
		
		Information("Version: "+ versionInfo.FullSemVer);
	});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
	.Does(() => {
		
		var msBuildSettings = new DotNetMSBuildSettings()
		{
			Version =  versionInfo.AssemblySemVer,
			InformationalVersion = versionInfo.InformationalVersion,
			PackageVersion = versionInfo.NuGetVersionV2
		}.WithProperty("PackageOutputPath", packageOutputDir.FullPath);	

		var settings = new DotNetBuildSettings {
			Configuration = "Release",			
			MSBuildSettings = msBuildSettings
		};	 		
	 
		DotNetBuild(project, settings);				
	});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var settings = new DotNetTestSettings {
			Loggers = new[]{"trx;"},
			ResultsDirectory = outputDirTests,
			NoBuild = true
		};				
				
		DotNetTest("./test/WebOptimizer.Dotless.Test.csproj", settings);		
	});
	
Task("Publish")	
	.IsDependentOn("Test")	
	.Description("Pushes the created NuGet packages to nuget.org")  
	.Does(() => {
	
		// Get the paths to the packages.
		var packages = GetFiles(packageOutputDir.CombineWithFilePath("*.nupkg").FullPath);

		// Push the package.
		NuGetPush(packages, new NuGetPushSettings {
			Source = nugetPublishFeed,
			ApiKey = nugetApiKey
		});	
	});
	
Task("Default")
	.IsDependentOn("Test");

RunTarget(target);