# CarProject

Note: 
  -this is still a work under Progress, And is aimed to have an interactive car with as close to real simulations.
  -This project is running in Unity version: 2021.3.17f1
  -All Assets are free and ive taken them from the unity asset store

Controls:
  -WASD (or arrows) to move
  -S (or back arrow) for footbreak 
  -Space For handbrake
  -L for front light options(Click low beam, Hold High Beam)

Calculations to note:
  -In this project, to have realistic values, consider the velocity of 66 as equal to 100km/h
  -The car is a Rear Wheel Drive car, and goes from 0-100kmh in ~9-10 seconds.
  -All wheel Tractions and calculations are calculated based on the selected power and mass of the vehicule, so changing them will require you to change in the wheel collider settings for realistic results

Features:
  Car Features:
    -Interactive tyre smoke and print system: like real cars, in the case of:
      -wheel spins, for rear tyres(at low speeds).
      -wheel lock(when braking hard), for rear and front wheels when using foot brake, or front wheels when using handbrake.
      -Drifting, if it passes a certain threshold
    -Interactive Light system:
      -After a certain speed(40), a trail light appears on the backlights, and fades away after going below that speed again.
      -Option for Low Beam and High Beam For Front Lights, and to switch to any of them no matter which one is enabled.
    
