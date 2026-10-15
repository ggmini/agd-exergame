# GREAT COOKING

## Project Layout

### Great Cooking 

The Unity Project for Great Cooking is located in [agd-exergame](agd-exergame).

#### Scripts

[SandwichAssembly](agd-exergame/Assets/Scripts/SandwichAssembly/) contains all scripts specifically for the sandwich assembly minigame.

[Wraps Assembly](agd-exergame/Assets/Scripts/Wraps%20Assembly/) contains scripts for a scrapped second minigame.

[Wiimote Input](agd-exergame/Assets/Scripts/Wiimote%20Input/) contains Scripts for the scrapped Input method using Wiimotes. The library used for this is found in the [plugins folder](agd-exergame/Assets/Plugins/Wiimote/). 

[Websocket Server](agd-exergame/Assets/Scripts/Websocket%20Server/) contains Scripts for the Websocket Connection. The library used for this is found in the [plugins folder](agd-exergame/Assets/Plugins).

Any other scripts located in the root of the [scripts](agd-exergame/Assets/Scripts/) folder are general scripts used in various areas.

Scripts for various menus are located in the [Menus folder](/agd-exergame/Assets/Scripts/Menus/).

Scripts used for initial testing are located in the [testing folder](agd-exergame/Assets/Scripts/Testing/).

#### Scenes

[MainMenu](agd-exergame/Assets/Scenes/Main%20Menu/) contains the Scene for the main menu.

[Testing](agd-exergame/Assets/Scenes/Testing/Wiimote%20Test/) contains a testing scene for the scrapped Wiimote input method.

[Wraps Assembly](agd-exergame/Assets/Scenes/Game%20Scenes/Wraps%20Assembly/) contains the scene for a scrapped second minigame.

[Sandwich Assembly](agd-exergame/Assets/Scenes/Game%20Scenes/Sandwich%20Assembly/) contains the scene and all its data for the main minigame.

#### Plugins

WebsocketSharp:
A websocket library for C#. Required for the connection between the game and the arduino controller/mobile controller. Installed in the [plugins folder](agd-exergame/Assets/Plugins/websocket-sharp.dll). [Original Repository](https://github.com/sta/websocket-sharp) by sta.

Unity-Wiimote:
A library to read inputs from Wiimotes. Scrapped feature. Installed in [plugins folder](agd-exergame/Assets/Plugins/Wiimote/). [Original Repository](https://github.com/Flafla2/Unity-Wiimote) by Flafla2, [updated repository](https://github.com/danger9819/Unity-Wiimote) by danger9819. Some slight changes/fixes were made in our project. 

#### Known Bugs/Issues

- Sometimes the game locks up on the final minigame (final cutting station).
- The tracking/sensitivity for the minigames has not been fine-tuned yet. Especially station 4 (assambly station) is not great.
- When starting the game it sometimes fails to recognize the 5s start button holding. Either let go of the start button and try holding it for 5 seconds again or restart the game.
- The camera fov is incorrect for 2 players.
- The black screen fade to next station doesn't work for 2 players.

### Mobile Controller

The mobile controller app built in Godot is located in [websocket-controller](websocket-controller).

#### Known Bugs

- A "grab" button is visible on the connection screen. It currently serves no function.
- The app will not recognize if the websocket connection has been closed (i.e. because Great Cooking was closed). A manual disconnect is required to reconnect.
- The app does not check if it was able to connect to the entered adress and will always proceed to the connected screen.