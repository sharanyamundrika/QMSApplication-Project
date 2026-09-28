using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMSApplication.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string? FirstName { get; set; }

        [BindProperty]
        public string? LastName { get; set; }

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? MobileNumber { get; set; }

        [BindProperty]
        public string? Otp { get; set; }

        public string? Message { get; set; }

        public string? ErrorMessage { get; set; }

        public bool ShowOtpSection { get; set; }

        private readonly string ExcelFilePath =
            @"C:\QMSData\Users.xlsx";

        private const string DemoOtp = "123456";

        public void OnGet()
        {
        }

        public IActionResult OnPostSendOtp()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = "Please enter your first name.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Please enter your last name.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Please enter your username.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(MobileNumber))
            {
                ErrorMessage = "Please enter your mobile number.";
                return Page();
            }

            if (!System.IO.File.Exists(ExcelFilePath))
            {
                ErrorMessage = "User workbook could not be found.";
                return Page();
            }

            try
            {
                using var workbook = new XLWorkbook(ExcelFilePath);

                var worksheet = workbook.Worksheet(1);

                var firstRow = worksheet.FirstRowUsed();

                if (firstRow == null)
                {
                    ErrorMessage = "Users.xlsx is empty.";
                    return Page();
                }

                int usernameColumn = 0;

                foreach (var cell in firstRow.CellsUsed())
                {
                    string header = cell.GetString().Trim();

                    if (header.Equals(
                        "Username",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        usernameColumn = cell.Address.ColumnNumber;
                        break;
                    }
                }

                if (usernameColumn == 0)
                {
                    ErrorMessage =
                        "Username column is missing in Users.xlsx.";

                    return Page();
                }

                // Check whether username already exists
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string existingUsername =
                        row.Cell(usernameColumn).GetString().Trim();

                    if (string.Equals(
                        existingUsername,
                        Username.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorMessage =
                            "Username already exists.";

                        return Page();
                    }
                }

                ShowOtpSection = true;

                Message =
                    "Your details have been accepted. Please enter the Demo OTP.";

                return Page();
            }
            catch
            {
                ErrorMessage =
                    "Unable to read the user workbook.";

                return Page();
            }
        }

        public IActionResult OnPostRegister()
        {
            ShowOtpSection = true;

            // OTP validation
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

            if (!System.IO.File.Exists(ExcelFilePath))
            {
                ErrorMessage = "User workbook could not be found.";
                return Page();
            }

            try
            {
                using var workbook = new XLWorkbook(ExcelFilePath);

                var worksheet = workbook.Worksheet(1);

                int nextRow =
                    (worksheet.LastRowUsed()?.RowNumber() ?? 1) + 1;

                // Save registration details
                worksheet.Cell(nextRow, 1).Value =
                    FirstName?.Trim();

                worksheet.Cell(nextRow, 2).Value =
                    LastName?.Trim();

                worksheet.Cell(nextRow, 3).Value =
                    Username?.Trim();

                worksheet.Cell(nextRow, 4).Value =
                    Password?.Trim();

                worksheet.Cell(nextRow, 5).Value =
                    Email?.Trim();

                worksheet.Cell(nextRow, 6).Value =
                    MobileNumber?.Trim();

                // Phase 1 default role
                worksheet.Cell(nextRow, 7).Value =
                    "User";

                workbook.Save();

                // Registration completed
                return RedirectToPage("/Login");
            }
            catch
            {
                ErrorMessage =
                    "Unable to save the registration details.";

                return Page();
            }
        }
    }
}