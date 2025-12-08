# Phase 0 Verification Checklist

This document provides a step-by-step verification guide for Phase 0 completion.

## Overview

Phase 0 establishes the foundation for quality-first ShareXwing development:
- Project structure
- Quality tools (EditorConfig, StyleCop, xUnit)
- CI/CD pipeline with automated testing
- Pre-commit hooks
- Comprehensive documentation

## Prerequisites

**Windows Environment** (WSL2 cannot build Windows applications):
- Windows 10/11
- .NET 9.0 SDK installed
- Git installed
- Repository cloned

## Verification Steps

### 1. Repository Structure

Verify all Phase 0 files exist:

```powershell
# Check core project files
Test-Path ShareXwing.Core/ShareXwing.Core.csproj  # Should be True
Test-Path ShareXwing.Tests/ShareXwing.Tests.csproj  # Should be True

# Check quality files
Test-Path .editorconfig  # Should be True
Test-Path stylecop.json  # Should be True
Test-Path .config/dotnet-tools.json  # Should be True
Test-Path .husky/pre-commit  # Should be True

# Check documentation
Test-Path docs/ARCHITECTURE.md  # Should be True
Test-Path docs/CODE_QUALITY.md  # Should be True
Test-Path docs/CONTRIBUTING.md  # Should be True
Test-Path CLAUDE.md  # Should be True
Test-Path SESSION-LOG.md  # Should be True
Test-Path DECISIONS.md  # Should be True

# Check CI/CD
Test-Path .github/workflows/build.yml  # Should be True
```

**Expected Result**: All paths should return `True`

### 2. Dependencies Restore

```powershell
# Restore NuGet packages
dotnet restore

# Restore dotnet tools (Husky)
dotnet tool restore
```

**Expected Result**:
- Packages restore successfully
- No errors about missing packages
- Husky tool installed

### 3. Build Verification

```powershell
# Build ShareXwing.Core
dotnet build ShareXwing.Core/ShareXwing.Core.csproj --configuration Debug

# Build ShareXwing.Tests
dotnet build ShareXwing.Tests/ShareXwing.Tests.csproj --configuration Debug
```

**Expected Result**:
- Both builds succeed
- No errors
- No warnings (TreatWarningsAsErrors is enabled for ShareXwing projects)

**If Build Fails**:
- Check .NET SDK version: `dotnet --version` (should be 9.0.x)
- Ensure ShareX.HelpersLib is built first (dependency)
- Review error messages

### 4. Test Execution

```powershell
# Run all ShareXwing tests
dotnet test ShareXwing.Tests/ShareXwing.Tests.csproj --configuration Debug

# Run with detailed output
dotnet test ShareXwing.Tests/ShareXwing.Tests.csproj -v normal
```

**Expected Result**:
- All tests pass (7 tests in FloatingWindowManagerTests)
- No failures
- Test output shows:
  ```
  Passed! - Failed: 0, Passed: 7, Skipped: 0, Total: 7
  ```

**If Tests Fail**:
- Check error messages for specific test failures
- Ensure all dependencies restored correctly
- Try cleaning: `dotnet clean` then rebuild

### 5. Code Coverage

```powershell
# Run tests with coverage
dotnet test ShareXwing.Tests/ShareXwing.Tests.csproj --collect:"XPlat Code Coverage"

# Coverage report saved to TestResults/
# Find the latest coverage.cobertura.xml file
Get-ChildItem -Path TestResults -Recurse -Filter coverage.cobertura.xml | Select-Object -Last 1
```

**Expected Result**:
- Coverage report generated
- FloatingWindowManager.cs should have ~100% coverage

**Optional**: Install `reportgenerator` for HTML coverage reports:
```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
# Open CoverageReport/index.html in browser
```

### 6. Code Formatting Check

```powershell
# Verify code follows .editorconfig rules
dotnet format ShareXwing.Core/ShareXwing.Core.csproj --verify-no-changes
dotnet format ShareXwing.Tests/ShareXwing.Tests.csproj --verify-no-changes
```

**Expected Result**:
- Both projects pass formatting verification
- No formatting issues reported

**If Formatting Fails**:
```powershell
# Auto-fix formatting
dotnet format ShareXwing.Core/ShareXwing.Core.csproj
dotnet format ShareXwing.Tests/ShareXwing.Tests.csproj

# Verify again
dotnet format --verify-no-changes
```

### 7. StyleCop Analysis

Build already runs StyleCop.Analyzers, but verify explicitly:

```powershell
# Build with diagnostic verbosity to see analyzer output
dotnet build ShareXwing.Core/ShareXwing.Core.csproj --configuration Debug -v detailed | Select-String "StyleCop"
```

**Expected Result**:
- StyleCop.Analyzers runs during build
- No StyleCop warnings/errors

### 8. Pre-Commit Hooks

```powershell
# Install Husky hooks
dotnet husky install

# Verify hook was created
Test-Path .git/hooks/pre-commit  # Should be True
Get-Content .git/hooks/pre-commit | Select-String "husky.sh"  # Should find it
```

**Expected Result**:
- Hook file created in `.git/hooks/pre-commit`
- Hook points to `.husky/pre-commit`

