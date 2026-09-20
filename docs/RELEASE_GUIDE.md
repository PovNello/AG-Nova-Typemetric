# Typemetric Release Guide

This guide describes the standardized steps to package and release a new version of Typemetric.
All commands and configurations are already set up to generate a lightweight **Single-File Executable** ("Portable") that does not require installation.

## Release Procedure

### 1. Update Version Numbers
Before packaging, make sure to update the version references throughout the project:
- **`Typemetric.csproj`**: Update the `<Version>X.Y.Z</Version>` tag.
- **`CHANGELOG.md`**: Replace the `[Unreleased]` entry with the new version number and today's date.

### 2. Generate the Executable
The build process has been automated using a batch script.
- From the project root folder, double-click on **`publish.bat`**.
- Wait for the process to finish; the script will run the following `.NET` command:
  ```bash
  dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
  ```
- *Note: The command will produce the complete executable containing the previously configured `icon.ico`.*

### 3. Retrieve the File
Once the console indicates that the build is complete, you will find the ready-to-release `.exe` file at the following path:
`bin/Release/net10.0-windows/win-x64/publish/Typemetric.exe`

### 4. GitHub Release
- Open your repository on GitHub via the browser.
- Select **Releases** in the right sidebar and click **Draft a new release**.
- Create a new tag and enter the title corresponding to the version (e.g., `v1.1.0`).
- Drag and drop the `Typemetric.exe` file (retrieved in step 3) into the attachments box.
- Copy the text from your `CHANGELOG.md` and paste it as the release description.
- Click **Publish Release**.
