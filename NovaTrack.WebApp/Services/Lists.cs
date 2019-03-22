using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NovaTrack.WebApp
{
    public static class Lists
    {
        public static Dictionary<string, string> AssetTypes()
        {
            return new Dictionary<string, string>
            {
                { "GLASS_FRIDGE", "Glass Fridge" },
                { "FREEZER", "Freezer" },
                { "FRIDGE", "Fridge" },
            };
        }

        public static Dictionary<string, string> Premises()
        {
            return new Dictionary<string, string>
            {
                { "RESTAURANT", "Restaurant" },
                { "HOTEL", "Hotel" },
                { "CLUB", "Club" },
                { "SUPERMARKET", "Supermarket" },
                { "STORE", "Store" },
                { "HOME", "Home" },
                { "WAREHOUSE", "Warehouse" },
                { "OTHER", "Other" },
            };
        }
    }
}
