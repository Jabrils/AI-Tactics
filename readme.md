## v0.0.51

## Notes
- There is ABSOLUTLEY NO randomness in this. This is 100% determanistic. EXCEPT for the level generation (not toggled) & candy Spawning.
- make sure you vs opp turn it is makes it into the state
- I can either go off the assumption that for every 1 node taking 20 inputs, it need 5 nodes in the middle, OR can try some NEAT inspired implimentation.
- dont forget if all else fails I can force 1 hot encoding for any network
- deriv of sigmoid is fucking bonk, it will not work for me, but I think it is close enough to use, so just add the summation of all output nodes & it should be alright.
- THE SMALLER EPSILON IS (change in input) THE MORE ACCURATE DERV OF SIGMOID IS, THATS JUST CALCULUS BABY
- if training doesn't work out, I can try beefing up the NN architecture, or adding memory to the reward (use the 3 outputs to try & predict the future & obtain a totalReward[i] vector for a few timesteps ahead.)
- For graphing, graph the acculative reward over time ( r=0; r+=2; r+= -3; r+=2 ...)
- try an AI that cycles, att, def, taunt
- Training strats:
- - Berserk Slayer -> Berserk (Locked); BS_Pupil -> Berserk_Slayer(Locked); Pupil -> BS_Pupil(Locked)
- - Some new NN vs itself
- - Thing 1 vs Thing 2
- - Ground a neural network by training it vs random output, then train it against something with more strategy
- - Beserk_Slayer vs New NN, both learn to get better
- NN Problem solutions
- - Investigate if NN is wired right
- - Try a smaller learning rate
- - Train for long
- If letting the AI train all night vs Beserk Slayer doesn't work, look into [ rewards / adjustable LR / NN connections / making NN bigger ]
- Att -> Def: Currently 

## Credits
- freepik - Running Icon
- freepik - ???
- becris - ???
- umeicon - AI Brain Icon
- NAN - Stadium
- NAN - brick texure
- NAN - dirt texure
- NAN - pillar model
- Grant Sanderson - Calculus Help
- Dan Walsh - Calculus Help

### To-Do
|Version|Date|Thing|Note
|-|-|-|-
|||Gives in-game message if there is not bots folder
|||Added a author's note button & made it functional
|||Added a credits button & made it functional
|||Made Support Developer button functional
|||You can now press esc in battle, menu pops up & you can select restart or main menu
|||Added the ability to adjust the learning rate in the battle menu
|||Now saves stats about the AI to the data file, like how many battles it's been in
|||Add splash & splash notes for "Not tested on device"

## Polish
|Version|Date|Thing|Note
|-|-|-|-
|||Automatically generates Bots list if it doesnt exist
|||Added 2 random Haxbots from the Bots directory to the main menu
|||Added menu music
|||Added a NN input visualizers | steal the visualizer used in ML Hoops 21
|||Added a little circle that shows up when youre selecting where to move
|||Now saving AI choice in menu select that persists even after battles
|||Added hp & decisions to taskbar
|||Added a rounds dispay in battle, as well at the rounds graphs
|||Added a reset & exit button to battle
|||Added helmets
|||Removed the weird thing on the Haxbots

### Perfect World
|Version|Date|Thing|Note
|-|-|-|-
|||Learning is split into 2|Learn: Changes weights in real time, & saves everything the network learns, Adapt: Also changes weights in real time, but does not save everything the network learns. Acts as like a baseline knowledge, then fit it to the current opponent.
|||Now added a complex local minimum algorithm that attempts to prevent AIs from getting stuck in a local minima
|||AI now learns their own reward outcome instead of being set by developer
|||You can now train a NN via a human player
|||Added a ML AI for Movement
|||Added a battle replay feature
|||The crowd now reacts to the battle with cheers
|||Added crowd SFX
|||Added last 10 battle actions to state data | going to have to build a memory NN to be able to properly feed this as input
|||optimize
|||model a few skins
|||Added a dummy AI|can simply just create a state heiarchy based on hp in 1/4 incriments
|||model new coleseum
|||units no longer get stuck in corners
|||Maybe add like a special attack
|||fixed level loader to read it properly
|||Added a level builder

