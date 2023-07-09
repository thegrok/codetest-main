using UnityEngine;
using UnityEngine.UI;

public class IndicatorArrow : MonoBehaviour
{

    void Update()
    {
        var player = GameObject.Find("Sphere");
        var gameRunner = GameObject.Find("GameRunner").GetComponent<GameRunner>();

        if (gameRunner.isOnAssignment)
        {
            GetComponent<Image>().enabled = true;

            var goVector = gameRunner.targetDestination.transform.position - player.transform.position;
            goVector.y = 0f;
            float angle = Vector3.SignedAngle(Vector3.forward, goVector, Vector3.down);
            angle += Camera.main.gameObject.transform.rotation.eulerAngles.y;
            GetComponent<RectTransform>().rotation = Quaternion.Euler(0f,0f, angle);
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
