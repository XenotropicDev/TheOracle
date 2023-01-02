namespace Server.Interactions
{
    /*
     * Doug wants to create some denizens for a delve site
     * He uses the /DenizenMatrix command
     * he adds a few denizens to the empty matrix
     */

    public class DenizenMatrix
    {
        public List<string> VeryCommon { get; set; } = new();
        public List<string> Common { get; set; } = new();
        public List<string> Uncommon { get; set; } = new();
        public List<string> Rare { get; set; } = new();
        public List<string> Unforseen { get; set; } = new();

        public string RollMatrix(Random random = null)
        {
            random ??= Random.Shared;

            var roll = random.Next(1, 101);

            if (roll < 28)
            {
                return RollMatrix(VeryCommon, 1, random);
            }
            else if (roll < 70)
            {
                return RollMatrix(Common, 3, random);
            }
            else if (roll < 94)
            {
                return RollMatrix(Uncommon, 4, random);
            }
            else if (roll < 100)
            {
                return RollMatrix(Rare, 3, random);
            }
            else if (roll == 100)
            {
                return RollMatrix(Unforseen, 1, random);
            }

            return "";
        }

        private string RollMatrix(List<string> matrix, int poolSize, Random random)
        {
            var roll = random.Next(0, poolSize);

            if (roll + 1 > matrix.Count)
            {
                return "Unknown";
            }

            return matrix[roll];
        }
    }

    public enum DenizenTier
    {
        VeryCommon,
        Common,
        Uncommon,
        Rare,
        Unforseen
    }
}
