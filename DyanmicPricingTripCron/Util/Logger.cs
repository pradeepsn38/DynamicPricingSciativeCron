using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;


namespace DyanmicPricingTripCron.Util
{
   public  class Logger
    {
        public static void WriteLog(string data)
        {
            try
            {
                int intServerMinsOffset = Convert.ToInt32(ConfigurationManager.AppSettings["ServerOffsetMins"]);
                StreamWriter log;
                string logpath = ConfigurationManager.AppSettings["LogPath"];
                if (!File.Exists(@logpath + "logfile_" + DateTime.Now.AddMinutes(intServerMinsOffset).ToString("dd-MMM-yyy") + ".txt"))
                {
                    log = new StreamWriter(@logpath + "logfile_" + DateTime.Now.AddMinutes(intServerMinsOffset).ToString("dd-MMM-yyy") + ".txt");
                }
                else
                {
                    log = File.AppendText(@logpath + "logfile_" + DateTime.Now.AddMinutes(intServerMinsOffset).ToString("dd-MMM-yyy") + ".txt");
                }

                string currenttime = Util.GetServerDateTime().ToString("dd-MMM-yyy HH:mm:ss.fff");
                if (data != "")
                    log.WriteLine(currenttime + " : " + data);

                log.Close();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        public static void WriteLogFailure(string data)
        {
            try
            {
                int intServerMinsOffset = Convert.ToInt32(ConfigurationManager.AppSettings["ServerOffsetMins"]);
                StreamWriter log;
                string logpath = ConfigurationManager.AppSettings["LogFailurePath"];
                if (!File.Exists(@logpath + "logfile_" + DateTime.Now.AddMinutes(intServerMinsOffset).ToString("dd-MMM-yyy") + ".txt"))
                {
                    log = new StreamWriter(@logpath + "logfile_" + DateTime.Now.AddMinutes(intServerMinsOffset).ToString("dd-MMM-yyy") + ".txt");
                }
                else
                {
                    log = File.AppendText(@logpath + "logfile_" + DateTime.Now.AddMinutes(intServerMinsOffset).ToString("dd-MMM-yyy") + ".txt");
                }

                string currenttime = Util.GetServerDateTime().ToString("dd-MMM-yyy HH:mm:ss.fff");
                if (data != "")
                    log.WriteLine(currenttime + " : " + data);

                log.Close();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        public static void WriteLog(string heading, string subheading, string data)
        {
            string msg = heading + "::" + subheading + "::" + data;
            WriteLog(msg);
        }

        public static void WriteLogFailure(string heading, string subheading, string data)
        {
            string msg = heading + "::" + subheading + "::" + data;
            WriteLogFailure(msg);
        }
    }
}
