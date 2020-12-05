using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TimeLibrary;

namespace TimeUnitTests {
    [TestClass]
    public class TimeUnitTests {
        #region Constructor tests ===============================
        public static IEnumerable<object[]> DataSet1Invalid3params_ArgumentOutOfRangeEx => new List<object[]> {
            new object[] {(byte)24},
            new object[] {(byte)12, (byte)69},
            new object[] {(byte)27, (byte)68},
            new object[] {(byte)27, (byte)45},
            new object[] {(byte)6, (byte)49, (byte)69},
            new object[] {(byte)6, (byte)89, (byte)69},
            new object[] {(byte)12, (byte)121, (byte)25},
            new object[] {(byte)45, (byte)49, (byte)45},
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSet1Invalid3params_ArgumentOutOfRangeEx))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Invalid_Hour_Minutes_Seconds_ArgumentOutOfRangeException(byte a, byte b = 0, byte c = 0) {
            Time t = new Time(a, b, c);
        }

        public static IEnumerable<object[]> DataSet1InvalidString_ArgumentOutOfRangeEx => new List<object[]> {
            new object[] {""},
            new object[] {"15.2.2"},
            new object[] {"safas"},
            new object[] {"15:124"},
            new object[] {"125:124"},
            new object[] {"24:59:59"},
            new object[] {"12:124:49"},
            new object[] {"12:24:149"},
            new object[] {"12:124:491"},
            new object[] {"125:124:492"},
            new object[] {"12:-12:-49"},
            new object[] {"-14:12:12"},
            new object[] {"1:23,42"},
            new object[] {"22:69:12"},
            new object[] {"1:59:123"},
            new object[] {"12:-1:-5"},
            new object[] {"12:12:12:12"},
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSet1InvalidString_ArgumentOutOfRangeEx))]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Invalid_String_ArgumentException(string a) {
            Time t = new Time(a);
        }

        public static IEnumerable<object[]> DataSet1ValidString => new List<object[]> {
            new object[] {"0", (byte)0, (byte)0, (byte)0 },
            new object[] {"15", (byte)15, (byte)0, (byte)0 },
            new object[] {"15:12", (byte)15, (byte)12, (byte)0 },
            new object[] {"12:59:59", (byte)12, (byte)59, (byte)59 },
            new object[] {"23:14:03", (byte)23, (byte)14, (byte)3 },
            new object[] {"1:1:1", (byte)1, (byte)1, (byte)1 },
            new object[] {"23:4:15", (byte)23, (byte)4, (byte)15 },
            new object[] {"14:15:1", (byte)14, (byte)15, (byte)1 }
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSet1ValidString))]
        public void Constructor_Valid_String(string a, 
            byte expectedHour, byte expectedMinutes, byte expectedSeconds) {
            Time t = new Time(a);
            Assert.AreEqual(expectedHour, t.Hours);
            Assert.AreEqual(expectedMinutes, t.Minutes);
            Assert.AreEqual(expectedSeconds, t.Seconds);
        }

        public static IEnumerable<object[]> DataSetValid3params => new List<object[]> {
            new object[] {(byte)0, (byte)2, (byte)5, (byte)0, (byte)2, (byte)5},
            new object[] {(byte)23, (byte)2, (byte)5, (byte)23, (byte)2, (byte)5},
            new object[] {(byte)12, (byte)12, (byte)55, (byte)12, (byte)12, (byte)55},
            new object[] {(byte)0, (byte)0, (byte)59, (byte)0, (byte)0, (byte)59},
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetValid3params))]
        public void Constructor_Valid_3Params(byte hour, byte minutes, byte seconds,
            byte expectedHour, byte expectedMinutes, byte expectedSeconds) {
            Time t = new Time(hour, minutes, seconds);
            Assert.AreEqual(expectedHour, t.Hours);
            Assert.AreEqual(expectedMinutes, t.Minutes);
            Assert.AreEqual(expectedSeconds, t.Seconds);
        }

        public static IEnumerable<object[]> DataSetValid2params => new List<object[]> {
            new object[] {(byte)0, (byte)2, (byte)0, (byte)2, (byte)0},
            new object[] {(byte)23, (byte)5, (byte)23, (byte)5, (byte)0},
            new object[] {(byte)12, (byte)12, (byte)12, (byte)12, (byte)0},
            new object[] {(byte)0, (byte)0, (byte)0, (byte)0, (byte)0},
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetValid2params))]
        public void Constructor_Valid_2Params(byte hour, byte minutes,
            byte expectedHour, byte expectedMinutes, byte expectedSeconds) {
            Time t = new Time(hour, minutes);
            Assert.AreEqual(expectedHour, t.Hours);
            Assert.AreEqual(expectedMinutes, t.Minutes);
            Assert.AreEqual(expectedSeconds, t.Seconds);
        }

        public static IEnumerable<object[]> DataSetValid1param => new List<object[]> {
            new object[] {(byte)0, (byte)0, (byte)0, (byte)0},
            new object[] {(byte)23, (byte)23, (byte)0, (byte)0},
            new object[] {(byte)12, (byte)12, (byte)0, (byte)0},
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetValid1param))]
        public void Constructor_Valid_1Param(byte hour,
            byte expectedHour, byte expectedMinutes, byte expectedSeconds) {
            Time t = new Time(hour);
            Assert.AreEqual(expectedHour, t.Hours);
            Assert.AreEqual(expectedMinutes, t.Minutes);
            Assert.AreEqual(expectedSeconds, t.Seconds);
        }

        #endregion

        #region ToString tests ===============================
        public static IEnumerable<object[]> DataSetToString => new List<object[]> {
            new object[] {(byte)0, (byte)2, (byte)1, "00:02:01"},
            new object[] { (byte)0, (byte)0, (byte)1, "00:00:01"},
            new object[] { (byte)6, (byte)45, (byte)15, "06:45:15"},
            new object[] { (byte)23, (byte)45, (byte)15, "23:45:15"},
            new object[] { (byte)1, (byte)5, (byte)59, "01:05:59"},
            new object[] { (byte)19, (byte)2, (byte)2, "19:02:02"},
            new object[] { (byte)5, (byte)59, (byte)59, "05:59:59"}
        };
        [DataTestMethod, TestCategory("String representation")]
        [DynamicData(nameof(DataSetToString))]
        public void ToString_Valid_Format(byte hour, byte minutes, byte seconds, string expected) {
            Time t = new Time(hour, minutes, seconds);
            Assert.AreEqual(expected, t.ToString());
        }
        #endregion

        #region Comparison tests ===============================
        public static IEnumerable<object[]> DataSetComparisonGreater => new List<object[]> {
            new object[] {"12:45:13", "13:45:13", false},
            new object[] {"1:5:4", "1:6:4", false},
            new object[] {"1:5:4", "1:5:5", false},
            new object[] {"23:22:21", "23:22:20", true},
            new object[] {"23:22:21", "23:21:21", true },
            new object[] {"23:22:21", "22:22:21", true }
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonGreater))]
        public void TimePeriod_Comparisons_Greater(string a, string b, bool expected) {
            Time t1 = new Time(a);
            Time t2 = new Time(b);
            Assert.AreEqual(expected, t1 > t2);
        }

        public static IEnumerable<object[]> DataSetComparisonGreaterEqual => new List<object[]> {
            new object[] {"12:45:13", "13:45:13", false},
            new object[] {"1:5:4", "1:6:4", false},
            new object[] {"1:5:4", "1:5:5", false},
            new object[] {"01:05:04", "1:5:4", true},
            new object[] {"23:22:21", "23:22:20", true},
            new object[] {"23:22:21", "23:21:21", true },
            new object[] {"23:22:21", "22:22:21", true },
            new object[] {"23:22:22", "23:22:22", true}
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonGreaterEqual))]
        public void TimePeriod_Comparisons_Greater_Equal(string a, string b, bool expected) {
            Time t1 = new Time(a);
            Time t2 = new Time(b);
            Assert.AreEqual(expected, t1 >= t2);
        }

        public static IEnumerable<object[]> DataSetComparisonLower => new List<object[]> {
            new object[] {"15:43:21", "13:45:13", false},
            new object[] {"1:23:55", "1:6:4", false},
            new object[] {"1:5:45", "01:05:05", false},
            new object[] {"23:22:21", "23:42:25", true},
            new object[] {"23:22:21", "23:31:21", true },
            new object[] {"21:22:21", "22:22:21", true }
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonLower))]
        public void TimePeriod_Comparisons_Lower(string a, string b, bool expected) {
            Time t1 = new Time(a);
            Time t2 = new Time(b);
            Assert.AreEqual(expected, t1 < t2);
        }

        public static IEnumerable<object[]> DataSetComparisonLowerEqual => new List<object[]> {
            new object[] {"15:43:21", "13:45:13", false},
            new object[] {"1:23:55", "1:6:4", false},
            new object[] {"1:5:45", "01:05:05", false},
            new object[] {"1:5:45", "01:05:45", true},
            new object[] {"23:22:21", "23:42:25", true},
            new object[] {"23:22:21", "23:31:21", true },
            new object[] {"21:22:21", "22:22:21", true },
            new object[] {"21:22:21", "21:22:21", true },
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonLowerEqual))]
        public void TimePeriod_Comparisons_Lower_Equal(string a, string b, bool expected) {
            Time t1 = new Time(a);
            Time t2 = new Time(b);
            Assert.AreEqual(expected, t1 <= t2);
        }

        public static IEnumerable<object[]> DataSetComparisonEqual => new List<object[]> {
            new object[] {"15:43:21", "13:45:13", false},
            new object[] {"1:23:55", "1:6:4", false},
            new object[] {"1:5:45", "01:05:05", false},
            new object[] {"1:5:45", "01:05:45", true},
            new object[] {"23:22:21", "23:42:25", false },
            new object[] {"23:22:21", "23:31:21", false },
            new object[] {"21:22:21", "22:22:21", false },
            new object[] {"21:22:21", "21:22:21", true },
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonEqual))]
        public void TimePeriod_Comparisons_Equal(string a, string b, bool expected) {
            Time t1 = new Time(a);
            Time t2 = new Time(b);
            Assert.AreEqual(expected, t1 == t2);
        }

        public static IEnumerable<object[]> DataSetComparisonNotEqual => new List<object[]> {
            new object[] {"15:43:21", "13:45:13", true},
            new object[] {"1:23:55", "1:6:4", true },
            new object[] {"1:5:45", "01:05:45", false },
            new object[] {"23:22:21", "23:42:25", true },
            new object[] {"23:22:21", "23:31:21", true },
            new object[] {"21:22:21", "21:22:21", false },
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonNotEqual))]
        public void TimePeriod_Comparisons_Not_Equal(string a, string b, bool expected) {
            Time t1 = new Time(a);
            Time t2 = new Time(b);
            Assert.AreEqual(expected, t1 != t2);
        }
        #endregion

        #region Add, Substract Time ===============================
        public static IEnumerable<object[]> DataSetAdd => new List<object[]> {
            new object[] {"12:12:12", 1, "12:12:13"},
            new object[] {"01:00:05", 5 * 3600 + 24 * 60 + 53, "06:24:58"},
            new object[] {"13:05:14", 23 * 3600 + 23 * 60 + 54, "12:29:08"},
        };
        [DataTestMethod, TestCategory("Add, substract")]
        [DynamicData(nameof(DataSetAdd))]
        public void Time_Add(string a, long b, string expected) {
            Time t1 = new Time(a);
            TimePeriod t2 = new TimePeriod(b);
            Time result = t1 + t2;
            Assert.AreEqual(expected, result.ToString());
        }

        public static IEnumerable<object[]> DataSetSubstract => new List<object[]> {
            new object[] {"12:12:12", 1, "12:12:11"},
            new object[] {"01:00:05", 5 * 3600 + 24 * 60 + 53, "19:35:12"},
            new object[] {"13:05:14", 23 * 3600 + 23 * 60 + 54, "13:41:20"},
        };
        [DataTestMethod, TestCategory("Add, substract")]
        [DynamicData(nameof(DataSetSubstract))]
        public void Substract(string a, long b, string expected) {
            Time t1 = new Time(a);
            TimePeriod t2 = new TimePeriod(b);
            Time result = t1 - t2;
            Assert.AreEqual(expected, result.ToString());
        }
        #endregion
    }
}
