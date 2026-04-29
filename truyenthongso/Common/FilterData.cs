namespace truyenthongso.Common
{
    public static class FilterData
    {
        public static List<T> searchData<T>(this List<T> source, 
            string key,
            params Func<T, string>[] listParams)
        {
            if(string.IsNullOrEmpty(key)) 
                return source;

            return source.Where(x => 
                listParams.Any(field => 
                field(x) != null 
                && field(x).Contains(key, StringComparison.OrdinalIgnoreCase))).ToList();
               
        }
    }
}
