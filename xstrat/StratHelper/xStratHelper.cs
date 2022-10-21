using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using xstrat.MVVM.View;
using XStrat;
using xstrat.Core;
using System.IO;

namespace xstrat.StratHelper
{
    public static class xStratHelper
    {
        public static StratMakerView stratView { get; set; }
        public static WallEditorView editorView { get; set; }
        public static bool WEMode = false;

        public static List<WallPositionObject> GetWallObjects(int map_id, int floor_id)
        {
            var list = new List<WallPositionObject>();
            string file_name = map_id + "_" + floor_id + ".xml";
            string path = SettingsHandler.MapsFolder + "/" + file_name;

            if (File.Exists(path))
            { 
                var serializer = new XmlSerializer(typeof(List<WallPositionObject>));
                using (var reader = XmlReader.Create(path))
                {
                    list = (List<WallPositionObject>)serializer.Deserialize(reader);
                }
            }
            return list;
        }

        public static void SaveWallObjects(List<WallPositionObject> list, int map_id, int floor_id)
        {
            string file_name = map_id + "_" + floor_id + ".xml";
            string path = SettingsHandler.MapsFolder + "/" + file_name;

            var serializer = new XmlSerializer(list.GetType());
            using (var writer = XmlWriter.Create(path))
            {
                serializer.Serialize(writer, list);
            }
        }

    }
}
