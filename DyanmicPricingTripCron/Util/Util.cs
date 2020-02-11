using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DyanmicPricingTripCron.Util
{
    public class Util
    {

        public static DateTime GetServerDateTime()
        {
            int intServerMinsOffset = 0;
            int.TryParse(ConfigurationManager.AppSettings["ServerOffsetMins"], out intServerMinsOffset);
            return DateTime.Now.AddMinutes(intServerMinsOffset);
        }
    }
}
