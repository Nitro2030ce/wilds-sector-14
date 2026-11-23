using System.Collections.Frozen;
using Content.Shared.Random.Helpers;
using Content.Shared.Speech.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Content.Shared.Speech.EntitySystems;

public sealed partial class ScurretAccentSystem : RelayAccentSystem<ScurretAccentComponent>
{
    //private static readonly Regex RegexIng = new(@"(?<=\w\w)(in)g(?!\w)", RegexOptions.IgnoreCase);
    //private static readonly Regex RegexLowerOr = new(@"(?<=\w)o[Rr](?=\w)");
    //private static readonly Regex RegexUpperOr = new(@"(?<=\w)O[Rr](?=\w)");
    //private static readonly Regex RegexLowerAr = new(@"(?<=\w)a[Rr](?=\w)");
    //private static readonly Regex RegexUpperAr = new(@"(?<=\w)A[Rr](?=\w)");
    private static readonly Regex RegexUpperRMid = new(@"(?<=\w)R(?=\w)");
    private static readonly Regex RegexLowerRMid = new(@"(?<=\w)r(?=\w)");
    private static readonly Regex RegexUpperRFirst = new(@"R(?=\w)");
    private static readonly Regex RegexLowerRFirst = new(@"r(?=\w)");
    private static readonly Regex RegexUpperRLast = new(@"(?<=\w)R");
    private static readonly Regex RegexLowerRLast = new(@"(?<=\w)r");

    private static readonly Regex RegexUpperSMid = new(@"(?<=\w)S(?=\w)");
    private static readonly Regex RegexLowerSMid = new(@"(?<=\w)s(?=\w)");
    private static readonly Regex RegexUpperSFirst = new(@"S(?=\w)");
    private static readonly Regex RegexLowerSFirst = new(@"s(?=\w)");
    private static readonly Regex RegexUpperSLast = new(@"(?<=\w)S");
    private static readonly Regex RegexLowerSLast = new(@"(?<=\w)s");

    private static readonly Regex RegexFirstWord = new(@"^(\S+)");
    private static readonly Regex RegexLastWord = new(@"(\S+)$");
    private static readonly Regex RegexLastPunctuation = new(@"([.!?]+$)(?!.*[.!?])|(?<![.!?])$");

    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private ReplacementAccentSystem _replacement = default!;


    public override string Accentuate(string message, Entity<ScurretAccentComponent>? ent = null)
    {
        // Order:
        // Do text manipulations first
        // Then prefix/suffix funnyies

        var random = ent.HasValue
            ? SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent.Value))
            : _random;

        // direct word replacements
        var msg = _replacement.ApplyReplacements(message, "scurret", ent?.Owner);

        // thinking -> thinkin'
        // king -> king
        //Uses captures groups to make sure the captialization of IN is kept
        //msg = RegexIng.Replace(msg, "$1'");

        // or -> uh and ar -> ah in the middle of words (fuhget, tahget)
        //msg = RegexLowerOr.Replace(msg, "uh");
        //msg = RegexUpperOr.Replace(msg, "UH");
        //msg = RegexLowerAr.Replace(msg, "ah");
        //msg = RegexUpperAr.Replace(msg, "AH");

        msg = RegexUpperRMid.Replace(msg, "W");
        msg = RegexLowerRMid.Replace(msg, "w");
        msg = RegexUpperRFirst.Replace(msg, "W");
        msg = RegexLowerRFirst.Replace(msg, "w");
        msg = RegexUpperRLast.Replace(msg, "W");
        msg = RegexLowerRLast.Replace(msg, "w");

        msg = RegexUpperSMid.Replace(msg, "SH");
        msg = RegexLowerSMid.Replace(msg, "sh");
        msg = RegexUpperSFirst.Replace(msg, "SH");
        msg = RegexLowerSFirst.Replace(msg, "sh");
        msg = RegexUpperSLast.Replace(msg, "SH");
        msg = RegexLowerSLast.Replace(msg, "sh");

                // Prefix
        if (random.Prob(0.15f))
        {
            //Checks if the first word of the sentence is all caps
            //So the prefix can be allcapped and to not resanitize the captial
            var firstWordAllCaps = !RegexFirstWord.Match(msg).Value.Any(char.IsLower);
            var pick = random.Next(1, 2);

            // Reverse sanitize capital
            var prefix = Loc.GetString($"accent-scurret-prefix-{pick}");
            if (!firstWordAllCaps)
                msg = msg[0].ToString().ToLower() + msg.Remove(0, 1);
            else
                prefix = prefix.ToUpper();
            msg = prefix + " " + msg;
        }

        // Sanitize capital again, in case we substituted a word that should be capitalized
        msg = msg[0].ToString().ToUpper() + msg.Remove(0, 1);

        // Suffixes
        if (random.Prob(0.4f))
        {
            //Checks if the last word of the sentence is all caps
            //So the suffix can be allcapped
            var lastWordAllCaps = !RegexLastWord.Match(msg).Value.Any(char.IsLower);
            string suffix;
            var pick = random.Next(1, 9);
            suffix = Loc.GetString($"accent-scurret-suffix-{pick}");

            if (lastWordAllCaps)
                suffix = suffix.ToUpper();
            msg = RegexLastPunctuation.Replace(msg, suffix);
        }

        return msg;
    }
}
