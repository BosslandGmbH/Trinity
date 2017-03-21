using System;
using System.Security.Cryptography;
using System.Text;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Helpers
{
    public static class HashGenerator
    {
        /*
         * Reference implementation: http://msdn.microsoft.com/en-us/library/s02tk69a.aspx
         */

        public static string GenerateItemHash(Vector3 position, int actorSNO, string name, int worldID, ItemQuality itemQuality, int itemLevel)
        {
            using (MD5 md5 = MD5.Create())
            {
                string itemHashBase = String.Format("{0}{1}{2}{3}{4}{5}", position, actorSNO, name, worldID, itemQuality, itemLevel);
                string itemHash = GetMd5Hash(md5, itemHashBase);
                return itemHash;
            }
        }
        internal static string GetMd5Hash(MD5 md5Hash, string input)
        {
            using (new PerformanceLogger("GetMd5Hash"))
            {
                byte[] data = md5Hash.ComputeHash(Encoding.Default.GetBytes(input));
                return BitConverter.ToString(data);
            }
        }

        private static UInt64 CalculateKnuthHash(string read)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

    }
}