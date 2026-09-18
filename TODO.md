# Typemetric TODO

## Current Tasks
- [ ] **Packaging**: Build the final Single-File executable optimized for low RAM footprint.
  - Run the following command from the project root to publish:
    ```bash
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=true
    ```
- [ ] Future feature: Allow user to reset database entirely.
- [ ] Future feature: Support for multiple monitor DPI scaling tweaks if needed.
