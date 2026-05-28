using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POE_P2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        // FIX 1: Single set of fields (removed duplicate declarations)
        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();
        User_Name check_name = new User_Name();

        string username = string.Empty;
        string pre_question = string.Empty;
        int counting = 0;

        // FIX 2: Single constructor (was duplicated before)
        public MainWindow()
        {
            InitializeComponent();

            // FIX 3: Correct class name is "Responses", not "respond"
            new Responses(reply, ignore);

            // FIX 4: "voice_greeting" does not exist — using the correct "Greeting" class
            Greeting greet = new Greeting();
            greet.greet();
        }

        // proceed event handler
        private void proceed(object sender, RoutedEventArgs e)
        {
            // FIX 5: home_grid now exists in XAML (was missing before)
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }

        // submit name event handler
        private void submit_name(object sender, RoutedEventArgs e)
        {
            username = check_name.submit_name(usernames_input, chats);
            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;
        }

        // send event handler
        private void send(object sender, RoutedEventArgs e)
        {
            string rawQuestion = question.Text.ToString().Trim();

            if (string.IsNullOrWhiteSpace(rawQuestion))
            {
                error_method("ChatBot", "Please enter a question.");
                return;
            }

            string questions = RemoveSpecialCharacters(rawQuestion);

            error_method(username, rawQuestion);

            auto_show_interest();
            ai_check(questions);
        }

        // ai_check method
        private void ai_check(string questions)
        {
            if (string.IsNullOrWhiteSpace(questions))
            {
                error_method("ChatBot", "Please enter a valid question.");
                question.Clear();
                return;
            }

            string[] words = questions.ToLower().Split(
                new char[] { ' ', ',', '.', '?', '!', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries);

            bool found = false;
            string message = string.Empty;
            Random indexer = new Random();
            List<string> per_word = new List<string>();
            List<string> answers_found = new List<string>();

            foreach (string word in words)
            {
                if (word.Length < 3 || ignore.Contains(word.ToLower()))
                    continue;

                per_word.Clear();

                // interests handling
                if (word.Contains("interested"))
                {
                    string store_interests = string.Empty;
                    bool found_interest = false;
                    HashSet<string> currentInterests = new HashSet<string>();

                    foreach (string interest in words)
                    {
                        string clean = interest.ToLower().Trim();
                        clean = Regex.Replace(clean, @"[^a-zA-Z0-9\s]", "");

                        if (!ignore.Contains(clean) && clean != "interested" &&
                            clean != "and" && clean != "in" && clean.Length >= 3)
                        {
                            found_interest = true;
                            currentInterests.Add(clean);
                        }
                    }

                    store_interests = string.Join(", ", currentInterests);

                    if (found_interest && !string.IsNullOrWhiteSpace(store_interests)) ;
                    else
                    {
                        message += "Please specify what you're interested in (e.g., 'I am interested in cybersecurity')";
                    }
                }

                // search for matching answers
                bool wordFound = false;
                foreach (string answer in reply)
                {
                    if (answer.ToLower().Contains(word))
                    {
                        wordFound = true;
                        per_word.Add(answer);
                    }
                }

                if (wordFound && per_word.Count > 0)
                {
                    found = true;
                    int indexing = indexer.Next(0, per_word.Count);
                    answers_found.Add(per_word[indexing]);
                }
            }

            if (found && answers_found.Count > 0) ;
            else
            {
                string[] fallbackMessages = {
                    "I'm sorry, I don't understand that. Could you rephrase your question?",
                    "I didn't quite get that. Try asking about cyber security topics.",
                    "Hmm, I'm not sure how to respond to that. Can you ask something else?",
                    "I couldn't find an answer for that. Please ask about programming, security, or technology.",
                    "My apologies, I don't have information on that topic yet."
                };
                Random random = new Random();
                error_method("ChatBot", fallbackMessages[random.Next(fallbackMessages.Length)]);
            }

            question.Clear();
        }

        // remove special characters
        private string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            StringBuilder sanitized = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '\'' || c == '-')
                    sanitized.Append(c);
                else
                    sanitized.Append(' ');
            }

            string result = sanitized.ToString();
            result = Regex.Replace(result, @"\s+", " ").Trim();
            return result;
        }

        // auto show interest every 3 messages
        private void auto_show_interest()
        {
            if (counting == 3)
            {
                string filename = "interested_topic.txt";
                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(username))
                        {
                            int colonIndex = line.IndexOf("interested in:");
                            if (colonIndex >= 0)
                            {
                                string interests = line.Substring(colonIndex + 14).Trim();
                                error_method("ChatBot", "Just a reminder, you are interested in " + interests + " and ");
                                ai_check(interests);
                                break;
                            }
                        }
                    }
                }
                counting = 0;
            }
            else
            {
                counting += 1;
            }
        }

        // error/display method
        private void error_method(string name, string message)
        {
            Border messageBorder = new Border
            {
                Margin = new Thickness(0, 2, 0, 2),
                Padding = new Thickness(5, 3, 5, 3),
                CornerRadius = new CornerRadius(5)
            };

            if (name.ToLower().Contains("chatbot") || name.ToLower().Contains("chat"))
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(173, 216, 230));
            }
            else
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(211, 211, 211));
            }

            messageBorder.BorderThickness = new Thickness(1);

            TextBlock messageText = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(2) };

            Brush nameColor = (name.ToLower().Contains("chatbot") || name.ToLower().Contains("chat"))
                ? Brushes.DarkBlue : Brushes.DarkGreen;

            messageText.Inlines.Add(new Run { Text = name + ": ", Foreground = nameColor, FontWeight = FontWeights.Bold });
            messageText.Inlines.Add(new Run { Text = message, Foreground = Brushes.Black });

            messageBorder.Child = messageText;
            chats.Items.Add(messageBorder);
            chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }




    }//end of class
}// end of namespace
