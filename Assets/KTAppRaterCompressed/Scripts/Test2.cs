using UnityEngine;
using System.Collections;

public class Test2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(replace());
	}

	IEnumerator replace () {
		yield return new WaitForSeconds(45.0f);
//		Debug.Break();
		AppraterScript.ShowRaterPopup();
	}
}
