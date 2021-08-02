export interface User {
    sessionId: string,
    username: string,
    accessToken: string
  }

export interface SessionRequest {
    title: string;
    choices: string[];
}

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
