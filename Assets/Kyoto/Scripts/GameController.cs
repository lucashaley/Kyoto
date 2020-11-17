using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

namespace Kyoto
{
    public class GameController : Singleton<GameController>
    {
        [Header("Debugging")]
        public bool debug;
        private GameObject mainCanvasObject;
        private GameObject debugTextObject;
        public Text gameDateNormalizedText;
        public Text gameTimeText;
        private TimeController timeController;
        public ViewStateController viewStateController;

        [Header("Global Variables")]
        // REFACTOR to data?
        public float rakeThreshold = 0.05f;
        public float stateChangeTime = 0.8f;

        [Header("Fading")]
        public float shadowDistance = 60f;
        public float shadowDistanceFaded = 30f;
        public float shadowFadeTime = 0.5f;

        public float fadeableFadeAmount = 0.1f;
        public Color fadeableOutlineColor = Color.red;
        public float fadeableOutlineWidth = 0.25f;

        // Start is called before the first frame update
        void Awake()
        {
            mainCanvasObject = GameObject.Find("MainCanvas");
            Debug.Assert(mainCanvasObject != null, "Could not find <color=red>MainCanvas</color>.", gameObject);
            debugTextObject = GameObject.Find("DebugText");
            Debug.Assert(debugTextObject != null, "Could not find <color=red>debugTextObject</color>.", gameObject);
            gameDateNormalizedText = GameObject.Find("GameDateNormalized").GetComponent<Text>();
            Debug.Assert(gameDateNormalizedText != null, "Could not find <color=red>gameDateNormalizedText</color>.", gameObject);
            gameTimeText = GameObject.Find("GameTime").GetComponent<Text>();
            Debug.Assert(gameTimeText != null, "Could not find <color=red>gameTimeText</color>.", gameObject);
            timeController = TimeController.Instance;
            // REFACTOR
            viewStateController = GameObject.Find("ViewController").GetComponent<ViewStateController>();
        }

        public void UpdateGameDateNormalizedText (float newValue)
        {
            gameDateNormalizedText.text = "Game Date Normalized: " + newValue;
        }

        public void UpdateGameTimeText(string newValue)
        {
            gameTimeText.text = "Game Time: " + newValue;
        }
    }
}
