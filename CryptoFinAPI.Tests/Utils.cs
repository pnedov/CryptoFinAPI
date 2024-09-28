//using Microsoft.AspNetCore.Http.Extensions;

//namespace CryptoFinAPI;

///// <summary>
///// Class for utils methods
///// </summary>
//public class Utils
//{
//    /// <summary>
//    /// Convert from unix milliseconds timespan to datetime
//    /// </summary>
//    /// <param name="unixTimeStamp">unix timespan</param>
//    /// <returns></returns>
//    public static DateTime convertUnixMillisecondsToDateTime(string unixTimeStamp)
//    {
//        var result = DateTime.UnixEpoch.AddMilliseconds(double.Parse(unixTimeStamp, System.Globalization.CultureInfo.InvariantCulture));

//        return result;
//    }

//    /// <summary>
//    /// convert from unix seconds timespan to datetime
//    /// </summary>
//    /// <param name="unixTimeStamp"></param>
//    /// <returns></returns>
//    public static DateTime convertUnixSecondsToDateTime(string unixTimeStamp)
//    {
//        var result = DateTime.UnixEpoch.AddSeconds(double.Parse(unixTimeStamp, System.Globalization.CultureInfo.InvariantCulture));

//        return result;
//    }

//    /// <summary>
//    /// Convert Datetime to timespan
//    /// </summary>
//    /// <param name="dateTime">date time</param>
//    /// <returns></returns>
//    public static long convertDateTimeToTimespan(DateTime dateTime)
//    {
//        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
//    }

//    /// <summary>
//    /// Build URI 
//    /// </summary>
//    /// <param name="basePath">url path</param>
//    /// <param name="queryParams">uri params</param>
//    /// <returns></returns>
//    public static string buildUrl(string basePath, Dictionary<string, string> queryParams)
//    {
//        var queryBuilder = new QueryBuilder(queryParams);
//        return basePath + queryBuilder;
//    }

//    /// <summary>
//    /// Format from/to timestamp from seconds to milliseconds
//    /// </summary>
//    /// <param name="startTimePoint">start timestamp</param>
//    /// <param name="endTimePoint">end timestamp</param>
//    public static void convertToMillisecondsRange(ref string startTimePoint, ref string endTimePoint)
//    {
//        if (!string.IsNullOrEmpty(startTimePoint))
//        {
//            startTimePoint = Convert.ToString(long.Parse(startTimePoint) * 1000);
//        }

//        if (!string.IsNullOrEmpty(endTimePoint))
//        {
//            endTimePoint = Convert.ToString(long.Parse(endTimePoint) * 1000);
//        }
//    }

//    /// <summary>
//    /// Format from/to timestamp from seconds to milliseconds
//    /// </summary>
//    /// <param name="startTimePoint">start timestamp</param>
//    public static void convertToMilliseconds2(ref string startTimePoint)
//    {
//        startTimePoint = Convert.ToString(long.Parse(startTimePoint) * 1000);
//    }

//    /// <summary>
//    /// Format from/to timestamp from seconds to milliseconds
//    /// </summary>
//    /// <param name="startTimePoint">start timestamp</param>
//    /// <param name="endTimePoint">end timestamp</param>
//    public static void convertToSeconds(ref string startTimePoint, ref string endTimePoint)
//    {
//        startTimePoint = Convert.ToString(long.Parse(startTimePoint) / 1000);
//        if (!string.IsNullOrEmpty(endTimePoint))
//        {
//            endTimePoint = Convert.ToString(long.Parse(endTimePoint) / 1000);
//        }
//    }

//    /// <summary>
//    /// Format from/to timestamp from seconds to milliseconds
//    /// </summary>
//    /// <param name="startTimePoint">start timestamp</param>
//    public static void convertToSeconds2(ref string startTimePoint)
//    {
//        startTimePoint = Convert.ToString(long.Parse(startTimePoint) / 1000);
//    }
//}
