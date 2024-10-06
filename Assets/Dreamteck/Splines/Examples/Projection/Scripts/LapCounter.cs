namespace Dreamteck.Splines.Examples
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LapCounter : MonoBehaviour
    {
        int currentLap;
        public TextMesh text;

        public void CountLap()
        {
            Debug.Log(1);
            currentLap++;
            text.text = "LAP " + currentLap;
        }
    }
}
