using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMSApplication.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public string? UsernameOrEmail { get; set; }

        [BindProperty]
        public string? MobileNumber { get; set; }

        [BindProperty]
        public string? Otp { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public bool ShowResetSection { get; set; }

        private readonly string ExcelFilePath =
            @"C:\QMSData\Users.xlsx";

        private const string DemoOtp = "123456";

        public void OnGet()
        {
        }

        // -----------------------------
        // VERIFY USER
        // -----------------------------
        public IActionResult OnPostVerify()
        {
            if (string.IsNullOrWhiteSpace(UsernameOrEmail))
            {
                ErrorMessage = "Please enter your username or email.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(MobileNumber))
            {
                ErrorMessage = "Please enter your mobile number.";
                return Page();
            }

            if (!System.IO.File.Exists(ExcelFilePath))
            {
                ErrorMessage =
                    @"Users.xlsx was not found at C:\QMSData\Users.xlsx.";

                return Page();
            }

            try
            {
                using var workbook = new XLWorkbook(ExcelFilePath);

                var worksheet = workbook.Worksheet(1);

                var headerRow = worksheet.FirstRowUsed();

                if (headerRow == null)
                {
                    ErrorMessage = "Users.xlsx is empty.";
                    return Page();
                }

                int usernameColumn = FindColumn(headerRow, "Username");
                int emailColumn = FindColumn(headerRow, "Email");
                int mobileColumn = FindColumn(headerRow, "MobileNumber");

                if (usernameColumn == 0 ||
                    emailColumn == 0 ||
                    mobileColumn == 0)
                {
                    ErrorMessage =
                        "Required columns are missing in Users.xlsx. " +
                        "Required columns: Username, Email, MobileNumber.";

                    return Page();
                }

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string username =
                        row.Cell(usernameColumn)
                           .GetString()
                           .Trim();

                    string email =
                        row.Cell(emailColumn)
                           .GetString()
                           .Trim();

                    string mobile =
                        row.Cell(mobileColumn)
                           .GetString()
                           .Trim();

                    bool usernameMatches =
                        string.Equals(
                            username,
                            UsernameOrEmail.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                    bool emailMatches =
                        string.Equals(
                            email,
                            UsernameOrEmail.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                    bool mobileMatches =
                        string.Equals(
                            mobile,
                            MobileNumber.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                    if ((usernameMatches || emailMatches) &&
                        mobileMatches)
                    {
                        ShowResetSection = true;

                        Message =
                            "Verification successful. Please enter the Demo OTP and your new password.";

                        return Page();
                    }
                }

                ErrorMessage =
                    "Username/Email and Mobile Number do not match.";

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    "Unable to read Users.xlsx: " + ex.Message;

                return Page();
            }
        }

        // -----------------------------
        // RESET PASSWORD
        // -----------------------------
        public IActionResult OnPostReset()
        {
            ShowResetSection = true;

            if (string.IsNullOrWhiteSpace(Otp))
            {
                ErrorMessage = "Please enter the OTP.";
                return Page();
            }

            if (Otp.Trim() != DemoOtp)
            {
                ErrorMessage = "Invalid OTP.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "Please enter a new password.";
                return Page();
            }

            if (!System.IO.File.Exists(ExcelFilePath))
            {
                ErrorMessage =
                    @"Users.xlsx was not found at C:\QMSData\Users.xlsx.";

                return Page();
            }

            try
            {
                using var workbook = new XLWorkbook(ExcelFilePath);

                var worksheet = workbook.Worksheet(1);

                var headerRow = worksheet.FirstRowUsed();

                if (headerRow == null)
                {
                    ErrorMessage = "Users.xlsx is empty.";
                    return Page();
                }

                int usernameColumn = FindColumn(headerRow, "Username");
                int emailColumn = FindColumn(headerRow, "Email");
                int mobileColumn = FindColumn(headerRow, "MobileNumber");
                int passwordColumn = FindColumn(headerRow, "Password");

                if (usernameColumn == 0 ||
                    emailColumn == 0 ||
                    mobileColumn == 0 ||
                    passwordColumn == 0)
                {
                    ErrorMessage =
                        "Required columns are missing in Users.xlsx.";

                    return Page();
                }

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string username =
                        row.Cell(usernameColumn)
                           .GetString()
                           .Trim();

                    string email =
                        row.Cell(emailColumn)
                           .GetString()
                           .Trim();

                    string mobile =
                        row.Cell(mobileColumn)
                           .GetString()
                           .Trim();

                    bool usernameMatches =
                        string.Equals(
                            username,
                            UsernameOrEmail?.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                    bool emailMatches =
                        string.Equals(
                            email,
                            UsernameOrEmail?.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                    bool mobileMatches =
                        string.Equals(
                            mobile,
                            MobileNumber?.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                    if ((usernameMatches || emailMatches) &&
                        mobileMatches)
                    {
                        row.Cell(passwordColumn).Value =
                            NewPassword.Trim();

                        workbook.Save();

                        return RedirectToPage("/Login");
                    }
                }

                ErrorMessage =
                    "Username/Email and Mobile Number do not match.";

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    "Unable to update the password: " + ex.Message;

                return Page();
            }
        }

        // -----------------------------
        // FIND EXCEL COLUMN
        // -----------------------------
        private int FindColumn(
            IXLRow headerRow,
            string requiredHeader)
        {
            foreach (var cell in headerRow.CellsUsed())
            {
                string header = cell
                    .GetString()
                    .Trim()
                    .Replace(" ", "")
                    .Replace("_", "")
                    .Replace("-", "");

                string required = requiredHeader
                    .Trim()
                    .Replace(" ", "")
                    .Replace("_", "")
                    .Replace("-", "");

                if (string.Equals(
                    header,
                    required,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return cell.Address.ColumnNumber;
                }
            }

            return 0;
        }
    }
}