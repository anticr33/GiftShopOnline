namespace GiftShop.Models
{
    public class QuestionnaireViewModel
    {
        public List<string> Questions { get; set; } = new List<string>();
        public Dictionary<int, List<string>> Options { get; set; } = new Dictionary<int, List<string>>();
        public List<string> SelectedAnswers { get; set; } = new List<string>();
    }
}
