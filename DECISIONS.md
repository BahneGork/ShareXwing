# ShareXwing - Key Decisions

This file documents important architectural and design decisions made during development.

---

## Decision Log

### D001: Fork Name - "ShareXwing"

**Date**: 2025-12-08
**Decided By**: User (Michael)
**Context**: Need memorable name that acknowledges ShareX origin while highlighting new features

**Options Considered:**
1. ScreenFloat - Emphasizes floating screenshots
2. ClipWing - Modern, brandable
3. PinCapture - Descriptive, straightforward
4. SnapHover - Modern feel
5. QuickPin - Simple
6. **ShareXwing** ✅ - User's suggestion

**Decision**: ShareXwing

**Rationale:**
- Acknowledges it's a ShareX fork (honest, community-friendly)
- "Wing" suggests floating/flying (perfect metaphor for floating screenshots)
- X-wing reference (memorable, fun)
- Unique and searchable
- Not trying to compete with ShareX, just enhance it

**Impact**: All documentation, repository name, project files use "ShareXwing"

---

### D002: Development Environment - WSL2 + GitHub Actions

**Date**: 2025-12-08
**Decided By**: Technical constraint + User preference
**Context**: ShareX is Windows-only, but development environment is WSL2 (Linux)

**Options Considered:**
1. Install Visual Studio in Windows, build locally
2. Use WSL2 + GitHub Actions for builds
3. Dual-boot or VM for Windows development

**Decision**: WSL2 for code writing + GitHub Actions for building

**Rationale:**
- ShareX requires Windows (WinForms, Win32 APIs) - cannot build in WSL2
- GitHub Actions already configured in ShareX (Windows runners)
- User can download built .exe from Actions (no local build needed initially)
- Faster to iterate without installing Visual Studio immediately
- Can add Visual Studio later (Phase 1+) for faster feedback loop

**Impact**:
- All code written in WSL2
- Commits pushed to GitHub
- GitHub Actions builds automatically
- User downloads .exe to test

---

### D003: Testing Strategy - GitHub Actions Downloads

**Date**: 2025-12-08
**Decided By**: User preference
**Context**: User wants to avoid Visual Studio installation initially

**Options Considered:**
1. Install Visual Studio, pull and build locally
2. Download built .exe from GitHub Actions
3. Hybrid (Actions early, Visual Studio later)

**Decision**: Use GitHub Actions downloads initially (Option 2)

**Rationale:**
- Avoids 5GB Visual Studio installation
- User just downloads and tests
- Sufficient for Phase 0 (documentation/planning)
- Can switch to local builds in Phase 1+ if needed

**Impact**:
- Build verification happens via GitHub Actions
- Testing cycle: Push → Wait 5-10 min → Download → Test → Feedback
- No local C# compilation required

---

### D004: Quality-First Development Approach

