using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn.Delve
{
    public class Danger
    {
        public int Chance { get; set; }
        public string Description { get; set; }

        public StandardOracle AsStandardOracle()
        {
            return new StandardOracle { Chance = this.Chance, Description = this.Description };
        }
    }
}
