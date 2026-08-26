using UnityEngine;

public class FinalCuttingStation : SandwichAssemblyStation {
    public Rigidbody Knife;
    public float Speed = 10f;
    public GameObject FirstHalf;
    public GameObject SecondHalf;

    private Vector3 bottomRight;
    private Vector3 topLeft;
    private Vector3 diagonal;

    private float t = 0;
    private float t2 = 0;
    private float runningCounter = 0;
    private int CurrentExtreme = 0;
    private int DepthCounter = 0;

    [SerializeField] bool useKM;

    IPlayerInput playerInput;

    protected override void Start() {
        base.Start();
        topLeft = Knife.transform.position + new Vector3(-0.2f, 0, 0.2f);
        bottomRight = Knife.transform.position + new Vector3(0.2f, 0, -0.2f);
        diagonal = topLeft - bottomRight;
    }

    void OnEnable() {
        playerInput = gameObject.AddComponent<WebSocketInput>(); //TODO: globalish type thing?
    }

    void FixedUpdate() {
        t += playerInput.GetAccelX() * Speed * Time.fixedDeltaTime;
        t = Mathf.Clamp(t, 0, 1);

        runningCounter += (Mathf.Abs(t - t2));
        t2 = t;
        if ((t == 0 || t == 1) && (int)t != CurrentExtreme) {
            CurrentExtreme = (int)t;
            DepthCounter++;
            if (t == 0) {
                SeparateLayer((DepthCounter) / 2);
            }
            if (DepthCounter >= 7) {
                SplitSandwich();
                return;
            }

        }

        Vector3 targetPos = GetNextPoint(t);
        targetPos.y = Mathf.Max(targetPos.y, 0.33f);

        Knife.MovePosition(targetPos);
    }

    Vector3 GetNextPoint(float t) {
        //Debug.Log(DepthCounter + t);
        return bottomRight + diagonal * t + new Vector3(0, -0.03f, 0) * runningCounter;
    }


    private void SeparateLayer(int Layer) {
        int childIdx = 0;
        if (Layer == 1) childIdx = 1;
        else if (Layer == 2) childIdx = 3;
        else if (Layer == 3) childIdx = 2;
        else if (Layer == 4) childIdx = 0;

        FirstHalf.transform.GetChild(childIdx).gameObject.transform.position += new Vector3(-0.01f, 0, -0.01f);
        SecondHalf.transform.GetChild(childIdx).gameObject.transform.position += new Vector3(0.01f, 0, 0.01f);
    }

    private void SplitSandwich() {
        OnStationCleared?.Invoke(this);
    }


}
