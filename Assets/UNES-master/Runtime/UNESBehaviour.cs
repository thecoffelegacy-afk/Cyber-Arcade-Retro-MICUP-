using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Aya.UNES.Controller;
using Aya.UNES.Input;
using Aya.UNES.Renderer;

namespace Aya.UNES
{
    // Interfaz para tratar diferentes emuladores de manera genérica
    public interface IUNES
    {
        void Boot(byte[] romData);
        uint[] RawBitmap { get; }
        RenderTexture RenderTexture { get; }
    }

    public class UNESBehaviour : MonoBehaviour, IUNES
    {
        [Header("Render")]
        public RenderTexture RenderTexture;
        public const int GameWidth = 256;
        public const int GameHeight = 240;
        public FilterMode FilterMode = FilterMode.Point;
        public bool LogicThread = true;

        [Header("Input")]
        public KeyConfig KeyConfig;

        public BaseInput Input { get; private set; }
        public IRenderer Renderer { get; private set; }
        public uint[] RawBitmap { get; private set; } = new uint[GameWidth * GameHeight];
        public bool Ready { get; private set; }
        public bool GameStarted { get; private set; }

        private bool _rendererRunning = true;
        private bool _isBooting = false;
        private IController _controller;
        private Emulator _emu;
        private bool _suspended;
        private Thread _renderThread;
        private int _activeSpeed = 1;

        // Implementación de interfaz
        RenderTexture IUNES.RenderTexture => RenderTexture;
        uint[] IUNES.RawBitmap => RawBitmap;

        /// <summary>
        /// Boot de la ROM en tiempo de ejecución
        /// </summary>
        public void Boot(byte[] romData)
        {
            if (romData == null || romData.Length == 0)
            {
                UnityEngine.Debug.LogError("[UNES] ROM inválida o vacía");
                return;
            }

            _isBooting = true;

            // Crear RenderTexture si no existe
            if (RenderTexture == null)
            {
                RenderTexture = new RenderTexture(GameWidth, GameHeight, 0);
                RenderTexture.filterMode = FilterMode.Point;
            }

            InitInput();
            InitRenderer();

            BootCartridge(romData);

            GameStarted = true;
            _isBooting = false;

            UnityEngine.Debug.Log("[UNES] ROM lista y emulador iniciado");
        }

        public void LoadSaveData(byte[] saveData)
        {
            _emu?.Mapper.LoadSaveData(saveData);
        }

        public byte[] GetSaveData()
        {
            return _emu?.Mapper.GetSaveData();
        }

        private void BootCartridge(byte[] romData)
        {
            _controller = new NesController(this);
            _emu = new Emulator(romData, _controller);
            _emu.CPU.Initialize();  // Asegurar CPU listo

            if (LogicThread)
            {
                _renderThread = new Thread(RenderThreadLoop);
                _renderThread.IsBackground = true;
                _renderThread.Start();
            }
        }

        private void RenderThreadLoop()
        {
            var s = new Stopwatch();
            var s0 = new Stopwatch();

            while (_rendererRunning)
            {
                if (_suspended || _emu == null || _emu.PPU == null || _emu.Mapper == null || _isBooting)
                {
                    Thread.Sleep(50);
                    continue;
                }

                s.Restart();
                for (var i = 0; i < 60 && !_suspended; i++)
                {
                    s0.Restart();
                    lock (RawBitmap)
                    {
                        _emu.PPU.ProcessFrame();
                        RawBitmap = _emu.PPU.RawBitmap;
                    }

                    s0.Stop();
                    Thread.Sleep(Math.Max((int)(980 / 60.0 - s0.ElapsedMilliseconds), 0) / _activeSpeed);
                }
                s.Stop();
            }
        }

        #region Monobehaviour

        public void OnDisable()
        {
            _rendererRunning = false;
            Renderer?.End();
        }

        public void Update()
        {
            if (!GameStarted) return;
            UpdateInput();
            UpdateRender();
        }

        #endregion

        #region Input

        private void InitInput()
        {
            _controller = new NesController(this);
            Input = new DefaultInput();
        }

        public void UpdateInput()
        {
            if (Input == null || _controller == null) return;

            Input.HandlerKeyDown(keyCode =>
            {
                switch (keyCode)
                {
                    case KeyCode.F2: _suspended = false; break;
                    case KeyCode.F3: _suspended = true; break;
                    default: _controller.PressKey(keyCode); break;
                }
            });

            Input.HandlerKeyUp(keyCode =>
            {
                _controller.ReleaseKey(keyCode);
            });
        }

        #endregion

        #region Render

        public void UpdateRender()
        {
            if (!_rendererRunning || _suspended || _emu == null || Renderer == null || _isBooting) return;

            if (!LogicThread)
            {
                _emu.PPU.ProcessFrame();
                RawBitmap = _emu.PPU.RawBitmap;
                Renderer.HandleRender();
            }
            else
            {
                lock (RawBitmap)
                {
                    Renderer.HandleRender();
                }
            }
        }

        private void InitRenderer()
        {
            Renderer?.End();
            Renderer = new UnityRenderer();
            Renderer.Init(this);
        }

        public byte[] GetMemory()
        {
            return _emu?.GetRAM();
        }

    }

    #endregion
}

