------------------------------------------------------------------
2D Sky 1.0.5
------------------------------------------------------------------

	This package contains sprites and scripts to help you quickly add 2D parallax sky into your scene. The background is always strength to fit on your screen and the large clouds is automatically tile on any screen size.

	Features:

		• Hand-painted sunny sky sprites.
		• Scripts to control cloud and resize background.
		• Unity 2D Sprite features.
		• Realtime strength background and auto-tile large cloud for any screen resolution.

		• Support all build player platforms.
		
	Compatible:

		• Unity 4.6.9 or higher.
		• Unity 5.3.2 or higher.

	Please direct any bugs/comments/suggestions to support e-mail (geteamdev@gmail.com).

	Thank you for your support.

	Gold Experience Team
	E-mail: geteamdev@gmail.com
	Website: http://ge-team.com

------------------------------------------------------------------
Use demo scene
------------------------------------------------------------------

	1. Open Demo in "2D Sky/Demo/Scenes/2D Sky Demo (960x600px)".
	2. In Hierarchy tab, look for NearCloud, MidCloud, FarCloud objects.
	3. Select any of them, you will see GE2DSky_CloudFlow component in Inspector tab. GE2DSky_CloudFlow component does update position of children objects.

			Parameters:

				Camera:			An orthographic camera that renders clouds and background
				Tile:				Enable tile for large cloud
				Behavior:	
					- Flow Mixed Left Right	:	Randomly left/right direction for children
					- Flow to Left:				Children objects move to left, they will repeat from right edge when they get off from screen.
					- Flow to Right:			Children objects move to right, they will repeat from left edge when they get off from screen.
				Min Speed:		Minimum speed of children
				Max Speed:		Maximum speed of children
				Speed Multiplier:	Current speed multiplier

	4. Select an object names Sunny_01_sky.
	5. Look for GE2DSky_SkyBG component in Inspector tab, this component does resize Sunny_01_sky sprite to strength  fit on screen.

			Parameters:

				Camera:			An orthographic camera that renders clouds and background

------------------------------------------------------------------
Use cloud on your scene
------------------------------------------------------------------
	
	1. Open Demo in "2D Sky/Demo/Scenes/2D Sky Demo (960x600px)".
	2. Look for Sky object, copy it.
	3. Open your scene then paste it into your scene.
	4. Press play, you shoud see Sky and its children active same as in the 2D Sky Demo (960x600px) scene.

------------------------------------------------------------------
Release notes
------------------------------------------------------------------

	Version 1.0.5 (Initial version)

		• Hand-painted sunny sky sprites.
		• Scripts to control cloud and resize background.
		• Unity 2D Sprite features.
		• Realtime strength background and auto-tile large cloud for any screen resolution.
		• Coud Speed multiplier.
		• Unity 4.6.9 and higher compatible.
		• Unity 5.3.2 and higher compatible.