**Date**: 2025-12-08
**Decided By**: User request + Best practices
**Context**: User asked for proper software design and quality standards (referencing ShareX's approach)

**Options Considered:**
1. Move fast, add tests later
2. Quality-first: tests, linting, CI before coding
3. Minimal quality standards

**Decision**: Quality-first approach with automated gates

**Rationale:**
- User specifically requested focus on proper design and testing
- ShareX has good quality standards (inspired our approach)
- Automated quality gates prevent technical debt
- 70%+ test coverage ensures maintainability
- Easier to maintain quality from start than retrofit

**Quality Standards:**
- Code formatting: EditorConfig + StyleCop.Analyzers
- Testing: xUnit, 70%+ coverage
- CI/CD: GitHub Actions with test runs
- Pre-commit hooks: Format check, build, test
- Static analysis: SonarLint, Roslyn Analyzers

**Impact**:
- Phase 0 dedicated to quality infrastructure
- No feature coding until quality tools in place
- All code changes require passing tests

---

### D005: Architecture - ShareXwing.Core Library

**Date**: 2025-12-08
**Decided By**: Architectural design
**Context**: Need to add features while minimizing changes to ShareX core

**Options Considered:**
1. Modify ShareX directly, no separate library
2. Create ShareXwing.Core for new logic
3. Complete rewrite (not a fork)

**Decision**: Create ShareXwing.Core library with interface-driven design

**Rationale:**
- **Separation of Concerns**: All new logic isolated in Core library
- **Testability**: Interfaces enable unit testing without Windows dependencies
- **Minimal ShareX changes**: ShareX code just calls Core library
- **Easy upstream sync**: Can pull ShareX updates without conflicts
- **Independent testing**: Core library tests run in WSL2 with .NET

**Project Structure:**
```
ShareXwing/
├── ShareX/                    (original, minimal changes)
├── ShareXwing.Core/          (NEW - our logic)
│   ├── FloatingWindows/
│   ├── ImageProcessing/
│   └── Win32/
├── ShareXwing.Tests/         (NEW - unit tests)
└── ShareXwing.IntegrationTests/  (NEW - integration tests)
```

**Integration Points** (minimal ShareX modifications):
- `AfterCaptureTasks` enum - add new flags
- `TaskHelpers.cs` - add handlers
- `HotkeyType` enum - add new hotkeys
- `MainForm.cs` - add hotkey handlers

**Impact**:
- Modular, testable architecture
- Easy to maintain and extend
- Can sync with upstream ShareX
- Most code in ShareXwing.Core is fully under our control

---

### D006: Priority Features Selection

**Date**: 2025-12-08
**Decided By**: User selection
**Context**: CleanShot X has many features, need to prioritize

**Options Considered**: All CleanShot X features

**Decision**: Priority features (Phases 1-4)
1. ✅ Floating/Pinned Screenshots (highest priority)
2. ✅ Quick Access Overlay
3. ✅ All-in-One Capture
4. Background Tool (secondary)
5. Screen Freeze (secondary)
6. Hide Desktop Icons (secondary)
7. UI Modernization (later phase)

**Rationale:**
- User explicitly selected top 3 as most important
- Floating screenshots = killer differentiator
- Quick Access = workflow improvement
- All-in-One = UX simplification
- Other features add value but not critical for MVP

**Impact**:
- Phase 1-4 focus on top 3 features
- Phase 5+ adds secondary features
- Phase 6-7 polish and UI modernization

---

### D007: Enhancement vs. Replacement Strategy

**Date**: 2025-12-08
**Decided By**: Research findings
**Context**: Discovered ShareX already has pinned screenshots feature

**Finding**: ShareX has pinned screenshots, but users want improvements:
- [Issue #7843](https://github.com/ShareX/ShareX/issues/7843) - Hide/show without closing
- [Issue #7518](https://github.com/ShareX/ShareX/issues/7518) - Clear all pinned
- [Issue #7086](https://github.com/ShareX/ShareX/issues/7086) - Better toolbar

**Options Considered:**
1. Replace ShareX's pinned screenshots entirely
2. Enhance existing feature with requested improvements
3. Create parallel feature (confusing for users)

**Decision**: Enhance existing pinned screenshots feature

**Rationale:**
- Build on proven foundation (users already love it)
- Address actual user pain points
- Faster development (don't rebuild from scratch)
- Respect upstream ShareX work
- Our additions directly respond to user requests

**Features We're Adding:**
- Enhanced opacity control
- Lock mode (click-through)
- Better context menu
- "Close All" hotkey
- Improved border/visual indicators

**Impact**:
- Work WITH ShareX's existing feature
- Focus on UX improvements users want
- Differentiate through polish, not replacement

---

### D008: Phased Development Timeline

**Date**: 2025-12-08
**Decided By**: Planning process
**Context**: 2-3 month timeline, need structured approach

**Decision**: 8-phase development plan

**Phases:**
- **Phase 0**: Foundation & Quality Infrastructure (5-7 days)
- **Phase 1**: Core Library - Floating Window System (7-10 days)
- **Phase 2**: Priority Feature 1 - Pinned Screenshots (7-10 days)
- **Phase 3**: Priority Feature 2 - Quick Access Overlay (10-12 days)
- **Phase 4**: Priority Feature 3 - All-in-One Capture (7-9 days)
- **Phase 5**: Secondary Features (15-20 days)
- **Phase 6**: UI Modernization (10-15 days)
- **Phase 7**: Polish & Release (10-12 days)

**Total**: ~70-95 days (~2-3 months at 2-3 hours/day)

**Rationale:**
- Phase 0 establishes quality foundation
- Phases 1-4 deliver core value incrementally
- Each phase has clear success criteria
- Flexible - can reorder based on feedback
- Realistic timeline with buffer

**Impact**:
- Structured progress tracking
- Clear milestones for user testing
- Quality gates at each phase
- Prevents scope creep

---

## Decision Categories

### Architecture Decisions
- D002: Development Environment
- D005: ShareXwing.Core Library
- D007: Enhancement vs. Replacement

### Process Decisions
- D003: Testing Strategy
- D004: Quality-First Approach
- D008: Phased Development

### Product Decisions
- D001: Fork Name
- D006: Priority Features

---

## Future Decision Template

When making new decisions, document using this format:

```markdown
### DXXX: Decision Title

**Date**: YYYY-MM-DD
**Decided By**: Who made the call
**Context**: Why this decision was needed

**Options Considered:**
1. Option A
2. Option B
3. Option C ✅ Chosen

**Decision**: What we chose

**Rationale:**
- Why this option
- Trade-offs considered
- Alternative approaches rejected

**Impact**: How this affects the project
```

---

**Last Updated**: 2025-12-08 (Session 1)
