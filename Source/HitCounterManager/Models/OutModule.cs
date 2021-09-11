﻿//MIT License

//Copyright (c) 2016-2021 Peter Kirmeier

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

using System;
using System.IO;

namespace HitCounterManager.Models
{
    /// <summary>
    /// Reads a file, patches it and writes to a new file
    /// </summary>
    public class OutModule
    {
        #region Settings

        public enum OM_Purpose {
            OM_Purpose_SplitCounter = 0,
            OM_Purpose_DeathCounter = 1,
            OM_Purpose_Checklist = 2,
            OM_Purpose_NoDeath = 3,
            OM_Purpose_ResetCounter = 4,
            OM_Purpose_MAX = 5
        };
        public enum OM_Severity {
            OM_Severity_AnyHitsCritical = 0,
            OM_Severity_BossHitCritical = 1,
            OM_Severity_ComparePB = 2,
            OM_Severity_MAX = 3
        };

        public OM_Purpose Purpose
        {
            get { return (_settings.Purpose < (int)OM_Purpose.OM_Purpose_MAX ? (OM_Purpose)_settings.Purpose : OM_Purpose.OM_Purpose_SplitCounter); }
            set { _settings.Purpose = (int)value; }
        }
        public OM_Severity Severity
        {
            get { return (_settings.Severity < (int)OM_Severity.OM_Severity_MAX ? (OM_Severity)_settings.Severity : OM_Severity.OM_Severity_AnyHitsCritical); }
            set { _settings.Severity = (int)value; }
        }

        private SettingsRoot _settings = null;
        private string template = null;

