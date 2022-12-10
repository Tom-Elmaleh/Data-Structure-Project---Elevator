using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Elevator
{
    public class Passenger                               // Class used to store the important informations related to the passenger        
    {
        public int initialposition { get; set; }
        public int requestedfloor { get; set; }
        public string direction { get; set; }
        public bool insideelevator { get; set; }
        public bool requestadded { get; set; }

        public Passenger(int _initialposition, int _requestedfloor, string _direction)
        {
            initialposition = _initialposition;
            requestedfloor = _requestedfloor;
            direction = _direction;
            insideelevator = false;                                                               // used to know if the passenger is inside the elevator. This variable is used in UpdateNbPassengers(pos) and Search methods.
            requestadded = true;                                                                // used to know if the request can be fullfilled (given the capacity) and to add it again if it can't (this variable is used in UpdateNbPassengers(pos))
                                                                                                // The requestadded is set by default to true 
        }
    }
}

