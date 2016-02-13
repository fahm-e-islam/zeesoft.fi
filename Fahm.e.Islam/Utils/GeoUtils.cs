namespace Zeesoft.MVC.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using WorldDomination.GeographyServices.Services.Microsoft;

    public class GeoUtils
    {
        #region Methods

        public static byte[] FindPointWKB(string Lat, string Lon)
        {
            byte[] wkbToReturn = new byte[0];

            var geoParser = new GeoParse();

            wkbToReturn = geoParser.Parse(decimal.Parse(Lat), decimal.Parse(Lon));

            return wkbToReturn;
        }

        #endregion Methods
    }
}