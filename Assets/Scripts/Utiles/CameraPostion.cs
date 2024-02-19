using Cinemachine;
using UnityEngine;
using Zenject;

public class CameraPostion : MonoBehaviour
{
    private PlayerController _playerController;

    [SerializeField] private Camera _mainCam;
    [SerializeField] private float threshold;
    [SerializeField] private LayerMask _layerMaskGround;
    [SerializeField] private PlayerSO _playerSO;

    [Inject]
    public void Construct(PlayerController playerController)
    {
        _playerController = playerController;
        Debug.Log("CameraPosition installed");
    }

    void Update()
    {
        if(_playerController != null)
        {
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _layerMaskGround);
            Vector3 mousePos = raycastHit.point;
            _playerSO.aimPos = mousePos;
            Vector3 targetPos = (_playerController.transform.position + mousePos) / 2f;

            targetPos.x = Mathf.Clamp(targetPos.x, -threshold + _playerController.transform.position.x, threshold + _playerController.transform.position.x);
            targetPos.z = Mathf.Clamp(targetPos.z, -threshold + _playerController.transform.position.z, threshold + _playerController.transform.position.z);

            this.transform.position = new Vector3(targetPos.x, 0, targetPos.z);
        }
    }
}

