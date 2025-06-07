using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace QuizCompetitionApp
{
    public partial class AdminWindow : Window
    {
        private readonly List<Question> questions = new();
        private Question? selectedQuestion = null;
        private const string DataFilePath = "questions.json";

        public AdminWindow()
        {
            InitializeComponent();
            LoadQuestions();
            RefreshQuestionsList();
        }

        private void SaveQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetPoints(out int points, out int bonusPoints)) return;

            List<string> options = new()
            {
                Answer1.Text,
                Answer2.Text,
                Answer3.Text,
                Answer4.Text
            };

            List<int> correctIndexes = GetCorrectIndexes();
            if (correctIndexes.Count == 0)
            {
                MessageBox.Show("Please select at least one correct answer.");
                return;
            }

            Question question = new()
            {
                Text = QuestionTextBox.Text,
                Options = options,
                CorrectAnswers = correctIndexes,
                ImagePath = ImagePathTextBox.Text,
                Point = points,
                BonusPoint = bonusPoints
            };

            questions.Add(question);
            SaveQuestions();
            RefreshQuestionsList();
            ClearInputs();
        }

        private void UpdateQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQuestion == null)
            {
                MessageBox.Show("Please select a question to update.");
                return;
            }

            if (!TryGetPoints(out int points, out int bonusPoints)) return;

            List<string> options = new()
            {
                Answer1.Text,
                Answer2.Text,
                Answer3.Text,
                Answer4.Text
            };

            List<int> correctIndexes = GetCorrectIndexes();
            if (correctIndexes.Count == 0)
            {
                MessageBox.Show("Please select at least one correct answer.");
                return;
            }

            selectedQuestion.Text = QuestionTextBox.Text;
            selectedQuestion.Options = options;
            selectedQuestion.CorrectAnswers = correctIndexes;
            selectedQuestion.ImagePath = ImagePathTextBox.Text;
            selectedQuestion.Point = points;
            selectedQuestion.BonusPoint = bonusPoints;

            SaveQuestions();
            RefreshQuestionsList();
            ClearInputs();
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsListBox.SelectedItem is Question selected)
            {
                var result = MessageBox.Show("Are you sure you want to delete this question?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    questions.Remove(selected);
                    SaveQuestions();
                    RefreshQuestionsList();
                    ClearInputs();
                }
            }
        }

        private void QuestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionsListBox.SelectedItem is Question selected)
            {
                selectedQuestion = selected;
                QuestionTextBox.Text = selected.Text;

                if (selected.Options.Count >= 4)
                {
                    Answer1.Text = selected.Options[0];
                    Answer2.Text = selected.Options[1];
                    Answer3.Text = selected.Options[2];
                    Answer4.Text = selected.Options[3];
                }

                Correct1.IsChecked = selected.CorrectAnswers.Contains(0);
                Correct2.IsChecked = selected.CorrectAnswers.Contains(1);
                Correct3.IsChecked = selected.CorrectAnswers.Contains(2);
                Correct4.IsChecked = selected.CorrectAnswers.Contains(3);

                ImagePathTextBox.Text = selected.ImagePath;
                PointsTextBox.Text = selected.Point.ToString();
                BonusPointsTextBox.Text = selected.BonusPoint.ToString();

                LoadImagePreview(selected.ImagePath);
            }
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = dialog.FileName;
                LoadImagePreview(dialog.FileName);
            }
        }

        private void LoadImagePreview(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImagePreview.Source = bitmap;
                }
                catch
                {
                    ImagePreview.Source = null;
                }
            }
            else
            {
                ImagePreview.Source = null;
            }
        }

        private List<int> GetCorrectIndexes()
        {
            List<int> correct = new();
            if (Correct1.IsChecked == true) correct.Add(0);
            if (Correct2.IsChecked == true) correct.Add(1);
            if (Correct3.IsChecked == true) correct.Add(2);
            if (Correct4.IsChecked == true) correct.Add(3);
            return correct;
        }

        private bool TryGetPoints(out int points, out int bonusPoints)
        {
            points = 0;
            bonusPoints = 0;

            if (!int.TryParse(PointsTextBox.Text, out points) ||
                !int.TryParse(BonusPointsTextBox.Text, out bonusPoints))
            {
                MessageBox.Show("Please enter valid numbers for points and bonus.");
                return false;
            }
            return true;
        }

        private void RefreshQuestionsList()
        {
            QuestionsListBox.ItemsSource = null;
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].DisplayText = $"{i + 1}. {questions[i].Text}";
            }
            QuestionsListBox.ItemsSource = questions;
            QuestionsListBox.DisplayMemberPath = "DisplayText";
        }

        private void ClearInputs_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            QuestionTextBox.Text = "";
            Answer1.Text = "";
            Answer2.Text = "";
            Answer3.Text = "";
            Answer4.Text = "";
            Correct1.IsChecked = false;
            Correct2.IsChecked = false;
            Correct3.IsChecked = false;
            Correct4.IsChecked = false;
            ImagePathTextBox.Text = "";
            ImagePreview.Source = null;
            PointsTextBox.Text = "";
            BonusPointsTextBox.Text = "";
            selectedQuestion = null;
        }

        private void SaveQuestions()
        {
            var json = JsonSerializer.Serialize(questions);
            File.WriteAllText(DataFilePath, json);
        }

        private void LoadQuestions()
        {
            if (File.Exists(DataFilePath))
            {
                string json = File.ReadAllText(DataFilePath);
                var loaded = JsonSerializer.Deserialize<List<Question>>(json);
                if (loaded != null)
                {
                    questions.Clear();
                    questions.AddRange(loaded);
                }
            }
        }
    }
}
