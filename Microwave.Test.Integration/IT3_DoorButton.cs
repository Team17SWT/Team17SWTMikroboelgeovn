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
    public class IT3_DoorButton
    {
        private IButton _powerButton;
        private IButton _startCancelButton;
        private IButton _timeButton;
        private IDoor _door;
        private IDisplay _display;
        private IUserInterface _userInterface;
        private IOutput _output;
        private IPowerTube _powerTube;
        private ICookController _cookController;
        private ITimer _timer;
        private ILight _light;

        [SetUp]

        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<Timer>();
            _light = new Light(_output);
            _powerButton = new Button();
            _startCancelButton = new Button();
            _timeButton = new Button();
            _door = new Door();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light,
                _cookController);

            _cookController.UI = _userInterface;
        }


        [Test]

        public void Door_DoorOpenLightsOn_OutputIsCorrect()
        {
            // Arrange

            _door.Open();

            // Assert

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Light") &&
                str.Contains("on")));
        }

        [Test]

        public void Door_DoorCloseLightsOff_OutputIsCorrect()
        {
            // Arrange
            _door.Open();
            _door.Close();

            // Assert

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Light") &&
                str.Contains("off")));
        }

        [TestCase(1,50)]
        [TestCase(2,100)]
        [TestCase(14,700)]

        public void PowerButton_ButtonPressed_OutputIsCorrect(int pressed, int watt)
        {
            // Arrange
            for (int i = 0; i < pressed; i++)
            {
                _powerButton.Press();
            }

            // Assert

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains($"{watt} W")));
        }

        [Test]
        public void PowerButton_ButtonPressedMoreThanMax_OutputIsNotCorrect()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _powerButton.Press();
            }

            // Assert

            _output.DidNotReceive().OutputLine(Arg.Is<string>(str =>
                str.Contains($"750 W")));
        }

        [TestCase(1,01)]
        [TestCase(61,61)]

        public void TimeButton_ButtonPressed_OutputIsCorrect(int time,int output)
        {
            // Arrange
            _powerButton.Press();

            for (int i = 0; i < time; i++)
            {
                _timeButton.Press();
            }

            // Assert

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains($"{output}:00")));
        }


    }
}