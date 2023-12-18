using FluentAssertions;
using Xunit;

namespace Codingame.FallChallenge2023.Business.Tests
{
    public class Tests
    {
        [Fact]
        void DistanceTest()
        {
            var p1 = new Position() { X = 3, Y = 4 };
            var p2 = new Position() { X = 6, Y = 8 };

            p1.DistanceTo(p2).Should().Be(5);
        }
    }
}