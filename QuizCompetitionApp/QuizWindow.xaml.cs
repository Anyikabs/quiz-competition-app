using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QuizCompetitionApp
{
    public partial class QuizWindow : Window
    {
        private List<Question> questions = new();
        private int currentQuestionIndex = 0;
        private List<Button> answerButtons;
        private DispatcherTimer timer = new();
        private int timeRemaining = 30;

        public QuizWindow()
        {
            InitializeComponent();

            // Initialize answer buttons
            answerButtons = new List<Button> { AnswerButton1, AnswerButton2, AnswerButton3, AnswerButton4 };

            foreach (var button in answerButtons)
            {
                button.Click += AnswerButton_Click;
            }

            // Set up timer
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            LoadQuestions();
            DisplayCurrentQuestion();
        }

        private void LoadQuestions()
        {
            string filePath = "questions.json";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var loadedQuestions = JsonSerializer.Deserialize<List<Question>>(json);
                if (loadedQuestions != null)
                {
                    questions = loadedQuestions;
                }
            }
            else
            {
                MessageBox.Show("Questions file not found.");
                Close();
            }
        }

        private void DisplayCurrentQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                MessageBox.Show("Quiz finished!");
                Close();
                return;
            }

            var q = questions[currentQuestionIndex];

            QuestionTextBlock.Text = q.Text;

            if (!string.IsNullOrEmpty(q.ImagePath) && File.Exists(q.ImagePath))
            {
                QuestionImage.Source = new BitmapImage(new Uri(q.ImagePath, UriKind.RelativeOrAbsolute));
                QuestionImage.Visibility = Visibility.Visible;
            }
            else
            {
                QuestionImage.Visibility = Visibility.Collapsed;
            }

            for (int i = 0; i < answerButtons.Count; i++)
            {
                answerButtons[i].Content = i < q.Options.Count ? q.Options[i] : "";
                answerButtons[i].IsEnabled = true;
                answerButtons[i].Background = null;
            }

            timeRemaining = 30;
            TimerTextBlock.Text = $"Time: {timeRemaining}";
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)

        {
            timeRemaining--;
            TimerTextBlock.Text = $"Time: {timeRemaining}";

            if (timeRemaining <= 0)
            {
                timer.Stop();
                MessageBox.Show("Time's up!");
                MoveToNextQuestion();
            }
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Button clickedButton = (Button)sender;
            int selectedIndex = answerButtons.IndexOf(clickedButton);

            var q = questions[currentQuestionIndex];
            bool isCorrect = q.CorrectAnswers.Contains(selectedIndex);

            clickedButton.Background = isCorrect ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.IndianRed;

            foreach (var btn in answerButtons)
            {
                btn.IsEnabled = false;
            }

            MessageBox.Show(isCorrect ? "Correct!" : "Wrong!");
            MoveToNextQuestion();
        }

        private void MoveToNextQuestion()
        {
            currentQuestionIndex++;
            DisplayCurrentQuestion();
        }
    }


}
