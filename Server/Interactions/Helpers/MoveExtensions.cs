using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TheOracle2;
using TheOracle2.Data;

namespace Server.Interactions.Helpers;

public static class MoveExtensions
{
    public static SelectMenuOptionBuilder MoveAsSelectOption(this Move move, IEmoteRepository emotes)
    {
        var builder = new SelectMenuOptionBuilder();

        //Find the second ** (if any) before we remove them for display
        var closingBolds = move.Text.IndexOf("**", move.Text.IndexOf("**") + 2);
        var desc = (closingBolds > 0 && closingBolds < 70) ? move.Text.Replace("**", "")[..closingBolds] : move.Text.Replace("**", "")[..67] + "...";

        builder.WithValue(move.JsonId).WithDescription(desc).WithEmote(emotes.Reference).WithLabel(move.Name);

        return builder;
    }
}
