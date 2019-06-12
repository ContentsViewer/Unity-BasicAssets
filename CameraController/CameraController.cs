/*
*プログラム: CameraController
*   最終更新日:
*       12.5.2016
*
*   説明:
*       カメラを操作します
*       一人称視点,三人称視点切り替えができます
*       カメラ位置,カメラ方向は外部ゲームオブジェクトから指定できます
*       カメラ位置移動,カメラ視点移動のときに線形補間,曲線補間ができます
*
*   必須:
*       GameObject:
*           attacheObjectPlayer:
*               一人称視点の位置向きを知るのに使います
*               このゲームオブジェクトの位置が一人称視点の位置になります
*               このゲームオブジェクトのforwardベクトルの向きが一人称視点の方向になります
*
*           attacheObjectObserver:
*               三人称視点の位置向きを知るのに使います
*               このゲームオブジェクトの位置が三人称視点の位置になります
*               このゲームオブジェクトのforwardベクトルの向きが三人称視点の方向になります
*
*   更新履歴:
*       2.22.2016:
*           プログラムの完成
*
*       3.12.2016; 3.18.2016:
*           スクリプト修正
*
*       3.23.2016:
*           autoAvoidCollider機能追加
*
*       3.27.2016:
*           スクリプト修正
*
*       3.31.2016:
*           POSITION,DIRECTIONでのCUSTUMモードの仕様変更
*           DIRECTION内にSTATIONモードを追加
*           DIRECTION.GAZEモードを実装
*
*       4.12.2016; 4.17.2016; 5.15.2016:
*           スクリプト修正
*
*       7.7.2016:
*           AutoAvoidColliderが有効時, TargetObjectが設定されているならばそのObjectを用い他処理を優先するように変更
*
*       7.22.2016:
*           CameraのZ軸方向の回転に対応
*
*       8.2.2016:
*           スクリプト修正
*
*       8.3.2016:
*           クラス名変更
*           
*       12.5.2016:
*           フレームの違いによらず, 速度を一定にするようにした.
*
*/


