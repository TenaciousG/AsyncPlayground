using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AsyncWpfSample.AsyncStuff;
using AsyncWpfSample.Commands;
using AsyncWpfSample.Properties;
using AsyncWpfSample.SyncStuff;

namespace AsyncWpfSample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string m_fetchAsyncResult;
        private string m_fetchSyncResult;
        private NotifyTaskCompletion<string> m_setupAsyncLoadedResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand FetchSomethingAsyncCommand { get; }

        public ICommand FetchSomethingSyncCommand { get; }

        public MainViewModel()
        {
            ThrowIfNotUiThread();
            FetchSomethingAsyncCommand = new AwaitableDelegateCommand(DoWebRequestsAsync);
            FetchAsyncResult = "Initial";

            FetchSomethingSyncCommand = new DelegateCommand(DoWebRequestsSync);
            FetchSyncResult = "Initial";

            //Laster data asynkront uten å blokke UI-tråden - og sender propertyChanged når den er ferdig
            SetupAsyncLoadedResult = new NotifyTaskCompletion<string>(LoadSomeDataAsync());
        }

        private async Task<string> LoadSomeDataAsync()
        {
            ThrowIfNotUiThread();
            var asyncMethods = new AsyncMethods();

            var urlResult = await asyncMethods.GetURLContentsAsync(@"http://msdn.microsoft.com");


            ThrowIfNotUiThread();
            var urlLength = urlResult.Length.ToString();
            return urlLength;
        }

        private static void ThrowIfNotUiThread()
        {
            if (!IsOnUiThread())
            {
                throw new Exception("Not on UI thread!");
            }
        }

        private static bool IsOnUiThread()
        {
            return Application.Current.Dispatcher.CheckAccess();
        }
        
        private async Task DoWebRequestsAsync()
        {
            ThrowIfNotUiThread();
            var asyncMethods = new AsyncMethods();
            // Make a list of web addresses.  
            List<string> urlList = asyncMethods.SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetURLContents returns the contents of url as a byte array.  
                var urlContents = await asyncMethods.GetURLContentsAsync(url);

                DisplayAsyncResults(url, urlContents);

                // Update the total.  
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.  
            FetchSyncResult +=
                string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
        }

        private void DoWebRequestsSync()
        {
            ThrowIfNotUiThread();
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
            ThrowIfNotUiThread();
            // Display the length of each website. The string format   
            // is designed to be used with a monospaced font, such as  
            // Lucida Console or Global Monospace.  
            var bytes = content.Length;
            // Strip off the "http://".  
            var displayURL = url.Replace("http://", "");
            FetchSyncResult += string.Format("\n{0,-58} {1,8}", displayURL, bytes);
        }

        private void DisplayAsyncResults(string url, byte[] content)
        {
            ThrowIfNotUiThread();
            // Display the length of each website. The string format   
            // is designed to be used with a monospaced font, such as  
            // Lucida Console or Global Monospace.  
            var bytes = content.Length;
            // Strip off the "http://".  
            var displayURL = url.Replace("http://", "");
            FetchAsyncResult += string.Format("\n{0,-58} {1,8}", displayURL, bytes);
        }

        public NotifyTaskCompletion<string> SetupAsyncLoadedResult
        {
            get { return m_setupAsyncLoadedResult; }
            set
            {
                m_setupAsyncLoadedResult = value;
                OnPropertyChanged();
            }
        }

        public string FetchAsyncResult
        {
            get { return m_fetchAsyncResult; }
            set
            {
                m_fetchAsyncResult = value;
                OnPropertyChanged();
            }
        }

        public string FetchSyncResult
        {
            get { return m_fetchSyncResult; }
            set
            {
                m_fetchSyncResult = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}