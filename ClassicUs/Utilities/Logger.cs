using Reactor.Utilities;

namespace ClassicUs.Utilities;

public static class ClassicLogger
{
    public enum LogFlags
    {
        Log,
        Debug,
        Warn,
        Error,
        Fatal,
    }

    public static void Log(string text, LogFlags flags = LogFlags.Log)
    {
        switch (flags)
        {
            case LogFlags.Log:
                Logger<ClassicUsPlugin>.Info(text);
                break;
            case LogFlags.Debug:
                Logger<ClassicUsPlugin>.Debug(text);
                break;
            case LogFlags.Warn:
                Logger<ClassicUsPlugin>.Warning(text);
                break;
            case LogFlags.Error:
                Logger<ClassicUsPlugin>.Error(text);
                break;
            case LogFlags.Fatal:
                Logger<ClassicUsPlugin>.Fatal(text);
                break;
        }
    }
}