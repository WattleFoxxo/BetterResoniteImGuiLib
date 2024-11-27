using System;
using System.Linq;
using Elements.Core;
using HarmonyLib;
using ResoniteModLoader;
using ImGuiUnityInject;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityFrooxEngineRunner;

namespace BetterResoniteImGuiLib;

public class BetterResoniteImGuiLib : ResoniteMod {

    public override string Name => "BetterResoniteGuiLib";
    public override string Author => "WattleFoxxo (wattle@wattlefoxxo.au)";
    public override string Version => "0.1.0";
    public override string Link => "https://www.wattlefoxxo.au/BetterResoniteGuiLib";
	
    public static event Action uiHandler;
	public static bool captureInput = false;

    public override void OnEngineInit() {
        Harmony harmony = new Harmony("au.wattlefoxxo.BetterResoniteGuiLib");
        harmony.PatchAll();
		
		ImGuiReady onReady = null;
		ImGuiInstance.GetOrCreate((gui, isNew) => {
			if (isNew) {
				gui._camera = SceneManager.GetActiveScene().GetRootGameObjects().Where(go => go.name == "FrooxEngine").First().GetComponent<FrooxEngineRunner>().OverlayCamera;
			}
			
			if (onReady != null) {
				onReady(gui, isNew);
			} else {
				gui.enabled = true;
			}
		}).Layout += () => {
			uiHandler?.Invoke();
		};
    }

    [HarmonyPatch(typeof(MouseDriver), "UpdateMouse")]
	class CursorUpdatePatch {
		public static bool Prefix(FrooxEngine.Mouse mouse) {
			if (captureInput) {
				mouse.LeftButton.UpdateState(false);
				mouse.RightButton.UpdateState(false);
				mouse.MiddleButton.UpdateState(false);
				mouse.MouseButton4.UpdateState(false);
				mouse.MouseButton5.UpdateState(false);
				mouse.DirectDelta.UpdateValue(float2.Zero, Time.deltaTime);
				mouse.ScrollWheelDelta.UpdateValue(float2.Zero, Time.deltaTime);
				mouse.NormalizedScrollWheelDelta.UpdateValue(float2.Zero, Time.deltaTime);
				return false;
			}

			return true;
		}
	}

    [HarmonyPatch(typeof(KeyboardDriver), "Current_onTextInput")]
	class KeyboardDeltaPatch {
		public static bool Prefix() {
			if (captureInput) {
				return false;
			}

			return true;
		}
	}
	
	[HarmonyPatch(typeof(KeyboardDriver), "GetKeyState")]
	class KeyboardStatePatch {
		public static bool Prefix(ref bool __result) {
			if (captureInput) {
				__result = false;
				return false;
			}

			return true;
		}
	}
}

public static class ImGuiLib {
	public static void RegisterUIHandler(Action handler) {
		BetterResoniteImGuiLib.uiHandler += handler;
	}

	public static void UnregisterUIHandler(Action handler) {
		BetterResoniteImGuiLib.uiHandler -= handler;
	}

    public static void CaptureInput() {
        BetterResoniteImGuiLib.captureInput = true;
    }

    public static void ReleseInput() {
        BetterResoniteImGuiLib.captureInput = false;
    }
}
