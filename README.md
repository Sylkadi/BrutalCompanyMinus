# Brutal Company Minus
![Screenshot](https://i.imgur.com/jS0vFm2.jpg)
#### This mod is required on all clients
#### Makes the game harder, download this mod if you want to suffer occasionally
#### Config is fully generated when you load on a moon.
#### Mod is highly configurable
#### Comes with modded events that only appear with said mods installed

# Features
<details>
  <summary><b>Event Mechanics</b></summary>
    
  - Whenever you land on a moon, a couple of events will be chosen (Between 2 to 5 by default), these events will appear in the UI in the top right hand corner of your screen, this can be open and closed by pressing 'k' or a custom set value in the config.
![Screenshot](https://i.imgur.com/wjsVK7b.jpg)

  - Events will come in 6 main types and are categorized by color
  - These types all have there own weights to get that type of event, this can be changed in the config to use custom set weights instead of type weights.

| Event Type | Base Weight | Increment | MinCap | MaxCap |
|-|-|-|-|-|
| Very Good | 3 | 0.14 | 3 | 17 |
| Good | 23 | -0.1 | 13 | 23 |
| Neutral | 10 | -0.05 | 5 | 10 |
| Bad | 40 | -0.15 | 25 | 40 |
| Very Bad | 5 | 0.25 | 5 | 30 |
| Remove Enemy | 15 | -0.05 | 10 | 15 |

</details>
<details>
  <summary><b>Weather Multipliers</b></summary>

  - Weathers will now come with scrapValue and scrapAmount multipliers.
  - These will also be displayed ingame on the terminal in this format <mark>(xScrapValue, xScrapAmount)</mark> as such
![Screenshot](https://i.imgur.com/biZv8ck.jpg)

  - You can also enable randomize weather multipliers in the config, which will give abit of rng to the weathers after every day.
  - Default multiplier values goes as follows, these can be changed in the config.

**WARNING: Setting a factory size value below 1.00 may crash your game**

| Weather    | Scrap value | Scrap amount | 
|------------|-------------|--------------|
| None       | 1.00        | 1.00         |
| DustClouds | 1.05        | 1.00         |
| Rainy      | 1.05        | 1.00         |
| Stormy     | 1.35        | 1.25         |
| Foggy      | 1.15        | 1.10         |
| Flooded    | 1.25        | 1.15         |
| Eclipsed   | 1.35        | 1.25         |
</details>

<details>
  <summary><b>Terminal Commands</b></summary>
    
  - The mod has commands that only host can use, to display all commands type 'mhelp' into the terminal.

![Screenshot](https://i.imgur.com/Xhk9RnL.png)

  - Example of 'mevents' command.

![Screenshot](https://i.imgur.com/eFrKnge.png)

  - Example of 'mevents nutcracker' command.

![Screenshot](https://i.imgur.com/RuXFo9A.png)

  - Example of 'mevent hell hell nutcrack' command.

![Screenshot](https://i.imgur.com/Wr9TDlK.png)

  - Example of 'menemies' command.

![Screenshot](https://i.imgur.com/GVSGqo0.png)

</details>

<details>
  <summary><b>Event List</b></summary>
  
  - List of all events in corresponding types
  
  <details>
    <summary><b>Very Good</b></summary>
    
| Name | Description |
|-|-|
| Big Bonus | Large sum of credis |
| Scrap Galore | Increased scrap value and amount |
| Golden Bars | Only golden bars will spawn on the map |
| Big Delivery | Spawns a shipment with a bunch of items |
| Plenty Outside Scrap | Spawns scrap outside |
| Black Friday | Everything will go on sale |
| Safe Outside | Will prevents enemies from spawning outside and certain events |
  </details>

  <details>
    <summary><b>Good</b></summary>

| Name | Description |
|-|-|
| Bounty | Killing enemies will now reward credits |
| Bonus | Sum of credits |
| Smaller Map | Reduces factory size |
| More Scrap | Increased scrap amount |
| Higher Scrap Value | Increased scrap value |
| Golden Facility | Only spawns Goldencup, Ring, Goldbar, Fancylamp, Perfumebottle, Painting and Cashregister on the map |
| Dentures | Only spawns teeth on the map |
| Pickles | Only spawns pickles on the map |
| Honk | Only spawns horns on the map |
| Transmute Scrap Small | Takes any one-handed scrap in map scrap pool and only spawns that |
| Small Delivery | Spawns a shipment with some items |
| Scarce Outside Scrap | Spawns scrap outside |
| Fragile Enemies | Decreases enemy hp |
| Full Access | All Doors are unlocked and open and big doors are all unlocked, prevents facility ghost. |
| Early Ship | Time will start earlier |
| More exits | Spawns entrances and exits |
  </details>

  <details>
    <summary><b>Remove Enemy</b></summary>
    - These will also prevent any event with that 'enemy' to be picked.
    

| Name | Description |
|-|-|
| No Baboons | Removes Baboon Hawk |
| No Bracken | Removes Bracken |
| No Coilhead | Removes Coil Head |
| No Dogs | Removes Eyeless Dog |
| No Giants | Removes Forest Keeper |
| No Hoarding Bugs | Removes Hoarding Bug |
| No Jester | Removes Jester |
| No Ghosts | Removes Ghost Girl and Facility Ghost |
| No Lizards | Removes Spore Lizard |
| No Nutcrackers | Removes Nutcracker |
| No Spiders | Removes Bunker Spider |
| No Thumpers | Removes Thumper |
| No Snare Fleas | Removes Snare Flea |
| No Worm | Removes Earth Leviathan |
| No Slimes | Removes Hygrodere |
| No Maksks | Removes Masked and removes related scraps |
| No Turrets | Removes Turret |
| No Landmines | Removes Landmine |
| No OldBirds | Removes Oldbird |
| No Butlers | Removes butlers |
| No Spiketraps | Removes spiketraps |
  </details>

  <details>
    <summary><b>Neutral</b></summary>

| Name | Description |
|-|-|
| Nothing | Nothing |
| Locusts | Spawns Roaming Locusts |
| Birds | Spawns Manticoils |
| Trees | Spawns Trees |
| Leafless Brown Trees | Spawns trees without leaves |
| Leafless Trees | Spawns spooky trees |
| Gloomy | Makes the atmosphere foggy |
| Rainy | Makes it rain (No mud) | 
| Heavy Rain | Makes the atmosphere rainy, flooded and stormy. Triple rain. |
  </details>

  <details>
    <summary><b>Bad</b></summary>

| Name | Description |
|-|-|
| Hoarding Bugs | Spawns Hoarding bugs outside and inside, comes with Scarce Outside Scrap |
| Bees | Spawns Bees outside |
| Landmines | Increased rates of landmines inside |
| Lizard | Spawns Spore Lizard inside |
| Slimes | Spawns Hygrodere outside and inside |
| Thumpers | Spawns Thumpers inside |
| Turrets | Increased rates of turrets inside |
| Spiders | Spawns Bunker Spiders outside and inside, comes with Leafless Brown Trees |
| Snare Fleas | Spawns Snare Fleas inside |
| Facility Ghost | The ghost can open/close bigdoors and doors, mess with lights, mess with the breaker and can lock/unlock doors(Rare) |
| Outside Turrets | Spawns turrets outside, comes with Trees |
| Outside Landmines | Spawns Landmines outside |
| Grabbable Turrets | Turns some of the turrets on the map into scrap |
| Grabbable Mines | Turns some of the mines on the map into scrap |
| Shipment Fees | Any shipment's on given moon will deduct credits as a fee |
| Strong Enemies | Increases enemy hp |
| Reality Warp | Attempting to grab scrap will make it transform into something else, sometimes a landmine or turret |
| Kamikazie Bugs | Hoarding bugs will now blow up when angered | 
| Masked | Spawned masked enemies |
| Butlers | Will Spawn butlers |
| Spike Traps | Will spawn spike traps inside |
| Flower Snake | Will spawn flower snakes inside and outside |
| Late Ship | Time will start a little later |
| Holiday Season | Turns scrap into mystery boxes and eggs and spawns nutcrackers and hoarding bugs inside |
  </details>

  <details>
    <summary><b>Very Bad</b></summary>
    
| Name | Description |
|-|-|
| Nutcracker | Spawns Nutcrackers outside and inside, comes with Turrets |
| Arachnophobia | Spawns alot of Bunker Spiders outside and inside, comes with Leafless Trees |
| Bracken | Spawns Brackens Inside |
| Coilhead | Spawns Coilheads Inside |
| BaboonHorde | Spawns alot of Baboon Hawks outside |
| Dogs | Spawns Eyeless Dogs outside and inside |
| Jester | Spawns Jesters inside |
| Little Girl | Spawns Ghost Girls inside |
| Anti Coilhead | Changes Coilhead AI and spawns them inside, comes with Leafless Trees and Gloomy |
| Chinese Produce | Decreased Scrap Value but Increased Scrap Amount |
| Transmute Scrap Big | Takes any two-handed scrap in map scrap pool and only spawns that |
| War Zone | Acts as Quad event including Turrets, Landmines, Outside Turrets and Outside Landmines and will also come with artillery fire!! |
| Bug Horde | Spawns a load of Hoarding Bugs outside and inside, comes with Scarce Outside Scrap |
| Forest Giant | Spawns a Forest Keeper inside |
| Inside Bees | Spawns Bees outside and inside |
| Nutslayer | Spawns the Nutslayer inside the facility, kills everything... comes with gloomy, thumpers, spiders and masked. |
| Hell | Great reward, but at what cost... |
| AllWeather | Acts as Eclipsed, Stormy, Flooded and Raining all in one day |
| Worms | Will spawn worms inside and outside and snare fleas inside and outside |
| OldBirds | Will spawn old birds and comes with Landmines and Outside Landmines |
    
  </details>
  
</details>

<details>
  <summary><b>Modded Event List</b></summary>
  
  - These events will only appear with said mods installed
  - Currently supported mods are Lethalthings, Diversity, Scopophobia, HerobrineMod, SirenHead, RollingGiant, TheFiend, Lockers, TheGiantSpecimens, Football, Mimics, LCGoldScrapMod, Toilhead, Moonswept, ShockwaveDrones, FacelessStalker, EmergencyDice and Peepers
  
  <details>
    <summary><b>Very Good</b></summary>
    
| Name | Description |
|-|-|
| CityOfGold | Will only spawn golden scrap |
  </details>

  <details>
    <summary><b>Good</b></summary>

| Name | Description |
|-|-|
| Dice | Only spawns various dice |
  </details>

  <details>
    <summary><b>Remove Enemy</b></summary>
    - These will also prevent any event with that 'enemy' to be picked.
    

| Name | Description |
|-|-|
| No Lockers | Prevents lockers from spawning |
| No ImmortalSnail | Prevents the immortal snail from spawning |
| No Fiend | Prevents the fiend from appearing |
| No ShyGuy | Prevents the shy guy from appearing |
| No Peepers | Prevents the peepers from appearing | 
| No Mimics | Prevents mimics from spawning |
  </details>

  <details>
    <summary><b>Neutral</b></summary>

| Name | Description |
|-|-|
| None | None |
  </details>

  <details>
    <summary><b>Bad</b></summary>

| Name | Description |
|-|-|
| Roomba | Spawns boombas inside and sometimes outside |
| TeleporterTraps | Spawns teleporters traps inside |
| Mimics | Increased mimics amount |
| Peepers | Spawns peepers inside and outside |
| Shrimp | Spawns the shrimp inside |
| RollingGiants | Spawns the rolling giants inside |
| ImmortalSnail | Spawns the immortal snail inside |
| Lockers | Spawns the lockers inside |
| Football | Spawns football inside |
| Cleaners | Spawns cleaners inside |
| Mobile Turrets | Spawns Walking turrets Inside |
| Shockwave Drones | Spawns ShockwaveDrones inside |
  </details>

  <details>
    <summary><b>Very Bad</b></summary>
    
| Name | Description |
|-|-|
| The Fiend | Will spawn the fiend |
| Herobrine | Will spawn herobrine |
| Sirenhead | Will spawn the sirenhead |
| Walkers | Will spawn the walker |
| ShyGuy | Will spawn the shyguy | 
| GiantShowdown | Will spawn the redwood giant and giants outside |
| Bad Dice | Only spawns bad dice |
| SlenderMan | Spawns slender man |
    
  </details>
  
</details>

# Configuration
Alot of options in the config will contain a **scale**, a scale will contain a **Base**, **Increment**, **MinCap** and **MaxCap**.

This is used to scale the game harder the more days pass.

The formula used to compute the **scale** is `Base + (DaysPassed * Increment)`.

**MinCap** is the value that the computed value wont go below.

**MaxCap** is the value that the computed value wont go above.

# Config overview
## Difficulty Config 
Location: `BrutalCompanyMinus\Difficulty_Settings.cfg`

`Ingore scaling`: Setting this to true will stop the mod from sacling as the days pass.

`Spawn Chance Multiplier`: A **scale** that multiplys the spawn rate.

`Inside Spawn Chance Additive`: This will add to all keyframes for the insideSpawn animation curve.

`Outside Spawn Chance Additive`: This will add to all keyframes for the outsideSpawn animation curve.

`Spawn Cap Multiplier`: A **scale** that multiplys the spawn caps, this allows for a higher maximum amount of enemies to spawn outside and inside.

`Additional Inside Max Enemy Power`: A **scale** that adds to the inside max enemy power count.

`Additional Outside Max Enemy Power`: A **scale** that adds to the outside max enemy power count.

`Addional hp`: A **scale** that adds bonus hp to enemies.

`Global scrap value multiplier scale`: A **scale** that multiplies all scrap value.

`Global scrap amount multiplier scale`: A **scale** that multiplies all scrap amount.

`Good event increment multiplier`: A global multiplier that will multiply all **Good** and **Very Good** increments.

`Bad event increment multiplier`: A global multiplier that will multiply all **Bad** and **Very Bad** increments.

`Events to spawn`: The amount of events to spawn.

`Weights for extra events`: Weights for additionaly events, "40, 40, 15, 5" is equivalent to (+0, +1, +2, +3) events, this can be expanded.

`Show events in chat`: Show events in chat?

`Use custom weights`: If enabled, Custom weights will be used otherwise EventType weights will be used.

#### Event Type Weights

`Very Good Weight scale`: The weight **scale** for **VeryGood** event to be chosen

**...**

`Very Bad Weight scale`: The weight **scale** for **VeryBad** event to be chosen

#### Quota Settings

`Enable Quota Changes`: Enable quota changes?

`Deadline Days Amount`: Deadline

`Starting credits`: Default is 60

`Starting quota`: It's in the name

`Base Increase`: Quota scaling

`Increase Steepness`: Quota scaling

## Weather config
Location: `BrutalCompanyMinus\Weather_Settings.cfg`

`Enable Weather Multipliers`: Enable weather multipliers?

`Enable Terminal Text`: Enable terminal text?

`Randomize Weather Multipliers`: This will randomize **ScrapValue**, **ScrapAmount** and **FactorySize** multipliers for every weather after every day.

`Random Weather Multiplier Min Inclusive`: Lower bound of random value for **ScrapValue**, **ScrapAmount** and **FactorySize**.

`Random Weather Multiplier Max Inclusive`: Upper bound of random value for **ScrapValue**, **ScrapAmount** and **FactorySize**.

#### None

`Value Multiplier`:  Multiply scrap value by.

`Amount Multiplier`: Multiply scrap amount by.

`Factory Size Multiplier`: Multiply factory size by.

**...**

#### Eclipsed

`Value Multiplier`:  Multiply scrap value by.

`Amount Multiplier`: Multiply scrap amount by.

`Factory Size Multiplier`: Multiply factory size by.

## UI config
Location: `BrutalCompanyMinus\UI_Settings.cfg`

`UI Key`: They key used to toggle the UI.

`Normalize Scrap Value Display`: In game default scrap value multiplier is 0.4, having this enabled will multiply the display value by 2.5 to make it look normal.

`Enable UI`: Enable UI?

`Show UI Letter Box`: Show UI Letter box that contains the key?

`Show Extra Properties`: Show additional info in the UI that is **DaysPassed**, **ScrapValue**, **ScrapAmount**, **FactorySize**, **SpawnChance**, **SpawnCap** and **BonusEnemyHp**

`Pop Up UI`: Will the UI popup when landing on a new moon?

`UI Time`: The time the UI will appear for when popped up.

## Events
Location: `BrutalCompanyMinus\CustomEvents.cfg`

This will contain conifgurable options for every single event.

`_Temp Custom Monster Event Count`: How many **Temp custom monster event** to generate in the config, this is temporary.

#### AntiCoilHead

`Custom Weight`: Requires **Use Custom Weights** enabled to be used.

`Description`: What will be outputted into the UI.

`Color Hex`: The color that the description will be displayed in.

`Event Type`: This can be eiter **VeryBad**, **Bad**, **Neutral**, **Good**, **Very Good**, **Remove**

`Event Enabled`: Will the event be enabled?

`Inside Enemy Rarity`: A **scale** that describes the chance for the enemy to spawn inside

`Outside Enemy Rarity`: A **scale** that describes the chance for the enemy to spawn outside

`Min Inside Enemy`: A **scale** that describes the minimum amount of enemies garunteed to spawn inside.

`Max Inside Enemy`: A **scale** that describes the maximum amount of enemies garunteed to spawn inside.

`Min Outside Enemy`: A **scale** that describes the minimum amount of enemies garunteed to spawn outside.

`Max Outside Enemy`: A **scale** that describes the maximum amount of enemies garunteed to spawn outside.

`Events To Remove`: Will prevent said event(s) from occuring.

`Events To Spawn With`: Will spawn with said event(s).

**...**

#### Higher Scrap Value

`Custom Weight`: Requires **Use Custom Weights** enabled to be used.

`Description`: What will be outputted into the UI.

`Color Hex`: The color that the description will be displayed in.

`Event Type`: This can be eiter **VeryBad**, **Bad**, **Neutral**, **Good**, **Very Good**, **Remove**

`Event Enabled`: Will the event be enabled?

`Scrap Value`: A **scale** that multiplies the scrap value.

`Events To Remove`: Will prevent said event(s) from occuring.

`Events To Spawn With`: Will spawn with said event(s).

**...**

#### WarZone

`Custom Weight`: Requires **Use Custom Weights** enabled to be used.

`Description`: What will be outputted into the UI.

`Color Hex`: The color that the description will be displayed in.

`Event Type`: This can be eiter **VeryBad**, **Bad**, **Neutral**, **Good**, **Very Good**, **Remove**

`Event Enabled`: Will the event be enabled?

`Events To Remove`: Will prevent said event(s) from occuring.

`Events To Spawn With`: Will spawn with said event(s).
