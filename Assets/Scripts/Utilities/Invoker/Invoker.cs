using Assets.Scripts.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities.Invoker
{
    public class Invoker : MonoBehaviour, INotifyPropertyChanged
    {
        private static Invoker _Instance;
        public static Invoker Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = FindObjectOfType<Invoker>();
                    if(_Instance == null)
                        _Instance = new GameObject("Invoker", typeof(Invoker)).GetComponent<Invoker>();
                }

                return _Instance;
            }
        }

        private static bool _StartOnInitialize = true;
        public static bool StartOnInitialize
        {
            get { return _StartOnInitialize; }
            set { _StartOnInitialize = value; }
        }

        private static bool _KeepAliveCallbacks = false;
        public static bool KeepAliveCallBacks
        {
            get { return _KeepAliveCallbacks; }
            set { _KeepAliveCallbacks = value; }
        }

        /// <summary>
        /// Object is getting destroyed, take down all the callbacks with it.
        /// </summary>
        private void OnDestroy()
        {
            if (!KeepAliveCallBacks)
            {
                foreach (KeyValuePair<Guid, ICallbackObject> entry in CallBacks)
                    if (entry.Value != null)
                        entry.Value.CancelCallBack();
            }
        }

        /// <summary>
        /// Initializes the callback on the invoker.
        /// </summary>
        /// <param name="callbackObject"></param>
        /// <returns></returns>
        private Guid InitializeCallBack(MonoBehaviour callbackObject)
        {
            if (!(callbackObject is ICallbackObject))
                throw new InvalidCastException(string.Format("CallBack object does not implement '{0}'", typeof(ICallbackObject).ToString()));

            Guid callBackGUID = Guid.NewGuid();
            ((ICallbackObject)callbackObject).UID = callBackGUID;
            callbackObject.name = callBackGUID.ToString();
            callbackObject.transform.parent = Instance.transform;

            ((ICallbackObject)callbackObject).CallBackSuccess += Invoker_CallBackDone;
            ((ICallbackObject)callbackObject).CallBackSuccess += Invoker_CallBackDone;

            Instance.CallBacks.Add(callBackGUID, (ICallbackObject)callbackObject);

            if (StartOnInitialize)
                StartCoroutine(StartAfterTheFrame((ICallbackObject)callbackObject));

            return callBackGUID;
        }

        /// <summary>
        /// Callback's job is complete, get rid of it.
        /// </summary>
        /// <param name="co"></param>
        private void Invoker_CallBackDone(ICallbackObject co)
        {
            if (CallBacks.ContainsKey(co.UID))
                CallBacks.Remove(co.UID);

            Destroy(((MonoBehaviour)co).gameObject);
        }

        /// <summary>
        /// A Delay is needed to be able to initialize an object before it starts so that the user can call additional code to register to events, or do something else with it before it starts.
        /// </summary>
        /// <param name="callbackObject"></param>
        /// <returns></returns>
        private System.Collections.IEnumerator StartAfterTheFrame(ICallbackObject callbackObject)
        {
            yield return new WaitForEndOfFrame();
            callbackObject.StartCallBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                var propChangedEvent = PropertyChanged;
                propChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        Dictionary<Guid, ICallbackObject> CallBacks = new Dictionary<Guid, ICallbackObject>();

        public static ICallbackObject CallWithIntervals(Action callback, int repeat = int.MaxValue, float waitTime = float.MinValue)
        {
            IntervalCallBackObject intervalCallBack = new GameObject("", typeof(IntervalCallBackObject)).GetComponent<IntervalCallBackObject>();
            intervalCallBack.Initialize(callback, repeat, waitTime);
            Instance.InitializeCallBack(intervalCallBack);

            return intervalCallBack;
        }

        public static ICallbackObject WaitThanCallback(Action callback, float waitTime)
        {
            WaitThanCallBackObject waitThanCallBackObject = new GameObject("", typeof(WaitThanCallBackObject)).GetComponent<WaitThanCallBackObject>();
            waitThanCallBackObject.Initialize(callback, waitTime);
            Instance.InitializeCallBack(waitThanCallBackObject);

            return waitThanCallBackObject;
        }

        public static ICallbackObject ExecuteOnCondition(Action callBack, Func<bool> condition, float checkInterval = float.MinValue)
        {
            ConditionCallBackObject conditionCallback = new GameObject("", typeof(ConditionCallBackObject)).GetComponent<ConditionCallBackObject>();
            conditionCallback.Initialize(callBack, condition, checkInterval);
            Instance.InitializeCallBack(conditionCallback);

            return conditionCallback;
        }

        public static bool IsRunning(Guid uid)
        {
            return (Instance.CallBacks.ContainsKey(uid)) ? Instance.CallBacks[uid].IsRunning : false;
        }

        public static bool CallBackExists(Guid uid)
        {
            return Instance.CallBacks.ContainsKey(uid);
        }

        public static void StartCallBack(Guid uid)
        {
            if (CallBackExists(uid) && !IsRunning(uid))
                Instance.CallBacks[uid].StartCallBack();
        }

        public static void StopCallBack(Guid uid)
        {
            if (CallBackExists(uid))
                Instance.CallBacks[uid].StopCallBack();
        }

        public static void ContinueCallBack(Guid uid)
        {
            if (CallBackExists(uid))
                Instance.CallBacks[uid].ResumeCallBack();
        }

        public static void CancelCallBack(Guid uid)
        {
            if (CallBackExists(uid))
                Instance.CallBacks[uid].CancelCallBack();
        }
    }
}
