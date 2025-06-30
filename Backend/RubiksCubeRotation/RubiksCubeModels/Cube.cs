namespace RubiksCubeModels
{
    public class Cube
    {
        private readonly Colour[,,] faces = new Colour[6, 3, 3];

        /// <summary>
        /// Cube class representing the Rubik's Cube
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Cube()
        {
            for (int f = 0; f < 6; f++)
            {
                Colour faceColor = f switch
                {
                    (int)Face.Up => Colour.White,
                    (int)Face.Down => Colour.Yellow,
                    (int)Face.Front => Colour.Green,
                    (int)Face.Back => Colour.Blue,
                    (int)Face.Right => Colour.Red,
                    (int)Face.Left => Colour.Orange,
                    _ => throw new ArgumentOutOfRangeException()
                };

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        faces[f, i, j] = faceColor;
                    }
                }
            }
        }

        /// <summary>
        /// Rotate the given face 90°
        /// </summary>
        /// <param name="face">The desired face.</param>
        /// <param name="clockwise">If it must be clockwise or not.</param>
        public void Rotate(Face face, bool clockwise)
        {
            int faceIndex = (int)face;
            RotateFaceTiles(faceIndex, clockwise);
            RotateAdjacentEdges(face, clockwise);
        }

        /// <summary>
        /// Rotates the 3×3 tile grid of a single face 90° in the given direction.
        /// </summary>
        /// <param name="faceIndex">Index of the face to rotate (0 = Up, 1 = Down, 2 = Front, …).</param>
        /// <param name="isClockwise">True for a clockwise turn; false for counter-clockwise.</param>
        private void RotateFaceTiles(int faceIndex, bool isClockwise)
        {
            var originalTiles = new Colour[3, 3];
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    originalTiles[row, col] = faces[faceIndex, row, col];
                }
            }

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (isClockwise)
                    {
                        // (r, c) → (c, 2-r)
                        faces[faceIndex, col, 2 - row] = originalTiles[row, col];
                    }
                    else
                    {
                        // (r, c) → (2-c, r)
                        faces[faceIndex, 2 - col, row] = originalTiles[row, col];
                    }
                }
            }
        }

        /// <summary>
        /// After rotating a face’s own 3×3 tile grid, this method shifts the bordering
        /// edge‐strips on the four adjacent faces to follow the turn.
        /// </summary>
        /// <param name="face">The face that was turned (Front, Back, Left, Right, Up, Down).</param>
        /// <param name="clockwise">
        ///   True to rotate the adjacent strips in the same direction as a clockwise turn;
        ///   false for a counter-clockwise shift.
        /// </param>
        private void RotateAdjacentEdges(Face face, bool clockwise)
        {
            // Lookup table: for each face, the four neighbouring strips in
            // clockwise order. Each entry is:
            //   neighbourFace – which face we’re pulling the strip from
            //   fixedIndex    – which row/column of that face
            //   isRow         – true if it’s a horizontal row, false if it’s a vertical column
            var adjacencyMap = new Dictionary<Face, (Face neighbourFace, int fixedIndex, bool isRow)[]>
            {
                [Face.Front] = new[]
                {
                    (Face.Up,    2, true),   // bottom row of Up
                    (Face.Right, 0, false),  // left  col of Right
                    (Face.Down,  0, true),   // top    row of Down
                    (Face.Left,  2, false),  // right col of Left
                },
                [Face.Back] = new[]
                {
                    (Face.Up,    0, true),   // top    row of Up
                    (Face.Left,  0, false),  // left   col of Left
                    (Face.Down,  2, true),   // bottom row of Down
                    (Face.Right, 2, false),  // right  col of Right
                },
                [Face.Left] = new[]
                {
                    (Face.Up,    0, false),  // left   col of Up
                    (Face.Front, 0, false),  // left   col of Front
                    (Face.Down,  0, false),  // left   col of Down
                    (Face.Back,  2, false),  // right  col of Back
                },
                [Face.Right] = new[]
                {
                    (Face.Up,    2, false),  // right  col of Up
                    (Face.Back,  0, false),  // left   col of Back
                    (Face.Down,  2, false),  // right  col of Down
                    (Face.Front, 2, false),  // right  col of Front
                },
                [Face.Up] = new[]
                {
                    (Face.Back,  0, true),   // top    row of Back
                    (Face.Right, 0, true),   // top    row of Right
                    (Face.Front, 0, true),   // top    row of Front
                    (Face.Left,  0, true),   // top    row of Left
                },
                [Face.Down] = new[]
                {
                    (Face.Front, 2, true),   // bottom row of Front
                    (Face.Right, 2, true),   // bottom row of Right
                    (Face.Back,  2, true),   // bottom row of Back
                    (Face.Left,  2, true),   // bottom row of Left
                },
            };

            // Retrieve the 4 neighbour definitions for the turned face
            var neighbourStrips = adjacencyMap[face];

            // 1) Extract each strip into a temporary array
            var extractedStrips = new Colour[4][];
            for (int stripIndex = 0; stripIndex < 4; stripIndex++)
            {
                var (neighbourFace, fixedIndex, isRow) = neighbourStrips[stripIndex];
                var strip = new Colour[3];

                for (int offset = 0; offset < 3; offset++)
                {
                    strip[offset] = isRow
                        ? faces[(int)neighbourFace, fixedIndex, offset]
                        : faces[(int)neighbourFace, offset, fixedIndex];
                }

                extractedStrips[stripIndex] = strip;
            }

            // 2) Determine shift direction: +1 for CW, -1 for CCW
            int shift = clockwise ? +1 : -1;

            // 3) Write each strip back into its new position
            for (int targetIndex = 0; targetIndex < 4; targetIndex++)
            {
                // Compute which original strip moves here
                var sourceStrip = extractedStrips[(targetIndex - shift + 4) % 4];
                var (neighbourFace, fixedIndex, isRow) = neighbourStrips[targetIndex];

                for (int offset = 0; offset < 3; offset++)
                {
                    if (isRow)
                    {
                        faces[(int)neighbourFace, fixedIndex, offset] = sourceStrip[offset];
                    }
                    else
                    {
                        faces[(int)neighbourFace, offset, fixedIndex] = sourceStrip[offset];
                    }
                }
            }
        }

        /// <summary>
        /// Returns copy of the cube state
        /// </summary>
        /// <returns></returns>
        public Colour[,,] GetState() => (Colour[,,])faces.Clone();
    }
}