using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QMSApplication.Data;

namespace QMSApplication.Pages
{
    public class MDImportModel : PageModel
    {
        private readonly MDExcelImporter _importer;

        public string Message { get; set; } = "";

        public MDImportModel(MDExcelImporter importer)
        {
            _importer = importer;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                int importedCount =
                    await _importer.ImportAsync();

                Message =
                    $"Import completed successfully. {importedCount} M&D records were imported into MySQL.";
            }
            catch (Exception ex)
            {
                Message =
                    $"Import failed: {ex.Message}";
            }

            return Page();
        }
    }
}