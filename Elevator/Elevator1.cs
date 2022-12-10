using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Elevator
{
    public class Elevator1                                                                   // This class makes the elevator based on the first in first out principle (less optimised)
    {
        public Queue<Passenger> requests { get; set; }                                       // used to store the requests for each passenger 
        public int elevatorposition = 0;                                                     // gives the actual position of the elevator
        public bool[] floors { get; set; }                                                  // floors [] indicates for each floor whether the elevator should stop (true) or not (false) 
        public List<int> PassengerPosition { get; set; }                                        // List which contains the position of the passengers who are not at the same position as the elevator
        public int nbpassengers;                                                             // Number of passengers inside the elevator
        public List<Passenger> Passengerstatus { get; set; }                                 // List storing each passenger of the elevator. It's used to determine how many passengers were delivered at a specific floor and to add a passenger's request in case the elevator reached a full capacity. we update it given how many passengers were delivered and retreived 
        public int nbpassengersretrieved { get; set; }
        public int nbpassengersdelivered { get; set; }
        public List<Passenger> differentFloor { get; set; }                                            // Store the passengers who are not at the same position as the elevator

        /// <summary>
        /// Create an elevator, we initialise PassengerStatus,floors,PassengerPosition,request,nbpassengers and differentFloor
        /// </summary>
        public Elevator1()
        {
            Passengerstatus = new List<Passenger>();
            floors = new bool[7];                                      // The elevator contains 6 floors from 0 to 6 
            PassengerPosition = new List<int>();
            requests = new Queue<Passenger>();
            nbpassengers = 0;
            differentFloor = new List<Passenger>();
        }

        /// <summary>
        /// Method used to add each requests to the queue requests.
        /// </summary>
        /// <param name="passenger"></param>
        public void Request(Passenger passenger)
        {
            requests.Enqueue(passenger);
        }

        /// <summary>
        /// Functions which ables the elevator to move based on the direction specified in the requests of the passengers 
        /// </summary>
        public void Move()
        {
            string direction = "";
            Passenger passenger;
            while (requests.Count != 0)    // if they are no more requests the elevator will stop moving 
            {
                direction = requests.Peek().direction;                                    // direction is defined by the most recent request
                while (requests.Count != 0 && direction == requests.Peek().direction)     // if they are requests remaining and the elevator is going in the same direction ( defined by the passengers in each request)
                {
                    passenger = requests.Dequeue();                                       // we take off the most recent request from the queue                          
                    Process(passenger);                                                   // We 
                }

                if (direction == "up")
                {
                    Up();                                                                // The elevator goes up
                }

                if (direction == "down")
                {
                    Down();                                                             // The elevator goes down       
                }
            }
        }

        /// <summary>
        /// Process ables to deal with the passenger's request
        /// </summary>
        /// <param name="passenger"></param>
        public void Process(Passenger passenger)
        {
            if (passenger.requestedfloor != passenger.initialposition)      // Takes into account the case in which  requestedfloor is wrongly specified by the passenger 
            {
                if (elevatorposition == passenger.initialposition)                  // Case in which the elevator is at the same position as the passenger waiting 
                {

                    if (nbpassengers <= 5)                                          // There can't be more than 6 passengers inside the elevator
                    {
                        nbpassengers++;                                             // As the passenger is at the same position as the elevator, there is one more passenger inside the elevator
                        floors[passenger.requestedfloor] = true;                    // We defined the requested floor as true in floors so that the elevator will stop there
                        passenger.insideelevator = true;                            // We also say that the passenger is inside the elevator since he's at the same position as the elevator
                    }
                    else                                                         // Case in which the passenger can't get inside the elevator because it's at full capacity and he
                    {
                        passenger.insideelevator = false;                        // The passenger is not inside the elevator
                        passenger.requestadded = false;                          // His request can't be fullfilled so requestadded is false used in UpdateNbPassengers(pos)
                    }
                }
                else                                                                // Case in which the passenger is not at the same position as the elevator 
                {
                    passenger.insideelevator = false;                                   // He is not inside the elevator
                    PassengerPosition.Add(passenger.initialposition);                  // We add the passenger's initial position to PassengerPostion
                    differentFloor.Add(passenger);                                     // We also add the passenger to the list differentFloor 
                }
                Passengerstatus.Add(passenger);
            }                                            // we add the passenger to the list passengerstatus 
        }


        /// <summary>
        /// Method to process what should be done with the passengers retreived and not retreived 
        /// </summary>
        /// <param name="initialpos"></param> the actual position of the elevator
        /// <param name="notretreived"></param>
        public void Search(int initialpos, int notretreived)
        {
            int count = 0;
            int index = 0;
            if (nbpassengersretrieved > 0)                                                               // If some passengers retreived we need to remove them from the list differentFloor and add them again to Passengerstatus as insideelevator changed 
            {
                while (differentFloor.Count != 0 && count != nbpassengersretrieved)
                {
                    if (differentFloor[index].initialposition == initialpos && count != nbpassengersretrieved)      
                    {
                        Passenger pass = differentFloor[index];
                        differentFloor.Remove(pass);                                                // As the passengers were retreived by the elevator we can take them off from the list differentFloor
                        Passengerstatus.Remove(pass);                                               // we remove as we need to change the boolean insideelevator and searchin for the passenger in the list is long
                        pass.insideelevator = true;                                                 // We indicates that the passenger is inside the elevator 
                        Passengerstatus.Add(pass);                                                  // We add him again to passenger status  
                        floors[pass.requestedfloor] = true;                                         // Permits the elevator to stop at the requestedfloor
                        count++;
                        if (index > 0) { index--; }
                    }                                                                               // The modification of index are made to avoid an ArgumentOutOfRangeException in case we remove a passenger from the list
                    else { index++; }
                }
            }
            count = 0;
            index = 0;
            if (notretreived > 0)                                                                      // If some passengers weren't retreived at initialpos we have to add there requests again (in case some of them were supposed to be retreived at this very moment)
            {
                while (differentFloor.Count != 0 && count != notretreived)
                {
                    if (differentFloor[index].initialposition == initialpos && count != notretreived)
                    {
                        Passenger pass = differentFloor[index];
                        differentFloor.Remove(pass);                                // We remove them from differentFloor as they might be added again in the Process method                          
                        Passengerstatus.Remove(pass);                               // We do the same for Passengerstatus for the same reason
                        requests.Enqueue(pass);                                     // We add the passenger again to the queue requests
                        count++;
                        if (index > 0) { index--; }
                    }                                                                                   // The modifications of the index are made to avoid an ArgumentOutOfRangeException in case we remove a passenger from the list
                    else { index++; }   
                }
            }
        }



        /// <summary>
        /// Method used to move the elevator upwards
        /// </summary>
        public void Up()
        {
            int NotRetreived = 0;
            bool display = true;
            for (int i = 0; i < floors.Length; i++)                                 // we read from 0 to 6th floor
            {
                nbpassengersretrieved = 0;
                if (floors[i] == true && PassengerPosition.Contains(i) == false)       // Case in which the elevator has to stop at the floor i and no passengers need to get inside the elevator
                {
                    UpdateNbPassengers(i);                                                                                                                                                                     // We update nbpassengers delivered                                     
                    Console.WriteLine($"\nElevator went up from floor {elevatorposition} to floor number {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers} passengers inside ");
                    nbpassengers -= nbpassengersdelivered;                                                                                                                                                     // We update nbpassengers                                                              
                    Console.WriteLine($"It now has {nbpassengers} passenger(s) inside ");
                    floors[i] = false;                                                                        // The elevator doesn't need to stop there again                           
                    elevatorposition = i;                                                                     // The elevator position is now i
                }
                if (PassengerPosition.Contains(i) == true)                                                       // Case in which the elevator reached the initial position of a passenger (we retrieve it by taking account of the capacity), passengers can also be delivered
                {
                    display = true;
                    nbpassengersretrieved = 0;
                    while (PassengerPosition.Count != 0 && PassengerPosition.Contains(i) == true)                           // while they are still people at this floor orr PassengerPosition is not empty we count the number of passengers retreived and not retreived. We also remove them from PassengerPosition
                    {
                        if (nbpassengers <= 5)
                        {
                            PassengerPosition.Remove(i);                            // We have to remove it from PassengerPosition so that it won't get taken into account again since the passenger has been retreived
                            nbpassengersretrieved++;
                            nbpassengers++;
                        }
                        else
                        {
                            PassengerPosition.Remove(i);                        // We have to remove it from PassengerPosition because any passenger related to this position will be removed as shown in the method Search
                            NotRetreived++;
                        }
                    }
                    Search(i, NotRetreived);                                  // We use Search to deal with passengers retreived and not retreived    
                    if (nbpassengersretrieved > 0)                                                                                                                // If some passengers were retrieved the elevator will move                           
                    {
                        Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} to get {nbpassengersretrieved} passenger(s) with {nbpassengers - nbpassengersretrieved} passengers inside ");
                        elevatorposition = i;
                        display = false;
                    }
                    UpdateNbPassengers(i);                                                      // We update nbpassengersdelivered
                    if (nbpassengersdelivered > 0 && floors[i] == true)                                                                                                                  // If some passengers have to be delivered at this floor 
                    {
                        nbpassengers -= nbpassengersdelivered;
                        if (display == false) { Console.WriteLine($"It also delivered {nbpassengersdelivered} passenger(s). It now has {nbpassengers} passenger(s)"); }
                        if (display == true)
                        {
                            Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers + nbpassengersdelivered} passengers inside. It now has {nbpassengers}");
                            elevatorposition = i;
                        }
                        floors[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Method used to move the elevator downwards. The principles are exactly the same as in the method up();
        /// </summary>
        public void Down()
        {
            int NotRetreived = 0;
            bool display = true;
            for (int i = floors.Length - 1; i >= 0; i--)                        // we read from the 6th floor to 0 
            {
                nbpassengersretrieved = 0;
                if (floors[i] == true && PassengerPosition.Contains(i) == false)             // Case in which the elevator has to stop at the floor i and no passengers get inside the elevator
                {
                    UpdateNbPassengers(i);                                                                                                                                                                   // We update nbpassengers delivered 
                    Console.WriteLine($"\nElevator went down from floor {elevatorposition} to floor number {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers} passengers inside");
                    nbpassengers -= nbpassengersdelivered;                                                                                                                                                    
                    Console.WriteLine($"It now has {nbpassengers} passenger(s) inside ");
                    elevatorposition = i;                                                                                                                                                                    // The elevator position is now i
                    floors[i] = false;                                                                                                                                                                      // The elevator doesn't need to stop there again
                }

                if (PassengerPosition.Contains(i) == true)                        // Case in which the elevator reached the initial position of a passenger (we retrieve it by taking account of the capacity), passengers can also be delivered                                                                                             
                {
                    display = true;
                    while (PassengerPosition.Count != 0 && PassengerPosition.Contains(i) == true)                           // while they are still people at this floor or PassengerPosition is not empty we count the number of passengers retreived and not retreived. We also remove them from PasssengerPosition
                    {
                        if (nbpassengers <= 5)                                                                      // We check if full capacity is reached 
                        {
                            PassengerPosition.Remove(i);                                                            // We have to remove it from PassengerPosition so that it won't get taken into account again since the passenger has been retreived
                            nbpassengersretrieved++;
                            nbpassengers++;
                        }
                        else
                        {
                            PassengerPosition.Remove(i);                                                            // We have to remove it from PassengerPosition because any passenger related to this position will be removed as shown in the method Search
                            NotRetreived++;
                        }
                    }
                    Search(i, NotRetreived);                                                            // We use Search to deal with passengers retreived and not retreived
                    if (nbpassengersretrieved > 0)
                    {
                        Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} to get {nbpassengersretrieved} passenger(s) with {nbpassengers - nbpassengersretrieved} passengers inside ");
                        elevatorposition = i;
                        display = false;
                    }
                    UpdateNbPassengers(i);                                                  // We update nbpassengers delivered 
                    if (nbpassengersdelivered > 0 && floors[i] == true)                                                                                                    // If some passengers have to be delivered at this floor 
                    {
                        nbpassengers -= nbpassengersdelivered;
                        if (display == false) { Console.WriteLine($"It also delivered {nbpassengersdelivered} passenger(s). It now has {nbpassengers} passenger(s)"); }
                        if (display == true)
                        {
                            Console.WriteLine($"\nElevator went from floor {elevatorposition} to floor {i} and delivered {nbpassengersdelivered} passenger(s) with {nbpassengers + nbpassengersdelivered} passengers inside. It now has {nbpassengers}");
                            elevatorposition = i;
                        }
                        floors[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Method to update the number of passengers delivered at a specific floor(position). It's also used to add a passenger who wasn't in requests because the elevator was at full capacity
        /// </summary>
        /// <param name="position"></param>
        public void UpdateNbPassengers(int position)
        {
            int delivered = 0;
            bool removed = false;
            for (int i = 0; i < Passengerstatus.Count && Passengerstatus.Count != 0; i++)                                       // condition about Passengerstatus.Count is added because we could remove all the passengers from the list 
            {
                if (Passengerstatus[i].requestedfloor == position && Passengerstatus[i].insideelevator == true)                 // Case in which the passenger is inside the elevator and the elevator has reached his requested floor. We don't need to check requestadded as it's used only to add a passenger to requests 
                {                                                                                                                   
                    delivered++;
                    Passengerstatus.RemoveAt(i);                                                                                // We remove him from Passengerstatus as his request is fullfilled
                    removed = true;
                }

                else
                {
                    if (Passengerstatus[i].requestadded == false && Passengerstatus[i].insideelevator == false && nbpassengers <= 5)                    // Case in which the elevator is not at full capacity, the passenger is not inside the elevator and his request couldn't be take into account because of full capacity  
                    {
                        Passengerstatus[i].requestadded = true;                                                                                         // We also set requestadded to true as it's the default value for the passenger and this value can be changed in Process
                        requests.Enqueue(Passengerstatus[i]);                                                                                           // We add the passenger to requests
                        Passengerstatus.RemoveAt(i);                                                                                                    // We remove him from Passengerstatus as he will be added again
                        removed = true;
                    }
                    else
                    {
                        removed = false;
                    }
                }

                if (removed == true) { i = i - 1; }                                                                                                     // The modifications of the index i are made to avoid an ArgumentOutOfRangeException in case we remove a passenger from the list
            }
            nbpassengersdelivered = delivered;
        }
    }
}