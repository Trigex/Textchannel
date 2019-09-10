using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TextbinChannel.Services
{
    public enum ToastLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    /// <summary>
    /// Service enabling the spawning of Toast notifications
    /// </summary>
    public class ToastService : IDisposable
    {
        public Timer Countdown;
        public event Action<string, ToastLevel> OnShow;
        public event Action OnHide;

        public void ShowToast(string message, ToastLevel level)
        {
            OnShow?.Invoke(message, level);
            StartCountdown();
        }

        private void HideToast(object source, ElapsedEventArgs args)
        {
            OnHide?.Invoke();
        }

        private void StartCountdown()
        {
            SetCountdown();

            if(Countdown.Enabled)
            {
                Countdown.Stop();
                Countdown.Start();
            } else
            {
                Countdown.Start();
            }
        }

        private void SetCountdown()
        {
            if(Countdown == null)
            {
                Countdown = new Timer(2000);
                Countdown.Elapsed += HideToast;
                Countdown.AutoReset = false;
            }
        }

        public void Dispose()
        {
            Countdown?.Dispose();
        }
    }
}
