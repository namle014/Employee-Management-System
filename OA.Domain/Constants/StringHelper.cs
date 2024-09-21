using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Core.Constants
{
    public static class StringHelper
    {
        public static bool Equals(string firstString, string secondString)
        {
            if (!string.IsNullOrEmpty(firstString) && !string.IsNullOrEmpty(secondString))
            {
                return string.Equals(firstString, secondString, StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        public static bool Contains(string baseString, string seekString)
        {
            if (!string.IsNullOrEmpty(baseString) && !string.IsNullOrEmpty(seekString))
            {
                return baseString.ToLower().Contains(seekString.ToLower());
            }

            return false;
        }
    }
}
