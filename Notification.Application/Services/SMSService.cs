using Microsoft.Extensions.Configuration;
using Notification.Application.Interfaces;
using Notification.Domain.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Services
{
    public class SMSService : ISMSService
    {
        private readonly IConfiguration _config;
        private readonly string _termiiApiKey;
        private readonly string _sendername;
        private readonly string _termiiBaseUrl;
        public SMSService(IConfiguration config)
        {
            _termiiApiKey = config["Termii:ApiKey"];
            _sendername = config["Termii:SenderName"];
            _termiiBaseUrl = config["Termii:BaseUrl"];

        }
        public async Task<bool> SendSms(SMSRequest smsRequest)
        {
            var client = new RestClient(_termiiBaseUrl);

            var request = new RestRequest("/api/sms/send")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(new
                {
                    to = smsRequest.ToPhoneNumber,
                    from = _sendername,
                    sms = smsRequest.Message,
                    type = "plain",
                    channel = "generic",
                    api_key = _termiiApiKey
                });

            var response = await client.PostAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception("SMS sending failed: " + response.Content);               
            }
            return true;
        }

    }
}
