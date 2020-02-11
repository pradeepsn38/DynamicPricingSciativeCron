using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DyanmicPricingTripCron
{
    class Program
    {
       public  static void Main(string[] args)
        {

            bool blnAllowTripSchedule = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowTripSchedule"]);
            if (blnAllowTripSchedule)
            {
                DynamicPricingTrip.RefreshTrips();
            }
     

            bool blnAllowBookingDetails = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowBookingDetails"]);
            if (blnAllowBookingDetails)
            {
                DynamicPricingTrip.RefreshBookingDetails();
            }



            bool blnAllowReplicationStatus = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowReplicationStatus"]);
            if (blnAllowReplicationStatus)
            {
                DynamicPricingTrip.CheckDifferenceReplication();
            }

        }
    }
}
