﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheOracle.BotCore {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class HelpResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal HelpResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TheOracle.BotCore.HelpResources", typeof(HelpResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string About {
            get {
                return ResourceManager.GetString("About", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use `!Help &lt;command&gt;` for detailed information or check the GitHub Page: https://github.com/XenotropicDev/TheOracle
        ///
        ///To add images to bot messages use an inline reply with a picture attachement, or a direct URL to an image (note: message must have an embed to add the image to).
        ///
        ///Use one of the following reactions on any message from TheOracle:
        ///📌 - `:push_pin:` Pin the message in the channel (remove your reaction to unpin).
        ///⏬ - `:arrow_double_down:` Recreates a bot message at the bottom of the chat, a [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AdditionalInfo {
            get {
                return ResourceManager.GetString("AdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **Aliases:** {0}.
        /// </summary>
        internal static string AliasList {
            get {
                return ResourceManager.GetString("AliasList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **{0}** - Help.
        /// </summary>
        internal static string CommandTitle {
            get {
                return ResourceManager.GetString("CommandTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No command could be found with that name..
        /// </summary>
        internal static string NoCommandError {
            get {
                return ResourceManager.GetString("NoCommandError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **__TheOracle Bot Help__**.
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The input matched too many commands, please be more specific.
        /// </summary>
        internal static string TooManyMatches {
            get {
                return ResourceManager.GetString("TooManyMatches", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **Usage:** {0}.
        /// </summary>
        internal static string Usage {
            get {
                return ResourceManager.GetString("Usage", resourceCulture);
            }
        }
    }
}
