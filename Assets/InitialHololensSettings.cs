
using UnityEngine;
using UnityEngine.XR;

public class InitialHololensSettings : MonoBehaviour {

	// Use this for initialization
	void Awake() {
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
