# Code Quality Standards

This document defines the code quality standards, testing requirements, and best practices for ShareXwing development.

## Table of Contents

- [Overview](#overview)
- [Code Formatting](#code-formatting)
- [Naming Conventions](#naming-conventions)
- [Testing Requirements](#testing-requirements)
- [Code Analysis](#code-analysis)
- [Documentation Standards](#documentation-standards)
- [Best Practices](#best-practices)
- [Quality Gates](#quality-gates)

## Overview

ShareXwing follows a **quality-first** approach where code quality is non-negotiable. All code must meet these standards before being merged.

### Quality Principles

1. **Correctness** - Code must work as intended
2. **Readability** - Code should be easy to understand
3. **Maintainability** - Code should be easy to modify
4. **Testability** - Code must be thoroughly tested
5. **Performance** - Code should be efficient

## Code Formatting

### EditorConfig

All code must follow the rules defined in `.editorconfig`:

```ini
# C# files
[*.cs]
indent_size = 4
charset = utf-8
end_of_line = crlf
trim_trailing_whitespace = true
```

**Key Rules**:
- 4 spaces for indentation (no tabs)
- UTF-8 encoding
- CRLF line endings (Windows standard)
- No trailing whitespace
- Braces required for all control statements

### Verification

Run before committing:

```powershell
# Check formatting
dotnet format --verify-no-changes

# Auto-fix formatting
dotnet format
```

### StyleCop.Analyzers

ShareXwing.Core and ShareXwing.Tests use StyleCop.Analyzers for additional code style enforcement.

**Configuration**: `stylecop.json` at solution root

**Key Rules Enforced**:
- SA1600: Elements should be documented (public APIs only)
- SA1633: File should have header (disabled - copyright in LICENSE)
- SA1101: Prefix local calls with this (disabled)
- SA1200: Using directives should be placed outside namespace
- SA1309: Field names should not begin with underscore (private fields: camelCase)

## Naming Conventions

### Classes and Interfaces

```csharp
// ✅ Good
public class FloatingWindowManager { }
public interface IFloatingWindow { }
public enum AfterCaptureTasks { }

// ❌ Bad
public class floatingwindowmanager { }
public interface FloatingWindow { }  // Missing 'I' prefix
public enum afterCaptureTasks { }    // Should be PascalCase
```

**Rules**:
- Classes: `PascalCase`
- Interfaces: `IPascalCase` (must start with 'I')
- Enums: `PascalCase`
- Structs: `PascalCase`

### Methods and Properties

```csharp
// ✅ Good
public void RegisterWindow(IFloatingWindow window) { }
public int ActiveCount { get; }

// ❌ Bad
public void register_window(IFloatingWindow window) { }
public int activeCount { get; }
```

**Rules**:
- Methods: `PascalCase`
- Properties: `PascalCase`
- Public fields: `PascalCase` (avoid public fields when possible)

### Parameters and Local Variables

```csharp
// ✅ Good
public void SetOpacity(double opacity)
{
    var clampedValue = Math.Clamp(opacity, 0.1, 1.0);
}

// ❌ Bad
public void SetOpacity(double Opacity)
{
    var ClampedValue = Math.Clamp(Opacity, 0.1, 1.0);
}
```

**Rules**:
- Parameters: `camelCase`
- Local variables: `camelCase`
- Use `var` when type is obvious from right side

### Private Fields

```csharp
// ✅ Good
private readonly List<IFloatingWindow> activeWindows;
private readonly object lockObject;

// ❌ Bad
private readonly List<IFloatingWindow> _activeWindows;  // No underscore prefix
private readonly object LockObject;                     // Should be camelCase
```

**Rules**:
- Private fields: `camelCase` (no underscore prefix)
- Use `readonly` when field is not reassigned

### Constants

```csharp
// ✅ Good
private const int MaxWindowCount = 50;
public const double MinOpacity = 0.1;

// ❌ Bad
private const int MAX_WINDOW_COUNT = 50;  // C-style all caps
public const double minOpacity = 0.1;      // Should be PascalCase
```

**Rules**:
- Constants: `PascalCase`

## Testing Requirements

### Code Coverage Target

**Minimum**: 70% line coverage for ShareXwing.Core
**Goal**: 80%+ line coverage

**Exclusions**:
- WinForms UI code (difficult to unit test)
- Win32 P/Invoke declarations
- Auto-generated code

### Test Organization

```
ShareXwing.Tests/
├── FloatingWindows/
│   ├── FloatingWindowManagerTests.cs
│   ├── FloatingWindowTests.cs
│   └── Mocks/
│       └── MockFloatingWindow.cs
├── ImageProcessing/
└── ...
```

**Rules**:
- Mirror directory structure of ShareXwing.Core
- One test class per production class
- Test class name: `{ClassName}Tests`
- Mock classes in `Mocks/` subdirectory

### Test Naming

```csharp
// ✅ Good
[Fact]
public void RegisterWindow_ShouldIncreaseActiveCount()
{
    // Arrange, Act, Assert
}

[Fact]
public void RegisterWindow_ShouldThrowArgumentNullException_WhenWindowIsNull()
{
    // Test
}

// ❌ Bad
[Fact]
public void Test1() { }

[Fact]
public void RegisterWindowWorks() { }
```

**Format**: `MethodName_ShouldExpectedBehavior[_WhenCondition]`

### Test Structure

Use **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public void CloseAll_ShouldCloseAllWindows()
{
    // Arrange - Set up test data
    var manager = new FloatingWindowManager();
    var window1 = new MockFloatingWindow();
    var window2 = new MockFloatingWindow();
    manager.RegisterWindow(window1);
    manager.RegisterWindow(window2);

    // Act - Execute the method under test
    manager.CloseAll();

    // Assert - Verify expected outcomes
    manager.ActiveCount.Should().Be(0);
    window1.IsClosed.Should().BeTrue();
    window2.IsClosed.Should().BeTrue();
}
```

### Testing Frameworks

- **Test Runner**: xUnit 2.9+
- **Assertions**: FluentAssertions 7.0+
- **Mocking**: Moq (when needed for interfaces)
- **Coverage**: Coverlet

### FluentAssertions Examples

```csharp
// ✅ Good - Readable assertions
result.Should().NotBeNull();
list.Should().HaveCount(2);
list.Should().Contain(item);
value.Should().BeGreaterThan(0);
action.Should().Throw<ArgumentNullException>();

// ❌ Bad - Traditional assertions (avoid)
Assert.NotNull(result);
Assert.Equal(2, list.Count);
Assert.Contains(item, list);
Assert.True(value > 0);
Assert.Throws<ArgumentNullException>(() => action());
```

### Running Tests

```powershell
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~FloatingWindowManagerTests"

# Run in watch mode during development
dotnet watch test
```

## Code Analysis

### Enabled Analyzers

1. **StyleCop.Analyzers** - Code style and documentation
2. **Microsoft.CodeAnalysis.NetAnalyzers** - .NET best practices
3. **Nullable Reference Types** - Null safety

### Treating Warnings as Errors

**ShareXwing projects only** treat warnings as errors:

```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

**Rationale**: New code should have zero warnings. ShareX core may have warnings.

### Suppressing Warnings

Only suppress warnings with good reason:

```csharp
// ✅ Good - With justification
#pragma warning disable CA1031 // Do not catch general exception types
    // Win32 APIs can throw various exceptions; catch all for graceful degradation
    catch (Exception ex)
    {
        Logger.LogError(ex, "Failed to create floating window");
        return null;
    }
#pragma warning restore CA1031

// ❌ Bad - No justification
#pragma warning disable CA1031
    catch (Exception) { }
#pragma warning restore CA1031
```

### Nullable Reference Types

All ShareXwing code uses nullable reference types:

```csharp
// ✅ Good
public void RegisterWindow(IFloatingWindow window)  // window is non-nullable
{
    if (window == null)
    {
        throw new ArgumentNullException(nameof(window));
    }
}

public IFloatingWindow? FindWindow(string id)  // Return can be null
{
    return activeWindows.FirstOrDefault(w => w.Id == id);
}

// ❌ Bad
public void RegisterWindow(IFloatingWindow window)  // No null check
{
    activeWindows.Add(window);  // Will crash if null
}
```

## Documentation Standards

### XML Documentation Comments

**Required** for all public APIs in ShareXwing.Core:

```csharp
/// <summary>
/// Registers a floating window with the manager.
/// </summary>
/// <param name="window">The window to register. Cannot be null.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="window"/> is null.
/// </exception>
public void RegisterWindow(IFloatingWindow window)
{
    // Implementation
}
```

**Optional** for:
- Internal members
- Private members
- Test code

### Inline Comments

Use sparingly, prefer self-documenting code:

```csharp
// ✅ Good - Comment explains WHY, not WHAT
// Copy to avoid collection modification during enumeration
var windowsCopy = new List<IFloatingWindow>(activeWindows);

// ❌ Bad - Comment just repeats code
// Create a new list
var windowsCopy = new List<IFloatingWindow>(activeWindows);

// ✅ Better - No comment needed, code is clear
var windowsCopy = new List<IFloatingWindow>(activeWindows);
foreach (var window in windowsCopy)
{
    window.Close();
}
```

### README Files

Each major component should have a README:

```
ShareXwing.Core/
├── FloatingWindows/
│   ├── README.md          # Explains floating window system
│   ├── IFloatingWindow.cs
│   └── ...
```

## Best Practices

### Thread Safety

Document and implement thread safety for shared state:

```csharp
/// <summary>
/// Registers a floating window. Thread-safe.
/// </summary>
public void RegisterWindow(IFloatingWindow window)
{
    lock (lockObject)
    {
        if (!activeWindows.Contains(window))
        {
            activeWindows.Add(window);
        }
    }
}
```

### Exception Handling

```csharp
// ✅ Good - Specific exceptions, no swallowing
public void SaveScreenshot(string path)
{
    if (string.IsNullOrEmpty(path))
    {
        throw new ArgumentException("Path cannot be empty", nameof(path));
    }

    try
    {
        image.Save(path);
    }
    catch (IOException ex)
    {
        Logger.LogError(ex, "Failed to save screenshot to {Path}", path);
        throw;  // Re-throw after logging
    }
}

// ❌ Bad - Swallowing exceptions
try
{
    DoSomething();
}
catch { }  // What happened? User has no idea
```

### LINQ Usage

Prefer LINQ for readability:

```csharp
// ✅ Good
var activeWindows = windows.Where(w => w.IsActive).ToList();

// ❌ Bad (when LINQ would work)
var activeWindows = new List<Window>();
foreach (var window in windows)
{
    if (window.IsActive)
    {
        activeWindows.Add(window);
    }
}
```

### Async/Await

Use async for I/O operations:

```csharp
// ✅ Good
public async Task<Bitmap> LoadImageAsync(string path)
{
    using var stream = File.OpenRead(path);
    return await Task.Run(() => new Bitmap(stream));
}

// ❌ Bad - Blocking I/O
public Bitmap LoadImage(string path)
{
    return new Bitmap(path);  // Blocks UI thread
}
```

### Resource Disposal

Always dispose IDisposable objects:

```csharp
// ✅ Good
using var bitmap = new Bitmap(width, height);
using var graphics = Graphics.FromImage(bitmap);
graphics.DrawImage(sourceImage, rect);

// ❌ Bad - Resource leak
var bitmap = new Bitmap(width, height);
var graphics = Graphics.FromImage(bitmap);
graphics.DrawImage(sourceImage, rect);
// bitmap and graphics never disposed!
```

## Quality Gates

All code must pass these gates before merging:

### Pre-Commit (Local)

Run automatically by Husky:
1. ✅ Code formatting verification (`dotnet format --verify-no-changes`)
2. ✅ Build succeeds (`dotnet build`)
3. ✅ Tests pass (`dotnet test`)

### CI/CD (GitHub Actions)

Run on every push:
1. ✅ Build succeeds (Debug and Release)
2. ✅ Tests pass with >70% coverage
3. ✅ No new warnings
4. ✅ StyleCop rules satisfied

### Pull Request

Before merging:
1. ✅ All CI checks pass
2. ✅ Code review approved
3. ✅ Conflicts resolved
4. ✅ Documentation updated

## Enforcement

### Automated

- **EditorConfig** - IDE enforces formatting rules
- **StyleCop.Analyzers** - Build fails on violations
- **Husky.Net** - Pre-commit hooks block bad commits
- **GitHub Actions** - CI fails on quality issues

### Manual

- **Code Reviews** - Reviewers check adherence to standards
- **Documentation Review** - Ensure docs match implementation

## Summary

ShareXwing's code quality standards ensure:
- ✅ Consistent code style across the project
- ✅ High test coverage and confidence in changes
- ✅ Well-documented public APIs
- ✅ Maintainable, readable codebase
- ✅ Early detection of bugs and issues

Following these standards makes ShareXwing a pleasure to work on and contribute to.

---

**Version**: 1.0
**Last Updated**: 2025-12-08
**Author**: ShareXwing Team
