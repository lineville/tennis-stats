using Newtonsoft.Json;
using Markdig;

public record Player
{
  public string? Name { get; init; }
  public int NationalRank { get; init; }
  public int SectionRank { get; init; }
  public int DistrictRank { get; init; }

  public string ToJSON() => JsonConvert.SerializeObject(this, Formatting.Indented);

  public string ToMarkDown(CLIOptions options)
  {
    // Print out the player ranking as markdown
    return @$"## {this.Name}

### {options.Section} {(options.Gender == "M" ? "Men's" : "Women's")} {ParseLevel(options.Level)} {options.Format?.ToLower()}

- National Rank: {this.NationalRank}
- Section Rank: {this.SectionRank}
- District Rank: {this.DistrictRank}";
  }

  public string ToHTML(CLIOptions options)
  {
    return Markdown.ToHtml(ToMarkDown(options));
  }

  public string ToString(CLIOptions options)
  {
    switch (options.Output)
    {
      case "json":
        return ToJSON();
      case "html":
        return ToHTML(options);
      default:
        return ToMarkDown(options);
    }
  }

  public string ParseLevel(string? level) => $"{level?.Split("_")[1]}.{level?.Split("_")[2]}";
}