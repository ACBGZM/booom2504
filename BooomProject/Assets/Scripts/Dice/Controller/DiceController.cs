using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;
public class DiceController : MonoBehaviour
{

    
    [SerializeField]
    float rollForce = 50f;
    [SerializeField]
    float torqueAmount = 20f;
    [SerializeField]
    float maxRollTime = 4f;
    [SerializeField]
    float minAnqularVelocity = 0.1f;
    [SerializeField]
    float smoothTime = 0.01f;
    [SerializeField]
    float maxSpeed = 15f;

    [Header("Audio & Particle Effects")]
    [SerializeField]
    AudioClip shakeClip;
    [SerializeField]
    AudioSource rollclip;
    [SerializeField]
    AudioClip impactClip;
    [SerializeField]
    AudioClip finalResultClip;
    [SerializeField]
    GameObject impactEffect;
    [SerializeField]
    GameObject finalResultEffect;
    [SerializeField]


    DiceSides diceSides;
    AudioSource audioSource;
    Rigidbody rb;

    CountdownTimer rollTimer;
    // ������ʼλ��
    [SerializeField]
    Vector3 originPosition;
    [SerializeField]
    // λ���ٶ�
    Vector3 currentVelocity;
    // �Ƿ�ع���ʼλ��
    public Vector3 pos;
    bool finalize;

    // ��������
    MeshCollider meshCollider;
    
    public int idx;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        diceSides = GetComponent<DiceSides>();
        meshCollider = GetComponent<MeshCollider>();
        originPosition = transform.localPosition;
        
        //  ������ʱ�������
        rollTimer = new CountdownTimer(maxRollTime);
        
        rollTimer.OnTimerStart += PerformInitialRoll;
        rollTimer.OnTimerStop += () => finalize = true;
        CalculateSides();
    }
    public void Show(int idx)
    {
        transform.rotation = diceSides.GetWorldRotationFor(idx);
    }
    private void Update()
    {
        pos = transform.position;
        // ���¼�����
        rollTimer.Tick(Time.deltaTime);
        // �����Ƿ�Ͷ������
        if (finalize)
        {

            MoveDiceToCenter();
        }
    }

    // ����ײ����Ե
    private void OnCollisionEnter(Collision collision)
    {
        // ����ֹͣ�趨
        if(rollTimer.IsRunning && rollTimer.Progress < 0.5f && rb.angularVelocity.magnitude < minAnqularVelocity)
        {
            finalize = true;
        }
        //audioSource.PlayOneShot(impactClip);
        // ײ������Ч��
        var particles = Instantiate(impactEffect, collision.contacts[0].point, Quaternion.identity);
        particles.transform.localScale = Vector3.one;
        particles.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);
        // 1�������
        Destroy(particles, 1f);
    }

    private void PerformInitialRoll()
    {
        ResetDiceState();
        // ���ʩ������
        //  Vector3 targetPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        //  Vector3 dir = (targetPos - transform.position).normalized;
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        rb.AddForce(dir * rollForce,ForceMode.Impulse);
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

        audioSource.loop = false;
        audioSource.Stop();
        //audioSource.PlayOneShot(finalResultClip);

        var particles = Instantiate(finalResultEffect, transform.position, Quaternion.identity);
        particles.transform.localScale = Vector3.one * 2.5f;
        
        particles.transform.rotation *= Quaternion.AngleAxis(90f, Vector3.up);
        Destroy(particles, 2f);

    }
    
    private void OnMouseUp()
    {
        if(rollTimer.IsRunning) return;
        rollTimer.Start();
    }

    public void OnShake()
    {
        if (rollTimer.IsRunning) return;
        rollTimer.Start();
    }
    // ��������״̬
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
        for(int i = 0; i < foundSides.Count; i++)
        {
            diceSides.sides[i].center = foundSides[i].center;
            diceSides.sides[i].normal = foundSides[i].normal;
        }

    }
    private List<DiceSide> FindDiceSides(Mesh mesh)
    {
        List<DiceSide> result = new List<DiceSide>();
        // ��ȡ�������������������
        int[] triangles = mesh.GetTriangles(0);
        // ��ȡ����Ķ�������
        Vector3[] vertices = mesh.vertices;
        // ��ȡ����ķ�������
        Vector3[] normals = mesh.normals;
        for(int i = 0; i < triangles.Length; i += 3)
        {
            // ����ÿ����������Ϣ
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];

            result.Add(new DiceSide
            {
                center = (a + b + c) / 3f,
                normal = Vector3.Cross(b - a,c - a).normalized,
            });
        }
        return result;
    }

    private bool InRnageof(Vector3 currentPos, Vector3 targetPos, float range)
    {
        return (targetPos - currentPos).sqrMagnitude <= range * range;
    }
}
