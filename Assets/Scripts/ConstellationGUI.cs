using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamLab.Unity
{
    public class ConstellationGUI : TeamLab.Unity.MenuBase
    {
        public Sky sky;
        // number of stars in constellation
        public int sliderNumStars;
        // toggle for each constellation
        public bool toggleUrsa = true;
        public bool toggleLeo = true;
        public bool toggleTiger = true;

        protected override void Start()
        {
            sky = GameObject.FindObjectOfType<Sky>();
            sliderNumStars = sky.numStars;

            sky.constellationList.Add(sky.Ursa);
            sky.constellationList.Add(sky.Leo);
            sky.constellationList.Add(sky.Tiger);

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
            GUILayout.BeginHorizontal();
            GUILayout.Label("Constellations to choose from:");

            toggleUrsa = GUILayout.Toggle(toggleUrsa, "Ursa");
            if(!toggleUrsa)
            {
                sky.constellationList.Remove(sky.Ursa);
            }
            else if (toggleUrsa & !sky.constellationList.Contains(sky.Ursa))
            {
                sky.constellationList.Add(sky.Ursa);
            }

            toggleLeo = GUILayout.Toggle(toggleLeo, "Leo");
            if (!toggleLeo)
            {
                sky.constellationList.Remove(sky.Leo);
            }
            else if (toggleLeo & !sky.constellationList.Contains(sky.Leo))
            {
                sky.constellationList.Add(sky.Leo);
            }

            toggleTiger = GUILayout.Toggle(toggleTiger, "Tiger");
            if (!toggleTiger)
            {
                sky.constellationList.Remove(sky.Tiger);
            }
            else if (toggleTiger & !sky.constellationList.Contains(sky.Tiger))
            {
                sky.constellationList.Add(sky.Tiger);
            }

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