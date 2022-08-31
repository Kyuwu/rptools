using System;
using System.Collections.Generic;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.ContextMenu;
using Dalamud.IoC;
using Dalamud.Plugin;
using RoleplayersToolbox.Tools;
using RoleplayersToolbox.Tools.Targeting;
using XivCommon;
using RoleplayersToolbox.Tools.Illegal.Emote;
using RoleplayersToolbox.Tools.Illegal.EmoteSnap;

namespace RoleplayersToolbox {
    internal class Plugin : IDalamudPlugin {
        #if DEBUG
        public string Name => "The Roleplayer's Toolbox (Debug)";
        #else
        public string Name => "The Roleplayer's Toolbox";
        #endif

        [PluginService]
        internal DalamudPluginInterface Interface { get; init; } = null!;

        [PluginService]
        internal ChatGui ChatGui { get; init; } = null!;

        [PluginService]
        internal ClientState ClientState { get; init; } = null!;

        [PluginService]
        internal CommandManager CommandManager { get; init; } = null!;
        
        [PluginService]
        internal DalamudContextMenu ContextMenu { get; init; } = null!;

        [PluginService]
        internal DataManager DataManager { get; init; } = null!;

        [PluginService]
        internal Framework Framework { get; init; } = null!;

        [PluginService]
        internal GameGui GameGui { get; init; } = null!;

        [PluginService]
        internal ObjectTable ObjectTable { get; init; } = null!;

        [PluginService]
        internal SigScanner SigScanner { get; init; } = null!;

        internal Configuration Config { get; }
        internal XivCommonBase Common { get; }
        internal List<ITool> Tools { get; } = new();
        internal PluginUi Ui { get; }
        private Commands Commands { get; }

        public Plugin() {
            this.Config = this.Interface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Common = new XivCommonBase(Hooks.PartyFinderListings);

            this.Tools.Add(new TargetingTool(this));

            this.Tools.Add(new EmoteTool(this));
            this.Tools.Add(new EmoteSnapTool(this));

            this.Ui = new PluginUi(this);

            this.Commands = new Commands(this);
        }

        public void Dispose() {
            this.Commands.Dispose();
            this.Ui.Dispose();

            foreach (var tool in this.Tools) {
                if (tool is IDisposable disposable) {
                    disposable.Dispose();
                }
            }

            this.Tools.Clear();

            this.Common.Dispose();
        }

        internal void SaveConfig() {
            this.Interface.SavePluginConfig(this.Config);
        }
    }
}
