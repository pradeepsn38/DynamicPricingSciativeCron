using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DyanmicPricingTripCron.Util;
using System.Configuration;
using System.Threading;
using System.Data;
using DyanmicPricingTripCron.CRSAPI;

namespace DyanmicPricingTripCron
{
    public class DynamicPricingTrip
    {

        public static void RefreshTrips()
        {
            Logger.WriteLog("Trips Refresh Started...");
           
            DateTime currentDate = Util.Util.GetServerDateTime();
       
            DateTime journeyDateTo = currentDate.AddDays(-1);

            CRSDAL dal = new CRSDAL();
            string strerror = "";
            string strErr1 = "";
            DataSet dstSciative = dal.ExecuteSelect("spdnpGetSceheduleTrips", CommandType.StoredProcedure, 0, ref strerror, "p_ErrMessage", false, "", false);

            if (strerror != "")
            {
                Logger.WriteLog("No Row Found Getting error while retriving " + strerror + "," + currentDate);
                Logger.WriteLogFailure("No Row Found Getting error while retriving " + strerror + "," + currentDate);
                Email.SendMail("Connection Issue BufferSlave While Getting Trips: " + strerror + "," + currentDate);
                SMS.SMSSent("Connection Issue BufferSlave While Getting Trips: " + strerror + "," + currentDate);
            }
  
            if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables.Count > 0)
            {
                DataTable dtConnectionSciative = dstSciative.Tables[0];
                if (dtConnectionSciative != null && dtConnectionSciative.Rows != null && dtConnectionSciative.Rows.Count>0)
                {

                    Logger.WriteLog("RowCount " + dtConnectionSciative.Rows.Count);
                    for (int k = 0; k < dtConnectionSciative.Rows.Count; k++)
                    {

                        DataRow drCompanyTrips = dtConnectionSciative.Rows[k];
                        int id = Convert.ToInt32(drCompanyTrips["id"].ToString());
                        int tripid = Convert.ToInt32(drCompanyTrips["tripid"].ToString());
                        int  companyid = Convert.ToInt32(drCompanyTrips["companyid"].ToString());
                        string strfromdate = drCompanyTrips["fromdate"].ToString();
                        string strtodate = drCompanyTrips["todate"].ToString();

                        if (companyid==10 && strfromdate != "" && strtodate != "")
                        {
                            DateTime fromdate = Convert.ToDateTime(strfromdate);
                            DateTime todate = Convert.ToDateTime(strtodate);

                            CRSDAL dal1 = new CRSDAL();
                           
                            dal1.AddParameter("p_TripId", tripid, ParameterDirection.Input);
                            dal1.AddParameter("p_UniqueID", id, ParameterDirection.Input);
                            dal1.AddParameter("p_DateStart", fromdate, ParameterDirection.Input);
                            dal1.AddParameter("p_DateEnd", todate, ParameterDirection.Input);

                            DataSet dstOutPut = dal1.ExecuteSelect("dnp_spServiceSubroutes_InsertUpdate_TripCurrDatePartNew", CommandType.StoredProcedure, 0, ref strErr1, "p_ErrMessage", false, "", false);


                        }
                        else
                        {

                            CRSDAL dal1 = new CRSDAL();
                            //string strErr1 = "";
                            dal1.AddParameter("p_TripId", tripid, ParameterDirection.Input);
                            dal1.AddParameter("p_UniqueID", id, ParameterDirection.Input);
                          

                            DataSet dstOutPut = dal1.ExecuteSelect("Test_utk_dnp_spServiceSubroutes_InsertUpdate_TripCurrDatePart", CommandType.StoredProcedure, 0, ref strErr1, "p_ErrMessage", false, "", false);


                        }


                        if (strErr1 != "")
                        {
                            Logger.WriteLog("No Row Found Getting error while Refreshing " + strerror + "," + "TripID: " + tripid + "," + "ID: " + id + "," + currentDate);
                            Logger.WriteLogFailure("No Row Found Getting error while Refreshing " + strerror + "," + "TripID: " + tripid + "," + "ID: " + id + "," + currentDate);
                            Email.SendMail("Connection Issue BufferSlave While Refreshing: " + strerror + "," + "TripID: " + tripid + "," + "ID: " + id + "," + currentDate);
                            //SMS.SMSSent("Connection Issue BufferSlave While Refreshing: " + strerror + "," + currentDate);
                        }
                        else
                        {
                            Logger.WriteLog("Trips Refresh END Successfully...  " + "TripID: " + tripid + "," + "ID: " + id + "," + currentDate);
                        }
                       

                    }


                }
                else
                {
                    Logger.WriteLog("No Row Found  while Refreshing");
                    Logger.WriteLogFailure("No Row Found while Refreshing");
                    Email.SendMail("No Row Found: " + dtConnectionSciative.Rows.Count);
                    
                }
            }
            else
            {
                Logger.WriteLog("No Record Found");
                Logger.WriteLogFailure("No Record Found while Refreshing");
                Email.SendMail("No Record Found: " + dstSciative.Tables.Count);
            }
        }