### Changelog
|Version|Date|Thing|Note
|-|-|-|-
|v0.0.51|04.08.020|Added menu button click sfx
|v0.0.48|04.03.020|Added a speedup time feature
|v0.0.47|03.31.020|Added 6 more inputs to state data | sum of last 10 choices / 10, myAtt, myDef, myTaunt, oppAtt, oppDef, oppTaunt. Tech: this is a list of Actions, that sums all of the 3 actions & / list count, & if list count > 10 then remove the first element. This will essentially ask: based on the past 10 or less battle actions, what is the distribution?
|v0.0.46|03.31.020|Map.OutputLocation is no longer called IF nothing has changed with the path / end selected tile|i think i can check it upon where ever its getting rounded
|v0.0.45|03.30.020|Made the human control be fully controllable with a keyboard
|v0.0.45|03.30.020|Added Human control to the Menu UI
|v0.0.44|03.21.020|Fixed NN code to be able to work for both player 1 & 2
|v0.0.44|03.21.020|Fixed waffle collision bug|Im sure i just need to move the waffle checker to be at the end of the turn, & not after moving
|v0.0.44|03.21.020|Removed the pointless level.map file loadin
|v0.0.44|03.21.020|Finished the Total Max Graph Mini feature
|v0.0.44|03.21.020|Fixed the uninitilize deal with the battle profile images | copy settings for menu
|v0.0.44|03.21.020|Confirmed all aspect ratios
|v0.0.44|03.21.020|fix standalone crashing
|v0.0.43|03.20.020|Added fire to the background of battle menu
|v0.0.43|03.20.020|Added exploitation sliders
|v0.0.43|03.20.020|Added a "Do you want NN to learn?" Scrollbar
|v0.0.42|03.19.020|Added random vs nn icons to battle decisions
|v0.0.42|03.19.020|Added a number on the graphs to display highest anything / max
|v0.0.42|03.19.020|Added the AI Name under the fighter's graph
|v0.0.41|03.18.020|Added graphs
|v0.0.40|03.15.020|Added Saving & Loading of AI Brains
|v0.0.40|03.15.020|Added a ML AI for Combat
|v0.0.37|03.09.020|Added exporation vs exploitation|for combat, the rest doesn't really make sense
|v0.0.36|03.08.020|Added audience to stands
|v0.0.35|03.08.020|A menu popup now displays to confirm
|v0.0.35|03.08.020|You can now click "start battle" or can still press space
|v0.0.34|03.08.020|Made a Main Menu UI|create a Haxbot, battle, train. Drag in the haxbot creator made for patreon
|v0.0.31|03.04.020|Added closest candy location to state data
|v0.0.31|03.04.020|Added Ran counts to state data
|v0.0.30|03.03.020|Made a Battle UI
|v0.0.29|03.03.020|Food Health now spawns on the map|now important that the AI learns this, but expert AI if so
|v0.0.27|03.02.020|added action camera mode
|v0.0.27|03.02.020|Added Human Input
|v0.0.21|02.24.020|Made nearby tiles see-through / invisible feature | maybe the simple solution is just a sphere that blocks rendering from some objects
|v0.0.20|02.24.020|units can now choose the same spot they were already on.|(maybe, think about this, can enable AI to never move if smart enough)
|v0.0.19|02.23.020|Added Camera Shake
|v0.0.19|02.23.020|Added UI
|v0.0.19|02.23.020|Added icons to make decisions clear
|v0.0.19|02.23.020|Added Sword
|v0.0.19|02.23.020|Added Shield
|v0.0.19|02.23.020|Added walking sfx
|v0.0.19|02.23.020|Added hit sfx
|v0.0.19|02.23.020|Added crit sfx
|v0.0.19|02.23.020|Added def sfx
|v0.0.19|02.23.020|Added step back sfx
|v0.0.19|02.23.020|Added power up sfx
|v0.0.19|02.23.020|Added power down sfx
|v0.0.19|02.23.020|Added hit sfx
|v0.0.19|02.23.020|Added poweup animation
|v0.0.18|02.22.020|added topdown camera mode
|v0.0.18|02.22.020|visualized stunned
|v0.0.18|02.22.020|visualized strength
|v0.0.18|02.22.020|Added nametags
|v0.0.17|02.22.020|Added dmg
|v0.0.16|02.22.020|Added health points
|v0.0.15|02.21.020|Replaced the path gizmo with something actually rendered
|v0.0.15|02.21.020|Added pose animations
|v0.0.15|02.21.020|added top down camera mode
|v0.0.15|02.21.020|added isometric camera mode
|v0.0.14|02.20.020|Added an algorithm that will ensure there is always a path from fighter to fighter
|v0.0.14|02.19.020|model character
|v0.0.14|02.19.020|State data is now organized
|v0.0.13|02.18.020|Added attacking phase
|v0.0.11|02.15.020|model wall|Just went with an asset store texture on a stretched cube
|v0.0.11|02.15.020|model pillar|Quite sure I am going to go with this asset store model, might re-model later
|?|02.014.020|add random noise blocks
|?|02.013.020|figure out adding block tiles to map
|?|02.012.020|figure out limited moves workflow