namespace Api.DTO;

public record HexCellDto(int Q, int R, int S, int Power, string? Owner, bool Size);