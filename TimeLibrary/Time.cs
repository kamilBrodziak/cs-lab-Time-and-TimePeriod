using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TimeLibrary {
    public struct Time : IEquatable<Time>, IComparable<Time> {
        public readonly byte Hours { get; }
        public readonly byte Minutes { get; }
        public readonly byte Seconds { get; }
        public readonly short MiliSeconds { get; }

        public Time(byte hours, byte minutes = 0, byte seconds = 0, short miliseconds = 0) {
            Hours = (byte)verifyRange(hours, 0, 23);
            Minutes = (byte)verifyRange(minutes, 0, 59);
            Seconds = (byte)verifyRange(seconds, 0, 59);
            MiliSeconds = verifyRange(miliseconds, 0, 999);

            short verifyRange(short value, short min, short max) =>
                (value >= min && value <= max) ? value : throw new ArgumentOutOfRangeException();
        }

        public Time(string time) {
            // Matches:
            // HH or H
            // HH:MM or H:M or HH:M or HH:MM
            // HH:MM:SS or H:MM:SS or HH:M:SS and so on..
            // Ending with nothing .S or .SS or .SSS when miliseconds
            if(!Regex.IsMatch(time, @"^([0-1]?[0-9]|2[0-3])((:[0-5]?[0-9]){0,2}|:[0-5]?[0-9]{2}\.([1-9][0-9]{0,2}))$"))
                throw new ArgumentException();
            string[] timePartsStrings = time.Split(":");
            byte[] timeParts = new byte[] { 0, 0, 0 };
            short miliseconds = 0;
            for(int i = 0; i < timePartsStrings.Length && i < 2; ++i) {
                timeParts[i] = byte.Parse(timePartsStrings[i]);
            }
            if(timePartsStrings.Length == 2) {
                string[] secondPartsStrings = timePartsStrings[2].Split(".");
                timeParts[2] = byte.Parse(secondPartsStrings[0]);
                if(secondPartsStrings.Length == 2) {
                    miliseconds = short.Parse(secondPartsStrings[1]);
                }
            }
            Hours = timeParts[0];
            Minutes = timeParts[1];
            Seconds = timeParts[2];
            MiliSeconds = miliseconds;
        }

        public override string ToString() => $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}";

        public string ToString(bool withMiliseconds) => ToString() + (withMiliseconds ? $".{MiliSeconds:D3}" : "");


        public bool Equals(Time other) => Hours == other.Hours &&
            Minutes == other.Minutes && Seconds == other.Seconds && MiliSeconds == other.MiliSeconds;

        public override bool Equals(object obj) {
            return obj is Time && Equals(obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Hours, Minutes, Seconds, MiliSeconds);
        }

        public int CompareTo(Time other) {
            int compare = Hours.CompareTo(other.Hours);
            if(compare != 0)
                return compare;
            compare = Minutes.CompareTo(other.Minutes);
            if(compare != 0)
                return compare;
            compare = Seconds.CompareTo(other.Seconds);
            if(compare != 0)
                return compare;
            return MiliSeconds.CompareTo(other.MiliSeconds);
        }

        public static bool operator <(Time t1, Time t2) => t1.CompareTo(t2) < 0;
        public static bool operator >(Time t1, Time t2) => t1.CompareTo(t2) > 0;
        public static bool operator ==(Time t1, Time t2) => t1.CompareTo(t2) == 0;
        public static bool operator !=(Time t1, Time t2) => t1.CompareTo(t2) != 0;
        public static bool operator <=(Time t1, Time t2) => t1.CompareTo(t2) <= 0;
        public static bool operator >=(Time t1, Time t2) => t1.CompareTo(t2) >= 0;

        public static Time Plus(Time time, TimePeriod timePeriod) {
            long timeMiliseconds = time.Hours * 3600000 + time.Minutes * 60000 + time.Seconds * 1000 + time.MiliSeconds;
            timeMiliseconds += timePeriod.Miliseconds;
            byte hours = (byte)((timeMiliseconds / 3600000) % 24);
            byte minutes = (byte)((timeMiliseconds / 60000) % 60);
            byte seconds = (byte)((timeMiliseconds / 1000) % 60);
            short miliseconds = (short)(timeMiliseconds % 1000);
            return new Time(hours, minutes, seconds, miliseconds);
        }

        public Time Plus(TimePeriod timePeriod) => Plus(this, timePeriod);
        public static Time operator +(Time time, TimePeriod timePeriod) => Plus(time, timePeriod);

        public static Time Minus(Time time, TimePeriod timePeriod) {
            long timeMiliseconds = time.Hours * 3600000 + time.Minutes * 60000 + time.Seconds * 1000 + time.MiliSeconds;
            timeMiliseconds -= timePeriod.Miliseconds;
            // If timeSeconds < 0 then get time from day in the past
            while(timeMiliseconds < 0)
                timeMiliseconds += 24 * 3600000;
            byte hours = (byte)((timeMiliseconds / 3600000) % 24);
            byte minutes = (byte)((timeMiliseconds / 60000) % 60);
            byte seconds = (byte)((timeMiliseconds / 1000) % 60);
            short miliseconds = (short)(timeMiliseconds % 1000);
            return new Time(hours, minutes, seconds, miliseconds);
        }

        public Time Minus(TimePeriod timePeriod) => Minus(this, timePeriod);
        public static Time operator -(Time time, TimePeriod timePeriod) => Minus(time, timePeriod);

    }
}
