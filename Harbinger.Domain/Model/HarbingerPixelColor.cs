using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerPixelColor(ushort Red, ushort Green, ushort Blue, ushort Alpha)
{


    public override string ToString() => $"rgb({Red}, {Green}, {Blue}, {Alpha})";


    public static HarPixCo operator |(HarPixCo fst, HarPixCo snd) => new HarPixCo(Red: fst.Red.Avg(snd.Red), Green: fst.Green.Avg(snd.Green), Blue: fst.Blue.Avg(snd.Blue), Alpha: fst.Alpha.Avg(snd.Alpha));




}

public static class HarbingerPixelColorExtensions
{

    public static HarPixCo ToColor(this ushort[] arr) => arr.Length == 4 ? new HarPixCo(arr[0], arr[1], arr[2], arr[3]) : new HarPixCo(arr[0], arr[1], arr[2], 0);

    public static HarPixCo AvgPix(this IEnumerable<HarPixCo> input) => input
        .Cipe(
            _ => _.Any(),
            coll => new HarPixCo(Red: coll.Avg(_ => _.Red), Green: coll.Avg(_ => _.Green), Blue: coll.Avg(_ => _.Blue), Alpha: coll.Avg(_ => _.Alpha))
        ) ?? new HarPixCo(0, 0, 0, 0);


    public static HarPixCo? ParseColor(this string inp) => inp.ParseRgbaColor() ?? inp.ParseRgbColor() ?? inp.ParseHexAColor() ?? inp.ParseHexColor();

    public static HarPixCo? ParseRgbaColor(this string inp) => inp.ParseColor(RGBARegex, str =>
    {
        var splitted = str.Split(',')
            .Select(_ => _.Trim())
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .ToArray();
        if (splitted.Length != 4)
            return null;
        var cols = new ushort[4];
        for(var indx = 0; indx < cols.Length; indx++)
        {
            if (!ushort.TryParse(splitted[indx], out var psd))
                return null;
            cols[indx] = psd;
        }
        return cols.ToColor();
    });

    public static HarPixCo? ParseRgbColor(this string inp) => inp.ParseColor(RGBRegex, str =>
    {
        var splitted = str.Split(',')
            .Select(_ => _.Trim())
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .ToArray();
        if (splitted.Length != 3)
            return null;
        var cols = new ushort[3];
        for (var indx = 0; indx < cols.Length; indx++)
        {
            if (!ushort.TryParse(splitted[indx], out var psd))
                return null;
            cols[indx] = psd;
        }
        return cols.ToColor();
    });

    public static HarPixCo? ParseHexAColor(this string inp) => inp.ParseColor(HexARegex, str =>
    {
        if (str.Length != 8)
            return null;
        var cols = new ushort[4];
        for (var indx = 0; indx < cols.Length; indx++)
        {
            var substr = str.Substring(indx * 2, length: 2).ToUpper();
            if (!ushort.TryParse(substr, style: NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var psd))
                return null;
            cols[indx] = psd;
        }
        return cols.ToColor();
    });


    public static HarPixCo? ParseHexColor(this string inp) => inp.ParseColor(HexRegex, str =>
    {
        if (str.Length != 6)
            return null;
        var cols = new ushort[3];
        for (var indx = 0; indx < cols.Length; indx++)
        {
            var substr = str.Substring(indx * 2, length: 2).ToUpper();
            if (!ushort.TryParse(substr, style: NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var psd))
                return null;
            cols[indx] = psd;
        }
        return cols.ToColor();
    });


    private static HarPixCo? ParseColor(this string inp, Regex regex, Func<string, HarPixCo?> mapper) 
    {
        var matches = regex.Matches(inp);
        var match = matches.FirstOrDefault();
        if (match == null || match.Groups.Count != 2)
            return null;
        var groupVal = match.Groups[1].Value;
        var returnee = mapper(groupVal);
        return returnee;
    }


    private static readonly Regex RGBARegex = new Regex(@"rgb\((\d+,\d+,\d+,\d+)\)", RegexOptions.IgnoreCase);
    private static readonly Regex RGBRegex = new Regex(@"rgb\((\d+,\d+,\d+)\)", RegexOptions.IgnoreCase);
    private static readonly Regex HexARegex = new Regex(@"#(\w{2}\w{2}\w{2}\w{2})");
    private static readonly Regex HexRegex = new Regex(@"#(\w{2}\w{2}\w{2})");

}
