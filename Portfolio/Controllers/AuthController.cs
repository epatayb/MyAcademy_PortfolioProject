using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using Portfolio.Services.Email;
using Portfolio.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Admin> _passwordHasher;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, IPasswordHasher<Admin> passwordHasher, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.UserName = model.UserName?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(x => x.UserName == model.UserName);

            if (admin == null)
            {
                AddInvalidLoginError();
                return View(model);
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(
                    admin,
                    admin.Password,
                    model.Password);

            if (passwordResult ==
               PasswordVerificationResult.Failed)
            {
                AddInvalidLoginError();
                return View(model);
            }

            if (passwordResult ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                admin.Password =
                    _passwordHasher.HashPassword(
                        admin,
                        model.Password);

                await _context.SaveChangesAsync();
            }

            var claims = new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    admin.Id.ToString()),

                new(
                    ClaimTypes.Name,
                    admin.UserName),

                new(
                    ClaimTypes.Email,
                    admin.Email),

                new(
                    "fullName",
                    admin.FullName)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            var authenticationProperties =
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    AllowRefresh = true,

                    ExpiresUtc = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(14)
                        : DateTimeOffset.UtcNow.AddMinutes(30)
                };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,

                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);

            HttpContext.Session.SetString(
                "fullName",
                admin.FullName);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction(
                "Index",
                "Dashboard");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            return RedirectToAction(
                "Index",
                "Default");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            model.Email =
                model.Email?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail =
                model.Email.ToLower();

            var admin = await _context.Admins
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == normalizedEmail);
                        
            if (admin is not null)
            {
                var tokenBytes =
                    RandomNumberGenerator.GetBytes(32);

                var token =
                    WebEncoders.Base64UrlEncode(
                        tokenBytes);

                admin.PasswordResetTokenHash =
                    CreateTokenHash(token);

                admin.PasswordResetTokenExpiresAt =
                    DateTime.UtcNow.AddMinutes(30);

                await _context.SaveChangesAsync();

                var resetLink = Url.Action(
                    nameof(ResetPassword),
                    "Auth",
                    new
                    {
                        adminId = admin.Id,
                        token
                    },
                    Request.Scheme);

                var emailBody = $"""
                    <div style="font-family:Arial,sans-serif;line-height:1.6;">
                        <h2>Portfolio Admin Şifre Sıfırlama</h2>

                        <p>Merhaba {admin.FullName},</p>

                        <p>
                            Yönetim paneli şifrenizi sıfırlamak için
                            aşağıdaki bağlantıya tıklayın.
                        </p>

                        <p>
                            <a href="{resetLink}"
                               style="
                                   display:inline-block;
                                   padding:12px 20px;
                                   color:#ffffff;
                                   background:#2563eb;
                                   border-radius:8px;
                                   text-decoration:none;">
                                Şifremi Sıfırla
                            </a>
                        </p>

                        <p>
                            Bu bağlantı 30 dakika boyunca geçerlidir.
                        </p>

                        <p>
                            Bu isteği siz oluşturmadıysanız
                            herhangi bir işlem yapmayın.
                        </p>
                    </div>
                    """;

                await _emailService.SendAsync(
                    admin.Email,
                    "Portfolio Admin şifre sıfırlama",
                    emailBody);
            }

            TempData["SuccessMessage"] =
                "E-posta adresi sistemde kayıtlıysa şifre sıfırlama bağlantısı gönderildi.";

            return RedirectToAction(
                nameof(ForgotPassword));
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(
           int adminId,
           string token)
        {
            var admin = await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == adminId);

            if (admin is null ||
                !IsResetTokenValid(admin, token))
            {
                return View("InvalidResetLink");
            }

            return View(new ResetPasswordViewModel
            {
                AdminId = adminId,
                Token = token
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(x =>
                    x.Id == model.AdminId);

            if (admin is null ||
                !IsResetTokenValid(
                    admin,
                    model.Token))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Şifre sıfırlama bağlantısı geçersiz veya süresi dolmuş.");

                return View(model);
            }

            admin.Password =
                _passwordHasher.HashPassword(
                    admin,
                    model.NewPassword);

            admin.PasswordResetTokenHash = null;
            admin.PasswordResetTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Şifreniz başarıyla değiştirildi. Yeni şifrenizle giriş yapabilirsiniz.";

            return RedirectToAction(
                nameof(Login));
        }


        private void AddInvalidLoginError()
        {
            ModelState.AddModelError(
                string.Empty,
                "Kullanıcı adı veya şifre hatalı.");
        }

        private static string CreateTokenHash(
            string token)
        {
            var tokenBytes =
                Encoding.UTF8.GetBytes(token);

            var hashBytes =
                SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }

        private static bool IsResetTokenValid(
            Admin admin,
            string? token)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(
                    admin.PasswordResetTokenHash) ||
                !admin.PasswordResetTokenExpiresAt.HasValue ||
                admin.PasswordResetTokenExpiresAt <=
                    DateTime.UtcNow)
            {
                return false;
            }

            var suppliedHash =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(token));

            byte[] storedHash;

            try
            {
                storedHash = Convert.FromHexString(
                    admin.PasswordResetTokenHash);
            }
            catch (FormatException)
            {
                return false;
            }

            return CryptographicOperations
                .FixedTimeEquals(
                    suppliedHash,
                    storedHash);
        }
    }
}