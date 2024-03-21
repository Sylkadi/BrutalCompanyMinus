# ChangeLog
<details>
  <summary>0.10.10</summary>
  
  - Spawn chance will no longer multiply negative values in key frames, Tldr; Higher spawn chance will now mean earlier spawns.
  - Other stuff
  
</details>
<details>
  <summary>0.10.9</summary>
  
  - Nut slayer is now immortal by default and slightly more dangerous.
  - Nut slayer event will now come with thumpers, spiders and masked events. And will also increase spawn rates and spawn cap.
  - Black friday values should now be synced to all clients(Small oversight)
  - Moved DDay event to warzone(Removed DDay event but now it will only appear in warzone and hell).
  - Added 2 new events(Nothing interesting):
  - (Bad) Masked: Spawns Masked enemies
  - (Good) FullAccess: Every door will be unlocked and opened, prevents facility ghost.
  
</details>
<details>
  <summary>0.10.7</summary>
  
  - Added config option to show event's after ship leaves.
  - Updated how UI key works
  - Added back DDay, if it dosen't work i will cut my own dick off.
  
</details>
<details>
  <summary>0.10.4</summary>

  - DDay persisting should be fixed
  - Lowered default Siren volume

</details>
<details>
  <summary>0.10.3</summary>

  - Added 2 new events
  - (Bad) DDay: I like this one
  - (VeryGood) BlackFriday: Every item in the shop will go on sale

  - Reality shift will now grab the shifted object forcibly.
  - WarZone will now come with DDay
  - Hell will now come with WarZone
  - Turned eventType Weights to scales
  - Very bad will now scale (day:0 => 10 to day:60 => 30)
  - Bad will now scale (day:0 => 40 to day:60 => 20)
  - Other eventTypes wont scale by default

  - Minus will now spawn outside Objects 8 per frame instead of being all in one frame.
  - Fixed terminal codes for objects spawned by Minus.

</details>
<details>
  <summary>0.10.2</summary>

  - Forgor to multiply transmuted scrap values.

</details>
<details>
  <summary>0.10.1</summary>

  - Changed how the mod handles scrapAmount and scrapValue multipliers for comptability reasons.
  - Reality Shift will now work on outside spawned scrap.
  - Bounty should no longer be claimed multiplie times on a single enemy.
  - Fixed some default scales for events.
  - Shipment fee's should now be working

</details>
<details>
  <summary>0.10.0</summary>

  - Changed spawn rate multiplier default:(day:0 => x1.0 to day:60 => x2.0)
  - Added spawn cap multiplier default:(day0 => x1.0, day:60 => x2.0)
  - Set insideEnemyPowerCountScaling and outsideEnemyPowerCountScaling to 0, 0, 0, 0 by default
    
  - Added 4 new events(1 is reworked but it's pratically new):
    
  - (Bad)Kamikazie Bugs: Spawns kamikazie bugs inside
  - (Bad)Reality Warp: Attempting to pick up any spawned scrap will transform it into something else, something a turret or landmine.
  - (Bad)FacilityGhost(Used to be DoorGlitch): Will now open/close big doors and normal doors, will flicker lights,
     will mess with breaker and can rarely lock/unlock doors. The ghost will sometimes go crazy and cause alot of things to happen.
  - (VeryBad)Hell: Great reward, but at what cost.
    
  - Update default values for events
  - Updated some eventNames and descriptions
  - Fixed some things
  - Changed assetBundle names to be less generic so it wont cause an IO_Exception with certain mods.

</details>
<details>
  <summary>0.9.0</summary>

  - Redid all the scaling for all events (Mostly nerfed sizably) (Sizable nerf to insideTurrets and insideLandmines also)
  - Added minCap and maxCap to scales.
  - Added new difficulty scalings.
  - Added spawn rate multiplier scale default:(Scales from day:0 => x0.8 to day:60 => x2.5)
  - Added hp bonus scale default:(Scales from day:0 => +0hp to day:60 => +5hp)
  - Added maxInsideEnemyPower scale default:(Scales from day:0 => +0 to day:60 => +60)
  - Added maxOutsideEnemyPower scale default:(Scales from day:0 => +0 to day:60 => +30)
  - Changed extra event chance to weights => (40, 40, 15, 5) is equivalent to (+0, +1, +2, +3) events...
  - TLDR: should be much easier at start but will now scale harder than it used to (In theory).
  - Upated Icon
  - Added 2 new events(Not interesting):
  - (Good)Fragile Enemies: Decreased enemy hp.
  - (Bad)Strong Enemies: Increased enemy hp.
  - Fixed weather effects disapearring when entering/exiting facility.
  - GrabbableLandmines will now longer be steppable on after ship leave(Stepping is enabled again on LoadNewLevel)
  - Added a 250 spawn cap to outsideLandmines and outsideTurrets so it should no longer lag the game when spawning on a big map, and added a 1000 cap to other object.
  - Generating and binding enemy + scrap configs should now be thread-safe (I hope).
  - Next patches will be new unique events.
</details>
<details>
  <summary>0.8.4</summary>
  
  - Changed how scrap is handled in this mod, should no longer cause compatibility issues.
  - Event default amount is now set 2 to but.
  - There is now a default chance of 50% to add another event up to 4. (Can be configured)
  - There is now 10 temp custom monster events at the bottom of the events config. (Haven't tested these too much)
  - Made nutslayer much more harsh...
  - Added config options for nutslayer, slayer shotgun, grabbable landmines and grabbable turrets.
</details>
<details>
  <summary>0.8.3</summary>
  
  - Added Quota config options
  - Added customizable config weights for all enemies and all scrap for all moons (Including modded ones)
  - Fixed items from this mod not saving
  - Shipment fee's should no longer happen on the company moon. Literally 
</details>
<details>
  <summary>0.8.2</summary>
  
  - Grabbable Landmines and Grabbable Turrets should no longer disapear.
  - Shipment fee's should no longer happen on the company moon.
</details>
<details>
  <summary>0.8.1</summary>
  
  - Game should no longer break when disabling all events of one type.
  - Fixed some things with grabbableLandmines(Still cant figure out why scan node is not working)
</details>
<details>
  <summary>0.8.0</summary>
  
  - Added 7 new events.
    
  - Raining: Turns the atmosphere to raining (withouth the puddles)
  - Gloomy: Turns atmopshere gloomy(foggy withouth the fog).
  - Heavy Rain: Triple rain
  - Shipment Fees: Any shipment on given planet will incur fees.
  - Grabbable Landmines: Some landmines on the map will turn into scrap. (Wont blow you up straight away)
  - Grabbable Turrets: Some turrets on the map will turn into scrap.
  - Nut slayer: Spawns the nutslayer inside the facility, kills everything... If killed drops shotgun with infinite ammo.

  - Updated antiCoilHead, no longer double audio.
  - Updated how transmuteScrapSmall and transmuteScrapBig scale scrapAmount.
  - Added Increment Global Multipliers config settings.

</details>
<details>
  <summary>0.7.6</summary>
  
  - Game wont outright break when loading any of the zingar moon mod's
  - AntiCoilHead is now a seperate enemy(audio is doubling yet to fix)
  - Reverted rm.mapSizeMultiplier to rm.currentLevel.factorySizeMultiplier
</details>
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
