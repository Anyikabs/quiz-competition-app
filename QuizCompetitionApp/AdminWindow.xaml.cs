// File: AdminWindow.xaml.cs
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuizCompetitionApp
{
    public partial class AdminWindow : Window
    {
        private string imagePath = "";
        private int? editingIndex = null;

        public AdminWindow()
        {
            InitializeComponent();
            LoadQuestionList();
        }

        private void SaveQuestion_Click(object sender, RoutedEventArgs e)
        {
            string questionText = QuestionTextBox.Text;
            List<string> answers = new List<string>
            {
                Answer1.Text,
                Answer2.Text,
                Answer3.Text,
                Answer4.Text
            };

            List<int> correctAnswers = new List<int>();
            if (Correct1.IsChecked == true) correctAnswers.Add(0);
            if (Correct2.IsChecked == true) correctAnswers.Add(1);
            if (Correct3.IsChecked == true) correctAnswers.Add(2);
            if (Correct4.IsChecked == true) correctAnswers.Add(3);

            int.TryParse(PointsTextBox.Text, out int points);
            int.TryParse(BonusTextBox.Text, out int bonusPoints);

            Question newQuestion = new Question
            {
                QuestionText = questionText,
                AnswerOptions = answers,
                CorrectAnswerIndices = correctAnswers,
                Points = points,
                BonusPoints = bonusPoints,
                ImagePath = imagePath
            };

            string filePath = "questions.json";
            List<Question> questions = new List<Question>();

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                questions = JsonSerializer.Deserialize<List<Question>>(existingJson) ?? new List<Question>();
            }

            if (editingIndex.HasValue)
            {
                questions[editingIndex.Value] = newQuestion;
                editingIndex = null;
            }
            else
            {
                questions.Add(newQuestion);
            }

            string json = JsonSerializer.Serialize(questions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);

            MessageBox.Show("Question saved successfully!");
            LoadQuestionList();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.png)|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                ImagePathText.Text = imagePath;
            }
        }

        private void LoadQuestionList()
        {
            QuestionList.Items.Clear();
            string filePath = "questions.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var questions = JsonSerializer.Deserialize<List<Question>>(json);
                if (questions != null)
                {
                    for (int i = 0; i < questions.Count; i++)
                    {
                        QuestionList.Items.Add($"{i + 1}. {questions[i].QuestionText}");
                    }
                }
            }
        }

        private void QuestionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionList.SelectedIndex >= 0)
            {
                string filePath = "questions.json";
                if (!File.Exists(filePath)) return;

                var questions = JsonSerializer.Deserialize<List<Question>>(File.ReadAllText(filePath));
                if (questions == null || QuestionList.SelectedIndex >= questions.Count) return;

                var selected = questions[QuestionList.SelectedIndex];
                editingIndex = QuestionList.SelectedIndex;

                QuestionTextBox.Text = selected.QuestionText;
                Answer1.Text = selected.AnswerOptions.ElementAtOrDefault(0) ?? "";
                Answer2.Text = selected.AnswerOptions.ElementAtOrDefault(1) ?? "";
                Answer3.Text = selected.AnswerOptions.ElementAtOrDefault(2) ?? "";
                Answer4.Text = selected.AnswerOptions.ElementAtOrDefault(3) ?? "";

                Correct1.IsChecked = selected.CorrectAnswerIndices.Contains(0);
                Correct2.IsChecked = selected.CorrectAnswerIndices.Contains(1);
                Correct3.IsChecked = selected.CorrectAnswerIndices.Contains(2);
                Correct4.IsChecked = selected.CorrectAnswerIndices.Contains(3);

                PointsTextBox.Text = selected.Points.ToString();
                BonusTextBox.Text = selected.BonusPoints.ToString();
                imagePath = selected.ImagePath ?? "";
                ImagePathText.Text = string.IsNullOrEmpty(imagePath) ? "No image selected" : imagePath;

                UpdateAnswerBorders();
            }
        }

        private void Answer_Checked(object sender, RoutedEventArgs e)
        {
            UpdateAnswerBorders();
        }

        private void Answer_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateAnswerBorders();
        }

        private void UpdateAnswerBorders()
        {
            Border1.Background = Correct1.IsChecked == true ? Brushes.LightGreen : Brushes.Transparent;
            Border2.Background = Correct2.IsChecked == true ? Brushes.LightGreen : Brushes.Transparent;
            Border3.Background = Correct3.IsChecked == true ? Brushes.LightGreen : Brushes.Transparent;
            Border4.Background = Correct4.IsChecked == true ? Brushes.LightGreen : Brushes.Transparent;
        }
    }
}
