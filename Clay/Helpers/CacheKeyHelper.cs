using System.Collections.Generic;
using System.Linq;

namespace Clay.Helpers
{
    public static class CacheKeyHelper
    {
        public const string Delimeter = "_";
        public static List<string> KeyList = new List<string>();
        public static string GenerateKeyWithPagination(string typeName, int pageNumber, int pageSize)
        {
            var generatedKey = string.Format(typeName + Delimeter + pageNumber + Delimeter + pageSize);
            KeyList.Add(generatedKey);
            return generatedKey;
        }

        public static List<string> GetKeysStartsWith(string typename)
        {
            return KeyList.Where(k => k.StartsWith(typename)).ToList();
        }

        public static void RemoveKeysStartsWith(string typename)
        {
            KeyList.RemoveAll(k => k.StartsWith(typename));
        }
    }
}