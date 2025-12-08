# ShareXwing - Claude Instructions

**DO NOT DELETE THIS FILE** - This file provides context for Claude Code sessions on this project.

## Project Overview

**Name**: ShareXwing
**Type**: C# WinForms Application (Windows-only)
**Base**: Fork of ShareX (https://github.com/ShareX/ShareX)
**Repository**: https://github.com/BahneGork/ShareXwing
**Goal**: Add CleanShot X-inspired features with professional code quality standards

## Key Decisions Made

1. **Name**: ShareXwing (X-wing reference, suggests floating/flying screenshots)
2. **Development Approach**: Quality-first with automated testing, CI/CD, code standards
3. **Testing Strategy**: GitHub Actions for builds, user downloads .exe to test (no local build required initially)
4. **Timeline**: 2-3 months, phased approach

## Priority Features

**Phase 1-4 (Must Have):**
1. ✅ Floating/Pinned Screenshots - Always-on-top reference windows with opacity control
2. ✅ Quick Access Overlay - Post-capture action panel with thumbnail preview
3. ✅ All-in-One Capture - Unified capture interface (single hotkey for all modes)

**Phase 5+ (Secondary):**
4. Hide Desktop Icons - Win32 API to hide icons during capture
5. Screen Freeze - Freeze screen to capture moving objects
6. Background Tool - Add professional backgrounds for social media
7. UI Modernization - Simplify ShareX's overwhelming interface

## Technology Stack

- **Language**: C# (.NET 9.0)
- **UI Framework**: Windows Forms (inherited from ShareX)
- **Platform**: Windows only (WinForms, Win32 APIs)
- **Testing**: xUnit + FluentAssertions
- **CI/CD**: GitHub Actions (already configured in ShareX)
- **Quality Tools**: StyleCop.Analyzers, EditorConfig, SonarLint

## Development Environment

**Working Environment**: WSL2 (Linux)
- ✅ Can write C# code (text files)
- ✅ Can commit/push to git
- ✅ Can create project structure
- ✅ Can write documentation
- ❌ Cannot build ShareX (Windows-only)
- ❌ Cannot run ShareX
- ❌ Cannot use Visual Studio

**Build Environment**: GitHub Actions (Windows)
- Automatically builds on every push
- Creates installer artifacts
- Runs tests (once we add them)

**Testing Environment**: Windows (user's machine)
- User downloads built .exe from GitHub Actions
- User runs and tests features
- User provides feedback

## Important: ShareX Already Has Pinned Screenshots!

**Research Finding**: ShareX already has basic pinned screenshots feature, but users want improvements:
- [Issue #7843](https://github.com/ShareX/ShareX/issues/7843) - Hide/show pinned screenshots
- [Issue #7518](https://github.com/ShareX/ShareX/issues/7518) - Clear all pinned screenshots
- [Issue #7086](https://github.com/ShareX/ShareX/issues/7086) - Better toolbar

**Our Enhancement Strategy**: Build on existing foundation, add CleanShot-style features users are requesting.

## Architecture Design

### Project Structure
```
ShareXwing/
├── ShareX/                          (original ShareX main app)
├── ShareX.HelpersLib/               (original ShareX helpers)
├── ShareX.ScreenCaptureLib/         (original ShareX capture)
├── ShareX.UploadersLib/             (original ShareX uploaders)
├── ShareXwing.Core/                 (NEW - our core logic)
│   ├── FloatingWindows/             (floating window system)
│   ├── ImageProcessing/             (thumbnails, backgrounds)
│   └── Win32/                       (Win32 API wrappers)
├── ShareXwing.Tests/                (NEW - unit tests)
└── ShareXwing.IntegrationTests/     (NEW - integration tests)
```

### Core Principles
1. **Separation of Concerns** - All new logic in ShareXwing.Core
2. **Minimal ShareX Changes** - Modify ShareX minimally, easy to sync upstream
3. **Testability First** - Interfaces, dependency injection, unit tests
4. **Quality Gates** - 70%+ coverage, CI validation, no warnings

## Critical Files to Modify (ShareX Core)

**Minimal invasive changes to these files:**

1. **ShareX.HelpersLib/AfterCaptureTasks.cs**
   - Add new enum flags: `PinScreenshot`, `AddBackground`

2. **ShareX/TaskHelpers.cs**
   - Add handlers for new after-capture tasks

3. **ShareX.HelpersLib/HotkeyType.cs**
   - Add new hotkey types: `CaptureUnified`, `CloseAllFloatingWindows`

4. **ShareX/Forms/MainForm.cs**
   - Add hotkey handlers

5. **ShareX/TaskSettings.cs**
   - Add new configuration properties

6. **ShareX/CaptureHelpers/CaptureBase.cs**
   - Add hooks for hide-icons feature

## Quality Standards

### Code Requirements
- **Format**: EditorConfig + StyleCop.Analyzers
- **Standards**: PEP-8 equivalent for C#, no warnings
- **Testing**: 70%+ code coverage on new code
- **Documentation**: XML comments on public APIs

### Pre-Commit Checks
```bash
dotnet format --verify-no-changes
dotnet build --no-incremental
dotnet test --no-build
```

### CI/CD Pipeline
1. Build solution (Release + Debug)
2. Run unit tests
3. Check code coverage
4. Static analysis
5. Create installer artifacts

## Current Status

**Phase**: Phase 0 - Foundation & Quality Infrastructure
**Progress**: 2/11 tasks complete
- ✅ Fork created (ShareXwing)
- ✅ Repository cloned
- 🔄 Documentation in progress
- ⏳ Quality tools setup pending
- ⏳ Test projects pending

## Development Workflow

### Iteration Cycle
1. **Planning** - Decide on feature to implement
2. **Code** - I write C# code in WSL2, commit to git
3. **Push** - Push to GitHub
4. **Build** - GitHub Actions builds automatically (~5-10 min)
5. **Test** - User downloads .exe from Actions, tests feature
6. **Feedback** - User reports results (works/broken/needs changes)
7. **Iterate** - Fix bugs, improve, repeat

### Git Workflow
```bash
# Feature branches from dev
git checkout -b feat/pinned-screenshots

# Conventional commits
git commit -m "feat(floating-windows): add pinned screenshot window"
git commit -m "test(floating-windows): add opacity tests"
git commit -m "fix(floating-windows): correct opacity clamping"

# Push for CI/CD
git push origin feat/pinned-screenshots
```

## Documentation Files

**Core Documentation:**
- `/home/exit/.claude/plans/transient-waddling-peacock.md` - Full implementation plan
- `CLAUDE.md` - This file, project context
- `SESSION-LOG.md` - Session history and progress
- `DECISIONS.md` - Key architectural/design decisions

**To Be Created:**
- `docs/ARCHITECTURE.md` - System architecture overview
- `docs/CODE_QUALITY.md` - Quality standards and testing guidelines
- `docs/CONTRIBUTING.md` - How to contribute
- `docs/FEATURES.md` - Feature list and keyboard shortcuts

## User Credentials

**Location**: `/home/exit/.claude/credentials.json`

**Contents:**
- GitHub username: BahneGork
- GitHub email: bahnen@gmail.com
- Name: Michael
- Projects root: /home/exit/dev/projects

## Important Reminders

### For Claude Code
- ✅ You are working in WSL2 (Linux environment)
- ✅ You can write C# code but cannot compile it
- ✅ All building happens via GitHub Actions or user's Windows machine
- ✅ Focus on code quality, testing, and documentation
- ✅ Use conventional commits
- ✅ Keep ShareX changes minimal

### For User
- Test builds by downloading from: https://github.com/BahneGork/ShareXwing/actions
- Visual Studio installation optional (can defer to Phase 1+)
- Feedback loop: Download → Test → Report results
- Git pull not required if using GitHub Actions artifacts

## Next Steps

**Immediate (Phase 0):**
1. Create ShareXwing.Core library project
2. Set up xUnit test projects
3. Configure code quality tools (EditorConfig, StyleCop)
4. Write architecture documentation
5. Verify CI/CD pipeline works

**Phase 1 (After Phase 0):**
1. Design floating window interfaces
2. Implement FloatingWindowBase class
3. Write unit tests (80%+ coverage)
4. Create sample floating window
5. Verify on Windows

## References

- **Main Plan**: `/home/exit/.claude/plans/transient-waddling-peacock.md`
- **ShareX Repo**: https://github.com/ShareX/ShareX
- **Our Fork**: https://github.com/BahneGork/ShareXwing
- **CleanShot X**: https://cleanshot.com (inspiration for features)

---

**Last Updated**: 2025-12-08 (Session 1 - Initial setup)
