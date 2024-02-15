using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Notifications
{
    public class FireBaseNotificationService : INotificationServiceProvider
    {
        public NotificationServiceOutput RegisterNotificationToken(string userId, string token)
        {
            NotificationServiceOutput output = new NotificationServiceOutput();
            FirebaseNotificationLog log = new FirebaseNotificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.MethodName = "RegisterNotificationToken";
            log.UserId = userId;
            //log.ReferenceId = referenceId;
            //log.Channel = channel;
            log.ServiceRequest = JsonConvert.SerializeObject(new { userId, token });
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    output.ErrorCode = ErrorCode.NullUserId;
                    output.Message = "userId is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(token))
                {
                    output.ErrorCode = ErrorCode.EmptyToken;
                    output.Message = "Token is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
                using (var context = new FirebaseContext())
                {
                    var record = context.UserFireBaseRegisterationTokens.Where(r => r.UserId == userId).OrderByDescending(a=>a.Id).FirstOrDefault();
                    if (record != null)
                    {
                        record.RegisterationToken = token;
                        record.ModificationDate = DateTime.Now;
                    }
                    else
                    {
                        context.UserFireBaseRegisterationTokens.Add(new UserFireBaseRegisterationToken
                        {
                            UserId = userId,
                            RegisterationToken = token,
                            CreationDate = DateTime.Now,
                            ModificationDate=DateTime.Now
                        });
                    }
                    context.SaveChanges();
                }
                output.ErrorCode = ErrorCode.Success;
                output.Message = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.Message;
                FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ErrorCode.Exception;
                output.Message = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.Message;
                FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                return output;
            }
        }
        public async Task<NotificationServiceOutput> SendNotification(string userId, string title, string body, string imageUrl = null)
        {
            using (var context = new FirebaseContext())
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(title))
                    {
                        context.SendNotificationLogs.Add(new SendNotificationLog
                        {
                            ErrorCode = (int)ErrorCode.EmptyTitle,
                            ErrorDescription = "Title is empty.",
                            Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                            UserId = userId,
                            CreationDate = DateTime.Now                            
                        });
                        context.SaveChanges();

                        return new NotificationServiceOutput
                        {
                            ErrorCode = ErrorCode.EmptyTitle,
                            Message = "Title is empty."
                        };
                    }

                    if(title.Length > 200)
                    {
                        context.SendNotificationLogs.Add(new SendNotificationLog
                        {
                            ErrorCode = (int)ErrorCode.TitleExceedsLimit,
                            ErrorDescription = "Title exceeds the 200 characters limit.",
                            Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                            UserId = userId,
                            CreationDate = DateTime.Now
                        });
                        context.SaveChanges();

                        return new NotificationServiceOutput
                        {
                            ErrorCode = ErrorCode.TitleExceedsLimit,
                            Message = "Title exceeds the 200 characters limit."
                        };
                    }

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        context.SendNotificationLogs.Add(new SendNotificationLog
                        {
                            ErrorCode = (int)ErrorCode.EmptyBody,
                            ErrorDescription = "Body is empty.",
                            Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                            UserId = userId,
                            CreationDate = DateTime.Now
                        });
                        context.SaveChanges();

                        return new NotificationServiceOutput
                        {
                            ErrorCode = ErrorCode.EmptyBody,
                            Message = "Body is empty."
                        };
                    }

                    if (body.Length > 1024)
                    {
                        context.SendNotificationLogs.Add(new SendNotificationLog
                        {
                            ErrorCode = (int)ErrorCode.BodyExceedsLimit,
                            ErrorDescription = "Body exceeds the 1024 characters limit.",
                            Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                            UserId = userId,
                            CreationDate = DateTime.Now
                        });
                        context.SaveChanges();

                        return new NotificationServiceOutput
                        {
                            ErrorCode = ErrorCode.BodyExceedsLimit,
                            Message = "Body exceeds the 1024 characters limit."
                        };
                    }

                    string messageId;
                    try
                    {
                        var userToken = context.UserFireBaseRegisterationTokens.FirstOrDefault(r => r.UserId == userId);
                        if(userToken == null)
                        {
                            context.SendNotificationLogs.Add(new SendNotificationLog
                            {
                                ErrorCode = (int)ErrorCode.NoUserTokenFound,
                                ErrorDescription = "No user token found.",
                                Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                                UserId = userId,
                                CreationDate = DateTime.Now
                            });
                            context.SaveChanges();

                            return new NotificationServiceOutput
                            {
                                ErrorCode = ErrorCode.NoUserTokenFound,
                                Message = "No user token found."
                            };
                        }
                        if(FirebaseMessaging.DefaultInstance == null)
                        {
                            FirebaseApp.Create(new AppOptions()
                            {
                                Credential = GoogleCredential.FromFile(@"C:\inetpub\wwwroot\Settings\bcare-c8228-firebase-adminsdk-ig14d-f7223ba18f.json"),
                            });
                        }

                        messageId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
                        {
                            Token = userToken.RegisterationToken,

                            Notification = new Notification
                            {
                                Title = title,
                                Body = body,
                                ImageUrl = imageUrl
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        context.SendNotificationLogs.Add(new SendNotificationLog
                        {
                            ErrorCode = (int)ErrorCode.FireBaseError,
                            ErrorDescription = ex.ToString(),
                            Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                            UserId = userId,
                            CreationDate = DateTime.Now
                        });
                        context.SaveChanges();

                        return new NotificationServiceOutput
                        {
                            ErrorCode = ErrorCode.FireBaseError,
                            Message = "Failed to send notification."
                        };
                    }

                    if (messageId == null)
                    {
                        context.SendNotificationLogs.Add(new SendNotificationLog
                        {
                            ErrorCode = (int)ErrorCode.NullMessageId,
                            ErrorDescription = "Unexpected null messageId from Firebase",
                            Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                            UserId = userId,
                            CreationDate = DateTime.Now
                        });
                        context.SaveChanges();

                        return new NotificationServiceOutput
                        {
                            ErrorCode = ErrorCode.NullMessageId,
                            Message = "Unexpected null messageId from Firebase"
                        };
                    }


                    context.UserNotifications.Add(new UserNotification
                    {
                        MessageId = messageId,
                        Body = body,
                        Title = title,
                        ImageUrl = imageUrl,
                        CreationDate = DateTime.Now
                    });
                    context.SaveChanges();


                    return new NotificationServiceOutput
                    {
                        ErrorCode = ErrorCode.Success,
                        Message = "Success"
                    };


                }
                catch (Exception ex)
                {
                    context.SendNotificationLogs.Add(new SendNotificationLog
                    {
                        ErrorCode = (int)ErrorCode.Exception,
                        ErrorDescription = ex.ToString(),
                        Input = JsonConvert.SerializeObject(new { userId, title, body, imageUrl }),
                        UserId = userId,
                        CreationDate = DateTime.Now
                    });
                    context.SaveChanges();

                    return new NotificationServiceOutput
                    {
                        ErrorCode = ErrorCode.Exception,
                        Message = "An error occurred."
                    };
                }
            }
        }
        public NotificationServiceOutput SendFireBaseNotification(string userId, string title, string body,string method,string referenceId,string channel, string imageUrl = null)
        {
            NotificationServiceOutput output = new NotificationServiceOutput();
            FirebaseNotificationLog log = new FirebaseNotificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.MethodName = method;
            log.UserId = userId;
            log.ReferenceId = referenceId;
            log.Channel = channel;
            log.ServiceRequest = JsonConvert.SerializeObject(new { userId, title, body,method, imageUrl });
            try
            {
                if (method == "UploadNajm")
                {
                    var notification = FirebaseNotificationLogDataAccess.GetFromFirebaseNotificationByRefernceId(referenceId, method);//already sent before
                    if (notification != null)
                    {
                        output.ErrorCode = ErrorCode.Success;
                        output.Message = "Success";
                        return output;
                    }
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    output.ErrorCode = ErrorCode.EmptyTitle;
                    output.Message = "Title is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
                if (title.Length > 200)
                {
                    output.ErrorCode = ErrorCode.TitleExceedsLimit;
                    output.Message = "Title exceeds the 200 characters limit";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(body))
                {
                    output.ErrorCode = ErrorCode.EmptyBody;
                    output.Message = "Body is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
                if (body.Length > 1024)
                {
                    output.ErrorCode = ErrorCode.BodyExceedsLimit;
                    output.Message = "Body exceeds the 1024 characters limit";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
                using (var context = new FirebaseContext())
                {
                    var userToken = context.UserFireBaseRegisterationTokens.Where(r => r.UserId == userId).OrderByDescending(a=>a.Id).FirstOrDefault();
                    if (userToken == null)
                    {

                        output.ErrorCode = ErrorCode.NoUserTokenFound;
                        output.Message = "No user token found";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.Message;
                        FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                        return output;
                    }
                    DateTime dtBeforeCalling = DateTime.Now;
                    if (FirebaseMessaging.DefaultInstance == null)
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile(@"C:\inetpub\wwwroot\Settings\bcare-c8228-firebase-adminsdk-ig14d-f7223ba18f.json"),
                        });
                    }

                    var messageResult = FirebaseMessaging.DefaultInstance.SendAsync(new Message
                    {
                        Token = userToken.RegisterationToken,

                        Notification = new Notification
                        {
                            Title = title,
                            Body = body,
                            ImageUrl = imageUrl
                        }
                    });
                    //log.ServiceResponse = JsonConvert.SerializeObject(messageResult.Result);
                    messageResult.Wait();
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    if (messageResult == null)
                    {
                        output.ErrorCode = ErrorCode.NullMessageId;
                        output.Message = "result is null from Firebase";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.Message;
                        FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                        return output;
                    }
                    log.ServiceResponse = JsonConvert.SerializeObject(messageResult.Result);
                    context.UserNotifications.Add(new UserNotification
                    {
                        MessageId = messageResult.Result,
                        Body = body,
                        Title = title,
                        ImageUrl = imageUrl,
                        CreationDate = DateTime.Now,
                        ReferenceId=referenceId,
                        UserId=userId
                    });
                    context.SaveChanges();

                    output.ErrorCode = ErrorCode.Success;
                    output.Message = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.Message;
                    FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = ErrorCode.Exception;
                output.Message = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.Message;
                FirebaseNotificationLogDataAccess.AddToFirebaseNotificationLog(log);
                return output;
            }
        }
        
    }
}
