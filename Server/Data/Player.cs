using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TheOracle2;

public record Player
{
    [Key]
    public ulong DiscordId { get; set; }
    public IronGame Game { get; set; }
}
