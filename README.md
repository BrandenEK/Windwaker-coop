# Windwaker-coop
A coop program for The Legend of Zelda: The Wind Waker that syncs player progress throughout the game

Progress board:
https://trello.com/b/eBKLJlX5/windwaker-coop

## How to use:
- Each computer that you are playing on must be running one instance of Dolphin and the Windwaker-coop application.
- The server can be running on any computer on the local network, even if it is running a client application.

- Server side
  - Open the Windwaker-coop application and enter the local ip address of this machine.
  - For the player name, input "host".

- Client side
  - Open the Windwaker-coop application and enter the ip address of the server machine.
  - For the player name, enter anything.

## Things to note:
- You must be using Dolphin 5.0 (Other versions may possibly work, but they changed how memory is initialized after 5.0 and I haven't tested them).
- At this time, you can only play between computers on a local network.
- As of now, you can not see other players in your game, but their progress will still sync
- Most events in the game such as cutscenes being triggered/watched or chests being opened require a room transition to take effect.  For example, in Dragon Roost Cavern, once somebody unlocks the first locked door, other players must enter this room after the event has synced in order for the change to take place.
- You can change some of the settings by editing the "Windwaker-coop.dll.config" file

## Sync details:
### Things that do sync:
- Capacity upgrades (Magic meter, quiver, etc)
- All inventory items
- All equipment items (Power bracelets, hero's charm, etc)
- Sword & shield levels
- Songs
- Pearls & Triforce shards
- Treasure & Triforce charts
- Sea map
- Dungeon progress (Unlocked doors, events, chests, etc)
- Story progression

### Things that sync but are not perfect:
- Heart containers/pieces
- Small keys
- Bottles

### Things that don't sync
- Rupees
- Bait bag & spoils bag contents
- Pictographs

### Things that will sync soon
- Figurines
- Delivery bag letters
- Mail?
