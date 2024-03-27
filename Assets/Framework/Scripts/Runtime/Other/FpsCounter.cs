using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public sealed class FpsCounter
    {
        private float m_UpdateInterval;
        private float m_CurrentFps;
        private int m_Frames;
        private float m_Accumulator;
        private float m_TimeLeft;

        public FpsCounter()
        {
            m_UpdateInterval = 0.5f;
        }
        public FpsCounter(float updateInterval)
        {
            if (updateInterval <= 0f)
            {
                Debug.LogError("Update interval is invalid.");
                return;
            }

            m_UpdateInterval = updateInterval;
        }

        public float UpdateInterval
        {
            get
            {
                return m_UpdateInterval;
            }
            set
            {
                if (value <= 0f)
                {
                    Debug.LogError("Update interval is invalid.");
                    return;
                }

                m_UpdateInterval = value;
                Reset();
            }
        }

        public float CurrentFps
        {
            get
            {
                return m_CurrentFps;
            }
        }

        public void Update()
        {
            Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_Frames++;
            m_Accumulator += realElapseSeconds;
            m_TimeLeft -= realElapseSeconds;

            if (m_TimeLeft <= 0f)
            {
                m_CurrentFps = m_Accumulator > 0f ? m_Frames / m_Accumulator : 0f;
                m_Frames = 0;
                m_Accumulator = 0f;
                m_TimeLeft += m_UpdateInterval;
            }
        }

        private void Reset()
        {
            m_CurrentFps = 0f;
            m_Frames = 0;
            m_Accumulator = 0f;
            m_TimeLeft = 0f;
        }
    }
}