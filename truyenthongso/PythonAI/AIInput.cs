namespace truyenthongso.PythonAI
{
    public class AIInput
    {
        public int? UserId { get; set; }
        public int CategoryId { get; set; }
        public float TimeOnPage { get; set; }
        public bool ScrolledToBottom { get; set; }
        public bool Liked { get; set; }
        public int TimeOfDay { get; set; }
        public string Device { get; set; }
    }

    public class AIResult
    {
        public float score { get; set; }
    }
}
