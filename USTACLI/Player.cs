using Newtonsoft.Json;
using Markdig;

public record Player
{
    public int NationalRank { get; init; }
    public int SectionRank { get; init; }
    public int DistrictRank { get; init; }
    public CLIOptions? Options { get; init; }


    public string ToMarkDown()
    {
        // Print out the player ranking as markdown
        return @$"## {this.Options?.Name}

### {this.Options?.Section} {(this.Options?.Gender == Gender.M ? "Men's" : "Women's")} {this.Options?.Level} {this.Options?.Format.ToString().ToLower()}

- National Rank: {this.NationalRank}
- Section Rank: {this.SectionRank}
- District Rank: {this.DistrictRank}";
    }

    public string ToJSON() => JsonConvert.SerializeObject(this, Formatting.Indented);

    public string ToHTML()
    {
        return Markdown.ToHtml(ToMarkDown());
    }

    public override string ToString()
    {
        switch (this.Options?.Output)
        {
            case Output.json:
                return ToJSON();
            case Output.html:
                return ToHTML();
            default:
                return ToMarkDown();
        }
    }
}