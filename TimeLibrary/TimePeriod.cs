using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace TimeLibrary {
    public struct TimePeriod : IEquatable<TimePeriod>, IComparable<TimePeriod> {
        public enum TimeUnit {
            Milisecond, Second, Minute, Hour
        }
        public readonly long Miliseconds { get; }
        public readonly long Seconds { get => Miliseconds / 1000; }

        public TimePeriod(long hours, byte minutes, byte seconds = 0, short miliseconds = 0) {
            Miliseconds = verifyRange(hours, 0, long.MaxValue) * 3600000
                + verifyRange(minutes, 0, 59) * 60000
                + verifyRange(seconds, 0, 59) * 1000
                + verifyRange(miliseconds, 0, 999);
            long verifyRange(long value, long min, long max) =>
                (value >= min && value <= max) ? value : throw new ArgumentOutOfRangeException();
        }

        public TimePeriod(long miliseconds) {
            if(miliseconds < 0)
                throw new ArgumentOutOfRangeException();
            Miliseconds = miliseconds;
        }

        public TimePeriod(long value, TimeUnit unit) {
            if(value < 0) {
                throw new ArgumentOutOfRangeException();
            }
            switch(unit) {
                case TimeUnit.Hour:
                    Miliseconds = value * 3600000;
                    break;
                case TimeUnit.Minute:
                    Miliseconds = value * 60000;
                    break;
                case TimeUnit.Second:
                    Miliseconds = value * 1000;
                    break;
                case TimeUnit.Milisecond:
                default:
                    Miliseconds = value;
                    break;
            }
        }

        public TimePeriod(string time) {
            // Matches:
            // H+
            // H+:MM or H+:M
            // H+:MM:SS or H+:M:SS or H+:MM:S or H+:M:S
            // Ending with nothing .S or .SS or .SSS when miliseconds
            if(!Regex.IsMatch(time, @"^([1-9]?[0-9]+)((:[0-5]?[0-9]){0,2}|((:[0-5]?[0-9]){2}\.([0]{0,3}|[1-9][0-9]{0,2})))$"))
                throw new ArgumentException();
            string[] timePartsStrings = time.Split(":");
            long hour = 0;
            byte minute = 0, second = 0;
            short miliseconds = 0;
            hour = long.Parse(timePartsStrings[0]);
            if(timePartsStrings.Length > 1) {
                minute = byte.Parse(timePartsStrings[1]);
                if(timePartsStrings.Length == 3) {
                    string[] secondParts = timePartsStrings[2].Split(".");
                    second = byte.Parse(secondParts[0]);
                    if(timePartsStrings.Length == 2) {
                        miliseconds = short.Parse(secondParts[1]);
                    }
                }
            }
            
            Miliseconds = hour * 3600000 + minute * 60000 + second * 1000 + miliseconds;
        }

        public override string ToString() => $"{Miliseconds / 3600000}:{(Miliseconds / 60000) % 60:D2}:{(Miliseconds / 1000) % 60:D2}";

        public string ToString(bool withMiliseconds) => ToString() + (withMiliseconds ? $".{(Miliseconds % 1000):D3}" : "");

        public bool Equals(TimePeriod other) {
            return Miliseconds == other.Miliseconds;
        }

        public override bool Equals(object obj) {
            return obj is TimePeriod && Equals(obj);
        }

        public override int GetHashCode() {
            return Miliseconds.GetHashCode();
        }

        public int CompareTo(TimePeriod other) {
            return Miliseconds.CompareTo(other.Miliseconds);
        }

        public static bool operator <(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) < 0;
        public static bool operator >(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) > 0;
        public static bool operator ==(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) == 0;
        public static bool operator !=(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) != 0;
        public static bool operator <=(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) <= 0;
        public static bool operator >=(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) >= 0;

        public static TimePeriod Plus(TimePeriod t1, TimePeriod t2) => new TimePeriod(t1.Miliseconds + t2.Miliseconds);
        public TimePeriod Plus(TimePeriod other) => Plus(this, other);
        public static TimePeriod operator +(TimePeriod t1, TimePeriod t2) => Plus(t1, t2);

        public static TimePeriod Minus(TimePeriod t1, TimePeriod t2) {
            long miliseconds = t1.Miliseconds - t2.Miliseconds;
            if(miliseconds < 0)
                throw new ArgumentException();
            return new TimePeriod(miliseconds);
        }
        public TimePeriod Minus(TimePeriod other) => Minus(this, other);

        public static TimePeriod operator -(TimePeriod t1, TimePeriod t2) => Minus(t1, t2);


    }
}