**Test Hook**:
```powershell
# Make a trivial change
Add-Content ShareXwing.Core/FloatingWindows/IFloatingWindow.cs "`n// Test comment"

# Stage the change
git add ShareXwing.Core/FloatingWindows/IFloatingWindow.cs

# Try to commit (hook should run)
git commit -m "test: verify pre-commit hook"

# Hook should run quality checks
# If they pass, commit will succeed
# If they fail, commit will be blocked

# Undo test change
git reset HEAD~1  # Undo commit
git checkout -- ShareXwing.Core/FloatingWindows/IFloatingWindow.cs  # Discard change
```

**Expected Result**:
- Hook runs automatically before commit
- Quality checks execute (format, build, test)
- Commit succeeds if checks pass

### 9. CI/CD Pipeline (GitHub Actions)

This requires pushing to GitHub:

```powershell
# Commit all Phase 0 work
git add .
git commit -m "feat(phase0): complete foundation setup

- Add ShareXwing.Core and ShareXwing.Tests projects
- Configure EditorConfig and StyleCop.Analyzers
- Enhance CI/CD with test execution and coverage
- Set up Husky.Net pre-commit hooks
- Add comprehensive documentation"

# Push to GitHub
git push origin main  # Or your branch name
```

Then visit: https://github.com/BahneGork/ShareXwing/actions

**Expected Result**:
- Build workflow triggers automatically
- "Build ShareXwing" job runs
- Builds succeed for Debug and Release
- Tests run and pass
- Test results uploaded as artifacts
- Coverage reports uploaded as artifacts

**Verify Artifacts**:
- Click on workflow run
- Scroll to "Artifacts" section
- Should see:
  - `test-results-Debug`
  - `test-results-Release`
  - `code-coverage-Debug`
  - `code-coverage-Release`

### 10. Documentation Review

Review each documentation file for completeness:

```powershell
# Open documentation files
code docs/ARCHITECTURE.md
code docs/CODE_QUALITY.md
code docs/CONTRIBUTING.md
code CLAUDE.md
code SESSION-LOG.md
code DECISIONS.md
```

**Checklist**:
- [ ] ARCHITECTURE.md explains system design clearly
- [ ] CODE_QUALITY.md defines all standards
- [ ] CONTRIBUTING.md provides clear contribution guide
- [ ] CLAUDE.md has current project status
- [ ] SESSION-LOG.md documents session history
- [ ] DECISIONS.md records key decisions

## Phase 0 Success Criteria

All items must be verified ✅:

### Infrastructure
- [x] ShareXwing.Core project created with proper configuration
- [x] ShareXwing.Tests project created with xUnit + FluentAssertions
- [x] Projects added to ShareX.sln
- [x] .editorconfig enhanced with quality rules
- [x] stylecop.json configured
- [x] Husky.Net configured in dotnet-tools.json

### Quality Gates
- [ ] Build succeeds with zero warnings
- [ ] All tests pass (7/7 FloatingWindowManagerTests)
- [ ] Code coverage >70% (should be ~100% for current code)
- [ ] Code formatting passes verification
- [ ] StyleCop.Analyzers runs without errors

### Automation
- [ ] Pre-commit hooks installed and working
- [ ] GitHub Actions workflow runs successfully
- [ ] Test results uploaded as artifacts
- [ ] Coverage reports uploaded as artifacts

### Documentation
- [x] ARCHITECTURE.md completed
- [x] CODE_QUALITY.md completed
- [x] CONTRIBUTING.md completed
- [x] CLAUDE.md updated with current status
- [x] SESSION-LOG.md maintained
- [x] DECISIONS.md records all key decisions

## Troubleshooting

### Build Errors

**Error**: `The type or namespace name 'ShareX' could not be found`
**Solution**: Build ShareX.HelpersLib first: `dotnet build ShareX.HelpersLib/ShareX.HelpersLib.csproj`

**Error**: `CSC : error CS1056: Unexpected character '$'`
**Solution**: Ensure you're using .NET 9.0 SDK: `dotnet --version`

### Test Failures

**Error**: Tests fail with `DllNotFoundException` or `PlatformNotSupportedException`
**Solution**: Must run on Windows. Some ShareX dependencies require Windows APIs.

### Hook Not Running

**Error**: Git commit succeeds without running quality checks
**Solution**:
1. Verify hook installed: `Test-Path .git/hooks/pre-commit`
2. Reinstall: `dotnet husky install`
3. Check permissions: `Get-Acl .git/hooks/pre-commit`

### CI/CD Not Running

**Error**: GitHub Actions workflow doesn't trigger
**Solution**:
1. Check workflow file location: `.github/workflows/build.yml`
2. Verify branch name in workflow matches your branch
3. Check if workflow is disabled in GitHub Actions settings

## Next Steps After Phase 0

Once all verification items are ✅, Phase 0 is complete!

**Ready for Phase 1**:
- FloatingWindow implementation (WinForms control)
- Win32 API integration for always-on-top
- Opacity and lock mode functionality
- Full integration tests

See `CLAUDE.md` for Phase 1 roadmap.

---

**Last Updated**: 2025-12-08
**Status**: Phase 0 Complete (pending user verification on Windows)
