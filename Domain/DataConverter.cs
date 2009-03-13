using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain
{
    public static class DataConverter
    {
        /// <summary>
        /// Convert a database field to a string.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted string or string.Empty.</returns>
        public static string FieldValueToString(object theValue)
        {
            return FieldValueToString(theValue, string.Empty);
        }

        /// <summary>
        /// Convert a database field to an string or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted String or def.</returns>
        public static string FieldValueToString(object theValue, string def)
        {
            if (theValue == null) return def;

            return theValue.ToString();
        }

        /// <summary>
        /// Convert a database field to a byte.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted byte or byte.Empty.</returns>
        public static byte FieldValueToByte(object theValue)
        {
            return FieldValueToByte(theValue, byte.MinValue);
        }

        /// <summary>
        /// Convert a database field to an byte or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted Byte or def.</returns>
        public static byte FieldValueToByte(object theValue, byte def)
        {
            if (theValue == null) return def;

            byte theResult;
            return byte.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to an Guid.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted Guid or Guid.Empty.</returns>
        public static Guid FieldValueToGuid(object theValue)
        {
            return FieldValueToGuid(theValue, Guid.Empty);
        }

        /// <summary>
        /// Convert a database field to an Guid.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default GUID value if theValue is null</param>
        /// <returns>The converted Guid or def</returns>
        public static Guid FieldValueToGuid(object theValue, Guid def)
        {
            if ((theValue == null) || (string.IsNullOrEmpty(theValue.ToString()))) return def;

            return new Guid(theValue.ToString());
        }

        /// <summary>
        /// Convert a database field to an Int32.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted Int32 or 0.</returns>
        public static int FieldValueToInt32(object theValue)
        {
            return FieldValueToInt32(theValue, 0);
        }

        /// <summary>
        /// Convert a database field to an Int32 or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted Int32 or def.</returns>
        public static int FieldValueToInt32(object theValue, int def)
        {
            if (theValue == null) return def;

            int theResult;
            return int.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to an Int64.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted Int64 or 0.</returns>
        public static Int64 FieldValueToInt64(object theValue)
        {
            return FieldValueToInt64(theValue, 0);
        }

        /// <summary>
        /// Convert a database field to an Int64 or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted Int64 or def.</returns>
        public static Int64 FieldValueToInt64(object theValue, Int64 def)
        {
            if (theValue == null) return def;
            Int64 theResult;
            return Int64.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to a decimal.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted decimal or 0.</returns>
        public static decimal FieldValueToDecimal(object theValue)
        {
            return FieldValueToDecimal(theValue, 0);
        }

        /// <summary>
        /// Convert a database field to a decimal or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted decimal or def.</returns>
        public static decimal FieldValueToDecimal(object theValue, decimal def)
        {
            if (theValue == null) return def;
            decimal theResult;
            return decimal.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to a float.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted float or 0.</returns>
        public static float FieldValueToFloat(object theValue)
        {
            return FieldValueToFloat(theValue, 0);
        }

        /// <summary>
        /// Convert a database field to a float or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted float or def.</returns>
        public static float FieldValueToFloat(object theValue, float def)
        {
            if (theValue == null) return def;
            float theResult;
            return float.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to a double.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted double or def.</returns>
        public static double FieldValueToDouble(object theValue)
        {
            return FieldValueToDouble(theValue, 0);
        }

        /// <summary>
        /// Convert a database field to a double or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted double or def.</returns>
        public static double FieldValueToDouble(object theValue, double def)
        {
            if (theValue == null) return def;

            double theResult;
            return double.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to a boolean.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted boolean or false.</returns>
        public static bool FieldValueToBoolean(object theValue)
        {
            return FieldValueToBoolean(theValue, false);
        }

        /// <summary>
        /// Convert a database field to a boolean.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted boolean or def.</returns>
        public static bool FieldValueToBoolean(object theValue, bool def)
        {
            if (theValue == null) return def;

            // Check if value converts to an integeter (0 indicates failed as it is the default)
            int intValue = FieldValueToInt32(theValue);
            if (intValue == 0)
            {
                bool theResult;
                return bool.TryParse(theValue.ToString(), out theResult) ? theResult : def;
            }
            // Return a bool comparison to 1
            return (Math.Abs(FieldValueToInt32(theValue)) == 1);
        }

        /// <summary>
        /// Convert a database field to a DateTime.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted DateTime or false.</returns>
        public static DateTime FieldValueToDateTime(object theValue)
        {
            return FieldValueToDateTime(theValue, DateTime.MinValue);
        }

        /// <summary>
        /// Convert a database field to a DateTime or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted DateTime or DateTime.MinValue.</returns>
        public static DateTime FieldValueToDateTime(object theValue, DateTime def)
        {
            if (theValue == null) return def;

            DateTime theResult;
            return DateTime.TryParse(theValue.ToString(), out theResult) ? theResult : def;
        }

        /// <summary>
        /// Convert a database field to a TimeSpan.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted TimeSpan or false.</returns>
        public static TimeSpan FieldValueToTimeSpan(object theValue)
        {
            return FieldValueToTimeSpan(theValue, TimeSpan.MinValue);
        }

        /// <summary>
        /// Convert a database field to a TimeSpan or sets it to the specified default.
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <param name="def">The default value if it can't convert.</param>
        /// <returns>The converted TimeSpan or TimeSpan.MinValue.</returns>
        public static TimeSpan FieldValueToTimeSpan(object theValue, TimeSpan def)
        {
            if (theValue == null) return def;

            int theResult;
            return int.TryParse(theValue.ToString(), out theResult) ? new TimeSpan(theResult) : def;
        }

        /// <summary>
        /// Convert a database field to a Byte[].
        /// </summary>
        /// <param name="theValue">The value to convert.</param>
        /// <returns>The converted Byte[].</returns>
        public static byte[] FieldValueToByteArray(object theValue)
        {
            return Convert.FromBase64String(theValue.ToString());
        }
    }
}
