using RubiksCubeModels;

namespace RubiksCubeApiModels
{
    public class MoveRequest
    {
        /// <summary>
        /// Which face to turn (Up, Down, Left, Right, Front, Back).
        /// </summary>
        public Face Face { get; set; }

        /// <summary>
        /// True for clockwise 90°; false for counter-clockwise.
        /// </summary>
        public bool Clockwise { get; set; }
    }
}
