namespace autopi.net.core
{
    public class Logger : ILogger
    {
        public void Error(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            System.Diagnostics.Trace.TraceError(System.DateTime.Now + string.Format(" - ERROR - " + message, arg1, arg2, arg3, arg4));
        }
        public void Error(System.Exception exception, string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            System.Diagnostics.Trace.TraceError(System.DateTime.Now + string.Format(" - ERROR - " + message, arg1, arg2, arg3, arg4, exception));
        }

        public void Info(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            System.Diagnostics.Trace.TraceInformation(System.DateTime.Now + string.Format(" - INFO - " + message, arg1, arg2, arg3, arg4));
        }
        public void Debug(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            System.Diagnostics.Trace.WriteLine(System.DateTime.Now + string.Format(" - DEBUG - " + message, arg1, arg2, arg3, arg4));
        }

        public void Fatal(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            System.Diagnostics.Trace.TraceError(System.DateTime.Now + string.Format(" - FATAL - " + message, arg1, arg2, arg3, arg4));
        }
        public void Fatal(System.Exception exception, string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            System.Diagnostics.Trace.TraceError(System.DateTime.Now + string.Format(" - FATAL - " + message, arg1, arg2, arg3, arg4, exception));
        }

    }

    public interface ILogger
    {
        void Error(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null);
        void Error(System.Exception exception, string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null);
        void Info(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null);
        void Debug(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null);
        void Fatal(string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null);
        void Fatal(System.Exception exception, string message, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null);

    }
}