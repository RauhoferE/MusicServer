export interface PaginationModel{
    page: number;
    take: number;
    sortAfter: string;
    asc: boolean;
    query: string;
}