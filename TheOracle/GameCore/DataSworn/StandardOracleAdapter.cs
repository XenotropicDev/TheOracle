using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.GameCore.DataSworn;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.Oracle
{
    public partial class StandardOracle
    {
        public StandardOracle(Table table)
        {
            this.Description = table.Description;
            this.Chance = table.Chance;
            this.Prompt = table.Details;
            this.QuestStarter = table.QuestStarter;
            this.Thumbnail = table.Thumbnail;
            this.Oracles = ConverterHelpers.DataSwornTableToStandardOracle(table.ChildTable);
        }
    }
}
