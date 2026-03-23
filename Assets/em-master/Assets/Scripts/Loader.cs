using GB.Graphics;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace GB
{
    public class Loader : MonoBehaviour
    {
        public static Loader instance;

        [SerializeField]
        public Drawer drawer;

        public Core core;

        // 🔥 Control de uso del emulador
        public bool isUsing = false;

        private KeyCode[] keys = {
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
            KeyCode.X, KeyCode.Z, KeyCode.Return, KeyCode.RightShift
        };

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            // ❌ NO cargar automáticamente
            // Load("red");
        }

        public void Load(string path)
        {
            if (core != null)
            {
                SaveSRAM();
                core.initialized = false;
            }

            TextAsset rom = Resources.Load(path) as TextAsset;

            if (rom == null)
            {
                Debug.LogError("ROM no encontrada en Resources: " + path);
                return;
            }

            core = new Core(rom, drawer);
            core.Start();
        }

        private void Update()
        {
            // 🔥 SOLO el jugador activo controla el emulador
            if (!isUsing) return;

            if (core != null && core.initialized)
            {
                core.Run();

                for (int i = 0; i < keys.Length; i++)
                {
                    if (UnityInput.GetKeyDown(keys[i]))
                        core.keyboard.JoyPadEvent(keys[i], true);

                    if (UnityInput.GetKeyUp(keys[i]))
                        core.keyboard.JoyPadEvent(keys[i], false);
                }
            }
        }

        private void SaveSRAM()
        {
            if (core != null && core.initialized)
                core.memory.SaveSRAM();
        }

        private void OnApplicationQuit()
        {
            SaveSRAM();
        }
    }
}