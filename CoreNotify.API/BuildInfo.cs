using Pekspro.BuildInformationGenerator;

namespace CoreNotify.API;

[BuildInformation(AddGitCommitId = true, AddLocalBuildTime = true, FakeIfDebug = false, FakeIfRelease = false, AddGitBranch = true)]
partial class BuildInfo
{
}