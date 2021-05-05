using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    public class IT2_UserInterface_Light_Display_CookController
    {
        private IUserInterface _uut;
        private CookController _cookController;
        private ITimer _timer;
        private ILight _light;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private IOutput _output;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _startCancelButton;
        private IButton _timeButton;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<ITimer>(); // HJÆLP
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _cookController = new CookController(_timer, _display, _powerTube, _uut);
            _light = new Light(_output);

            //Has to be made so uut can be made.
            _door = Substitute.For<IDoor>();
            _powerButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();

            _uut = new UserInterface(_powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookController);

            _cookController.UI = _uut; // HER SKAL UI SÆTTES TIL VORES _UUT. 
        }

        [Test]
        public void OnDoorOpenedFromReady_LightTurnOn_OutputShowsOn()
        {
            //Arrange
            _uut.OnDoorOpened(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("on")));
        }

        [Test]
        public void OnDoorClosedFromDoorIsOpen_LightTurnOff_OutputShowsOff()
        {
            //Arrange
            _uut.OnDoorOpened(this, EventArgs.Empty);
            _uut.OnDoorClosed(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("off")));
        }

        [Test]
        public void OnStartCancelPressedFromSetTime_LightTurnOn_OutputShowsOff()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("on")));
        }

        [Test]
        public void CookingDone_LightTurnOff_OutputShowsOff() 
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            _timer.Expired += Raise.Event();
            
            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Light is turned off")));
        }

        [Test]
        public void OnStartCancelPressedFromCooking_LightTurnOff_OutputShowsOff() 
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);


            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Light is turned off")));
        }

        [Test]
        public void OnPowerPressedFromReady_DisplayShowPower_OutputShowsPower() 
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display shows: 50 W")));
        }

        [Test]
        public void OnTimePressedFromSetPower_DisplayShowTime_OutputShowsTime()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display shows: 01:00")));
        }

        [Test]
        public void CookingDone_DisplayClear_OutputShowsCleared()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            _timer.Expired += Raise.Event();

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display cleared")));
        }

        [Test]
        public void OnDoorOpenedFromCooking_DisplayClear_OutputShowsCleared()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _uut.OnDoorOpened(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display cleared")));
        }

        [Test]
        public void OnStartCancelPressedCalledFromCooking_DisplayClear_OutputShowsCleared()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display cleared")));
        }

        [Test]
        public void OnStartCancelPressedCalledFromSetTime_CookControllerStartsCooking_OutputShowsRightResults()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);


            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("50 W")));

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("02:00")));

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("01:00")));
        }

        [Test]
        public void OnTimerExpiredFromCooking_UICookingIsDone_OutputShowsRightResults()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            _timer.Expired += Raise.Event();

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("cleared")));

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Light is turned off")));

        }

        [Test]
        public void OnDoorOpenedFromCooking_CookControllerStop_OutputShowsRightResults()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            _uut.OnDoorOpened(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("PowerTube turned off")));

        }

        [Test]
        public void OnStartCancelPressedFromCooking_CookControllerStop_OutputShowsRightResults()
        {
            //Arrange
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnTimePressed(this, EventArgs.Empty);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            _uut.OnStartCancelPressed(this, EventArgs.Empty);

            //Assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("PowerTube turned off")));

        }
    }
}
