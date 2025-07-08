namespace Darkness.Runtime.Log
{
    using UnityEngine;
    using System;

    /// <summary>
    /// A wrapper for Unity's Debug logging system with additional functionality.
    /// </summary>
    public class GameLogger
    {
        /// <summary>
        /// Log levels for filtering messages.
        /// </summary>
        public enum LogLevel
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
            All = 4
        }

        private LogLevel _currentLogLevel = LogLevel.All;
        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(object message)
        {
            if (_currentLogLevel >= LogLevel.Info)
            {
                Debug.Log(message);
            }
        }

        /// <summary>
        /// Logs a message to the console with a context object.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The object to which the message applies.</param>
        public void Log(object message, UnityEngine.Object context)
        {
            if (_currentLogLevel >= LogLevel.Info)
            {
                Debug.Log(message, context);
            }
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public void Warning(object message)
        {
            if (_currentLogLevel >= LogLevel.Warning)
            {
                Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Logs a warning message to the console with a context object.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        /// <param name="context">The object to which the warning applies.</param>
        public void Warning(object message, UnityEngine.Object context)
        {
            if (_currentLogLevel >= LogLevel.Warning)
            {
                Debug.LogWarning(message, context);
            }
        }

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public void Error(object message)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Logs an error message to the console with a context object.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="context">The object to which the error applies.</param>
        public void Error(object message, UnityEngine.Object context)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.LogError(message, context);
            }
        }
        
        /// <summary>
        /// Logs an exception to the console.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void Exception(Exception exception)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.LogException(exception);
            }
        }

        /// <summary>
        /// Logs an exception to the console with a context object.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="context">The object to which the exception applies.</param>
        public void Exception(Exception exception, UnityEngine.Object context)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.LogException(exception, context);
            }
        }

        /// <summary>
        /// Logs an assertion message to the console if the condition is false.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="message">The message to log if the condition is false.</param>
        public void Assert(bool condition, object message = null)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.Assert(condition, message);
            }
        }

        /// <summary>
        /// Logs an assertion message to the console if the condition is false, with a context object.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="message">The message to log if the condition is false.</param>
        /// <param name="context">The object to which the assertion applies.</param>
        public void Assert(bool condition, object message, UnityEngine.Object context)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.Assert(condition, message, context);
            }
        }

        /// <summary>
        /// Logs a formatted message to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogFormat(string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Info)
            {
                Debug.LogFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted message to the console with a context object.
        /// </summary>
        /// <param name="context">The object to which the message applies.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Info)
            {
                Debug.LogFormat(context, format, args);
            }
        }
        
        /// <summary>
        /// Logs a formatted warning message to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void WarningFormat(string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Warning)
            {
                Debug.LogWarningFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted warning message to the console with a context object.
        /// </summary>
        /// <param name="context">The object to which the warning applies.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void WarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Warning)
            {
                Debug.LogWarningFormat(context, format, args);
            }
        }

        /// <summary>
        /// Logs a formatted error message to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.LogErrorFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted error message to the console with a context object.
        /// </summary>
        /// <param name="context">The object to which the error applies.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void ErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                Debug.LogErrorFormat(context, format, args);
            }
        }

        /// <summary>
        /// Logs a formatted exception to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void ExceptionFormat(string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                string message = string.Format(format, args);
                Debug.LogException(new Exception(message));
            }
        }

        /// <summary>
        /// Logs a formatted exception to the console with a context object.
        /// </summary>
        /// <param name="context">The object to which the exception applies.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void ExceptionFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Error)
            {
                string message = string.Format(format, args);
                Debug.LogException(new Exception(message), context);
            }
        }

        /// <summary>
        /// Logs a formatted assertion message to the console if the condition is false.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void AssertFormat(bool condition, string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Error && !condition)
            {
                Debug.LogFormat(LogType.Assert, LogOption.None, null, format, args);
            }
        }

        /// <summary>
        /// Logs a formatted assertion message to the console if the condition is false, with a context object.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="context">The object to which the assertion applies.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
        {
            if (_currentLogLevel >= LogLevel.Error && !condition)
            {
                Debug.LogFormat(LogType.Assert, LogOption.None, context, format, args);
            }
        }

        /// <summary>
        /// Sets the current log level to control which messages are displayed.
        /// </summary>
        /// <param name="logLevel">The log level as an integer (0-4).</param>
        public void SetLevel(int logLevel)
        {
            if (logLevel is >= 0 and <= 4)
            {
                _currentLogLevel = (LogLevel)logLevel;
            }
            else
            {
                Debug.LogError($"Invalid log level: {logLevel}. Using default (All).");
                _currentLogLevel = LogLevel.All;
            }
        }

        /// <summary>
        /// Sets the current log level to control which messages are displayed.
        /// </summary>
        /// <param name="logLevel">The log level enum value.</param>
        public void SetLevel(LogLevel logLevel)
        {
            _currentLogLevel = logLevel;
        }
    }
}