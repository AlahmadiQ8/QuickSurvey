import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Session, SessionRequest } from '../models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private static readonly endpoint = '';

  constructor(private http: HttpClient, private authService: AuthService) { }

  public createSession(session: SessionRequest): Observable<string | null> {
    return this.http.post(`/api/session`, session, { observe: 'response' })
      .pipe(map(e => e.headers.get('Location')));
  }

  public getSession(): Observable<Session> {
    const { accessToken, sessionId, username } = this.authService.currentUser!;

    return this.http.get<Session>(`/api/session/${sessionId}/user/${username}`, { params: { access_token: accessToken } });
  }
}
