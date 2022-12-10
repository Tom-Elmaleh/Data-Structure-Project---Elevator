ESILV - Data Structure - Project 2022 <br>

<br>
<h1 align="center"> The Elevator Project </h1>


### Introduction
This repository contains all files related to the final project of the course Data Structures and Algorithms. This project aims to uses some data structures 
in order to build something that would be innovative. 


### Purpose
I chose to build a "smart" Elevator using object-oriented programming.

My elevator considers the number of passengers inside. 
For example, it won’t retrieve a passenger if it has reached its full capacity (6 passengers). 
I also tried to optimise its movement as much as I can. Every algorithm for an elevator has at least one flaw, I used two of them. 
I added to both algorithms some code to make it more optimised (capacity and motion).
At first, I started with a First Come First Serve algorithm combined with a process to serve multiple requests in parallel. 
This algorithm optimises the elevator motions the least, especially for a sequence of up and down (main flaw). 
Therefore, I decided to use a second algorithm called the LOOK algorithm (.
It’s the most optimised algorithm and it’s used for real elevator system. 
In both processes, I have also added an innovation. 
My elevator remembers the request of a passenger if he can’t get inside because of a full capacity (in terms of passenger).
The elevator will take care of his request once possible given the capacity and the passenger concerned won’t have to call the elevator again in the meantime.
