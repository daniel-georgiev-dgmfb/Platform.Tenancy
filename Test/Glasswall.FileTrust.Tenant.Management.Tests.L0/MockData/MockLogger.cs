using System;

namespace Platform.Tenant.Management.Tests.L0.MockData
{
	internal class MockLogger<T> : IGWLogger<T>
    {
        private readonly Action _onLog;

        public MockLogger(Action onLog = null)
        {
            this._onLog = onLog ?? new Action(() => { });
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this._onLog();
        }
    }
}