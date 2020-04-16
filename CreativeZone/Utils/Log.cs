
using System;

namespace CommunityBalancer.Utils
{
    static class Log
    {
        public static IPrinter Error { get; }
        public static IPrinter Debug { get; }
        public static IPrinter Default { get; }

        static Log()
        {
            var defaultFile = new FilePrinter("CommunityBalancer.log");
            var debugFile = new FilePrinter("CommunityBalancer_Debug.log");
            var errorFile = new FilePrinter("CommunityBalancer_Error.log");

            Debug = debugFile;
            Default = CompoundPrinter.Make(defaultFile, debugFile);
            Error = CompoundPrinter.Make
            (
                errorFile,
                new PrefixPrinter(debugFile, "Error: "),
                new PrefixPrinter(defaultFile, "Error: ")
            );

           AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CloseLog(Error);
            CloseLog(Debug);
            CloseLog(Default);

            void CloseLog(IPrinter printer)
            {
                printer.Print("Log closed");
                printer.PrintNewLine();

                if(printer is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }
    }
}
