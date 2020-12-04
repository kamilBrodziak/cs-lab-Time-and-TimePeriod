using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TimeLibrary;

namespace TimeUnitTests {
    [TestClass]
    public class TimePeriodUnitTests {

        #region Constructor tests ===============================
        public static IEnumerable<object[]> DataSet1NegativeSeconds_ArgumentOutOfRangeEx => new List<object[]> {
            new object[] {-5},
            new object[] {-12021002},
            new object[] {-1}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSet1NegativeSeconds_ArgumentOutOfRangeEx))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Negative_Seconds_ArgumentOutOfRangeException(long a) {
            TimePeriod t = new TimePeriod(a);
        }

        public static IEnumerable<object[]> DataSet1InvalidString_ArgumentOutOfRangeEx => new List<object[]> {
            new object[] {""},
            new object[] {"15.2.2"},
            new object[] {"safas"},
            new object[] {"15:124"},
            new object[] {"-14:12:12"},
            new object[] {"1:23,42"},
            new object[] {"22:69:12"},
            new object[] {"1:59:123"},
            new object[] {"12:-1:-5"},
            new object[] {"12:12:12:12"},
            new object[] {"145:145"}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSet1InvalidString_ArgumentOutOfRangeEx))]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Invalid_String_ArgumentException(string a) {
            TimePeriod t = new TimePeriod(a);
        }

        public static IEnumerable<object[]> DataSetInvalidHoursMinutes_ArgumentOutOfRangeEx => new List<object[]> {
            new object[] {-5, (byte)120},
            new object[] {12, (byte)61},
            new object[] {12, (byte)70}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetInvalidHoursMinutes_ArgumentOutOfRangeEx))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Invalid_Hours_Minutes(long a, byte b) {
            TimePeriod t = new TimePeriod(a, b);
        }

        public static IEnumerable<object[]> DataSetInvalidHoursMinutesSeconds_ArgumentOutOfRangeEx => new List<object[]> {
            new object[] {-5, (byte)12, (byte)0},
            new object[] {5, (byte)111, (byte)5},
            new object[] {12, (byte)112, (byte)1},
            new object[] {-12, (byte)12, (byte)5},
            new object[] {51, (byte)49, (byte)125}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetInvalidHoursMinutesSeconds_ArgumentOutOfRangeEx))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Invalid_Hours_Minutes_Seconds(long a, byte b, byte c) {
            TimePeriod t = new TimePeriod(a, b, c);
        }

        public static IEnumerable<object[]> DataSet1ValidString => new List<object[]> {
            new object[] {"0", 0},
            new object[] {"15", 15 * 3600},
            new object[] {"15:12", 15*3600 + 12 * 60},
            new object[] {"125:12", 125 * 3600 + 12 * 60},
            new object[] {"15:59:59", 15 * 3600 + 59 * 60 + 59},
            new object[] {"125:1:52", 125 * 3600 + 1 * 60 + 52},
            new object[] {"56:01:05", 56 * 3600 + 1 * 60 + 5},
            new object[] {"25:14:03", 25 * 3600 + 14 * 60 + 3}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSet1ValidString))]
        public void Constructor_Valid_String_ArgumentException(string a, long expectedSeconds) {
            Assert.AreEqual(expectedSeconds, (new TimePeriod(a)).Seconds);
        }

        public static IEnumerable<object[]> DataSetValidSeconds => new List<object[]> {
            new object[] {55555, 55555},
            new object[] {156, 156},
            new object[] {0, 0},
            new object[] {1, 1}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetValidSeconds))]
        public void Constructor_Valid_Seconds(long a, long expected) {
            TimePeriod t = new TimePeriod(a);

            Assert.AreEqual(expected, t.Seconds);
        }

        public static IEnumerable<object[]> DataSetValidHoursMinutes => new List<object[]> {
            new object[] {5, (byte)12, 5 * 3600 + 12 * 60},
            new object[] {156, (byte)41, 156 * 3600 + 41 * 60},
            new object[] {0, (byte)55, 55 * 60}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetValidHoursMinutes))]
        public void Constructor_Valid_Hours_Minutes(long a, byte b, long expected) {
            TimePeriod t = new TimePeriod(a, b);

            Assert.AreEqual(expected, t.Seconds);
        }

        public static IEnumerable<object[]> DataSetValidHoursMinutesSeconds => new List<object[]> {
            new object[] {5, (byte)12, (byte)13, 5 * 3600 + 12 * 60 + 13},
            new object[] {156, (byte)41, (byte)21, 156 * 3600 + 41 * 60 + 21},
            new object[] {0, (byte)55, (byte)55, 55 * 60 + 55}
        };
        [DataTestMethod, TestCategory("Constructors")]
        [DynamicData(nameof(DataSetValidHoursMinutesSeconds))]
        public void Constructor_Valid_Hours_Minutes_Seconds(long a, byte b, byte c, long expected) {
            TimePeriod t = new TimePeriod(a, b, c);

            Assert.AreEqual(expected, t.Seconds);
        }

        #endregion

        #region ToString tests ===============================
        public static IEnumerable<object[]> DataSetToString => new List<object[]> {
            new object[] {121, "0:02:01"},
            new object[] {1, "0:00:01"},
            new object[] {6 * 3600 + 45 * 60 + 15, "6:45:15"},
            new object[] {126 * 3600 + 45 * 60 + 15, "126:45:15"},
            new object[] {1 * 3600 + 5 * 60 + 59, "1:05:59"},
            new object[] {89 * 3600 + 2 * 60 + 12, "89:02:12"},
            new object[] {6 * 3600 + 35 * 60 + 1, "6:35:01"}
        };
        [DataTestMethod, TestCategory("String representation")]
        [DynamicData(nameof(DataSetToString))]
        public void ToString_Valid_Format(long a, string expected) {
            TimePeriod t = new TimePeriod(a);
            Assert.AreEqual(expected, t.ToString());
        }
        #endregion

        #region Comparison tests ===============================
        public static IEnumerable<object[]> DataSetComparisonGreater => new List<object[]> {
            new object[] {121, 125, false},
            new object[] {1, 145, false},
            new object[] {1259812, 888, true},
            new object[] {2, 1, true},
            new object[] {2, 2, false}
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonGreater))]
        public void TimePeriod_Comparisons_Greater(long a, long b, bool expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            Assert.AreEqual(t1 > t2, expected);
        }

        public static IEnumerable<object[]> DataSetComparisonGreaterEqual => new List<object[]> {
            new object[] {121, 125, false},
            new object[] {1, 145, false},
            new object[] {1259812, 888, true},
            new object[] {2, 1, true},
            new object[] {2, 2, true},
            new object[] {551, 551, true}
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonGreaterEqual))]
        public void TimePeriod_Comparisons_Greater_Equal(long a, long b, bool expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            Assert.AreEqual(t1 >= t2, expected);
        }

        public static IEnumerable<object[]> DataSetComparisonLower => new List<object[]> {
            new object[] {12, 11, false},
            new object[] {125, 5, false},
            new object[] {125, 888, true},
            new object[] {2, 15, true},
            new object[] {2, 2, false},
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonLower))]
        public void TimePeriod_Comparisons_Lower(long a, long b, bool expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            Assert.AreEqual(t1 < t2, expected);
        }

        public static IEnumerable<object[]> DataSetComparisonLowerEqual => new List<object[]> {
            new object[] {121, 125, true},
            new object[] {1, 145, true},
            new object[] {1259812, 888, false},
            new object[] {2, 1, false},
            new object[] {2, 2, true},
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonLowerEqual))]
        public void TimePeriod_Comparisons_Lower_Equal(long a, long b, bool expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            Assert.AreEqual(t1 <= t2, expected);
        }

        public static IEnumerable<object[]> DataSetComparisonEqual => new List<object[]> {
            new object[] {121, 125, false},
            new object[] {1, 145, false},
            new object[] {1259812, 888, false},
            new object[] {2, 2, true},
            new object[] {555, 555, true},
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonEqual))]
        public void TimePeriod_Comparisons_Equal(long a, long b, bool expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            Assert.AreEqual(t1 == t2, expected);
        }

        public static IEnumerable<object[]> DataSetComparisonNotEqual => new List<object[]> {
            new object[] {121, 125, true},
            new object[] {1, 145, true},
            new object[] {1259812, 888, true},
            new object[] {2, 2, false},
            new object[] {555, 555, false},
        };
        [DataTestMethod, TestCategory("Comparison")]
        [DynamicData(nameof(DataSetComparisonNotEqual))]
        public void TimePeriod_Comparisons_Not_Equal(long a, long b, bool expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            Assert.AreEqual(t1 != t2, expected);
        }
        #endregion

        #region Add, Substract TimePeriods ===============================
        public static IEnumerable<object[]> DataSetAdd => new List<object[]> {
            new object[] {121, 125, 121 + 125},
            new object[] {1, 145, 1 + 145},
            new object[] {1259812, 888, 1259812 + 888},
            new object[] {2, 1, 2 + 1},
            new object[] {0, 0, 0}
        };
        [DataTestMethod, TestCategory("Add, substract")]
        [DynamicData(nameof(DataSetAdd))]
        public void TimePeriod_Add(long a, long b, long expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            TimePeriod result = t1 + t2;
            Assert.AreEqual(expected, result.Seconds);
        }

        public static IEnumerable<object[]> DataSetSubstract => new List<object[]> {
            new object[] {121, 120, 1},
            new object[] {1, 0, 1},
            new object[] {1, 1, 0},
            new object[] {1259812, 888, 1259812 - 888},
            new object[] {251, 41, 210},
            new object[] {0, 0, 0}
        };
        [DataTestMethod, TestCategory("Add, substract")]
        [DynamicData(nameof(DataSetSubstract))]
        public void TimePeriod_Substract(long a, long b, long expected) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            TimePeriod result = t1 - t2;
            Assert.AreEqual(expected, result.Seconds);
        }

        public static IEnumerable<object[]> DataSetSubstract_ArgumentException => new List<object[]> {
            new object[] {121, 125},
            new object[] {1, 55},
            new object[] {1259812, 8979891},
            new object[] {251, 2222},
            new object[] {0, 1}
        };
        [DataTestMethod, TestCategory("Add, substract")]
        [DynamicData(nameof(DataSetSubstract_ArgumentException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TimePeriod_Substract_ArgumentException(long a, long b) {
            TimePeriod t1 = new TimePeriod(a);
            TimePeriod t2 = new TimePeriod(b);
            TimePeriod result = t1 - t2;
        }
        #endregion
    }
}
