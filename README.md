# CarProject
English Version:

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

Applied Features:
  Car Features:
    -Interactive tyre smoke and print system: like real cars, in the case of:
      -wheel spins, for rear tyres(at low speeds).
      -wheel lock(when braking hard), for rear and front wheels when using foot brake, or rear wheels when using handbrake.
      -Drifting, if it passes a certain threshold
    -Interactive Light system:
      -After a certain speed(40), a trail light appears on the backlights, and fades away after going below that speed again.
      -Option for Low Beam and High Beam For Front Lights, and to switch to any of them no matter which one is enabled.


    
Version Francais:

Remarque :
  -En cours de développement, ce projet vise à créer une expérience interactive de conduite avec des simulations aussi réalistes que possible.
  -Actuellement, le projet est en cours d'exécution avec Unity version 2021.3.17f1.
  -Tous les éléments utilisés proviennent du unity asset store, et sont disponibles gratuitement.
  
Commandes :
  -Utilisez WASD (ou les flèches) pour vous déplacer.
  -Appuyez sur "S" (ou utilisez la flèche arrière) pour freiner.
  -Appuyez sur "Espace" pour activer le frein à main.
  -Appuyez sur "L" pour accéder aux options des feux avant (cliquez pour les feux de croisement, maintenez pour les feux de route).
  
Remarques sur les calculs :
  -Pour des valeurs réalistes dans ce projet, considérez que la vitesse de 66 correspond à environ 100 km/h.
  -La voiture, à propulsion arrière, atteint 0-100 km/h en environ 9-10 secondes.
  -Toutes les tractions et les calculs des roues dépendent de la puissance et de la masse du véhicule. Ainsi, toute modification nécessite des ajustements dans les paramètres du collidé de roue pour des résultats cohérents.
  
Fonctionnalités mises en œuvre :
  Caractéristiques de la voiture :
    -Système interactif de fumée et d'empreinte de pneu reproduisant les caractéristiques réelles comme:
      -le patinage des roues arrière(wheel spin) à basse vitesse
      -le blocage des roues(wheel lock) lors d'un freinage brusque (avec le frein à pied ou à main)
      -la dérive(drifting) si un certain seuil est dépassé.
    -Système interactif de feux :
      -À une vitesse dépassant 40, un lighttrail apparaît sur les feux arrière et disparaît en dessous de cette vitesse.
      -Options pour les feux de croisement et les feux de route à l'avant, avec la possibilité de basculer entre les deux, peu importe lequel est activé.

