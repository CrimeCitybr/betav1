using UnityEngine;
using JUTPS.ActionScripts;
using JUTPS.VehicleSystem;
using UnityEngine.Events;

[RequireComponent(typeof(RCC_CarControllerV4))]
public class VehicleControlActivator : MonoBehaviour
{
    [Header("Raiz do veículo (ex: M3_E46_New)")]
    public GameObject thisVehicleRoot;

    private RCC_CarControllerV4 rccController;
    private DriveVehicles driver;

    private void Awake()
    {
        rccController = GetComponent<RCC_CarControllerV4>();
        if (rccController == null)
        {
            Debug.LogError("❌ RCC_CarControllerV4 não encontrado no objeto: " + gameObject.name);
            return;
        }

        DisableVehicleControl();
    }

    private void OnEnable()
    {
        driver = FindObjectOfType<DriveVehicles>();
        if (driver != null)
        {
            driver.OnEnterVehicle.AddListener(OnEnterVehicle);
            driver.OnExitVehicle.AddListener(OnExitVehicle);
        }
    }

    private void OnDisable()
    {
        if (driver != null)
        {
            driver.OnEnterVehicle.RemoveListener(OnEnterVehicle);
            driver.OnExitVehicle.RemoveListener(OnExitVehicle);
        }
    }

    private void OnEnterVehicle()
    {
        if (driver.CurrentVehicle == thisVehicleRoot)
        {
            EnableVehicleControl();
        }
    }

    private void OnExitVehicle()
    {
        if (driver.CurrentVehicle == thisVehicleRoot)
        {
            DisableVehicleControl();
        }
    }

    public void EnableVehicleControl()
    {
        if (rccController != null && !rccController.enabled)
        {
            rccController.enabled = true;
            Debug.Log("✅ RCC ativado para: " + gameObject.name);
        }
    }

    public void DisableVehicleControl()
    {
        if (rccController != null && rccController.enabled)
        {
            rccController.enabled = false;
            Debug.Log("⛔ RCC desativado para: " + gameObject.name);
        }
    }
}
