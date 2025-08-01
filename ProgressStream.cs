using System;

public class ProgressStream : Stream
{
    private readonly Stream _baseStream;
    private readonly IProgress<string> _progress;
    private long _totalRead;
    private readonly long _totalBytes;
    private readonly System.Diagnostics.Stopwatch _stopwatch;
    private int _lastReportedPercentage = 0;

    public ProgressStream(Stream baseStream, long totalBytes, System.Diagnostics.Stopwatch stopwatch, IProgress<string> progress)
    {
        _baseStream = baseStream;
        _progress = progress;
        _totalBytes = totalBytes;
        _stopwatch = stopwatch ?? throw new ArgumentNullException(nameof(stopwatch));
    }

    public override bool CanRead => _baseStream.CanRead;

    public override bool CanSeek => _baseStream.CanSeek;

    public override bool CanWrite => _baseStream.CanWrite;

    public override long Length => _baseStream.Length;

    public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }

    public override void Flush()
    {
        _baseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int read = _baseStream.Read(buffer, offset, count);
        _totalRead += read;
        _progress?.Report(_totalRead.ToString());
        return read;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int read = await _baseStream.ReadAsync(buffer, offset, count, cancellationToken);
        _totalRead += read;
        var percentage = (int)((_totalRead * 100) / _totalBytes);
        if (percentage > _lastReportedPercentage)
        {
            _lastReportedPercentage = percentage;
            var elapsed = _stopwatch.Elapsed;
            var totalEstimated = TimeSpan.FromMilliseconds(elapsed.TotalMilliseconds * 100 / percentage);
            TimeSpan? estimatedRemaining = totalEstimated - elapsed;
            string timeLeftStr = estimatedRemaining.HasValue
                ? $"Estimated finish time: {DateTime.Now.Add(estimatedRemaining.Value):HH:mm:ss}"
                : "Estimating...";

            _progress?.Report($"Copying... {percentage}%. {timeLeftStr}");
        }
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _baseStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _baseStream.Write(buffer, offset, count);
    }

    // Implement other required Stream members by delegating to _baseStream...
}