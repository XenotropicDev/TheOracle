using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.GameObjects;

namespace Server.GameInterfaces
{
    public class Crew
    {
        public int Id { get; set; }
        public List<PlayerCharacter> Characters { get; set; }
        public int Supply { get; set; }
    }
}
