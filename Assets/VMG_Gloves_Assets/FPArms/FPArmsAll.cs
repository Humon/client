using UnityEngine;
using System.Collections;

public class FPArmsAll : MonoBehaviour {
	//decided not to use arrays or any more complicated shader manipulatons so any newbie that want to see whats going on in this script can check the code and find related texture2D object


	public GameObject arms1; 
	public GameObject arms2;
	public GameObject arms3;
	public GameObject arms4;

	public GameObject arms1mesh; 
	public GameObject arms2mesh;
	public GameObject arms3mesh;
	public GameObject arms4mesh;

	public Camera mainCameraGO;
	public GameObject mainCameraSnapObject1;
	public GameObject mainCameraSnapObject2;
	public GameObject mainCameraSnapObject3;
	public GameObject mainCameraSnapObject4;

	public Texture arms1Light;
	public Texture arms2Light;
	public Texture arms2Light2;
	public Texture arms3Light;
	public Texture arms4Light;
	public Texture arms4Light2;

	public Texture arms1Medium;
	public Texture arms2Medium;
	public Texture arms2Medium2;
	public Texture arms3Medium;
	public Texture arms4Medium;
	public Texture arms4Medium2;

	public Texture arms1Dark;
	public Texture arms2Dark;
	public Texture arms2Dark2;
	public Texture arms3Dark;
	public Texture arms4Dark;
	public Texture arms4Dark2;

	public Texture arms1DarkBlue;
	public Texture arms1DarkPurple;
	public Texture arms1DarkRed;
	public Texture arms1DarkWine;

	public Texture arms1LightBlue;
	public Texture arms1LightPurple;
	public Texture arms1LightRed;
	public Texture arms1LightWine;

	public Texture arms1MediumBlue;
	public Texture arms1MediumPurple;
	public Texture arms1MediumRed;
	public Texture arms1MediumWine;

	public Texture arms2DarkBlue;
	public Texture arms2DarkPurple;
	public Texture arms2DarkRed;
	public Texture arms2DarkWine;

	public Texture arms2LightBlue;
	public Texture arms2LightPurple;
	public Texture arms2LightRed;
	public Texture arms2LightWine;

	public Texture arms2MediumBlue;
	public Texture arms2MediumPurple;
	public Texture arms2MediumRed;
	public Texture arms2MediumWine;


	public Texture arms2DarkBlue2;
	public Texture arms2DarkPurple2;
	public Texture arms2DarkRed2;
	public Texture arms2DarkWine2;

	public Texture arms2LightBlue2;
	public Texture arms2LightPurple2;
	public Texture arms2LightRed2;
	public Texture arms2LightWine2;

	public Texture arms2MediumBlue2;
	public Texture arms2MediumPurple2;
	public Texture arms2MediumRed2;
	public Texture arms2MediumWine2;



	public GameObject popup;

	public int skinTone = 0;
	public int nailColor = 0;
	public int camoTone = 0;

	public float cameraNormalFov = 60f;
	public float cameraNearClipping = 0.05f;
	// Use this for initialization
	void Start () {
		mainCameraGO = Camera.main;
		popup = GameObject.FindGameObjectWithTag("InformationPlate");
		mainCameraSnapObject1 = GameObject.FindGameObjectWithTag("mainCameraSnapObject1");
		mainCameraSnapObject2 = GameObject.FindGameObjectWithTag("mainCameraSnapObject2");
		mainCameraSnapObject3 = GameObject.FindGameObjectWithTag("mainCameraSnapObject3");
		mainCameraSnapObject4 = GameObject.FindGameObjectWithTag("mainCameraSnapObject4");

		arms1 = GameObject.FindGameObjectWithTag("ArmsObject1");
		arms2 = GameObject.FindGameObjectWithTag("ArmsObject2");
		arms3 = GameObject.FindGameObjectWithTag("ArmsObject3");
		arms4 = GameObject.FindGameObjectWithTag("ArmsObject4");

		arms1mesh = GameObject.FindGameObjectWithTag("ArmsMesh1");
		arms2mesh = GameObject.FindGameObjectWithTag("ArmsMesh2");
		arms3mesh = GameObject.FindGameObjectWithTag("ArmsMesh3");
		arms4mesh = GameObject.FindGameObjectWithTag("ArmsMesh4");


		mainCameraGO.transform.position = mainCameraSnapObject1.transform.position;
		mainCameraGO.transform.rotation = mainCameraSnapObject1.transform.rotation;
		mainCameraGO.nearClipPlane = cameraNearClipping;

		Debug.Log (mainCameraSnapObject1);
		Debug.Log (mainCameraSnapObject2);
		Debug.Log (mainCameraSnapObject3);
		Debug.Log (mainCameraSnapObject4);
		mainCameraGO.nearClipPlane = cameraNearClipping;
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKey("1"))
		{
			playIdle();
		}

