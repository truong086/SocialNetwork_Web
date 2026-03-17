using System.Text.RegularExpressions;
using truyenthongso.ViewModel;

namespace truyenthongso.Common
{
    public static class FormatTagUser
    {
        public static List<int> FomatData(string description)
        {
            var list = new List<int>();

            var pattern = Status.FORMATDESCRIPTION;
            var matches = Regex.Matches(description, pattern); // Tìm tất cả các đoạn khớp với pattern trong description

            foreach (Match item in matches)
            {
                if (int.TryParse(item.Groups[2].Value, out int userId)) // "item.Groups[2].Value" là giá trị trong nhóm thứ 2, tức là phần id:5 → lấy ra 5.
                {
                    list.Add(userId);
                }
            }

            return list;
        }
    }
}
