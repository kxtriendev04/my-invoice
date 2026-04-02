using Microsoft.AspNetCore.SignalR;
using MISA.WorkShift.Api.Hubs;
using MISA.WorkShift.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace MISA.WorkShift.Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(string method, object data)
        {
            await _hubContext.Clients.All.SendAsync(method, data);
        }
    }
}