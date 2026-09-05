import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameService } from './services/game'; // Adjusted to match your file structure
import { GameSession, GameMode, Player, Scoreboard, GameStatus } from './models/game.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html' // Pointing to app.html instead of app.component.html
})
export class App implements OnInit {
  session: GameSession | null = null;
  scoreboard: Scoreboard = { xWins: 0, oWins: 0, draws: 0 };
  
  Player = Player;
  GameMode = GameMode;
  GameStatus = GameStatus;

  constructor(private gameService: GameService) {}

  ngOnInit() {
    this.refreshScoreboard();
  }

  startGame(mode: GameMode) {
    this.gameService.createGame(mode).subscribe(res => this.session = res);
  }

  makeMove(row: number, col: number) {
    if (!this.session || this.session.status !== GameStatus.InProgress) return;
    if (this.session.board[row][col] !== Player.None) return;

    this.gameService.makeMove(this.session.gameId, this.session.currentPlayer, row, col).subscribe({
      next: (res) => {
        this.session = res;
        if (res.status !== GameStatus.InProgress) this.refreshScoreboard();
      },
      error: (err) => console.error(err)
    });
  }

  undoMove() {
    if (!this.session) return;
    this.gameService.undoMove(this.session.gameId).subscribe(res => this.session = res);
  }

  resetGame() {
    if (!this.session) return;
    this.gameService.resetGame(this.session.gameId).subscribe(res => this.session = res);
  }

  refreshScoreboard() {
    this.gameService.getScoreboard().subscribe(res => this.scoreboard = res);
  }

  resetScoreboard() {
    this.gameService.resetScoreboard().subscribe(res => this.scoreboard = res);
  }

  isWinningCell(row: number, col: number): boolean {
    if (!this.session || !this.session.winningCells) return false;
    return this.session.winningCells.some(cell => cell[0] === row && cell[1] === col);
  }
}