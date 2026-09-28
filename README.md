# SharedMemorySN
Strong named version of https://github.com/justinstenning/SharedMemory

## Release build

Run `build.cmd` from the repository root to execute the NUKE release build. It restores, builds all target frameworks, and creates the `SharedMemorySN` NuGet package in `artifacts`.

The strong-name key must be available at `c:\GitHub\sgKey.snk`, matching the existing project configuration. The GitHub Actions workflow runs this build for pull requests, version tags, and manual dispatches.
