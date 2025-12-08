# ShareXwing Architecture

This document provides a comprehensive overview of ShareXwing's architecture, design principles, and integration with ShareX.

## Table of Contents

- [Overview](#overview)
- [Design Principles](#design-principles)
- [Project Structure](#project-structure)
- [Architecture Layers](#architecture-layers)
- [Integration with ShareX](#integration-with-sharex)
- [Key Components](#key-components)
- [Data Flow](#data-flow)
- [Extension Points](#extension-points)
- [Future Considerations](#future-considerations)

## Overview

ShareXwing is a fork of ShareX that adds CleanShot X-inspired features while maintaining compatibility with ShareX's core functionality. The architecture follows a **minimal invasiveness** approach where all new logic resides in separate libraries, minimizing changes to ShareX's codebase.

### Goals

- **Separation of Concerns** - Isolate new features in dedicated libraries
- **Testability** - Enable comprehensive unit and integration testing
- **Maintainability** - Make it easy to sync with upstream ShareX changes
- **Quality-First** - Enforce high code quality standards through tooling
- **Backward Compatibility** - Preserve ShareX's existing functionality

## Design Principles

### 1. Minimal ShareX Modifications

**Principle**: Modify ShareX code minimally; implement new logic in ShareXwing.Core.

**Rationale**:
- Easier to merge upstream ShareX updates
- Clearer separation between original and new code
- Reduced risk of breaking existing functionality

**Implementation**:
- ShareX code only calls ShareXwing.Core interfaces
- All business logic lives in ShareXwing.Core
- ShareX changes limited to:
  - Adding new enum values (AfterCaptureTasks, HotkeyType)
  - Calling ShareXwing.Core methods
  - UI integration points

### 2. Interface-Driven Design

**Principle**: Define contracts through interfaces, implement in concrete classes.

**Rationale**:
- Enables dependency injection
- Facilitates unit testing with mocks
- Allows multiple implementations
- Improves code organization

**Example**:
```csharp
// Interface in ShareXwing.Core
public interface IFloatingWindowManager
{
    void RegisterWindow(IFloatingWindow window);
    void CloseAll();
}

// Concrete implementation in ShareXwing.Core
public class FloatingWindowManager : IFloatingWindowManager
{
    // Thread-safe implementation
}

// Usage in ShareX
var manager = new FloatingWindowManager();
manager.RegisterWindow(window);
```

### 3. Test-Driven Quality

**Principle**: Write tests alongside or before implementation code.

**Rationale**:
- Catches bugs early
- Documents expected behavior
- Enables confident refactoring
- Enforces good design (testable code is well-designed code)

**Target**: 70%+ code coverage on all ShareXwing.Core code

### 4. Thread-Safe by Default

**Principle**: All shared state must be protected against concurrent access.

**Rationale**:
- ShareX is a multi-threaded application
- Floating windows can be created/closed simultaneously
- Prevents race conditions and data corruption

**Implementation**:
- Use `lock` statements for critical sections
- Prefer immutable data structures where possible
- Document thread-safety guarantees

## Project Structure

```
ShareXwing/
├── ShareX/                          # Original ShareX main application
│   ├── Forms/                       # WinForms UI
│   ├── TaskHelpers.cs               # Modified: calls ShareXwing.Core
│   └── ...
│
├── ShareX.HelpersLib/               # Original ShareX helpers
│   ├── AfterCaptureTasks.cs         # Modified: new enum values
│   ├── HotkeyType.cs                # Modified: new hotkey types
│   └── ...
│
├── ShareX.ScreenCaptureLib/         # Original ShareX capture
│   └── ...
│
├── ShareX.UploadersLib/             # Original ShareX uploaders
│   └── ...
│
├── ShareXwing.Core/                 # NEW: Core business logic
│   ├── FloatingWindows/             # Floating window system
│   │   ├── IFloatingWindow.cs       # Window interface
│   │   ├── IFloatingWindowManager.cs# Manager interface
│   │   └── FloatingWindowManager.cs # Manager implementation
│   │
│   ├── ImageProcessing/             # Image manipulation (future)
│   │   ├── IThumbnailGenerator.cs
│   │   └── IBackgroundCompositor.cs
│   │
│   ├── UI/                          # UI components (future)
│   │   ├── QuickAccessOverlay/
│   │   └── UnifiedCapture/
│   │
│   └── Win32/                       # Win32 API wrappers (future)
│       ├── WindowManager.cs
│       └── DesktopIconManager.cs
│
├── ShareXwing.Tests/                # NEW: Unit tests
│   ├── FloatingWindows/
│   │   └── FloatingWindowManagerTests.cs
│   └── ...
│
├── ShareXwing.IntegrationTests/     # NEW: Integration tests (future)
│   └── ...
│
└── docs/                            # NEW: Documentation
    ├── ARCHITECTURE.md              # This file
    ├── CODE_QUALITY.md              # Quality standards
    └── CONTRIBUTING.md              # Contribution guide
```

## Architecture Layers

ShareXwing follows a layered architecture:

### Layer 1: Win32 APIs (Bottom)
- **Purpose**: Low-level Windows API interactions
- **Examples**: Window management, screen capture, desktop icons
- **Location**: `ShareXwing.Core/Win32/`
- **Dependencies**: Windows SDK

### Layer 2: Core Business Logic
- **Purpose**: Feature implementations independent of UI
- **Examples**: FloatingWindowManager, ImageProcessor
- **Location**: `ShareXwing.Core/`
- **Dependencies**: ShareX.HelpersLib (minimal)

### Layer 3: UI Components
- **Purpose**: WinForms controls and windows
- **Examples**: FloatingWindow (WinForms), QuickAccessOverlay
- **Location**: `ShareXwing.Core/UI/`
- **Dependencies**: Layer 2 + WinForms

### Layer 4: ShareX Integration (Top)
- **Purpose**: Connect ShareXwing features to ShareX workflows
- **Examples**: TaskHelpers modifications, hotkey handlers
- **Location**: ShareX project files
- **Dependencies**: All layers

## Integration with ShareX

### Integration Points

ShareXwing integrates with ShareX at these specific points:

#### 1. After-Capture Tasks

**Location**: `ShareX.HelpersLib/AfterCaptureTasks.cs`

```csharp
[Flags]
public enum AfterCaptureTasks
{
    // ... existing tasks ...

    // ShareXwing additions
    PinScreenshot = 1 << 20,        // NEW
    AddBackground = 1 << 21,        // NEW
}
```

**Integration**: `ShareX/TaskHelpers.cs`

```csharp
// Called after capture completes
if (taskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.PinScreenshot))
{
    var window = floatingWindowManager.CreateFloatingWindow(image);
    window.Show();
}
```

#### 2. Hotkeys

**Location**: `ShareX.HelpersLib/HotkeyType.cs`

```csharp
public enum HotkeyType
{
    // ... existing hotkeys ...

    // ShareXwing additions
    CaptureUnified,                 // NEW: All-in-one capture
    CloseAllFloatingWindows,        // NEW: Close all pinned
}
```

**Integration**: `ShareX/Forms/MainForm.cs`

```csharp
private void RegisterHotkeys()
{
    // ... existing registrations ...

    // ShareXwing hotkeys
    RegisterHotkey(HotkeyType.CaptureUnified,
        () => ShowUnifiedCaptureDialog());
}
```

#### 3. Settings

**Location**: `ShareX/TaskSettings.cs`

```csharp
public class TaskSettings
{
    // ... existing settings ...

    // ShareXwing additions
    public FloatingWindowSettings FloatingWindow { get; set; }
    public QuickAccessSettings QuickAccess { get; set; }
}
```

### Dependency Flow

```
ShareX.exe
    ↓ references
ShareX.HelpersLib.dll
ShareX.ScreenCaptureLib.dll
ShareX.UploadersLib.dll
    ↓ references
ShareXwing.Core.dll ← All new logic here
    ↓ references
.NET 9.0 BCL
Windows SDK
```

## Key Components

### FloatingWindowManager

**Purpose**: Manages lifecycle of all floating screenshot windows.

**Responsibilities**:
- Register/unregister windows
- Track active windows
- Close all windows on command
- Thread-safe operations

**Key Methods**:
```csharp
public interface IFloatingWindowManager
{
    void RegisterWindow(IFloatingWindow window);
    void UnregisterWindow(IFloatingWindow window);
    void CloseAll();
    int ActiveCount { get; }
    IEnumerable<IFloatingWindow> GetActiveWindows();
}
```

**Thread Safety**:
- All operations use `lock (lockObject)` for synchronization
- Safe to call from any thread
- No deadlock risk (short critical sections)

### FloatingWindow (Future)

**Purpose**: Display a screenshot in an always-on-top, configurable window.

**Features**:
- Adjustable opacity (10-100%)
- Lock mode (click-through)
- Drag to reposition
- Resize with aspect ratio preservation
- Context menu (close, opacity, lock)

**Key Methods**:
```csharp
public interface IFloatingWindow
{
    void SetOpacity(double opacity);    // 0.1 to 1.0
    void SetLocked(bool locked);        // Enable/disable click-through
    void Close();
    double CurrentOpacity { get; }
    bool IsLocked { get; }
}
```

### QuickAccessOverlay (Future Phase 3)

**Purpose**: Post-capture action panel with thumbnail preview.

**Features**:
- Show near cursor after capture
- Display thumbnail of captured image
- Quick action buttons (copy, save, pin, edit, upload)
- Keyboard shortcuts
- Auto-hide after action or timeout

### UnifiedCaptureDialog (Future Phase 4)

**Purpose**: Single dialog for all capture modes.

**Features**:
- Radio buttons: Region / Window / Fullscreen / Scrolling
- Preview last capture
- Quick settings (delay, cursor, effects)
- One-click capture with selected mode

## Data Flow

### Capture → Pin Flow

```
1. User triggers capture (hotkey/menu)
   ↓
2. ShareX.ScreenCaptureLib captures screen
   ↓
3. ShareX processes image (effects, watermark)
   ↓
4. After-capture tasks evaluated
   ↓
5. If PinScreenshot flag set:
   - ShareXwing.Core.FloatingWindowManager.CreateWindow(image)
   - Window displayed with default opacity/unlocked
   - Window registered with manager
   ↓
6. User interacts with floating window:
   - Adjust opacity via slider/hotkey
   - Toggle lock mode
   - Close window → unregistered from manager
```

### Unified Capture Flow (Future)

```
1. User presses unified capture hotkey
   ↓
2. ShareX shows UnifiedCaptureDialog
   ↓
3. User selects mode + options
   ↓
4. Dialog closes, capture executes
   ↓
5. QuickAccessOverlay appears with result
   ↓
6. User selects action (copy/save/pin/etc)
   ↓
7. Action executed, overlay closes
```

## Extension Points

Future features can extend ShareXwing at these points:

### 1. Custom After-Capture Tasks

Add new enum values to `AfterCaptureTasks` and implement handlers in ShareXwing.Core.

### 2. New Window Types

Implement `IFloatingWindow` for different window behaviors (e.g., annotation window, comparison window).

### 3. Image Processors

Implement `IImageProcessor` interface for new image manipulation features.

### 4. Custom Overlays

Extend `OverlayBase` class for new post-capture UI elements.

### 5. Win32 Integrations

Add new Win32 API wrappers in `ShareXwing.Core/Win32/` for system integrations.

## Future Considerations

### Performance

- **Bitmap Management**: Implement proper disposal of Image/Bitmap objects
- **Memory Usage**: Monitor memory when many floating windows are open
- **Thumbnail Generation**: Use efficient resizing algorithms (WIC)

### Scalability

- **Window Limit**: Consider max number of floating windows (performance/UX)
- **Settings Persistence**: Save/restore window positions and settings
- **Multi-Monitor**: Handle floating windows across multiple displays

### Accessibility

- **Screen Readers**: Ensure floating windows announce state changes
- **Keyboard Navigation**: Full keyboard support for all features
- **High Contrast**: Respect Windows high contrast themes

### Internationalization

- **Localization**: Use ShareX's localization system for new strings
- **RTL Support**: Test UI with right-to-left languages

### Security

- **Clipboard Security**: Sanitize clipboard content before pinning
- **File Paths**: Validate file paths before saving
- **Win32 API**: Use safe API wrappers, handle errors gracefully

## Summary

ShareXwing's architecture prioritizes:

1. **Minimal disruption** to ShareX codebase
2. **High testability** through interfaces and dependency injection
3. **Thread safety** for concurrent operations
4. **Quality enforcement** via automated tooling
5. **Future extensibility** through clear extension points

This design enables rapid feature development while maintaining code quality and compatibility with ShareX's ongoing development.

---

**Version**: 1.0
**Last Updated**: 2025-12-08
**Author**: ShareXwing Team
