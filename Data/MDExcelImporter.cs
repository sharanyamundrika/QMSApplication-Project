using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;
using System.Globalization;

namespace QMSApplication.Data
{
    public class MDExcelImporter
    {
        private readonly QMSPortalDbContext _db;

        private readonly string _excelFilePath =
            @"C:\QMSData\MilestoneDeliverables.xlsx";

        public MDExcelImporter(QMSPortalDbContext db)
        {
            _db = db;
        }

        public async Task<int> ImportAsync()
        {
            if (!File.Exists(_excelFilePath))
            {
                throw new FileNotFoundException(
                    "MilestoneDeliverables.xlsx could not be found.",
                    _excelFilePath);
            }

            if (await _db.MDRecords.AnyAsync())
            {
                throw new InvalidOperationException(
                    "M&D records already exist in MySQL. Import was not performed.");
            }

            using var workbook = new XLWorkbook(_excelFilePath);

            var worksheet = workbook.Worksheets
                .FirstOrDefault();

            if (worksheet == null)
            {
                throw new InvalidOperationException(
                    "M&D worksheet could not be found.");
            }

            int headerRow = 0;

            foreach (var row in worksheet.RowsUsed())
            {
                foreach (var cell in row.CellsUsed())
                {
                    if (cell.GetString()
                        .Trim()
                        .Equals(
                            "S.No",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        headerRow = row.RowNumber();
                        break;
                    }
                }

                if (headerRow > 0)
                {
                    break;
                }
            }

            if (headerRow == 0)
            {
                throw new InvalidOperationException(
                    "M&D header row could not be found.");
            }

            int generatedRecordNumber = 0;

            foreach (var row in worksheet.RowsUsed()
                         .Where(r => r.RowNumber() > headerRow))
            {
                bool hasData =
                    !string.IsNullOrWhiteSpace(row.Cell(1).GetString()) ||
                    !string.IsNullOrWhiteSpace(row.Cell(2).GetString()) ||
                    !string.IsNullOrWhiteSpace(row.Cell(3).GetString()) ||
                    !string.IsNullOrWhiteSpace(row.Cell(7).GetString()) ||
                    !string.IsNullOrWhiteSpace(row.Cell(13).GetString());

                if (!hasData)
                {
                    continue;
                }

                generatedRecordNumber++;

                string status = row.Cell(13).GetString().Trim();

                if (status.Equals(
                        "Deffered",
                        StringComparison.OrdinalIgnoreCase) ||
                    status.Equals(
                        "Defferred",
                        StringComparison.OrdinalIgnoreCase))
                {
                    status = "Deferred";
                }

                string scheduleDeviation =
                    row.Cell(8).GetString().Trim();

                if (scheduleDeviation.Equals(
                    "YES",
                    StringComparison.OrdinalIgnoreCase))
                {
                    scheduleDeviation = "Yes";
                }
                else if (scheduleDeviation.Equals(
                    "NO",
                    StringComparison.OrdinalIgnoreCase))
                {
                    scheduleDeviation = "No";
                }

                var milestoneTarget =
                    ParseDate(row.Cell(5));

                var deliverableTarget =
                    ParseDate(row.Cell(6));

                var actualCompleted =
                    ParseDate(row.Cell(11));

                var milestoneAchieved =
                    ParseDate(row.Cell(12));

                var record = new MDEntity
                {
                    RecordID =
                        $"MD-{generatedRecordNumber:0000}",

                    SNo =
                        row.Cell(1).GetString(),

                    MilestoneName =
                        row.Cell(2).GetString(),

                    DeliverableName =
                        row.Cell(3).GetString(),

                    DeliverableType =
                        row.Cell(4).GetString(),

                    MilestoneTargetDate =
                        milestoneTarget,

                    MilestoneTargetDateSource =
                        row.Cell(5).GetString(),

                    DeliverableTargetDate =
                        deliverableTarget,

                    DeliverableTargetDateSource =
                        row.Cell(6).GetString(),

                    AssignedTo =
                        row.Cell(7).GetString(),

                    ScheduleDeviation =
                        scheduleDeviation,

                    DeliverableActivity =
                        row.Cell(9).GetString(),

                    DeviationReason =
                        row.Cell(10).GetString(),

                    DeliverableActualCompletedDate =
                        actualCompleted,

                    DeliverableActualCompletedDateSource =
                        row.Cell(11).GetString(),

                    MilestoneAchievedDate =
                        milestoneAchieved,

                    MilestoneAchievedDateSource =
                        row.Cell(12).GetString(),

                    Status =
                        status,

                    Remarks =
                        row.Cell(14).GetString(),

                    UGs =
                        row.Cell(16).GetString()
                };

                _db.MDRecords.Add(record);
            }

            int importedCount =
                await _db.SaveChangesAsync();

            return importedCount;
        }

        private static DateOnly? ParseDate(
            IXLCell cell)
        {
            string value =
                cell.GetString().Trim();

            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals(
                    "NA",
                    StringComparison.OrdinalIgnoreCase) ||
                value == "-")
            {
                return null;
            }

            if (cell.DataType ==
                XLDataType.DateTime)
            {
                return DateOnly.FromDateTime(cell.GetDateTime());
            }

            string[] formats =
            {
                "dd-MMM-yyyy",
                "d-MMM-yyyy",
                "dd/MM/yyyy",
                "d/MM/yyyy",
                "dd-MM-yyyy",
                "d-MM-yyyy",
                "MM/dd/yyyy",
                "M/d/yyyy"
            };

            if (DateTime.TryParseExact(
                    value,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsed))
            {
                return DateOnly.FromDateTime(parsed);
            }

            if (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsed))
            {
                return DateOnly.FromDateTime(parsed);
            }

            return null;
        }
    }
}