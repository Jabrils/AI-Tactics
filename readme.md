## v0.0.33

## Notes
- There is ABSOLUTLEY NO randomness in this. This is 100% determanistic. EXCEPT for the level generation (not toggled) & candy Spawning.
- make sure you vs opp turn it is makes it into the state
- If your str is at least 3, & > opp str & hp > opp.hp, beserk
- if your str is higher, beserk

### To-Do
|Version|Date|Thing|Note
|-|-|-|-
|||Made a Main Menu UI|create a Haxbot, battle, train. Drag in the haxbot creator made for patreon
|||Added a ML AI

### Feature Creep
|||Added a dummy AI|can simply just create a state heiarchy based on hp in 1/4 incriments
|||Added last 10 battle actions to state data | going to have to build a memory NN to be able to properly feed this as input
|||Added audience to stans
|||optimize
|||model a few skins
|||fixed level loader to read it properly
|||model new coleseum
|||units no longer get stuck in corners
|||Maybe add like a special attack

### Changelog
|Version|Date|Thing|Note
|-|-|-|-
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