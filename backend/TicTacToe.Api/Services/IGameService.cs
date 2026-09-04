using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services
{
    public interface IGameService
    {
        GameSession CreateGame(GameMode mode);
        GameSession GetGame(string gameId);
        GameSession MakeMove(string gameId, Player player, int row, int col);
        GameSession UndoMove(string gameId);
        GameSession ResetGame(string gameId);
        Scoreboard GetScoreboard();
        void ResetScoreboard();
    }
}