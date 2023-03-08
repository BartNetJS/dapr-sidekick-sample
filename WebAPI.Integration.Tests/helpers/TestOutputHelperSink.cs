using Serilog.Core;
using Serilog.Events;
using Xunit.Abstractions;

namespace WebAPI.Integration.Tests.helpers
{
    public class TestOutputHelperSink : ILogEventSink
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestOutputHelperSink(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Emit(LogEvent logEvent)
        {
            var logOutput = new StringWriter();
            logEvent.RenderMessage(logOutput);
            try
            {
                _testOutputHelper.WriteLine(logOutput.ToString());
            }
            catch
            {
                Console.Write(logOutput.ToString());
            }
        }
    }
}
