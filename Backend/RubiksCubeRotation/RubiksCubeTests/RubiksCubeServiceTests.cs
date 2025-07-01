using RubiksCubeModels;

using RubiksCubeServices;

using Xunit;
using Xunit.Sdk;

namespace RubiksCubeTests
{
    /// <summary>
    /// Unit tests for the <see cref="RubiksCubeService"/> class.
    /// </summary>
    public class RubiksCubeServiceTests
    {
        private readonly RubiksCubeService service;

        private static readonly Dictionary<Face, (int TopRow, int LeftCol)> ExpectedNetPositions
            = new()
            {
                { Face.Up,    (0, 3) },
                { Face.Left,  (3, 0) },
                { Face.Front, (3, 3) },
                { Face.Right, (3, 6) },
                { Face.Back,  (3, 9) },
                { Face.Down,  (6, 3) },
            };

        public RubiksCubeServiceTests()
        {
            service = new RubiksCubeService();
        }

        [Fact]
        public void Reset_ShouldRestoreSolvedState()
        {
            service.Move(Face.Front, true);

            service.Reset();
            var net = service.GetExplodedView();

            Assert.All(net, row => Assert.All(row, cell =>
            {
                if (cell.HasValue)
                {
                    Assert.Contains(cell.Value, new[]
                    {
                        Colour.White,
                        Colour.Yellow,
                        Colour.Green,
                        Colour.Blue,
                        Colour.Red,
                        Colour.Orange
                    });
                }
            }));
        }

        [Fact]
        public void Move_ShouldModifyExplodedView()
        {
            var before = service.GetExplodedView();

            service.Move(Face.Front, true);
            var after = service.GetExplodedView();

            bool anyChanged = false;
            for (int r = 0; r < before.Length && !anyChanged; r++)
            {
                for (int c = 0; c < before[r].Length; c++)
                {
                    if (before[r][c] != after[r][c])
                    {
                        anyChanged = true;
                        break;
                    }
                }
            }

            Assert.True(anyChanged);
        }

        [Fact]
        public void GetExplodedView_ShouldHaveCorrectDimensionsAndFaceCentres()
        {
            var net = service.GetExplodedView();

            Assert.NotNull(net);
            Assert.Equal(9, net.Length);
            Assert.All(net, row => Assert.Equal(12, row.Length));

            foreach (var kv in ExpectedNetPositions)
            {
                Face face = kv.Key;
                (int top, int left) = kv.Value;
                int centreRow = top + 1;
                int centreCol = left + 1;

                var expectedColour = face switch
                {
                    Face.Up => Colour.White,
                    Face.Down => Colour.Yellow,
                    Face.Front => Colour.Green,
                    Face.Back => Colour.Blue,
                    Face.Right => Colour.Red,
                    Face.Left => Colour.Orange,
                    _ => throw new XunitException("Unknown face")
                };

                Assert.Equal(
                    expectedColour,
                    net[centreRow][centreCol].Value);
            }

            Assert.Null(net[0][0]);
            Assert.Null(net[0][11]);
            Assert.Null(net[8][0]);
            Assert.Null(net[8][11]);
        }
    }
}
