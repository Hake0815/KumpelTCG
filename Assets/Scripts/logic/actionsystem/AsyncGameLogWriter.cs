using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using gamecore.common;
using Newtonsoft.Json;

namespace gamecore.actionsystem
{
    public class AsyncGameLogWriter : IDisposable
    {
        private const int FLUSH_INTERVAL_MS = 100;

        private readonly string _filePath;
        private readonly ConcurrentQueue<GameActionLogEntry> _queue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _writerTask;
        private readonly ManualResetEventSlim _finishLogEvent = new(false);
        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        private bool _disposed;

        public AsyncGameLogWriter(string filePath)
        {
            _filePath = filePath;
            _writerTask = Task.Run(WriteLoopAsync);
        }

        public void Log(GameActionLogEntry entry)
        {
            _queue.Enqueue(entry);
        }

        public void FinishLog()
        {
            if (_disposed)
                return;

            // Signal to stop the write loop and process all remaining entries
            _finishLogEvent.Set();
            _cts.Cancel();
        }

        public List<GameActionLogEntry> LoadExistingLog()
        {
            var entries = new List<GameActionLogEntry>();

            if (!File.Exists(_filePath))
            {
                GlobalLogger.Instance.Error(() => $"ERROR: No log file found at {_filePath}");
                return entries;
            }

            using var stream = new FileStream(
                _filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite
            );
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    DeserializeLine(entries, line);
                }
            }

            return entries;
        }

        private static void DeserializeLine(List<GameActionLogEntry> entries, string line)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<GameActionLogEntry>(
                    line,
                    _serializerSettings
                );
                if (obj != null)
                    entries.Add(obj);
            }
            catch (JsonSerializationException ex)
            {
                GlobalLogger.Instance.Error(
                    () =>
                        $"ERROR: Deserialization error when processing line: {line}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}"
                );
            }
            catch (JsonReaderException ex)
            {
                GlobalLogger.Instance.Error(
                    () => $"ERROR: JSON read error: {ex.Message}\nStackTrace: {ex.StackTrace}"
                );
            }
            catch (Exception ex)
            {
                GlobalLogger.Instance.Error(
                    () =>
                        $"ERROR: Unknown error during deserialization: {ex.Message}\nStackTrace: {ex.StackTrace}"
                );
            }
        }

        private async Task WriteLoopAsync()
        {
            using var stream = new FileStream(
                _filePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read
            );
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            while (
                !_cts.Token.IsCancellationRequested || (!_queue.IsEmpty && _finishLogEvent.IsSet)
            )
            {
                while (_queue.TryDequeue(out var item))
                {
                    var jsonEntry = JsonConvert.SerializeObject(item, _serializerSettings);
                    await writer.WriteLineAsync(jsonEntry);
                }
                stream.Flush(true);

                // Only delay if we are not finishing and not cancelled
                if (!_finishLogEvent.IsSet && !_cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(FLUSH_INTERVAL_MS, _cts.Token).ContinueWith(_ => { });
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _cts.Cancel();
                _writerTask.Wait();
                _cts.Dispose();
                _finishLogEvent.Dispose();
            }

            _disposed = true;
        }

        ~AsyncGameLogWriter()
        {
            Dispose(false);
        }
    }
}
