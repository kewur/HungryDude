using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utilities.Invoker
{
    public interface ICallbackObject
    {
        Guid UID { get; set; }
        bool IsRunning { get; }
        void StartCallBack();
        void StopCallBack();
        void CancelCallBack();
        void ResumeCallBack();

        event CallBackStatusEventHandler CallBackCanceled;
        event CallBackStatusEventHandler CallBackSuccess;
        event CallBackStatusEventHandler CallBackStarted;
        event CallBackStatusEventHandler CallBackStopped;
        event CallBackStatusEventHandler CallBackResumed;
    }
}
