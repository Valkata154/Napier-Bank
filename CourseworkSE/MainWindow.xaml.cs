/*
 * Usage: MainWindow used to process the messages, load files, add output to a json file.
 * Created by: Valeri Vladimirov 40399682
 * Last modified: 15.11.2020
 */
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CourseworkSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Shutdown app when closing window
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Shutdown();
        }

        // Prepare variables
        public ProcessedMessage win = new ProcessedMessage();
        ListsWindow win2 = new ListsWindow();
        Dictionary<string, int> hashtagsDictionary = new Dictionary<string, int>();
        Dictionary<string, string> sirDictionary = new Dictionary<string, string>();
        List<string> quarantineList = new List<string>();
        List<string> mentionsList = new List<string>();
        List<string> loadedFile = new List<string>();
        List<Message> outputList = new List<Message>();

        // Opens the ProcessedMessage Window
        private void Open_Processed_Message_Window(object sender, RoutedEventArgs e)
        {
            win.Show();
        }

        // Opens the ListsWindow
        private void Open_Lists_Window(object sender, RoutedEventArgs e)
        {
            win2.Show();
        }

        // Method for the process button.
        private void Process_Message(object sender, RoutedEventArgs e)
        {
            Message message = new Message();

            // Checks if body and header are empty.
            if (bodyTextBox.Text.Length > 0 && headerTextBox.Text.Length > 0)
            {
                string header = headerTextBox.Text.ToUpper();
                string body = bodyTextBox.Text;

                // Checks if header starts with S, E or T.
                if(header.StartsWith("S") || header.StartsWith("E") || header.StartsWith("T"))
                {
                    // Checks for the header length.
                    if(header.Length == 10)
                    {
                        message.Header = header;
                        message.Body = body;

                        char startingLetter = header[0];

                        // Depending on the starting letter calls the appropriate method.
                        switch (startingLetter)
                        {
                            case 'S':
                                Sms_Process(message);
                                break;
                            case 'E':
                                Email_Process(message);
                                break;
                            case 'T':
                                Tweet_Process(message);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Header must be 10 characters long.");
                    }
                }
                else
                {
                    MessageBox.Show("Header must start with S, E or T.");
                }
            }
            else
            {
                MessageBox.Show("Header and Body can't be left empty.");
            }
        }

        // Method to process Tweet messages
        public void Tweet_Process(Message message)
        {
            Tweet tweet = new Tweet();
            tweet.Header = message.Header;
            tweet.Body = message.Body;
            tweet.Sender = tweet.Body.Split(' ')[0];

            // Remove the sender
            int length = message.Body.IndexOf(" ") + 1;
            string text = message.Body.Substring(length);
            tweet.Text = text;

            foreach (string word in tweet.Text.Split(' '))
            {
                // Mentions
                if (word.StartsWith("@"))
                {
                    string newWord = word;
                    if (word.EndsWith(",") || word.EndsWith(".") || word.EndsWith("!") || word.EndsWith(":") || word.EndsWith(";") || word.EndsWith("?"))
                        newWord = word.Remove(word.Length - 1);
                    if (!mentionsList.Contains(newWord))
                    {
                        mentionsList.Add(newWord);
                        win2.mentionsList.Items.Add(newWord);
                    }
                }

                // Hashtags
                if (word.StartsWith("#"))
                {
                    string newWord = word;
                    if (word.EndsWith(",") || word.EndsWith(".") || word.EndsWith("!") || word.EndsWith(":") || word.EndsWith(";") || word.EndsWith("?"))
                        newWord = word.Remove(word.Length - 1);
                    if (hashtagsDictionary.ContainsKey(newWord))
                        hashtagsDictionary[newWord]++;
                    else
                        hashtagsDictionary.Add(newWord, 1);
                }
            }

            // Add the abbreviations
            tweet.Text = Expand_Abbreviations(tweet.Text);
            outputList.Add(tweet);

            // Clear the old and set the new values for the ProcessedMessage Window 
            win.subjectTextBox.Visibility = Visibility.Hidden;
            win.subjectLabel.Visibility = Visibility.Hidden;
            win.bodyTextBox.Clear();
            win.messageTextBox.Clear();
            win.senderTextBox.Clear();
            win.bodyTextBox.Text += tweet.Text;
            win.senderTextBox.Text += tweet.Sender;
            win.messageTextBox.Text += tweet.Header;


            // Save the list in json file.
            Save s = new Save();
            s.Serialize(outputList);

            // Trending
            win2.trendingList.Items.Clear();
            foreach (var item in hashtagsDictionary)
                win2.trendingList.Items.Add(item);
        }

        // Method to process SMS messages
        public void Sms_Process(Message message)
        {
            Sms sms = new Sms();
            sms.Header = message.Header;
            sms.Body = message.Body;
            sms.Sender = sms.Body.Split(' ')[0];

            // Remove the phone number and add the abbreviations
            int length = sms.Body.IndexOf(" ") + 1;
            string text = sms.Body.Substring(length);
            sms.Text = Expand_Abbreviations(text);
            outputList.Add(sms);

            // Clear the old and set the new values for the ProcessedMessage Window 
            win.subjectTextBox.Visibility = Visibility.Hidden;
            win.subjectLabel.Visibility = Visibility.Hidden;
            win.bodyTextBox.Clear();
            win.messageTextBox.Clear();
            win.senderTextBox.Clear();
            win.bodyTextBox.Text += sms.Text;
            win.senderTextBox.Text += sms.Sender;
            win.messageTextBox.Text += sms.Header;

            // Save the list in json file.
            Save s = new Save();
            s.Serialize(outputList);
        }

        // Method to process Email messages
        public void Email_Process(Message message)
        {
            Email email = new Email();
            email.Header = message.Header;
            email.Body = message.Body;
            email.Sender = message.Body.Split(' ')[0];

            List<string> incidents = new List<string>() { "Staff Attack", "ATM Theft", "Theft", "Raid", "Customer Attack", "Staff Abuse", "Bomb Threat", "Terrorism", "Suspicious Incident", "Intelligence", "Cash Loss" };

            // Significant Incident Reports use the following pattern:
            // Email SIR dd/mm/yy SortCode Incident Text
            // Examples:
            // valkata154@gmail.com SIR 27/10/20 11-22-33 Staff Attack Hello my name is Valeri.
            // 40399682@live.napier.ac.uk SIR 28/10/20 46-55-66 ATM Theft Hello www.facebook.com I like vegetables from http:\\www.veg.bg
            if (email.Body.Contains("SIR"))
            {
                email.Subject = message.Body.Split(' ')[1] + " " + message.Body.Split(' ')[2];
                email.Text = message.Body.Substring(email.Sender.Length + email.Subject.Length + 2);
                foreach (string incident in incidents)
                {
                    if (email.Text.Contains(incident))
                    {
                        sirDictionary.Add(email.Text.Split(' ')[0], incident);
                        win2.sirList.Items.Add(email.Text.Split(' ')[0] + ", " + incident);
                        break;
                    }
                }
            }
            // Standard email messages follow the following pattern:
            // Email Subject(20char). Body
            // Examples:
            // valkata154@gmail.com Hello I'm Valeri. This is my text.
            // 40399682@live.napier.ac.uk This is my subject. Hello www.facebook.com I like vegetables from http:\\www.veg.bg
            else
            {
                email.Subject = message.Body.Substring(email.Sender.Length + 1).Split('.')[0];
                email.Text = message.Body.Substring(email.Sender.Length + email.Subject.Length + 2).TrimStart(' ');
            }

            // Logic to search for URL's and add them to the quarantine list
            foreach (string word in email.Text.Split(' '))
            {
                if (word.Contains("www") || word.Contains("http:") || word.Contains("https:"))
                {
                    email.Text = email.Text.Replace(word, "<URL Quarantined>");
                    if (!quarantineList.Contains(word))
                    {
                        quarantineList.Add(word);
                        win2.quarantineList.Items.Add(word);
                    }
                    
                }
            }
            outputList.Add(email);

            // Clear the old and set the new values for the ProcessedMessage Window 
            win.subjectTextBox.Visibility = Visibility.Visible;
            win.subjectLabel.Visibility = Visibility.Visible;
            win.bodyTextBox.Clear();
            win.messageTextBox.Clear();
            win.senderTextBox.Clear();
            win.subjectTextBox.Clear();
            win.bodyTextBox.Text += email.Text;
            win.senderTextBox.Text += email.Sender;
            win.messageTextBox.Text += email.Header;
            win.subjectTextBox.Text += email.Subject;

            // Save the list in json file.
            Save s = new Save();
            s.Serialize(outputList);
        }

        // Method used to open the dialog for the user to pick a file to load.
        private void Load_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            foreach (string line in File.ReadAllLines(openFileDialog.FileName))
            {
                loadedFile.Add(line);
                messagesList.Items.Add(line);
            }
        }

        // Method to show the double clicked message after it has been processed
        private void Message_Selection(object sender, RoutedEventArgs e)
        {
            string s = messagesList.SelectedItem.ToString();
            Message message = new Message();
            message.Header = s.Split(' ')[0];
            message.Body = s.Substring(message.Header.Length + 1);
            switch (s[0])
            {
                case 'S':
                    Sms_Process(message);
                    break;
                case 'E':
                    Email_Process(message);
                    break;
                case 'T':
                    Tweet_Process(message);
                    break;
            }
            win.Show();
        }

        // Method to expand the abbreviations.
        private string Expand_Abbreviations(string messageText)
        {
            List<string> abbreviationList = new List<string>();
            List<string> definitionList = new List<string>();

            // Read the textwords file to get the abbreviations.
            using (var reader = new StreamReader(@"../../textwords.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var textwords = reader.ReadLine().Split(new[] { ',' }, 2);
                    abbreviationList.Add(textwords[0]);
                    definitionList.Add(textwords[1]);
                }
            }

            // Abbreviation replacement logic.
            foreach (string word in messageText.Split(' '))
            {
                foreach (string abbrev in abbreviationList)
                {
                    string newWord = word;
                    if(word.EndsWith(",") || word.EndsWith(".") || word.EndsWith("!") || word.EndsWith(":") || word.EndsWith(";") || word.EndsWith("?"))
                    {
                        newWord = word.Remove(word.Length - 1); 
                    }
                    if (newWord.Equals(abbrev))
                    {
                        // Add the word + definition 
                        string sentence = word + " <" + definitionList[abbreviationList.IndexOf(abbrev)] + ">";
                        messageText = (messageText).Replace(word, sentence);
                    }
                }
            }
            return messageText;
        }
    }
}
