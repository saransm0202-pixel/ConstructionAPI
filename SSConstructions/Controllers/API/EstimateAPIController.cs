using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;
using System.Globalization;
using System.IO;

namespace SSConstructions.Controllers.API
{
    [ApiController]
    [EnableCors("addCors")]
    [Route("api/[controller]")]
    public class EstimateAPIController : Controller
    {
        private readonly IUsers _usersRepo;
        private readonly ILoginRepo _loginRepo;
        private readonly CustomLogger _logger;

        public EstimateAPIController(IUsers usersRepo, ILoginRepo loginRepo, CustomLogger logger)
        {
            _usersRepo = usersRepo;
            _loginRepo = loginRepo;
            _logger = logger;
        }

        [Route("SendEstimateReport")]
        [HttpPost()]
        public async Task<IActionResult> SendEstimateReport([FromForm] EstimateReportRequest request, IFormFile? reportPdf)
        {
            try
            {
                _logger.Log("EstimateAPIController.SendEstimateReport Started.");

                if (request == null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (reportPdf == null || reportPdf.Length == 0)
                {
                    return Ok(new { Success = false, Message = "The report PDF is missing." });
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

                string subject = $"New Construction Estimate — {accountName}";
                string body = BuildEmailHtml(accountName, request);

                byte[] pdfBytes;
                using (var ms = new MemoryStream())
                {
                    await reportPdf.CopyToAsync(ms);
                    pdfBytes = ms.ToArray();
                }

                var attachmentName = string.IsNullOrWhiteSpace(request.FileName)
                    ? $"GaneshBuilders-Estimate-{DateTime.Now:yyyyMMdd}.pdf"
                    : request.FileName;

                await Email.SendEmail(recipients, subject, body, accountName, pdfBytes, attachmentName);

                _logger.Log("EstimateAPIController.SendEstimateReport Completed.");
                return Ok(new { Success = true, Message = "Your estimate report has been emailed to our team." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Ok(new { Success = false, Message = "Unable to email the estimate report. Please try again." });
            }
        }

        private string BuildEmailHtml(string accountName, EstimateReportRequest r)
        {
            var customerName = string.IsNullOrWhiteSpace(r.CustomerName) ? "Web visitor" : r.CustomerName;
            var customerEmail = string.IsNullOrWhiteSpace(r.CustomerEmail) ? "—" : r.CustomerEmail;
            var customerPhone = string.IsNullOrWhiteSpace(r.CustomerPhone) ? "—" : r.CustomerPhone;
            var packageName = string.IsNullOrWhiteSpace(r.PackageName) ? "Not specified" : r.PackageName;
            string submittedAt = DateTime.Now.ToString("dd MMM yyyy, hh:mm tt");

            string baseCost = r.BaseCost.ToString("N0", CultureInfo.InvariantCulture);
            string extrasCost = r.ExtrasCost.ToString("N0", CultureInfo.InvariantCulture);
            string totalCost = r.TotalCost.ToString("N0", CultureInfo.InvariantCulture);
            string emi = r.EmiMonthly.ToString("N0", CultureInfo.InvariantCulture);
            string rate = r.RatePerSqft.ToString("N0", CultureInfo.InvariantCulture);

            return $@"
            <!DOCTYPE html>
            <html>
            <body style='font-family: Arial, Helvetica, sans-serif; background-color: #f4f1ea; margin:0; padding:0;'>
              <table align='center' cellpadding='0' cellspacing='0' style='max-width:640px; width:100%; background:#ffffff; border-radius:14px; overflow:hidden; margin:24px auto; box-shadow:0 10px 30px rgba(0,0,0,0.12); border:1px solid #e6dfcf;'>
                <tr>
                  <td style='background:linear-gradient(135deg,#ff7d00,#57120a); padding:26px 30px; color:#fff;'>
                    <h1 style='margin:0; font-size:22px; letter-spacing:1px;'>{accountName}</h1>
                    <p style='margin:6px 0 0; font-size:13px; color:#ffe6c9; text-transform:uppercase; letter-spacing:2px;'>New Construction Estimate Report</p>
                  </td>
                </tr>
                <tr>
                  <td style='padding:28px 30px;'>
                    <table cellpadding='0' cellspacing='0' style='width:100%;'>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Customer</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{customerName} &middot; {customerPhone} &middot; {customerEmail}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Category</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{r.Category}</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Package</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{packageName} &middot; Rs. {rate} / sqft</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Configuration</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{r.Configuration} &middot; {r.TotalBuiltUpSqft.ToString("N0", CultureInfo.InvariantCulture)} sqft total &middot; {r.PlotAreaSqft.ToString("N0", CultureInfo.InvariantCulture)} sqft plot</td></tr>
                      <tr><td style='padding:7px 0; font-size:12px; color:#8a8577; text-transform:uppercase; letter-spacing:1px;'>Duration</td></tr>
                      <tr><td style='padding:0 0 16px; font-size:16px; color:#333;'>{r.DurationMonths} months</td></tr>
                      <tr><td style='padding:20px 0 8px;'></td></tr>
                      <tr>
                        <td style='background:#fff7ee; border:1px solid #f2dcc3; border-radius:10px; padding:20px 22px;'>
                          <table cellpadding='0' cellspacing='0' style='width:100%;'>
                            <tr><td style='font-size:12px; color:#8a8577; padding-bottom:6px;'>Base cost</td><td align='right' style='font-size:15px; color:#333;'>Rs. {baseCost}</td></tr>
                            <tr><td style='font-size:12px; color:#8a8577; padding:6px 0;'>Add-on extras</td><td align='right' style='font-size:15px; color:#333;'>Rs. {extrasCost}</td></tr>
                            <tr><td style='font-size:12px; color:#8a8577; padding:6px 0;'>EMI (7.1% p.a. &middot; 20 yr)</td><td align='right' style='font-size:15px; color:#333;'>Rs. {emi} / month</td></tr>
                            <tr><td colspan='2' style='border-top:1px dashed #e2c9a8; padding:0; height:12px;'></td></tr>
                            <tr>
                              <td style='font-size:14px; font-weight:700; color:#b24a00;'>GRAND TOTAL</td>
                              <td align='right' style='font-size:20px; font-weight:700; color:#b24a00;'>Rs. {totalCost}</td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>
                    <div style='margin-top:22px; border-top:1px solid #efe9dc; padding-top:14px; font-size:12px; color:#8a8577;'>
                      Generated on {submittedAt} &middot; Full PDF report attached. A free site visit quote will be exact.
                    </div>
                  </td>
                </tr>
              </table>
            </body>
            </html>";
        }
    }
}