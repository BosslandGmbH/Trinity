# Trinity

This repo contains the source for the combat routines and logic library plugin
shipped with [Demonbuddy](http://www.demonbuddy.com/).

## Using Trinity

When a new version of Demonbuddy is released it automatically contains
a snapshot of the master branch of this repository at the time of the release.
This means that a new release of DB always will have the newest available version
of Trinity.

If you still want to update manually, simply download a .zip from GitHub by pressing
the `Clone or download` -> `Download ZIP` buttons. Then delete the `Trinity`
folder inside Demonbuddy\Plugins and extract the `Trinity` folder from the .zip into
the directory.

## Developing Trinity (Not working yet)

Since Demonbuddy compiles Trinity by itself there is no need to set up
a proper build environment. However, this is still beneficial if you are going to
be making changes to Trinity to make sure your changes still compile.

The repo includes at VS2017 solution which can be opened. To make the project compile
you must add references to Demonbuddy's `.exe` and `.dll` files. The project is already
set up to reference the correct assemblies in the `Dependencies` directory, so this
directory just needs to be created.

The easiest way to do that is with a symbolic link to your Demonbuddy installation. If
the path `C:\Path\to\Demonbuddy\Demonbuddy.exe` is valid, this is easily done by opening
a command prompt in the root of Trinity (in the same folder as the `.sln` file)
and running the following command (if using PowerShell, you should prefix the following command
with `cmd /c`):
```
mklink /J Dependencies "C:\Path\to\Demonbuddy"
```
Trinity should now build successfully in VS2017.

## Contributing

See the [Contributing document](CONTRIBUTING.md) for guidelines for making contributions.

## Discuss

You can discuss Demonbuddy in our Discord channel which can be found [here](https://discord.gg/XwZzyDb).
