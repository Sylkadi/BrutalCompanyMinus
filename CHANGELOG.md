# ChangeLog
<details>
  <summary>0.7.5</summary>
  
  - Changed shitty code so game wont break
</details>
<details>
  <summary>0.7.4</summary>
  
  - Added config option to disabled terminal text
  - Fixed eventType weights not updating properly with disabled events.
  - Moved enemy spawn restoring to 'ShipLeave' instead of on scrapSpawn(I am monkey).
</details>
<details>
  <summary>0.7.2</summary>
  
  - Made horrible transpiler run last
  - Updated how paying works
  - Bounty pay amount is now configurable
</details>
<details>
  <summary>0.7.1</summary>
  
  - Horrible fix for scrap not being restored (Can't figure out which mod causes this)
</details>
<details>
  <summary>0.7.0</summary>
  
  - Restructered Code
  - Slighly lowered density values and made it more conisitent throughout maps
  - Added config option to change event amount
  - Added config option to show events in chat
  - Fixed some stuff
  - Buffed weather multipliers
  - Mod should now be much more stable with certain moon mods
</details>
<details>
  <summary>0.6.5</summary>

  - Made weight setting more clear.
  - Added option to disable an event in the config.
</details>
<details>
  <summary>0.6.4</summary>

  - Added extra config options for UI.
  - Fixed some things to deal with certain moon mods. Yet to implement a proper fix.
</details>
<details>
<summary>0.6.3</summary>

  - Atlas_Abyss v1.1.3 by Zingar fucks with this mod for whatever reason by removing a bunch of prefabs? i have yet to get to the bottom of this
  - Added some null checks
</details>
<details>
<summary>0.6.2</summary>
  
  - Fixed mod breaking with certain moon mods.
</details>
<details>
  <summary>0.6.0</summary>
  
  - Made a proper README.md and a seperate CHANGELOG.md file
  - Added a UI in the top right corner of the screen that will display event information
  - UI can be open and closed with 'k' or a set value in the config
  - Changed up the code a little
  - Buffed Scrap value and amount events
  - Tweaked some other values
  - Added some events
  - Fixed some stuff
</details>
<details>
  <summary>0.5.2</summary>
  
  - Fixed Anticoilhead event not working with late game upgrades
  - Added noLandmine and noTurret events
</details>
<details>
<summary>0.5.1</summary>
  
  - Reduced Density values overall
  - Made outside and inside enemy spawning more proper
  - ^^ Because of this hoarding bugs should no longer be squashed when outside
</details>
<details>
<summary>0.5.0</summary>
  
  - Changed scaling for all events
  - Scaling can now be changed in config
  - Outside turret, landmine and tree density should be roughly conistent throught the maps
  - Fixed some things
</details>
<details>
<summary>0.4.0</summary>
  
  - Added Outside turret and Outside landmine events
</details>
<details>
<summary>0.3.2</summary>
  
  - Terminal text should now be working properly with lethal expansion
</details>
<details>
<summary>0.3.1</summary>
  - Changed MinInclusive from 0.6 to 0.9
  - Randomize multipliers is now set to false by default
  - Terminal text dosent work well with 'lethal expansion'...
</details>
<details>
<summary>0.3.0</summary>
  
  - Implemented proper netcode for syncing
  - Implemented weather multipliers
  - Weather multipliers come with a bit of rng, can be disabled in config
  - Made code abit better and cleaner
  - More config options for events
  - Config settings for weather multipliers
  - AntiCoilHead event should now be properly fixed on multiplayer
</details>
<details>
<summary>0.2.4</summary>
  
  - Implemented a basic config to set weights
</details>
<details>
<summary>0.2.3</summary>
  
  - Added 6 new scrap related events
</details>
<details>
<summary>0.2.2</summary>
  
  - You wont be stuck on waiting when trying to go to company building
</details>
<details>
<summary>0.2.1</summary>
  
  - Hoarding bugs should no longer be depressed inside the facility
</details>
<details>
<summary>0.2.0</summary>
  
  - Restructered Code
  - factorySizeMultiplier, scrapAmountMultiplier and scrapValueMultiplier should now be synced on all clients.
  - AntiCoilhead event should now work properly on all clients
</details>
<details>
<summary>0.1.6</summary>
  
  - Multiplayer maps should be synced now(not on first round for whatever reason??)
</details>
<details>
<summary>0.1.4</summary>
  
  - Scrap multipliers now reset properly
</details>
<details>
<summary>0.1.2</summary>
  
  - Added AntiCoilHead Event
</details>
<details>
<summary>0.1.1</summary>
  
  - Tweaked some things
  - Added 8 new events
</details>
<details>
<summary>0.1.0</summary>
  
  - Initial Release
</details>
