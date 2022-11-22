# celestial
Automatically change your desktop background based on the time of day.

## Features
- Supports Windows and Linux (GNOME and Cinnamon desktop environments)
- Constant time or celestial-based triggers
- Simple background service/app

## Installation
### Linux
The easiest way to run celestial on Linux is with a systemd user service. To set it up perform the following steps:

1. Copy the `celestial` binary to a location in your home directory, I would recommend `~/.local/bin`.
1. Modify the `ExecStart` value of `celestial.service` to point to the location of `celestial`. Note that this must be an absolute path, it cannot be relative or use variables such as `$HOME`.
1. Copy `celestial.service` to `~/.local/share/systemd/user/`.
1. Run `systemctl --user enable celestial` and `systemctl --user start celestial` to enable and start celestial. Once enabled it will automatically start upon login.

### Windows
Unfortunately user services aren't a thing on Windows, so the easiest way to run celestial on Windows is to set up a scheduled task to start `celestial.exe` on login.

## Configuration

celestial will create a config file on first start at `~/.config/celestial/config.json` or `%APPDATA%\Roaming\celestial\config.json`. All configuration is performed through this file. celestial will automatically reload the settings if a change is made to the file.

```
{
  "triggers": [],     // array of wallpaper triggers
  "latitude": null,   // location latitude for celestial-based triggers
  "longitude": null   // location longitude for celestial-based triggers
}
```

### Triggers
celestial has two types of trigger: time and celestial. To add a trigger, simply add it to the array in the config file. The order of triggers does not matter. You can make any combination of time or celestial triggers.

All trigger types have two properties in common:
- `type` - this defines the type of trigger, `Time` or `Celestial`
- `path` - this is the path of the wallpaper to display for a trigger. This should be an absolute path.

#### Time Triggers
Time triggers are the simplest trigger. They fire at a specific local time each day.

They are defined with a `type` value of `Time`, and have just one property: `time`. This is a time string of the format `HH:mm:ss` (24 hour format). This is the time that the trigger will fire at each day, so a trigger with a `time` value of `11:00:00` will fire at 11am local time each day.

##### Example
```
{
    "type": "Time",
    "time": "11:00:00",
    "path": "/path/to/some/image.png"
}
```

#### Celestial Triggers
Celestial triggers are triggers that fire depending on the position of celestial objects, e.g. the sun.

They are defined with a `type` value of `Celestial`, and have two properties:
- `time` - a celestial time (see below)
- `offset` - an optional constant offset from the celestial time

The possible values for `Time` are:
- `"Dawn"` - the start of dawn/civil twilight
- `"Sunrise"` - the start of sunrise (i.e. when the sun is first visible on the horizon)
- `"SunUp"` - when the sun is fully up (i.e. the bottom of the sun is on the horizon)
- `"Noon"` - when the sun is at it's highest point in the sky
- `"SunSetting"` - when the sun is about to start setting (i.e. when the bottom of the sun touches the horizon)
- `"Sunset"` - when the sun has set (i.e. disappeared below the horizon)
- `"Dusk"` - the end of dusk/civil twilight

Offset can be used to offset a constant amount from these celestial times. It has the same format as the `time` property of a time trigger, except it can also be negative.

##### Examples
Triggers at dawn:
```
{
    "type": "Celestial",
    "time": "Dawn",
    "path": "/path/to/some/image.png"
}
```

Triggers 1 hour before noon
```
{
    "type": "Celestial",
    "time": "Noon",
    "offset": "-01:00:00",
    "path": "/path/to/some/image.png"
}
```

Triggers 30 minutes after dusk
```
{
    "type": "Celestial",
    "time": "Dusk",
    "offset": "00:30:00",
    "path": "/path/to/some/image.png"
}
```

### Example Configuration
```
{
  "triggers": [
    {
        "type": "Celestial",
        "time": "Dawn",
        "path": "/home/james/Pictures/Wallpapers/1.png"
    },
    {
        "type": "Celestial",
        "time": "SunUp",
        "path": "/home/james/Pictures/Wallpapers/2.png"
    },
    {
        "type": "Celestial",
        "time": "Noon",
        "offset": "-01:30:00",
        "path": "/home/james/Pictures/Wallpapers/3.png"
    },
    {
        "type": "Celestial",
        "time": "Noon",
        "path": "/home/james/Pictures/Wallpapers/4.png"
    },
    {
        "type": "Celestial",
        "time": "Noon",
        "offset": "01:30:00",
        "path": "/home/james/Pictures/Wallpapers/5.png"
    },
    {
        "type": "Celestial",
        "time": "Sunset",
        "offset": "-01:00:00",
        "path": "/home/james/Pictures/Wallpapers/6.png"
    },
    {
        "type": "Celestial",
        "time": "Sunset",
        "path": "/home/james/Pictures/Wallpapers/7.png"
    },
    {
        "type": "Celestial",
        "time": "Dusk",
        "path": "/home/james/Pictures/Wallpapers/8.png"
    },
    {
        "type": "Time",
        "time": "22:00:00",
        "path": "/home/james/Pictures/Wallpapers/9.png"
    }
  ],
  "latitude": 12.34,
  "longitude": -1.23
}
```

## Downloads
Downloads can be found in releases [here](https://github.com/jamerst/celestial/releases).

Build file sizes are quite large due to bundled .NET runtimes and incompatibilities with trimming.