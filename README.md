# ☣️SCP-069 ☣️


### ❓What does it do?❓ 
SCP-069 is a new playable SCP, which will play out the same as SCP-049 without the ability to respawn zombies, but when it kills its first victim, it will take it's body, size and inventory, thus almost perfectly replicating his victim. This ability can and will be used every time it kills a human, which opens up new interesting scenarios with this SCP, as well as a different way to play.



### 📜Commands📜
| Command | Description |
| ------------- | ------------------------------ |
| `069`   | Test SCP-069 on the person using the command. |


### ⚙️ Configuration ⚙️
| Config | Default |Description |
| ------------- |------------------------------| ------------------------------ |
| `IsEnabled` | **true** | Determine if the plugin is enabled or not. |
| `ClonerDamageEvery` | **10** | SCP-069 will take the amount of damage you put in this setting every second. |
| `ClonerIncreaseDamageBy` | **10** | For every second that passes, the damage increases by the amount you put here. |
| `GracePeriodStart`| **30** | After this time, SCP-069 will begin to take damage for every second.|
| `GracePeriodOnKill` | **15** | When SCP-069 kills someone, they will not take damage per second, for as long as you specify (In seconds obviously) |
| `ClonerkRatsNeeded` | **4**| The amount of Class-D required for SCP-069 to appear. |
| `ClonerChance` | **55** | The probability that SCP-069 will appear, if the above requirement is met. |
| `ClonerkHealth` | **1540** | The amount of HP SCP-069 has. |
| `ClonerLifesteal` | **150** | As it says, the amount of life that is healed by killing a human. |
| `BroadcastDuration` | **8** | If this setting is greater than 0, the number you set will be the duration of the broadcast you send to the victim of SCP-069. |
| `Killbroadcast` | **You were killed by SCP-069** | The message that will come out to the victim when he is killed by SCP-069. |
| `SpawnBroadcastDuration` | **8** | Spawn Broadcast Duration|
| `SpawnBroadcast` | **You're SCP-069. When killing a human, you will steal it's shape, inventory and size. You will also receive {dmg} damage every few seconds until you find a new victim, also healing for {heal}hp on every kill.** | {dmg} represents the number they put in **ClonerDamageEvery** and {heal} the number they put in **ClonerLifesteal** |




### Thanks for watching
- And thanks to Beryl for allowing me to use his README as a base.
