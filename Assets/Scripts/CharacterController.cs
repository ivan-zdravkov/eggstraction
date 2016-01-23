using System;
using System;
using UnityEngine;
using System.Collections;
using Assets.Classes;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {
    private const string spritePath = "Sprites/Terrain/";
    private const float defaultWidth = 3840.0f;
    private const float defaultHeight = 2160.0f;
    private const float moveSpeed = 0.05f;
    
    private float cameraWidth;
    private float cameraHeight;

    private Vector3 scale;
    private Camera camera;
    private GameObject sky;
    private PositionTreshholds positionTreshholds;
    private LevelGenerator levelGenerator;

    private List<GameObject> instantiatedGameObjects;

    // Use this for initialization
    void Start () {
        try
        {
            this.camera = Camera.main;
            this.sky = GameObject.Find("Sky");

            this.cameraHeight = 2f * this.camera.orthographicSize;
            this.cameraWidth = cameraHeight * this.camera.aspect;

            this.scale = new Vector3(Screen.width / defaultWidth, Screen.height / defaultHeight, 1f);

            this.positionTreshholds = new PositionTreshholds(25, 25);

            this.levelGenerator = new LevelGenerator(100, 4, 8, 4, 8);
            this.levelGenerator.GenerateInnitialLevel(10);

            this.InstantiateLevel();

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
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Vector3 moveUpVector = new Vector3(0, moveSpeed);

                transform.Translate(moveUpVector);

                if (transform.position.y > positionTreshholds.UpperTreshhold)
                {
                    this.MoveEnvironment(moveUpVector);
                }
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                Vector3 moveDownVector = new Vector3(0, -moveSpeed);

                transform.Translate(moveDownVector);

                if (transform.position.y < positionTreshholds.LowerTreshhold)
                {
                    this.MoveEnvironment(moveDownVector);
                }
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 moveRightVector = new Vector3(moveSpeed, 0);

                transform.Translate(moveRightVector);

                if (transform.position.x > positionTreshholds.RightTreshhold)
                {
                    this.MoveEnvironment(moveRightVector);
                }
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 moveLeftVector = new Vector3(-moveSpeed, 0);

                transform.Translate(moveLeftVector);

                if (transform.position.x < positionTreshholds.LeftTreshhold)
                {
                    this.MoveEnvironment(moveLeftVector);
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            string message = ex.Message;

            throw ex;
        }
    }

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

    private void InstantiateLevel()
    {
        this.instantiatedGameObjects = new List<GameObject>();

        for (int rowNumber = 0; rowNumber < this.levelGenerator.Level.Length; rowNumber++)
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

                    platformToSpawn.AddComponent<SpriteRenderer>();
                    platformToSpawn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath + spriteName);

                    GameObject instantiatedGameObject = Instantiate(platformToSpawn, transform.position, transform.rotation) as GameObject;

                    this.instantiatedGameObjects.Add(instantiatedGameObject);
                }
            }
        }
    }
}
