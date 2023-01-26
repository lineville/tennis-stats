using Spectre.Console;

public sealed class TennisBallSpinner : Spinner
{
    // The interval for each frame
    public override TimeSpan Interval => TimeSpan.FromMilliseconds(50);

    // Whether or not the spinner contains unicode characters
    public override bool IsUnicode => false;

    // The individual frames of the spinner
    public override IReadOnlyList<string> Frames =>
        new List<string>
        {
            "🏓 🎾       🏓",
            "🏓  🎾      🏓",
            "🏓   🎾     🏓",
            "🏓    🎾    🏓",
            "🏓     🎾   🏓",
            "🏓      🎾  🏓",
            "🏓       🎾🏓 ",
            "🏓      🎾  🏓",
            "🏓     🎾   🏓",
            "🏓    🎾    🏓",
            "🏓   🎾     🏓",
            "🏓  🎾      🏓",
            " 🏓🎾       🏓",
        };
}