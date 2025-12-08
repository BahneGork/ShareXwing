using FluentAssertions;
using ShareXwing.Core.FloatingWindows;
using Xunit;

namespace ShareXwing.Tests.FloatingWindows;

public class FloatingWindowManagerTests
{
    [Fact]
    public void ActiveCount_ShouldBeZero_WhenNoWindowsRegistered()
    {
        // Arrange
        var manager = new FloatingWindowManager();

        // Act & Assert
        manager.ActiveCount.Should().Be(0);
    }

    [Fact]
    public void RegisterWindow_ShouldIncreaseActiveCount()
    {
        // Arrange
        var manager = new FloatingWindowManager();
        var window = new MockFloatingWindow();

        // Act
        manager.RegisterWindow(window);

        // Assert
        manager.ActiveCount.Should().Be(1);
    }

    [Fact]
    public void RegisterWindow_ShouldNotAddDuplicates()
    {
        // Arrange
        var manager = new FloatingWindowManager();
        var window = new MockFloatingWindow();

        // Act
        manager.RegisterWindow(window);
        manager.RegisterWindow(window);

        // Assert
        manager.ActiveCount.Should().Be(1);
    }

    [Fact]
    public void UnregisterWindow_ShouldDecreaseActiveCount()
    {
        // Arrange
        var manager = new FloatingWindowManager();
        var window = new MockFloatingWindow();
        manager.RegisterWindow(window);

        // Act
        manager.UnregisterWindow(window);

        // Assert
        manager.ActiveCount.Should().Be(0);
    }

    [Fact]
    public void CloseAll_ShouldCloseAllWindows()
    {
        // Arrange
        var manager = new FloatingWindowManager();
        var window1 = new MockFloatingWindow();
        var window2 = new MockFloatingWindow();
        manager.RegisterWindow(window1);
        manager.RegisterWindow(window2);

        // Act
        manager.CloseAll();

        // Assert
        manager.ActiveCount.Should().Be(0);
        window1.IsClosed.Should().BeTrue();
        window2.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void GetActiveWindows_ShouldReturnAllRegisteredWindows()
    {
        // Arrange
        var manager = new FloatingWindowManager();
        var window1 = new MockFloatingWindow();
        var window2 = new MockFloatingWindow();
        manager.RegisterWindow(window1);
        manager.RegisterWindow(window2);

        // Act
        var windows = manager.GetActiveWindows();

        // Assert
        windows.Should().HaveCount(2);
        windows.Should().Contain(window1);
        windows.Should().Contain(window2);
    }

    [Fact]
    public void RegisterWindow_ShouldThrowArgumentNullException_WhenWindowIsNull()
    {
        // Arrange
        var manager = new FloatingWindowManager();

        // Act
        Action act = () => manager.RegisterWindow(null!);

        // Assert
        act.Should().Throw<System.ArgumentNullException>();
    }

    // Mock implementation for testing
    private class MockFloatingWindow : IFloatingWindow
    {
        public double CurrentOpacity { get; private set; } = 1.0;
        public bool IsLocked { get; private set; }
        public bool IsClosed { get; private set; }

        public void SetOpacity(double opacity)
        {
            CurrentOpacity = System.Math.Clamp(opacity, 0.1, 1.0);
        }

        public void SetLocked(bool locked)
        {
            IsLocked = locked;
        }

        public void Close()
        {
            IsClosed = true;
        }
    }
}
