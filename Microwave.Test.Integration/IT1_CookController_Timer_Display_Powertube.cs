using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;
using NuGet.Frameworks;
using NUnit.Framework.Internal.Execution;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    public class Tests
    {
        private ICookController _uut;
        private ITimer _timer;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private IOutput _output;
     
        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _timer = new Timer();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);

            _uut = new CookController(_timer, _display, _powerTube);
        }

        [TestCase(2000, 1100, "Display shows: 00:01")]
        [TestCase(1000, 1100, "Display shows: 00:00")]
        [TestCase(1000, 2100, "Display shows: 00:00")]
        public void StartCooking_DisplayShowTimeCallsOutput_DisplayShowsOutputCorrect_(int time, int sleep, string message)
        {
            // Arrange
            _uut.StartCooking(100, time);
            Thread.Sleep(sleep); // er SIKKER på at den har tikket.
            // Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains(message)));
        }

        [TestCase(1)]
        [TestCase(100)]
        [TestCase(50)]

        public void StartCooking_PowerTubeTurnOn_DisplayShowsTurnOn(int power)
        {
            // Arrange

            _uut.StartCooking(power,1000);

            // Assert

            _output.Received().OutputLine($"PowerTube works with { power }");
        }

        [TestCase(0)]
        [TestCase(101)]
        [TestCase(-50)]

        public void StartCooking_PowerTubeTurnOnOutOfRange_NotAllowed(int power)
        {


            // Arrange + Assert

            Assert.That(() => _uut.StartCooking(power,2000), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]

        public void StartCooking_PowerTubeTurnOnAlreadyOn_NotAllowed()
        {
            // Arrange

            _uut.StartCooking(50,2000);

            // Assert

            Assert.That(() => _uut.StartCooking(50,1000), Throws.TypeOf<ApplicationException>());
        }

        [Test]

        public void StartCooking_PowerTubeTurnOffAfterTimeIsUp_OutputIsCorrect()
        {
            // Arrange

            _uut.StartCooking(50,1000);

            Thread.Sleep(1100);

            // Assert

            _output.Received().OutputLine($"PowerTube turned off");
        }

        [Test]

        public void StartCooking_PowerTubeTurnOffAfterStopIsCalled_OutputIsCorrect()
        {
            // Arrange

            _uut.StartCooking(50, 1000);

            _uut.Stop();

            // Assert

            _output.Received().OutputLine($"PowerTube turned off");
        }

        [TestCase(-1000)]
        [TestCase(1000)]
        [TestCase(0)]

        public void StartCooking_TimerStart_TimeRemainingIsCorrect(int time)
        {
            // Arrange

            _uut.StartCooking(50,time);

            // Assert

            Assert.That(() => _timer.TimeRemaining, Is.EqualTo(time));

        }

        [Test]

        public void StartCooking_TimerStopCalled_IsCalledCorrectly()
        { 
            // Arrange

            _uut.StartCooking(50,60000);

            _uut.Stop();

            Thread.Sleep(2000);

            // Assert

            
            _output.DidNotReceive().OutputLine(Arg.Is<string>(str =>
                str.Contains("00:59")));

        }

        [Test]

        public void StartCooking_TimerEventOnTimerEvent_OutputIsCorrect()
        {
            // Arrange

            _uut.StartCooking(50, 2000);

            Thread.Sleep(2000);

            // Assert

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("00:01")));

        }

        [Test]

        public void StartCooking_TimerEventOnTimerExpired_OutputIsCorrect()
        {
            // Arrange

            _uut.StartCooking(50,1000);

            Thread.Sleep(2000);

            // Assert

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("off")));

        }




    }
}