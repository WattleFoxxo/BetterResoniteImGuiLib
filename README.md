# BetterResoniteImGuiLib
 "not cake, its real"

 This is a replacement for [ResoniteImGuiLib by art0007i](https://github.com/art0007i/ResoniteImGuiLib) 

# Why?
 I pulled 3 consecutive all nighters trying to get [ResoniteImGuiLib](https://github.com/art0007i/ResoniteImGuiLib) working with [ResoniteHotReloadLib](https://github.com/Nytra/ResoniteHotReloadLib) thats why.

# Installation
 - Make sure you have [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) Installed.
 - Download [BetterResoniteImGuiLib](https://github.com/WattleFoxxo/BetterResoniteImGuiLib/releases/latest/download/BetterResoniteImGuiLib.dll) and put it in `rml_mods`
 - Download [ImGuiUnityInject](https://github.com/art0007i/ImGuiUnityInject/releases/latest/download/ImGuiUnityInject.dll) and put it in `rml_libs`
 - Download [cimgui-freetype.dll](https://github.com/WattleFoxxo/ImGuiUnityInject/raw/refs/heads/master/Plugins/cimgui-freetype.dll) and [cimgui.dll](https://github.com/WattleFoxxo/ImGuiUnityInject/raw/refs/heads/master/Plugins/cimgui.dll) then put them into `Resonite_Data/Plugins/x86_64`

# Examples
## Basic Usage
The following example displays a window with the text "Hello World!". 
```cs
using HarmonyLib;
using ResoniteModLoader;
using BetterResoniteImGuiLib;
using ImGuiNET;

namespace ImGuiExample;

public class ImGuiExample : ResoniteMod {
    public override string Name => "ImGuiExample";
    public override string Author => "Author";
    public override string Version => "0.1.0";
    public override string Link => "https://example.com/";

    const string harmonyId = "com.example.ImGuiExample";

    public override void OnEngineInit() {
        Setup();
    }

    static void Setup() {
        Harmony harmony = new Harmony(harmonyId);
        harmony.PatchAll();
        ImGuiLib.RegisterUIHandler(ExampleUI);
    }

    static void ExampleUI() {
        ImGui.Begin("ImGuiExample");
        ImGui.Text("Hello World!");
        ImGui.End();
    }
}
```

## Using HotReloadLib
This example uses the HotReloadLib to unregister the old UI handler and register the new UI handler on hot reload.
```cs
using HarmonyLib;
using ResoniteModLoader;
using ResoniteHotReloadLib;
using BetterResoniteImGuiLib;
using ImGuiNET;

namespace ImGuiExample;

public class ImGuiExample : ResoniteMod {
    public override string Name => "ImGuiExample";
    public override string Author => "Author";
    public override string Version => "0.1.0";
    public override string Link => "https://example.com/";

    const string harmonyId = "com.example.ImGuiExample";
    
    public override void OnEngineInit() {
        Setup();
    }

    static void BeforeHotReload() {
        ImGuiLib.UnregisterUIHandler(ExampleUI);
        harmony.UnpatchAll(harmonyId);
    }

    static void OnHotReload(ResoniteMod modInstance) {
        Setup();
    }

    static void Setup() {
        Harmony harmony = new Harmony(harmonyId);
        harmony.PatchAll();
        ImGuiLib.RegisterUIHandler(ExampleUI);
    }

    static void ExampleUI() {
        ImGui.Begin("ImGuiExample");
        ImGui.Text("Hello World!");
        ImGui.End();
    }
}
```

## Capturing Input
This example sets up a Task to listen to the right shift key to toggle input capture on UIs.
```cs
using HarmonyLib;
using ResoniteModLoader;
using BetterResoniteImGuiLib;
using ImGuiNET;

namespace ImGuiExample;

public class ImGuiExample : ResoniteMod {
    public override string Name => "ImGuiExample";
    public override string Author => "Author";
    public override string Version => "0.1.0";
    public override string Link => "https://example.com/";

    const string harmonyId = "com.example.ImGuiExample";
    
    public static bool captureInput = false;
    public static CancellationTokenSource cancellationToken = new CancellationTokenSource();

    public override void OnEngineInit() {
        Task.Run(() => CheckKeyState(cancellationToken.Token), cancellationToken.Token);
        Engine.Current.OnShutdownRequest += (r) => { //End Task on FrooxEngine shut down 
            cancellationToken.Cancel();
        };

        Setup();
    }

    static void Setup() {
        Harmony harmony = new Harmony(harmonyId);
        harmony.PatchAll();
        ImGuiLib.RegisterUIHandler(ExampleUI);
    }

    static void ExampleUI() {
      // Optionally hide the ui when not focused
      // if (!captureInput) return;
      ImGui.Begin("ImGuiExample");
      ImGui.Text("Hello World!");
      ImGui.End();
    }

    internal static void CheckKeyState(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            if (Keyboard.current.rightShiftKey.wasPressedThisFrame) {
                captureInput = !captureInput;

                if (captureInput) {
                    ImGuiLib.CaptureInput();
                } else {
                    ImGuiLib.ReleseInput();
                }

                while (Keyboard.current.rightShiftKey.isPressed) {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
```

# Creddits
 - [art0007i](https://github.com/art0007i) - Making [ImGuiUnityInject](https://github.com/art0007i/ImGuiUnityInject/releases/latest/download/ImGuiUnityInject.dll) and the original mod.
 - [NepuShiro](https://github.com/NepuShiro) - Being real and not cake.
 - [Icecubegame](https://github.com/Icecubegame) ~ "You should use constructors".
