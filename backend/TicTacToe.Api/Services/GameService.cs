using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services
{
    public class GameService : IGameService
    {
        private readonly ConcurrentDictionary<string, GameSession> _games = new();
        private readonly Scoreboard _scoreboard = new();

        public GameSession CreateGame(GameMode mode)
        {
            var session = new GameSession { Mode = mode };
            _games[session.GameId] = session;
            return session;
        }

        public GameSession GetGame(string gameId)
        {
            _games.TryGetValue(gameId, out var session);
            return session;
        }

        public Scoreboard GetScoreboard() => _scoreboard;

        public void ResetScoreboard()
        {
            _scoreboard.XWins = 0;
            _scoreboard.OWins = 0;
            _scoreboard.Draws = 0;
        }

        public GameSession MakeMove(string gameId, Player player, int row, int col)
        {
            var session = GetGame(gameId) ?? throw new ArgumentException("Game not found.");

            if (session.Status != GameStatus.InProgress) throw new InvalidOperationException("Game is completed.");
            if (session.CurrentPlayer != player) throw new InvalidOperationException("Not your turn.");
            if (row < 0 || row > 2 || col < 0 || col > 2) throw new ArgumentException("Outside board.");
            if (session.Board[row, col] != Player.None) throw new InvalidOperationException("Cell occupied.");

            ApplyMove(session, player, row, col);

            if (session.Mode == GameMode.Computer && session.Status == GameStatus.InProgress && session.CurrentPlayer == Player.O)
            {
                MakeComputerMove(session);
            }

            return session;
        }

        public GameSession UndoMove(string gameId)
        {
            var session = GetGame(gameId) ?? throw new ArgumentException("Game not found.");

            if (session.Status != GameStatus.InProgress)
                throw new InvalidOperationException("Undo is disabled after game completion (Option A).");
            
            if (session.MoveHistory.Count == 0)
                throw new InvalidOperationException("No moves to undo.");

            int movesToRemove = session.Mode == GameMode.Computer ? 2 : 1;
            movesToRemove = Math.Min(movesToRemove, session.MoveHistory.Count);

            for (int i = 0; i < movesToRemove; i++)
            {
                var lastMove = session.MoveHistory.Last();
                session.Board[lastMove.Row, lastMove.Column] = Player.None;
                session.MoveHistory.RemoveAt(session.MoveHistory.Count - 1);
            }

            session.CurrentPlayer = session.MoveHistory.Count % 2 == 0 ? Player.X : Player.O;
            return session;
        }

        public GameSession ResetGame(string gameId)
        {
            var session = GetGame(gameId) ?? throw new ArgumentException("Game not found.");

            session.Board = new Player[3, 3];
            session.MoveHistory.Clear();
            session.WinningCells.Clear();
            session.Winner = null;
            session.Status = GameStatus.InProgress;
            session.CurrentPlayer = Player.X;

            return session;
        }

        private void ApplyMove(GameSession session, Player player, int row, int col)
        {
            session.Board[row, col] = player;
            session.MoveHistory.Add(new Move 
            { 
                MoveNumber = session.MoveHistory.Count + 1, 
                Player = player, 
                Row = row, 
                Column = col 
            });

            EvaluateGameState(session);

            if (session.Status == GameStatus.InProgress)
            {
                session.CurrentPlayer = player == Player.X ? Player.O : Player.X;
            }
            else
            {
                UpdateScoreboard(session);
            }
        }

        private void EvaluateGameState(GameSession session)
        {
            var b = session.Board;
            var currentPlayer = session.MoveHistory.Last().Player;

            for (int i = 0; i < 3; i++)
            {
                if (b[i, 0] != Player.None && b[i, 0] == b[i, 1] && b[i, 1] == b[i, 2])
                {
                    MarkWin(session, currentPlayer, new[] { new[] { i, 0 }, new[] { i, 1 }, new[] { i, 2 } });
                    return;
                }
                if (b[0, i] != Player.None && b[0, i] == b[1, i] && b[1, i] == b[2, i])
                {
                    MarkWin(session, currentPlayer, new[] { new[] { 0, i }, new[] { 1, i }, new[] { 2, i } });
                    return;
                }
            }

            if (b[0, 0] != Player.None && b[0, 0] == b[1, 1] && b[1, 1] == b[2, 2])
            {
                MarkWin(session, currentPlayer, new[] { new[] { 0, 0 }, new[] { 1, 1 }, new[] { 2, 2 } });
                return;
            }
            if (b[0, 2] != Player.None && b[0, 2] == b[1, 1] && b[1, 1] == b[2, 0])
            {
                MarkWin(session, currentPlayer, new[] { new[] { 0, 2 }, new[] { 1, 1 }, new[] { 2, 0 } });
                return;
            }

            if (session.MoveHistory.Count == 9)
            {
                session.Status = GameStatus.Draw;
            }
        }

        private void MarkWin(GameSession session, Player winner, int[][] winningCells)
        {
            session.Status = GameStatus.Won;
            session.Winner = winner;
            session.WinningCells.AddRange(winningCells);
        }

        private void UpdateScoreboard(GameSession session)
        {
            if (session.Status == GameStatus.Won)
            {
                if (session.Winner == Player.X) _scoreboard.XWins++;
                if (session.Winner == Player.O) _scoreboard.OWins++;
            }
            else if (session.Status == GameStatus.Draw)
            {
                _scoreboard.Draws++;
            }
        }

        private void MakeComputerMove(GameSession session)
        {
            if (TryFindWinningMove(session.Board, Player.O, out int r, out int c)) 
                { ApplyMove(session, Player.O, r, c); return; }

            if (TryFindWinningMove(session.Board, Player.X, out r, out c)) 
                { ApplyMove(session, Player.O, r, c); return; }

            if (session.Board[1, 1] == Player.None) 
                { ApplyMove(session, Player.O, 1, 1); return; }

            var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
            foreach (var corner in corners)
            {
                if (session.Board[corner.Item1, corner.Item2] == Player.None)
                {
                    ApplyMove(session, Player.O, corner.Item1, corner.Item2); return;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (session.Board[i, j] == Player.None)
                    {
                        ApplyMove(session, Player.O, i, j); return;
                    }
                }
            }
        }

        private bool TryFindWinningMove(Player[,] board, Player player, out int r, out int c)
        {
            r = -1; c = -1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == Player.None)
                    {
                        board[i, j] = player;
                        bool isWin = CheckSimulatedWin(board, player);
                        board[i, j] = Player.None;
                        
                        if (isWin)
                        {
                            r = i; c = j;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckSimulatedWin(Player[,] b, Player p)
        {
            for (int i = 0; i < 3; i++)
            {
                if (b[i, 0] == p && b[i, 1] == p && b[i, 2] == p) return true;
                if (b[0, i] == p && b[1, i] == p && b[2, i] == p) return true;
            }
            if (b[0, 0] == p && b[1, 1] == p && b[2, 2] == p) return true;
            if (b[0, 2] == p && b[1, 1] == p && b[2, 0] == p) return true;
            
            return false;
        }
    }
}