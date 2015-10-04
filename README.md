# SAPPRemote
Remote control client for SAPP

----------

**Features**
* Fully functional remote console for SAPP 9.4.1
* Players list with (player/team) color (players stat is shown once you hover the mouse on the player)
* Custom context menu for players list

**Instructions**  
* If you want to run the client with different settings (e.g., ``SAPPRemote.exe path Setting2.json``)  

* If you want to add you menus to players list (this is how client generates menus)
Note: this is the default set of menus and once you run the client you can also see them in the settings file
```
"MenuItems": [
  {
    "text": "Kick",
    "command": "k %index"
  },
  {
    "text": "Ban",
    "command": "b %index"
  },
  {
    "text": "IP-Ban",
    "command": "ipban %index"
  }
]
```
