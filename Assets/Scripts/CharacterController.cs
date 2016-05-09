using System;
using UnityEngine;
using System.Collections;
using Assets.Classes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour {
    private const string characterPath = "Sprites/Character/";
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

    private float cameraMoveUpSpeedModifier = 50.0f;
    private int score = 0;

    private Vector3 scale;
    private Camera camera;
    private GameObject sky;
    private GameObject level;
    private GameObject scoreText;
    private PositionTreshholds positionTreshholds;
    private LevelGenerator levelGenerator;

    private List<GameObject> instantiatedGameObjects;

    private bool isLeftFacing = true;
    private bool isGrounded = false;
    private bool isFalling = true;

    private int shouldWalkCounter = 1;
    private int walkingState = 1;
    private int landingCounter = 1;

    private float lastFrameCharacterHeight;

    // Use this for initialization
    void Start () {
        try
        {
            this.camera = Camera.main;
            this.sky = GameObject.Find("Sky");
            this.level = GameObject.Find("Level");
            this.scoreText = GameObject.Find("Score Text");

            this.scoreText.transform.position = new Vector3(this.camera.transform.position.x - (this.cameraHeight / 2) + 125, this.camera.transform.position.y - (this.cameraWidth / 2) + 50, 0);

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

            if (lastFrameCharacterHeight == this.transform.position.y) // If character is staying horizontally, allow him to jump.
            {
                if (landingCounter == 3)
                {
                    this.Land(true);

                    landingCounter = 1;
                }
                else
                {
                    landingCounter++;
                }

                this.isFalling = false;
            }
            else
            {
                landingCounter = 1;

                if (lastFrameCharacterHeight > this.transform.position.y)
                {
                    this.isFalling = true;

                    this.Fall();
                }
                else
                {
                    this.isFalling = false;
                }

                this.GetComponent<Rigidbody>().velocity += new Vector3(0, -0.1f, 0);

                lastFrameCharacterHeight = this.transform.position.y;
            }

            #region CheckResize
            if (this.scale != new Vector3(defaultWidth / Screen.width, defaultHeight / Screen.height, 1f))
            {
                this.cameraHeight = 2f * this.camera.orthographicSize;
                this.cameraWidth = cameraHeight * this.camera.aspect;

                this.scale = new Vector3(defaultWidth / Screen.width, defaultHeight / Screen.height, 1f);

                this.UpdatePositionThresholds();
            }
            #endregion

            #region MovePlayer
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (this.isGrounded)
                {
                    this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpHeight);

                    this.Jump();
                }
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (transform.position.x < this.rightGameBorderPositionX) // Stop moving if we are the end of the screen
                {
                    if (this.isLeftFacing)
                    {
                        this.isLeftFacing = false;

                        transform.localRotation = Quaternion.Euler(0, 180, 0);
                    }

                    WalkAnimation();

                    Vector3 moveRightVector = new Vector3(moveSpeed * Time.deltaTime, 0);
                    transform.Translate(-moveRightVector); // Wibbly Woobly directional change fenomenal

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
                    if (!this.isLeftFacing)
                    {
                        this.isLeftFacing = true;
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }

                    WalkAnimation();

                    Vector3 moveLeftVector = new Vector3(moveSpeed * Time.deltaTime, 0);
                    transform.Translate(-moveLeftVector); // Wibbly Woobly directional change fenomenal

                    float cameraLeft = this.camera.transform.position.x - (this.cameraWidth / 2);

                    bool areWeNearTheLeftEndOfTheScreen = cameraLeft <= this.leftGameBorderPositionX;

                    if (transform.position.x < positionTreshholds.LeftTreshhold && !areWeNearTheLeftEndOfTheScreen)
                    {
                        this.MoveEnvironment(-moveLeftVector);
                    }
                }
            }
            #endregion

            if (transform.position.y > positionTreshholds.UpperTreshhold)
            {
                this.MoveEnvironment(Vector3.up * Time.deltaTime * 3); 
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
            this.MoveEnvironment(Vector3.up * (this.levelGenerator.Level.Count() / this.cameraMoveUpSpeedModifier * Time.deltaTime));
        }
        catch (Exception ex)
        {
            string message = ex.Message;

            throw ex;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        /*Vector3 contactPoint = collision.contacts[0].point;
        Vector3 center = collision.collider.bounds.center;

        bool right = contactPoint.x > center.x && collision.collider.bounds.extents.x - Math.Abs(contactPoint.x - center.x) > -0.01f;
        bool top = contactPoint.y > center.y && collision.collider.bounds.extents.y - Math.Abs(contactPoint.y - center.y) > -0.01f;
        bool left = contactPoint.x < center.x && collision.collider.bounds.extents.x - Math.Abs(contactPoint.x - center.x)  > -0.01f;
        bool under = contactPoint.y < center.y && collision.collider.bounds.extents.y - Math.Abs(contactPoint.y - center.y) > -0.01f;

        if (top)
        {
            this.Land(true);
        } */

        Collider collider = collision.collider;
        bool collideFromLeft = false;
        bool collideFromTop = false;
        bool collideFromRight = false;
        bool collideFromBottom = false;

        float RectWidth = this.GetComponent<Collider>().bounds.size.x;
        float RectHeight = this.GetComponent<Collider>().bounds.size.y;
        float circleRad = collider.bounds.size.x;

        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 center = collider.bounds.center;

        if (contactPoint.y > center.y && //checks that circle is on top of rectangle
            (contactPoint.x < center.x + RectWidth / 2 && contactPoint.x > center.x - RectWidth / 2))
        {
            collideFromTop = true;
        }
        else if (contactPoint.y < center.y &&
            (contactPoint.x < center.x + RectWidth / 2 && contactPoint.x > center.x - RectWidth / 2))
        {
            collideFromBottom = true;
        }
        else if (contactPoint.x > center.x &&
            (contactPoint.y < center.y + RectHeight / 2 && contactPoint.y > center.y - RectHeight / 2))
        {
            collideFromRight = true;
        }
        else if (contactPoint.x < center.x &&
            (contactPoint.y < center.y + RectHeight / 2 && contactPoint.y > center.y - RectHeight / 2))
        {
            collideFromLeft = true;
        }

        if (collideFromTop)
        {
            this.Land(true);
        }
    }

    private void Jump()
    {
        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(characterPath + "Character Jump");

        this.isGrounded = false;
    }

    private void Fall()
    {
        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(characterPath + "Character Fall");
    }

    private void Land(bool groundCharacter)
    {
        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(characterPath + "Character " + this.walkingState);

        if (groundCharacter)
        {
            this.isGrounded = true;
        }
    }

    private void WalkAnimation()
    {
        if (this.isGrounded && !this.isFalling)
        {
            if (this.shouldWalkCounter == 5)
            {
                if (this.walkingState < 4)
                {
                    this.walkingState++;
                }
                else
                {
                    this.walkingState = 1;
                }

                this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(characterPath + "Character " + this.walkingState);

                this.shouldWalkCounter = 1;
            }
            else
            {
                this.shouldWalkCounter++;
            } 
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
                    platformToSpawn.AddComponent<BoxCollider>();

                    SpriteRenderer spriteRenderer = platformToSpawn.GetComponent<SpriteRenderer>();
                    BoxCollider boxCollider = platformToSpawn.GetComponent<BoxCollider>();

                    spriteRenderer.sprite = Resources.Load<Sprite>(terrainPath + spriteName);
                    boxCollider.material = Resources.Load<PhysicMaterial>(materialsPath + "Platform");

                    this.instantiatedGameObjects.Add(platformToSpawn);
                }
            }
        }
    }
}
