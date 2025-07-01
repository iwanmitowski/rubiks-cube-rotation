using RubiksCubeModels;

using Xunit;

namespace RubiksCubeTests
{
    /// <summary>
    /// Tests covering the <see cref="Cube.Rotate"/> method behavior across all faces.
    /// </summary>
    public class CubeRotationTests
    {
        private readonly Cube cube;

        /// <summary>
        /// Initializes a new instance of <see cref="CubeRotationTests"/>, creating a fresh cube for each test.
        /// </summary>
        public CubeRotationTests()
        {
            cube = new Cube();
        }

        /// <summary>
        /// Verifies that rotating any face 90° clockwise four times returns the cube to its original state.
        /// </summary>
        [Theory]
        [InlineData(Face.Up)]
        [InlineData(Face.Down)]
        [InlineData(Face.Front)]
        [InlineData(Face.Back)]
        [InlineData(Face.Right)]
        [InlineData(Face.Left)]
        public void Rotate_FourTimesClockwise_ShouldReturnToOriginalState(Face face)
        {
            var original = cube.GetState();

            for (int turnCount = 0; turnCount < 4; turnCount++)
            {
                cube.Rotate(face, true);
            }
            var result = cube.GetState();

            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                for (int rowIndex = 0; rowIndex < 3; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < 3; colIndex++)
                    {
                        Assert.Equal(
                            original[faceIndex, rowIndex, colIndex],
                            result[faceIndex, rowIndex, colIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// Verifies that rotating any face clockwise then counter-clockwise restores the original state.
        /// </summary>
        [Theory]
        [InlineData(Face.Up)]
        [InlineData(Face.Down)]
        [InlineData(Face.Front)]
        [InlineData(Face.Back)]
        [InlineData(Face.Right)]
        [InlineData(Face.Left)]
        public void Rotate_ClockwiseThenCounterClockwise_ShouldReturnToOriginalState(Face face)
        {
            var original = cube.GetState();

            cube.Rotate(face, true);
            cube.Rotate(face, false);
            var result = cube.GetState();

            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                for (int rowIndex = 0; rowIndex < 3; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < 3; colIndex++)
                    {
                        Assert.Equal(
                            original[faceIndex, rowIndex, colIndex],
                            result[faceIndex, rowIndex, colIndex]);
                    }
                }
            }
        }

        [Fact]
        public void Rotate_InvalidFace_Throws()
        {
            var cube = new Cube();
            Assert.Throws<IndexOutOfRangeException>(() => cube.Rotate((Face)99, true));
        }
    }
}
