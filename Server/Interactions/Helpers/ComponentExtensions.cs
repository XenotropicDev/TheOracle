using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interactions.Helpers;

internal static class ComponentExtensions
{
    public static ComponentBuilder RemoveSelectionOptions(this ComponentBuilder builder, string[] valuesToRemove)
    {
        for (var i = builder.ActionRows.Count - 1; i >= 0; i--)
        {
            var component = builder.ActionRows[i].Components.FirstOrDefault();
            if (component == null || component is not SelectMenuComponent select) continue;

            var selectBuilder = select.ToBuilder();
            selectBuilder.Options.RemoveAll(o => valuesToRemove.Contains(o.Value));
            if (selectBuilder.Options.Count == 0)
            {
                builder.ActionRows.RemoveAt(i);
                continue;
            }

            builder.ActionRows[i].WithComponents(new List<IMessageComponent> { selectBuilder.Build() });
        }

        return builder;
    }

    public static ComponentBuilder RemoveComponentById(this ComponentBuilder builder, string id)
    {
        var row = GetRowContainingId(builder, id);
        var item = GetComponentById(builder, id);
        if (item != null && row != null)
        {
            row.Components.Remove(item);
            if (row.Components.Count == 0)
            {
                builder.ActionRows.Remove(row);
            }
        }
        return builder;
    }

    public static ComponentBuilder ReplaceComponentById(this ComponentBuilder builder, string id, IMessageComponent replacement)
    {
        var rows = builder.ActionRows.Where(r => r.Components.Any(c => c.CustomId == id));
        foreach (var row in rows)
        {
            int index = row.Components.FindIndex(c => c.CustomId == id);
            if (index != -1) row.Components[index] = replacement;
        }

        return builder;
    }

    public static ActionRowBuilder? GetRowContainingId(this ComponentBuilder builder, string id)
    {
        return builder.ActionRows.Find(r => r.Components.Any(c => c.CustomId == id));
    }

    public static IMessageComponent? GetComponentById(this ComponentBuilder builder, string id)
    {
        ActionRowBuilder? row = builder.ActionRows.Find(r => r.Components.Any(c => c.CustomId == id));
        return row?.Components.Find(c => c.CustomId == id);
    }
}
