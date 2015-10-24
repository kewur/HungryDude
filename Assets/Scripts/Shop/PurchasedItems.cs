using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Shop
{
    public static class PurchasedItems
    {
        private static List<ChefType> _ChefSkins = new List<ChefType>() { ChefType.Default, ChefType.Mega };
        public static List<ChefType> Chefskins
        {
            get
            {
                return _ChefSkins;
            }

            private set
            {
                if (_ChefSkins == value)
                    return;

                _ChefSkins = value;
            }
        }

        private static List<string> _PlayerSkins = new List<string>() { "Default" };
        public static List<string> PlayerSkins
        {
            get
            {
                return _PlayerSkins;
            }

            private set
            {
                if (_PlayerSkins == value)
                    return;

                _PlayerSkins = value;
            }
        }

        public static string ActivePlayerSkin = "Default";
    }
}
