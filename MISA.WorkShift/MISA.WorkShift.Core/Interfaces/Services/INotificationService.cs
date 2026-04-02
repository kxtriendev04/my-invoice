using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendNotification(string method, object data);
    }
}