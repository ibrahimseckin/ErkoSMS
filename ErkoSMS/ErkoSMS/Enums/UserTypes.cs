using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.Enums
{
    public static class UserTypes
    {
        public static (string Name, string DisplayName) Administrator { get; } = ("Administrator", "Yönetici");
        public static (string Name, string DisplayName) SalesMan { get; } = ("SalesMan", "Satış Görevlisi");
        public static (string Name, string DisplayName) Purchaser { get; } = ("Purchaser", "Satın Almacı");
        public static (string Name, string DisplayName) Accountant { get; } = ("Accountant", "Muhasebeci");
        public static (string Name, string DisplayName) WareHouseMan { get; } = ("WareHouseMan", "Depo Görevlisi");


        public static string GetDisplayName(string usertypeName)
        {
            object Name = "", DisplayName = "";
            var prop = typeof(UserTypes).GetProperties().FirstOrDefault(x => x.Name.Equals(usertypeName));
            if (prop != null)
            {
                return (((string Name, string DisplayName))prop.GetValue("")).DisplayName;
            }
            return "";

        }

    }
}