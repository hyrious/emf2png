# EMF2PNG

Convert an EMF (Enhanced Metafile) to PNG, allowing resizing.

## Prepare

Windows 10+, or install [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework).

## Build

### .Net Framework

Run `build.cmd` in current folder.

It will generate `emf2png.exe` at the root folder.

### .Net Core 7

Open `emf2png.csproj` and press Ctrl+Shift+B to build it.

It will generate `bin\Debug\net7.0\emf2png.exe`.

> [!NOTE]
> The .Net Core version seems fixed some bug in rendering some WMF files.
> I have also uploaded the prebuilt Windows-x64 version of this tool in the [Releases](https://github.com/hyrious/emf2png/releases) page.

## Usage

```console
emf2png file.emf 300:150 out.png
```

## License

MIT @ [hyrious](https://github.com/hyrious)
