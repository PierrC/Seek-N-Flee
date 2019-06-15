# Seek-N-Flee
This is a demo for seek and flee steering behavior. 

Works on Unity 2018

The demo is a playground where 3 types of agents, all using steering behavior to move, interect with each other. When the playground is instantiated, polygons obstacles and paths are created by placing invisble nodes in a grid like fashion, removing those that collide with the obstacles. If there no path to one of the endpoints, then the process starts over again. Once we get a valid playground, green and yellow agents are placed randomly.

![alt text](https://github.com/PierrC/Seek-N-Flee/blob/master/ReadMePics/SeekNFleepic1.png)

Red agents: they continuously spawn on the right side and try to reach either one of the goal post on the left. They will do so while trying to avoid the obstacles and the green agents trying to obstruct it.

Green agents: They move around the stage randomly until a red agent comes into range. Once they detect a red agent they will seek to obstruct said agent until it disappears (aka he reaches his endpoint).

Yellow agents: these agents are social agents, they also move randomly but when they detect another yellow agent the 2 agents form a "social circle" with him. After a short while, they will leave the circle and will temporary become a green agent.

![alt text](https://github.com/PierrC/Seek-N-Flee/blob/master/ReadMePics/SeekNFleepic2.png)

