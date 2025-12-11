using System;
using System.Drawing;
using FluentAssertions;
using ShareXwing.Core.QuickAccess;
using Xunit;

namespace ShareXwing.Tests.QuickAccess
{
    /// <summary>
    /// Tests for QuickAccessManager
    /// </summary>
    public class QuickAccessManagerTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var manager = new QuickAccessManager();

            // Assert
            manager.IsOverlayVisible.Should().BeFalse();
            manager.AutoDismissTimeout.Should().Be(5000);
        }

        [Fact]
        public void ShowOverlay_ShouldThrowArgumentNullException_WhenImageIsNull()
        {
            // Arrange
            var manager = new QuickAccessManager();

            // Act
            Action act = () => manager.ShowOverlay(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("image");
        }

        [Fact]
        public void HideOverlay_ShouldNotThrow_WhenNoOverlayVisible()
        {
            // Arrange
            var manager = new QuickAccessManager();

            // Act
            Action act = () => manager.HideOverlay();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void AutoDismissTimeout_ShouldUpdateValue_WhenSet()
        {
            // Arrange
            var manager = new QuickAccessManager();

            // Act
            manager.AutoDismissTimeout = 10000;

            // Assert
            manager.AutoDismissTimeout.Should().Be(10000);
        }

        [Fact]
        public void AutoDismissTimeout_ShouldAllowZero_ForNoAutoDismiss()
        {
            // Arrange
            var manager = new QuickAccessManager();

            // Act
            manager.AutoDismissTimeout = 0;

            // Assert
            manager.AutoDismissTimeout.Should().Be(0);
        }
    }
}
