export interface ApiRoutes{
    file: string;
    authentication: string;
    development: string;
    song: string;
    playlist: string;
    user: string;
    queue: string;
}

export const APIROUTES: ApiRoutes = {
    authentication: "authentication",
    development: "development",
    file: "file",
    playlist: "playlist",
    song: "song",
    user: "user",
    queue: "queue"
}