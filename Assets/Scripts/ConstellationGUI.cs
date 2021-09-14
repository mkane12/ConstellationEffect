using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using UnityEngine;

namespace TeamLab.Unity
{
    public class ConstellationGUI : TeamLab.Unity.MenuBase
    {
        public Sky sky;
        public GPUConstellation con;
        //public Star star;
        static public Data data = new Data();

        // number of stars in constellation
        public int sliderNumVertexStars; // on vertices
        public int sliderNumEdgeStars; // on edges

        // time over which stars move
        public float sliderTimeToMove;

        // size of stars
        public float sliderStarSize;

        // twinkle speed of stars
        public int sliderTwinkleSpeed;

        // lifespan of stars
        // for now, let's keep these the same for edge and vertex stars
        public float sliderLifespan;

        // time over which stars fade
        // for now, let's keep these the same for edge and vertex stars
        public float sliderTimeToFade;

        // distance between vertex stars
        public float sliderVertexStarMinDistance;

        // Utilizes SharedVariableColor helper class in TeamLabUnityFrameworks
        //public ShaderVariableColor starColorUpdate = new ShaderVariableColor("_Color"); // star color

        // color of stars
        public Color starColor;

        public GUIUtil.ColorField starColorGUI = new GUIUtil.ColorField();

        // And the rest of these are for constellation things
        // toggle for each constellation
        // TODO: some issue where toggles is only getting 3 values...?
        public bool[] constellationToggles;

        // slider for number of constellations to spawn
        public int sliderConstellationCount;

        // constellation mesh visibility
        public float sliderMeshAlpha;

        // constellation mesh complexity
        public float sliderMeshQuality;

        // toggle for constellation visual effect
        /*public GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>
            toggleConstellationMode
            = new GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>();*/

        // slider for percentage of stars to edge vs. vertex
        //public float sliderPercentEdge;

        protected override void Start()
        {
            base.Start();

            constellationToggles = new bool[Enum.GetNames(typeof(Sky.ConstellationType)).Length];

            sky = GameObject.FindObjectOfType<Sky>();

            sliderNumVertexStars = data.numVertexStars;
            sliderNumEdgeStars = data.numEdgeStars;

            sliderStarSize = data.size;

            sliderTwinkleSpeed = data.twinkleSpeed;

            sliderLifespan = data.lifespan;
            sliderTimeToMove = data.timeToMove;
            sliderTimeToFade = data.timeToFade;

            sliderVertexStarMinDistance = data.vertexStarMinDistance;

            // set all toggles to true to start
            for(int i = 0; i < constellationToggles.Length; i++)
            {
                constellationToggles[i] = true;
            }

            sliderMeshAlpha = data.meshAlpha;

            sliderConstellationCount = data.constellationCount;
            sliderMeshQuality = data.quality;

            // starting starColor
            if (ColorUtility.TryParseHtmlString(data.color, out Color newCol))
            {
                starColor = newCol;
            }

            con.instanceMaterial.SetVector("_Color", starColor);

            base.showButtons.save = true;
            base.showButtons.load = true;
            base.autoSaveOnClose = false;

            base.Start();
        }

