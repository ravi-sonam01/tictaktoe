namespace TicTacToe.Api.Models
{
    public class CreateGameRequest
    {
        public GameMode Mode { get; set; }
    }

    public class MakeMoveRequest
    {
        public Player Player { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}