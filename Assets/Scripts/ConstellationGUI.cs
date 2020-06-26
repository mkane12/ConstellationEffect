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

        // toggle for each constellation
        public bool toggleUrsa = true;
        public bool toggleLeo = true;
        public bool toggleTiger = true;
        // List of toggles
        public List<bool> toggleList = new List<bool>();

        // color of stars
        public Color starColor = new Color32(170, 236, 255, 255);
        public GUIUtil.ColorField starColorGUI = new GUIUtil.ColorField();

        //public ShaderVariableColor starColorUpdate = new ShaderVariableColor("_Color");

        public Renderer starMat;

        protected override void Start()
        {
            sky = GameObject.FindObjectOfType<Sky>();
            sliderNumStars = sky.numStars;

            toggleList.Add(toggleUrsa);
            toggleList.Add(toggleLeo);
            toggleList.Add(toggleTiger);

            starMat = star.GetComponent<Renderer>();

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

            GUILayout.BeginHorizontal();
            starColorGUI.OnGUI(ref starColor);
            // TODO: inefficient to change shader every frame
            starMat.sharedMaterial.SetColor("_Color", starColor);
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