using System;
using System.Timers;

namespace EllieLib
{
    // <summary>Class <c>EasyTimer</c> is an easy wrapper for the <c>Timer</c> class based on events.</summary>
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

            // If initial value is >= 0, use these values.
            // Values under 0 are not accepted.
            if (initialValue >= 0)
            {
                this.initialValue = initialValue;
                this.Value = initialValue;
            }

            // If the target value was >= 0, use it.
            // Since the initial value cannot be below 0, neither can the target.
            if (targetValue >= 0)
            {
                this.targetValue = targetValue;
            }

            // Setup the actual timer object.
            this.timer = new Timer
            {
                Interval = interval,
            };
            // Everytime it elapsed, run the internal OnTick method.
            this.timer.Elapsed += OnTick;
        }

        // <summary>Simplistic timers where tracking is not required.</summary>
        public EasyTimer(TimerType timerType, int interval) 
            : this(timerType, interval, 0, 0)
        {
        }

        // <summary>Starts the timer.</summary>
        public void Start()
        {
            timer.AutoReset = true;
            timer.Start();
        }

        // <summary>Stops the timer.</summary>
        public void Stop()
        {
            timer.Stop();
            timer.AutoReset = false;
        }
        
        // <summary>Returns the timer type of this timer.</c>
        public TimerType GetTimerType()
        {
            return this.timerType;
        }

        // <summary>Gets the initial set value.</<summary>
        public int GetInitialValue()
        {
            return this.initialValue;
        }

        // <summary>Dispose of the timer.</<summary>
        public void Dispose()
        {
            this.timer.Dispose();
        }

        // <summary>Restarts the timer</summary>
        public void Restart()
        {
            this.Stop();
            this.Value = initialValue;
            this.Start();
        }

        // <summary>Allows for setting an listener for everytime it ticks</<summary>
        public void Tick(ElapsedEventHandler elapsedEventHandler)
        {
            this.timer.Elapsed += elapsedEventHandler;
        }

        // <summary>Internal tick handle</<summary>
        private void OnTick(object sender, ElapsedEventArgs e)
        {

            // Increment or decrement the value depending on the type.
            if (timerType == TimerType.Up)
            {
                Value++;
            } else
            {
                Value--;
            }

            // If the target value was >= 0 to begin with and the current value is equal to the target.
            // Then the condition is met.
            if (targetValue >= 0 && Value == targetValue)
            {
                // Call event.
                this.TargetReachedEvent(this, new EventArgs());
                // Stop timer.
                this.Stop();
            }
            
        }
        

    }

}