        // called once per frame while GUI is open
        public override void OnGUIInternal() // <-- Please write all GUI code here
        {
            // slider to determine number of stars in constellation
            GUILayout.BeginHorizontal();
            // vertex stars
            GUILayout.Label("Number of stars on vertices:");
            sliderNumVertexStars = GUIUtil.Slider(sliderNumVertexStars, 50, 500);
            data.numVertexStars = sliderNumVertexStars;
            // edge stars
            GUILayout.Label("Number of stars on edges:");
            sliderNumEdgeStars = GUIUtil.Slider(sliderNumEdgeStars, 50, 500);
            data.numEdgeStars = sliderNumEdgeStars;
            GUILayout.EndHorizontal();

            // sliders to determine stars' size
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star size:");
            sliderStarSize = GUIUtil.Slider(sliderStarSize, 0.1f, 10.0f);
            data.size = sliderStarSize;
            GUILayout.EndHorizontal();

            // slider to determine rate at which stars twinkle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star twinkle speed:");
            sliderTwinkleSpeed = GUIUtil.Slider(sliderTwinkleSpeed, 1, 20);
            data.twinkleSpeed = sliderTwinkleSpeed;
            GUILayout.EndHorizontal();

            // slider to determine star lifespan
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star lifepan:");
            sliderLifespan = GUIUtil.Slider(sliderLifespan, 5, 500);
            data.lifespan = sliderLifespan;
            GUILayout.EndHorizontal();

            // slider to determine time over which stars move
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star move time:");
            sliderTimeToMove = GUIUtil.Slider(sliderTimeToMove, 1, 10);
            data.timeToMove = sliderTimeToMove;
            GUILayout.EndHorizontal();

            // slider to determine time over which stars fade
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star fade time:");
            sliderTimeToFade = GUIUtil.Slider(sliderTimeToFade, 1, 500);
            data.timeToFade = sliderTimeToFade;
            GUILayout.EndHorizontal();

            // slider to determine minimum distance between stars on vertices
            GUILayout.BeginHorizontal();
            GUILayout.Label("Min distance btw vertex stars:");
            sliderVertexStarMinDistance = GUIUtil.Slider(sliderVertexStarMinDistance, 0.0f, 1.0f);
            data.vertexStarMinDistance = sliderVertexStarMinDistance;
            GUILayout.EndHorizontal();

            // toggles to decide constellation patterns chosen from
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellations to choose from:");

            for(int i = 0; i < constellationToggles.Length; i++)
            {
                constellationToggles[i] = GUILayout.Toggle(constellationToggles[i], ((Sky.ConstellationType)i).ToString());
                sky.UpdateConstellationList(constellationToggles[i], (Sky.ConstellationType)i);
            }

            GUILayout.EndHorizontal();

            // Slider to determine the number of constellations to spawn
            GUILayout.BeginHorizontal();
            GUILayout.Label("Number of constellations:");
            sliderConstellationCount = GUIUtil.Slider(sliderConstellationCount, 1, 10);
            data.constellationCount = sliderConstellationCount;
            GUILayout.EndHorizontal();

            // Slider to determine transparency of constellation mesh
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellation mesh alpha:");
            float oldAlpha = sliderMeshAlpha; // store old alpha before slider checked
            sliderMeshAlpha = GUIUtil.Slider(sliderMeshAlpha, 0, 1);
            data.meshAlpha = sliderMeshAlpha;
            sky.UpdateConstellationAlpha(sliderMeshAlpha, oldAlpha);
            GUILayout.EndHorizontal();

            // Slider to determine complexity of constellation mesh
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellation mesh complexity:");
            sliderMeshQuality = GUIUtil.Slider(sliderMeshQuality, 0, 1);
            data.quality = (float) System.Math.Round(sliderMeshQuality, 1);
            GUILayout.EndHorizontal();

            // Color Field to determine colors of  stars
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star color:");
            starColorGUI.OnGUI(ref starColor);

            con.instanceMaterial.SetVector("_Color", starColor);
            data.color = "#" + ColorUtility.ToHtmlStringRGBA(starColor);

            GUILayout.EndHorizontal();
        }

        public override void Save()
        {
            var path = TeamLab.Unity.PackageAndSceneSpecificPath.Static.GetSaveLoadPathWithFileDefault("GUISettings.txt");
            using (var writer = new StreamWriter(path))
            {
                string json = UnityEngine.JsonUtility.ToJson(data, true);
                writer.Write(json);
            }
        }

        public override void Load()
        {
            var path = TeamLab.Unity.PackageAndSceneSpecificPath.Static.GetSaveLoadPathWithFileDefault("GUISettings.txt");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                UnityEngine.JsonUtility.FromJsonOverwrite(json, data);
            }

            UpdateGUIData();
        }

        // update sliders to reflect loaded data
        public void UpdateGUIData()
        {
            sliderNumVertexStars = data.numVertexStars;
            sliderNumEdgeStars = data.numEdgeStars;

            sliderStarSize = data.size;

            sliderTwinkleSpeed = data.twinkleSpeed;

            sliderLifespan = data.lifespan;
            sliderTimeToMove = data.timeToMove;
            sliderTimeToFade = data.timeToFade;

            sliderVertexStarMinDistance = data.vertexStarMinDistance;

            // first, reset all toggles to false
            for (int i = 0; i < constellationToggles.Length; i++)
            {
                constellationToggles[i] = false;
            }

            // then toggle on all toggles saved in constellationNames list
            foreach(var type in data.constellationNames)
            {
                constellationToggles[(int)type] = true;
            }

            sliderConstellationCount = data.constellationCount;

            sliderMeshAlpha = data.meshAlpha;

            sliderMeshQuality = data.quality;

            if (ColorUtility.TryParseHtmlString(data.color, out Color newCol))
            {
                starColor = newCol;
            }
        }
    } // end class
} // end namespace