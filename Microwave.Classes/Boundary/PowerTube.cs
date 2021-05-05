using System;
using Microwave.Classes.Interfaces;

namespace Microwave.Classes.Boundary
{
    public class PowerTube : IPowerTube
    {
        private IOutput myOutput;

        private bool IsOn = false;

        public PowerTube(IOutput output)
        {
            myOutput = output;
        }

        public void TurnOn(int power)
        {
            if (power < 50 || 700 < power) // ÆNDRET FRA 100 TIL 700 (PROCENT TIL WATT) OG FRA 1 TIL 50 (USE CASE)
            {
                throw new ArgumentOutOfRangeException("power", power, "Must be between 50 and 700 (incl.)"); //ændret 100 til 700 og 1 til 50
            }

            if (IsOn)
            {
                throw new ApplicationException("PowerTube.TurnOn: is already on");
            }

            myOutput.OutputLine($"PowerTube works with {power}");
            IsOn = true;
        }

        public void TurnOff()
        {
            if (IsOn)
            {
                myOutput.OutputLine($"PowerTube turned off");
            }

            IsOn = false;
        }
    }
}