using System.Threading.Tasks;
using CineMate.Data.Entities;

namespace CineMate.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
        Task SendReservationReceiptAsync(string to, Reservation reservation);
    }
}
