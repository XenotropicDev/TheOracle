using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using OracleGen;
using OracleGen.Shared;
using MudBlazor;
using TheOracle2.Data;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Net;
using DataswornPoco;
using System.Reflection.Metadata;
using System.ComponentModel;

namespace OracleGen.Pages;

public partial class Index
{
    private (string? clientId, string? clientSecret) DiscordOauth = new();

    Oracle oracle = new();
    OracleRoll selectedPair = null;
    Table tableRowToAdd = new() {Floor = 101, Ceiling = 101 };

    MudTextField<string> addTableResultText;
    MudNumericField<int?> CeilingAddInput;
    MudTable<Table> OracleResultTable;

    [Inject]
    public IJSRuntime JS { get; set; }

    public Index()
    {
        LoadOracle1();
    }

    void LoadOracle1()
    {
        string text = """{ "$id": "Starforged/Oracles/Core/Action", "Name": "Action", "Category": "Starforged/Oracles/Core", "Description": "When you [Ask the Oracle](Starforged/Moves/Fate/Ask_the_Oracle) about a goal, situation, or event, roll for an Action and Theme. Together, these provide an interpretative verb/noun prompt.", "Usage": { "Allow duplicates": false, "Suggestions": { "Oracle rolls": [ "Starforged/Oracles/Core/Theme" ] } }, "Table": [ { "Floor": 1, "Ceiling": 1, "$id": "Starforged/Oracles/Core/Action/1", "Result": "Abandon" }, { "Floor": 2, "Ceiling": 2, "$id": "Starforged/Oracles/Core/Action/2", "Result": "Acquire" }, { "Floor": 3, "Ceiling": 3, "$id": "Starforged/Oracles/Core/Action/3", "Result": "Advance" }, { "Floor": 4, "Ceiling": 4, "$id": "Starforged/Oracles/Core/Action/4", "Result": "Affect" }, { "Floor": 5, "Ceiling": 5, "$id": "Starforged/Oracles/Core/Action/5", "Result": "Aid" }, { "Floor": 6, "Ceiling": 6, "$id": "Starforged/Oracles/Core/Action/6", "Result": "Arrive" }, { "Floor": 7, "Ceiling": 7, "$id": "Starforged/Oracles/Core/Action/7", "Result": "Assault" }, { "Floor": 8, "Ceiling": 8, "$id": "Starforged/Oracles/Core/Action/8", "Result": "Attack" }, { "Floor": 9, "Ceiling": 9, "$id": "Starforged/Oracles/Core/Action/9", "Result": "Avenge" }, { "Floor": 10, "Ceiling": 10, "$id": "Starforged/Oracles/Core/Action/10", "Result": "Avoid" }, { "Floor": 11, "Ceiling": 11, "$id": "Starforged/Oracles/Core/Action/11", "Result": "Await" }, { "Floor": 12, "Ceiling": 12, "$id": "Starforged/Oracles/Core/Action/12", "Result": "Begin" }, { "Floor": 13, "Ceiling": 13, "$id": "Starforged/Oracles/Core/Action/13", "Result": "Betray" }, { "Floor": 14, "Ceiling": 14, "$id": "Starforged/Oracles/Core/Action/14", "Result": "Bolster" }, { "Floor": 15, "Ceiling": 15, "$id": "Starforged/Oracles/Core/Action/15", "Result": "Breach" }, { "Floor": 16, "Ceiling": 16, "$id": "Starforged/Oracles/Core/Action/16", "Result": "Break" }, { "Floor": 17, "Ceiling": 17, "$id": "Starforged/Oracles/Core/Action/17", "Result": "Capture" }, { "Floor": 18, "Ceiling": 18, "$id": "Starforged/Oracles/Core/Action/18", "Result": "Challenge" }, { "Floor": 19, "Ceiling": 19, "$id": "Starforged/Oracles/Core/Action/19", "Result": "Change" }, { "Floor": 20, "Ceiling": 20, "$id": "Starforged/Oracles/Core/Action/20", "Result": "Charge" }, { "Floor": 21, "Ceiling": 21, "$id": "Starforged/Oracles/Core/Action/21", "Result": "Clash" }, { "Floor": 22, "Ceiling": 22, "$id": "Starforged/Oracles/Core/Action/22", "Result": "Command" }, { "Floor": 23, "Ceiling": 23, "$id": "Starforged/Oracles/Core/Action/23", "Result": "Communicate" }, { "Floor": 24, "Ceiling": 24, "$id": "Starforged/Oracles/Core/Action/24", "Result": "Construct" }, { "Floor": 25, "Ceiling": 25, "$id": "Starforged/Oracles/Core/Action/25", "Result": "Control" }, { "Floor": 26, "Ceiling": 26, "$id": "Starforged/Oracles/Core/Action/26", "Result": "Coordinate" }, { "Floor": 27, "Ceiling": 27, "$id": "Starforged/Oracles/Core/Action/27", "Result": "Create" }, { "Floor": 28, "Ceiling": 28, "$id": "Starforged/Oracles/Core/Action/28", "Result": "Debate" }, { "Floor": 29, "Ceiling": 29, "$id": "Starforged/Oracles/Core/Action/29", "Result": "Defeat" }, { "Floor": 30, "Ceiling": 30, "$id": "Starforged/Oracles/Core/Action/30", "Result": "Defend" }, { "Floor": 31, "Ceiling": 31, "$id": "Starforged/Oracles/Core/Action/31", "Result": "Deflect" }, { "Floor": 32, "Ceiling": 32, "$id": "Starforged/Oracles/Core/Action/32", "Result": "Defy" }, { "Floor": 33, "Ceiling": 33, "$id": "Starforged/Oracles/Core/Action/33", "Result": "Deliver" }, { "Floor": 34, "Ceiling": 34, "$id": "Starforged/Oracles/Core/Action/34", "Result": "Demand" }, { "Floor": 35, "Ceiling": 35, "$id": "Starforged/Oracles/Core/Action/35", "Result": "Depart" }, { "Floor": 36, "Ceiling": 36, "$id": "Starforged/Oracles/Core/Action/36", "Result": "Destroy" }, { "Floor": 37, "Ceiling": 37, "$id": "Starforged/Oracles/Core/Action/37", "Result": "Distract" }, { "Floor": 38, "Ceiling": 38, "$id": "Starforged/Oracles/Core/Action/38", "Result": "Eliminate" }, { "Floor": 39, "Ceiling": 39, "$id": "Starforged/Oracles/Core/Action/39", "Result": "Endure" }, { "Floor": 40, "Ceiling": 40, "$id": "Starforged/Oracles/Core/Action/40", "Result": "Escalate" }, { "Floor": 41, "Ceiling": 41, "$id": "Starforged/Oracles/Core/Action/41", "Result": "Escort" }, { "Floor": 42, "Ceiling": 42, "$id": "Starforged/Oracles/Core/Action/42", "Result": "Evade" }, { "Floor": 43, "Ceiling": 43, "$id": "Starforged/Oracles/Core/Action/43", "Result": "Explore" }, { "Floor": 44, "Ceiling": 44, "$id": "Starforged/Oracles/Core/Action/44", "Result": "Falter" }, { "Floor": 45, "Ceiling": 45, "$id": "Starforged/Oracles/Core/Action/45", "Result": "Find" }, { "Floor": 46, "Ceiling": 46, "$id": "Starforged/Oracles/Core/Action/46", "Result": "Finish" }, { "Floor": 47, "Ceiling": 47, "$id": "Starforged/Oracles/Core/Action/47", "Result": "Focus" }, { "Floor": 48, "Ceiling": 48, "$id": "Starforged/Oracles/Core/Action/48", "Result": "Follow" }, { "Floor": 49, "Ceiling": 49, "$id": "Starforged/Oracles/Core/Action/49", "Result": "Fortify" }, { "Floor": 50, "Ceiling": 50, "$id": "Starforged/Oracles/Core/Action/50", "Result": "Gather" }, { "Floor": 51, "Ceiling": 51, "$id": "Starforged/Oracles/Core/Action/51", "Result": "Guard" }, { "Floor": 52, "Ceiling": 52, "$id": "Starforged/Oracles/Core/Action/52", "Result": "Hide" }, { "Floor": 53, "Ceiling": 53, "$id": "Starforged/Oracles/Core/Action/53", "Result": "Hold" }, { "Floor": 54, "Ceiling": 54, "$id": "Starforged/Oracles/Core/Action/54", "Result": "Hunt" }, { "Floor": 55, "Ceiling": 55, "$id": "Starforged/Oracles/Core/Action/55", "Result": "Impress" }, { "Floor": 56, "Ceiling": 56, "$id": "Starforged/Oracles/Core/Action/56", "Result": "Initiate" }, { "Floor": 57, "Ceiling": 57, "$id": "Starforged/Oracles/Core/Action/57", "Result": "Inspect" }, { "Floor": 58, "Ceiling": 58, "$id": "Starforged/Oracles/Core/Action/58", "Result": "Investigate" }, { "Floor": 59, "Ceiling": 59, "$id": "Starforged/Oracles/Core/Action/59", "Result": "Journey" }, { "Floor": 60, "Ceiling": 60, "$id": "Starforged/Oracles/Core/Action/60", "Result": "Learn" }, { "Floor": 61, "Ceiling": 61, "$id": "Starforged/Oracles/Core/Action/61", "Result": "Leave" }, { "Floor": 62, "Ceiling": 62, "$id": "Starforged/Oracles/Core/Action/62", "Result": "Locate" }, { "Floor": 63, "Ceiling": 63, "$id": "Starforged/Oracles/Core/Action/63", "Result": "Lose" }, { "Floor": 64, "Ceiling": 64, "$id": "Starforged/Oracles/Core/Action/64", "Result": "Manipulate" }, { "Floor": 65, "Ceiling": 65, "$id": "Starforged/Oracles/Core/Action/65", "Result": "Mourn" }, { "Floor": 66, "Ceiling": 66, "$id": "Starforged/Oracles/Core/Action/66", "Result": "Move" }, { "Floor": 67, "Ceiling": 67, "$id": "Starforged/Oracles/Core/Action/67", "Result": "Oppose" }, { "Floor": 68, "Ceiling": 68, "$id": "Starforged/Oracles/Core/Action/68", "Result": "Overwhelm" }, { "Floor": 69, "Ceiling": 69, "$id": "Starforged/Oracles/Core/Action/69", "Result": "Persevere" }, { "Floor": 70, "Ceiling": 70, "$id": "Starforged/Oracles/Core/Action/70", "Result": "Preserve" }, { "Floor": 71, "Ceiling": 71, "$id": "Starforged/Oracles/Core/Action/71", "Result": "Protect" }, { "Floor": 72, "Ceiling": 72, "$id": "Starforged/Oracles/Core/Action/72", "Result": "Raid" }, { "Floor": 73, "Ceiling": 73, "$id": "Starforged/Oracles/Core/Action/73", "Result": "Reduce" }, { "Floor": 74, "Ceiling": 74, "$id": "Starforged/Oracles/Core/Action/74", "Result": "Refuse" }, { "Floor": 75, "Ceiling": 75, "$id": "Starforged/Oracles/Core/Action/75", "Result": "Reject" }, { "Floor": 76, "Ceiling": 76, "$id": "Starforged/Oracles/Core/Action/76", "Result": "Release" }, { "Floor": 77, "Ceiling": 77, "$id": "Starforged/Oracles/Core/Action/77", "Result": "Remove" }, { "Floor": 78, "Ceiling": 78, "$id": "Starforged/Oracles/Core/Action/78", "Result": "Research" }, { "Floor": 79, "Ceiling": 79, "$id": "Starforged/Oracles/Core/Action/79", "Result": "Resist" }, { "Floor": 80, "Ceiling": 80, "$id": "Starforged/Oracles/Core/Action/80", "Result": "Restore" }, { "Floor": 81, "Ceiling": 81, "$id": "Starforged/Oracles/Core/Action/81", "Result": "Reveal" }, { "Floor": 82, "Ceiling": 82, "$id": "Starforged/Oracles/Core/Action/82", "Result": "Risk" }, { "Floor": 83, "Ceiling": 83, "$id": "Starforged/Oracles/Core/Action/83", "Result": "Scheme" }, { "Floor": 84, "Ceiling": 84, "$id": "Starforged/Oracles/Core/Action/84", "Result": "Search" }, { "Floor": 85, "Ceiling": 85, "$id": "Starforged/Oracles/Core/Action/85", "Result": "Secure" }, { "Floor": 86, "Ceiling": 86, "$id": "Starforged/Oracles/Core/Action/86", "Result": "Seize" }, { "Floor": 87, "Ceiling": 87, "$id": "Starforged/Oracles/Core/Action/87", "Result": "Serve" }, { "Floor": 88, "Ceiling": 88, "$id": "Starforged/Oracles/Core/Action/88", "Result": "Share" }, { "Floor": 89, "Ceiling": 89, "$id": "Starforged/Oracles/Core/Action/89", "Result": "Strengthen" }, { "Floor": 90, "Ceiling": 90, "$id": "Starforged/Oracles/Core/Action/90", "Result": "Summon" }, { "Floor": 91, "Ceiling": 91, "$id": "Starforged/Oracles/Core/Action/91", "Result": "Support" }, { "Floor": 92, "Ceiling": 92, "$id": "Starforged/Oracles/Core/Action/92", "Result": "Suppress" }, { "Floor": 93, "Ceiling": 93, "$id": "Starforged/Oracles/Core/Action/93", "Result": "Surrender" }, { "Floor": 94, "Ceiling": 94, "$id": "Starforged/Oracles/Core/Action/94", "Result": "Swear" }, { "Floor": 95, "Ceiling": 95, "$id": "Starforged/Oracles/Core/Action/95", "Result": "Threaten" }, { "Floor": 96, "Ceiling": 96, "$id": "Starforged/Oracles/Core/Action/96", "Result": "Transform" }, { "Floor": 97, "Ceiling": 97, "$id": "Starforged/Oracles/Core/Action/97", "Result": "Uncover" }, { "Floor": 98, "Ceiling": 98, "$id": "Starforged/Oracles/Core/Action/98", "Result": "Uphold" }, { "Floor": 99, "Ceiling": 99, "$id": "Starforged/Oracles/Core/Action/99", "Result": "Weaken" }, { "Floor": 100, "Ceiling": 100, "$id": "Starforged/Oracles/Core/Action/100", "Result": "Withdraw" } ]}""";
        oracle = JsonConvert.DeserializeObject<Oracle>(text) ?? new();
    }

