using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Controllers
{
	public class InputController : MonoBehaviour
	{
		private static InputController _Instance = null;
		public static InputController Instance
		{
			get 
			{
				if(_Instance == null)
				{
					_Instance = FindObjectOfType<InputController>();
					if(_Instance == null)
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
		const float DoubleClickInterval = 0.3f;
		const float InputRayMaxDistance = 30f;

		public static int InputLayerMask = Layers.ALL_LAYERS_MASK; 

		public Camera InputCamera;



		void Awake()
		{
			if (InputCamera == null)
				InputCamera = Camera.main;
		}

		void Update () 
		{
			Vector3? InputPosition = null;

#if UNITY_EDITOR
			if(Input.GetMouseButtonUp(0))
				InputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
#if MOBILE
		    //input.TouchDown()
#endif

			if(InputPosition.HasValue && Time.time - _LastClickTime < InputController.DoubleClickInterval)
			{
				RaycastHit hitout;

				if(Physics.Raycast(InputPosition.Value, InputCamera.transform.forward, out hitout, InputRayMaxDistance, InputLayerMask))
				{
				
				}


			}
		}
	}
}