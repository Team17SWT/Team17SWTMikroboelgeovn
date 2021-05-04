using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microwave.Classes.Interfaces
{
    public interface ICookController
    {
        public IUserInterface UI { get; set; } // HJÆLP
        void StartCooking(int power, int time);
        void Stop();
    }
}
