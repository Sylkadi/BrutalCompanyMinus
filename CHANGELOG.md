# ChangeLog
<details>
  <summary>0.12.0</summary>
  
  - Added 3 new vanilla events
  - (Good) MoreExits: Spawns more entrances and exits into the facility
  - (Good) EarlyShip: Ship arrives earlier than usual
  - (Bad) LateShip: Ship arrives later than usual

  - Added 4 new mooded events (ToilHeads, EmergencyDice and LCGoldScrapMod)
  - (VeryGood) CityOfGold: Uses only LCGoldScrapMod scrap table
  - (Good) Dice: Spawns only dice
  - (Very Bad) BadDice: Only spawns bad dice
  - (Very Bad) Toilhead: Spawns toilheads

  - Alot of numbers ahead
  - Added arrows to the UI so you can more easily scroll.
  - Added 3 new scaling options, scaleByDaysPassed, scaleByScrapInShip and scaleByMoonGrade.
  - Mod will now use a difficulty value to scale things, this go from 0 => 100, old scaling was 0 => 60 from days passed.
  
  - scaleByDaysPassed will add 1 difficulty per day
  - scaleByScrapInShip will add 1 difficulty per 400 scrap value in ship
  - scaleByMoonGrade will add (D => -8), (C => -8), (B => -4), (A => +5), (S => +10), (S+ => +15), (S++ => +20), (S+++ => +30), (Other => +10) difficulty

  - Updated eventType rarity scales
  - VeryBad (Difficulty:0 => 0) to (Difficulty:100 => 40)
  - Bad (Difficulty:0 => 50) to (Difficulty:100 => 10)
  - Neutral (Difficulty:0 => 15) to (Difficulty:30 => 10)
  - Good (Difficulty:0 => 23) to (Difficulty:30 => 15)
  - VeryGood (Difficulty:0 => 2) to (Difficulty:100 => 12)
  - Remove (Difficulty:0 => 15) to (Difficulty:30 => 10)

  - Updated weatherMultipliers
  - Removed factorySizeMultiplier
  - Rainy (x1.05, x1.00)
  - Foggy (x1.15, x1.10)
  - Flooded (x1.25, x1.15)
  - Stormy (x1.35, x1.20)
  - Eclipsed (x1.35, x1.20)

  - Some other stuff i forgot about
</details>
<details>
  <summary>0.11.3</summary>
  
  - UI event descriptions and chat event descriptions should now match.
  - Arachnophobia event should no longer spawn anti coilheads
  - Bees event should now appear

  - Added terminal commands which only host can use, to display all commands type 'mhelp' into the terminal.
  - MHELP => Provides help information for commands.
  - MEVENT => Forces a mEvent to occur for next day.
  - MCLEAR => Clears the forced event list.
  - MEVENTS => Displays all events.
  - MPAY => Adds or subtracts credits.
  - MENEMIES => Displays all enemies.
  - MITEMS => Displays all items.

</details>
<details>
  <summary>0.11.1</summary>
  
  - Landmine event will now properly spawn landmines instead of turrets...

</details>
<details>
  <summary>0.11.0</summary>

  - v50 update
  - Added 9 new vanilla events

  - (VeryGood) SafeOutside: Prevents spawning outside and related events
  - (Bad) Butlers: Spawns butlers
  - (Bad) SpikeTraps: Spawns spike traps
  - (Bad) FlowerSnake: Spawns flower snakes
  - (VeryBad) Worms: Spawns worms outside and inside and comes with alot of snare fleas
  - (VeryBad) OldBirds: Spawns Oldbirds inside and outside and comes with landmines and outside landmines
  - (RemoveEnemy) NoOldbirds: Prevents oldbirds from spawning and related events
  - (RemoveEnemy) NoButlers: Prevents butlers from spawning and related events
  - (RemoveEnemy) NoSpikeTraps: Prevents spike traps from spawning and related events

  - Added 21 modded events (Will only appear if related mods are downloaded) ()
  - Current supported mods are Lethalthings, Diversity, Scopophobia, HerobrineMod, SirenHead, RollingGiant, TheFiend, Lockers, TheGiantSpecimens, Football, Mimics and Peepers
  - Dm me on the lethal modding discord in the brutal company minus thread if there is any other you want to add

  - (Bad) Roomba: Spawns boombas inside and sometimes outside
  - (Bad) TeleporterTraps: Spawns teleporter traps inside
  - (Bad) Mimics: Increased spawn rates of mimics
  - (Bad) Peepers: Spawns peepers outside and inside
  - (Bad) Shrimp: Spawns shrimps inside
  - (Bad) Rollinggiants: Spawns rolling giants
  - (Bad) ImmortalSnail: Spawns the immortal snail
  - (Bad) Lockers: Spawns lockers inside
  - (Bad) Football: Spawns football
  - (VeryBad) TheFiend: Spawns the fiend
  - (Verybad) Herobrine: Spawns herobrine
  - (Verybad) Sirenhead: Spawns sirenheads
  - (VeryBad) Walkers: Spawns walkers
  - (VeryBad) ShyGuy: Spawns shyguys
  - (VeryBad) GiantShowdown: Spawns redwoodgiants and giants outside
  - (RemoveEnemy) NoLockers: Prevents the lockers from spawning 
  - (RemoveEnemy) NoImmortalSnail: Prevents the immmortal snail from spawning
  - (RemoveEnemy) NoFiend: Prevents the fiend from spawning
  - (RemoveEnemy) NoShyGuy: Prevents Shyguy from spawning
  - (RemoveEnemy) NoPeepers: Prevents peepers from spawning
  - (RemoveEnemy) NoMimics: Prevents mimics from appearing

  - Events descriptions will vary, nearly every event will have atleast 3 descriptions or more, some 2
  - Event count will now scale (2 => day:0, 3 => day:25, 4 => day:50)
  - There is now a no scale option in the config
  - You can now change which enemies spawns for events(And all inside and outside spawns)
  - You can now modify scrap transmuation events
  - Yippeee is now compatible with kamikazie bugs, kamikazie bugs will now say yippeeeee
  - Updated some events
  - Removed scrap and enemies weights config
  - Added 'all' and 'allall' config options which will allow spawning of all enemies on all moons, these will make the game harder
  - Made some slight event changes
  - Reduced default density values on tree related events
  - And other stuff i forgor about

</details>
<details>
  <summary>0.10.12</summary>
  
  - AllWeather event will no longer spawn with 20 instances of stormy(Oops)
  - AllWeather will come with x1.6 scrapValue and x1.3 scrapAmount
  - Bombardment from warzone will no longer hit the ship
  - Buffed BigBonus default values
  - Updated rng for object spawning
  
</details>
<details>
  <summary>0.10.11</summary>
  
  - Added 1 new event:
  - (VeryBad) AllWeather: Will spawn Eclipsed, Stormy, Flooded and Raining weather.
  - Added config options for 'eventsToSpawnWith' and 'eventsToRemove' for every event.
  - Added globalScrapValueMultiplier and globalScrapAmountMultipliers scales, both set to (1, 0, 1, 1) by default
  - Added additiveInsideSpawnChance and additiveOutsidespawnChance scales, both set to (0, 0, 0, 0) by default
  - Added some outsideSpawnChance to certain events.
  
</details>
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
