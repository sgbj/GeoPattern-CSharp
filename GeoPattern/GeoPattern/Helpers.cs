using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern
{
    public static class Helpers
    {
        public static string Sha1HexDigest(this string s)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
                return BitConverter.ToString(sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLower();
        }

        public static Dictionary<K, V> Merge<K, V>(this Dictionary<K, V> a, Dictionary<K, V> b)
        {
            var d = new Dictionary<K, V>();
            (a ?? d).ToList().ForEach(e => d[e.Key] = e.Value);
            (b ?? d).ToList().ForEach(e => d[e.Key] = e.Value);
            return d;
        }

        public static string ToAttributeString(this Dictionary<string, object> d)
        {
            var sb = new StringBuilder();
            foreach (var e in d)
            {
                var key = e.Key;
                var value = e.Value is Dictionary<string, object> ?
                    string.Join(";", ((Dictionary<string, object>)e.Value).ToList().Select(p => p.Key + ":" + p.Value)) :
                    e.Value;
                sb.AppendFormat("{0}=\"{1}\" ", key, value);
            }
            return sb.ToString();
        }

        public static string ToBase64(this string s)
        {
            return Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(s));
        }
    }
}
