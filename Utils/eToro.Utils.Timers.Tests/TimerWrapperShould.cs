using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace eToro.Utils.Timers.Tests
{
    [TestFixture]
    public class TimerWrapperShould
    {
        private const double INTERVAL_IN_MILLISECONDS = 20;

        private ITimerWrapper CreateTimer()
        {
            var timer = new TimerWrapper();
            timer.Initialize(INTERVAL_IN_MILLISECONDS);
            return timer;
        }

        [Test]
        public void Start()
        {
            // Given
            var timer = CreateTimer();
            var counter = 0;
            timer.Elapsed += (sender, args) =>
            {
                counter++;
            };

            // When
            timer.Start();
            Thread.Sleep(200);

            // Then
            counter.ShouldBeGreaterThan(0);
        }

        [Test]
        public void Stop()
        {
            // Given
            var timer = CreateTimer();
            var counter = 0;
            timer.Elapsed += (sender, args) =>
            {
                counter++;
            };
            timer.Start();
            Thread.Sleep(200);
            counter = 0;

            // When
            timer.Stop();
            Thread.Sleep(200);


            // Then
            counter.ShouldBe(0);
        }


        [Test]
        public void Dispose()
        {
            // Given
            var timer = CreateTimer();
            var counter = 0;
            timer.Elapsed += (sender, args) =>
            {
                counter++;
            };
            timer.Start();
            Thread.Sleep(200);
            counter = 0;

            // When
            timer.Dispose();
            Thread.Sleep(200);


            // Then
            counter.ShouldBe(0);
        }
    }
}
