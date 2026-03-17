namespace truyenthongso.PythonAI
{
    public class AIJson
    {
        public data1? edgesSpecifications { get; set; }
        public data2? cornerBendAngles { get; set; }
        public string? shape { get; set; }
        public string? frameShape { get; set; }
        public object? dimensions { get; set; }
        public string? slot { get; set; }
        public string? notes { get; set; }
    }

    public class data1
    {
        public string? leftsize { get; set; }
        public string? topsize { get; set; }
        public string? rightsize { get; set; }
        public string? bottonsize { get; set; }
    }

    public class data2
    {
        public string? toprightcornerbendangle { get; set; }
        public string? topleftcornerbendangle { get; set; }
        public string? bottomrightcornerbendangle { get; set; }
        public string? bottomleftcornerbendangle { get; set; }
    }

    public class tiengtrung
    {
        public string? data { get; set; }
    }
}
