using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using Notification.Application.Options;
using Notification.Domain.Models;
using RestSharp;

namespace Notification.Application.Services
{
    public class SMSService : ISMSService
    {
        private readonly TermiiSettings _settings;
        private readonly RestClient _client;

        public SMSService(IOptions<TermiiSettings> options)
        {
            _settings = options.Value;
            _client = new RestClient(_settings.BaseUrl);
        }

        public async Task<bool> SendSmsAsync(SMSRequest smsRequest)
        {
            var request = new RestRequest("/api/sms/send")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(new
                {
                    to = smsRequest.ToPhoneNumber,
                    from = _settings.SenderName,
                    sms = smsRequest.Message,
                    type = "plain",
                    channel = "generic",
                    api_key = _settings.ApiKey
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
