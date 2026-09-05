using System;
using System.Collections.Generic;

namespace TicTacToe.Api.Models
{
    public class GameSession
    {
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        
        // Change from Player[,] to Player[][]
        public Player[][] Board { get; set; } = new Player[][] 
        {
            new Player[3],
            new Player[3],
            new Player[3]
        };
        
        public Player CurrentPlayer { get; set; } = Player.X;
        public GameMode Mode { get; set; }
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public Player? Winner { get; set; }
        public List<int[]> WinningCells { get; set; } = new List<int[]>();
        public List<Move> MoveHistory { get; set; } = new List<Move>();
    }
}