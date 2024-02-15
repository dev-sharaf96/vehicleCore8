using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Cancellation.Utilities
{
    public class SmsMessage
    {
        //bool Connected=false;
        /// <summary>
        /// 
        /// </summary>
        int Accountid = 0;
        /// <summary>
        /// 
        /// </summary>
        public SmsMessage()
        {
        }

        /// <summary>
        ///  Must call that function  to initilize the account		
        /// </summary>	
        /// <returns>0 valid username or password</returns>
        /// <returns>-1 wrong username or password</returns>
        public int Initialize(string Username, string Password)
        {
            //check the account if ok Send 0 else -1
            Accountid = CheckAccount(Username, Password);
            if (Accountid > 0)
            {
                //Connected=true;
                return 0;
            }
            else
                return -1;
        }

        /// <summary>
        ///  check the username and the account 
        /// </summary>	
        /// <returns>0 valid username or password</returns>
        /// <returns>-1 wrong username or password</returns>
        private int CheckAccount(string Username, string Password)
        {

            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_AccountLogin", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameter1 = new SqlParameter("@AU_Login", SqlDbType.NVarChar, 50);
            parameter1.Value = Username;
            sqlCommand.Parameters.Add(parameter1);

            SqlParameter parameter2 = new SqlParameter("@AU_Password", SqlDbType.NVarChar, 50);
            parameter2.Value = Password;
            sqlCommand.Parameters.Add(parameter2);

            SqlParameter parameter4 = new SqlParameter("@AU_Id", SqlDbType.Int, 4);
            parameter4.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(parameter4);
            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
                // Calculate the CustomerID using Output Param from SPROC
                int id = Int32.Parse(parameter4.Value.ToString());

                return id;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                return 0;
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }
        }

        /// <summary>
        ///  Enalbe the account user to Pull Message from his incoming Messages
        /// </summary>		
        /// <returns></returns>		 
        public MessageDetails GetMessage()
        {
            if (Accountid == 0)
                return null;
            MessageDetails Msg = new MessageDetails();
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_GetMessage", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameter2 = new SqlParameter("@AU_ID", SqlDbType.Int, 4);
            parameter2.Value = this.Accountid;
            sqlCommand.Parameters.Add(parameter2);
            try
            {
                gatewayConnection.Open();
                SqlDataReader result = sqlCommand.ExecuteReader();//CommandBehavior.CloseConnection);
                while (result.Read())
                {
                    Msg.MessageStatus = result["AUI_Status"].ToString();
                    Msg.MessageTarget = result["AUI_Target"].ToString();
                    Msg.MessageBody = result["Message_Body"].ToString();
                    Msg.MessageDate = DateTime.Parse(result["AUI_Recvd_Date"].ToString());
                    Msg.MessageOption = Int32.Parse(result["Message_Option"].ToString());
                }
                result.Close();
                //Msg.Messageid=;		
                gatewayConnection.Close();
                return Msg;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                return null;
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public int SendMessage(MessageDetails Msg)
        {
            int Messageid = -1;
            SqlConnection sqlConnection = null;
            try
            {
                if (Accountid == 0)
                    return -1;
                //add the Message   
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
                SqlCommand sqlCommand = new SqlCommand("PR_AddMessage", sqlConnection);
                // Mark the Command as a SPROC
                sqlCommand.CommandType = CommandType.StoredProcedure;
                // Add Parameters to SPROC

                SqlParameter parameter1 = new SqlParameter("@Message_Body", SqlDbType.VarChar, 650);
                parameter1.Value = Msg.MessageBody;
                sqlCommand.Parameters.Add(parameter1);

                SqlParameter parameter2 = new SqlParameter("@AU_ID", SqlDbType.Int, 4);
                parameter2.Value = this.Accountid;
                sqlCommand.Parameters.Add(parameter2);

                SqlParameter parameter3 = new SqlParameter("@AUO_Target", SqlDbType.NVarChar, 50);
                parameter3.Value = Msg.MessageTarget;
                sqlCommand.Parameters.Add(parameter3);

                SqlParameter parameter4 = new SqlParameter("@MessageId", SqlDbType.Int, 4);
                parameter4.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(parameter4);

                SqlParameter parameter5 = new SqlParameter("@Message_Creation_Date", SqlDbType.DateTime, 8);
                parameter5.Value = DateTime.Now;
                sqlCommand.Parameters.Add(parameter5);

                SqlParameter parameter6 = new SqlParameter("@AUO_Creation_DateTime", SqlDbType.DateTime, 8);
                parameter6.Value = DateTime.Now;
                sqlCommand.Parameters.Add(parameter6);

                //SqlParameter parameter7 = new SqlParameter("@AUO_Retrial_Hours", SqlDbType.Int ,4);
                //parameter7.Value = 5;
                //sqlCommand.Parameters.Add(parameter7);

                SqlParameter parameter7 = new SqlParameter("@AUO_Status", SqlDbType.Int, 4);
                parameter7.Value = 0;
                sqlCommand.Parameters.Add(parameter7);

                SqlParameter parameter8 = new SqlParameter("@AUO_Number", SqlDbType.Int, 4);
                parameter8.Value = 5555;
                sqlCommand.Parameters.Add(parameter8);

                SqlParameter parameter9 = new SqlParameter("@option", SqlDbType.Int, 4);
                parameter9.Value = Msg.MessageOption;
                sqlCommand.Parameters.Add(parameter9);

                SqlParameter parameter10 = new SqlParameter("@AUO_Creation_Date", SqlDbType.DateTime, 8);
                parameter10.Value = DateTime.Now;
                sqlCommand.Parameters.Add(parameter10);

                SqlParameter parameter11 = new SqlParameter("@AUO_Alias", SqlDbType.NVarChar, 50);
                parameter11.Value = Msg.Alias;
                sqlCommand.Parameters.Add(parameter11);

                SqlParameter parameter12 = new SqlParameter("@AUO_Reference_ID", SqlDbType.NVarChar, 100);
                parameter12.Value = Msg.ReferenceID;
                sqlCommand.Parameters.Add(parameter12);


                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                // Calculate the CustomerID using Output Param from SPROC
                Messageid = (int)parameter4.Value;

                sqlCommand = new SqlCommand("PR_IncreaseMessagesAccount", sqlConnection);
                // Mark the Command as a SPROC
                sqlCommand.CommandType = CommandType.StoredProcedure;
                // Add Parameters to SPROC

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                System.Diagnostics.EventLog.WriteEntry("Send Message Error:", exp.Message, System.Diagnostics.EventLogEntryType.Error);
                ErrorLogger.LogError(exp.StackTrace, exp, false);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
            return Messageid;

        }

        /// <summary>
        ///  Get the Staus of the MEssage sned by the cuurent users
        /// </summary>
        /// <param name="ar">Messageid return when the user send a Message</param>
        /// <returns></returns>		 
        public int GetMessageStatus(int Messageid)
        {
            if (Accountid == 0)
                return -4;

            //MessageDetails Msg=new  MessageDetails();
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_GetMessageStatus", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameter1 = new SqlParameter("@AUO_ID", SqlDbType.Int, 4);
            parameter1.Value = Messageid;
            sqlCommand.Parameters.Add(parameter1);
            SqlParameter parameter2 = new SqlParameter("@AU_ID", SqlDbType.Int, 4);
            parameter2.Value = this.Accountid;
            sqlCommand.Parameters.Add(parameter2);
            SqlParameter parameter3 = new SqlParameter("@status", SqlDbType.Int, 4);
            parameter3.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(parameter3);
            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();//ExecuteReader(CommandBehavior.CloseConnection);
                gatewayConnection.Close();

                int s = (int)(parameter3.Value);
                return s;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                return 0;
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }

        }

        /// <summary>
        ///  Send SMS Message
        /// </summary>
        /// <param name="ar">Messageid return to used it get the Message Staus</param>
        /// <returns></returns>		 
        public static int SendMessage(int intAccountId, MessageDetails Msg)
        {
            if (intAccountId == 0 || intAccountId == -1)
                return -1;
            int Messageid = -1;
            //add the Message   
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_AddMessage", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC

            SqlParameter parameter1 = new SqlParameter("@Message_Body", SqlDbType.VarChar, 650);
            parameter1.Value = Msg.MessageBody;
            sqlCommand.Parameters.Add(parameter1);

            SqlParameter parameter2 = new SqlParameter("@AU_ID", SqlDbType.Int, 4);
            parameter2.Value = intAccountId;
            sqlCommand.Parameters.Add(parameter2);

            SqlParameter parameter3 = new SqlParameter("@AUO_Target", SqlDbType.NVarChar, 50);
            parameter3.Value = Msg.MessageTarget;
            sqlCommand.Parameters.Add(parameter3);

            SqlParameter parameter4 = new SqlParameter("@MessageId", SqlDbType.Int, 4);
            parameter4.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(parameter4);

            SqlParameter parameter5 = new SqlParameter("@Message_Creation_Date", SqlDbType.DateTime, 8);
            parameter5.Value = DateTime.Now;
            sqlCommand.Parameters.Add(parameter5);

            SqlParameter parameter6 = new SqlParameter("@AUO_Creation_DateTime", SqlDbType.DateTime, 8);
            parameter6.Value = DateTime.Now;
            sqlCommand.Parameters.Add(parameter6);

            //SqlParameter parameter7 = new SqlParameter("@AUO_Retrial_Hours", SqlDbType.Int ,4);
            //parameter7.Value = 5;
            //sqlCommand.Parameters.Add(parameter7);

            SqlParameter parameter7 = new SqlParameter("@AUO_Status", SqlDbType.Int, 4);
            parameter7.Value = 1;
            sqlCommand.Parameters.Add(parameter7);

            SqlParameter parameter8 = new SqlParameter("@AUO_Number", SqlDbType.Int, 4);
            parameter8.Value = 5555;
            sqlCommand.Parameters.Add(parameter8);

            SqlParameter parameter9 = new SqlParameter("@option", SqlDbType.Int, 4);
            parameter9.Value = Msg.MessageOption;
            sqlCommand.Parameters.Add(parameter9);

            SqlParameter parameter10 = new SqlParameter("@AUO_Creation_Date", SqlDbType.DateTime, 8);
            parameter10.Value = DateTime.Now;
            sqlCommand.Parameters.Add(parameter10);

            SqlParameter parameter11 = new SqlParameter("@AUO_Alias", SqlDbType.NVarChar, 50);
            parameter11.Value = Msg.Alias;
            sqlCommand.Parameters.Add(parameter11);

            SqlParameter parameter12 = new SqlParameter("@AUO_Reference_ID", SqlDbType.NVarChar, 100);;
            parameter12.Value = Msg.ReferenceID;
            sqlCommand.Parameters.Add(parameter12);

            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
                // Calculate the CustomerID using Output Param from SPROC
                Messageid = (int)parameter4.Value;

                sqlCommand = new SqlCommand("PR_IncreaseMessagesAccount", gatewayConnection);
                // Mark the Command as a SPROC
                sqlCommand.CommandType = CommandType.StoredProcedure;
                // Add Parameters to SPROC

                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();

                return Messageid;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                return 0;
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="intAccountId"></param>
        /// <returns></returns>
        public static int PullMessage(MessageDetails Msg, int intAccountId)
        {
            //if(a.Accountid==0)
            //	return -1;
            int Messageid = -1;
            //add the Message   
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_PullMessage", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameter1 = new SqlParameter("@Message_Body", SqlDbType.VarChar, 650);
            parameter1.Value = Msg.MessageBody;
            sqlCommand.Parameters.Add(parameter1);

            SqlParameter parameter2 = new SqlParameter("@AU_ID", SqlDbType.Int, 4);
            parameter2.Value = intAccountId;
            sqlCommand.Parameters.Add(parameter2);

            SqlParameter parameter12 = new SqlParameter("@Message_option", SqlDbType.Int, 4);
            parameter12.Value = Msg.MessageOption;
            sqlCommand.Parameters.Add(parameter12);

            SqlParameter parameter3 = new SqlParameter("@AUI_Originator", SqlDbType.NVarChar, 50);
            parameter3.Value = Msg.MessageOriginator;
            sqlCommand.Parameters.Add(parameter3);

            SqlParameter parameter4 = new SqlParameter("@AUI_Target", SqlDbType.NVarChar, 50);
            parameter4.Value = Msg.MessageTarget;
            sqlCommand.Parameters.Add(parameter4);

            SqlParameter parameter5 = new SqlParameter("@MessageId", SqlDbType.Int, 4);
            parameter5.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(parameter5);

            SqlParameter parameter6 = new SqlParameter("@MessageDate", SqlDbType.DateTime);
            parameter6.Value = Msg.MessageDate;
            sqlCommand.Parameters.Add(parameter6);


            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
                // Calculate the CustomerID using Output Param from SPROC
                Messageid = (int)parameter5.Value;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.StackTrace, exp, false);
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }
            return Messageid;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OutGoing_Event_TickID"></param>
        /// <param name="OutGoing_Event_Status"></param>
        /// <param name="Error_Code"></param>
        /// <param name="AUO_ID"></param>
        public static void AddMessageNotification(string OutGoing_Event_TickID, int OutGoing_Event_Status, int Error_Code, int AUO_ID)
        {

            //add the Message   
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_AddMessageNotification", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC

            SqlParameter parameter1 = new SqlParameter("@OE_TICKETID", SqlDbType.VarChar, 50);
            parameter1.Value = OutGoing_Event_TickID;
            sqlCommand.Parameters.Add(parameter1);

            SqlParameter parameter2 = new SqlParameter("@AUO_ID", SqlDbType.Int, 4);
            parameter2.Value = AUO_ID;
            sqlCommand.Parameters.Add(parameter2);

            SqlParameter parameter3 = new SqlParameter("@OE_Creation_Date", SqlDbType.DateTime, 8);
            parameter3.Value = DateTime.Now;
            sqlCommand.Parameters.Add(parameter3);

            SqlParameter parameter4 = new SqlParameter("@OE_Status", SqlDbType.Int, 4);
            parameter4.Value = OutGoing_Event_Status;
            sqlCommand.Parameters.Add(parameter4);

            SqlParameter parameter5 = new SqlParameter("@AUO_ERROR_CODE", SqlDbType.Int, 4);
            parameter5.Value = Error_Code;
            sqlCommand.Parameters.Add(parameter5);
            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OutGoing_Event_TickID"></param>
        /// <param name="OutGoing_Event_Status"></param>
        public static void UpdateMessageNotification(string OutGoing_Event_TickID, int OutGoing_Event_Status)
        {
            //add the Message   
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_UpdateMessageNotification", gatewayConnection);
            // Mark the Command as a SPROC
            sqlCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC

            SqlParameter parameter1 = new SqlParameter("@OE_TICKETID", SqlDbType.VarChar, 650);
            parameter1.Value = OutGoing_Event_TickID;
            sqlCommand.Parameters.Add(parameter1);

            SqlParameter parameter12 = new SqlParameter("@OE_Status", SqlDbType.Int, 4);
            parameter12.Value = OutGoing_Event_Status;
            sqlCommand.Parameters.Add(parameter12);
            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAccountName"></param>
        /// <param name="intDirection"></param>
        /// <returns></returns>
        public static int getMessageCount(string strAccountName, int intDirection)
        {
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("PR_GetMessageCount", gatewayConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameter1 = new SqlParameter("@Account_Name", SqlDbType.NVarChar);
            parameter1.Direction = ParameterDirection.Input;
            parameter1.Value = strAccountName;
            sqlCommand.Parameters.Add(parameter1);

            parameter1 = new SqlParameter("@DIRECTION_TYPE", SqlDbType.TinyInt);
            parameter1.Direction = ParameterDirection.Input;
            parameter1.Value = intDirection;
            sqlCommand.Parameters.Add(parameter1);

            parameter1 = new SqlParameter("@Message_Count", SqlDbType.Int, 4);
            parameter1.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(parameter1);
            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
                int intCount = (int)parameter1.Value;
                return intCount;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                return 0;
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAccountName"></param>
        /// <param name="intDirection"></param>
        public static void decreaseMessageCount(string strAccountName, int intDirection)
        {
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);

            // Update MessageCount - Decrease it ..
            SqlCommand sqlCommand = new SqlCommand("PR_DecreaseMessagesAccount", gatewayConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameter1 = new SqlParameter("@Account_Name", SqlDbType.NVarChar);
            parameter1.Direction = ParameterDirection.Input;
            parameter1.Value = strAccountName;
            sqlCommand.Parameters.Add(parameter1);

            parameter1 = new SqlParameter("@DIRECTION_TYPE", SqlDbType.TinyInt);
            parameter1.Direction = ParameterDirection.Input;
            parameter1.Value = intDirection;
            sqlCommand.Parameters.Add(parameter1);
            try
            {
                gatewayConnection.Open();
                sqlCommand.ExecuteNonQuery();
                gatewayConnection.Close();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAccountName"></param>
        /// <param name="strMobileNumber"></param>
        /// <returns></returns>
        public static int getAccountIdByOrigination(string strAccountName, string strMobileNumber)
        {
            SqlConnection gatewayConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SMSConnectionString"].ConnectionString);

            // Update MessageCount - Decrease it ..
            SqlCommand sqlCommand = new SqlCommand("PR_GETACCOUNTIDBYORIGINATION", gatewayConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterID = new SqlParameter("@ACCOUNTNAME", System.Data.SqlDbType.VarChar);
            parameterID.Value = strAccountName;
            SqlParameter prm = sqlCommand.Parameters.Add(parameterID);
            prm.Direction = ParameterDirection.Input;

            parameterID = new SqlParameter("@MobileNumber", System.Data.SqlDbType.VarChar);
            parameterID.Value = strMobileNumber;
            prm = sqlCommand.Parameters.Add(parameterID);
            prm.Direction = ParameterDirection.Input;

            try
            {
                gatewayConnection.Open();
                object objAccountId = sqlCommand.ExecuteScalar();
                gatewayConnection.Close();
                int intAccountId = -1;
                if (objAccountId != null)
                    intAccountId = (int)objAccountId;
                return intAccountId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                return 0;
            }
            finally
            {
                if (gatewayConnection.State == ConnectionState.Open)
                    gatewayConnection.Close();
            }
        }
    }
}
