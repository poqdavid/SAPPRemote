# SAPPRemote
Remote control client for SAPP

----------
<p align="center">
<a href="https://i.imgur.com/6XyKRIR.png" target="_blank">
<img title="Click to enlarge" src="https://i.imgur.com/6XyKRIR.png">
</a>
<br>
<a href="http://imgur.com/a/Errsf" target="_blank">More screenshots</a>
</p>


**Features**
* Fully functional remote console for SAPP 9.4.1
* Players list with (player/team) color (players stat is shown once you hover the mouse on the player)
* Custom context menu for players list (%index, %name)

**Instructions**  
* If you want to run the client with different settings you can use (e.g., ``SAPPRemote.exe Setting.sapp``)
* Or if you used the setup you can simpply double click on the file (<a href="https://github.com/poqdavid/SAPPRemote/raw/master/Default.sapp">Default.sapp</a>)

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
