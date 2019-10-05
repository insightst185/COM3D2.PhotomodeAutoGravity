using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace COM3D2.PhotomodeAutoGravity.Plugin
{
    [PluginFilter("COM3D2x64"),
     PluginName("COM3D2.PhotomodeAutoGravity.Plugin"), PluginVersion("0.0.0.1")]

    public class PhotomodeAutoGravity : PluginBase
    {
        private XmlManager xmlManager = null;
        private GravityControlWindow gcw;
        private int iSceneLevel;
        private Vector3 nowG;
        private Vector3 addG;
        private bool f=false;
        
        private FieldInfo fieldActiveMaidControllerList = (typeof(GravityControlWindow)).GetField("activeMaidControllerList", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);


        public void OnLevelWasLoaded(int level)
        {
            iSceneLevel = level;

            if(iSceneLevel != 26) {xmlManager = null;return;}

            Initialization();
        }

        //初期化処理
        private void Initialization()
        {
            xmlManager = new XmlManager();
            gcw = FindObjectOfType<GravityControlWindow>();
            nowG = new Vector3(0.0f, 0.0f, 0.0f);
            addG = new Vector3(xmlManager.xAdd, xmlManager.yAdd, xmlManager.zAdd);
            f=false;
        }

        public void Start()
        {
        }

        public void Update()
        {
            if(xmlManager == null) return;
            if(gcw == null) return;
            
            if(Input.GetKeyDown(KeyCode.G)) f = !f;
            if(!f) return;

            Dictionary<string, Dictionary<string, GravityTransformControl>> activeMaidControllerList =
                (Dictionary<string, Dictionary<string, GravityTransformControl>>)fieldActiveMaidControllerList.GetValue(gcw);

            if(activeMaidControllerList == null) return;

            if((addG.x >= 0.0f && nowG.x > xmlManager.xMax) ||
               (addG.x <  0.0f && nowG.x < xmlManager.xMin))
            {
                addG.x = addG.x * -1.0f;
            }
            nowG.x += addG.x;
            if((addG.y >= 0.0f && nowG.y > xmlManager.yMax) ||
               (addG.y <  0.0f && nowG.y < xmlManager.yMin))
            {
                addG.y = addG.y * -1.0f;
            }
            nowG.y += addG.y;
            if((addG.z >= 0.0f && nowG.z > xmlManager.zMax) ||
               (addG.z <  0.0f && nowG.z < xmlManager.zMin))
            {
                addG.z = addG.z * -1.0f;
            }
            nowG.z += addG.z;

            foreach(KeyValuePair<string, Dictionary<string, GravityTransformControl>> kvp1 in activeMaidControllerList){
                foreach(KeyValuePair<string, GravityTransformControl> kvp2 in kvp1.Value){
                    kvp2.Value.transform.localPosition = new Vector3(
                                                              kvp2.Value.transform.localPosition.x + addG.x
                                                          ,   kvp2.Value.transform.localPosition.y + addG.y
                                                          ,   kvp2.Value.transform.localPosition.z + addG.z
                                                      );
                }
            }

        }

        //------------------------------------------------------xml--------------------------------------------------------------------
        private class XmlManager
        {
            private string xmlFileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Config\PhotomodeAutoGravity.xml";
            private XmlDocument xmldoc = new XmlDocument();
            public float xMax = 0;
            public float xMin = 0;
            public float xAdd = 0;
            public float yMax = 0;
            public float yMin = 0;
            public float yAdd = 0;
            public float zMax = 0;
            public float zMin = 0;
            public float zAdd = 0;
            
            public XmlManager()
            {
                try{
                    InitXml();
                }
                catch(Exception e){
                    Debug.LogError("ComboCount.Plugin:" + e.Source + e.Message + e.StackTrace);
                }
            }

            private void InitXml()
            {
                xmldoc.Load(xmlFileName);
                XmlNodeList xList = xmldoc.GetElementsByTagName("X");
                foreach (XmlNode x in xList)
                {
                    xMax= float.Parse(((XmlElement)x).GetAttribute("Max"));
                    xMin= float.Parse(((XmlElement)x).GetAttribute("Min"));
                    xAdd= float.Parse(((XmlElement)x).GetAttribute("Add"));
                }
                XmlNodeList yList = xmldoc.GetElementsByTagName("Y");
                foreach (XmlNode y in yList)
                {
                    yMax= float.Parse(((XmlElement)y).GetAttribute("Max"));
                    yMin= float.Parse(((XmlElement)y).GetAttribute("Min"));
                    yAdd= float.Parse(((XmlElement)y).GetAttribute("Add"));
                }
                XmlNodeList zList = xmldoc.GetElementsByTagName("Z");
                foreach (XmlNode z in zList)
                {
                    zMax= float.Parse(((XmlElement)z).GetAttribute("Max"));
                    zMin= float.Parse(((XmlElement)z).GetAttribute("Min"));
                    zAdd= float.Parse(((XmlElement)z).GetAttribute("Add"));
                }
            }

        }
    }
}
