#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"

var target = Argument("target", "Default");
var nugetApiKey = Argument("nugetApiKey", EnvironmentVariable("nugetApiKey"));

//////////////////////////////////////////////////////////////////////
//    Build Variables
/////////////////////////////////////////////////////////////////////
var solution = "./WebOptimizer.Dotless.sln";
var project = "./src/WebOptimizer.Dotless.csproj";
var outputDir = "./buildArtifacts/";
var outputDirNuget = outputDir+"NuGet/";
var testResultsPath = System.IO.Path.Combine(System.IO.Path.GetFullPath(outputDir), "TestResults.xml");
var nugetPublishFeed = "https://api.nuget.org/v3/index.json";


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
	.Description("Removes the output directory")
	.Does(() => {
	  
	if (DirectoryExists(outputDir))
	{
		DeleteDirectory(outputDir, new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
	}
	CreateDirectory(outputDir);
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
		
		var settings = new DotNetCoreBuildSettings {        
			Configuration = "Release",		
			ArgumentCustomization = args => args.Append("/p:SemVer=" + versionInfo.NuGetVersionV2 + " /p:SourceLinkCreate=true")
		};

		DotNetCoreBuild(project, settings);			
	});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var settings = new DotNetCoreTestSettings {
			Logger = "trx;logfilename=" + testResultsPath
		};				
				
		DotNetCoreTest("./test/WebOptimizer.Dotless.Test.csproj", settings);		
	});


Task("Pack")
	.IsDependentOn("Test")
	.IsDependentOn("Version")
	.Does(() => {
		
		var packSettings = new DotNetCorePackSettings
		{			
			Configuration = "Release",
			OutputDirectory = outputDirNuget,
			ArgumentCustomization = args => args.Append("/p:PackageVersion=" + versionInfo.NuGetVersionV2+ " /p:SourceLinkCreate=true")
		};
		 
		DotNetCorePack(project, packSettings);			
	});
	
Task("Publish")	
	.IsDependentOn("Pack")	
	.Description("Pushes the created NuGet packages to nuget.org")  
	.Does(() => {
	
		// Get the paths to the packages.
		var packages = GetFiles(outputDirNuget + "*.nupkg");

		// Push the package.
		NuGetPush(packages, new NuGetPushSettings {
			Source = nugetPublishFeed,
			ApiKey = nugetApiKey
		});	
	});
	
Task("Default")
	.IsDependentOn("Test");

RunTarget(target);