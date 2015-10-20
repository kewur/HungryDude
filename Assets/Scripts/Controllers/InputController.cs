using UnityEngine;
using System.Collections;
using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Controllers
{
    public class InputController : MonoBehaviour
    {
        private static InputController _Instance = null;
        public static InputController Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<InputController>();
                    if (_Instance == null)
                    {
                        _Instance = new GameObject("InputManager", typeof(InputController)).GetComponent<InputController>();
                        _Instance.transform.parent = GameController.Instance.transform;
                        _Instance.transform.localPosition = Vector3.zero;
                    }
                }

                return _Instance;
            }
        }

        float _LastClickTime = float.MinValue;
        const float DoubleClickInterval = 1.0f;
        const float InputRayMaxDistance = 50f;

        public static int InputLayerMask = Layers.ALL_LAYERS_MASK;

        public Camera InputCamera;

        public const bool UseSingleClick = true;

        void Awake()
        {
            if (InputCamera == null)
                InputCamera = Camera.main;
        }

        void Update()
        {
            Vector3? InputPosition = null;
            Ray? InputRay = null;

#if UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0))
            {
                InputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                InputRay =  Camera.main.ScreenPointToRay(Input.mousePosition);
            }
#endif
#if UNITY_IOS || UNITY_ANDROID || UNITY_WINRT_8_0 || UNITY_WINRT_8_1
		    //input.TouchDown()
#endif 

            if (InputRay.HasValue && (Time.time - _LastClickTime < InputController.DoubleClickInterval || UseSingleClick))
            {
                RaycastHit hits;
                 
                if(Physics.Raycast(InputRay.Value, out hits, InputRayMaxDistance))
                {
                    IInteractable obj = hits.collider.GetComponent<IInteractable>();
                    if (obj != null)
                        obj.Interact();
                }

                //if (hits != null && hits.Length > 0)
                //{
                //    foreach(RaycastHit hitout in hits)
                //    {

                //    }
                //}

              
            }
            else if (InputPosition.HasValue)
                _LastClickTime = Time.time;
        }
    }
}