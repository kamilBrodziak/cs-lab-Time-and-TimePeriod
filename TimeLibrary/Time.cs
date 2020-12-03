using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TimeLibrary {
    public struct Time : IEquatable<Time>, IComparable<Time> {
        public readonly byte Hours {get;}
        public readonly byte Minutes { get; }
        public readonly byte Seconds { get; }

        public Time(byte hours, byte minutes = 0, byte seconds = 0) {
            Hours = verifyRange(hours, 0, 23);
            Minutes = verifyRange(minutes, 0, 59);
            Seconds = verifyRange(seconds, 0, 59);

            byte verifyRange(byte value, byte min, byte max) =>
                (value >= min && value <= max) ? value : throw new ArgumentOutOfRangeException();
        }

        public Time(string time) {
            // Matches:
            // HH or H
            // HH:MM or H:M or HH:M or HH:MM
            // HH:MM:SS or H:MM:SS or HH:M:SS and so on..
            if(!Regex.IsMatch(time, @"^([0-1]?[0-9]|2[0-3])(:[0-5]?[0-9]){0,2}$")) 
                throw new ArgumentOutOfRangeException();
            string[] timePartsStrings = time.Split(":");
            byte[] timeParts = new byte[] { 0, 0, 0 };
            for(int i = 0; i < timePartsStrings.Length; ++i) {
                timeParts[i] = byte.Parse(timePartsStrings[i]);
            }
            Hours = timeParts[0];
            Minutes = timeParts[1];
            Seconds = timeParts[2];
        }

        public override string ToString() => $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}";

        public bool Equals(Time other) => Hours == other.Hours &&
            Minutes == other.Minutes && Seconds == other.Seconds;

        public int CompareTo(Time other) {
            int compare = Hours.CompareTo(other.Hours);
            if(compare != 0) return compare;
            compare = Minutes.CompareTo(other.Minutes);
            if(compare != 0) return compare;
            return Seconds.CompareTo(other.Seconds);
        }

        public static bool operator <(Time t1, Time t2) => t1.CompareTo(t2) < 0;
        public static bool operator >(Time t1, Time t2) => t1.CompareTo(t2) > 0;
        public static bool operator ==(Time t1, Time t2) => t1.CompareTo(t2) == 0;
        public static bool operator !=(Time t1, Time t2) => t1.CompareTo(t2) != 0;
        public static bool operator <=(Time t1, Time t2) => t1.CompareTo(t2) <= 0;
        public static bool operator >=(Time t1, Time t2) => t1.CompareTo(t2) >= 0;
    
        public static Time Plus(Time time, TimePeriod timePeriod) {
            long timeSeconds = time.Hours * 3600 + time.Minutes * 60 + time.Seconds;
            timeSeconds += timePeriod.Seconds;
            byte hours = (byte)((timeSeconds / 3600) % 24);
            byte minutes = (byte)((timeSeconds / 60) % 60);
            byte seconds = (byte)(timeSeconds % 60);
            return new Time(hours, minutes, seconds);
        }

        public Time Plus(TimePeriod timePeriod) => Plus(this, timePeriod);
        public static Time operator +(Time time, TimePeriod timePeriod) => Plus(time, timePeriod);

        public static Time Minus(Time time, TimePeriod timePeriod) {
            long timeSeconds = time.Hours * 3600 + time.Minutes * 60 + time.Seconds;
            timeSeconds -= timePeriod.Seconds;
            // If timeSeconds < 0 then get time from day in the past
            while(timeSeconds < 0)
                timeSeconds += 24 * 3600;
            byte hours = (byte)((timeSeconds / 3600) % 24);
            byte minutes = (byte)((timeSeconds / 60) % 60);
            byte seconds = (byte)(timeSeconds % 60);
            return new Time(hours, minutes, seconds);
        }

        public Time Minus(TimePeriod timePeriod) => Minus(this, timePeriod);
        public static Time operator -(Time time, TimePeriod timePeriod) => Plus(time, timePeriod);

    }
}
