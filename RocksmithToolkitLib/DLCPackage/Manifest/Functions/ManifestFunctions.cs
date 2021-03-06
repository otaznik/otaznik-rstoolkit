﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using System.ComponentModel;
using System.Reflection;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class ManifestFunctions
    {
        int[] songPartitionCount = { 0 /* Combo count */, 0 /* Lead count */, 0 /* Rhythm count */, 0 /* Bass Count */ };
        private Dictionary<string, string> SectionUINames { get; set; }

        public ManifestFunctions(GameVersion gameVersion)
        {
            switch (gameVersion) {
                case GameVersion.RS2012:
                    SectionUINames = new Dictionary<string, string>();
                    SectionUINames.Add("intro", "$[6005] Intro [1]");
                    SectionUINames.Add("outro", "$[6006] Outro [1]");
                    SectionUINames.Add("verse", "$[6007] Verse [1]");
                    SectionUINames.Add("chorus", "$[6008] Chorus [1]");
                    SectionUINames.Add("bridge", "$[6009] Bridge [1]");
                    SectionUINames.Add("solo", "$[6010] Solo [1]");
                    SectionUINames.Add("ambient", "$[6011] Ambient [1]");
                    SectionUINames.Add("breakdown", "$[6012] Breakdown [1]");
                    SectionUINames.Add("interlude", "$[6013] Interlude [1]");
                    SectionUINames.Add("prechorus", "$[6014] Pre Chorus [1]");
                    SectionUINames.Add("transition", "$[6015] Transition [1]");
                    SectionUINames.Add("postchorus", "$[6016] Post Chorus [1]");
                    SectionUINames.Add("hook", "$[6017] Hook [1]");
                    SectionUINames.Add("riff", "$[6018] Riff [1]");
                    SectionUINames.Add("fadein", "$[6077] Fade In [1]");
                    SectionUINames.Add("fadeout", "$[6078] Fade Out [1]");
                    SectionUINames.Add("buildup", "$[6079] Buildup [1]");
                    SectionUINames.Add("preverse", "$[6080] Pre Verse [1]");
                    SectionUINames.Add("modverse", "$[6081] Modulated Verse [1]");
                    SectionUINames.Add("postvs", "$[6082] Post Verse [1]");
                    SectionUINames.Add("variation", "$[6083] Variation [1]");
                    SectionUINames.Add("modchorus", "$[6084] Modulated Chorus [1]");
                    SectionUINames.Add("head", "$[6085] Head [1]");
                    SectionUINames.Add("modbridge", "$[6086] Modulated Bridge [1]");
                    SectionUINames.Add("melody", "$[6087] Melody [1]");
                    SectionUINames.Add("postbrdg", "$[6088] Post Bridge [1]");
                    SectionUINames.Add("prebrdg", "$[6089] Pre Bridge [1]");
                    SectionUINames.Add("vamp", "$[6090] Vamp [1]");
                    SectionUINames.Add("noguitar", "$[6091] No Guitar [1]");
                    SectionUINames.Add("silence", "$[6092] Silence [1]");
                    break;
                case GameVersion.RS2014:
                    SectionUINames = new Dictionary<string, string>();
                    SectionUINames.Add("fadein", "$[34276] Fade In [1]");
                    SectionUINames.Add("fadeout", "$[34277] Fade Out [1]");
                    SectionUINames.Add("buildup", "$[34278] Buildup [1]");
                    SectionUINames.Add("chorus", "$[34279] Chorus [1]");
                    SectionUINames.Add("hook", "$[34280] Hook [1]");
                    SectionUINames.Add("head", "$[34281] Head [1]");
                    SectionUINames.Add("bridge", "$[34282] Bridge [1]");
                    SectionUINames.Add("breakdown", "$[34284] Breakdown [1]");//Where is 34283 ???
                    SectionUINames.Add("interlude", "$[34285] Interlude [1]");
                    SectionUINames.Add("intro", "$[34286] Intro [1]");
                    SectionUINames.Add("melody", "$[34287] Melody [1]");
                    SectionUINames.Add("modbridge", "$[34288] Modulated Bridge [1]");
                    SectionUINames.Add("modchorus", "$[34289] Modulated Chorus [1]");
                    SectionUINames.Add("modverse", "$[34290] Modulated Verse [1]");
                    SectionUINames.Add("outro", "$[34291] Outro [1]");
                    SectionUINames.Add("postbrdg", "$[34292] Post Bridge [1]");
                    SectionUINames.Add("postchorus", "$[34293] Post Chorus [1]");
                    SectionUINames.Add("postvs", "$[34294] Post Verse [1]");
                    SectionUINames.Add("prebrdg", "$[34295] Pre Bridge [1]");
                    SectionUINames.Add("prechorus", "$[34296] Pre Chorus [1]");
                    SectionUINames.Add("preverse", "$[34297] Pre Verse [1]");
                    SectionUINames.Add("riff", "$[34298] Riff [1]");//Where is 34299 ???
                    SectionUINames.Add("solo", "$[34300] Solo [1]");
                    SectionUINames.Add("transition", "$[34301] Transition [1]");
                    SectionUINames.Add("vamp", "$[34302] Vamp [1]");
                    SectionUINames.Add("variation", "$[34303] Variation [1]");
                    SectionUINames.Add("verse", "$[34304] Verse [1]");
                    SectionUINames.Add("noguitar", "$[6091] No Guitar [1]");
                    SectionUINames.Add("silence", "$[6092] Silence [1]");//Not found in RS2014
                    SectionUINames.Add("ambient", "$[6011] Ambient [1]");//Not found in RS2014
                    break;
            }
        }

        public int GetSongPartition(ArrangementName arrangementName, ArrangementType arrangementType) {
            switch (arrangementType) {
                case Sng.ArrangementType.Bass:
                    songPartitionCount[3]++;
                    return songPartitionCount[3];
                default:
                    switch (arrangementName) {
                        case RocksmithToolkitLib.Sng.ArrangementName.Lead:
                            songPartitionCount[1]++;
                            return songPartitionCount[1];
                        case RocksmithToolkitLib.Sng.ArrangementName.Rhythm:
                            songPartitionCount[2]++;
                            return songPartitionCount[2];
                        default:
                            songPartitionCount[0]++;
                            return songPartitionCount[0];
                    }
            };
        }

        public void GeneratePhraseIterationsData(IAttributes attribute, dynamic song)
        {
            if (song.PhraseIterations == null)
            {
                return;
            }
            for (int i = 0; i < song.PhraseIterations.Length; i++)
            {
                var phraseIteration = song.PhraseIterations[i];
                var phrase = song.Phrases[phraseIteration.PhraseId];
                var endTime = i >= song.PhraseIterations.Length - 1 ? song.SongLength : song.PhraseIterations[i + 1].Time;
                var phraseIt = new PhraseIteration
                {
                    StartTime = phraseIteration.Time,
                    EndTime = endTime,
                    PhraseIndex = phraseIteration.PhraseId,
                    Name = phrase.Name,
                    MaxDifficulty = phrase.MaxDifficulty,
                    MaxScorePerDifficulty = new List<float>()
                };
                attribute.PhraseIterations.Add(phraseIt);
            }
            var noteCnt = 0;
            foreach (var y in attribute.PhraseIterations)
            {
                if (song.Levels[y.MaxDifficulty].Notes != null)
                {
                    noteCnt += GetNoteCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                }
                if (song.Levels[y.MaxDifficulty].Chords != null )
                {
                    noteCnt += GetChordCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Chords);
                }
            }
            attribute.Score_MaxNotes = noteCnt;
            attribute.Score_PNV = ((float)attribute.TargetScore) / noteCnt;

            foreach (var y in attribute.PhraseIterations)
            {
                var phrase = song.Phrases[y.PhraseIndex];
                for (int o = 0; o <= phrase.MaxDifficulty; o++)
                {
                    var multiplier = ((float)(o + 1)) / (phrase.MaxDifficulty + 1);
                    var pnv = attribute.Score_PNV;
                    var noteCount = 0;
                    if (song.Levels[o].Chords != null)
                    {
                        noteCount += GetNoteCount(y.StartTime, y.EndTime, song.Levels[o].Notes);
                    }
                    if (song.Levels[o].Chords != null)
                    {
                        noteCount += GetChordCount(y.StartTime, y.EndTime, song.Levels[o].Chords);
                    }
                    var score = pnv * noteCount * multiplier;
                    y.MaxScorePerDifficulty.Add(score);
                }
            }
        }

        public void GenerateSectionData(IAttributes attribute, dynamic song)
        {
            if (song.Sections == null)
            {
                return;
            }
            for (int i = 0; i < song.Sections.Length; i++)
            {
                var section = song.Sections[i];
                var sect = new Section
                {
                    Name = section.Name,
                    Number = section.Number,
                    StartTime = section.StartTime,
                    EndTime = (i >= song.Sections.Length - 1) ? song.SongLength : song.Sections[i + 1].StartTime,
                    UIName = String.Format("$[6007] {0} [1]", section.Name)

                };
                var sep = sect.Name.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (sep.Length == 1)
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                        sect.UIName = uiName;
                }
                else
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                    {
                        try
                        {
                            if (Convert.ToInt32(sep[1]) != 0 || Convert.ToInt32(sep[1]) != 1)
                                uiName += String.Format("|{0}", sep[1]);
                        }
                        catch { }
                        sect.UIName = uiName;
                    }
                }
                var phraseIterStart = -1;
                var phraseIterEnd = 0;
                var isSolo = false;
                if (song.PhraseIterations != null)
                {
                    for (int o = 0; o < song.PhraseIterations.Length; o++)
                    {
                        var phraseIter = song.PhraseIterations[o];
                        if (phraseIterStart == -1 && phraseIter.Time >= sect.StartTime)
                            phraseIterStart = o;
                        if (phraseIter.Time >= sect.EndTime)
                            break;
                        phraseIterEnd = o;
                        if (song.Phrases[phraseIter.PhraseId].Solo > 0)
                            isSolo = true;
                    }
                }
                sect.StartPhraseIterationIndex = phraseIterStart;
                sect.EndPhraseIterationIndex = phraseIterEnd;
                sect.IsSolo = isSolo;
                attribute.Sections.Add(sect);
            }
        }

        public void GeneratePhraseData(IAttributes attribute, dynamic song)
        {
            if (song.Phrases == null)
                return;

            var ind = 0;
            foreach (var y in song.Phrases)
            {
                attribute.Phrases.Add(new Phrase
                {
                    IterationCount = PhraseIterationCount(song, ind),
                    MaxDifficulty = y.MaxDifficulty,
                    Name = y.Name
                });
                ind++;
            }
        }

        private int PhraseIterationCount(Song2014 song, int ind) {
            return song.PhraseIterations.Count(z => z.PhraseId == ind);
        }

        private int PhraseIterationCount(Song song, int ind)
        {
            return song.PhraseIterations.Count(z => z.PhraseId == ind);
        }

        public void GenerateChordTemplateData(IAttributes attribute, dynamic song)
        {
            var ind = 0;
            if (song.ChordTemplates == null)
                return;

            foreach (var y in song.ChordTemplates)
                attribute.ChordTemplates.Add(new ChordTemplate
                {
                    ChordId = ind++,
                    ChordName = y.ChordName,
                    Fingers = new List<int> { y.Finger0, y.Finger1, y.Finger2, y.Finger3, y.Finger4, y.Finger5 },
                    Frets = new List<int> { y.Fret0, y.Fret1, y.Fret2, y.Fret3, y.Fret4, y.Fret5 }
                });
        }

        public void GenerateDynamicVisualDensity(IAttributes attribute, dynamic song, Arrangement arrangement) {
            if (arrangement.ArrangementType == ArrangementType.Vocal)
            {
                attribute.DynamicVisualDensity = new List<float>{
                        4.5f, 4.3000001907348633f, 4.0999999046325684f, 3.9000000953674316f, 3.7000000476837158f,
                        3.5f, 3.2999999523162842f, 3.0999999046325684f, 2.9000000953674316f, 2.7000000476837158f,
                        2.5f, 2.2999999523162842f, 2.0999999046325684f,
                        2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f};
            }
            else
            {
                attribute.DynamicVisualDensity = new List<float>(20);
                float endSpeed = Math.Min(45f, Math.Max(10f, arrangement.ScrollSpeed)) / 10f;
                if (song.Levels.Length == 1)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        attribute.DynamicVisualDensity.Add(endSpeed);
                    }
                }
                else
                {
                    double beginSpeed = 4.5d;
                    double maxLevel = Math.Min(song.Levels.Length, 16d) - 1;
                    double factor = maxLevel == 0 ? 1d : Math.Pow(endSpeed / beginSpeed, 1d / maxLevel);
                    for (int i = 0; i < 20; i++)
                    {
                        if (i >= maxLevel)
                        {
                            attribute.DynamicVisualDensity.Add(endSpeed);
                        }
                        else
                        {
                            attribute.DynamicVisualDensity.Add((float)(beginSpeed * Math.Pow(factor, i)));
                        }
                    }
                }
            }
        }

        public int GetNoteCount(float startTime, float endTime, ICollection<SongNote> notes)
        {
            int count = 0;
            for (int i = 0; i < notes.Count(); i++)
            {
                if(notes.ElementAt(i).Time < endTime)
                if (notes.ElementAt(i).Time >= startTime)
                    count++;
            }
            return count;
        }

        public int GetChordCount(float startTime, float endTime, ICollection<SongChord> chords)
        {
            int count = 0;
            for (int i = 0; i < chords.Count(); i++)
            {
                if (chords.ElementAt(i).Time < endTime)
                    if (chords.ElementAt(i).Time >= startTime)
                        count++;
            }
            return count;
        }
    }
}
