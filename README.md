# Windwaker Co-op
A multiplayer program for The Legend of Zelda: The Wind Waker that syncs player progress throughout the game

Progress board:
https://trello.com/b/eBKLJlX5/windwaker-coop

## How to use:
- Each computer that you are playing on must be running one instance of Dolphin and the Windwaker-coop application.
- The server can be running on any computer on the local network, even if it is already running a client application.
- Specifying a port is optional, simply add ":xxx" after the ip address.

- Server side
  - Open the Windwaker-coop application and type 's'.
  - Enter the local ip address of this machine.

- Client side
  - Open the Windwaker-coop application and type 'c'.
  - Enter the ip address of the server machine.
  - Enter this player's name.

## Requirements:
- Dolphin 5.0 Stable Build (https://dolphin-emu.org/download)
- Microsoft .NET Core 3.1 (https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
- Each computer running the game and the application must be on the same local network (For now at least)

## Things to note:
- You must be using Dolphin 5.0 (Other versions may possibly work, but they changed how memory is initialized after 5.0 and I haven't tested them).
- As of now, you can not see other players in your game; the program only syncs inventory and story/dungeon progress
- Most events in the game such as cutscenes being triggered/watched or chests being opened require a room transition to take effect.  For example, in Dragon Roost Cavern, once somebody unlocks the first locked door, other players must enter this room after the event has synced in order for the change to take place.
- You can change some of the settings by editing the "config.json" file

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