        public static void CheckDifferenceReplication()
        {
            Logger.WriteLog("Check Difference Replication Started...");

            DateTime currentDate = Util.Util.GetServerDateTime();
            int[] companiesid = new int[] { 3398, 69, 406, 1 };

            for (int k = 0; k < companiesid.Length; k++)
            {
                int CompanyId = companiesid[k];
                CRSDAL dal = new CRSDAL();
                string mismatch = "";
                string strerror = "";
                dal.AddParameter("p_CompanyID", CompanyId, ParameterDirection.Input);
                dal.AddParameter("p_date", currentDate, ParameterDirection.Input);
                DataSet dstSciative = dal.ExecuteSelect("spdnpGetCheckRelicationDifference", CommandType.StoredProcedure, 0, ref strerror, "p_ErrMessage", false, "", false);

                if (strerror != "")
                {
                    Logger.WriteLog("No Row Found Getting error while retriving " + strerror + ", " + "CompanyId: " + CompanyId + ", " + currentDate);
                    Logger.WriteLogFailure("No Row Found Getting error while retriving " + strerror + ", " + "CompanyId: " + CompanyId + ", " + currentDate);
                    Email.SendMail("Connection Issue BufferSlave While Checking  Difference: " + strerror + "," + "CompanyId: " + CompanyId + "," + currentDate);
                }
                else
                {
                    if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables[0].Rows.Count > 0)
                    {
                        string Bookingids = Convert.ToString(dstSciative.Tables[0].Rows[0]["BookingIDs"]);

                        if (Bookingids != "" && Bookingids != "0")
                        {
                            mismatch = "Mismatch";
                            Logger.WriteLog("Booking  Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "BookingIDs: " + Bookingids);
                            Email.SendMail("Booking  Difference: "  + "CompanyId: " + CompanyId + "," + currentDate + "," + "BookingIDs: " + Bookingids);
                           
                        }
                    }
                    if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables[1].Rows.Count > 0)
                    {
                        string TripIDs = Convert.ToString(dstSciative.Tables[1].Rows[0]["TripIDs"]);

                        if (TripIDs != "" && TripIDs != "0")
                        {
                            mismatch = "Mismatch";
                            Logger.WriteLog("TripSchedule  Fare Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            Email.SendMail("TripSchedule  Fare Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            SMS.SMSSent("TripSchedule Fare Difference:: " + strerror + "," + currentDate + "," + "TripIDs: " + TripIDs);
                        }
                    }
                    if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables[2].Rows.Count > 0)
                    {
                        string TripIDs = Convert.ToString(dstSciative.Tables[2].Rows[0]["TripIDs"]);

                        if (TripIDs != "" && TripIDs != "0")
                        {
                            mismatch = "Mismatch";
                            Logger.WriteLog("TripSchedule  Date Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            Email.SendMail("TripSchedule Date  Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            SMS.SMSSent("TripSchedule Date Difference:: " + strerror + "," + currentDate + "," + "TripIDs: " + TripIDs);
                        }
                    }
                    if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables[3].Rows.Count > 0)
                    {
                        string TripIDs = Convert.ToString(dstSciative.Tables[3].Rows[0]["TripIDs"]);

                        if (TripIDs != "" && TripIDs != "0")
                        {
                            mismatch = "Mismatch";
                            Logger.WriteLog("Special  Quota Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            Email.SendMail("Special Quota  Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            SMS.SMSSent("Special Quota Difference:: " + strerror + "," + currentDate + "," + "TripIDs: " + TripIDs);
                        }
                    }
                    if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables[4].Rows.Count > 0)
                    {
                        string TripIDs = Convert.ToString(dstSciative.Tables[4].Rows[0]["TripIDs"]);

                        if (TripIDs != "" && TripIDs != "0")
                        {
                            mismatch = "Mismatch";
                            Logger.WriteLog("Partial  Quota Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            Email.SendMail("Partial Quota  Difference: " + "CompanyId: " + CompanyId + "," + currentDate + "," + "TripIDs: " + TripIDs);
                            SMS.SMSSent("Special Quota Difference:: " + strerror + "," + currentDate + "," + "TripIDs: " + TripIDs);
                        }
                    }
                    if (mismatch == "")
                    {
                        Email.SendMail("No Mismatch Found While Check Difference : " + "CompanyId: " + CompanyId + "," + currentDate );
                    }
                }

            }

        }


