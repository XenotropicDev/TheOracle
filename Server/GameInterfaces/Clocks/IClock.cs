namespace TheOracle2.GameObjects
{
    public interface IClock : IDiscordEntity
    {
        int Segments { get; }
        int Filled { get; set; }
        bool IsFull { get; }
        string? ClockFillMessage { get; }
    }
}
