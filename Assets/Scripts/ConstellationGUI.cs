using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TeamLab.Unity
{
    public class ConstellationGUI : TeamLab.Unity.MenuBase
    {
        public Sky sky;
        public Star star;
        static public Data data = new Data();

        // number of stars in constellation
        public int sliderNumStars;

        // velocity range of stars
        public float sliderStarVelocityMin;
        public float sliderStarVelocityMax;

        // size range of stars
        public float sliderStarSizeMin;
        public float sliderStarSizeMax;

        // twinkle speed of stars
        public int sliderTwinkleSpeed;

        // lifespan of stars
        public float sliderLifespan;

        // time over which stars fade
        public float sliderTimeToFade;

        // toggle for each constellation
        public bool toggleUrsa = true;
        public bool toggleLeo = true;
        public bool toggleTiger = true;
        // List of toggles
        public List<bool> toggleList = new List<bool>();

        // slider for number of constellations to spawn
        public int sliderConstellationCount;

        // constellation mesh visibility
        public float sliderMeshAlpha = 1.0f;

        // constellation mesh complexity
        public float sliderMeshQuality;

        // toggle for constellation visual effect
        public GUIUtil.SelectionGridForEnum<Data.ConstellationMode>
            toggleConstellationMode
            = new GUIUtil.SelectionGridForEnum<Data.ConstellationMode>();

        // color of stars
        public Color starColor = new Color32(170, 236, 255, 255);
        public GUIUtil.ColorField starColorGUI = new GUIUtil.ColorField();

        public Renderer starRenderer;

        protected override void Start()
        {
            base.Start();

            sky = GameObject.FindObjectOfType<Sky>();

            sliderNumStars = data.numStars;

            sliderStarVelocityMin = data.minVelocity;
            sliderStarVelocityMax = data.maxVelocity;

            sliderStarSizeMin = data.minSize;
            sliderStarSizeMax = data.maxSize;

            sliderTwinkleSpeed = data.twinkleSpeed;

            sliderLifespan = data.lifespan;

            sliderTimeToFade = data.timeToFade;

            toggleList.Add(toggleUrsa);
            toggleList.Add(toggleLeo);
            toggleList.Add(toggleTiger);

            sliderConstellationCount = data.constellationCount;
            sliderMeshQuality = data.quality;

            starRenderer = star.GetComponent<Renderer>();

            // starting starColor
            data.starColorUpdate.SetValueCPUOnly(starColor);
            data.starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);

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
            GUILayout.Label("Number of stars:");
            sliderNumStars = GUIUtil.Slider(sliderNumStars, 50, 500);
            data.numStars = sliderNumStars;
            GUILayout.EndHorizontal();

            // sliders to determine range of stars' velocity
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of star velocities:");
            sliderStarVelocityMin = GUIUtil.Slider(sliderStarVelocityMin, 1, 50);
            data.minVelocity = sliderStarVelocityMin;

            sliderStarVelocityMax = GUIUtil.Slider(sliderStarVelocityMax, 1, 50);
            data.maxVelocity = sliderStarVelocityMax;
            GUILayout.EndHorizontal();

            // sliders to determine range of stars' size
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of star sizes:");
            sliderStarSizeMin = GUIUtil.Slider(sliderStarSizeMin, 0.1f, 10.0f);
            data.minSize = sliderStarSizeMin;

            sliderStarSizeMax = GUIUtil.Slider(sliderStarSizeMax, 0.1f, 10.0f);
            data.maxSize = sliderStarSizeMax;
            GUILayout.EndHorizontal();

            // slider to determine rate at which stars twinkle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Twinkle speed:");
            sliderTwinkleSpeed = GUIUtil.Slider(sliderTwinkleSpeed, 1, 20);
            data.twinkleSpeed = sliderTwinkleSpeed;
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

            toggleUrsa = GUILayout.Toggle(toggleUrsa, "Ursa");
            sky.UpdateConstellationList(toggleUrsa, sky.Ursa);

            toggleLeo = GUILayout.Toggle(toggleLeo, "Leo");
            sky.UpdateConstellationList(toggleLeo, sky.Leo);

            toggleTiger = GUILayout.Toggle(toggleTiger, "Tiger");
            sky.UpdateConstellationList(toggleTiger, sky.Tiger);

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
            sky.UpdateConstellationAlpha(sliderMeshAlpha, oldAlpha);
            GUILayout.EndHorizontal();

            // Slider to determine complexity of constellation mesh
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellation mesh complexity:");
            sliderMeshQuality = GUIUtil.Slider(sliderMeshQuality, 0, 1);
            data.quality = sliderMeshQuality;
            GUILayout.EndHorizontal();

            // toggles to decide constellation mode
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellation Mode");
            // public E OnGUI(E enumInitialValue, int xWidth)
            // will probably need to update xWidth as more modes are added
            data.mode = toggleConstellationMode.OnGUI(data.mode, 3);

            GUILayout.EndHorizontal();

            // Color Field to determine colors of stars
            GUILayout.BeginHorizontal();

            starColorGUI.OnGUI(ref starColor);

            // Use ShaderVariableGeneric helper class to only update shader when changed
            data.starColorUpdate.SetValueCPUOnly(starColor);
            data.starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);

            GUILayout.EndHorizontal();
        }

        public override void Save()
        {

            // TODO: make filename variable more descriptive
            var path = TeamLab.Unity.PackageAndSceneSpecificPath.Static.GetSaveLoadPathWithFileDefault("saveFile.txt");
            using (var writer = new StreamWriter(path))
            {
                string json = UnityEngine.JsonUtility.ToJson(data, true);
                writer.Write(json);
            }
        }

        public override void Load()
        {

            var path = TeamLab.Unity.PackageAndSceneSpecificPath.Static.GetSaveLoadPathWithFileDefault("saveFile.txt");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                UnityEngine.JsonUtility.FromJsonOverwrite(json, data);
            }

            UpdateGUIData();
        }

        public void UpdateGUIData()
        {
            //TODO: set slider values to new data values when new Data is loaded
            // >> Maybe make an InitializeData() function?

            sliderNumStars = data.numStars;
            sliderStarVelocityMin = data.minVelocity;
            sliderStarVelocityMax = data.maxVelocity;
        }
    } // end class
} // end namespace