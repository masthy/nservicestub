using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace OrderService.Contracts
{
    /// <summary>
    /// Represents a date (to be used when time is not relevant)
    /// 
    /// </summary>
    [DataContract(Namespace = "http://internal.rikstoto.no/sharedcontracts/2008/06/")]
    [Serializable]
    public class Date : IComparable<Date>
    {
        private const string DateIntFormat = "yyyyMMdd";

        /// <summary>
        /// Gets or sets the year.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The year.
        /// </value>
        [DataMember]
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the month.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The month.
        /// </value>
        [DataMember]
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the day.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The day.
        /// </value>
        [DataMember]
        public int Day { get; set; }

        /// <summary>
        /// Gets the day of week.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The day of week.
        /// </value>
        public DayOfWeek DayOfWeek
        {
            get
            {
                return this.AsDateTime().DayOfWeek;
            }
        }

        internal Date()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Rikstoto.Service.SharedContracts.Date"/> struct.
        /// 
        /// </summary>
        /// <param name="year">The year.</param><param name="month">The month.</param><param name="day">The day.</param>
        public Date(int year, int month, int day)
        {
            Date.AssertValid(year, month, day);
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Rikstoto.Service.SharedContracts.Date"/> struct.
        /// 
        /// </summary>
        /// <param name="dateTime">The datetime (time is ignored)</param>
        public Date(DateTime dateTime)
        {
            this.Year = dateTime.Year;
            this.Month = dateTime.Month;
            this.Day = dateTime.Day;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// 
        /// </summary>
        /// <param name="left">The left.</param><param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(Date left, Date right)
        {
            if (left == null || right == null)
                throw new InvalidOperationException("Cannot compare null valued dates.");
            else
                return left.AsDateTime() < right.AsDateTime();
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// 
        /// </summary>
        /// <param name="left">The left.</param><param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(Date left, Date right)
        {
            if (left == null || right == null)
                throw new InvalidOperationException("Cannot compare null valued dates.");
            else
                return left.AsDateTime() > right.AsDateTime();
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// 
        /// </summary>
        /// <param name="left">The left.</param><param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(Date left, Date right)
        {
            if (left == null || right == null)
                throw new InvalidOperationException("Cannot compare null valued dates.");
            else
                return left.AsDateTime() >= right.AsDateTime();
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// 
        /// </summary>
        /// <param name="left">The left.</param><param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(Date left, Date right)
        {
            if (left == null || right == null)
                throw new InvalidOperationException("Cannot compare null valued dates.");
            else
                return left.AsDateTime() <= right.AsDateTime();
        }

        private static void AssertValid(int year, int month, int day)
        {
            DateTime dateTime = new DateTime(year, month, day);
        }

        /// <summary>
        /// Returns date as an int: yyyyMMdd
        /// 
        /// </summary>
        /// 
        /// <returns/>
        public int ToDateInt()
        {
            return Convert.ToInt32(this.AsDateTime().ToString("yyyyMMdd"));
        }

        /// <summary>
        /// Parses a date from int-format.
        /// 
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns/>
        public static Date FromDateInt(int date)
        {
            return new Date(DateTime.ParseExact(date.ToString(), "yyyyMMdd", (IFormatProvider)CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets date as date time.
        /// 
        ///             This is a helper method for where DateTime is needed.
        /// 
        /// </summary>
        /// 
        /// <returns/>
        public DateTime AsDateTime()
        {
            return new DateTime(this.Year, this.Month, this.Day);
        }

        /// <summary>
        /// Equalses the specified obj.
        /// 
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns/>
        public bool Equals(Date obj)
        {
            if (obj != null && obj.Year == this.Year && obj.Month == this.Month)
                return obj.Day == this.Day;
            else
                return false;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// 
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Date))
                return false;
            else
                return this.Equals((Date)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// 
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (this.Year * 397 ^ this.Month) * 397 ^ this.Day;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Date other)
        {
            return this.AsDateTime().CompareTo(other.AsDateTime());
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// 
        /// </returns>
        public override string ToString()
        {
            return string.Format("Year: {0}, Month: {1}, Day: {2}", (object)this.Year, (object)this.Month, (object)this.Day);
        }

        /// <summary>
        /// Format the date using datetime formatting.
        /// 
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns/>
        public string ToString(string format)
        {
            return this.AsDateTime().ToString(format);
        }

        /// <summary>
        /// Format the date using datetime formatting.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="formatProvider">The format provider.</param>
        /// <returns/>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.AsDateTime().ToString(format, formatProvider);
        }

        /// <summary>
        /// Returns the short date string.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Added for compatability with DateTime
        /// </returns>
        public string ToShortDateString()
        {
            return this.AsDateTime().ToShortDateString();
        }
    }
}
