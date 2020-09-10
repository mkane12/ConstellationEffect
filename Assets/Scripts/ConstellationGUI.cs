﻿using System.Collections;
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
        public bool[] constellationToggles = new bool[Enum.GetNames(typeof(Sky.ConstellationType)).Length];

        // slider for number of constellations to spawn
        public int sliderConstellationCount;

        // constellation mesh visibility
        public float sliderMeshAlpha = 1.0f;

        // constellation mesh complexity
        public float sliderMeshQuality;

        // toggle for constellation visual effect
        public GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>
            toggleConstellationMode
            = new GUIUtil.SelectionGridForEnum<Sky.ConstellationMode>();

        // Utilizes SharedVariableColor helper class in TeamLabUnityFrameworks
        public ShaderVariableColor starColorUpdate = new ShaderVariableColor("_Color"); // star color

        // color of stars
        public Color starColor;

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

            // set all toggles to true to start
            for(int i = 0; i < constellationToggles.Length; i++)
            {
                constellationToggles[i] = true;
            }

            sliderConstellationCount = data.constellationCount;
            sliderMeshQuality = data.quality;

            starRenderer = star.GetComponent<Renderer>();

            // starting starColor
            Color newCol;
            if (ColorUtility.TryParseHtmlString(data.starColor, out newCol))
            {
                starColor = newCol;
            }

            starColorUpdate.SetValueCPUOnly(starColor);
            starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);

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
            sky.mode = toggleConstellationMode.OnGUI(sky.mode, 3);

            GUILayout.EndHorizontal();

            // Color Field to determine colors of stars
            GUILayout.BeginHorizontal();

            starColorGUI.OnGUI(ref starColor);

            // Use ShaderVariableGeneric helper class to only update shader when changed
            starColorUpdate.SetValueCPUOnly(starColor);
            starColorUpdate.SetToMaterial(starRenderer.sharedMaterial);
            data.starColor = "#" + ColorUtility.ToHtmlStringRGBA(starColor);

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

            Debug.Log(string.Join(", ", data.constellationNames));
        }

        public override void Load()
        {

            var path = TeamLab.Unity.PackageAndSceneSpecificPath.Static.GetSaveLoadPathWithFileDefault("GUISettings.txt");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                UnityEngine.JsonUtility.FromJsonOverwrite(json, data);
            }

            Debug.Log(string.Join(", ", data.constellationNames));

            UpdateGUIData();
        }

        // update sliders to reflect loaded data
        public void UpdateGUIData()
        {
            sliderNumStars = data.numStars;
            sliderStarVelocityMin = data.minVelocity;
            sliderStarVelocityMax = data.maxVelocity;
            sliderStarSizeMin = data.minSize;
            sliderStarSizeMax = data.maxSize;
            sliderTwinkleSpeed = data.twinkleSpeed;
            sliderLifespan = data.lifespan;
            sliderTimeToFade = data.timeToFade;

            //TODO update toggles
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
            sliderMeshQuality = data.quality;

            Color newCol;
            if (ColorUtility.TryParseHtmlString(data.starColor, out newCol))
            {
                starColor = newCol;
            }
        }
    } // end class
} // end namespace