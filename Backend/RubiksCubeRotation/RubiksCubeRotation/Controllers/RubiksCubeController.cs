using Microsoft.AspNetCore.Mvc;

using RubiksCubeApiModels;

using RubiksCubeModels;

using RubiksCubeServices;

namespace RubiksCubeRotation.Controllers
{
    /// <summary>
    /// API controller for performing operations on a Rubik’s Cube.
    /// Exposes endpoints to reset the cube, make individual moves,
    /// and retrieve the exploded (net) view of its current state.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RubiksCubeController : ControllerBase
    {
        private readonly IRubiksCubeService rubiksCubeService;

        /// <summary>
        /// Constructs a new <see cref="RubiksCubeController"/>.
        /// </summary>
        /// <param name="rubiksCubeService">
        /// Service instance that encapsulates cube operations (reset, move, view).
        /// </param>
        public RubiksCubeController(IRubiksCubeService rubiksCubeService)
        {
            this.rubiksCubeService = rubiksCubeService;
        }

        /// <summary>
        /// Resets the cube to its solved state.
        /// </summary>
        /// <returns>
        /// HTTP 204 No Content on success.
        /// </returns>
        [HttpPost("reset")]
        public IActionResult Reset()
        {
            this.rubiksCubeService.Reset();
            return NoContent();
        }

        /// <summary>
        /// Performs a single 90° rotation of the specified face.
        /// </summary>
        /// <param name="request">
        /// Contains the face to rotate and direction (clockwise or counter-clockwise).
        /// </param>
        /// <returns>
        /// HTTP 204 No Content on success.
        /// </returns>
        [HttpPost("move")]
        public IActionResult Move([FromBody] MoveRequest request)
        {
            this.rubiksCubeService.Move(request.Face, request.Clockwise);
            return NoContent();
        }

        /// <summary>
        /// Retrieves the current exploded (“net”) view of the cube as a 2D grid.
        /// </summary>
        /// <returns>
        /// HTTP 200 OK with a 9×12 grid of <see cref="Colour"/> values (nullable),
        /// representing the unfolded layout of all six faces.
        /// </returns>
        [HttpGet("exploded")]
        public ActionResult<Colour?[,]> GetExplodedView()
        {
            var view = this.rubiksCubeService.GetExplodedView();
            return Ok(view);
        }
    }
}