        /// <summary>
        /// Object binding to the user settings
        /// </summary>
        public SettingsRoot Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                ReloadTemplate();
            }
        }

        #endregion

        /// <summary>
        /// Refreshes the file handles.
        /// Call when _settings.Inputfile changes!
        /// </summary>
        public void ReloadTemplate()
        {
            // Reload input file handle when possible
            if (File.Exists(_settings.Inputfile))
            {
                StreamReader sr = new StreamReader(_settings.Inputfile);
                template = sr.ReadToEnd();
                sr.Close();
            }
        }

        #region JSON helpers

        /// <summary>
        /// Escapes special HTML characters
        /// </summary>
        /// <param name="Str">String with special characters</param>
        /// <returns>String with HTML encoded special character</returns>
        public string SimpleHtmlEscape(string Str)
        {
            if (null != Str)
            {
                Str = Str.ToString().Replace("&", "&amp;").Replace(" ", "&nbsp;");
                // Keep for compatibility supporting designs up to version 1.15 as they have not used Unicode:
                Str = Str.Replace("ä", "&auml;").Replace("ö", "&ouml;").Replace("ü", "&uuml;");
                Str = Str.Replace("Ä", "&Auml;").Replace("Ö", "&Ouml;").Replace("Ü", "&Uuml;");
                Str = Str.Replace("\\", "\\\\").Replace("\"", "\\\"");
            }
            return Str;
        }

        /// <summary>
        /// Writes a JSON statement to assign a simple value
        /// </summary>
        private void WriteJsonSimpleValue(StreamWriter File, string Name, bool Bool)
        {
            File.WriteLine("\"" + Name + "\": " + (Bool ? "true" : "false") + ",");
        }
        /// <summary>
        /// Writes a JSON statement to assign a simple value
        /// </summary>
        private void WriteJsonSimpleValue(StreamWriter File, string Name, long Integer)
        {
            File.WriteLine("\"" + Name + "\": " + Integer.ToString() + ",");
        }
        /// <summary>
        /// Writes a JSON statement to assign a simple value
        /// </summary>
        private void WriteJsonSimpleValue(StreamWriter File, string Name, string String)
        {
            File.WriteLine("\"" + Name + "\": " + (null != String ? "\"" + String.Replace("\"", "\\\"") + "\"" : "undefined") + ",");
        }

        #endregion

        /// <summary>
        /// Use buffer to create outputfile while patching some data
        /// </summary>
        public void Update(ProfileModel pi, bool TimerRunning)
        {
            //Console.Beep(); // For debugging to check whenever output is beeing generated :)

            if (null == _settings) return;
            if (null == _settings.OutputFile) return;
            if (null == template) // no valid template read yet?
            {
                ReloadTemplate(); // try to read template again
                if (null == template) return; // still invalid, avoid writing empty output file
            }

            StreamWriter sr;
            bool IsWritingList = false; // Kept for old designs before version 1.10
            bool IsWritingJson = false;
            try
            {
                sr = new StreamWriter(_settings.OutputFile, false, System.Text.Encoding.Unicode); // UTF16LE
            }
            catch { return; }
            sr.NewLine = Environment.NewLine;

            foreach (string line in template.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (IsWritingJson)
                {
                    if (line.Contains("HITCOUNTER_JSON_END")) IsWritingJson = false;
                }
                else if (line.Contains("HITCOUNTER_JSON_START")) // Format data according to RFC 4627 (JSON)
                {
#if TODO
                    int TotalSplits, TotalActiveSplit, SuccessionHits, SuccessionHitsWay, SuccessionHitsPB;
                    long TotalTime;
                    profCtrl.GetCalculatedSums(out TotalSplits, out TotalActiveSplit, out SuccessionHits, out SuccessionHitsWay, out SuccessionHitsPB, out TotalTime, true);
#if TODO
        public void GetCalculatedSums(out int TotalSplits, out int TotalActiveSplit, out int TotalHits, out int TotalHitsWay, out int TotalPB, out long TotalTime, bool PastOnly)
        {
            bool ActiveProfileFound = false;

            TotalSplits = TotalActiveSplit = TotalHits = TotalHitsWay = TotalPB = 0;
            TotalTime = 0;

            foreach (ProfileViewControl pvc_tab in ptc.ProfileViewControls)
            {
                IProfileInfo pi_tab = pvc_tab.ProfileInfo;
                int Splits = pi_tab.SplitCount;

                if ((pi_tab == SelectedProfileInfo) && PastOnly) // When the past should be calculated only, stop when active profile tab found
                    break;

                TotalSplits += Splits;
                for (int i = 0; i < Splits; i++)
                {
                    TotalHits += pi_tab.GetSplitHits(i);
                    TotalHitsWay += pi_tab.GetSplitWayHits(i);
                    TotalPB += pi_tab.GetSplitPB(i);
                    TotalTime += pi_tab.GetSplitDuration(i);
                }

                if (!ActiveProfileFound)
                {
                    if (pi_tab == SelectedProfileInfo) // Active profile tab found
                    {
                        TotalActiveSplit += pi_tab.ActiveSplit;
                        ActiveProfileFound = true;
                    }
                    else TotalActiveSplit += Splits; // Add all splits of preceeding profiles
                }
            }
        }
#endif
#endif
                    int active = pi.ActiveSplit;
                    int iSplitCount = pi.Rows.Count;
                    int iSplitFirst;
                    int iSplitLast;
                    int InjectedSplitCount = 0;
                    int HiddenSplitCount = 0;
                    int RunIndex = 1;
                    int RunIndexActive;

                    sr.WriteLine("{");

                    sr.WriteLine("\"list\": [");
#if TODO
                    // ----- Splits of all previous runs:
                    if (_settings.Succession.IntegrateIntoProgressBar)
                    {
                        // Dump all splits of the (previous) non-visible profiles
                        foreach(ProfileViewControl pvc in profCtrl.ProfileTabControl.ProfileViewControls)
                        {
                            IProfileInfo pi_pvc = pvc.ProfileInfo;
                            if (pi_pvc == pi) break; // stop at current profile

                            for (int r = 0; r < pi_pvc.SplitCount; r++)
                            {
                                if (0 < r + HiddenSplitCount) sr.WriteLine(","); // separator
                                sr.Write("[\"" + SimpleHtmlEscape(pi_pvc.GetSplitTitle(r)) + "\", " + (pi_pvc.GetSplitHits(r) + pi_pvc.GetSplitWayHits(r)) + ", " + pi_pvc.GetSplitPB(r) + ", " + pi_pvc.GetSplitWayHits(r) + ", " + RunIndex + ", " + 0/*NoTime*/ + ", " + 0/*NoPBTime*/ + "]");
                            }
                            HiddenSplitCount += pi_pvc.SplitCount;
                            RunIndex++;
                        }
                    }
#endif

                    // ----- Splits of the current run:
                    RunIndexActive = RunIndex;
#if TODO
                    if (_settings.Succession.HistorySplitVisible && (1 < RunIndex))
                    {
                        // Insert the "history" split
                        InjectedSplitCount++;
                        if (0 < HiddenSplitCount) sr.WriteLine(","); // separator
                        sr.Write("[\"" + SimpleHtmlEscape(_settings.Succession.HistorySplitTitle) + "\", "
                            + (SuccessionHits + SuccessionHitsWay) + ", " + SuccessionHitsPB + ", " + SuccessionHitsWay + ", "
                            + 0 /*Invalid-RunIndex*/ + ", " + 0/*NoTime*/ + ", " + 0/*NoPBTime*/ + ", " + 0/*NoGoldTime*/ + "]");
                    }
#endif
                    for (int r = 0; r < iSplitCount; r++)
                    {
                        // Dump all actually visible splits of the current run
                        if (0 < r + HiddenSplitCount + InjectedSplitCount) sr.WriteLine(","); // separator
                        sr.Write("[\"" + SimpleHtmlEscape(pi.Rows[r].Title) + "\", "
                            + (pi.Rows[r].Hits + pi.Rows[r].WayHits) + ", " + pi.Rows[r].PB + ", " + pi.Rows[r].WayHits + ", " // TODO: Rework Hits/WayHits/PB combinations
                            + RunIndex + ", " + pi.Rows[r].Duration + ", " + pi.Rows[r].DurationPB + ", " + pi.Rows[r].DurationGold + "]");
                    }

                    // ----- Splits of the upcoming runs:
                    RunIndex++;
#if TODO
                    if (_settings.Succession.IntegrateIntoProgressBar)
                    {
                        bool found_active = false;
                        // Dump all splits of the (upcoming) non-visible profiles
                        foreach(ProfileViewControl pvc in profCtrl.ProfileTabControl.ProfileViewControls)
                        {
                            IProfileInfo pi_pvc = pvc.ProfileInfo;
                            if (found_active) // only walk over upcoming profiles
                            {
                                for (int r = 0; r < pi_pvc.SplitCount; r++)
                                {
                                    if (0 < r + HiddenSplitCount + InjectedSplitCount + iSplitCount) sr.WriteLine(","); // separator
                                    sr.Write("[\"" + SimpleHtmlEscape(pi_pvc.GetSplitTitle(r)) + "\", "
                                        + (pi_pvc.GetSplitHits(r) + pi_pvc.GetSplitWayHits(r)) + ", " + pi_pvc.GetSplitPB(r) + ", " + pi_pvc.GetSplitWayHits(r) + ", "
                                        + RunIndex + ", " + 0/*NoTime*/ + ", " + 0/*NoPBTime*/ + ", " + 0/*NoGoldTime*/ + "]");
                                }
                                RunIndex++;
                            }
                            else if (pi_pvc == pi) found_active = true;
                        }
                    }
#endif
                    sr.WriteLine(""); // no trailing separator
                    sr.WriteLine("],");

                    active += InjectedSplitCount;
                    iSplitCount += InjectedSplitCount;
                    WriteJsonSimpleValue(sr, "session_progress", pi.SessionProgress + InjectedSplitCount + HiddenSplitCount);

                    // Calculation to show same amount of splits independent from active split:
                    // Example: ShowSplitsCountFinished = 3 , ShowSplitsCountUpcoming = 2 , iSplitCount = 7 (0-6)
                    //  A:  B:  C:  D:  E:  F:  G:
                    // <0>  0   0   0
                    //  1  <1>  1   1   1   1   1
                    //  2   2  <2>  2   2   2   2
                    //  3   3   3  <3>  3   3   3
                    //  4   4   4   4  <4>  4   4
                    //  5   5   5   5   5  <5>  5
                    //                  6   6  <6>
// TODO: Fix calculation (set min =2 and max=999, it will show all) - same issue on old HCM!!
                    if (active < _settings.ShowSplitsCountFinished) // A-C: less previous, more upcoming
                    {
                        iSplitFirst = 0;
                        iSplitLast = _settings.ShowSplitsCountUpcoming + _settings.ShowSplitsCountFinished;
                    }
                    else if (iSplitCount - active > _settings.ShowSplitsCountUpcoming) // D-E: previous and upcoming as it is
                    {
                        iSplitFirst = active - _settings.ShowSplitsCountFinished;
                        iSplitLast = active + _settings.ShowSplitsCountUpcoming;
                    }
                    else // F-G: more previous, less upcoming
                    {
                        iSplitFirst = iSplitCount - 1 - _settings.ShowSplitsCountUpcoming - _settings.ShowSplitsCountFinished;
                        iSplitLast = iSplitCount - 1;
                    }

                    // safety limiters
                    if (iSplitFirst < 0) iSplitFirst = 0;
                    if (iSplitCount <= iSplitLast) iSplitLast =  iSplitCount-1;

                    active += HiddenSplitCount;
                    iSplitFirst += HiddenSplitCount;
                    iSplitLast += HiddenSplitCount;
                    WriteJsonSimpleValue(sr, "run_active", RunIndexActive);
                    WriteJsonSimpleValue(sr, "split_active", active);
                    WriteJsonSimpleValue(sr, "split_first", iSplitFirst);
                    WriteJsonSimpleValue(sr, "split_last", iSplitLast);

                    WriteJsonSimpleValue(sr, "attempts", pi.Attempts);
                    WriteJsonSimpleValue(sr, "show_attempts", _settings.ShowAttemptsCounter);
                    WriteJsonSimpleValue(sr, "show_headline", _settings.ShowHeadline);
                    WriteJsonSimpleValue(sr, "show_footer", _settings.ShowFooter);
                    WriteJsonSimpleValue(sr, "show_session_progress", _settings.ShowSessionProgress);
                    WriteJsonSimpleValue(sr, "show_progress_bar", _settings.ShowProgressBar);
                    WriteJsonSimpleValue(sr, "show_hits", _settings.ShowHits);
                    WriteJsonSimpleValue(sr, "show_hitscombined", _settings.ShowHitsCombined);
                    WriteJsonSimpleValue(sr, "show_numbers", _settings.ShowNumbers);
                    WriteJsonSimpleValue(sr, "show_pb", _settings.ShowPB);
                    WriteJsonSimpleValue(sr, "show_diff", _settings.ShowDiff);
                    WriteJsonSimpleValue(sr, "show_time", _settings.ShowTimeCurrent);
                    WriteJsonSimpleValue(sr, "show_time_pb", _settings.ShowTimePB);
                    WriteJsonSimpleValue(sr, "show_time_diff", _settings.ShowTimeDiff);
                    WriteJsonSimpleValue(sr, "show_time_footer", _settings.ShowTimeFooter);
                    WriteJsonSimpleValue(sr, "purpose", (int)Purpose);
                    WriteJsonSimpleValue(sr, "severity", (int)Severity);

                    WriteJsonSimpleValue(sr, "font_name", (_settings.StyleUseCustom ? _settings.StyleFontName : null));
                    WriteJsonSimpleValue(sr, "font_url", (_settings.StyleUseCustom ? _settings.StyleFontUrl : ""));
                    WriteJsonSimpleValue(sr, "css_url", (_settings.StyleUseCustom ? _settings.StyleCssUrl : "stylesheet.css"));
                    WriteJsonSimpleValue(sr, "high_contrast", _settings.StyleUseHighContrast);
                    WriteJsonSimpleValue(sr, "high_contrast_names", _settings.StyleUseHighContrastNames);
                    WriteJsonSimpleValue(sr, "use_roman", _settings.StyleUseRoman);
                    WriteJsonSimpleValue(sr, "highlight_active_split", _settings.StyleHightlightCurrentSplit);
                    WriteJsonSimpleValue(sr, "progress_bar_colored", _settings.StyleProgressBarColored);
                    WriteJsonSimpleValue(sr, "width", _settings.StyleDesiredWidth);
                    WriteJsonSimpleValue(sr, "supPB", _settings.StyleSuperscriptPB);
                    
                    WriteJsonSimpleValue(sr, "update_time", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                    WriteJsonSimpleValue(sr, "timer_paused", !TimerRunning);

                    sr.WriteLine("}");

                    IsWritingJson = true;
                }
                else if ((!IsWritingList) && (!IsWritingJson))
                {
                    sr.WriteLine(line.Replace(Environment.NewLine, ""));
                }
            }

            sr.Close();
        }
    }
}