using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraPostion : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;
    [SerializeField] private Transform player;
    [SerializeField] private float threshold;
    [SerializeField] private LayerMask _layerMaskGround;
    [SerializeField] private PlayerSO _playerSO;

    void Update()
    {
        if(player != null)
        {
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _layerMaskGround))
            {
                //Debug.Log(raycastHit.point);
            }

            Vector3 mousePos = raycastHit.point;
            _playerSO.aimPos = mousePos;
            Vector3 targetPos = (player.position + mousePos) / 2f;

            targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
            targetPos.z = Mathf.Clamp(targetPos.z, -threshold + player.position.z, threshold + player.position.z);

            this.transform.position = new Vector3(targetPos.x, 0, targetPos.z);
        }
    }
}

