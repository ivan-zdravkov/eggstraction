﻿using System;
using UnityEngine;
using System.Collections;
using Assets.Classes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour {
    private const string terrainPath = "Sprites/Terrain/";
    private const string materialsPath = "Materials/";

    private const float defaultWidth = 3840.0f;
    private const float defaultHeight = 2160.0f;
    private const float moveSpeed = 5.0f; // More speed than 5.0 causes the character pass through solid walls. Unity Bug
    private const float jumpHeight = 500.0f;
    
    private float cameraWidth;
    private float cameraHeight;

    private int leftGameBorderPositionX;
    private int rightGameBorderPositionX;

    private int score = 0;

    private Vector3 scale;
    private Camera camera;
    private GameObject sky;
    private GameObject level;
    private GameObject scoreText;
    private PositionTreshholds positionTreshholds;
    private LevelGenerator levelGenerator;

    private List<GameObject> instantiatedGameObjects;

    private bool isGrounded = false;

    private float lastFrameCharacterHeight;

    // Use this for initialization
    void Start () {
        try
        {
            this.camera = Camera.main;
            this.sky = GameObject.Find("Sky");
            this.level = GameObject.Find("Level");
            this.scoreText = GameObject.Find("Score Text");

            this.cameraHeight = 2f * this.camera.orthographicSize;
            this.cameraWidth = cameraHeight * this.camera.aspect;

            this.scale = new Vector3(Screen.width / defaultWidth, Screen.height / defaultHeight, 1f);

            this.positionTreshholds = new PositionTreshholds(25, 25);

            this.levelGenerator = new LevelGenerator(10, 4, 8, 4, 8);
            this.levelGenerator.GenerateInnitialLevel(10);

            this.rightGameBorderPositionX = this.levelGenerator.MaximumRowLength / 2;
            this.leftGameBorderPositionX = -rightGameBorderPositionX;

            this.InstantiateLevel(0);

            this.UpdatePositionThresholds();
        }
        catch (Exception ex)
        {
            string message = ex.Message;

            throw ex;
        }
    }

    // Update is called once per frame
    void Update () {
        try
        {
            #region Score
            int intHeight = (int)(this.transform.position.y / 3.0f) + 1;
            score = score >= intHeight ? score : intHeight;

            if (this.transform.position.y < 0.0f)
            {
                this.scoreText.GetComponent<Text>().text = "Score: 0";
            }
            else
            {
                this.scoreText.GetComponent<Text>().text = "Score: " + score;
            }
            #endregion

            if (lastFrameCharacterHeight == this.transform.position.y)
            {
                this.isGrounded = true;
            }
            else
            {
                lastFrameCharacterHeight = this.transform.position.y;
            }

            #region CheckResize
            if (this.scale != new Vector3(defaultWidth / Screen.width, defaultHeight / Screen.height, 1f))
            {
                this.cameraHeight = 2f * this.camera.orthographicSize;
                this.cameraWidth = cameraHeight * this.camera.aspect;

                this.scale = new Vector3(defaultWidth / Screen.width, defaultHeight / Screen.height, 1f);

                this.UpdatePositionThresholds();

                //TODO Resize all sprites!
                //sprite.transform.scale = Vector3.Scale(sprite.transform.scale, scale);
                //sprite.transform.position = Vector3.Scale(sprite.transform.position, scale); //not sure that you need this
            }
            #endregion

            #region MovePlayer
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (this.isGrounded)
                {
                    this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpHeight);  
                     
                    this.isGrounded = false;
                }
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (transform.position.x < this.rightGameBorderPositionX) // Stop moving if we are the end of the screen
                {
                    Vector3 moveRightVector = new Vector3(moveSpeed * Time.deltaTime, 0);
                    transform.Translate(moveRightVector);

                    float cameraRight = this.camera.transform.position.x + (this.cameraWidth / 2);

                    bool areWeNearTheRightEndOfTheScreen = cameraRight >= this.rightGameBorderPositionX;

                    if (transform.position.x > positionTreshholds.RightTreshhold && !areWeNearTheRightEndOfTheScreen)
                    {
                        this.MoveEnvironment(moveRightVector);
                    }
                }
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) // Stop moving if we are the end of the screen
            {
                if (transform.position.x > this.leftGameBorderPositionX)
                {
                    Vector3 moveLeftVector = new Vector3(-moveSpeed * Time.deltaTime, 0);
                    transform.Translate(moveLeftVector);

                    float cameraLeft = this.camera.transform.position.x - (this.cameraWidth / 2);

                    bool areWeNearTheLeftEndOfTheScreen = cameraLeft <= this.leftGameBorderPositionX;

                    if (transform.position.x < positionTreshholds.LeftTreshhold && !areWeNearTheLeftEndOfTheScreen)
                    {
                        this.MoveEnvironment(moveLeftVector);
                    }
                }
            }
            #endregion

            if (transform.position.y > positionTreshholds.UpperTreshhold)
            {
                this.MoveEnvironment(Vector3.up * 0.2f);
            }

            float theYPositionOfTheTopMostElement = this.instantiatedGameObjects.Max(x => x.transform.position.y);

            //Pregenerate Elements, half a camera before needed.
            if (this.camera.transform.position.y + this.cameraHeight > theYPositionOfTheTopMostElement)
            {
                this.levelGenerator.GenerateNewRows(10);
                this.InstantiateLevel(this.levelGenerator.Level.Count() - 10);
            }

            //Quit the game if the bottom catchesup to the character
            float cameraBottom = this.camera.transform.position.y - (this.cameraHeight / 2);
            if (cameraBottom > transform.position.y)
            {
                Application.Quit();
            }

            //Move the environment upwards, gradually increasing the speed as the game continues
            this.MoveEnvironment(Vector3.up * (this.levelGenerator.Level.Count() / 25.0f * Time.deltaTime));
        }
        catch (Exception ex)
        {
            string message = ex.Message;

            throw ex;
        }
    }

    //void OnCollisionEnter(Collision other)
    //{
    //    this.isGrounded = true;
    //}

    private void MoveEnvironment(Vector3 moveVector)
    {
        camera.transform.Translate(moveVector); 
        sky.transform.Translate(moveVector);

        this.UpdatePositionThresholds();
    }

    private void UpdatePositionThresholds()
    {
        float upper = this.camera.transform.position.y + (cameraHeight * positionTreshholds.VerticalTreshhold);
        float lower = this.camera.transform.position.y - (cameraHeight * positionTreshholds.VerticalTreshhold);
        float left = this.camera.transform.position.x - (cameraWidth * positionTreshholds.HorizontalTreshhold);
        float right = this.camera.transform.position.x + (cameraWidth * positionTreshholds.HorizontalTreshhold);

        positionTreshholds.UpdateTreshholds(upper, lower, left, right);
    }

    private void InstantiateLevel(int rowNumberToInstantiateFrom)
    {
        this.instantiatedGameObjects = new List<GameObject>();

        for (int rowNumber = rowNumberToInstantiateFrom; rowNumber < this.levelGenerator.Level.Length; rowNumber++)
        {
            Platform[] row = this.levelGenerator.Level[rowNumber];

            for (var columnNumber = 0; columnNumber < row.Length; columnNumber++)
            {
                Platform platform = row[columnNumber];

                if (platform != null)
                {
                    string leftJoint = platform.LeftJoint.HasValue ? platform.LeftJoint.Value.ToString() : "Start";
                    string rightJoint = platform.RightJoint.HasValue ? platform.RightJoint.Value.ToString() : "End";
                    string spriteName = leftJoint + "-" + rightJoint;

                    GameObject platformToSpawn = new GameObject();

                    platformToSpawn.name = String.Format("Platform[{0}][{1}]", rowNumber, columnNumber);
                    platformToSpawn.transform.parent = this.level.transform;
                    platformToSpawn.transform.position = new Vector3(this.leftGameBorderPositionX + columnNumber, (rowNumber - 1) * 3, -5);

                    platformToSpawn.AddComponent<SpriteRenderer>();
                    platformToSpawn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(terrainPath + spriteName);

                    platformToSpawn.AddComponent<BoxCollider>();
                    platformToSpawn.GetComponent<BoxCollider>().material = Resources.Load<PhysicMaterial>(materialsPath + "Platform");

                    //GameObject instantiatedGameObject = Instantiate(platformToSpawn, transform.position, transform.rotation) as GameObject;

                    this.instantiatedGameObjects.Add(platformToSpawn);
                }
            }
        }
    }
}
