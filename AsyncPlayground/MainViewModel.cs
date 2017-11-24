using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncPlayground.Annotations;
using AsyncPlayground.Commands;

namespace AsyncPlayground
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string m_fetchAsyncResult;
        private string m_fetchSyncResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand FetchSomethingAsyncCommand { get; }
        public ICommand FetchSomethingSyncCommand { get; }

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

        private int m_asyncFetchCounter;
        private int m_syncFetchCounter;

        public MainViewModel()
        {
            FetchSomethingAsyncCommand = new AwaitableDelegateCommand(FetchSomethingAsync);
            FetchAsyncResult = "Initial";

            FetchSomethingSyncCommand = new DelegateCommand(FetchSomethingSync);
            FetchSyncResult = "Initial";

            m_asyncFetchCounter = 0;
            m_syncFetchCounter = 0;
        }

        private void FetchSomethingSync()
        {
            var fetchResult = DoSomethingThatTakesAWhileSync();

            m_syncFetchCounter++;

            FetchSyncResult = $"SyncFetch {m_syncFetchCounter} :: {fetchResult}";
        }

       private async Task FetchSomethingAsync()
        {
            var timeConsumingTask = DoSomethingThatTakesAWhile();

            int a = DoSomethingQuickMeanwhile();

            var fetchResult = await timeConsumingTask;

            m_asyncFetchCounter++;
            FetchAsyncResult = $"AsyncFetch {m_asyncFetchCounter} ::  {fetchResult}";

        }

        private static int DoSomethingQuickMeanwhile()
        {
            return 1 + 2;
        }

        private async Task<TimeSpan> DoSomethingThatTakesAWhile()
        {
            var millisecondsDelay = new Random(DateTime.Now.Millisecond).Next(500, 4000);
            await Task.Delay(millisecondsDelay);
            return DateTime.Now.TimeOfDay;
        }

        private TimeSpan DoSomethingThatTakesAWhileSync()
        {
            var millisecondsDelay = new Random(DateTime.Now.Millisecond).Next(500, 4000);
            Thread.Sleep(millisecondsDelay);
            return DateTime.Now.TimeOfDay;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}