using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public FixedJoystick joystick;
    public FixedTouchField fixedTouchField;

    public RigidbodyFirstPersonController rigidbodyController;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rigidbodyController = GetComponent<RigidbodyFirstPersonController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() 
    {
        if(joystick != null && fixedTouchField != null)
        {
        rigidbodyController.joystickInput.x = joystick.Horizontal;
        rigidbodyController.joystickInput.y = joystick.Vertical;
        rigidbodyController.mouseLook.lookInputaxis = fixedTouchField.TouchDist;
        animator.SetFloat("Horizontal", joystick.Horizontal);
        animator.SetFloat("Vertical", joystick.Vertical);


        if(Mathf.Abs(joystick.Horizontal) > 0.9f || Mathf.Abs(joystick.Vertical) > 0.9f)
        {
            animator.SetBool("IsRunning", true);
            rigidbodyController.movementSettings.ForwardSpeed = 16;
            rigidbodyController.movementSettings.BackwardSpeed = 8;
        }
        else
        {
            animator.SetBool("IsRunning", false);
            rigidbodyController.movementSettings.ForwardSpeed = 2;
            rigidbodyController.movementSettings.BackwardSpeed = 2;
        }
        }
    }
}
