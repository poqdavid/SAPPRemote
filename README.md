# SAPPRemote
Remote control client for SAPP

----------

**Instructions**

You can simply run SAPPRemote like ``SAPPRemote.exe path <full path to setting file>`` so you can use same exe with different settings  

Here is some sample data for adding menus to players list (note that this will be created by default when you run the program for the first time)  
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