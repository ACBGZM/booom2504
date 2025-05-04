using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceController : MonoBehaviour
{
    [SerializeField]
    private float rollForce = 50f;

    [SerializeField]
    private float torqueAmount = 20f;

    [SerializeField]
    private float maxRollTime = 4f;

    [SerializeField]
    private float minAnqularVelocity = 0.1f;

    [SerializeField]
    private float smoothTime = 0.01f;

    //[SerializeField]
    //private float maxSpeed = 15f;

    [Header("Audio & Particle Effects")]
    [SerializeField]
    private AudioClip shakeClip;

    [SerializeField]
    private AudioSource rollclip;

    [SerializeField]
    private AudioClip impactClip;

    [SerializeField]
    private AudioClip finalResultClip;

    [SerializeField]
    private GameObject impactEffect;

    [SerializeField]
    private GameObject finalResultEffect;

    [SerializeField]
    private DiceSides diceSides;

    private AudioSource audioSource;
    private Rigidbody rb;

    private CountdownTimer rollTimer;
    // 骰子起始位置
    [SerializeField]
    private Vector3 originPosition;

    [SerializeField]
    private
    // 位移速度
    Vector3 currentVelocity;

    // 是否回归起始位置
    public Vector3 pos;

    private bool finalize;
    // 骰子网格
    private MeshCollider meshCollider;

    // public int idx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        diceSides = GetComponent<DiceSides>();
        meshCollider = GetComponent<MeshCollider>();
        originPosition = transform.localPosition;

        //  最大持续时间计数器
        rollTimer = new CountdownTimer(maxRollTime);

        rollTimer.OnTimerStart += PerformInitialRoll;
        rollTimer.OnTimerStop += () => finalize = true;
       
        CalculateSides();
    }
    private void OnEnable()
    {
        EventHandlerManager.startRoll += OnShake;
    }
    private void OnDisable()
    {
        EventHandlerManager.startRoll -= OnShake;
    }

    public void Show(int idx)
    {
        transform.rotation = diceSides.GetWorldRotationFor(idx);
    }

    private void Update()
    {
        
        // 更新计数器
        rollTimer.Tick(Time.deltaTime);
        // 骰子是否投掷结束
        if (finalize)
        {
            MoveDiceToCenter();
        }
    }

    // 骰子撞击边缘
    private void OnCollisionEnter(Collision collision)
    {
        // 骰子停止设定
        if (rollTimer.IsRunning && rollTimer.Progress < 0.5f && rb.angularVelocity.magnitude < minAnqularVelocity)
        {
            finalize = true;
        }
        //audioSource.PlayOneShot(impactClip);
        // 撞击粒子效果
        var particles = Instantiate(impactEffect, collision.contacts[0].point, Quaternion.identity);
        particles.transform.localScale = Vector3.one * 0.2f;
        particles.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);
        // 1秒后销毁
        Destroy(particles, 0.4f);
    }

    private void PerformInitialRoll()
    {
        ResetDiceState();
        // 随机施力方向
        //  Vector3 targetPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        //  Vector3 dir = (targetPos - transform.position).normalized;

        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        print(gameObject.name);
        rb.AddForce(dir * rollForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * torqueAmount, ForceMode.Impulse);
        audioSource.clip = shakeClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void MoveDiceToCenter()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, originPosition, ref currentVelocity, smoothTime);

        if (InRnageof(transform.localPosition, originPosition, 1f))
        {
            FinalizeRoll();
        }
    }

    private void FinalizeRoll()
    {
        rollTimer.Stop();
        finalize = false;
        print(diceSides.GetMatch());
        ResetDiceState();

        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = null;

        EventHandlerManager.CallRollFinish(diceSides.GetMatch());
        //audioSource.PlayOneShot(finalResultClip);

        var particles = Instantiate(finalResultEffect, transform.position, Quaternion.identity);
        particles.transform.localScale = Vector3.one * 1.5f;

        particles.transform.rotation *= Quaternion.AngleAxis(90f, Vector3.up);
        Destroy(particles, 2f);
    }

    private void OnMouseUp()
    {
        if (rollTimer.IsRunning) return;
        rollTimer.Start();
    }

    public void OnShake()
    {
        if (rollTimer.IsRunning) return;
        rollTimer.Start();
    }

    // 重置骰子状态
    private void ResetDiceState()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = originPosition;
    }

    private void CalculateSides()
    {
        Mesh mesh = meshCollider.sharedMesh;
        List<DiceSide> foundSides = FindDiceSides(mesh);
        for (int i = 0; i < foundSides.Count; i++)
        {
            diceSides.sides[i].center = foundSides[i].center;
            diceSides.sides[i].normal = foundSides[i].normal;
        }
    }

    private List<DiceSide> FindDiceSides(Mesh mesh)
    {
        List<DiceSide> result = new List<DiceSide>();
        // 获取网格的三角形索引数组
        int[] triangles = mesh.GetTriangles(0);
        // 获取网格的顶点数组
        Vector3[] vertices = mesh.vertices;
        // 获取网格的法线数组
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // 处理每个三角形信息
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];

            result.Add(new DiceSide
            {
                center = (a + b + c) / 3f,
                normal = Vector3.Cross(b - a, c - a).normalized,
            });
        }
        return result;
    }

    private bool InRnageof(Vector3 currentPos, Vector3 targetPos, float range)
    {
        return (targetPos - currentPos).sqrMagnitude <= range * range;
    }

   
}
