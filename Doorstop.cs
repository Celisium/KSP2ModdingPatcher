using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Doorstop {
	public class Entrypoint {

		static bool patched = false;

		public static void Start() {

			SceneManager.sceneLoaded += Patch;

		}

		public static void Patch(Scene scene, LoadSceneMode loadSceneMode) {
			
			if (!patched) {
				Harmony.DEBUG = true;

				Harmony harmony = new Harmony("com.github.ididmakethat.ksp2moddingpatcher");
				harmony.PatchAll();

				patched = true;

			}

		}

	}
}
