using System;
using FluentAssertions;
using ShareXwing.Core.Capture;
using Xunit;

namespace ShareXwing.Tests.Capture
{
    /// <summary>
    /// Tests for CaptureSelectorManager
    /// </summary>
    public class CaptureSelectorManagerTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var manager = new CaptureSelectorManager();

            // Assert
            manager.IsSelectorVisible.Should().BeFalse();
        }

        [Fact]
        public void HideSelector_ShouldNotThrow_WhenNoSelectorVisible()
        {
            // Arrange
            var manager = new CaptureSelectorManager();

            // Act
            Action act = () => manager.HideSelector();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void IsSelectorVisible_ShouldBeFalse_Initially()
        {
            // Arrange
            var manager = new CaptureSelectorManager();

            // Act & Assert
            manager.IsSelectorVisible.Should().BeFalse();
        }
    }
}
