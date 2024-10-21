# Disco
A Groovy external client mod for Discord

Compatibility:
| OS | Working? |
|----|----|
| Windows | ✅ (TESTED) |
| Linux | ❌ (UNTESTED) |
| OSX | ❌ (UNTESTED) |

Feel free to PR support for your favorite OS!

## ⚠️ Warning
Client mods are officially prohibited by Discord. Use this at your own risk! Technically this mod does not modify the client itself, but rather injects javascript code through the built-in electron debugger. To Discord this will most likely still be considered a client mod, though! 

By using Disco, you acknowledge and agree that I, Naamloos, am not responsible for any damages to your Discord account, including but not limited to account bans or other penalties.

## Features
- Custom CSS styling
- Hot-Reload for CSS
- Enables "Experiments"

## To-do
- [ ] Front-end with configuration
- [ ] Custom script injection
- [ ] Allow disabling the experiments enabler among other things
- [ ] Custom JS scripting API
- [ ] C# plugin API

## Limitations
Due to Discord limiting the domains that images and other resources may be loaded from, you may encounter issues with external styles or resources. At this moment it's not possible for me to circumvent this, which is actually kind of a good thing, technically!
### Possible workarounds
*Note: None of these have been tested so far, feel free to notify me whether these work! (via GitHub issues or on Discord)*
- Install something like OpenASAR which removes these limitations (You'd now be modifying your client's code, which is against the goal of this project!)
- Host your resources on your local disk (I do not know whether this may actually work.)

## How to use
Using Disco is quite simple. All you need to do is Run Disco, which restarts your Discord client with electron's remote debugger enabled. Disco will use the debugger to run two scripts: `Patcher.js`, which injects Disco's styling, and `Experiments.js`, which enabled Discord's experiments tab.

To modify CSS, edit the `style.css` file in Disco's directory. Do take note that Disco does not include any of the fancy stuff other client mods have, such as sass/scss support or importing styles from external resources. Luckily, Disco does have a hot-reload feature that will hot-reload your styling as you modify it.

At the moment, Disco does not support custom scripts. When it does, do expect it to be significantly less powerful than something like BetterDiscord or Vencord due to the fact Disco does not actually modify the actual client code.