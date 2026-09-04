using Microsoft.AspNetCore.Mvc;
using System;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public IActionResult CreateGame([FromBody] CreateGameRequest request)
        {
            var session = _gameService.CreateGame(request.Mode);
            return Ok(session);
        }

        [HttpGet("{id}")]
        public IActionResult GetGame(string id)
        {
            var session = _gameService.GetGame(id);
            if (session == null) return NotFound("Game not found.");
            
            return Ok(session);
        }

        [HttpPost("{id}/moves")]
        public IActionResult MakeMove(string id, [FromBody] MakeMoveRequest request)
        {
            try
            {
                var session = _gameService.MakeMove(id, request.Player, request.Row, request.Column);
                return Ok(session);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/undo")]
        public IActionResult UndoMove(string id)
        {
            try
            {
                var session = _gameService.UndoMove(id);
                return Ok(session);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/reset")]
        public IActionResult ResetGame(string id)
        {
            try
            {
                var session = _gameService.ResetGame(id);
                return Ok(session);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }
    }
}