using Car;
using UnityEngine;

public class SpeedPowerup : MonoBehaviour
{
    [SerializeField] private float speedPercent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out WheelsController car))
        {
            Vector3 toTarget = (car.transform.forward - transform.forward).normalized;
            if (Vector3.Dot(toTarget, transform.forward) < -.5f)
            {
                Debug.LogWarning("DEboost: " + -speedPercent);
                car.ApplySpeedPowerup(-speedPercent);
            }
            else
            {
                Debug.LogWarning("Boost: " + speedPercent);
                car.ApplySpeedPowerup(speedPercent);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out WheelsController wheelsController))
        {
            wheelsController.DeleteSpeedPowerup();
        }
    }
}