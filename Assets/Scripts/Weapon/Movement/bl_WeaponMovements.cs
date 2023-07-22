using UnityEngine;
using System.Collections;

public class bl_WeaponMovements : bl_MonoBehaviour
{
    private bl_FirstPersonController controller;
    [Space(5)]
    [Header("Weapon On Run Position")]
    [Tooltip("Weapon Position and Position On Run")]
    public Vector3 moveTo;
    [Tooltip("Weapon Rotation and Position On Run")]
    public Vector3 rotateTo;
    [Space(5)]
    [Header("Weapon On Run and Reload Position")]
    [Tooltip("Weapon Position and Position On Run and Reload")]
    public Vector3 moveToReload;
    [Tooltip("Weapon Rotation and Position On Run and Reload")]
    public Vector3 rotateToReload;
    [Space(5)]
    public float InSpeed = 15;
    public float OutSpeed = 12;

    //private
    private Transform myTransform;
    private float vel;
    private Quaternion DefaultRot;
    private Vector3 DefaultPos;
    private bl_Gun Gun;

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        this.myTransform = this.transform;
        DefaultRot = myTransform.localRotation;
        DefaultPos = myTransform.localPosition;
        Gun = transform.parent.GetComponent<bl_Gun>();
        controller = Gun.PlayerReferences.firstPersonController;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        if (controller == null)
            return;

        vel = controller.VelocityMagnitude;
        RotateControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void RotateControl()
    {
        float delta = Time.smoothDeltaTime;
        if ((vel > 1f && controller.isGrounded) && controller.State == PlayerState.Running && !Gun.isFiring && !Gun.isAiming)
        {
            if (Gun.isReloading)
            {
                myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, Quaternion.Euler(rotateToReload), delta * InSpeed);
                myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, moveToReload, delta * InSpeed);
            }
            else
            {
                myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, Quaternion.Euler(rotateTo), delta * InSpeed);
                myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, moveTo, delta * InSpeed);
            }
        }
        else
        {
            myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, DefaultRot, delta * OutSpeed);
            myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, DefaultPos, delta * OutSpeed);
        }
    }
}