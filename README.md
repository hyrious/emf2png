# EMF2PNG

Convert an EMF (Enhanced Metafile) to PNG, allowing resizing.

## Prepare

Windows 10+, or install [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework).

### Optional Softwares

- `rm.exe` from [Git\usr\bin\rm.exe](https://git-scm.com/),
- `make.exe` from [GNU](https://gnu.org/software/make/), just run `scoop install make` if you have [scoop](https://scoop.sh/).

## Build

If you have `rm.exe` and `make.exe` in your PATH, then simply run the Makefile:

```console
make
```

### Manual Steps

1. Find `csc.exe` (the C# compiler) in your .NET Framework by running `make-csc.cmd`.
2. Run `csc /nologo /o emf2png.cs` (append `/win32icon:icon.ico` to give it an icon).

You will get an `emf2png.exe` in the current directory, just take it to anywhere you like.

## Usage

```console
emf2png file.emf 300:150 out.png
```

## License

MIT @ [hyrious](https://github.com/hyrious)
