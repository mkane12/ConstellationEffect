using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamLab.Unity
{
    public class ConstellationGUI : TeamLab.Unity.MenuBase
    {
        public Sky sky;
        public int sliderNumStars;
        public GUIContent[] checkConstellationList;

        protected override void Start()
        {
            sky = GameObject.FindObjectOfType<Sky>();
            sliderNumStars = sky.numStars;

            base.showButtons.save = false;
            base.showButtons.load = false;
            base.autoSaveOnClose = false;

            base.Start();
        }

        public override void OnGUIInternal() // <-- Please write all GUI code here
        {
            // slider to determine number of stars in constellation
            GUILayout.BeginHorizontal();
            GUILayout.Label("Number of stars:");
            sliderNumStars = GUIUtil.Slider(sliderNumStars, 1, 500);
            sky.numStars = sliderNumStars;
            GUILayout.EndHorizontal();

            // checkbox to decide constellation patterns chosen from
            /*GUILayout.BeginHorizontal();
            GUILayout.Label("Constellations to choose from:");
            checkConstellationList = GUIUtil.DropdownList;
            GUILayout.EndHorizontal();*/

        }

        public override void Save() // If nothing to save, the function can be empty
        {
        }

        public override void Load() // If nothing to load, the function can be empty
        {
        }
    }// end class
} // end namespace