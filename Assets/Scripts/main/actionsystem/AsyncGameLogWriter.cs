using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gamecore.actionsystem
{
    public class AsyncGameLogWriter : IDisposable
    {
        private const int BUFFER_SIZE = 64;
        private const int FLUSH_INTERVAL_MS = 100;

        private readonly string _filePath;
        private readonly ConcurrentQueue<GameActionLogEntry> _queue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _writerTask;
        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
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

        public List<GameActionLogEntry> LoadExistingLog()
        {
            var entries = new List<GameActionLogEntry>();

            if (!File.Exists(_filePath))
                return entries;

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
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<GameActionLogEntry>(line);
                        if (obj != null)
                            entries.Add(obj);
                    }
                    catch
                    {
                        // Skip bad lines
                    }
                }
            }

            return entries;
        }

        private async Task WriteLoopAsync()
        {
            using var stream = new FileStream(
                _filePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read
            );
            using var writer = new StreamWriter(stream) { AutoFlush = false };

            var buffer = new List<GameActionLogEntry>(BUFFER_SIZE);
            var flushInterval = TimeSpan.FromMilliseconds(FLUSH_INTERVAL_MS);
            var lastFlush = DateTime.UtcNow;

            while (!_cts.Token.IsCancellationRequested)
            {
                while (_queue.TryDequeue(out var item))
                {
                    buffer.Add(item);
                    if (buffer.Count >= BUFFER_SIZE)
                        break;
                }

                foreach (var entry in buffer)
                {
                    var json = JsonConvert.SerializeObject(entry, _serializerSettings);
                    await writer.WriteLineAsync(json);
                }

                buffer.Clear();

                if (DateTime.UtcNow - lastFlush >= flushInterval)
                {
                    await writer.FlushAsync();
                    stream.Flush(true);
                    lastFlush = DateTime.UtcNow;
                }

                await Task.Delay(FLUSH_INTERVAL_MS, _cts.Token).ContinueWith(_ => { });
            }

            // Final flush
            await writer.FlushAsync();
            stream.Flush(true);
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
            }

            _disposed = true;
        }

        ~AsyncGameLogWriter()
        {
            Dispose(false);
        }
    }
}
