🎵 Console Audio Tracker
A high-performance .NET console application that provides real-time visual progress and dynamic auditory feedback using linear frequency scaling.
✨ Features
Dynamic Pitch Scaling: Uses Console.Beep logic to increase frequency as tasks progress.
Sanitized Architecture: Built to use environment variables to prevent sensitive data leaks.
Lightweight: Zero external dependencies; runs directly on the .NET runtime.
Process Management: Includes dedicated scripts to safely terminate the application.
🛠 Installation
Prerequisites
.NET 6.0 SDK or newer.
Windows OS (Required for Console.Beep frequency support).
Setup
Clone the Repository
bash
git clone https://github.com
cd console-audio-tracker
Use code with caution.
Configure Environment Variables
This app requires an API Key stored locally. Do not hardcode this.
PowerShell: $env:APP_SECRET_KEY="your_value_here"
CMD: set APP_SECRET_KEY=your_value_here
Build the App
bash
dotnet build --configuration Release
Use code with caution.
🚀 Usage
Run the application using the .NET CLI:
bash
dotnet run
Use code with caution.
Audio Logic
The application calculates sound frequency based on the current loop index:
Frequency = 800 + (index * 100) Hz
This creates a rising "siren" effect to signal task completion.
🔒 Security & Privacy
To keep this repository safe for public contribution:
Environment Variables: All credentials must be loaded via Environment.GetEnvironmentVariable.
GitIgnore: The .gitignore file is configured to block bin/, obj/, and .user files.
No Hardcoding: Never commit actual keys or personal file paths to the source code.
🛑 How to Stop the Program
If the program enters an infinite loop or you need to force-stop:
Keyboard Shortcut: Press Ctrl + C in the terminal.
Kill Script: Run the included kill_app.bat file in the root directory:
bash
./kill_app.bat
Use code with caution.
📂 Project Structure
Program.cs - Main application logic.
kill_app.bat - Force-terminate script.
.gitignore - Prevents sensitive file uploads.
README.md - Documentation (this file).
📄 License
This project is licensed under the MIT License - see the LICENSE file for details.