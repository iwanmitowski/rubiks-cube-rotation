using RubiksCubeModels;

namespace RubiksCubeServices
{
    public interface IRubiksCubeService
    {
        public void Reset();

        public void Move(Face face, bool clockwise);

        public Colour?[][] GetExplodedView();
    }
}
