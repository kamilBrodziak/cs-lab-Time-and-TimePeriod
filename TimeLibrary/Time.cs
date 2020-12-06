using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TimeLibrary {

    /// <summary>
    /// Time struct used for describing time.
    /// </summary>
    public struct Time : IEquatable<Time>, IComparable<Time> {

        /// <value>Get hours </value>
        public readonly byte Hours { get; }
        /// <value>Get hours </value>
        public readonly byte Minutes { get; }
        /// <value>Get seconds </value>
        public readonly byte Seconds { get; }
        /// <value>Get miliseconds </value>
        public readonly short MiliSeconds { get; }
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameters are below 0 
        /// or when first parameter is greater than 23
        /// or when second or third paramterer is greater than 59 
        /// or when fourth parameter is greater than 999.</exception>
        /// <param name="hours">A byte number used as a time representation of hours. Must be lower than 23</param>
        /// <param name="minutes">A byte number used as a time representation of minutes. Must be lower than 60</param>
        /// <param name="seconds">A optional byte number used as a time representation of seconds. Must be lower than 60</param>
        /// <param name="miliseconds">A optional short integer used as a time representation of miliseconds. Must be lower than 1000</param>
        public Time(byte hours, byte minutes = 0, byte seconds = 0, short miliseconds = 0) {
            Hours = (byte)verifyRange(hours, 0, 23);
            Minutes = (byte)verifyRange(minutes, 0, 59);
            Seconds = (byte)verifyRange(seconds, 0, 59);
            MiliSeconds = verifyRange(miliseconds, 0, 999);

            short verifyRange(short value, short min, short max) =>
                (value >= min && value <= max) ? value : throw new ArgumentOutOfRangeException();
        }

        /// <exception cref="System.ArgumentException">
        /// Thrown when first parameter doesn't match required format.
        /// </exception>
        /// <param name="time">A string used as a representation of time.
        /// Matches:
        /// 1. HH or H
        /// 2. HH:MM or HH:M or H:MM or H:M
        /// 3. HH:MM:SS or HH:M:SS or HH:MM:S or HH:M:S or H:MM:SS and so on
        /// 4. 3 + .S or .SS or .SSS for miliseconds
        /// </param>
        public Time(string time) {
            if(!Regex.IsMatch(time, @"^([0-1]?[0-9]|2[0-3])((:[0-5]?[0-9]){0,2}|(:[0-5]?[0-9]){2}\.[1-9][0-9]{0,2})$"))
                throw new ArgumentException();
            string[] timePartsStrings = time.Split(":");
            byte[] timeParts = new byte[] { 0, 0, 0 };
            short miliseconds = 0;
            for(int i = 0; i < timePartsStrings.Length && i < 2; ++i) {
                timeParts[i] = byte.Parse(timePartsStrings[i]);
            }
            if(timePartsStrings.Length == 3) {
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

        /// <returns>
        /// Returns string representation of time, in format HH:MM:SS
        /// </returns>
        public override string ToString() => $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}";

        /// <param name="withMiliseconds">
        /// A boolean parameter, true if you want to include miliseconds
        /// </param>
        /// <returns>
        /// Returns string representation of time, in format HH:MM:SS or HH:MM:SS.SSS
        /// </returns>
        public string ToString(bool withMiliseconds) => ToString() + (withMiliseconds ? $".{MiliSeconds:D3}" : "");

        /// <summary>
        /// Times are equal when their hour, minutes, seconds and miliseconds are equal.
        /// </summary>
        /// <returns>
        /// A boolean, true if timeperiods are equal.
        /// </returns>
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


        /// <summary>
        /// Adding two times t1, t2 by creating new Time with sum of t1, t2 times.
        /// Skip day/days when time period is long enough
        /// </summary>
        /// <param name="time">
        /// Time object
        /// </param>
        /// <param name="timePeriod">
        /// TimePeriod object
        /// </param>
        /// <returns>
        /// Time object
        /// </returns>
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


        /// <summary>
        /// Substracting t1 and TimePeriod t2 by creating new Time with time of t1 substracted by t2 miliseconds count.
        /// Goes back in time couple of days if TimePeriod is long enough
        /// </summary>
        /// <param name="time">
        /// Time object
        /// </param>
        /// <param name="timePeriod">
        /// TimePeriod object
        /// </param>
        /// <returns>
        /// Time object
        /// </returns>
        public static Time Minus(Time time, TimePeriod timePeriod) {
            long timeMiliseconds = time.Hours * 3600000 + time.Minutes * 60000 + time.Seconds * 1000 + time.MiliSeconds;
            timeMiliseconds -= timePeriod.Miliseconds;
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
