﻿//MIT License

//Copyright (c) 2021-2021 Peter Kirmeier

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Reflection;

namespace HitCounterManager.Common
{
    /// <summary>
    /// Miscellaneous statics
    /// </summary>
    public static class Statics
    {
        /// <summary>
        /// Name of the running OsLayer
        /// </summary>
        public static string CurrentOsLayerName { get => App.CurrentApp.OsLayer.Name; }

        /// <summary>
        /// Name of the running PlatformLayer
        /// </summary>
        public static string CurrentPlatformLayerName { get => App.CurrentApp.PlatformLayer.Name; }

        /// <summary>
        /// Name of the running application
        /// </summary>
        public static string ApplicationName { get => Assembly.GetExecutingAssembly().GetName().Name; }

        /// <summary>
        /// Version string of the running application
        /// </summary>
        public static string ApplicationVersionString { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(); }

        /// <summary>
        /// Titel string of the application including its version
        /// </summary>
        public static string ApplicationTitle { get => ApplicationName + " - v" + ApplicationVersionString + " ALPHAVERSION"; }

        /// <summary>
        /// Application is capable of global hotkeys
        /// </summary>
        public static bool GlobalHotKeySupport { get => App.CurrentApp.OsLayer.GlobalHotKeySupport; }

    }
}