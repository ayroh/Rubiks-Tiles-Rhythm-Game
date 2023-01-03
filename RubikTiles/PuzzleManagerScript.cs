using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;

public class PuzzleManagerScript : MonoBehaviour
{
    public class Arrow {

        public Arrow(GameObject FixedArrow, GameObject FallenArrow, CubeSwipeStartType StartType, CubeSwipe Swipe, float StartOffset) {
            fixedArrow = FixedArrow;
            fallenArrow = FallenArrow;
            startType = StartType;
            swipe = Swipe;
            startOffset = StartOffset;
        }

        public GameObject fixedArrow, fallenArrow;
        public CubeSwipeStartType startType;
        public CubeSwipe swipe;
        public float startOffset;
    }


    public enum CubeSwipeStartType { Right, Left, Middle, Top, Bottom};

    public enum CubeSwipe { Right, Left, Upward, Downward};

    public enum CubeType { LeftTop, Top, RightTop, Left, Middle, Right, LeftBottom, Bottom, RightBottom};


    [Header("Cube")]
    [SerializeField] private GameObject singleCube;
    [SerializeField] private GameObject cubeFace;
    [SerializeField] private Transform cubeParent;
    [SerializeField] private Transform cubeRotateParent;
    private bool isCubeHit = false;
    private bool isRotating = false;

    private Ease cubeSlideEase = Ease.InOutCubic;

    private float cubeSlideTime = .2f;
    private float offsetBetweenSingleCubes = 1.05f;
    private float minSwipeOffset = 100f;

    [Header("Song")]
    [SerializeField] private SongManagerScript songManager;
    private float tileDescendTime = 2f;


    // Cache
    private Vector2 startPos, mousePos;
    private RaycastHit hit;
    private CubeType hitCubeType;
    private Camera mainCam;

    void Awake() {
        cubeAtPosition = new Collider[1];
        arrows = new List<Arrow>();
        mainCam = Camera.main;
        isRotating = false;
    }

