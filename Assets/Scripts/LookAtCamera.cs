using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    
    private enum Mode {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    [SerializeField] private Mode mode;

    // запускается после Update
    private void LateUpdate() {
        switch (mode) {
            case Mode.LookAt:
                // раньше Camera.main не рекомендовали использовать из-за оптимизации, сейчас норм и поэтому можно
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                // направление от камеры к объекту
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                // смотрим в противоположную сторону от камеры
                transform.LookAt(transform.position + dirFromCamera);
                break;
            // то есть по высоте смотрит на камеру, а по горизонтали ужн прямо
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}