        public static void RefreshBookingDetails()
        {
            Logger.WriteLog("Booking Refresh Started...");

            DateTime currentDate = Util.Util.GetServerDateTime();

            DateTime journeyDateTo = currentDate.AddDays(-1);

            CRSDAL dal = new CRSDAL();
            string strerror = "";
            DataSet dstSciative = dal.ExecuteSelect("spdnpGetBookingDetails", CommandType.StoredProcedure, 0, ref strerror, "p_ErrMessage", false, "", false);

            if (strerror != "")
            {
                Logger.WriteLog("No Row Found Getting error while retriving Bookings" + strerror + "," + currentDate);
                Logger.WriteLogFailure("No Row Found Getting error while retriving Bookings" + strerror + "," + currentDate);
                Email.SendMail("Connection Issue BufferSlave While Getting Bookings: " + strerror + "," + currentDate);
                //SMS.SMSSent("Connection Issue BufferSlave While Getting Bookings: " + strerror + "," + currentDate);
            }

            if (dstSciative != null && dstSciative.Tables != null && dstSciative.Tables.Count > 0)
            {
                DataTable dtConnectionSciative = dstSciative.Tables[0];
                if (dtConnectionSciative != null && dtConnectionSciative.Rows != null && dtConnectionSciative.Rows.Count > 0)
                {
                    Logger.WriteLog("RowCount " + dtConnectionSciative.Rows.Count);
                    for (int k = 0; k < dtConnectionSciative.Rows.Count; k++)
                    {

                        DataRow drCompanyTrips = dtConnectionSciative.Rows[k];
                     //   int id = Convert.ToInt32(drCompanyTrips["id"].ToString());
                        int bookingid = Convert.ToInt32(drCompanyTrips["bookingid"].ToString());
                        DateTime entrytime = Convert.ToDateTime(drCompanyTrips["entrydatetime"].ToString());

                        CRSDAL dal1 = new CRSDAL();
                        string strErr1 = "";
                        dal1.AddParameter("p_BookingID", bookingid, ParameterDirection.Input);
                        dal1.AddParameter("p_EntryDateTime", entrytime, ParameterDirection.Input);
                       // dal1.AddParameter("p_UniqueID", id, ParameterDirection.Input);

                        DataSet dstOutPut = dal1.ExecuteSelect("dnp_spProcessTripwise_Occupancy_New", CommandType.StoredProcedure, 0, ref strErr1, "p_ErrMessage", false, "", false);

                        if (strErr1 != "")
                        {
                            Logger.WriteLog("No Row Found Getting error while Refreshing Bookings" + strerror + "," + "BookingID: " + bookingid + "," + currentDate);
                            Logger.WriteLogFailure("No Row Found Getting error while Refreshing Bookings" + strerror + "," + "BookingID: " + bookingid +  "," + currentDate);
                            Email.SendMail("Connection Issue BufferSlave While Refreshing Bookings: " + strerror + "," + "BookingID: " + bookingid  + "," + currentDate);
                           // SMS.SMSSent("Connection Issue BufferSlave While Refreshing Bookings: " + strerror + "," + currentDate);
                        }
                        else
                        {
                            Logger.WriteLog("Bookings Refresh END Successfully...  " + "BookingID: " + bookingid + ","  + currentDate);
                        }


                    }


                }
                else
                {
                    Logger.WriteLog("No Row Found  while Refreshing Bookings");
                    Logger.WriteLogFailure("No Row Found while Refreshing Booking");
                    Email.SendMail("No Row Found: " + dtConnectionSciative.Rows.Count);

                }
            }
            else
            {
                Logger.WriteLog("No Record Found");
                Logger.WriteLogFailure("No Record Found while Refreshing Booking Bookings");
                Email.SendMail("No Record Found: " + dstSciative.Tables.Count);
            }
        }
    }
}