    private void Update() {
        if (isRotating)
            return;
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit)) {
                isCubeHit = true;
                startPos = Input.mousePosition;
                hitCubeType = CheckCubeType(hit.transform.gameObject);
            }
        }
        else if (Input.GetMouseButton(0)) {
            if (!isCubeHit || currentArrow == null) 
                return;
            mousePos = Input.mousePosition;
            float surplusX = mousePos.x - startPos.x;
            float surplusY = mousePos.y - startPos.y;
            if (surplusX > minSwipeOffset) {
                switch (hitCubeType) {
                    case CubeType.LeftTop:
                    case CubeType.Top:
                    case CubeType.RightTop:
                        if (currentArrow.startType == CubeSwipeStartType.Top && currentArrow.swipe == CubeSwipe.Right)
                            Rotate(CubeSwipeStartType.Top, CubeSwipe.Right);
                        break;
                    case CubeType.Left:
                    case CubeType.Middle:
                    case CubeType.Right:
                        if (currentArrow.startType == CubeSwipeStartType.Middle && currentArrow.swipe == CubeSwipe.Right)
                            Rotate(CubeSwipeStartType.Middle, CubeSwipe.Right);
                        break;
                    case CubeType.LeftBottom:
                    case CubeType.Bottom:
                    case CubeType.RightBottom:
                        if (currentArrow.startType == CubeSwipeStartType.Bottom && currentArrow.swipe == CubeSwipe.Right)
                            Rotate(CubeSwipeStartType.Bottom, CubeSwipe.Right);
                        break;
                }
            }
            else if (surplusX < -minSwipeOffset) {
                switch (hitCubeType) {
                    case CubeType.LeftTop:
                    case CubeType.Top:
                    case CubeType.RightTop:
                        if (currentArrow.startType == CubeSwipeStartType.Top && currentArrow.swipe == CubeSwipe.Left)
                            Rotate(CubeSwipeStartType.Top, CubeSwipe.Left);
                        break;
                    case CubeType.Left:
                    case CubeType.Middle:
                    case CubeType.Right:
                        if (currentArrow.startType == CubeSwipeStartType.Middle && currentArrow.swipe == CubeSwipe.Left)
                            Rotate(CubeSwipeStartType.Middle, CubeSwipe.Left);
                        break;
                    case CubeType.LeftBottom:
                    case CubeType.Bottom:
                    case CubeType.RightBottom:
                        if (currentArrow.startType == CubeSwipeStartType.Bottom && currentArrow.swipe == CubeSwipe.Left)
                            Rotate(CubeSwipeStartType.Bottom, CubeSwipe.Left);
                        break;
                }
            }
            else if (surplusY > minSwipeOffset) {
                switch (hitCubeType) {
                    case CubeType.LeftTop:
                    case CubeType.Left:
                    case CubeType.LeftBottom:
                        if (currentArrow.startType == CubeSwipeStartType.Left && currentArrow.swipe == CubeSwipe.Upward)
                            Rotate(CubeSwipeStartType.Left, CubeSwipe.Upward);
                        break;
                    case CubeType.Top:
                    case CubeType.Middle:
                    case CubeType.Bottom:
                        if (currentArrow.startType == CubeSwipeStartType.Middle && currentArrow.swipe == CubeSwipe.Upward)
                            Rotate(CubeSwipeStartType.Middle, CubeSwipe.Upward);
                        break;
                    case CubeType.RightTop:
                    case CubeType.Right:
                    case CubeType.RightBottom:
                        if (currentArrow.startType == CubeSwipeStartType.Right && currentArrow.swipe == CubeSwipe.Upward)
                            Rotate(CubeSwipeStartType.Right, CubeSwipe.Upward);
                        break;
                }
            }
            else if (surplusY < -minSwipeOffset) {
                switch (hitCubeType) {
                    case CubeType.LeftTop:
                    case CubeType.Left:
                    case CubeType.LeftBottom:
                        if (currentArrow.startType == CubeSwipeStartType.Left && currentArrow.swipe == CubeSwipe.Downward)
                            Rotate(CubeSwipeStartType.Left, CubeSwipe.Downward);
                        break;
                    case CubeType.Top:
                    case CubeType.Middle:
                    case CubeType.Bottom:
                        if (currentArrow.startType == CubeSwipeStartType.Middle && currentArrow.swipe == CubeSwipe.Downward)
                            Rotate(CubeSwipeStartType.Middle, CubeSwipe.Downward);
                        break;
                    case CubeType.RightTop:
                    case CubeType.Right:
                    case CubeType.RightBottom:
                        if (currentArrow.startType == CubeSwipeStartType.Right && currentArrow.swipe == CubeSwipe.Downward)
                            Rotate(CubeSwipeStartType.Right, CubeSwipe.Downward);
                        break;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) 
            isCubeHit = false;
        return;
    }

    #region Rotations

    private Collider[] cubeAtPosition;
    private bool shuffling = false;

    private void Rotate(CubeSwipeStartType StartType, CubeSwipe Swipe, bool isInstantiating = false) {
        if(!isInstantiating) {
            if (Mathf.Abs(currentArrow.fixedArrow.transform.localPosition.y - currentArrow.fallenArrow.transform.localPosition.y) > 1f)
                return;

            currentArrow.fallenArrow.transform.DOKill();
            StopCurrentArrow(currentArrowIndex);

            isCubeHit = false;
        }

        isRotating = true;
        GameObject[] cubesToRotate = new GameObject[9];
        GameObject obj;
        float x, y, z;
        DetachCubeRotateParent();
        cubeRotateParent.transform.position = Vector3.zero;
        cubeRotateParent.transform.rotation = Quaternion.identity;
        switch (Swipe) {
            case CubeSwipe.Right:
                switch (StartType) {
                    case CubeSwipeStartType.Top:
                        x = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(x, offsetBetweenSingleCubes, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                x += offsetBetweenSingleCubes;
                            }
                            x = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, -90f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Middle:
                        x = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(x, 0, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                x += offsetBetweenSingleCubes;
                            }
                            x = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, -90f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);

                        break;
                    case CubeSwipeStartType.Bottom:
                        x = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(x, -offsetBetweenSingleCubes, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                x += offsetBetweenSingleCubes;
                            }
                            x = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, -90f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                }
                break;
            case CubeSwipe.Left:
                switch (StartType) {
                    case CubeSwipeStartType.Top:
                        x = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(x, offsetBetweenSingleCubes, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                x += offsetBetweenSingleCubes;
                            }
                            x = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, 90f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Middle:
                        x = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(x, 0, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                x += offsetBetweenSingleCubes;
                            }
                            x = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, 90f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Bottom:
                        x = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(x, -offsetBetweenSingleCubes, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                x += offsetBetweenSingleCubes;
                            }
                            x = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, 90f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                }
                break;
            case CubeSwipe.Upward:
                switch (StartType) {
                    case CubeSwipeStartType.Left:
                        y = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(-offsetBetweenSingleCubes, y, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                y += offsetBetweenSingleCubes;
                            }
                            y = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(90f, 0f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Middle:
                        y = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(0, y, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                y += offsetBetweenSingleCubes;
                            }
                            y = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(90f, 0f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Right:
                        y = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(offsetBetweenSingleCubes, y, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                y += offsetBetweenSingleCubes;
                            }
                            y = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(90f, 0f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                }
                break;
            case CubeSwipe.Downward:
                switch (StartType) {
                    case CubeSwipeStartType.Left:
                        y = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(-offsetBetweenSingleCubes, y, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                y += offsetBetweenSingleCubes;
                            }
                            y = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90f, 0f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Middle:
                        y = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(0, y, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                y += offsetBetweenSingleCubes;
                            }
                            y = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90f, 0f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                    case CubeSwipeStartType.Right:
                        y = -offsetBetweenSingleCubes;
                        z = -offsetBetweenSingleCubes;
                        for (int i = 0;i < 3;++i) {
                            for (int j = 0;j < 3;++j) {
                                obj = CubeAt(new Vector3(offsetBetweenSingleCubes, y, z));
                                obj.transform.DOComplete();
                                obj.transform.SetParent(cubeRotateParent);
                                cubesToRotate[i * 3 + j] = obj;
                                y += offsetBetweenSingleCubes;
                            }
                            y = -offsetBetweenSingleCubes;
                            z += offsetBetweenSingleCubes;
                        }
                        cubeRotateParent.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90f, 0f, 0f)), cubeSlideTime).SetEase(cubeSlideEase).OnComplete(() => isRotating = false);
                        break;
                }
                break;
        }
    }

    private void DetachCubeRotateParent() {
        for (int i = cubeRotateParent.childCount - 1; i >= 0; --i)
            cubeRotateParent.GetChild(i).SetParent(cubeParent, true);
    }

    private IEnumerator ShuffleRubiksCube() {
        for(int i = 0;i < arrows.Count; ++i) {
            Rotate(arrows[i].startType, RotateSwipeType(arrows[i].swipe), true);
            yield return new WaitUntil(() => isRotating == false);
        }
        shuffling = false;
    }

    private CubeSwipe RotateSwipeType(CubeSwipe StartType) {
        switch (StartType) {
            case CubeSwipe.Right:
                return CubeSwipe.Left;
            case CubeSwipe.Left:
                return CubeSwipe.Right;
            case CubeSwipe.Upward:
                return CubeSwipe.Downward;
            case CubeSwipe.Downward:
                return CubeSwipe.Upward;
            default:
                Debug.LogError("Wrong RotateSwipeType(CubeSwipe) argument!");
                return CubeSwipe.Upward;
        }
    }

    #endregion

    #region Cube Defines

    private GameObject CubeAt(Vector3 Pos) {
        Physics.OverlapSphereNonAlloc(Pos, .1f, cubeAtPosition);
        return cubeAtPosition[0].gameObject;
    }


    private CubeType CheckCubeType(GameObject Cube) {
        Vector3 cubePos = Cube.transform.position;
        
        if (cubePos == new Vector3(+offsetBetweenSingleCubes, offsetBetweenSingleCubes, -offsetBetweenSingleCubes))
            return CubeType.RightTop;
        else if(cubePos == new Vector3(0, offsetBetweenSingleCubes, -offsetBetweenSingleCubes))
            return CubeType.Top;
        else if (cubePos == new Vector3(-offsetBetweenSingleCubes, offsetBetweenSingleCubes, -offsetBetweenSingleCubes))
            return CubeType.LeftTop;
        else if (cubePos == new Vector3(offsetBetweenSingleCubes, 0, -offsetBetweenSingleCubes))
            return CubeType.Right;
        else if (cubePos == new Vector3(0, 0, -offsetBetweenSingleCubes))
            return CubeType.Middle;
        else if (cubePos == new Vector3(-offsetBetweenSingleCubes, 0, -offsetBetweenSingleCubes))
            return CubeType.Left;
        else if (cubePos == new Vector3(+offsetBetweenSingleCubes, -offsetBetweenSingleCubes, -offsetBetweenSingleCubes))
            return CubeType.RightBottom;
        else if (cubePos == new Vector3(0, -offsetBetweenSingleCubes, -offsetBetweenSingleCubes))
            return CubeType.Bottom;
        else
            return CubeType.LeftBottom;
    }

    #endregion

    #region Arrow

    [Header("Arrow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowEndPoint;
    List<Arrow> arrows;

    private Arrow currentArrow;
    private int currentArrowIndex = 0;
    private float arrowZ = -1.56f;


    public void NextMove(int nextMove) {
        arrows[nextMove].fallenArrow.transform.DOMoveY(arrowEndPoint.position.y, tileDescendTime).SetEase(Ease.Linear).OnComplete(() => StopCurrentArrow(nextMove));
        StartCoroutine(xd(nextMove));
        arrows[nextMove].fallenArrow.gameObject.SetActive(true);
        arrows[nextMove].fixedArrow.gameObject.SetActive(true);
    }

    IEnumerator xd(int x) {
        yield return new WaitUntil(() => arrows[x].fallenArrow.transform.position.y <= arrows[x].fixedArrow.transform.position.y);
        if (arrows[x].fallenArrow.activeSelf)
            Rotate(arrows[x].startType, arrows[x].swipe);
        songManager.PrintElapsedTime();
    }

    private void CreateMove(float Time) {
        int rotationZ = Random.Range(0, 4) * 90;
        int positionXorY = Random.Range(-1, 2);
        CubeSwipeStartType startType = CubeSwipeStartType.Bottom;
        CubeSwipe swipe = CubeSwipe.Downward;
        switch (rotationZ) {
            case 0:
                swipe = CubeSwipe.Right;
                switch (positionXorY) {
                    case -1:
                        startType = CubeSwipeStartType.Bottom;
                        break;
                    case 0:
                        startType = CubeSwipeStartType.Middle;
                        break;
                    case 1:
                        startType = CubeSwipeStartType.Top;
                        break;
                }
                break;
            case 90:
                swipe = CubeSwipe.Upward;
                switch (positionXorY) {
                    case -1:
                        startType = CubeSwipeStartType.Left;
                        break;
                    case 0:
                        startType = CubeSwipeStartType.Middle;
                        break;
                    case 1:
                        startType = CubeSwipeStartType.Right;
                        break;
                }
                break;
            case 180:
                swipe = CubeSwipe.Left;
                switch (positionXorY) {
                    case -1:
                        startType = CubeSwipeStartType.Bottom;
                        break;
                    case 0:
                        startType = CubeSwipeStartType.Middle;
                        break;
                    case 1:
                        startType = CubeSwipeStartType.Top;
                        break;
                }
                break;
            case 270:
                swipe = CubeSwipe.Downward;
                switch (positionXorY) {
                    case -1:
                        startType = CubeSwipeStartType.Left;
                        break;
                    case 0:
                        startType = CubeSwipeStartType.Middle;
                        break;
                    case 1:
                        startType = CubeSwipeStartType.Right;
                        break;
                }
                break;
        }
        Quaternion rot = Quaternion.Euler(new Vector3(0f, 0f, rotationZ));
        if(rotationZ % 180 == 0) {
            Time -= ((mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, -mainCam.transform.position.z + arrowZ)).y - positionXorY) / (mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, -mainCam.transform.position.z + arrowZ)).y - arrowEndPoint.position.y)) * tileDescendTime;
            arrows.Add(new Arrow(Instantiate(arrowPrefab, new Vector3(0f, positionXorY, arrowZ), rot, cubeParent), Instantiate(arrowPrefab, mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, -mainCam.transform.position.z + arrowZ)), rot, cubeParent), startType, swipe, Time));
        }
        else {
            Vector3 arrowPos = mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, -mainCam.transform.position.z + arrowZ));
            arrowPos.x += positionXorY;
            Time -= (mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, -mainCam.transform.position.z + arrowZ)).y / (mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, -mainCam.transform.position.z + arrowZ)).y - arrowEndPoint.position.y)) * tileDescendTime;
            arrows.Add(new Arrow(Instantiate(arrowPrefab, new Vector3(positionXorY, 0, arrowZ), rot, cubeParent), Instantiate(arrowPrefab, arrowPos, rot, cubeParent), startType, swipe, Time));
        }
    }

    private void StopCurrentArrow(int Index) {
        arrows[Index].fixedArrow.gameObject.SetActive(false);
        arrows[Index].fallenArrow.gameObject.SetActive(false);
        if (arrows[Index] == currentArrow && arrows.Count != Index + 1) {
            currentArrow = arrows[Index + 1];
            currentArrowIndex = Index + 1;
        }
    }

    #endregion

    #region Song

    public List<float> SetSongTiles(List<float> SongTiles) {
        for(int i = 0;i < SongTiles.Count; ++i)
            CreateMove(SongTiles[i]);
        currentArrow = arrows[0];
        return arrows.Select(obj => obj.startOffset).ToList();
    }

    public void RecordGame() {
        songManager.RecordSong();
    }

    public void StartGame() {
        StartCoroutine(StartGameAfterShuffle());
    }

    private IEnumerator StartGameAfterShuffle() {
        songManager.GetSongTiles();
        arrows.Reverse();
        shuffling = true;
        StartCoroutine(ShuffleRubiksCube());
        yield return new WaitUntil(() => shuffling == false);
        arrows.Reverse();
        songManager.StartSong();
    }

    public void StopRecording() {
        songManager.StopRecording();
    }

    #endregion


    #region Button

    public void ResetSceneButton() {
        SceneManager.LoadScene(0);
    }

    #endregion
}
