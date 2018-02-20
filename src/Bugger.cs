using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Autrage.LEX.NET
{
    public static class Bugger
    {
        public const string LogCategory = "LOG";
        public const string WarningCategory = "WARNING";
        public const string ErrorCategory = "ERROR";

        /// <summary>
        /// Asserts that a certain condition is met, otherwise an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <exception cref="AssertionException">If <paramref name="condition"/> is not met.</exception>
        /// <param name="message">Optional. The message of the exception.</param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition,
            string message = "",
            [CallerFilePath] string file = "",
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
            if (!condition)
            {
                throw new AssertionException(MakeMessage(file, caller, line, message));
            }
        }

        /// <summary>
        /// Asserts that the calling object is not in a bad state, otherwise an <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the <paramref name="condition"/> is not met.</exception>
        /// <param name="condition">The condition the object must fulfil.</param>
        /// <param name="message">Optional. The message of the exception.</param>
        [Conditional("DEBUG")]
        public static void AssertState(bool condition,
            string message = "",
            [CallerFilePath] string file = "",
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
            if (!condition)
            {
                throw new InvalidOperationException(MakeMessage(file, caller, line, message));
            }
        }

        /// <summary>
        /// Asserts that an argument is not null, otherwise an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <exception cref="ArgumentNullException">If <paramref name="param"/> is null.</exception>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">Optional. The name of the parameter.</param>
        /// <param name="message">Optional. The message of the exception.</param>
        [Conditional("DEBUG")]
        public static void AssertNotNull<T>(this T param,
            string paramName = "",
            string message = "",
            [CallerFilePath] string file = "",
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, MakeMessage(file, caller, line, message));
            }
        }

        /// <summary>
        /// Asserts that an argument meets a certain condition, otherwise an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <exception cref="ArgumentException">If <paramref name="param"/> does not fulfil <paramref name="condition"/>.</exception>
        /// <param name="param">The parameter.</param>
        /// <param name="condition">The condition the parameter must fulfil.</param>
        /// <param name="paramName">Optional. The name of the parameter.</param>
        /// <param name="message">Optional. The message of the exception.</param>
        [Conditional("DEBUG")]
        public static void Assert<T>(this T param,
            Predicate<T> condition,
            string paramName = "",
            string message = "",
            [CallerFilePath] string file = "",
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
            if (condition == null)
            {
                return;
            }

            if (!condition(param))
            {
                throw new ArgumentException(MakeMessage(file, caller, line, message), paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/>.
        /// </summary>
        /// <exception cref="AssertionException">Always.</exception>
        /// <param name="message">Optional. The message of the exception.</param>
        [Conditional("DEBUG")]
        public static void Fail(string message = "",
            [CallerFilePath] string file = "",
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
            throw new AssertionException(MakeMessage(file, caller, line, message));
        }

        [Conditional("DEBUG")]
        public static void Log(string message, [CallerFilePath] string file = "", [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            message = message == null ? "" : $" {message}";
            Debug.WriteLine(MakeMessage(file, caller, line, message), LogCategory);
        }

        [Conditional("DEBUG")]
        public static void Warning(string message, [CallerFilePath] string file = "", [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            message = message == null ? "" : $" {message}";
            Debug.WriteLine(MakeMessage(file, caller, line, message), WarningCategory);
        }

        [Conditional("DEBUG")]
        public static void Error(string message, [CallerFilePath] string file = "", [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            message = message == null ? "" : $" {message}";
            Debug.WriteLine(MakeMessage(file, caller, line, message), ErrorCategory);
        }

        private static string MakeMessage(string file, string caller, int line, string message)
        {
            message = message == null ? "" : $": {message}";
            return $"{file}({caller}, {line}) [{DateTime.Now}]{message}";
        }
    }

    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message)
        {
        }
    }
}