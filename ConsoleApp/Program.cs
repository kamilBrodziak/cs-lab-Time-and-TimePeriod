using System;
using TimeLibrary;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {
            Clock clock = new Clock();
            ConsoleKey key;
            int i = 0;
            Console.Clear();
            PrintMenu(i);
            while((key = Console.ReadKey().Key ) != ConsoleKey.Escape) {
                if(key == ConsoleKey.DownArrow && i < 4) {
                    i++;
                } else if(key == ConsoleKey.UpArrow && i > 0) {
                    i--;
                } else if(key == ConsoleKey.Enter) {
                    Console.Clear();
                    switch(i) {
                        case 0:
                            clock.StartClock();
                            break;
                        case 1:
                            clock.StartStopWatch();
                            break;
                        case 2:
                            UserTimeInput();
                            break;
                        case 3:
                            UserTimePeriodInput();
                            break;
                        case 4:
                            return;
                    }
                    Console.Clear();
                    i = 0;
                }
                PrintMenu(i);
            }
        }

        private static void UserTimeInput() {
            Console.WriteLine("Input time in format:");
            Console.WriteLine("1. 'H' or 'HH'");
            Console.WriteLine("2. 'HH:MM' or 'HH:M' or 'H:MM' or 'H:M'");
            Console.WriteLine("3. 'HH:MM:SS' or 'HH:M:SS' or 'HH:MM:S' or 'HH:M:S' or 'H:MM:SS' and so on");
            Console.WriteLine("4. 3 + .S or .SS or .SSS for miliseconds for example 'HH:MM:SS.SSS'");
            string input = Console.ReadLine();
            try {
                Time time = new Time(input);
                Console.WriteLine($"Hour: {time.Hours}");
                Console.WriteLine($"Minutes: {time.Minutes}");
                Console.WriteLine($"Seconds: {time.Seconds}");
                Console.WriteLine($"Miliseconds: {time.MiliSeconds}");
            } catch(ArgumentException e) {
                Console.WriteLine("Wrong input, try again later.");
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void UserTimePeriodInput() {
            Console.WriteLine("Input number of seconds");
            try {
                long n = long.Parse(Console.ReadLine());
                TimePeriod time = new TimePeriod(n, TimePeriod.TimeUnit.Second);
                Console.WriteLine($"Time elapsed: {time}");
            } catch(Exception e) {
                Console.WriteLine("Wrong input, try again later.");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void PrintMenu(int y) {
            void replaceLine(int x, int y, string line) {
                Console.SetCursorPosition(x, y);
                Console.Write(line);
                Console.SetCursorPosition(0, y + 1);
            } 

            string[] options = new string[] {
                "1. Start clock",
                "2. Start timer",
                "3. Convert time string to time object",
                "4. Convert number of seconds to time period string",
                "5. Quit"
            };
            int i = 0;
            foreach(string option in options) {
                replaceLine(0, i, option + (i++ == y ? " <--------------------" : "                      "));
            }
        }
    }
}
