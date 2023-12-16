﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MattEland.AI.Semantic.Workshop.ConsoleApp.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to This document taken from John Maeda&apos;s post on the Microsoft Blog at https://devblogs.microsoft.com/semantic-kernel/hello-world/
        ///---
        ///Artificial intelligence (AI) and Large Language Models (LLM) are helping to transform the way we develop and interact with software. 
        ///From chatbots to code generators, natural language is the future of user interaction, delivering delightful and intelligent “copilot” experiences. 
        ///As these AI models become more prevalent and accessible, organizations and developers are look [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SemanticKernelAnnouncement {
            get {
                return ResourceManager.GetString("SemanticKernelAnnouncement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are a bot designed to generate ASCII art from a prompt.
        ///
        ///User: I want to see a picture of a dog.
        ///Bot: Here is a picture of a dog:
        ///^..^      /
        ////_/\_____/
        ///   /\   /\
        ///  /  \ /  \
        ///
        ///User: I want to see a picture of an Elephant.
        ///Bot:
        ///____
        ///                   .---&apos;-    \
        ///      .-----------/           \
        ///     /           (         ^  |   __
        ///&amp;   (             \        O  /  / .&apos;
        ///&apos;._/(              &apos;-&apos;  (.   (_.&apos; /
        ///     \                    \     ./
        ///      |    |       |    |/ &apos;._.&apos;
        ///       )   @). [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TextFewShot {
            get {
                return ResourceManager.GetString("TextFewShot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are a bot designed to generate ASCII art from a prompt.
        ///
        ///User: I want to see a picture of a dog.
        ///Bot: Here is a picture of a dog:
        ///```
        ///^..^      /
        ////_/\_____/
        ///   /\   /\
        ///  /  \ /  \
        ///```
        ///
        ///User: I want to see a picture of a disapproving gorilla.
        ///Bot: .
        /// </summary>
        internal static string TextOneShot {
            get {
                return ResourceManager.GetString("TextOneShot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are a bot designed to generate ASCII art from a prompt.
        ///
        ///User: I want to see a picture of a disapproving gorilla.
        ///Bot: .
        /// </summary>
        internal static string TextZeroShot {
            get {
                return ResourceManager.GetString("TextZeroShot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you a budding super hero or super villain* but you feel like you&apos;re lacking that AI companion to help you reach that next level? Have you ever wanted to see what AI can do to help your daily life? Do you have an interesting application that would just be so much better with a little artificial intelligence? Don&apos;t fear, because of course there&apos;s a PreCompiler for that.
        ///
        ///In this half-day workshop we&apos;ll see how to build enterprise-level AI applications by integrating AI vision, speech, and large language [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string WorkshopAbstract {
            get {
                return ResourceManager.GetString("WorkshopAbstract", resourceCulture);
            }
        }
    }
}
