using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Colorful;
using Humanizer;
using Console = System.Console;

namespace CaddyLogParser
{
    class Program
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        static void Main(string[] args)
        {
            string s;
            while ((s = Console.ReadLine()) != null)
            {
                var sx = AccessLogItem.FromJson(s);
                var format = "{0} - {1} > {2} {3} {4}://{5}{6} - {7} {8} - {9}, {10} - {11}";
                

                var ip = sx.Request.RemoteAddress.Split(":");
                ip = ip.Take(ip.Length - 1).ToArray();
                Formatter[] log = new Formatter[]
                {
                    /*0*/
                    new Formatter(UnixTimeStampToDateTime(sx.Timestamp).ToString("G", CultureInfo.InvariantCulture),
                        Color.CornflowerBlue),
                    /*1*/new Formatter(String.Join("", ip), Color.DeepSkyBlue),
                    /*2*/
                    new Formatter(sx.Request.Proto,
                        Color.CadetBlue),
                    /*3*/
                    new Formatter(sx.Request.Method,
                        Color.CadetBlue),
                    /*4*/
                    new Formatter(sx.Request.TlsStatus != null ? "https" : "http",
                        sx.Request.TlsStatus != null ? Color.GreenYellow : Color.IndianRed),
                    /*5*/
                    new Formatter(sx.Request.Host, Color.White),
                    /*6*/
                    new Formatter(sx.Request.Uri, Color.LightGray),
                    /*7*/
                    new Formatter(sx.Status, Color.Aquamarine),
                    /*8*/
                    new Formatter(((HttpStatusCode) sx.Status).Humanize(LetterCasing.Title), Color.Aquamarine),
                    /*9*/
                    new Formatter(TimeSpan.FromSeconds(sx.Duration).Humanize(), Color.LemonChiffon),
                    /*10*/
                    new Formatter(sx.Size.Bytes().ToString("#.##"), Color.Linen),
                    /*11*/
                    new Formatter(sx.Request.Headers.Keys.Contains("User-Agent") ? sx.Request.Headers["User-Agent"].Humanize() : "", Color.Gold),
                };

                Colorful.Console.WriteLineFormatted(format, Color.Gray, log);
            }
        }
    }
}