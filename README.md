# DnD World Builder

Contributors:

|Name|Roles|
|---|---|
|[Jia-Wei Leong](https://github.com/JWL16038)| Project manager, Quality Controller, Multiplayer|
|[Finlay Metcalfe](https://github.com/Ofthemasses)| Quality Controller, Map Maker|
|[Nathan Bridge-Earney](https://github.com/nathanbridgeearney)| UI UX, Multiplayer|
|[David Lindsay](https://github.com/DavidLindsayV)| Assets, UI UX, Map Maker|
|[Anfri Hayward](https://github.com/AnfriH)| Map Maker, Game Engine
|[Shae West](https://github.com/westshae)| Map Maker, UI UX|

## Description

This project is a Dungeons and Dragons battle map
generator and an online RPG game-playing system,
named Crucible Of Worlds.

Key Features of the program:

- Battle maps are auto generated with a seed
- Maps can be printed as a single image file
- Generated maps can have different, unique themes (i.e, dungeons)
- Tokens to represent players and monsters alike moving around the screen
- Fog of War option to maintain suspenseful adventures.

This project was built by Team C for the ENGR302 course in Trimester 2, 2023.

## Documentation

The following documentations are available for this project:

- [Initial group component plan including risk management](
  https://zany-omelet-49b.notion.site/Initial-Plan-Group-Component-2202cf4a916a46e6bf4fa75a32f415e9?pvs=4)
- [Notes from the project kickoff lecture on 18th July 2023](
  https://zany-omelet-49b.notion.site/18-07-2023-Notes-2d7a60fde3fb4864ae8f32e595a4c74f?pvs=4)
- [Assets planning document](
  https://zany-omelet-49b.notion.site/Assets-cb0e1c978a424c8ab9c9c4e1d85a75e5?pvs=4)

## Installation

This project was developed and built on the Linux OS.
Please see [this tutorial](
https://learn.microsoft.com/en-us/windows/wsl/install)
on how to configure Windows Subsystem for Linux (WSL)
to install and run Linux if you are on a Windows machine.

Dotnet 7.0 is required to compile and build the project.
Please see [this tutorial](
https://learn.microsoft.com/en-us/dotnet/core/install/linux)
on how to install and configure Dotnet for Linux machines.

Clone the project and pull from the main branch by executing:

```bash
git clone https://gitlab.ecs.vuw.ac.nz/engr302teamc/dnd-world-builder.git
git pull --rebase
```

To use .NET:
Install .NET SDK

Build the project by executing dotnet build:

```bash
dotnet build
```

Run the program by executing the `UI.dll` located in `/UI/bin/Debug/net7.0/`.

A sample VSCode launch script has been provided as an example:

```json
{
    "name": "Crucible of Worlds",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build",
    "program": "${workspaceFolder}/UI/bin/Debug/net7.0/UI.dll",
    "args": [],
    "cwd": "${workspaceFolder}/UI/bin/Debug/net7.0/",
    "stopAtEntry": false,
    "console": "internalConsole"
},
```

A sample VSCode tasks.json script has been provided as well:

```json
{
  "label": "build",
  "type": "dotnet",
  "task": "build",
  "problemMatcher": [
    "$msCompile"
  ],
  "group": "build",
}
```

## Credits

[Map Background Image by kjpargeter on Freepik](https://www.freepik.com/free-vector/abstract-background-with-vintage-paper-design_18073291.htm#query=parchment&position=5&from_view=keyword&track=sph)

[Thanks to Forgotten Adventures for many map textures.](https://www.forgotten-adventures.net/)
