# ☣️SCP-069 ☣️


### ❓What does it do?❓ 
The SCP-069 is a new and playable SCP, which adds the feeling that gave the SPY in TF1 in the first place and that is the fear of your own team, the SCP-069 can steal the Name and form of its victim, adding interesting scenarios in SCPSL.Victims can see the real name and a tag "(SCP-069)" when they are spectators.



### 📜Commands📜
| Command | Description |
|:------------ |:------------------------------:|
| `069 help`   | Returns a list of available 069 commands. |
| `069 list`   | It gives you a list of players who are SCP-069 if any. |
| `069 give`   | Give SCP-069 to whoever is using the command. |
| `069 give [PlayerName/PlayerID]`| It gives the specified player the SCP-069, there can be more than 1 SCP-069 the plugin is prepared for that situation, but remember that it can affect a little bit the performance and experience of the other players. **PlayerID refers to the temporary ID given to the player per game, not the SteamID or DiscordID.** |
| `069 remove` | Remove SCP-069 from the player who is using the command, try not to use it if you don't have 069 in the first place. |
| `069 remove [PlayerName/PlayerID]`| Remove SCP-069 from the specified player, if it is 069 in the first place. |


### ⚙️ Configuration ⚙️
| Config | Default |Description |
|:------------- |:------------------------------:| ------------------------------:|
| `is_enabled` | **true** | Determine if the plugin is enabled or not. |
| `not_logo` | **false** | True to disable the ASCII logo on the console, in case one should bother you |
| `debug` | **false** | Activate some debug logs in the plugin, if you have any error activate this and send a screenshot of what the plugin says. |
| `spawn_victims_ragdolls` | **false** | Determines whether SCP-069 should leave the bodies of its victims. |
| `movement_speed_intesify` | **1** | The intensity of SCP-207 that will be given to SCP-069 when killing, if the amount is 0 no movement speed will be given. |
| `movement_speed_duration` | **15** | The duration of the movement speed to be given to SCP-069. This will be ignored if `movement_speed_intesify` = 0. |
| `movement_speed_shoulbe_accumulated` | **true** | The duration of the movement speed should be accumulated ? This means that if when you kill someone, you have 3 seconds of movement speed left, it will be accumulated with the new one for killing. |
| `cloner_damage_every` | **10** | SCP-069 will take the amount of damage you put in this setting every second. |
| `cloner_increase_damage_by` | **10** | For every second that passes, the damage increases by the amount you put here. |
| `grace_period_start`| **30** | After this time, SCP-069 will begin to take damage for every second.|
| `grace_period_on_kill` | **15** | When SCP-069 kills someone, they will not take damage per second, for as long as you specify (In seconds obviously) |
| `cloner_rats_needed` | **4**| The amount of Class-D required for SCP-069 to appear. |
| `cloner_chance` | **55** | The probability that SCP-069 will appear, if the above requirement is met. |
| `cloner_health` | **1540** | The amount of HP SCP-069 has. |
| `cloner_max_health` | **2000** | The maximum HP that the SCP-069 can obtain when killing |
| `cloner_max_health` | **150** | As it says, the amount of life that is healed by killing a human. |
| `broadcast_duration` | **8** | If this setting is greater than 0, the number you set will be the duration of the broadcast you send to the victim of SCP-069. |
| `killbroadcast` | **You were killed by SCP-069** | The message that will come out to the victim when he is killed by SCP-069. |
| `spawn_broadcast_duration` | **8** | Spawn Broadcast Duration|
| `spawn_broadcast` | **You're SCP-069. When killing a human, you will steal it's shape, inventory and size. You will also receive {dmg} damage every few seconds until you find a new victim, also healing for {heal}hp on every kill.** | {dmg} represents the number they put in **ClonerDamageEvery** and {heal} the number they put in **ClonerLifesteal** |




### Thanks for watching
- And thanks to Beryl for allowing me to use his README as a base.
