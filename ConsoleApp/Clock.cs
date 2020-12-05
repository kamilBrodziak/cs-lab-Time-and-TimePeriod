using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using TimeLibrary;

namespace ConsoleApp {
    class Clock {

        public void StartClock() {
            var currentTime = DateTime.Now;
            int second = currentTime.Second, minute = currentTime.Minute, hour = currentTime.Hour;
            Time time = new Time((byte)hour, (byte)minute, (byte)second);
            TimePeriod secondT = new TimePeriod(1, TimePeriod.TimeUnit.Second);
            Console.WriteLine($"Current time: {time}");
            Console.WriteLine($"Press any key to quit.");
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (Object source, ElapsedEventArgs e) => {
                time += secondT;
                ReplaceTime(14, 0, time.ToString());
            };
            timer.Start();
            timer.AutoReset = true;
            Console.WriteLine();
            Console.ReadKey();
            timer.Stop();
            Console.Clear();
        }

        public void StartStopWatch() {
            TimePeriod time = new TimePeriod(0);
            TimePeriod milisecond = new TimePeriod(1, TimePeriod.TimeUnit.Milisecond);
            Console.WriteLine($"Stopwatch: {time.ToString(true)}");
            Console.WriteLine($"Press ENTER to quit.");
            Console.WriteLine($"Press SPACE to start/stop.");
            Console.WriteLine($"It's slow because (I think) generating new TimePeriod object by + operator consume lots of time.");
            var timer = new System.Timers.Timer(1);
            timer.Elapsed += (Object source, ElapsedEventArgs e) => {
                time += milisecond;
                ReplaceTime(11, 0, time.ToString(true));
            };

            bool timerEnabled = false;
            timer.AutoReset = true;
            ConsoleKey key;
            while((key = Console.ReadKey().Key) != ConsoleKey.Enter) {
                if(key == ConsoleKey.Spacebar) {
                    if(!timerEnabled) {
                        timer.Start();
                        timerEnabled = true;
                    } else {
                        timer.Stop();
                        timerEnabled = false;
                    }
                }
            }
            timer.Stop();
            Console.Clear();
        }

        private void ReplaceTime(int x, int y, string time) {
            int curTop = Console.CursorTop;
            int curLeft = Console.CursorLeft;
            Console.SetCursorPosition(x, y);
            Console.Write(time);
            Console.SetCursorPosition(curLeft, curTop);
        }
    }
}
