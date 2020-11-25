using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn.Delve
{
    public class Feature : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }
        public string Prompt { get; set; }

        public StandardOracle AsStandardOracle()
        {
            return new StandardOracle
            {
                Chance = this.Chance,
                Description = this.Description,
                Prompt = this.Prompt,
                Oracles = null
            };
        }
    }
}