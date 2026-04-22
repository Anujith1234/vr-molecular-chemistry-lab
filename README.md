# VR Molecular Lab

## Overview

VR Molecular Lab is an interactive virtual reality experience built in Unity, where users construct molecules by combining atomic elements inside a controlled bonding environment.

The system focuses on clear, intentional interaction design in VR. Users physically grab atoms, place them inside a bonding chamber, and explicitly trigger molecule formation. The application validates combinations using a rule-based system and provides immediate visual and audio feedback.

This project was developed as part of an XR Developer assessment under a strict 48-hour constraint, with emphasis on clean architecture, stable XR interaction, and thoughtful system design.

---

## Design Approach

Rather than relying on automatic bonding, the system uses an explicit **"Form Molecule"** action.

This decision was intentional:
- avoids accidental bonding during VR interaction
- gives users clear control over when validation occurs
- ensures predictable system behavior under time constraints
- improves usability when handling multiple atoms in 3D space

This tradeoff prioritizes **interaction clarity and stability** over implicit automation.

---

## Core Features

### XR Interaction System
- Grab, move, and release atoms using XR controllers
- Stable interaction built using XR Interaction Toolkit
- Reusable atom system with reset capability

### Bonding & Validation System
- Atoms placed inside a bonding chamber
- Explicit molecule formation trigger
- Rule-based validation using predefined molecule data
- Supports all required molecules from the assignment

### Molecule Visualization
- Valid combinations generate molecule output
- Displays:
  - Molecule name
  - Chemical formula
  - Bond type (single/double/triple)

### Feedback System
- Audio feedback for:
  - Successful formation
  - Invalid combinations
  - Reset actions
- Real-time visual feedback through result panel

### Molecule Library
- Tracks discovered molecules
- Prevents duplicate entries
- Displays molecule information for reference

### Reset System
- Clears bonding chamber
- Restores atoms to original positions
- Enables repeat interaction cycles

### Completion Flow
- Detects when all required molecules are formed
- Displays completion panel
- Supports restarting the experience

### VR UI System
- Fully world-space UI (no screen-space elements)
- Ray-based interaction using XR controllers
- Panels positioned for comfortable viewing distance
- Clear and readable layout for VR usage

---

## Implemented Molecules

Minimum required molecules implemented:

- H₂ (Hydrogen Gas)
- O₂ (Oxygen Gas)
- N₂ (Nitrogen Gas)
- H₂O (Water)
- NH₃ (Ammonia)
- CO₂ (Carbon Dioxide)
- CH₄ (Methane)

The system is data-driven and can be extended to support additional molecules without structural changes.

---

## Controls

### XR Controls
- Grab Atom: Trigger
- Move Atom: Controller movement
- Release Atom: Release trigger

### UI Interaction
- Use XR Ray Interactor to point and press UI elements

### Actions
- Form Molecule: Validates atoms in chamber
- Reset: Clears chamber and restores atoms
- Start Over: Resets full session after completion

---

## How to Run

### Unity Editor
1. Open the project in Unity 6 (latest stable)
2. Load scene:
   ChemLab_Main
3. Ensure OpenXR is enabled in XR Plug-in Management
4. Connect VR headset (Quest via Link / PCVR)
5. Press Play

### Build Targets
- Windows (PCVR - DirectX 11)
- Android (Meta Quest compatible)

---

## Testing Notes

The application was tested using:
- Unity Editor with XR simulation
- Windows PCVR build with headset connection

Android build settings were prepared for Meta Quest compatibility, including IL2CPP, ARM64, OpenXR, and API level 29+.

---

## Project Architecture

The project follows a modular, responsibility-driven structure.

### Core Systems

- AtomController  
  Handles XR interaction and atom identity

- MoleculeDatabase  
  ScriptableObject defining valid molecule combinations

- ReactionZone  
  Detects atoms in chamber and manages validation input

- FormMoleculeController  
  Triggers validation and molecule formation

- LabResetController  
  Handles chamber clearing and atom reset

- LabCompletionController  
  Tracks progress and completion state

- LabAudioManager  
  Centralized audio feedback system

---

## Technical Decisions

- Explicit validation trigger instead of auto bonding for interaction clarity
- Data-driven molecule system using ScriptableObjects
- Modular architecture with separated responsibilities
- Simplified physics to ensure stable XR interaction
- World-space UI for VR-first usability
- DirectX 11 enforced for Windows compatibility

---

## AI Tools Used

AI tools were used as development accelerators throughout the project.

### ChatGPT
- Designed overall system architecture
- Helped structure molecule validation and controller separation
- Assisted in debugging XR UI interaction and input issues
- Supported refinement of project flow and documentation

### Claude
- Used for deeper code refinement and structure improvements
- Helped identify edge cases in interaction and validation flow
- Assisted in improving code readability and maintainability

### Claude MCP (Unity Integration)
- Used to accelerate Unity-side debugging and verification
- Helped align code logic with scene configuration
- Reduced iteration time during development and testing

### GitHub Copilot
- Assisted with boilerplate code and repetitive patterns
- Improved iteration speed during scripting

### How AI Helped This Project
- Structured MoleculeDatabase and validation logic
- Debugged XR ray interaction and UI issues
- Refined system architecture under time constraints
- Assisted in preparing submission-ready documentation

All final implementation decisions, integration, and validation were performed manually.

---

## Known Limitations

- Molecule visualization is functional but simplified (no full 3D bond geometry)
- Physics interactions are tuned for stability rather than realism
- Bond structure is represented logically rather than physically simulated

---

## Submission Notes

This project prioritizes:
- Stable XR interaction
- Clean and modular architecture
- Clear user feedback
- Reliable molecule validation system

The goal was to deliver a complete, testable experience within the given time constraints while maintaining code quality and usability.

---

## Deliverables

- Unity Project Source (this repository)
- Demo Video (submitted separately)
- Android APK build (submitted separately)