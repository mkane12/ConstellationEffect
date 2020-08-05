using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamLab.Unity
{
    public class ConstellationGUI : TeamLab.Unity.MenuBase
    {
        public Sky sky;
        public Star star;

        // number of stars in constellation
        public int sliderNumStars;

        // velocity range of stars
        public float sliderStarVelocityMin;
        public float sliderStarVelocityMax;

        // size range of stars
        public float sliderStarSizeMin;
        public float sliderStarSizeMax;

        // twinkle speed of stars
        public int twinkleSpeed;

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

        // toggle for constellation visual effect
        public GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>
            toggleConstellationMode
            = new GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>();

        // color of stars
        public Color starColor = new Color32(170, 236, 255, 255);
        public GUIUtil.ColorField starColorGUI = new GUIUtil.ColorField();

        // Utilizes SharedVariableColor helper class in TeamLabUnityFrameworks
        public ShaderVariableColor starColorUpdate = new ShaderVariableColor("_Color");
        public Renderer starRenderer;

        protected override void Start()
        {
            sky = GameObject.FindObjectOfType<Sky>();
            sliderNumStars = sky.numStars;

            sliderStarVelocityMin = StarManager.Instance.minVelocity;
            sliderStarVelocityMax = StarManager.Instance.maxVelocity;

            sliderStarSizeMin = StarManager.Instance.minSize;
            sliderStarSizeMax = StarManager.Instance.maxSize;

            twinkleSpeed = StarManager.Instance.twinkleSpeed;

            sliderLifespan = StarManager.Instance.lifespan;

            sliderTimeToFade = StarManager.Instance.timeToFade;

            toggleList.Add(toggleUrsa);
            toggleList.Add(toggleLeo);
            toggleList.Add(toggleTiger);

            starRenderer = star.GetComponent<Renderer>();

            // starting starColor
            starColorUpdate.SetValueCPUOnly(starColor);
            starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);

            base.showButtons.save = false;
            base.showButtons.load = false;
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
            sky.numStars = sliderNumStars;
            GUILayout.EndHorizontal();

            // sliders to determine range of stars' velocity
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of star velocities:");
            sliderStarVelocityMin = GUIUtil.Slider(sliderStarVelocityMin, 1, 50);
            StarManager.Instance.minVelocity = sliderStarVelocityMin;

            sliderStarVelocityMax = GUIUtil.Slider(sliderStarVelocityMax, 1, 50);
            StarManager.Instance.maxVelocity = sliderStarVelocityMax;
            GUILayout.EndHorizontal();

            // sliders to determine range of stars' size
            GUILayout.BeginHorizontal();
            GUILayout.Label("Range of star sizes:");
            sliderStarSizeMin = GUIUtil.Slider(sliderStarSizeMin, 0.1f, 10.0f);
            StarManager.Instance.minSize = sliderStarSizeMin;

            sliderStarSizeMax = GUIUtil.Slider(sliderStarSizeMax, 0.1f, 10.0f);
            StarManager.Instance.maxSize = sliderStarSizeMax;
            GUILayout.EndHorizontal();

            // slider to determine rate at which stars twinkle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Twinkle speed:");
            twinkleSpeed = GUIUtil.Slider(twinkleSpeed, 1, 20);
            StarManager.Instance.twinkleSpeed = twinkleSpeed;
            GUILayout.EndHorizontal();

            // slider to determine star lifespan
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star lifepan:");
            sliderLifespan = GUIUtil.Slider(sliderLifespan, 5, 500);
            StarManager.Instance.lifespan = sliderLifespan;
            GUILayout.EndHorizontal();

            // slider to determine time over which stars fade
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star fade time:");
            sliderTimeToFade = GUIUtil.Slider(sliderTimeToFade, 1, 500);
            StarManager.Instance.timeToFade = sliderTimeToFade;
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

            // toggles to decide constellation mode
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellation Mode");
            // public E OnGUI(E enumInitialValue, int xWidth)
            // will probably need to update xWidth as more modes are added
            sky.mode = toggleConstellationMode.OnGUI(sky.mode, 3);

            GUILayout.EndHorizontal();

            // Color Field to determine colors of stars
            GUILayout.BeginHorizontal();

            starColorGUI.OnGUI(ref starColor);

            // Use ShaderVariableGeneric helper class to only update shader when changed
            starColorUpdate.SetValueCPUOnly(starColor);
            starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);

            GUILayout.EndHorizontal();


        }

        public override void Save() // If nothing to save, the function can be empty
        {
        }

        public override void Load() // If nothing to load, the function can be empty
        {
        }
    }// end class
} // end namespace