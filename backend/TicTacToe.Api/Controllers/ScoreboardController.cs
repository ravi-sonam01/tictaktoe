using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreboardController : ControllerBase
    {
        private readonly IGameService _gameService;

        public ScoreboardController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public IActionResult GetScoreboard()
        {
            return Ok(_gameService.GetScoreboard());
        }

        [HttpPost("reset")]
        public IActionResult ResetScoreboard()
        {
            _gameService.ResetScoreboard();
            return Ok(_gameService.GetScoreboard());
        }
    }
}