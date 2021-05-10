import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface Session {
  title: string;
  choices: Choice[];
  participants: string[];
}

export interface Choice {
  text: string;
  id: number;
  voters: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private static readonly endpoint = "";

  constructor(private http: HttpClient, private authService: AuthService) { }

  getSession(): Observable<Session> {
    const { accessToken, sessionId, username } = this.authService.currentUser!;

    return this.http.get<Session>(`/api/session/${sessionId}/user/${username}`, { params: { access_token: accessToken } });
  }
}
