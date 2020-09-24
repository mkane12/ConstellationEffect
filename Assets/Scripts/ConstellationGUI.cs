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
        public Star star;
        public EdgeStar edgeStar;
        static public Data data = new Data();

        // number of stars in constellation
        public int sliderNumStars; // on vertices
        public int sliderNumEdgeStars; // on edges

        // velocity range of stars
        public float sliderStarVelocityMin; // vertex stars
        public float sliderStarVelocityMax; // vertex stars
        public float sliderEdgeStarVelocityMin; //edge stars
        public float sliderEdgeStarVelocityMax; // edge stars

        // size range of stars
        public float sliderStarSizeMin; // vertex stars
        public float sliderStarSizeMax; // vertex stars
        public float sliderEdgeStarSizeMin; // edge stars
        public float sliderEdgeStarSizeMax; // edge stars

        // twinkle speed of stars
        public int sliderTwinkleSpeed; // vertex stars
        public int sliderEdgeTwinkleSpeed; // edge stars

        // lifespan of stars
        // for now, let's keep these the same for edge and vertex stars
        public float sliderLifespan;

        // time over which stars fade
        // for now, let's keep these the same for edge and vertex stars
        public float sliderTimeToFade;

        // Utilizes SharedVariableColor helper class in TeamLabUnityFrameworks
        public ShaderVariableColor starColorUpdate = new ShaderVariableColor("_Color"); // star color
        public ShaderVariableColor edgeStarColorUpdate = new ShaderVariableColor("_Color"); // star color

        // color of vertex stars
        public Color starColor;
        // color of edge stars
        public Color edgeStarColor;

        public GUIUtil.ColorField starColorGUI = new GUIUtil.ColorField();
        public GUIUtil.ColorField edgeStarColorGUI = new GUIUtil.ColorField();

        private Renderer starRenderer;
        private Renderer edgeStarRenderer;


        // And the rest of these are for constellation things
        // toggle for each constellation
        public bool[] constellationToggles = new bool[Enum.GetNames(typeof(Sky.ConstellationType)).Length];

        // slider for number of constellations to spawn
        public int sliderConstellationCount;

        // constellation mesh visibility
        public float sliderMeshAlpha;

        // constellation mesh complexity
        public float sliderMeshQuality;

        // toggle for constellation visual effect
        public GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>
            toggleConstellationMode
            = new GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>();

        // slider for percentage of stars to edge vs. vertex
        //public float sliderPercentEdge;

        protected override void Start()
        {
            base.Start();

            sky = GameObject.FindObjectOfType<Sky>();

            sliderNumStars = data.numStars;
            sliderNumEdgeStars = data.numEdgeStars;

            sliderStarVelocityMin = data.minVelocity;
            sliderStarVelocityMax = data.maxVelocity;
            sliderEdgeStarVelocityMin = data.minVelocityEdge;
            sliderEdgeStarVelocityMax = data.maxVelocityEdge;

            sliderStarSizeMin = data.minSize;
            sliderStarSizeMax = data.maxSize;
            sliderEdgeStarSizeMin = data.minSizeEdge;
            sliderEdgeStarSizeMax = data.maxSizeEdge;

            sliderTwinkleSpeed = data.twinkleSpeed;
            sliderEdgeTwinkleSpeed = data.twinkleSpeedEdge;

            sliderLifespan = data.lifespan;

            sliderTimeToFade = data.timeToFade;

            // set all toggles to true to start
            for(int i = 0; i < constellationToggles.Length; i++)
            {
                constellationToggles[i] = true;
            }

            sliderMeshAlpha = data.meshAlpha;

            sliderConstellationCount = data.constellationCount;
            sliderMeshQuality = data.quality;

            //sliderPercentEdge = data.percentEdge;

            starRenderer = star.GetComponent<Renderer>();
            edgeStarRenderer = edgeStar.GetComponent<Renderer>();

            // starting starColor
            Color newCol;
            if (ColorUtility.TryParseHtmlString(data.starColor, out newCol))
            {
                starColor = newCol;
            }

            Color newEdgeCol;
            if (ColorUtility.TryParseHtmlString(data.edgeStarColor, out newEdgeCol))
            {
                edgeStarColor = newEdgeCol;
            }

            starColorUpdate.SetValueCPUOnly(starColor);
            starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);

            edgeStarColorUpdate.SetValueCPUOnly(edgeStarColor);
            edgeStarColorUpdate.SetToMaterial(edgeStarRenderer.sharedMaterial);

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
            sliderNumStars = GUIUtil.Slider(sliderNumStars, 50, 500);
            data.numStars = sliderNumStars;
            // edge stars
            GUILayout.Label("Number of stars on edges:");
            sliderNumEdgeStars = GUIUtil.Slider(sliderNumEdgeStars, 50, 500);
            data.numEdgeStars = sliderNumEdgeStars;
            GUILayout.EndHorizontal();

            // sliders to determine range of vertex stars' velocity
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of vertex star velocities:");
            sliderStarVelocityMin = GUIUtil.Slider(sliderStarVelocityMin, 1, 50);
            data.minVelocity = sliderStarVelocityMin;
            sliderStarVelocityMax = GUIUtil.Slider(sliderStarVelocityMax, 1, 50);
            data.maxVelocity = sliderStarVelocityMax;
            GUILayout.EndHorizontal();

            // sliders to determine range of edge stars' velocity
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of edge star velocities:");
            sliderEdgeStarVelocityMin = GUIUtil.Slider(sliderEdgeStarVelocityMin, 1, 50);
            data.minVelocityEdge = sliderEdgeStarVelocityMin;
            sliderEdgeStarVelocityMax = GUIUtil.Slider(sliderEdgeStarVelocityMax, 1, 50);
            data.maxVelocity = sliderEdgeStarVelocityMax;
            GUILayout.EndHorizontal();

            // sliders to determine range of stars' size
            // vertex stars
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of vertex star sizes:");
            sliderStarSizeMin = GUIUtil.Slider(sliderStarSizeMin, 0.1f, 10.0f);
            data.minSize = sliderStarSizeMin;
            sliderStarSizeMax = GUIUtil.Slider(sliderStarSizeMax, 0.1f, 10.0f);
            data.maxSize = sliderStarSizeMax;
            GUILayout.EndHorizontal();
            // edge stars
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of edge star sizes:");
            sliderEdgeStarSizeMin = GUIUtil.Slider(sliderEdgeStarSizeMin, 0.1f, 10.0f);
            data.minSizeEdge = sliderEdgeStarSizeMin;
            sliderEdgeStarSizeMax = GUIUtil.Slider(sliderEdgeStarSizeMax, 0.1f, 10.0f);
            data.maxSizeEdge = sliderEdgeStarSizeMax;
            GUILayout.EndHorizontal();

            // slider to determine rate at which stars twinkle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertex star twinkle speed:");
            sliderTwinkleSpeed = GUIUtil.Slider(sliderTwinkleSpeed, 1, 20);
            data.twinkleSpeed = sliderTwinkleSpeed;
            GUILayout.Label("Edge star twinkle speed:");
            sliderEdgeTwinkleSpeed = GUIUtil.Slider(sliderEdgeTwinkleSpeed, 1, 20);
            data.twinkleSpeedEdge = sliderEdgeTwinkleSpeed;
            GUILayout.EndHorizontal();

            // slider to determine star lifespan
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star lifepan:");
            sliderLifespan = GUIUtil.Slider(sliderLifespan, 5, 500);
            data.lifespan = sliderLifespan;
            GUILayout.EndHorizontal();

            // slider to determine time over which stars fade
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star fade time:");
            sliderTimeToFade = GUIUtil.Slider(sliderTimeToFade, 1, 500);
            data.timeToFade = sliderTimeToFade;
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
            data.quality = sliderMeshQuality;
            GUILayout.EndHorizontal();

            // slider to determine proportion of stars on edge vs vertex
            /*GUILayout.BeginHorizontal();
            GUILayout.Label("Percentage of stars on edge:");
            sliderPercentEdge = GUIUtil.Slider(sliderPercentEdge, 0, 1);
            data.percentEdge = sliderPercentEdge;
            GUILayout.EndHorizontal();*/

            // toggles to decide constellation mode
            /*GUILayout.BeginHorizontal();
            GUILayout.Label("Constellation Mode");
            // public E OnGUI(E enumInitialValue, int xWidth)
            // will probably need to update xWidth as more modes are added
            data.mode = toggleConstellationMode.OnGUI(data.mode, 3);
            GUILayout.EndHorizontal();*/

            // Color Field to determine colors of vertex stars
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertex star color:");
            starColorGUI.OnGUI(ref starColor);

            // Use ShaderVariableGeneric helper class to only update shader when changed
            starColorUpdate.SetValueCPUOnly(starColor);
            starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);
            data.starColor = "#" + ColorUtility.ToHtmlStringRGBA(starColor);

            GUILayout.EndHorizontal();

            // Color Field to determine colors of edge stars
            GUILayout.BeginHorizontal();
            GUILayout.Label("Edge star color:");
            edgeStarColorGUI.OnGUI(ref edgeStarColor);

            // Use ShaderVariableGeneric helper class to only update shader when changed
            edgeStarColorUpdate.SetValueCPUOnly(edgeStarColor);
            edgeStarColorUpdate.SetToMaterial(edgeStarRenderer.sharedMaterial);
            data.edgeStarColor = "#" + ColorUtility.ToHtmlStringRGBA(edgeStarColor);

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
            sliderNumStars = data.numStars;
            sliderNumEdgeStars = data.numEdgeStars;

            sliderStarVelocityMin = data.minVelocity;
            sliderStarVelocityMax = data.maxVelocity;
            sliderEdgeStarVelocityMin = data.minVelocityEdge;
            sliderEdgeStarVelocityMax = data.maxVelocityEdge;

            sliderStarSizeMin = data.minSize;
            sliderStarSizeMax = data.maxSize;
            sliderEdgeStarSizeMin = data.minSizeEdge;
            sliderEdgeStarSizeMax = data.maxSizeEdge;

            sliderTwinkleSpeed = data.twinkleSpeed;
            sliderEdgeTwinkleSpeed = data.twinkleSpeedEdge;

            sliderLifespan = data.lifespan;
            sliderTimeToFade = data.timeToFade;

            // first, reset all toggles to false
            for(int i = 0; i < constellationToggles.Length; i++)
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

            //sliderPercentEdge = data.percentEdge;

            Color newCol;
            if (ColorUtility.TryParseHtmlString(data.starColor, out newCol))
            {
                starColor = newCol;
            }
            Color newColEdge;
            if (ColorUtility.TryParseHtmlString(data.edgeStarColor, out newColEdge))
            {
                edgeStarColor = newColEdge;
            }
        }
    } // end class
} // end namespace