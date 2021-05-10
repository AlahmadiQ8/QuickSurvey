import { Inject, Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { WINDOW } from '../injection-tokens';

export interface User {
  sessionId: string,
  username: string,
  accessToken: string
}

@Injectable()
export class AuthService {
  private user: User | null = null;

  public constructor(private route: ActivatedRoute, @Inject(WINDOW) private window: Window) {
    const sessionId = this.route.snapshot.paramMap.get('id')!;
    const username = this.route.snapshot.paramMap.get('username')!;
    const accessToken = this.route.snapshot.queryParamMap.get("access_token")!;
    this.user = {
      sessionId,
      username,
      accessToken
    }
  }

  public get currentUser(): User | null  { return this.user}
}
