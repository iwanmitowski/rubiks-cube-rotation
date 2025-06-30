using RubiksCubeModels;

namespace RubiksCubeServices
{
    public class RubiksCubeService : IRubiksCubeService
    {
        private const int NetRows = 9;
        private const int NetCols = 12;
        private const int FaceSize = 3;

        /// <summary>
        /// Static map of where each face’s top-left corner sits in the 9×12 net
        /// </summary>
        private static readonly Dictionary<Face, (int TopRow, int LeftCol)> NetPositions =
            new()
            {
                [Face.Up] = (0, 3),
                [Face.Left] = (3, 0),
                [Face.Front] = (3, 3),
                [Face.Right] = (3, 6),
                [Face.Back] = (3, 9),
                [Face.Down] = (6, 3),
            };

        private Cube cube;

        // TODO obtain cube from repo by user id, now load the service as singleton to keep this cube
        // Later load as transient
        public RubiksCubeService()
        {
            cube = new Cube();
        }

        /// <summary>
        /// Restores the cube to its initial, solved state.
        /// </summary>
        public void Reset()
        {
            cube = new Cube();
        }

        /// <summary>
        /// Performs exactly one 90° turn of the given face.
        /// </summary>
        /// <param name="face">Which face to turn (Up, Down, Front, Back, Left, Right).</param>
        /// <param name="clockwise">
        ///   True for a clockwise 90° turn, false for counter-clockwise.
        /// </param>
        public void Move(Face face, bool clockwise)
        {
            cube.Rotate(face, clockwise);
        }

        /// <summary>
        /// Builds a 9×12 “net” view of the cube’s faces in an exploded layout:
        /// 
        ///       [   Up  ]
        /// [Left][ Front ][Right][ Back ]
        ///       [  Down ]
        ///
        /// Null entries represent empty corners.
        /// </summary>
        /// <returns>
        /// A 2D array (9 rows × 12 cols) where each cell is either a Colour
        /// or null if it’s outside the net.
        /// </returns>
        public Colour?[][] GetExplodedView()
        {
            var faceColours = cube.GetState();

            var netLayout = new Colour?[NetRows][];
            for (int row = 0; row < NetRows; row++)
            {
                netLayout[row] = new Colour?[NetCols];
            }

            foreach (var (face, (topRow, leftCol)) in NetPositions)
            {
                for (int r = 0; r < FaceSize; r++)
                {
                    for (int c = 0; c < FaceSize; c++)
                    {
                        netLayout[topRow + r][leftCol + c] =
                            faceColours[(int)face, r, c];
                    }
                }
            }

            return netLayout;
        }
    }
}
