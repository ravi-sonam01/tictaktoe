namespace TicTacToe.Api.Models
{
    public enum Player 
    { 
        None, 
        X, 
        O 
    }

    public enum GameMode 
    { 
        TwoPlayer, 
        Computer 
    }

    public enum GameStatus 
    { 
        InProgress, 
        Won, 
        Draw 
    }
}