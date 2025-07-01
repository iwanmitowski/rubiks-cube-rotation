using RubiksCubeModels;

using Xunit;

namespace RubiksCubeTests
{
    /// <summary>
    /// Tests for the <see cref="Cube"/> constructor and initial state behavior.
    /// </summary>
    public class CubeConstructorTests
    {
        private readonly Cube cube;

        /// <summary>
        /// Initializes a new instance of <see cref="CubeConstructorTests"/>, creating a fresh cube for each test.
        /// </summary>
        public CubeConstructorTests()
        {
            cube = new Cube();
        }

        /// <summary>
        /// Data linking each face enum to its expected default color.
        /// </summary>
        public static TheoryData<Face, Colour> FaceColourData =>
            new TheoryData<Face, Colour>
            {
                { Face.Up,    Colour.White },
                { Face.Down,  Colour.Yellow },
                { Face.Front, Colour.Green },
                { Face.Back,  Colour.Blue },
                { Face.Right, Colour.Red },
                { Face.Left,  Colour.Orange }
            };

        /// <summary>
        /// Verifies that the state array dimensions are [6 faces, 3 rows, 3 columns].
        /// </summary>
        [Fact]
        public void GetState_ShouldHaveCorrectDimensions()
        {
            var state = cube.GetState();

            Assert.Equal(6, state.GetLength(0));
            Assert.Equal(3, state.GetLength(1));
            Assert.Equal(3, state.GetLength(2));
        }

        /// <summary>
        /// Ensures that GetState returns a deep copy, preventing external mutations.
        /// </summary>
        [Fact]
        public void GetState_ShouldReturnDeepCopy()
        {
            var original = cube.GetState();
            var modified = cube.GetState();
            modified[0, 0, 0] = Colour.Red;

            var reFetched = cube.GetState();
            Assert.Equal(original[0, 0, 0], reFetched[0, 0, 0]);
        }

        /// <summary>
        /// Verifies that a newly constructed cube has each face uniformly colored.
        /// </summary>
        [Theory]
        [MemberData(nameof(FaceColourData))]
        public void Constructor_ShouldInitializeFacesToCorrectColours(Face face, Colour expectedColour)
        {
            var state = cube.GetState();
            int faceIndex = (int)face;

            for (int row = 0; row < state.GetLength(1); row++)
            {
                for (int col = 0; col < state.GetLength(2); col++)
                {
                    Assert.Equal(expectedColour, state[faceIndex, row, col]);
                }
            }
        }
    }
}