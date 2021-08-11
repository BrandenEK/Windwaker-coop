# Windwaker-coop
A coop program for The Legend of Zelda: The Wind Waker that syncs player progress throughout the game

Progress board:
https://trello.com/b/eBKLJlX5/windwaker-coop


### Things that do sync:
- Player stats (Max health, magic meter)
- All inventory items
- All equipment items (Power bracelets, hero's charm, etc)
- Sword & shield levels
- Songs
- Pearls & Triforce shards
- Treasure & Triforce charts
- Sea map
- Dungeon progress (Unlocked doors, events, etc)
- Story progression

### Things that don't sync
- Rupees
- Bait bag & spoils bag contents
- Pictographs

### Things that will sync soon
- Figurines
- Small keys
- Delivery bag letters
- Mail?


## How to use:
- First, you must be using Dolphin 5.0 - other versions may work, but they changed how memory is initialized after 5.0 and I haven't tested them.
- Also, for now this must all be done locally on the same computer (This is going to change very soon).  Open up as many instances of Dolphin as you want and load a save file.
- Open up as many instances of the Windwaker-coop application as you have players, and enter the information it prompts for (Server name, player name, & player number).
- When the first player joins, it will create the server files in C:\Users\Username\Documents\WW-coop\servers.
- After that everything should be good to go and notifications will show up in the console whenever someone obtains an item or progresses in the game.
