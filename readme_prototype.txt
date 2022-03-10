readme_prototype

This Project prototype is to test game creation and player creation using MonoGame Framework
Authors: Team 4 - Jason Cheung, Gabriel Baluyut, Justin Payne
Date: Mar 3, 2022
Source: https://docs.monogame.net/articles/getting_started/0_getting_started.html

In the prototype is a test Game Class which contains the main code for the program to run as a game.
Game Class definition: main class program of the MonoGame project.
This class needs to include these methods necessary for the game to run:

- Game Constructor: includes bits to tell the project how to start.

- Initialize method: Initialize the game upon its startup.

- LoadContent Method: Add assets from the running game from the Content project.
	note: there is also an UnloadContent() method which is currently unused.

- Update Method: Called on a regular interval to update the game state.
        Updating game state includes taking player input, 
	checking for collision, playing audio, animating entities, etc...
        This method is called multiple times per second.

- Draw Method - Called on a regular interval to take the current 
        game state and draw the game entities to the screen.
        This method is called multiple times per second.