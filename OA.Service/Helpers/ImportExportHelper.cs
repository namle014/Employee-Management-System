using Aspose.Pdf;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.VModels;
using OfficeOpenXml;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Runtime.Serialization;
using static OA.Core.Constants.CommonConstants;

namespace OA.Service.Helpers
{
    public static class ImportExportHelper<T> where T : class
    {
        public static ExportStream? ExportFile(ExportFileVModel exportModel, IEnumerable<T> fileContent)
        {
            string exportType = exportModel.Type ?? string.Empty;

            return String.Equals(exportType, ExportTypeConstant.EXCEL, StringComparison.OrdinalIgnoreCase)
                ? ExportExcel(exportModel.FileName ?? string.Empty, exportModel.SheetName ?? string.Empty, fileContent)
                : String.Equals(exportType, ExportTypeConstant.PDF, StringComparison.OrdinalIgnoreCase)
                    ? ExportPdf(exportModel.FileName ?? string.Empty, fileContent)
                    : null;
        }


        public static ExportStream? ExportExcel(string fileName, string sheetName, IEnumerable<T> fileContent, Action<ExcelPackage, string, IEnumerable<T>>? delegateAction = null)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(sheetName)) return null;
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    if (delegateAction == null)
                    {
                        ExportDefault(package, sheetName, fileContent);
                    }
                    else
                    {
                        delegateAction(package, sheetName, fileContent);
                    }
                }
                stream.Position = 0;

                return new ExportStream()
                {
                    FileName = $"{fileName}{CommonConstants.Excel.fileNameExtention}",
                    Stream = stream,
                    ContentType = Excel.openxmlformats
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void ExportDefault(ExcelPackage package, string sheetName, IEnumerable<T> fileContent)
        {
            var objectType = typeof(T);
            var properties = objectType.GetProperties();
            var headers = GetHeaders(objectType);
            var rows = properties.Select(p => p.Name).ToList();

            // name of the sheet
            var workSheet = package.Workbook.Worksheets.Add(sheetName);

            // Header of the Excel sheet
            for (int i = 0; i < headers.Count; i++)
            {
                workSheet.Cells[1, i + 1].Value = headers[i];
            }

            // Inserting the article data into excel
            // sheet by using the for each loop
            // As we have values to the first row
            // we will start with second row
            int recordIndex = 2;
            foreach (var item in fileContent)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    workSheet.Cells[recordIndex, i + 1].Value = objectType.GetProperty(rows[i])?.GetValue(item);
                    if (workSheet.Cells[recordIndex, i + 1].Value != null &&
                        workSheet.Cells[recordIndex, i + 1].Value.GetType() == typeof(DateTime))
                    {
                        workSheet.Cells[recordIndex, i + 1].Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
                    }
                }
                recordIndex++;
            }

            workSheet.Cells.AutoFitColumns();
            package.Save();
        }

        public static ExportStream ExportPdf(string fileName, IEnumerable<T> fileContent)
        {
            var objectType = typeof(T);
            var headers = GetHeaders(objectType);
            var rows = objectType.GetProperties().Select(p => p.Name).ToList();

            var document = new Document
            {
                PageInfo = new PageInfo
                {
                    Margin = new MarginInfo(28, 28, 28, 40)
                }
            };
            Page page = document.Pages.Add();
            Table table = new()
            {
                ColumnAdjustment = ColumnAdjustment.AutoFitToContent,
                DefaultCellPadding = new MarginInfo(5, 5, 5, 5),
                Border = new BorderInfo(BorderSide.All, .5f, Color.Black),
                DefaultCellBorder = new BorderInfo(BorderSide.All, .2f, Color.Black),
            };

            var headerRow = table.Rows.Add();
            foreach (string header in headers)
            {
                headerRow.Cells.Add(header);
            }

            foreach (var item in fileContent)
            {
                var dataRow = table.Rows.Add();
                for (int i = 0; i < rows.Count; i++)
                {
                    var rowData = Convert.ToString(objectType.GetProperty(rows[i])?.GetValue(item));

                    if (objectType.GetProperty(rows[i])?.GetValue(item) != null &&
                        objectType.GetProperty(rows[i])?.GetValue(item)?.GetType() == typeof(DateTime))//format datetime
                    {
                        DateTime? date = (DateTime?)objectType.GetProperty(rows[i])?.GetValue(item);
                        rowData = date?.ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    else if (objectType.GetProperty(rows[i])?.GetValue(item) != null &&
                        objectType.GetProperty(rows[i])?.GetValue(item)?.GetType() == typeof(decimal))// format number
                    {
                        decimal? number = (decimal?)objectType.GetProperty(rows[i])?.GetValue(item);
                        NumberFormatInfo numberFormatInfo =
                            (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                        numberFormatInfo.NumberDecimalSeparator = ",";
                        numberFormatInfo.NumberGroupSeparator = ".";
                        rowData = string.Format(numberFormatInfo, "{0:n}", number);
                    }
                    dataRow.Cells.Add(rowData);
                }
            }

            page.Paragraphs.Add(table);
            document.PageInfo.IsLandscape = true;
            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;

            return new ExportStream
            {
                FileName = $"{fileName}{CommonConstants.Pdf.fileNameExtention}",
                Stream = stream,
                ContentType = Pdf.format
            };
        }


        private static List<string> GetHeaders(Type type)
        {
            var properties = type.GetProperties();
            var headers = new List<string>();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(DataMemberAttribute), false);
                foreach (DataMemberAttribute dma in attributes.Cast<DataMemberAttribute>())
                {
                    if (!string.IsNullOrEmpty(dma.Name))
                    {
                        headers.Add(dma.Name);
                    }
                }
            }
            return headers;
        }

        public static List<dynamic> Import(string pathFile)
        {
            var resultObject = new List<dynamic>();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(pathFile)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.End.Row;
                var colCount = worksheet.Dimension.End.Column;
                for (int row = 2; row <= rowCount; row++)
                {
                    var dataRow = new ExpandoObject() as IDictionary<string, object>;
                    for (int col = 1; col <= colCount; col++)
                    {
                        string? fieldName = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                        var valueCell = worksheet.Cells[row, col].Value;
                        dataRow.Add(fieldName ?? "", valueCell);
                    }
                    resultObject.Add(dataRow);
                }
            }
            return resultObject;
        }
    }
}
