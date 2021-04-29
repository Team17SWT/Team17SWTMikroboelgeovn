using System;
using System.IO;
using System.Threading;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
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
        

    }
}