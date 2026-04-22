using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    Vector2 _movement;
    float _speed = 0.2f; // Cambiar segun las unidades de unity, cuidado¡¡¡¡!!!!
    float _rotSpeed = 10.0f;

    Transform _playerTransform;

    NetworkVariable<Color> miColor = new NetworkVariable<Color>(Color.grey);

    void Start()
    {
        _playerTransform = transform;

        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }

        miColor.OnValueChanged += OnChangeColorEvent;
        GetComponentInChildren<Renderer>().material.color = miColor.Value;
    }

    private void OnChangeColorEvent(Color previousValue, Color newValue)
    {
        GetComponentInChildren<Renderer>().material.color = miColor.Value;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        _playerTransform.Rotate(Vector3.up * (_movement.x * _rotSpeed)); // A y D para rotar
        _playerTransform.Translate(Vector3.forward * (_movement.y * _speed)); // S y W para moverse
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log("He recibido una acción");
        _movement = context.ReadValue<Vector2>(); // Se mira desde el mapa de acciones

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Cambio de color");
        OnChangeColorRpc();
        Debug.Log("Mi color es: " + miColor.Value);

    }

    [Rpc(SendTo.Server)]
    void OnChangeColorRpc()
    {
        Color newColor = new Color(Random.Range(0F, 1F), Random.Range(0F, 1F), Random.Range(0F, 1F));
        GetComponentInChildren<Renderer>().material.color = newColor;
        miColor.Value = newColor;
        //BackChangeColorRpc(newColor);
    }

    [Rpc(SendTo.Everyone)]
    void BackChangeColorRpc(Color newColor)
    {
        GetComponentInChildren<Renderer>().material.color = newColor;
    }
}
