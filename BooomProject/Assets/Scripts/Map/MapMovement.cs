using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UIElements;

public class MapMovement : MonoBehaviour
{

  //  InputActions inputActions;
    readonly float left = 20;
    readonly float right = Screen.width - 20;
    readonly float top = Screen.height - 20;
    readonly float bottom = 20;
    public Transform t1;
    public List<Transform> mapWorldCorners;
  
    public Vector2[] mapCurrentScreenCorners;
    public float lastMapPosX;
    public float lastMapPosY;
    public float lastScale;
    public bool draging;
    public Vector2 startPos;
    void Awake()
    {
        mapCurrentScreenCorners = new Vector2[mapWorldCorners.Count];
        print(mapWorldCorners[0].position);
        print(Camera.main.WorldToScreenPoint(mapWorldCorners[0].position));
        print(Camera.main.WorldToScreenPoint(new Vector3(-33, 16, 0)));
        //  inputActions = new InputActions();
        //inputActions.MapActions.MousePosition.performed += (ctx =>
        //{
        //    print(ctx.ReadValue<Vector2>());
        //});
        //   print(Camera.main.WorldToScreenPoint(t1.position));
    }

    //private void OnEnable()
    //{
    //    inputActions.Enable();
    //}
    //private void OnDisable()
    //{
    //    inputActions.Disable();
    //}
    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log(Mouse.current);
        //Debug.Log(Keyboard.current);
        //print(Mouse.current.position.ReadValue());
     //   print(mapCurrentScreenCorners[0].x);
        // ���µ�ͼ�߽�ӳ�䵽��Ļ
        Zoom();
        Movement();
        Drag();
        UpdateWorldCornersToScreen();
  
        Adjust();
    }
    
    void Movement()
    {
        var pos = Input.mousePosition;
        // ��¼�ƶ�ǰ��λ��
        lastMapPosX = Camera.main.transform.position.x;
        lastMapPosY = Camera.main.transform.position.y;
        // ������ƶ�
        if (pos.x <= left)
        {
            Camera.main.transform.Translate(Vector2.left * Settings.map_move_speed * Time.deltaTime);
        }
        if (pos.x >= right)
        {
            Camera.main.transform.Translate(Vector2.right * Settings.map_move_speed * Time.deltaTime);
        }
        
        if (pos.y >= top)
        {
            Camera.main.transform.Translate(Vector2.up * Settings.map_move_speed * Time.deltaTime);
        }
        if (pos.y <= bottom)
        {
            Camera.main.transform.Translate(Vector2.down * Settings.map_move_speed * Time.deltaTime);
        }
    }
    // ����
    void Zoom()
    {
        var delta = Input.mouseScrollDelta;
        lastScale = Camera.main.fieldOfView;
        float speed = Settings.map_scroll_speed;
        
        if (delta.y > 0 && Input.GetKey(KeyCode.LeftControl))
        {
            Camera.main.fieldOfView += speed;
        }
        if (delta.y < 0 && Input.GetKey(KeyCode.LeftControl))
        {
            Camera.main.fieldOfView -= speed;
        }
    }
    public void UpdateWorldCornersToScreen()
    {
        for(int i = 0; i < mapWorldCorners.Count; i++)
        {
            mapCurrentScreenCorners[i] = Camera.main.WorldToScreenPoint(mapWorldCorners[i].position);
        }
    }

    public void Adjust()
    {
        
        // ������Ҫ����
        if (mapCurrentScreenCorners[0].x > 0  || mapCurrentScreenCorners[2].x < Screen.width - 1)
        {
            // ��ԭ��С
            Camera.main.fieldOfView = lastScale;
            // ��ԭxλ��
            Camera.main.transform.position = new Vector3(lastMapPosX, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        // ������Ҫ����
        if (mapCurrentScreenCorners[1].y > 0 || mapCurrentScreenCorners[3].y < Screen.height - 1)
        {
            // ��ԭ��С
            Camera.main.fieldOfView = lastScale;
            // ��ԭyλ��
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, lastMapPosY, Camera.main.transform.position.z);
        }
    }
    //��ק
    public void Drag()
    {
        if(Input.GetMouseButtonDown(2))
        {
            draging = true;
            startPos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(2))
        {
            draging = false;
        }
        // ��ֹ��ק��������Ƴ���Ϸ�ɿ�
        if(draging && Input.GetMouseButton(2))
        {
            
            Vector2 currentPos = Input.mousePosition;
            Vector2 dir = -(currentPos - startPos).normalized;
            Camera.main.transform.Translate(dir * Settings.map_drag_speed * Time.deltaTime);
            startPos = Input.mousePosition;
        }
    }


  
}
