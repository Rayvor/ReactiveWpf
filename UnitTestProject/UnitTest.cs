using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var scheduler = new TestScheduler();

            var input = scheduler.CreateHotObservable(
                ReactiveTest.OnNext(100, "abc"),
                ReactiveTest.OnNext(200, "def"),
                ReactiveTest.OnNext(300, "ghi"),
                ReactiveTest.OnNext(400, "pqr"),
                ReactiveTest.OnNext(500, "test"),
                ReactiveTest.OnNext(600, "xyz"),
                ReactiveTest.OnCompleted<string>(800)
                );

            var results = scheduler.Start(
                () => input.Buffer(() => input.Throttle(TimeSpan.FromTicks(100), scheduler))
                           .Select(b => string.Join(",", b)),
                created: 0,
                subscribed: 100,
                disposed: 800);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<string>>[] {
                ReactiveTest.OnNext(700, "def,ghi,pqr,test,xyz"),
                ReactiveTest.OnNext(800, ""),
                ReactiveTest.OnCompleted<string>(800)
            });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                ReactiveTest.Subscribe(100, 800),
                ReactiveTest.Subscribe(100, 700),
                ReactiveTest.Subscribe(700, 800)
            });
        }
    }
}
