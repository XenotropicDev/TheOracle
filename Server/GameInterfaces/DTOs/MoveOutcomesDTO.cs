namespace Server.GameInterfaces.DTOs
{
    public class MoveOutcomesDTO
    {
        public MoveOutcomeDTO StrongHit { get; set; }
        public MoveOutcomeDTO WeakHit { get; set; }
        public MoveOutcomeDTO Miss { get; set; }
    }
}
