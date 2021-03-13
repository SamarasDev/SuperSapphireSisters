using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour
{
	/*
	Samaras:
	
	This is a manager for handling inputs on multiple platforms.
	It is based on the new Input System of Unity.
	It also mimics the original touch system used in the HTML5 version of Super Sapphire Sisters
	
	It handles three parts of the screen separately and assumes that
	only one finger can touch any part of the screen at any given time
	
	Let's say that this box is our 16:9 possible touchscreen
	
	=================================
	|								|
	|								|
	|								|
	|								|
	|								|
	|								|
	|								|
	=================================
	
	Now let's say that we want to divide the screen into three sections
	
	=================================
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	=================================
	
	Why would we do this?
	If you are holding the device sideways, or landscape style, 
	it is unlikely that you would ever touch any of these
	parts of the screen with more than one finger/thumb at a time,
	and we can group controls into each of the 3 sections.
	
	Therefore:
	
	We could now treat these three pieces separately and give them their own input event listeners
	since we assume only one finger would interact with each section
	
	Screen:
	
	This class attaches one "touch event listener" for each of these three screen sections.
	
				SECTION 2
	=================================
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	|		|				|		|
	=================================
	
	SECTION 1   			SECTION 3
	
	Dimensions of the Sections:
	
	(===================================================================================)
	
	The sections' widths and heights are controlled by the device widths and heights.
	However resolutions of the sections are INDEPENDENT of the sections widths and heights.
	
	It's confusing to think about but the resolution in pixels of the area drawn into each section isn't the same as the sections' device dimensions.
    Sections behave similar to HTML5 Canvas when scaled with CSS while having an internal width and height resolution..
	This system was chosen to behave similar to the responsive/resizable web design elements used in HTML5 for mobile devices.
	
	Section 2 must always be 4:3 ratio resolution and screen dimensions, even if the screen isn't 16:9.
	That means that the width of Section 2 is (4/3) of its height, whatever that height must be.
	This means that the heights of Section 1 and 3 must be the same as Section 2's height.
	The widths of Section 1 and 3 are equal, therefore they must each be half of what remains after section 2's width.
	
	Formulas for the section's dimensions are:

		Section 1 Width =  Math.round((Screen.width - ((4/3)*Screen.height))/2)
		Section 1 Height = Screen.height
		
		Section 2 Width =  Math.round((4/3)*Screen.height)
		Section 2 Height = Screen.height
		
		Section 1 Width =  Math.round((Screen.width - ((4/3)*Screen.height))/2)
		Section 1 Height = Screen.height
		
		
	Data Structures:
	
	(===================================================================================)
	
	Each Section is a struct:
	
	Width: The width of this section, in resolution pixels. This is used to calculate the X, Y, OldX, OldY, etc
	Height: The height of this section, in resolution pixels. This is used to calculate the X, Y, OldX, OldY, etc
	ScreenWidth: The width of this section in device screen pixels
	ScreenHeight: The height of this section in device screen pixels
	
	X: the x position in percentage within the element that the touch occured
	Y: the y position in percentage within the element that the touch occured
	OldX: the old x position in percentage of the touch event
	OldY: the old y position in percentage of the touch event
	Pressed: an integer representing whether the touch is still held and for how many game frames, returning 0 if no touch occurs
	
	*/
	
	public struct Section
	{	
		public int Width;
		public int Height;
		public int ScreenWidth;
		public int ScreenHeight;
		
		public double X;
		public double Y;
		public double OldX;
		public double OldY;
		
	    public int Pressed;
	}

	//Used for our wonky resizing code in Update. Please fix this if you know a better solution. (See below)
	private static int currentScreenWidth;
	private static int currentScreenHeight;
	public Text myText;
	
	//Our way of interacting with our sections outside of this class.
	public Section S1 = new Section();
	public Section S2 = new Section();
	public Section S3 = new Section();
	
	Touch touch;
	
    void Start()
    {
			currentScreenWidth = Screen.width;
			currentScreenHeight = Screen.height;
			myText=GameObject.Find("DebugText").GetComponent<Text>();
    }

    void Update()
    {
		
		myText.text = "test";
		// UNITY PROGRAMMERS PLEASE HELP!!!
		//
		// (Samaras - 3/10/2021): If anyone knows a better way to check for screen resize events in Unity please fix this
		// Javascript has window.onResize as an event listener which is elegant
		// AFAIK Unity has no such elegant event listeners for screen resizing events...
		// Putting this code in Update can't be the best solution
		
		// See: https://forum.unity.com/threads/window-resize-event.40253/
		
			if (Screen.width != currentScreenWidth || Screen.height != currentScreenHeight)
			{
				currentScreenWidth = Screen.width;
				currentScreenHeight = Screen.height;
				
				S1.ScreenWidth = (int)Math.Round((double)((currentScreenWidth - ((4d/3d) * currentScreenHeight))/2));
				S1.ScreenHeight = currentScreenHeight;
				S2.ScreenWidth = (int)Math.Round((double)((4d/3d) * currentScreenHeight));
				S2.ScreenHeight = currentScreenHeight;
				S3.ScreenWidth = (int)Math.Round((double)((currentScreenWidth - ((4d/3d) * currentScreenHeight))/2));
				S3.ScreenHeight = currentScreenHeight;
				
			} // updates our current sections screen sizes when our device resizes screens (Say a phone or any other device)
			
			
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

			myText.text = "x: " + touch.position.x.ToString() + "y: " + touch.position.y.ToString();
			
            // Move the cube if the screen has the finger moving.
            if (touch.phase == TouchPhase.Moved)
            {            
                myText.text = "x: " + touch.position.x.ToString() + "y: " + touch.position.y.ToString();
            }

            if (Input.touchCount == 2)
            {
                touch = Input.GetTouch(1);

                if (touch.phase == TouchPhase.Began)
                {
                    // Halve the size of the cube.
                    //transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    // Restore the regular size of the cube.
                    //transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
        }		
		

    }
}
