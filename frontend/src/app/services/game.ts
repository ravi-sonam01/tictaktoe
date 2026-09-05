import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GameSession, GameMode, Player, Scoreboard } from '../models/game.model';

@Injectable({ providedIn: 'root' })
export class GameService {
  private apiUrl = 'http://localhost:5175/api'; 

  constructor(private http: HttpClient) {}

  createGame(mode: GameMode): Observable<GameSession> {
    return this.http.post<GameSession>(`${this.apiUrl}/games`, { mode });
  }

  makeMove(gameId: string, player: Player, row: number, column: number): Observable<GameSession> {
    return this.http.post<GameSession>(`${this.apiUrl}/games/${gameId}/moves`, { player, row, column });
  }

  undoMove(gameId: string): Observable<GameSession> {
    return this.http.post<GameSession>(`${this.apiUrl}/games/${gameId}/undo`, {});
  }

  resetGame(gameId: string): Observable<GameSession> {
    return this.http.post<GameSession>(`${this.apiUrl}/games/${gameId}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.apiUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.apiUrl}/scoreboard/reset`, {});
  }
}