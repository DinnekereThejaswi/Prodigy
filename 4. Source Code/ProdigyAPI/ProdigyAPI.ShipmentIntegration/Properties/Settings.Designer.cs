﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProdigyAPI.ShipmentIntegration.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://netconnect.bluedart.com/Ver1.9/Demo/ShippingAPI/Pickup/PickupRegistrationS" +
            "ervice.svc/Basic")]
        public string ProdigyAPI_ShipmentIntegration_com_bluedart_netconnect_pickup_PickupRegistration {
            get {
                return ((string)(this["ProdigyAPI_ShipmentIntegration_com_bluedart_netconnect_pickup_PickupRegistration"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://netconnect.bluedart.com/Ver1.9/Demo/ShippingAPI/WayBill/WayBillGeneration." +
            "svc/Basic")]
        public string ProdigyAPI_ShipmentIntegration_com_bluedart_netconnect_waybill_WayBillGeneration {
            get {
                return ((string)(this["ProdigyAPI_ShipmentIntegration_com_bluedart_netconnect_waybill_WayBillGeneration"]));
            }
        }
    }
}
