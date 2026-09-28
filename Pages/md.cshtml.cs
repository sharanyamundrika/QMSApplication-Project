using Microsoft.AspNetCore.Mvc.RazorPages;
using ClosedXML.Excel;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class MDModel : PageModel
    {
        public List<MDRecord> Records { get; set; } = new List<MDRecord>();

        public void OnGet()
        {
            string filePath = @"C:\QMSData\MilestoneDeliverables.xlsx";

            if (!System.IO.File.Exists(filePath))
            {
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    Records.Add(new MDRecord
                    {
                        RecordID = row.Cell(1).GetString(),
                        SNo = row.Cell(2).GetString(),
                        MilestoneName = row.Cell(3).GetString(),
                        DeliverableName = row.Cell(4).GetString(),
                        DeliverableType = row.Cell(5).GetString(),
                        MilestoneTargetDate = row.Cell(6).GetString(),
                        DeliverableTargetDate = row.Cell(7).GetString(),
                        AssignedTo = row.Cell(8).GetString(),
                        ScheduleDeviation = row.Cell(9).GetString(),
                        DeliverableActivity = row.Cell(10).GetString(),
                        DeviationReason = row.Cell(11).GetString(),
                        DeliverableActualCompletedDate = row.Cell(12).GetString(),
                        MilestoneAchievedDate = row.Cell(13).GetString(),
                        Status = row.Cell(14).GetString(),
                        Remarks = row.Cell(15).GetString()
                    });
                }
            }
        }
    }
}