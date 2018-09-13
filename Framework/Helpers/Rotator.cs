using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Rotates an angle over time
    /// </summary>
    public class Rotator
    {
        // Usage:
        // var rotator = new Rotator
        // {
        //     RotateDuration = TimeSpan.FromSeconds(10),
        //     StartDelay = TimeSpan.FromSeconds(3),
        //     RotateAmount = 360,
        //     RotateAntiClockwise = true,
        //     StartAngleDegrees = 45
        // };
        // Task.FromResult(rotator.Rotate());

        public Rotator()
        {
            Id = Guid.NewGuid();
        }

        private float? _angle;
        private Stopwatch _timer;
        private float _speedMillisecondsPerDegree;
        private float _degreesRotatedSinceStart;

        /// <summary>
        /// Unique Id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Time to spend rotating
        /// </summary>
        public TimeSpan RotateDuration { get; set; }

        /// <summary>
        /// Time to wait before rotating
        /// </summary>
        public TimeSpan StartDelay { get; set; }

        /// <summary>
        /// Starting rotation
        /// </summary>
        public float StartAngleDegrees { get; set; }

        /// <summary>
        /// Time that animation was started
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// If condition is true the rotation will stop
        /// </summary>
        public Func<bool> StopCondition { get; set; }

        /// <summary>
        /// If rotation should be clockwise
        /// </summary>
        public bool RotateAntiClockwise { get; set; }

        /// <summary>
        /// If rotator is currently working.
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Maximum amount to rotate
        /// </summary>
        public int RotateAmount = 360;

        /// <summary>
        /// If 'clockwise' is going the wrong way, flip it.
        /// </summary>
        public bool FlipRotation { get; set; }

        /// <summary>
        /// Milliseconds between updates.
        /// </summary>
        public int TickDelay = 25;

        /// <summary>
        /// Decimal places to round angle to
        /// </summary>
        public int Precision = 2;

        /// <summary>
        /// If we should spew log messages
        /// </summary>
        public bool DebugLogging;

        /// <summary>
        /// Current rotation in degrees
        /// </summary>
        public float Angle
        {
            get
            {
                if (_angle.HasValue)
                    return _angle.Value;

                return 0;
            }
            set => _angle = value;
        }

        /// <summary>
        /// Step rotation to next position
        /// </summary>
        public async Task<bool> Rotate()
        {
            if (!_angle.HasValue)
                await Start();

            while (_timer.Elapsed.TotalMilliseconds <= RotateDuration.TotalMilliseconds && _timer.IsRunning)
            {
                if (StopCondition != null && StopCondition())
                    Stop();

                Angle = GetAngle(_timer.Elapsed.TotalMilliseconds);

                if (DebugLogging)
                    Core.Logger.Verbose("Id={0} Angle={1} ElapsedMs={2} StartDegrees={3}",
                        Id, Angle, _timer.Elapsed.TotalMilliseconds, StartAngleDegrees);

                if (_degreesRotatedSinceStart > RotateAmount)
                    Stop();

                if (TickDelay > 5)
                    await Task.Delay(TickDelay);
            }

            return true;
        }

        private float GetAngle(double elapsedMilliseconds)
        {
            float totalAngle;

            _degreesRotatedSinceStart = (float)elapsedMilliseconds * _speedMillisecondsPerDegree;

            if (RotateAntiClockwise)
                totalAngle = StartAngleDegrees - _degreesRotatedSinceStart;
            else
                totalAngle = StartAngleDegrees + _degreesRotatedSinceStart;

            var boundAngle = (float)Math.Round(MathUtil.FixAngleTo360(totalAngle), Precision, MidpointRounding.AwayFromZero);

            if (DebugLogging)
                Core.Logger.Verbose("TotalAngle={0} BoundAngle={1} DegreesRotated={2}", totalAngle, boundAngle, _degreesRotatedSinceStart);

            return boundAngle;
        }

        /// <summary>
        /// Get the angle for a future point in time
        /// </summary>
        public double GetFutureAngle(TimeSpan futureAmount)
        {
            if (!IsRunning)
                return Angle;

            var elapsedMilliseconds = _timer.Elapsed.TotalMilliseconds + futureAmount.TotalMilliseconds;
            return GetAngle(elapsedMilliseconds);
        }

        /// <summary>
        /// Stop/Pause rotation
        /// </summary>
        private void Stop()
        {
            _timer.Stop();

            if (IsRunning)
            {
                if (DebugLogging)
                    Core.Logger.Verbose("Stopped Rotation {0}", Id);

                IsRunning = false;
            }
        }

        /// <summary>
        /// Reset rotation to default state
        /// </summary>
        private void Reset()
        {
            Stop();
            _timer.Reset();
            _angle = null;
        }

        /// <summary>
        /// Start rotating
        /// </summary>
        private async Task<bool> Start()
        {
            IsRunning = true;

            if (RotateDuration.TotalMilliseconds > 0)
                _speedMillisecondsPerDegree = RotateAmount / (float)RotateDuration.TotalMilliseconds;
            else
                _speedMillisecondsPerDegree = 0f;

            if (DebugLogging)
                Core.Logger.Verbose("Starting Rotation {0} StartAngle={1} Speed={2} Clockwise={3} DurationMs={4} StartDelay={5} RotateAmount={6} Flipped={7}",
                    Id, StartAngleDegrees, _speedMillisecondsPerDegree, !RotateAntiClockwise, RotateDuration.TotalMilliseconds, StartDelay.TotalMilliseconds, RotateAmount, FlipRotation);

            if (StartDelay.TotalMilliseconds > 0)
            {
                if (DebugLogging)
                    Core.Logger.Verbose("Waiting {0}ms", StartDelay.TotalMilliseconds);

                await Task.Delay(StartDelay);
            }

            StartTime = DateTime.UtcNow;
            _timer = Stopwatch.StartNew();
            return true;
        }
    }
}