# AI_System_Implementation
Link to video showing gameplay <br>
https://youtu.be/v2gsZVqb1MA

**States Chart**
| State | Behaviour | Transitions |
| ------|-----------|------------ |
| Idle | Wait for a set amount of time before coming out of the "Home" area. | **Wander**: At the end of the timer, transition to the Wander state<br> **Alert**: When the player is in the vision cone moving above a certain velocity, or a loud sound happens (player or NPC) |
| Wander | Walk to a random spot from an array of determined spots. Upon reaching a spot, choose whether to Graze or Idle based on a chance. Will return home after a certain amount of time. Can be alerted by the player through Sound or Vision. Can also be alerted by itself if it steps on something loud/surprising. | **Idle/Graze/Return Home**: Determined randomly (still deterministic for the transitions) when arriving at a Wander spot<br> **Alert**: When the player is in the vision cone moving above a certain velocity, or a loud sound happens (player or NPC) |
| Graze | Stops and eats for a moment. After the time it takes to eat, wander to another spot. Can be alerted by the player or itself. Has reduced vision while in this state. | **Wander**: After eating timer runs out, return to wandering<br> **Alert**: When the player is in the vision cone moving above a certain velocity, or a loud sound happens (player or NPC) |
| Alert | Timer starts to check when to go back to wandering. Timer is interrupted and the NPC flees if there is another noise triggered while in the alert state, or if the player moves too far from the spot when the NPC was initially alerted. | **Wander**: After the alert timer runs out, return to wandering<br> **Flee**: Run away if, while the alert timer is still active, there is a sound triggered or player has moved too far from the spot they were in when the Alert state was entered. |
| Flee | Increase NPC speed and return to the "Home" area. | **Return Home**: Transition to this state when the NPC is in the "Home" area |
| Return Home | Arrive at the "Home" area and then idle until the timer runs out. The timer can be reset if the NPC is alerted. Speed is reset when arriving home. | **Idle**: Entered when the NPC reaches the home area<br> **Flee**: Enters flee if player is in the view area while trying to return home|

**Difficulties**<br>
Horses are almost impossible to be snuck up on, so this is merely a state machine demonstration project, and it uses some game logic over real world logic
Getting the player camera to work properly. It mostly works as a 3rd person camera, but moving backwards freaks out the player
Adding animations, so just the idles are present. Graze would have its own animation, I just couldn't figure out how to add it and I know it's not as important as the FSM for this assignment

**References**
- [Animal prefabs package](https://assetstore.unity.com/packages/3d/characters/animals/animals-free-animated-low-poly-3d-models-260727)
- [Sticks prefabs](https://assetstore.unity.com/packages/3d/vegetation/trees/low-poly-wood-pack-stylized-wooden-models-321987)
- [Horse Vision Cone](https://www.extension.iastate.edu/equine/vision-equine#:~:text=Each%20eye%20sees%20across%20an%20arc%20of%20approximately%20200%E2%80%93210%20degrees%20around%20the%20body%20at%20one%20time.%20The%20monocular%20fields%20straight%20in%20front%20of%20the%20horse%E2%80%99s%20face%20overlap%20slightly%20resulting%20in%20a%20%C2%A0binocular%20field%20between%2065%20and%2080%20degrees.)
- PlayerController code from the module 3 demo on Github (has some changes)
- IsInView method code from the module 3 demo on Github (has some changes)
