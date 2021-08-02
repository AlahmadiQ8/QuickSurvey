using FluentAssertions;
using QuickSurvey.Web.Authentication;
using Xunit;

namespace QuickSurvey.Tests
{
    public class BasicObfuscatorTests
    {
        private readonly BasicObfuscator _basicObfuscator = new();


        [Fact]
        public void Obfuscate_NormalInputString_ObfuscatesCorrectly()
        {
            // Arrange

            // Act
            var obfuscated = _basicObfuscator.Obfuscate(31, "Mohammad");

            // Assert
            var result = _basicObfuscator.DeObfuscate(obfuscated);
            result.sessionId.Should().Be(31);
            result.username.Should().Be("Mohammad");
        }
    }
}
