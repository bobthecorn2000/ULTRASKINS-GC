using BepInEx.Logging;
using System;
using BepInEx;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using static UnityEngine.RemoteConfigSettingsHelper;
using plog;

namespace BatonPassLogger
{


    public interface IBatonPassTerminal
    {
        bool Enabled { get; }
        void Log(string message, BatonPass.LogLevel level);
    }

    // Enum for log levels
    public static class BatonPass
    {
#if DEBUG
        public const bool ShouldDoBatonPass = true;
#else
        public const bool ShouldDoBatonPass = false;
#endif
        public const bool ShouldDoBatonPassUnity = false;

        public static ManualLogSource BatonPassLogger = new ManualLogSource("BatonPass");

        
        public enum LogLevel
        {
            Info,
            Debug,
            Warning,
            Error,
            Fatal,
            Success
        }

        private static readonly List<IBatonPassTerminal> _terminals = new();

        // Registers a terminal to send logs to
        public static void RegisterTerminal(IBatonPassTerminal terminal)
        {
            if (!_terminals.Contains(terminal))
                _terminals.Add(terminal);
        }

        // Main logging method
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            foreach (var terminal in _terminals)
            {
                if (terminal.Enabled)
                    terminal.Log(message, level);
            }
        }

        // Convenience methods
        public static void Info(string message) => Log(message, LogLevel.Info);
        public static void Debug(string message) => Log(message, LogLevel.Debug);
        public static void Warn(string message) => Log(message, LogLevel.Warning);
        public static void Error(string message) => Log(message, LogLevel.Error);
        public static void Success(string message) => Log(message, LogLevel.Success);
        public static void Fatal(string message) => Log(message, LogLevel.Fatal);
    }

    // Console-based logging terminal
    public class ConsoleTerminal : IBatonPassTerminal
    {
        public bool Enabled { get; set; } = true;

        public void Log(string message, BatonPass.LogLevel level)
        {
            var color = Console.ForegroundColor;

            // Color based on log level
            Console.ForegroundColor = level switch
            {
                BatonPass.LogLevel.Debug => ConsoleColor.Cyan,
                BatonPass.LogLevel.Warning => ConsoleColor.Yellow,
                BatonPass.LogLevel.Error => ConsoleColor.Red,
                BatonPass.LogLevel.Success => ConsoleColor.Green,
                BatonPass.LogLevel.Fatal => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };

            Console.WriteLine($"[{level}] {message}");
            Console.ForegroundColor = color;
        }
    }

    // Unity-based logging terminal (optional, if using Unity)
    public class UnityTerminal : IBatonPassTerminal
    {
        public bool Enabled { get; set; } = true;

        public void Log(string message, BatonPass.LogLevel level)
        {
            string formatted = $"[{level}] {message}";

            switch (level)
            {
                case BatonPass.LogLevel.Warning:
                    Debug.LogWarning(formatted);
                    break;
                case BatonPass.LogLevel.Error:
                    Debug.LogError(formatted);
                    break;
                default:
                    Debug.Log(formatted);
                    break;
            }
        }
    }

    public class UKPlog : IBatonPassTerminal
    {
        public bool Enabled { get; set; } = true;
        private readonly plog.Logger _logSource;
        public UKPlog(string name)
        {
            _logSource = new plog.Logger(name);

        }

        public void Log(string message, BatonPass.LogLevel level)
        {
            string tag = $"[{level}] ";
            switch (level)
            {
                case BatonPass.LogLevel.Debug:
                    _logSource.Info(message);

                    break;
                case BatonPass.LogLevel.Warning:
                    _logSource.Warning(message);
                    break;
                case BatonPass.LogLevel.Error:
                    _logSource.Error(message);
                    break;
                case BatonPass.LogLevel.Success:
                    _logSource.Info(tag + "\u001b[32m" + message + "\u001b[0m");
                    break;
                case BatonPass.LogLevel.Fatal:
                    _logSource.Exception(message);
                    break;
                case BatonPass.LogLevel.Info:
                default:
                    _logSource.Info(message);
                    break;
            }
        }
    }
    public class BepInExTerminal : IBatonPassTerminal
    {
        private readonly ManualLogSource _logSource;
        public bool Enabled { get; set; } = true;

        public BepInExTerminal(ManualLogSource source)
        {
            _logSource = source;
            
        }

        public void Log(string message, BatonPass.LogLevel level)
        {
            string tag = $"[{level}] ";

            switch (level)
            {
                case BatonPass.LogLevel.Debug:
                    _logSource.LogDebug(message);
                    
                    break;
                case BatonPass.LogLevel.Warning:
                    _logSource.LogWarning(message);
                    break;
                case BatonPass.LogLevel.Error:
                    _logSource.LogError(message);
                    break;
                case BatonPass.LogLevel.Success:
                    _logSource.LogMessage(tag + "\u001b[32m" + message + "\u001b[0m");
                    break;
                case BatonPass.LogLevel.Fatal:
                    _logSource.LogFatal(message);
                    break;
                case BatonPass.LogLevel.Info:
                default:
                    _logSource.LogInfo(message);
                    break;
            }
        }
    }

}

