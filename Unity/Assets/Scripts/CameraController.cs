using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float nearCameraDis = -30;
    public float farCameraDis = -60;
    public float zoomSpeed = 10f;

    public float speed = 15f; // 이동 속도

    float currentDis;

    float vFOV;       // 수직
    float hFOV;      // 수평

    // 시야 범위
    float vOriginView;    // 수직 넓이
    float hOriginView;    // 수평 넓이

    float vCurrentView;
    float hCurrentView;

    bool initialized = false;

    float originHeight;

    Vector2 cameraOffset;
    Vector3 cameraPosition;


    void Start()
    {
        currentDis = farCameraDis;
        cameraOffset = transform.localPosition;
        FOVCalculate();
        CurrentViewCalculate();
    }

    void FOVCalculate()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("Main Camera가 없습니다!");
            return;
        }

        // 수직 FOV
        vFOV = cam.fieldOfView;

        // 화면 비율 (가로 / 세로)
        float aspect = cam.aspect;

        // 수평 FOV 계산
        hFOV = 2f * Mathf.Atan(Mathf.Tan(vFOV * Mathf.Deg2Rad / 2f) * aspect) * Mathf.Rad2Deg;

        OriginViewCalculate();
    }

    // 이동 범위 초기값 설정
    void OriginViewCalculate()
    {
        vOriginView = Mathf.Tan(Mathf.Deg2Rad * vFOV * 0.5f) * -currentDis;
        hOriginView = Mathf.Tan(Mathf.Deg2Rad * hFOV * 0.5f) * -currentDis;

        initialized = true;
    }

    // 이동 범위 초기값 설정
    void CurrentViewCalculate()
    {
        vCurrentView = Mathf.Tan(Mathf.Deg2Rad * vFOV * 0.5f) * -currentDis;
        hCurrentView = Mathf.Tan(Mathf.Deg2Rad * hFOV * 0.5f) * -currentDis;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;

        Zoom();
        Move();

        transform.localPosition = (Vector3)cameraOffset + cameraPosition;
    }

    void Zoom()
    {
        // 마우스 휠 입력
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // +면 앞으로, -면 뒤로

        if (scroll != 0f)
        {
            currentDis += scroll * zoomSpeed;

            // 거리 제한
            currentDis = Mathf.Clamp(currentDis, farCameraDis, nearCameraDis);
        }

        // front 방향으로 이동
        cameraPosition.z = currentDis;

        CurrentViewCalculate();     // 거리가 변할 때만 변경
    }

    void Move()
    {
        // Input Axis 값 가져오기
        float inputX = Input.GetAxis("Horizontal"); // 좌우
        float inputY = Input.GetAxis("Vertical");   // 상하

        // 카메라 이동 계산
        cameraPosition.x += inputX * speed * Time.deltaTime;
        cameraPosition.y += inputY * speed * Time.deltaTime;

        // 허용 범위 계산
        float halfHRange = (hOriginView - hCurrentView);
        float halfVRange = (vOriginView - vCurrentView);

        // 오프셋 제한
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, -halfHRange, halfHRange);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, -halfVRange, halfVRange);
    }
}
