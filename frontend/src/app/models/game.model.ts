export enum Player { None = 0, X = 1, O = 2 }
export enum GameMode { TwoPlayer = 0, Computer = 1 }
export enum GameStatus { InProgress = 0, Won = 1, Draw = 2 }

export interface Move {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface GameSession {
  gameId: string;
  board: Player[][];
  currentPlayer: Player;
  mode: GameMode;
  status: GameStatus;
  winner: Player | null;
  winningCells: number[][];
  moveHistory: Move[];
}