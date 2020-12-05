using System;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {
            Clock clock = new Clock();
            ConsoleKey key;
            int i = 0;
            Console.Clear();
            PrintMenu(i);
            while((key = Console.ReadKey().Key ) != ConsoleKey.Escape) {
                if(key == ConsoleKey.DownArrow && i < 2) {
                    i++;
                } else if(key == ConsoleKey.UpArrow && i > 0) {
                    i--;
                } else if(key == ConsoleKey.Enter) {
                    Console.Clear();
                    if(i == 2)
                        return;
                    if(i == 0) {
                        clock.StartClock();
                    }
                    if(i == 1) {
                        clock.StartStopWatch();
                    }
                    i = 0;
                }
                PrintMenu(i);
            }
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
                "3. Quit"
            };
            int i = 0;
            foreach(string option in options) {
                replaceLine(0, i, option + (i++ == y ? " <--------------------" : "                      "));
            }
        }
    }
}
