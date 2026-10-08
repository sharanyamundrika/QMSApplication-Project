using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class ImportUsersModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public ImportUsersModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public string? Message { get; set; }

        private readonly string ExcelFilePath =
            @"C:\QMSData\Users.xlsx";

        public async Task<IActionResult> OnGetAsync()
        {
            // Allow only Admin or Manager to run the import
            var currentUserRole = HttpContext.Session.GetString("UserRole");

            if (!string.Equals(currentUserRole, "Admin",
        StringComparison.OrdinalIgnoreCase)
    &&
    !string.Equals(currentUserRole, "Manager",
        StringComparison.OrdinalIgnoreCase))

                if (!System.IO.File.Exists(ExcelFilePath))
            {
                Message = "Users.xlsx could not be found.";
                return Page();
            }

            try
            {
                using var workbook =
                    new XLWorkbook(ExcelFilePath);

                var worksheet = workbook.Worksheet(1);

                var firstRow = worksheet.FirstRowUsed();

                if (firstRow == null)
                {
                    Message = "Users.xlsx is empty.";
                    return Page();
                }

                int firstNameColumn = 0;
                int lastNameColumn = 0;
                int usernameColumn = 0;
                int roleColumn = 0;

                foreach (var cell in firstRow.CellsUsed())
                {
                    string header =
                        cell.GetString().Trim();

                    if (header.Equals(
                        "FirstName",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        firstNameColumn =
                            cell.Address.ColumnNumber;
                    }

                    if (header.Equals(
                        "LastName",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        lastNameColumn =
                            cell.Address.ColumnNumber;
                    }

                    if (header.Equals(
                        "Username",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        usernameColumn =
                            cell.Address.ColumnNumber;
                    }

                    if (header.Equals(
                        "Role",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        roleColumn =
                            cell.Address.ColumnNumber;
                    }
                }

                if (usernameColumn == 0)
                {
                    Message =
                        "Username column is missing.";

                    return Page();
                }

                int imported = 0;
                int skipped = 0;

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string username =
                        row.Cell(usernameColumn)
                           .GetString()
                           .Trim();

                    if (string.IsNullOrWhiteSpace(username))
                    {
                        continue;
                    }

                    // Check whether this user already exists
                    bool exists =
                        await _db.PortalUsers
                            .AnyAsync(x =>
                                x.Username == username);

                    if (exists)
                    {
                        skipped++;
                        continue;
                    }

                    string firstName = "";

                    if (firstNameColumn != 0)
                    {
                        firstName =
                            row.Cell(firstNameColumn)
                               .GetString()
                               .Trim();
                    }

                    string lastName = "";

                    if (lastNameColumn != 0)
                    {
                        lastName =
                            row.Cell(lastNameColumn)
                               .GetString()
                               .Trim();
                    }

                    string fullName =
                        $"{firstName} {lastName}".Trim();

                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        fullName = username;
                    }

                    string role = "User";

                    if (roleColumn != 0)
                    {
                        string excelRole =
                            row.Cell(roleColumn)
                               .GetString()
                               .Trim();

                        if (!string.IsNullOrWhiteSpace(excelRole))
                        {
                            role = excelRole;
                        }
                    }

                    var user = new PortalUser
                    {
                        Username = username,
                        FullName = fullName,
                        Role = role,
                        IsActive = true
                    };

                    _db.PortalUsers.Add(user);

                    imported++;
                }

                await _db.SaveChangesAsync();

                Message =
                    $"Import completed. " +
                    $"Imported: {imported}. " +
                    $"Skipped existing users: {skipped}.";

                return Page();
            }
            catch
            {
                Message =
                    "Unable to import users from Users.xlsx.";

                return Page();
            }
        }
    }
}