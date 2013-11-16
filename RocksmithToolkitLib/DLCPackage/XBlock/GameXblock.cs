﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    [XmlRoot("game", Namespace = "", IsNullable = false)]
    public class GameXblock<T> where T : IEntity
    {
        public static readonly string URN_TEMPLATE = "urn:{0}:{1}:{2}";

        [XmlArray("entitySet")]
        [XmlArrayItem("entity")]
        public List<T> EntitySet { get; set; }

        public static GameXblock<T> LoadFromFile(string xblockFile) {
            GameXblock<T> xblock = null;

            using (var reader = new StreamReader(xblockFile)) {
                var serializer = new XmlSerializer(typeof(GameXblock<T>));
                xblock = (GameXblock<T>)serializer.Deserialize(reader);
            }

            return xblock;
        }

        public GameXblock() {
            EntitySet = new List<T>();
        }

        #region RS2014

        public static GameXblock<Entity2014> Generate2014(DLCPackageData info) {
            GameXblock<Entity2014> game = new GameXblock<Entity2014>();
            game.EntitySet = new List<Entity2014>();

            var dlcName = info.Name.ToLower();
            
            foreach (var arrangement in info.Arrangements) {
                var entity = new Entity2014();
                var arrangementName = arrangement.Name.ToString().ToLower();

                entity.Id = IdGenerator.IdString().ToLower();
                entity.ModelName = "RSEnumerable_Song";
                entity.Name = String.Format("{0}_{1}", info.Name, arrangement.Name);
                entity.Iterations = 0;

                entity.Properties = new List<Property2014>();
                entity.Properties.Add(new Property2014() { Name = "Header", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Database.GetDescription(), TagValue.HsanDB.GetDescription(), String.Format(AggregateGraph2014.NAME_HSAN, dlcName)) } });
                entity.Properties.Add(new Property2014() { Name = "Manifest", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Database.GetDescription(), TagValue.JsonDB.GetDescription(), String.Format(AggregateGraph2014.NAME_DEFAULT, dlcName, arrangementName)) } });
                entity.Properties.Add(new Property2014() { Name = "SngAsset", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.MusicgameSong.GetDescription(), String.Format(AggregateGraph2014.NAME_DEFAULT, dlcName, arrangementName)) } });
                entity.Properties.Add(new Property2014() { Name = "AlbumArtSmall", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), String.Format("album_{0}_64", dlcName)) } });
                entity.Properties.Add(new Property2014() { Name = "AlbumArtMedium", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), String.Format("album_{0}_128", dlcName)) } });
                entity.Properties.Add(new Property2014() { Name = "AlbumArtLarge", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), String.Format("album_{0}_256", dlcName)) } });
                entity.Properties.Add(new Property2014() { Name = "LyricArt", Set = new Set() { Value = "" } });
                entity.Properties.Add(new Property2014() { Name = "ShowLightsXMLAsset", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.XML.GetDescription(), String.Format(AggregateGraph2014.NAME_SHOWLIGHT, dlcName)) } });
                entity.Properties.Add(new Property2014() { Name = "SoundBank", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Audio.GetDescription(), TagValue.WwiseSoundBank.GetDescription(), String.Format(AggregateGraph2014.NAME_SOUNDBANK, dlcName)) } });
                entity.Properties.Add(new Property2014() { Name = "PreviewSoundBank", Set = new Set() { Value = String.Format(URN_TEMPLATE, TagValue.Audio.GetDescription(), TagValue.WwiseSoundBank.GetDescription(), String.Format(AggregateGraph2014.NAME_SOUNDBANKPREVIEW, dlcName)) } });

                game.EntitySet.Add(entity);
            }

            return game;
        }

        public void SerializeXml(Stream outStream) {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(GameXblock<T>));
            serializer.Serialize(outStream, this, ns);
        }

        #endregion

        #region RS1

        public void Serialize(Stream outStream) {
            var document = new XDocument();
            var el = new XElement("entitySet");
            foreach (var x in EntitySet)
                x.Serialize(el);
            document.Add(new XElement("game", el));
            document.Save(outStream);
        }

        public static void Generate(string dlcName, Manifest.Manifest manifest, AggregateGraph.AggregateGraph aggregateGraph, Stream outStream) {
            var game = new GameXblock<Entity>();
            game.EntitySet = new List<Entity>();
            var ent = new Entity() { Id = IdGenerator.Guid().ToString().Replace("-", ""), Name = "SoundScene0", Iterations = 1, ModelName = "SoundScene", Properties = new List<Property>() { CreateMultiItemProperty("SoundBanks", new string[1] { aggregateGraph.SoundBank.Name + ".bnk" }) } };
            game.EntitySet.Add(ent);
            foreach (var x in manifest.Entries) {
                var entry = x.Value["Attributes"];
                var entity = new Entity();
                bool isVocal = entry.ArrangementName == "Vocals";
                bool isBass = entry.ArrangementName == "Bass";

                entity.Id = entry.PersistentID.ToLower();
                entity.Name = String.Format("GRSong_Asset_{0}_{1}", dlcName, entry.ArrangementName);
                entity.ModelName = "GRSong_Asset";
                entity.Iterations = 46;
                game.EntitySet.Add(entity);
                var properties = new List<Property>();
                var addProperty = new Action<string, object>((a, b) => properties.Add(CreateProperty(a, b.ToString())));

                if (isBass || isVocal)
                    addProperty("BinaryVersion", entry.BinaryVersion);

                addProperty("SongKey", entry.SongKey);
                addProperty("SongAsset", entry.SongAsset);
                addProperty("SongXml", entry.SongXml);
                addProperty("ForceUseXML", entry.ForceUseXML);
                addProperty("Shipping", entry.Shipping);
                addProperty("DisplayName", entry.DisplayName);

                addProperty("SongEvent", entry.SongEvent);

                if (isVocal)
                    addProperty("InputEvent", entry.InputEvent);

                addProperty("ArrangementName", entry.ArrangementName);
                addProperty("RepresentativeArrangement", entry.RepresentativeArrangement);

                if (!isVocal && !String.IsNullOrEmpty(entry.VocalsAssetId)) {
                    addProperty("VocalsAssetId", entry.VocalsAssetId.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                    var dynVisDen = new List<object>();
                    foreach (var y in entry.DynamicVisualDensity)
                        dynVisDen.Add(y);
                    properties.Add(CreateMultiItemProperty("DynamicVisualDensity", dynVisDen));
                }

                addProperty("ArtistName", entry.ArtistName);
                addProperty("ArtistNameSort", entry.ArtistNameSort);
                addProperty("SongName", entry.SongName);
                addProperty("SongNameSort", entry.SongNameSort);
                addProperty("AlbumName", entry.AlbumName);
                addProperty("AlbumNameSort", entry.AlbumNameSort);
                addProperty("SongYear", entry.SongYear);

                if (!isVocal) {
                    addProperty("RelativeDifficulty", entry.RelativeDifficulty);
                    addProperty("AverageTempo", entry.AverageTempo);//fix this

                    addProperty("NonStandardChords", true);//fix this
                    addProperty("DoubleStops", entry.DoubleStops);
                    addProperty("PowerChords", entry.PowerChords);
                    addProperty("OpenChords", entry.OpenChords);
                    addProperty("BarChords", entry.BarChords);
                    addProperty("Sustain", entry.Sustain);
                    addProperty("Bends", entry.Bends);
                    addProperty("Slides", entry.Slides);
                    addProperty("HOPOs", entry.HOPOs);
                    addProperty("PalmMutes", entry.PalmMutes);
                    addProperty("Vibrato", entry.Vibrato);

                    addProperty("MasterID_Xbox360", entry.MasterID_Xbox360);
                    addProperty("EffectChainName", entry.EffectChainName);
                    addProperty("CrowdTempo", "Fast");//fix this
                }

                addProperty("AlbumArt", entry.AlbumArt);

                entity.Properties = properties;
            }
            game.Serialize(outStream);
        }

        private static Property CreateProperty(string name, string value) {
            return new Property() { Name = name, Set = new Set() { Value = value } };
        }

        private static Property CreateMultiItemProperty(string name, IEnumerable<object> values) {
            var prop = new Property() { Name = name };
            var multiItemSet = new MultiItemSet();
            multiItemSet.Values = new List<string>();
            foreach (var x in values)
                multiItemSet.Values.Add(x.ToString());
            prop.Set = multiItemSet;
            return prop;
        }

        #endregion
    }
}
