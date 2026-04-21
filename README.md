# 🚀 Console Audio Tracker

A sleek **.NET** utility providing real-time task tracking with **dynamic audio feedback**. It uses linear frequency scaling to give you an eyes-free status update on background processes.

---

## 💎 Features
*   🔊 **Dynamic Pitch**: Audio frequency shifts (`800Hz + idx * 100`) as tasks progress.
*   🛡️ **Leak Protection**: Uses Environment Variables to keep API keys off GitHub.
*   ⚡ **Zero Bloat**: No external dependencies.
*   🛑 **Process Control**: Includes a "Kill Switch" script for safety.

---

## 🛠️ Installation & Setup

### 1. Prerequisites
*   [.NET SDK](https://microsoft.com)
*   Windows OS (for `Console.Beep` frequency support)

### 2. Setup
```bash
git clone https://github.com
cd repo-name
dotnet build -c Release
```

### 3. Security Config
* Set your secret key in your terminal before running:
*  $env:MY_APP_SECRET = "your_private_value_here"

## 🚀 Usage
* dotnet run

### 🛑 Emergency Stop
* If the application hangs, run the included kill script:
* Double-click kill_app.bat
* Or press Ctrl + C in the terminal.

### 📂 Project Structure
* Program.cs: Main logic & sanitized loop.
* kill_app.bat: Force-kill script.
* .gitignore: Prevents leaking bin/ and obj/ folders.

---

### 3. `kill_app.bat`
A failsafe to stop the program instantly.

```batch
@echo off
set APP_NAME=YourProjectName.exe
echo Attempting to terminate %APP_NAME%...
taskkill /F /IM %APP_NAME% /T
if %ERRORLEVEL% EQU 0 (
    echo [SUCCESS] Process terminated.
) else (
    echo [ERROR] Process not found or already closed.
)
pause
```
