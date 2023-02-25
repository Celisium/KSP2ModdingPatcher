using HarmonyLib;
using KSP.Api.CoreTypes;
using KSP.Game;
using KSP.Modding;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KSP2ModdingPatcher {

	class Util {
		public static void dumpGameObject(GameObject gameObject, int indent = 0) {

			string indent_base = new string('\t', indent);

			Debug.LogFormat("{0}|{1}:", indent_base, gameObject.name);
			foreach (Component component in gameObject.GetComponents<Component>()) {
				Debug.LogFormat("{0}|\t{1}", indent_base, component.GetType().Name);
			}

			for (int i = 0; i < gameObject.transform.childCount; i++) {
				dumpGameObject(gameObject.transform.GetChild(i).gameObject, indent + 1);
			}

		}
	}

	[HarmonyPatch(typeof(KSP2Mod))]
	[HarmonyPatch(nameof(KSP2Mod.Load))]
	class PatchKSP2ModLoad {

		public static void Postfix(KSP2Mod __instance, ref bool __result, ref IKSP2ModCore ___modCore, ref KSP2ModState ___currentState) {

			Debug.Log("Using patched KSP2Mod.Load");

			if (__instance.CurrentState == KSP2ModState.Inactive) {

				if (__instance.EntryPoint.EndsWith(".lua")) {
					___modCore = new KSP2LuaModCore(__instance.APIVersion, __instance.ModName, __instance.EntryPoint, __instance.ModRootPath);
					__result = ___modCore.ModCoreState == KSP2ModCoreState.Active;
					___currentState = KSP2ModState.Active;
				}

			}

			if (__instance.CurrentState == KSP2ModState.Active) {
				___modCore.ModStart();
			}

		}

	}

	[HarmonyPatch(typeof(KSP.Game.StartupFlow.LandingHUD))]
	[HarmonyPatch("Start")]
	class PatchKSPLandingHUDStart {

		public static void Postfix(KSP.Game.StartupFlow.LandingHUD __instance) {

			Transform menuItemsGroupTransform = __instance.transform.FindChildEx("MenuItemsGroup");

			Transform singleplayerButtonTransform = menuItemsGroupTransform.FindChildEx("Singleplayer");

			GameObject modsButton = Object.Instantiate(singleplayerButtonTransform.gameObject, menuItemsGroupTransform, false);
			modsButton.name = "Mods";

			// Move the button to be above the Exit button.
			modsButton.transform.SetSiblingIndex(modsButton.transform.GetSiblingIndex() - 1);

			// Rebind the button's action to open the mod manager dialog.
			UIAction_Void_Button uiAction = modsButton.GetComponent<UIAction_Void_Button>();
			DelegateAction action = new DelegateAction();
			action.BindDelegate(ModsOnClick);
			uiAction.BindAction(action);

			// Set the label to "Mods".
			TextMeshProUGUI tmp = modsButton.GetComponentInChildren<TextMeshProUGUI>();
			tmp.SetText("Mods");

		}

		static void ModsOnClick() {
			GameManager.Instance.Game.ShowModManagerDialog();
		}

	}

}
