using System;
using System.Linq;

namespace ChutesAndLadders.Tools.ReplaceGuids
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = new IdCache();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                string newLine = line;
                var elements = line.Split(',');
                Guid g = elements[0].ToGuid();
                if (g != Guid.Empty)
                {
                    elements[0] = cache.Locate(g).ToString();
                    newLine = string.Join(',', elements);
                }

                Console.WriteLine(newLine);
            }
        }
    }
}
