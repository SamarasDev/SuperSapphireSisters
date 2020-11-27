using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayScript : MonoBehaviour
{
	
	public GameObject mainCamera;
	public MeshRenderer cubeRender;
	public Text buttonText;

	private Gyroscope gyro;
	private float rotMultiplier = 50;
	private bool lockCam = false;
	
    // Start is called before the first frame update
    void Start()
    {     
		if (SystemInfo.supportsGyroscope)
		{
			gyro = Input.gyro;
			gyro.enabled = true;
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(
						 new Vector3(Input.GetTouch(0).position.x, 
						 Input.GetTouch(0).position.y, 0));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit))
			{
				float r, g, b;
				r = Random.Range(0.0f, 1.0f);
				g = Random.Range(0.0f, 1.0f);
				b = Random.Range(0.0f, 1.0f);
				cubeRender.material.color = new Color(r, g, b);
			}
		}
		
		if (lockCam == false)
			mainCamera.transform.rotation = Quaternion.Euler(
            gyro.attitude.x * rotMultiplier, 
            gyro.attitude.y * rotMultiplier, 0);
    }
	
	public void LockButton()
	{
		if (lockCam == false)
		{
			lockCam = true;
			buttonText.text = "UNLOCK";
		}
		else
		{
			lockCam = false;
			buttonText.text = "LOCK";
		} 
	}
}
