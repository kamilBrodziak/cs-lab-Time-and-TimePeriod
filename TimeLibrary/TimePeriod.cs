using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace TimeLibrary {
    public struct TimePeriod : IEquatable<TimePeriod>, IComparable<TimePeriod> {
        public readonly long Seconds { get; }

        public TimePeriod(long hours, byte minutes, byte seconds = 0) 
            : this(hours * 60 * 60 + minutes * 60 + seconds) {

            verifyRange(minutes, 0, 59);
            verifyRange(seconds, 0, 59);
            verifyRange(hours, 0, long.MaxValue);
            bool verifyRange(long value, long min, long max) =>
                (value >= min && value <= max) ? true : throw new ArgumentOutOfRangeException();
        }

        public TimePeriod(long seconds) {
            if(seconds < 0)
                throw new ArgumentOutOfRangeException();
            Seconds = seconds;
        }

        public TimePeriod(string time) {
            // Matches:
            // H+
            // H+:MM or H+:M
            // H+:MM:SS or H+:M:SS or H+:MM:S or H+:M:S
            if(!Regex.IsMatch(time, @"^([1-9]?[0-9]+)(:[0-5]?[0-9]){0,2}$"))
                throw new ArgumentOutOfRangeException();
            string[] timePartsStrings = time.Split(":");
            byte[] timeParts = new byte[] { 0, 0 };
            long hour = long.Parse(timePartsStrings[0]);
            for(int i = 1; i < timePartsStrings.Length; ++i) {
                timeParts[i] = byte.Parse(timePartsStrings[i]);
            }
            Seconds = hour * 60 * 60 + timeParts[0] * 60 + timeParts[1];
        }

        public override string ToString() => $"{Seconds/3600}:{(Seconds/60)%60:D2}:{Seconds%60:D2}";

        public bool Equals(TimePeriod other) {
            return Seconds == other.Seconds;
        }

        public int CompareTo(TimePeriod other) {
            return Seconds.CompareTo(other.Seconds);
        }

        public static bool operator <(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) < 0;
        public static bool operator >(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) > 0;
        public static bool operator ==(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) == 0;
        public static bool operator !=(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) != 0;
        public static bool operator <=(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) <= 0;
        public static bool operator >=(TimePeriod t1, TimePeriod t2) => t1.CompareTo(t2) >= 0;

        public static TimePeriod Plus(TimePeriod t1, TimePeriod t2) => new TimePeriod(t1.Seconds + t2.Seconds);
        public TimePeriod Plus(TimePeriod other) => Plus(this, other);
        public static TimePeriod operator +(TimePeriod t1, TimePeriod t2) => Plus(t1, t2);

        public static TimePeriod Minus(TimePeriod t1, TimePeriod t2) {
            long seconds = t1.Seconds - t2.Seconds;
            if(seconds < 0)
                throw new ArgumentException();
            return new TimePeriod(seconds);
        }
        public TimePeriod Minus(TimePeriod other) => Minus(this, other);
        public static TimePeriod operator -(TimePeriod t1, TimePeriod t2) => Minus(t1, t2);


    }
}
