public record RoomTheme
{
    public required int id { get; init; }
    public required int minWidth { get; init; }
    public required int maxWidth { get; init; }
    public required int minHeight { get; init; }
    public required int maxHeight { get; init; }
    public required int[] connectorIds { get; init; }
    public required int maxConnectors { get; init; }
    public required int minConnectors { get; init; }
}