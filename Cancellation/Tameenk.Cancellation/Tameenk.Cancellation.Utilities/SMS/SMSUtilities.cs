using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Cancellation.Utilities
{
    public class SMSUtilities
    {

        public static string SMSTitle
        {
            get { return Utilities.GetAppSetting(Strings.SMSTitle); }
        }
        
        /// <summary>
        /// save SMS to temp DB
        /// </summary>
        /// <param name="MobileNumber"></param>
        /// <param name="msgBody"></param>
        /// <returns></returns>
        public static bool SendSMSMsg(string MobileNumber, string msgBody, string language, string alias)
        {
            bool flage = false;
            try
            {
                string international = "00";
                string temp = MobileNumber;
                if (MobileNumber.StartsWith(international))
                    MobileNumber = temp;
                else if (MobileNumber.StartsWith(0.ToString()))
                    MobileNumber = 2.ToString() + MobileNumber;
                else if (!MobileNumber.StartsWith(2.ToString()))
                    MobileNumber = 20.ToString() + MobileNumber;

                //if (alias.StartsWith(0.ToString()))
                //    alias = 2.ToString() + alias;
                //else if (!MobileNumber.StartsWith(2.ToString()))
                //    alias = 20.ToString() + alias;

                if (language.ToLower().Trim().Equals(Strings.Arabic.ToLower()))
                {
                    msgBody += "\n من Orange.eg";
                }
                else
                {
                    msgBody += "\n Sent from Orange.eg";
                }
                int msgOption;
                string tmp = msgBody;
                if (language.ToLower().Trim().Equals(Strings.Arabic.ToLower()))//if (msgBody.Length <= 70)//arabic 
                {
                    if (tmp.Length > 70)
                    {
                        int MesssageLength = 67;
                        int NumberOfMessages = msgBody.Length / MesssageLength + 1;
                        msgOption = 192;

                        string Message;
                        for (int i = 1; i <= NumberOfMessages; i++)
                        {
                            //"050003120";
                            //"060804000E0";
                            //string Header = "060804EB12";
                            string Header = "05000312";
                            //Header += string.Format("{0:x2}", 123); // Multi-part Message-ID
                            Header += string.Format("{0:x2}", (byte)NumberOfMessages); // Number of Part
                            Header += string.Format("{0:x2}", (byte)i); // Current Part
                            if (i < NumberOfMessages)
                            {
                                Message = msgBody.Substring((i - 1) * MesssageLength, MesssageLength);
                            }
                            else
                            {
                                Message = msgBody.Substring((i - 1) * MesssageLength);
                            }


                            tmp = string.Empty;
                            for (int _iLoop = 0; _iLoop < Message.Length; _iLoop++)
                            {
                                tmp += string.Format(Strings.FormateBody, (int)Message[_iLoop]);
                            }

                            Message = Header + tmp;

                            SmsMessage SendMessage = new SmsMessage();
                            MessageDetails msgDetails = new MessageDetails();
                            string msgUsername = Utilities.GetAppSetting(Strings.SMSUsername);
                            string msgPassword = Utilities.GetAppSetting(Strings.SMSpassword);

                            SendMessage.Initialize(msgUsername, msgPassword);

                            msgDetails.MessageDate = DateTime.Now;
                            msgDetails.MessageTarget = MobileNumber;
                            msgDetails.MessageOption = Convert.ToInt32(msgOption);
                            msgDetails.MessageBody = Message;
                            if (alias != string.Empty)
                            {
                                msgDetails.Alias = alias;
                            }
                            else
                            {
                                msgDetails.Alias = Utilities.GetAppSetting(Strings.SMSTitle);
                            }

                            int Messageid = SendMessage.SendMessage(msgDetails);
                            if (Messageid > 0)
                            {
                                flage = true;
                            }
                        }
                    }
                    else
                    {
                        msgOption = 64;
                        tmp = string.Empty;
                        for (int _iLoop = 0; _iLoop < msgBody.Length; _iLoop++)
                        {
                            tmp += string.Format(Strings.FormateBody, (int)msgBody[_iLoop]);
                        }
                        SmsMessage SendMessage = new SmsMessage();
                        MessageDetails msgDetails = new MessageDetails();
                        string msgUsername = Utilities.GetAppSetting(Strings.SMSUsername);
                        string msgPassword = Utilities.GetAppSetting(Strings.SMSpassword);

                        SendMessage.Initialize(msgUsername, msgPassword);

                        msgDetails.MessageDate = DateTime.Now;
                        msgDetails.MessageTarget = MobileNumber;
                        msgDetails.MessageOption = Convert.ToInt32(msgOption);
                        msgDetails.MessageBody = tmp;
                        if (alias != string.Empty)
                        {
                            msgDetails.Alias = alias;
                        }
                        else
                        {
                            msgDetails.Alias = Utilities.GetAppSetting(Strings.SMSTitle);
                        }

                        int Messageid = SendMessage.SendMessage(msgDetails);
                        if (Messageid > 0)
                        {
                            flage = true;
                        }
                    }
                }
                else
                {
                    if (tmp.Length > 160)
                    {
                        int MesssageLength = 153;
                        int NumberOfMessages = msgBody.Length / MesssageLength + 1;
                        msgOption = 128;
                        string Message;
                        for (int i = 1; i <= NumberOfMessages; i++)
                        {
                            //"050003120";
                            //"060804000E0";
                            string Header = "05000312";
                            //Header += string.Format("{0:x2}", 123); // Multi-part Message-ID
                            Header += string.Format("{0:x2}", (byte)NumberOfMessages); // Number of Part
                            Header += string.Format("{0:x2}", (byte)i); // Current Part
                            if (i < NumberOfMessages)
                            {
                                Message = msgBody.Substring((i - 1) * MesssageLength, MesssageLength);
                            }
                            else
                            {
                                Message = msgBody.Substring((i - 1) * MesssageLength);
                            }

                            tmp = string.Empty;
                            for (int j = 0; j < Message.Length; j++)
                            {
                                tmp += string.Format("{0:X2}", (char)Message[j]);
                            }

                            Message = Header + tmp;

                            SmsMessage SendMessage = new SmsMessage();
                            MessageDetails msgDetails = new MessageDetails();
                            string msgUsername = Utilities.GetAppSetting(Strings.SMSUsername);
                            string msgPassword = Utilities.GetAppSetting(Strings.SMSpassword);

                            SendMessage.Initialize(msgUsername, msgPassword);

                            msgDetails.MessageDate = DateTime.Now;
                            msgDetails.MessageTarget = MobileNumber;
                            msgDetails.MessageOption = Convert.ToInt32(msgOption);
                            msgDetails.MessageBody = Message;
                            if (alias != string.Empty)
                            {
                                msgDetails.Alias = alias;
                            }
                            else
                            {
                                msgDetails.Alias = Utilities.GetAppSetting(Strings.SMSTitle);
                            }

                            int Messageid = SendMessage.SendMessage(msgDetails);
                            if (Messageid > 0)
                            {
                                flage = true;
                            }
                        }
                    }
                    else
                    {
                        msgOption = 0;
                        SmsMessage SendMessage = new SmsMessage();
                        MessageDetails msgDetails = new MessageDetails();
                        string msgUsername = Utilities.GetAppSetting(Strings.SMSUsername);
                        string msgPassword = Utilities.GetAppSetting(Strings.SMSpassword);

                        SendMessage.Initialize(msgUsername, msgPassword);

                        msgDetails.MessageDate = DateTime.Now;
                        msgDetails.MessageTarget = MobileNumber;
                        msgDetails.MessageOption = Convert.ToInt32(msgOption);
                        msgDetails.MessageBody = tmp;
                        if (alias != string.Empty)
                        {
                            msgDetails.Alias = alias;
                        }
                        else
                        {
                            msgDetails.Alias = Utilities.GetAppSetting(Strings.SMSTitle);
                        }

                        int Messageid = SendMessage.SendMessage(msgDetails);
                        if (Messageid > 0)
                        {
                            flage = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex, false);
                flage = false;
            }

            return flage;
        }

    

    }
}
