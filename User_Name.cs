using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace POE_P2
{
    public class User_Name
    {


        public string submit_name(TextBox user_name, ListView chats)
        {
            string filename = "user_names.txt";

            if (!File.Exists(filename))
            {
                File.AppendAllText(filename, "auto_create\n");
            }

            string name = user_name.Text.ToString();
            bool found = check_name(name);

            if (!found)
            {
                File.AppendAllText(filename, name + "\n");
                error_method("ChatBot", "Hey " + name + " welcome to AI cybersecurity, How can i help you with today", chats);
            }
            else
            {
                error_method("ChatBot", "Hey " + name + " welcome back, how can i help you today", chats);
            }

            return name;
        }

        private Boolean check_name(string name)
        {
            string filename = "user_names.txt";
            bool found_name = false;

            string[] names = File.ReadAllLines(filename);

            foreach (string name_found in names)
            {
                if (name_found.ToLower() == name.ToLower())
                {
                    found_name = true;
                }
            }

            return found_name;
        }

        private void error_method(string name, string message, ListView chats)
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
        }




    }//end of class
}//end of namespace