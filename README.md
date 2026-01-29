# SafeFallServer (Valheim Server Mod)

Project: SafeFallServer — server-side Valheim mod (BepInEx + Harmony)

Build (Windows recommended):

1. Open the project in Visual Studio with .NET Framework 4.8 installed, or use MSBuild.

2. Build configuration: `Release | x64`.

Output: `SafeFallServer.dll` (found in `bin/Release`)

Required DLL references (from your Valheim server install):
- `Assembly-CSharp.dll`
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`
- `BepInEx.dll` (BepInEx/core)
- `0Harmony.dll` (BepInEx/core)

Place the built `SafeFallServer.dll` into your server's BepInEx plugins folder. Example target folder you provided:

/home/Lucasajones24/Desktop/new mod/

Notes:
- This plugin is purely server-side and checks `ZNet.instance.IsServer()`.
- Admin players are exempt via `ZNet.instance.IsAdmin(...)`.
- Fall tracking uses `Time.deltaTime` and Harmony postfix on `Player.Update()`.
