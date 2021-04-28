using System.IO;
using System.Threading;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
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
        public void StartCooking_ShowTimeCallsOutput_DisplayShowsOutputCorrect_(int time, int sleep, string message)
        {
            // Arrange
            _uut.StartCooking(100, time);
            Thread.Sleep(sleep); // er SIKKER på at den har tikket.
            // Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains(message)));
        }
        

    }
}