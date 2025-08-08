using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using Notification.Application.Options;
using Notification.Domain.Models;
using RestSharp;

namespace Notification.Application.Services
{
    public class SMSService : ISMSService
    {
        private readonly AfricanstalkingSettings _settings;
        private readonly RestClient _client;

        public SMSService(IOptions<AfricanstalkingSettings> options)
        {
            _settings = options.Value;
            _client = new RestClient(_settings.BaseUrl);
        }

        public async Task<bool> SendSmsAsync(SMSRequest smsRequest)
        {
            string[] phoneNumbers = { smsRequest.ToPhoneNumber };
            
            var request = new RestRequest("/version1/messaging/bulk")
                .AddHeader("Content-Type", "application/json")
                .AddHeader("apiKey", _settings.ApiKey)
                .AddJsonBody(new
                {
                    phoneNumbers,
                    userName = _settings.SenderName,
                    message = smsRequest.Message,
                    
                });

            var response = await _client.PostAsync(request);

            if (!response.IsSuccessful)
            {
                // You can log here or create a custom exception
                throw new ApplicationException($"Failed to send SMS: {response.StatusCode} - {response.Content}");
            }

            return true;
        }
    }
}
