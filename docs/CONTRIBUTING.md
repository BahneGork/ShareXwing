# Contributing to ShareXwing

Thank you for your interest in contributing to ShareXwing! This guide will help you get started.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Development Workflow](#development-workflow)
- [Code Standards](#code-standards)
- [Testing](#testing)
- [Submitting Changes](#submitting-changes)
- [Project Structure](#project-structure)
- [Communication](#communication)

## Code of Conduct

Be respectful, constructive, and professional in all interactions. We're building something awesome together!

### Expected Behavior

- ✅ Provide constructive feedback in code reviews
- ✅ Help newcomers get started
- ✅ Be patient with questions
- ✅ Celebrate successes and learn from failures
- ✅ Focus on what's best for the project

### Unacceptable Behavior

- ❌ Personal attacks or insults
- ❌ Discrimination or harassment
- ❌ Publishing others' private information
- ❌ Unprofessional conduct

## Getting Started

### Prerequisites

**Required**:
- Windows 10/11 (ShareXwing requires Windows APIs)
- .NET 9.0 SDK
- Git
- Visual Studio 2022 or VS Code with C# extension

**Recommended**:
- Visual Studio 2022 Community Edition (free)
- ReSharper or Rider (optional, for enhanced C# development)
- Windows Terminal

### Fork and Clone

1. **Fork the repository** on GitHub:
   - Visit https://github.com/BahneGork/ShareXwing
   - Click "Fork" button

2. **Clone your fork**:
   ```powershell
   git clone https://github.com/YOUR_USERNAME/ShareXwing.git
   cd ShareXwing
   ```

3. **Add upstream remote**:
   ```powershell
   git remote add upstream https://github.com/BahneGork/ShareXwing.git
   ```

## Development Setup

### 1. Install .NET 9.0 SDK

Download from: https://dotnet.microsoft.com/download/dotnet/9.0

Verify installation:
```powershell
dotnet --version  # Should show 9.0.x
```

### 2. Restore Dependencies

```powershell
dotnet restore
```

### 3. Set Up Git Hooks

```powershell
# Restore dotnet tools (includes Husky)
dotnet tool restore

# Install Husky hooks
dotnet husky install
```

This sets up pre-commit hooks that run quality checks automatically.

### 4. Build Solution

```powershell
# Build all projects
dotnet build

# Or open in Visual Studio and build (F6)
```

### 5. Run Tests

```powershell
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 6. Run ShareX

Since ShareXwing is a fork of ShareX, you can run the main ShareX project:

```powershell
# Run in Debug mode
dotnet run --project ShareX/ShareX.csproj --configuration Debug
```

Or press F5 in Visual Studio with ShareX set as startup project.

## Development Workflow

### Branching Strategy

- `main` - Stable releases only
- `develop` - Development branch (merge here)
- `feat/feature-name` - New features
- `fix/bug-description` - Bug fixes
- `docs/documentation-update` - Documentation changes

### Starting New Work

1. **Update your fork**:
   ```powershell
   git checkout develop
   git pull upstream develop
   git push origin develop
   ```

2. **Create feature branch**:
   ```powershell
   git checkout -b feat/your-feature-name
   ```

3. **Make changes** following our [code standards](#code-standards)

4. **Commit regularly** with conventional commit messages:
   ```powershell
   git commit -m "feat(floating-windows): add opacity slider"
   git commit -m "test(floating-windows): add opacity tests"
   git commit -m "fix(floating-windows): correct opacity clamping"
   git commit -m "docs: update ARCHITECTURE.md"
   ```

### Conventional Commits

Format: `type(scope): description`

**Types**:
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only
- `test` - Adding/updating tests
- `refactor` - Code restructuring (no behavior change)
- `perf` - Performance improvement
- `style` - Code formatting (whitespace, etc.)
- `chore` - Maintenance tasks

**Scopes** (examples):
- `floating-windows`
- `quick-access`
- `unified-capture`
- `core`
- `tests`
- `ci`

**Examples**:
```
feat(floating-windows): implement lock mode toggle
fix(quick-access): correct thumbnail scaling on 4K displays
test(floating-windows): add multi-window scenario tests
docs(architecture): add data flow diagrams
refactor(core): extract window manager interface
```

### Pre-Commit Quality Checks

Husky runs these automatically before each commit:

1. Code formatting verification
2. Build verification
3. Test execution

If any check fails, fix the issue and commit again.

**Bypass hooks** (emergencies only):
```powershell
git commit --no-verify -m "Emergency fix"
```

## Code Standards

See [CODE_QUALITY.md](CODE_QUALITY.md) for detailed standards.

### Quick Reference

- **Formatting**: 4 spaces, CRLF, UTF-8
- **Naming**: PascalCase for classes/methods, camelCase for parameters/fields
- **Interfaces**: Must start with `I` (e.g., `IFloatingWindow`)
- **Documentation**: XML comments on all public APIs
- **Tests**: 70%+ coverage, FluentAssertions for assertions
- **Null Safety**: Nullable reference types enabled

### Code Review Checklist

Before submitting PR, verify:

- [ ] Code follows naming conventions
- [ ] All public APIs have XML documentation
- [ ] Tests added/updated for changes
- [ ] No compiler warnings
- [ ] Pre-commit hooks pass
- [ ] ARCHITECTURE.md updated if design changed

## Testing

### Writing Tests

1. **Create test class** in `ShareXwing.Tests/` matching source structure
2. **Name test**: `MethodName_ShouldExpectedBehavior[_WhenCondition]`
3. **Use Arrange-Act-Assert** pattern
4. **Use FluentAssertions** for readable assertions

**Example**:
```csharp
[Fact]
public void SetOpacity_ShouldClampValue_WhenOutOfRange()
{
    // Arrange
    var window = new FloatingWindow();

    // Act
    window.SetOpacity(1.5);  // Above max

    // Assert
    window.CurrentOpacity.Should().Be(1.0);
}
```

### Running Tests

```powershell
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FloatingWindowTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Watch mode (runs tests on file changes)
dotnet watch test --project ShareXwing.Tests
```

### Coverage Requirements

- **Minimum**: 70% line coverage for ShareXwing.Core
- **Goal**: 80%+ coverage
- **Exclusions**: WinForms UI code, P/Invoke declarations

Check coverage reports in `TestResults/` after running with `--collect:"XPlat Code Coverage"`.

## Submitting Changes

### Pull Request Process

1. **Push your branch**:
   ```powershell
   git push origin feat/your-feature-name
   ```

2. **Create Pull Request** on GitHub:
   - Go to https://github.com/BahneGork/ShareXwing
   - Click "Pull Requests" → "New Pull Request"
   - Base: `develop` ← Compare: `feat/your-feature-name`
   - Fill out PR template

3. **PR Title Format**:
   ```
   feat(floating-windows): Add opacity slider control
   fix(quick-access): Correct thumbnail scaling
   ```

4. **PR Description Should Include**:
   - What changed and why
   - Testing performed
   - Screenshots/videos (for UI changes)
   - Breaking changes (if any)
   - Related issues (e.g., "Closes #123")

### PR Review Process

1. **CI Checks** run automatically:
   - Build succeeds
   - Tests pass
   - Code coverage >70%
   - No new warnings

2. **Code Review** by maintainer:
   - Code quality
   - Architecture alignment
   - Test coverage
   - Documentation

3. **Approval and Merge**:
   - Address review feedback
   - Get approval
   - Squash and merge to `develop`

### What to Expect

- **Response Time**: Reviews typically within 2-3 days
- **Feedback**: Constructive suggestions for improvement
- **Iteration**: May require changes before approval
- **Recognition**: Contributors listed in release notes!

## Project Structure

```
ShareXwing/
├── ShareX/                      # Original ShareX (minimal changes)
├── ShareX.HelpersLib/           # ShareX helpers (minimal changes)
├── ShareXwing.Core/             # NEW: Our core logic (focus here!)
│   ├── FloatingWindows/         # Floating window system
│   ├── ImageProcessing/         # Image manipulation
│   └── UI/                      # WinForms controls
├── ShareXwing.Tests/            # NEW: Unit tests (focus here!)
└── docs/                        # Documentation
    ├── ARCHITECTURE.md          # System design
    ├── CODE_QUALITY.md          # Quality standards
    └── CONTRIBUTING.md          # This file
```

### Where to Make Changes

**✅ Make changes in**:
- `ShareXwing.Core/` - All new feature logic
- `ShareXwing.Tests/` - Tests for new features
- `docs/` - Documentation updates

**⚠️ Minimal changes only**:
- `ShareX/TaskHelpers.cs` - Integrate new features
- `ShareX.HelpersLib/AfterCaptureTasks.cs` - Add enum values
- `ShareX.HelpersLib/HotkeyType.cs` - Add new hotkeys

**❌ Avoid changing**:
- Other ShareX core files (harder to sync with upstream)

## Communication

### Questions?

- **Issues**: Open an issue on GitHub for bugs or feature requests
- **Discussions**: Use GitHub Discussions for questions and ideas
- **Email**: Contact maintainers at bahnen@gmail.com

### Reporting Bugs

Include:
1. ShareXwing version
2. Windows version
3. Steps to reproduce
4. Expected vs actual behavior
5. Screenshots/logs if applicable

### Feature Requests

Include:
1. Use case (what problem does it solve?)
2. Proposed solution
3. Alternatives considered
4. Mockups/examples (if UI change)

## Recognition

Contributors are recognized in:
- Release notes
- GitHub contributors page
- Special thanks in major releases

Thank you for contributing to ShareXwing! 🚀

---

**Need Help?**
- Read [ARCHITECTURE.md](ARCHITECTURE.md) for system design
- Read [CODE_QUALITY.md](CODE_QUALITY.md) for code standards
- Open an issue if stuck
- Ask in Discussions for guidance

Happy coding! 🎉
