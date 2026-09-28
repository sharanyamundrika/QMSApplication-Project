using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMSApplication.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? Message { get; set; }

        private readonly string ExcelFilePath =
            @"C:\QMSData\Users.xlsx";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Username validation
            if (string.IsNullOrWhiteSpace(Username))
            {
                ModelState.AddModelError(
                    nameof(Username),
                    "Please enter your username.");

                return Page();
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(
                    nameof(Password),
                    "Please enter your password.");

                return Page();
            }

            // Check Excel file
            if (!System.IO.File.Exists(ExcelFilePath))
            {
                Message = "User workbook could not be found.";
                return Page();
            }

            try
            {
                using var workbook = new XLWorkbook(ExcelFilePath);

                var worksheet = workbook.Worksheet(1);

                var firstRow = worksheet.FirstRowUsed();

                if (firstRow == null)
                {
                    Message = "Users.xlsx is empty.";
                    return Page();
                }

                int usernameColumn = 0;
                int passwordColumn = 0;
                int firstNameColumn = 0;

                // Find columns using the Excel headers
                foreach (var cell in firstRow.CellsUsed())
                {
                    string header = cell.GetString().Trim();

                    if (header.Equals(
                        "Username",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        usernameColumn = cell.Address.ColumnNumber;
                    }

                    if (header.Equals(
                        "Password",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        passwordColumn = cell.Address.ColumnNumber;
                    }

                    if (header.Equals(
                        "FirstName",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        firstNameColumn = cell.Address.ColumnNumber;
                    }
                }

                if (usernameColumn == 0 || passwordColumn == 0)
                {
                    Message =
                        "Username or Password column is missing in Users.xlsx.";

                    return Page();
                }

                // Check each user
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string excelUsername =
                        row.Cell(usernameColumn).GetString().Trim();

                    string excelPassword =
                        row.Cell(passwordColumn).GetString().Trim();

                    if (string.Equals(
                            excelUsername,
                            Username.Trim(),
                            StringComparison.OrdinalIgnoreCase)
                        &&
                        excelPassword == Password)
                    {
                        string firstName = Username.Trim();

                        if (firstNameColumn != 0)
                        {
                            string excelFirstName =
                                row.Cell(firstNameColumn).GetString().Trim();

                            if (!string.IsNullOrWhiteSpace(excelFirstName))
                            {
                                firstName = excelFirstName;
                            }
                        }

                        // Store logged-in user in session
                        HttpContext.Session.SetString(
                            "LoggedInUser",
                            firstName);

                        // Login successful
                        return RedirectToPage("/Welcome");
                    }
                }

                // Invalid credentials
                Message = "Invalid username or password.";

                return Page();
            }
            catch
            {
                Message = "Unable to read the user workbook.";

                return Page();
            }
        }
    }
}
