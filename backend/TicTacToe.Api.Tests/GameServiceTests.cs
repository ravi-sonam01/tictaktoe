using System;
using System.Linq;
using Xunit;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Tests
{
    public class GameServiceTests
    {
        private readonly GameService _service;

        public GameServiceTests()
        {
            _service = new GameService();
        }

        [Fact]
        public void MakeMove_ValidMove_UpdatesBoardAndSwitchesTurn()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.TwoPlayer);

            // Act
            _service.MakeMove(game.GameId, Player.X, 0, 0);

            // Assert
            Assert.Equal(Player.X, game.Board[0, 0]);
            Assert.Equal(Player.O, game.CurrentPlayer);
            Assert.Single(game.MoveHistory);
        }

        [Fact]
        public void MakeMove_OccupiedCell_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.TwoPlayer);
            _service.MakeMove(game.GameId, Player.X, 0, 0);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => 
                _service.MakeMove(game.GameId, Player.O, 0, 0));
            Assert.Equal("Cell occupied.", ex.Message);
        }

        [Fact]
        public void MakeMove_RowWin_UpdatesStatusAndScoreboard()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.TwoPlayer);

            // Act - X wins on top row
            _service.MakeMove(game.GameId, Player.X, 0, 0);
            _service.MakeMove(game.GameId, Player.O, 1, 0);
            _service.MakeMove(game.GameId, Player.X, 0, 1);
            _service.MakeMove(game.GameId, Player.O, 1, 1);
            _service.MakeMove(game.GameId, Player.X, 0, 2);

            // Assert
            Assert.Equal(GameStatus.Won, game.Status);
            Assert.Equal(Player.X, game.Winner);
            Assert.Equal(3, game.WinningCells.Count);
            Assert.Equal(1, _service.GetScoreboard().XWins);
        }

        [Fact]
        public void MakeMove_AfterGameComplete_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.TwoPlayer);
            game.Status = GameStatus.Won; // Manually mock completion

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => 
                _service.MakeMove(game.GameId, Player.X, 0, 0));
            Assert.Equal("Game is completed.", ex.Message);
        }

        [Fact]
        public void UndoMove_TwoPlayerMode_RemovesOneMove()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.TwoPlayer);
            _service.MakeMove(game.GameId, Player.X, 0, 0);
            _service.MakeMove(game.GameId, Player.O, 1, 1);

            // Act
            _service.UndoMove(game.GameId);

            // Assert
            Assert.Single(game.MoveHistory); // Only X's move remains
            Assert.Equal(Player.None, game.Board[1, 1]); // O's move reversed
            Assert.Equal(Player.O, game.CurrentPlayer); // Turn goes back to O
        }

        [Fact]
        public void UndoMove_ComputerMode_RemovesTwoMoves()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.Computer);
            
            // Act: Human plays X, computer immediately plays O
            _service.MakeMove(game.GameId, Player.X, 0, 0); 
            
            _service.UndoMove(game.GameId);

            // Assert
            Assert.Empty(game.MoveHistory); // Both X and auto-O moves removed
            Assert.Equal(Player.X, game.CurrentPlayer);
        }

        [Fact]
        public void ResetGame_ClearsBoardButKeepsScoreboard()
        {
            // Arrange
            var game = _service.CreateGame(GameMode.TwoPlayer);
            
            // Force a win for X
            _service.MakeMove(game.GameId, Player.X, 0, 0);
            _service.MakeMove(game.GameId, Player.O, 1, 0);
            _service.MakeMove(game.GameId, Player.X, 0, 1);
            _service.MakeMove(game.GameId, Player.O, 1, 1);
            _service.MakeMove(game.GameId, Player.X, 0, 2);

            // Act
            _service.ResetGame(game.GameId);

            // Assert
            Assert.Equal(GameStatus.InProgress, game.Status);
            Assert.Null(game.Winner);
            Assert.Empty(game.MoveHistory);
            Assert.Empty(game.WinningCells);
            Assert.Equal(1, _service.GetScoreboard().XWins); // Scoreboard untouched
        }
    }
}