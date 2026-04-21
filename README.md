# DialSwitch
DialSwitch is a lightweight "Hardware Translator." It sits between your USB device and your Operating System, catching specific signals (like a Mute or Play/Pause command) and converting them into a high-level system action: switching your audio output.
A high-performance, ultra-lightweight (<50KB) Windows utility written in C to intercept HID signals from the **Hagibis Knob Hub** and other generic desktop controllers. 

## The Problem
Many desktop knobs (Hagibis, Vaydeer, etc.) come with hardcoded shortcut buttons (e.g., Mute, Play/Pause, or Lock) and no official software to rebind them. **ApexDial** hooks these inputs at the system level and allows you to trigger custom functions—like toggling between your Speakers and Headset.

## Features
* **Zero Overhead:** No heavy frameworks. Pure Windows API (User32/Ole32).
* **Binary Size:** ~35KB (compiled with optimized GCC flags).
* **Direct Audio Switching:** Uses the `IPolicyConfig` COM interface to swap default playback devices instantly.
* **Invisible:** Runs in the background with no tray icon or window bloat.

## Quick Start
1. **Identify your button:** Run `check_hub.exe` to find the Virtual Key (VK) code sent by your hub.
2. **Configure:** Update `config.h` with your device names and target VK code.
3. **Compile:** ```bash
   gcc main.c -o ApexDial.exe -lole32 -loleaut32 -luser32 -s -Os -mwindows