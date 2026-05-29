namespace truyenthongso.ViewModel
{
    public class SuggestionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Address { get; set; }
        public int MutualCount { get; set; }
        public int totalFriends { get; set; }
        public List<SuggestionDto>? Mutual_friend { get; set; }
    }

    public class MutualMap
    {
        public int User1 { get; set; }

        public int User2 { get; set; }
    }

    public class MutualInfoDto
    {
        public int Count { get; set; }

        public List<int> MutualFriendIds { get; set; } = new();
    }
}
