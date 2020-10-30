using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

namespace SuperTreeView
{

    public class ButtonEx : Button
    {
        [Serializable]
        public class DoubleClickedEvent : UnityEvent { }

        [SerializeField]
        private DoubleClickedEvent mOnDoubleClick = new DoubleClickedEvent();
        public DoubleClickedEvent OnDoubleClick
        {
            get { return mOnDoubleClick; }
            set { mOnDoubleClick = value; }
        }

        private DateTime mFirstClickTime;
        private DateTime mSecondClickTime;

        private void OnPress()
        {
            if (null != mOnDoubleClick)
            {
                mOnDoubleClick.Invoke();
            }
            ResetClickTime();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (mFirstClickTime.Equals(default(DateTime)))
            {
                mFirstClickTime = DateTime.Now;
            }
            else
            {
                mSecondClickTime = DateTime.Now;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!mFirstClickTime.Equals(default(DateTime)) && !mSecondClickTime.Equals(default(DateTime)))
            {
                var intervalTime = mSecondClickTime - mFirstClickTime;
                float milliSeconds = intervalTime.Seconds * 1000 + intervalTime.Milliseconds;
                if (milliSeconds < 400)
                {
                    OnPress();
                }
                else
                {
                    ResetClickTime();
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            ResetClickTime();
        }

        private void ResetClickTime()
        {
            mFirstClickTime = default(DateTime);
            mSecondClickTime = default(DateTime);
        }
    }
}