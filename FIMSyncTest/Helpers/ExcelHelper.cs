using System;
using System.Collections.Generic;
using System.Linq;
using FIMSyncTest.Helpers;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class ExcelHelper {
        private string mFilePath = null;
        private string mSheet = null;
        private string mFileCopied = "";
        private SpreadsheetDocument mSpreadSheetDocument = null;
        private WorkbookPart mWorkbook = null;
        private WorksheetPart mWorksheet = null;
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public ExcelHelper(string filepath) {
            mFilePath = filepath;
            mWorkbook = OpenWorkbookReadonly();
        }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public void CloseExcel() {
            mFilePath = "";
            mSpreadSheetDocument.Close();
            mWorkbook = null;
            mWorksheet = null;
            mSpreadSheetDocument = null;
            if (File.Exists(mFileCopied)) File.Delete(mFileCopied);
            mFileCopied = "";
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public ExcelHelper Sheet(string sheetName) {
            if (mSheet != sheetName) {
                mSheet = sheetName;
                mWorksheet = OpenSpreadsheet(mSheet);
            }
            return this;
        }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private string CopyUsedFile(string sFile) {
            using (var inputFile = new FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                mFileCopied = sFile + ".TMP.xlsx";
                using (var outputFile = new FileStream(mFileCopied, FileMode.Create)) {
                    var buffer = new byte[0x10000];
                    int bytes;

                    while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0) {
                        outputFile.Write(buffer, 0, bytes);
                        }
                    }
                }
            return mFileCopied;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private WorkbookPart OpenWorkbookReadonly() {
            // Open a SpreadsheetDocument based on a filepath.
            try {
                mSpreadSheetDocument = SpreadsheetDocument.Open(CopyUsedFile(mFilePath), false);
                }
            catch (Exception) { }
            // Retrieve a reference to the workbook part.
            WorkbookPart wbPart = mSpreadSheetDocument.WorkbookPart;
            return wbPart;
        }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private WorksheetPart OpenSpreadsheet(string sheetName) {
            // Find the sheet with the supplied name, and then use that Sheet object to retrieve a reference to the first worksheet.
            Sheet theSheet = mWorkbook.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
            // Throw an exception if there is no sheet.
            if (theSheet == null) throw new ArgumentException("sheetName");
            // Retrieve a reference to the worksheet part.
            WorksheetPart wsPart = (WorksheetPart)(mWorkbook.GetPartById(theSheet.Id));
            return wsPart;
        }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string Cell(long Row, long Col) {
            // don't work for Col > 26
            //string addressName = string.Format("{0}{1}{2}{3}", Convert.ToChar(64 + (Col / 26 / 26)), Convert.ToChar(64 + ((Col - (Col % 26)) % 26)), Convert.ToChar(64 + (Col % 26)), Row).Replace("@", "");
            string addressName = GetStandardExcelColumnName((int)Col) + Row;
            string sValue = Cell(addressName);
            if (sValue != null && sValue.Length > 100 && sValue.StartsWith(CryptHelper.Mark)) {
                try { sValue = CryptHelper.Decrypt(sValue.Replace(CryptHelper.Mark, "")); } catch (Exception ex) { LogHelper.Msg("Decrypt failed in Row[" + Row + "] Col[" + Col + "]: " + ex.Message); sValue = CryptHelper.Mark; }
                }
            return sValue;
        }
        //_________________________________________________________________________________________________________
        //https://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa
        //_________________________________________________________________________________________________________
        public static string GetStandardExcelColumnName(int columnNumberOneBased) {
            int baseValue = Convert.ToInt32('A');
            int columnNumberZeroBased = columnNumberOneBased - 1;
            string ret = "";
            if (columnNumberOneBased > 26) ret = GetStandardExcelColumnName(columnNumberZeroBased / 26);
            return ret + Convert.ToChar(baseValue + (columnNumberZeroBased % 26));
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string Cell(string addressName) {
            string value = null;
            // Use its Worksheet property to get a reference to the cell whose address matches the address you supplied.
            Cell theCell = mWorksheet.Worksheet.Descendants<Cell>().Where(c => c.CellReference == addressName).FirstOrDefault();

            // If the cell does not exist, return an empty string.
            if (theCell == null) return value;

            value = theCell.CellValue == null ? "" : theCell.CellValue.Text; //.CellFormula.Text;//.InnerText;
            // If the cell represents an integer number, you are done. 
            // For dates, this code returns the serialized value that represents the date. The code handles strings and 
            // Booleans individually. For shared strings, the code looks up the corresponding value in the shared string table. 
            // For Booleans, the code converts the value into the words TRUE or FALSE.
            if (theCell.DataType == null) return value;
            switch (theCell.DataType.Value) {
                case CellValues.SharedString:
                    // For shared strings, look up the value in the shared strings table.
                    var stringTable = mWorkbook.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                    // If the shared string table is missing, something is wrong. Return the index that is in the cell. Otherwise, look up the correct text in the table.
                    if (stringTable != null) value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                    break;

                case CellValues.Boolean:
                    value = (value == "0") ? "FALSE" : "TRUE";
                    break;
            }

            return value;
        }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public DataTable Table() {
            List<string> Cols = new List<string>();
            DataTable oDatos = new DataTable(mSheet);
            int Row = 1, Col = 1;
            string r = this.Cell(Row, Col);
            while (r != null) {
                Cols.Add(r);
                oDatos.Columns.Add(r, typeof(String));
                r = this.Cell(Row, ++Col);
            }
            Col = 1; Row = 2;
            r = this.Cell(Row, Col);
            while (r != null && r != "") {
                DataRow dataRow = oDatos.NewRow();
                int cont = 0;
                foreach (string s in Cols) {
                    dataRow[s] = this.Cell(Row, Col + cont++);
                }
                oDatos.Rows.Add(dataRow);
                Row++;
                r = this.Cell(Row, Col);
            }

            return oDatos;
        }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public List<string> Columns() {
            List<string> Cols = new List<string>();
            int Row = 1, Col = 1;
            string r = this.Cell(Row, Col);
            while (r != null) {
                Cols.Add(r);
                r = this.Cell(Row, ++Col);
            }
            return Cols;
        }
    }
}
