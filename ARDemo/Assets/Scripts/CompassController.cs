using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    private float northAngle = 0f;

    private void Start()
    {
        Input.compass.enabled = true; //cihaz pusulasını aktif eder.
    }

    private void FixedUpdate()
    {
        Input.location.Start(); //cihaz konumunu kullanmaya başlar.
        northAngle = Input.compass.trueHeading; //kuzey kutbunun verisini tutar.
        transform.localEulerAngles = new Vector3(0f, 0f, northAngle); // pusulayı kuzeye doğru döndürür.
    }
}
