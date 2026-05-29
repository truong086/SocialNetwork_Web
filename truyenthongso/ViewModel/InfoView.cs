namespace truyenthongso.ViewModel
{
    public class InfoView
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public string? image { get; set; }
        public string? Address { get; set; }
        public int? total_Mutual_friend { get; set; }
        public int? total_friend { get; set; }
        public bool isFriend { get; set; }
        public List<SuggestionDto>? Mutual_friend { get; set; }
    }

}
