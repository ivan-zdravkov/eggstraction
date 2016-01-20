using UnityEngine;
using System.Collections;
using Assets.Classes;
using System;

public class CharacterController : MonoBehaviour {
    private Camera camera;
    private GameObject sky;

    private PositionTreshholds positionTreshholds;

    private const float moveSpeed = 0.05f;

	// Use this for initialization
	void Start () {
        this.camera = Camera.main;
        this.sky = GameObject.Find("Sky");

        positionTreshholds = new PositionTreshholds(25, 25);

        this.UpdatePositionThresholds();
    }

    // Update is called once per frame
    void Update () {
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

    private void MoveEnvironment(Vector3 moveVector)
    {
        camera.transform.Translate(moveVector);
        sky.transform.Translate(moveVector);

        this.UpdatePositionThresholds();
    }

    private void UpdatePositionThresholds()
    {
        float height = 2f * this.camera.orthographicSize;
        float width = height * this.camera.aspect;

        float upper = this.camera.transform.position.y + (height * positionTreshholds.VerticalTreshhold);
        float lower = this.camera.transform.position.y - (height * positionTreshholds.VerticalTreshhold);
        float left = this.camera.transform.position.x - (width * positionTreshholds.HorizontalTreshhold);
        float right = this.camera.transform.position.x + (width * positionTreshholds.HorizontalTreshhold);

        positionTreshholds.UpdateTreshholds(upper, lower, left, right);
    }
}
