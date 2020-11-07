namespace TheOracle
{
    public class HookedEvents
    {
        public bool OracleTableReactions = false;
        public bool PlanetReactions = false;
        public bool ProgressReactions = false;
        public bool StarCreatureReactions = false;
        public bool StarNPCReactions = false;
        public bool StarSettlementReactions = false;
        public bool StarShipReactions = false;
        public bool InitiativeReactions = false;
        public bool AskTheOracleReactions = false;

        public bool PlayerCardReactions { get; internal set; }
    }
}