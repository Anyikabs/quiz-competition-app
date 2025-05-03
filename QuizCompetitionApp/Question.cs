// File: Question.cs
using System.Collections.Generic;

namespace QuizCompetitionApp
{
    public class Question
    {
        public string QuestionText { get; set; }
        public List<string> AnswerOptions { get; set; }
        public List<int> CorrectAnswerIndices { get; set; } = new List<int>();
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public string ImagePath { get; set; }
    }
}
