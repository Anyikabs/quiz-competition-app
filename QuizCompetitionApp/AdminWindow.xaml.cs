using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace QuizCompetitionApp
{
    public partial class AdminWindow : Window
    {
        private List<Question> questions = new List<Question>();
        private Question? selectedQuestion = null;

        public AdminWindow()
        {
            InitializeComponent();
            QuestionsListBox.ItemsSource = questions;
        }

        private void SaveQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PointsTextBox.Text, out int points) ||
                !int.TryParse(BonusPointsTextBox.Text, out int bonusPoints))
            {
                MessageBox.Show("Please enter valid numbers for points and bonus.");
                return;
            }

            List<string> options = new()
        {
            Answer1.Text,
            Answer2.Text,
            Answer3.Text,
            Answer4.Text
        };

            List<int> correctIndexes = new();
            if (Correct1.IsChecked == true) correctIndexes.Add(0);
            if (Correct2.IsChecked == true) correctIndexes.Add(1);
            if (Correct3.IsChecked == true) correctIndexes.Add(2);
            if (Correct4.IsChecked == true) correctIndexes.Add(3);

            if (correctIndexes.Count == 0)
            {
                MessageBox.Show("Please select at least one correct answer.");
                return;
            }

            Question question = new Question
            {
                Text = QuestionTextBox.Text,
                Options = options,
                CorrectAnswers = correctIndexes,
                ImagePath = ImagePathTextBox.Text,
                Point = points,
                BonusPoint = bonusPoints
            };

            questions.Add(question);
            UpdateQuestionNumbers();
            QuestionsListBox.Items.Refresh();
            ClearInputs();
        }

        private void UpdateQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQuestion == null)
            {
                MessageBox.Show("Please select a question to update.");
                return;
            }

            if (!int.TryParse(PointsTextBox.Text, out int points) ||
                !int.TryParse(BonusPointsTextBox.Text, out int bonusPoints))
            {
                MessageBox.Show("Please enter valid numbers for points and bonus.");
                return;
            }

            List<string> options = new()
        {
            Answer1.Text,
            Answer2.Text,
            Answer3.Text,
            Answer4.Text
        };

            List<int> correctIndexes = new();
            if (Correct1.IsChecked == true) correctIndexes.Add(0);
            if (Correct2.IsChecked == true) correctIndexes.Add(1);
            if (Correct3.IsChecked == true) correctIndexes.Add(2);
            if (Correct4.IsChecked == true) correctIndexes.Add(3);

            selectedQuestion.Text = QuestionTextBox.Text;
            selectedQuestion.Options = options;
            selectedQuestion.CorrectAnswers = correctIndexes;
            selectedQuestion.ImagePath = ImagePathTextBox.Text;
            selectedQuestion.Point = points;
            selectedQuestion.BonusPoint = bonusPoints;

            UpdateQuestionNumbers();
            QuestionsListBox.Items.Refresh();
            ClearInputs();
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsListBox.SelectedItem is Question selected)
            {
                questions.Remove(selected);
                UpdateQuestionNumbers();
                QuestionsListBox.Items.Refresh();
                ClearInputs();
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

                try
                {
                    if (!string.IsNullOrEmpty(selected.ImagePath))
                        PreviewImage.Source = new BitmapImage(new Uri(selected.ImagePath));
                    else
                        PreviewImage.Source = null;
                }
                catch
                {
                    PreviewImage.Source = null;
                }
            }
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = dialog.FileName;
                PreviewImage.Source = new BitmapImage(new Uri(dialog.FileName));
            }
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
            PointsTextBox.Text = "";
            BonusPointsTextBox.Text = "";
            selectedQuestion = null;
            PreviewImage.Source = null;
        }

        private void UpdateQuestionNumbers()
        {
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].DisplayText = $"{i + 1}. {questions[i].Text}";
            }
        }
    }
}


