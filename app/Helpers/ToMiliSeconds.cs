using System;
namespace app {
    public static class Helper 
    {
        public static double ToMiliseconds (DateTime date) 
        {
            // TODO : append GUIDD
            var mili =  date.ToUniversalTime ().Subtract (new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return Math.Round(mili,2);
        }
    }
}