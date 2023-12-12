using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AvatarBuilder;
using UnityEngine.EventSystems;
using System.Linq;

namespace AssetBuilder
{
    public class CamerControllerforConfigurator : MonoBehaviour
    {
        [Header("Main Camera")]
        public Camera mainCamera;

        [Header("Main Camera")]
        public Camera avatarScreenCamera;


        [Header("Movement Identifiers")]
        public Transform uperView;
        public Transform lowerView;
        public Transform socksView;
        public Transform shoesView;
        public Transform capView;
        public Transform glassesView;
        public List<Transform> jewelleryView = new List<Transform>();
        public List<Transform> tattooView = new List<Transform>();
        public Transform fullBodyView;
        public Transform screenCenterView;

        [Header("Camera Zoom Identifiers")]
        public float zoomSpeed;
        public float zoomLimit;
        public bool hasZoomed;
        public float maxZoomDistance;
        public float currentZoomDistance;

        [Header("Preview UI Btn for Zoom")]
        [Tooltip("Special Button for preview Screen, if user zoom the character this button will appear")]
        public Button previewBtn;

        [Header("Camera Panning Identifiers")]
        [SerializeField]
        public bool cameraDragging;
        public float dragSpeed;
        private Vector3 dragOrigin;
        public float minPaningHeight;
        public float maxPaningHeight;

        public static CamerControllerforConfigurator instance;
        public enum AvatarPosition
        {
            upperView,
            lowerView,
            socksView,
            shoesView,
            capView,
            glassesView,
            jewelleryView,
            tattooView,
            fullBodyView,
            screenCenterView
            
        }

        enum CameraStates
        {
            Zoomed,
            Normal
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            previewBtn.onClick.AddListener(() => ChangeCameraFromZoomStateToNormalState(CameraStates.Normal));
        }

        private void Update()
        {
            if (AssetsPreviewScreen.isAssetPreviewScreenEnabled && !EventSystem.current.IsPointerOverGameObject())
            {
                ZoomInOut();

                if (hasZoomed)
                    CameraPanning();
            }
        }

        private void CameraPanning()
        {
            //Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //float up = Screen.width * 0.2f;
            //float down = Screen.width - (Screen.width * 0.2f);

            //if (mousePosition.y < up)
            //{
            //    cameraDragging = true;
            //}
            //else if (mousePosition.y > down)
            //{
            //    cameraDragging = true;
            //}

            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(1)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(0, pos.y * dragSpeed, 0);

            if (move.y > 0f)
            {
                if (mainCamera.transform.position.y < maxPaningHeight)
                    mainCamera.transform.Translate(move, Space.World);
            }
            else
            {
                if (mainCamera.transform.position.y > minPaningHeight)
                    mainCamera.transform.Translate(move, Space.World);
            }
        }

        void ChangeCameraFromZoomStateToNormalState(CameraStates cameraState)
        {
            Debug.Log("cameraState: " + cameraState);
            switch (cameraState)
            {
                case CameraStates.Normal:
                    previewBtn.gameObject.SetActive(false);
                    AssetsScreenManager.instance.assetPreviewScreen.gameObject.SetActive(true);
                    hasZoomed = false;
                    break;
                case CameraStates.Zoomed:
                    previewBtn.gameObject.SetActive(true);
                    AssetsScreenManager.instance.assetPreviewScreen.gameObject.SetActive(false);
                    break;
            }
        }

        void ZoomInOut()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                RaycastHit hit;
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                hasZoomed = false;
                if (/*Zoom in*/ scroll > 0 && Physics.Raycast(ray, out hit))
                {
                    Vector3 desiredPosition = hit.point;
                    currentZoomDistance = Vector3.Distance(desiredPosition, mainCamera.transform.position);
                    if (currentZoomDistance > maxZoomDistance)
                    {
                        Vector3 direction = Vector3.Normalize(desiredPosition - mainCamera.transform.position) * (currentZoomDistance * zoomSpeed);
                        mainCamera.transform.position += direction;
                    }
                    //else
                    //{
                    //    hasZoomed = true;
                    //}
                }
                if (/*Zoom out*/ scroll < 0)
                {
                    float distance = Vector3.Distance(screenCenterView.transform.position, mainCamera.transform.position);
                    currentZoomDistance = Vector3.Distance(screenCenterView.transform.position, mainCamera.transform.position);
                    Vector3 direction = Vector3.Normalize(screenCenterView.transform.position - mainCamera.transform.position) * (distance * -zoomSpeed);
                    mainCamera.transform.position -= direction;
                }

                if (AssetsPreviewScreen.isAssetPreviewScreenEnabled)
                {
                    if (mainCamera.transform.position.z > -22.5f)
                    {
                        ChangeCameraFromZoomStateToNormalState(CameraStates.Zoomed);
                        if (mainCamera.transform.position.z > zoomLimit)
                        {
                            hasZoomed = true;
                        }
                        if (mainCamera.transform.position.z <= -21f)
                        {
                            ChangeCameraFromZoomStateToNormalState(CameraStates.Normal);
                        }
                    }

                }
            }
        }

        public void MoveCamera(AvatarPosition avatarPosition,string itemShortCode = null)
        {       
            Debug.Log("MoveCamera(AvatarPosition avatarPosition): " + avatarPosition.ToString());
            //AvatarPreviewScreen.isAvatarPreviewScreenEnabled = false;

            if (avatarPosition == AvatarPosition.upperView)
            {
                //AvatarParent_FbxHolder.instance.currentSelectedAvatar.GetComponent<Animator>().speed = 0.5f;
                MoveSmoothly(uperView);
            }
            else if (avatarPosition == AvatarPosition.lowerView)
            {
                MoveSmoothly(lowerView);
            }
            else if (avatarPosition == AvatarPosition.socksView)
            {
                MoveSmoothly(socksView);
            }
            else if (avatarPosition == AvatarPosition.shoesView)
            {
                MoveSmoothly(shoesView);
            }
            else if (avatarPosition == AvatarPosition.capView)
            {
                MoveSmoothly(capView);
            }
            else if (avatarPosition == AvatarPosition.glassesView)
            {
                MoveSmoothly(glassesView);
            }
            else if (avatarPosition == AvatarPosition.jewelleryView)
            {
                foreach (var item in jewelleryView)
                {
                    if (itemShortCode.Contains(item.name.ToLower()))
                    {   
                        MoveSmoothly(item);
                        break;                       
                    }
                }
            }
            else if (avatarPosition == AvatarPosition.tattooView)
            {
                foreach (var item in tattooView)
                {
                    if (itemShortCode.Contains(item.name.ToLower()))
                    {
                        MoveSmoothly(item);
                        break;
                    }
                }
            }
            else if (avatarPosition == AvatarPosition.screenCenterView)
            {
                Debug.Log("is isAssetPreviewScreenEnabled = "  + AssetsPreviewScreen.isAssetPreviewScreenEnabled);
                //AvatarParent_FbxHolder.instance.currentSelectedAvatar.GetComponent<Animator>().speed = 1f;
                MoveSmoothly(screenCenterView);
            }
            else
            {
                MoveSmoothly(fullBodyView);
                //AvatarParent_FbxHolder.instance.currentSelectedAvatar.GetComponent<Animator>().speed = 1f;
            }
        }

        void MoveSmoothly(Transform view)
        {
            LeanTween.move(mainCamera.gameObject, view.position, 0.75f);//.setEaseOutSine();
            LeanTween.rotate(mainCamera.gameObject, view.eulerAngles, 0.75f);//.setEaseOutSine();
        }
    }


}
