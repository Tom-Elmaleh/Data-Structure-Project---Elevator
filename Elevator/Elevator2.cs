using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Elevator
{
    public class Elevator2                                                                  // This class uses a more optimised process called the Elevator Algorithm 
    {
        public Queue<Passenger> requests { get; set; }                                       // Almost all  attributes are  the same as those of the Elevator1 class except up,down and goingdown
        public Queue<Passenger> goingdown { get; set; }                                      // The queue goingdown permits to store the requests for the passengers who want to go down
        public int elevatorposition = 0;
        public bool[] up { get; set; }                                                      // up [] indicates for each floor whether the elevator should stop (true) or not (false) when the elevator is going up
        public bool[] down { get; set; }                                                     // down [] indicates for each floor whether the elevator should stop (true) or not (false) when the elevator is going down
        List<int> PassengerPosition { get; set; }                                               
        public int nbpassengers;
        public List<Passenger> Passengerstatus { get; set; }                                
        public int nbpassengersretrieved { get; set; }
        public int nbpassengersdelivered { get; set; }
        public List<Passenger> differentFloor { get; set; }


        public Elevator2()                                          // Same constructor as the one from one for Elevator1 except we initialize up, down and goingdown as well
        {
            Passengerstatus = new List<Passenger>();
            up = new bool[7];                                     // The elevator contains 6 floors from 0 to 6
            down = new bool[7];
            PassengerPosition = new List<int>();
            requests = new Queue<Passenger>();
            goingdown = new Queue<Passenger>();
            nbpassengers = 0;
            differentFloor = new List<Passenger>();
        }

        public void Request(Passenger passenger)
        {
            requests.Enqueue(passenger);
        }

        /// <summary>
        /// Functions which ables the elevator to move by using direction. For this process direction is not set by the most recent request as in Elevator1. It changes from up to down (once all the request to go up are processed) and does the same for the request implying to go down and so on 
        /// </summary>
        public void Move()
        {
            Passenger passenger = null;
            string direction = "up";
            while (requests.Count != 0 )                           // if they are no more elements in the queues requests, the elevator will stop moving 
            {
                while (requests.Count != 0)                         // The objective is to have zero elements in requests which contains all the requests at the beginning
                {
                    passenger = requests.Dequeue();                          // we take off the most recent request from the queue                          
                    if (passenger.direction == "up")                        
                    {
                        Process(passenger, up);                              // We start by processing all the passengers who want to go up as the elevator will start by doing so
                    }

                    else
                    {
                        goingdown.Enqueue(passenger);                       // In the other hand we have to add to the passenger to goingdown without processing   
                    }
                }
                if (direction == "up")
                {
                    Up();                                                   // The elevator goes up
                    direction = "down";                                    // Once the elevator went to up he goes down
                }
                if (direction == "down")
                {
                    while (goingdown.Count != 0)                          // while they are no more request in going down we process each one of them
                    {
                        Process(goingdown.Dequeue(), down);             
                    }
                    Down();                                               // The elevator goes down
                    direction = "up";                                     // Once the elevator went down it goes up
                }
            }
        }

        /// <summary>
        /// Process ables to deal with the passenger's request. The method has the method works in the same way as in the class Elevator1 except it uses another parameter : [] direction which ables to use the right boolean array given the passenger's direction (either up or down) 
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="direction"></param>
        public void Process(Passenger pass, bool[] direction)
        {
            if (pass.requestedfloor != pass.initialposition)      // Takes into account the case in which  requestedfloor is wrongly specified by the passenger
            {
                if (elevatorposition == pass.initialposition)       // case in which the elevator is at the same position as the passenger waiting 
                {
                    if (nbpassengers <= 5)
                    {                                                      // There can't be more than 6 passengers inside the elevator
                        nbpassengers++;
                        direction[pass.requestedfloor] = true;             // We defined the requested floor as true in direction so that the elevator will stop there
                        pass.insideelevator = true;
                    }

                    else
                    {
                        pass.insideelevator = false;                        // Case in which the passenger can't get inside the elevator because it's at full capacity (inside elevator is used in UpdateNbPassengers)
                        pass.requestadded = false;
                    }

                }

                else                                                        // Case in which the passenger is not at the same position as the elevator
                {
                    pass.insideelevator = false;
                    PassengerPosition.Add(pass.initialposition);               // We also add the initial position of the passenger in differenetfloor as the elevator isn't at the same position as the passenger
                    differentFloor.Add(pass);
                }
                Passengerstatus.Add(pass);                         // we add the passenger to the list passengerstatus
            }
        }


        /// <summary>
        /// Method to process what should be done with with the passengers retreived and not retreived. Works in the same way as in Elevator1 except it uses another parameter [] direction which ables to use the right boolean array given the passenger's direction (either up or down)
        /// </summary>
        /// <param name="initialpos"></param>
        /// <param name="notretreived"></param>
        /// <param name="direction"></param>
        public void Search(int initialpos, int notretreived, bool[] direction)
        {
            int count = 0;
            int index = 0;

            if (nbpassengersretrieved > 0)                                                                // NB passengers retrieved
            {
                while (differentFloor.Count != 0 && count != nbpassengersretrieved)
                {
                    if (differentFloor[index].initialposition == initialpos && count != nbpassengersretrieved)
                    {
                        Passenger pass = differentFloor[index];
                        differentFloor.Remove(pass);                                                          
                        Passengerstatus.Remove(pass);
                        pass.insideelevator = true;                                                  
                        Passengerstatus.Add(pass);                                                  
                        direction[pass.requestedfloor] = true;                                      // up[i] or down[i] will be true so that the elevator will stop there
                        count++;
                        if (index > 0) { index--; }
                    }
                    else { index++; }
                }
            }

            count = 0;
            index = 0;
            if (notretreived > 0)                                                                      // Passengers who weren't retreived
            {
                while (differentFloor.Count != 0 && count != notretreived)
                {
                    if (differentFloor[index].initialposition == initialpos && count != notretreived)
                    {
                        Passenger pass = differentFloor[index];
                        differentFloor.Remove(pass);                                                          
                        Passengerstatus.Remove(pass);
                        pass.insideelevator = false;
                        requests.Enqueue(pass);
                        count++;
                        if (index > 0) { index--; }
                    }
                    else { index++; }
                }
            }
        }

        /// <summary>
        /// Method used to move the elevator upwards (we use the array up instead of floors). Works in the same way as in the class Elevator1
        /// </summary>
        public void Up()
        {
            int NotRetreived = 0;
            bool display = true;
            for (int i = 0; i < up.Length; i++)
            {
                nbpassengersretrieved = 0;
                if (up[i] == true && PassengerPosition.Contains(i) == false)       
                {
                    UpdateNbPassengers(i);
                    Console.WriteLine($"\nElevator went up from floor {elevatorposition} to floor number {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers} passenger(s) inside.");
                    nbpassengers -= nbpassengersdelivered;
                    Console.WriteLine($"It now has {nbpassengers} passenger(s) inside ");
                    elevatorposition = i;
                    up[i] = false;                                              
                }

                if (PassengerPosition.Contains(i) == true)                         
                {
                    display = true;
                    nbpassengersretrieved = 0;
                    while (PassengerPosition.Count != 0 && PassengerPosition.Contains(i) == true)                           
                    {
                        if (nbpassengers <= 5)
                        {
                            PassengerPosition.Remove(i);
                            nbpassengersretrieved++;
                            nbpassengers++;
                        }

                        else
                        {
                            PassengerPosition.Remove(i);
                            NotRetreived++;
                        }
                    }
                    Search(i, NotRetreived, up);
                    if (nbpassengersretrieved > 0)
                    {
                        Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} to get {nbpassengersretrieved} passenger(s) with {nbpassengers - nbpassengersretrieved} passengers inside ");
                        elevatorposition = i;
                        display = false;
                    }

                    UpdateNbPassengers(i);                                                       
                    if (nbpassengersdelivered > 0 && up[i] == true)                                                                                                     
                    {
                        nbpassengers -= nbpassengersdelivered;
                        if (display == false) { Console.WriteLine($"It also delivered {nbpassengersdelivered} passenger(s). It now has {nbpassengers} passenger(s)"); }
                        if (display == true)
                        {
                            Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers + nbpassengersdelivered} passengers inside. It now has {nbpassengers}");
                            elevatorposition = i;
                        }
                        up[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Method used to move the elevator downwards.(we use the array down instead of floors). Works in the same way as in the class Elevator1
        /// </summary>
        public void Down()
        {
            int NotRetreived = 0;
            bool display = true;
            for (int i = down.Length - 1; i >= 0; i--)
            {
                nbpassengersretrieved = 0;
                if (down[i] == true && PassengerPosition.Contains(i) == false)         
                {
                    UpdateNbPassengers(i);
                    Console.WriteLine($"\nElevator went down from floor {elevatorposition} to floor number {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers} passenger(s) inside");
                    nbpassengers -= nbpassengersdelivered;
                    Console.WriteLine($"It now has {nbpassengers} passenger(s) inside ");
                    elevatorposition = i;
                    down[i] = false;
                }

                if (PassengerPosition.Contains(i) == true)                           
                {
                    display = true;
                    while (PassengerPosition.Count != 0 && PassengerPosition.Contains(i) == true)                           
                    {
                        if (nbpassengers <= 5)
                        {
                            PassengerPosition.Remove(i);
                            nbpassengersretrieved++;
                            nbpassengers++;
                        }

                        else
                        {
                            PassengerPosition.Remove(i);
                            NotRetreived++;
                        }
                    }

                    Search(i, NotRetreived, down);

                    if (nbpassengersretrieved > 0)
                    {
                        Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} to get {nbpassengersretrieved} passenger(s) with {nbpassengers - nbpassengersretrieved} passengers inside ");
                        elevatorposition = i;
                        display = false;
                    }

                    UpdateNbPassengers(i);                                                
                    if (nbpassengersdelivered > 0 && down[i] == true)                                                                                                    
                    {
                        nbpassengers -= nbpassengersdelivered;
                        if (display == false) { Console.WriteLine($"It also delivered {nbpassengersdelivered} passenger(s). It now has {nbpassengers} passenger(s)"); }
                        if (display == true)
                        {
                            Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers + nbpassengersdelivered} passengers inside. It now has {nbpassengers}");
                            elevatorposition = i;
                        }
                        down[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Exactly the same method as the one from Elevator1
        /// </summary>
        /// <param name="position"></param>
        public void UpdateNbPassengers(int position)
        {
            int delivered = 0;
            bool removed = false;
            for (int i = 0; i < Passengerstatus.Count && Passengerstatus.Count != 0; i++)                                        
            {
                if (Passengerstatus[i].requestedfloor == position && Passengerstatus[i].insideelevator == true)                 
                {
                    delivered++;
                    Passengerstatus.RemoveAt(i);
                    removed = true;
                }

                else
                {
                    if (Passengerstatus[i].requestadded == false && Passengerstatus[i].insideelevator == false && nbpassengers <= 5)                    
                    {
                        Passengerstatus[i].requestadded = true;
                        requests.Enqueue(Passengerstatus[i]);
                        Passengerstatus.RemoveAt(i);
                        removed = true;
                    }
                    else
                    {
                        removed = false;
                    }
                }

                if (removed == true) { i = i - 1; }
            }
            nbpassengersdelivered = delivered;
        }
    }
}