using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour
{
    
    public enum CameraPosition
    {
        Player,
        Observer,
        Station,
        Custom
    }
    
    public enum CameraRotation
    {
        Player,
        Observer,
        Gaze,
        Station,
        Custom
    }
    
    public enum HomingModePosition
    {
        Direct,
        Lerp,
        Slerp,
        Stop
    }
    
    public enum HomingModeRotation
    {
        Direct,
        Lerp,
        Slerp,
        Stop
    }


    [System.Serializable]
    public class ControlSettings
    {
        public bool autoAvoidCollider = true;

        [Space(10)]
        public CameraPosition positionType = CameraPosition.Player;
        public CameraRotation rotationType = CameraRotation.Player;

        [Space(10)]
        public HomingModePosition homingModePosition = HomingModePosition.Direct;
        public HomingModeRotation homingModeRotation = HomingModeRotation.Direct;

        [Space(10)]
        public Vector3 homingPosition = new Vector3(2.0f, 2.0f, 2.0f);
        public float homingRotation = 20.0f;

        [Space(10)]
        public Transform station;
        public Transform target;
        public Transform gazeAt;

        [Space(10)]
        public Vector3 cameraRotation = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // === 外部パラメータ(Inspector表示) ======================================
    public Transform cameraPointPlayer;
    public Transform cameraPointObserver;

    [Space(10)]
    public LayerMask layerMaskCollider = ~0x00;
    public float distanceFromCollider = 0.1f;
    public float escapeFromTrap = 3.0f;

    [Space(10)]
    public ControlSettings controlSettings;



    Camera camera;
    

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 targetPosition = controlSettings.cameraPosition;
        Vector3 position = transform.position;

        Quaternion targetRotation = Quaternion.Euler(controlSettings.cameraRotation);
        Quaternion rotation = transform.rotation;


        //ターゲットの設定
        switch (controlSettings.positionType)
        {
            case CameraPosition.Player:
                if (cameraPointPlayer)
                {
                    targetPosition = cameraPointPlayer.position;
                }
                break;

            case CameraPosition.Observer:
                if (cameraPointObserver)
                {
                    targetPosition = cameraPointObserver.position;
                }
                break;

            case CameraPosition.Station:
                if (controlSettings.station)
                {
                    targetPosition = controlSettings.station.position;
                }
                break;

            case CameraPosition.Custom:
                targetPosition = controlSettings.cameraPosition;
                break;
        }

        //カメラの方向設定
        switch (controlSettings.rotationType)
        {
            case CameraRotation.Player:
                if (cameraPointPlayer)
                {
                    targetRotation = cameraPointPlayer.rotation;
                }
                break;

            case CameraRotation.Observer:
                if (cameraPointObserver)
                {
                    targetRotation = cameraPointObserver.rotation;
                }
                break;

            case CameraRotation.Gaze:
                if (controlSettings.gazeAt)
                {
                    targetRotation = Quaternion.FromToRotation(Vector3.forward, controlSettings.gazeAt.position - transform.position);
                }
                break;

            case CameraRotation.Station:
                if (controlSettings.station)
                {
                    targetRotation = controlSettings.station.rotation;
                }
                break;

            case CameraRotation.Custom:
                targetRotation = Quaternion.Euler(controlSettings.cameraRotation);
                break;
        }

        //コライダーを避ける
        if (controlSettings.autoAvoidCollider)
        {
            //TargetObjectとの間に壁があるときTargetが見えるようにカメラが移動する
            //TargetObjectからCamera移動後の位置にレーザーを飛ばしその間にコライダーがある場合そのコライダーの手前にカメラを移動する
            //レイヤーマスクに登録されているコライダーを判定の対象にする
            if (controlSettings.target)
            {
                RaycastHit hit;
                Transform targetTrfm = controlSettings.target.transform;
                if (Physics.Linecast(targetTrfm.position, targetPosition, out hit, layerMaskCollider))
                {
                    targetPosition = targetTrfm.position + ((targetPosition - targetTrfm.position) / (targetPosition - targetTrfm.position).magnitude) * (hit.distance - distanceFromCollider);
                }
            }
            else
            {
                //コライダーを抜けないようにする
                //レイヤーマスクに登録されているコライダーを判定の対象にする
                {
                    RaycastHit hit;
                    if (Physics.Linecast(transform.position, targetPosition, out hit, layerMaskCollider))
                    {
                        if (Vector3.Distance(targetPosition, transform.position) < escapeFromTrap)
                        {
                            targetPosition = transform.position + ((targetPosition - transform.position) / (targetPosition - transform.position).magnitude) * (hit.distance - distanceFromCollider);
                        }
                    }
                }

                //視野で壁が抜けないようにする
                {
                    Vector3[] fieldOfViewVecList = new Vector3[9];
                    float near = camera.nearClipPlane;
                    float fieldOfViewRadian = camera.fieldOfView * (3.14f / 180.0f);
                    Vector3 forwardVec = (transform.forward / transform.forward.magnitude) * near;
                    Vector3 upVec = (transform.up / transform.up.magnitude) * near * Mathf.Tan(fieldOfViewRadian / 2.0f);
                    Vector3 rightVec = (transform.right / transform.right.magnitude) * near * Mathf.Tan(fieldOfViewRadian / 2.0f) * camera.aspect;
                    fieldOfViewVecList[0] = forwardVec + upVec + rightVec;
                    fieldOfViewVecList[1] = forwardVec - upVec + rightVec;
                    fieldOfViewVecList[2] = forwardVec + upVec - rightVec;
                    fieldOfViewVecList[3] = forwardVec - upVec - rightVec;
                    fieldOfViewVecList[4] = forwardVec + upVec;
                    fieldOfViewVecList[5] = forwardVec - upVec;
                    fieldOfViewVecList[6] = forwardVec + rightVec;
                    fieldOfViewVecList[7] = forwardVec - rightVec;
                    fieldOfViewVecList[8] = forwardVec;
                    foreach (Vector3 vec in fieldOfViewVecList)
                    {
                        RaycastHit hit;

                        //移動後の位置を用いて計算する
                        //targetから見て前に壁があるかどうか判別
                        Debug.DrawLine(targetPosition, targetPosition + vec, Color.red);
                        if (Physics.Linecast(targetPosition, targetPosition + vec, out hit, layerMaskCollider))
                        {
                            targetPosition -= vec / vec.magnitude * (vec.magnitude - hit.distance);
                        }
                    }
                }
            }
        }

        //カメラ移動(位置): ホーミング
        switch (controlSettings.homingModePosition)
        {
            case HomingModePosition.Direct:
                position = targetPosition;
                break;

            case HomingModePosition.Lerp:
                position.x = Mathf.Lerp(position.x, targetPosition.x, controlSettings.homingPosition.x * Time.deltaTime);
                position.y = Mathf.Lerp(position.y, targetPosition.y, controlSettings.homingPosition.y * Time.deltaTime);
                position.z = Mathf.Lerp(position.z, targetPosition.z, controlSettings.homingPosition.z * Time.deltaTime);
                break;

            case HomingModePosition.Slerp:
                position.x = Mathf.SmoothStep(position.x, targetPosition.x, controlSettings.homingPosition.x * Time.deltaTime);
                position.y = Mathf.SmoothStep(position.y, targetPosition.y, controlSettings.homingPosition.y * Time.deltaTime);
                position.z = Mathf.SmoothStep(position.z, targetPosition.z, controlSettings.homingPosition.z * Time.deltaTime);
                break;

            case HomingModePosition.Stop:
                break;
        }



        //カメラ移動(回転): ホーミング
        switch (controlSettings.homingModeRotation)
        {
            case HomingModeRotation.Direct:
                rotation = targetRotation;
                break;

            case HomingModeRotation.Lerp:
                rotation = Quaternion.Lerp(transform.rotation, targetRotation, controlSettings.homingRotation * Time.deltaTime);
                break;

            case HomingModeRotation.Slerp:
                rotation = Quaternion.Slerp(transform.rotation, targetRotation, controlSettings.homingRotation * Time.deltaTime);
                break;

            case HomingModeRotation.Stop:
                break;
        }

        //カメラ位置向き更新
        transform.position = position;
        transform.rotation = rotation;
    }

    public void SetCamera(ControlSettings settings)
    {
        controlSettings = settings;
    }
}
