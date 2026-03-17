namespace truyenthongso.Common
{
    public static class ConvertCacheKey
    {
        public static string GetCacheKey(int userId) => $"user:{userId}:liked_posts";
    }
}
