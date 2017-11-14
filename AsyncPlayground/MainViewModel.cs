using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncPlayground.Annotations;
using AsyncPlayground.Commands;

namespace AsyncPlayground
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string m_fetchResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand FetchSomethingCommand { get; }

        public string FetchResult
        {
            get { return m_fetchResult; }
            set
            {
                m_fetchResult = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            FetchSomethingCommand = new AwaitableDelegateCommand(FetchSomething);
            FetchResult = "Initial";
        }

        private async Task FetchSomething()
        {
            var timeConsumingTask = DoSomethingThatTakesAWhile();

            int a = DoSomethingQuickMeanwhile();

            var fetchResult = await timeConsumingTask;
            FetchResult = $"fetched {fetchResult}";

        }

        private static int DoSomethingQuickMeanwhile()
        {
            return 1 + 2;
        }

        private async Task<DateTime> DoSomethingThatTakesAWhile()
        {
            await Task.Delay(5000);
            return DateTime.Now;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}