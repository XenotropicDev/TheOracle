using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheOracle2.Data;

namespace TheOracle2;

public record Player
{
    [Key]
    public ulong DiscordId { get; set; }
    public IronGame Game { get; set; }
    public bool SubsPerServer { get; set; }
    public List<Asset> Assets { get; set; }
    public List<Oracle> Oracles { get; set; }

}
