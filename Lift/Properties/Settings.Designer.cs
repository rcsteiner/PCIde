﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lift.Properties {
    
    
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("0")]
        public double Top {
            get {
                return ((double)(this["Top"]));
            }
            set {
                this["Top"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("0")]
        public double Left {
            get {
                return ((double)(this["Left"]));
            }
            set {
                this["Left"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("800")]
        public double Height {
            get {
                return ((double)(this["Height"]));
            }
            set {
                this["Height"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("400")]
        public double Width {
            get {
                return ((double)(this["Width"]));
            }
            set {
                this["Width"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("True")]
        public bool Maximized {
            get {
                return ((bool)(this["Maximized"]));
            }
            set {
                this["Maximized"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("500")]
        public double StatsLeft {
            get {
                return ((double)(this["StatsLeft"]));
            }
            set {
                this["StatsLeft"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("20")]
        public double StatsTop {
            get {
                return ((double)(this["StatsTop"]));
            }
            set {
                this["StatsTop"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("100")]
        public double MaxTime {
            get {
                return ((double)(this["MaxTime"]));
            }
            set {
                this["MaxTime"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("4")]
        public int MaxPassangers {
            get {
                return ((int)(this["MaxPassangers"]));
            }
            set {
                this["MaxPassangers"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("1")]
        public int ElevatorCount {
            get {
                return ((int)(this["ElevatorCount"]));
            }
            set {
                this["ElevatorCount"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("4")]
        public int FloorCount {
            get {
                return ((int)(this["FloorCount"]));
            }
            set {
                this["FloorCount"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("0.5")]
        public double SpawnRate {
            get {
                return ((double)(this["SpawnRate"]));
            }
            set {
                this["SpawnRate"] = value;
            }
        }
    }
}