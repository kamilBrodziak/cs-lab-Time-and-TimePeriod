using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace TimeLibrary {

    /// <summary>
    /// TimePeriod struct used for time describing period in time.
    /// </summary>
    public struct TimePeriod : IEquatable<TimePeriod>, IComparable<TimePeriod> {
        public enum TimeUnit {
            Milisecond, Second, Minute, Hour
        }
        /// <value>
        /// Get miliseconds
        /// </value>
        public readonly long Miliseconds { get; }
        /// <value>
        /// Get seconds
        /// </value>
        public readonly long Seconds { get => Miliseconds / 1000; }

        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameters are below 0
        /// or when second or third paramterer is greater than 59 or when fourth parameter is greater than 999.</exception>
        /// <param name="hours">A byte number used as a time representation of hours.</param>
        /// <param name="minutes">A byte number used as a time representation of minutes. Must be lower than 60</param>
        /// <param name="seconds">A optional byte number used as a time representation of seconds. Must be lower than 60</param>
        /// <param name="miliseconds">A optional short integer used as a time representation of miliseconds. Must be lower than 1000</param>
        public TimePeriod(long hours, byte minutes, byte seconds = 0, short miliseconds = 0) {
            Miliseconds = verifyRange(hours, 0, long.MaxValue) * 3600000
                + verifyRange(minutes, 0, 59) * 60000
                + verifyRange(seconds, 0, 59) * 1000
                + verifyRange(miliseconds, 0, 999);
            long verifyRange(long value, long min, long max) =>
                (value >= min && value <= max) ? value : throw new ArgumentOutOfRangeException();
        }

        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameter are below 0.</exception>
        /// <param name="miliseconds">A short integer used as a time representation of miliseconds.</param>
        public TimePeriod(long miliseconds) {
            if(miliseconds < 0)
                throw new ArgumentOutOfRangeException();
            Miliseconds = miliseconds;
        }
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when first parameter is below 0.</exception>
        /// <param name="value">A long integer used as a time representation of time.</param>
        /// <param name="unit">A TimeUnit used as a time unit choice. Can be miliseconds, seconds, minutes, hours.</param>
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

        /// <exception cref="System.ArgumentException">Thrown when first parameter doesn't match required format.</exception>
        /// <param name="time">A string used as a time representation of time.
        /// Matches:
        /// 1. H+
        /// 2. H+:MM or H+:M
        /// 3. H+MM:SS or H+:M:SS or H+:MM:S or H+:M:S
        /// 4. 3 + .S or .SS or .SSS for miliseconds
        /// </param>
        public TimePeriod(string time) {
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

        /// <returns>
        /// Returns string representation of time period, in format H:MM:SS
        /// </returns>
        public override string ToString() => $"{Miliseconds / 3600000}:{(Miliseconds / 60000) % 60:D2}:{(Miliseconds / 1000) % 60:D2}";

        /// <param name="withMiliseconds">
        /// A boolean parameter, true if you want to include miliseconds
        /// </param>
        /// <returns>
        /// Returns string representation of time period, in format H:MM:SS or H:MM:SS.SSS
        /// </returns>
        public string ToString(bool withMiliseconds) => ToString() + (withMiliseconds ? $".{(Miliseconds % 1000):D3}" : "");

        /// <summary>
        /// TimePeriods are equal when their miliseconds are equal.
        /// </summary>
        /// <returns>
        /// A boolean, true if timeperiods are equal.
        /// </returns>
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

        /// <summary>
        /// Adding two timeperiods t1, t2 by creating new TimePeriod with sum t1 and t2 miliseconds
        /// </summary>
        /// <returns>
        /// TimePeriod object with miliseconds sum of parameters
        /// </returns>
        public static TimePeriod Plus(TimePeriod t1, TimePeriod t2) => new TimePeriod(t1.Miliseconds + t2.Miliseconds);
        public TimePeriod Plus(TimePeriod other) => Plus(this, other);
        public static TimePeriod operator +(TimePeriod t1, TimePeriod t2) => Plus(t1, t2);

        /// <exception cref="System.ArgumentException">
        /// Thrown when TimePeriod t1 is lower than TimePeriod t2
        /// </exception>
        /// <summary>
        /// Substracting two timeperiods t1, t2 by creating new TimePeriod with t1 substracted by t2 miliseconds
        /// </summary>
        /// <returns>
        /// TimePeriod object with miliseconds substract of parameters
        /// </returns>
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
