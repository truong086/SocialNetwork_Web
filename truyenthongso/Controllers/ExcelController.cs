using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        public ExcelController()
        {
            
        }
        //[HttpPost]
        //[Route(nameof(LoadExcel))]
        //public async Task<PayLoad<object>> LoadExcel(string excel1, string excel2)
        //{
        //    var allStudents = new List<studentKyTuc>();
        //    var ktx1Dict = new Dictionary<string, string>(); // MaSV -> TenPhong

        //    using (var package = new ExcelPackage(new FileInfo(excel1)))
        //    {
        //        if (package.Workbook.Worksheets.Count == 0)
        //        {
        //            throw new Exception("File Excel không có sheet nào.");
        //        }
        //        var sheet = package.Workbook.Worksheets[0];

        //        int rowCount = sheet.Dimension.Rows;

        //        if(rowCount > 0) {
        //            for (int row = 2; row <= rowCount; row++)
        //            {
        //                string maSV = sheet.Cells[row, 3].Text.Trim();
        //                string tenPhong = sheet.Cells[row, 7].Text.Trim();

        //                if (!ktx1Dict.ContainsKey(maSV) && sheet.Cells[row, 7].Text.Trim() == "二宿")
        //                    ktx1Dict[maSV] = tenPhong;
        //            }
        //        }
        //    }

        //    // Bước 2: Đọc File tất cả sinh viên và đánh dấu
        //    using (var package = new ExcelPackage(new FileInfo(excel2)))
        //    {
        //        if (package.Workbook.Worksheets.Count == 0)
        //        {
        //            throw new Exception("File Excel không có sheet nào.");
        //        }
        //        var sheet = package.Workbook.Worksheets[0];
        //        int rowCount = sheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            var sv = new studentKyTuc
        //            {
        //                班級代號 = sheet.Cells[row, 1].Text.Trim(),
        //                班級名稱 = sheet.Cells[row, 2].Text.Trim(),
        //                學號 = sheet.Cells[row, 3].Text.Trim(),
        //                學生姓名 = sheet.Cells[row, 4].Text.Trim(),
        //                費用1金額 = sheet.Cells[row, 5].Text.Trim(),
        //                宿舍 = "",
        //                原本床位 = "",
        //                新床位 = sheet.Cells[row, 8].Text.Trim(),
        //                繳費情況 = sheet.Cells[row, 9].Text.Trim(),
        //                備註 = sheet.Cells[row, 10].Text.Trim()
        //            };

        //            if (ktx1Dict.ContainsKey(sv.學號))
        //            {
        //                sv.宿舍 = "二宿";
        //                sv.原本床位 = ktx1Dict[sv.學號];
        //            }

        //            allStudents.Add(sv);
        //        }
        //    }
        //    ExportMergedExcel(allStudents, "FileHoanChinh.xlsx");
        //    return await Task.FromResult(PayLoad<object>.Successfully(allStudents));
        //}

        [HttpPost]
        [Route(nameof(ExportMergedExcel))]
        public IActionResult ExportMergedExcel(string excel1, string excel2)
        {
            var allStudents = new List<studentKyTuc>();
            var ktx1Dict = new Dictionary<string, string>(); // MaSV -> TenPhong
            var ktx1DictList = new List<studenUpdate>(); // MaSV -> TenPhong

            using (var package = new ExcelPackage(new FileInfo(excel1)))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("File Excel không có sheet nào.");
                }
                var sheet = package.Workbook.Worksheets[0];

                int rowCount = sheet.Dimension.Rows;

                if (rowCount > 0)
                {
                    for (int row = 2; row <= rowCount; row++)
                    {
                        string maSV = sheet.Cells[row, 3].Text;
                        string tenPhong = sheet.Cells[row, 9].Text;
                        string tienPhong = sheet.Cells[row, 6].Text;

                        var checkList = ktx1DictList.FirstOrDefault(x => x.學號 == maSV);
                        if (checkList == null && sheet.Cells[row, 8].Text == "二宿" && !ktx1Dict.ContainsKey(maSV))
                        {
                            ktx1Dict[maSV] = tenPhong;
                            ktx1DictList.Add(new studenUpdate
                            {
                                學號 = maSV,
                                原本床位 = tenPhong,
                                費用1金額 = tienPhong
                            });
                        }
                    }
                }
            }

            // Bước 2: Đọc File tất cả sinh viên và đánh dấu
            using (var package = new ExcelPackage(new FileInfo(excel2)))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("File Excel không có sheet nào.");
                }
                var sheet = package.Workbook.Worksheets[0];
                int rowCount = sheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var sv = new studentKyTuc
                    {
                        班級代號 = sheet.Cells[row, 1].Text.Trim(),
                        班級名稱 = sheet.Cells[row, 2].Text.Trim(),
                        學號 = sheet.Cells[row, 3].Text.Trim(),
                        學生姓名 = sheet.Cells[row, 4].Text.Trim(),
                        宿舍 = "",
                        原本床位 = "",
                        新床位 = sheet.Cells[row, 8].Text.Trim(),
                        繳費情況 = sheet.Cells[row, 9].Text.Trim(),
                        備註 = sheet.Cells[row, 10].Text.Trim()
                    };

                    var checkListData = ktx1DictList.FirstOrDefault(x => x.學號 == sv.學號);
                    if (ktx1Dict.ContainsKey(sv.學號) && checkListData != null)
                    {
                        sv.宿舍 = "二宿";
                        sv.原本床位 = ktx1Dict[sv.學號];
                        sv.費用1金額 = checkListData.費用1金額;
                    }

                    allStudents.Add(sv);
                }
            }

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("DanhSachHoanChinh");

                // Header
                sheet.Cells[1, 1].Value = "班級代號";
                sheet.Cells[1, 2].Value = "班級名稱";
                sheet.Cells[1, 3].Value = "學號";
                sheet.Cells[1, 4].Value = "學生姓名";
                sheet.Cells[1, 5].Value = "費用1金額";
                sheet.Cells[1, 6].Value = "宿舍";
                sheet.Cells[1, 7].Value = "原本床位";
                sheet.Cells[1, 8].Value = "新床位";
                sheet.Cells[1, 9].Value = "繳費情況";
                sheet.Cells[1, 10].Value = "備註";

                // Data
                allStudents = allStudents.OrderByDescending(x =>
                {
                    var match = Regex.Match(x.原本床位, @"^\d{4}");
                    if (match.Success && int.TryParse(match.Value, out int soPhong))
                    {
                        return soPhong;
                    }
                    return int.MinValue;
                }).ToList();

              
                int row = 2;
                for (int i = 0; i < allStudents.Count; i++)
                {
                    var sv = allStudents[i];

                    sheet.Cells[row, 1].Value = sv.班級代號;
                    sheet.Cells[row, 2].Value = sv.班級名稱;
                    sheet.Cells[row, 3].Value = sv.學號;
                    sheet.Cells[row, 4].Value = sv.學生姓名;
                    sheet.Cells[row, 5].Value = sv.費用1金額;
                    sheet.Cells[row, 6].Value = sv.宿舍;
                    sheet.Cells[row, 7].Value = sv.原本床位;
                    sheet.Cells[row, 8].Value = sv.新床位;
                    sheet.Cells[row, 9].Value = sv.繳費情況;
                    sheet.Cells[row, 10].Value = sv.備註;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachHoanChinh.xlsx");
                
            }
        }
    }

    public class studenUpdate
    {
        public string? 學號 { get; set; }
        public string? 宿舍 { get; set; }
        public string? 原本床位 { get; set; }
        public string? 費用1金額 { get; set; }
    }
}
