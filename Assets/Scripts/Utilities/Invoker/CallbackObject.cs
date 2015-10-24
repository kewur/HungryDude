using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities.Invoker
{
    public delegate void CallBackStatusEventHandler(CallbackObjectBase co);

    public abstract class CallbackObjectBase : MonoBehaviour, INotifyPropertyChanged, ICallbackObject
    {
        public const string UIDPropertyName = "UID";
        private Guid _UID;
        public Guid UID
        {
            get { return _UID; }
            set
            {
                if (_UID == value)
                    return;

                _UID = value;
                RaisePropertyChanged(UIDPropertyName);
            }
        }

        public const string IsRunningPropertyName = "IsRunning";
        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            protected set
            {
                if (_IsRunning == value)
                    return;

                _IsRunning = value;
                RaisePropertyChanged(IsRunningPropertyName);
            }
        }

        public const string CallBackPropertyName = "CallBack";
        private Action _CallBack;
        public Action CallBack
        {
            get { return _CallBack; }
            protected set
            {
                if (_CallBack == value)
                    return;

                _CallBack = value;
                RaisePropertyChanged(CallBackPropertyName);
            }
        }
       
        protected void RaiseSuccess()
        {
            if(CallBackSuccess != null)
            {
                var successEvent = CallBackSuccess;
                successEvent(this);
            }
        }

        private IEnumerator _Coroutine()
        {
            return CoroutineFunction();
        }

        protected abstract IEnumerator CoroutineFunction();

        public virtual void StartCallBack()
        {
            IsRunning = true;
            StartCoroutine(_Coroutine());

            if (CallBackStarted != null)
            {
                var startedEvent = CallBackStarted;
                startedEvent(this);
            }
        }

        public virtual void StopCallBack()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            StopAllCoroutines();

            if (CallBackStopped != null)
            {
                var stoppedEvent = CallBackStopped;
                stoppedEvent(this);
            }
        }

        public virtual void CancelCallBack()
        {
            IsRunning = false;
            StopAllCoroutines();

            if(CallBackCanceled != null)
            {
                var canceledEvent = CallBackCanceled;
                canceledEvent(this);
            }
        }

        public virtual void ResumeCallBack()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            StartCoroutine(CoroutineFunction());
            if(CallBackResumed != null)
            {
                var resumedEvent = CallBackResumed;
                resumedEvent(this);
            }
        }

        public event CallBackStatusEventHandler CallBackCanceled; //TODO make one event, StatusChanged. CallBackStatusEventHandler(object source, eventArgs status) EventArgs = OldStatus, NewStatus
        //new State = Initializing.
        public event CallBackStatusEventHandler CallBackSuccess;
        public event CallBackStatusEventHandler CallBackStarted;
        public event CallBackStatusEventHandler CallBackStopped;
        public event CallBackStatusEventHandler CallBackResumed;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var propChangeEvent = PropertyChanged;
                propChangeEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class IntervalCallBackObject : CallbackObjectBase
    {
        public const string CurrentCallCountPropertyName = "CurrentCallCount";
        private int _CurrentCallCount = 0;
        /// <summary>
        /// Amount of times this callback has been called already.
        /// </summary>
        public int CurrentCallCount
        {
            get { return _CurrentCallCount; }
            private set
            {
                if (_CurrentCallCount == value)
                    return;

                _CurrentCallCount = value;
                RaisePropertyChanged(CurrentCallCountPropertyName);
            }
        }

        public const string RepeatCountPropertyName = "RepeatCount";
        private int _RepeatCount = int.MaxValue;
        /// <summary>
        /// Amount of times the Callback will be called. int.MaxValue for infinity.
        /// </summary>
        public int RepeatCount
        {
            get { return _RepeatCount; }
            protected set
            {
                if (_RepeatCount == value)
                    return;

                _RepeatCount = value;
                RaisePropertyChanged(RepeatCountPropertyName);
            }
        }

        public const string CallIntervalPropertyName = "CallInterval";
        private float _CallInterval = float.MinValue;
        /// <summary>
        /// Time between calls. float.MinValue means it will be called at the end of every frame.
        /// </summary>
        public float CallInterval
        {
            get { return _CallInterval; }
            set
            {
                if (_CallInterval == value)
                    return;

                _CallInterval = value;
                RaisePropertyChanged(CallIntervalPropertyName);
            }
        }

        protected override IEnumerator CoroutineFunction()
        {
            while(CurrentCallCount < RepeatCount)
            {
                if (CallInterval == float.MinValue)
                    yield return new WaitForEndOfFrame();
                else
                    yield return new WaitForSeconds(CallInterval);

                CurrentCallCount++;
                CallBack();
            }

            RaiseSuccess();
        }

        public void Initialize(Action callback, int repeatAmount, float callInterval)
        {
            CallBack = callback;
            RepeatCount = repeatAmount;
            CallInterval = callInterval;
        }
    }

    public class ConditionCallBackObject : CallbackObjectBase
    {
        public const string ConditionFunctionPropertyName = "ConditionFunction";
        private Func<bool> _ConditionFunction;
        public Func<bool> ConditionFunction
        {
            get { return _ConditionFunction; }
            set
            {
                if (_ConditionFunction == value)
                    return;

                _ConditionFunction = value;
                RaisePropertyChanged(ConditionFunctionPropertyName);
            }
        }

        public const string CheckIntervalPropertyName = "CheckInterval";
        private float _CheckInterval = float.MinValue;
        public float CheckInterval
        {
            get { return _CheckInterval; }
            set
            {
                if (_CheckInterval == value)
                    return;

                _CheckInterval = value;
                RaisePropertyChanged(CheckIntervalPropertyName);
            }
        }

        protected override IEnumerator CoroutineFunction()
        {
            while(!ConditionFunction())
            {
                if (CheckInterval == float.MinValue)
                    yield return new WaitForEndOfFrame();
                else
                    yield return new WaitForSeconds(CheckInterval);
            }

            CallBack();
            RaiseSuccess();
        }

        public void Initialize(Action callBack, Func<bool> condition, float checkInterval) 
        {
            CallBack = callBack;
            ConditionFunction = condition;
            CheckInterval = checkInterval;
        }
    }

    public class WaitThanCallBackObject : CallbackObjectBase
    {
        public const string WaitTimePropertyName = "WaitTime";
        private float _WaitTime;
        public float WaitTime
        {
            get { return _WaitTime; }
            protected set
            {
                if (_WaitTime == value)
                    return;

                _WaitTime = value;
                RaisePropertyChanged(WaitTimePropertyName);
            }
        }

        protected override IEnumerator CoroutineFunction()
        {
            if (WaitTime <= 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSeconds(WaitTime);

            if (CallBack != null && IsRunning)
                CallBack();

            RaiseSuccess();
        }

        public void Initialize(Action callBack, float waitTime)
        {
            CallBack = callBack;
            WaitTime = waitTime;
        }
    }
}
