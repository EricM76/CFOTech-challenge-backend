namespace ChallengeCFOTech.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url_Image { get; set; }
        public string Description { get; set; } 
        public List<string> Comics { get; set; }  
        public List<string> Series { get; set;}
        public List<string> Stories { get; set;}
    }
}
