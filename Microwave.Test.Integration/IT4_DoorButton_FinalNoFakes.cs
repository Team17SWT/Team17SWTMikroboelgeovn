using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using NuGet.Frameworks;
using NUnit.Framework.Internal.Execution;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    public class IT4_DoorButton_FinalNoFakes
    {
        private StringWriter _str;
        private IButton _powerButton;
        private IButton _startCancelButton;
        private IButton _timeButton;
        private IDoor _door;
        private IDisplay _display;
        private IUserInterface _userInterface;
        private IOutput _output;
        private IPowerTube _powerTube;
        private CookController _cookController;
        private ITimer _timer;
        private ILight _light;

        [SetUp]

        public void Setup()
        {
            _str = new StringWriter();
            Console.SetOut(_str);
            _output = new Output();
            _timer = new Timer();
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

            Assert.That(_str.ToString().Contains("Light is turned on"));
        }

        [Test]

        public void Door_DoorCloseLightOff_OutputIsCorrect()
        {
            // Arrange

            _door.Open();
            _door.Close();

            // Assert

            Assert.That(_str.ToString().Contains("Light is turned off"));
        }

        [TestCase(1, 50)]
        [TestCase(2, 100)]
        [TestCase(14, 700)]

        public void PowerButton_ButtonPressed_OutputIsCorrect(int pressed, int watt)
        {
            // Arrange
            for (int i = 0; i < pressed; i++)
            {
                _powerButton.Press();
            }

            // Assert

            Assert.That(_str.ToString().Contains($"{watt} W"));
        }

        [Test]
        public void PowerButton_ButtonPressedMoreThanMax_OutputIsCorrect()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _powerButton.Press();
            }

            // Assert

            Assert.That(_str.ToString().Contains($"50 W"));
            
        }

        [TestCase(1, 01)]
        [TestCase(61, 61)]

        public void TimeButton_ButtonPressed_OutputIsCorrect(int time, int output)
        {
            // Arrange
            _powerButton.Press();

            for (int i = 0; i < time; i++)
            {
                _timeButton.Press();
            }

            // Assert

            Assert.That(_str.ToString().Contains($"{output}:00"));

        }

        [Test]

        public void StartCancelButton_ButtonPressedWhenNotCooking_OutputIsCorrect()
        {
            // Arrange
            
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            Thread.Sleep(61000);

            // Assert

            Assert.That(_str.ToString().Contains($"Light is turned on"));
            Assert.That(_str.ToString().Contains($"Display shows: 01:00"));
            Assert.That(_str.ToString().Contains($"Display shows: 00:59"));
            Assert.That(_str.ToString().Contains($"Display shows: 00:05"));
            Assert.That(_str.ToString().Contains($"PowerTube works with 50"));
            Assert.That(_str.ToString().Contains($"Display cleared"));
            Assert.That(_str.ToString().Contains($"Light is turned off"));
        }

        [Test]

        public void StartCancelButton_ButtonPressedWhenCooking_OutputIsCorrect()
        {
            // Arrange

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            // Assert

            Assert.That(_str.ToString().Contains($"PowerTube turned off"));
            Assert.That(_str.ToString().Contains($"Light is turned off"));
            Assert.That(_str.ToString().Contains($"Display cleared"));
        }

        [Test]

        public void Door_DoorOpenWhenCooking_OutputIsCorrect()
        {
            // Arrange

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _door.Open();

            // Assert

            Assert.That(_str.ToString().Contains($"PowerTube turned off"));
            Assert.That(_str.ToString().Contains($"Display cleared"));
            
        }
    }
}