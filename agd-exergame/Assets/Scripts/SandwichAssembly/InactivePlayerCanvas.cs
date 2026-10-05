using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InactivePlayerCanvas : MonoBehaviour
{
    [SerializeField] private PlayerSignUp playerSignUp;
    [SerializeField] private Sprite PlaceHolderSprite;
    [SerializeField] private Sprite DisconnectSprite;
    [SerializeField] private GameObject[] Images;

    private bool gameStarted;

    void Start() {
        playerSignUp.GameStarted += OnGameStarted;
        playerSignUp.OnDeviceAssigned += OnDeviceAssigned;
        playerSignUp.OnDeviceDeassigned += OnDeviceDeassigned;

        foreach (GameObject obj in Images) {
            Image img = obj.GetComponent<Image>();
            img.sprite = PlaceHolderSprite;
        }
    }

    private void OnGameStarted() {
        gameStarted = true;
    }

    private void OnDeviceAssigned(int idx, string id) {
        Image obj = Images[idx].gameObject.GetComponent<Image>();
        obj.sprite = PlaceHolderSprite;
    }

    private void OnDeviceDeassigned(int idx) {
        Image obj = Images[idx].gameObject.GetComponent<Image>();
        if (gameStarted) {
            obj.sprite = DisconnectSprite;
        } else {
            obj.sprite = PlaceHolderSprite;
        }
    }
}
