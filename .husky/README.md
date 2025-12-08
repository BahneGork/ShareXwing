# Husky Pre-Commit Hooks

This directory contains Git pre-commit hooks for ShareXwing quality assurance.

## Setup

**Note:** Husky hooks require Windows to run (cannot be set up in WSL2 for this project).

### First-Time Setup (Windows)

1. Restore dotnet tools:
   ```powershell
   dotnet tool restore
   ```

2. Install Husky:
   ```powershell
   dotnet husky install
   ```

3. Verify hook is installed:
   ```powershell
   # Check that .git/hooks/pre-commit exists and points to .husky/pre-commit
   cat .git/hooks/pre-commit
   ```

## What the Hook Does

The pre-commit hook runs automatically when you commit code. It performs these quality checks **only on ShareXwing projects**:

1. **Code Formatting** - Verifies code follows .editorconfig and StyleCop rules
2. **Build Verification** - Ensures ShareXwing.Core builds successfully
3. **Test Execution** - Runs all ShareXwing unit tests

If any check fails, the commit is blocked. Fix the issues and try again.

## Manual Quality Check

Run quality checks manually without committing:

```powershell
# Check formatting
dotnet format ShareXwing.Core/ShareXwing.Core.csproj --verify-no-changes

# Build
dotnet build ShareXwing.Core/ShareXwing.Core.csproj --configuration Debug

# Run tests
dotnet test ShareXwing.Tests/ShareXwing.Tests.csproj --configuration Debug
```

## Auto-Fix Formatting Issues

```powershell
dotnet format ShareXwing.Core/ShareXwing.Core.csproj
dotnet format ShareXwing.Tests/ShareXwing.Tests.csproj
```

## Bypassing Hooks (Not Recommended)

Only in emergencies:

```powershell
git commit --no-verify -m "Emergency commit message"
```

**Warning:** Bypassing hooks may cause CI/CD failures.

## Troubleshooting

### Hook not running?

```powershell
# Reinstall Husky
dotnet husky install
```

### Hook running on ShareX files?

The hook is configured to only run checks when ShareXwing files are staged. If you're changing ShareX core files, the hook should skip quality checks.

### Tests failing in hook but passing manually?

Ensure you're using the same .NET SDK version:

```powershell
dotnet --version  # Should be 9.0.x
```

## CI/CD Integration

These same quality checks run on GitHub Actions for every push. The pre-commit hook helps catch issues earlier in the development cycle.
