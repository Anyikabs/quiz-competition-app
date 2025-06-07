using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace QuizCompetitionApp
{
    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public List<int> CorrectAnswers { get; set; } = new List<int>();
        public string ImagePath { get; set; } = string.Empty;
        public int Point { get; set; }
        public int BonusPoint { get; set; }

        [JsonIgnore]
        public string DisplayText { get; set; } = string.Empty;

        public override string ToString()
        {
            return DisplayText;
        }
    }
}

