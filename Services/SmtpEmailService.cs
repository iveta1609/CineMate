using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CineMate.Data.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using QRCoder;

namespace CineMate.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _opt;
        public SmtpEmailService(IOptions<SmtpOptions> opt) => _opt = opt.Value;

        // Прост HTML имейл
        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var builder = new BodyBuilder { HtmlBody = htmlBody };
            await SendWithBodyBuilderAsync(to, subject, builder);
        }

        // Имейл с QR е-билет (inline image чрез MimeKit LinkedResources)
        public async Task SendReservationReceiptAsync(string to, Reservation r)
        {
            var movie = r.Screening?.Movie?.Title ?? "Movie";
            var cinema = r.Screening?.Cinema?.Name ?? "Cinema";
            var start = r.Screening?.StartTime.ToString("dd.MM.yyyy • HH:mm") ?? "";
            var total = r.TotalPrice.ToString("0.00");
            var refNo = string.IsNullOrWhiteSpace(r.PaymentRef) ? $"RES-{r.Id}" : r.PaymentRef;

            // Списък с места, ако ги имаме
            var seats = (r.ReservationSeats != null && r.ReservationSeats.Any())
                ? string.Join(", ", r.ReservationSeats
                    .Where(x => x.Seat != null)
                    .OrderBy(x => x.Seat.Row).ThenBy(x => x.Seat.Number)
                    .Select(x => $"R{x.Seat.Row}-N{x.Seat.Number}"))
                : "(not specified)";

            // QR payload (mock)
            var payload = $"CINEMATE|RES:{r.Id}|REF:{refNo}|START:{r.Screening?.StartTime:O}|MOVIE:{movie}";

            // Генерираме PNG на QR в паметта
            byte[] qrPng;
            using (var gen = new QRCodeGenerator())
            using (var data = gen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
            using (var png = new PngByteQRCode(data))
            {
                qrPng = png.GetGraphic(10);
            }

            // HTML (вграден QR чрез CID)
            var html = $@"
<style>
  .card{{border:1px solid #eee;border-radius:12px;max-width:560px;padding:16px;font-family:system-ui,Segoe UI,Roboto,Arial}}
  .muted{{color:#6b7280}}
  .kv{{margin:6px 0}}
  .qr{{display:inline-block;border:1px solid #e5e7eb;border-radius:8px;padding:6px;background:#fff}}
</style>
<div class='card'>
  <h2 style='margin:0 0 8px 0'>Payment confirmation</h2>
  <div class='muted'>Reference: <b>{refNo}</b></div>
  <hr/>
  <div class='kv'><b>Movie:</b> {movie}</div>
  <div class='kv'><b>Cinema:</b> {cinema}</div>
  <div class='kv'><b>Start:</b> {start}</div>
  <div class='kv'><b>Seats:</b> {seats}</div>
  <div class='kv'><b>Total:</b> {total} BGN</div>
  <hr/>
  <div class='kv'><b>Your QR e-ticket:</b></div>
  <div class='qr'>
    <img src=""cid:qrImage"" width=""160"" height=""160"" alt=""QR ticket"" />
  </div>
  <div class='muted' style='margin-top:12px'>Show this QR at the cinema entrance.</div>
</div>";

            var builder = new BodyBuilder { HtmlBody = html };

            // Вграден ресурс (CID) за QR
            var qr = builder.LinkedResources.Add("qr.png", qrPng);
            qr.ContentId = "qrImage";
            qr.ContentType.MediaType = "image";
            qr.ContentType.MediaSubtype = "png";
            qr.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);

            await SendWithBodyBuilderAsync(to, $"Your CineMate tickets – {movie}", builder);
        }

        // Изпращане чрез MailKit + MimeKit
        private async Task SendWithBodyBuilderAsync(string to, string subject, BodyBuilder builder)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_opt.FromName, _opt.From));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = builder.ToMessageBody();

            // Явно посочваме MailKit клиента, за да няма двусмислие със System.Net.Mail
            using var client = new MailKit.Net.Smtp.SmtpClient();
            var socket = _opt.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_opt.Host, _opt.Port, socket);
            if (!string.IsNullOrWhiteSpace(_opt.User))
                await client.AuthenticateAsync(_opt.User, _opt.Pass);

            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }
    }
}