		if (Input.GetKey("2"))
		{
			playJump();
		}

		if (Input.GetKey("3"))
		{
			playPunch();
		}

		if (Input.GetKey("4"))
		{
			playPushDoor();
		}

		if (Input.GetKey("5"))
		{
			playSprint();
		}

		if (Input.GetKey("6"))
		{
			playThrow();
		}

		if (Input.GetKey("q"))
		{
			switch1();
		}

		if (Input.GetKey("w"))
		{
			switch2();
		}

		if (Input.GetKey("e"))
		{
			switch4();
		}

		if (Input.GetKey("r"))
		{
			switch3();
		}

		if (Input.GetKeyDown("a"))
		{
			toggleColorCycle();
		}

		if (Input.GetKeyDown("d"))
		{
			toggleNailCycle();
		}
		if (Input.GetKeyDown("s"))
		{
			toggleCamoCycle();
		}

		if (Input.GetKeyDown("0"))
		{
			if (popup.activeInHierarchy){
			popup.SetActive(false);
			} else { popup.SetActive(true);}

		}



	}

	void toggleColorCycle()
	{
		skinTone++;
		if (skinTone >= 3)
		{
			skinTone =0;
		}

		switch (skinTone)
		{
		case 0:
			toggleColor0();
			break;
		case 1:
			toggleColor1();
			break;
		case 2:
			toggleColor2();
			break;
		case 3:
			skinTone = 0;
			break;
		}

	}

	void toggleCamoCycle()
	{
		camoTone++;
		if (camoTone >= 2)
		{
			camoTone =0;
		}
		if (camoTone == 1)
		{
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light2;
			arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light2;
		}

		else if (camoTone == 0)
		{
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light;
			arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light;
		}




	}

	void toggleNailCycle()
	{
		nailColor++;
		if (nailColor >= 5)
		{
			nailColor = 0;
		}

		switch (nailColor)
		{
		case 0:
			toggleNails0();
			break;
		case 1:
			toggleNails1();
			break;
		case 2:
			toggleNails2();
			break;
		case 3:
			toggleNails3();
			break;
		case 4:
			toggleNails4();
			break;
		case 5:
			nailColor = 0;
			break;


		}



	}


	public void playIdle()
	{
		arms1.GetComponent<Animation>().Play("FPArms_Unarmed_Idle");
		arms2.GetComponent<Animation>().Play("FPArms_Unarmed_Idle");
		arms3.GetComponent<Animation>().Play("FPArms_Unarmed_Idle");
		arms4.GetComponent<Animation>().Play("FPArms_Unarmed_Idle");

	}

	public void playJump()
	{
		arms1.GetComponent<Animation>().Play("FPArms_Unarmed_Jump");
		arms2.GetComponent<Animation>().Play("FPArms_Unarmed_Jump");
		arms3.GetComponent<Animation>().Play("FPArms_Unarmed_Jump");
		arms4.GetComponent<Animation>().Play("FPArms_Unarmed_Jump");

	}
	public void playPunch()
	{
		arms1.GetComponent<Animation>().Play("FPArms_Unarmed_Punch");
		arms2.GetComponent<Animation>().Play("FPArms_Unarmed_Punch");
		arms3.GetComponent<Animation>().Play("FPArms_Unarmed_Punch");
		arms4.GetComponent<Animation>().Play("FPArms_Unarmed_Punch");

	}
	public void playPushDoor()
	{
		arms1.GetComponent<Animation>().Play("FPArms_Unarmed_Push-Door");
		arms2.GetComponent<Animation>().Play("FPArms_Unarmed_Push-Door");
		arms3.GetComponent<Animation>().Play("FPArms_Unarmed_Push-Door");
		arms4.GetComponent<Animation>().Play("FPArms_Unarmed_Push-Door");

	}

	public void playSprint()
	{
		arms1.GetComponent<Animation>().Play("FPArms_Unarmed_Sprint");
		arms2.GetComponent<Animation>().Play("FPArms_Unarmed_Sprint");
		arms3.GetComponent<Animation>().Play("FPArms_Unarmed_Sprint");
		arms4.GetComponent<Animation>().Play("FPArms_Unarmed_Sprint");

	}
	public void playThrow()
	{
		arms1.GetComponent<Animation>().Play("FPArms_Unarmed_Throw");
		arms2.GetComponent<Animation>().Play("FPArms_Unarmed_Throw");
		arms3.GetComponent<Animation>().Play("FPArms_Unarmed_Throw");
		arms4.GetComponent<Animation>().Play("FPArms_Unarmed_Throw");

	}

	public void toggleNails0()
	{
		if (skinTone == 0)
		{
		arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1Light;
		arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light;
		arms3mesh.GetComponent<Renderer>().material.mainTexture = arms3Light;
		arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light;
				arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light;
			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light2;
				arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light2;

			}



		} else if (skinTone == 1)
		{
		arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1Medium;
		arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Medium;
		arms3mesh.GetComponent<Renderer>().material.mainTexture = arms3Medium;
		arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Medium;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Medium;
				arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Medium;
			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Medium2;
				arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Medium2;

			}


		} else if (skinTone == 2)
		{
		arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1Dark;
		arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Dark;
		arms3mesh.GetComponent<Renderer>().material.mainTexture = arms3Dark;
		arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Dark;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Dark;
				arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Dark;
			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Dark2;
				arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Dark2;

			}
		}
	}

	public void toggleNails1()
	{
		{
			if (skinTone == 0)
			{
				arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1LightBlue;
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightBlue;
				if (camoTone == 0)
				{
					arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightBlue;

				} else if (camoTone == 1)
				{
					arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightBlue2;


				}
			}  if (skinTone == 1)
			{
				arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1MediumBlue;
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumBlue;
				if (camoTone == 0)
				{
					arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumBlue;

				} else if (camoTone == 1)
				{
					arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumBlue2;


				}
			}  if (skinTone == 2)
			{
				arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1DarkBlue;
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkBlue;
				if (camoTone == 0)
				{
					arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkBlue;

				} else if (camoTone == 1)
				{
					arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkBlue2;


				}
			}
		}

	}

	public void toggleNails2()
	{
		if (skinTone == 0)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1LightPurple;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightPurple;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightPurple;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightPurple2;


			}
		}  if (skinTone == 1)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1MediumPurple;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumPurple;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumPurple;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumPurple2;


			}
		}  if (skinTone == 2)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1DarkPurple;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkPurple;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkPurple;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkPurple2;


			}
		}

	}

	public void toggleNails3()
	{
		if (skinTone == 0)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1LightRed;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightRed;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightRed;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightRed2;


			}
		}  if (skinTone == 1)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1MediumRed;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumRed;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumRed;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumRed2;


			}
		}  if (skinTone == 2)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1DarkRed;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkRed;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkRed;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkRed2;


			}
		}

	}

	public void toggleNails4()
	{
		if (skinTone == 0)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1LightWine;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightWine;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightWine;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2LightWine2;


			}
		}  if (skinTone == 1)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1MediumWine;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumWine;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumWine;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2MediumWine2;


			}
		}  if (skinTone == 2)
		{
			arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1DarkWine;
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkWine;
			if (camoTone == 0)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkWine;

			} else if (camoTone == 1)
			{
				arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2DarkWine2;


			}
		}

	}

	public void toggleColor0()
	{
		arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1Light;
		arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light;
		arms3mesh.GetComponent<Renderer>().material.mainTexture = arms3Light;
		arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light;

		if (camoTone == 1)
		{
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Light2;
			arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Light2;
		}

	}

	public void toggleColor1()
	{
		arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1Medium;
		arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Medium;
		arms3mesh.GetComponent<Renderer>().material.mainTexture = arms3Medium;
		arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Medium;
		if (camoTone == 1)
		{
			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Medium2;
			arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Medium2;

		}
	}

	public void toggleColor2()
	{
		arms1mesh.GetComponent<Renderer>().material.mainTexture = arms1Dark;
		arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Dark;
		arms3mesh.GetComponent<Renderer>().material.mainTexture = arms3Dark;
		arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Dark;
		if (camoTone == 1)
		{

			arms2mesh.GetComponent<Renderer>().material.mainTexture = arms2Dark2;
			arms4mesh.GetComponent<Renderer>().material.mainTexture = arms4Dark2;
		}
	}


	public void switch1()
	{
		mainCameraGO.transform.position = mainCameraSnapObject1.transform.position;
		mainCameraGO.transform.rotation = mainCameraSnapObject1.transform.rotation;
		Debug.Log("switch Female");

	}

	public void switch2()
	{
		mainCameraGO.transform.position = mainCameraSnapObject2.transform.position;
		mainCameraGO.transform.rotation = mainCameraSnapObject2.transform.rotation;
		Debug.Log("switch Female Military");

	}

	public void switch3()
	{
		mainCameraGO.transform.position = mainCameraSnapObject3.transform.position;
		mainCameraGO.transform.rotation = mainCameraSnapObject3.transform.rotation;
		Debug.Log("switch Male");

	}
	public void switch4()
	{
		mainCameraGO.transform.position = mainCameraSnapObject4.transform.position;
		mainCameraGO.transform.rotation = mainCameraSnapObject4.transform.rotation;
		Debug.Log("switch Male Military");

	}


}

