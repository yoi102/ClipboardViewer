namespace Commons;
public record ClipboardDataItem
{
    public required string Format { get; init; }

    public object? Content { get; init; }

    public Exception? Error { get; init; }
}
public record ClipboardData
{
    public ClipboardDataItem[] Items { get; set; } = [];
    public required DateTimeOffset CreateAt { get; init; }
}