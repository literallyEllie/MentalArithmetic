using System;
using System.Timers;

namespace EllieLib
{
    class EasyTimer
    {

        public enum TimerType { Up, Down }

        private Timer timer;
        private TimerType timerType;
        private int interval, initialValue, targetValue = -1;

        public int Value { set; get; }

        public delegate void ValueReachedTargetEvent(object sender, EventArgs e);
        public event ValueReachedTargetEvent TargetReachedEvent;


        public EasyTimer(TimerType timerType, int interval, int initialValue, int targetValue)
        {
            this.timerType = timerType;
            this.interval = interval;

            if (initialValue >= 0)
            {
                this.initialValue = initialValue;
                this.Value = initialValue;
            }

            if (targetValue >= 0)
            {
                this.targetValue = targetValue;
            }

            this.timer = new Timer
            {
                Interval = interval
            };
            this.timer.Elapsed += OnTick;
        }

        public EasyTimer(TimerType timerType, int interval) 
            : this(timerType, interval, 0, 0)
        {
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Dispose()
        {
            this.timer.Dispose();
        }

        public void Restart()
        {
            this.Stop();
            this.Value = initialValue;
            this.Start();
        }

        public void Tick(ElapsedEventHandler elapsedEventHandler)
        {
            this.timer.Elapsed += elapsedEventHandler;
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {

            if (timerType == TimerType.Up)
            {
                Value++;
            } else
            {
                Value--;
            }

            if (timerType == TimerType.Down)
            {

            }

            if (targetValue >= 0 && Value == targetValue)
            {
                Console.WriteLine("yeet");
                this.TargetReachedEvent(this, new EventArgs());
                this.Stop();
            }
            
        }

    }

}