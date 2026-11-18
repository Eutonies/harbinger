using System.Text;

namespace Harbinger.UI.Util;

public static class StringExtensions
{


    public static string Repeat(this string toRepeat, int noOfTimes)
    {
        var sb = new StringBuilder();
        foreach (var indx in Enumerable.Range(0, noOfTimes))
            sb.Append(toRepeat);
        return sb.ToString();
    }
    public static string ToSpaces(this int noOfTimes) => " ".Repeat(noOfTimes);


}
