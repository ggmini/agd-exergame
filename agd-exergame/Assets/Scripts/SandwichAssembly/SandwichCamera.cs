using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class SandwichCamera : MonoBehaviour
{
    [SerializeField] float animationDuration = 2f;
    public int xDistanceIncrement = 2;
    public float InitalWaitTime = 1.0f;

    private bool isAnimationRunning = false;


    // Update is called once per frame
    void Update()
    {
        if (isAnimationRunning) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            Vector3 moveDirection = new Vector3(xDistanceIncrement, 0, 0);
            StartCoroutine(MoveCamera(Vector3.right, xDistanceIncrement));
        }
    }

    public IEnumerator MoveCamera(Vector3 direction, int distance) {
        yield return new WaitForSeconds(InitalWaitTime);
        isAnimationRunning = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * distance;
        float elapsed = 0f;

        transform.position += direction;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        isAnimationRunning = false;
    }

}
