import { User } from "./user.model";

export interface AuthResponse {
    token: string;
    message?: string;
    user?: User;
  }