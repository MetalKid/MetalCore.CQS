using System;

namespace MetalCore.CQS.Validation
{
    /// <summary>
    /// This class is used to throw ane exception if a condition is true.
    /// </summary>
    public static class Guard
    {
        #region << GuardHandler ==> Guard.If(...)?.Throw(() => new Exception(...)); >>

        private static readonly GuardHandler GuardHandlerInstance = new GuardHandler();

        /// <summary>
        /// This class is used to actually throw the exception.  By using ?. syntax, we can
        /// avoid creating strings/anonymous functions when we wouldn't throw the exception anyway.
        /// This reduces the amount of memory/cpu being used.
        /// </summary>
        public sealed class GuardHandler
        {
            /// <summary>
            /// Only this assembly should work be able to new up this class.
            /// </summary>
            internal GuardHandler()
            {
            }

            /// <summary>
            /// Throw the exception since something bad happened.
            /// </summary>
            /// <typeparam name="T">The type of exception being thrown.</typeparam>
            /// <param name="createException">The anonymous method that returns the exception to throw.</param>
#pragma warning disable CA1822 // Mark members as static - Is used in an instance specific way
            public void Throw<T>(Func<T> createException) where T : Exception
#pragma warning restore CA1822 // Mark members as static
            {
                throw createException();
            }
        }

        #endregion

        /// <summary>
        /// Returns the ability to throw an exception if the given condition is true; null if false.
        /// </summary>
        /// <param name="condition">The condition that will throw an exception if true.</param>
        /// <returns>Instance of GuardHandler if condition is true; null if false.</returns>
        /// <example>If(param == null)?.Throw(() => new ArgumentNullException(paramName));</example>
        public static GuardHandler If(bool condition)
        {
            return condition ? GuardHandlerInstance : null;
        }

        /// <summary>
        /// Throws an ArgumentNullException if the given param is null.
        /// </summary>
        /// <param name="param">The object to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void IsNotNull<T>(T param, string paramName)
        {
            If(param == null)?.Throw(() => new ArgumentNullException(paramName));
        }

        /// <summary>
        /// Throws an ArgumentException if the given param is null or empty string.
        /// </summary>
        /// <param name="param">The string to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void IsNotNullOrEmpty(string param, string paramName)
        {
            If(string.IsNullOrEmpty(param))?
                .Throw(() => new ArgumentException("String value must not be null or empty.", paramName));
        }

        /// <summary>
        /// Throws an ArgumentException if the given param is null, empty string, or white space.
        /// </summary>
        /// <param name="param">The string to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void IsNotNullOrWhiteSpace(string param, string paramName)
        {
            If(string.IsNullOrWhiteSpace(param))?
                .Throw(() => new ArgumentException("String value must not be null, empty, or whitespaces.", paramName));
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the given value is less than or equal to the check value.
        /// </summary>
        /// <param name="value">The value being inspected.</param>
        /// <param name="check">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static void IsGreaterThan<T>(T value, T check, string paramName) where T : IComparable<T>
        {
            If(value.CompareTo(check) <= 0)?
                .Throw(
                () =>
                    new ArgumentOutOfRangeException(
                    paramName,
                    string.Format("Parameter must be greater than {0}.", check)));
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the given value is less than the check value.
        /// </summary>
        /// <param name="value">The value being inspected.</param>
        /// <param name="check">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static void IsGreaterThanOrEqualTo<T>(T value, T check, string paramName) where T : IComparable<T>
        {
            If(value.CompareTo(check) < 0)?
                .Throw(
                () =>
                    new ArgumentOutOfRangeException(
                    paramName,
                    string.Format("Parameter must be greater than or equal to {0}", check)));
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the given value is greater than or equal to the check value.
        /// </summary>
        /// <param name="value">The value being inspected.</param>
        /// <param name="check">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static void IsLessThan<T>(T value, T check, string paramName) where T : IComparable<T>
        {
            If(value.CompareTo(check) >= 0)?
                .Throw(
                () =>
                    new ArgumentOutOfRangeException(
                    paramName,
                    string.Format("Parameter must be less than {0}.", check)));
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the given value is greater than the check value.
        /// </summary>
        /// <param name="value">The value being inspected.</param>
        /// <param name="check">The value to compare against.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static void IsLessThanOrEqualTo<T>(T value, T check, string paramName) where T : IComparable<T>
        {
            If(value.CompareTo(check) > 0)?
                .Throw(
                () =>
                    new ArgumentOutOfRangeException(
                    paramName,
                    string.Format("Parameter must be less than or equal to {0}.", check)));
        }

        /// <summary>
        /// Throws an ArgumentException if the given value is not assignable to type (T).
        /// </summary>
        /// <typeparam name="T">The type the value needs to be assignable to.</typeparam>
        /// <param name="value">The value to check against.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void IsAssignableTo<T>(object value, string paramName)
        {
            If(!(value is T))?
                .Throw(
                () =>
                    new ArgumentException(
                    string.Format("Parameter must be assignable to {0}.", typeof(T).FullName),
                    paramName));
        }

        /// <summary>
        /// Throws an ArgumentException if the given Guid is equal to Guid.Empty.
        /// </summary>
        /// <param name="guid">The guid value to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void IsNotEmptyGuid(Guid guid, string paramName)
        {
            If(guid == Guid.Empty)?.Throw(() => new ArgumentException("Parameter must not be an empty Guid.", paramName));
        }

        /// <summary>
        /// Throws an ArgumentException if the given DateTime is equal to the default DateTime.
        /// </summary>
        /// <param name="date">The DateTime to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void IsNotDefaultDateTime(DateTime date, string paramName)
        {
            If(date == default)?
                .Throw(() => new ArgumentException("Parameter must not be a default DateTime.", paramName));
        }

        /// <summary>
        /// Throws an ArgumentException if the given Url is not valid.
        /// </summary>
        /// <param name="url">The Url to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="uriKind">The type of Url to be validated.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void IsValidUrl(string url, string paramName, UriKind uriKind = UriKind.Absolute)
        {
            If(!Uri.TryCreate(url, uriKind, out Uri _))?
                .Throw(() => new ArgumentException("Value must be a valid Url.", paramName));
        }
    }
}