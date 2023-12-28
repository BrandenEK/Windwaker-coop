# Windwaker Co-op
![Release version](https://img.shields.io/github/v/release/BrandenEK/Windwaker-coop)
![Last commit](https://img.shields.io/github/last-commit/BrandenEK/Windwaker-coop?color=important)
![Downloads](https://img.shields.io/github/downloads/BrandenEK/Windwaker-coop/total?color=success)

A multiplayer program for The Legend of Zelda: The Wind Waker that syncs player progress throughout the game
<br>
Progress board:
https://trello.com/b/eBKLJlX5/windwaker-coop

<br>

# Project is under development again!
This time for real I hope, there should be a hugely improved version coming soon, along with a real UI
<br><br>

## How to use:
1. On any computer, open the Windwaker-coop application and run it as a server using the local ip address of the machine.
2. On each computer that will be playing, open dolphin and load a save file.  Then open the Windwaker-coop application and run it as a client using the local ip address of the server machine.

## Requirements:
- Dolphin 5.0 Stable Build (https://dolphin-emu.org/download)
- Microsoft .NET Core 3.1 (https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
- Each client application must be on the same local network as the server application or be using Hamachi

## Things to note:
- You must be using Dolphin 5.0 Legacy (Other versions may possibly work, but they changed how memory is initialized after 5.0 and I haven't tested them).
- ~~Each computer can only be running one instance of the Windwaker-coop application.~~ (This might not be true anymore)
- As of now, you can not see other players in your game; the program only syncs inventory and story/dungeon progress.
- Most events in the game such as cutscenes being triggered/watched or chests being opened require a room transition to take effect.  For example, in Dragon Roost Cavern, once somebody unlocks the first locked door, other players must enter this room after the event has synced in order for the change to take place.
- You can change some of the settings by editing the "config.json" file.
- Specifying a port is optional, simply add ":xxx" after the ip address.

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
- Tingle Statues

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
- Bag contents
- Mail?

## How to set up Hamachi (To simulate a local network):
If Hamachi doesn't work immediately, you may need to set up an inbound rule through your firewall
1. Download & install LogMeIn Hamachi (https://www.vpn.net)
2. Open "Windows Defender Firewall with Advanced Security" (Easiest way is by searching for it in the search bar)
![Windows Defender Firewall with Advanced Security](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-firewall/images/fw01-profiles.png)
4. In the top left, click on "Inbound Rules" and create a new rule
5. For "Rule Type", leave 'Program' selected
6. For "Program", select the program path of the Hamachi exe file (Most likely: C:\Program Files (x86)\LogMeIn Hamachi\x64\hamachi-2.exe)
7. For "Action" and "Profile", leave the default options selected
8. For "Name", give it a name such as 'Windwaker Co-op'
9. Now when Hamachi is powered on, you can create/join a network and play Windwaker Co-op without being on a local network
