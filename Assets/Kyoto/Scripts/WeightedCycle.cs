using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public abstract class WeightedCycle : MonoBehaviour
    {
        public float weight;
        public float key;
        public bool isConstant = false;
        public AnimationCurve curve;

        public WeightedCycle priorNode, nextNode;
        private float priorKey, nextKey;
        // public TimeController timeController;

        public FloatVariable percentage;

        // Start is called before the first frame update
        void Start()
        {
            // timeController = TimeController.Instance;
            InitSiblings();
            InitCurve();
        }

        private void InitSiblings()
        {
            int siblingCount = transform.parent.childCount;
            priorNode = transform.parent.GetChild((transform.GetSiblingIndex()+siblingCount-1) % siblingCount).GetComponent<WeightedCycle>();
            nextNode = transform.parent.GetChild((transform.GetSiblingIndex()+1) % siblingCount).GetComponent<WeightedCycle>();

            // it would be great to not do this
            priorKey = priorNode.key < key ? priorNode.key : priorNode.key - 1;
            nextKey = nextNode.key > key ? nextNode.key : nextNode.key + 1;

        }

        private void InitCurve()
        {
            // set looping
            curve.preWrapMode = WrapMode.Loop;
            curve.postWrapMode = WrapMode.Loop;

            // create key for this season
            curve.AddKey(key, 1f);

            // create keys for surrounding nodes
            if (priorKey > 0)
            {
                curve.AddKey(priorKey, 0f);
                // curve.AddKey(0f, 0f);
            } else {
                curve.AddKey(1+priorKey, 0f);
                curve.AddKey(priorKey, 0f);
            }

            if (nextKey < 1)
            {
                curve.AddKey(nextKey, 0f);
                // curve.AddKey(1f, 0f);
            } else {
                curve.AddKey(nextKey-1, 0f);
                curve.AddKey(nextKey, 0f);
            }

            if (priorKey > 0 && nextKey < 1)
            {
                curve.AddKey(0f, 0f);
                curve.AddKey(1f, 0f);
            }

            // set all keys to flat
            // because Unity keys is by-value, we need to copy and replace
            Keyframe[] tempKeys = curve.keys;
            for (int i = 0; i < tempKeys.Length; i++)
            {
                tempKeys[i].inTangent = isConstant ? Mathf.Infinity : 0f;
                tempKeys[i].outTangent = isConstant ? Mathf.Infinity : 0f;

                // tempKeys[i].inTangent = 0f;
                // tempKeys[i].outTangent = 0f;
                //
                // if (isConstant)
                // {
                //     Debug.Log("Create Constant Key: " + gameObject.name);
                //     tempKeys[i].inTangent = float.PositiveInfinity;
                //     tempKeys[i].outTangent = float.PositiveInfinity;
                // }
            }
            curve.keys = tempKeys;
        }

        // Update is called once per frame
        void Update()
        {
            SetWeight();
        }

        protected abstract void SetWeight();

        public float GetWeight()
        {
            if (float.IsNaN(weight))
            {
                // Debug.Log(transform.parent.name + ":" + gameObject.name);
                return 0f;
            } else {
                return weight;
            }
        }
    }
}
