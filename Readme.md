# gem
General (Git) Environments Manager

If you're using Git (da..) and you maintain more than one Git workspace environment - you will find this tool handy.

I'd be more than happy to hear any feedback/suggestion/request. Thank you for using :)

## What is GEM?
**GEM - General (Git) Environments Manager** is a small Windows (WPF) desktop application to help manage and track the various Git environment workspaces you have. It sits as an icon in the systray and thus is very accessible, while not in the way.

### Features
- Systray icon.
- Hovering on the icon gives a summary of the Git workspaces you're maintaining (Folder | State | Comment).
- Clicking the icon, you get a list of environments for which you can:
-- Click to edit the comment for each environment to keep track of what you're doing with it.
-- Right click for sub-menu with *Explore workspace folder*, *Copy information to clipboard* and *Custom* commands (for creating custom commands, see below the section for Custom Workspace Commands.
- The Git workspaces recognized, as well as your comments, are persistent and will load again when you restart GEM.
- GEM can be configured to start with Windows.
- GEM tracks changes automatically for the managed workspaces, and display an up-to date information when required to.
- The application is designed for minimum maintaining and maximum information display.

## What GEM is *not*?
There are a lot of Git workspace managers that will help you do all kinds of Git stuff like commit, pull, rebase etc.
This application is not one of them, and is not built with the intention to.

## Install
### Download
The installation is pretty simple. Download the release .zip file. After extracting - run GemGui.exe.

### Requirements
- Either of Windows 10, Windows 8.1, Windows 8, Windows 7
- [.NET runtime 4.5.2](https://www.microsoft.com/en-us/download/details.aspx?id=42643)

### First time
Right click on the systray icon, and choose 'Configure'. Set the root folder where to scan for Git workspaces, and click 'OK'.

### Operate
Right clicking on the systray icon you get the following commands:
1. **Configure** (also with double-click): This is where you can enter the root folder where GEM will look for Git workspace folders. You can configure other stuff there as well.
2. **Refresh Folders Structure**: Causes GEM to re-scan the root folder, in search of Git workspaces. Useful when you've added a new Git workspace.
3. **Open Latest Log**
4. **Open Configuration**: Please see below for explanation about GEM configuration. If you're going to change it, it is recommended to save a copy beforehand, and restart GEM afterwards.
5. **About**
6. **Exit**

Hovering on the systray icon gives a summary of the managed workspaces.
Clicking on the systray icon, shows the same workspace list, and allows to edit the comment for each envrionment as well as a right-click on every workspace for additional operations.

## Building
If you want to build the sources, download the source code, and use Visual Studio 2015 (recommended Update 3, Community Edition is sufficient) for loading the Gem.sln at the root, building and running.
You may also build with just *msbuild.exe*, which is available with the installation of .NET 4.5.2 SDK.
The first time build will also acquire the relevant NuGet packages.

## Logs
The rotating log files can be found in the *logs/* folder of the application execution folder.
The logs can be very handy when analyzing issues, so be sure to add them if you submit any.

## GEM Configuration file
### Configuration file purposes:
The GEM configuration is an XML file, containing:
1. The persisted list of identified Git workspaces and their attached comments.
2. The definition of custom commands, and their attachment per workspace.

It is recommended to let GEM handle this file, but since currently there's no other way to define custom commands for a Git workspace, you can do it via editing this file. If you get it wrong - GEM will not start and will give an error pointing at the problem in the XML file.

### GEM Configuration file whereabouts
The configuration file resides in *%USERPROFILE%\AppData\Local\Gem\GemConfiguration.config* which is an XML .NET standard application configuration file.

### Custom Workspace Commands
Right clicking on a Git workspace in the workspaces list shows a sub-menu with commands such as explore the workspace folder and copy the folder path to the clipboard.
You can assign additional commands, custom commands, to this sub-menu, which are meant to spawn processes that are related to the specific Git workspace.

The configuration file has the following major sections under `<GemApplication />`:
1. `<CustomeCommands />`
2. `<SccEnvironments />`

#### Defining a custom command
`<CustomCommands />` is a list of `<Command />` entries, each accepting the following attributes:
```xml
<Command
    name="Git GUI"
    executablePath="&quot;C:\Program Files\Git\mingw64\bin\wish.exe&quot;"
    parameters="&quot;C:/Program Files/Git/mingw64/bin/gitk&quot; $CustomInfo -- --"
    workingDirectory="$FolderName"
    runElevated="true"
    description="Open Git GUI history tree" />
```
where:
1. **name**: The name of the command. This is used to identify the command when assigning it to a Git workspace. *Required*
2. **executablePath**: The full path to the executable/batch file. Remember this is XML, so when in need of quotes for a path that contains, e.g., spaces - use the standard *&quote;*. More on such XML special chars - [here](https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references). *Required*
3. **parameters**: The parameters to give the command in the `executablePath`. *Optional, Defaults to ""*
4. **workingDirectory**: The working directory to assign the spawned process. *Optional, Defaults to the GEM working directory*
5. **runElevated**: When `true` executes the required process as 'Elevated' (Administrator mode). *Optional, Defaults to `false`*
6. **description**: The description of the command, appearing as a tooltip on the relevant sub-menu. *Optional, Defaults to ""*

The values for the `executablePath`, `parameters` and `workingDirectory` attributes can incorporate the following special parameters (as can be seen in the example above):
- **$FolderName**: The absolute path of the folder of the relevant Git workspace
- **$CustomInfo**: The name of the current Git branch/commit-hash of the relevant Git workspace

In the example above, we create the command for opening the log-history tree GUI presentation of [gitgui](https://git-for-windows.github.io/) for the relevant Git workspace.

#### Assigning the command to a Git workspace
`<SccEnvironments />` Contains a list of `<Add />` entries, which are formed when GEM has identified Git workspaces and is used by GEM to know what workspaces are there and their comments when starting.
Normally you want to leave this configuration untouched, unless you'd like to assign a custom command to a Git workspace:
```xml
<Add
    folder="c:\XXX\Gem" 
    sccInfo="master"
    customInfo=""
    customCommandIds="Open Gem.sln;Git GUI" />
```
The `customCommandIds` attribute is a semicolon-separated list of custom command `name`s. As an example - the 'Git GUI' which is the name of the custom command we created earlier, is assigned to the Git workspace at the folder 'c:\XXX\Gem'.

So the full example will look like this (let GEM first build the skeleton of the .config file):
```xml
<GemApplication searchRootFolder="c:\XXX">
  <CustomCommands>
    <Command
      name="Git GUI"
      executablePath="&quot;C:\Program Files\Git\mingw64\bin\wish.exe&quot;"
      parameters="&quot;C:/Program Files/Git/mingw64/bin/gitk&quot; $CustomInfo -- --"
      workingDirectory="$FolderName"
      description="Open Git GUI history tree" />
    <Command
      name="Open Gem.sln"
      executablePath="&quot;C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe&quot;"
      parameters="&quot;$FolderName\Gem.sln&quot;" runElevated="true"
      description="Open VS2015 with GEM solution." />
  </CustomCommands>
  <SccEnvironments>
    <Add
      folder="c:\XXX\Gem" 
      sccInfo="master"
      customInfo=""
      customCommandIds="Open Gem.sln;Git GUI" />
  </SccEnvironments>
</GemApplication>
```
