using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;

namespace SSConstructions.Controllers.API
{
    [ApiController]
    [EnableCors("addCors")]
    [Route("api/[controller]")]
    public class EnquiryAPIController : Controller
    {
        private readonly IUsers _usersRepo;
        private readonly ILoginRepo _loginRepo;
        private readonly CustomLogger _logger;

        public EnquiryAPIController(IUsers usersRepo, ILoginRepo loginRepo, CustomLogger logger)
        {
            _usersRepo = usersRepo;
            _loginRepo = loginRepo;
            _logger = logger;
        }

        [Route("SubmitConsultation")]
        [HttpPost()]
        public async Task<IActionResult> SubmitConsultation([FromBody] ConsultationRequest request)
        {
            try
            {
                _logger.Log("EnquiryAPIController.SubmitConsultation Started.");

                if (request == null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountName = await _loginRepo.GetAccountName(request.AccountId);
                var admins = await _usersRepo.GetAdminUsers(request.AccountId);

                if (admins == null || admins.Count == 0)
                {
                    return Ok(new { Success = false, Message = "No admin recipient is configured for this account." });
                }

                var recipients = string.Join(",", admins
                    .Select(a => a.Email)
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct(StringComparer.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(recipients))
                {
                    return Ok(new { Success = false, Message = "No admin email address is configured." });
                }

                string subject = $"New Consultation Request — {accountName}";
                string body = BuildEmailHtml(accountName, request);

                await Email.SendEmail(recipients, subject, body, accountName);

                _logger.Log("EnquiryAPIController.SubmitConsultation Completed.");
                return Ok(new { Success = true, Message = "Your consultation request has been submitted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Ok(new { Success = false, Message = "Unable to submit the consultation request. Please try again." });
            }
        }

        private string BuildEmailHtml(string accountName, ConsultationRequest r)
        {
            var package = string.IsNullOrWhiteSpace(r.Package) ? "Not specified" : r.Package;
            var plotArea = string.IsNullOrWhiteSpace(r.PlotArea) ? "Not specified" : r.PlotArea;
            var message = string.IsNullOrWhiteSpace(r.Message) ? "—" : r.Message;
            string submittedAt = DateTime.Now.ToString("dd MMM yyyy, hh:mm tt");

            return $@"
            <!DOCTYPE html>
            <html>
            <body style='font-family: Arial, Helvetica, sans-serif; background-color: #f4f1ea; margin:0; padding:0;'>
              <table align='center' cellpadding='0' cellspacing='0' style='max-width:640px; width:100%; background:#ffffff; border-radius:14px; overflow:hidden; margin:24px auto; box-shadow:0 10px 30px rgba(0,0,0,0.12); border:1px solid #e6dfcf;'>
                <tr>
                  <td style='background:linear-gradient(135deg,#24382c,#101712); padding:26px 30px; color:#f6f2ea;'>
                    <h1 style='margin:0; font-size:22px; letter-spacing:1px;'>{accountName}</h1>
                    <p style='margin:6px 0 0; font-size:13px; color:#e6d4ac; text-transform:uppercase; letter-spacing:2px;'>New Consultation Request</p>
                  </td>
                </tr>
                <tr>
                  <td style='padding:28px 30px;'>
                    <table cellpadding='0' cellspacing='0' style='width:100%;'>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Customer Name</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:17px; color:#24382c; font-weight:600;'>{r.Name}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Phone Number</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{r.Phone}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Email</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{r.Email}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Plot Location</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{r.PlotLocation}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Plot Area</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{plotArea}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Interested Package</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{package}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Message</td></tr>
                      <tr><td style='padding:0 0 6px; font-size:16px; color:#333;'>{message}</td></tr>
                    </table>
                    <div style='margin-top:22px; border-top:1px solid #efe9dc; padding-top:14px; font-size:12px; color:#8a8577;'>
                      Submitted on {submittedAt}
                    </div>
                  </td>
                </tr>
              </table>
            </body>
            </html>";
        }
    }
}