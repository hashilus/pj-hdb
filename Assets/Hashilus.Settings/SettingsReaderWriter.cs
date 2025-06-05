using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Hashilus.Setting
{
    public static class SettingsReaderWriter
    {
        static void CreateDirectory(string pathWithoutExtension)
        {
            var dirPath = Path.GetDirectoryName(pathWithoutExtension);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        }

        public static void Load<StaticSettingT>(string path, bool allowNoFile = false)
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            var group = GetFieldHierarchy<StaticSettingT>();
            WalkingLoad(group, pathWithoutExtension, allowNoFile);
        }

        static void WalkingLoad(SettingFieldGroup group, string pathWithoutExtension, bool allowNoFile)
        {
            var suffix = string.IsNullOrEmpty(group.name) ? "" : "." + group.name;
            pathWithoutExtension += suffix;

            if (group.fields.Length > 0)
            {
                if (File.Exists(pathWithoutExtension + ".xml"))
                {
                    try
                    {
                        using (var streamReader = new StreamReader(pathWithoutExtension + ".xml", Encoding.UTF8))
                        using (var xmlReader = XmlReader.Create(streamReader))
                        {
                            while (xmlReader.Read())
                            {
                                foreach (var field in group.fields)
                                {
                                    if (field.Key != xmlReader.LocalName) continue;
                                    field.SetByString(xmlReader.ReadString());
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError("設定ファイル" + pathWithoutExtension + ".xml の読み込みに失敗しました\n" + e);
                    }
                }
                else if (!allowNoFile)
                {
                    UnityEngine.Debug.LogError("設定ファイル" + pathWithoutExtension + ".xml が存在しません");
                }
            }

            foreach (var childGroup in group.childGroups)
            {
                WalkingLoad(childGroup, pathWithoutExtension, allowNoFile);
            }
        }

        public static void Save<StaticSettingT>(string path, bool saveModifiedOnly = false)
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

            CreateDirectory(path);

            var group = GetFieldHierarchy<StaticSettingT>();
            var writerSettings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            WalkingSave(group, pathWithoutExtension, writerSettings, saveModifiedOnly);
        }

        static void WalkingSave(SettingFieldGroup group, string pathWithoutExtension, XmlWriterSettings writerSettings, bool saveModifiedOnly)
        {
            var suffix = string.IsNullOrEmpty(group.name) ? "" : "." + group.name;
            pathWithoutExtension += suffix;

            var fieldsToSave = group.fields.Where(f => !saveModifiedOnly || !f.EqualsDefault());

            if (fieldsToSave.Any())
            {
                using (var streamWriter = new StreamWriter(pathWithoutExtension + ".xml", false))
                using (var xmlWriter = XmlWriter.Create(streamWriter, writerSettings))
                {
                    xmlWriter.WriteStartElement("Settings");

                    foreach (var field in fieldsToSave)
                    {
                        xmlWriter.WriteComment(field.Description);
                        xmlWriter.WriteStartElement(field.Key);
                        xmlWriter.WriteValue(field.ToString());
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }
            }
            else if (File.Exists(pathWithoutExtension + ".xml"))
            {
                File.Delete(pathWithoutExtension + ".xml");
            }

            foreach (var childGroup in group.childGroups)
            {
                WalkingSave(childGroup, pathWithoutExtension, writerSettings, saveModifiedOnly);
            }
        }

        public static void Remove<StaticSettingT>(string path)
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

            var group = GetFieldHierarchy<StaticSettingT>();
            WalkingRemove(group, pathWithoutExtension);
        }

        static void WalkingRemove(SettingFieldGroup group, string pathWithoutExtension)
        {
            var suffix = string.IsNullOrEmpty(group.name) ? "" : "." + group.name;
            pathWithoutExtension += suffix;

            if (File.Exists(pathWithoutExtension + ".xml"))
            {
                File.Delete(pathWithoutExtension + ".xml");
            }

            foreach (var childGroup in group.childGroups)
            {
                WalkingRemove(childGroup, pathWithoutExtension);
            }
        }

        static SettingFieldGroup cachedGroup;
        public static SettingFieldGroup GetFieldHierarchy<StaticSettingT>()
        {
            if (cachedGroup != null) return cachedGroup;
            cachedGroup = WalkFieldHierarchy(typeof(StaticSettingT), null);
            return cachedGroup;
        }

        static SettingFieldGroup WalkFieldHierarchy(Type type, string name)
        {
            var childGroups = type
                 .GetNestedTypes()
                 .Select(nestedType => WalkFieldHierarchy(nestedType, nestedType.Name))
                 .ToArray();

            var fields = type
                      .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField)
                      .Where(f => typeof(ISettingField).IsAssignableFrom(f.FieldType) && !f.FieldType.IsInterface && !f.FieldType.IsAbstract)
                      .Select(f =>
                      {
                          var field = (ISettingField)f.GetValue(null);
                          field.Key = f.Name;
                          return field;
                      })
                      .ToArray();

            return new SettingFieldGroup(name, childGroups, fields);
        }

        public class SettingFieldGroup
        {
            public readonly string name;
            public readonly SettingFieldGroup[] childGroups;
            public readonly ISettingField[] fields;

            public SettingFieldGroup(string name, SettingFieldGroup[] childGroups, ISettingField[] fields)
            {
                this.name = name;
                this.childGroups = childGroups;
                this.fields = fields;
            }
        }
    }
}
