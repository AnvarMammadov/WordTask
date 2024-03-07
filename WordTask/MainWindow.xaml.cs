using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WordTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool _stopRequested = false;
        private bool _paused = false;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void wordTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string wordToAdd = wordTxt.Text;
                Thread threadAddWord = new Thread(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AddWordToListBox(wordToAdd);
                    });
                });
                threadAddWord.Start();
                wordTxt.Clear();
            }
        }

        private void AddWordToListBox(string word)
        {
            wordListBox.Items.Add(word);
        }


        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> words = new List<string>();
            foreach (var item in wordListBox.Items)
            {
                words.Add(item.ToString());
            }

            Thread threadEncrypt = new Thread(() =>
            {
                foreach (var word in words)
                {
                    if (_stopRequested)
                        break;

                    while (_paused)
                    {
                        Thread.Sleep(100);
                    }

                    var encryptedWord = EncryptWithGUID(word);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        wordListBox.Items.Remove(word);
                        guidWordsTxt.Items.Insert(0, encryptedWord);
                    });

                    Thread.Sleep(2000);
                }
            });
            threadEncrypt.Start();
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            _paused = true;
        }

        private void resumeBtn_Click(object sender, RoutedEventArgs e)
        {
            _paused = false;
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            _stopRequested = true;
            MessageBox.Show("Window is closing", "Process Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }


        private string EncryptWithGUID(string word)
        {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString();
            string encryptedWord = "";
            foreach (char letter in guidString)
            {
                if (letter != '-')
                {
                    encryptedWord += "*";
                }
            }
            return encryptedWord;
        }
    }
}
