import { User } from "./user.model";

export interface AuthResponse {
    token?: string;
    access_token?: string;
    accessToken?: string;
    refresh_token?: string;
    refreshToken?: string;
    expires_in?: number;
    preferred_username?: string;
    roles?: string[];
    claims?: Record<string, string>;
    message?: string;
    user?: User;
}