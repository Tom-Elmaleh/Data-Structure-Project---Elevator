using System;
using System.Collections.Generic;

namespace Elevator
{
    class Program
    {
        static void Main(string[] args)
        {
            Elevator2 elevator = new Elevator2();
            elevator.Request(new Passenger(0, 0, "down"));
            elevator.Request(new Passenger(0,2,"up"));
            elevator.Request(new Passenger(2,1,"down"));
            elevator.Request(new Passenger(0,3,"up"));
            elevator.Request(new Passenger(3,2, "down"));
            elevator.Request(new Passenger(0,4,"up"));
            elevator.Request(new Passenger(6, 2 ,"down")) ;
            elevator.Request(new Passenger(0,6,"up"));
            elevator.Request(new Passenger(6, 4, "down"));
            elevator.Request(new Passenger(0, 5, "up"));
            elevator.Request(new Passenger(5, 4, "down"));
            elevator.Request(new Passenger(0,1, "up"));
            elevator.Request(new Passenger(3,0, "down"));
            elevator.Move();
        }
    }
}
