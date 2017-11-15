using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncWpfSample.Commands;
using AsyncWpfSample.Properties;
using AsyncWpfSample.SyncStuff;

namespace AsyncWpfSample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string m_fetchAsyncResult;
        private string m_fetchSyncResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand FetchSomethingAsyncCommand { get; }

        public string FetchAsyncResult
        {
            get { return m_fetchAsyncResult; }
            set
            {
                m_fetchAsyncResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand FetchSomethingSyncCommand { get; }

        public string FetchSyncResult
        {
            get { return m_fetchSyncResult; }
            set
            {
                m_fetchSyncResult = value;
                OnPropertyChanged();
            }
        }

        //private int m_asyncFetchCounter;
        //private int m_syncFetchCounter;

        public MainViewModel()
        {
            //FetchSomethingAsyncCommand = new AwaitableDelegateCommand(FetchSomethingAsync);
            //FetchAsyncResult = "Initial";

            FetchSomethingSyncCommand = new DelegateCommand(DoWebRequestsSync);
            FetchSyncResult = "Initial";

            //m_asyncFetchCounter = 0;
            //m_syncFetchCounter = 0;
        }

        private void DoWebRequestsSync()
        {
            var syncMethods = new SyncMethods();
            // Make a list of web addresses.  
            List<string> urlList = syncMethods.SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetURLContents returns the contents of url as a byte array.  
                byte[] urlContents = syncMethods.GetURLContents(url);

                DisplayResults(url, urlContents);

                // Update the total.  
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.  
            FetchSyncResult +=
                string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
        }

        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each website. The string format   
            // is designed to be used with a monospaced font, such as  
            // Lucida Console or Global Monospace.  
            var bytes = content.Length;
            // Strip off the "http://".  
            var displayURL = url.Replace("http://", "");
            FetchSyncResult += string.Format("\n{0,-58} {1,8}", displayURL, bytes);
        }

        //private async Task FetchSomethingAsync()
        //{
        //    var timeConsumingTask = DoSomethingThatTakesAWhile();

        //    int a = DoSomethingQuickMeanwhile();

        //    var fetchResult = await timeConsumingTask;

        //    m_asyncFetchCounter++;
        //    FetchAsyncResult = $"AsyncFetch {m_asyncFetchCounter} ::  {fetchResult}";

        //}

        //private static int DoSomethingQuickMeanwhile()
        //{
        //    return 1 + 2;
        //}

        //private async Task<TimeSpan> DoSomethingThatTakesAWhile()
        //{
        //    var millisecondsDelay = new Random(DateTime.Now.Millisecond).Next(500, 4000);
        //    await Task.Delay(millisecondsDelay);
        //    return DateTime.Now.TimeOfDay;
        //}

        //private TimeSpan DoSomethingThatTakesAWhileSync()
        //{
        //    var millisecondsDelay = new Random(DateTime.Now.Millisecond).Next(500, 4000);
        //    Thread.Sleep(millisecondsDelay);
        //    return DateTime.Now.TimeOfDay;
        //}

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}