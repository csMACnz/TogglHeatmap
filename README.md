Toggl Heatmap
================

[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![NuGet](https://img.shields.io/nuget/v/TogglHeatmap.svg)](https://www.nuget.org/packages/TogglHeatmap)
[![NuGet](https://img.shields.io/nuget/dt/TogglHeatmap.svg)](https://www.nuget.org/packages/TogglHeatmap)

[![AppVeyor Build status](https://img.shields.io/appveyor/ci/MarkClearwater/togglheatmap.svg)](https://ci.appveyor.com/project/MarkClearwater/togglheatmap)

A tool for parsing Toggl time entry data and producing a heatmap csv.
The output can be written to a csv file and presented as required.

To use
------

The app is published as a dotnet SDK 2.1 tool. This requires dotnet sdk version 2.1 to be installed to install this and run this tool.

``` powershell
# install globally
dotnet tool install -g TogglHeatmap --version 1.0.0

# install into a local folder
dotnet tool install TogglHeatmap --version 1.0.0 --tools-path tools
```

To run, simply use the command:

``` powershell
# if installed globally, this should just be available on your path
TogglHeatmap <args>

# if installed into a tools path, you can run it from there.
.\tools\TogglHeatmap <args>
```