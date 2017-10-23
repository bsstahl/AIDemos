using System;
using System.Collections.Generic;
using System.Text;

namespace ChutesAndLadders.Tools.ReplaceGuids
{
    public static class Extensions
    {
        public static Guid ToGuid(this string source)
        {
            Guid g = Guid.Empty;
            Guid.TryParse(source, out g);
            return g;
        }
    }
}
