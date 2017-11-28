VMG_Gloves_Assets = GLove files

Tracker_Handler = Kinova Arms and tracker code. Future: Gloves also impact arm code.

Video Chat = Jeff says incorrectly implemented. Need to fix this eventually.

Streaming Assets = useless or related to video chat.

SerialComm = Written for Unity. taken from github. Currently controls hands. Will eventually be integrated with udoo and other serial peripherals.

Plugins = AVProVideo.bundle for the video. ARM_base_32.dll is the dll for the arm.

HTC Vive Unity Plugin = HTC Vive related stuff. Figure out what it does?

Dolby_Audio_Plugin = Could be part of the video asset. Investigate.

AVProVideo = For video playback. 

Scene Files = Eventually move them to a Scenes folder.


=-----------=-=-=--========--===================================================



//AVProVideo
	Asset from Unity asset Store: Not currently in use.

	**Needs Photon implementation.  Check Asset on the store and follow the directions
	**Better solution is probably to just write our own code for video feed for networking aspect.

//Dolby_Audio_Plugin
	-Standard Asset(probably) or implemented before Jeffs arival

//HTC.UnityPlugin
	-Valve VR stuff

//Materials
	2560x1440
	black circle
	Earth Color Map
	MaterialInside  //material for 360 video sphere
	RedSphere
	StarryBackGournd
	TextureInsideSurface
	TextureInsideUnit
	TextureInsideColorUnit
		//Materials
			black
			black circle
			Camera_Plane
			red
		
//Plugins
	Android

	AvProVideo.bundle
		-Unity asset store video chat asset
	
	Editor
	iOS
	tvOS
	WebGL
	WSA
	x86
	x86_64

	ARM_base_32
		-Kinova dll stuff

	**everything not marked is  standard Unity Plugin

//Prefabs
	Glove_Arms  
		-altered VMGLite glove
	
	RedBall  
		-Previously used to indicte bad locations of controllers

//Scripts
	Hand_CMD_Test_UnityPort  //testing for the hand serial stuff.  is stable
	LeftHand_Controller  //hand testing script---DELETE(broken and un-used)
	MyNetworkManager  //Handles all Network Traffic except video
	RightHand_Controller  //hand testing script--DELETE
	Tracker_Handler  //Handles both tracker pucks.  has boolean rightHand to determine which hand

//SerialComm  
	-Folder containing threading and serialization scripts for controlling the hands.  Pulled from a git project Shawn can pull up.

	-ASSETS NOT NEEDED IN SCENE FOR CLIENT

//Standard Assets

//SteamVR

//StreamingAssets

//VideoChat
	-Photon video chat folder.  Currently the video feed which is in the scene.

//VMG_Gloves_Assets
	-Unity package that comes with the gloves.

2560x1440-Black-solid-etc
CameraController
Earth Color Map

HandController.cs
	-IMPORTANT SCRIPT---original robot arm controlling script

KinovaAPI.cs
Main.cs
Red sphere
StarryBackGround

Working_Gloves_AND_Trackers
	-Main scene we are currently working from.
	-NOT NEEDED ON SERVER











