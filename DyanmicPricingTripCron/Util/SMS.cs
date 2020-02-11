using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace DyanmicPricingTripCron.Util
{
    public class SMS
    {

        public static void SMSSent(string Message)
        {
            string strResult = "";

            try
            {
                System.Net.WebRequest request = null;
                System.Net.WebResponse response = null;
                string SMSUserName = Convert.ToString(ConfigurationManager.AppSettings["SMSLogin"]);
                string SMSPassword = Convert.ToString(ConfigurationManager.AppSettings["SMSPassword"]);
                string SMSSenderID = Convert.ToString(ConfigurationManager.AppSettings["SMSSenderID"]);
                //string strURL = Convert.ToString(ConfigurationManager.AppSettings["SMSURL"]);
                //string strURL = "http://www.myvaluefirst.com/smpp/sendsms?username=##login##&password=##pwd##&to=##mobile##&from=##senderid##&text=##message##";
                string MobileNo = Convert.ToString(ConfigurationManager.AppSettings["SMSMobileNos"]);
                string[] mobileNos = MobileNo.Split(',');

                foreach (string mobile in mobileNos)
                {
                    string strURL = "http://www.myvaluefirst.com/smpp/sendsms?username=##login##&password=##pwd##&to=##mobile##&from=##senderid##&text=##message##";

                    strURL = strURL.Replace("##login##", SMSUserName);
                    strURL = strURL.Replace("##pwd##", SMSPassword);
                    strURL = strURL.Replace("##senderid##", SMSSenderID);
                    strURL = strURL.Replace("##mobile##", mobile);
                    strURL = strURL.Replace("##message##", Message);
                    strURL = strURL.Replace("#", "");
                    string ReqURL = strURL;
                    string mStatus = "SID:" + 1 + ", No:" + MobileNo + ", URL:" + ReqURL + ", Sts:";
                    try
                    {
                        request = (System.Net.WebRequest)System.Net.WebRequest.Create(ReqURL);

                        // Get response   
                        response = (System.Net.WebResponse)request.GetResponse();

                        System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());

                        string Str = "";
                        Str = reader.ReadToEnd();

                        if (response != null)
                            response.Close();

                        mStatus = mStatus + "[" + Str + "]";
                        strResult = mStatus;

                        if ((Str.Contains("-") && Str.Contains("_")) || Str.ToUpper().Contains("OK") || Str.ToUpper().Contains("SENT") ||
                            (Str.Contains("-") && Str.Split('-')[0].Trim().Length == 12) || Str.ToUpper().Contains("SUCCESS")
                            || (Str.Contains(":") && Str.Contains("|") && Str.Contains("-")) || Str.Contains("3001"))
                        {
                            // SMSLogRequestResponse(Convert.ToInt32(SMSId), ReqURL, Str, 1, intUserID);
                            // strSentMobile += "," + MobileNo;
                            Logger.WriteLog("SMS Succesfully Sent...");
                        }
                        else
                        {
                            //SMSLogRequestResponse(Convert.ToInt32(SMSId), ReqURL, Str, 0, intUserID);
                            Logger.WriteLog("SMS Sent Error..." + Str);
                            Logger.WriteLogFailure("SMS Sent Error..." + Str);
                        }
                    }
                    catch (Exception ex)
                    {
                        // SMSLogRequestResponse(Convert.ToInt32(SMSId), ReqURL, ex.Message, 0, intUserID);
                        Logger.WriteLog("SMS Exception..." + ex);
                        Logger.WriteLogFailure("SMS Sent Exception..." + ex);
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.WriteLog("SMS Exception..." + ex);
                Logger.WriteLogFailure("SMS Sent Exception..." + ex);
            }
          
           
        }
    }
}