    void FormatOracle()
    {
        oracle.Id = $"{oracle.Category}/{oracle.Name}";

        foreach (var result in oracle.Table)
        {
            var value = result.Floor == result.Ceiling ? result.Floor.ToString() : $"{result.Floor}-{result.Ceiling}";
            result.Id = $"{oracle.Category}/{oracle.Name}/{value}";
        }
    }

    async Task DownloadJson()
    {
        FormatOracle();

        var json = JsonConvert.SerializeObject(oracle, Formatting.Indented);
        var fileName = $"{oracle.Name}.json";

        var bytes = Encoding.UTF8.GetBytes(json);
        var stream = new MemoryStream(bytes);

        using var streamRef = new DotNetStreamReference(stream);

        try
        {
            await JS.InvokeVoidAsync("window.downloadFileFromStream", fileName, streamRef);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    void ClearData()
    {
        oracle = new();
        tableRowToAdd = new() {Ceiling = 1, Floor = 1 };
    }

    void AddTableRow()
    {
        oracle.Table.Add(tableRowToAdd.CloneJson() ?? new());
        var newMax = oracle.Table.Max(t => t.Ceiling) + 1;
        tableRowToAdd = new() {Ceiling =  newMax, Floor = newMax};
    }

    void AddSuggestionRow()
    {
        oracle.Usage.Suggestions.OracleRolls.Add(new("Follow-Up Oracle"));
    }

    void deleteSuggestion()
    {
        if (selectedPair != null)
            oracle.Usage.Suggestions.OracleRolls.Remove(selectedPair);
    }

    async Task EnterOnAddTableRow(KeyboardEventArgs args)
    {
        if (args.Code == "Enter" || args.Code == "NumpadEnter")
        {
            //set the focus to another element to force the value to update
            await CeilingAddInput.FocusAsync();
            AddTableRow();
            await addTableResultText.FocusAsync();
        }
    }

    void RemoveFromTable(Table value)
    {
        var removed = oracle.Table.Remove(oracle.Table.FirstOrDefault(t => t == value));
        oracle.Table = new(oracle.Table);
    }
}
