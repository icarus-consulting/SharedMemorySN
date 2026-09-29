using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Release);

    AbsolutePath ProjectFile => RootDirectory / "SharedMemory" / "SharedMemory.csproj";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    [Parameter("Configuration to build")]
    readonly Configuration Configuration = Configuration.Release;

    Target Clean => _ => _
        .Executes(() =>
        {
            (RootDirectory / "SharedMemory").GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() => DotNetRestore(s => s.SetProjectFile(ProjectFile)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(ProjectFile)
            .SetConfiguration(Configuration)
            .EnableNoRestore()));

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetPack(s => s
            .SetProject(ProjectFile)
            .SetConfiguration(Configuration)
            .SetOutputDirectory(ArtifactsDirectory)
            .EnableNoBuild()
            .EnableNoRestore()));

    Target Release => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            NuGetTasks.NuGetPush((options) =>
            options
                .SetApiKey(Environment.GetEnvironmentVariable("NUGET_API_KEY"))
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetTargetPath(ArtifactsDirectory / "*.nupkg")
            );
        });
}